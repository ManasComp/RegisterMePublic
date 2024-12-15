// ReSharper disable InvalidXmlDocComment

namespace RegisterMe.Application.Services.Ems;

// Source: https://www.google.com/url?sa=t&source=web&rct=j&opi=89978449&url=http://home.tiscali.cz/chsk.peli/ems-%2520kody.doc&ved=2ahUKEwi17aD2oeuIAxXL0wIHHbTsCs4QFnoECAkQAQ&usg=AOvVaw3aakHgu60HLHmzJYBSXdtv

public static class EmsInitializer
{
// "code": "BOM",
// "code": "LYO",


    private static IEnumerable<TypeOfCat>? InitializedDataTypes;

    public static IEnumerable<TypeOfCat> Initialize()
    {
        return InitializedDataTypes ??= GetData();
    }

    private static IEnumerable<TypeOfCat> GetData()
    {
        /**
         * b
         * Jedno malé písmeno kódu zbarvení srsti
         * Kód zbarvení srsti se neuvádí u plemen uznaných pouze v jediném zbarvení srsti, např. Ruská modrá [RUS] a Kartouzská kočka [CHA], povinný je naopak všude tam, kde slouží k rozlišení barevných variet, např. Britská modrá [BRI a]. Obecně rozeznáváme zbarvení:
            1.	10 odstínů základního zbarvení: [kódy n, a, b, c, d, e, o, p]
            2.	 6 odstínů želvovinového zbarvení [kódy f, g, h, j, q,r]
            3.	 1 zbarvení bílé [kód w]
            Konkrétní odstín zbarvení kočky se u daného plemene může lišit zejména vlivem stupně depigmentace: kupř. u siamské kočky se zbarvení černé mění na zbarvení s tmavohnědými odznaky (seal-point), u barmské kočky se zbarvení černé mění na zbarvení hnědé atd. Kódy zbarvení srsti jsou uvedeny v tabulce 3.
            Bílé zbarvení je u kočky zbarvením dominantním, které navíc působí epistaticky, takže maskuje projev jakéhokoliv pigmentu v srsti. Naopak kočka s bílou skvrnitostí má vždy v srstním pokryvu alespoň malou část plně pigmentovanou.
             Písmenem „x“ se nahrazuje písmeno označující konkrétní zbarvení, nelze-li zbarvení zjistit. V případě, že pro dané plemeno není konkrétní zbarvení srsti povoleno, před kód zbarvení srsti se někdy předsazuje malé písmeno „x“.
         */
        EmsCodePartDto n = new()
        {
            Code = "n",
            PotentialCodePartInCzech =
            [
                "černá",
                "černý",
                "základní neředěná",
                "seal",
                "ruddy",
                "s červenohnědými odznaky",
                "černohnědý",
                "červenohnědá",
                "divoce zbarevená"
            ]
        };
        EmsCodePartDto a = new()
        {
            Code = "a",
            PotentialCodePartInCzech =
            [
                "modrá",
                "modře ředěná",
                "modrý",
                "modře ředěný",
                "s modrými odznaky",
                "s modrými",
                "čokoládová želvovinová"
            ]
        };
        EmsCodePartDto b = new()
        {
            Code = "b",
            PotentialCodePartInCzech =
            [
                "čokoládová",
                "čokoládové ředěná",
                "čokoládový",
                "čokoládově ředěný",
                "čokoládovobílá",
                "s čokoládovými"
            ]
        };
        EmsCodePartDto c = new()
        {
            Code = "c", PotentialCodePartInCzech = ["lilová", "souběh modrého a čokoládového ředění", "lilla"]
        };
        EmsCodePartDto d = new() { Code = "d", PotentialCodePartInCzech = ["červená", "červený"] };
        EmsCodePartDto e = new() { Code = "e", PotentialCodePartInCzech = ["krémová", "krémový", "krémový"] };
        EmsCodePartDto f = new()
        {
            Code = "f",
            PotentialCodePartInCzech =
            [
                "želvovinová",
                "černě želvovinová",
                "želvovinový",
                "černě želvovinový",
                "černý želvovinový",
                "černá želvovinová",
                "modrá"
            ]
        };
        EmsCodePartDto g = new()
        {
            Code = "g",
            PotentialCodePartInCzech =
            [
                "modře želvovinová",
                "modrokrémová",
                "modře želvovinový",
                "modrý želvovinový",
                "modrá",
                "krémový",
                "modrá želvovinová",
                "modroželvovinová"
            ]
        };
        EmsCodePartDto h = new()
        {
            Code = "h",
            PotentialCodePartInCzech =
                ["čokoládově želvovinová", "čokoládově želvovinový", "čokoládová želvovinová"]
        };
        EmsCodePartDto j = new()
        {
            Code = "j",
            PotentialCodePartInCzech =
                ["lilově želvovinová", "lilově želvovinový", "lilová želvovinová"]
        };
        EmsCodePartDto o = new()
        {
            Code = "o",
            PotentialCodePartInCzech =
            [
                "skořicová",
                " červenohnědá",
                "sorrel",
                "cinnamon",
                "skořicový, červenohnědý"
            ]
        };
        EmsCodePartDto p = new() { Code = "p", PotentialCodePartInCzech = ["plavá", " fawn"] };
        EmsCodePartDto q = new()
        {
            Code = "q", PotentialCodePartInCzech = ["skořicově želvovinová", "skořicově želvovinový"]
        };
        EmsCodePartDto r = new() { Code = "r", PotentialCodePartInCzech = ["plavě želvovinová", "plavě želvovinový"] };
        EmsCodePartDto w = new() { Code = "w", PotentialCodePartInCzech = ["bílá", "dominantně bílá", "bílý"] };
        EmsCodePartDto x = new()
        {
            Code = "x", PotentialCodePartInCzech = ["neuznané", "neidentifikovatelné zbarvení"]
        };
        EmsCodePartDto vaar = new()
        {
            Code = "var",
            PotentialCodePartInCzech = ["označení krátkosrsté kočky nesoucí genetickou vlohu pro dlouhosrstost"]
        };
        List<EmsCodePartDto> zbaerveniSrsti =
        [
            n,
            a,
            b,
            c,
            e,
            d,
            e,
            f,
            g,
            h,
            j,
            o,
            p,
            q,
            r,
            w,
            x,
            vaar
        ];

        /*
         c
         * Jedno malé doplňkové písmeno kódu depigmentace srsti (tipingu)
         * Kód depigmentace srsti (tipingu) se uvádí pouze tam, kde má smysl. Obecně rozeznáváme depigmentaci srsti v odstínu:
           1.	stříbřitém [doplňkový kód s]
           2.	zlatém [doplňkový kód y]
           Kódy depigmentace srsti jsou uvedeny v tabulce 4A. Není-li doplňkový kód konkretizován dvojčíslím kódu depigmentace srsti, jde o tiping kouřový. Například: „BRI ns“= britská černá stříbřitě kouřová, „BRI ns 12“= Britská černá stříbřitě stínovaná.
         */
        EmsCodePartDto s = new()
        {
            Code = "s", PotentialCodePartInCzech = ["stříbřitém", "stříbřitě", "stříbřitá", ""]
        };
        EmsCodePartDto y = new() { Code = "y", PotentialCodePartInCzech = ["zlatém"] };

        List<EmsCodePartDto> kodDepigmentaceSrsti = [s, y];


        /**
         * Dvojčíslí skupinových kódů exteriérových znaků:
         * Dvojčíslí skupinových kódů se uvádějí tam, kde jsou pro úplnou identifikaci kočky zapotřebí. Řadí se vzestupně a oddělují mezerou. Podrobněji definují další exteriérové vlastnosti, např. bílou skvrnitost (tabulka 4B.), stupeň depigmentace (tabulka 4C.), typ kresby v srsti (tabulka 4D.), snížení intenzity pigmentace srsti (tabulka 4E.), stupeň zkrácení ocasu (tabulka 4F.) a zbarvení očí (tabulka 4G.).
         */

        /**
         *e
         *
         * Kódy bílé skvrnitosti: 01, 02, 03, 04, 09
         * Kódy bílé skvrnitosti vyjadřují počet, velikost a umístění bílých skvrn v srsti. Rozeznáváme:
        1.	bílou skvrnitost typu „van“ [kód 01]
        2.	bílou skvrnitost typu „harlekýn“ [kód 02]
        3.	bílou skvrnitost typu „bikolor“ [kód 03]
        4.	bílou skvrnitost typu „mitted“ [kód 04]
        5.	bílou skvrnitost nespecifikovanou [kód 09]
        Typy bílé skvrnitosti 1., 2. a 3. se v zásadě liší počtem, t.j. velikostí a rozsahem bílých skvrn a zařazení kočky do příslušného typu se řídí standardem plemene konkrétní kočky.
        Typ bílé skvrnitosti 4. označuje bílé skvrny, jejichž výskyt je typický pro specielní plemena, např. Ragdol [RAG] nebo Birma [SBI[. Bílá skvrnitost typu 4. je obvykle vázána na umístění bílých skvrn v přesně určených, t.zv. predilekčních místech povrchu těla, např. ve formě bílých „ponožek“ na tlapkách.
        Pro bílé skvrny, umístěné na krku, t.zv. „medailónek“ není kód stanoven, protože takto umístěné bílé skvrny jsou zpravidla pokládány za vadu, naopak bílé skvrny na bradě u koček t.zv. „divokého zbarvení“ jsou obvykle tolerovány, protože jde o typický plemenný znak.
        Typ bílé skvrnitosti 5. označuje nespecifikovanou bílou skvrnitost u plemen, u nichž je povolena bez omezení, např. kočka mainská mývalí [MCO], norská lesní [NFO] a sibiřská {SIB].
Vždy platí, že dvojčíslí kódu bílé skvrnitosti se uvádí pouze tehdy, neplyne-li přímo ze standardu plemene, proto se např. neuvádí u birmy [SBI].

         */
        EmsCodePartDto s_01 = new() { Code = "01", PotentialCodePartInCzech = ["van"] };
        EmsCodePartDto s_02 = new() { Code = "02", PotentialCodePartInCzech = ["harlekýn"] };
        EmsCodePartDto s_03 = new() { Code = "03", PotentialCodePartInCzech = ["bikolor"] };
        EmsCodePartDto s_04 = new() { Code = "04", PotentialCodePartInCzech = ["mitted", "s odznaky"] };
        EmsCodePartDto s_09 = new()
        {
            Code = "09", PotentialCodePartInCzech = ["bílou skvrnitost nespecifikovanou", ""]
        };
        List<EmsCodePartDto> kodyBileSkrrvrnitosti =
        [
            s_01,
            s_02,
            s_03,
            s_04,
            s_09
        ];

        /*
         * Kódy stupně depigmentace srsti (tipingu): 11, 12
         * Kódy označující stupeň depigmentace (tipingu) jsou vázány na plemena, u nich je uplatněn doplňkový kód označující stříbřitý nebo zlatý tiping (viz poznámka c.).
           Depigmentaci rozeznáváme :
           1.	stínovaná (shaded) [11], nižší stupeň depigmentace, při němž je jednotlivý chlup zbaven pigmentu zhruba v 1/3 své délky od kořínku
           2.	závojová (shell) [12], vyšší stupeň depigmentace, při němž je jednotlivých chlup zbaven pigmentu zhruba v 7/8 své délky od kořínku, takže zbarven je pouze koneček chlupu. Depigmentace se mění podle tělesných partií a podle kresby v srsti. U závojové kočky depigmentací vzniká tak velký kontrast, že se černě pigmentovaná kočka může jevit jako bílá.
           Depigmentace se kromě toho menší měrou projevuje u zvířat s kresbou a zvířat jednobarevných bez kresby.

         */
        EmsCodePartDto x_11 = new() { Code = "11", PotentialCodePartInCzech = ["stínovaná"] };
        EmsCodePartDto x_12 = new() { Code = "12", PotentialCodePartInCzech = ["závojová"] };
        List<EmsCodePartDto> kodyStupnedepigmentace = [x_11, x_12];


        /*
         g

         Kódy typu kresby v srsti: 21, 22, 23, 24, 25
         * Kódy kresby v srsti vyjadřují konkrétní typ kresby v srsti kočky. Rozlišuje se:
           1.	srst zcela bez kresby (non- aguti), bez kódu
           2.	srst s kresbou nespecifikovanou [kód 21], obecný kód pro jakoukoli kresbu, kterou nelze přesně identifikovat pro nejasnost či neúplnost a pro typické žíhání siamské kočky
           3.	srst s kresbou mramorovanou (blotched) [kód 22]
           4.	srst s kresbou tygrovanou (mackerel) [kód 23]
           5.	srst s kresbou tečkovanou (spotted) [kód 24]
           6.	srst s tikingem (ticked tabby) [kód 25]. Srst s tikingem je současně  projevem kresby typickým pro habešské kočky a současně projevem zonálního zbarvení chlupu. Chlupy jsou po své délce proužkovány střídáním silněji a slaběji pigmentovaných zón.
           U červených a krémových odstínů zbarvení srsti bývá často problematické jednoznačně určit, zda kočka má či nemá v srsti kresbu správně a plně vyjádřenu, proto ani kód EMS zde nemusí vždy být plně směrodatný.

         */
        EmsCodePartDto x_21 = new()
        {
            Code = "21",
            PotentialCodePartInCzech =
            [
                "srst s nespecifikovanou kresbou nebo se žíháním",
                "žíhovanými odznaky",
                "žíhanými odznaky",
                "s nepecifikovanou kresnou",
                "s nespecifckou kresbou",
                "s kresbou",
                "a kresbou"
            ]
        };
        EmsCodePartDto x_22 = new()
        {
            Code = "22", PotentialCodePartInCzech = ["mramorovaná", "mramorovaná s bílou", ""]
        };
        EmsCodePartDto x_23 = new() { Code = "23", PotentialCodePartInCzech = ["tygrovaná kresby v srsti (mackerel)"] };
        EmsCodePartDto x_24 = new()
        {
            Code = "24",
            PotentialCodePartInCzech =
                ["tečkovaná kresba v srsti (spotted)", "tečkovaný", "tečkovaná"]
        };
        EmsCodePartDto x_25 = new()
        {
            Code = "25", PotentialCodePartInCzech = ["kresba (a zbarvení) s tikingem (ticked tabby)"]
        };
        List<EmsCodePartDto> kodTypuKresbVSrsti =
        [
            x_21,
            x_22,
            x_23,
            x_24,
            x_25
        ];

        /**
         * h
         * * Kódy snížené intenzity pigmentace srsti: 31, 32, 33¨
         * Kód snížené intenzity pigmentace je charakteristický pro plemennou skupinu barmských [kód 31] a siamských [kód 33[ koček, pro birmy [kód 33] a pro neuznané tonkinské kočky [kód 32]. Rozlišují se tři stupně zesvětlení:
1.	barmské zesvětlení- depigmentace (burmese) [kód 31] způsobuje mírné zesvětlení barevných odstínů, z nichž nejmarkantnější je zesvětlení z černé na (barmsky) hnědou
2.	tonkinská depigmentace (tonkinese, tonkanese) [kód 32] způsobuje střední zesvětlení barevných odstínů, kompromisně mezi typem 1 a 3
3.	siamská depigmentace (siamese)- zbarvení s odznaky- [kód 33] způsobuje silné zesvětlení barevných odstínů, při čemž výrazně zbarveny zůstávají pouze koncové části těla, t.zv. „odznaky (points)“, kdežto ostatní plochy srsti jsou ledově bílé. Siamské zesvětlení je typickým projevem akromelanismu.
Kód se uvádí pouze u těch plemen, kde jej jej třeba k úplné identifikaci, zásadně se vypouští u kódového označení koček barmských, siamských, balinéských a u birem, kde je pokládán za samozřejmý

         */
        EmsCodePartDto x_31 = new() { Code = "31", PotentialCodePartInCzech = ["barmské zesvětlení"] };
        EmsCodePartDto x_32 = new() { Code = "32", PotentialCodePartInCzech = ["tonkinská depigmentac"] };
        EmsCodePartDto x_33 = new() { Code = "33", PotentialCodePartInCzech = ["siamská depigmentace"] };
        List<EmsCodePartDto> kodSnizenePigmentace = [x_31, x_32, x_33];


        /**
         * Kódy stupně zkrácení ocasu (u Manxe): 51, 52, 53, 54
         * Kód stupně zkrácení ocasu je vyhrazen do plemeno Manx [MAN]. Pro ostatní plemena včetně Japonského bobtaila [JBT] je bez významu. U plemene Manx se často vypouští kód skutečného zbarvení, protože zbarvení srsti je pokládáno na bezvýznamné.
         */
        /*
         Tři velká písmena kódu plemene
         Označení plemene, pokud je známo, je povinné. K označení neurčeného plemene se obecně užívá znaku „X“, k označení neuznaného (neurčeného) dlouhosrstého nebo polodlouhosrstého plemene se užije kód „XLH“, k označení neuznaného (neurčeného) krátkosrstého plemene se užije kód „XSH“. Alternativní označení „non-LH“ a „non-SH“ se nepoužívá. Rozlišují se:
           1.	plemena koček ve FIFe uznávaná
           2.	plemena koček ve FIFe (dosud anebo natrvalo) neuznávaná
           Kódy uznaných i neuznaných plemen koček jsou uvedeny v Registračním řádu FIFe (vydáno v češtině). Kódy plemen koček jsou uvedeny v tabulce 2.
         */
        EmsCodePartDto x_51 = new() { Code = "31", PotentialCodePartInCzech = ["Manx rumpy- bezocasý"] };
        EmsCodePartDto x_52 = new()
        {
            Code = "32", PotentialCodePartInCzech = ["Manx rumpy riser- nepatrně prodloužená páteř"]
        };
        EmsCodePartDto x_53 = new() { Code = "33", PotentialCodePartInCzech = ["Manx stumpy- ocasní pahýl"] };
        EmsCodePartDto x_54 = new() { Code = "33", PotentialCodePartInCzech = ["Manx longie- normální ocas"] };
        List<EmsCodePartDto> kodZkraceniOcasu = [x_51, x_52, x_53, x_54];


        /**
         * Kódy zbarvení očí: 61, 62, 63, 64, 65, 66, 67
         *
         * Kódy zbarvení očí se uvádějí pouze u těch plemen koček, u nichž jsou ve vazbě na zbarvení srsti povoleny různé barvy očí. U plemen, kde je povolena jediná barva očí, se kód neuvádí.46
         */
        EmsCodePartDto x_61 = new() { Code = "61", PotentialCodePartInCzech = ["modré oči"] };
        EmsCodePartDto x_62 = new()
        {
            Code = "62",
            PotentialCodePartInCzech =
            [
                "oranžové oči",
                "oranžovooká",
                "a oranžovými očima",
                "s oranžovýma očima",
                "a s oranžovýma očima",
                "a oranžovýma očima"
            ]
        };
        EmsCodePartDto x_63 = new()
        {
            Code = "63",
            PotentialCodePartInCzech =
                ["nestejně zbarvené oči", "nestejně zbarvené oči (odd eyes): jedno oko modré, druhé oranžové)"]
        };
        EmsCodePartDto x_64 = new() { Code = "64", PotentialCodePartInCzech = ["zelené oči", "zelenooká"] };
        EmsCodePartDto x_65 = new() { Code = "65", PotentialCodePartInCzech = ["barmsky žluté oči"] };
        EmsCodePartDto x_66 = new() { Code = "66", PotentialCodePartInCzech = ["tonkinsky tyrkysové oči"] };
        EmsCodePartDto x_67 = new() { Code = "67", PotentialCodePartInCzech = ["siamsky modré oči"] };
        List<EmsCodePartDto> kodZbarveniOci =
        [
            x_61,
            x_62,
            x_63,
            x_64,
            x_65,
            x_66,
            x_67
        ];

        EmsCodePartDto x_81 = new() { Code = "81", PotentialCodePartInCzech = ["dlouhá srst", "dlouhosrstý"] };
        EmsCodePartDto x_82 = new() { Code = "82", PotentialCodePartInCzech = ["krátká srst", "krátkosrstý"] };
        EmsCodePartDto x_83 = new() { Code = "83", PotentialCodePartInCzech = ["brush"] };
        List<EmsCodePartDto> kodsrsti = [x_81, x_82, x_83];

        List<TypeOfCat> kocky = [];

        Breed hcs = new() { Code = "HCS", PotentialCodePartInCzech = ["Domácí krátkosrstá"], RequiresGroup = false };
        BreedDto hcs_n = new(hcs, Status.Valid);
        TypeOfCat kocka_HCS = new(
            hcs_n,
            zbaerveniSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodDepigmentaceSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyBileSkrrvrnitosti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyStupnedepigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodTypuKresbVSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodSnizenePigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZkraceniOcasu
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZbarveniOci
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodsrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList()
        );
        kocky.Add(kocka_HCS);

        Breed hcl = new() { Code = "HCL", PotentialCodePartInCzech = ["Domácí dlouhosrstá"], RequiresGroup = false };
        BreedDto hcl_n = new(hcl, Status.Valid);
        TypeOfCat kocka_HCL = new(
            hcl_n,
            zbaerveniSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodDepigmentaceSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyBileSkrrvrnitosti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyStupnedepigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodTypuKresbVSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodSnizenePigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZkraceniOcasu
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZbarveniOci
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodsrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList()
        );
        kocky.Add(kocka_HCL);

        Breed aby = new()
        {
            Code = "ABY", PotentialCodePartInCzech = ["Habešská kočka", "Habešská"], RequiresGroup = false
        };
        BreedDto aby_n = new(aby, Status.Valid);
        TypeOfCat kocka_aby = new(
            aby_n,
            zbaerveniSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodDepigmentaceSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyBileSkrrvrnitosti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyStupnedepigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodTypuKresbVSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodSnizenePigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZkraceniOcasu
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZbarveniOci
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodsrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList()
        );
        kocky.Add(kocka_aby);


        Breed ACL = new()
        {
            Code = "ACL", PotentialCodePartInCzech = ["Americký curl dlouhosrstý́"], RequiresGroup = false
        };
        BreedDto ACL_n = new(ACL, Status.Valid);
        TypeOfCat kocka_ACL = new(
            ACL_n,
            zbaerveniSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodDepigmentaceSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyBileSkrrvrnitosti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyStupnedepigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodTypuKresbVSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodSnizenePigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZkraceniOcasu
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZbarveniOci
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodsrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList()
        );
        kocky.Add(kocka_ACL);

        Breed ACS = new()
        {
            Code = "ACS", PotentialCodePartInCzech = ["Americký curl krátkosrstý"], RequiresGroup = false
        };
        BreedDto ACS_n = new(ACS, Status.Valid);
        TypeOfCat kocka_ACS = new(
            ACS_n,
            zbaerveniSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodDepigmentaceSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyBileSkrrvrnitosti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyStupnedepigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodTypuKresbVSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodSnizenePigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZkraceniOcasu
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZbarveniOci
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodsrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList()
        );
        kocky.Add(kocka_ACS);

        Breed BAL = new()
        {
            Code = "BAL", PotentialCodePartInCzech = ["Balinéská kočka", "Balinéská"], RequiresGroup = false
        };
        BreedDto BAL_n = new(BAL, Status.Valid);
        TypeOfCat kocka_BAL = new(
            BAL_n,
            zbaerveniSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodDepigmentaceSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyBileSkrrvrnitosti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyStupnedepigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodTypuKresbVSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodSnizenePigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZkraceniOcasu
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZbarveniOci
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodsrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList()
        );
        kocky.Add(kocka_BAL);

        Breed BEN = new()
        {
            Code = "BEN", PotentialCodePartInCzech = ["Bengálská kočka", "Bengálská"], RequiresGroup = false
        };
        BreedDto BENL_n = new(BEN, Status.Valid);
        TypeOfCat kocka_BEN = new(
            BENL_n,
            zbaerveniSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodDepigmentaceSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyBileSkrrvrnitosti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyStupnedepigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodTypuKresbVSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodSnizenePigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZkraceniOcasu
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZbarveniOci
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodsrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList()
        );
        kocky.Add(kocka_BEN);

        Breed BLH = new()
        {
            Code = "BLH",
            PotentialCodePartInCzech = ["Britská dlouhosrstá kočka", "Britská dlouhosrstá"],
            RequiresGroup = false
        };
        BreedDto BLH_n = new(BLH, Status.Valid);
        TypeOfCat kocka_BLH = new(
            BLH_n,
            zbaerveniSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodDepigmentaceSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyBileSkrrvrnitosti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyStupnedepigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodTypuKresbVSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodSnizenePigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZkraceniOcasu
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZbarveniOci
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodsrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList()
        );
        kocky.Add(kocka_BLH);

        Breed BML = new() { Code = "BML", PotentialCodePartInCzech = ["Burmilla"], RequiresGroup = true };
        BreedDto BML_n = new(BML, Status.Valid);
        TypeOfCat kocka_BML = new(
            BML_n,
            zbaerveniSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodDepigmentaceSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyBileSkrrvrnitosti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyStupnedepigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodTypuKresbVSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodSnizenePigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZkraceniOcasu
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZbarveniOci
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodsrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList()
        );
        kocky.Add(kocka_BML);

        Breed BSH = new()
        {
            Code = "BSH",
            PotentialCodePartInCzech = ["Britská krátkosrstá kočka", "Britská krátkosrstá"],
            RequiresGroup = false
        };
        BreedDto BSH_n = new(BSH, Status.Valid);
        TypeOfCat kocka_BSH = new(
            BSH_n,
            zbaerveniSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodDepigmentaceSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyBileSkrrvrnitosti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyStupnedepigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodTypuKresbVSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodSnizenePigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZkraceniOcasu
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZbarveniOci
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodsrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList()
        );
        kocky.Add(kocka_BSH);

        Breed BRI = new()
        {
            Code = "BRI",
            PotentialCodePartInCzech =
            [
                "Britská krátkosrstá kočka", "Britská krátkosrstá", "Britská dlouhosrstá kočka",
                "Britská dlouhosrstá", "Britská"
            ],
            RequiresGroup = false
        };
        BreedDto BRI_n = new(BRI, Status.Valid);
        TypeOfCat kocka_BRI = new(
            BRI_n,
            zbaerveniSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodDepigmentaceSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyBileSkrrvrnitosti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyStupnedepigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodTypuKresbVSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodSnizenePigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZkraceniOcasu
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZbarveniOci
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodsrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList()
        );
        kocky.Add(kocka_BRI);

        Breed BUR = new()
        {
            Code = "BUR", PotentialCodePartInCzech = ["Barmská kočka", "Barmská"], RequiresGroup = false
        };
        BreedDto BUR_n = new(BUR, Status.Valid);
        TypeOfCat kocka_BUR = new(
            BUR_n,
            zbaerveniSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodDepigmentaceSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyBileSkrrvrnitosti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyStupnedepigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodTypuKresbVSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodSnizenePigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZkraceniOcasu
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZbarveniOci
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodsrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList()
        );
        kocky.Add(kocka_BUR);

        Breed CHA = new()
        {
            Code = "CHA", PotentialCodePartInCzech = ["Kartouzská kočka", "Kartouzská"], RequiresGroup = false
        };
        BreedDto CHA_n = new(CHA, Status.Valid);
        TypeOfCat kocka_CHA = new(
            CHA_n,
            zbaerveniSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodDepigmentaceSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyBileSkrrvrnitosti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyStupnedepigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodTypuKresbVSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodSnizenePigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZkraceniOcasu
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZbarveniOci
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodsrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList()
        );
        kocky.Add(kocka_CHA);

        Breed CRX = new() { Code = "CRX", PotentialCodePartInCzech = ["Cornish rex"], RequiresGroup = true };
        BreedDto CRX_n = new(CRX, Status.Valid);
        TypeOfCat kocka_CRX = new(
            CRX_n,
            zbaerveniSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodDepigmentaceSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyBileSkrrvrnitosti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyStupnedepigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodTypuKresbVSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodSnizenePigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZkraceniOcasu
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZbarveniOci
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodsrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList()
        );
        kocky.Add(kocka_CRX);

        Breed CYM = new()
        {
            Code = "CYM", PotentialCodePartInCzech = ["Kymerská kočka", "Kymerská"], RequiresGroup = false
        };
        BreedDto CYM_n = new(CYM, Status.Valid);
        TypeOfCat kocka_CYM = new(
            CYM_n,
            zbaerveniSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodDepigmentaceSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyBileSkrrvrnitosti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyStupnedepigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodTypuKresbVSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodSnizenePigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZkraceniOcasu
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZbarveniOci
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodsrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList()
        );
        kocky.Add(kocka_CYM);

        Breed DRX = new() { Code = "DRX", PotentialCodePartInCzech = ["Devon rex"], RequiresGroup = true };
        BreedDto DRX_n = new(DRX, Status.Valid);
        TypeOfCat kocka_DRX = new(
            DRX_n,
            zbaerveniSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodDepigmentaceSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyBileSkrrvrnitosti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyStupnedepigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodTypuKresbVSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodSnizenePigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZkraceniOcasu
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZbarveniOci
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodsrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList()
        );
        kocky.Add(kocka_DRX);

        Breed DSP = new()
        {
            Code = "DSP", PotentialCodePartInCzech = ["Donský Sphynx", "donský sphynx"], RequiresGroup = true
        };
        BreedDto DSP_n = new(DSP, Status.Valid);
        TypeOfCat kocka_DSP = new(
            DSP_n,
            zbaerveniSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodDepigmentaceSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyBileSkrrvrnitosti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyStupnedepigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodTypuKresbVSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodSnizenePigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZkraceniOcasu
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZbarveniOci
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodsrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList()
        );
        kocky.Add(kocka_DSP);

        Breed EUR = new()
        {
            Code = "EUR", PotentialCodePartInCzech = ["Evropská kočka", "Evropská"], RequiresGroup = false
        };
        BreedDto EUR_n = new(EUR, Status.Valid);
        TypeOfCat kocka_EUR = new(
            EUR_n,
            zbaerveniSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodDepigmentaceSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyBileSkrrvrnitosti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyStupnedepigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodTypuKresbVSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodSnizenePigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZkraceniOcasu
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZbarveniOci
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodsrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList()
        );
        kocky.Add(kocka_EUR);

        Breed exo = new()
        {
            Code = "EXO", PotentialCodePartInCzech = ["Exotická kočka", "Exotická"], RequiresGroup = false
        };
        BreedDto exo_n = new(exo, Status.Valid);
        TypeOfCat kocka_exo = new(
            exo_n,
            zbaerveniSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodDepigmentaceSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyBileSkrrvrnitosti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyStupnedepigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodTypuKresbVSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodSnizenePigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZkraceniOcasu
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZbarveniOci
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodsrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList()
        );
        kocky.Add(kocka_exo);

        Breed GRX = new() { Code = "GRX", PotentialCodePartInCzech = ["German rex"], RequiresGroup = false };
        BreedDto GRX_n = new(GRX, Status.Valid);
        TypeOfCat kocka_GRXL = new(
            GRX_n,
            zbaerveniSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodDepigmentaceSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyBileSkrrvrnitosti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyStupnedepigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodTypuKresbVSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodSnizenePigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZkraceniOcasu
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZbarveniOci
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodsrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList()
        );
        kocky.Add(kocka_GRXL);

        Breed JBS = new()
        {
            Code = "JBS", PotentialCodePartInCzech = ["Japonský bobtail krátkosrstý"], RequiresGroup = false
        };
        BreedDto JBS_n = new(JBS, Status.Valid);
        TypeOfCat kocka_JBS = new(
            JBS_n,
            zbaerveniSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodDepigmentaceSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyBileSkrrvrnitosti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyStupnedepigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodTypuKresbVSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodSnizenePigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZkraceniOcasu
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZbarveniOci
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodsrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList()
        );
        kocky.Add(kocka_JBS);


        Breed KBL = new()
        {
            Code = "KBL", PotentialCodePartInCzech = ["Kurilský bobtail dlouhosrstý"], RequiresGroup = true
        };
        BreedDto KBL_n = new(KBL, Status.Valid);
        TypeOfCat kocka_KBL = new(
            KBL_n,
            zbaerveniSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodDepigmentaceSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyBileSkrrvrnitosti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyStupnedepigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodTypuKresbVSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodSnizenePigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZkraceniOcasu
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZbarveniOci
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodsrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList()
        );
        kocky.Add(kocka_KBL);

        Breed KBS = new()
        {
            Code = "KBS", PotentialCodePartInCzech = ["Kurilský bobtail krátkosrstý"], RequiresGroup = true
        };
        BreedDto KBS_n = new(KBS, Status.Valid);
        TypeOfCat kocka_KBS = new(
            KBS_n,
            zbaerveniSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodDepigmentaceSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyBileSkrrvrnitosti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyStupnedepigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodTypuKresbVSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodSnizenePigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZkraceniOcasu
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZbarveniOci
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodsrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList()
        );
        kocky.Add(kocka_KBS);


        Breed KOR = new() { Code = "KOR", PotentialCodePartInCzech = ["Korat"], RequiresGroup = false };
        BreedDto KOR_n = new(KOR, Status.Valid);
        TypeOfCat kocka_KOR = new(
            KOR_n,
            zbaerveniSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodDepigmentaceSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyBileSkrrvrnitosti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyStupnedepigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodTypuKresbVSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodSnizenePigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZkraceniOcasu
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZbarveniOci
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodsrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList()
        );
        kocky.Add(kocka_KOR);

        Breed LPL = new() { Code = "LPL", PotentialCodePartInCzech = ["LaPerm dlouhosrstá"], RequiresGroup = false };
        BreedDto LPL_n = new(LPL, Status.Valid);
        TypeOfCat kocka_LPL = new(
            LPL_n,
            zbaerveniSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodDepigmentaceSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyBileSkrrvrnitosti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyStupnedepigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodTypuKresbVSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodSnizenePigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZkraceniOcasu
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZbarveniOci
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodsrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList()
        );
        kocky.Add(kocka_LPL);

        Breed LPS = new() { Code = "LPS", PotentialCodePartInCzech = ["LaPerm krátkosrstá"], RequiresGroup = false };
        BreedDto LPS_n = new(LPS, Status.Valid);
        TypeOfCat kocka_LPS = new(
            LPS_n,
            zbaerveniSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodDepigmentaceSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyBileSkrrvrnitosti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyStupnedepigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodTypuKresbVSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodSnizenePigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZkraceniOcasu
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZbarveniOci
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodsrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList()
        );
        kocky.Add(kocka_LPS);


        Breed MAN = new()
        {
            Code = "MAN", PotentialCodePartInCzech = ["Manská kočka", "Manská"], RequiresGroup = false
        };
        BreedDto MAN_n = new(MAN, Status.Valid);
        TypeOfCat kocka_MAN = new(
            MAN_n,
            zbaerveniSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodDepigmentaceSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyBileSkrrvrnitosti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyStupnedepigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodTypuKresbVSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodSnizenePigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZkraceniOcasu
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZbarveniOci
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodsrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList()
        );
        kocky.Add(kocka_MAN);


        Breed MAU = new()
        {
            Code = "MAU", PotentialCodePartInCzech = ["Egyptská mau", "Egyptská"], RequiresGroup = false
        };
        BreedDto MAU_n = new(MAU, Status.Valid);
        TypeOfCat kocka_MAU = new(
            MAU_n,
            zbaerveniSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodDepigmentaceSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyBileSkrrvrnitosti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyStupnedepigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodTypuKresbVSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodSnizenePigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZkraceniOcasu
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZbarveniOci
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodsrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList()
        );
        kocky.Add(kocka_MAU);


        Breed MCO = new()
        {
            Code = "MCO",
            PotentialCodePartInCzech = ["Mainská kočka mývalí", "Mainská mývalí", "Mainská", "Mainská kočka"],
            RequiresGroup = true
        };
        BreedDto MCO_n = new(MCO, Status.Valid);
        TypeOfCat kocka_MCO = new(
            MCO_n,
            zbaerveniSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodDepigmentaceSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyBileSkrrvrnitosti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyStupnedepigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodTypuKresbVSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodSnizenePigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZkraceniOcasu
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZbarveniOci
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodsrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList()
        );
        kocky.Add(kocka_MCO);

        Breed NEM = new() { Code = "NEM", PotentialCodePartInCzech = ["Něvská maškaráda"], RequiresGroup = true };
        BreedDto NEM_n = new(NEM, Status.Valid);
        TypeOfCat kocka_NEM = new(
            NEM_n,
            zbaerveniSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodDepigmentaceSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyBileSkrrvrnitosti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyStupnedepigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodTypuKresbVSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodSnizenePigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZkraceniOcasu
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZbarveniOci
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodsrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList()
        );
        kocky.Add(kocka_NEM);


        Breed NFO = new()
        {
            Code = "NFO", PotentialCodePartInCzech = ["Norská kočka lesní", "Norská lesní"], RequiresGroup = true
        };
        BreedDto NFO_n = new(NFO, Status.Valid);
        TypeOfCat kocka_NFO = new(
            NFO_n,
            zbaerveniSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodDepigmentaceSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyBileSkrrvrnitosti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyStupnedepigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodTypuKresbVSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodSnizenePigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZkraceniOcasu
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZbarveniOci
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodsrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList()
        );
        kocky.Add(kocka_NFO);


        Breed OCI = new() { Code = "OCI", PotentialCodePartInCzech = ["Ocikat", "Ocicat"], RequiresGroup = false };
        BreedDto OCI_n = new(OCI, Status.Valid);
        TypeOfCat kocka_NOCI = new(
            OCI_n,
            zbaerveniSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodDepigmentaceSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyBileSkrrvrnitosti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyStupnedepigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodTypuKresbVSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodSnizenePigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZkraceniOcasu
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZbarveniOci
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodsrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList()
        );
        kocky.Add(kocka_NOCI);


        Breed OLH = new()
        {
            Code = "OLH",
            PotentialCodePartInCzech =
                ["Orientální kočka dlouhosrstá", "Orientální dlouhosrstá"],
            RequiresGroup = false
        };
        BreedDto OLH_n = new(OLH, Status.Valid);
        TypeOfCat kocka_OLH = new(
            OLH_n,
            zbaerveniSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodDepigmentaceSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyBileSkrrvrnitosti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyStupnedepigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodTypuKresbVSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodSnizenePigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZkraceniOcasu
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZbarveniOci
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodsrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList()
        );
        kocky.Add(kocka_OLH);

        Breed OSH = new()
        {
            Code = "OSH",
            PotentialCodePartInCzech =
                ["Orientální kočka krátkosrstá", "Orientální krátkosrstá"],
            RequiresGroup = false
        };
        BreedDto OSH_n = new(OSH, Status.Valid);
        TypeOfCat kocka_OSH = new(
            OSH_n,
            zbaerveniSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodDepigmentaceSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyBileSkrrvrnitosti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyStupnedepigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodTypuKresbVSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodSnizenePigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZkraceniOcasu
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZbarveniOci
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodsrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList()
        );
        kocky.Add(kocka_OSH);

        Breed PEB = new() { Code = "PEB", PotentialCodePartInCzech = ["Peterbald"], RequiresGroup = false };
        BreedDto PEB_n = new(PEB, Status.Valid);
        TypeOfCat kocka_PEB = new(
            PEB_n,
            zbaerveniSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodDepigmentaceSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyBileSkrrvrnitosti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyStupnedepigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodTypuKresbVSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodSnizenePigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZkraceniOcasu
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZbarveniOci
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodsrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList()
        );
        kocky.Add(kocka_PEB);


        Breed PER = new()
        {
            Code = "PER", PotentialCodePartInCzech = ["Perská kočka", "Perská"], RequiresGroup = false
        };
        BreedDto PER_n = new(PER, Status.Valid);
        TypeOfCat kocka_PER = new(
            PER_n,
            zbaerveniSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodDepigmentaceSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyBileSkrrvrnitosti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyStupnedepigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodTypuKresbVSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodSnizenePigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZkraceniOcasu
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZbarveniOci
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodsrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList()
        );
        kocky.Add(kocka_PER);


        Breed RAG = new() { Code = "RAG", PotentialCodePartInCzech = ["Ragdoll"], RequiresGroup = false };
        BreedDto RAG_n = new(RAG, Status.Valid);
        TypeOfCat kocka_RAG = new(
            RAG_n,
            zbaerveniSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodDepigmentaceSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyBileSkrrvrnitosti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyStupnedepigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodTypuKresbVSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodSnizenePigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZkraceniOcasu
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZbarveniOci
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodsrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList()
        );
        kocky.Add(kocka_RAG);


        Breed RUS = new()
        {
            Code = "RUS", PotentialCodePartInCzech = ["Ruská modrá kočka", "Ruská modrá"], RequiresGroup = false
        };
        BreedDto RUS_n = new(RUS, Status.Valid);
        TypeOfCat kocka_RUS = new(
            RUS_n,
            zbaerveniSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodDepigmentaceSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyBileSkrrvrnitosti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyStupnedepigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodTypuKresbVSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodSnizenePigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZkraceniOcasu
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZbarveniOci
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodsrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList()
        );
        kocky.Add(kocka_RUS);

        Breed SBI = new() { Code = "SBI", PotentialCodePartInCzech = ["Birma"], RequiresGroup = false };
        BreedDto SBI_n = new(SBI, Status.Valid);
        TypeOfCat kocka_SBI = new(
            SBI_n,
            zbaerveniSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodDepigmentaceSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyBileSkrrvrnitosti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyStupnedepigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodTypuKresbVSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodSnizenePigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZkraceniOcasu
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZbarveniOci
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodsrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList()
        );
        kocky.Add(kocka_SBI);

        Breed SIA = new()
        {
            Code = "SIA", PotentialCodePartInCzech = ["Siamská kočka", "Siamská"], RequiresGroup = false
        };
        BreedDto SIA_n = new(SIA, Status.Valid);
        TypeOfCat kocko_SIA = new(
            SIA_n,
            zbaerveniSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodDepigmentaceSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyBileSkrrvrnitosti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyStupnedepigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodTypuKresbVSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodSnizenePigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZkraceniOcasu
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZbarveniOci
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodsrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList()
        );
        kocky.Add(kocko_SIA);


        Breed SIB = new()
        {
            Code = "SIB", PotentialCodePartInCzech = ["Sibiřská kočka", "Sibiřská"], RequiresGroup = true
        };
        BreedDto SIB_n = new(SIB, Status.Valid);
        TypeOfCat kocka_SIA = new(
            SIB_n,
            zbaerveniSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodDepigmentaceSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyBileSkrrvrnitosti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyStupnedepigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodTypuKresbVSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodSnizenePigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZkraceniOcasu
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZbarveniOci
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodsrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList()
        );
        kocky.Add(kocka_SIA);


        Breed SIN = new() { Code = "SIN", PotentialCodePartInCzech = ["Singapura"], RequiresGroup = false };
        BreedDto SIN_n = new(SIN, Status.Valid);
        TypeOfCat kocka_SIB = new(
            SIN_n,
            zbaerveniSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodDepigmentaceSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyBileSkrrvrnitosti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyStupnedepigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodTypuKresbVSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodSnizenePigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZkraceniOcasu
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZbarveniOci
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodsrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList()
        );
        kocky.Add(kocka_SIB);


        Breed SNO = new() { Code = "SNO", PotentialCodePartInCzech = ["Snowshoe"], RequiresGroup = false };
        BreedDto SNO_n = new(SNO, Status.Valid);
        TypeOfCat kocka_SNO = new(
            SNO_n,
            zbaerveniSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodDepigmentaceSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyBileSkrrvrnitosti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyStupnedepigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodTypuKresbVSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodSnizenePigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZkraceniOcasu
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZbarveniOci
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodsrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList()
        );
        kocky.Add(kocka_SNO);


        Breed SOK = new() { Code = "SOK", PotentialCodePartInCzech = ["Sokoke"], RequiresGroup = false };
        BreedDto SOK_n = new(SOK, Status.Valid);
        TypeOfCat kocka_SOK = new(
            SOK_n,
            zbaerveniSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodDepigmentaceSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyBileSkrrvrnitosti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyStupnedepigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodTypuKresbVSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodSnizenePigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZkraceniOcasu
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZbarveniOci
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodsrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList()
        );
        kocky.Add(kocka_SOK);


        Breed SOM = new()
        {
            Code = "SOM", PotentialCodePartInCzech = ["Somálská kočka", "Somálská"], RequiresGroup = false
        };
        BreedDto SOM_n = new(SOM, Status.Valid);
        TypeOfCat kocka_SOM = new(
            SOM_n,
            zbaerveniSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodDepigmentaceSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyBileSkrrvrnitosti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyStupnedepigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodTypuKresbVSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodSnizenePigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZkraceniOcasu
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZbarveniOci
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodsrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList()
        );
        kocky.Add(kocka_SOM);


        Breed SPH = new() { Code = "SPH", PotentialCodePartInCzech = ["Sphynx"], RequiresGroup = true };
        BreedDto SPH_n = new(SPH, Status.Valid);
        TypeOfCat kocka_SPH = new(
            SPH_n,
            zbaerveniSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodDepigmentaceSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyBileSkrrvrnitosti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyStupnedepigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodTypuKresbVSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodSnizenePigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZkraceniOcasu
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZbarveniOci
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodsrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList()
        );
        kocky.Add(kocka_SPH);


        Breed SRL = new()
        {
            Code = "SRL", PotentialCodePartInCzech = ["Selkirk rex dlouhosrstý"], RequiresGroup = false
        };
        BreedDto SRL_n = new(SRL, Status.Valid);
        TypeOfCat kocka_SRL = new(
            SRL_n,
            zbaerveniSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodDepigmentaceSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyBileSkrrvrnitosti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyStupnedepigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodTypuKresbVSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodSnizenePigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZkraceniOcasu
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZbarveniOci
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodsrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList()
        );
        kocky.Add(kocka_SRL);


        Breed SRS = new()
        {
            Code = "SRS", PotentialCodePartInCzech = ["Selkirk rex krátkosrstý"], RequiresGroup = false
        };
        BreedDto SRS_n = new(SRS, Status.Valid);
        TypeOfCat kocka_SRS = new(
            SRS_n,
            zbaerveniSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodDepigmentaceSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyBileSkrrvrnitosti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyStupnedepigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodTypuKresbVSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodSnizenePigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZkraceniOcasu
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZbarveniOci
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodsrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList()
        );
        kocky.Add(kocka_SRS);


        Breed THA = new()
        {
            Code = "THA", PotentialCodePartInCzech = ["Thajská kočka", "Thajská"], RequiresGroup = false
        };
        BreedDto THA_n = new(THA, Status.Valid);
        TypeOfCat kocka_THA = new(
            THA_n,
            zbaerveniSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodDepigmentaceSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyBileSkrrvrnitosti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyStupnedepigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodTypuKresbVSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodSnizenePigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZkraceniOcasu
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZbarveniOci
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodsrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList()
        );
        kocky.Add(kocka_THA);


        Breed abTUAy = new() { Code = "TUA", PotentialCodePartInCzech = ["Turecká angora"], RequiresGroup = false };
        BreedDto TUA_n = new(abTUAy, Status.Valid);
        TypeOfCat kocka_TUA = new(
            TUA_n,
            zbaerveniSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodDepigmentaceSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyBileSkrrvrnitosti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyStupnedepigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodTypuKresbVSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodSnizenePigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZkraceniOcasu
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZbarveniOci
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodsrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList()
        );
        kocky.Add(kocka_TUA);


        Breed TUV = new() { Code = "TUV", PotentialCodePartInCzech = ["Turecká van"], RequiresGroup = false };
        BreedDto TUV_n = new(TUV, Status.Valid);
        TypeOfCat kocka_TUV = new(
            TUV_n,
            zbaerveniSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodDepigmentaceSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyBileSkrrvrnitosti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodyStupnedepigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodTypuKresbVSrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodSnizenePigmentace
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZkraceniOcasu
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodZbarveniOci
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList(),
            kodsrsti
                .Select(data => new EmsCodePartPerCatTypeDto(data, Status.Valid))
                .ToList()
        );
        kocky.Add(kocka_TUV);
        return kocky;
    }
}
