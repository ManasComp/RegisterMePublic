#region

using FluentAssertions;
using NUnit.Framework;
using RegisterMe.Application.Services.Ems;
using RegisterMe.Domain.Common;

#endregion

namespace RegisterMe.Application.UnitTests.Ems;

public class EmsCodeTest
{
    [Test]
    [TestCase("PER a", "Perská modrá")]
    [TestCase("PER ns 11", "Perská černá stříbřitě stínovaná")]
    [TestCase("PER d 03", "Perská červený bikolor")]
    [TestCase("PER g 03", "Perská modře želvovinový bikolor")]
    [TestCase("PER n 03 22", "Perská černá bikolor mramorovaná")]
    [TestCase("PER ns 01 21 62", "Perská černá stříbřitá van s kresbou a oranžovýma očima")]
    [TestCase("RAG n", "Ragdoll s červenohnědými odznaky")]
    [TestCase("RAG a", "Ragdoll s modrými odznaky")]
    [TestCase("RAG a 21", "Ragdoll s modrými žíhanými odznaky")]
    [TestCase("RAG n 03", "Ragdoll černý bikolor")]
    [TestCase("RAG a 03", "Ragdoll modrý bikolor")]
    [TestCase("RAG b 03", "Ragdoll čokoládovobílá bikolor")]
    [TestCase("RAG n 04", "Ragdoll černohnědý mitted")]
    [TestCase("RAG a 04", "Ragdoll modrý mitted")]
    [TestCase("RAG f 04", "Ragdoll černý želvovinový mitted")]
    [TestCase("RAG g 04", "Ragdoll modře želvovinový mitted")]
    [TestCase("RAG e 04 21", "Ragdoll krémový s odznaky s kresbou")]
    [TestCase("RAG g 04 21", "Ragdoll modrý želvovinový s odznaky a kresbou")]
    [TestCase("SBI n", "Birma červenohnědá")]
    [TestCase("SBI c", "Birma lilová")]
    [TestCase("SBI e", "Birma krémová")]
    [TestCase("SBI f", "Birma černě želvovinový")]
    [TestCase("SBI a 21", "Birma s modrými odznaky s kresbou")]
    [TestCase("SBI a 21", "Birma čokoládová želvovinová s nespecifckou kresbou")]
    [TestCase("BEN n 24", "Bengálská černá tečkovaná")]
    [TestCase("BLH b", "Britská dlouhosrstá čokoládová")]
    [TestCase("BLH c", "Britská dlouhosrstá lilová")]
    [TestCase("BLH j 02 62", "Britská dlouhosrstá lilová želvovinová harlekýn s oranžovýma očima")]
    [TestCase("BSH a", "Britská krátkosrstá modrá")]
    [TestCase("BSH b", "Britská krátkosrstá čokoládová")]
    [TestCase("BSH c", "Britská krátkosrstá lilová")]
    [TestCase("BSH e", "Britská krátkosrstá krémová")]
    [TestCase("BSH g", "Britská krátkosrstá modrá želvovinová")]
    [TestCase("BSH j", "Britská krátkosrstá lilově želvovinová")]
    [TestCase("BSH o", "Britská krátkosrstá skořicová")]
    [TestCase("BSH ns 11", "Britská krátkosrstá černá stříbřitá stínovaná")]
    [TestCase("BSH h 24", "Britská krátkosrstá čokoládová želvovinová tečkovaná")]
    [TestCase("BSH ns 22 64", "Britská krátkosrstá černá stříbřitá mramorovaná zelenooká")]
    [TestCase("BSH d 02 62", "Britská krátkosrstá červená harlekýn oranžovooká")]
    [TestCase("BSH e 02 62", "Britská krátkosrstá krémová harlekýn oranžovooká")]
    [TestCase("BSH b 03", "Britská krátkosrstá čokoládový bikolor")]
    [TestCase("BSH c 03", "Britská krátkosrstá lilová bikolor")]
    [TestCase("BSH f 03", "Britská krátkosrstá černá želvovinová bikolor")]
    [TestCase("BSH g 03", "Britská krátkosrstá modrá želvovinová bikolor")]
    [TestCase("BSH bs 02 62", "Britská krátkosrstá čokoládová stříbřitá harlekýn oranžovooká")]
    [TestCase("BSH b 03 22", "Britská krátkosrstá čokoládová bikolor mramorovaná")]
    [TestCase("BSH d 03 22", "Britská krátkosrstá červená bikolor mramorovaná")]
    [TestCase("BSH b 03 24", "Britská krátkosrstá čokoládová bikolor tečkovaná")]
    [TestCase("BSH d 03 24", "Britská krátkosrstá červená bikolor tečkovaná")]
    [TestCase("BSH ns 02 21 64", "Britská krátkosrstá černá stříbřitá harlekýn s kresbou zelenooká")]
    [TestCase("BSH ns 02 22 64", "Britská krátkosrstá černá stříbřitá harlekýn mramorovaná zelenooká")]
    [TestCase("BUR n", "Barmská černá")]
    [TestCase("BUR b", "Barmská čokoládová")]
    [TestCase("BUR f", "Barmská černá želvovinová")]
    [TestCase("OCI b 24", "Ocicat čokoládová tečkovaná")]
    [TestCase("ABY n", "Habešská divoce zbarevená")]
    [TestCase("THA n", "Thajská seal")]
    [TestCase("THA a", "Thajská modrá")]
    [TestCase("THA b", "Thajská čokoládová")]
    [TestCase("THA c", "Thajská lilová")]
    [TestCase("MCO ds 09 22", "Mainská mývalí červená stříbřitá mramorovaná s bílou")]
    [TestCase("MCO f 22", "Mainská mývalí želvovinová mramorovaná")]
    [TestCase("PEB f 03 24 83", "Peterbald černě želvovinový bikolor tečkovaný brush")]
    [TestCase("BSH f 22", "Britská krátkosrstá modrá")]
    [TestCase("BSH c", "Britská krátkosrstá lilla")]
    [TestCase("BSH g", "Britská krátkosrstá modroželvovinová")]
    [TestCase("BRI c", "Britská krátkosrstá lilla")]
    [TestCase("BRI g", "Britská krátkosrstá modroželvovinová")]
    [TestCase("MCO d 09 22", "Mainská mývalí červená mramorovaná s bílou")]
    [TestCase("MCO ns 22", "Mainská mývalí černá mramorovaná")]
    [TestCase("MCO d 09 22", "Mainská mývalí červená mramorovaná s bílou")]
    public void TestCorrectEms(string emsCode, string expectedRepresentation)
    {
        EmsCode controller = EmsCode.Create(emsCode).Value;
        List<string> parsed = controller.GetRepresentationsOfEms().Value;

        parsed.Should().Contain(expectedRepresentation);
    }

    [Test]
    [TestCase("PER g 01 21 62", "Perská modrá želvovinový van s nepecifikovanou kresnou oranžovooká")]
    [TestCase("BSH b 21 33", "Britská krátkosrstá s čokoládovými žíhanými odznaky")]
    [TestCase("DSP n 21 33 81", "donský sphynx s kresbou a černohnědými odznaky dlouhosrstý")]
    [TestCase("PEB n 33 83", "Peterbald s černohnědými odznaky brush")]
    public void TestInCorrectEms(string emsCode, string expectedRepresentation)
    {
        EmsCode controller = EmsCode.Create(emsCode).Value;
        List<string> parsed = controller.GetRepresentationsOfEms().Value;

        parsed.Should().NotContain(expectedRepresentation);
    }

    [Test]
    [TestCase("NFO at 22", "Norská lesní světlý amber mramorovaná")]
    [TestCase("BEN x nt 24", "Bengálská kočka charcoal černě tečkovaná (neuznaná varieta)")]
    [TestCase("XSH n (SPH)", "neuznaný bezsrstý černý (cílové plemeno: Sphynx)")]
    [TestCase("XSH n 82 (SPH)", "XSH n 82 neuznaný krátkosrstý černý (cílové plemeno: Sphynx)")]
    [TestCase("BXSH n (DRX)", "neuznaný krátkosrstý černý (cílové plemeno: Devon Rex)")]
    [TestCase("XSH n 84 (DRX)", "neuznanýs krátkou rovnou srstí černý(cílové plemeno: Devon Rex)")]
    [TestCase("DSP x f 03 24 82", "donský sphynx černě želvovinový bikolor tečkovaný krátkosrstý")]
    [TestCase("DSP x n 33 83", "donský sphynx s černohnědými odznaky brush")]
    [TestCase("DSP x f 03 21 83", "donský sphynx černě želvovinový bikolor s kresbou brush")]
    [TestCase("PEB x n 33 81", "Peterbald s černohnědými odznaky dlouhosrstý")]
    [TestCase("PEB x f 03 24 82", "Peterbald černě želvovinový bikolor tečkovaný krátkosrstý")]
    public void TestWrongEms(string emsCode, string expectedRepresentation)
    {
        EmsCode controller = EmsCode.Create(emsCode).Value;
        EmsResult<List<string>> result = controller.GetRepresentationsOfEms();

        result.IsFailure.Should().BeTrue();
    }

    [Test]
    [TestCase(" ")]
    [TestCase("")]
    public void TestWrongEmsController(string emsCode)
    {
        Result<EmsCode> controller = EmsCode.Create(emsCode);
        controller.IsFailure.Should().BeTrue();
    }
}
