using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteGiayKingShoes.Models;

namespace WebsiteGiayKingShoes.Areas.Admin.Controllers
{
    public class ManaBlogController : Controller
    {
        dbDataContext db = new dbDataContext();
        public ActionResult Index()
        {
            return View(db.TinTucs);
        }

        [HttpGet]
        public JsonResult Detail(int maTT)
        {
            try
            {
                var tt = (from s in db.TinTucs
                          where (s.MaTinTuc == maTT)
                          select new
                          {
                              MaTT = s.MaTinTuc,
                              Title = s.Title
                          }).SingleOrDefault();
                return Json(new { code = 200, tt, msg = "Lấy thông tin tin tức thành công " },
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Lấy thông tin tin tức thất bại" + ex.Message },
                    JsonRequestBehavior.AllowGet);
            }
        }



        [HttpPost]
        public JsonResult AddTinTuc(string strTenTT)
        {
            try
            {
                var tt = new TinTuc();
                tt.Title = strTenTT;
                tt.Publish_date = DateTime.Now;
                db.TinTucs.InsertOnSubmit(tt);
                db.SubmitChanges();

                return Json(new { code = 200, msg = "Thêm tin tức thành công " },
                  JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = " Them tin tức thất bại. Loi" + ex.Message },
                  JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult Update(string strTenTT, int maTT)
        {
            try
            {
                var tt = db.TinTucs.SingleOrDefault(c => c.MaTinTuc == maTT);
                tt.Title = strTenTT;
                db.SubmitChanges();

                return Json(new { code = 200, msg = "Sửa tin tức thành công" },
                  JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Sửa tin tức thất bại. Lỗi" + ex.Message },
                  JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult Delete(int maTT)
        {
            try
            {
                var tt = db.TinTucs.SingleOrDefault(c => c.MaTinTuc == maTT);
                db.TinTucs.DeleteOnSubmit(tt);
                db.SubmitChanges();

                return Json(new { code = 200, msg = "Xóa tin tức thành công " },
                  JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = " Xóa tin tức thất bại. Lỗi" + ex.Message },
                  JsonRequestBehavior.AllowGet);
            }
        }


    }
}