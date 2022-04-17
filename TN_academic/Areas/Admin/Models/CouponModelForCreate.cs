using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TN_academic.CustomValidation;

namespace TN_academic.Areas.Admin.Models
{
    public class CouponModelForCreate
    {
        public CouponModelForCreate()
        {
        }
        [DisplayName("Coupon ID")]
        public int CouponID { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter a valid coupon code!")]
        [StringLength(maximumLength: 50, MinimumLength = 5, ErrorMessage = "The coupon code must be between 5 and 50 characters long.")]
        [DisplayName("Code")]
        public string CouponCode { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter a valid coupon name!")]

        [StringLength(maximumLength: 50, MinimumLength = 5, ErrorMessage = "The coupon name must be between 5 and 50 characters long.")]

        [DisplayName("Coupon Name")]
        public string CouponName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter a valid description!")]
        public string Description { get; set; }
        public string Thumbnail { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter a valid rate!")]
        [Range(0, 100, ErrorMessage = "The rate must be greater than 0 and less than 100.")]
        public Nullable<decimal> Rate { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter quantity!")]
        public Nullable<int> Quantity { get; set; }

        public Nullable<bool> Status { get; set; }

        [ImageValidationForCreate]
        public HttpPostedFileBase ImageFile { get; set; }
    }
}