using FluentValidation;
using Messages.Commands;

namespace Validators.Parameters
{
	public class DeleteOptionOrderValidator : AbstractValidator<DeleteOptionOrder>
	{
		public DeleteOptionOrderValidator()
		{
			RuleFor(x => x.OptionId)
				.NotNull().WithMessage("Należy podać identyfikator opcji")
				.GreaterThan(0).WithMessage("Identyfikator opcji musi być dodatni");
		}
	}
}
