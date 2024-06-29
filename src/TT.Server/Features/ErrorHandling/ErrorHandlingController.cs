using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace TT.Server.Features.ErrorHandling;

public record ErrorDetail(string RequestId, string Message, string InnerException, string StackTrace);

[AllowAnonymous]
public class ErrorHandlingController : Controller
{
    [HttpGet("/error")]
    [HttpPost("/error")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        var error = HttpContext.Features.Get<IExceptionHandlerPathFeature>().Error;
        var model = new ErrorDetail(
            Activity.Current?.Id ?? HttpContext.TraceIdentifier,
            error.Message,
            error.InnerException?.Message,
            error.StackTrace
        );

        return View(model);
    }
}