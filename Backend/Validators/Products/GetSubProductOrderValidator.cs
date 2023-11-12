using FluentValidation;
using Messages.Products;

namespace Validators.Products
{
	public class GetSubProductOrderValidator : AbstractValidator<GetSubProductOrder>
	{
		public GetSubProductOrderValidator()
		{
			RuleFor(x => x.SubProductId)
				.NotNull().WithMessage("Należy podać identyfikator podproduktu")
				.GreaterThan(0).WithMessage("Identyfikator podproduktu musi być dodatni");
		}
	}
}
