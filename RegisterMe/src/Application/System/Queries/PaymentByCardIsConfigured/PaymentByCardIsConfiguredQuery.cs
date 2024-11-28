#region

using Microsoft.Extensions.Configuration;

#endregion

namespace RegisterMe.Application.System.Queries.PaymentByCardIsConfigured;

// ReSharper disable always UnusedType.Global
public record PaymentByCardIsConfiguredQuery : IRequest<bool>
{
}

public class
    PaymentByCardIsConfiguredQueryValidator : AbstractValidator<
    PaymentByCardIsConfiguredQuery>
{
}

public class PaymentByCardIsConfiguredQueryHandler(
    IConfiguration configuration,
    ISystemService systemService) : IRequestHandler<PaymentByCardIsConfiguredQuery, bool>
{
    public Task<bool> Handle(
        PaymentByCardIsConfiguredQuery request, CancellationToken cancellationToken)
    {
        // anyone can check if payment by card is configured
        return Task.FromResult(systemService.PaymentByCardIsConfigured(configuration));
    }
}
