using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebPhimNosime.Models;
using WebPhimNosime.ViewModel;
using WebPhimNosime.Identity;
using System.Web.Helpers;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System.IO;
using System.Threading.Tasks;

namespace WebPhimNosime.Controllers
{
  
    public class AccountController : Controller
    {
        private SecurityDbContext scdBContext = new SecurityDbContext();
        // GET: Account
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(Register rvm)
        {
            if (ModelState.IsValid)
            {
                var securityDbContext = new SecurityDbContext();
                var userStore = new UserStore(securityDbContext);
                var userManager = new UserManager(userStore);
                var passwdHash = Crypto.HashPassword(rvm.Password);
                var user = new User()
                {
                    UserName = rvm.Username,
                    PasswordHash = passwdHash,
                    Email = rvm.Email,
                   
                };

                IdentityResult identityResult = userManager.Create(user);
                if (identityResult.Succeeded)
                {
                    userManager.AddToRole(user.Id, "Customer");

                    var authenManager = HttpContext.GetOwinContext().Authentication;
                    var userIdentity = userManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
                    authenManager.SignIn(new AuthenticationProperties(), userIdentity);
                    securityDbContext.SaveChanges();
                }
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("New Error", "Invalid Data");
                return View();
            }
           
        }



        public ActionResult Login()
        {
            Session["ReturnUrl"] = Request.UrlReferrer?.AbsoluteUri;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Login lvm)
        {
            if (ModelState.IsValid) // Check if model is valid
            {
                if (!string.IsNullOrEmpty(lvm.Username) && !string.IsNullOrEmpty(lvm.Password))
                {
                    var securityDbContext = new SecurityDbContext();
                    var userStore = new UserStore(securityDbContext);
                    var userManager = new UserManager(userStore);
                    var user = userManager.Find(lvm.Username, lvm.Password);

                    if (user != null)
                    {
                        var authenManager = HttpContext.GetOwinContext().Authentication;
                        var userIdentity = userManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
                        authenManager.SignIn(new AuthenticationProperties(), userIdentity);

                        // Lấy lại URL đã lưu từ phiên
                        var returnUrl = Session["ReturnUrl"] as string;

                        // Xóa URL đã lưu từ phiên
                        Session["ReturnUrl"] = null;

                        if (userManager.IsInRole(user.Id, "Admin"))
                        {
                            return RedirectToAction("Index", "Admin", new { area = "Admin" });
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(returnUrl))
                            {
                                // Chuyển hướng người dùng trở lại trang trước đó
                                return Redirect(returnUrl);
                            }
                        }
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("New Error", "Invalid Username and Password");
                    }
                }
            }

            return View();
        }

        public ActionResult Logout()
        {
            var authenManager = HttpContext.GetOwinContext().Authentication;
            authenManager.SignOut();
            return RedirectToAction("Index", "Home");
        }
        public ActionResult MyAcc()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.Identity.GetUserId();
                var userManager = new UserManager(new UserStore(new SecurityDbContext()));
                var user = userManager.FindById(userId);

                if (user != null)
                {
                    var myAccViewModel = new MyAccViewModel
                    {
                        User = user,
                        ChangePasswordViewModel = new ChangePasswordViewModel()
                    };

                    return View(myAccViewModel);
                }
            }

            return RedirectToAction("Login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(MyAccViewModel model)
        {
            var userId = User.Identity.GetUserId();
            var userManager = new UserManager(new UserStore(new SecurityDbContext()));

            if (ModelState.IsValid)
            {
                var result = await userManager.ChangePasswordAsync(userId, model.ChangePasswordViewModel.CurrentPassword, model.ChangePasswordViewModel.NewPassword);

                if (result.Succeeded)
                {
                    var user = await userManager.FindByIdAsync(userId);

                    if (user != null)
                    {
                        var authenManager = HttpContext.GetOwinContext().Authentication;
                        var userIdentity = userManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
                        authenManager.SignIn(new AuthenticationProperties(), userIdentity);

                        TempData["SuccessMessage"] = "Password changed successfully!";
                        return RedirectToAction("MyAcc");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Failed to retrieve user information.");
                    }
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }
            }

            // If we are here, the model is not valid or there was an error changing the password
            var updatedModel = new MyAccViewModel
            {
                User = userManager.FindById(userId),
                ChangePasswordViewModel = model.ChangePasswordViewModel
            };

            return View("MyAcc", updatedModel);
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeAvatar(HttpPostedFileBase image)
        {
            if (image != null && image.ContentLength > 0)
            {
                try
                {
                    var userId = User.Identity.GetUserId();
                    var securityDbContext = new SecurityDbContext();
                    var userStore = new UserStore(securityDbContext);
                    var userManager = new UserManager(userStore);
                    var user = userManager.FindById(userId);
                  

                    if (user != null)
                    {
                        if (image != null && image.ContentLength > 0)
                        {
                            string fileExtension = Path.GetExtension(image.FileName);
                            string[] allowedExtensions = { ".png", ".jpg", ".jpeg", ".gif" };

                            if (allowedExtensions.Contains(fileExtension))
                            {


                                if (!string.IsNullOrEmpty(user.Avatar))
                                {
                                    string fullPath = Path.Combine(Server.MapPath($"~/images/img_user"), user.Avatar);
                                    if (System.IO.File.Exists(fullPath))
                                    {
                                        System.IO.File.Delete(fullPath);
                                    }
                                }


                                string fileName = Guid.NewGuid().ToString() + fileExtension;
                                string urlImage = Path.Combine(Server.MapPath($"~/images/img_user"), fileName);
                                image.SaveAs(urlImage);

                                user.Avatar = fileName;
                                var result = userManager.Update(user);
                                if (result.Succeeded)
                                {
                                    TempData["SuccessMessage"] = "Avatar changed successfully!";
                                    return RedirectToAction("MyAcc");
                                }
                                else
                                {
                                    ModelState.AddModelError("", "Failed to update user avatar.");
                                }
                            }
                            ModelState.AddModelError("", "Vui lòng chọn ảnh có đuôi | .png | .jpg | .jpeg | .gif");
                        }


                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while changing the avatar.");
                }
            }
           
           
          
            return View("MyAcc");
        }


    }

}