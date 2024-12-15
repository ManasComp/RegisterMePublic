#region

using System.Globalization;
using FluentValidation;
using RegisterMe.Application;
using RegisterMe.Application.BackgroundWorkers;
using RegisterMe.Infrastructure;
using RegisterMe.Infrastructure.Data;
using RegisterMe.Infrastructure.Middleware;
using Serilog;
using Stripe;
using WebApi;
using WebApi.Infrastructure;
using WebApi.Services;
using IUser = RegisterMe.Application.Common.Interfaces.IUser;

#endregion

const string myAllowSpecificOrigins = "_myAllowSpecificOrigins";

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddInfrastructureServices(builder);
builder.Services.AddApplicationServices();
builder.Services.AddScoped<IUser, CurrentUser>();
builder.Services.AddCors(options =>
{
    options.AddPolicy(myAllowSpecificOrigins,
        policy =>
        {
            policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
        });
});


builder.Services.AddWebServices();

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
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCookiePolicy();
app.UseExceptionHandler(_ => { });

app.UseMiddleware<RequestLogContextMiddleware>();

app.UseCors(myAllowSpecificOrigins);
app.UseSerilogRequestLogging();

app.UseHealthChecks("/health");

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseSwaggerUi(settings =>
{
    settings.Path = "/api";
    settings.DocumentPath = "/api/specification.json";
});
app.MapFallbackToFile("index.html");
app.Map("/", () => Results.Redirect("/api"));

app.MapEndpoints();

StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();
CultureInfo.CurrentUICulture = new CultureInfo("cs-CZ");
ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo("cs-CZ");
app.Run();

namespace WebApi
{
    public class Program
    {
    }
}
