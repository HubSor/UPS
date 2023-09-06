using Core;
using MassTransit;
using Messages.Users;
using Models.Entities;

namespace Consumers.Products;
public class LoginConsumer : BaseConsumer<LoginOrder, LoginResponse>
{
    private IRepository<User> users;
    public LoginConsumer(IRepository<User> users)
    {
        this.users = users;
    }

    public override async Task Consume(ConsumeContext<LoginOrder> context)
    {
        await ValidationFailAsync(context, "test", "test");
    }
}
