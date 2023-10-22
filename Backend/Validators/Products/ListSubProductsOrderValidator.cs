using FluentValidation;
using Messages.Products;

namespace Validators.Products
{
	public class ListSubProductsOrderValidator: AbstractValidator<ListSubProductsOrder>
	{
		public ListSubProductsOrderValidator()
		{
			When(x => x.ProductId.HasValue, () => 
			{
				RuleFor(x => x.ProductId)
					.GreaterThan(0).WithMessage("Niepoprawny identyfikator produktu");				
			});
		}
	}
}
