using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace WebPhimNosime.Models
{
    public partial class WEBPHIM_SECURITY_Model : DbContext
    {
        public WEBPHIM_SECURITY_Model()
            : base("name=WEBPHIM_SECURITY_Context")
        {
        }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
