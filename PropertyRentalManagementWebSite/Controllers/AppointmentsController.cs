﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PropertyRentalManagementWebSite.Models;

namespace PropertyRentalManagementWebSite.Controllers
{
    public class AppointmentsController : Controller
    {
        private PropertyRentalManagementDBEntities db = new PropertyRentalManagementDBEntities();

        // GET: Appointments
        public ActionResult Index()
        {
            if (Session["UserRole"] as string != "Manager")
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
            var appointments = db.Appointments.Include(a => a.User).Include(a => a.User1);
            return View(appointments.ToList());
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
            // Find the UserId based on the UserName in the database
            var user = db.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);

            if (user == null)
            {
                // Handle the case where the user is not found - perhaps redirect to an error page or log out
                return RedirectToAction("Error"); // Replace with actual error handling
            }

            var appointment = new Appointment
            {
                // If user is found, it is safe to use the Value property because user.UserId will not be null
                Sender = user.UserId
            };

            // Only provide a list of potential Receivers for the dropdown
            ViewBag.Receiver = new SelectList(db.Users, "UserId", "UserName");

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
    }
}
