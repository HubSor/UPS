using FluentValidation;
using Messages.Queries;

namespace Validators.Sales
{
	public class ListSalesOrderValidator : AbstractValidator<ListSalesQuery>
	{
		public ListSalesOrderValidator()
		{
			RuleFor(x => x.Pagination)
				.NotNull().WithMessage("Należy podać parametry stronicowania")
				.SetValidator(new PaginationValidator());
		}
	}
}
