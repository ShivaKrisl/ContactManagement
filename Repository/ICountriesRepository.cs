using Entities_Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository_Contracts
{
    public interface ICountriesRepository
    {
        /// <summary>
        /// Add a Country
        /// </summary>
        /// <param name="country"></param>
        /// <returns></returns>
        public  Task<Country> AddCountry(Country country);

        /// <summary>
        /// Get All Countries
        /// </summary>
        /// <returns></returns>
        public Task<List<Country>?> GetAllCountries();

        /// <summary>
        /// Get Country By Id
        /// </summary>
        /// <param name="countryId"></param>
        /// <returns></returns>
        public Task<Country?> GetCountryById(Guid countryId);

        /// <summary>
        /// Get Country By Name
        /// </summary>
        /// <param name="countryName"></param>
        /// <returns></returns>
        public Task<Country?> GetCountryByName(string countryName);
    }
}
