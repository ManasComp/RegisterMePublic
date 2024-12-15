namespace RegisterMe.Infrastructure.Exceptions;

public class MigrationException : Exception
{
    public MigrationException(string eMessage) : base(eMessage)
    {
    }

    public MigrationException(string eMessage, Exception innerException) : base(eMessage, innerException)
    {
    }
}
