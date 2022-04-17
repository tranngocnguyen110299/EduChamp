using Scrypt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TN_academic.Models;
using TN_academic.ViewModels;

namespace TN_academic.Controllers
{
    public class LoginController : Controller
    {
        private TN_academic_DBEntities db = new TN_academic_DBEntities();

        public ActionResult Index()
        {
            if (Session[TN_academic.Common.CommonConstants.USER_LOGIN_MODEL] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public ActionResult Index(UserLoginModel account)
        {
            if (ModelState.IsValid)
            {
                ScryptEncoder encoder = new ScryptEncoder();
                var user = db.Users.SingleOrDefault(model => model.Username == account.Username);
                if (user == null)
                {
                    ViewBag.ErrorLogin = "Username or password is incorrect";
                    return View(account);
                }

                bool isValidPass = encoder.Compare(account.Password, user.Password);

                if (isValidPass)
                {
                    if (user.UserStatu.IsBlock == true)
                    {
                        ViewBag.ErrorLogin = "Your account has been locked.";
                        return View(account);
                    }


                    var us = new UserModel(user);
                    Session.Add(Common.CommonConstants.USER_LOGIN_MODEL, us);
                    TempData.Add(Common.CommonConstants.LOGIN_SUCCESSFULLY, true);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.ErrorLogin = "Username or password is incorrect";
                    return View(account);
                }
            }
            return View();
        }

        public ActionResult Logout()
        {
            Session.Remove(Common.CommonConstants.USER_LOGIN_MODEL);
            return RedirectToAction("Index", "Home");
        }
    }
}