using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TN_academic.Models;
using TN_academic.ViewModels;

namespace TN_academic.Controllers
{
    public class CartController : Controller
    {
        private TN_academic_DBEntities db = new TN_academic_DBEntities();
        // GET: Cart
        public ActionResult Index()
        {
            var cart = Session[Common.CommonConstants.CART_SESSION];
            var list = new List<Cart>();
                
            if (cart != null)
            {
                list = (List<Cart>)cart;
                if (Session[Common.CommonConstants.USER_LOGIN_MODEL] != null)
                {
                    var s = (UserModel)Session[Common.CommonConstants.USER_LOGIN_MODEL];

                    foreach (var item in list.ToList())
                    {
                        var check = db.OrderDetails.Where(m => m.CourseID == item.Course.CourseID && m.Order.Username == s.Username && m.Order.Status == 1).ToList();
                        if (check.Count > 0)
                        {
                            var id = db.OrderDetails.FirstOrDefault(m => m.CourseID == item.Course.CourseID && m.Order.Username == s.Username && m.Order.Status == 1);
                            list.RemoveAll(x => x.Course.CourseID == id.CourseID);
                            Session[Common.CommonConstants.CART_SESSION] = list;
                            list = (List<Cart>)Session[Common.CommonConstants.CART_SESSION];
                        }
                    }

                }
            }
            return View(list);
        }

        public ActionResult addItem(int id) {
            var s = (UserModel)Session[Common.CommonConstants.USER_LOGIN_MODEL];
            if (s != null && s.Username != null)
            {
                if (db.OrderDetails.FirstOrDefault(c => c.CourseID == id && c.Order.Username == s.Username) != null)
                {
                    TempData.Add(Common.CommonConstants.REGISTERED_COURSE, true);
                    return RedirectToAction("Index", "Courses");

                }
            }
            
            var course = db.Courses.FirstOrDefault(m => m.CourseID == id);
            var cart = Session[Common.CommonConstants.CART_SESSION];
            if (cart != null)
            {
                if (Session[Common.CommonConstants.USER_LOGIN_MODEL] != null)
                {
                    var check = db.OrderDetails.Where(m => m.CourseID == id && m.Order.Username == s.Username && m.Order.Status == 1).ToList();
                    if (check.Count > 0)
                    {

                        return RedirectToAction("Index", "Cart");

                    }
                    else
                    {
                        var list1 = (List<Cart>)cart;

                        if (list1.Exists(m => m.Course.CourseID == id))
                        {
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            var item1 = new Cart();
                            item1.Course = course;
                            list1.Add(item1);
                        }
                        Session[Common.CommonConstants.CART_SESSION] = list1;
                    }
                }
                else
                {
                    var list = (List<Cart>)cart;

                    if (list.Exists(m => m.Course.CourseID == id))
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        var item = new Cart();
                        item.Course = course;
                        list.Add(item);
                    }
                    Session[Common.CommonConstants.CART_SESSION] = list;
                }
            }
            else
            {
                if (Session[Common.CommonConstants.USER_LOGIN_MODEL] != null)
                {
                    var check = db.OrderDetails.Where(m => m.CourseID == id && m.Order.Username == s.Username && m.Order.Status == 1).ToList();
                    if (check.Count > 0)
                    {
                        return RedirectToAction("Index", "Cart");

                    }
                    else
                    {
                        var list1 = new List<Cart>();
                        var item1 = new Cart();
                        item1.Course = course;
                        list1.Add(item1);

                        Session[Common.CommonConstants.CART_SESSION] = list1;
                    }
                }
                else
                {
                    var list = new List<Cart>();
                    var item = new Cart();
                    item.Course = course;
                    list.Add(item);

                    Session[Common.CommonConstants.CART_SESSION] = list;
                }
            }

            return RedirectToAction("Index");
        }
        public ActionResult Checkout()
        {
            if (Session[Common.CommonConstants.USER_LOGIN_MODEL] == null) {
                return RedirectToAction("Index", "Login");
            }
            var cart = Session[Common.CommonConstants.CART_SESSION];
            var list = new List<Cart>();
            if (cart != null)
            {
                list = (List<Cart>)cart;
            }
            ViewBag.Payment = db.Payments.ToList();
            if (Session[Common.CommonConstants.COUPON] != null) {
                string code = (string)Session[Common.CommonConstants.COUPON];
                ViewBag.Coupon = (double)db.Coupons.FirstOrDefault(m => m.CouponCode == code).Rate;
            }
            else
            {
                ViewBag.Coupon = 0;
            }
            
            return View(list);
        }

        [HttpPost]
        public ActionResult Checkout(int? paymentID) {
            if (paymentID.ToString().Length == 0)
            {
                TempData["ErrorMess"] = "Please choose your form of payment!";
                return RedirectToAction("Checkout");
            }
            if (Session[Common.CommonConstants.USER_LOGIN_MODEL] == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var cart = (List<Cart>)Session[Common.CommonConstants.CART_SESSION];
            var user = (UserModel)Session[Common.CommonConstants.USER_LOGIN_MODEL];
            double totalprice = 0;

            if (user != null)
            {
                int couponID = 1;
                decimal couponRate = 0;
                if (Session[Common.CommonConstants.COUPON] != null)
                {
                    string code = Session[Common.CommonConstants.COUPON].ToString();
                    couponID = db.Coupons.FirstOrDefault(m=>m.CouponCode == code).CouponID;
                    couponRate = (decimal)db.Coupons.FirstOrDefault(m => m.CouponID == couponID).Rate;
                }
        
                foreach (var item in cart)
                {
                    totalprice += (double)item.Course.Price;
                }

                if (Session[Common.CommonConstants.COUPON] != null)
                {
                    totalprice -= (totalprice * ((double)couponRate / 100));

                }


                Order r = new Order();
                r.Username = user.Username;
                r.CreatedDate = DateTime.Now;
                r.PaymentID = (int)paymentID;
                if (r.PaymentID == 1)
                {
                    r.PaymentStatus = 0;
                }
                else
                {
                    r.PaymentStatus = 1;
                }
                           
                r.Total = (decimal)totalprice;
                r.CouponID = couponID;
                r.Status = 1;
                db.Orders.Add(r);
                db.SaveChanges();

                foreach (var item in cart)
                {
                    OrderDetail od = new OrderDetail();
                    od.OrderID = r.OrderID;
                    od.CourseID = item.Course.CourseID;
                    od.UnitPrice = item.Course.Price;
                    db.OrderDetails.Add(od);
                    db.SaveChanges();
                }
            }
            else
            {
                return RedirectToAction("Cart");
            }

            Session[Common.CommonConstants.COUPON] = null;
            Session[Common.CommonConstants.TOTALPRICE] = null;
            Session[Common.CommonConstants.CART_SESSION] = null;
            TempData["Success"] = true;
            return RedirectToAction("Success");
        }

        public JsonResult CheckPayment(int? paymentId)
        {
            if (paymentId.ToString().Length == 0)
            {
                return Json(new
                {
                    status = false,
                    data = "Please select a payment!"

                });
            }
            else
            {
                return Json(new
                {
                    status = true

                });
            }
        }


        public ActionResult Success()
        {
            if (Session[TN_academic.Common.CommonConstants.USER_LOGIN_MODEL] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public JsonResult Later(int id) {
            var sessionCart = (List<Cart>)Session[Common.CommonConstants.CART_SESSION];
            sessionCart.RemoveAll(x => x.Course.CourseID == id);
            Session[Common.CommonConstants.CART_SESSION] = sessionCart;

            var course = db.Courses.FirstOrDefault(m => m.CourseID == id);
            var cart = Session[Common.CommonConstants.LATER_CART_SESSION];
            if (cart != null)
            {
                var list = (List<Cart>)cart;

                if (list.Exists(m => m.Course.CourseID == id))
                {
                    return Json(new
                    {
                        status = true
                    });
                }
                else
                {
                    var item = new Cart();
                    item.Course = course;
                    list.Add(item);
                }
                Session[Common.CommonConstants.LATER_CART_SESSION] = list;
            }
            else
            {
                var list = new List<Cart>();
                var item = new Cart();
                item.Course = course;
                list.Add(item);

                Session[Common.CommonConstants.LATER_CART_SESSION] = list;
            }
            return Json(new
            {
                status = true
            });
        }

        public JsonResult Delete(int id)
        {
            var sessionCart = (List<Cart>)Session[Common.CommonConstants.CART_SESSION];
            sessionCart.RemoveAll(x => x.Course.CourseID == id);
            Session[Common.CommonConstants.CART_SESSION] = sessionCart;
            return Json(new
            {
                status = true
            });
        }

        public JsonResult Coupon(String id)
        {
            Session[Common.CommonConstants.COUPON] = null;
            Session[Common.CommonConstants.TOTALPRICE] = null;
            var check = db.Coupons.FirstOrDefault(m => m.CouponCode == id);
            var list = (List<Cart>)Session[Common.CommonConstants.CART_SESSION];
            double totalprice = 0;
            foreach (var item in list)
            {
                totalprice += (double)item.Course.Price;
            }
            if (check != null)
            {
                Session.Add(Common.CommonConstants.COUPON, check.CouponCode);
                double discount = 0;
                foreach (var item in list)
                {
                    discount += (double)item.Course.Price;
                }
                discount -= (discount * ((double)check.Rate / 100));
                //Session.Add(Common.CommonConstants.TOTALPRICE, discount);
                return Json(new
                {
                    status = true,
                    notice = "Coupon has been applied",
                    total = discount
                });
            }
            else
            {
                Session.Add(Common.CommonConstants.TOTALPRICE, (decimal)totalprice);
                return Json(new
                {
                    status = false,
                    notice = "Coupon code is not applicable because it does not exist!",
                    total = totalprice
                });
            }
        }
    }
}