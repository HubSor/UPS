using FluentValidation;
using Messages.Queries;

namespace Validators.Sales
{
	public class GetSaleDetailsOrderValidator : AbstractValidator<GetSaleQuery>
	{
		public GetSaleDetailsOrderValidator()
		{
			RuleFor(x => x.SaleId)
				.NotNull().WithMessage("Należy podać identyfikator sprzedaży")
				.GreaterThan(0).WithMessage("Identyfikator sprzedaży musi być dodatni");
		}
	}
}