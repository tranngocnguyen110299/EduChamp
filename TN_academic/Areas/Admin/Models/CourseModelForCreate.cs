using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TN_academic.CustomValidation;

namespace TN_academic.Areas.Admin.Models
{
    public class CourseModelForCreate
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CourseModelForCreate()
        {
        }

        public int CourseID { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter a valid course name!")]
        [StringLength(maximumLength: 50, MinimumLength = 5, ErrorMessage = "The course name must be between 5 and 50 characters long.")]
        [DisplayName("Course Name")]
        public string CourseName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please select a instructor for this course!")]
        [DisplayName("Instructor")]
        public string Username { get; set; }

        [DisplayName("Category")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please select a category for this course!")]
        public Nullable<int> CategoryID { get; set; }

        [DisplayName("Old Price")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter a valid old price!")]
        public Nullable<decimal> OldPrice { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter a valid price!")]
        public Nullable<decimal> Price { get; set; }

        [DisplayName("Short Description")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter short description for this course!")]
        [StringLength(maximumLength: 500, MinimumLength = 5, ErrorMessage = "Short description must be between 5 and 500 characters long.")]
        public string ShortDescription { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter description for this course!")]
        public string Description { get; set; }

        public string Thumbnail { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please select a status!")]
        public Nullable<int> Status { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter quantity for this course!")]
        public Nullable<int> Quantity { get; set; }

        [ImageValidationForCreate]
        public HttpPostedFileBase ImageFile { get; set; }
    }
}