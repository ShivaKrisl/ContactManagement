using Entities_DTO;
using Service_Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities_Core;
using Service_Classes.Helpers;
using Microsoft.EntityFrameworkCore;
using Repository_Contracts;

namespace Service_Classes
{
    public class CountriesService : ICountriesService
    {

        private readonly ICountriesRepository _countriesRepository;

        public CountriesService(ICountriesRepository countriesRepository)
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
            if(countryRequest == null)
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
            if(await _countriesRepository.GetCountryByName(countryRequest.CountryName) != null)
            {
                throw new ArgumentException(nameof(countryRequest), "Country already exists");
            }

            Country country = countryRequest.ToCountry();
            country.CountryId = Guid.NewGuid();
            
            await _countriesRepository.AddCountry(country);

            return country.ToCountryResponse();

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
            if(countryId == Guid.Empty)
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
