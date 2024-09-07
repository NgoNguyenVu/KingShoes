using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteGiayKingShoes.Models;

namespace WebsiteGiayKingShoes.Areas.Admin.Controllers
{
    public class ManaCaController : Controller
    {
        dbDataContext db = new dbDataContext();
        public ActionResult Index()
        {
            return View(db.DANHMUCs);
        }
    }
}