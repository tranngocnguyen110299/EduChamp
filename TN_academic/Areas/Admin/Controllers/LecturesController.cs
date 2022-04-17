using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TN_academic.Areas.Admin.Models;
using TN_academic.Models;
using TN_academic.Providers;

namespace TN_academic.Areas.Admin.Controllers
{
    [Authorize(Roles = "1")]
    public class LecturesController : BaseController
    {
        private TN_academic_DBEntities db = new TN_academic_DBEntities();

        public ActionResult Index(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                ViewBag.Course = db.Courses.Find(id);
                ViewBag.LectureList = db.Lectures.Where(c => c.CourseID == id).ToList();
                return View();
            }
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lecture lecture = db.Lectures.Find(id);
            if (lecture == null)
            {
                return HttpNotFound();
            }
            return View(lecture);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult CreateLecture([Bind(Include = "LectureID,LectureName,CourseID,Overview,Path,VideoFile")] LectureModelForCreate lecture)
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
                return RedirectToAction("Index", "Lectures", new { id = lecture.CourseID });
            }

            ViewBag.Course = db.Courses.Find(lecture.CourseID);
            ViewBag.LectureList = db.Lectures.Where(c => c.CourseID == lecture.CourseID).ToList();
            return RedirectToAction("Index", "Lectures",  new { id = lecture.CourseID });
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lecture lecture = db.Lectures.Find(id);
            Session.Add(Common.CommonConstants.TEMP_LECTURE_VIDEO, lecture.Path);
            if (lecture == null)
            {
                return HttpNotFound();
            }
            return View(new LectureModelForEdit(lecture));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit([Bind(Include = "LectureID,LectureName,CourseID,Overview,Path,VideoFile")] LectureModelForEdit lecture)
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
                return RedirectToAction("Index", "Lectures", new { id = lecture.CourseID });
            }
            ViewBag.Course = db.Courses.Find(lecture.CourseID);
            ViewBag.LectureList = db.Lectures.Where(c => c.CourseID == lecture.CourseID).ToList();
            return View(lecture);
        }

        public ActionResult Delete(int id)
        {
            Lecture lecture = db.Lectures.Find(id);
            int? course = lecture.CourseID;
            try
            {
                db.Lectures.Remove(lecture);
                db.SaveChanges();
                if (lecture.Path != null)
                {
                    System.IO.File.Delete(Server.MapPath(lecture.Path));
                }
                TempData.Add(Common.CommonConstants.DELETE_SUCCESSFULLY, true);
            }
            catch (Exception)
            {
                TempData.Add(Common.CommonConstants.DELETE_FAILED, true);
            }
            return RedirectToAction("Index", "Lectures", new { id = course });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
