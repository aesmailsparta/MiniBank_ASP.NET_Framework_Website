using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MiniBank.Models
{
    public class TransactionData
    {
        [Display(Name = "From Account")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter a valid account number")]
        public string FromAccountNumber { get; set; }

        [Display(Name = "To Account")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter a valid account number")]
        [RegularExpression(@"^[0-9]+?$", ErrorMessage = "Please enter a valid account number")]
        [MinLength(6, ErrorMessage = "Please enter a valid account number")]
        [MaxLength(6, ErrorMessage = "Please enter a valid account number")]
        public string ToAccountNumber { get; set; }

        [Display(Name = "Amount")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter a valid transfer amount")]
        [DisplayFormat(DataFormatString = "{0:C0}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Currency, ErrorMessage = "Please enter a valid transfer amount")]
        [RegularExpression(@"^[0-9]+(\.[0-9]{1,2})?$", ErrorMessage = "Please enter a valid transfer amount")]
        public decimal Amount { get; set; }

        [Display(Name = "Ref")]
        [MaxLength(50, ErrorMessage = "The reference you have entered is too long")]
        public string Reference { get; set; }

        [Display(Name = "Password")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter your password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}