using System;
using System.Collections.Generic;

#nullable disable

namespace gRPCRapChieuPhim.Models
{
    public partial class ThanhVien
    {
        public int Id { get; set; }
        public string HoTen { get; set; }
        public bool GioiTinh { get; set; }
        public DateTime NgaySinh { get; set; }
        public string Email { get; set; }
        public string DienThoai { get; set; }
        public string MatKhau { get; set; }
    }
}
