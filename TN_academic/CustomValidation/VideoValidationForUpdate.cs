using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;

namespace TN_academic.CustomValidation
{
    public class VideoValidationForUpdate : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }
            HttpPostedFileBase file = (HttpPostedFileBase)value;
            string fileName = Path.GetFileNameWithoutExtension(file.FileName);
            string extension = Path.GetExtension(file.FileName);
            if ((extension == ".mp4" || extension == ".flv" || extension == ".webm" || extension == ".ogg" || extension == ".mov") == false)
            {
                return new ValidationResult("The File, which has extension is " + extension + ", hasn't accepted. The extension of the image must be .png or .jpg or .jpeg.");
            }

            long fileSize = ((file.ContentLength) / 1024);
            if (fileSize > 1048576)
            {
                return new ValidationResult("The File, which size greater than 1GB, hasn't accepted. Please try again!");
            }
            return ValidationResult.Success;

        }
    }
}