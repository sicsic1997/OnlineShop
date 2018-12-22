using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OnlineShop.Models;

namespace OnlineShop.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class UserRightsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: UserRights
        public ActionResult Index()
        {
            var userRights = db.UserRights.Include(u => u.ApplicationUser);
            return View(userRights.ToList());
        }

        // GET: UserRights/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserRights userRights = db.UserRights.Find(id);
            if (userRights == null)
            {
                return HttpNotFound();
            }
            return View(userRights);
        }

        // GET: UserRights/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserRights userRights = db.UserRights.Find(id);
            if (userRights == null)
            {
                return HttpNotFound(); 
            }
            ViewBag.UserId = new SelectList(db.Users, "Id", "Email", userRights.UserId);
            return View(userRights);
        }

        // POST: UserRights/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserId,CanBuy,CanReview")] UserRights userRights)
        {
            if (ModelState.IsValid)
            {
                db.Entry(userRights).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserId = new SelectList(db.Users, "Id", "Email", userRights.UserId);
            return View(userRights);
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
