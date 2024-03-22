using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebPhimNosime.Models
{
    public class PHIM_BOOKMARK_Model
    {
        public int PhimID { get; set; }
        public string Title { get; set; }
        public string Theloai { get; set; }
        public string Namsx { get; set; }
        public int BookmarkCount { get; set; }
        public List<string> Users { get; set; }
    }
}