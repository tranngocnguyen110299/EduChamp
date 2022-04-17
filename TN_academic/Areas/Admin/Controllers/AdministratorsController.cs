using Scrypt;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using TN_academic.Areas.Admin.Models;
using TN_academic.Models;
using TN_academic.Providers;

namespace TN_academic.Areas.Admin.Controllers
{
    [Authorize(Roles = "1")]
    public class AdministratorsController : BaseController
    {
        private TN_academic_DBEntities db = new TN_academic_DBEntities();

        public ActionResult Index()
        {
            var users = db.Users.Include(u => u.UserStatu).Where(u => u.Role.Equals("1"));
            return View(users.ToList());
        }

        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        public ActionResult Create()
        {
            ViewBag.StatusID = new SelectList(db.UserStatus, "StatusID", "StatusName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Username,FirstName,LastName,Password,Gender,DOB,Phone,Email,Address,Avatar,StatusID,Role,ImageFile")] UserModelForCreate user)
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
                user.Role = "1"; // 1 => admin

                var userEntity = new User(user);

                db.Users.Add(userEntity);

                if (db.SaveChanges() > 0)
                    TempData.Add(Common.CommonConstants.CREATE_SUCCESSFULLY, true);
                return RedirectToAction("Index");
            }

            ViewBag.StatusID = new SelectList(db.UserStatus, "StatusID", "StatusName", user.StatusID);
            return View(user);
        }

        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            Session[Common.CommonConstants.TEMP_ADMIN_IMAGE] = user.Avatar;
            ViewBag.StatusID = new SelectList(db.UserStatus, "StatusID", "StatusName", user.StatusID);
            return View(new UserModelForEdit(user));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Username,FirstName,LastName,Password,Gender,DOB,Phone,Email,Address,Avatar,StatusID,Role,EditedImage")] UserModelForEdit user)
        {
            if (ModelState.IsValid)
            {
                if (user.EditedImage == null)
                {
                    user.Avatar = Session[Common.CommonConstants.TEMP_ADMIN_IMAGE].ToString();
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
                        System.IO.File.Delete(Server.MapPath(Session[Common.CommonConstants.TEMP_ADMIN_IMAGE].ToString()));
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
                userEntity.StatusID = user.StatusID;

                if (db.SaveChanges() > 0)
                {
                    Session.Remove(Common.CommonConstants.TEMP_ADMIN_IMAGE);
                    TempData.Add(Common.CommonConstants.SAVE_SUCCESSFULLY, true);
                }
                return RedirectToAction("Index");
            }
            ViewBag.StatusID = new SelectList(db.UserStatus, "StatusID", "StatusName", user.StatusID);
            return View(user);
        }

        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            try
            {
                User user = db.Users.Find(id);
                db.Users.Remove(user);
                db.SaveChanges();
                try
                {
                    System.IO.File.Delete(Server.MapPath(user.Avatar));
                }
                catch (Exception) { }
                TempData.Add(Common.CommonConstants.DELETE_SUCCESSFULLY, true);
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
