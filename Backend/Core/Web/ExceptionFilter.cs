using System.Net;
using Core;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Core.Web;

public class ExceptionFilter(IHostEnvironment hostEnvironment, ILogger<ExceptionFilter> logger) : IExceptionFilter
{
    public void OnException(ExceptionContext context)
	{
		if (context.Exception is MassTransit.RequestException requestException && requestException.InnerException is ValidationException validationException)
		{
			context.Result = new ObjectResult(new ApiResponse<object>(validationException.Errors))
			{
				StatusCode = (int)HttpStatusCode.BadRequest
			};
		}
		else
		{
			logger.LogError(context.Exception, "Http request failed");
			context.Result = new ContentResult
			{
				Content = hostEnvironment.IsDevelopment() ? context.Exception.ToString() : "Błąd serwera",
				StatusCode = (int)HttpStatusCode.InternalServerError
			};
		}
	}
}