using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TN_academic.Areas.Admin.Models;
using TN_academic.Models;

namespace TN_academic.Areas.Admin.Controllers
{
    [Authorize(Roles = "1")]
    public class CourseCommentsController : BaseController
    {
        private TN_academic_DBEntities db = new TN_academic_DBEntities();

        public ActionResult Index()
        {
            var courseComments = db.CourseComments.Include(c => c.Cours).Include(c => c.User);
            return View(courseComments.ToList());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseComment courseComment = db.CourseComments.Find(id);
            if (courseComment == null)
            {
                return HttpNotFound();
            }
            return View(courseComment);
        }

        public ActionResult ReplyCourse(int? id)
        {
            if (id == null || db.CourseComments.Find(id) == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var courseComment = db.CourseComments.Find(id);

            ViewBag.ReplyList = db.ReplyingCourseComments.Where(r => r.CommentID == id).ToList();
            ViewBag.CourseComment = courseComment;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Reply([Bind(Include = "ReplyID,Username,CommentID,Content,CreatedDate,Status")] ReplyingCourseComment replyingCourseComment)
        {
            if (ModelState.IsValid)
            {
                replyingCourseComment.Username = ((AdminLoginModel)Session[Common.CommonConstants.ADMIN_LOGIN_SESSION]).Username;
                replyingCourseComment.CreatedDate = DateTime.Now;
                replyingCourseComment.Status = 1;
                db.ReplyingCourseComments.Add(replyingCourseComment);
                db.SaveChanges();
                TempData[Common.CommonConstants.CREATE_SUCCESSFULLY] = true;
                return RedirectToAction("ReplyCourse", "CourseComments", new { id = replyingCourseComment.CommentID });
            }

            return RedirectToAction("ReplyCourse", "CourseComments", new { id = replyingCourseComment.CommentID });
        }

        public ActionResult DeleteReply(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "CourseComments");
            }
            var replyingCourseComment = db.ReplyingCourseComments.Find(id);
            var commentID = replyingCourseComment.CommentID;
            try
            {
                db.ReplyingCourseComments.Remove(replyingCourseComment);
                if (db.SaveChanges() > 0)
                {
                    TempData.Add(Common.CommonConstants.DELETE_SUCCESSFULLY, true);
                }
            }
            catch (Exception)
            {
                TempData.Add(Common.CommonConstants.DELETE_FAILED, true);
            }
            return RedirectToAction("ReplyCourse", "CourseComments", new { id = commentID });
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseComment courseComment = db.CourseComments.Find(id);
            if (courseComment == null)
            {
                return HttpNotFound();
            }
            return View(courseComment);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                CourseComment courseComment = db.CourseComments.Find(id);
                db.CourseComments.Remove(courseComment);
                if (db.SaveChanges() > 0)
                {
                    TempData.Add(Common.CommonConstants.DELETE_SUCCESSFULLY, true);
                }
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
            var comment = db.CourseComments.Find(id);
            comment.Status = true;
            db.SaveChanges();
            return Json(new
            {
                status = true
            });
        }

        public JsonResult DisableStatus(int id)
        {
            var comment = db.CourseComments.Find(id);
            comment.Status = false;
            db.SaveChanges();
            return Json(new
            {
                status = true
            });
        }
    }
}
