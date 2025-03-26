using Microsoft.AspNetCore.Mvc;

namespace PhotonPiano.Api.Requests.Application;

public record RefundApplicationRequest : SendApplicationRequest
{
    [FromForm(Name = "bankName")]
    public required string BankName { get; init; }

    [FromForm(Name = "bankAccountName")]
    public required string BankAccountName { get; init; }

    [FromForm(Name = "bankAccountNumber")]
    public required string BankAccountNumber { get; init; }

}