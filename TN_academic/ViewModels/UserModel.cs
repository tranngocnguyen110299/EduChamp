using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TN_academic.Models;

namespace TN_academic.ViewModels
{
    public class UserModel
    {
        public UserModel()
        {

        }
        public UserModel(User user)
        {
            this.Username = user.Username;
            this.FirstName = user.FirstName;
            this.LastName = user.LastName;
            this.Password = user.Password;
            this.DOB = user.DOB;
            this.Gender = user.Gender;
            this.Phone = user.Phone;
            this.Email = user.Email;
            this.Address = user.Address;
            this.Avatar = user.Avatar;
            this.StatusID = user.StatusID;
            this.Role = user.Role;
        }

        public string Username { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Password { get; set; }

        public Nullable<bool> Gender { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> DOB { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string Address { get; set; }

        public string Avatar { get; set; }

        public Nullable<int> StatusID { get; set; }

        public string Role { get; set; }
    }
}