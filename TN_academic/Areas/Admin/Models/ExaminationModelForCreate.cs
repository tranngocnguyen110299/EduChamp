using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TN_academic.CustomValidation;

namespace TN_academic.Areas.Admin.Models
{
    public class ExaminationModelForCreate
    {
        [DisplayName("Exam ID")]
        public int ExamID { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter a valid exam name!")]
        [StringLength(maximumLength: 255, MinimumLength = 5, ErrorMessage = "The exam name must be between 5 and 255 characters long.")]
        [DisplayName("Exam name")]
        public string ExamName { get; set; }
        public Nullable<int> CourseID { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter a valid time")]
        [Range(maximum: Int64.MaxValue, minimum: 0, ErrorMessage = "Time must be a number")]
        public Nullable<int> Time { get; set; }
        [ValidateExaminationDate]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm:ss}")]
        public Nullable<System.DateTime> ExaminationDate { get; set; }
        public Nullable<int> Status { get; set; }
    }
}