using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OnlineShop.Models;
using Microsoft.AspNet.Identity;

namespace OnlineShop.Controllers
{
    public class ProductsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Products
        public ActionResult Index(string searchText, int? sortType, string sortOrder)
        {
            var products = db.Products.Include(p => p.ApplicationUser).Include(p => p.Categories);

            searchText = String.IsNullOrEmpty(searchText) ? "" : searchText;

            sortType = sortType.Equals(null) ? 1 : sortType;
            sortOrder = String.IsNullOrEmpty(sortOrder) ? "rating" : sortOrder;

            ViewBag.sortType = sortType;
            ViewBag.searchText = searchText;

            if (User.IsInRole("User"))
            {
                products = products.Where(item => item.IsApproved == true);
            }

            products = products.Where(item => item.ProductName.Contains(searchText));

            switch(sortOrder)
            {
                case "rating":
                    products = sortType == 1 ? 
                        products.OrderBy(s => s.AverageRating) : products.OrderByDescending(s => s.AverageRating);
                    break;
                case "price":
                    products = sortType == 1 ?
                        products.OrderBy(s => s.Price) : products.OrderByDescending(s => s.Price);
                    break;
            }

            ViewBag.HasAdminRole = User.IsInRole("Administrator");
            ViewBag.LoggedUserId = User.Identity.GetUserId();

            return View(products.ToList());
        }

        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Products/Create
        [Authorize(Roles = "Administrator, Collaborator")]
        public  ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoriesId", "Description");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Administrator, Collaborator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductId,ProductName,ProductDescription,MediaUrl,IsApproved,Price,CategoryId")] Product product)
        {
            product.AuthorId = User.Identity.GetUserId();
            product.AverageRating = 0;
            product.NumberOfReviews = 0;

            if(User.IsInRole("Administrator"))
            {
                product.IsApproved = true;
            }

            if (ModelState.IsValid)
            {
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoriesId", "Description", product.CategoryId);
            return View(product);
        }

        // GET: Products/Edit/5
        [Authorize(Roles = "Administrator, Collaborator")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.HasAdminRole = User.IsInRole("Administrator");
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoriesId", "Description", product.CategoryId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Administrator, Collaborator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductId,ProductName,ProductDescription,MediaUrl,IsApproved,Price,CategoryId,AuthorId")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AuthorId = new SelectList(db.Users, "Id", "Email", product.AuthorId);
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoriesId", "Description", product.CategoryId);
            return View(product);
        }

        // GET: Products/Delete/5
        [Authorize(Roles = "Administrator, Collaborator")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [Authorize(Roles = "Administrator, Collaborator")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "User,Collaborator,Administrator")]
        public ActionResult AddToCartCookie(int id)
        {
            // Encoded like id1|id2|id3
            if (Request.Cookies["CartCookie"] == null)
            {
                Response.Cookies["CartCookie"].Value = id.ToString();
            }
            else
            {
                // First check if exists
                string objCartListString = Request.Cookies["CartCookie"].Value.ToString();
                string[] objCartListStringSplit = objCartListString.Split('|');
                foreach (string s in objCartListStringSplit)
                {
                    if(s == "")
                    {
                        continue;
                    }

                    int currentProductId = int.Parse(s);
                    if (currentProductId == id)
                    {
                        return RedirectToAction("Index");
                    }
                }
                Response.Cookies["CartCookie"].Value = Request.Cookies["CartCookie"].Value + "|" + id.ToString();
            }

            Response.Cookies["CartCookie"].Expires = DateTime.Now.AddYears(30);
            return RedirectToAction("Index"); ;
        }

        [Authorize(Roles = "User,Collaborator,Administrator")]
        public ActionResult RemoveFromCartCookie(int id)
        {
            if (Request.Cookies["CartCookie"] == null)
            {
                return RedirectToAction("Index"); ;
            }
            else
            {
                // First check if exists
                string objCartListString = Request.Cookies["CartCookie"].Value.ToString();
                string[] objCartListStringSplit = objCartListString.Split('|');
                string newCookie = "";
                foreach (string s in objCartListStringSplit)
                {
                    if(s == "")
                    {
                        continue;
                    }
                    int currentProductId = int.Parse(s);
                    if (currentProductId == id)
                    {
                        continue;
                    }
                    if (newCookie != "")
                        newCookie = newCookie + "|";
                    newCookie  = newCookie + currentProductId.ToString();
                }
                Response.Cookies["CartCookie"].Value = newCookie;
            }

            Response.Cookies["CartCookie"].Expires = DateTime.Now.AddYears(30);
            return RedirectToAction("Index"); ;
        }

        /// GET
        [Authorize(Roles = "User,Collaborator,Administrator")]
        public ActionResult ViewCart()
        {
            List<Product> productList = new List<Product>();
            if(Request.Cookies["CartCookie"] != null && Request.Cookies["CartCookie"].Value != null)
            {
                var productListFromCookies = Request.Cookies["CartCookie"].Value;
                string[] objCartListStringSplit = productListFromCookies.Split('|');
                foreach (string s in objCartListStringSplit)
                {
                    if (s == "")
                    {
                        continue;
                    }
                    int currentProductId = int.Parse(s);
                    productList.Add(db.Products.Find(currentProductId));
                }
            }

            var currentUserRights = db.UserRights.Find(User.Identity.GetUserId());
            if(currentUserRights == null)
            {
                //It's manager or collaborator autocreated
                ViewBag.HasRolesToBuy = true;
            }
            else
            {
                ViewBag.HasRolesToBuy = currentUserRights.CanBuy;
            }

            return View(productList);
        }

        [Authorize(Roles = "User,Collaborator,Administrator")]
        public ActionResult BuyFromCart()
        {
            Response.Cookies["CartCookie"].Value = null;
            return View();
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
