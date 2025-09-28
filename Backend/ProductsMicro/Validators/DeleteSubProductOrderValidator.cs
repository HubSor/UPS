using Core.Messages;
using FluentValidation;

namespace ProductsMicro.Validators
{
	public class DeleteSubProductOrderValidator: AbstractValidator<DeleteSubProductOrder>
	{
		public DeleteSubProductOrderValidator()
		{
			RuleFor(x => x.SubProductId)
				.NotNull().WithMessage("Należy podać identyfikator podproduktu")
				.GreaterThan(0).WithMessage("Identyfikator podproduktu musi być dodatni");
		}
	}
}
