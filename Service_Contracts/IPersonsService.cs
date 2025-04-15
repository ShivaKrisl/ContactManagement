using Entities_DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_Contracts
{
    public interface IPersonsService
    {
        /// <summary>
        /// Add a new Person
        /// </summary>
        /// <param name="personRequest"></param>
        /// <returns></returns>
        public Task<PersonResponse> AddPerson(PersonRequest personRequest);

        /// <summary>
        /// Get All Persons
        /// </summary>
        /// <returns></returns>
        public Task<List<PersonResponse>?> GetAllPersons();

        /// <summary>
        /// Get a Person by Person Id
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        public Task<PersonResponse?> GetPersonById(Guid personId);

        /// <summary>
        /// Update a Person by Person Id
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="personRequest"></param>
        /// <returns></returns>
        public Task<PersonResponse?> UpdatePerson(Guid personId, PersonRequest personRequest);

        /// <summary>
        /// Delete a Person by Person Id
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        public Task<bool> DeletePerson(Guid personId);

        /// <summary>
        /// Get Filtered Persons
        /// </summary>
        /// <param name="searchBy"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public Task<List<PersonResponse>?> GetFilteredPersons(string searchBy, string searchValue);

        /// <summary>
        /// Sort Persons
        /// </summary>
        /// <param name="sortBy"></param>
        /// <param name="sortOrder"></param>
        /// <returns></returns>
        public Task<List<PersonResponse>?> SortPersons(List<PersonResponse>? personResponses, string sortBy, SortOrderEnum sortOrder);
    }
}
