using System;
using System.Collections.Generic;

#nullable disable

namespace grpcQLRapChieuPhim.Models
{
    public partial class SuatChieu
    {
        public SuatChieu()
        {
            LichChieus = new HashSet<LichChieu>();
        }

        public int Id { get; set; }
        public string TenSuatChieu { get; set; }
        public string GioBatDau { get; set; }
        public string GioKetThuc { get; set; }

        public virtual ICollection<LichChieu> LichChieus { get; set; }
    }
}
