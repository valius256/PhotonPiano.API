using System.Linq.Expressions;
using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Class;
using PhotonPiano.BusinessLogic.BusinessModel.EntranceTest;
using PhotonPiano.BusinessLogic.BusinessModel.EntranceTestResult;
using PhotonPiano.BusinessLogic.BusinessModel.EntranceTestStudent;
using PhotonPiano.BusinessLogic.BusinessModel.Level;
using PhotonPiano.BusinessLogic.BusinessModel.Payment;
using PhotonPiano.BusinessLogic.BusinessModel.Room;
using PhotonPiano.BusinessLogic.BusinessModel.SystemConfig;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.BusinessLogic.Services;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.Shared.Enums;
using PhotonPiano.Shared.Exceptions;
using PhotonPiano.Shared.Utils;

namespace PhotonPiano.Test.UnitTest.EntranceTest;

[Collection("Entrance test unit tests")]
public class EntranceTestServiceTest
{
    private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());

    private readonly Mock<IServiceFactory> _serviceFactoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    private readonly Mock<IEntranceTestRepository> _entranceTestRepositoryMock;
    private readonly Mock<IRoomRepository> _roomRepositoryMock;
    private readonly Mock<IEntranceTestStudentRepository> _entranceTestStudentRepositoryMock;
    private readonly Mock<ITransactionRepository> _transactionRepositoryMock;
    private readonly Mock<IAccountRepository> _accountRepositoryMock;

    private readonly Mock<IRoomService> _roomServiceMock;
    private readonly Mock<IAccountService> _accountServiceMock;
    private readonly Mock<ISystemConfigService> _systemConfigServiceMock;

    private readonly IEntranceTestService _entranceTestService;

    private readonly AccountModel _sampleStaff;
    private readonly AccountModel _sampleStudent;

    private readonly List<SystemConfigModel> _systemConfigs =
    [
        new SystemConfigModel
        {
            Id = Guid.NewGuid(),
            ConfigName = ConfigNames.AllowEntranceTestRegistering,
            ConfigValue = "false",
            Role = Role.Administrator
        }
    ];

    public EntranceTestServiceTest()
    {
        _sampleStaff = new AccountModel
        {
            AccountFirebaseId = "test",
            Email = "staff@test.com",
            Role = Role.Staff
        };

        _sampleStudent = new AccountModel
        {
            AccountFirebaseId = "teststudent",
            Email = "student@test.com",
            Role = Role.Student
        };

        _serviceFactoryMock = _fixture.Freeze<Mock<IServiceFactory>>();
        _unitOfWorkMock = _fixture.Freeze<Mock<IUnitOfWork>>();

        _entranceTestRepositoryMock = _fixture.Freeze<Mock<IEntranceTestRepository>>();
        _roomRepositoryMock = _fixture.Freeze<Mock<IRoomRepository>>();
        _entranceTestStudentRepositoryMock = _fixture.Freeze<Mock<IEntranceTestStudentRepository>>();
        _transactionRepositoryMock = _fixture.Freeze<Mock<ITransactionRepository>>();
        _accountRepositoryMock = _fixture.Freeze<Mock<IAccountRepository>>();

        _roomServiceMock = _fixture.Freeze<Mock<IRoomService>>();
        _accountServiceMock = _fixture.Freeze<Mock<IAccountService>>();
        _systemConfigServiceMock = _fixture.Freeze<Mock<ISystemConfigService>>();

        _unitOfWorkMock.Setup(uow => uow.EntranceTestRepository).Returns(_entranceTestRepositoryMock.Object);
        _unitOfWorkMock.Setup(uow => uow.RoomRepository).Returns(_roomRepositoryMock.Object);
        _unitOfWorkMock.Setup(uow => uow.EntranceTestStudentRepository)
            .Returns(_entranceTestStudentRepositoryMock.Object);
        _unitOfWorkMock.Setup(uow => uow.TransactionRepository).Returns(_transactionRepositoryMock.Object);
        _unitOfWorkMock.Setup(uow => uow.AccountRepository).Returns(_accountRepositoryMock.Object);

        _serviceFactoryMock.Setup(sf => sf.RoomService).Returns(_roomServiceMock.Object);
        _serviceFactoryMock.Setup(sf => sf.SystemConfigService).Returns(_systemConfigServiceMock.Object);

        _entranceTestService = new EntranceTestService(_unitOfWorkMock.Object, _serviceFactoryMock.Object);
    }

    [Fact]
    public async Task GetEntranceTestDetailById_EntranceTestNotFound_ThrowsNotFoundException()
    {
        //Arrange
        var entranceTestId = Guid.NewGuid();
        _entranceTestRepositoryMock.Setup(e => e.FindSingleProjectedAsync<EntranceTestDetailModel>(
            x => x.Id == entranceTestId, false,
            false, TrackingOption.IdentityResolution)).ReturnsAsync((EntranceTestDetailModel?)null);

        //Act
        var result = await Record.ExceptionAsync(() =>
            _entranceTestService.GetEntranceTestDetailById(entranceTestId, _sampleStaff));

        //Assert
        Assert.IsType<NotFoundException>(result);
        Assert.Equal("EntranceTest not found.", result.Message);
    }

    [Fact]
    public async Task GetEntranceTestDetailById_StudentIsNotInTheFoundEntranceTest_ThrowsForbiddenMethodException()
    {
        //Arrange
        var entranceTestId = Guid.NewGuid();

        var entranceTest = _fixture.Build<EntranceTestDetailModel>()
            .With(e => e.Date, DateOnly.FromDateTime(DateTime.UtcNow))
            .With(e => e.EntranceTestStudents, [])
            .Create();

        _entranceTestRepositoryMock.Setup(e => e.FindSingleProjectedAsync<EntranceTestDetailModel>(
            x => x.Id == entranceTestId, false,
            true, TrackingOption.IdentityResolution)).ReturnsAsync(entranceTest);

        //Act
        var result = await Record.ExceptionAsync(() =>
            _entranceTestService.GetEntranceTestDetailById(entranceTestId, _sampleStudent));

        //Assert
        Assert.IsType<ForbiddenMethodException>(result);
        Assert.Equal("You are not allowed to view this entrance test information.", result.Message);
    }

    [Fact]
    public async Task EnrollEntranceTest_EntranceTestRegisteringIsClosed_ThrowsBadRequestException()
    {
        //Arrange

        _systemConfigServiceMock.Setup(service => service.GetEntranceTestConfigs(new List<string>())).ReturnsAsync(_systemConfigs);

        //Act
        var record =
            await Record.ExceptionAsync(() => _entranceTestService.EnrollEntranceTest(_sampleStudent, "", "", ""));

        //Assert
        Assert.IsType<BadRequestException>(record);
        Assert.Equal("Entrance test registering is closed for now.", record.Message);
    }

    [Fact]
    public async Task EnrollEntranceTest_StudentIsNotValidToEnroll_ThrowsBadRequestException()
    {
        //Arrange
        _systemConfigServiceMock.Setup(service => service.GetEntranceTestConfigs(new List<string>())).ReturnsAsync([
            new SystemConfigModel
            {
                Id = Guid.NewGuid(),
                ConfigName = ConfigNames.AllowEntranceTestRegistering,
                ConfigValue = "true",
                Role = Role.Administrator
            }
        ]);

        //Act
        var record =
            await Record.ExceptionAsync(() => _entranceTestService.EnrollEntranceTest(_sampleStudent with
            {
                StudentStatus = StudentStatus.AttemptingEntranceTest
            }, "", "", ""));

        //Assert
        Assert.IsType<BadRequestException>(record);
        Assert.Equal(
            "Student is must be in DropOut or Unregistered in order to be accepted to enroll in entrance test.",
            record.Message);
    }

    [Fact]
    public async Task HandleEnrollmentPaymentCallback_TransactionNotFound_ThrowsPaymentRequiredException()
    {
        //Arrange
        var transactionCode = Guid.NewGuid();
        var callbackModel = _fixture.Build<VnPayCallbackModel>()
            .With(x => x.VnpTxnRef, transactionCode.ToString())
            .Create();

        _transactionRepositoryMock.Setup(repo => repo.FindSingleAsync(t => t.Id == transactionCode, true, false))
            .ReturnsAsync((Transaction?)null);

        //Act
        var record = await Record.ExceptionAsync(() =>
            _entranceTestService.HandleEnrollmentPaymentCallback(callbackModel, _sampleStudent.AccountFirebaseId));

        //Assert
        Assert.IsType<PaymentRequiredException>(record);
        Assert.Equal("Payment is required", record.Message);
    }

    [Fact]
    public async Task HandleEnrollmentPaymentCallback_TransactionAccountNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var transactionCode = Guid.NewGuid();
        var callbackModel = _fixture.Build<VnPayCallbackModel>()
            .With(x => x.VnpTxnRef, transactionCode.ToString())
            .Create();

        _transactionRepositoryMock.Reset(); // Ensure previous setups don't interfere

        _transactionRepositoryMock.Setup(repo => repo.FindSingleAsync(t => t.Id == transactionCode, true, false))
            .ReturnsAsync(new Transaction
            {
                Id = transactionCode, // Make sure the transaction ID matches
                CreatedById = "",
                CreatedAt = DateTime.UtcNow,
                CreatedByEmail = ""
            });

        _accountRepositoryMock.Reset(); // Reset mock to avoid conflicts from other tests

        _accountRepositoryMock.Setup(repo =>
                repo.FindSingleAsync(It.IsAny<Expression<Func<Account, bool>>>(), true, false))
            .ReturnsAsync((Account?)null);

        // Act
        var record = await Record.ExceptionAsync(() =>
            _entranceTestService.HandleEnrollmentPaymentCallback(callbackModel, _sampleStudent.AccountFirebaseId));

        // Assert
        Assert.IsType<NotFoundException>(record);
        Assert.Equal("Account not found", record.Message);
    }

    [Fact]
    public async Task AutoArrangeEntranceTests_StudentsNotFound_ThrowsBadRequestException()
    {
        //Arrange
        var requestModel = _fixture.Build<AutoArrangeEntranceTestsModel>()
            .With(x => x.StudentIds, ["test@abc.com"])
            .Create();

        _accountRepositoryMock.Setup(repo =>
                repo.FindProjectedAsync<AccountDetailModel>(It.IsAny<Expression<Func<Account, bool>>>(), false, false,
                    TrackingOption.Default))
            .ReturnsAsync([]);

        //Act
        var record = await Record.ExceptionAsync(() =>
            _entranceTestService.AutoArrangeEntranceTests(requestModel, _sampleStaff));

        //Assert
        Assert.IsType<BadRequestException>(record);
        Assert.Equal("Some students are not found.", record.Message);
    }

    [Fact]
    public async Task AutoArrangeEntranceTests_StudentsAreNotValidToBeArranged_ThrowsBadRequestException()
    {
        //Arrange
        var requestModel = _fixture.Build<AutoArrangeEntranceTestsModel>()
            .With(x => x.StudentIds, ["student1", "student2"])
            .Create();

        var students = new List<AccountDetailModel>
        {
            new AccountDetailModel
            {
                AccountFirebaseId = "student1",
                Email = "student1@abc.com",
                Role = Role.Student,
                StudentStatus = StudentStatus.Unregistered
            },
            new AccountDetailModel
            {
                AccountFirebaseId = "student2",
                Email = "student2@abc.com",
                Role = Role.Student,
                StudentStatus = StudentStatus.Leave
            }
        };

        _accountRepositoryMock.Setup(repo =>
                repo.FindProjectedAsync<AccountDetailModel>(It.IsAny<Expression<Func<Account, bool>>>(), false, false,
                    TrackingOption.Default))
            .ReturnsAsync(students);

        //Act
        var record = await Record.ExceptionAsync(() =>
            _entranceTestService.AutoArrangeEntranceTests(requestModel, _sampleStaff));

        //Assert
        Assert.IsType<BadRequestException>(record);
        Assert.Equal("Some students are not valid to be arranged with entrance tests.", record.Message);
    }

    [Fact]
    public async Task AutoArrangeEntranceTests_SomeStudentsAreArranged_ThrowsConflictException()
    {
        //Arrange
        var requestModel = _fixture.Build<AutoArrangeEntranceTestsModel>()
            .With(x => x.StudentIds, ["student1", "student2"])
            .Create();

        var students = new List<AccountDetailModel>
        {
            new()
            {
                AccountFirebaseId = "student1",
                Email = "student1@abc.com",
                Role = Role.Student,
                StudentStatus = StudentStatus.WaitingForEntranceTestArrangement,
                EntranceTestStudents =
                [
                    new EntranceTestStudentDetail
                    {
                        EntranceTestId = Guid.NewGuid(),
                        Id = Guid.NewGuid(),
                        StudentFirebaseId = "student1",
                    }
                ]
            },
            new()
            {
                AccountFirebaseId = "student2",
                Email = "student2@abc.com",
                Role = Role.Student,
                StudentStatus = StudentStatus.WaitingForEntranceTestArrangement,
                EntranceTestStudents =
                [
                    new EntranceTestStudentDetail
                    {
                        EntranceTestId = Guid.NewGuid(),
                        Id = Guid.NewGuid(),
                        StudentFirebaseId = "student2",
                    }
                ]
            }
        };

        _accountRepositoryMock.Setup(repo =>
                repo.FindProjectedAsync<AccountDetailModel>(It.IsAny<Expression<Func<Account, bool>>>(), false, false,
                    TrackingOption.Default))
            .ReturnsAsync(students);

        //Act
        var record = await Record.ExceptionAsync(() =>
            _entranceTestService.AutoArrangeEntranceTests(requestModel, _sampleStaff));

        //Assert
        Assert.IsType<ConflictException>(record);
        Assert.Equal(
            $"Students: {string.Join(", ", students.Select(s => $"{s.AccountFirebaseId}-{s.Email}"))} are already arranged.",
            record.Message);
    }

    [Fact]
    public async Task UpdateStudentEntranceResults_EntranceTestStudentNotFound_ThrowsNotFoundException()
    {
        //Arrange
        var updateModel = _fixture.Build<UpdateEntranceTestResultsModel>()
            .Create();
        var entranceTestId = Guid.NewGuid();
        var studentId = Guid.NewGuid();

        _entranceTestStudentRepositoryMock.Setup(repo => repo.FindFirstAsync(
                It.IsAny<Expression<Func<DataAccess.Models.Entity.EntranceTestStudent, bool>>>(), true, false,
                null, true))
            .ReturnsAsync((EntranceTestStudent?)null);

        //Act
        var record = await Record.ExceptionAsync(() =>
            _entranceTestService.UpdateStudentEntranceResults(entranceTestId, studentId.ToString(), updateModel,
                _sampleStaff));

        //Assert
        Assert.IsType<NotFoundException>(record);
        Assert.Equal("Entrance test not found or student not found.", record.Message);
    }

    [Fact]
    public async Task CreateEntranceTest_RoleIsNotInstructor_ThrowsBadRequestException()
    {
        //Arrange
        var instructorId = Guid.NewGuid().ToString();
        var createModel = _fixture.Build<CreateEntranceTestModel>()
            .With(e => e.InstructorId, instructorId)
            .With(e => e.Date, DateOnly.FromDateTime(DateTime.UtcNow))
            .Create();

        var roomId = Guid.NewGuid();

        var room = _fixture.Build<RoomDetailModel>()
            .With(r => r.EntranceTests, [])
            .Create();

        var account = _fixture.Build<AccountDetailModel>()
            .With(a => a.CurrentClass, (ClassModel?)null)
            .With(a => a.Role, Role.Student)
            .With(a => a.StudentClasses, [])
            .With(a => a.EntranceTestStudents, [])
            .Create();

        _roomServiceMock.Setup(service => service.GetRoomDetailById(roomId)).ReturnsAsync(room);

        _accountServiceMock.Setup(service => service.GetAccountById(instructorId))
            .ReturnsAsync(account);

        //Act
        var record =
            await Record.ExceptionAsync(() => _entranceTestService.CreateEntranceTest(createModel, _sampleStaff));

        //Assert
        Assert.IsType<BadRequestException>(record);
        Assert.Equal("This is not Instructor, please try again", record.Message);
    }

    [Fact]
    public async Task UpdateEntranceTest_EntranceTestNotFound_ThrowsNotFoundException()
    {
        //Arrange
        var entranceTestId = Guid.NewGuid();

        var updateModel = _fixture.Build<UpdateEntranceTestModel>()
            .With(x => x.Date, DateOnly.FromDateTime(DateTime.UtcNow.AddHours(7)))
            .Create();

        _entranceTestRepositoryMock.Setup(repo => repo.FindSingleAsync(e => e.Id == entranceTestId, true, false))
            .ReturnsAsync((DataAccess.Models.Entity.EntranceTest?)null);

        //Act
        var record = await Record.ExceptionAsync(() => _entranceTestService.UpdateEntranceTest(entranceTestId,
            updateModel, _sampleStaff.AccountFirebaseId));

        //Assert
        Assert.IsType<NotFoundException>(record);
        Assert.Equal("This EntranceTest not found.", record.Message);
    }

    [Fact]
    public async Task UpdateEntranceTest_RoomNotFound_ThrowsNotFoundException()
    {
        //Arrange
        var entranceTestId = Guid.NewGuid();

        var roomId = Guid.NewGuid();

        var updateModel = _fixture.Build<UpdateEntranceTestModel>()
            .With(x => x.RoomId, roomId)
            .With(x => x.Date, DateOnly.FromDateTime(DateTime.UtcNow.AddHours(7)))
            .Create();

        _entranceTestRepositoryMock.Setup(repo => repo.FindSingleAsync(e => e.Id == entranceTestId, true, false))
            .ReturnsAsync(new DataAccess.Models.Entity.EntranceTest
            {
                Id = Guid.NewGuid(),
                Date = DateOnly.FromDateTime(DateTime.UtcNow),
                Name = "test",
                Shift = Shift.Shift1_7h_8h30,
                RoomId = roomId,
                CreatedById = Guid.NewGuid().ToString()
            });


        _roomRepositoryMock.Setup(repo => repo.FindSingleAsync(It.IsAny<Expression<Func<Room, bool>>>(), false, false))
            .ReturnsAsync((Room?)null);

        //Act
        var record = await Record.ExceptionAsync(() => _entranceTestService.UpdateEntranceTest(entranceTestId,
            updateModel, _sampleStaff.AccountFirebaseId));

        //Assert
        Assert.IsType<NotFoundException>(record);
        Assert.Equal("Room not found.", record.Message);
    }

    [Fact]
    public async Task UpdateEntranceTest_ResultsAreNotReadyToBePublished_ThrowsBadRequestException()
    {
        //Arrange
        var entranceTestId = Guid.NewGuid();

        var roomId = Guid.NewGuid();

        var updateModel = _fixture.Build<UpdateEntranceTestModel>()
            .With(x => x.RoomId, roomId)
            .With(x => x.Date, DateOnly.FromDateTime(DateTime.UtcNow.AddHours(7)))
            .With(x => x.IsAnnouncedScore, true)
            .Create();

        _entranceTestRepositoryMock.Setup(repo => repo.FindSingleAsync(e => e.Id == entranceTestId, true, false))
            .ReturnsAsync(new DataAccess.Models.Entity.EntranceTest
            {
                Id = Guid.NewGuid(),
                Date = DateOnly.FromDateTime(DateTime.UtcNow),
                Name = "test",
                Shift = Shift.Shift1_7h_8h30,
                RoomId = Guid.NewGuid(),
                CreatedById = Guid.NewGuid().ToString()
            });

        _roomRepositoryMock.Setup(repo => repo.FindSingleAsync(r => r.Id == roomId, false, false))
            .ReturnsAsync(new Room
            {
                Id = Guid.NewGuid(),
                CreatedById = Guid.NewGuid().ToString()
            });

        var entranceTestStudents =
            _fixture.Build<EntranceTestStudentWithResultsModel>()
                .With(e => e.Level, (LevelModel?)null)
                .With(e => e.Student, (AccountModel?)null)
                .With(e => e.EntranceTestId, entranceTestId)
                .With(e => e.EntranceTestResults, [])
                .CreateMany(3)
                .ToList();

        _entranceTestStudentRepositoryMock.Setup(repo =>
                repo.FindProjectedAsync<EntranceTestStudentWithResultsModel>(
                    x => x.EntranceTestId == entranceTestId, false, false, TrackingOption.Default))
            .ReturnsAsync(entranceTestStudents);

        //Act
        var record = await Record.ExceptionAsync(() => _entranceTestService.UpdateEntranceTest(entranceTestId,
            updateModel, _sampleStaff.AccountFirebaseId));

        //Assert
        Assert.IsType<BadRequestException>(record);
        Assert.Equal("Can't publish the score of this entrance test", record.Message);
    }

    [Fact]
    public async Task DeleteEntranceTest_EntranceTestNotFound_ThrowsNotFoundException()
    {
        //Arrange
        var entranceTestId = Guid.NewGuid();
        _entranceTestRepositoryMock.Setup(repo => repo.FindSingleAsync(e => e.Id == entranceTestId, true, false))
            .ReturnsAsync((DataAccess.Models.Entity.EntranceTest?)null);

        //Act
        var record = await Record.ExceptionAsync(() =>
            _entranceTestService.DeleteEntranceTest(entranceTestId, _sampleStaff.AccountFirebaseId));

        //Assert
        Assert.IsType<NotFoundException>(record);
        Assert.Equal("This EntranceTest not found.", record.Message);
    }
}