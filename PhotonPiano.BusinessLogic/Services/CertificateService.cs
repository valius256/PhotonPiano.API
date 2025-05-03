using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Certificate;
using PhotonPiano.BusinessLogic.BusinessModel.Class;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.Shared.Exceptions;


namespace PhotonPiano.BusinessLogic.Services;

public class CertificateService : ICertificateService
{
    private readonly IServiceFactory _serviceFactory;
    private readonly IUnitOfWork _unitOfWork;


    private readonly ILogger<ServiceFactory> _logger;
    

    public CertificateService(IServiceFactory serviceFactory, IUnitOfWork unitOfWork, ILogger<ServiceFactory> logger)
    {
        _serviceFactory = serviceFactory;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// Validates if a student is eligible for a certificate
    private async Task<bool> IsEligibleForCertificateAsync(Guid? studentClassId)
    {
        try
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
                //throw new NotFoundException($"Student class with ID {studentClassId} not found");
                return false;
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
    public async Task<(string? certificateUrl, Guid studentClassId)> GenerateCertificateAsync(Guid studentClassId)
    {
        if (!await IsEligibleForCertificateAsync(studentClassId))
        {
            return (null, studentClassId);
        }

        var studentClass =
            await _unitOfWork.StudentClassRepository.FindFirstProjectedAsync<StudentClassDetailModel>(
                sc => sc.Id == studentClassId
            );

        if (studentClass is null)
        {
            return (null, studentClassId);
        }
        
        var certificateModel = new CertificateModel
        {
            StudentName = studentClass.Student.FullName ?? studentClass.Student.UserName ?? "Unknown",
            ClassName = studentClass.Class.Name,
            LevelName = studentClass.Class.Level!.Name,
            CompletionDate = DateTime.UtcNow.AddHours(7), // Vietnam time
            GPA = studentClass.GPA ?? 0m, 
            IsPassed = studentClass.IsPassed,
            CertificateId = $"CERT-{DateTime.UtcNow.Year}-{studentClassId.ToString().Substring(0, 8).ToUpper()}",
            SkillsEarned =
                studentClass.Class.Level.SkillsEarned,
            InstructorName = studentClass.Class.Instructor?.FullName ??
                             studentClass.Class.Instructor?.UserName ?? "Unknown",
            InstructorSignatureUrl = studentClass.Class.Instructor?.AvatarUrl ?? ""
        };

        // Generate HTML
        var certificateHtml =
            await _serviceFactory.ViewRenderService.RenderToStringAsync("Certificate", certificateModel);
        var studentClassEntity = await _unitOfWork.StudentClassRepository.GetByIdAsync(studentClassId);
        studentClassEntity!.CertificateHtml = certificateHtml;
        studentClassEntity.HasCertificateHtml = true;
        var certificateUrl = $"/endpoint/certificates/{studentClassId}/html";
        studentClassEntity.CertificateUrl = certificateUrl;
        await _unitOfWork.SaveChangesAsync();
        return (certificateUrl: certificateUrl, studentClassId: studentClassId);
    }
    
    public async Task AutoGenerateCertificatesAsync(Guid classId)
    {
        var studentClasses = await _unitOfWork.StudentClassRepository.FindAsync(
            sc => sc.ClassId == classId && 
                  sc.IsPassed == true && 
                  !sc.HasCertificateHtml);

        foreach (var studentClass in studentClasses)
        {
            var (url, _) = await GenerateCertificateAsync(studentClass.Id);
            studentClass.CertificateUrl = url;
        }

        await _unitOfWork.SaveChangesAsync();
    }

    private async Task<List<StudentClass>> GetEligibleStudentsWithoutCertificatesAsync(Guid classId)
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
                var (certificateUrl, _) = await GenerateCertificateAsync(studentClass.Id);
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
            CertificateHtml = studentClass.CertificateHtml,
            HasCertificateHtml = studentClass.HasCertificateHtml,
            GPA = studentClass.GPA ?? 0,
            StudentName = studentClass.Student.FullName ?? studentClass.Student.UserName ?? "Unknown",
            InstructorName = studentClass.Class.Instructor?.FullName ??
                             studentClass.Class.Instructor?.UserName ?? "Unknown",
            SkillsEarned = studentClass.Class.Level.SkillsEarned
        };
    }

    
}