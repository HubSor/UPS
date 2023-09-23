namespace Messages.Users;

public record LoginOrder(string Username, string Password);
public record LogoutOrder();