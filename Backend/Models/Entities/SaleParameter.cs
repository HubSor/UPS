namespace Models.Entities
{
	public class SaleParameter : IEntity
	{
		public string? Value { get; set; }
		public int SaleId { get; set; }
		public virtual Sale Sale { get; set; } = default!;
		public int ParameterId { get; set; }
		public virtual Parameter Parameter { get; set; } = default!;
		public int? OptionId { get; set; }
		public virtual ParameterOption? Option { get; set; }
	}
}