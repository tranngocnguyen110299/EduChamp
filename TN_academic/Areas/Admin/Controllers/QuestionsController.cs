using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TN_academic.Areas.Admin.Models;
using TN_academic.Models;

namespace TN_academic.Areas.Admin.Controllers
{
    [Authorize(Roles = "1")]
    public class QuestionsController : BaseController
    {
        private TN_academic_DBEntities db = new TN_academic_DBEntities();

        public ActionResult Index()
        {
            var questions = db.Questions.Include(q => q.Cours).Include(q => q.QuestionType);
            return View(questions.ToList());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Question question = db.Questions.Find(id);
            if (question == null)
            {
                return HttpNotFound();
            }
            return View(question);
        }

        public ActionResult CreateNormalQuestion()
        {
            ViewBag.CourseID = new SelectList(db.Courses, "CourseID", "CourseName");
            ViewBag.Level = new SelectList(new List<SelectListItem> { new SelectListItem { Text = "Easy", Value = "-1" }, new SelectListItem { Text = "Normal", Value = "0" }, new SelectListItem { Text = "Difficult", Value = "1" }, new SelectListItem { Text = "Extremely difficult", Value = "2" } }, "Value", "Text");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult CreateNormalQuestion([Bind(Include = "QuestionID,TypeID,CourseID,Content,Description,ChoiceA,ChoiceB,ChoiceC,ChoiceD,Answer,Level")] NormalQuestion question)
        {
            if (ModelState.IsValid)
            {
                var questionEntity = new Question(question);
                questionEntity.TypeID = 1;
                db.Questions.Add(questionEntity);
                if (db.SaveChanges() > 0)
                {
                    TempData.Add(Common.CommonConstants.CREATE_SUCCESSFULLY, true);
                }
                return RedirectToAction("Index");
            }

            ViewBag.CourseID = new SelectList(db.Courses, "CourseID", "CourseName");
            ViewBag.Level = new SelectList(new List<SelectListItem> { new SelectListItem { Text = "Easy", Value = "-1" }, new SelectListItem { Text = "Normal", Value = "0" }, new SelectListItem { Text = "Difficult", Value = "1" }, new SelectListItem { Text = "Extremely difficult", Value = "2" } }, "Value", "Text");
            return View(question);
        }

        public ActionResult EditNormalQuestion(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Question question = db.Questions.Find(id);
            if (question == null)
            {
                return HttpNotFound();
            }
            ViewBag.CourseList = new SelectList(db.Courses, "CourseID", "CourseName");
            ViewBag.LevelList = new SelectList(new List<SelectListItem> { new SelectListItem { Text = "Easy", Value = "-1" }, new SelectListItem { Text = "Normal", Value = "0" }, new SelectListItem { Text = "Difficult", Value = "1" }, new SelectListItem { Text = "Extremely difficult", Value = "2" } }, "Value", "Text");
            return View(new NormalQuestion(question));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult EditNormalQuestion([Bind(Include = "QuestionID,TypeID,CourseID,Content,Description,ChoiceA,ChoiceB,ChoiceC,ChoiceD,Answer,Level")] NormalQuestion question)
        {
            if (ModelState.IsValid)
            {
                var questionEntity = db.Questions.Find(question.QuestionID);
                questionEntity.QuestionID = question.QuestionID;
                questionEntity.CourseID = question.CourseID;
                questionEntity.Description = question.Description;
                questionEntity.Content = question.Content;
                questionEntity.ChoiceA = question.ChoiceA;
                questionEntity.ChoiceB = question.ChoiceB;
                questionEntity.ChoiceC = question.ChoiceC;
                questionEntity.ChoiceD = question.ChoiceD;
                questionEntity.Answer = question.Answer;
                questionEntity.Level = question.Level;
                if (db.SaveChanges() > 0)
                {
                    TempData.Add(Common.CommonConstants.SAVE_SUCCESSFULLY, true);
                }
                return RedirectToAction("Index");
            }
            ViewBag.CourseList = new SelectList(db.Courses, "CourseID", "CourseName");
            ViewBag.LevelList = new SelectList(new List<SelectListItem> { new SelectListItem { Text = "Easy", Value = "-1" }, new SelectListItem { Text = "Normal", Value = "0" }, new SelectListItem { Text = "Difficult", Value = "1" }, new SelectListItem { Text = "Extremely difficult", Value = "2" } }, "Value", "Text");
            return View(question);
        }

        public ActionResult CreateImageQuestion()
        {
            ViewBag.CourseID = new SelectList(db.Courses, "CourseID", "CourseName");
            ViewBag.Level = new SelectList(new List<SelectListItem> { new SelectListItem { Text = "Easy", Value = "-1" }, new SelectListItem { Text = "Normal", Value = "0" }, new SelectListItem { Text = "Difficult", Value = "1" }, new SelectListItem { Text = "Extremely difficult", Value = "2" } }, "Value", "Text");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateImageQuestion([Bind(Include = "QuestionID,TypeID,CourseID,Content,Description,ChoiceA,ChoiceB,ChoiceC,ChoiceD,Answer,Level,MediaFile")] ImageQuestionModelForCreate question)
        {
            if (ModelState.IsValid)
            {
                string fileName = Path.GetFileNameWithoutExtension(question.MediaFile.FileName) + DateTime.Now.ToString("yymmssfff") + Path.GetExtension(question.MediaFile.FileName);

                question.Content = "~/public/uploadedFiles/questionPictures/" + fileName;

                string uploadFolderPath = Server.MapPath("~/public/uploadedFiles/questionPictures/");

                if (Directory.Exists(uploadFolderPath) == false)
                {
                    Directory.CreateDirectory(uploadFolderPath);
                }

                fileName = Path.Combine(uploadFolderPath, fileName);

                question.MediaFile.SaveAs(fileName);

                var questionEntity = new Question(question);
                questionEntity.TypeID = 3;
                db.Questions.Add(questionEntity);
                if (db.SaveChanges() > 0)
                {
                    TempData.Add(Common.CommonConstants.CREATE_SUCCESSFULLY, true);
                }
                return RedirectToAction("Index");
            }

            ViewBag.CourseID = new SelectList(db.Courses, "CourseID", "CourseName");
            ViewBag.Level = new SelectList(new List<SelectListItem> { new SelectListItem { Text = "Easy", Value = "-1" }, new SelectListItem { Text = "Normal", Value = "0" }, new SelectListItem { Text = "Difficult", Value = "1" }, new SelectListItem { Text = "Extremely difficult", Value = "2" } }, "Value", "Text");
            return View(question);
        }

        public ActionResult EditImageQuestion(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Question question = db.Questions.Find(id);
            Session.Add(Common.CommonConstants.TEMP_QUESTION_IMAGE, question.Content);
            if (question == null)
            {
                return HttpNotFound();
            }
            ViewBag.CourseList = new SelectList(db.Courses, "CourseID", "CourseName");
            ViewBag.LevelList = new SelectList(new List<SelectListItem> { new SelectListItem { Text = "Easy", Value = "-1" }, new SelectListItem { Text = "Normal", Value = "0" }, new SelectListItem { Text = "Difficult", Value = "1" }, new SelectListItem { Text = "Extremely difficult", Value = "2" } }, "Value", "Text");
            return View(new ImageQuestionModelForEdit(question));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditImageQuestion([Bind(Include = "QuestionID,TypeID,CourseID,Content,Description,ChoiceA,ChoiceB,ChoiceC,ChoiceD,Answer,Level,MediaFile")] ImageQuestionModelForEdit question)
        {
            if (ModelState.IsValid)
            {
                if (question.MediaFile == null)
                {
                    question.Content = Session[Common.CommonConstants.TEMP_QUESTION_IMAGE].ToString();
                }
                else
                {
                    string fileName = Path.GetFileNameWithoutExtension(question.MediaFile.FileName) + DateTime.Now.ToString("yymmssfff") + Path.GetExtension(question.MediaFile.FileName);

                    question.Content = "~/public/uploadedFiles/questionPictures/" + fileName;

                    string uploadFolderPath = Server.MapPath("~/public/uploadedFiles/questionPictures/");

                    if (Directory.Exists(uploadFolderPath) == false)
                    {
                        Directory.CreateDirectory(uploadFolderPath);
                    }
                    fileName = Path.Combine(uploadFolderPath, fileName);
                    try
                    {
                        System.IO.File.Delete(Server.MapPath(Session[Common.CommonConstants.TEMP_QUESTION_IMAGE].ToString()));
                    }
                    catch (Exception)
                    {
                    }
                    question.MediaFile.SaveAs(fileName);

                }

                var questionEntity = db.Questions.Find(question.QuestionID);
                questionEntity.QuestionID = question.QuestionID;
                questionEntity.CourseID = question.CourseID;
                questionEntity.Description = question.Description;
                questionEntity.Content = question.Content;
                questionEntity.ChoiceA = question.ChoiceA;
                questionEntity.ChoiceB = question.ChoiceB;
                questionEntity.ChoiceC = question.ChoiceC;
                questionEntity.ChoiceD = question.ChoiceD;
                questionEntity.Answer = question.Answer;
                questionEntity.Level = question.Level;

                if (db.SaveChanges() > 0)
                {
                    Session.Remove(Common.CommonConstants.TEMP_QUESTION_IMAGE);
                    TempData.Add(Common.CommonConstants.SAVE_SUCCESSFULLY, true);
                }
                return RedirectToAction("Index");

            }
            ViewBag.CourseList = new SelectList(db.Courses, "CourseID", "CourseName");
            ViewBag.LevelList = new SelectList(new List<SelectListItem> { new SelectListItem { Text = "Easy", Value = "-1" }, new SelectListItem { Text = "Normal", Value = "0" }, new SelectListItem { Text = "Difficult", Value = "1" }, new SelectListItem { Text = "Extremely difficult", Value = "2" } }, "Value", "Text");
            return View(question);
        }

        public ActionResult CreateListeningQuestion()
        {
            ViewBag.CourseID = new SelectList(db.Courses, "CourseID", "CourseName");
            ViewBag.Level = new SelectList(new List<SelectListItem> { new SelectListItem { Text = "Easy", Value = "-1" }, new SelectListItem { Text = "Normal", Value = "0" }, new SelectListItem { Text = "Difficult", Value = "1" }, new SelectListItem { Text = "Extremely difficult", Value = "2" } }, "Value", "Text");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateListeningQuestion([Bind(Include = "QuestionID,TypeID,CourseID,Content,Description,ChoiceA,ChoiceB,ChoiceC,ChoiceD,Answer,Level,MediaFile")] ListeningQuestionModelForCreate question)
        {
            if (ModelState.IsValid)
            {
                string fileName = Path.GetFileNameWithoutExtension(question.MediaFile.FileName) + DateTime.Now.ToString("yymmssfff") + Path.GetExtension(question.MediaFile.FileName);

                question.Content = "~/public/uploadedFiles/questionAudio/" + fileName;

                string uploadFolderPath = Server.MapPath("~/public/uploadedFiles/questionAudio/");

                if (Directory.Exists(uploadFolderPath) == false)
                {
                    Directory.CreateDirectory(uploadFolderPath);
                }

                fileName = Path.Combine(uploadFolderPath, fileName);

                question.MediaFile.SaveAs(fileName);

                var questionEntity = new Question(question);
                questionEntity.TypeID = 2;
                db.Questions.Add(questionEntity);
                if (db.SaveChanges() > 0)
                {
                    TempData.Add(Common.CommonConstants.CREATE_SUCCESSFULLY, true);
                }
                return RedirectToAction("Index");
            }

            ViewBag.CourseID = new SelectList(db.Courses, "CourseID", "CourseName");
            ViewBag.Level = new SelectList(new List<SelectListItem> { new SelectListItem { Text = "Easy", Value = "-1" }, new SelectListItem { Text = "Normal", Value = "0" }, new SelectListItem { Text = "Difficult", Value = "1" }, new SelectListItem { Text = "Extremely difficult", Value = "2" } }, "Value", "Text");
            return View(question);
        }

        public ActionResult EditListeningQuestion(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Question question = db.Questions.Find(id);
            Session.Add(Common.CommonConstants.TEMP_QUESTION_AUDIO, question.Content);
            if (question == null)
            {
                return HttpNotFound();
            }
            ViewBag.CourseList = new SelectList(db.Courses, "CourseID", "CourseName");
            ViewBag.LevelList = new SelectList(new List<SelectListItem> { new SelectListItem { Text = "Easy", Value = "-1" }, new SelectListItem { Text = "Normal", Value = "0" }, new SelectListItem { Text = "Difficult", Value = "1" }, new SelectListItem { Text = "Extremely difficult", Value = "2" } }, "Value", "Text");
            return View(new ListeningQuestionModelForEdit(question));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditListeningQuestion([Bind(Include = "QuestionID,TypeID,CourseID,Content,Description,ChoiceA,ChoiceB,ChoiceC,ChoiceD,Answer,Level,MediaFile")] ListeningQuestionModelForEdit question)
        {
            if (ModelState.IsValid)
            {
                if (question.MediaFile == null)
                {
                    question.Content = Session[Common.CommonConstants.TEMP_QUESTION_AUDIO].ToString();
                }
                else
                {
                    string fileName = Path.GetFileNameWithoutExtension(question.MediaFile.FileName) + DateTime.Now.ToString("yymmssfff") + Path.GetExtension(question.MediaFile.FileName);

                    question.Content = "~/public/uploadedFiles/questionAudio/" + fileName;

                    string uploadFolderPath = Server.MapPath("~/public/uploadedFiles/questionAudio/");

                    if (Directory.Exists(uploadFolderPath) == false)
                    {
                        Directory.CreateDirectory(uploadFolderPath);
                    }
                    fileName = Path.Combine(uploadFolderPath, fileName);
                    try
                    {
                        System.IO.File.Delete(Server.MapPath(Session[Common.CommonConstants.TEMP_QUESTION_AUDIO].ToString()));
                    }
                    catch (Exception)
                    {
                    }
                    question.MediaFile.SaveAs(fileName);

                }

                var questionEntity = db.Questions.Find(question.QuestionID);
                questionEntity.QuestionID = question.QuestionID;
                questionEntity.CourseID = question.CourseID;
                questionEntity.Description = question.Description;
                questionEntity.Content = question.Content;
                questionEntity.ChoiceA = question.ChoiceA;
                questionEntity.ChoiceB = question.ChoiceB;
                questionEntity.ChoiceC = question.ChoiceC;
                questionEntity.ChoiceD = question.ChoiceD;
                questionEntity.Answer = question.Answer;
                questionEntity.Level = question.Level;

                if (db.SaveChanges() > 0)
                {
                    Session.Remove(Common.CommonConstants.TEMP_QUESTION_AUDIO);
                    TempData.Add(Common.CommonConstants.SAVE_SUCCESSFULLY, true);
                }
                return RedirectToAction("Index");

            }
            ViewBag.CourseList = new SelectList(db.Courses, "CourseID", "CourseName");
            ViewBag.LevelList = new SelectList(new List<SelectListItem> { new SelectListItem { Text = "Easy", Value = "-1" }, new SelectListItem { Text = "Normal", Value = "0" }, new SelectListItem { Text = "Difficult", Value = "1" }, new SelectListItem { Text = "Extremely difficult", Value = "2" } }, "Value", "Text");
            return View(question);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Question question = db.Questions.Find(id);
            if (question == null)
            {
                return HttpNotFound();
            }
            return View(question);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Question question = db.Questions.Find(id);
            var type = question.TypeID;
            try
            {
                db.Questions.Remove(question);
                if(db.SaveChanges() > 0)
                {
                    if (type == 2 || type == 3)
                    {
                        try
                        {
                            System.IO.File.Delete(Server.MapPath(question.Content));
                        }
                        catch (Exception)
                        {
                        }
                    }
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
