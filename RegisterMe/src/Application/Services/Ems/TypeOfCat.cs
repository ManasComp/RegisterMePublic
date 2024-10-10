namespace RegisterMe.Application.Services.Ems;

public class TypeOfCat(
    BreedDto breed,
    List<EmsCodePartPerCatTypeDto> zbarveniSrsti,
    List<EmsCodePartPerCatTypeDto> kodDepigmentaceSrsti,
    List<EmsCodePartPerCatTypeDto> kodyBileSkvrnitosti,
    List<EmsCodePartPerCatTypeDto> kodyStupneDepigmentace,
    List<EmsCodePartPerCatTypeDto> kodTypuKresbVSrsti,
    List<EmsCodePartPerCatTypeDto> kodSnizenePigmentace,
    List<EmsCodePartPerCatTypeDto> kodZkraceniOcasu,
    List<EmsCodePartPerCatTypeDto> kodZbarveniOci,
    List<EmsCodePartPerCatTypeDto> kodSrsti)
{
    public List<EmsCodePartPerCatTypeDto> KodSnizenePigmentace { get; } = kodSnizenePigmentace;
    public List<EmsCodePartPerCatTypeDto> KodZbarveniOci { get; } = kodZbarveniOci;
    public List<EmsCodePartPerCatTypeDto> KodSrsti { get; } = kodSrsti;
    public List<EmsCodePartPerCatTypeDto> KodZkraceniOcasu { get; } = kodZkraceniOcasu;

    public List<EmsCodePartPerCatTypeDto> KodDepigmentaceSrsti { get; } = kodDepigmentaceSrsti;
    public List<EmsCodePartPerCatTypeDto> KodTypuKresbVSrsti { get; } = kodTypuKresbVSrsti;
    public List<EmsCodePartPerCatTypeDto> KodyBileSkvrnitosti { get; } = kodyBileSkvrnitosti;
    public List<EmsCodePartPerCatTypeDto> KodyStupneDepigmentace { get; } = kodyStupneDepigmentace;
    public BreedDto Breed { get; } = breed;
    public List<EmsCodePartPerCatTypeDto> ZbarveniSrsti { get; } = zbarveniSrsti;
}
