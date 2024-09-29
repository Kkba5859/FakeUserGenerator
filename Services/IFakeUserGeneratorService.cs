using FakeUserGenerator.Models;
using System.Collections.Generic;

namespace FakeUserGenerator.Services
{
    public interface IFakeUserGeneratorService
    {
        List<User> GenerateUsers(string region, double errorCount, int seed, int pageNumber, int pageSize);
    }
}
