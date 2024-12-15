namespace RegisterMe.Application.Services.Converters;

public interface IJsonExporterService
{
    Task<string> GetDataAsync(int exhibitionId);
}
