using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AfpEat;
using AfpEat.Models;

namespace AfpEat.Controllers
{
    public class RestaurantsController : Controller
    {
        private AfpEatEntities db = new AfpEatEntities();

        // GET: Restaurants
        public ActionResult Index()
        {
            var restaurants = db.Restaurants.Include(r => r.TypeCuisine);
            return View(restaurants.ToList());
        }
        public ActionResult ListeRestaurants()
        {
            var restaurants = db.Restaurants;
            return View(restaurants.ToList());
        }
        public ActionResult TypeCuisine()
        {
            var typecuisines = db.TypeCuisines;
            return View(typecuisines.ToList());
        }

        // GET: Restaurants/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Restaurant restaurant = db.Restaurants.Include(r => r.Produits)
                                                     .Include(r => r.Menus)
                                                     .Where(r => r.IdRestaurant == id).First;
            if (restaurant == null)
            {
                return HttpNotFound();
            }

            ViewBag.MesProduits = db.Produits.Where(p => p.IdRestaurant == id).ToList();

            return View(restaurant);
        }
        //ADD PRODUIT
        public JsonResult AddProduit(int IdProduit, string idsession)
        {
            SessionUtilisateur sessionUtilisateur = db.SessionUtilisateurs.Find(Session.SessionID);
            List<ProduitPanier> produitPaniers = null;
            if (sessionUtilisateur != null)
            {
                if (HttpContext.Application[idsession] != null)
                {
                    produitPaniers = (List<ProduitPanier>)HttpContext.Application[idsession];
                }
                else
                {
                    produitPaniers = new List<ProduitPanier>();
                }
                Produit produit = db.Produits.Find(IdProduit);
                ProduitPanier produitPanier = new ProduitPanier();
                produitPanier.IdProduit = IdProduit;
                produitPanier.Nom = produit.Nom;
                produitPanier.Description = produit.Description;
                produitPanier.Quantite = 1;
                produitPanier.Prix = produit.Prix;
                produitPanier.Photo = produit.Photos.First().Nom;
                produitPaniers.Add(produitPanier);
                HttpContext.Application[idsession] = produitPaniers;
            }
            return Json(produitPaniers.Count, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProduits(string idsession)
        {
            SessionUtilisateur sessionUtilisateur = db.SessionUtilisateurs.Find(Session.SessionID);
            List<ProduitPanier> panier = null;
            if (sessionUtilisateur != null)
            {
                if (HttpContext.Application[idsession] != null)
                {
                    panier = (List<ProduitPanier>)HttpContext.Application[idsession];
                }
            }
            return Json(panier, JsonRequestBehavior.AllowGet);
        }


        // GET: Restaurants/Create
        public ActionResult Create()
        {
            ViewBag.IdTypeCuisine = new SelectList(db.TypeCuisines, "IdTypeCuisine", "Specialite");
            return View();
        }

        // POST: Restaurants/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdRestaurant,IdTypeCuisine,Nom,Responsable,Description,Tag,Budget,Adresse,Telephone,Mobile,Email,CodePostal,Ville")] Restaurant restaurant)
        {
            if (ModelState.IsValid)
            {
                db.Restaurants.Add(restaurant);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdTypeCuisine = new SelectList(db.TypeCuisines, "IdTypeCuisine", "Specialite", restaurant.IdTypeCuisine);
            return View(restaurant);
        }

        // GET: Restaurants/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Restaurant restaurant = db.Restaurants.Find(id);
            if (restaurant == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdTypeCuisine = new SelectList(db.TypeCuisines, "IdTypeCuisine", "Specialite", restaurant.IdTypeCuisine);
            return View(restaurant);
        }

        // POST: Restaurants/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdRestaurant,IdTypeCuisine,Nom,Responsable,Description,Tag,Budget,Adresse,Telephone,Mobile,Email,CodePostal,Ville")] Restaurant restaurant)
        {
            if (ModelState.IsValid)
            {
                db.Entry(restaurant).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdTypeCuisine = new SelectList(db.TypeCuisines, "IdTypeCuisine", "Specialite", restaurant.IdTypeCuisine);
            return View(restaurant);
        }

        // GET: Restaurants/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Restaurant restaurant = db.Restaurants.Find(id);
            if (restaurant == null)
            {
                return HttpNotFound();
            }
            return View(restaurant);
        }

        // POST: Restaurants/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Restaurant restaurant = db.Restaurants.Find(id);
            db.Restaurants.Remove(restaurant);
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
