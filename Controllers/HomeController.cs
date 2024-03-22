using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using WebPhimNosime.Models;
using PagedList;
using WebPhimNosime.Identity;
using WebPhimNosime.Filters;

namespace WebPhim.Controllers
{
    public class HomeController : Controller
    {
        private WebPhimDBContext dBContext = new WebPhimDBContext();
        private SecurityDbContext scdBContext = new SecurityDbContext();
        public ActionResult Index()
        {
            List<PHIM> ListPhim = dBContext.PHIMs.ToList();
            return View(ListPhim);
        }

        public ActionResult Search(int? subtagId, int? tagId, int? page)
        {
            var phims = dBContext.PHIMs.Include(p => p.TAG);
            var pageNumber = (page ?? 1);
            var pageSize = 15;

            if (subtagId.HasValue)
            {
                phims = phims.Where(p => p.ID_SUBTAG == subtagId.Value);
                var subtagName = dBContext.SUBTAGs.Where(t => t.ID_SUBTAG == subtagId.Value).Select(t => t.SUBTAG_NAME).FirstOrDefault();
                ViewData["Title"] = subtagName;
                ViewBag.SearchString = subtagName;
            }

            if (tagId.HasValue)
            {
                if (tagId == 1)
                {
                    var phimspm = dBContext.PHIMs.ToList();
                    var tagName = dBContext.TAGs.Where(t => t.ID_TAG == tagId.Value).Select(t => t.TAG_NAME).FirstOrDefault();
                    ViewData["Title"] = tagName;
                    ViewBag.SearchString = tagName;
                    return View(new PhimBySubtagViewModel
                    {
                        Phims = phimspm.ToPagedList(pageNumber, pageSize),
                        Tags = dBContext.TAGs.ToList(),
                        SelectedSubtagId = subtagId ?? 0,
                        SelectedTagId = tagId ?? 0
                    });
                }
                else
                {
                    phims = phims.Where(p => p.ID_TAG == tagId.Value);
                    var tagName = dBContext.TAGs.Where(t => t.ID_TAG == tagId.Value).Select(t => t.TAG_NAME).FirstOrDefault();
                    ViewData["Title"] = tagName;
                    ViewBag.SearchString = tagName;
                }
            }

            phims = phims.OrderBy(p => p.ID_PHIM);

            var tags = dBContext.TAGs.ToList();

            var model = new PhimBySubtagViewModel
            {
                Phims = phims.ToPagedList(pageNumber, pageSize),
                Tags = tags,
                SelectedSubtagId = subtagId ?? 0,
                SelectedTagId = tagId ?? 0
            };

            return View(model);
        }

        public ActionResult SearchString(string searchString, int? page)
        {
            // Tìm kiếm phim theo từ khóa searchString
            var phims = dBContext.PHIMs
                .Where(p => p.TEN_PHIM.Contains(searchString) || p.TITLE_PHIM.Contains(searchString));


            const int pageSize = 15;
            int pageNumber = (page ?? 1);

            phims = phims.OrderBy(p => p.ID_PHIM);

            var tags = dBContext.TAGs.ToList();

            var model = new PhimBySubtagViewModel
            {
                Phims = phims.ToPagedList(pageNumber, pageSize),
            };

            ViewBag.SearchString = searchString; // Truyền searchString để hiển thị trên View
            return View("Search", model);
        }

        [HttpGet]
        public ActionResult SearchStringKey(string searchString)
        {
            // Perform the search and return a partial view with the results
            var phims = dBContext.PHIMs
                .Where(p => p.TEN_PHIM.Contains(searchString) || p.TITLE_PHIM.Contains(searchString))
                .ToList();

            return PartialView("_SearchResults", phims);
        }
        public ActionResult Contact()
        {

            return View();
        }
        public ActionResult GioiThieu()
        {

            return View();
        }


        public ActionResult DetailPhim(int IdPhim)
        {


            PHIM InfoPhim = dBContext.PHIMs.FirstOrDefault(x => x.ID_PHIM == IdPhim);

           
            var tapPhimEntities = dBContext.TAP_PHIM
                .Where(tap => tap.ID_PHIM == IdPhim)
                .OrderBy(tap => tap.TAP) 
                .ToList();
           
            List<TAP_PHIM> DSTapPhim = tapPhimEntities
                .Select(tap => new TAP_PHIM
                {
                    ID_TAP = tap.ID_TAP,
                    TAP = tap.TAP,
                    MA_TAP = tap.MA_TAP
                })
                .ToList();

           
            PHIM_TAPPHIM_Model ListInfo_TapPhim = new PHIM_TAPPHIM_Model
            {
                InfoPhim = InfoPhim,
                TAP_PHIMs = DSTapPhim
            };

            return View(ListInfo_TapPhim);
        }

        [MyAuthenFilter]
        public ActionResult BookmarkedMovies(string userId, int? page)
        {
            var pageNumber = (page ?? 1);
            var pageSize = 15;
           
            var bookmarkedMovies = dBContext.BOOKMARKs
                .Where(b => b.ID_USER == userId)
                .Select(b => b.PHIM)
                .ToList();
            var bm = bookmarkedMovies.OrderBy(p => p.ID_PHIM).ToPagedList(pageNumber, pageSize);
            return View(bm);
        }

       
        [HttpPost]
       
        public ActionResult ToggleBookmark(string userId, int movieId)
        {
            
            

            if (userId != null && movieId != null)
            {
                var bookmark = dBContext.BOOKMARKs
                    .FirstOrDefault(b => b.ID_USER == userId && b.ID_PHIM == movieId);

                if (bookmark == null)
                {
                    var newBookmark = new BOOKMARK
                    {
                        ID_USER = userId,
                        ID_PHIM = movieId,
                        NGAY = DateTime.Now 
                    };

                    // Thêm bookmark vào database
                    dBContext.BOOKMARKs.Add(newBookmark);
                   
                }
                else
                {
                    dBContext.BOOKMARKs.Remove(bookmark);
                    
                }

               dBContext.SaveChanges();
            }

            // Trả về một đối tượng JSON để cập nhật trạng thái nút
            var isBookmarked = IsMovieBookmarked(movieId, userId);
            return Json(new { isBookmarked = isBookmarked });
        }


        private bool IsMovieBookmarked(int movieId, string userId)
        {
            return dBContext.BOOKMARKs
                .Any(b => b.ID_USER == userId && b.ID_PHIM == movieId);
        }
        [HttpGet]
        public ActionResult StatusBookmarked(int movieId, string userId)
        {
            var isBookmarked = IsMovieBookmarked(movieId, userId);
            return Json(new { isBookmarked = isBookmarked }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RemoveBookmark(int movieId, string userId)
        {



            if (userId != null && movieId != null)
            {
                var bookmark = dBContext.BOOKMARKs
                    .FirstOrDefault(b => b.ID_USER == userId && b.ID_PHIM == movieId);

                if (bookmark != null)
                {
                    dBContext.BOOKMARKs.Remove(bookmark);
                }


                dBContext.SaveChanges();
            }
            return RedirectToAction("BookmarkedMovies", "Home" ,new { userId =userId });
        }

        [HttpPost]
        public ActionResult SaveLSXem(int idPhim, int idTap)
        {
          
            var userId = scdBContext.Users.FirstOrDefault(p => p.UserName == User.Identity.Name)?.Id;

            if (User.Identity.IsAuthenticated) { 
                    
                var existingRecord = dBContext.LS_XEM.FirstOrDefault(ls => ls.ID_PHIM == idPhim && ls.ID_TAP == idTap && ls.ID_USER == userId);

            if (existingRecord == null)
            {
               
                var newRecord = new LS_XEM
                {
                    ID_PHIM = idPhim,
                    ID_TAP = idTap,
                    ID_USER = userId,
                    NGAY = DateTime.Now
                };

              
                dBContext.LS_XEM.Add(newRecord);

               
                dBContext.SaveChanges();
            }

           }
            return Json(new { success = true });
        }
        public ActionResult ListLSXem(int? page)
        {
            var userId = scdBContext.Users.FirstOrDefault(p => p.UserName == User.Identity.Name)?.Id;
            var pageNumber = (page ?? 1);
            var pageSize = 15;

            var listLSXem = dBContext.LS_XEM
                                    .Where(b => b.ID_USER == userId)
                                    .OrderByDescending(b => b.NGAY)  // Sắp xếp theo ngày giảm dần để hiển thị lịch sử mới nhất đầu tiên
                                    .ToList();

            var llx = listLSXem.ToPagedList(pageNumber, pageSize);

            return View(llx);
        }


    }

}