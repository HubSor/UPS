using FluentValidation;
using Messages.Queries;

namespace Validators.Users
{
	public class ListUsersOrderValidator: AbstractValidator<ListUsersQuery>
	{
		public ListUsersOrderValidator()
		{
			RuleFor(x => x.Pagination)
				.NotNull().WithMessage("Należy podać parametry stronicowania")
				.SetValidator(new PaginationValidator());
		}
	}
}
