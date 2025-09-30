using Core.Messages;
using Core.Validators;
using FluentValidation;

namespace SalesMicro.Validators
{
	public class ListSalesOrderValidator : AbstractValidator<ListSalesOrder>
	{
		public ListSalesOrderValidator()
		{
			RuleFor(x => x.Pagination)
				.NotNull().WithMessage("Należy podać parametry stronicowania")
				.SetValidator(new PaginationValidator());
		}
	}
}
