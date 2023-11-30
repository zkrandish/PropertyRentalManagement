﻿using PropertyRentalManagementWebSite.Models;
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

        
            var hashedPassword = HashPassword(model.Password);
            bool userExist = db.Users.Any(x => x.UserName == model.Username && x.Password == hashedPassword);
            User user = db.Users.FirstOrDefault(x => x.UserName == model.Username && x.Password == hashedPassword);
            if (userExist)
            {
                // Check if the user's status is 'Pending'
                var pendingStatus = db.Statuses.FirstOrDefault(s => s.Description.Equals("Pending", StringComparison.OrdinalIgnoreCase));
                if (user.StatusId == pendingStatus.StatusId)
                {
                 
                    ModelState.AddModelError("", "Your account is pending approval. Please wait until a manager activates your account.");
                    return View(model);
                }
                var userRole = user.UserType.UserRole;
                FormsAuthentication.SetAuthCookie(user.UserName, false);
                Session["UserRole"] = userRole;
                if(userRole == "Manager")
                {
                    return RedirectToAction("Index", "ManagerDashboard");
                }
                if (userRole == "Tenant")
                {
                    return RedirectToAction("Index", "TenantDashboard");
                }
                if (userRole == "Owner")
                {
                    return RedirectToAction("Index", "OwnerDashboard");
                }

                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError("", "UserName or password is wrong");
            return View();

        }
        public ActionResult Signout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
        public ActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignUp(SignUpViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Check if username/email already exists
                    if (db.Users.Any(u => u.UserName == model.Username) || db.Users.Any(u => u.Email == model.Email))
                    {
                        ModelState.AddModelError("", "Username or Email already exists.");
                        return View(model);
                    }
                    var pendingStatus = db.Statuses.FirstOrDefault(s => s.Description.Equals("Pending", StringComparison.OrdinalIgnoreCase));

                    var user = new User
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Password = HashPassword(model.Password),
                        StatusId = pendingStatus.StatusId,
                        UserTypeId = db.UserTypes.FirstOrDefault(ut => ut.UserRole == "Tenant").UserTypeId,
                        UserName = model.Username,
                        Email = model.Email,
                    };

                    db.Users.Add(user);
                    db.SaveChanges();

                    // Log the user in or redirect to a confirmation page
                    //FormsAuthentication.SetAuthCookie(model.Username, false);
                    return RedirectToAction("Login", "Account");
                }catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
                }
                


            // If we got this far, something failed, redisplay form
            return View(model);
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
