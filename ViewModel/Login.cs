using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace WebPhimNosime.ViewModel
{
    public class Login
    {
        [Required(ErrorMessage = "Username không được để trống")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Password không được để trống")]
        public string Password { get; set; }
    }
}