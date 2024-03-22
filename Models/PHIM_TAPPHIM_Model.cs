using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebPhimNosime.Models
{
    public class PHIM_TAPPHIM_Model
    {

        public PHIM InfoPhim { get; set; }
        public List<TAP_PHIM> TAP_PHIMs { get; set; }
        public bool IsBookmarked { get; set; }

    }
}