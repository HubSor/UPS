using FluentValidation;
using Messages.Products;

namespace Validators.Products
{
	public class EditSubProductAssignmentOrderValidator : AbstractValidator<EditSubProductAssignmentOrder>
	{
		public EditSubProductAssignmentOrderValidator()
		{
			RuleFor(x => x.ProductId)
				.NotNull().WithMessage("Należy podać identyfikator produktu")
				.GreaterThan(0).WithMessage("Identyfikator produktu musi być dodatni");
				
			RuleFor(x => x.SubProductId)
				.NotNull().WithMessage("Należy podać identyfikator podproduktu")
				.GreaterThan(0).WithMessage("Identyfikator podproduktu musi być dodatni");
				
			RuleFor(x => x.NewPrice)
				.GreaterThanOrEqualTo(0).WithMessage("Cena nie może być ujemna")
				.LessThanOrEqualTo(1_000_000).WithMessage("Zbyt wysoka cena"); // todo konfig
		}
	}
}
