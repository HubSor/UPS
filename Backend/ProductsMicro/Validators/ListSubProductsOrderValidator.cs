using Core.Messages;
using FluentValidation;

namespace ProductsMicro.Validators
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
