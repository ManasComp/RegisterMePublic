#region

using RegisterMe.Application.Services.Ems;
using RegisterMe.Application.System.Queries.GetAllSupportedCats;
using RegisterMe.Domain.Common;

#endregion

namespace RegisterMe.Application.System.Queries.ParseEms;

// ReSharper disable always UnusedType.Global
public record ParseEmsQuery : IRequest<Result>
{
    public required string Ems { get; set; }
    public required string Breed { get; set; }
    public required string Colour { get; set; }
}

public class
    ParseEmsQueryValidator : AbstractValidator<
    GetAllSupportedCatsQuery>
{
}

public class ParseEmsQueryQueryHandler : IRequestHandler<ParseEmsQuery, Result>
{
    public Task<Result> Handle(ParseEmsQuery request, CancellationToken cancellationToken)
    {
        // anyone can check what cats can be registered
        if (string.IsNullOrEmpty(request.Ems))
        {
            return Task.FromResult(Result.Failure(new Error("EmsCode", "Ems kód je ve špatném formátu")));
        }

        Result<EmsCode> emsValidator = EmsCode.Create(request.Ems);
        if (emsValidator.IsFailure)
        {
            return Task.FromResult(Result.Failure(new Error("EmsCode", "Ems kód je ve špatném formátu")));
        }

        if (!emsValidator.Value.CanBeParsed())
        {
            return Task.FromResult(Result.Failure(new Error("EmsCode", "Ems kód je ve špatném formátu")));
        }

        if (!emsValidator.Value.VerifyEmsCode(request.Breed, request.Colour))
        {
            return Task.FromResult(Result.Failure(new Error("EmsCode", "Ems kód neodpovídá zadaným parametrům")));
        }

        return Task.FromResult(Result.Success());
    }
}
