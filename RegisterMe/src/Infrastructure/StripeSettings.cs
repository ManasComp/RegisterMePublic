namespace RegisterMe.Infrastructure;

public class StripeSettings
{
    public string SecretKey { get; init; } = null!;
    public string PublishableKey { get; init; } = null!;
}
