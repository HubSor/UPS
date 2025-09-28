using Core.Messages;
using FluentValidation;

namespace ProductsMicro.Validators
{
	public class ListProductsOrderValidator: AbstractValidator<ListProductsOrder>
	{
		public ListProductsOrderValidator()
		{
			RuleFor(x => x.Statuses)
				.ForEach(x => x.IsInEnum().WithMessage("Niepoprawny status produktu"))
				.NotEmpty().WithMessage("Należy podać przynajmniej jeden status");
		}
	}
}
