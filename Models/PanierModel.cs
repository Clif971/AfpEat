using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AfpEat.Models
{
    public class PanierModel : List<ItemPanier>
    {
        public int IdRestaurant { get; set; }
        public int Quantite { get; set; }
        public decimal Montant { get; set; }
        public void CalculPanier()
        {
            Quantite = 0;
            Montant = 0;
        }
        private void parcourir()
        {
            foreach (ItemPanier itemPanier in this)
            {
                Montant += itemPanier.Quantite * itemPanier.Prix;
                Quantite += itemPanier.Quantite;
                IdRestaurant = itemPanier.IdRestaurant;
            }

        }

        public bool AddItem(ItemPanier itemPanier)
        {
            int idProduit = itemPanier.GetIdProduit();
            int idMenu = itemPanier.GetIdMenu();
            bool isReturnOk = true;

            if (itemPanier != null)
            {
                if (itemPanier is ProduitPanier && idProduit > 0) //Je suis sur un produit
                {
                    // On vérifie si ce produit n'est pas dans le panier
                    ItemPanier item = this.FirstOrDefault(p => p.GetIdProduit() == idProduit);

                    if (itemPanier != null) // J'ai trouvé mon produit dans le panier
                    {
                        itemPanier.Quantite++;
                    }
                    else // Je n'ai pas trouvé ce produit dans le panier donc j'ajoute
                    {   
                        //On récupère le produit en dd que l'on souhaite ajouter
                        this.Add(item);
                    }
                }
            }
            else
            {
                isReturnOk = false;
            }
            return isReturnOk;
        }
        public bool RemoveItem(int? idProduit, int? idMenu)
        {
            bool isReturnOk = false;
            ItemPanier itemPanier = null;

            if(idProduit != null && idProduit > 0)
            {
                itemPanier = this.FirstOrDefault(p => p.GetIdProduit() == idProduit);
                
            }
            else if (idMenu != null &&idMenu > 0)
            {
                itemPanier = this.FirstOrDefault(p => p.GetIdMenu() == idMenu);
              
            }
            if (itemPanier != null)
            {
                this.Remove(itemPanier);
                isReturnOk = true;

                CalculPanier();
            }

            return isReturnOk;

        }
        
    }

}
            
            