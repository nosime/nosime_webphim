using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebPhimNosime.Identity;

namespace WebPhimNosime.Models
{
    public class MyAccViewModel
    {
        public User User { get; set; }
        public ChangePasswordViewModel ChangePasswordViewModel { get; set; }
    }
}
