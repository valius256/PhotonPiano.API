using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using PhotonPiano.Api.Requests.Auth;
using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.DataAccess.Models;

namespace PhotonPiano.Test.Extensions;

public static class Extensions
{
    public static async Task EnsureDbCreated(this IHost app)
    {
        using var serviceScope = app.Services.CreateScope();
        var db = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await db.Database.EnsureCreatedAsync();
        await db.Database.MigrateAsync();
    }

    public static async Task<string> GetAuthToken(this HttpClient client, string email, string password)
    {
        var signInRequest = new SignInRequest(email, password);
        var content = new StringContent(JsonConvert.SerializeObject(signInRequest), Encoding.UTF8, "application/json");
        var response = await client.PostAsync("/api/auth/sign-in", content);
        response.EnsureSuccessStatusCode();

        return JsonConvert.DeserializeObject<AuthModel>(await response.Content.ReadAsStringAsync())!.IdToken;
    }

    public static StringContent SerializeRequest<T>(T request)
    {
        return new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
    }

    public static async Task<TResponse> DeserializeResponse<TResponse>(HttpResponseMessage response)
    {
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<TResponse>(content)!;
    }
}