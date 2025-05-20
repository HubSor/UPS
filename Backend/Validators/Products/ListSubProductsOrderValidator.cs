using FluentValidation;
using Messages.Queries;

namespace Validators.Products
{
	public class ListSubProductsOrderValidator: AbstractValidator<ListSubProductsQuery>
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
