using PagedList;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteGiayKingShoes.Models;

namespace WebsiteGiayKingShoes.Areas.Admin.Controllers
{
    public class ManaProController : Controller
    {
        dbDataContext db = new dbDataContext();

        public ActionResult Index(int? page)
        {
            int pageSize = 10; // Số lượng sản phẩm trên mỗi trang
            int pageNumber = (page ?? 1);

            var products = db.SANPHAMs.OrderBy(p => p.MaSP).ToList();

            return View(products.ToPagedList(pageNumber, pageSize));
        }

        [HttpGet]
        public JsonResult Detail(int maSP)
        {
            try
            {
                var sp = (from s in db.SANPHAMs
                          where (s.MaSP == maSP)
                          select new
                          {
                              MaSP = s.MaSP,
                              TenSP = s.TenSP,
                              MaDM = s.MaDM,
                              CodeSP = s.CodeSP,
                              color = s.color,
                              GiaBan = s.GiaBan,
                              TrangThai = s.TrangThai,
                              MoTa = s.MoTa,
                              CoverImage = s.AnhBia,
                              DetailImages = (from pd in db.PicDetails
                                              where pd.MaSP == s.MaSP
                                              select pd.DuongDan).ToList(),
                              Sizes = (from size in db.SizeSanPhams
                                       where size.MaSP == s.MaSP
                                       select size.Size).ToList()
                          }).SingleOrDefault();
                return Json(new { code = 200, sp, msg = "Lấy thông tin sản phẩm thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Lấy thông tin sản phẩm thất bại" + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult Add(string tensp, int brand, string code, string color, int price, int status, string description, HttpPostedFileBase coverImage, HttpPostedFileBase detailImage1, HttpPostedFileBase detailImage2, HttpPostedFileBase detailImage3, HttpPostedFileBase detailImage4, List<string> sizes)
        {
            try
            {
                var sp = new SANPHAM();
                sp.TenSP = tensp;
                sp.MaDM = brand;
                sp.CodeSP = code;
                sp.color = color;
                sp.GiaBan = price;
                sp.TrangThai = status;

                // Add additional fields
                sp.MoTa = description;
                sp.NgayCapNhat = DateTime.Now; // Set the current date and time

                db.SANPHAMs.InsertOnSubmit(sp);
                db.SubmitChanges();

                // Assuming that MaSP is an identity column and is generated upon insertion
                int maSP = sp.MaSP;

                // Handle cover image
                if (coverImage != null && coverImage.ContentLength > 0)
                {
                    // Save the cover image to the server or perform any necessary processing
                    // Retrieve the path where the cover image is saved and update the database
                    string coverImagePath = SaveImageToServer(coverImage);

                    // Update the cover image path in the Product table
                    UpdateCoverImage(maSP, coverImagePath);
                }

                // Handle detail images
                HandleDetailImage(maSP, detailImage1);
                HandleDetailImage(maSP, detailImage2);
                HandleDetailImage(maSP, detailImage3);
                HandleDetailImage(maSP, detailImage4);

                HandleSizes(maSP, sizes);

                return Json(new { code = 200, msg = "Thêm thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Thêm thất bại. Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        private void HandleSizes(int maSP, List<string> sizes)
        {
            if (sizes != null && sizes.Count > 0)
            {
                foreach (var size in sizes)
                {
                    // Phân tách chuỗi kích thước và thêm từng kích thước vào bảng Size
                    string[] sizeArray = size.Split(',');
                    foreach (var individualSize in sizeArray)
                    {
                        db.SizeSanPhams.InsertOnSubmit(new SizeSanPham { MaSP = maSP, Size = individualSize.Trim() });
                    }
                }
                db.SubmitChanges();
            }
        }

        private string SaveImageToServer(HttpPostedFileBase imageFile)
        {
            // Đường dẫn tới thư mục ảnh trong solution
            string imageDirectory = Server.MapPath("~/images/SanPham");

            // Tạo đường dẫn cho ảnh
            string imagePath = Path.Combine(imageDirectory, Path.GetFileName(imageFile.FileName));

            // Lưu ảnh vào thư mục của solution
            imageFile.SaveAs(imagePath);

            return imagePath;
        }

        private void UpdateCoverImage(int maSP, string imagePath)
        {
            // Update đường dẫn ảnh trong record của SANPHAM
            var product = db.SANPHAMs.FirstOrDefault(p => p.MaSP == maSP);
            if (product != null)
            {
                // Gán đường dẫn ảnh
                product.AnhBia = imagePath;

                // Lưu thay đổi vào database
                db.SubmitChanges();
            }
        }
        private void HandleDetailImage(int maSP, HttpPostedFileBase detailImage)
        {
            if (detailImage != null && detailImage.ContentLength > 0)
            {
                // Save the detail image to the server or perform any necessary processing
                // Retrieve the path where the detail image is saved and update the database
                string detailImagePath = SaveImageToServer(detailImage);

                // Insert a new record into the PicDetail table
                var picDetail = new PicDetail
                {
                    MaSP = maSP,
                    DuongDan = detailImagePath
                };

                db.PicDetails.InsertOnSubmit(picDetail);
                db.SubmitChanges();
            }
        }




        [HttpPost]
        public JsonResult Update(string tensp, int brand, string code, string color, int price, int status, int maSP, string description, HttpPostedFileBase coverImage, HttpPostedFileBase detailImage1, HttpPostedFileBase detailImage2, HttpPostedFileBase detailImage3, HttpPostedFileBase detailImage4, List<string> sizes)
        {
            try
            {
                var sp = db.SANPHAMs.SingleOrDefault(c => c.MaSP == maSP);
                sp.TenSP = tensp;
                sp.MaDM = brand;
                sp.CodeSP = code;
                sp.MoTa = description;
                sp.color = color;
                sp.GiaBan = price;
                sp.TrangThai = status;

                if (coverImage != null && coverImage.ContentLength > 0)
                {
                    string coverImagePath = SaveImageToServer(coverImage);
                    UpdateCoverImage(maSP, coverImagePath);
                }

                // Handle detail image updates
                HandleDetailImageUpdate(maSP, detailImage1, 1);
                HandleDetailImageUpdate(maSP, detailImage2, 2);
                HandleDetailImageUpdate(maSP, detailImage3, 3);
                HandleDetailImageUpdate(maSP, detailImage4, 4);

                // Update other fields and save changes
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


        private void HandleDetailImageUpdate(int maSP, HttpPostedFileBase detailImage, int imageNumber)
        {
            if (detailImage != null && detailImage.ContentLength > 0)
            {
                // Save the detail image to the server or perform any necessary processing
                // Retrieve the path where the detail image is saved and update the database
                string detailImagePath = SaveImageToServer(detailImage);

                // Retrieve existing detail image record for the specified number
                var existingDetailImage = db.PicDetails.FirstOrDefault(d => d.MaSP == maSP && d.MaPD == imageNumber);

                if (existingDetailImage != null)
                {
                    // Update existing detail image path
                    existingDetailImage.DuongDan = detailImagePath;
                }
                else
                {
                    // Insert a new record into the PicDetail table if it doesn't exist
                    var picDetail = new PicDetail
                    {
                        MaSP = maSP,
                        DuongDan = detailImagePath,
                        MaPD = imageNumber
                    };

                    db.PicDetails.InsertOnSubmit(picDetail);
                }

                db.SubmitChanges();
            }
        }

        [HttpPost]
        public JsonResult Delete(int maSP)
        {
            try
            {
                // Xóa tất cả các chi tiết ảnh liên quan từ bảng PicDetail
                var picDetails = db.PicDetails.Where(pd => pd.MaSP == maSP);
                db.PicDetails.DeleteAllOnSubmit(picDetails);

                var sizes = db.SizeSanPhams.Where(s => s.MaSP == maSP);
                db.SizeSanPhams.DeleteAllOnSubmit(sizes);

                // Lưu thay đổi trong SizeSanPhams
                db.SubmitChanges();


                // Xóa sản phẩm từ bảng SANPHAM
                var sp = db.SANPHAMs.SingleOrDefault(c => c.MaSP == maSP);
                db.SANPHAMs.DeleteOnSubmit(sp);

                // Lưu thay đổi
                db.SubmitChanges();

                return Json(new { code = 200, msg = "Xóa thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Xóa thất bại. Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }



    }
}