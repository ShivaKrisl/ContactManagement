﻿using Entities_DTO;
using Service_Contracts;
using Entities_Core;
using Service_Classes.Helpers;
using Microsoft.EntityFrameworkCore;
using Repository_Contracts;
using Microsoft.Extensions.Logging;
using Serilog;
using SerilogTimings;
using Exceptions;

namespace Service_Classes
{
    public class PersonsService : IPersonsService
    {

        private readonly ICountriesService _countriesService;

        private readonly IPersonsRepository _personsRepository;

        // logger for PersonsService logging
        private readonly ILogger<PersonsService> _logger;

        private readonly IDiagnosticContext _diagnosticContext;

        public PersonsService(ICountriesService countriesService, IPersonsRepository personsRepository, ILogger<PersonsService> logger, IDiagnosticContext diagnosticContext)
        {
            _countriesService = countriesService;
            _personsRepository = personsRepository;

            _diagnosticContext = diagnosticContext; // for logging the context
            _logger = logger; // logger for PersonsService
        }

       
        //private async Task<PersonResponse> ConvertToPersonResponse(Person person)
        //{
        //    CountryResponse? countryResponse = await _countriesService.GetCountryById(person.CountryId);
        //    PersonResponse personResponse = person.ToPersonResponse();
        //    personResponse.CountryName = countryResponse?.CountryName;
        //    return personResponse;
        //}

        /// <summary>
        /// Add a new Person
        /// </summary>
        /// <param name="personRequest"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<PersonResponse> AddPerson(PersonRequest personRequest)
        {
            if(personRequest == null)
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
            if(personId == Guid.Empty)
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
        /// Update a Person by Person Id
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="personRequest"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<PersonResponse?> UpdatePerson(Guid personId, PersonRequest personRequest)
        {
            if(personId == Guid.Empty)
            {
                throw new ArgumentException(nameof(personId), "Invalid Person Id");
            }

            if (personRequest == null)
            {
                throw new ArgumentException(nameof(personRequest), "PersonRequest cannot be null");
            }

            
            bool isModelValid = ValidationHelper.IsModelValid(personRequest);
            if (!isModelValid)
            {
                throw new ArgumentException(nameof(personRequest), ValidationHelper.Errors);
            }

            Person? person = await _personsRepository.GetPersonById(personId);
            if (person == null)
            {
                throw new PersonNotFoundException("Person not found!!");
            }

            Person? p = await _personsRepository.GetPersonByEmail(personRequest?.Email);
            if (p != null && p.PersonId != personId)
            {
                throw new ArgumentException(nameof(personRequest.Email), "Email already exists");
            }

            p = await _personsRepository.GetPersonByMobile(personRequest?.PhoneNumber);
            if (p != null && p.PersonId != personId)
            {
                throw new ArgumentException(nameof(personRequest.PhoneNumber), "Phone number already exists");
            }

            CountryResponse? countryResponse = await _countriesService.GetCountryById(personRequest.CountryId);
            if (countryResponse == null)
            {
                throw new ArgumentException(nameof(personRequest.CountryId), "Country not found");
            }

           Person personUpdated = await _personsRepository.UpdatePerson(personId, personRequest.ToPerson());

            return personUpdated.ToPersonResponse();
        }

        /// <summary>
        /// Delete Person by Id
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<bool> DeletePerson(Guid personId)
        {
           if(personId == Guid.Empty)
            {
                throw new ArgumentException(nameof(personId), "Invalid Person Id");
            }

            return await _personsRepository.DeletePerson(personId);
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

        /// <summary>
        /// Sort Persons
        /// </summary>
        /// <param name="sortBy"></param>
        /// <param name="sortOrder"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<List<PersonResponse>?> SortPersons(List<PersonResponse>? personResponses, string sortBy, SortOrderEnum sortOrder)
        {
            _logger.LogInformation("SortPersons called");
            _logger.LogDebug($"sortBy : {sortBy} and sortOrder : {sortOrder}");

            if (personResponses == null)
            {
                return null;
            }

            List<PersonResponse> sortedPersonResponses = (sortBy, sortOrder) switch
            {
                (nameof(PersonRequest.FirstName), SortOrderEnum.ASCENDING) => personResponses.OrderBy(p => p.FirstName).ToList(),

                (nameof(PersonRequest.FirstName), SortOrderEnum.DESCENDING) => personResponses.OrderByDescending(p => p.FirstName).ToList(),

                (nameof(PersonRequest.LastName), SortOrderEnum.ASCENDING) => personResponses.OrderBy(p => p.LastName).ToList(),

                (nameof(PersonRequest.LastName), SortOrderEnum.DESCENDING) => personResponses.OrderByDescending(p => p.LastName).ToList(),

                (nameof(PersonResponse.Email), SortOrderEnum.ASCENDING) => personResponses.OrderBy(p => p.Email).ToList(),

                (nameof(PersonResponse.Email), SortOrderEnum.DESCENDING) => personResponses.OrderByDescending(p => p.Email).ToList(),

                (nameof(PersonResponse.PhoneNumber), SortOrderEnum.ASCENDING) => personResponses.OrderBy(p => p.PhoneNumber).ToList(),

                (nameof(PersonResponse.PhoneNumber), SortOrderEnum.DESCENDING) => personResponses.OrderByDescending(p => p.PhoneNumber).ToList(),

                _ => personResponses
            };

            return sortedPersonResponses;
        }

        
    }
}
