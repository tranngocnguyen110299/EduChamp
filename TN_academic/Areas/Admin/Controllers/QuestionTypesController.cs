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
    public class QuestionTypesController : BaseController
    {
        private TN_academic_DBEntities db = new TN_academic_DBEntities();

        public ActionResult Index()
        {
            return View(db.QuestionTypes.ToList());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuestionType questionType = db.QuestionTypes.Find(id);
            if (questionType == null)
            {
                return HttpNotFound();
            }
            return View(questionType);
        }

        public ActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TypeID,TypeName,Description")] QuestionType questionType)
        {
            if (ModelState.IsValid)
            {
                db.QuestionTypes.Add(questionType);
                if (db.SaveChanges() > 0)
                {
                    TempData.Add(Common.CommonConstants.CREATE_SUCCESSFULLY, true);
                }
                return RedirectToAction("Index");
            }

            return View(questionType);
        }


        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuestionType questionType = db.QuestionTypes.Find(id);
            if (questionType == null)
            {
                return HttpNotFound();
            }
            return View(questionType);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TypeID,TypeName,Description")] QuestionType questionType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(questionType).State = EntityState.Modified;
                if (db.SaveChanges() > 0)
                {
                    TempData.Add(Common.CommonConstants.SAVE_SUCCESSFULLY, true);
                }
                return RedirectToAction("Index");
            }
            return View(questionType);
        }


        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuestionType questionType = db.QuestionTypes.Find(id);
            if (questionType == null)
            {
                return HttpNotFound();
            }
            return View(questionType);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                QuestionType questionType = db.QuestionTypes.Find(id);
                db.QuestionTypes.Remove(questionType);
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
