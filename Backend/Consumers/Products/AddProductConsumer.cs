using Core;
using MassTransit;
using Messages.Products;
using Models.Entities;

namespace Consumers.Products;
public class AddProductConsumer : IConsumer<AddProductOrder>
{
    private IRepository<Product> products;
    public AddProductConsumer(IRepository<Product> products)
    {
        this.products = products;
    }

    public async Task Consume(ConsumeContext<AddProductOrder> context)
    {
        await products.AddAsync(new Product() { Name = context.Message.Name, CreatedAt = DateTime.Now });
        await context.RespondAsync(new AddProductResponse());
    }
}
