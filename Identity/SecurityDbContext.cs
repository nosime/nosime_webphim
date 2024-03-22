using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;
using WebPhimNosime.Identity;

namespace WebPhimNosime.Identity
{
    public class SecurityDbContext : IdentityDbContext<User>
    {
        public SecurityDbContext() : base("WEBPHIM_SECURITY_Context") { }
    }
}