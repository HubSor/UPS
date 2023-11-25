using FluentValidation;
using Messages.Clients;

namespace Validators.Clients
{
	public class ListClientsOrderValidator : AbstractValidator<ListClientsOrder>
	{
		public ListClientsOrderValidator()
		{
			RuleFor(x => x.Pagination)
				.NotNull().WithMessage("Należy podać parametry stronicowania")
				.SetValidator(new PaginationValidator());
		}
	}
}
