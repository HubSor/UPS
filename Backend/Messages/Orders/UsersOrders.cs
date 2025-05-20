using Models.Entities;

namespace Messages.Orders;

public record AddUserOrder(string Username, string Password, ICollection<RoleEnum> RoleIds);
public record EditUserOrder(int Id, string Username, string? Password, ICollection<RoleEnum> RoleIds);
public record DeleteUserOrder(int Id);