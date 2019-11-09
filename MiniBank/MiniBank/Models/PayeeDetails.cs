using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MiniBank.Models
{
    public class PayeeDetails
    {
        public Account account { get; set; }
        public Payee payee { get; set; }

    }
}