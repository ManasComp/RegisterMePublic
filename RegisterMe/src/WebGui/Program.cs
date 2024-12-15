#region

using System.Globalization;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegisterMe.Application;
using RegisterMe.Application.BackgroundWorkers;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Infrastructure;
using RegisterMe.Infrastructure.Data;
using RegisterMe.Infrastructure.Middleware;
using Serilog;
using Stripe;
using WebGui.Areas.Visitor.Controllers.ViewModelServices;
using WebGui.Localization;
using WebGui.Services;
using XLocalizer;
using XLocalizer.Translate.MyMemoryTranslate;

#endregion


WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder);
builder.Services.AddScoped<IUser, CurrentUser>();
builder.Services.AddScoped<IMemoryDataService, MemoryDataService>();
builder.Services.AddScoped<IMultipleStepFormService, MultipleStepFormServices>();

LocalizationModel localization = Localization.Localize();

builder.Services.AddControllersWithViews(options =>
    {
        options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
    })
    .AddXLocalizer<LocalSource, MyMemoryTranslateService>(ops =>
    {
        ops.ValidationErrors = localization.ValidationErrors;
        ops.ModelBindingErrors = localization.ModelBindingErrors;
        ops.IdentityErrors = localization.IdentityErrors;
    });

builder.Services.AddRazorPages()
    .AddXLocalizer<LocalSource, MyMemoryTranslateService>(ops =>
    {
        ops.ValidationErrors = localization.ValidationErrors;
        ops.ModelBindingErrors = localization.ModelBindingErrors;
        ops.IdentityErrors = localization.IdentityErrors;
    });

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(b =>
    {
        b.WithOrigins("http://localhost", "https://localhost")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = _ => true;
    options.MinimumSameSitePolicy = SameSiteMode.Unspecified; // Because of Stripe and GDPR
    options.Secure = builder.Environment.IsDevelopment()
        ? CookieSecurePolicy.None
        : // Because of Stripe
        CookieSecurePolicy.SameAsRequest;
    options.ConsentCookieValue = "true";
});

builder.Services.AddAuthorizationBuilder()
    .SetFallbackPolicy(new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build());

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDatabaseDeveloperPageExceptionFilter();
}

WebApplication app = builder.Build();

app.Use(async (context, next) =>
{
    if (!GetWebAddress.WasInitialized)
    {
        HttpRequest request = context.Request;
        Uri location = new($"{request.Scheme}://{request.Host}");
        GetWebAddress.WebAddress = location.AbsoluteUri;

        using IServiceScope scope = app.Services.CreateScope();
        TimedHostedService scopedProcessingService =
            scope.ServiceProvider
                .GetRequiredService<TimedHostedService>();

        await scopedProcessingService.StartAsync(GetWebAddress.WebAddress, CancellationToken.None);
    }

    await next();
});

await app.InitialiseDatabaseAsync();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/visitor/home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCookiePolicy();

app.UseMiddleware<RequestLogContextMiddleware>();

app.UseSerilogRequestLogging();

app.Use(async (context, next) =>
{
    await next();
    if (context.Response.StatusCode == 404)
    {
        context.Request.Path = "/visitor/home/PageNotFound";
        context.Response.StatusCode = 200;
        await next();
    }
});

app.UseRouting();
app.UseAuthentication();
app.UseCors();

app.UseRequestLocalization();
app.UseAuthorization();

app.MapControllerRoute(
    "default",
    "{area=Visitor}/{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();
CultureInfo.CurrentUICulture = new CultureInfo("cs-CZ");
ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo("cs-CZ");

app.Run();
