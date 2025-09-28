using Core;
using Core.Messages;
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using WebCommons;

namespace UsersMicro.Consumers;

public class LogoutConsumer : BaseConsumer<LogoutOrder, LogoutResponse>
{
	private readonly IHttpContextAccessor httpContextAccessor;
	public LogoutConsumer(IHttpContextAccessor httpContextAccessor, ILogger<LogoutConsumer> logger)
		: base(logger)
	{
		this.httpContextAccessor = httpContextAccessor;
	}

	public override async Task Consume(ConsumeContext<LogoutOrder> context)
	{
		if (httpContextAccessor.HttpContext == null)
			throw new UPSException("No HttpContext on logout");
			
		await httpContextAccessor.HttpContext!.SignOutAsync();
		await RespondAsync(context, new LogoutResponse());
		logger.LogInformation("Logged out");
	}
}
