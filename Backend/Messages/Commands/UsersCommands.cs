using Models.Entities;

namespace Messages.Commands;

public record AddUserOrder(string Username, string Password, ICollection<RoleEnum> RoleIds) : Command;
public record EditUserOrder(int Id, string Username, string? Password, ICollection<RoleEnum> RoleIds) : Command;
public record DeleteUserOrder(int Id) : Command;