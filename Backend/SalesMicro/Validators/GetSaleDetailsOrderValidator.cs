using Core.Messages;
using FluentValidation;

namespace SalesMicro.Validators
{
	public class GetSaleDetailsOrderValidator : AbstractValidator<GetSaleOrder>
	{
		public GetSaleDetailsOrderValidator()
		{
			RuleFor(x => x.SaleId)
				.NotNull().WithMessage("Należy podać identyfikator sprzedaży")
				.GreaterThan(0).WithMessage("Identyfikator sprzedaży musi być dodatni");
		}
	}
}