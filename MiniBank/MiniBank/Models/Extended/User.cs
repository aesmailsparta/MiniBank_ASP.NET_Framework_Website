using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MiniBank
{
    public class RestrictedDate : ValidationAttribute
    {
        protected override ValidationResult IsValid(object date, ValidationContext validationContext)
        {
            if (date != null)
            {
                DateTime d = (DateTime)date;
                if (d < DateTime.Now || d == DateTime.MinValue)
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult(ErrorMessage);
                }
            }
            else
            {
                return ValidationResult.Success;
            }
        }
    }

    [MetadataType(typeof(UserMetadata))]
    public partial class User
    {
        [NotMapped]
        public string ConfirmPassword { get; set; }
    }

    public class UserMetadata
    {
        [Display(Name = "First Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter a first name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter a last name")]
        public string LastName { get; set; }

        [Display(Name = "Email")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter an email address")]
        [EmailAddress(ErrorMessage = "The email address you have entered is not valid")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Display(Name = "Date of birth")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [RestrictedDate(ErrorMessage = "Please enter a valid date of birth")]
        public DateTime DateOfBirth { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter a password")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Your password needs to be at least 6 characters long")]
        public string Password { get; set; }

        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The passwords you entered do not match")]
        public string ConfirmPassword { get; set; }

    }
}