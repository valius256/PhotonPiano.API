using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.EntityFrameworkCore;
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

namespace PhotonPiano.Test.UnitTest.Scheduler;

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
    public async Task GetSLotDetailById_ReturnsSlot_WhenSlotExists()
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
        var result = await _slotService.GetSLotDetailById(slotId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(slotId, result.Id);
    }

    [Fact]
    public async Task GetSLotDetailById_ThrowsNotFoundException_WhenSlotDoesNotExist()
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
        await Assert.ThrowsAsync<NotFoundException>(() => _slotService.GetSLotDetailById(slotId));
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

//    [Fact]
// public async Task CronAutoChangeSlotStatus_ChangesStatusOfExpiredSlots()
// {
//     // Arrange
//     var currentDate = DateOnly.FromDateTime(DateTime.UtcNow.AddHours(7));
//     var slotClass = new Class { Id = Guid.NewGuid(), Name = "Class jne",CreatedById = "godIsMe123",Status = ClassStatus.NotStarted };
//     
//     var expiredSlots = new List<Slot>
//     {
//         new Slot { 
//             Id = Guid.NewGuid(), 
//             Status = SlotStatus.NotStarted, 
//             ClassId = slotClass.Id, 
//             Class = slotClass 
//         },
//         new Slot { 
//             Id = Guid.NewGuid(), 
//             Status = SlotStatus.Ongoing, 
//             ClassId = slotClass.Id, 
//             Class = slotClass 
//         }
//     };
//
//     // Setup for past slots
//     _slotRepositoryMock
//         .Setup(r => r.FindAsync(
//             It.IsAny<Expression<Func<Slot, bool>>>(), 
//             It.IsAny<bool>(), 
//             It.IsAny<bool>()))
//         .ReturnsAsync(expiredSlots);
//
//     // Setup for today's slots
//     var todaySlots = expiredSlots.ToList();
//     var mockQueryable = todaySlots.AsQueryable();
//     
//     // Mock DbSet and IQueryable implementation
//     var mockDbSet = new Mock<DbSet<Slot>>();
//     mockDbSet.As<IQueryable<Slot>>().Setup(m => m.Provider).Returns(mockQueryable.Provider);
//     mockDbSet.As<IQueryable<Slot>>().Setup(m => m.Expression).Returns(mockQueryable.Expression);
//     mockDbSet.As<IQueryable<Slot>>().Setup(m => m.ElementType).Returns(mockQueryable.ElementType);
//     mockDbSet.As<IQueryable<Slot>>().Setup(m => m.GetEnumerator()).Returns(() => mockQueryable.GetEnumerator());
//     
//     // Mock ToListAsync to return your test data
//     _slotRepositoryMock.Setup(r => r.Entities).Returns(mockDbSet.Object);
//     
//     // Act
//     await _slotService.CronAutoChangeSlotStatus();
//
//     // Assert
//     // Verify slots were updated to Finished
//     foreach (var slot in expiredSlots)
//     {
//         Assert.Equal(SlotStatus.Finished, slot.Status);
//     }
//     
//     _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
//     _classRepositoryMock.Verify(r => r.UpdateRangeAsync(It.IsAny<IEnumerable<Class>>()), Times.Once);
// }
//
//
//     [Fact]
//     public async Task PublicNewSlot_ThrowsNotFoundException_WhenRoomNotFound()
//     {
//         // Arrange
//         var model = new PublicNewSlotModel
//         {
//             Shift = Shift.Shift1_7h_8h30,
//             Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
//             RoomId = Guid.NewGuid(),
//             ClassId = Guid.NewGuid()
//         };
//
//         var accountFirebaseId = "testUser";
//
//         _unitOfWorkMock.Setup(uow => uow.RoomRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Room)null);
//
//         // Act & Assert
//         await Assert.ThrowsAsync<NotFoundException>(() => _slotService.PublicNewSlot(model, accountFirebaseId));
//     }
//
//     [Fact]
//     public async Task PublicNewSlot_ThrowsNotFoundException_WhenClassNotFound()
//     {
//         // Arrange
//         var model = new PublicNewSlotModel
//         {
//             Shift = Shift.Shift1_7h_8h30,
//             Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
//             RoomId = Guid.NewGuid(),
//             ClassId = Guid.NewGuid()
//         };
//
//         var accountFirebaseId = "testUser";
//
//         _unitOfWorkMock.Setup(uow => uow.RoomRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new Room { Id = model.RoomId, CreatedById = "admin001" });
//         _unitOfWorkMock.Setup(uow => uow.ClassRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Class)null);
//
//         // Act & Assert
//         await Assert.ThrowsAsync<NotFoundException>(() => _slotService.PublicNewSlot(model, accountFirebaseId));
//     }

}