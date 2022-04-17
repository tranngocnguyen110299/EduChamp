//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TN_academic.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Web;
    using TN_academic.Areas.Admin.Models;

    public partial class Lecture
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Lecture()
        {
            this.LectureComments = new HashSet<LectureComment>();
        }

        public Lecture(LectureModelForCreate lecture)
        {
            this.LectureID = lecture.LectureID;
            this.LectureName = lecture.LectureName;
            this.CourseID = lecture.CourseID;
            this.Overview = lecture.Overview;
            this.Path = lecture.Path;
        }

        public Lecture(LectureModelForEdit lecture)
        {
            this.LectureID = lecture.LectureID;
            this.LectureName = lecture.LectureName;
            this.CourseID = lecture.CourseID;
            this.Overview = lecture.Overview;
            this.Path = lecture.Path;
        }

        public int LectureID { get; set; }
        [DisplayName("Lecture")]
        public string LectureName { get; set; }
        public Nullable<int> CourseID { get; set; }
        public string Overview { get; set; }
        [DisplayName("Video")]
        public string Path { get; set; }

        public virtual Cours Cours { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LectureComment> LectureComments { get; set; }
    }
}