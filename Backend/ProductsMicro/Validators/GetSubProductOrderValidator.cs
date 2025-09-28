using Core.Messages;
using FluentValidation;

namespace ProductsMicro.Validators
{
	public class GetSubProductOrderValidator : AbstractValidator<GetSubProductOrder>
	{
		public GetSubProductOrderValidator()
		{
			RuleFor(x => x.SubProductId)
				.NotNull().WithMessage("Należy podać identyfikator podproduktu")
				.GreaterThan(0).WithMessage("Identyfikator podproduktu musi być dodatni");
		}
	}
}
