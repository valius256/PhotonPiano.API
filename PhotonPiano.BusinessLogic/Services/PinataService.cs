using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PhotonPiano.BusinessLogic.Interfaces;
namespace PhotonPiano.BusinessLogic.Services;

public class PinataService : IPinataService
{
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _jwtToken;
    private readonly string _gatewayBaseUrl;

    public PinataService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
        _jwtToken = _configuration["Pinata:JwtToken"]!;
        _gatewayBaseUrl = _configuration["Pinata:GatewayBaseUrl"]!;
    }

    public async Task<string> UploadFile(IFormFile file, string? fileName = default)
    {
        string apiUrl = "https://uploads.pinata.cloud/v3/files";
        using var client = _httpClientFactory.CreateClient();
        

        // Set Authorization header
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("multipart/form-data"));

        using var formData = new MultipartFormDataContent();

        // Open file stream
        await using var stream = file.OpenReadStream();
    
        var fileContent = new StreamContent(stream);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        // Explicitly set the Content-Disposition header for the file content
        fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
        {
            Name = "\"file\"",
            FileName = "\"" + file.FileName + "\""
        };

        formData.Add(fileContent);
        formData.Add(new StringContent(_configuration["Pinata:DefaultGroupId"]!), "group_id");

        // Send request
        using var response = await client.PostAsync(apiUrl, formData);

        if (!response.IsSuccessStatusCode)
        {
            string content = await response.Content.ReadAsStringAsync();
            throw new Exception($"Upload failed with status: {response.StatusCode}, content: {content}");
        }

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var responseObject = JsonConvert.DeserializeObject<dynamic>(jsonResponse)!;

        string cid = responseObject.data.cid;

        return GetFileUrl(cid);
    }

    
    private string GetFileUrl(string fileCid)
    {
        return $"{_gatewayBaseUrl}/{fileCid}";
    }


}