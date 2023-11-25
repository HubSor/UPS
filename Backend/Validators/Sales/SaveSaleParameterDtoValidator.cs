using Dtos.Sales;
using FluentValidation;

namespace Validators.Sales
{
	public class SaveSaleParameterDtoValidator : AbstractValidator<SaveSaleParameterDto>
	{
		public SaveSaleParameterDtoValidator()
		{
			RuleFor(x => x.ParameterId)
				.NotNull().WithMessage("Należy podać identyfikator parametru")
				.GreaterThan(0).WithMessage("Identyfikator parametru musi być dodatni");

			When(x => !string.IsNullOrEmpty(x.Answer), () => 
			{
				RuleFor(x => x.Answer)
					.MaximumLength(1024).WithMessage("Zbyt długa odpowiedź");
			});
		}
	}
}