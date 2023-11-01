using Dtos.Parameters;
using Models.Entities;

namespace Messages.Parameters;

public record AddParameterOrder(string Name, bool Required, ParameterTypeEnum Type, int? ProductId, int? SubProductId, ICollection<OptionDto>? Options);