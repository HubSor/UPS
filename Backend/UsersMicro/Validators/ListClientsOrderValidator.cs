using Core.Messages;
using Core.Validators;
using FluentValidation;

namespace UsersMicro.Validators
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
