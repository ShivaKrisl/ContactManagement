using Entities_Core;
using Entities_DTO;
using Service_Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repository_Contracts;

namespace Service_Classes
{
    public class CountriesGetterService : ICountriesGetterService
    {

        private readonly ICountriesRepository _countriesRepository;

        public CountriesGetterService(ICountriesRepository countriesRepository)
        {
            _countriesRepository = countriesRepository;
        }

        /// <summary>
        /// Get All Countries
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<List<CountryResponse>?> GetAllCountries()
        {
            return (await _countriesRepository.GetAllCountries())
             ?.Select(c => c.ToCountryResponse())
                 .ToList();
        }

        /// <summary>
        /// Get a Country by Country Id
        /// </summary>
        /// <param name="countryId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<CountryResponse?> GetCountryById(Guid countryId)
        {
            if (countryId == Guid.Empty)
            {
                throw new ArgumentException(nameof(countryId), "Country Id cannot be empty");
            }

            Country? country = await _countriesRepository.GetCountryById(countryId);
            if (country == null)
            {
                return null;
            }
            return country.ToCountryResponse();
        }
    }
}
