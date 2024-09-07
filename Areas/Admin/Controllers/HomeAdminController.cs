using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteGiayKingShoes.Models;

namespace WebsiteGiayKingShoes.Areas.Admin.Controllers
{
    public class HomeAdminController : Controller
    {
        dbDataContext db = new dbDataContext();
        public ActionResult Index()
        {
            return View(db.Users);
        }

        public ActionResult NavAdminPartial()
        {
            return PartialView();
        }

        public ActionResult SlideBarPartial()
        {
            return PartialView();
        }

        public ActionResult SpinnerPartial()
        {
            return PartialView();
        }


    }
}