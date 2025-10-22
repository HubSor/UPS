using Core.Dtos;
using Core.Models;

namespace Core.Messages;

public record AddParameterOrder(string Name, bool Required, ParameterTypeEnum Type, int? ProductId, int? SubProductId, ICollection<OptionDto>? Options) : BaseOrder();
public record EditParameterOrder(int ParameterId, string Name, bool Required, ParameterTypeEnum Type, ICollection<OptionDto>? Options) : BaseOrder();
public record DeleteParameterOrder(int ParameterId) : BaseOrder();
public record AddOptionOrder(int ParameterId, string Value) : BaseOrder();
public record DeleteOptionOrder(int OptionId) : BaseOrder();
public record GetSaleParametersOrder(int SaleId, int ProductId) : BaseOrder();
