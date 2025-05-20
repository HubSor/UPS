using FluentValidation;
using Messages.Queries;

namespace Validators.Users
{
	public class LoginOrderValidator: AbstractValidator<LoginQuery>
	{
		public LoginOrderValidator()
		{
			RuleFor(x => x.Password)
				.NotEmpty().WithMessage("Należy podać hasło")
				.MaximumLength(128).WithMessage("Zbyt długie hasło")
				.MinimumLength(1).WithMessage("Zbyt krótkie hasło");
								
			RuleFor(x => x.Username)
				.NotEmpty().WithMessage("Należy podać login")
				.SetValidator(new UsernameValidator());
		}
	}
}
