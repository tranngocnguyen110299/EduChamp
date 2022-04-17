using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TN_academic.Models;

namespace TN_academic.ViewModels
{
    public class StudentExamModel
    {
        public StudentExamModel() { }
        public StudentExamModel(Question question)
        {
            this.QuestionID = question.QuestionID;
            this.TypeID = question.TypeID;
            this.Description = question.Description;
            this.Content = question.Content;
            this.ChoiceA = question.ChoiceA;
            this.ChoiceB = question.ChoiceB;
            this.ChoiceC = question.ChoiceC;
            this.ChoiceD = question.ChoiceD;
        }

        public int QuestionID { get; set; }

        public Nullable<int> TypeID { get; set; }

        public string Description { get; set; }

        public string Content { get; set; }

        public string ChoiceA { get; set; }

        public string ChoiceB { get; set; }

        public string ChoiceC { get; set; }

        public string ChoiceD { get; set; }

        public string Choice { get; set; }

        public decimal Mark { get; set; }
    }
}