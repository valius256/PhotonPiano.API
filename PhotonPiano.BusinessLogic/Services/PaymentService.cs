using System.Net;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.Shared.Extensions;

namespace PhotonPiano.BusinessLogic.Services;

public class PaymentService : IPaymentService
{
    private readonly IConfiguration _configuration;

    public PaymentService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string CreateVnPayPaymentUrl(Transaction transaction, string ipAddress, string apiBaseUrl, string accountId,
        string clientRedirectUrl)
    {
        // Retrieve VNPAY configuration values
        string tmnCode = _configuration["VnPay:TmnCode"]!;
        string hashSecret = _configuration["VnPay:HashSecret"]!;
        string vnpUrl = _configuration["VnPay:BaseUrl"]!;

        string returnUrl = $"{apiBaseUrl}/api/entrance-tests/{accountId}/enrollment-payment-callback?url={clientRedirectUrl}";

        // Prepare immutable dictionary for query parameters
        var queryParams = new SortedList<string, string>
        {
            { "vnp_Version", "2.1.0" },
            { "vnp_Command", "pay" },
            { "vnp_TmnCode", tmnCode },
            {
                "vnp_Amount", ((long)(transaction.Amount) * 100).ToString()
            }, // Amount in VND, multiplied by 100 to eliminate decimals
            { "vnp_CurrCode", "VND" },
            { "vnp_TxnRef", transaction.Id.ToString() },
            { "vnp_OrderInfo", $"THANH TOAN LE PHI THI DAU VAO - PHOTON PIANO_{transaction.Id}" },
            { "vnp_OrderType", "250006" }, // Type of order
            { "vnp_Locale", "vn" }, // Locale
            { "vnp_ReturnUrl", returnUrl },
            { "vnp_IpAddr", ipAddress },
            { "vnp_CreateDate", DateTime.UtcNow.ToVietnamTime().ToString("yyyyMMddHHmmss") }
        };

        // Generate raw data string to create the secure hash
        // Create raw data string for hash

        var signData = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={WebUtility.UrlEncode(kvp.Value)}"));
        var hmacResult = HmacSha512(hashSecret, signData);
        queryParams.Add("vnp_SecureHash", hmacResult);

        var queryString = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={WebUtility.UrlEncode(kvp.Value)}"));

        return $"{vnpUrl}?{queryString}";
    }

    private string HmacSha512(string key, string inputData)
    {
        var hash = new HMACSHA512(Encoding.UTF8.GetBytes(key));
        var hmac = hash.ComputeHash(Encoding.UTF8.GetBytes(inputData));
        return BitConverter.ToString(hmac).Replace("-", "").ToLower();
    }

    public bool ValidateSignature(IQueryCollection queryCollection)
    {
        string vnpHashSecret = _configuration["VnPay:HashSecret"]!;
        var vnpSecureHash = queryCollection["vnp_SecureHash"].ToString();

        var inputHash = new StringBuilder();
        foreach (var (key, value) in queryCollection.OrderBy(k => k.Key))
        {
            if (!string.IsNullOrEmpty(value) && key.StartsWith("vnp_") && key != "vnp_SecureHash")
            {
                inputHash.Append($"{key}={value}&");
            }
        }

        inputHash.Remove(inputHash.Length - 1, 1);
        var calculatedHash = HmacSha512(vnpHashSecret, inputHash.ToString());

        return vnpSecureHash.Equals(calculatedHash, StringComparison.InvariantCultureIgnoreCase);
    }
}