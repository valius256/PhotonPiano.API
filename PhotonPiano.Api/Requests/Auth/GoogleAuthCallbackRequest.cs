using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;

namespace PhotonPiano.Api.Requests.Auth;

public record GoogleAuthCallbackRequest
{
    [FromQuery(Name = "code")]
    [Description("The code returned from google auth callback")]
    public required string Code { get; init; }

    [FromQuery(Name = "url")]
    [Description("The client google auth redirect url used for google signin url")]
    public required string RedirectUrl { get; init; }

    public void Deconstruct(out string code, out string redirectUrl)
    {
        code = Code;
        redirectUrl = RedirectUrl;
    }
}