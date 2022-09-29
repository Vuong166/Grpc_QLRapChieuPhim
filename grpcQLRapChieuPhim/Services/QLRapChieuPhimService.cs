using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using grpcQLRapChieuPhim.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace grpcQLRapChieuPhim.Services
{
    public class QLRapChieuPhimService : RapChieuPhim.RapChieuPhimBase
    {
        private readonly QLRapChieuPhimContext _context;
        public QLRapChieuPhimService(QLRapChieuPhimContext context)
        {
            _context = context;
        }

        public override Task<TheLoaiOuput> DocDanhSachTheLoai(Empty request, ServerCallContext context)
        {
            var dsTheLoais = _context.TheLoaiPhims.Select(t => new ThongTinTheLoai
            {
                Id = t.Id,
                Ten = t.Ten
            }).ToList();            

            TheLoaiOuput response = new TheLoaiOuput();
            response.DanhSachTheLoai.AddRange(dsTheLoais);

            try
            {
                return Task.FromResult(response);
            }
            catch (Exception ex)
            {
                throw new RpcException(Status.DefaultCancelled, ex.Message);
            }
        }

        public override Task<XepHangPhimOutput> DocDanhSachXepHangPhim(Empty request, ServerCallContext context)
        {
            var dsXephangs = _context.XepHangPhims.Select(t => new ThongTinXepHangPhim
            {
                Id = t.Id,
                Ten = t.Ten,
                KyHieu = t.KyHieu
            }).ToList();

            XepHangPhimOutput response = new XepHangPhimOutput();
            response.DanhSachXepHang.AddRange(dsXephangs);

            try
            {
                return Task.FromResult(response);
            }
            catch (Exception ex)
            {
                throw new RpcException(Status.DefaultCancelled, ex.Message);
            }
        }

        public override Task<PhimTheoTheLoaiOutput> DocPhimTheoTheLoai(PhimTheoTheLoaiInput request, ServerCallContext context)
        {
            var theloaiid = "," + request.TheLoaiId + ",";
            var dsPhims = _context.Phims.Where(p => p.DanhSachTheLoaiId.Contains(theloaiid))
                .Select(p => new ThongTinPhim
                {
                    Id = p.Id,
                    TenPhim = p.TenPhim,
                    TenGoc = p.TenGoc == null ? "" : p.TenGoc,
                    ThoiLuong = p.ThoiLuong,
                    DaoDien = p.DaoDien == null ? "" : p.DaoDien,
                    DienVien = p.DienVien == null ? "" : p.DienVien,
                    NgayKhoiChieu = p.NgayKhoiChieu == null ? Timestamp.FromDateTimeOffset(DateTime.Now.AddYears(10)) : Timestamp.FromDateTimeOffset(p.NgayKhoiChieu.Value),
                    NuocSanXuat = p.NuocSanXuat == null ? "" : p.NuocSanXuat,
                    NhaSanXuat = p.NhaSanXuat == null ? "" : p.NhaSanXuat,
                    Poster = p.Poster,
                    DanhSachTheLoaiId = p.DanhSachTheLoaiId,
                    NgonNgu = p.NgonNgu == null ? "" : p.NgonNgu,
                    NoiDung = p.NoiDung,
                    Trailer = p.Trailer == null ? "" : p.Trailer,
                    XepHangPhimId = p.XepHangPhimId.Value
                }).ToList();
            PhimTheoTheLoaiOutput response = new PhimTheoTheLoaiOutput();
            response.DanhSachPhim.AddRange(dsPhims);
            try
            {
                return Task.FromResult(response);
            }
            catch (Exception ex)
            {
                throw new RpcException(Status.DefaultCancelled, ex.Message);
            }
        }

        public override Task<ThongTinPhimOutput> DocThongTinPhim(ThongTinPhimInput request, ServerCallContext context)
        {
            ThongTinPhimOutput response = new ThongTinPhimOutput();
            response.Phim = new ThongTinPhim();

            if (request.PhimId > 0)
            {
                //Đọc phim theo id
                var phim = _context.Phims.FirstOrDefault(x => x.Id.Equals(request.PhimId));
                if (phim != null)
                {
                    response.Phim = new ThongTinPhim
                    {
                        Id = phim.Id,
                        TenPhim = phim.TenPhim,
                        TenGoc = phim.TenGoc,
                        DanhSachTheLoaiId = phim.DanhSachTheLoaiId,
                        DaoDien = phim.DaoDien,
                        DienVien = phim.DienVien,
                        NgayKhoiChieu = phim.NgayKhoiChieu == null ?
                                        Timestamp.FromDateTimeOffset(DateTime.Now.AddYears(10)) :
                                        Timestamp.FromDateTimeOffset(phim.NgayKhoiChieu.Value),
                        NgonNgu = phim.NgonNgu,
                        NhaSanXuat = phim.NhaSanXuat,
                        NoiDung = phim.NoiDung,
                        NuocSanXuat = phim.NuocSanXuat,
                        Poster = phim.Poster,
                        ThoiLuong = phim.ThoiLuong,
                        Trailer = phim.Trailer,
                        XepHangPhimId = phim.XepHangPhimId.Value
                    };
                }
            }
            return Task.FromResult(response);
        }

        public override Task<ThongBaoOutput> ThemPhimMoi(ThemPhimMoiInput request, ServerCallContext context)
        {
            ThongBaoOutput response; // = new ThongBaoOutput();
            var phim = request.Phim;
            if (string.IsNullOrEmpty(phim.TenPhim))
                response = new ThongBaoOutput { MaSoLoi = 1, NoiDungThongBao = "Tên phim không được rỗng" };
            else if (phim.ThoiLuong <=0 )
                response = new ThongBaoOutput { MaSoLoi = 2, NoiDungThongBao = "Thời lượng phải > 0" };
            else
            {
                try
                {
                    var phimmoi = new Phim
                    {
                        TenPhim = phim.TenPhim,
                        TenGoc = phim.TenGoc,
                        DanhSachTheLoaiId = phim.DanhSachTheLoaiId,
                        DaoDien = phim.DaoDien,
                        DienVien = phim.DienVien,
                        NgayKhoiChieu = phim.NgayKhoiChieu.ToDateTime().Year >= DateTime.Now.Year + 10 ? null : phim.NgayKhoiChieu.ToDateTime(),
                        NgonNgu = phim.NgonNgu,
                        NhaSanXuat = phim.NhaSanXuat,
                        NoiDung = phim.NoiDung,
                        NuocSanXuat = phim.NuocSanXuat,
                        Poster = phim.Poster,
                        ThoiLuong = phim.ThoiLuong,
                        Trailer = phim.Trailer,
                        XepHangPhimId = phim.XepHangPhimId
                    };
                    _context.Phims.Add(phimmoi);
                    _context.SaveChanges();
                    response = new ThongBaoOutput { MaSoLoi = 0, NoiDungThongBao = "Thêm phim mới thành công. " };
                }
                catch (Exception ex)
                {
                    response = new ThongBaoOutput { MaSoLoi = 3, NoiDungThongBao = "Lỗi thêm phim: " + ex.Message };
                }
            }
            return Task.FromResult(response);
        }

        public override Task<ThongBaoOutput> CapNhatPhim(CapNhatPhimInput request, ServerCallContext context)
        {
            ThongBaoOutput response; // = new ThongBaoOutput();
            var phim = request.Phim;

            if (phim.Id <= 0)
                response = new ThongBaoOutput { MaSoLoi = 1, NoiDungThongBao = "Thông tin phim cần cập nhật không hợp lệ" };
            else
            {
                try
                {
                    var phimcapnhat = _context.Phims.FirstOrDefault(x => x.Id.Equals(phim.Id));
                    if(phimcapnhat != null)
                    {
                        phimcapnhat.TenPhim = phim.TenPhim;
                        phimcapnhat.TenGoc = phim.TenGoc;
                        phimcapnhat.DanhSachTheLoaiId = phim.DanhSachTheLoaiId;
                        phimcapnhat.DaoDien = phim.DaoDien;
                        phimcapnhat.DienVien = phim.DienVien;
                        phimcapnhat.NgayKhoiChieu = phim.NgayKhoiChieu.ToDateTime().Year >= DateTime.Now.Year + 10 ? null : phim.NgayKhoiChieu.ToDateTime();
                        phimcapnhat.NgonNgu = phim.NgonNgu;
                        phimcapnhat.NhaSanXuat = phim.NhaSanXuat;
                        phimcapnhat.NoiDung = phim.NoiDung;
                        phimcapnhat.NuocSanXuat = phim.NuocSanXuat;
                        phimcapnhat.Poster = phim.Poster;
                        phimcapnhat.ThoiLuong = phim.ThoiLuong;
                        phimcapnhat.Trailer = phim.Trailer;
                        phimcapnhat.XepHangPhimId = phim.XepHangPhimId;
                        _context.SaveChanges();
                        response = new ThongBaoOutput { MaSoLoi = 0, NoiDungThongBao = "Cập nhật thông tin phim thành công. " };
                    }                    
                    else
                        response = new ThongBaoOutput { MaSoLoi = 2, NoiDungThongBao = "Không tìm thấy phim cần cập nhật." };
                }
                catch (Exception ex)
                {
                    response = new ThongBaoOutput { MaSoLoi = 3, NoiDungThongBao = "Lỗi cập nhật phim: " + ex.Message };
                }
            }

            return Task.FromResult(response);
        }

        public override Task<ThongBaoOutput> XoaPhim(XoaPhimInput request, ServerCallContext context)
        {
            ThongBaoOutput response;
            if(request.PhimId > 0)
            {
                var phim = _context.Phims.FirstOrDefault(x => x.Id.Equals(request.PhimId));
                if(phim != null)
                {
                    var lichchieu = _context.LichChieus.FirstOrDefault(x => x.PhimId.Equals(request.PhimId));
                    if (lichchieu != null)
                        response = new ThongBaoOutput { MaSoLoi = 1, NoiDungThongBao = "Phim này đã có lịch chiếu. Không hủy được." };
                    else
                    {
                        _context.Phims.Remove(phim);
                        _context.SaveChanges();
                        response = new ThongBaoOutput { MaSoLoi = 0, NoiDungThongBao = "Đã hủy phim thành công." };
                    }
                }
                else
                    response = new ThongBaoOutput { MaSoLoi = 2, NoiDungThongBao = "Không tìm thấy phim cần hủy." };
            }
            else
                response = new ThongBaoOutput { MaSoLoi = 3, NoiDungThongBao = "Id của phim cần hủy không hợp lệ." };
            return Task.FromResult(response);
        }
    }
}
