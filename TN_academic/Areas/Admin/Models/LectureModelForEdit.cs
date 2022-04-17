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
    public class LectureModelForEdit
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public LectureModelForEdit()
        {
        }

        public LectureModelForEdit(Lecture lecture)
        {
            this.LectureID = lecture.LectureID;
            this.LectureName = lecture.LectureName;
            this.CourseID = lecture.CourseID;
            this.Overview = lecture.Overview;
            this.Path = lecture.Path;
        }

        public int LectureID { get; set; }

        [DisplayName("Lecture Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter a valid lecture name!")]
        [StringLength(maximumLength: 255, MinimumLength = 5, ErrorMessage = "The lecture name must be between 5 and 255 characters long.")]
        public string LectureName { get; set; }

        [DisplayName("Lecture Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please select a category for this lecture!")]
        public Nullable<int> CourseID { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter overview for this lecture!")]
        public string Overview { get; set; }

        [DisplayName("Video")]
        public string Path { get; set; }

        [DisplayName("Video File")]
        [VideoValidationForUpdate]
        public HttpPostedFileBase VideoFile { get; set; }
    }
}