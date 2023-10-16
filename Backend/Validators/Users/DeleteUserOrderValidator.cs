using FluentValidation;
using Messages.Users;

namespace Validators.Users
{
	public class DeleteUserOrderValidator: AbstractValidator<DeleteUserOrder>
	{
		public DeleteUserOrderValidator()
		{			
			RuleFor(x => x.Id)
				.NotNull().WithMessage("Należy podać identyfikator użytkownika")
				.GreaterThan(0).WithMessage("Identyfikator musi być dodatni");
		}
	}
}
