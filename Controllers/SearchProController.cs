using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteGiayKingShoes.Models;
using System.Data.Entity;
using PagedList;
using PagedList.Mvc;
namespace WebsiteGiayKingShoes.Controllers
{
    [RateLimit]
    public class SearchProController : Controller
    {
         dbDataContext db = new dbDataContext();
        public ActionResult Search(int? page, string size, string price, string sort, string searchString, int pageSize = 18)
        {
            var kq = db.SANPHAMs.Include(b => b.DANHMUC).Include(b => b.SizeSanPhams);

            // Filter by search string
            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                kq = kq.Where(b =>
                    b.TenSP.ToLower().Contains(searchString) ||
                    b.CodeSP.ToLower().Contains(searchString) ||
                    b.DANHMUC.TenDM.ToLower().Contains(searchString));
            }

            // Filter by size
            if (!string.IsNullOrEmpty(size) && size != "all")
            {
                kq = kq.Where(b => b.SizeSanPhams.Any(s => s.Size == size));
            }

            // Filter by price
            if (!string.IsNullOrEmpty(price) && price != "all")
            {
                switch (price)
                {
                    case "under3tr":
                        kq = kq.Where(b => b.GiaBan < 3000000);
                        break;
                    case "3to5":
                        kq = kq.Where(b => b.GiaBan >= 3000000 && b.GiaBan <= 5000000);
                        break;
                    case "5to10":
                        kq = kq.Where(b => b.GiaBan >= 5000000 && b.GiaBan <= 10000000);
                        break;
                    case "10to15":
                        kq = kq.Where(b => b.GiaBan >= 10000000 && b.GiaBan <= 15000000);
                        break;
                    case "over15":
                        kq = kq.Where(b => b.GiaBan > 15000000);
                        break;
                        // Add additional cases for more price ranges if needed
                }
            }

            // Apply sorting
            switch (sort)
            {
                case "latest":
                    kq = kq.OrderByDescending(b => b.NgayCapNhat);
                    break;
             /*   case "popular":
                    // Add logic for popular sorting
                    break;  */
                case "priceHighToLow":
                    kq = kq.OrderByDescending(b => b.GiaBan);
                    break;
                case "priceLowToHigh":
                    kq = kq.OrderBy(b => b.GiaBan);
                    break;
                default:
                    // Default sorting, if needed
                    break;
            }

            // Pagination
            int pageNumber = page ?? 1;
            int totalItems = kq.Count();
            var pagedData = kq.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            // Create a PagedList and return it to the view
            var pagedList = new StaticPagedList<SANPHAM>(pagedData, pageNumber, pageSize, totalItems);
            return View(pagedList);
        }



    }
}