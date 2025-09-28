using Core.Models;

namespace Core.Dtos;

public class SaleParameterDto : ParameterDto
{
	public SaleParameterDto(Parameter param, string? answer) : base(param)
	{
		Answer = answer;
	}
	
	public string? Answer { get; set; }
}