using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NetCore.Data.ViewModels
{
    public class WithdrawnInfo
    {
        
        public string UserId { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Please input the Password")]
        [MinLength(6, ErrorMessage = "You must input more 6 characters.")]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }
}
