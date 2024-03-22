using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebPhimNosime.Models
{


    public class ChangePasswordViewModel
    {
        
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Nhập Mật khẩu hiện tại")]
        public string CurrentPassword { get; set; }

       
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Nhập Mật khẩu mới")]
        public string NewPassword { get; set; }

       
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu mới và mật khẩu xác nhận không khớp.")]
        [Required(ErrorMessage = "Xác nhận mật khẩu mới")]
        public string ConfirmPassword { get; set; }
    }


}