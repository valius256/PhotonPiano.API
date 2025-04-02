using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Class;
using PhotonPiano.BusinessLogic.BusinessModel.EntranceTest;
using PhotonPiano.BusinessLogic.BusinessModel.EntranceTestStudent;
using PhotonPiano.BusinessLogic.BusinessModel.Level;
using PhotonPiano.BusinessLogic.BusinessModel.Room;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.BusinessLogic.Services;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.Shared.Exceptions;

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

    private readonly Mock<IRoomService> _roomServiceMock;
    private readonly Mock<IAccountService> _accountServiceMock;

    private readonly IEntranceTestService _entranceTestService;

    private readonly AccountModel _sampleStaff;
    private readonly AccountModel _sampleStudent;

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

        _roomServiceMock = _fixture.Freeze<Mock<IRoomService>>();
        _accountServiceMock = _fixture.Freeze<Mock<IAccountService>>();

        _unitOfWorkMock.Setup(uow => uow.EntranceTestRepository).Returns(_entranceTestRepositoryMock.Object);
        _unitOfWorkMock.Setup(uow => uow.RoomRepository).Returns(_roomRepositoryMock.Object);
        _unitOfWorkMock.Setup(uow => uow.EntranceTestStudentRepository)
            .Returns(_entranceTestStudentRepositoryMock.Object);

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
            .With(x => x.StartTime, DateOnly.FromDateTime(DateTime.UtcNow))
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
            .With(x => x.StartTime, DateOnly.FromDateTime(DateTime.UtcNow))
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
            .With(x => x.StartTime, DateOnly.FromDateTime(DateTime.UtcNow))
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