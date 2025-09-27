using FluentValidation;
using UsersMicro.Messages;

namespace UsersMicro.Validators
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
