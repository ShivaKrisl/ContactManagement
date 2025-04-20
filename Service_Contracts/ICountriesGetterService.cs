using Entities_DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_Contracts
{
    public interface ICountriesGetterService
    {
        /// <summary>
        /// Get All Countries
        /// </summary>
        /// <returns></returns>
        public Task<List<CountryResponse>?> GetAllCountries();

        /// <summary>
        /// Get a Country by Country Id
        /// </summary>
        /// <param name="countryId"></param>
        /// <returns></returns>
        public Task<CountryResponse?> GetCountryById(Guid countryId);
    }
}
