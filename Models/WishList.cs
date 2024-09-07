using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebsiteGiayKingShoes.Models
{
    public class WishList
    {
        dbDataContext db = new dbDataContext();
        public int iMaSP { get; set; }
        public string sTenSP { get; set; }
        public string sAnhBia { get; set; }
        public double dDonGia { get; set; }
        public int iSoLuong { get; set; }
        public string sTenTrangThai { get; set; }
        public WishList(int ms) 
        {
            iMaSP = ms;
            SANPHAM s = db.SANPHAMs.Single( n => n.MaSP == iMaSP);
            sTenSP = s.TenSP;
            sAnhBia = s.AnhBia;
            dDonGia = double.Parse(s.GiaBan.ToString());
            iSoLuong = 1;
            if (s.TrangThai.HasValue)
            {
                int maTrangThai = (int)s.TrangThai;
                // Thực hiện truy vấn để lấy tên trạng thái từ bảng Trạng thái
                var trangThai = db.TrangThais.Single(tt => tt.MaTT == maTrangThai);
                sTenTrangThai = trangThai.TenTT;
            }
            else
            {
                // Xử lý khi trạng thái không có giá trị
                sTenTrangThai = "Không xác định";
            }
        }
    }
}