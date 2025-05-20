using FluentValidation;
using Models.Entities;
using Validators.Products;
using Messages.Commands;

namespace Validators.Parameters
{
	public class AddParameterOrderValidator: AbstractValidator<AddParameterOrder>
	{
		public AddParameterOrderValidator()
		{
			RuleFor(x => x.Name)
				.NotEmpty().WithMessage("Należy podać nazwę parametru")
				.MaximumLength(128).WithMessage("Zbyt długa nazwa parametru");
				
			RuleFor(x => x.Required)
				.NotNull().WithMessage("Należy podać obowiązkowość parametru");
				
			RuleFor(x => x.Type)
				.IsInEnum().WithMessage("Niepoprawny typ parametru");

			RuleFor(x => x.SubProductId)
				.GreaterThan(0).WithMessage("Identyfikator podproduktu musi być dodatni");

			RuleFor(x => x.ProductId)
				.GreaterThan(0).WithMessage("Identyfikator podproduktu musi być dodatni")
				.Must((order, id) => order.SubProductId.HasValue != order.ProductId.HasValue)
					.WithMessage("Należy podać identyfikator produktu albo subproduktu");
				
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
