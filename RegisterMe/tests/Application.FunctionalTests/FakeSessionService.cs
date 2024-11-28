#region

using Stripe;
using Stripe.Checkout;

#endregion

namespace RegisterMe.Application.FunctionalTests;

public class FakeSessionService : SessionService
{
    public override Session Create(SessionCreateOptions options, RequestOptions? requestOptions = null)
    {
        Session session = new() { Id = "id" };
        return session;
    }

    public override Task<Session> CreateAsync(SessionCreateOptions options, RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new Session { Id = "id" });
    }

    public override Task<Session?> GetAsync(string id, SessionGetOptions? options = null,
        RequestOptions? requestOptions = null, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new Session { Id = "id", PaymentStatus = "paid", PaymentIntentId = "paymentinentId" })!;
    }
}
