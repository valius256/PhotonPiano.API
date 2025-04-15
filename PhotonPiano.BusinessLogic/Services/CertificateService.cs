using System.Drawing;
using System.Drawing.Imaging;
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
using PdfiumViewer;
using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Certificate;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.Shared.Exceptions;
using ColorMode = DinkToPdf.ColorMode;
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
    private async Task<bool> IsEligibleForCertificateAsync(Guid? studentClassId)
    {
        try
        {
            // Use AsNoTracking to avoid entity tracking issues
            var studentClass = await _unitOfWork.StudentClassRepository.Entities
                .AsNoTracking()
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


    //Generate a Certificate for a student class and returns the certificate URL
    public async Task<string> GenerateCertificateAsync(Guid studentClassId)
    {
        try
        {
            if (!await IsEligibleForCertificateAsync(studentClassId))
            {
                throw new BadRequestException("Student is not eligible for a certificate");
            }

            // Use AsNoTracking for the initial query to avoid tracking issues
            var studentClass = await _unitOfWork.StudentClassRepository.Entities
                .AsNoTracking()
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

            // Generate HTML and PDF
            var certificateHtml = await RenderViewToStringAsync("Certificate", certificateModel);
            var pdfBytes = GeneratePdfFromHtml(certificateHtml);

            // Convert PDF to high-quality image
            var imageBytes = await ConvertPdfToImageWithPdfiumViewer(pdfBytes);

            // Create temporary files
            string certificateFileName = $"certificate_{studentClassId}.png";
            string tempFilePath = Path.Combine(Path.GetTempPath(), certificateFileName);

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

                // Upload to Pinata
                var pinataUrl = await _serviceFactory.PinataService.UploadFile(formFile, certificateFileName);

                // Get a fresh instance of the student class for updating
                var studentClassToUpdate = await _unitOfWork.StudentClassRepository.GetByIdAsync(studentClassId);
                if (studentClassToUpdate != null)
                {
                    // Update student class with Piñata URL
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

    // Converts a PDF to a high-quality image using PdfiumViewer
    private async Task<byte[]> ConvertPdfToImageWithPdfiumViewer(byte[] pdfBytes)
    {
        return await Task.Run(() =>
        {
            // Use a high DPI for better quality
            const float dpi = 300f;

            using var stream = new MemoryStream(pdfBytes);
            using var pdfDocument = PdfDocument.Load(stream);

            // Get the size of the first page
            var pageSize = pdfDocument.PageSizes[0];

            // Calculate the image dimensions based on DPI
            int width = (int)(pageSize.Width / 72.0f * dpi);
            int height = (int)(pageSize.Height / 72.0f * dpi);

            // Render the first page to an image
            using var image = pdfDocument.Render(0, width, height, dpi, dpi, PdfRenderFlags.Annotations);

            // Enhance the image quality
            using var enhancedImage = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            using var graphics = Graphics.FromImage(enhancedImage);

            // Set high quality rendering
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

            // Fill with white background
            graphics.FillRectangle(Brushes.White, 0, 0, width, height);

            // Draw the PDF image
            graphics.DrawImage(image, 0, 0, width, height);

            // Add a subtle border
            int borderWidth = 2;
            graphics.DrawRectangle(new Pen(Color.FromArgb(230, 230, 230), borderWidth),
                borderWidth / 2, borderWidth / 2, width - borderWidth, height - borderWidth);

            // Convert to PNG
            using var memoryStream = new MemoryStream();
            enhancedImage.Save(memoryStream, ImageFormat.Png);
            return memoryStream.ToArray();
        });
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
}