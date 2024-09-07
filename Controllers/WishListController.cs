using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteGiayKingShoes.Models;

namespace WebsiteGiayKingShoes.Controllers
{
    [RateLimit]
    public class WishListController : Controller
    {
        dbDataContext db = new dbDataContext();
        public List<WishList> LayWishList()
        {
            List<WishList> lstWish = Session["Wish"] as List<WishList>;
            if (lstWish == null)
            {
                lstWish = new List<WishList>();
                Session["Wish"] = lstWish;
            }
            return lstWish;
        }

          
        public ActionResult WishList()
        {
            List<WishList> lstWish = LayWishList();
            if (lstWish.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.TongSoLuong = TongSoLuong();
            return View(lstWish);
        }

        public ActionResult MoveToCart(int ms)
        {
            List<WishList> lstWish = LayWishList();
            WishList sp = lstWish.SingleOrDefault(n => n.iMaSP == ms);

            if (sp != null)
            {
                // Remove the item from the wishlist
                lstWish.Remove(sp);

                // Redirect to the CartController's AddToCart action with product details
                return RedirectToAction("AddToCart", "Cart", new { ms = ms });
            }

            // Handle if the product is not found in the wishlist
            return RedirectToAction("WishList");
        } 

        public ActionResult AddWishList(int ms, string url)
        {
            List<WishList> lstWish = LayWishList();
            WishList sp = lstWish.Find(n => n.iMaSP == ms);

            if (sp == null)
            {
                sp = new WishList(ms);
                lstWish.Add(sp);
            }
            else
            {
                sp.iSoLuong++;
            }

            return Redirect(url);
        }
        public ActionResult RemoveWish(int iMaSP)
        {
            List<WishList> lstWish = LayWishList();
            WishList sp = lstWish.SingleOrDefault(n => n.iMaSP == iMaSP);

            if (sp != null)
            {
                lstWish.RemoveAll(n => n.iMaSP == iMaSP );

                if (lstWish.Count == 0)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            return RedirectToAction("WishList");
        }

        private int TongSoLuong()
        {
            int iTongSoLuong = 0;
            List<WishList> lstWish = Session["Wish"] as List<WishList>;
            if (lstWish != null)
            {
                iTongSoLuong = lstWish.Sum(n => n.iSoLuong);
            }
            return iTongSoLuong;

        }


    }
}