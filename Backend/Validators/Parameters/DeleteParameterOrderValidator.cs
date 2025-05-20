using FluentValidation;
using Messages.Commands;

namespace Validators.Parameters
{
	public class DeleteParameterOrderValidator : AbstractValidator<DeleteParameterOrder>
	{
		public DeleteParameterOrderValidator()
		{
			RuleFor(x => x.ParameterId)
				.NotNull().WithMessage("Należy podać identyfikator parametru")
				.GreaterThan(0).WithMessage("Identyfikator parametru musi być dodatni");
		}
	}
}
