using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TN_academic.Models;

namespace TN_academic.CustomValidation
{
    public class ExistedUsername : ValidationAttribute
    {
        private TN_academic_DBEntities db = new TN_academic_DBEntities();

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult("Please enter a valid username.");
            }
            if (value.ToString().Length < 6 || value.ToString().Length > 50)
            {
                return new ValidationResult("The username must be between 6 and 50 characters long.");
            }
            if (db.Users.Find(value.ToString()) != null)
            {
                return new ValidationResult("The username '" + value + "' already exists. Try again, please!");
            }
            else
            {
                return ValidationResult.Success;
            }
        }
    }
}