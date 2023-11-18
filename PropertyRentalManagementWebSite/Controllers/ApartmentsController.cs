using System;
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
    public class ApartmentsController : Controller
    {
        private PropertyRentalManagementDBEntities db = new PropertyRentalManagementDBEntities();

        // GET: Apartments
        public ActionResult Index()
        {
            var apartments = db.Apartments.Include(a => a.Building).Include(a => a.User).Include(a => a.User1);
            return View(apartments.ToList());
        }

        // GET: Apartments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Apartment apartment = db.Apartments.Find(id);
            if (apartment == null)
            {
                return HttpNotFound();
            }
            return View(apartment);
        }

        // GET: Apartments/Create
        public ActionResult Create()
        {
            var managers= db.Users.Where(u=>u.UserType.UserRole=="Manager").ToList();
            var Tenants = db.Users.Where(u => u.UserType.UserRole == "Tenant").ToList();
            ViewBag.BuildingId = new SelectList(db.Buildings, "BuildingId", "PostalCode");
            ViewBag.ManagerId = new SelectList(managers, "UserId", "UserName");
            ViewBag.TenantId = new SelectList(Tenants, "UserId", "UserName");
            return View();
        }

        // POST: Apartments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ApartmentId,BuildingId,Type,StatusId,Price,ManagerId,TenantId")] Apartment apartment)
        {
            if (ModelState.IsValid)
            {
                db.Apartments.Add(apartment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            var managers = db.Users.Where(u => u.UserType.UserRole == "Manager").ToList();
            var Tenants = db.Users.Where(u => u.UserType.UserRole == "Tenant").ToList();

            ViewBag.BuildingId = new SelectList(db.Buildings, "BuildingId", "PostalCode", apartment.BuildingId);
            ViewBag.ManagerId = new SelectList(managers, "UserId", "UserName", apartment.ManagerId);
            ViewBag.TenantId = new SelectList(Tenants, "UserId", "UserName", apartment.TenantId);
            return View(apartment);
        }

        // GET: Apartments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Apartment apartment = db.Apartments.Find(id);
            if (apartment == null)
            {
                return HttpNotFound();
            }
            ViewBag.BuildingId = new SelectList(db.Buildings, "BuildingId", "PostalCode", apartment.BuildingId);
            ViewBag.ManagerId = new SelectList(db.Users, "UserId", "UserName", apartment.ManagerId);
            ViewBag.TenantId = new SelectList(db.Users, "UserId", "UserName", apartment.TenantId);
            return View(apartment);
        }

        // POST: Apartments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ApartmentId,BuildingId,Type,StatusId,Price,ManagerId,TenantId")] Apartment apartment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(apartment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BuildingId = new SelectList(db.Buildings, "BuildingId", "PostalCode", apartment.BuildingId);
            ViewBag.ManagerId = new SelectList(db.Users, "UserId", "UserName", apartment.ManagerId);
            ViewBag.TenantId = new SelectList(db.Users, "UserId", "UserName", apartment.TenantId);
            return View(apartment);
        }

        // GET: Apartments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Apartment apartment = db.Apartments.Find(id);
            if (apartment == null)
            {
                return HttpNotFound();
            }
            return View(apartment);
        }

        // POST: Apartments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Apartment apartment = db.Apartments.Find(id);
            db.Apartments.Remove(apartment);
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
