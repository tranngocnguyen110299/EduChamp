using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
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
    public class CouponsController : BaseController
    {
        private TN_academic_DBEntities db = new TN_academic_DBEntities();

        public ActionResult Index()
        {
            var list = db.Coupons.Where(c => c.CouponID != 1).ToList();
            return View(list);
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Coupon coupon = db.Coupons.Find(id);
            if (coupon == null)
            {
                return HttpNotFound();
            }
            return View(coupon);
        }

        public ActionResult Create()
        {
            ViewBag.StatusList = new SelectList(new List<SelectListItem> { new SelectListItem { Text = "active", Value = "true" }, new SelectListItem { Text = "disable", Value = "false" }, }, "Value", "Text");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create([Bind(Include = "CouponID,CouponCode,CouponName,Description,Thumbnail,Rate,Quantity,Status,ImageFile")] CouponModelForCreate coupon)
        {
            if (ModelState.IsValid)
            {
                string fileName = Path.GetFileNameWithoutExtension(coupon.ImageFile.FileName) + DateTime.Now.ToString("yymmssfff") + Path.GetExtension(coupon.ImageFile.FileName);

                coupon.Thumbnail = "~/public/uploadedFiles/couponPictures/" + fileName;

                string uploadFolderPath = Server.MapPath("~/public/uploadedFiles/couponPictures/");

                if (Directory.Exists(uploadFolderPath) == false)
                {
                    Directory.CreateDirectory(uploadFolderPath);
                }

                fileName = Path.Combine(uploadFolderPath, fileName);

                coupon.ImageFile.SaveAs(fileName);

                var couponEntity = new Coupon(coupon);


                db.Coupons.Add(couponEntity);

                if (db.SaveChanges() > 0)
                {
                    TempData.Add(Common.CommonConstants.CREATE_SUCCESSFULLY, true);
                }

                return RedirectToAction("Index");
            }
            ViewBag.StatusList = new SelectList(new List<SelectListItem> { new SelectListItem { Text = "active", Value = "true" }, new SelectListItem { Text = "disable", Value = "false" }, }, "Value", "Text");
            return View(coupon);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Coupon coupon = db.Coupons.Find(id);
            if (coupon == null)
            {
                return HttpNotFound();
            }
            Session.Add(Common.CommonConstants.TEMP_COUPON_IMAGE, coupon.Thumbnail);
            ViewBag.StatusList = new SelectList(new List<SelectListItem> { new SelectListItem { Text = "active", Value = "true" }, new SelectListItem { Text = "disable", Value = "false" }, }, "Value", "Text");
            return View(new CouponModelForEdit(coupon));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit([Bind(Include = "CouponID,CouponCode,CouponName,Description,Thumbnail,Rate,Quantity,Status,ImageFile")] CouponModelForEdit coupon)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (coupon.ImageFile == null)
                    {
                        coupon.Thumbnail = Session[Common.CommonConstants.TEMP_COUPON_IMAGE].ToString();
                    }
                    else
                    {
                        string fileName = Path.GetFileNameWithoutExtension(coupon.ImageFile.FileName) + DateTime.Now.ToString("yymmssfff") + Path.GetExtension(coupon.ImageFile.FileName);

                        coupon.Thumbnail = "~/public/uploadedFiles/couponPictures/" + fileName;

                        string uploadFolderPath = Server.MapPath("~/public/uploadedFiles/couponPictures/");

                        if (Directory.Exists(uploadFolderPath) == false)
                        {
                            Directory.CreateDirectory(uploadFolderPath);
                        }
                        fileName = Path.Combine(uploadFolderPath, fileName);
                        try
                        {
                            System.IO.File.Delete(Server.MapPath(Session[Common.CommonConstants.TEMP_COUPON_IMAGE].ToString()));
                        }
                        catch (Exception)
                        {
                        }
                        coupon.ImageFile.SaveAs(fileName);

                    }
                    var couponEntity = db.Coupons.Find(coupon.CouponID);
                    couponEntity.CouponID = coupon.CouponID;
                    couponEntity.CouponCode = coupon.CouponCode;
                    couponEntity.CouponName = coupon.CouponName;
                    couponEntity.Description = coupon.Description;
                    couponEntity.Thumbnail = coupon.Thumbnail;
                    couponEntity.Rate = coupon.Rate;
                    couponEntity.Quantity = coupon.Quantity;
                    couponEntity.Status = coupon.Status;

                    if (db.SaveChanges() > 0)
                    {
                        Session.Remove(Common.CommonConstants.TEMP_COUPON_IMAGE);
                        TempData.Add(Common.CommonConstants.SAVE_SUCCESSFULLY, true);
                    }
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    TempData.Add(Common.CommonConstants.SAVE_FAILED, true);
                    ex.Entries.Single().Reload();
                }
                return RedirectToAction("Index");
            }
            ViewBag.StatusList = new SelectList(new List<SelectListItem> { new SelectListItem { Text = "active", Value = "true" }, new SelectListItem { Text = "disable", Value = "false" }, }, "Value", "Text");
            return View(coupon);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Coupon coupon = db.Coupons.Find(id);
            if (coupon == null)
            {
                return HttpNotFound();
            }
            return View(coupon);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Coupon coupon = db.Coupons.Find(id);
                db.Coupons.Remove(coupon);
                if (db.SaveChanges() > 0)
                {
                    try
                    {
                        System.IO.File.Delete(Server.MapPath(coupon.Thumbnail));
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

        public JsonResult ActiveStatus(int id)
        {

            var coupon = db.Coupons.Find(id);
            coupon.Status = true;
            db.SaveChanges();
            return Json(new
            {
                status = true
            });
        }

        public JsonResult DisableStatus(int id)
        {
            var coupon = db.Coupons.Find(id);
            coupon.Status = false;
            db.SaveChanges();
            return Json(new
            {
                status = true
            });
        }

    }
}
