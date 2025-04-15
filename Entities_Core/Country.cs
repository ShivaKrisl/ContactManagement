using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Entities_Core
{
    public class Country
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
        [StringLength(100)]
        public string? CountryName { get; set; }

        // Navigation property to the list of persons in the country

        /// <summary>
        /// List of persons associated with the country.
        /// </summary>
        public virtual ICollection<Person>? Persons { get; set; }

        public override string ToString()
        {
            return $"{this.CountryName}";
        }
    }
}
