using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace AfpEat.Models
{
    public class ProduitPanier : ItemPanier
    {
        public int IdProduit { get; set; }
        public List<ProduitPanier> produits { get; set; }
        
    }
}