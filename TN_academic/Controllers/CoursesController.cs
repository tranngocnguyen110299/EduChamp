using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using TN_academic.Models;
using TN_academic.ViewModels;

namespace TN_academic.Controllers
{
    public class CoursesController : Controller
    {
        private TN_academic_DBEntities db = new TN_academic_DBEntities();

        public ActionResult Index(int? page)
        {
            if (page == null) page = 1;
            int pageSize = 9;
            int pageNumber = (page ?? 1);
            var list = db.Courses.Where(m=>m.Status == 1).OrderByDescending(m => m.CourseID).ToList();
            var model = list.ToPagedList(pageNumber, pageSize);

            return View(model);
        }

        public ActionResult Details(int? id, int? page)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (page == null) page = 1;
            int pageSize = 8;
            int pageNumber = (page ?? 1);
            var courseComments = db.CourseComments.Where(m => m.CourseID == id && m.Status == true).OrderByDescending(m => m.CreatedDate).ToList();
            var model = courseComments.ToPagedList(pageNumber, pageSize);

            var course = db.Courses.FirstOrDefault(m => m.CourseID == id && m.Status == 1);
            ViewBag.Course = course;
            return View(model);
        }

        [HttpPost]
        public ActionResult Reply(int? commentID, string content, int courseID)
        {
            var user = (UserModel)Session[Common.CommonConstants.USER_LOGIN_MODEL];
            if (user is null)
            {
                TempData.Add(Common.CommonConstants.LOGIN_TO_COMMENT, true);
                return RedirectToAction("Index", "Courses");
            }
            if (commentID == null)
            {
                TempData.Add(Common.CommonConstants.INVALID_COMMENT, true);
                return RedirectToAction("Index", "Courses");
            }
            if (content.Length == 0)
            {
                TempData.Add(Common.CommonConstants.INVALID_BLOG_COMMENT, true);
                return RedirectToAction("Details", "Courses", new { id = courseID });
            }
            var rep = new ReplyingCourseComment();

            rep.Username = user.Username;
            rep.CommentID = commentID;
            rep.Content = content;
            rep.CreatedDate = DateTime.Now;
            rep.Status = 1;
            bool isValid = false;
            try
            {
                db.ReplyingCourseComments.Add(rep);
                if (db.SaveChanges() > 0)
                {
                    isValid = true;
                }
            }
            catch (Exception)
            {
                ViewBag.ErrorMessage = "Please complete the form before submitting";
                return RedirectToAction("Details", "Courses", new { id = courseID });
            }

            if (isValid == true)
            {
                TempData.Add(Common.CommonConstants.CREATE_SUCCESSFULLY, true);
                return RedirectToAction("Details", "Courses", new { id = courseID });
            }
            TempData.Add(Common.CommonConstants.CREATE_FAILED, true);
            return RedirectToAction("Details", "Courses", new { id = courseID });
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Comment(string comment, int? courseID)
        {
            var user = (UserModel)Session[Common.CommonConstants.USER_LOGIN_MODEL];
            if (user is null)
            {
                TempData.Add(Common.CommonConstants.LOGIN_TO_COMMENT, true);
                return RedirectToAction("Index", "Courses");
            }
            if (courseID == null)
            {
                TempData.Add(Common.CommonConstants.INVALID_COMMENT, true);
                return RedirectToAction("Index", "Home");
            }
            if (comment.Length == 0)
            {
                TempData.Add(Common.CommonConstants.INVALID_BLOG_COMMENT, true);
                return RedirectToAction("Details", "Courses", new { id = courseID });
            }
            
            var cmt = new CourseComment();

            cmt.Username = user.Username;
            cmt.Content = comment;
            cmt.CourseID = (int)courseID;
            cmt.CreatedDate = DateTime.Now;
            cmt.Status = true;
            bool isValid = false;
            try
            {
                db.CourseComments.Add(cmt);
                if (db.SaveChanges() > 0)
                {
                    isValid = true;
                }
            }
            catch (Exception)
            {
                ViewBag.ErrorMessage = "Please complete the form before submitting";
                return RedirectToAction("Details", "Courses", new { id = courseID });
            }

            if (isValid == true)
            {
                TempData.Add(Common.CommonConstants.CREATE_SUCCESSFULLY, true);
                return RedirectToAction("Details", "Courses", new { id = cmt.CourseID });
            }
            TempData.Add(Common.CommonConstants.CREATE_FAILED, true);
            return RedirectToAction("Details", "Courses", new { id = cmt.CourseID });
        }

        public ActionResult Category(int? id, int? page) {
            if (page == null) page = 1;
            int pageSize = 9;
            int pageNumber = (page ?? 1);
            var list = db.Courses.Where(m=>m.CategoryID == id && m.Status == 1).ToList();
            var model = list.ToPagedList(pageNumber, pageSize);
            ViewBag.Child = db.Categories.Find(id).CategoryName;
            return View(model);
        }

        [HttpPost]
        public ActionResult Search(string txtSearch, int? page)
        {
            if (page == null) page = 1;
            int pageSize = 9;
            int pageNumber = (page ?? 1);
            if (txtSearch != null)
            {
                var list = db.Courses.Where(m => m.CourseName.Contains(txtSearch) && m.Status ==1).ToList();
                var model = list.ToPagedList(pageNumber, pageSize);
                return View(model);
            }
            
            return View();
        }

        public PartialViewResult _CourseNavigation()
        {
            ViewBag.Courses = db.Categories.ToList();
            ViewBag.RecentCourses = db.Courses.Where(m => m.Status == 1).OrderByDescending(m => m.CourseID).Take(3).ToList();
            return PartialView();
        }

        
        public ActionResult MyCourses()
        {
            if (Session[Common.CommonConstants.USER_LOGIN_MODEL] == null)
            {
                TempData.Add(Common.CommonConstants.LOGIN_TO_LEARN, true);
                return RedirectToAction("Index", "Courses");
            }
            var user = (UserModel)Session[Common.CommonConstants.USER_LOGIN_MODEL];
            var course = db.OrderDetails.Where(c => c.Order.Username == user.Username && c.Order.Status == 1 && c.Order.PaymentStatus == 1);
            return View(course.ToList());
        }

        public ActionResult Learn(int? course, int? lecture, int? page)
        {
            var user = (UserModel)Session[Common.CommonConstants.USER_LOGIN_MODEL];
            if (page == null) page = 1;
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            if (user is null || course is null)
            {
                TempData.Add(Common.CommonConstants.LOGIN_TO_LEARN, true);
                return RedirectToAction("Index", "Home");
            }
            var access = db.OrderDetails.FirstOrDefault(i => i.CourseID == course && i.Order.Username == user.Username);
            if (access is null)
            {
                TempData.Add(Common.CommonConstants.ACCESS_DENIED, true);
                return RedirectToAction("MyCourses", "Courses");
            }
            ViewBag.LectureList = db.Lectures.Where(c => c.CourseID == course).ToList();
            Lecture ltr;
            if (lecture != null)
            {
                ltr = db.Lectures.FirstOrDefault(i => i.CourseID == course && i.LectureID == lecture);
                if (ltr == null)
                {
                    TempData.Add(Common.CommonConstants.ACCESS_DENIED, true);
                    return RedirectToAction("MyCourses", "Courses");
                }
                ViewBag.Lecture = ltr;
            }
            else
            {
                ltr = db.Lectures.FirstOrDefault(i => i.CourseID == course);
                if (ltr == null)
                {
                    TempData.Add(Common.CommonConstants.ACCESS_DENIED, true);
                    return RedirectToAction("MyCourses", "Courses");
                }
                ViewBag.Lecture = ltr;
            }
            var cmtList = db.LectureComments.Where(l => l.LectureID == ltr.LectureID && l.Status == true).OrderByDescending(l => l.CreatedDate).ToList();
            var model = cmtList.ToPagedList(pageNumber, pageSize);
            return View(model);
        }

        public JsonResult CommentLecture(String comment, String lectureID)
        {
            var user = (UserModel)Session[Common.CommonConstants.USER_LOGIN_MODEL];
            if (user is null)
            {
                return null;
            }
            if (Session[Common.CommonConstants.USER_LOGIN_MODEL] == null)
            {
                return Json(new
                {
                    status = false
                });
            }
            if (comment.Equals("") || lectureID.Equals("") || Session[Common.CommonConstants.USER_LOGIN_MODEL] is null)
            {
                return Json(new
                {
                    status = false
                });
            }
            var jsonID = new JavaScriptSerializer().Deserialize<int>(lectureID);
            var jsonComment = new JavaScriptSerializer().Deserialize<String>(comment);
            LectureComment cmt = new LectureComment();
            cmt.Username = user.Username;
            cmt.LectureID = jsonID;
            cmt.Content = jsonComment;
            cmt.CreatedDate = DateTime.Now;
            cmt.Status = true;

            db.LectureComments.Add(cmt);
            db.SaveChanges();
            return Json(new
            {
                status = true,
                cmt= jsonComment,
                name = user.FirstName + " " + user.LastName,
                date = ((DateTime)cmt.CreatedDate).ToString("dd/MM/yyyy h:mm:ss tt"),
                content = cmt.Content,
                avt = Url.Content(user.Avatar)
            });
        }




    }
}