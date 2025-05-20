using FluentValidation;
using Messages.Commands;

namespace Validators.Products
{
	public class EditSubProductOrderValidator: AbstractValidator<EditSubProductOrder>
	{
		public EditSubProductOrderValidator()
		{
			RuleFor(x => x.Id)
				.GreaterThan(0).WithMessage("Niepoprawny identyfikator podproduktu");

			RuleFor(x => x.Name)
				.NotEmpty().WithMessage("Należy podać nazwę podproduktu")
				.MaximumLength(128).WithMessage("Zbyt długa nazwa podproduktu");
				
			RuleFor(x => x.Code)
				.NotEmpty().WithMessage("Należy podać unikalny kod podproduktu")
				.SetValidator(new CodeValidator());
					
			RuleFor(x => x.BasePrice)
				.GreaterThanOrEqualTo(0).WithMessage("Cena nie może być ujemna")
				.LessThanOrEqualTo(1_000_000).WithMessage("Zbyt wysoka cena");
				
			RuleFor(x => x.Description)
				.MaximumLength(1000).WithMessage("Opis zbyt długi");

			RuleFor(x => x.TaxRate)
				.NotNull().WithMessage("Należy podać stawkę podatku")
				.GreaterThanOrEqualTo(0).WithMessage("Stawka nie może być ujemna")
				.LessThanOrEqualTo(200).WithMessage("Zbyt wysoki podatek");
		}
	}
}
