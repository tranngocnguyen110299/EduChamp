using Scrypt;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using TN_academic.Areas.Admin.Models;
using TN_academic.Models;
using TN_academic.ViewModels;

namespace TN_academic.Controllers
{
    public class ProfileController : Controller
    {
        private TN_academic_DBEntities db = new TN_academic_DBEntities();
        public ActionResult Index()
        {
            if ((UserModel)Session[Common.CommonConstants.USER_LOGIN_MODEL] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var user = (UserModel)Session[Common.CommonConstants.USER_LOGIN_MODEL];
            Session.Add(Common.CommonConstants.TEMP_STUDENT_IMAGE, user.Avatar);
            

            var userentity = db.Users.Find(user.Username);
            var model = new UserModelForEdit(userentity);
            var result = db.ExamResults.Where(e => e.Username == user.Username).ToList();
            ViewBag.ResultList = result;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index([Bind(Include = "Username,FirstName,LastName,Gender,DOB,Phone,Email,Address,Avatar,StatusID,Role,EditedImage")] UserModelForEdit user)
        {
            if (ModelState.IsValid)
            {
                if (user.EditedImage == null)
                {
                    user.Avatar = Session[Common.CommonConstants.TEMP_STUDENT_IMAGE].ToString();
                }
                else
                {
                    string fileName = Path.GetFileNameWithoutExtension(user.EditedImage.FileName) + DateTime.Now.ToString("yymmssfff") + Path.GetExtension(user.EditedImage.FileName);

                    user.Avatar = "~/public/uploadedFiles/userPictures/" + fileName;

                    string uploadFolderPath = Server.MapPath("~/public/uploadedFiles/userPictures/");

                    if (Directory.Exists(uploadFolderPath) == false)
                    {
                        Directory.CreateDirectory(uploadFolderPath);
                    }

                    fileName = Path.Combine(uploadFolderPath, fileName);

                    try
                    {
                        System.IO.File.Delete(Server.MapPath(Session[Common.CommonConstants.TEMP_STUDENT_IMAGE].ToString()));
                    }
                    catch (Exception)
                    {
                    }
                    user.EditedImage.SaveAs(fileName);

                }
                var userEntity = db.Users.Find(user.Username);
                userEntity.FirstName = user.FirstName;
                userEntity.LastName = user.LastName;
                userEntity.DOB = user.DOB;
                userEntity.Gender = user.Gender;
                userEntity.Phone = user.Phone;
                userEntity.Email = user.Email;
                userEntity.Address = user.Address;
                userEntity.Avatar = user.Avatar;

                if (db.SaveChanges() > 0)
                {
                    Session.Remove(Common.CommonConstants.TEMP_STUDENT_IMAGE);
                    var userSession = new UserModel(userEntity);
                    Session.Add(Common.CommonConstants.USER_LOGIN_MODEL, userSession);
                    TempData.Add(Common.CommonConstants.SAVE_SUCCESSFULLY, true);
                }
                return RedirectToAction("Index");
            }
            var us = (UserModel)Session[Common.CommonConstants.USER_LOGIN_MODEL];
            Session.Add(Common.CommonConstants.TEMP_STUDENT_IMAGE, us.Avatar);
            return View(user);
        }

        public JsonResult ChangePassword(String currentpass, String newpass)
        {
            var jsonCurrent = new JavaScriptSerializer().Deserialize<string>(currentpass);
            var jsonNew = new JavaScriptSerializer().Deserialize<string>(newpass);

            ScryptEncoder encoder = new ScryptEncoder();
            var userStudent = (UserModel)Session[Common.CommonConstants.USER_LOGIN_MODEL];
            var user = db.Users.SingleOrDefault(model => model.Username == userStudent.Username);

            bool isValidPass = encoder.Compare(jsonCurrent, user.Password);

            if (isValidPass)
            {
                user.Password = encoder.Encode(jsonNew);
                db.SaveChanges();
                return Json(new
                {
                    status = true
                });
            }
            else
            {
                return Json(new
                {
                    status = false
                });
            }
        }

    }
}