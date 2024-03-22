using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using WebPhimNosime.Models;
using PagedList;
using System.Web;
using System.IO;

namespace WebPhimNosime.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private WebPhimDBContext db = new WebPhimDBContext();

        // GET: Movies
        public ActionResult Index(int? page)
        {
            var pageNumber = (page ?? 1);
            var pageSize = 15;
            var movies = db.PHIMs.OrderBy(p=>p.ID_PHIM).ToPagedList(pageNumber, pageSize);
            return View(movies);
        }

        // GET: Movies/AddMovie
        public ActionResult AddMovie()
        {

            return View(new PHIM());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveMovie(PHIM movie, HttpPostedFileBase imagebg, HttpPostedFileBase imagephim)
        {
            if (ModelState.IsValid)
            {
                if (imagebg != null && imagebg.ContentLength > 0)
                {
                    string fileExtension = Path.GetExtension(imagebg.FileName);
                    string[] allowedExtensions = { ".png", ".jpg", ".jpeg", ".gif" };


                    if (allowedExtensions.Contains(fileExtension))
                    {
                        
                        string fileName = Guid.NewGuid().ToString() + fileExtension;

                      
                        string urlImage = Server.MapPath("~/images/background/" + fileName);

                       
                        imagebg.SaveAs(urlImage);

                        
                        movie.IMG_BG = fileName;
                    }
                }

                if (imagephim != null && imagephim.ContentLength > 0)
                {
                    string fileExtension = Path.GetExtension(imagephim.FileName);
                    string[] allowedExtensions = { ".png", ".jpg", ".jpeg", ".gif" };

                    if (allowedExtensions.Contains(fileExtension))
                    {
                        
                        string fileName = Guid.NewGuid().ToString() + fileExtension;

                       
                        string urlImage = Server.MapPath("~/images/img/" + fileName); 



                        imagephim.SaveAs(urlImage);

                       
                        movie.IMG_PHIM = fileName;
                    }
                }

                using (var dbContext = new WebPhimDBContext())
                {
                    // Save changes to the database
                    dbContext.PHIMs.Add(movie);
                    dbContext.SaveChanges();
                }

                return RedirectToAction("Index");
            }

            return View("AddMovie", movie);
        }



        // GET: Admin/EditMovie/5
        public ActionResult EditMovie(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            PHIM movie = db.PHIMs.Find(id);

            if (movie == null)
            {
                return HttpNotFound();
            }

            return View(movie);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditMovie(PHIM movie, HttpPostedFileBase imagephim, HttpPostedFileBase imagebg)
        {
            if (ModelState.IsValid)
            {
                // Lấy thông tin phim cũ từ cơ sở dữ liệu
                PHIM oldMovie = db.PHIMs.Find(movie.ID_PHIM);

                // Kiểm tra và xử lý ảnh nền
                if (imagebg != null && imagebg.ContentLength > 0)
                {
                    string fileExtension = Path.GetExtension(imagebg.FileName);
                    string[] allowedExtensions = { ".png", ".jpg", ".jpeg", ".gif" };

                    if (allowedExtensions.Contains(fileExtension))
                    {
                        
                       
                            DeleteOldImages(oldMovie.IMG_BG, "background");
                            
                            movie.IMG_BG = SaveImage(imagebg, "background"); 
                        
                       
                    }
                  
                }  else { 
                    movie.IMG_BG = oldMovie.IMG_BG;
                    }

                // Kiểm tra và xử lý ảnh phim
                if (imagephim != null && imagephim.ContentLength > 0)
                {
                    string fileExtension = Path.GetExtension(imagephim.FileName);
                    string[] allowedExtensions = { ".png", ".jpg", ".jpeg", ".gif" };

                    if (allowedExtensions.Contains(fileExtension))
                    {
                       
                       
                            DeleteOldImages(oldMovie.IMG_PHIM, "img");
                            
                            movie.IMG_PHIM = SaveImage(imagephim, "img");
                        
                       
                    }
                }
                else
                {
                    movie.IMG_PHIM = oldMovie.IMG_PHIM;
                }

                // Cập nhật thông tin phim
                db.Entry(oldMovie).CurrentValues.SetValues(movie);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(movie);
        }


        // Hàm xóa ảnh cũ
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

        // Hàm lưu ảnh mới
        private string SaveImage(HttpPostedFileBase image, string folder)
        {
            if (image != null && image.ContentLength > 0)
            {
                string fileExtension = Path.GetExtension(image.FileName);
                string[] allowedExtensions = { ".png", ".jpg", ".jpeg", ".gif" };

                if (allowedExtensions.Contains(fileExtension))
                {
                    string fileName = Guid.NewGuid().ToString() + fileExtension;
                    string urlImage = Path.Combine(Server.MapPath($"~/images/{folder}"), fileName);
                    image.SaveAs(urlImage);

                    return fileName; // Trả về tên file đã lưu
                }
            }

            return null;
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteMovie(int id)
        {
            PHIM movie = db.PHIMs.Find(id);

            if (movie == null)
            {
                return HttpNotFound();
            }
            DeleteOldImages(movie.IMG_BG, "background");
            DeleteOldImages(movie.IMG_PHIM, "img");
            // Thủ công xóa các bản ghi TAP_PHIM liên quan
            var relatedTaps = db.TAP_PHIM.Where(t => t.ID_PHIM == id);
            db.TAP_PHIM.RemoveRange(relatedTaps);

            // Thủ công xóa các bản ghi BOOKMARK liên quan
            var relatedBookmarks = db.BOOKMARKs.Where(b => b.ID_PHIM == id);
            db.BOOKMARKs.RemoveRange(relatedBookmarks);

            var relatedLSXem = db.LS_XEM.Where(b => b.ID_PHIM == id);
            db.LS_XEM.RemoveRange(relatedLSXem);

            // Xóa bản ghi PHIM
            db.PHIMs.Remove(movie);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult AddEpisode(int id)
        {
            // Lấy thông tin bộ phim
            var movie = db.PHIMs.Find(id);

            // Lấy danh sách tập phim của bộ phim
            var taps = db.TAP_PHIM.Where(t => t.ID_PHIM == id).ToList();

            // Truyền dữ liệu sang view
            ViewBag.MovieTitle = movie.TITLE_PHIM;
            ViewBag.MovieId = id;

            return View(taps);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveEpisode(string tap, string maTap, int idPhim)
        {
            // Thực hiện lưu vào cơ sở dữ liệu
            var newTap = new TAP_PHIM
            {
                TAP = tap,
                MA_TAP = maTap,
                ID_PHIM = idPhim
            };

            db.TAP_PHIM.Add(newTap);
            db.SaveChanges();

            // Lấy danh sách tập phim của bộ phim
            var taps = db.TAP_PHIM.Where(t => t.ID_PHIM == newTap.ID_PHIM).ToList();

            // Trả về PartialView
            return PartialView("_EpisodeListPartial", taps);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteEpisode(int tapId)
        {
            var tapToDelete = db.TAP_PHIM.Find(tapId);
            if (tapToDelete != null)
            {
                db.TAP_PHIM.Remove(tapToDelete);
                db.SaveChanges();
            }

            // Trả về PartialView chứa danh sách tập phim sau khi xóa thành công
            var idPhim = tapToDelete?.ID_PHIM ?? 0;
            var taps = db.TAP_PHIM.Where(t => t.ID_PHIM == idPhim).ToList();
            return PartialView("_EpisodeListPartial", taps);
        }


    }
}