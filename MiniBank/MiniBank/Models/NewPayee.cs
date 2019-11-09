using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MiniBank.Models
{
    public class NewPayee
    {
        [Display(Name = "Payee Reference")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter a valid payee reference")]
        [MinLength(1, ErrorMessage = "Please enter a valid payee reference")]
        [MaxLength(100, ErrorMessage = "The reference is over 100 characters")]
        public string PayeeReference { get; set; }

        [Display(Name = "Payee Account")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter a valid account number")]
        [RegularExpression(@"^[0-9]+?$", ErrorMessage = "Please enter a valid account number")]
        [MinLength(6, ErrorMessage = "Please enter a valid account number")]
        [MaxLength(6, ErrorMessage = "Please enter a valid account number")]
        public string PayeeAccount { get; set; }

        [Display(Name = "Password")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter your password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}