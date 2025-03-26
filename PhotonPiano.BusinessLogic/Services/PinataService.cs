using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PhotonPiano.BusinessLogic.Interfaces;
using System.Net.Http.Headers;

namespace PhotonPiano.BusinessLogic.Services;

public class PinataService : IPinataService
{
    private readonly IConfiguration _configuration;
    private readonly string _gatewayBaseUrl;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _jwtToken;

    public PinataService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
        _jwtToken = _configuration["Pinata:JwtToken"]!;
        _gatewayBaseUrl = _configuration["Pinata:GatewayBaseUrl"]!;
    }

    public async Task<string> UploadFile(IFormFile file, string? fileName = null)
    {
        string apiUrl = "https://uploads.pinata.cloud/v3/files";
        using var client = _httpClientFactory.CreateClient();

        // Set Authorization header
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);

        // Convert file to byte array
        byte[] fileBytes;
        await using (var ms = new MemoryStream())
        {
            await file.CopyToAsync(ms);
            fileBytes = ms.ToArray();
        }

        // Manually build the multipart request body (Postman-style)
        var boundary = "----WebKitFormBoundary" + Guid.NewGuid().ToString("N");
        var content = new MultipartFormDataContent(boundary);

        fileName ??= file.FileName;

        // Manually format Content-Disposition header
        var fileContent = new ByteArrayContent(fileBytes);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        fileContent.Headers.TryAddWithoutValidation("Content-Disposition",
            $"form-data; name=\"file\"; filename=\"{fileName}\"");

        content.Add(fileContent, "file", fileName);

        // Add group_id if present
        if (!string.IsNullOrEmpty(_configuration["Pinata:DefaultGroupId"]))
        {
            var groupIdContent = new StringContent(_configuration["Pinata:DefaultGroupId"]!);
            groupIdContent.Headers.TryAddWithoutValidation("Content-Disposition", "form-data; name=\"group_id\"");
            content.Add(groupIdContent, "group_id");
        }

        // Explicitly set boundary
        content.Headers.TryAddWithoutValidation("Content-Type", $"multipart/form-data; boundary={boundary}");

        // Debugging: Log FormData Headers
        foreach (var httpContent in content)
        {
            Console.WriteLine($"DEBUG Content-Disposition: {httpContent.Headers.ContentDisposition}");
            Console.WriteLine($"DEBUG Content-Type: {httpContent.Headers.ContentType}");
        }

        // Send request
        using var response = await client.PostAsync(apiUrl, content);

        // Read response content
        string responseContent = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Upload failed with status: {response.StatusCode}, response: {responseContent}");
        }

        var responseObject = JsonConvert.DeserializeObject<dynamic>(responseContent)!;
        string cid = responseObject.data.cid;

        return GetFileUrl(cid);
    }


    // public async Task<string> UploadFile(IFormFile file, string? fileName = default)
    // {
    //     var pinataClient = new PinataClient(_jwtToken);
    //     var fileList = await pinataClient.GetFilesListAsync();
    //     try
    //     {
    //         await using var stream = file.OpenReadStream();
    //         var response = await pinataClient.UploadFileAsync(stream, fileName ?? file.FileName);
    //
    //         var cid = response?.Data.Cid ?? throw new Exception("Upload failed, no CID received.");
    //
    //         return GetFileUrl(cid);
    //     }
    //     catch (JsonException ex)
    //     {
    //         Console.WriteLine($"Error: {ex.Message}");
    //     }
    //     catch (Exception ex)
    //     {
    //         Console.WriteLine($"An unexpected error occurred: {ex.Message}");
    //     }
    //
    //     return null;
    // }


    private string GetFileUrl(string fileCid)
    {
        return $"{_gatewayBaseUrl}/{fileCid}";
    }
}