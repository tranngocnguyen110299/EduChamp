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
    public class CouponModelForEdit
    {
        public CouponModelForEdit()
        {
        }

        public CouponModelForEdit(Coupon coupon)
        {
            this.CouponID = coupon.CouponID;
            this.CouponCode = coupon.CouponCode;
            this.CouponName = coupon.CouponName;
            this.Description = coupon.Description;
            this.Thumbnail = coupon.Thumbnail;
            this.Rate = coupon.Rate;
            this.Quantity = coupon.Quantity;
            this.Status = coupon.Status;
        }

        public int CouponID { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter a valid coupon code!")]
        [StringLength(maximumLength: 50, MinimumLength = 5, ErrorMessage = "The coupon code must be between 5 and 50 characters long.")]
        [DisplayName("Code")]
        public string CouponCode { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter a valid coupon name!")]
        [StringLength(maximumLength: 50, MinimumLength = 5, ErrorMessage = "The coupon name must be between 5 and 150 characters long.")]
        [DisplayName("Coupon Name")]
        public string CouponName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter description!")]
        public string Description { get; set; }
        public string Thumbnail { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter a valid rate!")]
        [Range(0, 100, ErrorMessage = "The rate must be greater than 0 and less than 100.")]
        public Nullable<decimal> Rate { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter quantity!")]
        public Nullable<int> Quantity { get; set; }

        public Nullable<bool> Status { get; set; }

        [ImageValidationForUpdate]
        public HttpPostedFileBase ImageFile { get; set; }
    }
}