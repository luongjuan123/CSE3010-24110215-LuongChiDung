using System;

namespace DTO
{
    public class NhanVienViewModel
    {
        public string MaNV { get; set; }
        public string HoTen { get; set; }
        public bool GioiTinh { get; set; }
        public string GioiTinhStr => GioiTinh ? "Nam" : "Nữ";
        public DateTime NgaySinh { get; set; }
        public string NgaySinhStr => NgaySinh.ToString("dd/MM/yyyy");
        public string DiaChi { get; set; }
        public decimal Luong { get; set; }
        public string LuongStr => Luong.ToString("N0");
        public string MaPB { get; set; }
        public string TenPB { get; set; }
    }
}
