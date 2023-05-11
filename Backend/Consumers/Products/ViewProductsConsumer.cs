using Core;
using MassTransit;
using Messages.Products;
using Models.Dtos;
using Models.Entities;

namespace Consumers.Products;
public class ViewProductConsumer : IConsumer<ViewProductsOrder>
{
    private IRepository<Product> products;
    public ViewProductConsumer(IRepository<Product> products)
    {
        this.products = products;
    }

    public async Task Consume(ConsumeContext<ViewProductsOrder> context)
    {
        var dtos = products.GetAll().Select(x => new ProductDto() { Name = x.Name, CreatedAt = x.CreatedAt });
        await context.RespondAsync(new ViewProductsResponse() { Products = dtos });
    }
}
