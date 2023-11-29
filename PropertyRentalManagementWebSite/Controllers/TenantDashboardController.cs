using PropertyRentalManagementWebSite.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PropertyRentalManagementWebSite.Controllers
{
   
    public class TenantDashboardController : Controller
    {
        private PropertyRentalManagementDBEntities db = new PropertyRentalManagementDBEntities();
        // GET: TenantDashboard
        public ActionResult Index()
        {
            if (Session["UserRole"] as string == "Tenant")
            {
                var currentUsername = User.Identity.Name;
                var currentUser = db.Users.FirstOrDefault(u => u.UserName == currentUsername);

                if (currentUser == null)
                {

                    return RedirectToAction("Error"); 
                }
                //to check the appointments
                var userId = currentUser.UserId;
                var upcomingAppointments = db.Appointments
                    .Where(a => a.Receiver == userId && a.AppointmentDate >= DateTime.Now && a.Status.Description == "Unread")
                    .OrderBy(a => a.AppointmentDate)
                    .ToList();
                ViewBag.UpcomingAppointments = upcomingAppointments;
                return View();
            }
            return View("UnauthorizedAccess");
        }

        public ActionResult ManageApartments()
        {
            return RedirectToAction("Index", "Apartments");
        }
        public ActionResult ManageAppointments()
        {
            return RedirectToAction("Index", "Appointments");
        }
        public ActionResult SendMessage()
        {
            return RedirectToAction("Index", "Messages");
        }



    }
}