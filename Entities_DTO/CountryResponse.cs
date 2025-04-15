using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities_Core;

namespace Entities_DTO
{
    public class CountryResponse
    {
        /// <summary>
        /// Unique identifier for the country.
        /// </summary>
        [Key]
        public Guid CountryId { get; set; }

        /// <summary>
        /// Name of the country.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string? CountryName { get; set; }

        /// <summary>
        /// Overrides the default Equals method to compare two CountryResponse objects.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj)
        {
            if (obj == null || obj.GetType() != this.GetType()) return false;

            CountryResponse countryResponse = (CountryResponse)obj;
            return this.CountryId == countryResponse.CountryId &&
                   this.CountryName == countryResponse.CountryName;
        }

        /// <summary>
        /// Converts the CountryResponse object to a Country object.
        /// </summary>
        /// <returns></returns>
        public Country ToCountry()
        {
            return new Country()
            {
                CountryId = this.CountryId,
                CountryName = this.CountryName,
            };
        }
    }

    public static class CountryExtensions
    {
        public static CountryResponse ToCountryResponse(this Country country)
        {
            return new CountryResponse()
            {
                CountryId = country.CountryId,
                CountryName = country.CountryName,
            };
        }
    }
}
