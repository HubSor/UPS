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
				.MaximumLength(64).WithMessage("Zbyt długie hasło")
				.MinimumLength(8).WithMessage("Zbyt krótkie hasło");
								
			RuleFor(x => x.Username)
				.NotEmpty().WithMessage("Należy podać login")
				.SetValidator(new UsernameValidator());
		}
	}
}
