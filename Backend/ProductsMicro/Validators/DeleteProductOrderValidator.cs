using Core.Messages;
using FluentValidation;

namespace ProductsMicro.Validators
{
	public class DeleteProductOrderValidator: AbstractValidator<DeleteProductOrder>
	{
		public DeleteProductOrderValidator()
		{
			RuleFor(x => x.ProductId)
				.NotNull().WithMessage("Należy podać identyfikator produktu")
				.GreaterThan(0).WithMessage("Identyfikator produktu musi być dodatni");
		}
	}
}
