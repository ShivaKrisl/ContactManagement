using Entities_DTO;
using Service_Contracts;
using Entities_Core;
using Service_Classes.Helpers;
using Microsoft.EntityFrameworkCore;


namespace Service_Classes
{
    public class PersonsService : IPersonsService
    {

        private readonly ICountriesService _countriesService;

        private readonly ApplicationDbContext _db;

        public PersonsService(ICountriesService countriesService, ApplicationDbContext applicationDbContext)
        {
            _countriesService = countriesService;
            _db = applicationDbContext;
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

            bool isEmailExists = await _db.Persons.AnyAsync(p => p.Email == personRequest.Email);
            if (isEmailExists)
            {
                throw new ArgumentException(nameof(personRequest.Email), "Email already exists");
            }

            bool isPhoneExists = await _db.Persons.AnyAsync(p => p.PhoneNumber == personRequest.PhoneNumber);
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
            
            
            await _db.Persons.AddAsync(person);
            await _db.SaveChangesAsync();

            return person.ToPersonResponse();

        }

        /// <summary>
        /// Get All Persons
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<List<PersonResponse>?> GetAllPersons()
        {
            if (!await _db.Persons.AnyAsync())
            {
                return null;
            }

            return _db.Persons.Include("Country").Select(p => p.ToPersonResponse()).ToList();
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

            Person? person = await _db.Persons.Include("Country").FirstOrDefaultAsync(p => p.PersonId == personId);

            if(person == null)
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

            Person? person = await _db.Persons.FirstOrDefaultAsync(p => p.PersonId == personId);
            if (person == null)
            {
                return null;
            }

            bool isEmailExists = await _db.Persons.AnyAsync(p => p.Email == personRequest.Email && p.PersonId != personId);
            if (isEmailExists)
            {
                throw new ArgumentException(nameof(personRequest.Email), "Email already exists");
            }

            bool isPhoneExists = await _db.Persons.AnyAsync(p => p.PhoneNumber == personRequest.PhoneNumber && p.PersonId != personId);
            if (isPhoneExists)
            {
                throw new ArgumentException(nameof(personRequest.PhoneNumber), "Phone number already exists");
            }

            CountryResponse? countryResponse = await _countriesService.GetCountryById(personRequest.CountryId);
            if (countryResponse == null)
            {
                throw new ArgumentException(nameof(personRequest.CountryId), "Country not found");
            }

            person.FirstName = personRequest.FirstName;
            person.LastName = personRequest.LastName;
            person.Email = personRequest.Email;
            person.PhoneNumber = personRequest.PhoneNumber;
            person.DateOfBirth = personRequest.DateOfBirth;
            person.Gender = personRequest.Gender.ToString();
            person.CountryId = personRequest.CountryId;
            
            
            await _db.SaveChangesAsync();

            return person.ToPersonResponse();
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

            Person? person = await _db.Persons.FirstOrDefaultAsync(p => p.PersonId == personId);
            if (person == null)
            {
                return false;
            }
            _db.Persons.Remove(person);
            await _db.SaveChangesAsync();
            return true;
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
            if(string.IsNullOrEmpty(searchBy) || string.IsNullOrEmpty(searchValue))
            {
                return await this.GetAllPersons();
            }

            List<PersonResponse>? _persons = await this.GetAllPersons();

            if (_persons == null)
            {
                return null;
            }

            List<PersonResponse> personResponses = searchBy switch
            {
                nameof(PersonResponse.FirstName) =>
                _persons.Where(p => p.FirstName.Contains(searchValue, StringComparison.OrdinalIgnoreCase)).ToList(),

                nameof(PersonResponse.LastName) => 
                _persons.Where(p => p.LastName.Contains(searchValue, StringComparison.OrdinalIgnoreCase)).ToList(),

                nameof(PersonResponse.Email) => 
                _persons.Where(p => p.Email.Contains(searchValue, StringComparison.OrdinalIgnoreCase)).ToList(),

                nameof(PersonResponse.PhoneNumber) => 
                _persons.Where(p => p.PhoneNumber.Contains(searchValue, StringComparison.OrdinalIgnoreCase)).ToList(),

                _ => _persons
            };
            return personResponses;
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
           if(personResponses == null)
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
