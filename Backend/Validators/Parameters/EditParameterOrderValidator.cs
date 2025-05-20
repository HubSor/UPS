using FluentValidation;
using Models.Entities;
using Validators.Products;
using Messages.Commands;

namespace Validators.Parameters
{
	public class EditParameterOrderValidator: AbstractValidator<EditParameterOrder>
	{
		public EditParameterOrderValidator()
		{
			RuleFor(x => x.Name)
				.NotEmpty().WithMessage("Należy podać nazwę parametru")
				.MaximumLength(128).WithMessage("Zbyt długa nazwa parametru");
				
			RuleFor(x => x.Required)
				.NotNull().WithMessage("Należy podać obowiązkowość parametru");
				
			RuleFor(x => x.Type)
				.IsInEnum().WithMessage("Niepoprawny typ parametru");
				
			When(order => order.Type.AllowsOptions(), () => 
			{
				RuleFor(x => x.Options)
					.NotEmpty().WithMessage("Należy podać opcje do wyboru")
					.ForEach(y => y.SetValidator(new OptionValidator()));
					
			}).Otherwise(() => 
			{
				RuleFor(x => x.Options)
					.Empty().WithMessage("Opcje nie są dostępne dla tego typu parametru");
			});
		}
	}
}
