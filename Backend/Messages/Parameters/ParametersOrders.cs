using Dtos.Parameters;
using Models.Entities;

namespace Messages.Parameters;

public record AddParameterOrder(string Name, bool Required, ParameterTypeEnum Type, int? ProductId, int? SubProductId, ICollection<OptionDto>? Options);
public record EditParameterOrder(int ParameterId, string Name, bool Required, ParameterTypeEnum Type);
public record DeleteParameterOrder(int ParameterId);
public record AddOptionOrder(int ParameterId, string Value);
public record DeleteOptionOrder(int OptionId);