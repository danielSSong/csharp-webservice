using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NetCore.Data.ViewModels
{
    public class RegisterInfo
    {
        [Required(ErrorMessage = "Please input the ID")]
        [MinLength(6, ErrorMessage = "You must input more 6 characters.")]
        [Display(Name = "User ID")]
        public string UserId { get; set; }
        [Required(ErrorMessage = "Please input the Name")]
        [Display(Name = "User Name")]
        public string UserName { get; set; }
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Please input the Email")]
        [Display(Name = "User Email")]
        public string UserEmail { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Please input the Password")]
        [MinLength(6, ErrorMessage = "You must input more 6 characters.")]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }
}
