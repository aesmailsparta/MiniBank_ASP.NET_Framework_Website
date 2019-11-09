using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MiniBank.Models
{
    public class AccountHelper
    {
        [Display(Name = "Email")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter an email address")]
        [EmailAddress(ErrorMessage = "The email address you have entered is not valid")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Display(Name = "Verification Code")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter a valid verification code")]
        [RegularExpression(@"^[0-9]+?$", ErrorMessage = "Please enter a valid verification code")]
        [MinLength(8, ErrorMessage = "Please enter a valid verification code")]
        [MaxLength(8, ErrorMessage = "Please enter a valid verification code")]
        public string QuickCode { get; set; }

        [Display(Name = "New Password")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter a password")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Your password needs to be at least 6 characters long")]
        public string Password { get; set; }

        [Display(Name = "Confirm New Password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The passwords you entered do not match")]
        public string ConfirmPassword { get; set; }

    }
}