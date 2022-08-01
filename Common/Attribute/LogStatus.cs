using Microsoft.AspNetCore.Mvc.Filters;

[AttributeUsage(AttributeTargets.All, Inherited = false)]
public class LogStatusAttribute : Attribute, IActionFilter
  {
    private readonly ILogger<LogStatusAttribute> _logger;
    public LogStatusAttribute(ILogger<LogStatusAttribute> logger)
    {
      _logger = logger;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
      _logger.LogInformation("Request: ",context.HttpContext.Request);

    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
      _logger.LogInformation("Response: ",context.HttpContext.Response);
    }
  }