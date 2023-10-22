using FluentValidation;
using Messages.Products;

namespace Validators.Products
{
	public class UnassignSubProductsOrderValidator: AbstractValidator<UnassignSubProductsOrder>
	{
		public UnassignSubProductsOrderValidator()
		{
			RuleFor(x => x.ProductId)
				.NotNull().WithMessage("Należy podać identyfikator produktu")
				.GreaterThan(0).WithMessage("Identyfikator produktu musi być dodatni");
				
			RuleFor(x => x.SubProductIds)
				.NotEmpty().WithMessage("Należy podać identyfikatoy podproduktów")
				.Must(x => x.Length == x.Distinct().Count()).WithMessage("Zduplikowane identyfikatory podproduktów")
				.Must(x => x.All(id => id > 0)).WithMessage("Każdy identyfikator podproduktu musi być dodatni");
		}
	}
}
