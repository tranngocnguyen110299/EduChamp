using Scrypt;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TN_academic.Models;
using TN_academic.ViewModels;

namespace TN_academic.Controllers
{
    public class RegisterController : Controller
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

        public ActionResult RegisterInstructor()
        {
            if (Session[TN_academic.Common.CommonConstants.USER_LOGIN_MODEL] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public ActionResult Index([Bind(Include = "Username,FirstName,LastName,Password,Gender,DOB,Phone,Email,Address,Avatar,ImageFile")] UserRegisterModel user)
        {
            if (ModelState.IsValid)
            {
                string fileName = Path.GetFileNameWithoutExtension(user.ImageFile.FileName) + DateTime.Now.ToString("yymmssfff") + Path.GetExtension(user.ImageFile.FileName);

                user.Avatar = "~/public/uploadedFiles/userPictures/" + fileName;

                string uploadFolderPath = Server.MapPath("~/public/uploadedFiles/userPictures/");

                if (Directory.Exists(uploadFolderPath) == false)
                {
                    Directory.CreateDirectory(uploadFolderPath);
                }

                fileName = Path.Combine(uploadFolderPath, fileName);

                user.ImageFile.SaveAs(fileName);

                ScryptEncoder encoder = new ScryptEncoder();
                user.Password = encoder.Encode(user.Password);
                user.Role = "3"; // 3 => student
                user.StatusID = 1;

                var userEntity = new User(user);

                db.Users.Add(userEntity);

                if (db.SaveChanges() > 0)
                    TempData.Add(Common.CommonConstants.REGISTER_SUCCESSFULLY, true);
                return View();
            }

            return View();
        }

        [HttpPost]
        public ActionResult RegisterInstructor([Bind(Include = "Username,FirstName,LastName,Password,Gender,DOB,Phone,Email,Address,Avatar,ImageFile")] UserRegisterModel user)
        {
            if (ModelState.IsValid)
            {
                string fileName = Path.GetFileNameWithoutExtension(user.ImageFile.FileName) + DateTime.Now.ToString("yymmssfff") + Path.GetExtension(user.ImageFile.FileName);

                user.Avatar = "~/public/uploadedFiles/userPictures/" + fileName;

                string uploadFolderPath = Server.MapPath("~/public/uploadedFiles/userPictures/");

                if (Directory.Exists(uploadFolderPath) == false)
                {
                    Directory.CreateDirectory(uploadFolderPath);
                }

                fileName = Path.Combine(uploadFolderPath, fileName);

                user.ImageFile.SaveAs(fileName);

                ScryptEncoder encoder = new ScryptEncoder();
                user.Password = encoder.Encode(user.Password);
                user.Role = "2"; 
                user.StatusID = 1;

                var userEntity = new User(user);

                db.Users.Add(userEntity);

                if (db.SaveChanges() > 0)
                    TempData.Add(Common.CommonConstants.REGISTER_SUCCESSFULLY, true);
                return View();
            }

            return View();
        }
    }
}