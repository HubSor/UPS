using FluentValidation;
using Messages.Users;

namespace Validators.Users
{
	public class LoginValidator: AbstractValidator<LoginOrder>
	{
		public LoginValidator()
		{
			RuleFor(x => x.Password)
				.NotEmpty().WithMessage("Należy podać hasło")
				.SetValidator(new PasswordValidator());
				
			RuleFor(x => x.Username)
				.NotEmpty().WithMessage("Należy podać login")
				.SetValidator(new UsernameValidator());
		}
	}
}
