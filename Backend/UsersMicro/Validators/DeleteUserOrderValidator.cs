using Core.Messages;
using FluentValidation;

namespace UsersMicro.Validators
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
