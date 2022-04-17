using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using TN_academic.Areas.Admin.Models;
using TN_academic.Models;
using TN_academic.ViewModels;

namespace TN_academic.Controllers
{
    public class ExamController : Controller
    {
        private TN_academic_DBEntities db = new TN_academic_DBEntities();

        public ActionResult Index(int? id)
        {
            var user = (UserModel)Session[Common.CommonConstants.USER_LOGIN_MODEL];
            if (user is null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (id is null)
            {
                return RedirectToAction("Index", "Home");
            }
            var model = db.Examinations.Where(e => e.CourseID == id && e.Status == 1 && e.ExaminationDate >= DateTime.Now && e.ExaminationContents.Count > 0).ToList();
            ViewBag.CourseName = db.Courses.Find(id).CourseName;
            return View(model);
        }

        public ActionResult Start(int? id)
        {
            var user = (UserModel)Session[Common.CommonConstants.USER_LOGIN_MODEL];
            if (user is null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (id is null)
            {
                return RedirectToAction("Index", "Home");
            }
            var list = db.ExaminationContents.Where(e => e.ExamID == id).ToList();
            List<StudentExamModel> examContent = new List<StudentExamModel>();
            foreach (var item in list)
            {
                StudentExamModel studentExamModel = new StudentExamModel(item.Question);
                studentExamModel.Mark = (decimal)item.Mark;
                examContent.Add(studentExamModel);
            }
            Session.Add(Common.CommonConstants.STUDENT_EXAM_SESSION, examContent);

            int count = 0;
            Session.Add(Common.CommonConstants.QUESTION_COUNT, count);
            ViewBag.FirstQuestion = examContent[count];
            if (examContent.Count == (count + 1))
            {
                ViewBag.Next = false;
            }
            else
            {
                ViewBag.Next = true;
            }
            var examInformation = db.Examinations.FirstOrDefault(e => e.ExamID == id);
            ViewBag.ExamTime = examInformation.Time;
            ViewBag.CourseName = examInformation.Cours.CourseName;

            var exam = new ExamResult();
            exam.ExamID = id;
            exam.Username = user.Username;
            exam.ExamDate = DateTime.Now;

            Session.Add(Common.CommonConstants.EXAM_SESSION, exam);
            return View();
        }

        public JsonResult Next(String id, String choice)
        {
            var jsonID = new JavaScriptSerializer().Deserialize<int>(id);
            String jsonChoice;
            if (choice != null)
            {
                jsonChoice = new JavaScriptSerializer().Deserialize<String>(choice);
            }
            else
            {
                jsonChoice = null;
            }
            
            
            var examContent = (List<StudentExamModel>)Session[Common.CommonConstants.STUDENT_EXAM_SESSION];

            foreach (var item in examContent)
            {
                if (item.QuestionID == jsonID)
                {
                    item.Choice = jsonChoice;
                }
            }
 
            Session.Add(Common.CommonConstants.STUDENT_EXAM_SESSION, examContent);

            int count = (int)Session[Common.CommonConstants.QUESTION_COUNT];
            count += 1;
            Session.Add(Common.CommonConstants.QUESTION_COUNT, count);

            bool next = true;
            if (examContent.Count == (count+1))
            {
                next = false;
            }

            bool prev = true;
            if (count == 0)
            {
                prev = false;
            }

            var content = "";
            if (examContent[count].TypeID == 1)
            {
                content = examContent[count].Content;
            }
            if (examContent[count].TypeID == 2 || examContent[count].TypeID == 3)
            {
                content = Url.Content(examContent[count].Content);
            }

            return Json(new { 
                isNext = next,
                isPrev = prev,
                des = "Question " + (count + 1) + ": "+ examContent[count].Description,
                ID = examContent[count].QuestionID,
                question = content,
                choicea = examContent[count].ChoiceA,
                choiceb = examContent[count].ChoiceB,
                choicec = examContent[count].ChoiceC,
                choiced = examContent[count].ChoiceD,
                type = examContent[count].TypeID,
                mark = examContent[count].Mark,
                choiceCheck = examContent[count].Choice
            });
        }


        public JsonResult Previous(String id, String choice)
        {
            var jsonID = new JavaScriptSerializer().Deserialize<int>(id);
            String jsonChoice;
            if (choice != null)
            {
                jsonChoice = new JavaScriptSerializer().Deserialize<String>(choice);
            }
            else
            {
                jsonChoice = null;
            }

            var examContent = (List<StudentExamModel>)Session[Common.CommonConstants.STUDENT_EXAM_SESSION];

            foreach (var item in examContent)
            {
                if (item.QuestionID == jsonID)
                {
                    item.Choice = jsonChoice;
                }
            }

            Session.Add(Common.CommonConstants.STUDENT_EXAM_SESSION, examContent);

            int count = (int)Session[Common.CommonConstants.QUESTION_COUNT];
            count -= 1;
            Session.Add(Common.CommonConstants.QUESTION_COUNT, count);

            bool next = true;
            if (examContent.Count == (count + 1))
            {
                next = false;
            }

            bool prev = true;
            if (count == 0)
            {
                prev = false;
            }

            var content = "";
            if (examContent[count].TypeID == 1)
            {
                content = examContent[count].Content;
            }
            if (examContent[count].TypeID == 2 || examContent[count].TypeID == 3)
            {
                content = Url.Content(examContent[count].Content);
            }

            return Json(new
            {
                isNext = next,
                isPrev = prev,
                des = "Question " + (count + 1) + ": " + examContent[count].Description,
                ID = examContent[count].QuestionID,
                question = content,
                choicea = examContent[count].ChoiceA,
                choiceb = examContent[count].ChoiceB,
                choicec = examContent[count].ChoiceC,
                choiced = examContent[count].ChoiceD,
                type = examContent[count].TypeID,
                mark = examContent[count].Mark,
                choiceCheck = examContent[count].Choice
            });
        }

        public ActionResult Finish()
        {
            var user = (UserModel)Session[Common.CommonConstants.USER_LOGIN_MODEL];
            if (user is null)
            {
                return RedirectToAction("Index", "Login");
            }
            try
            {
                var examContent = (List<StudentExamModel>)Session[Common.CommonConstants.STUDENT_EXAM_SESSION];
                var examLog = (ExamResult)Session[Common.CommonConstants.EXAM_SESSION];
                var answer = db.ExaminationContents.Where(e => e.ExamID == examLog.ExamID).ToList();
                decimal mark = 0;
                int qCount = 0;
                decimal maxMark = 0;
                foreach (var item in answer)
                {
                    string ans = null;
                    maxMark += (decimal)item.Mark;
                    switch (item.Question.Answer)
                    {
                        case "1":
                            ans = item.Question.ChoiceA;
                            break;
                        case "2":
                            ans = item.Question.ChoiceB;
                            break;
                        case "3":
                            ans = item.Question.ChoiceC;
                            break;
                        case "4":
                            ans = item.Question.ChoiceD;
                            break;
                    }

                    foreach (var cont in examContent)
                    {
                        if (cont.QuestionID == item.QuestionID)
                        {
                            if (cont.Choice != null && cont.Choice.Equals(ans))
                            {
                                mark += cont.Mark;
                                qCount += 1;
                            }
                        }
                    }
                }

                examLog.Mark = decimal.Round(((mark * 100) / maxMark), 0);
                examLog.Time = DateTime.Now;
                examLog.Result = qCount + "/" + examContent.Count;

                if (examLog.Mark > 50)
                {
                    examLog.Status = 1;
                }
                else
                {
                    examLog.Status = 0;
                }

                db.ExamResults.Add(examLog);
                db.SaveChanges();

                var examInformation = db.Examinations.FirstOrDefault(e => e.ExamID == examLog.ExamID);
                ViewBag.ExamInformation = examInformation;
                ViewBag.StudentName = user.FirstName + " " + user.LastName;

                return View(examLog);
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }
        }


        public JsonResult Complete(String id, String choice)
        {
            var jsonID = new JavaScriptSerializer().Deserialize<int>(id);
            String jsonChoice;
            if (choice != null)
            {
                jsonChoice = new JavaScriptSerializer().Deserialize<String>(choice);
            }
            else
            {
                jsonChoice = null;
            }


            var examContent = (List<StudentExamModel>)Session[Common.CommonConstants.STUDENT_EXAM_SESSION];

            foreach (var item in examContent)
            {
                if (item.QuestionID == jsonID)
                {
                    item.Choice = jsonChoice;
                }
            }

            Session.Add(Common.CommonConstants.STUDENT_EXAM_SESSION, examContent);

            return Json(new
            {
                status = true
            });
        }

        public ActionResult Manage()
        {
            var user = (UserModel)Session[Common.CommonConstants.USER_LOGIN_MODEL];
            if (user is null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (user.Role.Equals("2"))
            {
                var examinations = db.Examinations.Where(i => i.Cours.Username == user.Username).ToList();
                return View(examinations.ToList());
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

            
        }

        public ActionResult Create()
        {
            var user = (UserModel)Session[Common.CommonConstants.USER_LOGIN_MODEL];
            if (user is null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (user.Role.Equals("2"))
            {
                ViewBag.CourseID = new SelectList(db.Courses.Where(i => i.Username == user.Username), "CourseID", "CourseName");
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ExamID,ExamName,CourseID,Time,ExaminationDate,Status")] ExaminationModelForCreate examination)
        {
            var user = (UserModel)Session[Common.CommonConstants.USER_LOGIN_MODEL];
            if (user is null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (user.Role.Equals("2"))
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
                    return RedirectToAction("Manage", "Exam", new { examId = exam.ExamID });
                }
            }
            
            ViewBag.CourseID = new SelectList(db.Courses.Where(i => i.Username == user.Username), "CourseID", "CourseName", examination.CourseID);
            return View(examination);
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

        public ActionResult AddQuestions(int? examId)
        {
            var user = (UserModel)Session[Common.CommonConstants.USER_LOGIN_MODEL];
            if (user is null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (examId == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (user.Role.Equals("2"))
            {
                var examination = db.Examinations.FirstOrDefault(e => e.ExamID == examId);
                Session.Add(Common.CommonConstants.TEMPORARY_EXAMINATION, examination);
                ViewBag.QuestionList = db.Questions.SqlQuery("select * from Questions where CourseID = " + examination.CourseID + " AND QuestionID NOT IN (select QuestionID from ExaminationContent where ExamID = " + examId + ")").ToList();
                ViewBag.TypeID = new SelectList(db.QuestionTypes, "TypeID", "TypeName");
                ViewBag.Contents = db.ExaminationContents.Where(c => c.ExamID == examId).ToList();
                ViewBag.Level = new SelectList(new List<SelectListItem> { new SelectListItem { Text = "Easy", Value = "-1" }, new SelectListItem { Text = "Normal", Value = "0" }, new SelectListItem { Text = "Difficult", Value = "1" }, new SelectListItem { Text = "Extremely difficult", Value = "2" } }, "Value", "Text");
            }
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddQuestions([Bind(Include = "QuestionID,TypeID,CourseID,Content,Description,ChoiceA,ChoiceB,ChoiceC,ChoiceD,Answer,Level,MediaFile,Mark")] Question question, int examID, int? mark)
        {
            var user = (UserModel)Session[Common.CommonConstants.USER_LOGIN_MODEL];
            if (user is null)
            {
                return RedirectToAction("Index", "Login");
            }

            if (question.Content is null && question.TypeID == 1)
            {
                TempData.Add(Common.CommonConstants.INVALID_QUESTION_CONTENT, true);
                return RedirectToAction("AddQuestions", "Exam", new { examId = examID });
            }
            else if (question.Description is null)
            {
                TempData.Add(Common.CommonConstants.INVALID_QUESTION_DESCRIPTION, true);
                return RedirectToAction("AddQuestions", "Exam", new { examId = examID });
            }
            else if (question.ChoiceA is null)
            {
                TempData.Add(Common.CommonConstants.INVALID_QUESTION_CHOICEA, true);
                return RedirectToAction("AddQuestions", "Exam", new { examId = examID });
            }
            else if (question.ChoiceB is null)
            {
                TempData.Add(Common.CommonConstants.INVALID_QUESTION_CHOICEB, true);
                return RedirectToAction("AddQuestions", "Exam", new { examId = examID });
            }
            else if (question.ChoiceC is null)
            {
                TempData.Add(Common.CommonConstants.INVALID_QUESTION_CHOICEC, true);
                return RedirectToAction("AddQuestions", "Exam", new { examId = examID });
            }
            else if (question.ChoiceD is null)
            {
                TempData.Add(Common.CommonConstants.INVALID_QUESTION_CHOICED, true);
                return RedirectToAction("AddQuestions", "Exam", new { examId = examID });
            }
            else if (mark.ToString() is null)
            {
                TempData.Add(Common.CommonConstants.INVALID_QUESTION_MARK, true);
                return RedirectToAction("AddQuestions", "Exam", new { examId = examID });
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
                            return RedirectToAction("AddQuestions", "Exam", new { examId = examID });
                        }
                        if (ValidateAudio(question.MediaFile) == -2)
                        {
                            TempData.Add(Common.CommonConstants.INVALID_AUDIO_FILE_SIZE, true);
                            return RedirectToAction("AddQuestions", "Exam", new { examId = examID });
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
                            return RedirectToAction("AddQuestions", "Exam", new { examId = examID });
                        }
                        if (ValidateImage(question.MediaFile) == -2)
                        {
                            TempData.Add(Common.CommonConstants.INVALID_IMAGE_FILE_SIZE, true);
                            return RedirectToAction("AddQuestions", "Exam", new { examId = examID });
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


            return RedirectToAction("AddQuestions", "Exam", new { examId = examID });
        }

        [HttpPost]
        public ActionResult AddAvailableQuestion(int questionId, int examId, int mark)
        {
            var user = (UserModel)Session[Common.CommonConstants.USER_LOGIN_MODEL];
            if (user is null)
            {
                return RedirectToAction("Index", "Login");
            }
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
                return RedirectToAction("AddQuestions", "Exam", new { examId = examId });
            }
            return RedirectToAction("AddQuestions", "Exam", new { examId = examId });
        }

        [HttpPost]
        public ActionResult EditMarkOfTheQuestion(int? questionId, int? examId, int? mark)
        {
            var user = (UserModel)Session[Common.CommonConstants.USER_LOGIN_MODEL];
            if (user is null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (user.Role.Equals("2"))
            {
                if (questionId is null || examId is null || mark is null)
                {
                    return RedirectToAction("Index", "Home");
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
                    return RedirectToAction("AddQuestions", "Exam", new { examId = examId });
                }
            }
            
            return RedirectToAction("AddQuestions", "Exam", new { examId = examId });
        }

        [HttpPost]
        public ActionResult DeleteQuestion(int? questionId, int? examId)
        {
            var user = (UserModel)Session[Common.CommonConstants.USER_LOGIN_MODEL];
            if (user is null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (user.Role.Equals("2"))
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
                    return RedirectToAction("AddQuestions", "Exam", new { examId = examId });
                }
            }
            
            return RedirectToAction("AddQuestions", "Exam", new { examId = examId });
        }

        public ActionResult Details(int? id)
        {
            var user = (UserModel)Session[Common.CommonConstants.USER_LOGIN_MODEL];
            if (user is null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (user.Role.Equals("2"))
            {

            }
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }
            Examination examination = db.Examinations.FirstOrDefault(e => e.ExamID == id && e.Cours.Username == user.Username);
            if (examination == null)
            {
                return HttpNotFound();
            }
            return View(examination);
        }

        public ActionResult Edit(int? id)
        {
            var user = (UserModel)Session[Common.CommonConstants.USER_LOGIN_MODEL];
            if (user is null)
            {
                TempData.Add(Common.CommonConstants.ACCESS_DENIED, true);
                return RedirectToAction("Index", "Home");
            }
            if (id == null)
            {
                TempData.Add(Common.CommonConstants.ACCESS_DENIED, true);
                return RedirectToAction("Index", "Home");
            }
            Examination examination = db.Examinations.FirstOrDefault(e => e.ExamID == id && e.Cours.Username == user.Username);
            if (examination == null)
            {
                TempData.Add(Common.CommonConstants.ACCESS_DENIED, true);
                return RedirectToAction("Index", "Home");
            }
            ViewBag.CourseID = new SelectList(db.Courses.Where(i => i.Username == user.Username), "CourseID", "CourseName", examination.CourseID);
            return View(examination);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ExamID,ExamName,CourseID,Time,ExaminationDate,Status")] Examination examination)
        {
            var user = (UserModel)Session[Common.CommonConstants.USER_LOGIN_MODEL];
            if (user is null)
            {
                TempData.Add(Common.CommonConstants.ACCESS_DENIED, true);
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                db.Entry(examination).State = EntityState.Modified;
                db.Entry(examination).Property(x => x.Status).IsModified = false;
                if (db.SaveChanges() > 0)
                {
                    TempData.Add(Common.CommonConstants.SAVE_SUCCESSFULLY, true);
                }
                return RedirectToAction("Manage");
            }
            ViewBag.CourseID = new SelectList(db.Courses.Where(i => i.Username == user.Username), "CourseID", "CourseName", examination.CourseID);
            return View(examination);
        }

        public ActionResult Delete(int? id)
        {
            var user = (UserModel)Session[Common.CommonConstants.USER_LOGIN_MODEL];
            if (user is null)
            {
                TempData.Add(Common.CommonConstants.ACCESS_DENIED, true);
                return RedirectToAction("Index", "Home");
            }
            if (id == null)
            {
                TempData.Add(Common.CommonConstants.ACCESS_DENIED, true);
                return RedirectToAction("Index", "Home");
            }
            Examination examination = db.Examinations.FirstOrDefault(e => e.ExamID == id && e.Cours.Username == user.Username);
            if (examination == null)
            {
                TempData.Add(Common.CommonConstants.ACCESS_DENIED, true);
                return RedirectToAction("Index", "Home");
            }
            return View(examination);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var user = (UserModel)Session[Common.CommonConstants.USER_LOGIN_MODEL];
            if (user is null)
            {
                TempData.Add(Common.CommonConstants.ACCESS_DENIED, true);
                return RedirectToAction("Index", "Home");
            }
            Examination examination = db.Examinations.FirstOrDefault(e => e.ExamID == id && e.Cours.Username == user.Username);
            if (examination == null)
            {
                TempData.Add(Common.CommonConstants.ACCESS_DENIED, true);
                return RedirectToAction("Index", "Home");
            }
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
            return RedirectToAction("Manage");
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
    }
}