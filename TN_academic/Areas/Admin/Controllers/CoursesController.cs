using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TN_academic.Models;
using TN_academic.Providers;
using TN_academic.Areas.Admin.Models;

namespace TN_academic.Areas.Admin.Controllers
{
    [Authorize(Roles = "1")]
    public class CoursesController : BaseController
    {
        private TN_academic_DBEntities db = new TN_academic_DBEntities();
        
        public ActionResult Index()
        {
            var courses = db.Courses.Include(c => c.Category).Include(c => c.User);
            foreach (var item in courses.ToList())
            {
                if (item.ShortDescription.Length > 122)
                    item.ShortDescription = item.ShortDescription.Substring(0, 122) + "...";
            }
            return View(courses.ToList());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cours cours = db.Courses.Find(id);
            if (cours == null)
            {
                return HttpNotFound();
            }
            return View(cours);
        }

        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName");
            ViewBag.Username = new SelectList(db.Users.Where(u => u.Role.Equals("2")), "Username", "FirstName");
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
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cours cours = db.Courses.Find(id);
            Session.Add(Common.CommonConstants.TEMP_COURSE_IMAGE, cours.Thumbnail);
            if (cours == null)
            {
                return HttpNotFound();
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

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cours cours = db.Courses.Find(id);
            if (cours == null)
            {
                return HttpNotFound();
            }
            return View(cours);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Cours cours = db.Courses.Find(id);
                db.Courses.Remove(cours);
                db.SaveChanges();
                try
                {
                    System.IO.File.Delete(Server.MapPath(cours.Thumbnail));
                }
                catch (Exception) { }
                TempData.Add(Common.CommonConstants.DELETE_SUCCESSFULLY, true);
            }
            catch (Exception)
            {
                TempData.Add(Common.CommonConstants.DELETE_FAILED, true);
            }
            
            return RedirectToAction("Index");
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
    }
}
