using Entities_DTO;

namespace Service_Contracts
{
    public interface ICountriesAdderService
    {

        /// <summary>
        /// Add a new Country
        /// </summary>
        /// <param name="countryRequest"></param>
        /// <returns></returns>
        public Task<CountryResponse> AddCountry(CountryRequest countryRequest);

    }
}
