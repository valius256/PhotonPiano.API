using System.Linq.Expressions;
using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Survey;
using PhotonPiano.BusinessLogic.BusinessModel.SurveyQuestion;
using PhotonPiano.BusinessLogic.BusinessModel.SystemConfig;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.BusinessLogic.Services;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.Shared.Exceptions;
using PhotonPiano.Shared.Utils;

namespace PhotonPiano.Test.UnitTest.Survey;

[Collection("Piano survey unit tests")]
public class PianoSurveyServiceTest
{
    private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());

    private readonly Mock<IServiceFactory> _serviceFactoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    private readonly Mock<IPianoSurveyRepository> _pianoSurveyRepositoryMock;

    private readonly Mock<ISystemConfigService> _systemConfigServiceMock;

    private readonly IPianoSurveyService _pianoSurveyService;

    private readonly AccountModel _sampleStaff;

    private readonly AccountModel _sampleStudent;

    private readonly List<SystemConfigModel> _sampleConfigs =
    [
        new()
        {
            Id = Guid.NewGuid(),
            ConfigName = ConfigNames.InstrumentName,
            ConfigValue = "Piano"
        },
        new()
        {
            Id = Guid.NewGuid(),
            ConfigName = ConfigNames.InstrumentFrequencyInResponse,
            ConfigValue = "1"
        },
        new()
        {
            Id = Guid.NewGuid(),
            ConfigName = ConfigNames.InstrumentFrequencyInResponse,
            ConfigValue = "1"
        },
        new()
        {
            Id = Guid.NewGuid(),
            ConfigName = ConfigNames.MinQuestionsPerSurvey,
            ConfigValue = "1"
        },
        new()
        {
            Id = Guid.NewGuid(),
            ConfigName = ConfigNames.MaxQuestionsPerSurvey,
            ConfigValue = "10"
        },
        new()
        {
            Id = Guid.NewGuid(),
            ConfigName = ConfigNames.EntranceSurvey,
            ConfigValue = Guid.NewGuid().ToString()
        }
    ];

    public PianoSurveyServiceTest()
    {
        _sampleStaff = new AccountModel
        {
            AccountFirebaseId = "test",
            Email = "staff@test.com",
            Role = Role.Staff
        };

        _sampleStudent = new AccountModel
        {
            AccountFirebaseId = "test123",
            Email = "student@test.com",
            Role = Role.Student
        };

        _serviceFactoryMock = _fixture.Freeze<Mock<IServiceFactory>>();
        _unitOfWorkMock = _fixture.Freeze<Mock<IUnitOfWork>>();

        _pianoSurveyRepositoryMock = _fixture.Freeze<Mock<IPianoSurveyRepository>>();

        _systemConfigServiceMock = _fixture.Freeze<Mock<ISystemConfigService>>();


        _unitOfWorkMock.Setup(uow => uow.PianoSurveyRepository).Returns(_pianoSurveyRepositoryMock.Object);

        _serviceFactoryMock.Setup(sf => sf.SystemConfigService).Returns(_systemConfigServiceMock.Object);

        _pianoSurveyService = new PianoSurveyService(_unitOfWorkMock.Object, _serviceFactoryMock.Object);
    }

    [Fact]
    public async Task CreatePianoSurvey_InvalidQuestionCount_ThrowsBadRequestException()
    {
        //Arrange
        _systemConfigServiceMock.Setup(service => service.GetAllSurveyConfigs())
            .ReturnsAsync(_sampleConfigs);

        var createModel = _fixture.Build<CreatePianoSurveyModel>()
            .With(x => x.CreateQuestionRequests, [
            ])
            .Create();

        //Act
        var record =
            await Record.ExceptionAsync(() => _pianoSurveyService.CreatePianoSurvey(createModel, _sampleStaff));

        //Assert
        Assert.IsType<BadRequestException>(record);
        Assert.Equal("Number of questions in survey must between 1 and 10", record.Message);
    }

    [Fact]
    public async Task CreatePianoSurvey_InvalidSurveyName_ThrowsBadRequestException()
    {
        //Arrange
        _systemConfigServiceMock.Setup(service => service.GetAllSurveyConfigs())
            .ReturnsAsync(_sampleConfigs);

        var createModel = _fixture.Build<CreatePianoSurveyModel>()
            .With(x => x.CreateQuestionRequests, [
                _fixture.Create<CreateQuestionInSurveyModel>()
            ])
            .Create();

        //Act
        var record =
            await Record.ExceptionAsync(() => _pianoSurveyService.CreatePianoSurvey(createModel, _sampleStaff));

        //Assert
        Assert.IsType<BadRequestException>(record);
        Assert.Equal("The survey name doesn't contains the instrument name: Piano", record.Message);
    }

    [Fact]
    public async Task UpdatePianoSurvey_SurveyNotFound_ThrowsNotFoundException()
    {
        //Arrange
        var id = Guid.NewGuid();

        _pianoSurveyRepositoryMock.Setup(repo => repo.GetPianoSurveyWithQuestionsAsync(It.IsAny<Guid>()))
            .ReturnsAsync((PianoSurvey?)null);

        _systemConfigServiceMock.Setup(service => service.GetAllSurveyConfigs())
            .ReturnsAsync(_sampleConfigs);

        var updateModel = _fixture.Build<UpdatePianoSurveyModel>()
            .With(x => x.Questions, [
            ])
            .Create();

        //Act
        var record =
            await Record.ExceptionAsync(() => _pianoSurveyService.UpdatePianoSurvey(id, updateModel, _sampleStaff));

        //Assert
        Assert.IsType<NotFoundException>(record);
        Assert.Equal("Survey not found", record.Message);
    }

    [Fact]
    public async Task UpdatePianoSurvey_InvalidQuestionCount_ThrowsBadRequestException()
    {
        //Arrange
        var id = Guid.NewGuid();

        _pianoSurveyRepositoryMock.Setup(repo => repo.GetPianoSurveyWithQuestionsAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new PianoSurvey
            {
                Id = Guid.NewGuid(),
                Name = "Test Piano",
            });

        _systemConfigServiceMock.Setup(service => service.GetAllSurveyConfigs())
            .ReturnsAsync(_sampleConfigs);

        var updateModel = _fixture.Build<UpdatePianoSurveyModel>()
            .With(x => x.Name, "Test Piano")
            .With(x => x.Questions, [
            ])
            .Create();

        //Act
        var record =
            await Record.ExceptionAsync(() => _pianoSurveyService.UpdatePianoSurvey(id, updateModel, _sampleStaff));

        //Assert
        Assert.IsType<BadRequestException>(record);
        Assert.Equal("Number of questions in survey must between 1 and 10", record.Message);
    }

    [Fact]
    public async Task UpdatePianoSurvey_InvalidSurveyName_ThrowsBadRequestException()
    {
        //Arrange
        _systemConfigServiceMock.Setup(service => service.GetAllSurveyConfigs())
            .ReturnsAsync(_sampleConfigs);

        var createModel = _fixture.Build<CreatePianoSurveyModel>()
            .With(x => x.CreateQuestionRequests, [
                _fixture.Create<CreateQuestionInSurveyModel>()
            ])
            .Create();

        //Act
        var record =
            await Record.ExceptionAsync(() => _pianoSurveyService.CreatePianoSurvey(createModel, _sampleStaff));

        //Assert
        Assert.IsType<BadRequestException>(record);
        Assert.Equal("The survey name doesn't contains the instrument name: Piano", record.Message);
    }

    [Fact]
    public async Task GetSurveyDetails_SurveyNotFound_ThrowsNotFoundException()
    {
        //Arrange
        var id = Guid.NewGuid();

        _pianoSurveyRepositoryMock.Setup(repo => repo.FindSingleProjectedAsync<PianoSurveyDetailsModel>(
            It.IsAny<Expression<Func<PianoSurvey, bool>>>(), false, false,
            TrackingOption.Default, false
        )).ReturnsAsync((PianoSurveyDetailsModel?)null);

        //Act
        var record = await Record.ExceptionAsync(() => _pianoSurveyService.GetSurveyDetails(id, _sampleStaff));

        //Assert
        Assert.IsType<NotFoundException>(record);
        Assert.Equal("Survey not found", record.Message);
    }

    [Fact]
    public async Task GetSurveyDetails_ForbiddenAccess_ThrowsForbiddenMethodException()
    {
        //Arrange
        var id = Guid.NewGuid();

        _pianoSurveyRepositoryMock.Setup(repo => repo.FindSingleProjectedAsync<PianoSurveyDetailsModel>(
            It.IsAny<Expression<Func<PianoSurvey, bool>>>(), false, false,
            TrackingOption.Default, false
        )).ReturnsAsync(new PianoSurveyDetailsModel
        {
            Id = id,
            Name = "test",
            LearnerSurveys =
            [
                new LearnerSurveyWithAnswersModel
                {
                    LearnerId = "dg2yfr",
                    PianoSurveyId = default,
                    LearnerEmail = "vewfgwefewf@gmail.com",
                }
            ]
        });

        //Act
        var record = await Record.ExceptionAsync(() => _pianoSurveyService.GetSurveyDetails(id, _sampleStaff with
        {
            Role = Role.Student
        }));

        //Assert
        Assert.IsType<ForbiddenMethodException>(record);
        Assert.Equal("You do not have access to this survey", record.Message);
    }

    [Fact]
    public async Task DeletePianoSurvey_SurveyNotFound_ThrowsNotFoundException()
    {
        //Arrange
        var id = Guid.NewGuid();

        _pianoSurveyRepositoryMock.Setup(repo => repo.FindSingleAsync(
            It.IsAny<Expression<Func<PianoSurvey, bool>>>(), true, false
        )).ReturnsAsync((PianoSurvey?)null);

        //Act
        var record = await Record.ExceptionAsync(() => _pianoSurveyService.DeletePianoSurvey(id, _sampleStaff));

        //Assert
        Assert.IsType<NotFoundException>(record);
        Assert.Equal("Survey not found", record.Message);
    }

    [Fact]
    public async Task GetEntranceSurvey_SurveyNotFound_ThrowsNotFoundException()
    {
        //Arrange
        _systemConfigServiceMock.Setup(service => service.GetAllSurveyConfigs())
            .ReturnsAsync([]);

        //Act
        var record = await Record.ExceptionAsync(() => _pianoSurveyService.GetEntranceSurvey());

        //Assert
        Assert.IsType<NotFoundException>(record);
        Assert.Equal("Entrance survey not found", record.Message);
    }

    [Fact]
    public async Task SendEntranceSurveyAnswers_EntranceSurveyNotFound_ThrowsNotFoundException()
    {
        //Arrange
        var model = _fixture.Build<SendEntranceSurveyAnswersModel>()
            .Create();

        _systemConfigServiceMock.Setup(service => service.GetAllSurveyConfigs())
            .ReturnsAsync([]);

        //Act
        var record = await Record.ExceptionAsync(() => _pianoSurveyService.GetEntranceSurvey());

        //Assert
        Assert.IsType<NotFoundException>(record);
        Assert.Equal("Entrance survey not found", record.Message);
    }

    [Fact]
    public async Task SendEntranceSurveyAnswers_LackOfAnswers_ThrowsBadRequestException()
    {
        //Arrange
        var model = _fixture.Build<SendEntranceSurveyAnswersModel>()
            .With(x => x.SurveyAnswers, [])
            .Create();

        _systemConfigServiceMock.Setup(service => service.GetConfig(ConfigNames.EntranceSurvey, false, false))
            .ReturnsAsync(new SystemConfigModel
            {
                Id = Guid.NewGuid(),
                ConfigName = ConfigNames.EntranceSurvey,
                ConfigValue = Guid.NewGuid().ToString()
            });

        _pianoSurveyRepositoryMock.Setup(repo => repo.FindSingleProjectedAsync<PianoSurveyWithQuestionsModel>(
            It.IsAny<Expression<Func<PianoSurvey, bool>>>(), false, false, TrackingOption.Default
            , false)).ReturnsAsync(new PianoSurveyWithQuestionsModel
        {
            Id = Guid.NewGuid(),
            Name = "Test",
            PianoSurveyQuestions = new List<PianoSurveyQuestionModel>
            {
                _fixture.Create<PianoSurveyQuestionModel>()
            }
        });

        //Act
        var record = await Record.ExceptionAsync(() => _pianoSurveyService.SendEntranceSurveyAnswers(model));

        //Assert
        Assert.IsType<BadRequestException>(record);
        Assert.Equal("Some required questions are not answered", record.Message);
    }
}