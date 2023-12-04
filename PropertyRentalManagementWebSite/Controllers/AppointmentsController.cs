using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using PropertyRentalManagementWebSite.Models;

namespace PropertyRentalManagementWebSite.Controllers
{
    public class AppointmentsController : Controller
    {
        private PropertyRentalManagementDBEntities db = new PropertyRentalManagementDBEntities();

        // GET: Appointments
        public ActionResult Index()
        {
            if (Session["UserRole"] as string != "Manager" && Session["UserRole"] as string != "Tenant")
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
            var currentUser = db.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
            var currentUserId = currentUser?.UserId;
            string userRole = Session["UserRole"] as string;

            if (userRole != "Manager" && userRole != "Tenant")
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            IQueryable<Appointment> appointmentsQuery = db.Appointments.Where(a => false); ;

            if (userRole == "Manager")
            {
                // If the user is a manager, they can see all appointments or specific ones based on their needs
                appointmentsQuery = db.Appointments.Include(a => a.User).Include(a => a.User1);
                // Additional logic for managers, if any
            }
            else if (userRole == "Tenant")
            {
                
                // If the user is a tenant, they only see appointments related to them
                 currentUserId = currentUser.UserId;
                appointmentsQuery = db.Appointments.Where(a => a.Receiver == currentUserId || a.Sender == currentUserId)
                                                   .Include(a => a.User).Include(a => a.User1);
                // Additional logic for tenants, if any
            }

            var appointmentsList = appointmentsQuery.ToList();
            ViewBag.CurrentUserId = currentUserId;
            return View(appointmentsList);
            
        }

        // GET: Appointments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Appointment appointment = db.Appointments.Find(id);
            if (appointment == null)
            {
                return HttpNotFound();
            }
            return View(appointment);
        }

        // GET: Appointments/Create
        public ActionResult Create()
        {
            if (Session["UserRole"] as string != "Manager" && Session["UserRole"] as string != "Tenant")
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
            // Find the UserId based on the UserName in the database
            var currentUser = db.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);

            if (currentUser == null)
            {
                // Handle the case where the user is not found - perhaps redirect to an error page or log out
                return RedirectToAction("Error"); 
            }

            var appointment = new Appointment
            {
                // If user is found, it is safe to use the Value property because user.UserId will not be null
                Sender = currentUser.UserId
            };
            if (currentUser.UserType.UserRole == "Manager")
            {
                var tenants = db.Users.Where(u => u.UserType.UserRole == "Tenant").ToList();
                ViewBag.Receiver = new SelectList(tenants, "UserId", "UserName");
            }
            else if (currentUser.UserType.UserRole == "Tenant")
            {
                var managers = db.Users.Where(u => u.UserType.UserRole == "Manager").ToList();
                ViewBag.Receiver = new SelectList(managers, "UserId", "UserName");
            }
            // Prepare the default status
            var defaultStatus = db.Statuses.FirstOrDefault(s => s.Description == "Unread");
            if (defaultStatus != null)
            {
                appointment.StatusId = defaultStatus.StatusId; // Assign the default status ID for new messages
            }
            else
            {
                // Handle the case where the default status is not found
                // You may want to log this error or handle it as per your business logic
                ModelState.AddModelError("", "Default status not found.");
                return View(appointment);
            }


            // Pass the appointment object to the view, which has the Sender set
            return View(appointment);
        }

        // POST: Appointments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AppointmentId,Receiver,Sender,AppointmentDate,from,to,Purpose")] Appointment appointment)
        {
            // Get the current user again to ensure we're setting the current logged-in user as the sender
            var currentUser = db.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);

            if (currentUser == null)
            {
                // Handle the case where the user is not found - perhaps redirect to an error page or log out
                return RedirectToAction("Error"); // Replace with actual error handling
            }
            if (ModelState.IsValid)
            {
                // Set the sender from the current logged-in user
                appointment.Sender = currentUser.UserId;
                // Ensure the StatusId is set for the new message
                var defaultStatus = db.Statuses.FirstOrDefault(s => s.Description == "Unread");
                if (defaultStatus != null)
                {
                    appointment.StatusId = defaultStatus.StatusId;
                }
                else
                {
                    // Handle the case where the default status is not found
                    ModelState.AddModelError("", "Default status not found.");
                    return View(appointment);
                }

                db.Appointments.Add(appointment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Receiver = new SelectList(db.Users, "UserId", "UserName", appointment.Receiver);
            //ViewBag.Sender = new SelectList(db.Users, "UserId", "UserName", appointment.Sender);
            return View(appointment);
        }

        // GET: Appointments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Appointment appointment = db.Appointments.Find(id);
            if (appointment == null)
            {
                return HttpNotFound();
            }
            ViewBag.Receiver = new SelectList(db.Users, "UserId", "UserName", appointment.Receiver);
            ViewBag.Sender = new SelectList(db.Users, "UserId", "UserName", appointment.Sender);
            return View(appointment);
        }

        // POST: Appointments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AppointmentId,Receiver,Sender,AppointmentDate,from,to,Purpose")] Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(appointment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Receiver = new SelectList(db.Users, "UserId", "UserName", appointment.Receiver);
            ViewBag.Sender = new SelectList(db.Users, "UserId", "UserName", appointment.Sender);
            return View(appointment);
        }

        // GET: Appointments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Appointment appointment = db.Appointments.Find(id);
            if (appointment == null)
            {
                return HttpNotFound();
            }
            return View(appointment);
        }

        // POST: Appointments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Appointment appointment = db.Appointments.Find(id);
            db.Appointments.Remove(appointment);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MarkAsRead(int appointmentId)
        {
            var currentUser = db.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
            if (currentUser == null)
            {
                return RedirectToAction("Error");
            }

            var appointment = db.Appointments.Find(appointmentId);
            if (appointment == null || appointment.Receiver != currentUser.UserId)
            {
                return RedirectToAction("UnauthorizedAccess");
            }

            var readStatus = db.Statuses.FirstOrDefault(s => s.Description == "Read");
            if (readStatus != null)
            {
                appointment.StatusId = readStatus.StatusId;
                db.Entry(appointment).State = EntityState.Modified;
                db.SaveChanges();
            }
            TempData["SuccessMessage"] = "Appointment has been marked as read.";
            return RedirectToAction("Index"); // Redirect back to the dashboard
        }
    }
}
