using Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Entities;
using Data;
using Messages.Products;
using Dtos.Products;
using Dtos;

namespace Services.Application;

public interface IProductsApplicationService : IBaseApplicationService
{
    Task<GetProductResponse> GetProductAsync(GetProductOrder order);
    Task<GetSubProductResponse> GetSubProductAsync(GetSubProductOrder order);
    Task<AddProductResponse> AddProductAsync(AddProductOrder order);
    Task<EditProductResponse> EditProductAsync(EditProductOrder order);
    Task<DeleteProductResponse> DeleteProductAsync(DeleteProductOrder order);
    Task<ListProductsResponse> ListProductAsync(ListProductsOrder order);
    Task<AddSubProductResponse> AddSubProductAsync(AddSubProductOrder order);
    Task<EditSubProductResponse> EditSubProductAsync(EditSubProductOrder order);
    Task<DeleteSubProductResponse> DeleteSubProductAsync(DeleteSubProductOrder order);
    Task<AssignSubProductResponse> AssignSubProductAsync(AssignSubProductOrder order);
    Task<UnassignSubProductsResponse> UnassignSubProductsAsync(UnassignSubProductsOrder order);
    Task<ListSubProductsResponse> ListSubProductsAsync(ListSubProductsOrder order);
    Task<EditSubProductAssignmentResponse> EditSubProductAssignmentAsync(EditSubProductAssignmentOrder order);
}

public class ProductsApplicationService(
    ILogger<ProductsApplicationService> logger,
    IUnitOfWork _unitOfWork,
    IRepository<Product> products,
    IRepository<SubProduct> subProducts,
    IRepository<SubProductInProduct> subProductsInProducts,
    IRepository<Parameter> parameters
#pragma warning disable CS9107 // Parameter is captured into the state of the enclosing type and its value is also passed to the base constructor. The value might be captured by the base class as well.
) : BaseApplicationService(logger, _unitOfWork), IProductsApplicationService
#pragma warning restore CS9107 // Parameter is captured into the state of the enclosing type and its value is also passed to the base constructor. The value might be captured by the base class as well.
{
    public async Task<AddProductResponse> AddProductAsync(AddProductOrder order)
	{
		if (await products.GetAll().AnyAsync(x => x.Code == order.Code.ToUpper()))
		{
			ThrowValidationException("Code", "Istnieje już produkt o takim kodzie");
		}
		
		var product = new Product()
		{
			Name = order.Name,
			Code = order.Code.ToUpper(),
			Description = order.Description,
			BasePrice = order.BasePrice,
			AnonymousSaleAllowed = order.AnonymousSaleAllowed,
			Status = ProductStatusEnum.NotOffered,
			TaxRate = order.TaxRate != 0 ? order.TaxRate / 100m : 0.00m
		};
		
		await products.AddAsync(product);
		logger.LogInformation("Added product {ProductId}", product.Id);

		return new AddProductResponse();
    }

    public async Task<AddSubProductResponse> AddSubProductAsync(AddSubProductOrder order)
    {
		if (await subProducts.GetAll().AnyAsync(x => x.Code == order.Code.ToUpper()))
		{
			ThrowValidationException("Code", "Istnieje już podprodukt o takim kodzie");
		}
		
		var subProduct = new SubProduct()
		{
			Name = order.Name,
			Code = order.Code.ToUpper(),
			Description = order.Description,
			BasePrice = order.BasePrice,
			TaxRate = order.TaxRate != 0 ? order.TaxRate / 100m : 0.00m
		};

		await subProducts.AddAsync(subProduct);
		logger.LogInformation("Added subproduct {SubProductId}", subProduct.Id);

		if (order.ProductId.HasValue)
		{
			logger.LogInformation("Assigning new subproduct to product {ProductId}", order.ProductId);
			var assignSubProductOrder = new AssignSubProductOrder(order.ProductId.Value, subProduct.Id, subProduct.BasePrice);
			try
			{
				var client = await AssignSubProductAsync(assignSubProductOrder);
				logger.LogInformation("Successfully assigned new subproduct");
				return new AddSubProductResponse();
			}
			catch (Exception ex)
			{
				ThrowValidationException("ProductId", "Powiązanie z produktem nie powiodło się");
				logger.LogError(ex, "Error while assigning subproduct {SubProductId} to product {ProductId}", subProduct.Id, order.ProductId);
			}
		}

		return new AddSubProductResponse();
    }

    public async Task<AssignSubProductResponse> AssignSubProductAsync(AssignSubProductOrder order)
    {
        if (!await subProducts.GetAll().AnyAsync(x => x.Id == order.SubProductId && !x.Deleted))
		{
			ThrowValidationException("SubProductId", "Nie znaleziono podproduktu");
		}
		
		if (!await products.GetAll().AnyAsync(x => x.Id == order.ProductId && !x.Deleted))
		{
			ThrowValidationException("ProductId", "Nie znaleziono produktu");
		}

		if (await subProductsInProducts.GetAll().AnyAsync(x => x.SubProductId == order.SubProductId && x.ProductId == order.ProductId))
		{
			ThrowValidationException("ProductId", "Podprodukt jest już przypisany do tego produktu");
		}
		
		var subProductInProduct = new SubProductInProduct()
		{
			SubProductId = order.SubProductId,
			ProductId = order.ProductId,
			InProductPrice = order.Price,
		};
		
		await subProductsInProducts.AddAsync(subProductInProduct);
		logger.LogInformation("Assigned subproduct {SubProductId} to product {ProductId}", subProductInProduct.SubProductId, order.ProductId);

		return new AssignSubProductResponse();
    }

    public async Task<DeleteProductResponse> DeleteProductAsync(DeleteProductOrder order)
    {
		if (!await products.GetAll().AnyAsync(x => x.Id == order.ProductId))
		{
			ThrowValidationException("ProductId", "Nie znaleziono produktu");
		}
		
		var product = await products.GetAll()
			.Include(p => p.SubProductInProducts)
			.Include(p => p.Sales)
			.Include(p => p.Parameters)
			.ThenInclude(p => p.SaleParameters)
			.FirstAsync(p => p.Id == order.ProductId);

		logger.LogInformation("Deleting product {ProductId}", product.Id);

		foreach (var assignment in product.SubProductInProducts.ToList())
		{
			logger.LogInformation("Deleting assignments for deleted product");
			await subProductsInProducts.DeleteAsync(assignment);
			product.SubProductInProducts.Remove(assignment);
		}

		foreach (var param in product.Parameters.ToList())
		{
			logger.LogInformation("Deleting parameters for deleted product");
			if (param.SaleParameters.Any())
			{
				param.Deleted = true;
				await parameters.UpdateAsync(param);
				logger.LogInformation("Soft deleted parameter {ParameterId}", param.Id);
			}
			else
			{
				await parameters.DeleteAsync(param);
				product.Parameters.Remove(param);
				logger.LogInformation("Hard deleted parameter {ParameterId}", param.Id);
			}
		}

		if (product.Sales.Any() || product.Parameters.Any(p => p.SaleParameters.Any()))
		{
			product.Deleted = true;
			await products.UpdateAsync(product);
			logger.LogInformation("Soft deleted product {ProductId}", product.Id);
		}
		else
		{
			await products.DeleteAsync(product);
			logger.LogInformation("Hard deleted product {ProductId}", product.Id);
		}

		await _unitOfWork.FlushAsync();
		return new DeleteProductResponse();
    }

    public async Task<DeleteSubProductResponse> DeleteSubProductAsync(DeleteSubProductOrder order)
    {
		if (!await subProducts.GetAll().AnyAsync(x => x.Id == order.SubProductId))
		{
			ThrowValidationException("SubProductId", "Nie znaleziono podproduktu");
		}
		
		var subProduct = await subProducts.GetAll()
			.Include(p => p.SubProductInProducts)
			.Include(p => p.SubProductInSales)
			.Include(p => p.Parameters)
			.ThenInclude(p => p.SaleParameters)
			.FirstAsync(p => p.Id == order.SubProductId);

		logger.LogInformation("Deleting subproduct {SubProductId}", subProduct.Id);

		foreach (var assignment in subProduct.SubProductInProducts.ToList())
		{
			logger.LogInformation("Deleting assignments for deleted subproduct");
			await subProductsInProducts.DeleteAsync(assignment);
			subProduct.SubProductInProducts.Remove(assignment);
		}
			
		foreach(var param in subProduct.Parameters.ToList())
		{
			logger.LogInformation("Deleting parameters for deleted subproduct");
			if (param.SaleParameters.Any())
			{
				param.Deleted = true;
				await parameters.UpdateAsync(param);
				logger.LogInformation("Soft deleted parameter {ParameterId}", param.Id);
			}
			else
			{
				await parameters.DeleteAsync(param);
				subProduct.Parameters.Remove(param);
				logger.LogInformation("Hard deleted parameter {ParameterId}", param.Id);
			}
		}
		
		if (subProduct.SubProductInSales.Any() || subProduct.Parameters.Any(p => p.SaleParameters.Any()))
		{
			subProduct.Deleted = true;
			await subProducts.UpdateAsync(subProduct);
			logger.LogInformation("Soft deleted subproduct {SubProductId}", subProduct.Id);
		}
		else 
		{
			await subProducts.DeleteAsync(subProduct);
			logger.LogInformation("Hard deleted subproduct {SubProductId}", subProduct.Id);
		}

		await _unitOfWork.FlushAsync();
		return new DeleteSubProductResponse();
    }

    public async Task<EditProductResponse> EditProductAsync(EditProductOrder order)
    {
        if (await products.GetAll().AnyAsync(x => x.Code == order.Code.ToUpper() && x.Id != order.Id))
		{
			ThrowValidationException("Code", "Istnieje już inny produkt o takim kodzie");
		}

		if (!await products.GetAll().AnyAsync(x => x.Id == order.Id && !x.Deleted))
		{
			ThrowValidationException("Id", "Nie znaleziono produktu");
		}
		
		var product = await products.GetAll().FirstAsync(p => p.Id == order.Id);
		
		product.Description = order.Description;
		product.Name = order.Name;
		product.Code = order.Code;
		product.BasePrice = order.BasePrice;
		product.AnonymousSaleAllowed = order.AnonymousSaleAllowed;
		product.Status = order.Status;
		product.TaxRate = order.TaxRate != 0 ? order.TaxRate / 100m : 0.00m;

		await products.UpdateAsync(product);
		logger.LogInformation("Edited product {ProductId}", product.Id);

		return new EditProductResponse();
    }

    public async Task<EditSubProductAssignmentResponse> EditSubProductAssignmentAsync(EditSubProductAssignmentOrder order)
    {
        var editedEntity = await subProductsInProducts.GetAll()
			.FirstOrDefaultAsync(x => x.SubProductId == order.SubProductId && x.ProductId == order.ProductId);
		if (editedEntity == null)
		{
			ThrowValidationException("ProductId", "Nie znaleziono przypisania tego podproduktu do tego produktu");
		}
		else
        {
			editedEntity.InProductPrice = order.NewPrice;
			await subProductsInProducts.UpdateAsync(editedEntity);
			logger.LogInformation("Edited subproduct assignment for product {ProductId} and subproduct {SubProductId}", editedEntity.ProductId, editedEntity.SubProductId);
        }

		return new EditSubProductAssignmentResponse();
    }

    public async Task<EditSubProductResponse> EditSubProductAsync(EditSubProductOrder order)
    {
        if (await subProducts.GetAll().AnyAsync(x => x.Code == order.Code.ToUpper() && x.Id != order.Id))
		{
			ThrowValidationException("Code", "Istnieje już inny podprodukt o takim kodzie");
		}

		if (!await subProducts.GetAll().AnyAsync(x => x.Id == order.Id && !x.Deleted))
		{
			ThrowValidationException("Id", "Nie znaleziono podproduktu");
		}
		
		var subProduct = await subProducts.GetAll().FirstAsync(p => p.Id == order.Id);
		
		subProduct.Description = order.Description;
		subProduct.Name = order.Name;
		subProduct.Code = order.Code;
		subProduct.BasePrice = order.BasePrice;
		subProduct.TaxRate = order.TaxRate != 0 ? order.TaxRate / 100m : 0.00m;

		await subProducts.UpdateAsync(subProduct);
		logger.LogInformation("Edited subproduct {SubProductId}", subProduct.Id);

		return new EditSubProductResponse();
    }

	public async Task<GetProductResponse> GetProductAsync(GetProductOrder order)
	{
		if (!await products.GetAll().AnyAsync(x => x.Id == order.ProductId && !x.Deleted))
		{
			ThrowValidationException("ProductId", "Nie znaleziono produktu");
		}

		var product = await products.GetAll()
			.Include(p => p.Parameters)
			.ThenInclude(o => o.Options)
			.Include(p => p.SubProductInProducts)
			.ThenInclude(sp => sp.SubProduct)
			.ThenInclude(sp => sp.Parameters)
			.ThenInclude(p => p.Options)
			.FirstAsync(p => p.Id == order.ProductId);

		var productDto = new ExtendedProductDto(product);
		logger.LogInformation("Got product {ProductId}", product.Id);

		return new GetProductResponse()
		{
			Product = productDto
		};
	}
	
    public async Task<GetSubProductResponse> GetSubProductAsync(GetSubProductOrder order)
    {
        if (!await subProducts.GetAll().AnyAsync(x => x.Id == order.SubProductId && !x.Deleted))
		{
			ThrowValidationException("SubProductId", "Nie znaleziono podproduktu");
		}
		
		var subProduct = await subProducts.GetAll()
			.Include(p => p.Parameters)
			.ThenInclude(o => o.Options)
			.Include(p => p.SubProductInProducts)
			.ThenInclude(sp => sp.Product)
			.FirstAsync(p => p.Id == order.SubProductId);

		var subProductDto = new ExtendedSubProductDto(subProduct);
		logger.LogInformation("Got subproduct {SubProductId}", subProduct.Id);

		return new GetSubProductResponse() 
		{
			SubProduct = subProductDto
		};
    }

    public async Task<ListProductsResponse> ListProductAsync(ListProductsOrder order)
    {
		var query = products.GetAll().Where(x => order.Statuses.Contains(x.Status) && !x.Deleted);
		
		var totalCount = await query.CountAsync();
		var dtos = await query
			.OrderBy(x => x.Id)
			.Skip(order.Pagination.PageIndex * order.Pagination.PageSize)
			.Take(order.Pagination.PageSize)
			.Select(p => new ProductDto(p))
			.ToListAsync();
			
		var response = new ListProductsResponse()
		{
			Products = new PagedList<ProductDto>(dtos, totalCount, order.Pagination.PageIndex, order.Pagination.PageSize)	
		};
		logger.LogInformation("Listed products");

		return response;
    }

    public async Task<ListSubProductsResponse> ListSubProductsAsync(ListSubProductsOrder order)
    {
		if (order.ProductId.HasValue && !await products.GetAll().AnyAsync(x => x.Id == order.ProductId.Value))
		{
			ThrowValidationException("ProductId", "Nie znaleziono produktu");
		}
		
		var query = subProducts.GetAll().Where(sp => !sp.Deleted);
		if (order.ProductId.HasValue)
		{
			query = query
				.Include(s => s.SubProductInProducts)
				.Where(s => !s.SubProductInProducts.Any(sp => sp.ProductId == order.ProductId));
		}
		
		var totalCount = await query.CountAsync();
		
		var dtos = (await query.ToListAsync())
			.OrderBy(x => x.Id)
			.Skip(order.Pagination.PageIndex * order.Pagination.PageSize)
			.Take(order.Pagination.PageSize)
			.Select(p => new SubProductDto(p))
			.ToList();
			
		var response = new ListSubProductsResponse()
		{
			SubProducts = new PagedList<SubProductDto>(dtos, totalCount, order.Pagination.PageIndex, order.Pagination.PageSize)
		};
		logger.LogInformation("Listed subproducts");

		return response;
    }

    public async Task<UnassignSubProductsResponse> UnassignSubProductsAsync(UnassignSubProductsOrder order)
    {
        var subProductIds = await subProducts.GetAll().Select(x => x.Id).ToListAsync();
		if (!order.SubProductIds.All(x => subProductIds.Contains(x)))
		{
			ThrowValidationException("SubProductIds", "Nie znaleziono podproduktu");
		}
		
		if (!await products.GetAll().AnyAsync(x => x.Id == order.ProductId))
		{
			ThrowValidationException("ProductId", "Nie znaleziono produktu");
		}

		if (!await subProductsInProducts.GetAll()
			.AnyAsync(x => order.SubProductIds.Contains(x.SubProductId) && order.ProductId == x.ProductId))
		{
			return new UnassignSubProductsResponse();
		}
		
		var toDeleteList = await subProductsInProducts.GetAll()
			.Where(x => order.SubProductIds.Contains(x.SubProductId) && order.ProductId == x.ProductId).ToListAsync();
		foreach (var toDelete in toDeleteList)
		{
			logger.LogInformation("Deleting assignment of subproduct {SubProductId} to product {ProductId}", toDelete.SubProductId, toDelete.ProductId);
			await subProductsInProducts.DeleteAsync(toDelete);
		}

		return new UnassignSubProductsResponse();
    }
}