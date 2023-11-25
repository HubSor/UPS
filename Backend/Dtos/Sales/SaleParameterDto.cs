using Dtos.Parameters;
using Models.Entities;

namespace Dtos.Sales;
public class SaleParameterDto : ParameterDto
{
	public SaleParameterDto(Parameter param, string? answer) : base(param)
	{
		Answer = answer;
	}
	
	public string? Answer { get; set; }
}