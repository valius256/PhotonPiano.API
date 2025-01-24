using Microsoft.AspNetCore.Mvc;

namespace PhotonPiano.Api.Requests.Payment;

public record VnPayReturnRequest
{
    [FromQuery(Name = "vnp_TxnRef")]
    public required string VnpTxnRef { get; init; } // Transaction Reference (Order ID)

    [FromQuery(Name = "vnp_Amount")]
    public required string VnpAmount { get; init; } // Payment Amount

    [FromQuery(Name = "vnp_ResponseCode")]
    public required string VnpResponseCode { get; init; } // Response Code from VNPAY

    [FromQuery(Name = "vnp_TransactionNo")]
    public required string VnpTransactionNo { get; init; } // VNPAY Transaction Number

    [FromQuery(Name = "vnp_SecureHash")]
    public required string VnpSecureHash { get; init; } // Secure Hash to validate response
    
}