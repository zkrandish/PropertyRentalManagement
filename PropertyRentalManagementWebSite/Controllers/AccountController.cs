using PropertyRentalManagementWebSite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using PropertyRentalManagementWebSite.ViewModels;
using System.Net;
using System.Web.UI.WebControls;

namespace PropertyRentalManagementWebSite.Controllers
{
    public class AccountController : Controller
    {
        private PropertyRentalManagementDBEntities db = new PropertyRentalManagementDBEntities();
        // GET: Account
        //[HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
           
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            
            var hashedPassword = HashPassword(model.Password);

            
            var user = db.Users.FirstOrDefault(u => u.UserId == model.UserId && u.Password == hashedPassword);
            if (user != null)
            {
                // Check the role of the user and set the session or authentication cookie accordingly
                var userRole = user.UserType.UserRole;

                FormsAuthentication.SetAuthCookie(user.UserId.ToString(), false);

                Session["UserRole"] = userRole;
              
                Session["FirstName"] = user.FirstName;

                return RedirectToAction("Index", "Home"); 
            }
            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "Invalid username or password.");
            return View(model);

        }
        public ActionResult Signout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
        private byte[] HashPassword(string password)
        {
            // Use a proper hashing algorithm with a salt
            using (var sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
}
