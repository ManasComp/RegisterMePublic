#region

using RegisterMe.Application.RegistrationToExhibition.Dtos;

#endregion

namespace RegisterMe.Application.Services.Converters;

public interface IWordService
{
    Invoice FillBookmarks(string templateFilePath, string finalName, Dictionary<string, string> bookmarks,
        int fontSizeVal = 15);

    Invoice CombineMultipleWordDocumentsIntoOne(List<Invoice> documentPaths);
}
