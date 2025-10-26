using Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Entities;
using Data;
using Messages.Parameters;

namespace Services.Application;

public interface IParametersApplicationService : IBaseApplicationService
{
    Task<AddParameterResponse> AddParameterAsync(AddParameterOrder order);
    Task<EditParameterResponse> EditParameterAsync(EditParameterOrder order);
    Task<DeleteParameterResponse> DeleteParameterAsync(DeleteParameterOrder order);
    Task<AddOptionResponse> AddOptionAsync(AddOptionOrder order);
    Task<DeleteOptionResponse> DeleteOptionAsync(DeleteOptionOrder order);
}

public class ParametersApplicationService(
    ILogger<ParametersApplicationService> logger,
    IUnitOfWork unitOfWork,
    IRepository<Parameter> parameters,
    IRepository<ParameterOption> options,
    IRepository<Product> products,
    IRepository<SubProduct> subProducts
) : BaseApplicationService(logger, unitOfWork), IParametersApplicationService
{
    public async Task<AddOptionResponse> AddOptionAsync(AddOptionOrder order)
    {
        var param = await parameters.GetAll().FirstOrDefaultAsync(x => x.Id == order.ParameterId && !x.Deleted);
		if (param == null)
		{
			ThrowValidationException("ParameterId", "Nie znaleziono parametru");
		}

		if (!param!.Type.AllowsOptions())
		{
			ThrowValidationException("ParameterId", "Typ parametru nie pozwala na dodanie opcji");
		}

		var option = new ParameterOption()
		{
			Value = order.Value,
			ParameterId = param.Id
		};
		await options.AddAsync(option);
		logger.LogInformation("Added option to parameter {ParameterId}", param.Id);

		return new AddOptionResponse();
    }

    public async Task<AddParameterResponse> AddParameterAsync(AddParameterOrder order)
    {
		if (order.ProductId.HasValue && !await products.GetAll().AnyAsync(x => x.Id == order.ProductId && !x.Deleted))
		{
			ThrowValidationException("ProductId", "Nie znaleziono produktu");
		}

		if (order.SubProductId.HasValue && !await subProducts.GetAll().AnyAsync(x => x.Id == order.SubProductId && !x.Deleted))
		{
			ThrowValidationException("SubProductId", "Nie znaleziono podproduktu");
		}

		var parameter = new Parameter()
		{
			Name = order.Name,
			Required = order.Required,
			Type = order.Type,
			ProductId = order.ProductId,
			SubProductId = order.SubProductId
		};
		
		parameter.Options = order.Options?.Select(x => new ParameterOption()
		{
			Value = x.Value,
			Parameter = parameter
		}).ToList() ?? [];
		
		await parameters.AddAsync(parameter);
		logger.LogInformation("Added parameter with ProductId: {ProductId} SubProductId: {SubProductId}", parameter.ProductId, parameter.SubProductId);

		return new AddParameterResponse();
    }

    public async Task<DeleteOptionResponse> DeleteOptionAsync(DeleteOptionOrder order)
    {
        var opt = await options.GetAll()
			.FirstOrDefaultAsync(x => x.Id == order.OptionId);

		if (opt == null)
		{
			ThrowValidationException("OptionId", "Nie znaleziono opcji");
		}
		
		await options.DeleteAsync(opt!);
		logger.LogInformation("Deleted option {OptionId}", opt!.Id);

		return new DeleteOptionResponse();
    }

    public async Task<DeleteParameterResponse> DeleteParameterAsync(DeleteParameterOrder order)
    {
        var param = await parameters.GetAll()
			.Include(x => x.SaleParameters)
			.FirstOrDefaultAsync(x => x.Id == order.ParameterId && !x.Deleted);

		if (param == null)
		{
			ThrowValidationException("ParameterId", "Nie znaleziono parametru");
		}

		if (param!.SaleParameters.Any())
		{
			param.Deleted = true;
			await parameters.UpdateAsync(param);
			logger.LogInformation("Soft deleted parameter {ParameterId}", param.Id);
		}
		else
		{
			await parameters.DeleteAsync(param);
			logger.LogInformation("Hard deleted parameter {ParameterId}", param.Id);
		}

		return new DeleteParameterResponse();
    }

    public async Task<EditParameterResponse> EditParameterAsync(EditParameterOrder order)
    {
        var parameter = await parameters.GetAll()
			.Include(x => x.Options)
			.FirstOrDefaultAsync(x => x.Id == order.ParameterId && !x.Deleted);

		if (parameter == null)
		{
			ThrowValidationException("ParameterId", "Nie znaleziono parametru");
		}
		
		logger.LogInformation("Editing parameter {ParameterId}", parameter!.Id);
		parameter.Required = order.Required;
		parameter.Name = order.Name;
		
		var oldAllowsOptions = parameter.Type.AllowsOptions();
		var newAllowsOptions = order.Type.AllowsOptions();
		parameter.Type = order.Type;

		if (oldAllowsOptions)
		{
			logger.LogInformation("Deleting options of old parameter");
			foreach (var opt in parameter.Options.ToList())
			{
				await options.DeleteAsync(opt);
				parameter.Options.Remove(opt);
			}
		}
		
		if (newAllowsOptions)
		{
			logger.LogInformation("Adding options for new parameter");
			parameter.Options = order.Options?.Select(x => new ParameterOption()
			{
				Value = x.Value,
				Parameter = parameter
			}).ToList() ?? new List<ParameterOption>();
		}

		await parameters.UpdateAsync(parameter);
		logger.LogInformation("Edited parameter {ParameterId}", parameter.Id);

		return new EditParameterResponse();
    }
}