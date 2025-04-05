using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using PhotonPiano.BusinessLogic.BusinessModel.Certificate;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.Shared.Exceptions;
using PaperKind = DinkToPdf.PaperKind;
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

    public CertificateService(IServiceFactory serviceFactory, IUnitOfWork unitOfWork, IRazorViewEngine razorViewEngine,
        IWebHostEnvironment webHostEnvironment, ITempDataProvider tempDataProvider,
        IHttpContextAccessor httpContextAccessor, IConverter converter)
    {
        _serviceFactory = serviceFactory;
        _unitOfWork = unitOfWork;
        _razorViewEngine = razorViewEngine;
        _webHostEnvironment = webHostEnvironment;
        _tempDataProvider = tempDataProvider;
        _httpContextAccessor = httpContextAccessor;
        _converter = converter;
    }

    /// Validates if a student is eligible for a certificate
    public async Task<bool> IsEligibleForCertificateAsync(Guid? studentClassId)
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

        var minimumGpa = studentClass.Class.Level.MinimumTheoreticalScore;
        if (studentClass.GPA < minimumGpa)
        {
            return false;
        }

        return true;
    }

    //Generate a Certificate for a student class and returns the certificate URL
    public async Task<string> GenerateCertificateAsync(Guid studentClassId)
    {
        if (!await IsEligibleForCertificateAsync(studentClassId))
        {
            throw new BadRequestException("Student is not eligible for a certificate");
        }

        var studentClass = await _unitOfWork.StudentClassRepository.Entities
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
            GPA = studentClass.GPA ?? 0m,  // Correctly handle nullable decimal
            IsPassed = studentClass.IsPassed,
            CertificateId = $"CERT-{DateTime.UtcNow.Year}-{studentClassId.ToString().Substring(0, 8).ToUpper()}",
            SkillsEarned = studentClass.Class.Level.SkillsEarned, // Direct assignment since it's already List<string>
            InstructorName = studentClass.Class.Instructor?.FullName ?? studentClass.Class.Instructor?.UserName ?? "Unknown",
            InstructorSignatureUrl = studentClass.Class.Instructor?.AvatarUrl ?? ""
        };
        
        var certificateHtml = await RenderViewToStringAsync("Certificate", certificateModel);
        // Convert HTML to PDF
        var pdfBytes = GeneratePdfFromHtml(certificateHtml);

        // Save PDF to file system
        string certificateFileName = $"certificate_{studentClassId}.pdf";
        string tempFilePath = Path.Combine(Path.GetTempPath(), certificateFileName);
            
        // // Create directory if it doesn't exist
        // if (!Directory.Exists(certificatesDirectory))
        // {
        //     Directory.CreateDirectory(certificatesDirectory);
        // }
        //     
        // var certificatePath = Path.Combine(certificatesDirectory, certificateFileName);
        // await File.WriteAllBytesAsync(certificatePath, pdfBytes);
        //
        // // Generate URL
        // var certificateUrl = $"/certificates/{certificateFileName}";
        //
        // // Update student class with certificate URL
        // studentClass.CertificateUrl = certificateUrl;
        // await _unitOfWork.StudentClassRepository.UpdateAsync(studentClass);
        // await _unitOfWork.SaveChangesAsync();
        //
        // return certificateUrl;
        try
        {
            await File.WriteAllBytesAsync(tempFilePath, pdfBytes); 
            using var fileStream = new FileStream(tempFilePath, FileMode.Create);
            var formFile = new FormFile(
                fileStream,
                0,
                fileStream.Length,
                "certificate",
                certificateFileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/pdf"
            };
            var pinataUrl = await _serviceFactory.PinataService.UploadFile(formFile, certificateFileName);
            studentClass.CertificateUrl = pinataUrl;
            await _unitOfWork.StudentClassRepository.UpdateAsync(studentClass);
            await _unitOfWork.SaveChangesAsync();
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
    private byte[] GeneratePdfFromHtml(string html)
    {
        // Existing implementation
        var globalSettings = new GlobalSettings
        {
            ColorMode = ColorMode.Color,
            Orientation = Orientation.Portrait,
            PaperSize = PaperKind.A4,
            Margins = new MarginSettings { Top = 10, Bottom = 10, Left = 10, Right = 10 },
            DocumentTitle = "Certificate of Completion"
        };
        var objectSettings = new ObjectSettings
        {
            PagesCount = true,
            HtmlContent = html,
            WebSettings = { DefaultEncoding = "utf-8" },
            HeaderSettings = { FontName = "Arial", FontSize = 9, Right = "Page [page] of [toPage]", Line = false },
            FooterSettings =
                { FontName = "Arial", FontSize = 9, Line = false, Center = "Certificate generated by Photon Piano" }
        };

        var pdf = new HtmlToPdfDocument()
        {
            GlobalSettings = globalSettings,
            Objects = { objectSettings }
        };

        return _converter.Convert(pdf);
    }   
}