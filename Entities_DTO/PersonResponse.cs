using Entities_Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities_DTO
{
    public class PersonResponse
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
        public string? FirstName { get; set; }

        /// <summary>
        /// Last name of the person.
        /// </summary>
        [Required]
        [MaxLength(50)]
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
        public string? Email { get; set; }

        /// <summary>
        /// Phone number of the person.
        /// </summary>
        [Required]
        [Phone]
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
        public string CountryName { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj == null || obj.GetType() != this.GetType())
            {
                return false;
            }

            PersonResponse personResponse = (PersonResponse)obj;

            return (
                    personResponse.FirstName == this.FirstName && personResponse.LastName == this.LastName && personResponse.Email == this.Email && personResponse.PhoneNumber == this.PhoneNumber && personResponse.PersonId == this.PersonId && personResponse.DateOfBirth == this.DateOfBirth &&personResponse.Gender == this.Gender &&  personResponse.CountryId == this.CountryId
                );
        }

        public PersonRequest ToPersonRequest()
        {
            return new PersonRequest()
            {
                FirstName = this.FirstName,
                LastName = this.LastName,
                DateOfBirth = this.DateOfBirth,
                Email = this.Email,
                PhoneNumber = this.PhoneNumber,
                Gender = Enum.TryParse<GenderOptions>(this.Gender?.ToUpper(), out GenderOptions gender) ? gender : GenderOptions.OTHER,
                CountryId = this.CountryId,
            };
        }
    }

    public static class PersonExtesions
    {
        public static PersonResponse ToPersonResponse(this Person person)
        {
            return new PersonResponse()
            {
                PersonId = person.PersonId,
                FirstName = person.FirstName,
                LastName = person.LastName,
                DateOfBirth = person.DateOfBirth,
                Email = person.Email,
                PhoneNumber = person.PhoneNumber,
                Gender = person.Gender,
                CountryId = person.CountryId,
                CountryName = person.Country?.CountryName
            };
        }
    }

}
