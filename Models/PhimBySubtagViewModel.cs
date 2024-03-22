using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;

namespace WebPhimNosime.Models
{
    public class PhimBySubtagViewModel
    {
        public IPagedList<PHIM> Phims { get; set; }
        public List<TAG> Tags { get; set; }
        public List<SUBTAG> Subtags { get; set; }
        public int SelectedSubtagId { get; set; }
        public int SelectedTagId { get; set; }
    }

}