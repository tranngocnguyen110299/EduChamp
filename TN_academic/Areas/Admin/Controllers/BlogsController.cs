﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using TN_academic.Areas.Admin.Models;
using TN_academic.Models;
using TN_academic.Providers;

namespace TN_academic.Areas.Admin.Controllers
{
    [Authorize(Roles = "1")]
    public class BlogsController : BaseController
    {
        private TN_academic_DBEntities db = new TN_academic_DBEntities();


        public ActionResult Index()
        {
            var list = db.Blogs.Include(b => b.BlogCategory).Include(b => b.User).Where(b => b.Status != -1 && b.Status != -2).ToList();
            return View(list);
        }


        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Blog blog = db.Blogs.Find(id);
            BlogCategory blogCategories = db.BlogCategories.Find(blog.CategoryID);
            ViewBag.BlogCategory = blogCategories.CategoryName;
            if (blog == null)
            {
                return HttpNotFound();
            }
            return View(blog);
        }


        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(db.BlogCategories, "CategoryID", "CategoryName");
            ViewBag.Username = new SelectList(db.Users, "Username", "FirstName");
            ViewBag.StatusList = new SelectList(new List<SelectListItem>{ new SelectListItem {Text = "Show", Value = "1"}, new SelectListItem {Text = "Hide", Value = "0"},}, "Value", "Text");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create([Bind(Include = "BlogID,BlogName,Username,CategoryID,Content,Thumbnail,CreatedDate,Status,ImageFile,ShortDescription")] BlogModelForCreate blog)
        {
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
                var user = (AdminLoginModel)Session[Common.CommonConstants.ADMIN_LOGIN_SESSION];
                blog.Username = user.Username;

                var blogEntity = new Blog(blog);

                db.Blogs.Add(blogEntity);

                if (db.SaveChanges() > 0)
                {
                    TempData.Add(Common.CommonConstants.CREATE_SUCCESSFULLY, true);
                }
                    
                return RedirectToAction("Index");
            }

            ViewBag.CategoryID = new SelectList(db.BlogCategories, "CategoryID", "CategoryName", blog.CategoryID);
            ViewBag.Username = new SelectList(db.Users, "Username", "FirstName", blog.Username);
            ViewBag.StatusList = new SelectList(new List<SelectListItem> { new SelectListItem { Text = "Show", Value = "1" }, new SelectListItem { Text = "Hide", Value = "0" }, }, "Value", "Text");
            return View(blog);
        }


        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Blog blog = db.Blogs.Find(id);
            if (blog == null)
            {
                return HttpNotFound();
            }
            Session.Add(Common.CommonConstants.TEMP_BLOG_IMAGE, blog.Thumbnail);
            ViewBag.CategoryID = new SelectList(db.BlogCategories, "CategoryID", "CategoryName", blog.CategoryID);
            ViewBag.Username = new SelectList(db.Users, "Username", "FirstName", blog.Username);
            ViewBag.StatusList = new SelectList(new List<SelectListItem> { new SelectListItem { Text = "Show", Value = "1" }, new SelectListItem { Text = "Hide", Value = "0" }, }, "Value", "Text");
            return View(new BlogModelForEdit(blog));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit([Bind(Include = "BlogID,BlogName,Username,CategoryID,Content,Thumbnail,CreatedDate,Status,ImageFile,ShortDescription")] BlogModelForEdit blog)
        {
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
                blogEntity.CreatedDate = blog.CreatedDate;
                blogEntity.Status = blog.Status;
                blogEntity.ShortDescription = blog.ShortDescription;

                if (db.SaveChanges() > 0)
                {
                    Session.Remove(Common.CommonConstants.TEMP_BLOG_IMAGE);
                    TempData.Add(Common.CommonConstants.SAVE_SUCCESSFULLY, true);
                }
                return RedirectToAction("Index");
            }
            ViewBag.CategoryID = new SelectList(db.BlogCategories, "CategoryID", "CategoryName", blog.CategoryID);
            ViewBag.Username = new SelectList(db.Users, "Username", "FirstName", blog.Username);
            ViewBag.StatusList = new SelectList(new List<SelectListItem> { new SelectListItem { Text = "Show", Value = "1" }, new SelectListItem { Text = "Hide", Value = "0" }, }, "Value", "Text");
            return View(blog);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Blog blog = db.Blogs.Find(id);
            BlogCategory blogCategories = db.BlogCategories.Find(blog.CategoryID);
            ViewBag.BlogCategory = blogCategories.CategoryName;
            if (blog == null)
            {
                return HttpNotFound();
            }
            return View(blog);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Blog blog = db.Blogs.Find(id);
                db.Blogs.Remove(blog);
                if (db.SaveChanges() > 0)
                {
                    try
                    {
                        System.IO.File.Delete(Server.MapPath(blog.Thumbnail));
                    }
                    catch (Exception)
                    {
                    }
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
            var blog = db.Blogs.Find(id);
            blog.Status = 1;
            db.SaveChanges();
            return Json(new
            {
                status = true
            }) ;
        }

        public JsonResult DisableStatus(int id)
        {
            var blog = db.Blogs.Find(id);
            blog.Status = 0;
            db.SaveChanges();
            return Json(new
            {
                status = true
            });
        }

        public ActionResult Approve()
        {
            var list = db.Blogs.Include(b => b.BlogCategory).Include(b => b.User).Where(b => b.Status == -1).ToList();
            return View(list);
        }

        public JsonResult ApproveBlog(int id)
        {
            var blog = db.Blogs.Find(id);
            blog.Status = 1;
            db.SaveChanges();
            return Json(new
            {
                status = true
            });
        }

        public JsonResult Ignore(int id)
        {
            var blog = db.Blogs.Find(id);
            blog.Status = -2;
            db.SaveChanges();
            return Json(new
            {
                status = true
            });
        }

    }
}
