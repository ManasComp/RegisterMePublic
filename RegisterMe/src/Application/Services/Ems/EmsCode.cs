#region

using RegisterMe.Application.Common.Exceptions;
using RegisterMe.Domain.Common;

#endregion

namespace RegisterMe.Application.Services.Ems;

public record ParsedEms(string Breed, string Variant);

public class EmsCode(string[] splitEmsCode)
{
    private const string Separator = " ";
    private TypeOfCat _kocka = null!;
    private List<string> _kodDepigmentaceSrsti = [];

    private List<string> _kodSnizenePigmentac = [];
    private List<string> _kodSrsti = [];
    private List<string> _kodTypuKresbVSrsti = [];
    private List<string> _kodyBileSkrrvrnitosti = [];
    private List<string> _kodyStupnedepigmentace = [];
    private List<string> _kodZbarveniOci = [];
    private List<string> _kodZkraceniOcasu = [];

    private List<string> _plemeno = [];
    private int _positionInCode;
    private bool _run;
    private List<string> _zbarveniSrsti = [];

    public static Result<EmsCode> Create(string emsCode)
    {
        if (string.IsNullOrEmpty(emsCode))
        {
            return Result.Failure<EmsCode>(Errors.EmptyEmsKodError);
        }

        if (emsCode.Trim() != emsCode)
        {
            return Result.Failure<EmsCode>(Errors.TrailingSpacesEmsCodeError);
        }

        string[] emsCodeSplit = emsCode.Split(Separator);
        if (emsCodeSplit.Length < 1)
        {
            return Result.Failure<EmsCode>(Errors.WrongEmsCodeError);
        }

        return Result.Success(new EmsCode(emsCodeSplit));
    }


    public ParsedEms GetEms()
    {
        string variant = string.Join(Separator, splitEmsCode[new Range(1, splitEmsCode.Length)]);
        return new ParsedEms(splitEmsCode[0], variant);
    }

    public bool VerifyEmsCode(string breed, string? colour)
    {
        bool canBeParsed = CanBeParsed();
        if (!canBeParsed)
        {
            return false;
        }

        EmsResult<List<string>> representationsOfEms = GetRepresentationsOfEms();
        if (representationsOfEms.IsFailure)
        {
            return false;
        }

        if (string.IsNullOrEmpty(colour))
        {
            return representationsOfEms.Value.Contains(breed);
        }

        return representationsOfEms.Value.Contains(breed + Separator + colour);
    }

    public static bool RequiresGroup(string ems)
    {
        string[] emsCodeSplited = ems.Split(Separator);
        if (emsCodeSplited.Length == 0)
        {
            return false;
        }

        IEnumerable<TypeOfCat> x = EmsInitializer.Initialize();

        bool isRequired = x.Where(typeOfCat => typeOfCat.Breed.RequiresGroup)
            .Select(code => code.Breed.Attribute.Code).Any(breed =>
                emsCodeSplited[0].Contains(breed, StringComparison.CurrentCultureIgnoreCase));
        return isRequired;
    }

    public EmsResult<List<string>> GetRepresentationsOfEms()
    {
        EmsResult parse = Parse();
        if (Parse().IsFailure)
        {
            return EmsResult.Failure<List<string>>(parse.Error);
        }

        List<List<string>> result =
        [
            _plemeno,
            _zbarveniSrsti,
            _kodDepigmentaceSrsti,
            _kodyBileSkrrvrnitosti,
            _kodyStupnedepigmentace,
            _kodTypuKresbVSrsti,
            _kodSnizenePigmentac,
            _kodZkraceniOcasu,
            _kodZbarveniOci,
            _kodSrsti
        ];

        List<string> output = [""];
        foreach (List<string> list in result)
        {
            List<string> old = output;
            output = [];

            foreach (string s in old)
            foreach (string s1 in list)
            {
                if (s.Length == 0 || s1.Length == 0)
                {
                    output.Add(s + s1);
                }
                else
                {
                    output.Add(s + Separator + s1);
                }
            }

            if (output.Count == 0)
            {
                output = old;
            }
        }

        return EmsResult.Success(output);
    }

    public bool CanBeParsed()
    {
        return Parse().IsValid;
    }


    public EmsResult Parse()
    {
        if (_run)
        {
            bool differentLength1 = _positionInCode != splitEmsCode.Length;
            return differentLength1
                ? EmsResult.Failure(Errors.WrongEmsCodeError)
                : EmsResult.Success();
        }

        try
        {
            EmsResult x1 = ParsePlemeno();
            if (x1.IsFailure)
            {
                return x1;
            }

            EmsResult x2 = ParseZbarveniSrsti();
            if (x2.IsFailure)
            {
                return x2;
            }

            EmsResult x3 = BilaSkvrnitost();
            if (x3.IsFailure)
            {
                return x3;
            }

            EmsResult x4 = StupenDepigmentace();
            if (x4.IsFailure)
            {
                return x4;
            }

            EmsResult x5 = TypKresby();
            if (x5.IsFailure)
            {
                return x5;
            }

            EmsResult x6 = SnizenaPigmentace();
            if (x6.IsFailure)
            {
                return x6;
            }

            EmsResult x7 = ZkraceniOcastu();
            if (x7.IsFailure)
            {
                return x7;
            }

            EmsResult x8 = ZbarveniOci();
            if (x8.IsFailure)
            {
                return x8;
            }

            EmsResult x9 = KodSrsti();
            if (x9.IsFailure)
            {
                return x9;
            }
        }
        catch (Exception)
        {
            return EmsResult.Failure(Errors.WrongEmsCodeError);
        }

        _run = true;
        bool differentLength = _positionInCode != splitEmsCode.Length;
        return differentLength
            ? EmsResult.Failure(Errors.WrongEmsCodeError)
            : EmsResult.Success();
    }

    private EmsResult<string> GetCodeSegment()
    {
        if (_positionInCode >= splitEmsCode.Length)
        {
            return "";
        }

        string zbarv = splitEmsCode[_positionInCode];
        if (zbarv != zbarv.Trim() || string.IsNullOrEmpty(zbarv.Trim()))
        {
            return EmsResult.Failure<string>(Errors.WrongEmsCodeError);
        }

        return zbarv;
    }

    /*
     * a
     */
    private EmsResult ParsePlemeno()
    {
        EmsResult<string> breed = GetCodeSegment();
        IEnumerable<TypeOfCat> x = EmsInitializer.Initialize();
        TypeOfCat? cat = x.FirstOrDefault(typeOfCat => typeOfCat.Breed.Attribute.Code == breed.Value);

        if (cat == null)
        {
            return EmsResult.FatalFailure(Errors.InvalidEmsCodePlemenoError);
        }

        _kocka = cat;
        _positionInCode++;
        _plemeno = _kocka.Breed.Attribute.PotentialCodePartInCzech;
        return _kocka.Breed.Status == Status.Error
            ? EmsResult.Failure(Errors.InvalidEmsCodePlemenoError)
            : EmsResult.Success();
    }

    /**
     * b, c
     */
    private EmsResult ParseZbarveniSrsti()
    {
        EmsResult<string> zbarv = GetCodeSegment();
        if (zbarv.IsFailure)
        {
            return zbarv;
        }

        if (zbarv.Value.Length == 0)
        {
            return EmsResult.Success();
        }

        EmsCodePartPerCatTypeDto? x =
            _kocka.ZbarveniSrsti.FirstOrDefault(x => x.Attribute.Code == zbarv.Value[0].ToString());
        if (x == null)
        {
            return EmsResult.Success();
        }

        _positionInCode++;
        _zbarveniSrsti = x.Attribute.PotentialCodePartInCzech;
        if (x.Status == Status.Error)
        {
            return EmsResult.Failure(Errors.InvalidEmsCodeZbarveniSrstiError);
        }

        if (zbarv.Value.Length != 2)
        {
            return zbarv.Value.Length == 1
                ? EmsResult.Success()
                : EmsResult.Failure(Errors.InvalidEmsCodeZbarveniSrstiError);
        }

        {
            EmsCodePartPerCatTypeDto? x1 =
                _kocka.KodDepigmentaceSrsti.FirstOrDefault(emsCodePartPerCatTypeDto =>
                    emsCodePartPerCatTypeDto.Attribute.Code == zbarv.Value[1].ToString());
            if (x1 == null)
            {
                return EmsResult.Failure(Errors.InvalidEmsCodeZbarveniSrstiError);
            }

            _kodDepigmentaceSrsti = x1.Attribute.PotentialCodePartInCzech;
            return x.Status == Status.Error
                ? EmsResult.Failure(Errors.InvalidEmsCodeZbarveniSrstiError)
                : EmsResult.Success();
        }
    }


    /*
     * e
     */
    private EmsResult BilaSkvrnitost()
    {
        EmsResult<string> zbarv = GetCodeSegment();
        if (zbarv.IsFailure)
        {
            return zbarv;
        }

        EmsCodePartPerCatTypeDto? x = _kocka.KodyBileSkvrnitosti.FirstOrDefault(x => x.Attribute.Code == zbarv.Value);
        if (x == null)
        {
            return EmsResult.Success();
        }

        _positionInCode++;
        _kodyBileSkrrvrnitosti = x.Attribute.PotentialCodePartInCzech;
        return x.Status == Status.Error
            ? EmsResult.Failure(Errors.InvalidEmsCodeBilaSkvrnitostError)
            : EmsResult.Success();
    }

    /*
     * f
     */
    private EmsResult StupenDepigmentace()
    {
        EmsResult<string> zbarv = GetCodeSegment();
        if (zbarv.IsFailure)
        {
            return zbarv;
        }

        EmsCodePartPerCatTypeDto?
            x = _kocka.KodyStupneDepigmentace.FirstOrDefault(x => x.Attribute.Code == zbarv.Value);
        if (x == null)
        {
            return EmsResult.Success();
        }

        _positionInCode++;
        _kodyStupnedepigmentace = x.Attribute.PotentialCodePartInCzech;
        return x.Status == Status.Error
            ? EmsResult.Failure(Errors.InvalidEmsCodeStypenDepigmentaceError)
            : EmsResult.Success();
    }

    /**
     * g
     */
    private EmsResult TypKresby()
    {
        EmsResult<string> zbarv = GetCodeSegment();
        if (zbarv.IsFailure)
        {
            return zbarv;
        }

        EmsCodePartPerCatTypeDto? x = _kocka.KodTypuKresbVSrsti.FirstOrDefault(x => x.Attribute.Code == zbarv.Value);
        if (x == null)
        {
            return EmsResult.Success();
        }

        _positionInCode++;
        _kodTypuKresbVSrsti = x.Attribute.PotentialCodePartInCzech;
        return x.Status == Status.Error
            ? EmsResult.Failure(Errors.InvalidEmsCodeTypKresbyError)
            : EmsResult.Success();
    }


    /**
* h
*/
    private EmsResult SnizenaPigmentace()
    {
        EmsResult<string> zbarv = GetCodeSegment();
        if (zbarv.IsFailure)
        {
            return zbarv;
        }

        EmsCodePartPerCatTypeDto? x = _kocka.KodSnizenePigmentace.FirstOrDefault(x => x.Attribute.Code == zbarv.Value);
        if (x == null)
        {
            return EmsResult.Success();
        }

        _positionInCode++;
        _kodSnizenePigmentac = x.Attribute.PotentialCodePartInCzech;
        return x.Status == Status.Error
            ? EmsResult.Failure(Errors.InvalidEmsCodeZSnizenaPigmentaceError)
            : EmsResult.Success();
    }

    /**
 * j
 */
    private EmsResult ZkraceniOcastu()
    {
        EmsResult<string> zbarv = GetCodeSegment();
        if (zbarv.IsFailure)
        {
            return zbarv;
        }

        EmsCodePartPerCatTypeDto? x = _kocka.KodZkraceniOcasu.FirstOrDefault(x => x.Attribute.Code == zbarv.Value);
        if (x == null)
        {
            return EmsResult.Success();
        }

        _positionInCode++;
        _kodZkraceniOcasu = x.Attribute.PotentialCodePartInCzech;
        return x.Status == Status.Error
            ? EmsResult.Failure(Errors.InvalidEmsCodeZkraceníOcasuError)
            : EmsResult.Success();
    }

    /**
* k
*/
    private EmsResult ZbarveniOci()
    {
        EmsResult<string> zbarv = GetCodeSegment();
        if (zbarv.IsFailure)
        {
            return zbarv;
        }

        EmsCodePartPerCatTypeDto? x = _kocka.KodZbarveniOci.FirstOrDefault(x => x.Attribute.Code == zbarv.Value);
        if (x == null)
        {
            return EmsResult.Success();
        }

        _positionInCode++;
        _kodZbarveniOci = x.Attribute.PotentialCodePartInCzech;
        return x.Status == Status.Error
            ? EmsResult.Failure(Errors.InvalidEmsCodeZbaveniuOciError)
            : EmsResult.Success();
    }

    private EmsResult KodSrsti()
    {
        EmsResult<string> zbarv = GetCodeSegment();
        if (zbarv.IsFailure)
        {
            return zbarv;
        }

        EmsCodePartPerCatTypeDto? x = _kocka.KodSrsti.FirstOrDefault(x => x.Attribute.Code == zbarv.Value);
        if (x == null)
        {
            return EmsResult.Success();
        }

        _positionInCode++;
        _kodSrsti = x.Attribute.PotentialCodePartInCzech;
        return x.Status == Status.Error
            ? EmsResult.Failure(Errors.InvalidEmsCodeKodSrstioError)
            : EmsResult.Success();
    }
}
