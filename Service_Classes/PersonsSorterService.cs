using Service_Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities_DTO;
using Microsoft.Extensions.Logging;

namespace Service_Classes
{
    public class PersonsSorterService : IPersonsSorterService
    {

        private readonly ILogger<IPersonsSorterService> _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        public PersonsSorterService(ILogger<IPersonsSorterService> logger)
        {
            _logger = logger;
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
