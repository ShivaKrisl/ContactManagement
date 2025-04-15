using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities_Core;

namespace Entities_DTO
{
    public class CountryRequest
    {
        /// <summary>
        /// Name of the country.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string CountryName { get; set; }

        /// <summary>
        /// Convert Country Request DTO to entity
        /// </summary>
        /// <returns></returns>
        public Country ToCountry()
        {
            return new Country()
            {
                CountryName = this.CountryName
            };
        }
    }
}
