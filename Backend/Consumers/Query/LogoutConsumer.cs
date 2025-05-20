using Core;
using MassTransit;
using Messages.Queries;
using Messages.Responses;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Consumers.Query;
public class LogoutConsumer : BaseQueryConsumer<LogoutQuery, LogoutResponse>
{
	private readonly IHttpContextAccessor httpContextAccessor;
	public LogoutConsumer(IHttpContextAccessor httpContextAccessor, ILogger<LogoutConsumer> logger)
		: base(logger)
	{
		this.httpContextAccessor = httpContextAccessor;
	}

	public override async Task Consume(ConsumeContext<LogoutQuery> context)
	{
		if (httpContextAccessor.HttpContext == null)
			throw new UPSException("No HttpContext on logout");
			
		await httpContextAccessor.HttpContext!.SignOutAsync();
		await RespondAsync(context, new LogoutResponse());
		logger.LogInformation("Logged out");
	}
}
