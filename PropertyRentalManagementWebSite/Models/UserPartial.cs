using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PropertyRentalManagementWebSite.Models
{
    public partial class User
    {

        [NotMapped]
        [DataType(DataType.Password)]
        //[Required(ErrorMessage = "Password is required.")]
        public string PasswordString { get; set; }
    }
}