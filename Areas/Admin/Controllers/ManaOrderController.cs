using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteGiayKingShoes.Models;

namespace WebsiteGiayKingShoes.Areas.Admin.Controllers
{
    public class ManaOrderController : Controller
    {
        dbDataContext db = new dbDataContext();

        public ActionResult Index()
        {
            return View(db.DONDATHANGs);
        }

        [HttpGet]
        public JsonResult Detail(int maDH)
        {
            try
            {
                var dh = (from s in db.DONDATHANGs
                          where (s.MaDonHang == maDH)
                          select new
                          {
                              MaDonHang = s.MaDonHang,
                              DaThanhToan = s.DaThanhToan,
                              TinhTrangGiaoHang = s.TinhTrangGiaoHang,
                              NgayDat = s.NgayDat,
                              NgayGiao = s.NgayGiao,
                              MaUser = s.MaUser,
                          }).SingleOrDefault();
                return Json(new { code = 200, dh, msg = "Lấy thông tin tin tức thành công " },
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Lấy thông tin tin tức thất bại" + ex.Message },
                    JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult Add(int madh, bool tttt, int ttgh, DateTime nd, DateTime ng, int makh)
        {
            try
            {
                var dh = new DONDATHANG();
                dh.MaDonHang = madh;
                dh.DaThanhToan = tttt;
                dh.TinhTrangGiaoHang = ttgh;
                dh.NgayDat = nd;
                dh.NgayGiao = ng;
                dh.MaUser = makh;

                db.DONDATHANGs.InsertOnSubmit(dh);
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
        public JsonResult Update(int maDH, bool tttt, int ttgh, string nd, string ng, int makh)
        {
            try
            {
                var dh = db.DONDATHANGs.SingleOrDefault(c => c.MaDonHang == maDH);

                if (dh != null)
                {
                    dh.DaThanhToan = tttt;
                    dh.TinhTrangGiaoHang = ttgh;

                    string dateFormat = "dd/MM/yyyy"; // Replace with your actual date format

                    if (DateTime.TryParseExact(nd, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedNgayDat) &&
                        DateTime.TryParseExact(ng, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedNgayGiao))
                    {
                        // Compare both the month and day, ignoring the year
                        if (parsedNgayDat.Month > parsedNgayGiao.Month || (parsedNgayDat.Month == parsedNgayGiao.Month && parsedNgayDat.Day > parsedNgayGiao.Day))
                        {
                            return Json(new { code = 500, msg = "Sửa thất bại. Ngày đặt hàng không thể nhỏ hơn ngày giao hàng." });
                        }

                        dh.NgayDat = parsedNgayDat;
                        dh.NgayGiao = parsedNgayGiao;

                        dh.MaUser = makh;

                        db.SubmitChanges();

                        return Json(new { code = 200, msg = "Sửa thành công" });
                    }
                    else
                    {
                        return Json(new { code = 500, msg = "Sửa thất bại. Ngày đặt hoặc ngày giao hàng không hợp lệ." });
                    }
                }
                else
                {
                    return Json(new { code = 404, msg = "Không tìm thấy đơn hàng" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Sửa thất bại. Lỗi" + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Delete(int maDH)
        {
            try
            {
                // Find the main record in DONDATHANGs
                var dh = db.DONDATHANGs.SingleOrDefault(c => c.MaDonHang == maDH);

                // Check if the record exists
                if (dh != null)
                {
                    // Find and delete related records in ChiTietDatHang
                    var chiTietRecords = db.CHITIETDATHANGs.Where(ct => ct.MaDonHang == maDH);
                    db.CHITIETDATHANGs.DeleteAllOnSubmit(chiTietRecords);

                    // Delete the main record in DONDATHANGs
                    db.DONDATHANGs.DeleteOnSubmit(dh);

                    // Submit changes to the database
                    db.SubmitChanges();

                    return Json(new { code = 200, msg = "Xóa thành công " }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { code = 404, msg = "Không tìm thấy đơn hàng" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Xóa thất bại. Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


    }
}