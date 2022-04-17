using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TN_academic.CustomValidation;
using TN_academic.Models;

namespace TN_academic.Areas.Admin.Models
{
    public class UserModelForEdit
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public UserModelForEdit()
        {
        }

        public UserModelForEdit(User user)
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

        public UserModelForEdit(AdminLoginModel user)
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

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter a valid first name!")]
        [StringLength(maximumLength: 50, MinimumLength = 3, ErrorMessage = "The first name must be between 3 and 50 characters long.")]
        public string FirstName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter a valid last name!")]
        [StringLength(maximumLength: 50, MinimumLength = 3, ErrorMessage = "The last name must be between 3 and 50 characters long.")]
        public string LastName { get; set; }

        public string Password { get; set; }

        public Nullable<bool> Gender { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "The date of birth must be not empty.")]
        [DataType(DataType.Date, ErrorMessage = "The date of birth must be date.")]
        [DOBValidation]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Date of birth")]
        public Nullable<System.DateTime> DOB { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter a valid phone number!")]
        [StringLength(maximumLength: 15, MinimumLength = 3, ErrorMessage = "Phone number must be between 3 and 15 characters long.")]
        [RegularExpression(@"^[+]*[(]{0,1}[0-9]{1,4}[)]{0,1}[-\s\./0-9]*$", ErrorMessage = "The phone number is incorrect format. Try again, please!")]
        public string Phone { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter a valid email!")]
        [StringLength(maximumLength: 60, MinimumLength = 6, ErrorMessage = "The email must be between 6 and 60 characters long.")]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "The email is incorrect format. Try again, please!")]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter a valid address!")]
        [StringLength(maximumLength: 200, MinimumLength = 6, ErrorMessage = "The address must be between 6 and 200 characters long.")]
        public string Address { get; set; }

        public string Avatar { get; set; }
        public Nullable<int> StatusID { get; set; }
        public string Role { get; set; }

        [ImageValidationForUpdate]
        public HttpPostedFileBase EditedImage { get; set; }

    }
}