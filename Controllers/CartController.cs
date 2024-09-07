using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Reflection;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
using WebsiteGiayKingShoes.Models;
using Google.Apis.PeopleService.v1.Data;

namespace WebsiteGiayKingShoes.Controllers
{
    [RateLimit]
    public class CartController : Controller
    {
        dbDataContext db = new dbDataContext();


        private void GuiEmailDonHangThanhCong(string diaChiEmail, List<Cart> lstGioHang)
        {
            var fromAddress = new MailAddress("ngonguyenvupl93@gmail.com", "King Shoes");
            var toAddress = new MailAddress(diaChiEmail, "Recipient Name");
            const string fromPassword = "xtet syaj faku tuxf";
            const string subject = "Đơn hàng của bạn đã được xác nhận";

            // Tạo nội dung email sử dụng HTML
            string body = @"
        <!DOCTYPE html>
        <html lang='en'>
        <head>
            <meta charset='UTF-8'>
            <meta name='viewport' content='width=device-width, initial-scale=1.0'>
            <style>
                body {
                    font-family: 'Arial', sans-serif;
                    margin: 0;
                    padding: 0;
                }
                h2 {
                    color: #333;
                }
                table {
                    border-collapse: collapse;
                    width: 100%;
                }
                th, td {
                    border: 1px solid #333;
                    padding: 10px;
                    text-align: left;
                }
                img {
                    max-width: 100px;
                    max-height: 100px;
                }
            </style>
        </head>
        <body>
            <h2>Đơn hàng của bạn đã được xác nhận</h2>
            <p>Cảm ơn bạn đã đặt hàng! Đơn hàng của bạn đã được nhận và đang được xử lý.</p>
            <table>
                <thead>
                    <tr>
                        <th>Tên Sản Phẩm</th>
                        <th>Ảnh Bìa</th>
                        <th>Size</th>
                        <th>Số Lượng</th>
                        <th>Đơn Giá</th>
                        <th>Thành Tiền</th>
                    </tr>
                </thead>
                <tbody>";

            foreach (var item in lstGioHang)
            {
                body += $@"
                    <tr>
                        <td>{item.sTenSP}</td>
                        <td><img src='{item.sAnhBia}' alt='Ảnh sản phẩm'></td>
                        <td>{item.sSize}</td>
                        <td>{item.iSoLuong}</td>
                        <td>{item.dDonGia}</td>
                        <td>{item.dThanhTien}</td>
                    </tr>";
            }

            body += @"
                </tbody>
            </table>
        </body>
        </html>";

            var smtpClient = new SmtpClient
            {
                Host = "smtp.gmail.com", // Địa chỉ máy chủ SMTP của bạn
                Port = 587, // Cổng SMTP
                EnableSsl = true, // Kích hoạt giao thức bảo mật SSL
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true // Đánh dấu nội dung là HTML
            })
            {
                smtpClient.Send(message);
            }
        }






        public ActionResult Cart()
        {
            List<Cart> lstGioHang = LayGioHang();
            if (lstGioHang.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return View(lstGioHang);
        }

        public List<Cart> LayGioHang()
        {
            List<Cart> lstGioHang = Session["GioHang"] as List<Cart>;
            if (lstGioHang == null)
            {
                lstGioHang = new List<Cart>();
                Session["GioHang"] = lstGioHang;
            }
            return lstGioHang;
        }
        public ActionResult AddToCart(int ms)
        {
            List<Cart> lstGioHang = LayGioHang();
            Cart cartItem = lstGioHang.SingleOrDefault(n => n.iMaSP == ms);

            if (cartItem == null)
            {
                cartItem = new Cart(ms);
                lstGioHang.Add(cartItem);
            }
            else
            {
                cartItem.iSoLuong++;
            }

            // Save the shopping cart back to the session
            Session["GioHang"] = lstGioHang;

            return RedirectToAction("Cart");
        } 

        public ActionResult ThemGioHang(int ms, string url, string selectedSize)
        {
            if (string.IsNullOrEmpty(selectedSize))
            {
                return Redirect(url);
            }
            List<Cart> lstGioHang = LayGioHang();
            Cart sp = lstGioHang.Find(n => n.iMaSP == ms && n.sSize == selectedSize);

            if (sp == null)
            {
                sp = new Cart(ms);
                sp.sSize = selectedSize; // Set the selected size
                lstGioHang.Add(sp);
            }
            else
            {
                sp.iSoLuong++;
            }

            return Redirect(url);
        }

        public ActionResult BuyNow(int ms, string selectedSize, string url)
        {
            if (string.IsNullOrEmpty(selectedSize))
            {
                // Handle invalid selection
                return Redirect(url);
            }

            // Check if the user is logged in
            if (Session["TaiKhoan"] == null || Session["TaiKhoan"].ToString() == "")
            {
                // Redirect to the login page with a return URL
                return RedirectToAction("Account", "User", new { url = Request.Url.ToString() });
            }

            List<Cart> lstGioHang = LayGioHang();
            Cart sp = lstGioHang.Find(n => n.iMaSP == ms && n.sSize == selectedSize);

            if (sp == null)
            {
                // If the product is not in the cart, add it with a quantity of 1
                sp = new Cart(ms);
                sp.sSize = selectedSize;
                sp.iSoLuong = 1;
                lstGioHang.Add(sp);
            }

            // Save the shopping cart back to the session
            Session["GioHang"] = lstGioHang;

            // Redirect to the checkout page (DatHang action)
            return RedirectToAction("DatHang");
        }



        [HttpPost]
        public JsonResult UpdateQuantity(int productId, string size, int newQuantity)
        {
            List<Cart> lstGioHang = LayGioHang();
            Cart sp = lstGioHang.SingleOrDefault(n => n.iMaSP == productId && n.sSize == size);

            if (sp != null)
            {
                sp.iSoLuong = newQuantity;
            }

            Session["GioHang"] = lstGioHang;

            // Return a JSON result indicating success
            return Json(new { success = true });
        }


        public ActionResult XoaSPKhoiGioHang(int iMaSP, string selectedSize)
        {
            List<Cart> lstGioHang = LayGioHang();
            Cart sp = lstGioHang.SingleOrDefault(n => n.iMaSP == iMaSP && n.sSize == selectedSize);

            if (sp != null)
            {
                lstGioHang.RemoveAll(n => n.iMaSP == iMaSP && n.sSize == selectedSize);

                if (lstGioHang.Count == 0)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            return RedirectToAction("Cart");
        }


        public ActionResult GioHangPartial()
        {
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return PartialView();
        }



        private int TongSoLuong()
        {
            int iTongSoLuong = 0;
            List<Cart> lstGioHang = Session["GioHang"] as List<Cart>;
            if (lstGioHang != null)
            {
                iTongSoLuong = lstGioHang.Sum(n => n.iSoLuong);
            }
            return iTongSoLuong;

        }
        private double TongTien()
        {
            double dTongTien = 0;
            List<Cart> lstGioHang = Session["GioHang"] as List<Cart>;
            if (lstGioHang != null)
            {
                dTongTien = lstGioHang.Sum(n => n.dThanhTien);
            }
            return dTongTien;
        }



        [HttpGet]
        public ActionResult DatHang()
        {
            if (Session["TaiKhoan"] == null || Session["TaiKhoan"].ToString() == "")
            {
                return RedirectToAction("Account", "User", new { url = Request.Url.ToString() });
            }

            if (Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            List<Cart> lstGioHang = LayGioHang();
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return View(lstGioHang);
        }

        [HttpPost]
        public ActionResult DatHang(FormCollection f)
        {
            DONDATHANG ddh = new DONDATHANG();
            User kh = (User)Session["TaiKhoan"];
            List<Cart> lstGioHang = LayGioHang();
            ddh.MaUser = kh.MaUser;
            ddh.NgayDat = DateTime.Now;

            var NgayGiao = String.Format("{0:MM/dd/yyyy}", f["NgayGiao"]);
            if (String.IsNullOrEmpty(NgayGiao))
            {
                ModelState.AddModelError("NgayGiao", "Ngày giao không được để trống!");
            }
            else
            {
                DateTime ngayGiao;
                bool isvalidNgayGiao = DateTime.TryParse(NgayGiao, out ngayGiao);
                if (!isvalidNgayGiao)
                {
                    ModelState.AddModelError("NgayGiao", "Ngày giao không hợp lệ!");
                }
                else if (ngayGiao < DateTime.Now.Date)
                {
                    ModelState.AddModelError("NgayGiao", "Ngày giao không thể nhỏ hơn ngày Đặt!");
                }
                else
                {
                    // Assuming NgayGiao is a DateTime property in your model
                    ddh.NgayGiao = ngayGiao; // Set the value in your model
                }
            }
            // Kiểm tra ModelState để xác định có lỗi hay không
            if (!ModelState.IsValid)
            {
                // Nếu có lỗi, gán lại ViewBag và trả về view với ModelState để hiển thị thông báo lỗi
                ViewBag.TongSoLuong = TongSoLuong();
                ViewBag.TongTien = TongTien();
                ViewData["Err1"] = ModelState["NgayGiao"].Errors[0].ErrorMessage;
                return View(lstGioHang);
            }


           
            ddh.DaThanhToan = false;
            db.DONDATHANGs.InsertOnSubmit(ddh);
            db.SubmitChanges();

            foreach (var item in lstGioHang)
            {
                CHITIETDATHANG ctdh = new CHITIETDATHANG();
                ctdh.MaDonHang = ddh.MaDonHang;
                ctdh.SoLuong = item.iSoLuong;
                ctdh.DonGia = (decimal)item.dDonGia;
                ctdh.Color = item.sColor;
                ctdh.Size = item.sSize;
                ctdh.MaSP = item.iMaSP;
                db.CHITIETDATHANGs.InsertOnSubmit(ctdh);
            }
            db.SubmitChanges();
          

       

            double totalAmount = TongTien() * 100;
            string url = ConfigurationManager.AppSettings["vnp_Url"];
            string returnUrl = ConfigurationManager.AppSettings["vnp_Returnurl"];
            string tmnCode = ConfigurationManager.AppSettings["vnp_TmnCode"];
            string hashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"];

            VnPayLibrary pay = new VnPayLibrary();
            pay.AddRequestData("vnp_BankCode", "VNBANK");
            pay.AddRequestData("vnp_Version", "2.1.0"); //Phiên bản api mà merchant kết nối. Phiên bản hiện tại là 2.1.0
            pay.AddRequestData("vnp_Command", "pay"); //Mã API sử dụng, mã cho giao dịch thanh toán là 'pay'
            pay.AddRequestData("vnp_TmnCode", tmnCode); //Mã website của merchant trên hệ thống của VNPAY (khi đăng ký tài khoản sẽ có trong mail VNPAY gửi về)
            pay.AddRequestData("vnp_Amount", totalAmount.ToString());    
            pay.AddRequestData("vnp_BankCode", ""); //Mã Ngân hàng thanh toán (tham khảo: https://sandbox.vnpayment.vn/apis/danh-sach-ngan-hang/), có thể để trống, người dùng có thể chọn trên cổng thanh toán VNPAY
            pay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss")); //ngày thanh toán theo định dạng yyyyMMddHHmmss
            pay.AddRequestData("vnp_CurrCode", "VND"); //Đơn vị tiền tệ sử dụng thanh toán. Hiện tại chỉ hỗ trợ VND
            pay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress()); //Địa chỉ IP của khách hàng thực hiện giao dịch
            pay.AddRequestData("vnp_Locale", "vn"); //Ngôn ngữ giao diện hiển thị - Tiếng Việt (vn), Tiếng Anh (en)
            pay.AddRequestData("vnp_OrderInfo", "Nạp tiền"); //Thông tin mô tả nội dung thanh toán
            pay.AddRequestData("vnp_OrderType", "other"); //topup: Nạp tiền điện thoại - billpayment: Thanh toán hóa đơn - fashion: Thời trang - other: Thanh toán trực tuyến
            pay.AddRequestData("vnp_ReturnUrl", returnUrl); //URL thông báo kết quả giao dịch khi Khách hàng kết thúc thanh toán
            pay.AddRequestData("vnp_TxnRef", DateTime.Now.Ticks.ToString()); //mã hóa đơn
            string paymentUrl = pay.CreateRequestUrl(url, hashSecret);
            Session["GioHang"] = null;
            GuiEmailDonHangThanhCong(kh.Email, lstGioHang);
            ddh.TinhTrangGiaoHang = 1;
            return Redirect(paymentUrl);
        }

        public ActionResult DonHang()
        {
            // Check the payment result from VNPAY
            string vnpResponseCode = HttpContext.Request["vnp_ResponseCode"];

            if (vnpResponseCode == "00") // Assuming "00" means a successful payment, adjust based on VNPAY response codes
            {
                // Payment successful
                return RedirectToAction("XacNhanThanhCong");
            }
            else
            {
                // Payment failed
                return RedirectToAction("XacNhanThatBai");
            }
        }

        public ActionResult XacNhanThatBai()
        {
            // Handle failed payment confirmation
            return View();
        }
        public ActionResult XacNhanThanhCong()
        {
            // Handle failed payment confirmation
            return View();
        }

    }
}