using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace TN_academic.Areas.Admin.Models
{
    public class ReportModel
    {
        [DisplayName("User Registration")]
        public int UserRegistration { get; set; }

        [DisplayName("Registered Courses")]
        public int RegisteredCourses { get; set; }

        [DisplayName("Total Courses")]
        public int TotalCourses { get; set; }

        public long Proceeds { get; set; }

        [DisplayName("The Blogs Pending Approval")]
        public int BlogApproval { get; set; }

        [DisplayName("Total sales last 7 days")]
        public long WeekProceeds { get; set; }

        [DisplayName("Total sales last 30 days")]
        public long MonthProceeds { get; set; }

        [DisplayName("Total sales last 1 day")]
        public long ToDayProceeds { get; set; }

        [DisplayName("Total Comments")]
        public int TotalComments { get; set; }

        [DisplayName("Total Blogs")]
        public int TotalBlogs { get; set; }

        [DisplayName("Good Students")]
        public int GraduatedStudent { get; set; }

        [DisplayName("Opened Exam")]
        public int OpenedExam { get; set; }

    }
}