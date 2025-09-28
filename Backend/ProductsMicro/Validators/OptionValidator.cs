using Core.Dtos;
using FluentValidation;

namespace ProductsMicro.Validators
{
	public class OptionValidator: AbstractValidator<OptionDto>
	{
		public OptionValidator()
		{
			RuleFor(x => x.Value)
				.NotEmpty().WithMessage("Należy podać wartość odpowiedzi")
				.MaximumLength(256).WithMessage("Zbyt długa wartość odpowiedzi");
		}
	}
}
