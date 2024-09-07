using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteGiayKingShoes.Models;

namespace WebsiteGiayKingShoes.Areas.Admin.Controllers
{
    public class ManaUserController : Controller
    {
        dbDataContext db = new dbDataContext();
        public ActionResult Index()
        {
            return View(db.Users);
        }

        [HttpGet]
        public JsonResult Detail(int maUser)
        {
            try
            {
                var us = (from s in db.Users
                          where (s.MaUser == maUser)
                          select new
                          {
                              MaUser = s.MaUser,
                              HoTen = s.HoTen,
                              TaiKhoan = s.TaiKhoan,
                              MatKhau = s.MatKhau,
                              Email = s.Email,
                              DienThoai = s.DienThoai,
                          }).SingleOrDefault();
                return Json(new { code = 200, us, msg = "Lấy thông tin tin tức thành công " },
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Lấy thông tin tin tức thất bại" + ex.Message },
                    JsonRequestBehavior.AllowGet);
            }
        }



        [HttpPost]
        public JsonResult Add(string ht, string tk, string mk, string email, string dt)
        {
            try
            {
                var us = new User();
                us.HoTen = ht;
                us.TaiKhoan = tk;
                us.MatKhau = mk;
                us.Email = email;
                us.DienThoai = dt;
              
                db.Users.InsertOnSubmit(us);
                db.SubmitChanges();

                return Json(new { code = 200, msg = "Thêm thành công " },
                  JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = " Thêm thất bại. Loi" + ex.Message },
                  JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult Update(string ht, string tk, string mk, string email, string dt, int maUser)
        {
            try
            {
                var us = db.Users.SingleOrDefault(c => c.MaUser == maUser);
                us.HoTen = ht;
                us.TaiKhoan = tk;
                us.MatKhau = mk;
                us.Email = email;
                us.DienThoai = dt;
                db.SubmitChanges();

                return Json(new { code = 200, msg = "Sửa thành công" },
                  JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Sửa thất bại. Lỗi" + ex.Message },
                  JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult Delete(int maUser)
        {
            try
            {
                var us = db.Users.SingleOrDefault(c => c.MaUser == maUser);
                db.Users.DeleteOnSubmit(us);
                db.SubmitChanges();

                return Json(new { code = 200, msg = "Xóa thành công " },
                  JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = " Xóa thất bại. Lỗi" + ex.Message },
                  JsonRequestBehavior.AllowGet);
            }
        }

    }
}