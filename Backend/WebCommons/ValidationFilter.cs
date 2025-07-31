using FluentValidation;
using FluentValidation.Results;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace WebCommons;

public class ValidationFilter<T>(ILogger<ValidationFilter<T>> logger, IEnumerable<IValidator<T>> validators) : IFilter<SendContext<T>>
	where T : class
{
    public void Probe(ProbeContext context)
	{
	}

	public async Task Send(SendContext<T> context, IPipe<SendContext<T>> next)
	{
		var errors = new List<ValidationFailure>();
		if (validators.Any())
		{
			foreach (var validator in validators)
			{
				var validationResult = await validator.ValidateAsync(context.Message);
				if (!validationResult.IsValid)
				{
					errors.AddRange(validationResult.Errors);
					logger.LogInformation("Validator {Validator} detected validation errors:\n\t{ValidationErrors}",
						validator.GetType().Name, string.Join("\n\t", errors.ToList()));
				}
			}
		}

		if (errors.Count != 0)
		{
			throw new ValidationException(errors);
		}

		await next.Send(context);
	}
}
