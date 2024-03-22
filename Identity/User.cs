using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;
namespace WebPhimNosime.Identity
{
    public class User : IdentityUser
    {
       
        public string Avatar { get; set; }
    }
}