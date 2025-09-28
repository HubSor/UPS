using Core.Messages;
using FluentValidation;

namespace UsersMicro.Validators
{
	public class LoginOrderValidator: AbstractValidator<LoginOrder>
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
