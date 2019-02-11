using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NetCore.Data.ViewModels
{
    public class LoginInfo
    {
        [Required(ErrorMessage ="Please input the ID")]
        [MinLength(6, ErrorMessage = "You must input more 6 characters.")]
        [Display(Name = "User ID")]
        public string UserId { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Please input the Password")]
        [MinLength(6, ErrorMessage = "You must input more 6 characters.")]
        [Display(Name = "Password")]
        public string Password { get; set; }
        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }
}
