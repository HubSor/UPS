using FluentValidation;
using Messages.Products;

namespace Validators.Products
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
