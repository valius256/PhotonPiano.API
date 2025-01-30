using System.Net;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Models;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.Shared.Extensions;

namespace PhotonPiano.BusinessLogic.Services;

public class PaymentService : IPaymentService
{
    private readonly IConfiguration _configuration;
    private readonly VnPay _vnPay;


    public PaymentService(IConfiguration configuration, IOptions<VnPay> vnpay)
    {
        _configuration = configuration;
        _vnPay = vnpay.Value;
    }

    public string CreateVnPayPaymentUrl(Transaction transaction, string ipAddress, string apiBaseUrl, string accountId,
        string clientRedirectUrl, string? customReturnUrl)
    {
        var returnUrl =
            $"{apiBaseUrl}/api/entrance-tests/{accountId}/enrollment-payment-callback?url={clientRedirectUrl}";

        if (!string.IsNullOrEmpty(customReturnUrl)) returnUrl = customReturnUrl;


        var typeOfTransaction = transaction.TransactionType == TransactionType.EntranceTestFee
            ? "THANH TOAN LE PHI THI DAU VAO"
            : "THANH TOAN PHI DAY HOC";


        // Prepare immutable dictionary for query parameters
        var queryParams = new SortedList<string, string>
        {
            { "vnp_Version", _vnPay.Version },
            { "vnp_Command", _vnPay.Command },
            { "vnp_TmnCode", _vnPay.TmnCode },
            {
                "vnp_Amount", ((long)(transaction.Amount) * 100).ToString()
            }, // Amount in VND, multiplied by 100 to eliminate decimals
            { "vnp_CurrCode", _vnPay.CurrCode },
            { "vnp_TxnRef", transaction.Id.ToString() },
            { "vnp_OrderInfo", $"{typeOfTransaction} - PHOTON PIANO_{transaction.Id}" },
            { "vnp_OrderType", "250006" }, // Type of order
            { "vnp_Locale", _vnPay.Locale }, // Locale
            { "vnp_ReturnUrl", returnUrl },
            { "vnp_IpAddr", ipAddress },
            { "vnp_CreateDate", DateTime.UtcNow.ToVietnamTime().ToString("yyyyMMddHHmmss") }
        };

        // Generate raw data string to create the secure hash
        // Create raw data string for hash

        var signData = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={WebUtility.UrlEncode(kvp.Value)}"));
        var hmacResult = HmacSha512(_vnPay.HashSecret, signData);
        queryParams.Add("vnp_SecureHash", hmacResult);

        var queryString = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={WebUtility.UrlEncode(kvp.Value)}"));

        return $"{_vnPay.BaseUrl}?{queryString}";
    }

    private string HmacSha512(string key, string inputData)
    {
        var hash = new HMACSHA512(Encoding.UTF8.GetBytes(key));
        var hmac = hash.ComputeHash(Encoding.UTF8.GetBytes(inputData));
        return BitConverter.ToString(hmac).Replace("-", "").ToLower();
    }

    public bool ValidateSignature(IQueryCollection queryCollection)
    {
        var vnpSecureHash = queryCollection["vnp_SecureHash"].ToString();

        var inputHash = new StringBuilder();
        foreach (var (key, value) in queryCollection.OrderBy(k => k.Key))
            if (!string.IsNullOrEmpty(value) && key.StartsWith("vnp_") && key != "vnp_SecureHash")
                inputHash.Append($"{key}={value}&");

        inputHash.Remove(inputHash.Length - 1, 1);
        var calculatedHash = HmacSha512(_vnPay.HashSecret, inputHash.ToString());

        return vnpSecureHash.Equals(calculatedHash, StringComparison.InvariantCultureIgnoreCase);
    }
}