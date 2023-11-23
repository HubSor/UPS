using FluentValidation;
using Messages.Sales;

namespace Validators.Products
{
	public class SaveSaleOrderValidator : AbstractValidator<SaveSaleOrder>
	{
		public SaveSaleOrderValidator()
		{
			RuleFor(x => x.ProductId)
				.NotNull().WithMessage("Należy podać identyfikator produktu")
				.GreaterThan(0).WithMessage("Identyfikator produktu musi być dodatni");

			RuleFor(x => x.ClientId)
				.GreaterThan(0).WithMessage("Identyfikator klienta musi być dodatni").When(x => x.ClientId.HasValue);

			RuleFor(x => x.SubProductIds)
				.NotEmpty().WithMessage("Należy podać identyfikatory podproduktów")
				.Must(ids => ids.All(id => id > 0)).WithMessage("Niepoprawne identyfikatory produktów");

			RuleFor(x => x.TotalPrice)
				.NotEmpty().WithMessage("Należy podać ostateczną cenę")
				.GreaterThan(0m).WithMessage("Cena musi być większa od zera")
				.LessThan(1_000_000_000m).WithMessage("Zbyt wysoka cena");

			RuleFor(x => x.Answers)
				.NotNull().WithMessage("Należy podać listę odpowiedzi")
				.ForEach(a => a.SetValidator(new SaveSaleParameterDtoValidator()))
				.Must(a => a.DistinctBy(x => x.ParameterId).Count() == a.Count()).WithMessage("Więcej niż jedna odpowiedź na jeden parametr");
		}
	}
}
