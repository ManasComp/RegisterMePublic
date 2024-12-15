#region

using XLocalizer.ErrorMessages;

#endregion

namespace WebGui.Localization;

public static class Localization
{
    public static LocalizationModel Localize()
    {
        ValidationErrors validationErrors = new()
        {
            RequiredAttribute_ValidationError = "Pole {0} je povinné.",
            CompareAttribute_MustMatch = "Hodnoty '{0}' a '{1}' se neshodují.",
            StringLengthAttribute_ValidationError = "Pole {0} musí být text s maximální délkou {1}.",
            CreditCardAttribute_Invalid = "Pole {0} není platné číslo kreditní karty.",
            CustomValidationAttribute_ValidationError = "{0} není platné.",
            DataTypeAttribute_EmptyDataTypeString = "Vlastní řetězec DataType nemůže být prázdný.",
            EmailAddressAttribute_Invalid = "Pole {0} není platná e-mailová adresa.",
            FileExtensionsAttribute_Invalid = "Pole {0} přijímá pouze soubory s následujícími příponami: {1}",
            MaxLengthAttribute_ValidationError = "Pole {0} musí být text s maximální délkou {1}.",
            MinLengthAttribute_ValidationError = "Pole {0} musí být text s minimální délkou {1}.",
            PhoneAttribute_Invalid = "Pole {0} není platné telefonní číslo.",
            RangeAttribute_ValidationError = "Pole {0} musí být mezi {1} a {2}.",
            RegexAttribute_ValidationError = "Pole {0} musí odpovídat regulárnímu výrazu '{1}'.",
            StringLengthAttribute_ValidationErrorIncludingMinimum =
                "Pole {0} musí být text s délkou mezi {2} a {1}.",
            UrlAttribute_Invalid = "Pole {0} není platná URL adresa.",
            ValidationAttribute_ValidationError = "Pole {0} je neplatné."
        };

        ModelBindingErrors modelBindingErrors = new()
        {
            AttemptedValueIsInvalidAccessor = "Honota'{0}' není správná pro {1}.",
            MissingBindRequiredValueAccessor = "Hodnota pro pole '{0}' nebyla zadána.",
            MissingKeyOrValueAccessor = "Hodnota je povinná.",
            MissingRequestBodyRequiredValueAccessor = "Požadováno je neprázdné tělo požadavku.",
            NonPropertyAttemptedValueIsInvalidAccessor = "Hodnota '{0}' není správná.",
            NonPropertyUnknownValueIsInvalidAccessor = "Zadaná hodnota je neplatná.",
            NonPropertyValueMustBeANumberAccessor = "Pole musí být číslo.",
            UnknownValueIsInvalidAccessor = "Zadaná hodnota je neplatná pro {0}.",
            ValueIsInvalidAccessor = "Hodnota '{0}' je neplatná.",
            ValueMustBeANumberAccessor = "Pole {0} musí být číslo.",
            ValueMustNotBeNullAccessor = "Hodnota '{0}' je neplatná."
        };

        IdentityErrors identityErrors = new()
        {
            DuplicateEmail = "Email '{0}' je už používán.",
            DuplicateUserName = "Uživatelské jméno '{0}' je už používáno.",
            InvalidEmail = "Email '{0}' je neplatný.",
            DuplicateRoleName = "Název role '{0}' je už používán.",
            InvalidRoleName = "Název role '{0}' je neplatný.",
            InvalidToken = "Neplatný token.",
            InvalidUserName = "Uživatelské jméno '{0}' je neplatné, může obsahovat pouze písmena nebo číslice.",
            LoginAlreadyAssociated = "Uživatel s těmito údaji už existuje.",
            PasswordMismatch = "Hela se neshodují",
            PasswordRequiresDigit = "Heslo musí obsahovat alespoň jednu číslici ('0'-'9').",
            PasswordRequiresLower = "Heslo musí obsahovat alespoň jedno malé písmeno ('a'-'z').",
            PasswordRequiresNonAlphanumeric =
                "Heslo musí obsahovat alespoň jeden znak, který není písmeno nebo číslice.",
            PasswordRequiresUniqueChars = "Heslo musí obsahovat alespoň {0} jedinečných znaků.",
            PasswordRequiresUpper = "Heslo musí obsahovat alespoň jedno velké písmeno ('A'-'Z').",
            PasswordTooShort = "Heslo musí být alespoň '{0}' znaků dlouhé.",
            UserAlreadyHasPassword = "Uživatel už má heslo nastavené.",
            UserAlreadyInRole = "Uživatel už je v roli '{0}'.",
            UserNotInRole = "Uživatel není v roli '{0}'.",
            UserLockoutNotEnabled = "Zablokování uživatele není povoleno.",
            RecoveryCodeRedemptionFailed = "Obnovení kódu selhalo.",
            ConcurrencyFailure = "Optimistický zámek selhal, záznam byl upraven jiným uživatelem.",
            DefaultError = "Došlo k chybě."
        };

        LocalizationModel localizationModel = new()
        {
            ValidationErrors = validationErrors,
            ModelBindingErrors = modelBindingErrors,
            IdentityErrors = identityErrors
        };

        return localizationModel;
    }
}
