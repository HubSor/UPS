using Core.Dtos;
using Core.Models;

namespace Core.Messages;

public record LoginOrder(string Username, string Password) : BaseOrder();
public record LogoutOrder() : BaseOrder();
public record SessionOrder() : BaseOrder();
public record AddUserOrder(string Username, string Password, ICollection<RoleEnum> RoleIds) : BaseOrder();
public record ListUsersOrder(PaginationDto Pagination) : BaseOrder();
public record EditUserOrder(int Id, string Username, string? Password, ICollection<RoleEnum> RoleIds) : BaseOrder();
public record DeleteUserOrder(int Id) : BaseOrder();