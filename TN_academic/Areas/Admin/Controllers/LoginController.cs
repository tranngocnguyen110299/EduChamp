﻿using Scrypt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TN_academic.Areas.Admin.Models;
using TN_academic.Models;

namespace TN_academic.Areas.Admin.Controllers
{
    public class LoginController : Controller
    {
        private TN_academic_DBEntities db = new TN_academic_DBEntities();

        public ActionResult Index()
        {
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

                    if (!user.Role.Equals("1"))
                    {
                        ViewBag.ErrorLogin = "Username or password is incorrect.";
                        return View(account);
                    }

                    FormsAuthentication.SetAuthCookie(user.Username, false);

                    var admin = new AdminLoginModel(user);

                    Session.Add(Common.CommonConstants.ADMIN_LOGIN_SESSION, admin);
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
            FormsAuthentication.SignOut();
            Session.Remove(Common.CommonConstants.ADMIN_LOGIN_SESSION);
            return Redirect("Index");
        }
    }
}