using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteGiayKingShoes.Models;

namespace WebsiteGiayKingShoes.Controllers
{
    [RateLimit]
    public class HomeController : Controller
    {
        dbDataContext db = new dbDataContext();
        private List<SANPHAM> LayGiay(int count, int category)
        {
            return db.SANPHAMs.Where(a => a.MaDM == category)
                              .OrderBy(a => a.MaSP)
                              .Take(count)
                              .ToList();
        }

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult NavPartial()
        {
            return PartialView();
        }

        public ActionResult FooterPartial()
        {
            return PartialView();
        }

        public ActionResult VoucherPartial()
        {
            return PartialView();
        }
        public ActionResult Intro()
        {
            return View();
        }

        public ActionResult Nike(int? page, string size, string price, string sort, string searchString)
        {
            // Assuming you want to keep the LayGiay method
            var listsbn = LayGiay(14, 1);

            var kq = db.SANPHAMs.Include(b => b.DANHMUC).Include(b => b.SizeSanPhams)
                        .Where(a => a.MaDM == 1); // Assuming MaDM for Nike is 1

            // Apply search, size, price filters using your existing logic
            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                kq = kq.Where(b =>
                    b.TenSP.ToLower().Contains(searchString) ||
                    b.CodeSP.ToLower().Contains(searchString) ||
                    b.DANHMUC.TenDM.ToLower().Contains(searchString));
            }

            if (!string.IsNullOrEmpty(size) && size != "all")
            {
                kq = kq.Where(b => b.SizeSanPhams.Any(s => s.Size == size));
            }

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
            int pageSize = 14;
            int pageNumber = page ?? 1;
            int totalItems = kq.Count();
            var pagedData = kq.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            // Create a PagedList and return it to the view
            var pagedList = new StaticPagedList<SANPHAM>(pagedData, pageNumber, pageSize, totalItems);

            var newsArticles = db.TinTucs.OrderByDescending(t => t.Publish_date).Take(5).ToList();

            // Pass filter values to the view
            ViewBag.Size = size;
            ViewBag.Price = price;
            ViewBag.Sort = sort;
            ViewBag.SearchString = searchString;
            ViewBag.Page = page;

            // Pass the listsbn to the view if needed
            ViewBag.Listsbn = listsbn;
            ViewBag.NewsArticles = newsArticles;

            return View(pagedList);
        }


        public ActionResult Adidas(int? page, string size, string price, string sort, string searchString)
        {
            // Assuming you want to keep the LayGiay method
            var listsbn = LayGiay(14, 2);

            var kq = db.SANPHAMs.Include(b => b.DANHMUC).Include(b => b.SizeSanPhams)
                        .Where(a => a.MaDM == 2); // Assuming MaDM for Nike is 1

            // Apply search, size, price filters using your existing logic
            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                kq = kq.Where(b =>
                    b.TenSP.ToLower().Contains(searchString) ||
                    b.CodeSP.ToLower().Contains(searchString) ||
                    b.DANHMUC.TenDM.ToLower().Contains(searchString));
            }

            if (!string.IsNullOrEmpty(size) && size != "all")
            {
                kq = kq.Where(b => b.SizeSanPhams.Any(s => s.Size == size));
            }

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
            int pageSize = 14;
            int pageNumber = page ?? 1;
            int totalItems = kq.Count();
            var pagedData = kq.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            // Create a PagedList and return it to the view
            var pagedList = new StaticPagedList<SANPHAM>(pagedData, pageNumber, pageSize, totalItems);

            var newsArticles = db.TinTucs.OrderByDescending(t => t.Publish_date).Take(5).ToList();

            // Pass filter values to the view
            ViewBag.Size = size;
            ViewBag.Price = price;
            ViewBag.Sort = sort;
            ViewBag.SearchString = searchString;
            ViewBag.Page = page;

            // Pass the listsbn to the view if needed
            ViewBag.Listsbn = listsbn;
            ViewBag.NewsArticles = newsArticles;

            return View(pagedList);
        }

        public ActionResult Jordan(int? page, string size, string price, string sort, string searchString)
        {
            // Assuming you want to keep the LayGiay method
            var listsbn = LayGiay(14, 3);

            var kq = db.SANPHAMs.Include(b => b.DANHMUC).Include(b => b.SizeSanPhams)
                        .Where(a => a.MaDM == 3); // Assuming MaDM for Nike is 1

            // Apply search, size, price filters using your existing logic
            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                kq = kq.Where(b =>
                    b.TenSP.ToLower().Contains(searchString) ||
                    b.CodeSP.ToLower().Contains(searchString) ||
                    b.DANHMUC.TenDM.ToLower().Contains(searchString));
            }

            if (!string.IsNullOrEmpty(size) && size != "all")
            {
                kq = kq.Where(b => b.SizeSanPhams.Any(s => s.Size == size));
            }

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
            int pageSize = 14;
            int pageNumber = page ?? 1;
            int totalItems = kq.Count();
            var pagedData = kq.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            // Create a PagedList and return it to the view
            var pagedList = new StaticPagedList<SANPHAM>(pagedData, pageNumber, pageSize, totalItems);

            var newsArticles = db.TinTucs.OrderByDescending(t => t.Publish_date).Take(5).ToList();

            // Pass filter values to the view
            ViewBag.Size = size;
            ViewBag.Price = price;
            ViewBag.Sort = sort;
            ViewBag.SearchString = searchString;
            ViewBag.Page = page;

            // Pass the listsbn to the view if needed
            ViewBag.Listsbn = listsbn;
            ViewBag.NewsArticles = newsArticles;

            return View(pagedList);
        }

        public ActionResult Yeezy(int? page, string size, string price, string sort, string searchString)
        {
            // Assuming you want to keep the LayGiay method
            var listsbn = LayGiay(14, 4);

            var kq = db.SANPHAMs.Include(b => b.DANHMUC).Include(b => b.SizeSanPhams)
                        .Where(a => a.MaDM == 4); // Assuming MaDM for Nike is 1

            // Apply search, size, price filters using your existing logic
            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                kq = kq.Where(b =>
                    b.TenSP.ToLower().Contains(searchString) ||
                    b.CodeSP.ToLower().Contains(searchString) ||
                    b.DANHMUC.TenDM.ToLower().Contains(searchString));
            }

            if (!string.IsNullOrEmpty(size) && size != "all")
            {
                kq = kq.Where(b => b.SizeSanPhams.Any(s => s.Size == size));
            }

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
            int pageSize = 14;
            int pageNumber = page ?? 1;
            int totalItems = kq.Count();
            var pagedData = kq.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            // Create a PagedList and return it to the view
            var pagedList = new StaticPagedList<SANPHAM>(pagedData, pageNumber, pageSize, totalItems);
            var newsArticles = db.TinTucs.OrderByDescending(t => t.Publish_date).Take(5).ToList();

            // Pass filter values to the view
            ViewBag.Size = size;
            ViewBag.Price = price;
            ViewBag.Sort = sort;
            ViewBag.SearchString = searchString;
            ViewBag.Page = page;

            // Pass the listsbn to the view if needed
            ViewBag.Listsbn = listsbn;
            ViewBag.NewsArticles = newsArticles;

            return View(pagedList);
        }

        public ActionResult Blog()
        {
            // Assuming Blog is the name of the table in your database
            var blogs = db.TinTucs.ToList();

            // You can now pass the list of blogs to the view
            return View(blogs);
        }
        public ActionResult BlogDetail(int id)
        {
            // Assuming Blog is the name of the table in your database
            var blog = db.TinTucs.SingleOrDefault(b => b.MaTinTuc == id);

            if (blog == null)
            {
                // You may want to handle the case where the blog with the given ID is not found
                return HttpNotFound();
            }

            // Pass the blog to the view
            return View(blog);
        }

        public ActionResult SpaShoes()
        {
            return View();
        }
        public ActionResult Contact()
        {
            return View();
        }

        
        public ActionResult SearchValueShoes() 
        {
            return PartialView();
        }

        public ActionResult ProductDetail(int id)
        {
            SANPHAM product = db.SANPHAMs.FirstOrDefault(p => p.MaSP == id);
            ViewBag.Product = product;

            // Lấy danh sách hình ảnh chi tiết từ cơ sở dữ liệu dựa trên MaSP
            List<PicDetail> productImages = db.PicDetails.Where(img => img.MaSP == id).ToList();
            ViewBag.ProductImages = productImages;

            TrangThai trangThai = db.TrangThais.FirstOrDefault(t => t.MaTT == product.TrangThai);
            ViewBag.TenTrangThai = trangThai;

            List<SizeSanPham> sizespList = db.SizeSanPhams.Where(s => s.MaSP == id).ToList();
            ViewBag.SizeSP = sizespList;

            return View();
        }

    }
}