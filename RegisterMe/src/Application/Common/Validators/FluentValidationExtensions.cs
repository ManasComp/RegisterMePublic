namespace RegisterMe.Application.Common.Validators;

public static class FluentValidationExtensions
{
    public static IRuleBuilderOptions<T, int> ForeignKeyValidator<T>(this IRuleBuilder<T, int> ruleBuilder)
    {
        return ruleBuilder.GreaterThan(0);
    }

    public static IRuleBuilderOptions<T, string> ForeignKeyValidator<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.NotEmpty();
    }

    public static IRuleBuilderOptions<T, int?> OptionalForeignKeyValidator<T>(this IRuleBuilder<T, int?> ruleBuilder)
    {
        return ruleBuilder.Must(id => id is null or > 0).WithMessage("If provided, the ID must be greater than 0");
    }

    public static IRuleBuilderOptions<T, string?> OptionalForeignKeyValidator<T>(
        this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder.Must(id => string.IsNullOrEmpty(id) || !string.IsNullOrWhiteSpace(id))
            .WithMessage("If provided, the ID must not be empty or whitespace.");
    }

    public static IRuleBuilderOptionsConditions<T, T> ValidPagination<T>(this IRuleBuilderInitial<T, T> ruleBuilder,
        Func<T, int> pageNumberSelector, Func<T, int> pageSizeSelector, int maxPageSize = 100, int maxPageNumber = 100)
    {
        IRuleBuilderOptionsConditions<T, T>? result = ruleBuilder.Custom((entity, context) =>
        {
            int pageNumber = pageNumberSelector(entity);
            int pageSize = pageSizeSelector(entity);

            if (IsValidPagination(pageNumber, pageSize))
            {
                return;
            }

            context.AddFailure("PageNumber and PageSize validation failed");
            context.AddFailure("PageNumber", $"Page number must be greater than 0 and less than {maxPageNumber}");
            context.AddFailure("PageSize",
                $"Page size must be greater than 0 and less than or equal to {maxPageSize}");
        });
        return result;

        bool IsValidPagination(int pageNumber, int pageSize)
        {
            return pageNumber > 0 && pageSize > 0 && pageSize <= maxPageSize && pageNumber < maxPageNumber;
        }
    }
}
