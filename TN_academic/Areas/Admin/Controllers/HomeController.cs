using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TN_academic.Areas.Admin.Models;
using TN_academic.Models;

namespace TN_academic.Areas.Admin.Controllers
{
    [Authorize(Roles = "1")]
    public class HomeController : BaseController
    {
        private TN_academic_DBEntities db = new TN_academic_DBEntities();

        public ActionResult Index()
        {
            ReportModel report = CreateReport();

            var chartList = DrawSaleChart();
            ViewBag.LabelSaleChart = chartList.Select(x => x.Label.ToShortDateString());
            ViewBag.SaleChart = chartList.Select(x => x.Total);

            var cateChart = DrawCategoriesChart();
            ViewBag.CateLabel = cateChart.Select(x => x.Label);
            ViewBag.CateData = cateChart.Select(x => x.Quantity);

            ViewBag.RencentlyBlog = GetRecentlyAddedBlogs();
            ViewBag.RecentlyOrders = GetRecentlyOrder();

            return View(report);
        }

        [ChildActionOnly]
        public ActionResult Aside()
        {
            var admin = (AdminLoginModel)Session[Common.CommonConstants.ADMIN_LOGIN_SESSION];
            ViewBag.FullName = admin.FirstName + " " + admin.LastName;
            ViewBag.Avatar = admin.Avatar;
            ViewBag.UserRole = admin.Role;
            return PartialView();
        }

        private List<CategoriesChart> DrawCategoriesChart()
        {
            List<CategoriesChart> cateChart = new List<CategoriesChart>();
            foreach (var item in db.Categories.ToList())
            {
                CategoriesChart chart = new CategoriesChart();
                chart.Label = item.CategoryName;
                chart.Quantity = item.Courses.Count;
                cateChart.Add(chart);
            }
            
            return cateChart;
        }

        private List<Chart> DrawSaleChart()
        {
            DateTime today = DateTime.Today;
            List<Chart> chartList = new List<Chart>();
            for (int i = 15; i >= 0; i--)
            {
                Chart chart = new Chart();
                chart.Label = today.AddDays(-(i + 1));
                DateTime end = today.AddDays(-i);
                DateTime start = today.AddDays(-(i + 1));
                chartList.Add(chart);
                db.Orders.Where(n => n.CreatedDate >= start && n.CreatedDate < end).ToList().ForEach(item => chart.Total += (long)item.Total);
            }
            return chartList;
        }

        private ReportModel CreateReport()
        {
            var report = new ReportModel();
            report.UserRegistration = db.Users.Where(u => u.Role == "3" || u.Role == "2").ToList().Count;
            report.RegisteredCourses = db.OrderDetails.ToList().Count;
            report.TotalCourses = db.Courses.ToList().Count;
            report.Proceeds = 0;
            db.Orders.ToList().ForEach(item => report.Proceeds += (long)item.Total);
            report.BlogApproval = db.Blogs.Where(b => b.Status == -1).ToList().Count;
            DateTime today = DateTime.Today;
            DateTime sevenDayAgo = today.AddDays(-7);
            db.Orders.Where(i => i.CreatedDate >= sevenDayAgo).ToList().ForEach(item => report.WeekProceeds += (long)item.Total);
            DateTime thirtyDayAgo = today.AddDays(-30);
            DateTime tomorrow = today.AddDays(-1);
            db.Orders.Where(i => i.CreatedDate >= thirtyDayAgo).ToList().ForEach(item => report.MonthProceeds += (long)item.Total);
            db.Orders.Where(i => i.CreatedDate >= tomorrow).ToList().ForEach(item => report.ToDayProceeds += (long)item.Total);
            report.TotalComments = db.CourseComments.ToList().Count
                                    + db.ReplyingCourseComments.ToList().Count
                                    + db.LectureComments.ToList().Count
                                    + db.ReplyingLectureComments.ToList().Count
                                    + db.BlogComments.ToList().Count
                                    + db.ReplyingBlogComments.ToList().Count;
            report.TotalBlogs = db.Blogs.ToList().Count;
            report.GraduatedStudent = db.ExamResults.Where(i => i.Mark >= 75).ToList().Count;
            report.OpenedExam = db.Examinations.Where(e => e.Status == 1).ToList().Count;
            return report;
        }

        private List<Blog> GetRecentlyAddedBlogs()
        {
            return db.Blogs.Where(b => b.Status == 1).OrderByDescending(b => b.CreatedDate).Take(5).ToList();
        }

        private List<Order> GetRecentlyOrder()
        {
            return db.Orders.Where(o => o.Status == 1).OrderByDescending(o => o.CreatedDate).Take(8).ToList();
        }
    }
}