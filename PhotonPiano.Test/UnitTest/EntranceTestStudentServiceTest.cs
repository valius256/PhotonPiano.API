using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using PhotonPiano.DataAccess.Abstractions;

namespace PhotonPiano.Test.UnitTest;

public class EntranceTestStudentServiceTest
{
    private readonly Mock<IEntranceTestStudentRepository> _entranceTestStudentRepositoryMock;
    private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());

    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public EntranceTestStudentServiceTest()
    {
        _entranceTestStudentRepositoryMock = _fixture.Freeze<Mock<IEntranceTestStudentRepository>>();
        _unitOfWorkMock = _fixture.Freeze<Mock<IUnitOfWork>>();
        _unitOfWorkMock.Setup(uow => uow.EntranceTestStudentRepository)
            .Returns(_entranceTestStudentRepositoryMock.Object);
    }

    //[Fact]
    //public async Task GetEntranceTestById_CorrectId_ReturnValidResult()
    //{
    //}
}