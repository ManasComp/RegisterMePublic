namespace RegisterMe.Application.Exceptions;

public class InvalidDatabaseStateException : Exception
{
    public InvalidDatabaseStateException()
    {
    }

    public InvalidDatabaseStateException(string message) : base(message)
    {
    }
}
