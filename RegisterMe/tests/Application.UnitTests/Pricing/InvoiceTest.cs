#region

using System.Reflection;
using Ardalis.GuardClauses;
using FluentAssertions;
using NUnit.Framework;
using RegisterMe.Application.RegistrationToExhibition.Dtos;
using RegisterMe.Application.Services.Converters;

#endregion

namespace RegisterMe.Application.UnitTests.Pricing;

[TestFixture]
public class BookmarkServiceTests
{
    [Test]
    public async Task FillBookmarks_Test()
    {
        InvoiceCreator invoiceCreator =
            new(new InvoiceDataProviderMock(), new StringInvoiceFormatter(), new WordService(),
                new RegistrationToExhibitionMock(), new ExhibitorServiceMock());

        string? assemblyDir = Assembly.GetAssembly(typeof(BookmarkServiceTests))?.Location;
        string[]? directories = assemblyDir?.Split(Path.DirectorySeparatorChar);
        Guard.Against.Null(directories, nameof(directories));
        List<string> toJoin = [];

        foreach (string split in directories)
        {
            toJoin.Add(split);
            if (split == "Application.UnitTests")
            {
                break;
            }
        }

        string path = Path.Combine(Path.Combine(toJoin.ToArray()), "Pricing");
        path = Path.DirectorySeparatorChar + path;

        Invoice invoices = await invoiceCreator.CreateCatRegistrationInvoice(1, "http://localhost:5000", path);
        invoices.Should().NotBeNull();
    }
}
