using Entities_DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_Contracts
{
    public interface IPersonsSorterService
    {
        /// <summary>
        /// Sort Persons
        /// </summary>
        /// <param name="sortBy"></param>
        /// <param name="sortOrder"></param>
        /// <returns></returns>
        public Task<List<PersonResponse>?> SortPersons(List<PersonResponse>? personResponses, string sortBy, SortOrderEnum sortOrder);
    }
}
