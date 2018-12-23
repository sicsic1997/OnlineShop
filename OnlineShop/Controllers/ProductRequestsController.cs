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
    public class ProductRequestsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ProductRequests
        public ActionResult Index()
        {
            var productRequestsList = db.ProductRequests
                .Include(p => p.Product)
                .ToList();

            if (User.IsInRole("Collaborator"))
            {
                productRequestsList = productRequestsList
                    .Where(item => item.Product.AuthorId == User.Identity.GetUserId())
                    .ToList();
            }

            ViewBag.hasAdministratorRole = User.IsInRole("Administrator");

            return View(productRequestsList);
        }

        // GET: ProductRequests/Create
        public ActionResult Create()
        {
            var loggedInUserId = User.Identity.GetUserId();

            var productList = db.Products
                .Where(item => item.IsApproved == false && item.AuthorId == loggedInUserId);

            ViewBag.ProductId = new SelectList(productList, "ProductId", "ProductName");
            return View();
        }

        // POST: ProductRequests/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductId,ProductRequestsDescription,IsApproved")] ProductRequests productRequests)
        {
            if (ModelState.IsValid)
            {
                db.ProductRequests.Add(productRequests);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ProductId = new SelectList(db.Products, "ProductId", "ProductName", productRequests.ProductId);
            return View(productRequests);
        }

        // GET: ProductRequests/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductRequests productRequests = db.ProductRequests.Find(id);
            if (productRequests == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProductId = new SelectList(db.Products, "ProductId", "ProductName", productRequests.ProductId);
            return View(productRequests);
        }

        // POST: ProductRequests/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductId,ProductRequestsDescription,IsApproved")] ProductRequests productRequests)
        {
            if (ModelState.IsValid)
            {
                db.Entry(productRequests).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            var loggedInUserId = User.Identity.GetUserId();

            var productList = db.Products
                .Where(item => item.IsApproved == false)
                .Where(item => item.AuthorId == loggedInUserId);
            ViewBag.ProductId = new SelectList(productList, "ProductId", "ProductName", productRequests.ProductId);
            return View(productRequests);
        }

        // GET: ProductRequests/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductRequests productRequests = db.ProductRequests.Find(id);
            if (productRequests == null)
            {
                return HttpNotFound();
            }
            return View(productRequests);
        }

        // POST: ProductRequests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProductRequests productRequests = db.ProductRequests.Find(id);
            db.ProductRequests.Remove(productRequests);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: ProductRequests/Approve/5
        public ActionResult Approve(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductRequests productRequests = db.ProductRequests.Find(id);
            if (productRequests == null)
            {
                return HttpNotFound();
            }
            productRequests.IsApproved = true;
            productRequests.Product.IsApproved = true;
            db.Entry(productRequests).State = EntityState.Modified;
            db.Entry(productRequests.Product).State = EntityState.Modified;
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
