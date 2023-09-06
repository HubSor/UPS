using FluentValidation;

namespace Validators.Users
{
	public class PasswordValidator: AbstractValidator<string>
	{
		public PasswordValidator()
		{
			When(x => !string.IsNullOrEmpty(x), () => 
			{
				RuleFor(x => x)
					.MinimumLength(8).WithMessage("Minimalna długość hasła to 8")
					.MaximumLength(64).WithMessage("Maksymalna długość hasła to 64");
			});
		}
	}
}
