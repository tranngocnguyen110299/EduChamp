using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TN_academic.CustomValidation;
using TN_academic.Models;

namespace TN_academic.Areas.Admin.Models
{
    public class CategoryModelForEdit
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CategoryModelForEdit()
        {
        }

        public CategoryModelForEdit(Category category)
        {
            this.CategoryID = category.CategoryID;
            this.CategoryName = category.CategoryName;
            this.Description = category.Description;
            this.Thumbnail = category.Thumbnail;
        }

        public int CategoryID { get; set; }
        [DisplayName("Category")]

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter a valid blog name!")]
        [StringLength(maximumLength: 150, MinimumLength = 5, ErrorMessage = "The category name must be between 5 and 150 characters long.")]
        public string CategoryName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter a valid blog name!")]
        [StringLength(maximumLength: 255, MinimumLength = 5, ErrorMessage = "Description must be between 5 and 255 characters long.")]
        public string Description { get; set; }

        public string Thumbnail { get; set; }

        [ImageValidationForUpdate]
        public HttpPostedFileBase ImageFile { get; set; }

    }
}