using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TN_academic.Models;
using TN_academic.ViewModels;

namespace TN_academic.Controllers
{
    public class HomeController : Controller
    {
        private TN_academic_DBEntities db = new TN_academic_DBEntities();

        public ActionResult Index()
        {
            var listCourses = db.OrderDetails.Where(m=>m.Cours.Status == 1).OrderBy(m=>m.CourseID).Distinct().Take(8).ToList();
            ViewBag.CountStudent = (int)db.Users.Where(m => m.Role == "3").ToList().Count;
            ViewBag.CountCourse = (int)db.Courses.Where(m=>m.Status == 1).ToList().Count;
            ViewBag.CountBlog = (int)db.Blogs.Where(m => m.Status == 1).ToList().Count;
            ViewBag.NewCourses = db.Courses.Where(m => m.Status == 1).OrderByDescending(m=>m.CourseID).Take(5).ToList();
            ViewBag.Blogs = db.Blogs.Where(m => m.Status == 1).OrderByDescending(m => m.CreatedDate).Take(5).ToList();
            return View(listCourses);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public PartialViewResult _Header()
        {
            if (Session[Common.CommonConstants.USER_LOGIN_MODEL] != null)
            {
                var user = (UserModel)Session[Common.CommonConstants.USER_LOGIN_MODEL];
                ViewBag.FullName = user.FirstName + " " + user.LastName;
                ViewBag.Permission = true;
                if (Session[Common.CommonConstants.CART_SESSION] != null)
                {
                    ViewBag.CountCart = ((List<Cart>)Session[Common.CommonConstants.CART_SESSION]).Count;
                }

                if (user.Role.Equals("1"))
                {
                    ViewBag.IsAdmin = true;
                }
                if (user.Role.Equals("2"))
                {
                    ViewBag.IsIns = true;
                }
            }
            ViewBag.CategoriesList = db.Categories.ToList();
            ViewBag.BlogCategoriesList = db.BlogCategories.ToList();
            return PartialView();
        }

        public PartialViewResult _Footer()
        {
            ViewBag.CateList = db.Categories.Take(4).ToList();
            ViewBag.BlogCateList = db.BlogCategories.Take(4).ToList();
            return PartialView();
        }
    }
}