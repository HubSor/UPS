namespace Dtos.Parameters
{
	public class OptionDto
	{
		public string Value { get; set; } = default!;
	}

	public class ExtendedOptionDto : OptionDto
	{
		public int Id { get; set; }
		public int ParameterId { get; set; }
	}
}
