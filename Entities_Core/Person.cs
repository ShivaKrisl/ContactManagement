using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities_Core
{
    public class Person
    {
        /// <summary>
        /// Unique identifier for the person.
        /// </summary>
        [Key]
        public Guid PersonId { get; set; }

        /// <summary>
        /// First name of the person.
        /// </summary>
        [Required]
        [MaxLength(50)]
        [StringLength(50)]
        public string? FirstName { get; set; }

        /// <summary>
        /// Last name of the person.
        /// </summary>
        [Required]
        [MaxLength(50)]
        [StringLength(50)]
        public string? LastName { get; set; }

        /// <summary>
        /// Date of birth of the person.
        /// </summary>
        [Required]
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Email address of the person.
        /// </summary>
        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }

        /// <summary>
        /// Phone number of the person.
        /// </summary>
        [Required]
        [Phone]
        [StringLength(15)]
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// Gender of the person.
        /// </summary>
        [Required]
        public string? Gender { get; set; }

        /// <summary>
        /// Country of residence of the person.
        /// </summary>
        [Required]
        public Guid CountryId { get; set; }

        /// <summary>
        /// Navigation property to the country entity.
        /// </summary>
        [Required]
        [ForeignKey("CountryId")]
        public virtual Country Country { get; set; }
    }
}
