using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities_DTO;

namespace Service_Contracts
{
    public interface IPersonsGetterService
    {
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
        /// Get Filtered Persons
        /// </summary>
        /// <param name="searchBy"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public Task<List<PersonResponse>?> GetFilteredPersons(string searchBy, string searchValue);
    }
}
