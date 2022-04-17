using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
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
    public class CategoriesController : BaseController
    {
        private TN_academic_DBEntities db = new TN_academic_DBEntities();

        public ActionResult Index()
        {
            return View(db.Categories.ToList());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }


        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CategoryID,CategoryName,Description,Thumbnail,ImageFile")] CategoryModelForCreate category)
        {
            if (ModelState.IsValid)
            {
                string fileName = Path.GetFileNameWithoutExtension(category.ImageFile.FileName) + DateTime.Now.ToString("yymmssfff") + Path.GetExtension(category.ImageFile.FileName);

                category.Thumbnail = "~/public/uploadedFiles/categoryPictures/" + fileName;

                string uploadFolderPath = Server.MapPath("~/public/uploadedFiles/categoryPictures/");

                if (Directory.Exists(uploadFolderPath) == false)
                {
                    Directory.CreateDirectory(uploadFolderPath);
                }

                fileName = Path.Combine(uploadFolderPath, fileName);

                category.ImageFile.SaveAs(fileName);

                var categoryEntity = new Category(category);

                db.Categories.Add(categoryEntity);

                if (db.SaveChanges() > 0)
                {
                    TempData.Add(Common.CommonConstants.CREATE_SUCCESSFULLY, true);
                }

                return RedirectToAction("Index");
            }

            return View(category);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            Session.Add(Common.CommonConstants.TEMP_CATEGORY_IMAGE, category.Thumbnail);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(new CategoryModelForEdit(category));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CategoryID,CategoryName,Description,Thumbnail,ImageFile")] CategoryModelForEdit category)
        {
            if (ModelState.IsValid)
            {
                if (category.ImageFile == null)
                {
                    category.Thumbnail = Session[Common.CommonConstants.TEMP_CATEGORY_IMAGE].ToString();
                }
                else
                {
                    string fileName = Path.GetFileNameWithoutExtension(category.ImageFile.FileName) + DateTime.Now.ToString("yymmssfff") + Path.GetExtension(category.ImageFile.FileName);

                    category.Thumbnail = "~/public/uploadedFiles/categoryPictures/" + fileName;

                    string uploadFolderPath = Server.MapPath("~/public/uploadedFiles/categoryPictures/");

                    if (Directory.Exists(uploadFolderPath) == false)
                    {
                        Directory.CreateDirectory(uploadFolderPath);
                    }
                    fileName = Path.Combine(uploadFolderPath, fileName);
                    try
                    {
                        System.IO.File.Delete(Server.MapPath(Session[Common.CommonConstants.TEMP_CATEGORY_IMAGE].ToString()));
                    }
                    catch (Exception)
                    {
                    }
                    category.ImageFile.SaveAs(fileName);

                }
                var categoryEntity = db.Categories.Find(category.CategoryID);
                categoryEntity.CategoryID = category.CategoryID;
                categoryEntity.CategoryName = category.CategoryName;
                categoryEntity.Description = category.Description;
                categoryEntity.Thumbnail = category.Thumbnail;

                if (db.SaveChanges() > 0)
                {
                    Session.Remove(Common.CommonConstants.TEMP_CATEGORY_IMAGE);
                    TempData.Add(Common.CommonConstants.SAVE_SUCCESSFULLY, true);
                }
                return RedirectToAction("Index");
            }
            return View(category);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Category category = db.Categories.Find(id);
                db.Categories.Remove(category);
                if (db.SaveChanges() > 0)
                {
                    try
                    {
                        System.IO.File.Delete(Server.MapPath(category.Thumbnail));
                    }
                    catch (Exception) { }
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
    }
}
