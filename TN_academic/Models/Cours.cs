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
    using System.Web;
    using TN_academic.Areas.Admin.Models;

    public partial class Cours
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Cours()
        {
            this.CourseComments = new HashSet<CourseComment>();
            this.Examinations = new HashSet<Examination>();
            this.Lectures = new HashSet<Lecture>();
            this.OrderDetails = new HashSet<OrderDetail>();
            this.Questions = new HashSet<Question>();
        }
        public Cours(CourseModelForCreate course)
        {
            this.CourseID = course.CourseID;
            this.CourseName = course.CourseName;
            this.Username = course.Username;
            this.CategoryID = course.CategoryID;
            this.OldPrice = course.OldPrice;
            this.Price = course.Price;
            this.ShortDescription = course.ShortDescription;
            this.Description = course.Description;
            this.Thumbnail = course.Thumbnail;
            this.Status = course.Status;
            this.Quantity = course.Quantity;
        }

        public Cours(CourseModelForEdit course)
        {
            this.CourseID = course.CourseID;
            this.CourseName = course.CourseName;
            this.Username = course.Username;
            this.CategoryID = course.CategoryID;
            this.OldPrice = course.OldPrice;
            this.Price = course.Price;
            this.ShortDescription = course.ShortDescription;
            this.Description = course.Description;
            this.Thumbnail = course.Thumbnail;
            this.Status = course.Status;
            this.Quantity = course.Quantity;
        }

        public int CourseID { get; set; }
        public string CourseName { get; set; }
        public string Username { get; set; }
        public Nullable<int> CategoryID { get; set; }
        public Nullable<decimal> OldPrice { get; set; }
        public Nullable<decimal> Price { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public string Thumbnail { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<int> Quantity { get; set; }

        public HttpPostedFileBase ImageFile { get; set; }

        public virtual Category Category { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CourseComment> CourseComments { get; set; }
        public virtual User User { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Examination> Examinations { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Lecture> Lectures { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Question> Questions { get; set; }
    }
}