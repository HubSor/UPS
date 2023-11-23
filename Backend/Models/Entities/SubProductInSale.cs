namespace Models.Entities
{
	public class SubProductInSale : IEntity
	{
		public int SaleId { get; set; }
		public virtual Sale Sale { get; set; } = default!;
		public int SubProductId { get; set; }
		public virtual SubProduct SubProduct { get; set; } = default!;
	}
}