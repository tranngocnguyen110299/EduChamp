using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TN_academic.CustomValidation;

namespace TN_academic.Areas.Admin.Models
{
    public class ListeningQuestionModelForCreate
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ListeningQuestionModelForCreate()
        {
        }

        public int QuestionID { get; set; }

        public Nullable<int> TypeID { get; set; }

        [Required(ErrorMessage = "Please select a course for the question!")]
        [DisplayName("Course")]
        public Nullable<int> CourseID { get; set; }

        [Required(ErrorMessage = "Please enter description for the question!")]
        public string Description { get; set; }

        public string Content { get; set; }

        [Required(ErrorMessage = "Please enter choice A for the question!")]
        public string ChoiceA { get; set; }

        [Required(ErrorMessage = "Please enter choice A for the question!")]
        public string ChoiceB { get; set; }

        [Required(ErrorMessage = "Please enter choice A for the question!")]
        public string ChoiceC { get; set; }

        [Required(ErrorMessage = "Please enter choice A for the question!")]
        public string ChoiceD { get; set; }

        [Required(ErrorMessage = "Please enter choice A for the question!")]
        public string Answer { get; set; }

        [Required(ErrorMessage = "Please select a level for the question!")]
        public Nullable<int> Level { get; set; }

        [AudioValidationForCreate]
        public HttpPostedFileBase MediaFile { get; set; }
    }
}