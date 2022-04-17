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
    public class BlogCommentsController : BaseController
    {
        private TN_academic_DBEntities db = new TN_academic_DBEntities();

        public ActionResult Index()
        {
            var blogComments = db.BlogComments.Include(b => b.Blog).Include(b => b.User);
            return View(blogComments.ToList());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogComment blogComment = db.BlogComments.Find(id);
            if (blogComment == null)
            {
                return HttpNotFound();
            }
            return View(blogComment);
        }

        public ActionResult ReplyBlog(int? id)
        {
            if (id == null || db.BlogComments.Find(id) == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var blogComment = db.BlogComments.Find(id);

            ViewBag.ReplyList = db.ReplyingBlogComments.Where(r => r.CommentID == id).ToList();
            ViewBag.BlogComment = blogComment;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Reply([Bind(Include = "ReplyID,Username,CommentID,Content,CreatedDate,Status")] ReplyingBlogComment replyingBlogComment)
        {
            if (ModelState.IsValid)
            {
                replyingBlogComment.Username = ((AdminLoginModel)Session[Common.CommonConstants.ADMIN_LOGIN_SESSION]).Username;
                replyingBlogComment.CreatedDate = DateTime.Now;
                replyingBlogComment.Status = 1;
                db.ReplyingBlogComments.Add(replyingBlogComment);
                db.SaveChanges();
                TempData[Common.CommonConstants.CREATE_SUCCESSFULLY] = true;
                return RedirectToAction("ReplyBlog", "BlogComments", new { id = replyingBlogComment.CommentID });
            }

            return RedirectToAction("ReplyBlog", "BlogComments", new { id = replyingBlogComment.CommentID });
        }

        public ActionResult DeleteReply(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "BlogComments");
            }
            var replyingBlogComment = db.ReplyingBlogComments.Find(id);
            var commentID = replyingBlogComment.CommentID;
            try
            {
                db.ReplyingBlogComments.Remove(replyingBlogComment);
                if (db.SaveChanges() > 0)
                {
                    TempData.Add(Common.CommonConstants.DELETE_SUCCESSFULLY, true);
                }
            }
            catch (Exception)
            {
                TempData.Add(Common.CommonConstants.DELETE_FAILED, true);
            }
            return RedirectToAction("ReplyBlog", "BlogComments", new { id = commentID });
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogComment blogComment = db.BlogComments.Find(id);
            if (blogComment == null)
            {
                return HttpNotFound();
            }
            return View(blogComment);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                BlogComment comment = db.BlogComments.Find(id);
                db.BlogComments.Remove(comment);
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
            var comment = db.BlogComments.Find(id);
            comment.Status = true;
            db.SaveChanges();
            return Json(new
            {
                status = true
            });
        }

        public JsonResult DisableStatus(int id)
        {
            var comment = db.BlogComments.Find(id);
            comment.Status = false;
            db.SaveChanges();
            return Json(new
            {
                status = true
            });
        }
    }
}
