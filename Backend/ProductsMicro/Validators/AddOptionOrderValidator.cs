using Core.Messages;
using FluentValidation;

namespace ProductsMicro.Validators
{
	public class AddOptionOrderValidator : AbstractValidator<AddOptionOrder>
	{
		public AddOptionOrderValidator()
		{
			RuleFor(x => x.Value)
				.NotEmpty().WithMessage("Należy podać wartość odpowiedzi")
				.MaximumLength(256).WithMessage("Zbyt długa wartość odpowiedzi");

			RuleFor(x => x.ParameterId)
				.NotNull().WithMessage("Należy podać identyfikator parametru")
				.GreaterThan(0).WithMessage("Identyfikator parametru musi być dodatni");
		}
	}
}
