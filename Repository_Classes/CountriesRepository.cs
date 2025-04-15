using Entities_Core;
using Repository_Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Repository_Classes
{
    public class CountriesRepository : ICountriesRepository
    {

        private readonly ApplicationDbContext _db;

        public CountriesRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Add a Country to a data base
        /// </summary>
        /// <param name="country"></param>
        /// <returns></returns>
        public async Task<Country> AddCountry(Country country)
        {
            _db.Countries.Add(country);
            await _db.SaveChangesAsync();
            return country;
        }

        /// <summary>
        /// Get all Countries from data base
        /// </summary>
        /// <returns></returns>
        public async Task<List<Country>?> GetAllCountries()
        {
            return await _db.Countries.ToListAsync();
        }

        /// <summary>
        /// Get Country By Id from data base
        /// </summary>
        /// <param name="countryId"></param>
        /// <returns></returns>
        public async Task<Country?> GetCountryById(Guid countryId)
        {
            Country? country = await _db.Countries.FirstOrDefaultAsync(c => c.CountryId == countryId);
            if (country == null)
            {
                return null;
            }
            return country;
        }

        /// <summary>
        /// Get Country By Name from data base
        /// </summary>
        /// <param name="countryName"></param>
        /// <returns></returns>
        public async Task<Country?> GetCountryByName(string countryName)
        {
            Country? country = await _db.Countries.FirstOrDefaultAsync(c => c.CountryName == countryName);
            if (country == null)
            {
                return null;
            }
            return country;

        }
    }
}
