using FluentValidation;
using Messages.Clients;

namespace Validators.Clients
{
	public class FindClientOrderValidator : AbstractValidator<FindClientOrder>
	{
		public FindClientOrderValidator()
		{
			RuleFor(x => x.IsPerson)
				.NotNull().WithMessage("Należy podać flagę");

			RuleFor(x => x.Identifier)
				.NotEmpty().WithMessage("Należy podać identyfikator klienta")
				.MaximumLength(16).WithMessage("Zbyt długi identyfikator");
		}
	}
}
