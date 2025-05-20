using FluentValidation;
using Messages.Queries;

namespace Validators.Products
{
	public class ListProductsOrderValidator: AbstractValidator<ListProductsQuery>
	{
		public ListProductsOrderValidator()
		{
			RuleFor(x => x.Statuses)
				.ForEach(x => x.IsInEnum().WithMessage("Niepoprawny status produktu"))
				.NotEmpty().WithMessage("Należy podać przynajmniej jeden status");
		}
	}
}
