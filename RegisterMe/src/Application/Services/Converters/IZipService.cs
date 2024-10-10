#region

using RegisterMe.Application.RegistrationToExhibition.Dtos;

#endregion

namespace RegisterMe.Application.Services.Converters;

public interface IZipService
{
    Task<Stream> CreateZipAsync(ICollection<Invoice> paths);
}
