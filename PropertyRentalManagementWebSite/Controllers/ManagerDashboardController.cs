﻿using PropertyRentalManagementWebSite.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PropertyRentalManagementWebSite.Controllers
{
    
    public class ManagerDashboardController : Controller
    {
        private PropertyRentalManagementDBEntities db = new PropertyRentalManagementDBEntities(); 
        // GET: ManagerDashboard
        public ActionResult Index()
        {
            if (Session["UserRole"] as string == "Manager")
            {
                var currentUsername = User.Identity.Name;
                var currentUser = db.Users.FirstOrDefault(u => u.UserName == currentUsername);

                if (currentUser == null)
                {

                    return RedirectToAction("Error");
                }
                //to check the appointments
                var userId = currentUser.UserId;
                var unreadAppointments = db.Appointments
                    .Where(a => a.Receiver == userId && a.AppointmentDate >= DateTime.Now&& a.Status.Description=="UnRead")
                    .OrderBy(a => a.AppointmentDate)
                    .ToList();
                ViewBag.UpcomingAppointments = unreadAppointments;

                int pendingApprovalsCount = db.Users.Count(u => u.Status.Description == "Pending");
                ViewBag.PendingApprovalsCount = pendingApprovalsCount;
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
      
        public ActionResult PendingTenantApprovals()
        {
            if (Session["UserRole"] as string != "Manager")
            {
                
                return RedirectToAction("UnauthorizedAccess");
            }
            var pendingUsers = db.Users.Where(u => u.Status.Description == "Pending").ToList();
            return View(pendingUsers); // Pass the list to the view
        }
        [HttpPost]
        [ValidateAntiForgeryToken] 
        public ActionResult ApproveTenant(int id)
        {
            var userToApprove = db.Users.Find(id);
            if (userToApprove != null && userToApprove.Status.Description == "Pending")
            {
                var activeStatus = db.Statuses.FirstOrDefault(s => s.Description == "Active");
                if (activeStatus != null)
                {
                    userToApprove.StatusId = activeStatus.StatusId;
                    db.Entry(userToApprove).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    
                }
            }
            return RedirectToAction("PendingTenantApprovals"); // Redirect back to the list
        }
        
    }
}