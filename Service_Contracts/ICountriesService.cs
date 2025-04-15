using Entities_DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_Contracts
{
    public interface ICountriesService
    {
        /// <summary>
        /// Add a new Country
        /// </summary>
        /// <param name="countryRequest"></param>
        /// <returns></returns>
        public Task<CountryResponse> AddCountry(CountryRequest countryRequest);

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
