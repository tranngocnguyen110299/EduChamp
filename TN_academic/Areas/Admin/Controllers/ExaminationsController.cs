using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TN_academic.Areas.Admin.Models;
using TN_academic.Models;
using TN_academic.Providers;

namespace TN_academic.Areas.Admin.Controllers
{
    [Authorize(Roles = "1")]
    public class ExaminationsController : BaseController
    {
        private TN_academic_DBEntities db = new TN_academic_DBEntities();

        public ActionResult Index()
        {
            var examinations = db.Examinations.Include(e => e.Cours);
            return View(examinations.ToList());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Examination examination = db.Examinations.Find(id);
            if (examination == null)
            {
                return HttpNotFound();
            }
            return View(examination);
        }

        public ActionResult Create()
        {
            ViewBag.CourseID = new SelectList(db.Courses, "CourseID", "CourseName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ExamID,ExamName,CourseID,Time,ExaminationDate,Status")] ExaminationModelForCreate examination)
        {
            if (ModelState.IsValid)
            {
                examination.Status = 0;
                var exam = new Examination(examination);
                db.Examinations.Add(exam);
                if (db.SaveChanges() > 0)
                {
                    TempData.Add(Common.CommonConstants.CREATE_SUCCESSFULLY, true);
                }
                return RedirectToAction("AddQuestions", "Examinations", new { examId = exam.ExamID });
            }

            ViewBag.CourseID = new SelectList(db.Courses, "CourseID", "CourseName", examination.CourseID);
            return View(examination);
        }

        public ActionResult AddQuestions(int? examId)
        {
            var examination = db.Examinations.FirstOrDefault(e => e.ExamID == examId);
            Session.Add(Common.CommonConstants.TEMPORARY_EXAMINATION, examination);
            ViewBag.QuestionList = db.Questions.SqlQuery("select * from Questions where CourseID = "+ examination.CourseID + " AND QuestionID NOT IN (select QuestionID from ExaminationContent where ExamID = "+ examId+ ")").ToList();
            ViewBag.TypeID = new SelectList(db.QuestionTypes, "TypeID", "TypeName");
            ViewBag.Contents = db.ExaminationContents.Where(c => c.ExamID == examId).ToList();
            ViewBag.Level = new SelectList(new List<SelectListItem> { new SelectListItem { Text = "Easy", Value = "-1" }, new SelectListItem { Text = "Normal", Value = "0" }, new SelectListItem { Text = "Difficult", Value = "1" }, new SelectListItem { Text = "Extremely difficult", Value = "2" } }, "Value", "Text");
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddQuestions([Bind(Include = "QuestionID,TypeID,CourseID,Content,Description,ChoiceA,ChoiceB,ChoiceC,ChoiceD,Answer,Level,MediaFile,Mark")] Question question, int examID, int? mark)
        {
            if (question.Content is null && question.TypeID == 1)
            {
                TempData.Add(Common.CommonConstants.INVALID_QUESTION_CONTENT, true);
                return RedirectToAction("AddQuestions", "Examinations", new { examId = examID });
            }
            else if (question.Description is null)
            {
                TempData.Add(Common.CommonConstants.INVALID_QUESTION_DESCRIPTION, true);
                return RedirectToAction("AddQuestions", "Examinations", new { examId = examID });
            }
            else if (question.ChoiceA is null)
            {
                TempData.Add(Common.CommonConstants.INVALID_QUESTION_CHOICEA, true);
                return RedirectToAction("AddQuestions", "Examinations", new { examId = examID });
            }
            else if (question.ChoiceB is null)
            {
                TempData.Add(Common.CommonConstants.INVALID_QUESTION_CHOICEB, true);
                return RedirectToAction("AddQuestions", "Examinations", new { examId = examID });
            }
            else if (question.ChoiceC is null)
            {
                TempData.Add(Common.CommonConstants.INVALID_QUESTION_CHOICEC, true);
                return RedirectToAction("AddQuestions", "Examinations", new { examId = examID });
            }
            else if (question.ChoiceD is null)
            {
                TempData.Add(Common.CommonConstants.INVALID_QUESTION_CHOICED, true);
                return RedirectToAction("AddQuestions", "Examinations", new { examId = examID });
            }
            else if (mark.ToString() is null)
            {
                TempData.Add(Common.CommonConstants.INVALID_QUESTION_MARK, true);
                return RedirectToAction("AddQuestions", "Examinations", new { examId = examID });
            }
            try
            {
                string fileName = "";
                string uploadFolderPath = "";
                switch (question.TypeID)
                {
                    case 1:
                        break;
                    case 2:
                        if (ValidateAudio(question.MediaFile) == -1)
                        {
                            TempData.Add(Common.CommonConstants.INVALID_AUDIO_EXTENSION, true);
                            return RedirectToAction("AddQuestions", "Examinations", new { examId = examID });
                        }
                        if (ValidateAudio(question.MediaFile) == -2)
                        {
                            TempData.Add(Common.CommonConstants.INVALID_AUDIO_FILE_SIZE, true);
                            return RedirectToAction("AddQuestions", "Examinations", new { examId = examID });
                        }
                        fileName = Path.GetFileNameWithoutExtension(question.MediaFile.FileName) + DateTime.Now.ToString("yymmssfff") + Path.GetExtension(question.MediaFile.FileName);
                        question.Content = "~/public/uploadedFiles/questionAudio/" + fileName;
                        uploadFolderPath = Server.MapPath("~/public/uploadedFiles/questionAudio/");
                        if (Directory.Exists(uploadFolderPath) == false)
                        {
                            Directory.CreateDirectory(uploadFolderPath);
                        }
                        fileName = Path.Combine(uploadFolderPath, fileName);
                        question.MediaFile.SaveAs(fileName);
                        break;
                    case 3:
                        if (ValidateImage(question.MediaFile) == -1)
                        {
                            TempData.Add(Common.CommonConstants.INVALID_IMAGE_EXTENSION, true);
                            return RedirectToAction("AddQuestions", "Examinations", new { examId = examID });
                        }
                        if (ValidateImage(question.MediaFile) == -2)
                        {
                            TempData.Add(Common.CommonConstants.INVALID_IMAGE_FILE_SIZE, true);
                            return RedirectToAction("AddQuestions", "Examinations", new { examId = examID });
                        }
                        fileName = Path.GetFileNameWithoutExtension(question.MediaFile.FileName) + DateTime.Now.ToString("yymmssfff") + Path.GetExtension(question.MediaFile.FileName);
                        question.Content = "~/public/uploadedFiles/questionPictures/" + fileName;
                        uploadFolderPath = Server.MapPath("~/public/uploadedFiles/questionPictures/");
                        if (Directory.Exists(uploadFolderPath) == false)
                        {
                            Directory.CreateDirectory(uploadFolderPath);
                        }
                        fileName = Path.Combine(uploadFolderPath, fileName);
                        question.MediaFile.SaveAs(fileName);
                        break;
                }

                db.Questions.Add(question);
                var content = new ExaminationContent();
                content.ExamID = examID;
                content.QuestionID = question.QuestionID;
                content.Mark = mark;
                db.ExaminationContents.Add(content);
                if (db.SaveChanges() > 0)
                {
                    Session.Remove(Common.CommonConstants.TEMPORARY_EXAMINATION);
                    TempData.Add(Common.CommonConstants.CREATE_SUCCESSFULLY, true);
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                TempData.Add(Common.CommonConstants.CREATE_FAILED, true);
                ex.Entries.Single().Reload();
            }
            
            
            return RedirectToAction("AddQuestions", "Examinations", new { examId = examID });
        }

        [HttpPost]
        public ActionResult AddAvailableQuestion(int questionId, int examId, int mark)
        {
            var content = new ExaminationContent();
            try
            {
                content.ExamID = examId;
                content.QuestionID = questionId;
                content.Mark = mark;
                db.ExaminationContents.Add(content);
                if (db.SaveChanges() > 0)
                {
                    TempData.Add(Common.CommonConstants.CREATE_SUCCESSFULLY, true);
                }
            }
            catch (Exception)
            {
                TempData.Add(Common.CommonConstants.CREATE_FAILED, true);
                return RedirectToAction("AddQuestions", "Examinations", new { examId = examId });
            }
            return RedirectToAction("AddQuestions", "Examinations", new { examId = examId });
        }

        [HttpPost]
        public ActionResult EditMarkOfTheQuestion(int? questionId, int? examId, int? mark)
        {
            if (questionId is null || examId is null || mark is null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var content = db.ExaminationContents.FirstOrDefault(e => e.ExamID == examId && e.QuestionID == questionId);
            try
            {
                content.Mark = mark;
                if (db.SaveChanges() > 0)
                {
                    TempData.Add(Common.CommonConstants.SAVE_SUCCESSFULLY, true);
                }
            }
            catch (Exception)
            {
                TempData.Add(Common.CommonConstants.SAVE_FAILED, true);
                return RedirectToAction("AddQuestions", "Examinations", new { examId = examId });
            }
            return RedirectToAction("AddQuestions", "Examinations", new { examId = examId });
        }

        [HttpPost]
        public ActionResult DeleteQuestion(int? questionId, int? examId)
        {
            if (questionId is null || examId is null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var content = db.ExaminationContents.FirstOrDefault(e => e.ExamID == examId && e.QuestionID == questionId);
            try
            {
                db.ExaminationContents.Remove(content);
                if (db.SaveChanges() > 0)
                {
                    TempData.Add(Common.CommonConstants.DELETE_SUCCESSFULLY, true);
                }
            }
            catch (Exception)
            {
                TempData.Add(Common.CommonConstants.DELETE_FAILED, true);
                return RedirectToAction("AddQuestions", "Examinations", new { examId = examId });
            }
            return RedirectToAction("AddQuestions", "Examinations", new { examId = examId });
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Examination examination = db.Examinations.Find(id);
            if (examination == null)
            {
                return HttpNotFound();
            }
            ViewBag.CourseID = new SelectList(db.Courses, "CourseID", "CourseName", examination.CourseID);
            return View(examination);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ExamID,ExamName,CourseID,Time,ExaminationDate,Status")] Examination examination)
        {
            if (ModelState.IsValid)
            {
                db.Entry(examination).State = EntityState.Modified;
                db.Entry(examination).Property(x => x.Status).IsModified = false;
                if (db.SaveChanges() > 0)
                {
                    TempData.Add(Common.CommonConstants.SAVE_SUCCESSFULLY, true);
                }
                return RedirectToAction("Index");
            }
            ViewBag.CourseID = new SelectList(db.Courses, "CourseID", "CourseName", examination.CourseID);
            return View(examination);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Examination examination = db.Examinations.Find(id);
            if (examination == null)
            {
                return HttpNotFound();
            }
            return View(examination);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Examination examination = db.Examinations.Find(id);
            try
            {
                db.Examinations.Remove(examination);
                if (db.SaveChanges() > 0)
                {
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
            var exam = db.Examinations.Find(id);
            exam.Status = 1;
            db.SaveChanges();
            return Json(new
            {
                status = true
            });
        }

        public JsonResult DisableStatus(int id)
        {
            var exam = db.Examinations.Find(id);
            exam.Status = 0;
            db.SaveChanges();
            return Json(new
            {
                status = true
            });
        }

        public int ValidateAudio(HttpPostedFileBase file)
        {
            string fileName = Path.GetFileNameWithoutExtension(file.FileName);
            string extension = Path.GetExtension(file.FileName);
            if ((extension == ".mp3" || extension == ".wav" || extension == ".wma" || extension == ".ogg" || extension == ".pcm") == false)
            {
                return -1;
            }
            long fileSize = ((file.ContentLength) / 1024);
            if (fileSize > 1048576)
            {
                return -2;
            }
            return 0;
        }

        public int ValidateImage(HttpPostedFileBase file)
        {
            string fileName = Path.GetFileNameWithoutExtension(file.FileName);
            string extension = Path.GetExtension(file.FileName);
            if ((extension == ".png" || extension == ".jpg" || extension == ".jpeg") == false)
            {
                return -1;
            }

            long fileSize = ((file.ContentLength) / 1024);
            if (fileSize > 5120)
            {
                return -2;
            }

            return 0;
        }

    }
}
