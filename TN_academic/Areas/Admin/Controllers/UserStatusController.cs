﻿using System;
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
    public class UserStatusController : BaseController
    {
        private TN_academic_DBEntities db = new TN_academic_DBEntities();

        public ActionResult Index()
        {
            return View(db.UserStatus.ToList());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserStatu userStatu = db.UserStatus.Find(id);
            if (userStatu == null)
            {
                return HttpNotFound();
            }
            return View(userStatu);
        }

        public ActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "StatusID,StatusName,IsBlock")] UserStatu userStatu)
        {
            if (ModelState.IsValid)
            {
                db.UserStatus.Add(userStatu);
                if (db.SaveChanges() > 0)
                {
                    TempData.Add(Common.CommonConstants.CREATE_SUCCESSFULLY, true);
                }
                return RedirectToAction("Index");
            }

            return View(userStatu);
        }


        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserStatu userStatu = db.UserStatus.Find(id);
            if (userStatu == null)
            {
                return HttpNotFound();
            }
            return View(userStatu);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "StatusID,StatusName,IsBlock")] UserStatu userStatu)
        {
            if (ModelState.IsValid)
            {
                db.Entry(userStatu).State = EntityState.Modified;
                if (db.SaveChanges() > 0)
                {
                    TempData.Add(Common.CommonConstants.SAVE_SUCCESSFULLY, true);
                }
                return RedirectToAction("Index");
            }
            return View(userStatu);
        }


        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserStatu userStatu = db.UserStatus.Find(id);
            if (userStatu == null)
            {
                return HttpNotFound();
            }
            return View(userStatu);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                UserStatu userStatu = db.UserStatus.Find(id);
                db.UserStatus.Remove(userStatu);
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
