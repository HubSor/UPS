using Core;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;

namespace Services.Application;

public abstract class BaseApplicationService(ILogger<BaseApplicationService> logger)
{
    public virtual ApiResponse<TResponse> PerformRequest<TRequest, TResponse>(
        Func<TRequest, TResponse> func,
        TRequest request,
        IEnumerable<IValidator<TRequest>> validators
    ) where TResponse : class
    {
        var errors = ValidateRequest(validators, request);
        if (errors.Any())
        {
            return new ApiResponse<TResponse>(errors);
        }

        try
        {
            var result = func(request);
            return new ApiResponse<TResponse>(result);
        }
        catch (ValidationException vex)
        {
            return HandleValidationException<TResponse>(vex);
        }
        catch (Exception)
        {
            return HandleOtherException<TResponse>();
        }
    }

    public async virtual Task<ApiResponse<TResponse>> PerformRequestAsync<TRequest, TResponse>(
        Func<TRequest, Task<TResponse>> func,
        TRequest request,
        IEnumerable<IValidator<TRequest>> validators
    ) where TResponse : class
    {
        var errors = ValidateRequest(validators, request);
        if (errors.Any())
        {
            return new ApiResponse<TResponse>(errors);
        }

        try
        {
            var result = await Task.Run(() => func(request));
            return new ApiResponse<TResponse>(result);
        }
        catch (ValidationException vex)
        {
            return HandleValidationException<TResponse>(vex);
        }
        catch (Exception e)
        {
            return HandleOtherException<TResponse>(e);
        }
    }

    private ICollection<ValidationFailure> ValidateRequest<TRequest>(IEnumerable<IValidator<TRequest>> validators, TRequest request)
    {
        var errors = new List<ValidationFailure>();
        var validatorsList = validators.ToList();
        if (!validatorsList.Any())
            return errors;

        foreach (var validator in validatorsList)
        {
            var validationResult = validator.Validate(request);
            if (validationResult.IsValid)
                continue;

            errors.AddRange(validationResult.Errors);
            logger.LogInformation("Validator {Validator} detected validation errors:\n\t{ValidationErrors}",
                validator.GetType().Name, string.Join("\n\t", errors.ToList()));
        }

        return errors;
    }

    private ApiResponse<TResponse> HandleValidationException<TResponse>(ValidationException vex)
        where TResponse : class
    {
        logger.LogInformation("Validation errors while performing request: \n\t {ValidationErrors}",
            string.Join("\n\t", vex.Errors.ToList()));

        return new ApiResponse<TResponse>(vex.Errors);
    }

    private ApiResponse<TResponse> HandleOtherException<TResponse>(Exception? ex = null)
        where TResponse : class
    {
        logger.LogError(ex, "Unhandled error while performing request");

        return new ApiResponse<TResponse>(new ValidationFailure("_", "Coś poszło nie tak"));
    }

}