using FluentValidation;
using FluentValidation.Results;
using MassTransit;

namespace UPS.Filters
{
	public class ValidationFilter<T> : IFilter<SendContext<T>>
		where T : class
	{
		private readonly ILogger<ValidationFilter<T>> _logger;
		private readonly IEnumerable<IValidator<T>> _validators;

		public ValidationFilter(ILogger<ValidationFilter<T>> logger, IEnumerable<IValidator<T>> validators) 
		{
			_logger = logger;
			_validators = validators;
		}
		
		public void Probe(ProbeContext context)
		{
		}

		public async Task Send(SendContext<T> context, IPipe<SendContext<T>> next)
		{
			var errors = new List<ValidationFailure>();
			if (_validators.Any())
			{
				foreach (var validator in _validators)
				{
					var validationResult = await validator.ValidateAsync(context.Message);
					if (!validationResult.IsValid)
					{
						errors.AddRange(validationResult.Errors);
						_logger.LogInformation("Validator {Validator} detected validation errors:\n\t{ValidationErrors}",
							validator.GetType().Name, string.Join("\n\t", errors.ToList()));
					}
				}
			}

			if (errors.Any())
			{
				throw new ValidationException(errors);
			}

			await next.Send(context);
		}
	}
}