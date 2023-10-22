using FluentValidation;
using Messages.Products;

namespace Validators.Products
{
	public class EditSubProductOrderValidator: AbstractValidator<EditSubProductOrder>
	{
		public EditSubProductOrderValidator()
		{
			RuleFor(x => x.SubProductDto.Id)
				.GreaterThan(0).WithMessage("Niepoprawny identyfikator podproduktu");

			RuleFor(x => x.SubProductDto.Name)
				.NotEmpty().WithMessage("Należy podać nazwę podproduktu")
				.MaximumLength(128).WithMessage("Zbyt długa nazwa podproduktu");
				
			RuleFor(x => x.SubProductDto.Code)
				.NotEmpty().WithMessage("Należy podać unikalny kod podproduktu")
				.SetValidator(new CodeValidator());
					
			RuleFor(x => x.SubProductDto.BasePrice)
				.GreaterThanOrEqualTo(0).WithMessage("Cena nie może być ujemna")
				.LessThanOrEqualTo(1_000_000).WithMessage("Zbyt wysoka cena"); // todo konfig
				
			RuleFor(x => x.SubProductDto.Description)
				.MaximumLength(1000).WithMessage("Opis zbyt długi");
		}
	}
}
