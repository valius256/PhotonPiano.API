namespace PhotonPiano.BusinessLogic.BusinessModel.Payment;

public record VnPayCallbackModel
{

    public required string VnpTxnRef { get; init; } // Transaction Reference (Order ID)


    public required string VnpAmount { get; init; } // Payment Amount

    public required string VnpResponseCode { get; init; } // Response Code from VNPAY


    public required string VnpTransactionNo { get; init; } // VNPAY Transaction Number


    public required string VnpSecureHash { get; init; } // Secure Hash to validate response
}