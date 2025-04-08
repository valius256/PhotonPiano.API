using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Slot;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.BusinessLogic.Services;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.Shared.Exceptions;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using PhotonPiano.BusinessLogic.BusinessModel.Class;
using PhotonPiano.BusinessLogic.BusinessModel.Room;
using PhotonPiano.Test.Extensions;

namespace PhotonPiano.Test.UnitTest.Scheduler;

[Collection("Slot Unit Tests")]
public class SlotServiceTest
{
    private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());

    private readonly Mock<IServiceFactory> _serviceFactoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ISlotRepository> _slotRepositoryMock;
    private readonly Mock<IRoomRepository> _roomRepositoryMock;
    private readonly Mock<IClassRepository> _classRepositoryMock;
    private readonly Mock<ILevelRepository> _levelRepositoryMock;
    private readonly Mock<IDayOffRepository> _dayOffRepositoryMock;
    private readonly Mock<ISlotStudentRepository> _slotStudentRepositoryMock;
    private readonly Mock<IStudentClassRepository> _studentClassRepositoryMock;
    private readonly Mock<IRedisCacheService> _redisCacheServiceMock;
    private readonly Mock<IClassService> _classServiceMock;
    private readonly Mock<IRoomService> _roomServiceMock;
    private readonly Mock<INotificationService> _notificationServiceMock;

    private readonly SlotService _slotService;
    
    public SlotServiceTest()
    {
        // Freeze mocks to ensure consistent instances
        _serviceFactoryMock = _fixture.Freeze<Mock<IServiceFactory>>();
        _unitOfWorkMock = _fixture.Freeze<Mock<IUnitOfWork>>();
        _slotRepositoryMock = _fixture.Freeze<Mock<ISlotRepository>>();
        _roomRepositoryMock = _fixture.Freeze<Mock<IRoomRepository>>();
        _classRepositoryMock = _fixture.Freeze<Mock<IClassRepository>>();
        _levelRepositoryMock = _fixture.Freeze<Mock<ILevelRepository>>();
        _dayOffRepositoryMock = _fixture.Freeze<Mock<IDayOffRepository>>();
        _slotStudentRepositoryMock = _fixture.Freeze<Mock<ISlotStudentRepository>>();
        _studentClassRepositoryMock = _fixture.Freeze<Mock<IStudentClassRepository>>();
        _redisCacheServiceMock = _fixture.Freeze<Mock<IRedisCacheService>>();
        _classServiceMock = _fixture.Freeze<Mock<IClassService>>();
        _roomServiceMock = _fixture.Freeze<Mock<IRoomService>>();
        _notificationServiceMock = _fixture.Freeze<Mock<INotificationService>>();

        // Setup UnitOfWork repositories
        _unitOfWorkMock.Setup(uow => uow.SlotRepository).Returns(_slotRepositoryMock.Object);
        _unitOfWorkMock.Setup(uow => uow.RoomRepository).Returns(_roomRepositoryMock.Object);
        _unitOfWorkMock.Setup(uow => uow.ClassRepository).Returns(_classRepositoryMock.Object);
        _unitOfWorkMock.Setup(uow => uow.LevelRepository).Returns(_levelRepositoryMock.Object);
        _unitOfWorkMock.Setup(uow => uow.DayOffRepository).Returns(_dayOffRepositoryMock.Object);
        _unitOfWorkMock.Setup(uow => uow.SlotStudentRepository).Returns(_slotStudentRepositoryMock.Object);
        _unitOfWorkMock.Setup(uow => uow.StudentClassRepository).Returns(_studentClassRepositoryMock.Object);

        // Setup ServiceFactory services
        _serviceFactoryMock.Setup(sf => sf.RedisCacheService).Returns(_redisCacheServiceMock.Object);
        _serviceFactoryMock.Setup(sf => sf.ClassService).Returns(_classServiceMock.Object);
        _serviceFactoryMock.Setup(sf => sf.RoomService).Returns(_roomServiceMock.Object);
        _serviceFactoryMock.Setup(sf => sf.NotificationService).Returns(_notificationServiceMock.Object);
        
        
        // Create the service under test
        _slotService = new SlotService(_serviceFactoryMock.Object, _unitOfWorkMock.Object);
    
    }
    [Fact]
    public void GetShiftStartTime_ReturnsCorrectTime_ForAllShifts()
    {
        // Arrange
        var expectedStartTimes = new Dictionary<Shift, TimeOnly>
        {
            { Shift.Shift1_7h_8h30, new TimeOnly(7, 0) },
            { Shift.Shift2_8h45_10h15, new TimeOnly(8, 45) },
            { Shift.Shift3_10h45_12h, new TimeOnly(10, 45) },
            { Shift.Shift4_12h30_14h00, new TimeOnly(12, 30) },
            { Shift.Shift5_14h15_15h45, new TimeOnly(14, 15) },
            { Shift.Shift6_16h00_17h30, new TimeOnly(16, 0) },
            { Shift.Shift7_18h_19h30, new TimeOnly(18, 0) },
            { Shift.Shift8_19h45_21h15, new TimeOnly(19, 45) }
        };

        // Act & Assert
        foreach (var shift in expectedStartTimes.Keys)
        {
            var startTime = _slotService.GetShiftStartTime(shift);
            Assert.Equal(expectedStartTimes[shift], startTime);
        }
    }
    
    [Fact]
    public void GetShiftStartTime_ThrowsException_ForInvalidShift()
    {
        // Arrange
        var invalidShift = (Shift)999;

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => _slotService.GetShiftStartTime(invalidShift));
        Assert.Equal("Invalid shift value.", ex.Message);
    }

    [Fact]
    public void GetShiftEndTime_ReturnsCorrectTime_ForAllShifts()
    {
        // Arrange
        var expectedEndTimes = new Dictionary<Shift, TimeOnly>
        {
            { Shift.Shift1_7h_8h30, new TimeOnly(8, 30) },
            { Shift.Shift2_8h45_10h15, new TimeOnly(10, 45) },
            { Shift.Shift3_10h45_12h, new TimeOnly(12, 00) },
            { Shift.Shift4_12h30_14h00, new TimeOnly(14, 00) },
            { Shift.Shift5_14h15_15h45, new TimeOnly(15, 45) },
            { Shift.Shift6_16h00_17h30, new TimeOnly(17, 30) },
            { Shift.Shift7_18h_19h30, new TimeOnly(19, 30) },
            { Shift.Shift8_19h45_21h15, new TimeOnly(21, 15) }
        };

        // Act & Assert
        foreach (var shift in expectedEndTimes.Keys)
        {
            var endTime = _slotService.GetShiftEndTime(shift);
            Assert.Equal(expectedEndTimes[shift], endTime);
        }
    }
    
    [Fact]
    public void GetShiftEndTime_ThrowsException_ForInvalidShift()
    {
        // Arrange
        var invalidShift = (Shift)999;

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => _slotService.GetShiftEndTime(invalidShift));
        Assert.Equal("Invalid shift value.", ex.Message);
    }

    [Fact]
    public async Task GetSlotDetailById_ReturnsSlot_WhenSlotExists()
    {
        // Arrange
        var slotId = Guid.NewGuid();
        var expectedSlot = new SlotDetailModel { Id = slotId };

        _slotRepositoryMock
            .Setup(r => r.FindSingleProjectedAsync<SlotDetailModel>(
                It.IsAny<Expression<Func<Slot, bool>>>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<TrackingOption>()))
            .ReturnsAsync(expectedSlot);

        // Act
        var result = await _slotService.GetSlotDetailById(slotId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(slotId, result.Id);
    }

    [Fact]
    public async Task GetSlotDetailById_ThrowsNotFoundException_WhenSlotDoesNotExist()
    {
        // Arrange
        var slotId = Guid.NewGuid();

        _slotRepositoryMock
            .Setup(r => r.FindSingleProjectedAsync<SlotDetailModel>(
                It.IsAny<Expression<Func<Slot, bool>>>(),
                false,
                false,
                TrackingOption.Default))
            .ReturnsAsync((SlotDetailModel)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _slotService.GetSlotDetailById(slotId));
    }


   [Fact]
    public async Task GetSlotsAsync_ReturnsFilteredSlots_ForRegularUser()
    {
        // Arrange
        var slotModel = new GetSlotModel
        {
            StartTime = DateOnly.FromDateTime(DateTime.Now),
            EndTime = DateOnly.FromDateTime(DateTime.Now.AddDays(7)),
            Shifts = new List<Shift> { Shift.Shift1_7h_8h30 },
            SlotStatuses = new List<SlotStatus> { SlotStatus.NotStarted }
        };

        var accountModel = new AccountModel
        {
            Role = Role.Student,
            Email = "testEmailIsFun@gmail.com",
            AccountFirebaseId = "testUser"
        };

        var expectedSlots = new List<SlotDetailModel>
        {
            new SlotDetailModel { Id = Guid.NewGuid() },
            new SlotDetailModel { Id = Guid.NewGuid() }
        };

        _slotRepositoryMock
            .Setup(r => r.FindProjectedAsync<SlotDetailModel>(
                It.IsAny<Expression<Func<Slot, bool>>>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<TrackingOption>()))
            .ReturnsAsync(expectedSlots);

        // Act
        var result = await _slotService.GetSlotsAsync(slotModel, accountModel);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedSlots.Count, result.Count);
    }

    [Fact]
    public async Task GetSlotsAsync_ReturnsFilteredSlots_ForInstructor()
    {
        // Arrange
        var slotModel = new GetSlotModel
        {
            StartTime = DateOnly.FromDateTime(DateTime.Now),
            EndTime = DateOnly.FromDateTime(DateTime.Now.AddDays(7)),
            Shifts = new List<Shift> { Shift.Shift1_7h_8h30 },
            SlotStatuses = new List<SlotStatus> { SlotStatus.NotStarted }
        };

        var accountModel = new AccountModel
        {
            Role = Role.Instructor,
            Email = "testEmailIsFun@gmail.com",

            AccountFirebaseId = "instructorUser"
        };

        var expectedSlots = new List<SlotDetailModel>
        {
            new SlotDetailModel { Id = Guid.NewGuid() },
            new SlotDetailModel { Id = Guid.NewGuid() }
        };

        _slotRepositoryMock
            .Setup(r => r.FindProjectedAsync<SlotDetailModel>(
                It.IsAny<Expression<Func<Slot, bool>>>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<TrackingOption>()))
            .ReturnsAsync(expectedSlots);

        // Act
        var result = await _slotService.GetSlotsAsync(slotModel, accountModel);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedSlots.Count, result.Count);
    }

    [Fact]
    public async Task GetSlotsAsync_ReturnsFilteredSlots_ForNoAccountModel()
    {
        // Arrange
        var slotModel = new GetSlotModel
        {
            StartTime = DateOnly.FromDateTime(DateTime.Now),
            EndTime = DateOnly.FromDateTime(DateTime.Now.AddDays(7)),
            Shifts = new List<Shift> { Shift.Shift1_7h_8h30 },
            SlotStatuses = new List<SlotStatus> { SlotStatus.NotStarted }
        };

        var expectedSlots = new List<SlotDetailModel>
        {
            new SlotDetailModel { Id = Guid.NewGuid() },
            new SlotDetailModel { Id = Guid.NewGuid() }
        };

        _slotRepositoryMock
            .Setup(r => r.FindProjectedAsync<SlotDetailModel>(
                It.IsAny<Expression<Func<Slot, bool>>>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<TrackingOption>()))
            .ReturnsAsync(expectedSlots);

        // Act
        var result = await _slotService.GetSlotsAsync(slotModel);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedSlots.Count, result.Count);
    }

    [Fact]
    public async Task CreateSlot_ThrowsBadRequestException_WhenSlotInPast()
    {
        // Arrange
        var createSlotModel = new CreateSlotModel
        {
            Shift = Shift.Shift1_7h_8h30,
            Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
            RoomId = Guid.NewGuid(),
            ClassId = Guid.NewGuid()
        };

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _slotService.CreateSlot(createSlotModel, "testUser"));
    }

    [Fact]
    public async Task CreateSlot_ThrowsNotFoundException_WhenRoomNotFound()
    {
        // Arrange
        var createSlotModel = new CreateSlotModel
        {
            Shift = Shift.Shift1_7h_8h30,
            Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            RoomId = Guid.NewGuid(),
            ClassId = Guid.NewGuid()
        };

        _unitOfWorkMock.Setup(uow => uow.RoomRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Room)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _slotService.CreateSlot(createSlotModel, "testUser"));
    }
    

    [Fact]
    public async Task InvalidateCacheForClassAsync_DeletesCacheForAllRoles()
    {
        // Arrange
        var classId = Guid.NewGuid();
        var slotDate = DateOnly.FromDateTime(DateTime.Now);

        // Act
        await _slotService.InvalidateCacheForClassAsync(classId, slotDate);

        // Assert
        _redisCacheServiceMock.Verify(r => r.DeleteAsync(It.IsAny<string>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task GetWeeklyScheduleAsync_ReturnsSlotsFromDatabase_WhenCacheDoesNotExist()
    {
        // Arrange
        var slotModel = new GetSlotModel
        {
            StartTime = DateOnly.FromDateTime(DateTime.Now),
            EndTime = DateOnly.FromDateTime(DateTime.Now.AddDays(7)),
            Shifts = new List<Shift> { Shift.Shift1_7h_8h30 },
            SlotStatuses = new List<SlotStatus> { SlotStatus.NotStarted }
        };

        var accountModel = new AccountModel { 
            Role = Role.Student, 
            Email = "quangphat7a1@gmail.com", 
            AccountFirebaseId = "testUser" 
        };

        var classId = Guid.NewGuid();
        var slotsFromDb = new List<SlotSimpleModel>
        {
            new SlotSimpleModel { Id = Guid.NewGuid() },
            new SlotSimpleModel { Id = Guid.NewGuid() }
        };

        // Setup FindAsync to return a slot with the test class ID
        _slotRepositoryMock
            .Setup(r => r.FindAsync(
                It.IsAny<Expression<Func<Slot, bool>>>(),
                It.IsAny<bool>(),
                It.IsAny<bool>()))
            .ReturnsAsync(new List<Slot> { new Slot { Id = Guid.NewGuid() ,ClassId = classId } });

        // Setup cache to return null (cache miss)
        _redisCacheServiceMock
            .Setup(r => r.GetAsync<List<SlotSimpleModel>>(It.IsAny<string>()))
            .ReturnsAsync((List<SlotSimpleModel>)null);
    
        // Setup FindProjectedAsync to return slots when queried with the class ID
        _slotRepositoryMock
            .Setup(r => r.FindProjectedAsync<SlotSimpleModel>(
                It.Is<Expression<Func<Slot, bool>>>(expr => true),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<TrackingOption>()))
            .ReturnsAsync(slotsFromDb);

        // Act
        var result = await _slotService.GetWeeklyScheduleAsync(slotModel, accountModel);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(slotsFromDb.Count, result.Count);
    }
    

    [Fact]
    public async Task GetWeeklyScheduleAsync_ReturnsCachedSlots_WhenCacheExists()
    {
        // Arrange
        var slotModel = new GetSlotModel
        {
            StartTime = DateOnly.FromDateTime(DateTime.Now),
            EndTime = DateOnly.FromDateTime(DateTime.Now.AddDays(7)),
            Shifts = new List<Shift> { Shift.Shift1_7h_8h30 },
            SlotStatuses = new List<SlotStatus> { SlotStatus.NotStarted }
        };

        var accountModel = new AccountModel { 
            Role = Role.Student, 
            Email = "quangphat7a1@gmail.com", 
            AccountFirebaseId = "testUser" 
        };

        var classId = Guid.NewGuid();
        var cachedSlots = new List<SlotSimpleModel>
        {
            new SlotSimpleModel { Id = Guid.NewGuid() },
            new SlotSimpleModel { Id = Guid.NewGuid() }
        };

        // Setup to return a class ID when checking for the user's classes
        _slotRepositoryMock
            .Setup(r => r.FindAsync(
                It.IsAny<Expression<Func<Slot, bool>>>(),
                It.IsAny<bool>(),
                It.IsAny<bool>()))
            .ReturnsAsync(new List<Slot> { new Slot { Id = Guid.NewGuid() ,ClassId = classId } });

        // Match the specific cache key pattern using the class ID
        string cacheKeyPattern = $"schedule:{accountModel.Role}:class:{classId}:week:";
        _redisCacheServiceMock
            .Setup(r => r.GetAsync<List<SlotSimpleModel>>(It.Is<string>(s => s.Contains(cacheKeyPattern))))
            .ReturnsAsync(cachedSlots);

        // Act
        var result = await _slotService.GetWeeklyScheduleAsync(slotModel, accountModel);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(cachedSlots.Count, result.Count);
    }
    [Fact]
    public async Task UpdateSlot_ThrowsNotFoundException_WhenSlotNotFound()
    {
        // Arrange
        var updateSlotModel = new UpdateSlotModel
        {
            Id = Guid.NewGuid(),
            Shift = Shift.Shift1_7h_8h30,
            Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            RoomId = Guid.NewGuid()
        };

        _unitOfWorkMock.Setup(uow => uow.SlotRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Slot)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _slotService.UpdateSlot(updateSlotModel, "testUser"));
    }

    [Fact]
    public async Task UpdateSlot_ThrowsBadRequestException_WhenSlotAlreadyStarted()
    {
        // Arrange
        var updateSlotModel = new UpdateSlotModel
        {
            Id = Guid.NewGuid(),
            Shift = Shift.Shift1_7h_8h30,
            Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            RoomId = Guid.NewGuid()
        };

        var existingSlot = new Slot { Id = Guid.NewGuid(), Status = SlotStatus.Ongoing };

        _unitOfWorkMock.Setup(uow => uow.SlotRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(existingSlot);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _slotService.UpdateSlot(updateSlotModel, "testUser"));
    }

    [Fact]
    public async Task DeleteSlot_ThrowsNotFoundException_WhenSlotNotFound()
    {
        // Arrange
        var slotId = Guid.NewGuid();

        _unitOfWorkMock.Setup(uow => uow.SlotRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Slot)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _slotService.DeleteSlot(slotId, "testUser"));
    }

    [Fact]
    public async Task DeleteSlot_ThrowsBadRequestException_WhenSlotAlreadyStarted()
    {
        // Arrange
        var slotId = Guid.NewGuid();
        var existingSlot = new Slot { Id = Guid.NewGuid(), Status = SlotStatus.Ongoing };

        _unitOfWorkMock.Setup(uow => uow.SlotRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(existingSlot);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _slotService.DeleteSlot(slotId, "testUser"));
    }

    [Fact]
    public async Task GetAttendanceStatusAsync_ReturnsAttendanceStatus_WhenSlotExists()
    {
        // Arrange
        var slotId = Guid.NewGuid();
        var slot = new Slot
        {
            Id = slotId,
            SlotStudents = new List<SlotStudent>
            {
                new SlotStudent { StudentFirebaseId = "student1", AttendanceStatus = AttendanceStatus.Attended, CreatedById = "admin001", SlotId = Guid.NewGuid() },
                new SlotStudent { StudentFirebaseId = "student2", AttendanceStatus = AttendanceStatus.Absent, CreatedById = "admin001", SlotId = Guid.NewGuid() }
            }
        };

        _slotRepositoryMock
            .Setup(r => r.FindSingleProjectedAsync<Slot>(
                It.IsAny<Expression<Func<Slot, bool>>>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<TrackingOption>()))
            .ReturnsAsync(slot);

        // Act
        var result = await _slotService.GetAttendanceStatusAsync(slotId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains(result, r => r.StudentFirebaseId == "student1" && r.AttendanceStatus == AttendanceStatus.Attended);
        Assert.Contains(result, r => r.StudentFirebaseId == "student2" && r.AttendanceStatus == AttendanceStatus.Absent);
    }

    [Fact]
    public async Task GetAttendanceStatusAsync_ThrowsNotFoundException_WhenSlotDoesNotExist()
    {
        // Arrange
        var slotId = Guid.NewGuid();
    
        _slotRepositoryMock
            .Setup(r => r.FindSingleProjectedAsync<Slot>(
                It.IsAny<Expression<Func<Slot, bool>>>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<TrackingOption>()))
            .ReturnsAsync((Slot)null);
    
        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _slotService.GetAttendanceStatusAsync(slotId));
    }

    [Fact]
    public async Task CronAutoChangeSlotStatus_SetsExpiredSlotsToFinished_WhenNoTodaySlots()
    {
        // Arrange
        var pastDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1).AddHours(7));
        var slotClass = new Class { Id = Guid.NewGuid(), Name = "Class jne", CreatedById = "godIsMe123", Status = ClassStatus.NotStarted };

        var expiredSlots = new List<Slot>
        {
            new Slot {
                Id = Guid.NewGuid(),
                Status = SlotStatus.Ongoing,
                ClassId = slotClass.Id,
                Class = slotClass,
                Date = pastDate,
            }
        };

        _slotRepositoryMock
            .SetupSequence(r => r.FindAsync(
                It.IsAny<Expression<Func<Slot, bool>>>(),
                It.IsAny<bool>(),
                It.IsAny<bool>()))
            .ReturnsAsync(expiredSlots) // First call: pastSlots
            .ReturnsAsync(new List<Slot>());

        // Act
        await _slotService.CronAutoChangeSlotStatus();

        // Assert
        foreach (var slot in expiredSlots)
        {
            Assert.Equal(SlotStatus.Finished, slot.Status);
        }

        _classRepositoryMock.Verify(r => r.UpdateRangeAsync(It.IsAny<IEnumerable<Class>>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }
    
    [Fact]
    public async Task CronAutoChangeSlotStatus_ShouldUpdateClassAndSlotStatus_WhenFirstSlotOfTheDayIsNotStarted()
    {
        var fakeNow = new DateTime(2025, 4, 8, 9, 0, 0); // 9h00 -> during Shift2 (8h45 - 10h15)
        var today = DateOnly.FromDateTime(fakeNow);
        var classId = Guid.NewGuid();

        var slotClass = new Class { Id = classId, Status = ClassStatus.NotStarted, Name = "Class piano 1", CreatedById = "admin001"};
        var todaySlot = new Slot
        {
            Id = Guid.NewGuid(),
            Date = today,
            Shift = Shift.Shift2_8h45_10h15,
            ClassId = classId,
            Class = slotClass,
            Status = SlotStatus.NotStarted
        };

        _slotRepositoryMock
            .SetupSequence(r => r.FindAsync(
                It.IsAny<Expression<Func<Slot, bool>>>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new List<Slot>()) // pastSlots
            .ReturnsAsync(new List<Slot> { todaySlot }) // todaySlots
            .ReturnsAsync(new List<Slot> { todaySlot }); // allSlots

        _classRepositoryMock
            .Setup(r => r.FindAsync(It.IsAny<Expression<Func<Class, bool>>>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new List<Class> { slotClass });

        _slotRepositoryMock
            .Setup(r => r.FindProjectedAsync<Slot>(
                It.IsAny<Expression<Func<Slot, bool>>>(),
                It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<TrackingOption>()))
            .ReturnsAsync(new List<Slot>());

        var testService = new TestableSlotService(_serviceFactoryMock.Object, _unitOfWorkMock.Object, fakeNow);

        // Act
        await testService.CronAutoChangeSlotStatus();

        // Assert
        Assert.Equal(SlotStatus.Ongoing, todaySlot.Status);
        Assert.Equal(ClassStatus.Ongoing, slotClass.Status);
    }
    
     [Fact]
     public async Task PublicNewSlot_ThrowsNotFoundException_WhenRoomNotFound()
     {
         // Arrange
         var model = new PublicNewSlotModel
         {
             Shift = Shift.Shift1_7h_8h30,
             Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
             RoomId = Guid.NewGuid(),
             ClassId = Guid.NewGuid()
         };

         var accountFirebaseId = "testUser";

         _serviceFactoryMock
             .Setup(sf => sf.RoomService.GetRoomDetailById(model.RoomId))
             .ThrowsAsync(new NotFoundException("Room not found"));

         // Act & Assert
         await Assert.ThrowsAsync<NotFoundException>(() => _slotService.PublicNewSlot(model, accountFirebaseId));
     }

     [Fact]
     public async Task PublicNewSlot_ThrowsNotFoundException_WhenClassNotFound()
     {
         // Arrange
         var model = new PublicNewSlotModel
         {
             Shift = Shift.Shift1_7h_8h30,
             Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
             RoomId = Guid.NewGuid(),
             ClassId = Guid.NewGuid()
         };

         var accountFirebaseId = "testUser";

         _unitOfWorkMock.Setup(uow => uow.RoomRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new Room { Id = model.RoomId, CreatedById = "admin001" });
         _serviceFactoryMock
             .Setup(sf => sf.ClassService.GetClassDetailById(model.ClassId))
             .ThrowsAsync(new NotFoundException("Class not found"));

         // Act & Assert
         await Assert.ThrowsAsync<NotFoundException>(() => _slotService.PublicNewSlot(model, accountFirebaseId));
     }
     
     
     [Fact]
    public async Task PublicNewSlot_ThrowsIllegalArgumentException_WhenRoomIsClosed()
    {
        // Arrange
        var model = new PublicNewSlotModel
        {
            Shift = Shift.Shift1_7h_8h30,
            Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            RoomId = Guid.NewGuid(),
            ClassId = Guid.NewGuid()
        };

        var accountFirebaseId = "testUser";
        var roomDetail = new RoomDetailModel
        {
            Id = model.RoomId,
            Status = RoomStatus.Closed
        };

        // Setup room service to return a closed room
        _serviceFactoryMock
            .Setup(sf => sf.RoomService.GetRoomDetailById(model.RoomId))
            .ReturnsAsync(roomDetail);

        // Act & Assert
        await Assert.ThrowsAsync<IllegalArgumentException>(() => 
            _slotService.PublicNewSlot(model, accountFirebaseId));
    }

    [Fact]
    public async Task PublicNewSlot_ThrowsIllegalArgumentException_WhenSlotConflictExists()
    {
        // Arrange
        var model = new PublicNewSlotModel
        {
            Shift = Shift.Shift1_7h_8h30,
            Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            RoomId = Guid.NewGuid(),
            ClassId = Guid.NewGuid()
        };

        var accountFirebaseId = "testUser";
        var roomDetail = new RoomDetailModel
        {
            Id = model.RoomId,
            Status = RoomStatus.Opened
        };

        // Setup room service to return a valid room
        _serviceFactoryMock
            .Setup(sf => sf.RoomService.GetRoomDetailById(model.RoomId))
            .ReturnsAsync(roomDetail);

        // Setup slot repository to indicate a conflict
        _slotRepositoryMock
            .Setup(r => r.AnyAsync(It.IsAny<Expression<Func<Slot, bool>>>()))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<IllegalArgumentException>(() => 
            _slotService.PublicNewSlot(model, accountFirebaseId));
    }

    [Fact]
    public async Task PublicNewSlot_CreatesSlotSuccessfully_WhenAllConditionsAreMet()
    {
        // Arrange
        var model = new PublicNewSlotModel
        {
            Shift = Shift.Shift1_7h_8h30,
            Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            RoomId = Guid.NewGuid(),
            ClassId = Guid.NewGuid()
        };

 

         var account = new AccountModel()
         {
             AccountFirebaseId = "testUser",
             Email = "testUser123"
         };
        
        var roomDetail = new RoomDetailModel
        {
            Id = model.RoomId,
            Status = RoomStatus.Opened,
            Name = "Test Room"
        };
        
        var studentClasses = new List<StudentClass>
        {
            new StudentClass {Id = Guid.NewGuid(), ClassId = model.ClassId, CreatedById = "admin001" ,StudentFirebaseId = "student1" },
            new StudentClass {Id = Guid.NewGuid(), ClassId = model.ClassId, CreatedById = "admin001" ,StudentFirebaseId = "student2" }
        };
        
        
        var classDetail = new ClassDetailModel { Id = model.ClassId, Name = "Test Class", CreatedById = "admin001" };
       
        // Setup room service
        _serviceFactoryMock
            .Setup(sf => sf.RoomService.GetRoomDetailById(model.RoomId))
            .ReturnsAsync(roomDetail);
            
        // Setup slot repository - no conflict
        _slotRepositoryMock
            .Setup(r => r.AnyAsync(It.IsAny<Expression<Func<Slot, bool>>>()))
            .ReturnsAsync(false);
            
        // Capture the created slot
        Slot capturedSlot = new Slot 
        { 
            Id = Guid.NewGuid(),
            ClassId = model.ClassId,
            RoomId = model.RoomId,
            Shift = model.Shift,
            Date = model.Date,
            Status = SlotStatus.NotStarted
        };
        _slotRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Slot>()))
            .Callback<Slot>(s => capturedSlot = s)
            .ReturnsAsync(capturedSlot);
            
        // Setup student classes
        _studentClassRepositoryMock
            .Setup(r => r.FindAsync(It.IsAny<Expression<Func<StudentClass, bool>>>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(studentClasses);
            
        // Setup transaction
        _unitOfWorkMock
            .Setup(u => u.ExecuteInTransactionAsync(It.IsAny<Func<Task>>()))
            .Returns(async (Func<Task> action) => await action());
            
        // Setup student slot repository
        _slotStudentRepositoryMock
            .Setup(r => r.AddRangeAsync(It.IsAny<IEnumerable<SlotStudent>>()))
            .Returns(Task.CompletedTask);
            
        // Setup class service
        _serviceFactoryMock
            .Setup(sf => sf.ClassService.GetClassDetailById(model.ClassId))
            .ReturnsAsync(classDetail);
        
        
        // Setup notification service
        _notificationServiceMock
            .Setup(s => s.SendNotificationToManyAsync(
                It.IsAny<List<string>>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);
            
        // Setup Redis cache
        _redisCacheServiceMock
            .Setup(s => s.DeleteByPatternAsync(It.IsAny<string>()))
            .Returns(Task.CompletedTask);
            
        // Act
        var result = await _slotService.PublicNewSlot(model, account.AccountFirebaseId);
        
        // Assert
        Assert.NotNull(result);
        Assert.NotNull(capturedSlot);
        Assert.Equal(model.ClassId, capturedSlot.ClassId);
        Assert.Equal(model.RoomId, capturedSlot.RoomId);
        Assert.Equal(model.Shift, capturedSlot.Shift);
        Assert.Equal(model.Date, capturedSlot.Date);
        Assert.Equal(SlotStatus.NotStarted, capturedSlot.Status);
        
        _slotRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Slot>()), Times.Once);
        _slotStudentRepositoryMock.Verify(r => r.AddRangeAsync(It.IsAny<IEnumerable<SlotStudent>>()), Times.Once);
        _notificationServiceMock.Verify(s => s.SendNotificationToManyAsync(
            It.IsAny<List<string>>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        _redisCacheServiceMock.Verify(s => s.DeleteByPatternAsync("schedule:*"), Times.Once);
    }

}