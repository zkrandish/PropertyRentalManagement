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
        public ActionResult Index(int? searchStatus, string searchProvince, string searchCity)
        {
            var userRole = Session["UserRole"] as string;
            
            

            // Start with a query that includes related data
            IQueryable<Apartment> apartmentsQuery = db.Apartments.Include(a => a.Building).Include(a => a.Status).Include(a => a.User).Include(a => a.User1);
            if (userRole != "Manager")
            {
                // If the user is not a manager, restrict the list to certain criteria, e.g., available apartments
                apartmentsQuery = apartmentsQuery.Where(a => a.Status.Description == "Available");
            }
            // Fetch the list of statuses for the dropdown
            ViewBag.Statuses = db.Statuses.Select(s => new SelectListItem
            {
                Value = s.StatusId.ToString(),
                Text = s.Description
            }).ToList();


            // If a status filter is set, adjust the query to filter by status
            if (searchStatus.HasValue)
            {
                apartmentsQuery = apartmentsQuery.Where(a => a.StatusId == searchStatus.Value);
            }
            ViewBag.ProvinceList = new SelectList(db.Buildings.Select(b => b.Province).Distinct().ToList());
            ViewBag.CityList = new SelectList(db.Buildings.Select(b => b.City).Distinct().ToList());
            ViewBag.TypeList = new SelectList(db.Apartments.Select(b => b.Type).Distinct().ToList());
         
            // Filter by province if searchProvince is not null or empty
            if (!String.IsNullOrEmpty(searchProvince))
            {
                apartmentsQuery = apartmentsQuery.Where(a => a.Building.Province == searchProvince);
            }

            // Filter by city if searchCity is not null or empty
            if (!String.IsNullOrEmpty(searchCity))
            {
                apartmentsQuery = apartmentsQuery.Where(a => a.Building.City == searchCity);
            }

            // Execute the query and convert to a list
            var apartmentsList = apartmentsQuery.ToList();
            // Pass the search criteria back to the view to maintain the state of the search form
            ViewBag.SearchProvince = searchProvince;
            ViewBag.SearchCity = searchCity;

            return View(apartmentsList);
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
            if (Session["UserRole"] as string != "Manager")
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
            var managers = db.Users.Where(u=>u.UserType.UserRole=="Manager").ToList();
            var Tenants = db.Users.Where(u => u.UserType.UserRole == "Tenant").ToList();
            var statuses = db.Statuses.ToList(); // Fetch all statuses
            ViewBag.BuildingId = new SelectList(db.Buildings, "BuildingId", "PostalCode");
            ViewBag.ManagerId = new SelectList(managers, "UserId", "UserName");
            ViewBag.TenantId = new SelectList(Tenants, "UserId", "UserName");
            ViewBag.StatusId = new SelectList(statuses, "StatusId", "Description"); 
            return View();
        }

        // POST: Apartments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ApartmentId,BuildingId,Type,StatusId,Price,ManagerId,TenantId")] Apartment apartment)
        {
            if (Session["UserRole"] as string != "Manager")
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
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
            if (Session["UserRole"] as string != "Manager")
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
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
            if (Session["UserRole"] as string != "Manager")
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
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
            if (Session["UserRole"] as string != "Manager")
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
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
            if (Session["UserRole"] as string != "Manager")
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
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
