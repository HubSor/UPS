using Core;
using Messages.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Entities;
using Helpers;
using Services.Domain;
using Data;

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

    public Task<EditUserResponse> EditUserAsync(EditUserOrder order)
    {
        throw new NotImplementedException();
    }

    public Task<ListUsersResponse> ListUsersAsync(ListUsersOrder order)
    {
        throw new NotImplementedException();
    }

    public Task<LoginResponse> LoginAsync(LoginOrder order)
    {
        throw new NotImplementedException();
    }

    public Task<LogoutResponse> LogoutAsync(LogoutOrder order)
    {
        throw new NotImplementedException();
    }
}