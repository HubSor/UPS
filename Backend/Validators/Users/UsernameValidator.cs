using FluentValidation;

namespace Validators.Users
{
	public class UsernameValidator: AbstractValidator<string>
	{
		public UsernameValidator()
		{				
			When(x => !string.IsNullOrEmpty(x), () => 
			{
				RuleFor(x => x)
					.MinimumLength(4).WithMessage("Minimalna długość loginu to 4")
					.MaximumLength(64).WithMessage("Maksymalna długość loginu to 64")
					.Must(x => x.All(c => !char.IsWhiteSpace(c))).WithMessage("Nazwa użytkownika nie może zawierać białych znaków");
			});
		}
	}
}
