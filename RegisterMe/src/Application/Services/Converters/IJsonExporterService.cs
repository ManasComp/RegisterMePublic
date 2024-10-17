namespace RegisterMe.Application.Services.Converters;

public interface IJsonExporterService
{
    Task<String> GetDataAsync(int exhibitionId);
}
