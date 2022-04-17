using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using TN_academic.Models;
using TN_academic.ViewModels;

namespace TN_academic.Controllers
{
    public class BlogsController : Controller
    {
        private TN_academic_DBEntities db = new TN_academic_DBEntities();
        public ActionResult Index(int? page)
        {
            if (page == null) page = 1;
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            var list = db.Blogs.Where(m => m.Status == 1).OrderByDescending(m=>m.CreatedDate).ToList();
            var model = list.ToPagedList(pageNumber, pageSize);
            return View(model);
        }

        public ActionResult Category(int? id, int? page)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Blogs");
            }
            if (page == null) page = 1;
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            var list = db.Blogs.Where(m => m.Status == 1 && m.CategoryID == id).OrderByDescending(m => m.CreatedDate).ToList();
            ViewBag.Child = db.BlogCategories.Find(id).CategoryName;
            var model = list.ToPagedList(pageNumber, pageSize);
            return View(model);
        }

        [HttpPost]
        public ActionResult Search(string txtSearch, int? page)
        {
            if (page == null) page = 1;
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            if (txtSearch != null)
            {
                var list = db.Blogs.Where(m => m.BlogName.Contains(txtSearch) && m.Status == 1).ToList();
                var model = list.ToPagedList(pageNumber, pageSize);
                return View(model);
            }
            return View();
        }

        public ActionResult Details(int? id, int? page)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Blogs");
            }
            if (page == null) page = 1;
            int pageSize = 8;
            int pageNumber = (page ?? 1);
            var blogComments = db.BlogComments.Where(m=>m.BlogID == id && m.Status == true).OrderByDescending(m => m.CreatedDate).ToList();
            var model = blogComments.ToPagedList(pageNumber, pageSize);
            ViewBag.BlogDetail = db.Blogs.FirstOrDefault(m => m.BlogID == id && m.Status == 1);
            return View(model);
        }

        [HttpPost]
        public ActionResult Reply(int? commentID, string content, int blogID)
        {
            var user = (UserModel)Session[Common.CommonConstants.USER_LOGIN_MODEL];
            if (user is null)
            {
                TempData.Add(Common.CommonConstants.LOGIN_TO_COMMENT, true);
                return RedirectToAction("Index", "Blogs");
            }
            if (commentID == null)
            {
                TempData.Add(Common.CommonConstants.INVALID_COMMENT, true);
                return RedirectToAction("Index", "Blogs");
            }
            if (content.Length == 0)
            {
                TempData.Add(Common.CommonConstants.INVALID_BLOG_COMMENT, true);
                return RedirectToAction("Details", "Blogs", new { id = blogID });
            }
            var rep = new ReplyingBlogComment();

            rep.Username = user.Username;
            rep.CommentID = commentID;
            rep.Content = content;
            rep.CreatedDate = DateTime.Now;
            rep.Status = 1;
            bool isValid = false;
            try
            {
                db.ReplyingBlogComments.Add(rep);
                if (db.SaveChanges() > 0)
                {
                    isValid = true;
                }
            }
            catch (Exception)
            {
                ViewBag.ErrorMessage = "Please complete the form before submitting";
                return RedirectToAction("Details", "Blogs", new { id = blogID });
            }

            if (isValid == true)
            {
                TempData.Add(Common.CommonConstants.CREATE_SUCCESSFULLY, true);
                return RedirectToAction("Details", "Blogs", new { id = blogID });
            }

            return RedirectToAction("Details", "Blogs", new { id = blogID });
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Comment(string comment, int? blogID)
        {
            var user = (UserModel)Session[Common.CommonConstants.USER_LOGIN_MODEL];
            if (user is null)
            {
                TempData.Add(Common.CommonConstants.LOGIN_TO_COMMENT, true);
                return RedirectToAction("Index", "Blogs");
            }
            if (blogID == null)
            {
                TempData.Add(Common.CommonConstants.INVALID_COMMENT, true);
                return RedirectToAction("Index", "Blogs");
            }

            if (comment.Length == 0)
            {
                TempData.Add(Common.CommonConstants.INVALID_BLOG_COMMENT, true);
                return RedirectToAction("Details", "Blogs", new { id = blogID });
            }
            var cmt = new BlogComment();

            cmt.Username = user.Username;
            cmt.Content = comment;
            cmt.BlogID = (int)blogID;
            cmt.CreatedDate = DateTime.Now;
            cmt.Status = true;
            bool isValid = false;
            try
            {
                db.BlogComments.Add(cmt);
                if (db.SaveChanges() > 0)
                {
                    isValid = true;
                }
            }
            catch (Exception)
            {
                ViewBag.ErrorMessage = "Please complete the form before submitting";
                return RedirectToAction("Details", "Blogs", new { id = blogID });
            }

            if (isValid == true)
            {
                TempData.Add(Common.CommonConstants.CREATE_SUCCESSFULLY, true);
                return RedirectToAction("Details", "Blogs", new { id = cmt.BlogID });
            }

            return RedirectToAction("Details", "Blogs", new { id = cmt.BlogID });
        }

        public PartialViewResult _BlogSlideBar()
        {
            ViewBag.RecentPosts = db.Blogs.Where(m => m.Status == 1).OrderByDescending(m => m.CreatedDate).Take(3).ToList();
            return PartialView();
        }

        public ActionResult Create()
        {
            if ((UserModel)Session[Common.CommonConstants.USER_LOGIN_MODEL] == null)
            {
                TempData.Add(Common.CommonConstants.LOGIN_TO_WRITE_BLOG, true);
                return RedirectToAction("Index", "Home");
            }
            ViewBag.BlogCategories = new SelectList(db.BlogCategories, "CategoryID", "CategoryName");
            var user = (UserModel)Session[Common.CommonConstants.USER_LOGIN_MODEL];
            ViewBag.User = user;
            ViewBag.BlogList = db.Blogs.Where(i => i.Username == user.Username).ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create([Bind(Include = "BlogID,BlogName,Username,CategoryID,Content,Thumbnail,CreatedDate,Status,ImageFile,ShortDescription")] ClientBlogModel blog)
        {
            if ((UserModel)Session[Common.CommonConstants.USER_LOGIN_MODEL] == null)
            {
                TempData.Add(Common.CommonConstants.LOGIN_TO_WRITE_BLOG, true);
                return RedirectToAction("Index", "Home");
            }
            if (ModelState.IsValid)
            {
                string fileName = Path.GetFileNameWithoutExtension(blog.ImageFile.FileName) + DateTime.Now.ToString("yymmssfff") + Path.GetExtension(blog.ImageFile.FileName);

                blog.Thumbnail = "~/public/uploadedFiles/blogPictures/" + fileName;

                string uploadFolderPath = Server.MapPath("~/public/uploadedFiles/blogPictures/");

                if (Directory.Exists(uploadFolderPath) == false)
                {
                    Directory.CreateDirectory(uploadFolderPath);
                }

                fileName = Path.Combine(uploadFolderPath, fileName);

                blog.ImageFile.SaveAs(fileName);
                var us = (UserModel)Session[Common.CommonConstants.USER_LOGIN_MODEL];
                blog.Username = us.Username;
                blog.CreatedDate = DateTime.Now;
                blog.Status = -1;

                var blogEntity = new Blog(blog);

                db.Blogs.Add(blogEntity);

                if (db.SaveChanges() > 0)
                {
                    TempData.Add(Common.CommonConstants.CREATE_SUCCESSFULLY, true);
                }

                return RedirectToAction("Create");
            }

            if ((UserModel)Session[Common.CommonConstants.USER_LOGIN_MODEL] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            TempData.Add(Common.CommonConstants.NOTICE_COMPLETED_FORM, true);
            return RedirectToAction("Create", "Blogs");
        }

        public ActionResult Edit(int? id)
        {
            var user = (UserModel)Session[Common.CommonConstants.USER_LOGIN_MODEL];
            if (user == null)
            {
                TempData.Add(Common.CommonConstants.LOGIN_TO_WRITE_BLOG, true);
                return RedirectToAction("Index", "Blogs");
            }
            if (id == null)
            {
                return RedirectToAction("Index", "Blogs");
            }
            Blog blog = db.Blogs.FirstOrDefault(i => i.BlogID == id && i.Username == user.Username);
            if (blog == null)
            {
                TempData.Add(Common.CommonConstants.ACCESS_DENIED, true);
                return RedirectToAction("Index", "Blogs");
            }
            Session.Add(Common.CommonConstants.TEMP_BLOG_IMAGE, blog.Thumbnail);
            ViewBag.User = user;
            ViewBag.BlogCategories = new SelectList(db.BlogCategories, "CategoryID", "CategoryName");
            return View(new ClientBlogModelForEdit(blog));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit([Bind(Include = "BlogID,BlogName,Username,CategoryID,Content,Thumbnail,CreatedDate,Status,ImageFile,ShortDescription")] ClientBlogModelForEdit blog)
        {
            if ((UserModel)Session[Common.CommonConstants.USER_LOGIN_MODEL] == null)
            {
                TempData.Add(Common.CommonConstants.LOGIN_TO_WRITE_BLOG, true);
                return RedirectToAction("Index", "Home");
            }
            if (ModelState.IsValid)
            {
                if (blog.ImageFile == null)
                {
                    blog.Thumbnail = Session[Common.CommonConstants.TEMP_BLOG_IMAGE].ToString();
                }
                else
                {
                    string fileName = Path.GetFileNameWithoutExtension(blog.ImageFile.FileName) + DateTime.Now.ToString("yymmssfff") + Path.GetExtension(blog.ImageFile.FileName);

                    blog.Thumbnail = "~/public/uploadedFiles/blogPictures/" + fileName;

                    string uploadFolderPath = Server.MapPath("~/public/uploadedFiles/blogPictures/");

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
                    blog.ImageFile.SaveAs(fileName);

                }
                var blogEntity = db.Blogs.Find(blog.BlogID);
                blogEntity.BlogID = blog.BlogID;
                blogEntity.BlogName = blog.BlogName;
                blogEntity.CategoryID = blog.CategoryID;
                blogEntity.Content = blog.Content;
                blogEntity.Thumbnail = blog.Thumbnail;
                blogEntity.ShortDescription = blog.ShortDescription;
                blogEntity.Status = -1;

                if (db.SaveChanges() > 0)
                {
                    Session.Remove(Common.CommonConstants.TEMP_BLOG_IMAGE);
                    TempData.Add(Common.CommonConstants.SAVE_SUCCESSFULLY, true);
                }
                return RedirectToAction("Create");
            }

            Session.Add(Common.CommonConstants.TEMP_BLOG_IMAGE, blog.Thumbnail);
            ViewBag.BlogCategories = new SelectList(db.BlogCategories, "CategoryID", "CategoryName");
            var user = (UserModel)Session[Common.CommonConstants.USER_LOGIN_MODEL];
            ViewBag.User = user;
            return View(blog);
        }
    }
}