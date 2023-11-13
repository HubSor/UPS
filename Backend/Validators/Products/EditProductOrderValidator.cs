using FluentValidation;
using Messages.Products;

namespace Validators.Products
{
	public class EditProductOrderValidator: AbstractValidator<EditProductOrder>
	{
		public EditProductOrderValidator()
		{
			RuleFor(x => x.Id)
				.GreaterThan(0).WithMessage("Niepoprawny identyfikator produktu");
				
			RuleFor(x => x.Status)
				.IsInEnum().WithMessage("Niepoprawny status produktu");

			RuleFor(x => x.Name)
				.NotEmpty().WithMessage("Należy podać nazwę produktu")
				.MaximumLength(128).WithMessage("Zbyt długa nazwa produktu");
				
			RuleFor(x => x.Code)
				.NotEmpty().WithMessage("Należy podać unikalny kod produktu")
				.SetValidator(new CodeValidator());
					
			RuleFor(x => x.BasePrice)
				.GreaterThanOrEqualTo(0).WithMessage("Cena nie może być ujemna")
				.LessThanOrEqualTo(1_000_000).WithMessage("Zbyt wysoka cena"); // todo konfig
				
			RuleFor(x => x.Description)
				.MaximumLength(1000).WithMessage("Opis zbyt długi");
		}
	}
}
