using FluentValidation;
using Messages.Sales;

namespace Validators.Sales
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
