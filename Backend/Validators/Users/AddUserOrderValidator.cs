using FluentValidation;
using Messages.Users;
using Models.Entities;

namespace Validators.Users
{
	public class AddUserOrderValidator: AbstractValidator<AddUserOrder>
	{
		public AddUserOrderValidator()
		{
			RuleFor(x => x.Password)
				.NotEmpty().WithMessage("Należy podać hasło")
				.SetValidator(new PasswordValidator());
								
			RuleFor(x => x.Username)
				.NotEmpty().WithMessage("Należy podać login")
				.SetValidator(new UsernameValidator());
				
			RuleFor(x => x.RoleIds)
				.ForEach(x => x.IsInEnum().WithMessage("Niepoprawne id roli"));
		}
	}
}
