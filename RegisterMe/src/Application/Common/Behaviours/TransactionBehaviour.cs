#region

using System.Transactions;

#endregion

namespace RegisterMe.Application.Common.Behaviours;

public sealed class TransactionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // https://www.cockroachlabs.com/blog/sql-isolation-levels-explained/
        IsolationLevel isolationLevel = IsNotCommand() ? IsolationLevel.RepeatableRead : IsolationLevel.Serializable;

        using TransactionScope transactionScope = new(TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = isolationLevel },
            TransactionScopeAsyncFlowOption.Enabled);

        TResponse response = await next();

        transactionScope.Complete();

        return response;
    }

    private bool IsNotCommand()
    {
        return !typeof(TRequest).Name.EndsWith("Command");
    }
}
