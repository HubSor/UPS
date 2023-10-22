using FluentValidation;

namespace Validators.Products
{
	public class CodeValidator: AbstractValidator<string?>
	{
		public CodeValidator()
		{
			When(x => !string.IsNullOrEmpty(x), () => 
			{
				RuleFor(x => x)
					.MinimumLength(1).WithMessage("Minimalna długość kodu to 1")
					.MaximumLength(6).WithMessage("Maksymalna długość kodu to 6")
					.Must(y => y.All(c => char.IsUpper(c) || char.IsDigit(c) || c == '-'))
						.WithMessage("Kod może składać się tylko z wielkich liter, cyfr lub znaku \'-\'");
			});
		}
	}
}
