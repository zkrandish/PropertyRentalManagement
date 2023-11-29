using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using PropertyRentalManagementWebSite.Models;

namespace PropertyRentalManagementWebSite.Controllers
{
    public class UsersController : Controller
    {
        private PropertyRentalManagementDBEntities db = new PropertyRentalManagementDBEntities();

        // GET: Users
        public ActionResult Index()
        {
            var users = db.Users.Include(u => u.Status).Include(u => u.UserType);
            return View(users.ToList());
        }

        // GET: Users/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            ViewBag.StatusId = new SelectList(db.Statuses, "StatusId", "Description");
            ViewBag.UserTypeId = new SelectList(db.UserTypes, "UserTypeId", "UserRole");
            var ownerRole = "Owner"; // Define the role name for owners
            var managerTypeId = db.UserTypes.FirstOrDefault(ut => ut.UserRole == "Manager")?.UserTypeId; // Get the ID for the manager role
            var userRole = Session["UserRole"] as string;

            if (userRole == ownerRole && managerTypeId.HasValue)
            {
                ViewBag.UserTypeId = managerTypeId.Value; // Set the manager type ID as an integer
                ViewBag.IsOwner = true;
            }
            else
            {
                ViewBag.UserTypeId = new SelectList(db.UserTypes, "UserTypeId", "UserRole");
                ViewBag.IsOwner = false;
            }

            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserId,FirstName,LastName,PasswordString,StatusId,UserTypeId,UserName,Email")] User user)
        {
            if (db.Users.Any(u => u.UserName == user.UserName))
            {
                ModelState.AddModelError("Username", "Username is already taken.");
            }

            if (db.Users.Any(u => u.Email == user.Email))
            {
                ModelState.AddModelError("Email", "Email is already in use.");
            }
            if (ModelState.IsValid)
            {
               
                var passwordHash = HashPassword(user.PasswordString);
                user.Password = passwordHash;
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
               
            }
            ModelState.Remove("Password");

            ViewBag.StatusId = new SelectList(db.Statuses, "StatusId", "Description", user.StatusId);
            ViewBag.UserTypeId = new SelectList(db.UserTypes, "UserTypeId", "UserRole", user.UserTypeId);
            return View(user);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.StatusId = new SelectList(db.Statuses, "StatusId", "Description", user.StatusId);
            ViewBag.UserTypeId = new SelectList(db.UserTypes, "UserTypeId", "UserRole", user.UserTypeId);
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "UserId,FirstName,LastName,PasswordString,StatusId,UserTypeId,UserName,Email")] User user)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var passwordHash = HashPassword(user.PasswordString);
        //        user.Password = passwordHash;
        //        db.Entry(user).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.StatusId = new SelectList(db.Statuses, "StatusId", "Description", user.StatusId);
        //    ViewBag.UserTypeId = new SelectList(db.UserTypes, "UserTypeId", "UserRole", user.UserTypeId);
        //    return View(user);
        //}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserId,FirstName,LastName,PasswordString,StatusId,UserTypeId,UserName,Email")] User user)
        {
            if (ModelState.IsValid)
            {
                // Get the existing user from the database
                var existingUser = db.Users.Find(user.UserId);
                if (existingUser == null)
                {
                    return HttpNotFound();
                }

                // Update the user properties, but don't change the password if PasswordString is empty
                existingUser.FirstName = user.FirstName;
                existingUser.LastName = user.LastName;
                existingUser.StatusId = user.StatusId;
                existingUser.UserTypeId = user.UserTypeId;
                existingUser.UserName = user.UserName;
                existingUser.Email = user.Email;

                // Only update the password if a new one was provided
                if (!string.IsNullOrWhiteSpace(user.PasswordString))
                {
                    var passwordHash = HashPassword(user.PasswordString);
                    existingUser.Password = passwordHash;
                }

                // Save the changes
                db.Entry(existingUser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.StatusId = new SelectList(db.Statuses, "StatusId", "Description", user.StatusId);
            ViewBag.UserTypeId = new SelectList(db.UserTypes, "UserTypeId", "UserRole", user.UserTypeId);
            return View(user);
        }


        // GET: Users/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
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

