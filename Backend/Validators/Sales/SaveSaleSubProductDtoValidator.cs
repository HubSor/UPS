using Dtos.Sales;
using FluentValidation;

namespace Validators.Sales
{
	public class SaveSaleSubProductDtoValidator : AbstractValidator<SaveSaleSubProductDto>
	{
		public SaveSaleSubProductDtoValidator()
		{
			RuleFor(x => x.SubProductId)
				.NotNull().WithMessage("Należy podać identyfikator podproduktu")
				.GreaterThan(0).WithMessage("Identyfikator podproduktu musi być dodatni");

			RuleFor(x => x.Price)
				.NotEmpty().WithMessage("Należy podać cenę podproduktu")
				.GreaterThan(0m).WithMessage("Cena musi być większa od zera")
				.LessThan(1_000_000_000m).WithMessage("Zbyt wysoka cena");
		}
	}
}