using Bogus;
using FakeUserGenerator.Models;

namespace FakeUserGenerator.Services
{
    public class FakeUserGeneratorService : IFakeUserGeneratorService
    {
        private readonly Dictionary<string, (Faker FirstNameFaker, Faker LastNameFaker, Faker AddressFaker, Faker PhoneFaker)> _regionFakers;
        private readonly IFakeErrorSimulationService _errorSimulationService;

        public FakeUserGeneratorService(IFakeErrorSimulationService errorSimulationService)
        {
            _errorSimulationService = errorSimulationService;
            
            _regionFakers = new Dictionary<string, (Faker, Faker, Faker, Faker)>
            {
                { "US+English", (new Faker("en_US"), new Faker("en_US"), new Faker("en_US"), new Faker("en_US")) },
                { "Poland+Polish", (new Faker("pl"), new Faker("pl"), new Faker("pl"), new Faker("pl")) },
                { "France+French", (new Faker("fr"), new Faker("fr"), new Faker("fr"), new Faker("fr")) }
            };
        }

        public List<User> GenerateUsers(string region, double errorCount, int seed, int pageNumber, int pageSize)
        {           
            if (!_regionFakers.ContainsKey(region))
                throw new ArgumentException($"Region '{region}' not found.");
            
            var (firstNameFaker, lastNameFaker, addressFaker, phoneFaker) = _regionFakers[region];
            
            int combinedSeed = seed + pageNumber;
            var random = new Random(combinedSeed);
            
            var users = new List<User>();
            var generatedFullNames = new HashSet<string>();
            for (int i = 0; i < pageSize; i++)
            {
                string fullName;
                do
                {                    
                    string firstName = firstNameFaker.Name.FirstName();
                    string lastName = lastNameFaker.Name.LastName();
                    fullName = $"{firstName} {lastName}";
                } while (!generatedFullNames.Add(fullName)); 

                
                string address = addressFaker.Address.FullAddress();
                string phone = phoneFaker.Phone.PhoneNumber();

                
                var user = new User
                {
                    Number = (pageNumber * pageSize) + i + 1, 
                    Id = Guid.NewGuid(),
                    FullName = fullName,
                    Address = address,
                    Phone = phone
                };

                
                _errorSimulationService.SimulateError(user, random, errorCount);

               
                users.Add(user);
            }

            return users;
        }
    }
}
