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
        if (IsNotCommand())
        {
            return await next();
        }

        using TransactionScope transactionScope = new(TransactionScopeAsyncFlowOption.Enabled);
        TResponse response = await next();

        transactionScope.Complete();

        return response;
    }

    private bool IsNotCommand()
    {
        return !typeof(TRequest).Name.EndsWith("Command");
    }
}
