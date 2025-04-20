using System.Drawing;
using System.Drawing.Imaging;
using DinkToPdf.Contracts;
using Ghostscript.NET.Rasterizer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Certificate;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.Shared.Exceptions;
using PuppeteerSharp;
using PuppeteerSharp.Media;
using static System.Drawing.Imaging.ImageCodecInfo;
using Color = System.Drawing.Color;
using ImageFormat = System.Drawing.Imaging.ImageFormat;
using RouteData = Microsoft.AspNetCore.Routing.RouteData;

namespace PhotonPiano.BusinessLogic.Services;

public class CertificateService : ICertificateService
{
    private readonly IServiceFactory _serviceFactory;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRazorViewEngine _razorViewEngine;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly ITempDataProvider _tempDataProvider;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConverter _converter;
    private readonly IServiceProvider _serviceProvider;

    public CertificateService(IServiceFactory serviceFactory, IUnitOfWork unitOfWork, IRazorViewEngine razorViewEngine,
        IWebHostEnvironment webHostEnvironment, ITempDataProvider tempDataProvider,
        IHttpContextAccessor httpContextAccessor, IConverter converter, IServiceProvider serviceProvider)
    {
        _serviceFactory = serviceFactory;
        _unitOfWork = unitOfWork;
        _razorViewEngine = razorViewEngine;
        _webHostEnvironment = webHostEnvironment;
        _tempDataProvider = tempDataProvider;
        _httpContextAccessor = httpContextAccessor;
        _converter = converter;
        _serviceProvider = serviceProvider;
    }

    /// Validates if a student is eligible for a certificate
    private async Task<bool> IsEligibleForCertificateAsync(Guid? studentClassId, IUnitOfWork unitOfWork = null)
    {
        unitOfWork = unitOfWork ?? _unitOfWork;
        try
        {
            // Use AsNoTracking to avoid entity tracking issues
            var studentClass = await unitOfWork.StudentClassRepository.Entities
                .Include(sc => sc.Student)
                .Include(sc => sc.Class)
                .ThenInclude(c => c.Level)
                .Include(sc => sc.Class)
                .ThenInclude(c => c.Instructor)
                .FirstOrDefaultAsync(sc => sc.Id == studentClassId);

            if (studentClass == null)
            {
                throw new NotFoundException($"Student class with ID {studentClassId} not found");
            }

            if (studentClass.Class.Status != ClassStatus.Finished)
            {
                return false;
            }

            if (!studentClass.IsPassed)
            {
                return false;
            }

            return true;
        }
        catch (Exception ex) when (ex is not NotFoundException)
        {
            // Log the error but don't throw - this helps with debugging
            Console.WriteLine(
                $"Error checking certificate eligibility for student class {studentClassId}: {ex.Message}");
            throw;
        }
    }
    
    //Generate a Certificate for a student class and returns the certificate URL
    public async Task<string> GenerateCertificateAsync(Guid studentClassId)
    {
        using var scope = _serviceProvider.CreateScope();
    
        try
        {
            // Get all required services from the new scope
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var pinataService = scope.ServiceProvider.GetRequiredService<IPinataService>();
        
            if (!await IsEligibleForCertificateAsync(studentClassId, unitOfWork))
            {
                throw new BadRequestException("Student is not eligible for a certificate");
            }
            
            var studentClass = await unitOfWork.StudentClassRepository.Entities
                .Include(sc => sc.Student)
                .Include(sc => sc.Class)
                .ThenInclude(c => c.Level)
                .Include(sc => sc.Class)
                .ThenInclude(c => c.Instructor)
                .FirstOrDefaultAsync(sc => sc.Id == studentClassId);

            if (studentClass == null)
            {
                throw new NotFoundException($"Student class with ID {studentClassId} not found");
            }

            // Create certificate model
            var certificateModel = new CertificateModel
            {
                StudentName = studentClass.Student.FullName ?? studentClass.Student.UserName ?? "Unknown",
                ClassName = studentClass.Class.Name,
                LevelName = studentClass.Class.Level.Name,
                CompletionDate = DateTime.UtcNow.AddHours(7), // Vietnam time
                GPA = studentClass.GPA ?? 0m, // Correctly handle nullable decimal
                IsPassed = studentClass.IsPassed,
                CertificateId = $"CERT-{DateTime.UtcNow.Year}-{studentClassId.ToString().Substring(0, 8).ToUpper()}",
                SkillsEarned =
                    studentClass.Class.Level.SkillsEarned, // Direct assignment since it's already List<string>
                InstructorName = studentClass.Class.Instructor?.FullName ??
                                 studentClass.Class.Instructor?.UserName ?? "Unknown",
                InstructorSignatureUrl = studentClass.Class.Instructor?.AvatarUrl ?? ""
            };

            // Generate HTML
            var certificateHtml = await RenderViewToStringAsync("Certificate", certificateModel);

            // Generate image directly from HTML (skipping PDF generation)
            var imageBytes = await GenerateImageFromHtml(certificateHtml);

            // Create temporary file
            var certificateFileName = $"certificate_{studentClassId}.png";
            var tempFilePath = Path.Combine(Path.GetTempPath(), certificateFileName);

            try
            {
                await File.WriteAllBytesAsync(tempFilePath, imageBytes);

                // Create FormFile from the temp file
                using var fileStream = new FileStream(tempFilePath, FileMode.Open, FileAccess.Read);
                var formFile = new FormFile(
                    fileStream,
                    0,
                    fileStream.Length,
                    "certificate",
                    certificateFileName)
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "image/png"
                };

                var pinataUrl = await pinataService.UploadFile(formFile, certificateFileName);

                // Update student class with Piñata URL
                var studentClassToUpdate = await _unitOfWork.StudentClassRepository.GetByIdAsync(studentClassId);
                if (studentClassToUpdate != null)
                {
                    studentClassToUpdate.CertificateUrl = pinataUrl;
                    await _unitOfWork.StudentClassRepository.UpdateAsync(studentClassToUpdate);
                    await _unitOfWork.SaveChangesAsync();
                }

                return pinataUrl;
            }
            finally
            {
                // Clean up temp file
                if (File.Exists(tempFilePath))
                {
                    File.Delete(tempFilePath);
                }
            }
        }
        catch (Exception ex) when (ex is not BadRequestException && ex is not NotFoundException)
        {
            // Log the error but rethrow
            Console.WriteLine($"Error generating certificate for student class {studentClassId}: {ex.Message}");
            throw;
        }
    }


    //Renders a Razor view to string
    private async Task<string> RenderViewToStringAsync(string viewName, object model)
    {
        var actionContext = new ActionContext(
            new DefaultHttpContext { RequestServices = _httpContextAccessor.HttpContext.RequestServices },
            new RouteData(),
            new ActionDescriptor());
        using (var sw = new StringWriter())
        {
            var viewResult = _razorViewEngine.GetView("~/", $"Views/{viewName}.cshtml", false);
            if (viewResult.View == null)
            {
                throw new InvalidOperationException($"Could not find view '{viewName}'");
            }

            var viewDictionary =
                new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                {
                    Model = model
                };
            var viewContext = new ViewContext(
                actionContext,
                viewResult.View,
                viewDictionary,
                new TempDataDictionary(actionContext.HttpContext, _tempDataProvider),
                sw,
                new HtmlHelperOptions()
            );
            await viewResult.View.RenderAsync(viewContext);
            return sw.ToString();
        }
    }

    // Converts HTML to PDF
    // private async Task<byte[]> GeneratePdfFromHtml(string html)
    // {
    //     try
    //     {
    //         // Download Chrome browser if needed
    //         var browserFetcher = new BrowserFetcher();
    //         await browserFetcher.DownloadAsync();
    //     
    //         // Launch browser
    //         using var browser = await Puppeteer.LaunchAsync(new LaunchOptions 
    //         { 
    //             Headless = true,
    //             Args = new[] { "--no-sandbox", "--disable-setuid-sandbox" }
    //         });
    //     
    //         // Open new page
    //         using var page = await browser.NewPageAsync();
    //     
    //         // Set content - using the correct enum value for current versions
    //         await page.SetContentAsync(html, new NavigationOptions 
    //         { 
    //             WaitUntil = new[] { WaitUntilNavigation.Networkidle0 } 
    //             // Alternative options: Networkidle0, Networkidle2, Load, DOMContentLoaded
    //         });
    //     
    //         // Generate PDF
    //         var pdfBytes = await page.PdfDataAsync(new PdfOptions 
    //         { 
    //             Format = PaperFormat.A4,
    //             PrintBackground = true,
    //             MarginOptions = new MarginOptions
    //             {
    //                 Top = "20px",
    //                 Bottom = "20px",
    //                 Left = "20px",
    //                 Right = "20px"
    //             }
    //         });
    //     
    //         return pdfBytes;
    //     }
    //     catch (Exception ex)
    //     {
    //         Console.WriteLine($"Error generating PDF: {ex.Message}");
    //         throw;
    //     }
    // }

    // Converts a PDF to a high-quality image using PdfiumViewer
    // private async Task<byte[]> ConvertPdfToImage(byte[] pdfBytes)
    // {
    //     try
    //     {
    //         // Create a temporary PDF file
    //         string tempPdfPath = Path.Combine(Path.GetTempPath(), $"temp_pdf_{Guid.NewGuid()}.pdf");
    //         await File.WriteAllBytesAsync(tempPdfPath, pdfBytes);
    //     
    //         // Download Chrome browser if needed (can be moved to app startup)
    //         var browserFetcher = new BrowserFetcher();
    //         await browserFetcher.DownloadAsync();
    //     
    //         // Launch browser
    //         using var browser = await Puppeteer.LaunchAsync(new LaunchOptions 
    //         { 
    //             Headless = true,
    //             Args = new[] { "--no-sandbox", "--disable-setuid-sandbox" }
    //         });
    //     
    //         // Open new page
    //         using var page = await browser.NewPageAsync();
    //     
    //         // Navigate to the PDF file using file:// protocol
    //         await page.GoToAsync($"file://{tempPdfPath}");
    //     
    //         // Wait for PDF to render
    //         await Task.Delay(1000); // Give it a moment to render
    //     
    //         // Set viewport size to match PDF dimensions (A4 at 300 DPI)
    //         await page.SetViewportAsync(new ViewPortOptions
    //         {
    //             Width = 2480, // A4 width at 300 DPI (8.27 × 11.69 inches)
    //             Height = 3508, // A4 height at 300 DPI
    //             DeviceScaleFactor = 1
    //         });
    //     
    //         // Take a screenshot
    //         var screenshotBytes = await page.ScreenshotDataAsync(new ScreenshotOptions
    //         {
    //             FullPage = true,
    //             Type = ScreenshotType.Png,
    //             Quality = 100
    //         });
    //     
    //         // Clean up temp file
    //         if (File.Exists(tempPdfPath))
    //         {
    //             File.Delete(tempPdfPath);
    //         }
    //     
    //         return screenshotBytes;
    //     }
    //     catch (Exception ex)
    //     {
    //         Console.WriteLine($"Error converting PDF to image: {ex.Message}");
    //         throw;
    //     }
    // }
    //
    // // Helper method to get image encoder
    // private static ImageCodecInfo GetEncoder(ImageFormat format)
    // {
    //     var codecs = GetImageEncoders();
    //     foreach (var codec in codecs)
    //     {
    //         if (codec.FormatID == format.Guid)
    //         {
    //             return codec;
    //         }
    //     }
    //
    //     return null;
    // }

    private async Task<byte[]> GenerateImageFromHtml(string html)
    {
        try
        {
            // Download Chrome browser if needed (can be moved to app startup)
            var browserFetcher = new BrowserFetcher();
            await browserFetcher.DownloadAsync();
        
            // Launch browser
            using var browser = await Puppeteer.LaunchAsync(new LaunchOptions 
            { 
                Headless = true,
                Args = new[] { "--no-sandbox", "--disable-setuid-sandbox" }
            });
        
            // Open new page
            using var page = await browser.NewPageAsync();

            await page.SetViewportAsync(new ViewPortOptions
            {
                Width = 794,  
                Height = 1123,  
                DeviceScaleFactor = 1.5 
            });
        
            // Set content - use your existing HTML with base64 images working
            await page.SetContentAsync(html);
        
            // Wait for any images to load (using Task.Delay as a reliable alternative)
            await Task.Delay(1500);
        
            // Take screenshot with high quality
            var screenshotBytes = await page.ScreenshotDataAsync(new ScreenshotOptions
            {
                FullPage = true,
                Type = ScreenshotType.Png,
            });
        
            return screenshotBytes;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error generating image from HTML: {ex.Message}");
            throw;
        }
    }

    public async Task<List<CertificateInfoModel>> GetStudentCertificatesAsync(AccountModel account)
    {
        var studentClass = await _unitOfWork.StudentClassRepository.Entities
            .Include(sc => sc.Class)
            .ThenInclude(c => c.Level)
            .Where(sc => sc.StudentFirebaseId == account.AccountFirebaseId &&
                         sc.IsPassed &&
                         sc.CertificateUrl != null)
            .ToListAsync();

        return studentClass.Select(sc => new CertificateInfoModel
        {
            StudentClassId = sc.Id,
            ClassName = sc.Class.Name,
            LevelName = sc.Class.Level.Name,
            CompletionDate = sc.UpdatedAt ?? sc.CreatedAt,
            CertificateUrl = sc.CertificateUrl,
            GPA = sc.GPA ?? 0
        }).ToList();
    }

    public async Task<Dictionary<string, string>> GenerateCertificatesForClassAsync(Guid classId)
    {
        var eligibleStudents = await GetEligibleStudentsWithoutCertificatesAsync(classId);
        if (!eligibleStudents.Any())
        {
            return new Dictionary<string, string>();
        }

        var results = new Dictionary<string, string>();
        foreach (var studentClass in eligibleStudents)
        {
            try
            {
                string certificateUrl = await GenerateCertificateAsync(studentClass.Id);
                results.Add(studentClass.StudentFirebaseId, certificateUrl);
            }
            catch (Exception ex)
            {
                // Log error but continue with other students
                throw new Exception(ex.Message);
            }
        }

        return results;
    }

    public async Task<CertificateInfoModel> GetCertificateByIdAsync(Guid studentClassId)
    {
        var studentClass = await _unitOfWork.StudentClassRepository.Entities
            .Include(sc => sc.Student)
            .Include(sc => sc.Class)
            .ThenInclude(c => c.Level)
            .Include(sc => sc.Class)
            .ThenInclude(c => c.Instructor)
            .FirstOrDefaultAsync(sc => sc.Id == studentClassId);

        if (studentClass == null)
        {
            throw new NotFoundException($"Certificate with ID {studentClassId} not found");
        }

        if (string.IsNullOrEmpty(studentClass.CertificateUrl))
        {
            throw new NotFoundException($"No certificate found for student class with ID {studentClassId}");
        }

        return new CertificateInfoModel
        {
            StudentClassId = studentClass.Id,
            ClassName = studentClass.Class.Name,
            LevelName = studentClass.Class.Level.Name,
            CompletionDate = studentClass.UpdatedAt ?? studentClass.CreatedAt,
            CertificateUrl = studentClass.CertificateUrl,
            GPA = studentClass.GPA ?? 0,
            StudentName = studentClass.Student.FullName ?? studentClass.Student.UserName ?? "Unknown",
            InstructorName = studentClass.Class.Instructor?.FullName ??
                             studentClass.Class.Instructor?.UserName ?? "Unknown",
            SkillsEarned = studentClass.Class.Level.SkillsEarned
        };
    }
    
    public async Task<List<StudentClass>> GetEligibleStudentsWithoutCertificatesAsync(Guid classId)
    {
        // Get the class to check if it's finished
        var classInfo = await _unitOfWork.ClassRepository.GetByIdAsync(classId);
        if (classInfo == null)
        {
            throw new NotFoundException($"Class with ID {classId} not found");
        }

        // Class must be finished to generate certificates
        if (classInfo.Status != ClassStatus.Finished)
        {
            return new List<StudentClass>();
        }

        // Get all student classes for this class
        var studentClasses = await _unitOfWork.StudentClassRepository.Entities
            .Include(sc => sc.Student)
            .Include(sc => sc.Class)
            .ThenInclude(c => c.Level)
            .Where(sc =>
                sc.ClassId == classId &&
                sc.IsPassed &&
                sc.GPA >= sc.Class.Level.MinimumTheoreticalScore &&
                string.IsNullOrEmpty(sc.CertificateUrl))
            .ToListAsync();

        return studentClasses;
    }
}