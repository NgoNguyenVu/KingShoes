using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebsiteGiayKingShoes.Controllers
{
    [RateLimit]
    public class SP_CSController : Controller
    {
        // GET: SP_CS
        public ActionResult CSKH()
        {
            return View();
        }
        public ActionResult ThanhToan()
        {
            return View();
        }

        public ActionResult HuongDanMuaHang()
        {
            return View();
        }

        public ActionResult CheDoBaoHanh()
        {
            return View();
        }

        public ActionResult ChinhSachDoiHang()
        {
            return View();
        }

        public ActionResult BaoMat()
        {
            return View();
        }

        public ActionResult ChinhSachGiaoNhan()
        {
            return View();
        }

    }
}