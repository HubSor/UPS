using Core.Messages;
using FluentValidation;

namespace ProductsMicro.Validators
{
	public class GetProductOrderValidator: AbstractValidator<GetProductOrder>
	{
		public GetProductOrderValidator()
		{
			RuleFor(x => x.ProductId)
				.NotNull().WithMessage("Należy podać identyfikator produktu")
				.GreaterThan(0).WithMessage("Identyfikator produktu musi być dodatni");
		}
	}
}
