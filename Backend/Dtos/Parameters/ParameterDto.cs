using Models.Entities;

namespace Dtos.Parameters
{
	public class ParameterDto 
	{
		public int Id { get; set; }
		public string Name { get; set; } = default!;
		public ParameterTypeEnum Type { get; set; }
		public bool Required { get; set; }
		public ICollection<ExtendedOptionDto> Options { get; set; } = new List<ExtendedOptionDto>();
		
		public ParameterDto(Parameter param)
		{
			Id = param.Id;
			Name = param.Name;
			Type = param.Type;
			Required = param.Required;
			Options = param.Options.Select(o => new ExtendedOptionDto() 
			{
				Id = o.Id,
				Value = o.Value,
				ParameterId = param.Id
			}).ToList();
		}
	}
}
