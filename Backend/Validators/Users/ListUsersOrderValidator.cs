using FluentValidation;
using Messages.Users;

namespace Validators.Users
{
	public class ListUsersOrderValidator: AbstractValidator<ListUsersOrder>
	{
		public ListUsersOrderValidator()
		{
			RuleFor(x => x.Pagination)
				.NotNull().WithMessage("Należy podać parametry stronicowania")
				.SetValidator(new PaginationValidator());
		}
	}
}
