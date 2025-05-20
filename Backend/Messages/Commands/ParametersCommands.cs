using Dtos.Parameters;
using Models.Entities;

namespace Messages.Commands;

public record AddParameterOrder(string Name, bool Required, ParameterTypeEnum Type, int? ProductId, int? SubProductId, ICollection<OptionDto>? Options) : Command;
public record EditParameterOrder(int ParameterId, string Name, bool Required, ParameterTypeEnum Type, ICollection<OptionDto>? Options) : Command;
public record DeleteParameterOrder(int ParameterId) : Command;
public record AddOptionOrder(int ParameterId, string Value) : Command;
public record DeleteOptionOrder(int OptionId) : Command;