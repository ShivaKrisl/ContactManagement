using Entities_Core;
using Entities_DTO;
using Repository_Contracts;
using Service_Classes.Helpers;
using Service_Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_Classes
{
    public class CountriesAdderService : ICountriesAdderService
    {

        private readonly ICountriesRepository _countriesRepository;

        public CountriesAdderService(ICountriesRepository countriesRepository)
        {
            _countriesRepository = countriesRepository;
        }

        /// <summary>
        /// Add a new Country
        /// </summary>
        /// <param name="countryRequest"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<CountryResponse> AddCountry(CountryRequest countryRequest)
        {
            if (countryRequest == null)
            {
                throw new ArgumentNullException(nameof(countryRequest));
            }

            if (string.IsNullOrEmpty(countryRequest.CountryName))
            {
                throw new ArgumentException(nameof(countryRequest.CountryName), "Country Name cannot be Null");
            }

            bool isValidRequest = ValidationHelper.IsModelValid(countryRequest);
            if (!isValidRequest)
            {
                throw new ArgumentException(nameof(countryRequest), ValidationHelper.Errors);
            }

            // Check if country already exists
            if (await _countriesRepository.GetCountryByName(countryRequest.CountryName) != null)
            {
                throw new ArgumentException(nameof(countryRequest), "Country already exists");
            }

            Country country = countryRequest.ToCountry();
            country.CountryId = Guid.NewGuid();

            await _countriesRepository.AddCountry(country);

            return country.ToCountryResponse();

        }
    }
}
