using FakeUserGenerator.Models;
namespace FakeUserGenerator.Services
{
    public interface IFakeErrorSimulationService
    {
        void SimulateError(User user, Random random, double errorCount);
    }
}
