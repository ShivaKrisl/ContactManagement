using Entities_Core;
using Entities_DTO;
using Microsoft.Extensions.Logging;
using Repository_Contracts;
using Serilog;
using SerilogTimings;
using Service_Contracts;

namespace Service_Classes
{
    public class PersonsGetterService : IPersonsGetterService
    {

        private readonly IPersonsRepository _personsRepository;

        private readonly ILogger<IPersonsGetterService> _logger;

        private readonly IDiagnosticContext _diagnosticContext;

        public PersonsGetterService(IPersonsRepository personsRepository, ILogger<IPersonsGetterService> logger, IDiagnosticContext diagnosticContext)
        {
            _personsRepository = personsRepository;
            _logger = logger;
            _diagnosticContext = diagnosticContext;
        }

        /// <summary>
        /// Get All Persons
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<List<PersonResponse>?> GetAllPersons()
        {
            return (await _personsRepository.GetAllPersons())?
            .Select(p => p.ToPersonResponse()).ToList();
        }

        /// <summary>
        /// Get a Person by Person Id
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<PersonResponse?> GetPersonById(Guid personId)
        {
            if (personId == Guid.Empty)
            {
                throw new ArgumentException(nameof(personId), "Invalid Person Id");
            }

            Person? person = await _personsRepository.GetPersonById(personId);

            if (person == null)
            {
                return null;
            }

            return person.ToPersonResponse();
        }

        /// <summary>
        /// Get Filtered Persons
        /// </summary>
        /// <param name="searchBy"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<List<PersonResponse>?> GetFilteredPersons(string searchBy, string searchValue)
        {

            _logger.LogInformation("GetFilteredPersons called");
            _logger.LogDebug($"searchBy : {searchBy} and searchValue : {searchValue}");

            if (string.IsNullOrEmpty(searchBy) || string.IsNullOrEmpty(searchValue))
            {
                return await this.GetAllPersons();
            }

            List<PersonResponse> matchingPersons = new List<PersonResponse>();

            using (Operation.Time("Time for filtered persons"))
            {

                matchingPersons = searchBy switch
                {
                    nameof(PersonResponse.FirstName) =>
                    (await _personsRepository.GetFilteredPersons(p => p.FirstName.Contains(searchValue, StringComparison.OrdinalIgnoreCase)))?.Select(p => p.ToPersonResponse()).ToList(),

                    nameof(PersonResponse.LastName) =>
                    (await _personsRepository.GetFilteredPersons(p => p.LastName.Contains(searchValue, StringComparison.OrdinalIgnoreCase)))?.Select(p => p.ToPersonResponse()).ToList(),

                    nameof(PersonResponse.Email) =>
                    (await _personsRepository.GetFilteredPersons(p => p.Email.Contains(searchValue, StringComparison.OrdinalIgnoreCase)))?.Select(p => p.ToPersonResponse()).ToList(),

                    nameof(PersonResponse.PhoneNumber) =>
                    (await _personsRepository.GetFilteredPersons(p => p.PhoneNumber.Contains(searchValue, StringComparison.OrdinalIgnoreCase)))?.Select(p => p.ToPersonResponse()).ToList(),

                    _ => await this.GetAllPersons()
                };
            }

            // Log the filtered persons
            _diagnosticContext.Set("Persons", matchingPersons);

            return matchingPersons;
        }

    }
}
