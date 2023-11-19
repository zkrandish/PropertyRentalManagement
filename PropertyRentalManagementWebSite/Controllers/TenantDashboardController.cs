using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PropertyRentalManagementWebSite.Controllers
{
    public class TenantDashboardController : Controller
    {
        // GET: TenantDashboard
        public ActionResult Index()
        {
            if (Session["UserRole"] as string == "Tenant")
            {
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
    }
}