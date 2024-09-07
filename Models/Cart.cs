using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebsiteGiayKingShoes.Models
{
    public class Cart
    {
        dbDataContext db = new dbDataContext();

        public int iMaSP { get; set; }
        public string sTenSP { get; set; }
        public string sAnhBia { get; set; }
        public string sSize { get; set; }
        public string sColor { get; set; }
        public double dDonGia { get; set; }
        public int iSoLuong { get; set; }
        public double dThanhTien
        {
            get { return dDonGia * iSoLuong; }
        }

        public Cart(int ms)
        {
            iMaSP = ms;
            SANPHAM s = db.SANPHAMs.Single(n => n.MaSP == iMaSP);
            sTenSP = s.TenSP;
            sAnhBia = s.AnhBia;
            sColor = s.color;
            dDonGia = double.Parse(s.GiaBan.ToString());
            iSoLuong = 1;
            iSoLuong = 1;
            SizeSanPham t = db.SizeSanPhams.SingleOrDefault(n => n.MaSize == iMaSP);

            if (t != null)
            {
                sSize = t.Size;
            }
            else
            {
                // Handle the case where no matching element is found.
                // You can throw an exception, set a default value, or handle it based on your application logic.
                // Example: throw new InvalidOperationException("No matching element found for MaSize: " + iMaSP);
            }
        }
    }
}