using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using TN_academic.Models;

namespace TN_academic.Areas.Admin.Controllers
{
    [Authorize(Roles = "1")]
    public class ValuesController : BaseController
    {
        private TN_academic_DBEntities db = new TN_academic_DBEntities();

        public String CheckUsername(String username)
        {
            if (username.Trim().Length < 6 || username.Trim().Length > 50)
            {
                return "The username must be between 6 and 50 characters long.";
            }
            else
            {
                if (db.Users.Find(username) != null)
                    return "The username already exists.";
                else
                    return null;
            }
        }

        public String CheckEmail(String emailAddress)
        {
            if (emailAddress.Trim().Length < 6 || emailAddress.Trim().Length > 60)
            {
                return "The email must be between 6 and 60 characters long.";
            }
            bool isEmail = Regex.IsMatch(emailAddress, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", RegexOptions.IgnoreCase);
            if (isEmail)
            {
                bool isExist = db.Users.ToList().Exists(model => model.Email.Equals(emailAddress, StringComparison.OrdinalIgnoreCase));
                if (isExist)
                {
                    return "The email already exists.";
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return "Email is invalid.";
            }
        }

        public JsonResult ActiveStatus(String username)
        {
            var jsonUsername = new JavaScriptSerializer().Deserialize<string>(username);
            var user = db.Users.Find(jsonUsername);
            user.StatusID = 1;
            db.SaveChanges();
            return Json(new
            {
                status = true
            });
        }

        public JsonResult DisableStatus(String username)
        {
            var jsonUsername = new JavaScriptSerializer().Deserialize<string>(username);
            var user = db.Users.Find(jsonUsername);
            user.StatusID = 2;
            db.SaveChanges();
            return Json(new
            {
                status = true
            });
        }

    }
}