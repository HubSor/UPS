using System.Net.Mail;
using System.Text.RegularExpressions;
using Core.Messages;
using FluentValidation;

namespace Validators.Clients
{
	public partial class UpsertClientOrderValidator : AbstractValidator<UpsertClientOrder>
	{
		public UpsertClientOrderValidator()
		{
			RuleFor(x => x.IsCompany)
				.NotNull().WithMessage("Należy podać flagę");
			
			RuleFor(x => x.ClientId)
				.GreaterThan(0).WithMessage("Identyfikator musi być dodatni")
				.When(x => x.ClientId.HasValue);
				
			RuleFor(x => x.Nip)
				.Matches("^[0-9]{10}$").WithMessage("Niepoprawny NIP").When(o => o.Nip != null);

			RuleFor(x => x.Pesel)
				.Matches("^[0-9]{11}$").WithMessage("Niepoprawny PESEL").When(o => o.Pesel != null);

			RuleFor(x => x.Regon)
				.Must(x => ShortRegonRegex().Match(x!).Success || LongRegonRegex().Match(x!).Success)
				.WithMessage("Niepoprawny REGON").When(o => o.Regon != null);

			RuleFor(x => x.CompanyName)
				.NotEmpty().WithMessage("Należy podać nazwę firmy").When(x => x.IsCompany)
				.MaximumLength(256).WithMessage("Zbyt długa nazwa firmy");

			RuleFor(x => x.FirstName)
				.NotEmpty().WithMessage("Należy podać imię").When(x => !x.IsCompany)
				.MaximumLength(128).WithMessage("Zbyt długie imię");

			RuleFor(x => x.LastName)
				.NotEmpty().WithMessage("Należy podać nazwisko").When(x => !x.IsCompany)
				.MaximumLength(128).WithMessage("Zbyt długie nazwisko");

			When(x => x.PhoneNumber != null, () => 
			{
				RuleFor(x => x.PhoneNumber)
					.MinimumLength(9).WithMessage("Zbyt krótki numer telefonu")
					.MaximumLength(15).WithMessage("Zbyt długi numer telefonu")
					.Must(num => num.All(c =>  char.IsDigit(c))).WithMessage("Numer musi składać się z cyfr");
			});

			When(x => x.Email != null, () =>
			{
				RuleFor(x => x.Email)
					.Must(email => MailAddress.TryCreate(email, out _)).WithMessage("Niepoprawny adres email");
			});
		}

		[GeneratedRegex("^[0-9]{9}$")]
		private static partial Regex ShortRegonRegex();
		[GeneratedRegex("^[0-9]{14}$")]
		private static partial Regex LongRegonRegex();
	}
}
