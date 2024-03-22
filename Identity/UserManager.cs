using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace WebPhimNosime.Identity
{
    public class UserManager : UserManager<User>
    {
        public UserManager(IUserStore<User> store) : base (store) { }
    }
}