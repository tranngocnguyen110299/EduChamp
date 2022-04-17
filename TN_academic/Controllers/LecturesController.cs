using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TN_academic.Areas.Admin.Models;
using TN_academic.Models;
using TN_academic.ViewModels;

namespace TN_academic.Controllers
{
    public class LecturesController : BaseInstructorController
    {
        private TN_academic_DBEntities db = new TN_academic_DBEntities();

        public ActionResult Index()
        {
            var user = (UserModel)Session[Common.CommonConstants.USER_LOGIN_MODEL];
            var courses = db.Courses.Where(i => i.Username == user.Username).ToList();
            foreach (var item in courses.ToList())
            {
                if (item.ShortDescription.Length > 122)
                    item.ShortDescription = item.ShortDescription.Substring(0, 122) + "...";
            }
            return View(courses.ToList());
        }

        public ActionResult Details(int? id)
        {
            var user = (UserModel)Session[Common.CommonConstants.USER_LOGIN_MODEL];
            if (id == null)
            {
                TempData.Add(Common.CommonConstants.ACCESS_DENIED, true);
                return RedirectToAction("Index", "Home");
            }
            Cours cours = db.Courses.FirstOrDefault(i => i.CourseID == id && i.Username == user.Username);
            if (cours == null)
            {
                TempData.Add(Common.CommonConstants.ACCESS_DENIED, true);
                return RedirectToAction("Index", "Home");
            }
            return View(cours);
        }

        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName");
            ViewBag.StatusList = new SelectList(new List<SelectListItem> { new SelectListItem { Text = "Open Registration", Value = "1" }, new SelectListItem { Text = "Stop Registration", Value = "0" }, }, "Value", "Text");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create([Bind(Include = "CourseID,CourseName,Username,CategoryID,OldPrice,Price,ShortDescription,Description,Thumbnail,Status,Quantity,ImageFile")] CourseModelForCreate course)
        {
            if (ModelState.IsValid)
            {
                string fileName = Path.GetFileNameWithoutExtension(course.ImageFile.FileName) + DateTime.Now.ToString("yymmssfff") + Path.GetExtension(course.ImageFile.FileName);

                course.Thumbnail = "~/public/uploadedFiles/coursesPictures/" + fileName;

                string uploadFolderPath = Server.MapPath("~/public/uploadedFiles/coursesPictures/");

                if (Directory.Exists(uploadFolderPath) == false)
                {
                    Directory.CreateDirectory(uploadFolderPath);
                }

                fileName = Path.Combine(uploadFolderPath, fileName);

                course.ImageFile.SaveAs(fileName);

                var courseEntity = new Cours(course);

                db.Courses.Add(courseEntity);

                if (db.SaveChanges() > 0)
                {
                    TempData.Add(Common.CommonConstants.CREATE_SUCCESSFULLY, true);
                }

                return RedirectToAction("Index");
            }

            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName");
            ViewBag.Username = new SelectList(db.Users.Where(u => u.Role.Equals("2")), "Username", "FirstName");
            ViewBag.StatusList = new SelectList(new List<SelectListItem> { new SelectListItem { Text = "Open Registration", Value = "1" }, new SelectListItem { Text = "Stop Registration", Value = "0" }, }, "Value", "Text");
            return View(course);
        }

        public ActionResult Edit(int? id)
        {
            var user = (UserModel)Session[Common.CommonConstants.USER_LOGIN_MODEL];
            if (id == null)
            {
                TempData.Add(Common.CommonConstants.ACCESS_DENIED, true);
                return RedirectToAction("Index", "Home");
            }
            Cours cours = db.Courses.FirstOrDefault(i => i.CourseID == id && i.Username == user.Username);
            Session.Add(Common.CommonConstants.TEMP_COURSE_IMAGE, cours.Thumbnail);
            if (cours == null)
            {
                TempData.Add(Common.CommonConstants.ACCESS_DENIED, true);
                return RedirectToAction("Index", "Home");
            }
            ViewBag.CategoryList = new SelectList(db.Categories, "CategoryID", "CategoryName");
            ViewBag.InstructorList = new SelectList(db.Users.Where(i => i.Role.Equals("2")), "Username", "FirstName");
            ViewBag.StatusList = new SelectList(new List<SelectListItem> { new SelectListItem { Text = "Open Registration", Value = "1" }, new SelectListItem { Text = "Stop Registration", Value = "0" }, }, "Value", "Text");
            return View(new CourseModelForEdit(cours));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit([Bind(Include = "CourseID,CourseName,Username,CategoryID,OldPrice,Price,ShortDescription,Description,Thumbnail,Status,Quantity,ImageFile")] CourseModelForEdit course)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (course.ImageFile == null)
                    {
                        course.Thumbnail = Session[Common.CommonConstants.TEMP_COURSE_IMAGE].ToString();
                    }
                    else
                    {
                        string fileName = Path.GetFileNameWithoutExtension(course.ImageFile.FileName) + DateTime.Now.ToString("yymmssfff") + Path.GetExtension(course.ImageFile.FileName);

                        course.Thumbnail = "~/public/uploadedFiles/coursesPictures/" + fileName;

                        string uploadFolderPath = Server.MapPath("~/public/uploadedFiles/coursesPictures/");

                        if (Directory.Exists(uploadFolderPath) == false)
                        {
                            Directory.CreateDirectory(uploadFolderPath);
                        }
                        fileName = Path.Combine(uploadFolderPath, fileName);
                        try
                        {
                            System.IO.File.Delete(Server.MapPath(Session[Common.CommonConstants.TEMP_COURSE_IMAGE].ToString()));
                        }
                        catch (Exception)
                        {
                        }
                        course.ImageFile.SaveAs(fileName);

                    }
                    var courseEntity = db.Courses.Find(course.CourseID);
                    courseEntity.CourseID = course.CourseID;
                    courseEntity.CourseName = course.CourseName;
                    courseEntity.Username = course.Username;
                    courseEntity.CategoryID = course.CategoryID;
                    courseEntity.OldPrice = course.OldPrice;
                    courseEntity.Price = course.Price;
                    courseEntity.ShortDescription = course.ShortDescription;
                    courseEntity.Description = course.Description;
                    courseEntity.Thumbnail = course.Thumbnail;
                    courseEntity.Status = course.Status;
                    courseEntity.Quantity = course.Quantity;


                    if (db.SaveChanges() > 0)
                    {
                        Session.Remove(Common.CommonConstants.TEMP_COURSE_IMAGE);
                        TempData.Add(Common.CommonConstants.SAVE_SUCCESSFULLY, true);
                    }
                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    TempData.Add(Common.CommonConstants.SAVE_FAILED, true);
                    ex.Entries.Single().Reload();
                }

                return RedirectToAction("Index");
            }
            ViewBag.CategoryList = new SelectList(db.Categories, "CategoryID", "CategoryName");
            ViewBag.InstructorList = new SelectList(db.Users.Where(i => i.Role.Equals("2")), "Username", "FirstName");
            ViewBag.StatusList = new SelectList(new List<SelectListItem> { new SelectListItem { Text = "Open Registration", Value = "1" }, new SelectListItem { Text = "Stop Registration", Value = "0" }, }, "Value", "Text");
            return View(course);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public JsonResult ActiveStatus(int id)
        {

            var course = db.Courses.Find(id);
            course.Status = 1;
            db.SaveChanges();
            return Json(new
            {
                status = true
            });
        }

        public JsonResult DisableStatus(int id)
        {
            var course = db.Courses.Find(id);
            course.Status = 0;
            db.SaveChanges();
            return Json(new
            {
                status = true
            });
        }

        public ActionResult AddLecture(int? id)
        {
            var user = (UserModel)Session[Common.CommonConstants.USER_LOGIN_MODEL];
            if (id == null)
            {
                TempData.Add(Common.CommonConstants.ACCESS_DENIED, true);
                return RedirectToAction("Index", "Home");
            }
            var course = db.Courses.FirstOrDefault(i => i.CourseID == id && user.Username == user.Username);
            if(course == null)
            {
                TempData.Add(Common.CommonConstants.ACCESS_DENIED, true);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Course = course;
                ViewBag.LectureList = db.Lectures.Where(c => c.CourseID == id).ToList();
                return View();
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddLecture([Bind(Include = "LectureID,LectureName,CourseID,Overview,Path,VideoFile")] LectureModelForCreate lecture)
        {
            if (ModelState.IsValid)
            {
                string fileName = Path.GetFileNameWithoutExtension(lecture.VideoFile.FileName) + DateTime.Now.ToString("yymmssfff") + Path.GetExtension(lecture.VideoFile.FileName);
                lecture.Path = "~/public/uploadedFiles/lectureVideos/" + fileName;
                string uploadFolderPath = Server.MapPath("~/public/uploadedFiles/lectureVideos/");
                if (Directory.Exists(uploadFolderPath) == false)
                {
                    Directory.CreateDirectory(uploadFolderPath);
                }
                fileName = Path.Combine(uploadFolderPath, fileName);
                lecture.VideoFile.SaveAs(fileName);
                var lectureEntity = new Lecture(lecture);
                db.Lectures.Add(lectureEntity);
                if (db.SaveChanges() > 0)
                {
                    TempData.Add(Common.CommonConstants.CREATE_SUCCESSFULLY, true);
                }
                ViewBag.Course = db.Courses.Find(lecture.CourseID);
                ViewBag.LectureList = db.Lectures.Where(c => c.CourseID == lecture.CourseID).ToList();
                return RedirectToAction("AddLecture", "Lectures", new { id = lecture.CourseID });
            }

            ViewBag.Course = db.Courses.Find(lecture.CourseID);
            ViewBag.LectureList = db.Lectures.Where(c => c.CourseID == lecture.CourseID).ToList();
            return View(lecture);
        }

        public ActionResult EditLecture(int? id)
        {
            var user = (UserModel)Session[Common.CommonConstants.USER_LOGIN_MODEL];
            if (id == null)
            {
                TempData.Add(Common.CommonConstants.ACCESS_DENIED, true);
                return RedirectToAction("Index", "Home");
            }
            Lecture lecture = db.Lectures.FirstOrDefault(i => i.LectureID == id && i.Cours.Username == user.Username);
            if (lecture == null)
            {
                TempData.Add(Common.CommonConstants.ACCESS_DENIED, true);
                return RedirectToAction("Index", "Home");
            }
            Session.Add(Common.CommonConstants.TEMP_LECTURE_VIDEO, lecture.Path);
            return View(new LectureModelForEdit(lecture));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult EditLecture([Bind(Include = "LectureID,LectureName,CourseID,Overview,Path,VideoFile")] LectureModelForEdit lecture)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (lecture.VideoFile == null)
                    {
                        lecture.Path = Session[Common.CommonConstants.TEMP_LECTURE_VIDEO].ToString();
                    }
                    else
                    {
                        string fileName = Path.GetFileNameWithoutExtension(lecture.VideoFile.FileName) + DateTime.Now.ToString("yymmssfff") + Path.GetExtension(lecture.VideoFile.FileName);
                        lecture.Path = "~/public/uploadedFiles/lectureVideos/" + fileName;
                        string uploadFolderPath = Server.MapPath("~/public/uploadedFiles/lectureVideos/");
                        if (Directory.Exists(uploadFolderPath) == false)
                        {
                            Directory.CreateDirectory(uploadFolderPath);
                        }
                        fileName = Path.Combine(uploadFolderPath, fileName);
                        try
                        {
                            System.IO.File.Delete(Server.MapPath(Session[Common.CommonConstants.TEMP_BLOG_IMAGE].ToString()));
                        }
                        catch (Exception)
                        {
                        }
                        lecture.VideoFile.SaveAs(fileName);
                    }
                    var lectureEntity = db.Lectures.Find(lecture.LectureID);
                    lectureEntity.LectureID = lecture.LectureID;
                    lectureEntity.LectureName = lecture.LectureName;
                    lectureEntity.CourseID = lecture.CourseID;
                    lectureEntity.Overview = lecture.Overview;
                    lectureEntity.Path = lecture.Path;

                    if (db.SaveChanges() > 0)
                    {
                        Session.Remove(Common.CommonConstants.TEMP_LECTURE_VIDEO);
                        TempData.Add(Common.CommonConstants.SAVE_SUCCESSFULLY, true);
                    }
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    TempData.Add(Common.CommonConstants.SAVE_FAILED, true);
                    ex.Entries.Single().Reload();
                }
                return RedirectToAction("AddLecture", "Lectures", new { id = lecture.CourseID });
            }
            ViewBag.Course = db.Courses.Find(lecture.CourseID);
            ViewBag.LectureList = db.Lectures.Where(c => c.CourseID == lecture.CourseID).ToList();
            return View(lecture);
        }

        public ActionResult Approve(int? id)
        {
            if (id == null)
            {
                TempData.Add(Common.CommonConstants.ACCESS_DENIED, true);
                return RedirectToAction("Index", "Home");
            }
            var user = (UserModel)Session[Common.CommonConstants.USER_LOGIN_MODEL];
            var list = db.OrderDetails.Where(i => i.Cours.Username == user.Username && i.CourseID == id);
            return View(list);
        }
        

        public JsonResult TurnedOut(int? id)
        {
            var user = (UserModel)Session[Common.CommonConstants.USER_LOGIN_MODEL];
            if (id == null || user is null)
            {
                return null;
            }
            var order = db.Orders.Find(id);
            order.PaymentStatus = 0;
            db.SaveChanges();
            return Json(new
            {
                status = true
            });
        }
        public JsonResult ApproveStudent(int? id)
        {
            var user = (UserModel)Session[Common.CommonConstants.USER_LOGIN_MODEL];
            if (id == null || user is null)
            {
                return null;
            }
            var order = db.Orders.Find(id);
            order.PaymentStatus = 1;
            db.SaveChanges();
            return Json(new
            {
                status = true
            });
        }
    }
}