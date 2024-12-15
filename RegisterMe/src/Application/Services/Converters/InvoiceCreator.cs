#region

using RegisterMe.Application.Exhibitors;
using RegisterMe.Application.Exhibitors.Dtos;
using RegisterMe.Application.RegistrationToExhibition;
using RegisterMe.Application.RegistrationToExhibition.Dtos;

#endregion

namespace RegisterMe.Application.Services.Converters;

public class InvoiceCreator(
    IInvoiceDataProvider invoiceDataProvider,
    StringInvoiceFormatter formatter,
    IWordService wordService,
    IRegistrationToExhibitionService registrationToExhibitionService,
    IExhibitorService exhibitorService
) : IInvoiceCreator
{
    public async Task<HashSet<Invoice>> CreateRegistrationToExhibitionInvoice(int registrationToExhibitionId,
        string webUrl, string rootPath)
    {
        RegistrationToExhibitionDto registrationToExhibitionById =
            await registrationToExhibitionService.GetRegistrationToExhibitionById(registrationToExhibitionId);
        HashSet<Invoice> paths =
            await CreateRegistrationToExhibitionInvoice(registrationToExhibitionById, webUrl, rootPath);

        return paths;
    }

    public async Task<List<List<Invoice>>> CreateExhibitionInvoice(int exhibitionId,
        string webUrl, string rootPath)
    {
        List<RegistrationToExhibitionDto> registrationToExhibitions =
            await registrationToExhibitionService.GetRegistrationsToExhibitionByExhibitionId(exhibitionId);

        List<List<Invoice>> registrationExports = new();
        foreach (RegistrationToExhibitionDto registration in registrationToExhibitions)
        {
            HashSet<Invoice> registrationPaths =
                await CreateRegistrationToExhibitionInvoice(registration, webUrl, rootPath);

            string folderName = await GetFolderName(registration);
            List<Invoice> paths = registrationPaths.ToList()
                .Select(x => new Invoice(x.Stream, folderName + "/" + x.FileName))
                .ToList();
            registrationExports.Add(paths);
        }

        return registrationExports;
    }

    private async Task<HashSet<Invoice>> CreateRegistrationToExhibitionInvoice(
        RegistrationToExhibitionDto registrationToExhibitionById,
        string webUrl, string rootPath)
    {
        HashSet<Invoice> paths = [];
        foreach (int catRegistration in registrationToExhibitionById.CatRegistrationIds)
        {
            Invoice stream = await CreateCatRegistrationInvoice(catRegistration, webUrl, rootPath);
            stream.Stream.Position = 0;
            paths.Add(stream);
        }

        return paths;
    }

    private async Task<string> GetFolderName(RegistrationToExhibitionDto registrationToExhibition)
    {
        int exhibitorId = registrationToExhibition.ExhibitorId;
        ExhibitorAndUserDto exhibitor = await exhibitorService.GetExhibitorById(exhibitorId);
        string exhibitorName = exhibitor.FirstName + "-" + exhibitor.LastName + "-" + exhibitor.MemberNumber;

        return exhibitorName;
    }

    public async Task<Invoice> CreateCatRegistrationInvoice(int catRegistrationId, string webUrl, string path)
    {
        const string templateName = "catRegistrationTemplate.docx";
        string final = Path.Combine(path, "Data");
        if (!Directory.Exists(final))
        {
            final = Path.Combine(path, "wwwroot", "Data");
        }

        if (!Directory.Exists(final))
        {
            throw new DirectoryNotFoundException("Directory not found");
        }

        string[] inputFileArray = Directory.GetFiles(final, templateName, SearchOption.AllDirectories);
        string inputFile = inputFileArray[0];

        List<InvoiceModel> models = await invoiceDataProvider.GetInvoiceModel(catRegistrationId, webUrl);
        List<Invoice> streams = models.Select(model =>
                wordService.FillBookmarks(inputFile, $"vystavniFormular_{catRegistrationId}.docx", GetData(model)))
            .ToList();

        Invoice stream = wordService.CombineMultipleWordDocumentsIntoOne(streams);
        return stream;
    }

    private Dictionary<string, string> GetData(InvoiceModel invoiceModel)
    {
        Dictionary<string, string> bookmarks = new()
        {
            { "Url", formatter.Format(invoiceModel.Url) },
            { "Id", formatter.Format(invoiceModel.Id.ToString()) },
            { "NOfCatsAndLitters", formatter.Format(invoiceModel.NOfCatsAndLitters.ToString()) },
            { "TotalListsForCat", formatter.Format(invoiceModel.TotalListsForCat.ToString()) },
            { "ActualList", formatter.Format(invoiceModel.ActualList.ToString()) },
            { "DateSend", formatter.Format(invoiceModel.DateSend) },
            { "CatPrice", formatter.Format(invoiceModel.CatPrice, invoiceModel.Currency) },
            { "RegistrationPrice", formatter.Format(invoiceModel.RegistrationPrice, invoiceModel.Currency) },
            { "DatePaymentAccepted", formatter.Format(invoiceModel.DatePaymentAccepted) },
            { "PaymentType", formatter.Format(invoiceModel.PaymentType) },
            { "ExhibitorPrice", formatter.Format(invoiceModel.ExhibitorPrice, invoiceModel.Currency) },
            { "ExhibitionName", formatter.Format(invoiceModel.ExhibitionName) },
            { "OrganizationName", formatter.Format(invoiceModel.OrganizationName) },
            { "VisitedDays", formatter.Format(invoiceModel.VisitedDays) },
            { "Advertisement", formatter.Format(invoiceModel.Advertisement) },
            { "CschY", formatter.Format(invoiceModel.CschY) },
            { "CschN", formatter.Format(invoiceModel.CschN) },
            { "LitterY", formatter.Format(invoiceModel.LitterY) },
            { "LitterN", formatter.Format(invoiceModel.LitterN) },
            { "CatName", formatter.Format(invoiceModel.CatName) },
            { "CatEms", formatter.Format(invoiceModel.CatEms) },
            { "CatGroup", formatter.Format(invoiceModel.CatGroup) },
            {
                "CatColour", formatter.Format(invoiceModel.CatColour + " " +
                                              (invoiceModel.CatBreed ?? invoiceModel.NameOfBreedingStation))
            },
            { "CatPedigreeNumber", formatter.Format(invoiceModel.CatPedigreeNumber ?? invoiceModel.PassOfOrigin) },
            { "CatBorn", formatter.Format(formatter.FormatWithYear(invoiceModel.CatBorn)) },
            { "GenderM", formatter.Format(invoiceModel.GenderM) },
            { "GenderF", formatter.Format(invoiceModel.GenderF) },
            { "CastratedY", formatter.Format(invoiceModel.CastratedY) },
            { "CastratedN", formatter.Format(invoiceModel.CastratedN) },
            { "BreederFirstName", formatter.Format(invoiceModel.BreederFirstName) },
            { "BreederLastName", formatter.Format(invoiceModel.BreederLastName) },
            { "BreederCountry", formatter.Format(invoiceModel.BreederCountryName) },
            { "BSameAsEY", formatter.Format(invoiceModel.BreederSameAsExhibitorY) },
            { "BSameAsEN", formatter.Format(invoiceModel.BreederSameAsExhibitorN) },
            { "FatherName", formatter.Format(invoiceModel.FatherName) },
            { "FatherEms", formatter.Format(invoiceModel.FatherEms) },
            { "FatherColour", formatter.Format(invoiceModel.FatherColour) },
            { "FatherPedigreeNumber", formatter.Format(invoiceModel.FatherPedigreeNumber) },
            { "MotherName", formatter.Format(invoiceModel.MotherName) },
            { "MotherEms", formatter.Format(invoiceModel.MotherEms) },
            { "MotherColour", formatter.Format(invoiceModel.MotherColour) },
            { "MotherPedigreeNumber", formatter.Format(invoiceModel.MotherPedigreeNumber) },
            { "ExhibitorSurname", formatter.Format(invoiceModel.ExhibitorSurname) },
            { "ExhibitorFirstname", formatter.Format(invoiceModel.ExhibitorFirstname) },
            { "ExhibitorEmail", formatter.Format(invoiceModel.ExhibitorEmail) },
            { "ExhibitorPhoneNumber", formatter.Format(invoiceModel.ExhibitorPhoneNumber) },
            { "ExhibitorStreet", formatter.Format(invoiceModel.ExhibitorStreet) },
            { "ExhibitorHouse", formatter.Format(invoiceModel.ExhibitorHouse) },
            { "ExhibitorZIP", formatter.Format(invoiceModel.ExhibitorZip) },
            { "ExhibitorCountry", formatter.Format(invoiceModel.ExhibitorCountry) },
            { "ExhibitorCity", formatter.Format(invoiceModel.ExhibitorCity) },
            { "ExhibitorDateOfBirth", formatter.Format(formatter.FormatWithYear(invoiceModel.ExhibitorDateOfBirth)) },
            { "EOrganizationName", formatter.Format(invoiceModel.EOrganizationName) },
            { "EMemberNumber", formatter.Format(invoiceModel.EMemberNumber) },
            { "Note", formatter.Format(invoiceModel.Note) },
            { "One", formatter.Format(invoiceModel.One) },
            { "Two", formatter.Format(invoiceModel.Two) },
            { "Three", formatter.Format(invoiceModel.Three) },
            { "Four", formatter.Format(invoiceModel.Four) },
            { "Five", formatter.Format(invoiceModel.Five) },
            { "Six", formatter.Format(invoiceModel.Six) },
            { "Seven", formatter.Format(invoiceModel.Seven) },
            { "Eight", formatter.Format(invoiceModel.Eight) },
            { "Nine", formatter.Format(invoiceModel.Nine) },
            { "Ten", formatter.Format(invoiceModel.Ten) },
            { "Eleven", formatter.Format(invoiceModel.Eleven) },
            { "Twelve", formatter.Format(invoiceModel.Twelve) },
            { "ThirteenA", formatter.Format(invoiceModel.ThirteenA) },
            { "ThirteenB", formatter.Format(invoiceModel.ThirteenB) },
            { "ThirteenC", formatter.Format(invoiceModel.ThirteenC) },
            { "Fourteen", formatter.Format(invoiceModel.Fourteen) },
            { "Fifteen", formatter.Format(invoiceModel.Fifteen) },
            { "Sixteen", formatter.Format(invoiceModel.Sixteen) },
            { "Seventeen", formatter.Format(invoiceModel.Seventeen) },
            { "CatFeeOne", formatter.Format(invoiceModel.CatFeeOne) },
            { "CatFeeTwo", formatter.Format(invoiceModel.CatFeeTwo) },
            { "CatFeeThree", formatter.Format(invoiceModel.CatFeeThree) },
            { "CatFeeFour", formatter.Format(invoiceModel.CatFeeFour) },
            { "CatFeeFive", formatter.Format(invoiceModel.CatFeeFive) },
            { "CatFeeSix", formatter.Format(invoiceModel.CatFeeSix) },
            { "CatPriceOne", formatter.Format(invoiceModel.CatPriceOne, invoiceModel.Currency) },
            { "CatPriceTwo", formatter.Format(invoiceModel.CatPriceTwo, invoiceModel.Currency) },
            { "CatPriceThree", formatter.Format(invoiceModel.CatPriceThree, invoiceModel.Currency) },
            { "CatPriceFour", formatter.Format(invoiceModel.CatPriceFour, invoiceModel.Currency) },
            { "CatPriceFive", formatter.Format(invoiceModel.CatPriceFive, invoiceModel.Currency) },
            { "CatPriceSix", formatter.Format(invoiceModel.CatPriceSix, invoiceModel.Currency) },
            { "CageRentedY", formatter.Format(invoiceModel.CageRentedY) },
            { "CageRentedN", formatter.Format(invoiceModel.CageRentedN) },
            { "DoubleCageY", formatter.Format(invoiceModel.DoubleCageY) },
            { "DoubleCageN", formatter.Format(invoiceModel.DoubleCageN) },
            { "CageLength", formatter.Format(invoiceModel.CageLength.ToString()) },
            { "CageWidth", formatter.Format(invoiceModel.CageWidth.ToString()) },
            { "CageHeight", formatter.Format(invoiceModel.CageHeight.ToString()) },
            { "ReportGenerated", formatter.Format(invoiceModel.ReportGenerated) },
            { "AmountPaid", formatter.Format(invoiceModel.AmountPaid) },
            { "OrganizationEmail", formatter.Format(invoiceModel.EmailToOrganization) }
        };
        return bookmarks;
    }
}
