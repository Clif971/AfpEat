using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AfpEat;

namespace AfpEat.Controllers
{
    public class UtilisateursController : Controller
    {
        private AfpEatEntities db = new AfpEatEntities();

        // GET: Utilisateurs
        public ActionResult Index()
        {
            return View(db.Utilisateurs.ToList());
        }

        public ActionResult ValidationCommande(int? id)
        {
            Utilisateur utilisateur = db.Utilisateurs.Find(id);
            Session["Utilisateur"] = utilisateur;

            return View(db.Utilisateurs.ToList());
        }

        public ActionResult RecapCommande()
        {
            return View();
        }

        // GET: Utilisateurs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Utilisateur utilisateur = db.Utilisateurs.Find(id);
            if (utilisateur == null)
            {
                return HttpNotFound();
            }
            return View(utilisateur);
        }

        // GET: Utilisateurs/Create
        public ActionResult Create()
        {
            ViewBag.IdAllergene = new SelectList(db.Allergenes, "IdAllergene", "Nom");
            ViewBag.IdCategorie = new SelectList(db.Categories, "IdCategorie", "Nom");
            ViewBag.IdRestaurant = new SelectList(db.Restaurants, "IdRestaurant", "Nom");
            return View();
        }
        
        // GET: Utilisateurs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Utilisateur utilisateur = db.Utilisateurs.Find(id);
            if (utilisateur == null)
            {
                return HttpNotFound();
            }
            return View(utilisateur);
        }

        // POST: Utilisateurs/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdUtilisateur,Nom,Prenom,Matricule,Password,Statut,Solde,IdSession")] Utilisateur utilisateur)
        {
            if (ModelState.IsValid)
            {
                db.Entry(utilisateur).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(utilisateur);
        }

        // GET: Utilisateurs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Utilisateur utilisateur = db.Utilisateurs.Find(id);
            if (utilisateur == null)
            {
                return HttpNotFound();
            }
            return View(utilisateur);
        }

        // POST: Utilisateurs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Utilisateur utilisateur = db.Utilisateurs.Find(id);
            db.Utilisateurs.Remove(utilisateur);
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

        // GET: Utilisateurs/Connexion
        public ActionResult Connection()
        {
            return View();
        }
        // POST: Utilisateurs/Connexion
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Connection([Bind(Include = "Matricule,Password")] Utilisateur utilisateur)
        {
            if (ModelState.IsValid)
            {
                Utilisateur user = db.Utilisateurs.FirstOrDefault(u => u.Matricule == utilisateur.Matricule && u.Password == utilisateur.Password);
                if (user != null)
                {
                    user.IdSession = Session.SessionID;
                    db.SaveChanges();
                    Session["Utilisateur"] = user;
                    return RedirectToAction("Accueil", "Home");
                }
                else
                {
                    ViewBag.Erreur = "Erreur de connexion !";
                }
            }
            return View(utilisateur);
        }
    }
}
