using AfpEat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AfpEat.Controllers
{
    public class HomeController : Controller
    {
        private AfpEatEntities db = new AfpEatEntities();
        public ActionResult Accueil()
        {
            return View();
        }

        public ActionResult Identification()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }       
        
        public ActionResult Panier()
        {
            PanierModel panier = (PanierModel)HttpContext.Application[Session.SessionID];

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


    }
}