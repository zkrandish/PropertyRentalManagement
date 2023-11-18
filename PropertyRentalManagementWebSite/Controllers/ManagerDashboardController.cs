using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PropertyRentalManagementWebSite.Controllers
{
    
    public class ManagerDashboardController : Controller
    {
        // GET: ManagerDashboard
        public ActionResult Index()
        {
            if (Session["UserRole"] as string == "Manager")
            {
                return View();

            }
            return View("UnauthorizedAccess");
        }
        public ActionResult ManageBuildings()
        {
            return RedirectToAction("Index", "Buildings");
        }
        public ActionResult ManageApartments()
        {
            return RedirectToAction("Index", "Apartments");
        }
        public ActionResult ManageAppointments()
        {
            return RedirectToAction("Index", "Appointments");
        }
    }
}