using Core;
using MassTransit;
using Messages.Products;
using Models.Dtos;

namespace Consumers.Products;
public class ViewProductConsumer : IConsumer<ViewProductsOrder>
{
    private UPSContext products;
    public ViewProductConsumer(UPSContext products)
    {
        this.products = products;
    }

    public async Task Consume(ConsumeContext<ViewProductsOrder> context)
    {
        var dtos = products.Products.Select(x => new ProductDto() { Name = x.Name, CreatedAt = x.CreatedAt });
        await context.RespondAsync(new ViewProductsResponse() { Products = dtos });
    }
}
