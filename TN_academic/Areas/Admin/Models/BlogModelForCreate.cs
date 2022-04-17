using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TN_academic.CustomValidation;

namespace TN_academic.Areas.Admin.Models
{
    public class BlogModelForCreate
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public BlogModelForCreate()
        {
        }

        public int BlogID { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter a valid blog name!")]
        [StringLength(maximumLength: 255, MinimumLength = 5, ErrorMessage = "The blog name must be between 5 and 255 characters long.")]
        [DisplayName("Blog Name")]
        public string BlogName { get; set; }

        public string Username { get; set; }

        [DisplayName("Category")]
        public Nullable<int> CategoryID { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter a content of blog!")]
        public string Content { get; set; }

        public string Thumbnail { get; set; }

        [DisplayName("Created Date")]
        [Required(ErrorMessage = "Please enter a valid date!")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm:ss}")]
        public Nullable<System.DateTime> CreatedDate { get; set; }

        public Nullable<int> Status { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter short description!")]
        [StringLength(maximumLength: 100, MinimumLength = 5, ErrorMessage = "Short description must be between 5 and 100 characters long.")]
        public string ShortDescription { get; set; }

        [ImageValidationForCreate]
        public HttpPostedFileBase ImageFile { get; set; }
    }
}