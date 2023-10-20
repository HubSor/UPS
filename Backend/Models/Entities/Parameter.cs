using System.ComponentModel.DataAnnotations;

namespace Models.Entities
{
	public class Parameter : Entity<int>
	{
		[MaxLength(128)]
		public string Name { get; set; } = default!;
		public bool IsRequired { get; set; }
		public ParameterTypeEnum Type { get; set; }
		public ParameterType TypeObject { get; set; } = default!;
		public IEnumerable<ParameterOption> Options { get; set; } = default!;
		public int? ProductId { get; set; }
		public virtual Product? Product { get; set; }
		public int? SubProductId { get; set; }
		public virtual SubProduct? SubProduct { get; set; } 
	}
	
	public class ParameterOption : Entity<int>
	{
		public int ParameterId { get; set; }
		public virtual Parameter Parameter { get; set; } = default!;
		[MaxLength(256)]
		public string Value { get; set; } = default!;
	}
	
	public enum ParameterTypeEnum
	{
		Text = 0,
		Integer = 1,
		Decimal = 2,
		Select = 3,
		Checkbox = 4,
		TextArea = 5
	}
	
	public class ParameterType : DictEntity<ParameterTypeEnum>
	{
		[MaxLength(64)]
		public string Name { get; set; } = default!;
	}
}