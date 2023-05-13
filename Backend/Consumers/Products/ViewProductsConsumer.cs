using Core;
using MassTransit;
using Messages.Products;
using Models.Dtos;
using Models.Entities;

namespace Consumers.Products;
public class ViewProductConsumer : IConsumer<ViewProductsOrder>
{
    private readonly IRepository<Product> products;
    public ViewProductConsumer(IRepository<Product> products)
    {
        this.products = products;
    }

    public Task Consume(ConsumeContext<ViewProductsOrder> context)
    {
        var dtos = products.GetAll().Select(x => new ProductDto() { Id = x.Id, Name = x.Name });
        context.Respond(new ViewProductsResponse() { Products = dtos });
        return Task.CompletedTask;
    }
}
