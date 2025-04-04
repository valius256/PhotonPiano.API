using System.Linq.Expressions;
using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using PhotonPiano.BusinessLogic.BusinessModel.Account;
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

[Collection("Survey questions service unit tests")]
public class SurveyQuestionServiceTest
{
    private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());

    private readonly Mock<IServiceFactory> _serviceFactoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    private readonly Mock<ISurveyQuestionRepository> _surveyQuestionRepositoryMock;
    private readonly Mock<IPianoSurveyRepository> _pianoSurveyRepositoryMock;

    private readonly Mock<ISystemConfigService> _systemConfigServiceMock;

    private readonly ISurveyQuestionService _surveyQuestionService;

    private readonly AccountModel _sampleStaff;

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

    public SurveyQuestionServiceTest()
    {
        _sampleStaff = new AccountModel
        {
            AccountFirebaseId = "test",
            Email = "staff@test.com",
            Role = Role.Staff
        };
        _serviceFactoryMock = _fixture.Freeze<Mock<IServiceFactory>>();
        _unitOfWorkMock = _fixture.Freeze<Mock<IUnitOfWork>>();

        _surveyQuestionRepositoryMock = _fixture.Freeze<Mock<ISurveyQuestionRepository>>();
        _pianoSurveyRepositoryMock = _fixture.Freeze<Mock<IPianoSurveyRepository>>();

        _systemConfigServiceMock = _fixture.Freeze<Mock<ISystemConfigService>>();

        _unitOfWorkMock.Setup(uow => uow.SurveyQuestionRepository).Returns(_surveyQuestionRepositoryMock.Object);
        _unitOfWorkMock.Setup(uow => uow.PianoSurveyRepository).Returns(_pianoSurveyRepositoryMock.Object);

        _serviceFactoryMock.Setup(sf => sf.SystemConfigService).Returns(_systemConfigServiceMock.Object);

        _surveyQuestionService = new SurveyQuestionService(_unitOfWorkMock.Object, _serviceFactoryMock.Object);
    }

    [Fact]
    public async Task GetSurveyQuestionDetails_QuestionNotFound_ThrowsNotFoundException()
    {
        //Arrange
        var id = Guid.NewGuid();

        _surveyQuestionRepositoryMock.Setup(repo => repo.FindSingleProjectedAsync<SurveyQuestionDetailsModel>(
            It.IsAny<Expression<Func<SurveyQuestion, bool>>>(), false, false, TrackingOption.IdentityResolution
        )).ReturnsAsync((SurveyQuestionDetailsModel?)null);

        //Act
        var record = await Record.ExceptionAsync(() => _surveyQuestionService.GetSurveyQuestionDetails(id));

        //Assert
        Assert.IsType<NotFoundException>(record);
        Assert.Equal("Survey question not found", record.Message);
    }

    [Fact]
    public async Task GetQuestionAnswers_QuestionNotFound_ThrowsNotFoundException()
    {
        //Arrange
        var id = Guid.NewGuid();
        var queryModel = new QueryPagedAnswersModel
        {
            SortColumn = "Id"
        };

        _surveyQuestionRepositoryMock.Setup(repo => repo.AnyAsync(It.IsAny<Expression<Func<SurveyQuestion, bool>>>()))
            .ReturnsAsync(false);

        //Act
        var record =
            await Record.ExceptionAsync(() => _surveyQuestionService.GetQuestionAnswers(id, queryModel, _sampleStaff));

        //Assert
        Assert.IsType<NotFoundException>(record);
        Assert.Equal("Survey question not found", record.Message);
    }

    [Fact]
    public async Task CreateSurveyQuestion_EmptyOptionsForQuestionType_ThrowsBadRequestException()
    {
        //Arrange
        var createModel = _fixture.Build<CreateSurveyQuestionModel>()
            .With(x => x.Type, QuestionType.MultipleChoice)
            .With(x => x.Options, [])
            .Create();

        //Act
        var record =
            await Record.ExceptionAsync(() => _surveyQuestionService.CreateSurveyQuestion(createModel, _sampleStaff));

        //Assert
        Assert.IsType<BadRequestException>(record);
        Assert.Equal("Options can't be empty for this question type", record.Message);
    }

    [Fact]
    public async Task CreateSurveyQuestion_InvalidOptionsData_ThrowsBadRequestException()
    {
        //Arrange
        var createModel = _fixture.Build<CreateSurveyQuestionModel>()
            .With(x => x.Type, QuestionType.MultipleChoice)
            .With(x => x.Options, ["abc"])
            .Create();

        _systemConfigServiceMock.Setup(service => service.GetAllSurveyConfigs())
            .ReturnsAsync(_sampleConfigs);

        //Act
        var record =
            await Record.ExceptionAsync(() => _surveyQuestionService.CreateSurveyQuestion(createModel, _sampleStaff));

        //Assert
        Assert.IsType<BadRequestException>(record);
        Assert.Equal("Options in question must contain the instrument name Piano at least 1 times", record.Message);
    }

    [Fact]
    public async Task CreateSurveyQuestion_SurveyNotFound_ThrowsNotFoundException()
    {
        //Arrange
        var createModel = _fixture.Build<CreateSurveyQuestionModel>()
            .With(x => x.SurveyId, Guid.NewGuid())
            .With(x => x.Type, QuestionType.MultipleChoice)
            .With(x => x.Options, ["Piano"])
            .Create();

        _pianoSurveyRepositoryMock.Setup(repo => repo.GetPianoSurveyWithQuestionsAsync(It.IsAny<Guid>()))
            .ReturnsAsync((PianoSurvey?)null);

        //Act
        var record =
            await Record.ExceptionAsync(() => _surveyQuestionService.CreateSurveyQuestion(createModel, _sampleStaff));

        //Assert
        Assert.IsType<NotFoundException>(record);
        Assert.Equal("Survey not found", record.Message);
    }

    [Fact]
    public async Task CreateSurveyQuestion_ConflictOrderIndex_ThrowsConflictException()
    {
        //Arrange
        var createModel = _fixture.Build<CreateSurveyQuestionModel>()
            .With(x => x.OrderIndex, 0)
            .With(x => x.SurveyId, Guid.NewGuid())
            .With(x => x.Type, QuestionType.MultipleChoice)
            .With(x => x.Options, ["Piano"])
            .Create();

        _pianoSurveyRepositoryMock.Setup(repo => repo.GetPianoSurveyWithQuestionsAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new PianoSurvey
            {
                Id = Guid.NewGuid(),
                Name = "Piano",
                PianoSurveyQuestions = new List<PianoSurveyQuestion>
                {
                    new()
                    {
                        Id = Guid.NewGuid(),
                        OrderIndex = 0
                    }
                }
            });

        //Act
        var record =
            await Record.ExceptionAsync(() => _surveyQuestionService.CreateSurveyQuestion(createModel, _sampleStaff));

        //Assert
        Assert.IsType<ConflictException>(record);
        Assert.Equal("This order index is already in use", record.Message);
    }

    [Fact]
    public async Task CreateSurveyQuestion_InvalidAgeConstraint_ThrowsBadRequestException()
    {
        //Arrange
        var createModel = _fixture.Build<CreateSurveyQuestionModel>()
            .With(x => x.MinAge, 1)
            .With(x => x.MaxAge, 50)
            .With(x => x.OrderIndex, 1)
            .With(x => x.SurveyId, Guid.NewGuid())
            .With(x => x.Type, QuestionType.MultipleChoice)
            .With(x => x.Options, ["Piano"])
            .Create();

        _pianoSurveyRepositoryMock.Setup(repo => repo.GetPianoSurveyWithQuestionsAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new PianoSurvey
            {
                Id = Guid.NewGuid(),
                Name = "Piano",
                MinAge = 3,
                MaxAge = 30,
                PianoSurveyQuestions = new List<PianoSurveyQuestion>
                {
                    new()
                    {
                        Id = Guid.NewGuid(),
                        OrderIndex = 0
                    }
                }
            });

        //Act
        var record =
            await Record.ExceptionAsync(() => _surveyQuestionService.CreateSurveyQuestion(createModel, _sampleStaff));

        //Assert
        Assert.IsType<BadRequestException>(record);
        Assert.Equal("This question has invalid age for this survey", record.Message);
    }

    [Fact]
    public async Task UpdateSurveyQuestion_QuestionNotFound_ThrowsNotFoundException()
    {
        //Arrange
        var id = Guid.NewGuid();

        _surveyQuestionRepositoryMock.Setup(repo =>
                repo.FindSingleAsync(It.IsAny<Expression<Func<SurveyQuestion, bool>>>(), true, false))
            .ReturnsAsync((SurveyQuestion?)null);

        //Act
        var record = await Record.ExceptionAsync(() =>
            _surveyQuestionService.UpdateSurveyQuestion(id, _fixture.Create<UpdateSurveyQuestionModel>(),
                _sampleStaff));

        //Assert
        Assert.IsType<NotFoundException>(record);
        Assert.Equal("Survey question not found.", record.Message);
    }

    [Fact]
    public async Task DeleteSurveyQuestion_QuestionNotFound_ThrowsNotFoundException()
    {
        //Arrange
        var id = Guid.NewGuid();

        _surveyQuestionRepositoryMock.Setup(repo =>
                repo.FindSingleAsync(It.IsAny<Expression<Func<SurveyQuestion, bool>>>(), true, false))
            .ReturnsAsync((SurveyQuestion?)null);

        //Act
        var record = await Record.ExceptionAsync(() =>
            _surveyQuestionService.DeleteSurveyQuestion(id, _sampleStaff));

        //Assert
        Assert.IsType<NotFoundException>(record);
        Assert.Equal("Survey question not found.", record.Message);
    }
}