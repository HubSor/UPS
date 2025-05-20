using FluentValidation;
using Messages.Queries;

namespace Validators.Clients
{
	public class ListClientsOrderValidator : AbstractValidator<ListClientsQuery>
	{
		public ListClientsOrderValidator()
		{
			RuleFor(x => x.Pagination)
				.NotNull().WithMessage("Należy podać parametry stronicowania")
				.SetValidator(new PaginationValidator());
		}
	}
}
