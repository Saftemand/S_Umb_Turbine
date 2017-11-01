using Dynamicweb.Content.Items.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace S_Umb_Turbine.Model
{
    public class LoginModel
    {

        [Display(Name = "Username")]
        [System.ComponentModel.DataAnnotations.Required]
        public string Username { get; set; }

        [Display(Name = "Password")]
        [System.ComponentModel.DataAnnotations.Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

    }
}