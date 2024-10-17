#region

using System.Diagnostics;
using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RegisterMe.Application.Common.Exceptions;
using RegisterMe.Application.Exceptions;
using WebGui.Areas.Visitor.Models;
using ValidationException = FluentValidation.ValidationException;

#endregion

namespace WebGui.Areas.Visitor.Controllers;

[Area(Areas.Visitor)]
[AllowAnonymous]
public class HomeController(IConfiguration configuration) : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        string? superAdministratorMail = configuration.GetValue<string>("Email:Mail");
        Guard.Against.NullOrEmpty(superAdministratorMail, nameof(superAdministratorMail));
        return View(new EmailModel { Email = superAdministratorMail });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        IExceptionHandlerPathFeature? exceptionHandlerPathFeature =
            HttpContext.Features.Get<IExceptionHandlerPathFeature>();

        string exceptionMessage = exceptionHandlerPathFeature?.Error switch
        {
            ValidationException => "Nastal problém s validací.",
            NotFoundException => "Záznam nebyl nalezen.",
            UnauthorizedAccessException or ForbiddenAccessException => "Přístup byl odepřen.",
            InvalidDatabaseStateException => "Naše chyba, kontaktujte nás prosím.",
            _ => "Nastala neočekávaná chyba. Kontaktujte nás prosím."
        };

        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, ExceptionMessage = exceptionMessage
        });
    }

    public IActionResult PageNotFound()
    {
        return View();
    }
}
