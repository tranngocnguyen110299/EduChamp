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
    public class AboutUsController : BaseController
    {
        private TN_academic_DBEntities db = new TN_academic_DBEntities();

        public ActionResult Index()
        {
            return View(db.AboutUs.ToList());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AboutU aboutU = db.AboutUs.Find(id);
            if (aboutU == null)
            {
                return HttpNotFound();
            }
            return View(aboutU);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create([Bind(Include = "AboutUsID,Description")] AboutU aboutU)
        {
            if (ModelState.IsValid)
            {
                db.AboutUs.Add(aboutU);
                if (db.SaveChanges() > 0)
                    TempData.Add(Common.CommonConstants.CREATE_SUCCESSFULLY, true);
                return RedirectToAction("Index");
            }

            return View(aboutU);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AboutU aboutU = db.AboutUs.Find(id);
            if (aboutU == null)
            {
                return HttpNotFound();
            }
            return View(aboutU);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit([Bind(Include = "AboutUsID,Description")] AboutU aboutU)
        {
            if (ModelState.IsValid)
            {
                db.Entry(aboutU).State = EntityState.Modified;
                if (db.SaveChanges() > 0)
                    TempData.Add(Common.CommonConstants.SAVE_SUCCESSFULLY, true);
                return RedirectToAction("Index");
            }
            return View(aboutU);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AboutU aboutU = db.AboutUs.Find(id);
            if (aboutU == null)
            {
                return HttpNotFound();
            }
            return View(aboutU);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                AboutU aboutU = db.AboutUs.Find(id);
                db.AboutUs.Remove(aboutU);
                if (db.SaveChanges() > 0)
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
    }
}
