#region

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using RegisterMe.Application.RegistrationToExhibition.Dtos;

#endregion

namespace RegisterMe.Application.Services.Converters;

public class WordService : IWordService
{
    public Invoice FillBookmarks(string templateFilePath, string finalName, Dictionary<string, string> bookmarks,
        int fontSizeVal = 15)
    {
        MemoryStream memoryStream = new();
        using (FileStream file = new(templateFilePath, FileMode.Open))
        {
            file.CopyTo(memoryStream);
        }

        memoryStream.Position = 0;

        using WordprocessingDocument wordDoc = WordprocessingDocument.Open(memoryStream, true);
        Document? doc = wordDoc.MainDocumentPart?.Document;
        Guard.Against.Null(doc, nameof(doc));

        HashSet<BookmarkStart>? allBookmarks = doc.Body?.Descendants<BookmarkStart>().ToHashSet();
        Guard.Against.Null(allBookmarks, nameof(allBookmarks));

        foreach (KeyValuePair<string, string> bookmark in bookmarks)
        {
            BookmarkStart? bookmarksStart = allBookmarks.SingleOrDefault(b => b.Name == bookmark.Key);
            if (bookmarksStart == null)
            {
                continue;
            }

            OpenXmlElement? parent = bookmarksStart.Parent;
            Guard.Against.Null(parent, nameof(parent));

            RemoveBookmarkContent(bookmarksStart);

            Run run = new(new Text(bookmark.Value));

            RunProperties runProperties = new();
            FontSize fontSize = new() { Val = fontSizeVal.ToString() };
            runProperties.Append(fontSize);
            run.PrependChild(runProperties);

            parent.InsertAfter(run, bookmarksStart);
        }

        doc.Save();
        memoryStream.Position = 0;
        return new Invoice(memoryStream, finalName);
    }

    public Invoice CombineMultipleWordDocumentsIntoOne(List<Invoice> documentPaths)
    {
        if (documentPaths.Count == 0)
        {
            throw new InvalidOperationException("No documents to combine.");
        }

        if (documentPaths.Select(x => x.FileName).Distinct().Count() != 1)
        {
            throw new InvalidOperationException("All documents must have the same file name.");
        }

        Stream output = new MemoryStream();
        foreach (Invoice path in documentPaths)
        {
            path.Stream.Position = 0;
        }

        documentPaths[0].Stream.CopyTo(output);

        using WordprocessingDocument targetDoc = WordprocessingDocument.Open(output, true);
        MainDocumentPart? mainPart = targetDoc.MainDocumentPart;

        for (int i = 1; i < documentPaths.Count; i++)
        {
            string altChunkId = "AltChunkId" + i;
            AlternativeFormatImportPart? chunk = mainPart?.AddAlternativeFormatImportPart(
                AlternativeFormatImportPartType.WordprocessingML, altChunkId);

            chunk?.FeedData(documentPaths[i].Stream);

            AltChunk altChunk = new() { Id = altChunkId };
            mainPart?.Document.Body?.AppendChild(altChunk);
        }

        mainPart?.Document.Save();

        output.Position = 0;
        foreach (Invoice path in documentPaths)
        {
            path.Stream.Position = 0;
        }

        return new Invoice(output, documentPaths[0].FileName);
    }

    private void RemoveBookmarkContent(BookmarkStart bookmarkStart)
    {
        OpenXmlElement? parent = bookmarkStart.Parent;
        BookmarkEnd? bookmarkEnd = parent?.Elements<BookmarkEnd>().Single(b => b.Id == bookmarkStart.Id);

        OpenXmlElement? element = bookmarkStart.NextSibling();
        while (element != null && element != bookmarkEnd)
        {
            OpenXmlElement? nextElement = element.NextSibling();
            element.Remove();
            element = nextElement;
        }
    }
}
