using FluentValidation;
using Messages.Queries;

namespace Validators.Products
{
	public class GetProductOrderValidator: AbstractValidator<GetProductQuery>
	{
		public GetProductOrderValidator()
		{
			RuleFor(x => x.ProductId)
				.NotNull().WithMessage("Należy podać identyfikator produktu")
				.GreaterThan(0).WithMessage("Identyfikator produktu musi być dodatni");
		}
	}
}
