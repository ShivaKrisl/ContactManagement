using System.ComponentModel.DataAnnotations;
using Entities_Core;

namespace Entities_DTO
{
    public class PersonRequest
    {
        /// <summary>
        /// First name of the person.
        /// </summary>
        [Required(ErrorMessage ="First Name is required")]
        [MaxLength(50, ErrorMessage ="Name cannot be more than 50 characters")]
        public string? FirstName { get; set; }

        /// <summary>
        /// Last name of the person.
        /// </summary>
        [Required(ErrorMessage = "Last Name cannot be Null")]
        [MaxLength(50, ErrorMessage = "Name cannot have more than 50 characters")]
        public string? LastName { get; set; }

        /// <summary>
        /// Date of birth of the person.
        /// </summary>
        [Required(ErrorMessage = "Date of Birth is required")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Email address of the person.
        /// </summary>
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = " Only valid email is allowed")]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }

        /// <summary>
        /// Phone number of the person.
        /// </summary>
        [Required(ErrorMessage = "Phone Number is required")]
        [Phone(ErrorMessage ="Only Valid phone number is allowed")]
        [DataType(DataType.PhoneNumber)]
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// Gender of the person.
        /// </summary>
        [Required(ErrorMessage = "Gender is required")]
        public GenderOptions Gender { get; set; }

        /// <summary>
        /// Country of residence of the person.
        /// </summary>
        [Required(ErrorMessage = "Country Id is required")]
        public Guid CountryId { get; set; }


        public Person ToPerson()
        {
            return new Person()
            {
                FirstName = this.FirstName,
                LastName = this.LastName,
                DateOfBirth = this.DateOfBirth,
                Email = this.Email,
                PhoneNumber = this.PhoneNumber,
                Gender = this.Gender.ToString(),
                CountryId = this.CountryId,
            };
        }

    }
}
