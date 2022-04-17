using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TN_academic.Models;

namespace TN_academic.Areas.Admin.Controllers
{
    [Authorize(Roles = "1")]
    public class BlogCategoriesController : BaseController
    {
        private TN_academic_DBEntities db = new TN_academic_DBEntities();

        public ActionResult Index()
        {
            return View(db.BlogCategories.ToList());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogCategory blogCategory = db.BlogCategories.Find(id);
            if (blogCategory == null)
            {
                return HttpNotFound();
            }
            return View(blogCategory);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CategoryID,CategoryName")] BlogCategory blogCategory)
        {
            if (ModelState.IsValid)
            {
                db.BlogCategories.Add(blogCategory);
                if (db.SaveChanges() > 0)
                {
                    TempData.Add(Common.CommonConstants.CREATE_SUCCESSFULLY, true);
                }
                return RedirectToAction("Index");
            }

            return View(blogCategory);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogCategory blogCategory = db.BlogCategories.Find(id);
            if (blogCategory == null)
            {
                return HttpNotFound();
            }
            return View(blogCategory);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CategoryID,CategoryName")] BlogCategory blogCategory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(blogCategory).State = EntityState.Modified;
                if (db.SaveChanges() > 0)
                {
                    TempData.Add(Common.CommonConstants.SAVE_SUCCESSFULLY, true);
                }
                
                return RedirectToAction("Index");
            }
            return View(blogCategory);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogCategory blogCategory = db.BlogCategories.Find(id);
            if (blogCategory == null)
            {
                return HttpNotFound();
            }
            return View(blogCategory);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                BlogCategory blogCategory = db.BlogCategories.Find(id);
                db.BlogCategories.Remove(blogCategory);
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
    }
}
