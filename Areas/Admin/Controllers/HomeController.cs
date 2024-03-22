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
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System.IO;

namespace WebPhimNosime.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        private WebPhimDBContext dBContext = new WebPhimDBContext();
        private SecurityDbContext scdBContext = new SecurityDbContext();
        // GET: Admin/Home
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult BookmarkedMovies(int? page)
        {
            var pageNumber = (page ?? 1);
            var pageSize = 15;

            // Lấy danh sách phim đã đánh dấu của người dùng
            var bookmarkCounts = dBContext.PHIMs
                .Select(phim => new PHIM_BOOKMARK_Model
                {
                    PhimID = phim.ID_PHIM,
                    Title = phim.TITLE_PHIM,
                    Theloai = phim.THE_LOAI,
                    Namsx = phim.NAM_SX,
                    BookmarkCount = phim.BOOKMARKs.Count(),
                    Users = phim.BOOKMARKs.Select(bookmark => bookmark.ID_USER).ToList()
                })
                .ToList();
            var bm = bookmarkCounts.OrderBy(p => p.PhimID).ToPagedList(pageNumber, pageSize);
            return View(bm);
        }

        public ActionResult BookmarkDetails(int phimId, string titlePhim, int? page)
        {
            var pageNumber = (page ?? 1);
            var pageSize = 15;
            // Lấy danh sách người dùng đã đánh dấu bộ phim
            var userIdsWhoBookmarked = dBContext.BOOKMARKs
                .Where(b => b.ID_PHIM == phimId)
                .ToList();
            var ubm =userIdsWhoBookmarked.OrderBy(p => p.ID_PHIM).ToPagedList(pageNumber, pageSize);
            ViewBag.tt = titlePhim;
            ViewBag.idp = phimId;
            return View(ubm);
        }
       
        public ActionResult RemoveBookmark( int movieId,string userId)
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
            return RedirectToAction("BookmarkedMovies", "Home");
        }

        public ActionResult AccList(int? page)
        {
            var pageNumber = (page ?? 1);
            var pageSize = 15;
            List<User> ListUser = scdBContext.Users.ToList();
            var usl = ListUser.OrderBy(p => p.Id).ToPagedList(pageNumber, pageSize);
            return View(usl);
        }
        public ActionResult RemoveUser(string userId)
        {
            if (userId != null)
            {
                var user = scdBContext.Users.FirstOrDefault(b => b.Id == userId);

                if (user != null)
                {
                    var userManager = new UserManager<User>(new UserStore<User>(scdBContext));

                  
                    if (userManager.IsInRole(userId, "Customer"))
                    {
                        DeleteOldImages(user.Avatar, "img_user");
                        var relatedBookmarks = dBContext.BOOKMARKs.Where(b => b.ID_USER == userId);
                        dBContext.BOOKMARKs.RemoveRange(relatedBookmarks);

                        var relatedLSXem = dBContext.LS_XEM.Where(b => b.ID_USER == userId);
                        dBContext.LS_XEM.RemoveRange(relatedLSXem);
                        dBContext.SaveChanges();
                        scdBContext.Users.Remove(user);
                        scdBContext.SaveChanges();
                    }
                   
                }
            }

            return RedirectToAction("AccList", "Home");
        }
        private void DeleteOldImages(string imagePath, string folder)
        {
            if (!string.IsNullOrEmpty(imagePath))
            {
                string fullPath = Path.Combine(Server.MapPath($"~/images/{folder}"), imagePath);
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }
        }

        public ActionResult AllsHistory(int? page)
        {
            var pageNumber = (page ?? 1);
            var pageSize = 15;

            var allUsersHistory = dBContext.LS_XEM
                                            .OrderByDescending(b => b.NGAY)  
                                            .ToPagedList(pageNumber, pageSize);

            return View(allUsersHistory);
        }
        public ActionResult RemoveHistory(int movieId, int idTap ,  string userId)
        {



            if (userId != null && movieId != null && idTap != null)
            {
                var history = dBContext.LS_XEM
                    .FirstOrDefault(b => b.ID_USER == userId && b.ID_PHIM == movieId && b.ID_TAP == idTap);

                if (history != null)
                {
                    dBContext.LS_XEM.Remove(history);
                }


                dBContext.SaveChanges();
            }
            return RedirectToAction("AllsHistory", "Home");
        }
    }
}