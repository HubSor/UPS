using Dtos;
using Models.Entities;

namespace UsersMicro.Messages;

public record LoginOrder(string Username, string Password);
public record LogoutOrder();
public record SessionOrder();
public record AddUserOrder(string Username, string Password, ICollection<RoleEnum> RoleIds);
public record ListUsersOrder(PaginationDto Pagination);
public record EditUserOrder(int Id, string Username, string? Password, ICollection<RoleEnum> RoleIds);
public record DeleteUserOrder(int Id);