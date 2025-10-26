using System.Net;
using Core;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace UPS.Filters;

public class ExceptionFilter : IExceptionFilter
{
	private readonly ILogger<ExceptionFilter> _logger;
	private readonly IHostEnvironment _hostEnvironment;
	public ExceptionFilter(IHostEnvironment hostEnvironment, ILogger<ExceptionFilter> logger)
	{
		_logger = logger;
		_hostEnvironment = hostEnvironment;
	}
	public void OnException(ExceptionContext context)
	{
		if (context.Exception is ValidationException validationException)
		{
			context.Result = new ObjectResult(new ApiResponse<object>(validationException.Errors))
			{
				StatusCode = (int)HttpStatusCode.BadRequest
			};
		}
		else
		{
			_logger.LogError(context.Exception, "Http request failed");
			context.Result = new ContentResult
			{
				Content = _hostEnvironment.IsDevelopment() ? context.Exception.ToString() : "Błąd serwera",
				StatusCode = (int)HttpStatusCode.InternalServerError
			};
		}
	}
}