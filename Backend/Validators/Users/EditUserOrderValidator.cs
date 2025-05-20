using FluentValidation;
using Messages.Commands;

namespace Validators.Users
{
	public class EditUserOrderValidator: AbstractValidator<EditUserOrder>
	{
		public EditUserOrderValidator()
		{
			RuleFor(x => x.Password)
				.SetValidator(new PasswordValidator());
				
			RuleFor(x => x.Id)
				.NotNull().WithMessage("Należy podać identyfikator użytkownika")
				.GreaterThan(0).WithMessage("Identyfikator musi być dodatni");
								
			RuleFor(x => x.Username)
				.NotEmpty().WithMessage("Należy podać login")
				.SetValidator(new UsernameValidator());
				
			RuleFor(x => x.RoleIds)
				.ForEach(x => x.IsInEnum().WithMessage("Niepoprawne id roli"));
		}
	}
}
