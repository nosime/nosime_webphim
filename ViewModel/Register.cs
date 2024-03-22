using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace WebPhimNosime.ViewModel
{
    public class Register
    {
        [Required(ErrorMessage = "Username không được để trống")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Password không được để trống")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Confilm Password không được để trống")]
        [Compare("Password", ErrorMessage = "Password và Confilm Password không giống")]
        public string ConfirmPassword { get; set; }
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        public string Email { get; set; }
        
        [DataType(DataType.Upload)]
        public HttpPostedFileBase AvatarFile { get; set; }
    }
}