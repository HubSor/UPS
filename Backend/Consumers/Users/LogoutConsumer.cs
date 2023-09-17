using Core;
using MassTransit;
using Messages.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Services;

namespace Consumers.Products;
public class LogoutConsumer : BaseConsumer<LogoutOrder, LogoutResponse>
{
	private IHttpContextAccessor httpContextAccessor;
	public LogoutConsumer(IHttpContextAccessor httpContextAccessor, ILogger<LogoutConsumer> logger, IPasswordService passwordService)
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
	}
}
