using System.Linq.Expressions;
using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Class;
using PhotonPiano.BusinessLogic.BusinessModel.Payment;
using PhotonPiano.BusinessLogic.BusinessModel.SystemConfig;
using PhotonPiano.BusinessLogic.BusinessModel.Tuition;
using PhotonPiano.BusinessLogic.BusinessModel.Tution;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.BusinessLogic.Services;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.Shared.Exceptions;

namespace PhotonPiano.Test.UnitTest.Tuition;

[Collection("Tuition Unit Tests")]
public class TuitionServiceTest
{
    private readonly Mock<IAccountRepository> _accountRepositoryMock;
    private readonly Mock<IClassRepository> _classRepositoryMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
    private readonly Mock<ILevelRepository> _levelRepositoryMock;
    private readonly Mock<INotificationService> _notificationServiceMock;
    private readonly Mock<IPaymentService> _paymentServiceMock;
    private readonly Mock<IServiceFactory> _serviceFactoryMock;
    private readonly Mock<ISlotRepository> _slotRepositoryMock;
    private readonly Mock<IStudentClassRepository> _studentClassRepositoryMock;
    private readonly Mock<ISystemConfigService> _systemConfigServiceMock;
    private readonly Mock<ITransactionRepository> _transactionRepositoryMock;
    private readonly Mock<ITransactionService> _transactionServiceMock;
    private readonly Mock<ITuitionRepository> _tuitionRepositoryMock;

    private readonly TuitionService _tuitionService;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public TuitionServiceTest()
    {
        _unitOfWorkMock = _fixture.Freeze<Mock<IUnitOfWork>>();
        _serviceFactoryMock = _fixture.Freeze<Mock<IServiceFactory>>();
        _tuitionRepositoryMock = _fixture.Freeze<Mock<ITuitionRepository>>();
        _transactionRepositoryMock = _fixture.Freeze<Mock<ITransactionRepository>>();
        _accountRepositoryMock = _fixture.Freeze<Mock<IAccountRepository>>();
        _studentClassRepositoryMock = _fixture.Freeze<Mock<IStudentClassRepository>>();
        _levelRepositoryMock = _fixture.Freeze<Mock<ILevelRepository>>();
        _classRepositoryMock = _fixture.Freeze<Mock<IClassRepository>>();
        _slotRepositoryMock = _fixture.Freeze<Mock<ISlotRepository>>();
        _paymentServiceMock = _fixture.Freeze<Mock<IPaymentService>>();
        _emailServiceMock = _fixture.Freeze<Mock<IEmailService>>();
        _transactionServiceMock = _fixture.Freeze<Mock<ITransactionService>>();
        _notificationServiceMock = _fixture.Freeze<Mock<INotificationService>>();
        _systemConfigServiceMock = _fixture.Freeze<Mock<ISystemConfigService>>();

        // Setup UnitOfWork repositories
        _unitOfWorkMock.Setup(uow => uow.TuitionRepository).Returns(_tuitionRepositoryMock.Object);
        _unitOfWorkMock.Setup(uow => uow.TransactionRepository).Returns(_transactionRepositoryMock.Object);
        _unitOfWorkMock.Setup(uow => uow.AccountRepository).Returns(_accountRepositoryMock.Object);
        _unitOfWorkMock.Setup(uow => uow.StudentClassRepository).Returns(_studentClassRepositoryMock.Object);
        _unitOfWorkMock.Setup(uow => uow.LevelRepository).Returns(_levelRepositoryMock.Object);
        _unitOfWorkMock.Setup(uow => uow.ClassRepository).Returns(_classRepositoryMock.Object);
        _unitOfWorkMock.Setup(uow => uow.SlotRepository).Returns(_slotRepositoryMock.Object);

        // Setup ServiceFactory services
        _serviceFactoryMock.Setup(sf => sf.PaymentService).Returns(_paymentServiceMock.Object);
        _serviceFactoryMock.Setup(sf => sf.EmailService).Returns(_emailServiceMock.Object);
        _serviceFactoryMock.Setup(sf => sf.TransactionService).Returns(_transactionServiceMock.Object);
        _serviceFactoryMock.Setup(sf => sf.NotificationService).Returns(_notificationServiceMock.Object);
        _serviceFactoryMock.Setup(sf => sf.SystemConfigService).Returns(_systemConfigServiceMock.Object);

        _tuitionService = new TuitionService(_unitOfWorkMock.Object, _serviceFactoryMock.Object);
    }

    [Fact]
    public async Task PayTuition_ValidRequest_ReturnsPaymentUrl()
    {
        // Arrange
        var currentAccount = new AccountModel { AccountFirebaseId = "student123", Email = "student@example.com" };
        var tuitionId = Guid.NewGuid();
        var returnUrl = "https://example.com/return";
        var ipAddress = "127.0.0.1";
        var apiBaseUrl = "https://api.example.com";
        var studentClassId = Guid.NewGuid();

        var tuition = new DataAccess.Models.Entity.Tuition
        {
            Id = tuitionId,
            StudentClassId = Guid.NewGuid(),
            Amount = 1000m,
            PaymentStatus = PaymentStatus.Pending,
            StudentClass = new StudentClass
                { StudentFirebaseId = "student123", CreatedById = "admin001", Id = studentClassId }
        };

        var systemConfig = new SystemConfigModel
            { ConfigValue = "0.1", ConfigName = "TuitionTaxRate", Id = Guid.NewGuid() };
        var transactionCode = "TRANS-123";
        var paymentUrl = "https://payment.example.com/url";

        _tuitionRepositoryMock
            .Setup(r => r.FindSingleProjectedAsync<DataAccess.Models.Entity.Tuition>(
                It.IsAny<Expression<Func<DataAccess.Models.Entity.Tuition, bool>>>(), false, false,
                TrackingOption.Default))
            .ReturnsAsync(tuition);

        _systemConfigServiceMock
            .Setup(s => s.GetTaxesRateConfig(It.IsAny<int>()))
            .ReturnsAsync(systemConfig);

        _transactionServiceMock
            .Setup(s => s.GetTransactionCode(It.IsAny<TransactionType>(), It.IsAny<DateTime>(), It.IsAny<Guid>()))
            .Returns(transactionCode);

        _paymentServiceMock
            .Setup(s => s.CreateVnPayPaymentUrl(It.IsAny<Transaction>(), ipAddress, apiBaseUrl,
                currentAccount.AccountFirebaseId, returnUrl, It.IsAny<string>()))
            .Returns(paymentUrl);

        var paymentModel = new PayTuitionModel
        {
            ApiBaseUrl = apiBaseUrl,
            IpAddress = ipAddress,
            ReturnUrl = returnUrl,
            TuitionId = tuitionId
        };
        // Act
        var result = await _tuitionService.PayTuition(currentAccount, paymentModel);

        // Assert
        Assert.Equal(paymentUrl, result);
        _transactionRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Transaction>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task PayTuition_TuitionNotFound_ThrowsNullReferenceException()
    {
        // Arrange
        var currentAccount = new AccountModel { AccountFirebaseId = "student123", Email = "student@example.com" };
        var tuitionId = Guid.NewGuid();

        _tuitionRepositoryMock
            .Setup(r => r.FindFirstProjectedAsync<DataAccess.Models.Entity.Tuition>(
                It.IsAny<Expression<Func<DataAccess.Models.Entity.Tuition, bool>>>(), false, false,
                TrackingOption.Default))
            .ReturnsAsync((DataAccess.Models.Entity.Tuition)null);


        var paymentModel = new PayTuitionModel
        {
            ApiBaseUrl = "",
            IpAddress = "",
            ReturnUrl = "",
            TuitionId = tuitionId
        };

        // Act & Assert
        await Assert.ThrowsAsync<NullReferenceException>(() =>
            _tuitionService.PayTuition(currentAccount, paymentModel));
    }


    [Fact]
    public async Task PayTuition_ValidTuition_ReturnsPaymentUrl()
    {
        // Arrange
        var tuitionId = Guid.NewGuid();
        var accountId = "student-id";
        var account = new AccountModel { AccountFirebaseId = accountId, Email = "student@example.com" };
        var returnUrl = "https://example.com/return";
        var ipAddress = "127.0.0.1";
        var apiBaseUrl = "https://api.example.com";
        var expectedPaymentUrl = "https://payment.example.com/pay";

        var tuition = new DataAccess.Models.Entity.Tuition
        {
            Id = tuitionId,
            StudentClassId = Guid.NewGuid(),
            Amount = 100m,
            PaymentStatus = PaymentStatus.Pending,
            StudentClass = new StudentClass
                { StudentFirebaseId = accountId, Id = Guid.NewGuid(), CreatedById = "admin001" }
        };

        var taxRate = 0.1;
        var systemConfig = new SystemConfigModel
            { ConfigValue = taxRate.ToString(), Id = Guid.NewGuid(), ConfigName = "TuitionTaxRate" };

        _tuitionRepositoryMock.Setup(repo => repo.FindSingleProjectedAsync<DataAccess.Models.Entity.Tuition>(
                It.IsAny<Expression<Func<DataAccess.Models.Entity.Tuition, bool>>>(), false, false,
                TrackingOption.Default))
            .ReturnsAsync(tuition);

        _systemConfigServiceMock.Setup(service => service.GetTaxesRateConfig(It.IsAny<int>()))
            .ReturnsAsync(systemConfig);

        _transactionServiceMock.Setup(service => service.GetTransactionCode(
                It.IsAny<TransactionType>(), It.IsAny<DateTime>(), It.IsAny<Guid>()))
            .Returns("TR123456");

        _paymentServiceMock.Setup(service => service.CreateVnPayPaymentUrl(
                It.IsAny<Transaction>(), ipAddress, apiBaseUrl, accountId, returnUrl, It.IsAny<string>()))
            .Returns(expectedPaymentUrl);


        var paymentModel = new PayTuitionModel
        {
            ApiBaseUrl = apiBaseUrl,
            IpAddress = ipAddress,
            ReturnUrl = returnUrl,
            TuitionId = tuitionId
        };

        // Act
        var result = await _tuitionService.PayTuition(account, paymentModel);

        // Assert
        Assert.Equal(expectedPaymentUrl, result);
        _transactionRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Transaction>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task PayTuition_AlreadyPaid_ThrowsBadRequestException()
    {
        // Arrange
        var currentAccount = new AccountModel { AccountFirebaseId = "student123", Email = "student@example.com" };
        var tuitionId = Guid.NewGuid();

        var tuition = new DataAccess.Models.Entity.Tuition
        {
            Id = tuitionId,
            StudentClassId = Guid.NewGuid(),
            Amount = 1000m,
            PaymentStatus = PaymentStatus.Succeed,
            StudentClass = new StudentClass
                { StudentFirebaseId = "student123", CreatedById = "admin001", Id = Guid.NewGuid() }
        };

        _tuitionRepositoryMock
            .Setup(r => r.FindSingleProjectedAsync<DataAccess.Models.Entity.Tuition>(
                It.IsAny<Expression<Func<DataAccess.Models.Entity.Tuition, bool>>>(), false, false,
                TrackingOption.Default))
            .ReturnsAsync(tuition);

        _systemConfigServiceMock
            .Setup(s => s.GetTaxesRateConfig(It.IsAny<int>()))
            .ReturnsAsync(
                new SystemConfigModel { ConfigValue = "0.1", Id = Guid.NewGuid(), ConfigName = "TuitionRate" });


        var paymentModel = new PayTuitionModel
        {
            ApiBaseUrl = "",
            IpAddress = "",
            ReturnUrl = "",
            TuitionId = tuitionId
        };


        // Act & Assert
        await Assert.ThrowsAsync<IllegalArgumentException>(() =>
            _tuitionService.PayTuition(currentAccount, paymentModel));
    }

    [Fact]
    public async Task PayTuition_DifferentStudent_ThrowsBadRequestException()
    {
        // Arrange
        var currentAccount = new AccountModel { AccountFirebaseId = "student123", Email = "student@example.com" };
        var tuitionId = Guid.NewGuid();

        var tuition = new DataAccess.Models.Entity.Tuition
        {
            Id = tuitionId,
            StudentClassId = Guid.NewGuid(),
            Amount = 1000m,
            PaymentStatus = PaymentStatus.Pending,
            StudentClass = new StudentClass
                { StudentFirebaseId = "student456", CreatedById = "admin001", Id = Guid.NewGuid() }
        };

        _tuitionRepositoryMock
            .Setup(r => r.FindSingleProjectedAsync<DataAccess.Models.Entity.Tuition>(
                It.IsAny<Expression<Func<DataAccess.Models.Entity.Tuition, bool>>>(), false, false,
                TrackingOption.Default))
            .ReturnsAsync(tuition);

        _systemConfigServiceMock
            .Setup(s => s.GetTaxesRateConfig(It.IsAny<int>()))
            .ReturnsAsync(
                new SystemConfigModel { ConfigValue = "0.1", Id = Guid.NewGuid(), ConfigName = "TuitionRate" });

        var paymentModel = new PayTuitionModel
        {
            ApiBaseUrl = "",
            IpAddress = "",
            ReturnUrl = "",
            TuitionId = tuitionId
        };

        // Act & Assert
        await Assert.ThrowsAsync<IllegalArgumentException>(() =>
            _tuitionService.PayTuition(currentAccount, paymentModel));
    }

    [Fact]
    public async Task HandleTuitionPaymentCallback_SuccessfulPayment_UpdatesStatusAndSendsEmail()
    {
        // Arrange
        var callbackModel = new VnPayCallbackModel
        {
            VnpTxnRef = Guid.NewGuid().ToString(),
            VnpResponseCode = "00",
            VnpTransactionNo = "TRANS-123",
            VnpAmount = "30000",
            VnpSecureHash = Guid.NewGuid().ToString()
        };
        var accountId = "student123";

        var transactionId = Guid.Parse(callbackModel.VnpTxnRef);
        var tuitionId = Guid.NewGuid();

        var tuition = new DataAccess.Models.Entity.Tuition
        {
            Id = tuitionId,
            StudentClassId = Guid.NewGuid(),
            PaymentStatus = PaymentStatus.Pending,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddMonths(1)
        };

        var transaction = new Transaction
        {
            Id = transactionId,
            CreatedById = "admin001",
            CreatedByEmail = "admin@gmail.com",
            TutionId = tuitionId,
            PaymentStatus = PaymentStatus.Pending,
            PaymentMethod = PaymentMethod.VnPay,
            Amount = 1000m,
            Tution = tuition
        };

        var account = new Account
        {
            AccountFirebaseId = accountId,
            Email = "learner012@gmail.com"
        };


        _transactionRepositoryMock
            .Setup(r => r.FindSingleProjectedAsync<Transaction>(It.IsAny<Expression<Func<Transaction, bool>>>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<TrackingOption>()))
            .ReturnsAsync(transaction);

        _accountRepositoryMock
            .Setup(r => r.FindSingleAsync(It.IsAny<Expression<Func<Account, bool>>>(),
                It.IsAny<bool>(),
                It.IsAny<bool>()))
            .ReturnsAsync(account);

        _tuitionRepositoryMock
            .Setup(r => r.FindFirstAsync(It.IsAny<Expression<Func<DataAccess.Models.Entity.Tuition, bool>>>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                null,
                It.IsAny<bool>()))
            .ReturnsAsync(tuition);

        _unitOfWorkMock
            .Setup(uow => uow.BeginTransactionAsync())
            .ReturnsAsync(Mock.Of<IDbContextTransaction>());

        // Act
        await _tuitionService.HandleTuitionPaymentCallback(callbackModel, accountId);

        // Assert
        Assert.Equal(PaymentStatus.Succeed, transaction.PaymentStatus);
        Assert.Equal(callbackModel.VnpTransactionNo, transaction.TransactionCode);

        _emailServiceMock.Verify(s => s.SendAsync(
                "PaymentSuccess",
                It.IsAny<List<string>>(),
                null,
                It.IsAny<Dictionary<string, string>>(),
                false),
            Times.Once);
    }

    [Fact]
    public async Task HandleTuitionPaymentCallback_SuccessfulPayment_UpdatesTuitionAndTransaction()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var tuitionId = Guid.NewGuid();
        var accountId = "student-id";
        var email = "student@example.com";

        var callbackModel = new VnPayCallbackModel
        {
            VnpTxnRef = transactionId.ToString(),
            VnpAmount = "100000",
            VnpSecureHash = Guid.NewGuid().ToString(),
            VnpResponseCode = "00", // Success code
            VnpTransactionNo = "TNX123456"
        };

        var tuition = new DataAccess.Models.Entity.Tuition
        {
            Id = tuitionId,
            StudentClassId = Guid.NewGuid(),
            Amount = 1m,
            PaymentStatus = PaymentStatus.Pending
        };

        var transaction = new Transaction
        {
            Id = transactionId,
            TutionId = tuitionId,
            Tution = tuition,
            CreatedByEmail = "test@gmail",
            CreatedById = "admin001",
            PaymentStatus = PaymentStatus.Pending,
            PaymentMethod = PaymentMethod.VnPay,
            Amount = 100m
        };


        var account = new Account
        {
            AccountFirebaseId = accountId,
            Email = email
        };

        _transactionRepositoryMock.Setup(repo => repo.FindSingleProjectedAsync<Transaction>(
                It.IsAny<Expression<Func<Transaction, bool>>>(), It.IsAny<bool>(), It.IsAny<bool>(),
                It.IsAny<TrackingOption>()))
            .ReturnsAsync(transaction);

        _accountRepositoryMock.Setup(repo => repo.FindSingleAsync(
                It.IsAny<Expression<Func<Account, bool>>>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(account);

        _tuitionRepositoryMock.Setup(repo => repo.FindFirstAsync(
                It.IsAny<Expression<Func<DataAccess.Models.Entity.Tuition, bool>>>(), It.IsAny<bool>(),
                It.IsAny<bool>(), It.IsAny<Expression<Func<DataAccess.Models.Entity.Tuition, object>>?>(),
                It.IsAny<bool>()))
            .ReturnsAsync(tuition);

        _unitOfWorkMock.Setup(uow => uow.BeginTransactionAsync())
            .ReturnsAsync(Mock.Of<IDbContextTransaction>());

        // Act
        await _tuitionService.HandleTuitionPaymentCallback(callbackModel, accountId);

        // Assert
        Assert.Equal(PaymentStatus.Succeed, transaction.PaymentStatus);

        _emailServiceMock.Verify(service => service.SendAsync(
                It.IsAny<string>(), It.Is<List<string>>(e => e.Contains(email)),
                It.IsAny<List<string>>(), It.IsAny<Dictionary<string, string>>(), false),
            Times.Once);
    }

    [Fact]
    public async Task HandleTuitionPaymentCallback_TransactionNotFound_ThrowsPaymentRequiredException()
    {
        // Arrange
        var callbackModel = new VnPayCallbackModel
        {
            VnpTxnRef = Guid.NewGuid().ToString(),
            VnpResponseCode = "00",
            VnpAmount = "30000",
            VnpSecureHash = Guid.NewGuid().ToString(),
            VnpTransactionNo = "TRANS-123"
        };
        var accountId = "student123";

        _transactionRepositoryMock
            .Setup(r => r.FindSingleProjectedAsync<Transaction>(It.IsAny<Expression<Func<Transaction, bool>>>(), false,
                false, TrackingOption.Default))
            .ReturnsAsync((Transaction)null);

        // Act & Assert
        await Assert.ThrowsAsync<PaymentRequiredException>(() =>
            _tuitionService.HandleTuitionPaymentCallback(callbackModel, accountId));
    }

    [Fact]
    public async Task HandleTuitionPaymentCallback_AccountNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var callbackModel = new VnPayCallbackModel
        {
            VnpTxnRef = Guid.NewGuid().ToString(),
            VnpResponseCode = "00",
            VnpAmount = "30000",
            VnpSecureHash = Guid.NewGuid().ToString(),
            VnpTransactionNo = "TRANS-123"
        };

        var accountId = "student123";

        var tuitionId = Guid.NewGuid();

        var tuitionModel = new DataAccess.Models.Entity.Tuition
        {
            Id = tuitionId,
            StudentClassId = Guid.NewGuid(),
            PaymentStatus = PaymentStatus.Pending
        };

        var transaction = new Transaction
        {
            Id = Guid.Parse(callbackModel.VnpTxnRef),
            CreatedById = "admin001",
            TutionId = tuitionId,
            Tution = tuitionModel,
            CreatedByEmail = "admin@gmail.com",
            PaymentStatus = PaymentStatus.Pending
        };

        _transactionRepositoryMock
            .Setup(r => r.FindSingleProjectedAsync<Transaction>(It.IsAny<Expression<Func<Transaction, bool>>>(),
                It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<TrackingOption>()))
            .ReturnsAsync(transaction);

        _accountRepositoryMock
            .Setup(r => r.FindSingleAsync(It.IsAny<Expression<Func<Account, bool>>>(), It.IsAny<bool>(),
                It.IsAny<bool>()))
            .ReturnsAsync((Account)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _tuitionService.HandleTuitionPaymentCallback(callbackModel, accountId));
    }

    [Fact]
    public async Task HandleTuitionPaymentCallback_FailedPayment_ThrowsBadRequestException()
    {
        // Arrange
        var callbackModel = new VnPayCallbackModel
        {
            VnpTxnRef = Guid.NewGuid().ToString(),
            VnpResponseCode = "99", // Failed
            VnpTransactionNo = "TRANS-123",
            VnpAmount = "30000",
            VnpSecureHash = Guid.NewGuid().ToString()
        };
        var accountId = "student123";

        var tuitionId = Guid.NewGuid();

        var tuitionModel = new DataAccess.Models.Entity.Tuition
        {
            Id = tuitionId,
            StudentClassId = Guid.NewGuid(),
            PaymentStatus = PaymentStatus.Pending
        };

        var transaction = new Transaction
        {
            Id = Guid.Parse(callbackModel.VnpTxnRef),
            CreatedByEmail = "admin001@gmail.com",
            CreatedById = "admin001",
            TutionId = tuitionId,
            Tution = tuitionModel,
            PaymentStatus = PaymentStatus.Pending
        };

        var account = new Account
        {
            AccountFirebaseId = accountId,
            Email = "learner012@gmail.com"
        };

        _transactionRepositoryMock
            .Setup(r => r.FindSingleProjectedAsync<Transaction>(It.IsAny<Expression<Func<Transaction, bool>>>(),
                It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<TrackingOption>()))
            .ReturnsAsync(transaction);

        _accountRepositoryMock
            .Setup(r => r.FindSingleAsync(It.IsAny<Expression<Func<Account, bool>>>(), It.IsAny<bool>(),
                It.IsAny<bool>()))
            .ReturnsAsync(account);

        _unitOfWorkMock
            .Setup(uow => uow.BeginTransactionAsync())
            .ReturnsAsync(Mock.Of<IDbContextTransaction>());

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() =>
            _tuitionService.HandleTuitionPaymentCallback(callbackModel, accountId));
    }

    [Fact]
    public async Task GetTuitionRefundAmount_CalculatesCorrectRefundAmount()
    {
        // Arrange
        var studentId = "student123";
        var classId = Guid.NewGuid();
        var currentTime = DateTime.UtcNow.AddHours(7);

        var studentClass = new StudentClass
        {
            Id = Guid.NewGuid(),
            StudentFirebaseId = studentId,
            CreatedById = "admin001",
            ClassId = classId,
            Class = new Class { Id = classId, LevelId = Guid.NewGuid(), Name = "Class ne", CreatedById = "admin001" }
        };

        var tuition = new DataAccess.Models.Entity.Tuition
        {
            Id = Guid.NewGuid(),
            StudentClassId = studentClass.Id,
            PaymentStatus = PaymentStatus.Succeed,
            Amount = 1000m
        };

        var level = new Level
        {
            Id = studentClass.Class.LevelId,
            PricePerSlot = 200m
        };

        var slotId1 = Guid.NewGuid();
        var slotId2 = Guid.NewGuid();

        var slots = new List<Slot>
        {
            new()
            {
                Id = slotId1,
                ClassId = classId,
                Date = new DateOnly(currentTime.Year, currentTime.Month, 15),
                SlotStudents = new List<SlotStudent>
                {
                    new()
                    {
                        SlotId = slotId1,
                        CreatedById = "admin001",
                        StudentFirebaseId = studentId,
                        AttendanceStatus = AttendanceStatus.Attended
                    }
                }
            },
            new()
            {
                Id = slotId2,
                ClassId = classId,
                Date = new DateOnly(currentTime.Year, currentTime.Month, 20),
                SlotStudents = new List<SlotStudent>
                {
                    new()
                    {
                        SlotId = slotId2,
                        CreatedById = "admin001",
                        StudentFirebaseId = studentId,
                        AttendanceStatus = AttendanceStatus.Attended
                    }
                }
            }
        };

        _studentClassRepositoryMock
            .Setup(r => r.FindSingleAsync(It.IsAny<Expression<Func<StudentClass, bool>>>(), It.IsAny<bool>(),
                It.IsAny<bool>()))
            .ReturnsAsync(studentClass);

        _tuitionRepositoryMock
            .Setup(r => r.FindSingleAsync(It.IsAny<Expression<Func<DataAccess.Models.Entity.Tuition, bool>>>(),
                It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(tuition);

        _slotRepositoryMock
            .Setup(r => r.FindProjectedAsync<Slot>(It.IsAny<Expression<Func<Slot, bool>>>(), It.IsAny<bool>(),
                It.IsAny<bool>(), TrackingOption.Default))
            .ReturnsAsync(slots);

        _levelRepositoryMock
            .Setup(r => r.FindSingleAsync(It.IsAny<Expression<Func<Level, bool>>>(), It.IsAny<bool>(),
                It.IsAny<bool>()))
            .ReturnsAsync(level);

        // Expected refund: tuition amount - (price per slot * attended slots)
        // 1000 - (200 * 2) = 600
        var expectedRefund = 600m;

        // Act
        var result = await _tuitionService.GetTuitionRefundAmount(studentId, classId);

        // Assert
        Assert.Equal(expectedRefund, result);
    }

    [Fact]
    public async Task GetTuitionRefundAmount_StudentNotInClass_ThrowsNotFoundException()
    {
        // Arrange
        var studentId = "student123";
        var classId = Guid.NewGuid();

        _studentClassRepositoryMock
            .Setup(r => r.FindSingleAsync(It.IsAny<Expression<Func<StudentClass, bool>>>(),
                It.IsAny<bool>(),
                It.IsAny<bool>()))
            .ReturnsAsync((StudentClass)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _tuitionService.GetTuitionRefundAmount(studentId, classId));
    }

    [Fact]
    public async Task GetTuitionRefundAmount_NoPaidTuition_ThrowsNotFoundException()
    {
        // Arrange
        var studentId = "student123";
        var classId = Guid.NewGuid();

        var studentClass = new StudentClass
        {
            Id = Guid.NewGuid(),
            CreatedById = "admin001",
            StudentFirebaseId = studentId,
            ClassId = classId
        };

        _studentClassRepositoryMock
            .Setup(r => r.FindSingleAsync(It.IsAny<Expression<Func<StudentClass, bool>>>(),
                It.IsAny<bool>(),
                It.IsAny<bool>()))
            .ReturnsAsync(studentClass);

        _tuitionRepositoryMock
            .Setup(r => r.FindSingleAsync(It.IsAny<Expression<Func<DataAccess.Models.Entity.Tuition, bool>>>(),
                It.IsAny<bool>(),
                It.IsAny<bool>()))
            .ReturnsAsync((DataAccess.Models.Entity.Tuition)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _tuitionService.GetTuitionRefundAmount(studentId, classId));
    }

    [Fact]
    public async Task CronAutoCreateTuition_CreatesAndSendsTuitionNotifications()
    {
        // Arrange
        var utcNow = DateTime.UtcNow.AddHours(7);
        var lastDay = DateTime.DaysInMonth(utcNow.Year, utcNow.Month);
        var endDate = new DateTime(utcNow.Year, utcNow.Month, lastDay, 23, 59, 59);

        // Ensure tuitions don't already exist
        _tuitionRepositoryMock
            .Setup(r => r.AnyAsync(It.IsAny<Expression<Func<DataAccess.Models.Entity.Tuition, bool>>>()))
            .ReturnsAsync(false);

        // Setup class with proper ID
        var classId = Guid.NewGuid();
        var levelId = Guid.NewGuid();
        var ongoingClass = new Class
        {
            Id = classId,
            CreatedById = "admin001",
            Status = ClassStatus.Ongoing,
            IsPublic = true,
            LevelId = levelId,
            Name = "Test Class"
        };

        // Setup student class with student info
        var studentClassId = Guid.NewGuid();
        var studentClass = new StudentClass
        {
            Id = studentClassId,
            CreatedById = "admin001",

            StudentFirebaseId = "student123",
            ClassId = classId,
            Class = ongoingClass,
            Student = new Account
            {
                Email = "student@example.com",
                AccountFirebaseId = "student123"
            }
        };

        // Setup level with non-zero price
        var level = new Level
        {
            Id = levelId,
            PricePerSlot = 200m
        };

        // Setup slots in the current month
        var slots = new List<Slot>
        {
            new()
            {
                Id = Guid.NewGuid(),
                ClassId = classId,
                Date = new DateOnly(utcNow.Year, utcNow.Month, 15)
            }
        };

        // Link slots to class
        ongoingClass.Slots = slots;

        // Setup repository mocks
        _classRepositoryMock
            .Setup(r => r.FindProjectedAsync<Class>(It.IsAny<Expression<Func<Class, bool>>>(), It.IsAny<bool>(),
                It.IsAny<bool>(), It.IsAny<TrackingOption>()))
            .ReturnsAsync(new List<Class> { ongoingClass });

        _studentClassRepositoryMock
            .Setup(r => r.FindProjectedAsync<StudentClass>(It.IsAny<Expression<Func<StudentClass, bool>>>(),
                It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<TrackingOption>()))
            .ReturnsAsync(new List<StudentClass> { studentClass });

        _levelRepositoryMock
            .Setup(r => r.FindSingleProjectedAsync<Level>(It.IsAny<Expression<Func<Level, bool>>>(), It.IsAny<bool>(),
                It.IsAny<bool>(), It.IsAny<TrackingOption>()))
            .ReturnsAsync(level);

        // Setup transaction execution
        _unitOfWorkMock
            .Setup(uow => uow.ExecuteInTransactionAsync(It.IsAny<Func<Task>>()))
            .Returns(async (Func<Task> action) => await action());

        // Act
        await _tuitionService.CronAutoCreateTuition();

        // Assert
        _tuitionRepositoryMock.Verify(
            r => r.AddRangeAsync(It.Is<IEnumerable<DataAccess.Models.Entity.Tuition>>(tuitions => tuitions.Any())),
            Times.Once);

        _emailServiceMock.Verify(
            s => s.SendAsync(
                "NotifyTuitionCreated",
                It.IsAny<List<string>>(),
                null,
                It.IsAny<Dictionary<string, string>>(),
                false),
            Times.Once);
    }

    [Fact]
    public async Task CronAutoCreateTuition_TuitionAlreadyExists_DoesNotCreateNewTuition()
    {
        // Arrange
        var utcNow = DateTime.UtcNow.AddHours(7);

        var studentClassId = Guid.NewGuid();

        var studentClass = new StudentClass
        {
            Id = studentClassId,
            CreatedById = "admin001",
            StudentFirebaseId = "student123",

            Student = new Account
            {
                Email = "studentemail@gmail.com",
                AccountFirebaseId = "student123"
            },
            ClassId = Guid.NewGuid(),
            Class = new Class
            {
                Id = Guid.NewGuid(),
                Name = "Class test",
                CreatedById = "admin001"
            }
        };

        _studentClassRepositoryMock.Setup(sc =>
                sc.FindSingleAsync(sc => sc.Id == studentClassId, It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(studentClass);

        _tuitionRepositoryMock
            .Setup(r => r.AnyAsync(It.IsAny<Expression<Func<DataAccess.Models.Entity.Tuition, bool>>>()))
            .ReturnsAsync(true); // Tuition already exists


        // Act
        await _tuitionService.CronAutoCreateTuition();

        // Assert
        _classRepositoryMock.Verify(
            r => r.FindProjectedAsync<Class>(It.IsAny<Expression<Func<Class, bool>>>(), false, false,
                TrackingOption.Default),
            Times.Never);
        _tuitionRepositoryMock.Verify(r => r.AddRangeAsync(It.IsAny<IEnumerable<DataAccess.Models.Entity.Tuition>>()),
            Times.Never);
    }

    [Fact]
    public async Task CronForTuitionReminder_SendsReminderForPendingTuition()
    {
        // Arrange
        var pendingTuitions = new List<DataAccess.Models.Entity.Tuition>
        {
            new()
            {
                Id = Guid.NewGuid(),
                StudentClassId = Guid.NewGuid(),
                PaymentStatus = PaymentStatus.Pending,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(1),
                Amount = 1000m
            }
        };

        var studentClass = new StudentClass
        {
            Id = pendingTuitions[0].StudentClassId,
            StudentFirebaseId = "student123",
            CreatedById = "admin001",
            Student = new Account { Email = "learner012@gmail.com", AccountFirebaseId = "student123" },
            Class = new Class { Name = "Class 101", Id = Guid.NewGuid(), CreatedById = "admin001" }
        };

        _tuitionRepositoryMock
            .Setup(r => r.FindProjectedAsync<DataAccess.Models.Entity.Tuition>(
                It.IsAny<Expression<Func<DataAccess.Models.Entity.Tuition, bool>>>(),
                It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<TrackingOption>()))
            .ReturnsAsync(pendingTuitions);

        _studentClassRepositoryMock
            .Setup(r => r.FindSingleProjectedAsync<StudentClass>(It.IsAny<Expression<Func<StudentClass, bool>>>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(), It.IsAny<TrackingOption>()))
            .ReturnsAsync(studentClass);

        // Act
        await _tuitionService.CronForTuitionReminder();

        // Assert
        _emailServiceMock.Verify(s => s.SendAsync(
                "NotifyTuitionReminder",
                It.Is<List<string>>(emails => emails.Contains(studentClass.Student.Email)),
                It.IsAny<List<string>>(),
                It.IsAny<Dictionary<string, string>>(),
                It.IsAny<bool>()),
            Times.Once);

        _notificationServiceMock.Verify(s => s.SendNotificationAsync(
                studentClass.Student.AccountFirebaseId,
                It.Is<string>(body => body.Contains("tuition") || body.Contains(studentClass.Class.Name)),
                It.IsAny<string>(),
                It.IsAny<string>()),
            Times.Once);
    }

    [Fact]
    public async Task GetTuitionById_ExistingTuition_ReturnsCorrectTuition()
    {
        // Arrange
        var accountFirebaseId = "student123";
        var tuitionId = Guid.NewGuid();

        var account = new AccountModel
        {
            AccountFirebaseId = accountFirebaseId,
            Email = "test@gmail.com",
            Role = Role.Student
        };

        var studentClassId = Guid.NewGuid();
        var classId = Guid.NewGuid();

        _tuitionRepositoryMock.Setup(repo => repo.FindSingleProjectedAsync<TuitionWithStudentClassModel>(
                It.IsAny<Expression<Func<DataAccess.Models.Entity.Tuition, bool>>>(),
                false, false, TrackingOption.Default))
            .ReturnsAsync(new TuitionWithStudentClassModel
            {
                Id = tuitionId,
                StudentClassId = studentClassId,
                StudentClass = new StudentClassModel
                {
                    Id = studentClassId,
                    ClassId = classId,
                    CreatedById = "admin001",
                    StudentFirebaseId = account.AccountFirebaseId
                }
            });

        // Act
        var result = await _tuitionService.GetTuitionById(tuitionId, account);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(tuitionId, result.Id);
        _tuitionRepositoryMock.Verify(repo => repo.FindSingleProjectedAsync<TuitionWithStudentClassModel>(
            It.IsAny<Expression<Func<DataAccess.Models.Entity.Tuition, bool>>>(),
            It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<TrackingOption>()), Times.Once);
    }

    [Fact]
    public async Task GetTuitionById_StaffCanViewAnyTuition_ReturnsCorrectTuition()
    {
        // Arrange
        var tuitionId = Guid.NewGuid();
        var studentFirebaseId = "student-id";
        var studentClassId = Guid.NewGuid();
        var staffAccount = new AccountModel
            { AccountFirebaseId = "staff-id", Role = Role.Staff, Email = "staff@gmail.com" };

        _tuitionRepositoryMock.Setup(repo => repo.FindSingleProjectedAsync<TuitionWithStudentClassModel>(
                It.IsAny<Expression<Func<DataAccess.Models.Entity.Tuition, bool>>>(),
                It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<TrackingOption>()))
            .ReturnsAsync(new TuitionWithStudentClassModel
            {
                Id = tuitionId,
                StudentClassId = studentClassId,
                StudentClass = new StudentClassModel
                {
                    Id = Guid.NewGuid(),
                    ClassId = studentClassId,
                    CreatedById = "admin001",
                    StudentFirebaseId = studentFirebaseId
                }
            });

        // Act
        var result = await _tuitionService.GetTuitionById(tuitionId, staffAccount);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(tuitionId, result.Id);
    }

    [Fact]
    public async Task GetTuitionById_StudentAccessingAnotherStudentTuition_ThrowsForbiddenException()
    {
        // Arrange
        var tuitionId = Guid.NewGuid();
        var studentClassId = Guid.NewGuid();
        var classId = Guid.NewGuid();

        var studentAccount = new AccountModel
        {
            AccountFirebaseId = "student1",
            Email = "student123@gmail.com",
            Role = Role.Student
        };

        _tuitionRepositoryMock.Setup(repo => repo.FindSingleProjectedAsync<TuitionWithStudentClassModel>(
                It.IsAny<Expression<Func<DataAccess.Models.Entity.Tuition, bool>>>(),
                It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<TrackingOption>()))
            .ReturnsAsync(new TuitionWithStudentClassModel
            {
                Id = tuitionId,
                StudentClassId = studentClassId,
                StudentClass = new StudentClassModel
                {
                    Id = Guid.NewGuid(),
                    ClassId = classId,
                    CreatedById = "admin001",
                    StudentFirebaseId = "student2" // Different student
                }
            });

        // Act & Assert
        await Assert.ThrowsAsync<ForbiddenMethodException>(() =>
            _tuitionService.GetTuitionById(tuitionId, studentAccount));
    }

    [Fact]
    public async Task GetTuitionById_TuitionNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var tuitionId = Guid.NewGuid();
        var account = new AccountModel
            { AccountFirebaseId = "account-id", Role = Role.Staff, Email = "user@gmail.com" };

        _tuitionRepositoryMock.Setup(repo => repo.FindSingleProjectedAsync<TuitionWithStudentClassModel>(
                It.IsAny<Expression<Func<DataAccess.Models.Entity.Tuition, bool>>>(),
                It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<TrackingOption>()))
            .ReturnsAsync((TuitionWithStudentClassModel)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _tuitionService.GetTuitionById(tuitionId, account));
    }
}