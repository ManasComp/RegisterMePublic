#region

using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using RegisterMe.Application.Authorization;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.BackgroundWorkers;
using RegisterMe.Application.Cages;
using RegisterMe.Application.CatRegistrations;
using RegisterMe.Application.Common.Behaviours;
using RegisterMe.Application.Exhibitions;
using RegisterMe.Application.Exhibitors;
using RegisterMe.Application.Organizations;
using RegisterMe.Application.Pricing;
using RegisterMe.Application.RegistrationToExhibition;
using RegisterMe.Application.Services.Converters;
using RegisterMe.Application.Services.Groups;
using RegisterMe.Application.Services.Workflows;
using RegisterMe.Application.System;
using RegisterMe.Application.Users;
using SessionService = Stripe.Checkout.SessionService;
using Utils = RegisterMe.Application.Pricing.Utils;

#endregion

// do not move this to a different namespace
namespace RegisterMe.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddScoped<SessionService, SessionService>();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehaviour<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingPerformanceBehaviour<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(TransactionBehaviour<,>));
        });

        services.AddScoped<AuthorizationHelperMethods, AuthorizationHelperMethods>();

        services.AddScoped<IAuthorizationHandler, RegistrationToExhibitionAuthorizationCrudHandler>();
        services.AddScoped<IAuthorizationHandler, ExhibitionsAuthorizationCrudHandler>();
        services.AddScoped<IAuthorizationHandler, ExhibitorAuthorizationCrudHandler>();
        services.AddScoped<IAuthorizationHandler, OwnDataAuthorizationCrudHandler>();
        services.AddScoped<IAuthorizationHandler, OrganizationsAuthorizationCrudHandler>();
        services.AddScoped<IJsonExporterService, JsonExporterService>();
        services.AddScoped<NotifierService, NotifierService>();
        services.AddScoped<BackgroundTaskRunner, BackgroundTaskRunner>();

        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<GroupInitializer, GroupInitializer>();
        services.AddSingleton<GroupService, GroupService>();
        services.AddSingleton<Utils, Utils>();
        services.AddSingleton<TimedHostedService, TimedHostedService>();


        services.AddScoped<ICagesService, CagesService>();
        services.AddScoped<ICatRegistrationService, CatRegistrationService>();
        services.AddScoped<IExhibitionService, ExhibitionService>();
        services.AddScoped<IExhibitorService, ExhibitorService>();
        services.AddScoped<IOrganizationService, OrganizationService>();
        services.AddScoped<IRegistrationToExhibitionService, RegistrationToExhibitionService>();
        services.AddScoped<WorkflowService, WorkflowService>();
        services.AddScoped<IPricingFacade, PricingFacade>();
        services.AddScoped<IStripeInvoiceBuilder, StripeInvoiceBuilder>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ISystemService, SystemService>();
        services.AddScoped<IInvoiceCreator, InvoiceCreator>();
        services.AddScoped<IInvoiceDataProvider, InvoiceDataProvider>();
        services.AddScoped<StringInvoiceFormatter, StringInvoiceFormatter>();
        services.AddScoped<IWordService, WordService>();
        services.AddScoped<IInvoiceSenderService, InvoiceSenderService>();
        services.AddScoped<IZipService, ZipService>();
        services.AddScoped<CagesServiceHelper, CagesServiceHelper>();

        return services;
    }
}
