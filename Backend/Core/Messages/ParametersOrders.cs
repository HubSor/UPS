using Core.Dtos;
using Core.Models;
using Models.Entities;

namespace Core.Messages;

public record AddParameterOrder(string Name, bool Required, ParameterTypeEnum Type, int? ProductId, int? SubProductId, ICollection<OptionDto>? Options);
public record EditParameterOrder(int ParameterId, string Name, bool Required, ParameterTypeEnum Type, ICollection<OptionDto>? Options);
public record DeleteParameterOrder(int ParameterId);
public record AddOptionOrder(int ParameterId, string Value);
public record DeleteOptionOrder(int OptionId);