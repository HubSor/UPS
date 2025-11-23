using Core.Models;

namespace Core.Dtos
{
	public class SubProductDto 
	{
		public int Id { get; set; }
		public string Name { get; set; } = default!;
		public string Code { get; set; } = default!;
		public decimal BasePrice { get; set; }
		public decimal TaxRate { get; set; }
		public string? Description { get; set; }

		public SubProductDto(){}		
		public SubProductDto(SubProduct sp)
		{
			Id = sp.Id;
			Name = sp.Name;
			Description = sp.Description;
			Code = sp.Code;
			BasePrice = sp.BasePrice;
			TaxRate = sp.TaxRate;
		}
	}
}
