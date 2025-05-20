using FluentValidation;
using Messages.Queries;

namespace Validators.Products
{
	public class GetSubProductOrderValidator : AbstractValidator<GetSubProductQuery>
	{
		public GetSubProductOrderValidator()
		{
			RuleFor(x => x.SubProductId)
				.NotNull().WithMessage("Należy podać identyfikator podproduktu")
				.GreaterThan(0).WithMessage("Identyfikator podproduktu musi być dodatni");
		}
	}
}
