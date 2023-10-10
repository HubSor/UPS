using Dtos;
using FluentValidation;

namespace Validators
{
	public class PaginationValidator: AbstractValidator<PaginationDto>
	{
		public PaginationValidator()
		{				
			When(x => x != null, () => 
			{
				RuleFor(y => y.PageNumber)
					.GreaterThanOrEqualTo(0).WithMessage("Numer strony nie może być ujemny");
				
				RuleFor(x => x.PageSize)
					.GreaterThan(0).WithMessage("Rozmiar strony musi być dodatni");
			});
		}
	}
}
