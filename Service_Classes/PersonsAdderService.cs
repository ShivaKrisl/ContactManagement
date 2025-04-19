using Entities_Core;
using Entities_DTO;
using Repository_Contracts;
using Service_Classes.Helpers;
using Service_Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_Classes
{
    public class PersonsAdderService : IPersonsAdderService
    {
        private readonly IPersonsRepository _personsRepository;

        private readonly ICountriesService _countriesService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="personsRepository"></param>
        /// <param name="countriesService"></param>
        public PersonsAdderService(IPersonsRepository personsRepository, ICountriesService countriesService)
        {
            _personsRepository = personsRepository;
            _countriesService = countriesService;
        }

        /// <summary>
        /// Add a Person to the database
        /// </summary>
        /// <param name="personRequest"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public async Task<PersonResponse> AddPerson(PersonRequest personRequest)
        {
            if (personRequest == null)
            {
                throw new ArgumentNullException(nameof(personRequest), "PersonRequest cannot be null");
            }

            bool isModelValid = ValidationHelper.IsModelValid(personRequest);
            if (!isModelValid)
            {
                throw new ArgumentException(nameof(personRequest), ValidationHelper.Errors);
            }

            bool isEmailExists = await _personsRepository.GetPersonByEmail(personRequest?.Email) != null ? true : false;
            if (isEmailExists)
            {
                throw new ArgumentException(nameof(personRequest.Email), "Email already exists");
            }

            bool isPhoneExists = await _personsRepository.GetPersonByMobile(personRequest?.PhoneNumber) != null ? true : false;

            if (isPhoneExists)
            {
                throw new ArgumentException(nameof(personRequest.PhoneNumber), "Phone number already exists");
            }

            CountryResponse? countryResponse = await _countriesService.GetCountryById(personRequest.CountryId);
            if (countryResponse == null)
            {
                throw new ArgumentException(nameof(personRequest.CountryId), "Country not found");
            }

            Person person = personRequest.ToPerson();
            person.PersonId = Guid.NewGuid();

            await _personsRepository.AddPerson(person);

            return person.ToPersonResponse();

        }
    }
}
