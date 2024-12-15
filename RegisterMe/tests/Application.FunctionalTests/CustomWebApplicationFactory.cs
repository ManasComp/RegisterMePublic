#region

using System.Data.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Interfaces;
using RegisterMe.Application.Pricing;
using RegisterMe.Domain.Entities;
using RegisterMe.Infrastructure.Data;
using Stripe.Checkout;
using WebApi;

#endregion

namespace RegisterMe.Application.FunctionalTests;

#region

using static Testing;

#endregion

public class CustomWebApplicationFactory(DbConnection connection) : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services
                .RemoveAll<IUser>()
                .AddTransient(_ =>
                {
                    Mock<IUser> mockUser = new();
                    mockUser.Setup(s => s.Id).Returns(GetUserId());
                    mockUser.Setup(s => s.User).Returns(GetClaimsPrincipal);
                    return mockUser.Object;
                });

            services
                .RemoveAll<DbContextOptions<ApplicationDbContext>>()
                .AddDbContext<ApplicationDbContext>((sp, options) =>
                {
                    options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
                    options.UseNpgsql(connection);
                });

            services.RemoveAll<IEmailSender>()
                .AddTransient<IEmailSender, FakeEmailSender>();

            services.RemoveAll<IEmailSender<ApplicationUser>>()
                .AddTransient<IEmailSender, FakeEmailSender>();

            services.RemoveAll<IStripeInvoiceBuilder>()
                .AddTransient<IStripeInvoiceBuilder, FakeStripeInvoiceBuilder>();

            services.RemoveAll<SessionService>()
                .AddTransient<SessionService, FakeSessionService>();
        });
    }
}
