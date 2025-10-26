using Core;
using Messages.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Entities;
using Helpers;
using Services.Domain;
using Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Dtos;
using Dtos.Users;

namespace Services.Application;

public interface IUsersApplicationService : IBaseApplicationService
{
    Task<LoginResponse> LoginAsync(LoginOrder order);
    Task<LogoutResponse> LogoutAsync(LogoutOrder order);
    Task<AddUserResponse> AddUserAsync(AddUserOrder order);
    Task<ListUsersResponse> ListUsersAsync(ListUsersOrder order);
    Task<EditUserResponse> EditUserAsync(EditUserOrder order);
    Task<DeleteUserResponse> DeleteUserAsync(DeleteUserOrder order);
}

public class UsersApplicationService(
    ILogger<UsersApplicationService> logger,
    IUnitOfWork unitOfWork,
    IRepository<User> usersRepository,
    IRepository<Role> roleRepository,
    IHttpContextAccessor httpContextAccessor,
    IPasswordService passwordService
) : BaseApplicationService(logger, unitOfWork), IUsersApplicationService
{
    public async Task<AddUserResponse> AddUserAsync(AddUserOrder order)
    {
        if (await usersRepository.GetAll().AnyAsync(x => x.Name == order.Username))
        {
            ThrowValidationException("Username", "Istnieje już użytkownik o takiej nazwie");
        }
        
        var salt = passwordService.GenerateSalt();
		var hash = passwordService.GenerateHash(order.Password, salt);
		var newRoles = await roleRepository.GetAll().Where(r => order.RoleIds.Contains(r.Id)).ToListAsync();
		
		var newUser = new User()
		{
			Active = true,
			Name = order.Username,
			Hash = hash,
			Salt = salt,
			Roles = newRoles,
		};

        await usersRepository.AddAsync(newUser);
        logger.LogInformation("Added new user {UserId}", newUser.Id);

        return new AddUserResponse();
    }

    public async Task<DeleteUserResponse> DeleteUserAsync(DeleteUserOrder order)
    {
        var user = await usersRepository.GetAll()
			.Include(u => u.Roles)
			.FirstOrDefaultAsync(x => x.Id == order.Id);
			
		if (user == null)
		{
			ThrowValidationException("Id", "Nie znaleziono użytkownika");
		}
		
		if (user!.Id == httpContextAccessor.GetUserId())
		{
			ThrowValidationException("Id", "Nie można usunąć samego siebie");
		}

        if (user.Roles.Any(r => r.Id == RoleEnum.Administrator) && !httpContextAccessor.HasAnyRole(RoleEnum.Administrator))
        {
            ThrowValidationException("Id", "Nie można usunąć administratora");
        }

        await usersRepository.DeleteAsync(user);
        logger.LogInformation("Deleted user {UserId}", user?.Id);

        return new DeleteUserResponse();
    }

    public async Task<EditUserResponse> EditUserAsync(EditUserOrder order)
    {
        if (!await usersRepository.GetAll().AnyAsync(x => x.Id == order.Id))
		{
			ThrowValidationException("Username", "Nie znaleziono użytkownika");
		}
		
		var isAdmin = httpContextAccessor.HasAnyRole(RoleEnum.Administrator);
		if (!isAdmin && httpContextAccessor.GetUserId() == order.Id)
		{
			ThrowValidationException("Username", "Tylko administrator może edytować swoje konto");
		}
		
		if (isAdmin && !order.RoleIds.Contains(RoleEnum.Administrator) && httpContextAccessor.GetUserId() == order.Id)
		{
			ThrowValidationException("RoleIds", "Administrator nie może odebrać sobie uprawnień administratora");
		}

        if (await usersRepository.GetAll().AnyAsync(u => u.Name == order.Username && u.Id != order.Id))
        {
            ThrowValidationException("Username", "Istnieje inny użytkownik o takiej nazwie");
        }
        
        var newRoles = await roleRepository.GetAll().Where(r => order.RoleIds.Contains(r.Id)).ToListAsync();
		var user = await usersRepository.GetAll()
			.Include(x => x.Roles)
			.FirstOrDefaultAsync(u => u.Id == order.Id) ??
			throw new UPSException("No user");
			
		logger.LogInformation("Editing user {UserId}", user.Id);
		
		if (user.Roles.Any(r => r.Id == RoleEnum.Administrator) && !httpContextAccessor.HasAnyRole(RoleEnum.Administrator))
		{
			ThrowValidationException("Id", "Nie można edytować administratora");
		}
			
		user.Name = order.Username;
		user.Roles = newRoles;
		if (!string.IsNullOrEmpty(order.Password))
		{
			logger.LogInformation("Changing password for edited user");
			var salt = passwordService.GenerateSalt();
			var hash = passwordService.GenerateHash(order.Password, salt);
			user.Hash = hash;
			user.Salt = salt;
		}
		
		await usersRepository.UpdateAsync(user);

        if (user.Id == httpContextAccessor.GetUserId())
        {
            logger.LogInformation("Updating claims after self edit");
            var claimsIdentity = new ClaimsIdentity(user.GetClaims(), CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(claimsIdentity);
            await httpContextAccessor.HttpContext!.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }

        return new EditUserResponse();
    }

    public async Task<ListUsersResponse> ListUsersAsync(ListUsersOrder order)
    {
        var userCount = usersRepository.GetAll().Count();
		var userList = await usersRepository.GetAll()
			.OrderBy(x => x.Id)
			.Include(u => u.Roles)
			.Skip(order.Pagination.PageIndex * order.Pagination.PageSize)
			.Take(order.Pagination.PageSize)
			.Select(u => new UserDto()
			{
				Id = u.Id,
				Username = u.Name,
				Roles = u.Roles.Select(r => r.Id.ToString()).ToList()
			})
			.ToListAsync();
			
		logger.LogInformation("Listing users");
		return new ListUsersResponse()
		{
			Users = new PagedList<UserDto>(
                userList,
                userCount,
                order.Pagination.PageIndex,
                order.Pagination.PageSize
            )
		};
    }

    public async Task<LoginResponse> LoginAsync(LoginOrder order)
    {
        logger.LogInformation("Login initiated");
		var user = await usersRepository.GetAll().Include(x => x.Roles).FirstOrDefaultAsync(u => u.Name == order.Username);
		
		if (user == null || !user.Active)
		{
			logger.LogInformation("Faking login");
			passwordService.FakeGenerateHash();
			ThrowValidationException(nameof(LoginOrder.Password), "Niepoprawne hasło");
		}
		
		var newHash = passwordService.GenerateHash(order.Password, user!.Salt);
		if (newHash.SequenceEqual(user.Hash))
		{
			var claims = user.GetClaims();
			var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
			var principal = new ClaimsPrincipal(claimsIdentity);
			await httpContextAccessor.HttpContext!.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
			return new LoginResponse() 
			{
				UserDto = new UserDto()
				{
					Id = user.Id,
					Username = user.Name,
					Roles = user.Roles.Select(r => r.Id.ToString()).ToList()
				}
			};			
		}
		
		if (!user.Active)
		{
			ThrowValidationException(nameof(LoginOrder.Username), "Konto nieaktywne");
		}

        ThrowValidationException(nameof(LoginOrder.Password), "Niepoprawne hasło");

        return new LoginResponse();
    }

    public async Task<LogoutResponse> LogoutAsync(LogoutOrder order)
    {
        if (httpContextAccessor.HttpContext == null)
			throw new UPSException("No HttpContext on logout");
			
		await httpContextAccessor.HttpContext!.SignOutAsync();
		logger.LogInformation("Logged out");
		return new LogoutResponse();
    }
}