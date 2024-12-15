#region

using System.IO.Compression;
using RegisterMe.Application.RegistrationToExhibition.Dtos;

#endregion

namespace RegisterMe.Application.Services.Converters;

public class ZipService : IZipService
{
    public async Task<Stream> CreateZipAsync(ICollection<Invoice> paths)
    {
        MemoryStream memoryStream = new();

        using (ZipArchive archive = new(memoryStream, ZipArchiveMode.Create, true))
        {
            foreach (Invoice path in paths)
            {
                ZipArchiveEntry entry = archive.CreateEntry(path.FileName, CompressionLevel.Fastest);
                await using Stream entryStream = entry.Open();
                await path.Stream.CopyToAsync(entryStream);
            }
        }

        memoryStream.Position = 0;
        return memoryStream;
    }
}
