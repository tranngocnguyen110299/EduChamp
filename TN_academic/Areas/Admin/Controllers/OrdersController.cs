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
    public class OrdersController : Controller
    {
        private TN_academic_DBEntities db = new TN_academic_DBEntities();


        public ActionResult Index()
        {
            var orders = db.Orders.Include(o => o.Coupon).Include(o => o.Payment).Include(o => o.User);
            return View(orders.ToList());
        }


        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var order = db.Orders.Find(id);
            ViewBag.OrderDetail = db.OrderDetails.Where(m=>m.OrderID == id).ToList();
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }


        public ActionResult Create()
        {
            ViewBag.CouponID = new SelectList(db.Coupons, "CouponID", "CouponCode");
            ViewBag.PaymentID = new SelectList(db.Payments, "PaymentID", "PaymentName");
            ViewBag.Username = new SelectList(db.Users, "Username", "FirstName");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OrderID,Username,CreatedDate,PaymentID,PaymentStatus,Total,CouponID,Status")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Orders.Add(order);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CouponID = new SelectList(db.Coupons, "CouponID", "CouponCode", order.CouponID);
            ViewBag.PaymentID = new SelectList(db.Payments, "PaymentID", "PaymentName", order.PaymentID);
            ViewBag.Username = new SelectList(db.Users, "Username", "FirstName", order.Username);
            return View(order);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            ViewBag.CouponID = new SelectList(db.Coupons, "CouponID", "CouponCode", order.CouponID);
            ViewBag.PaymentID = new SelectList(db.Payments, "PaymentID", "PaymentName", order.PaymentID);
            ViewBag.Username = new SelectList(db.Users, "Username", "FirstName", order.Username);
            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OrderID,Username,CreatedDate,PaymentID,PaymentStatus,Total,CouponID,Status")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CouponID = new SelectList(db.Coupons, "CouponID", "CouponCode", order.CouponID);
            ViewBag.PaymentID = new SelectList(db.Payments, "PaymentID", "PaymentName", order.PaymentID);
            ViewBag.Username = new SelectList(db.Users, "Username", "FirstName", order.Username);
            return View(order);
        }


        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Admin/Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Order order = db.Orders.Find(id);
            db.Orders.Remove(order);
            db.SaveChanges();
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
