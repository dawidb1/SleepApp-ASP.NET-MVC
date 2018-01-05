using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace User_Registration_MVC.Models
{
    [MetadataType(typeof(UserMetadata))]
    public partial class User
    {
        public string ConfirmPassword { get; set; }
        public bool RememberMe { get; set; }

        public void InitOtherData()
        {
            throw new NotImplementedException();
        }
        public int ValidateUser(string username, string password)
        {
            throw new NotImplementedException();
        }
    }
    public class UserMetadata
    {
        [Required(AllowEmptyStrings = false,ErrorMessage = "User name required")]
        public string Username { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Password required")]
        [DataType(DataType.Password)]
        [MinLength(6,ErrorMessage ="Minimum 6 characters required")]
        public string Password { get; set; }

        [Display(Name = "Confirm password")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Password required")]
        [DataType(DataType.Password)]
        [Compare("Password",ErrorMessage ="Passwords not match")]
        public string ConfirmPassword { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Email adress required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        public System.DateTime CreatedDate { get; set; }
        public Nullable<System.DateTime> LastLoginDate { get; set; }

        [Display(Name ="Date of birth")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true,DataFormatString ="{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> DateOfBirth { get; set; }
    }
}