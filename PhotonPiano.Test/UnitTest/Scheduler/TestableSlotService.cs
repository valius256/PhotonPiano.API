using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.BusinessLogic.Services;
using PhotonPiano.DataAccess.Abstractions;

namespace PhotonPiano.Test.UnitTest.Scheduler;

class TestableSlotService : SlotService
{
    private readonly DateTime _fakeNow;

    public TestableSlotService(
        IServiceFactory serviceFactory,
        IUnitOfWork unitOfWork,
        DateTime fakeNow)
        : base(serviceFactory, unitOfWork)
    {
        _fakeNow = fakeNow;
    }

    protected override DateTime GetVietnamNow() => _fakeNow;
}
