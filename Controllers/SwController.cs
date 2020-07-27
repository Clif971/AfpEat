using AfpEat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;

namespace AfpEat.Controllers
{
    public class SwController : Controller
    {
        private AfpEatEntities db = new AfpEatEntities();
        // GET: Sw
        public ActionResult Index()
        {
            return View();
        }

        //JSON ADD PRODUIT
        public JsonResult AddProduit(int IdProduit, string idsession)
        {
            SessionUtilisateur sessionUtilisateur = db.SessionUtilisateurs.Find(Session.SessionID);
            PanierModel panier = GetPanier(idsession);
            bool isReturnOk = true;
            if (sessionUtilisateur != null && panier != null && IdProduit > 0)
            {
                ProduitPanier produitPanier = FindProduit(IdProduit);
                if (produitPanier != null)
                {
                    if (panier.IdRestaurant == 0)
                    {
                        panier.IdRestaurant = produitPanier.IdRestaurant;
                        panier.AddItem(produitPanier);
                    }
                    else if (panier.IdRestaurant == produitPanier.IdRestaurant)
                    {
                        panier.AddItem(produitPanier);
                        isReturnOk = false;                
                    }
                }//Je sauve mon panier en session
                HttpContext.Application[idsession] = panier;
            }// Je retourne en Json le nombre de ligne
            return Json(new { returnOk = isReturnOk, qte = panier.Quantite, montant=panier.Montant }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProduits(string idsession)
        {
            SessionUtilisateur sessionUtilisateur = db.SessionUtilisateurs.Find(Session.SessionID);
            List<ItemPanier> panier = null;
            if (sessionUtilisateur != null)
            {
                if (HttpContext.Application[idsession] != null)
                {
                    panier = (List<ItemPanier>)HttpContext.Application[idsession];
                }
            }
            return Json(panier, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveCommande(string idsession)
        {
            SessionUtilisateur sessionUtilisateur = db.SessionUtilisateurs.Find(Session.SessionID);
            PanierModel panier = null;
            if (sessionUtilisateur != null && HttpContext.Application[idsession] != null)
            {
                panier = (PanierModel)HttpContext.Application[idsession];
            }
            try
            {
                Utilisateur utilisateur = db.Utilisateurs.First(p => p.IdSession == idsession);
                if (utilisateur != null && utilisateur.Solde > 0 && panier != null && panier.Count > 0)
                {
                    decimal prixTotal = 0;                    
                    prixTotal = panier.Montant;
                    if (prixTotal <= utilisateur.Solde)
                    {
                        Commande commande = new Commande();
                        commande.IdUtilisateur = utilisateur.IdUtilisateur;
                        commande.IdRestaurant = panier.IdRestaurant;
                        commande.Date = DateTime.Now;
                        commande.Prix = prixTotal;
                        commande.IdEtatCommande = 1;
                        utilisateur.Solde -= prixTotal;
                        foreach (ItemPanier item in panier)
                        {
                            CommandeProduit commandeProduit = new CommandeProduit();
                            commandeProduit.IdProduit = item.GetIdProduit();
                            commandeProduit.Prix = item.Prix;
                            commandeProduit.Quantite = item.Quantite;
                            commande.CommandeProduits.Add(commandeProduit);
                            try
                            {
                                Produit produit = db.Produits.Find(commandeProduit.IdProduit);
                                if (produit.IdProduit == commandeProduit.IdProduit)
                                {
                                    produit.Quantite -= commandeProduit.Quantite;
                                }
                            }
                            catch (Exception ex)
                            {
                                string er = ex.Message;
                            }
                        }
                        db.Commandes.Add(commande);
                        db.SaveChanges();
                        HttpContext.Application.Clear();
                    }
                }
                return Json(new { idutilisateur = utilisateur.IdUtilisateur }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string er = ex.Message;
            }
            return Json(JsonRequestBehavior.AllowGet);
        }
        public JsonResult LoginUtilisateur(string idSession, string matricule, string password)
        {
            Utilisateur utilisateur = db.Utilisateurs.FirstOrDefault(u => u.Matricule == matricule && u.Password == password);

            if (utilisateur != null)
            {
                utilisateur.IdSession = idSession;
                db.SaveChanges();

                return Json(new { error = 0, message = utilisateur.Solde }, JsonRequestBehavior.AllowGet);
                    
            }
            return Json(new { error = 1, message = utilisateur.Solde });


            }
        //public JsonResult GetListeRestaurants(int? idTypeCuisine, idRestaurant)
        //{
        //    SessionUtilisateur sessionUtilisateur = db.SessionUtilisateurs.Find(Session.SessionID);
        //    List<ProduitPanier> panier = null;
        //    if (sessionUtilisateur != null)
        //    {
        //        if (HttpContext.Application[idsession] != null)
        //        {
        //            panier = (List<ProduitPanier>)HttpContext.Application[idsession];
        //        }
        //    }
        //    return Json(panier, JsonRequestBehavior.AllowGet);
        //}
        public JsonResult AddMenu(int IdMenu, List<int> IdProduits, string idsession)
        {
            SessionUtilisateur sessionUtilisateur = db.SessionUtilisateurs.Find(Session.SessionID);
            PanierModel produitPaniers = null;
            if (sessionUtilisateur != null)
            {
                if (HttpContext.Application[idsession] != null)
                {
                    produitPaniers = (PanierModel)HttpContext.Application[idsession];
                }
                else
                {
                    produitPaniers = new PanierModel();
                }
                Menu menu = db.Menus.Find(IdMenu);
                if (menu != null)
                {
                    MenuPanier menuPanier = new MenuPanier();
                    menuPanier.IdMenu = IdMenu;
                    foreach (int IdProduit in IdProduits)
                    {
                        ProduitPanier produitPanier = FindProduit(IdProduit);

                        if (produitPanier != null)
                        {
                            menuPanier.produits.Add(produitPanier);
                        }
                    }
                    produitPaniers.Add(menuPanier);
                }
                HttpContext.Application[idsession] = produitPaniers;
            }
            return Json(produitPaniers.Quantite, JsonRequestBehavior.AllowGet);
        }
        private ProduitPanier FindProduit(int IdProduit)
        {
            Produit produit = db.Produits.Find(IdProduit);
            ProduitPanier produitPanier = null;
            if (produitPanier != null)
            {
                produitPanier = new ProduitPanier();
                produitPanier.IdProduit = IdProduit;
                produitPanier.Nom = produit.Nom;
                produitPanier.Description = produit.Description;
                produitPanier.Quantite = 1;
                produitPanier.Prix = produit.Prix;
                produitPanier.Photo = produit.Photos.First().Nom;
                produitPanier.IdRestaurant = produit.IdRestaurant;
            }

            return produitPanier;
        }
        private PanierModel GetPanier(string idsession)
        {
            PanierModel panier = null;
            if (HttpContext.Application[idsession] != null)
            {
                panier = (PanierModel)HttpContext.Application[idsession];
            }
            else
            {
                panier = new PanierModel();
                panier.IdRestaurant = 0;
            }
            return panier;
        }

    }
        
}
