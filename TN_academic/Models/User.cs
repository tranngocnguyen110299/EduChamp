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
    using TN_academic.Areas.Admin.Models;
    using TN_academic.ViewModels;

    public partial class User
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public User()
        {
            this.BlogComments = new HashSet<BlogComment>();
            this.Blogs = new HashSet<Blog>();
            this.CourseComments = new HashSet<CourseComment>();
            this.Courses = new HashSet<Cours>();
            this.ExamResults = new HashSet<ExamResult>();
            this.LectureComments = new HashSet<LectureComment>();
            this.Orders = new HashSet<Order>();
            this.ReplyingBlogComments = new HashSet<ReplyingBlogComment>();
            this.ReplyingCourseComments = new HashSet<ReplyingCourseComment>();
            this.ReplyingLectureComments = new HashSet<ReplyingLectureComment>();
        }
        
        public User(UserModelForEdit user)
        {
            this.Username = user.Username;
            this.FirstName = user.FirstName;
            this.LastName = user.LastName;
            this.Password = user.Password;
            this.DOB = user.DOB;
            this.Gender = user.Gender;
            this.Phone = user.Phone;
            this.Email = user.Email;
            this.Address = user.Address;
            this.Avatar = user.Avatar;
            this.Role = user.Role;
            this.StatusID = user.StatusID;
        }

        public User(UserModelForCreate user)
        {
            this.Username = user.Username;
            this.FirstName = user.FirstName;
            this.LastName = user.LastName;
            this.Password = user.Password;
            this.DOB = user.DOB;
            this.Gender = user.Gender;
            this.Phone = user.Phone;
            this.Email = user.Email;
            this.Address = user.Address;
            this.Avatar = user.Avatar;
            this.Role = user.Role;
            this.StatusID = user.StatusID;
        }

        public User(UserRegisterModel user)
        {
            this.Username = user.Username;
            this.FirstName = user.FirstName;
            this.LastName = user.LastName;
            this.Password = user.Password;
            this.DOB = user.DOB;
            this.Gender = user.Gender;
            this.Phone = user.Phone;
            this.Email = user.Email;
            this.Address = user.Address;
            this.Avatar = user.Avatar;
            this.Role = user.Role;
            this.StatusID = user.StatusID;
        }

        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public Nullable<bool> Gender { get; set; }
        public Nullable<System.DateTime> DOB { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Avatar { get; set; }
        public Nullable<int> StatusID { get; set; }
        public string Role { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BlogComment> BlogComments { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Blog> Blogs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CourseComment> CourseComments { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Cours> Courses { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ExamResult> ExamResults { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LectureComment> LectureComments { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Order> Orders { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReplyingBlogComment> ReplyingBlogComments { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReplyingCourseComment> ReplyingCourseComments { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReplyingLectureComment> ReplyingLectureComments { get; set; }
        public virtual UserStatu UserStatu { get; set; }
    }
}
