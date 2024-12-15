#region

using QRCoder;
using RegisterMe.Domain.Enums;

#endregion

namespace RegisterMe.Application;

public class QrCode
{
    public QrCode(string accountNumber, decimal amount, Currency currencies, string message, long variableSymbol)
    {
        if (accountNumber.Length != 24)
        {
            throw new ArgumentException("ACC must be 24 characters long");
        }

        AccountNumber = accountNumber;

        if (amount is < 0.0m or >= 9999999.99m)
        {
            throw new ArgumentException("Amount must be greater than 0");
        }

        Amount = amount;

        if (currencies.ToString().Length != 3)
        {
            throw new ArgumentException("Currency must be 3 characters long");
        }

        Currency = currencies.ToString();

        if (message.Length > 60)
        {
            throw new ArgumentException("Message must be less than 60 characters long");
        }

        Message = message;

        if (variableSymbol.ToString().Length > 10)
        {
            throw new ArgumentException("VariableSymbol must be less than 10 characters long");
        }

        VariableSymbol = variableSymbol;
    }

    private string AccountNumber { get; }
    private decimal Amount { get; }
    private string Currency { get; }
    private string Message { get; }
    private long VariableSymbol { get; } // registrationToExhibitionId

    /**
     * follows the standard https://mojebanka.kb.cz/file/cs/format_qr_kb.pdf
     */
    private string GetPayload()
    {
        return
            $"SPD*1.0*ACC:{AccountNumber}*AM:{Amount}*CC:{Currency}*MSG:{Message}*X-VS:{VariableSymbol}*";
    }

    public string GenerateQrCode()
    {
        QRCodeGenerator qrGenerator = new();
        QRCodeData qrCodeData = qrGenerator.CreateQrCode(GetPayload(), QRCodeGenerator.ECCLevel.Q);
        PngByteQRCode qrCode = new(qrCodeData);
        byte[] qrCodeAsPngByteArr = qrCode.GetGraphic(20);

        string x = "data:image/png;base64," + Convert.ToBase64String(qrCodeAsPngByteArr);
        return x;
    }
}
