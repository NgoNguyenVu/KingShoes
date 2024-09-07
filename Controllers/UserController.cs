using Facebook;
using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using WebsiteGiayKingShoes.Models;
using Microsoft.AspNet.Identity.Owin;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;

namespace WebsiteGiayKingShoes.Controllers
{
    [RateLimit]
    public class UserController : Controller
    {
        dbDataContext db = new dbDataContext();
        public ActionResult Account(string url)
        {
            ViewBag.url = url;
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(FormCollection f)
        {
            var sTenDN = f["TenDN"];
            var sMatKhau = f["MatKhau"];
            var recaptchaResponse = f["g-recaptcha-response"];
            var url = f["url"];

            if (String.IsNullOrEmpty(url))
                url = "~/Home/Index";

            if (String.IsNullOrEmpty(sTenDN))
            {
                ViewData["Err1"] = "Bạn chưa nhập tên đăng nhập";
            }
            else if (String.IsNullOrEmpty(sMatKhau))
            {
                ViewData["Err2"] = "Bạn chưa nhập mật khẩu";
            }
            else if (String.IsNullOrEmpty(recaptchaResponse))
            {
                ViewBag.ThongBao = "Vui lòng xác minh reCAPTCHA";
            }
            else
            {
                // Xác thực reCAPTCHA
                var client = new System.Net.WebClient();
                var secretKey = "6LcHfuEpAAAAABJ8ueqEXFjZLtUENFmG3nOGNNt7";
                var result = client.DownloadString($"https://www.google.com/recaptcha/api/siteverify?secret={secretKey}&response={recaptchaResponse}");
                var obj = Newtonsoft.Json.Linq.JObject.Parse(result);
                var status = (bool)obj.SelectToken("success");

                if (!status)
                {
                    ViewBag.ThongBao = "Xác minh reCAPTCHA thất bại, vui lòng thử lại.";
                    return View("Account");
                }

                User kh = db.Users.SingleOrDefault(n => n.TaiKhoan == sTenDN);

                if (kh != null)
                {
                    if (kh.MatKhau == HashPassword(sMatKhau))
                    {
                        // Generate and send OTP
                        var otp = GenerateOTP();
                        Session["OTP"] = otp;
                        Session["UserLogin"] = kh;
                        Session["Url"] = url;

                        SendOTPEmail(kh.Email, otp);

                        return RedirectToAction("VerifyOTP");
                    }
                    else
                    {
                        ViewBag.ThongBao = "Tài khoản hoặc mật khẩu không đúng";
                    }
                }
                else
                {
                    ViewBag.ThongBao = "Tài khoản không tồn tại";
                }
            }

            return View("Account");
        }


        [HttpGet]
        public ActionResult VerifyOTP()
        {
            return View();
        }

        [HttpPost]
        public ActionResult VerifyOTP(FormCollection f)
        {
            var otp = f["OTP"];
            var sessionOtp = Session["OTP"]?.ToString();
            var kh = Session["UserLogin"] as User;
            var url = Session["Url"]?.ToString() ?? "~/Home/Index";

            if (otp == sessionOtp)
            {
                Session["TaiKhoan"] = kh;

                if (kh.VaiTro == 0) // Admin
                {
                    return RedirectToAction("Index", "HomeAdmin", new { area = "Admin" });
                }
                else if (kh.VaiTro == 1) // Khách hàng
                {
                    return Redirect(url);
                }
            }
            else
            {
                ViewBag.ThongBao = "OTP không chính xác";
            }

            return View();
        }

        private string GenerateOTP()
        {
            Random random = new Random();
            string otp = "";

            for (int i = 0; i < 5; i++)
            {
                otp += random.Next(0, 10).ToString(); // Append a random digit between 0 and 9
            }

            return otp;
        }

        private void SendOTPEmail(string email, string otp)
        {
            var fromAddress = new MailAddress("ngonguyenvupl93@gmail.com", "King Shoes");
            var toAddress = new MailAddress(email);
            const string fromPassword = "xtet syaj faku tuxf";
            const string subject = "Your OTP Code";
            string body = $"Your OTP code is {otp}";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                smtp.Send(message);
            }
        }



        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(string email)
        {
            var user = db.Users.SingleOrDefault(u => u.Email == email);

            if (user != null)
            {
                // Tạo mã đặt lại mật khẩu (có thể sử dụng thư viện mã hóa)
                string resetCode = GenerateUniquePassword();

                // Lưu mã đặt lại mật khẩu vào cơ sở dữ liệu
                user.ResetCode = resetCode;
                db.SubmitChanges();

                // Gửi email chứa liên kết đặt lại mật khẩu
                SendResetPasswordEmail(user.Email, resetCode);

                ViewBag.Message = "Kiểm tra email của bạn để đặt lại mật khẩu.";
            }
            else
            {
                ViewBag.Error = "Địa chỉ email không tồn tại trong hệ thống.";
            }

            return RedirectToAction("ConfirmEmailMK");
        }




        [HttpGet]
        public ActionResult ResetPassword(string email, string resetCode)
        {
            var user = db.Users.SingleOrDefault(u => u.Email == email && u.ResetCode == resetCode);

            if (user != null)
            {
                // Hợp lệ, cho phép người dùng đặt lại mật khẩu
                return View();
            }
            else
            {
                ViewBag.Error = "Liên kết đặt lại mật khẩu không hợp lệ.";
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult ResetPassword(string email, string MatKhau, bool fromGetRequest = false)
        {
            if (string.IsNullOrEmpty(MatKhau))
            {
                ViewBag.Error = "Mật khẩu mới không được để trống.";
                return View("Error");
            }

            var user = db.Users.SingleOrDefault(u => u.Email == email);

            if (user != null)
            {
                // Cập nhật mật khẩu mới cho người dùng
                user.MatKhau = MatKhau;

                // Xóa mã đặt lại mật khẩu sau khi sử dụng
                user.ResetCode = null;

                db.SubmitChanges();

                ViewBag.Message = "Mật khẩu đã được đặt lại thành công.";
            }
            else
            {
                ViewBag.Error = "Đặt lại mật khẩu không thành công.";
                return View("Error");
            }

            return RedirectToAction("ResetConfirm");
        }



        private void SendResetPasswordEmail(string email, string resetCode)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

                mail.From = new MailAddress("ngonguyenvupl93@gmail.com");
                mail.To.Add(email);
                mail.Subject = "Reset Password";
                mail.Body = "Click the link below to reset your password:\n\n" +
                            "https://localhost:44383/User/ResetPassword?email=" + email + "&resetCode=" + resetCode;

                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential("ngonguyenvupl93@gmail.com", "xtet syaj faku tuxf");
                SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);
                Console.WriteLine("Reset password email sent successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending reset password email: " + ex.Message);
            }
        }







        public ActionResult Logout()
        {
            // Xóa thông tin đăng nhập Facebook từ Session (hoặc cơ chế lưu trữ khác nếu có)
            Session.Remove("FacebookUserId");
            Session.Remove("FacebookAccessToken");

            // Xóa các thông tin khác liên quan đến đăng nhập nếu cần
            Session.Clear();

            return RedirectToAction("Index", "Home"); // Chuyển hướng về trang chủ hoặc trang đăng nhập
        }



        [HttpGet]
        public ActionResult FacebookLogin()
        {
            var fb = new FacebookClient();
            var loginUrl = fb.GetLoginUrl(new
            {
                client_id = "359377490109099",
                client_secret = "9739302cddd30158d8ee21a01a2875ab",
                redirect_uri = Url.Action("FacebookCallback", "User", null, Request.Url.Scheme),
                response_type = "code",
                scope = "email,public_profile",
                auth_type = "reauthenticate",
            });

            return Redirect(loginUrl.AbsoluteUri);
        }

        [HttpGet]
        public ActionResult FacebookCallback(string code)
        {
            var fb = new FacebookClient();

          
            
                // Exchange the Facebook code for an access token
                dynamic result = fb.Post("oauth/access_token", new
                {
                    client_id = "359377490109099",
                    client_secret = "9739302cddd30158d8ee21a01a2875ab",
                    redirect_uri = Url.Action("FacebookCallback", "User", null, Request.Url.Scheme),
                    code = code
                });

                var accessToken = result.access_token;
                fb.AccessToken = accessToken;

                // Retrieve user information from Facebook
                dynamic me = fb.Get("me?fields=id,email,name");

                // Extract user details
                string facebookId = me.id;
                string email = me.email;
                string fullName = me.name;

                // Check if the user with this Facebook ID already exists in the database
                User existingUser = db.Users.SingleOrDefault(n => n.FacebookUserId == facebookId);

                if (existingUser != null)
                {
                    // Log in the existing user
                    Session["TaiKhoan"] = existingUser;
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    // Create a new user in the database
                    User newUser = new User
                    {
                        FacebookUserId = facebookId,
                        Email = email,
                        VaiTro = 1, // Assuming the user role is 1 for regular users
                        HoTen = fullName,
                        MatKhau = GenerateUniquePassword() // You may want to generate a unique password for Facebook users
                    };

                    db.Users.InsertOnSubmit(newUser);
                    db.SubmitChanges();

                    Session["TaiKhoan"] = newUser;
                    return RedirectToAction("Index", "Home");
                }
            
           
        }

        private string GenerateUniquePassword()
        {
            // You can customize this method to generate a unique and secure password
            // This is just a basic example; consider using a more robust password generation approach

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new Random();
            int length = 12; // You can adjust the length of the generated password

            // Generate a random password with the specified length
            string password = new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            return password;
        }


        [AllowAnonymous]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Redirect to Google for authentication
            var redirectUri = Url.Action("ExternalLoginCallback", "User", new { ReturnUrl = returnUrl }, Request.Url.Scheme);
            var googleAuthenticationUrl = GetGoogleAuthenticationUrl(redirectUri);
            return Redirect(googleAuthenticationUrl);
        }

        private string GetGoogleAuthenticationUrl(string redirectUri)
        {
            // Replace with your actual Google API credentials
            var clientId = "14128625655-5bq43rddq38qquuhc51pd5v19be3ldlt.apps.googleusercontent.com";
            var scope = "openid email profile"; // Adjust the scope as needed

            return $"https://accounts.google.com/o/oauth2/auth?client_id={clientId}&redirect_uri={redirectUri}&response_type=code&scope={scope}";
        }

        public ActionResult ExternalLoginCallback(string returnUrl)
        {
            var code = Request.QueryString["code"];
            if (code == null)
            {
                // Handle the error accordingly
                return RedirectToAction("Login");
            }

            var accessToken = GetGoogleAccessToken(code);
            var userDetails = GetGoogleUserDetails(accessToken);

            if (userDetails != null)
            {
                // Extract user details from the Google response
                string googleId = userDetails.id;
                string email = userDetails.email;
                string fullName = userDetails.name;

                // Check if the user with this Google ID already exists in the database
                User existingUser = db.Users.SingleOrDefault(n => n.GoogleID == googleId);

                if (existingUser != null)
                {
                    // Log in the existing user
                    Session["TaiKhoan"] = existingUser;
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    // Check if a user with the same email already exists
                    existingUser = db.Users.SingleOrDefault(n => n.Email == email);

                    if (existingUser != null)
                    {
                        // Handle the case when a user with the same email already exists
                        // You may want to log in the existing user or display an error message
                        Session["TaiKhoan"] = existingUser;
                        return RedirectToAction("Index", "Home");
                    }

                    // Create a new user in the database
                    User newUser = new User
                    {
                        GoogleID = googleId,
                        Email = email,
                        VaiTro = 1, // Assuming the user role is 1 for regular users
                        HoTen = fullName,
                        MatKhau = GenerateUniquePassword() // You may want to generate a unique password for Google users
                    };

                    db.Users.InsertOnSubmit(newUser);
                    db.SubmitChanges();

                    Session["TaiKhoan"] = newUser;
                    return RedirectToAction("Index", "Home");
                }
            }

            // Handle the case when user details retrieval fails
            return RedirectToAction("Login");
        }


        private string GetGoogleAccessToken(string code)
        {
            // Replace with your actual Google API credentials
            var clientId = "14128625655-5bq43rddq38qquuhc51pd5v19be3ldlt.apps.googleusercontent.com";
            var clientSecret = "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";
            var redirectUri = Url.Action("ExternalLoginCallback", "User", null, Request.Url.Scheme);

            var tokenUrl = "https://accounts.google.com/o/oauth2/token";
            var tokenRequestData = $"code={code}&client_id={clientId}&client_secret={clientSecret}&redirect_uri={redirectUri}&grant_type=authorization_code";

            using (var client = new WebClient())
            {
                client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                var response = client.UploadString(tokenUrl, "POST", tokenRequestData);
                dynamic result = Newtonsoft.Json.JsonConvert.DeserializeObject(response);
                return result.access_token;
            }
        }

        private dynamic GetGoogleUserDetails(string accessToken)
        {
            var userInfoUrl = "https://www.googleapis.com/oauth2/v1/userinfo";
            using (var client = new WebClient())
            {
                var json = client.DownloadString($"{userInfoUrl}?access_token={accessToken}");
                return Newtonsoft.Json.JsonConvert.DeserializeObject(json);
            }
        }


        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(FormCollection f, User kh)
        {
            // Retrieve form data
            var sHoTen = f["HoTen"];
            var sTaiKhoan = f["TaiKhoan"];
            var sMatKhau = f["MatKhau"]; // Mật khẩu chưa được mã hóa
            var sMatKhauNL = f["MatKhauNL"];
            var sDiaChi = f["DiaChi"];
            var sEmail = f["Email"];
            var sDienThoai = f["DienThoai"];
            var dNgaySinh = String.Format("{0:MM/dd/yyyy}", f["NgaySinh"]);

            // Check if the username or email already exists
            if (db.Users.SingleOrDefault(n => n.TaiKhoan == sTaiKhoan) != null)
            {
                ViewBag.ThongBao = "Tên đăng nhập đã tồn tại";
            }
            else if (db.Users.SingleOrDefault(n => n.Email == sEmail) != null)
            {
                ViewBag.ThongBao = "Email đã tồn tại";
            }
            else if (ModelState.IsValid)
            {
                // Encrypt the password before saving to the database
                kh.MatKhau = HashPassword(sMatKhau);

                // Populate the User object with form data
                kh.HoTen = sHoTen;
                kh.TaiKhoan = sTaiKhoan;
                kh.Email = sEmail;
                kh.DiaChi = sDiaChi;
                kh.DienThoai = sDienThoai;
                kh.NgaySinh = DateTime.Parse(dNgaySinh);
                kh.VaiTro = 1;

                // Insert the User object into the database
                db.Users.InsertOnSubmit(kh);
                db.SubmitChanges();

                return Redirect("~/User/Account");
            }

            return View("Account");
        }
        // Method to encrypt the password using SHA-256
        // Method to hash the password using SHA-256
        private string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Convert the input string to a byte array and compute the hash
                byte[] data = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Create a new StringBuilder to collect the bytes
                StringBuilder stringBuilder = new StringBuilder();

                // Loop through each byte of the hashed data and format each one as a hexadecimal string
                for (int i = 0; i < data.Length; i++)
                {
                    stringBuilder.Append(data[i].ToString("x2"));
                }

                // Return the hexadecimal string
                return stringBuilder.ToString();
            }
        }




        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ConfirmEmailMK()
        {
            return View();
        }

        public ActionResult ResetConfirm()
        {
            return View();
        }

    }
}