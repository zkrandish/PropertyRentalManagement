using PropertyRentalManagementWebSite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PropertyRentalManagementWebSite.Controllers
{
    public class OwnerDashboardController : Controller
    {
        private PropertyRentalManagementDBEntities db = new PropertyRentalManagementDBEntities();
        // GET: OwnerDashboard
        public ActionResult Index()
        {
            if (Session["UserRole"] as string == "Owner")
            {
                var currentUsername = User.Identity.Name;
                var currentUser = db.Users.FirstOrDefault(u => u.UserName == currentUsername);

                if (currentUser == null)
                {

                    return RedirectToAction("Error");
                }
            }
            return View();
        }
        public ActionResult ManageUsers()
        {
            return RedirectToAction("Index", "Users");
        }
        public ActionResult CreateManager()
        {
            var userTypes = db.UserTypes.Where(ut => ut.UserRole == "Manager").ToList();
            ViewBag.UserTypeId = new SelectList(userTypes, "UserTypeId", "UserRole");
            return View();
        }

    }
}