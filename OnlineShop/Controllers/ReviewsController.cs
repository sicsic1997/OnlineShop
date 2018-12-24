using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using OnlineShop.Models;

namespace OnlineShop.Controllers
{
    public class ReviewsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Reviews
        public ActionResult Index(int id)
        {
            var reviews = 
                db.Reviews
                    .Include(r => r.Product)
                    .Where(item => item.ProductId == id);

            ViewBag.Product = db.Products.Find(id);
            ViewBag.LoggedUserId = User.Identity.GetUserId();
            ViewBag.HasAdministratorRole = User.IsInRole("Administrator");

            var currentUserRights = db.UserRights.Find(User.Identity.GetUserId());
            ViewBag.canAddReview = currentUserRights == null ? false : currentUserRights.CanReview;

            return View(reviews.ToList());
        }

        // GET: Reviews/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reviews reviews = db.Reviews.Find(id);
            if (reviews == null)
            {
                return HttpNotFound();
            }

            ViewBag.LoggedUserId = User.Identity.GetUserId();
            ViewBag.HasAdministratorRole = User.IsInRole("Administrator");
            return View(reviews);
        }

        // GET: Reviews/Create
        [Authorize(Roles = "Administrator,Collaborator,User")]
        public ActionResult Create(int id)
        {
            ViewBag.Product = db.Products.Find(id);
            return View();
        }

        // POST: Reviews/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Administrator,Collaborator,User")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Rating,Comment,ProductId")] Reviews reviews)
        {
            int id;
            if(Int32.TryParse((string)RouteData.Values["id"], out id)) {
                reviews.ProductId = id;
            }
            reviews.AuthorId = User.Identity.GetUserId();
            if (ModelState.IsValid)
            {
                db.Reviews.Add(reviews);
                var product = db.Products.Find(id);
                product.AverageRating = (product.AverageRating * product.NumberOfReviews + reviews.Rating) / (product.NumberOfReviews + 1);
                product.NumberOfReviews = product.NumberOfReviews + 1;
                db.Entry(product).State = EntityState.Modified;

                db.SaveChanges();
                return RedirectToAction("Index", new { id = id });
            }

            return View(reviews);
        }

        // GET: Reviews/Edit/5
        [Authorize(Roles = "Administrator,Collaborator,User")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reviews reviews = db.Reviews.Find(id);
            if (reviews == null)
            {
                return HttpNotFound();
            }
            return View(reviews);
        }

        // POST: Reviews/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Administrator,Collaborator,User")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Rating,Comment,ProductId,AuthorId")] Reviews reviews)
        {
            if (ModelState.IsValid)
            {
                float oldRate = db.Reviews.Find(reviews.Id).Rating;
                db.Entry(reviews).State = EntityState.Modified;
                var product = db.Products.Find(reviews.ProductId);
                product.AverageRating = (product.AverageRating * product.NumberOfReviews - oldRate + reviews.Rating) / (product.NumberOfReviews);
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { id = reviews.ProductId });
            }
            return View(reviews);
        }

        // GET: Reviews/Delete/5
        [Authorize(Roles = "Administrator,Collaborator,User")]
        public ActionResult Delete(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reviews reviews = db.Reviews.Find(id);
            if (reviews == null)
            {
                return HttpNotFound();
            }
            return View(reviews);
        }

        // POST: Reviews/Delete/5
        [Authorize(Roles = "Administrator,Collaborator,User")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Reviews reviews = db.Reviews.Find(id);

            var product = db.Products.Find(reviews.ProductId);
            product.AverageRating = (product.AverageRating * product.NumberOfReviews - reviews.Rating) / (product.NumberOfReviews - 1);
            product.NumberOfReviews = product.NumberOfReviews - 1;
            db.Entry(product).State = EntityState.Modified;

            db.Reviews.Remove(reviews);
            db.SaveChanges();
            return RedirectToAction("Index", new { id = reviews.ProductId });
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
