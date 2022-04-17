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
    public class LectureCommentsController : BaseController
    {
        private TN_academic_DBEntities db = new TN_academic_DBEntities();

        public ActionResult Index()
        {
            var lectureComments = db.LectureComments.Include(l => l.Lecture).Include(l => l.User);
            return View(lectureComments.ToList());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LectureComment lectureComment = db.LectureComments.Find(id);
            if (lectureComment == null)
            {
                return HttpNotFound();
            }
            return View(lectureComment);
        }

        public ActionResult ReplyLecture(int? id)
        {
            if (id == null || db.LectureComments.Find(id) == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var lectureComment = db.LectureComments.Find(id);

            ViewBag.ReplyList = db.ReplyingLectureComments.Where(r => r.CommentID == id && r.Status == true).ToList();
            ViewBag.LectureComment = lectureComment;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Reply([Bind(Include = "ReplyID,Username,CommentID,Content,CreatedDate,Status")] ReplyingLectureComment replyingLectureComment)
        {
            if (ModelState.IsValid)
            {
                replyingLectureComment.Username = ((AdminLoginModel)Session[Common.CommonConstants.ADMIN_LOGIN_SESSION]).Username;
                replyingLectureComment.CreatedDate = DateTime.Now;
                replyingLectureComment.Status = true;
                db.ReplyingLectureComments.Add(replyingLectureComment);
                if (db.SaveChanges() > 0)
                {
                    TempData.Add(Common.CommonConstants.CREATE_SUCCESSFULLY, true);
                }
                return RedirectToAction("ReplyLecture", "LectureComments", new { id = replyingLectureComment.CommentID });
            }
           
            return RedirectToAction("ReplyLecture", "LectureComments", new { id = replyingLectureComment.CommentID });
        }


        public ActionResult DeleteReply(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "BlogComments");
            }
            var replyingLectureComment = db.ReplyingLectureComments.Find(id);
            var commentID = replyingLectureComment.CommentID;
            try
            {
                db.ReplyingLectureComments.Remove(replyingLectureComment);
                if (db.SaveChanges() > 0)
                {
                    TempData.Add(Common.CommonConstants.DELETE_SUCCESSFULLY, true);
                }
            }
            catch (Exception)
            {
                TempData.Add(Common.CommonConstants.DELETE_FAILED, true);
            }
            return RedirectToAction("ReplyLecture", "LectureComments", new { id = commentID });
        }


        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LectureComment lectureComment = db.LectureComments.Find(id);
            if (lectureComment == null)
            {
                return HttpNotFound();
            }
            return View(lectureComment);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                LectureComment lectureComment = db.LectureComments.Find(id);
                db.LectureComments.Remove(lectureComment);
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
            var comment = db.LectureComments.Find(id);
            comment.Status = true;
            db.SaveChanges();
            return Json(new
            {
                status = true
            });
        }

        public JsonResult DisableStatus(int id)
        {
            var comment = db.LectureComments.Find(id);
            comment.Status = false;
            db.SaveChanges();
            return Json(new
            {
                status = true
            });
        }
    }
}
