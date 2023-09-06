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
					.MinimumLength(8).WithMessage("Minimalna długość loginu to 8")
					.MaximumLength(64).WithMessage("Maksymalna długość loginu to 64");
			});
		}
	}
}
