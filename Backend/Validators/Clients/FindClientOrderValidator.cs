using FluentValidation;
using Messages.Queries;

namespace Validators.Clients
{
	public class FindClientOrderValidator : AbstractValidator<FindClientQuery>
	{
		public FindClientOrderValidator()
		{
			RuleFor(x => x.Identifier)
				.NotEmpty().WithMessage("Należy podać identyfikator klienta")
				.MaximumLength(16).WithMessage("Zbyt długi identyfikator");
		}
	}
}
