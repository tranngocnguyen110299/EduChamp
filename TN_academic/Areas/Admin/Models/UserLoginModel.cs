using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TN_academic.Areas.Admin.Models
{
    public class UserLoginModel
    {
        [Key]
        [Required(ErrorMessage = "Username must be not empty")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password must be not empty")]
        public string Password { get; set; }
    }
}