namespace Messages.Clients;

public abstract record FindClientOrder(string Identifier);
public record FindPersonClientOrder(string Identifier) : FindClientOrder(Identifier);
public record FindCompanyClientOrder(string Identifier) : FindClientOrder(Identifier);