using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using gRPCRapChieuPhim.Common;
using gRPCRapChieuPhim.Models;
using Microsoft.Extensions.Logging;

namespace gRPCRapChieuPhim.Services
{
    public class QLRapChieuPhimService : RapChieuPhim.RapChieuPhimBase
    {
        private readonly QLRapChieuPhimContext _context;
        private readonly ILogger<QLRapChieuPhimService> _logger;
        public QLRapChieuPhimService(ILogger<QLRapChieuPhimService> logger, QLRapChieuPhimContext context)
        {
            _logger = logger;
            _context = context;
        }




        public override Task<Output.Types.TheLoaiPhims> DanhSachTheLoai(Input.Types.Empty request, ServerCallContext context)
        {

            Output.Types.TheLoaiPhims responseData = new Output.Types.TheLoaiPhims();
            try
            {
                var dsTheLoai = _context.TheLoaiPhims.Select(x => new Output.Types.TheLoaiPhim { Id = x.Id, Ten = x.Ten });
                responseData.Items.AddRange(dsTheLoai);
            }
            catch (Exception ex)
            {
                responseData.ThongBao = "Lỗi: " + ex.Message;
            }
            return Task.FromResult(responseData);
        }




        public override Task<Output.Types.XepHangPhims> DanhSachXepHangPhim(Input.Types.Empty request, 
                    ServerCallContext context)
        {
            
            Output.Types.XepHangPhims responseData = new Output.Types.XepHangPhims();
            try
            {
                //Xử lý trả về danh sách xếp hạng phim
                var dsXepHang = _context.XepHangPhims.Select(x => new Output.Types.XepHangPhim
                {
                    Id = x.Id,
                    Ten = x.Ten,
                    KyHieu = x.KyHieu
                });
                responseData.Items.AddRange(dsXepHang);
            }
            catch (Exception ex)
            {
                responseData.ThongBao = "Lỗi: " + ex.Message;
            }
            return Task.FromResult(responseData);
        }




        public override Task<Output.Types.Phims> DanhSachPhimTheoTheLoai(Input.Types.PhimTheoTheLoai request, 
            ServerCallContext context)
        {
            //Xử lý trả về danh sách phim theo thể loại được chọn
            var theLoais = _context.TheLoaiPhims.ToList();
            Output.Types.TheLoaiPhim theLoaiHienHanh;
            List<Models.Phim> dsPhimTheoTheLoai = null;
            if (request.TheLoaiId > 0)
            {
                var theLoai = "," + request.TheLoaiId.ToString() + ",";
                var tl = theLoais.FirstOrDefault(t => t.Id.Equals(request.TheLoaiId));
                theLoaiHienHanh = new Output.Types.TheLoaiPhim { Id = tl.Id, Ten = tl.Ten };
                dsPhimTheoTheLoai = _context.Phims
                    .Where(x => x.DanhSachTheLoaiId.Contains(theLoai)).ToList();
            }
            else
            {
                theLoaiHienHanh = new Output.Types.TheLoaiPhim();
                dsPhimTheoTheLoai = _context.Phims.ToList();
            }

            float numberpage = (float)dsPhimTheoTheLoai.Count() / request.PageSize;
            int pageCount = (int)Math.Ceiling(numberpage);
            int currentPage = request.CurrentPage;
            if (currentPage > pageCount) currentPage = pageCount;

            dsPhimTheoTheLoai = dsPhimTheoTheLoai
                .Skip((currentPage - 1) * request.PageSize)
                .Take(request.PageSize).ToList();

            Output.Types.Phims responseData = new Output.Types.Phims();
            if (dsPhimTheoTheLoai.Count > 0)
            {
                var dsPhim = dsPhimTheoTheLoai
                .Select(x => new Output.Types.Phim
                {
                    Id = x.Id,
                    TenPhim = x.TenPhim,
                    TenGoc = string.IsNullOrEmpty(x.TenGoc) ? "" : x.TenGoc,
                    NgayKhoiChieu = x.NgayKhoiChieu == null ?
                        Timestamp.FromDateTimeOffset(new DateTime(1900, 1, 1)) :
                        Timestamp.FromDateTimeOffset(x.NgayKhoiChieu.Value),
                    DanhSachTheLoaiId = x.DanhSachTheLoaiId,
                    DaoDien = x.DaoDien ?? "",
                    DienVien = x.DienVien ?? "",
                    XepHangPhimId = x.XepHangPhimId ?? 0,
                    NgonNgu = x.NgonNgu ?? "",
                    NhaSanXuat = x.NhaSanXuat ?? "",
                    NoiDung = x.NoiDung ?? "",
                    NuocSanXuat = x.NuocSanXuat ?? "",
                    Poster = x.Poster ?? "",
                    ThoiLuong = x.ThoiLuong,
                    Trailer = x.Trailer ?? ""
                });

                try
                {
                    responseData.Items.AddRange(dsPhim);
                    responseData.PageCount = pageCount;
                    responseData.TheLoaiHienHanh = theLoaiHienHanh;
                }
                catch (Exception ex)
                {
                    responseData.ThongBao = "Lỗi: " + ex.Message;
                }
            }
            else
            {
                responseData.ThongBao = "Lỗi: Không tìm thấy thông tin phim";
            }
            return Task.FromResult(responseData);
        }




        public override Task<Output.Types.Phim> XemThongTinPhim(Input.Types.Phim request, 
                ServerCallContext context)
        {
            if(request.Id > 0) {
                try {
                    var phim = _context.Phims.FirstOrDefault(x => x.Id.Equals(request.Id));
                    Output.Types.Phim thongtinPhim = null;
                    if (phim != null)
                    {
                        //Gán thông tin của phim cho thongtinPhim
                        //...   
                        thongtinPhim = new Output.Types.Phim
                        {
                            Id = phim.Id,
                            TenPhim = phim.TenPhim,
                            TenGoc = string.IsNullOrEmpty(phim.TenGoc) ? "" : phim.TenGoc,
                            NgayKhoiChieu = phim.NgayKhoiChieu == null ? Timestamp.FromDateTimeOffset(new DateTime(1900, 1, 1)) : Timestamp.FromDateTimeOffset(phim.NgayKhoiChieu.Value),
                            DanhSachTheLoaiId = phim.DanhSachTheLoaiId,
                            DaoDien = phim.DaoDien ?? "",
                            DienVien = phim.DienVien ?? "",
                            XepHangPhimId = phim.XepHangPhimId ?? 0,
                            NgonNgu = phim.NgonNgu ?? "",
                            NhaSanXuat = phim.NhaSanXuat ?? "",
                            NoiDung = phim.NoiDung ?? "",
                            NuocSanXuat = phim.NuocSanXuat ?? "",
                            Poster = phim.Poster ?? "",
                            ThoiLuong = phim.ThoiLuong,
                            Trailer = phim.Trailer ?? ""
                        };
                    }
                    else
                    {
                        thongtinPhim = new Output.Types.Phim();
                    }
                    return Task.FromResult(thongtinPhim);
                }
                catch (Exception ex) {
                    return null;
                }                
            }
            else {
                return null;
            }
        }



        public override Task<Output.Types.ThongBao> ThemPhimMoi(Input.Types.Phim request, ServerCallContext context)
        {
            Output.Types.ThongBao tb = new Output.Types.ThongBao { Maso = 1 };
            try
            {
                var phimMoi = new Models.Phim
                {
                    //Gán thông tin từ request vào phimMoi
                    Id = request.Id,
                    TenPhim = request.TenPhim,
                    TenGoc = string.IsNullOrEmpty(request.TenGoc) ? "" : request.TenGoc,
                    NgayKhoiChieu = request.NgayKhoiChieu == null ? null : request.NgayKhoiChieu.ToDateTime(),
                    DanhSachTheLoaiId = request.DanhSachTheLoaiId,
                    DaoDien = request.DaoDien,
                    DienVien = request.DienVien,
                    XepHangPhimId = request.XepHangPhimId,
                    NgonNgu = request.NgonNgu,
                    NhaSanXuat = request.NhaSanXuat,
                    NoiDung = request.NoiDung,
                    NuocSanXuat = request.NuocSanXuat,
                    Poster = request.Poster,
                    ThoiLuong = request.ThoiLuong,
                    Trailer = request.Trailer
                };
                var chuoiTB = "";
                if (string.IsNullOrEmpty(phimMoi.TenPhim))
                    chuoiTB = "Tên phim phải khác rỗng";
                if (phimMoi.ThoiLuong <= 0)
                    chuoiTB += "Thời lượng phim phải > 0";

                if (string.IsNullOrEmpty(chuoiTB))
                {
                    _context.Phims.Add(phimMoi);
                    int kq = _context.SaveChanges();
                    if (kq > 0)
                    {
                        tb.Maso = 0;
                        chuoiTB = "Lưu thông tin phim mới thành công";
                    }
                    else
                        chuoiTB = "Lưu thông tin phim mới không thành công";
                }

                tb.NoiDung = chuoiTB;
            }
            catch (Exception ex)
            {
                tb.NoiDung = "Lỗi: " + ex.Message;
            }
            return Task.FromResult(tb);
        }




        public override Task<Output.Types.ThongBao> CapNhatPhim(Input.Types.Phim request, ServerCallContext context)
        {
            Output.Types.ThongBao tb = new Output.Types.ThongBao { Maso = 1 };
            try {
                var phimCapNhat = _context.Phims.FirstOrDefault(p => p.Id.Equals(request.Id));
                if (phimCapNhat != null)
                {
                    //Gán thông tin từ request vào phimCapNhat
                    phimCapNhat.TenPhim = request.TenPhim;
                    phimCapNhat.TenGoc = string.IsNullOrEmpty(request.TenGoc) ? "" : request.TenGoc;
                    phimCapNhat.NgayKhoiChieu = request.NgayKhoiChieu == null ? null : request.NgayKhoiChieu.ToDateTime();
                    phimCapNhat.DanhSachTheLoaiId = request.DanhSachTheLoaiId;
                    phimCapNhat.DaoDien = request.DaoDien;
                    phimCapNhat.DienVien = request.DienVien;
                    phimCapNhat.XepHangPhimId = request.XepHangPhimId;
                    phimCapNhat.NgonNgu = request.NgonNgu;
                    phimCapNhat.NhaSanXuat = request.NhaSanXuat;
                    phimCapNhat.NoiDung = request.NoiDung;
                    phimCapNhat.NuocSanXuat = request.NuocSanXuat;
                    phimCapNhat.Poster = request.Poster;
                    phimCapNhat.ThoiLuong = request.ThoiLuong;
                    phimCapNhat.Trailer = request.Trailer;

                    var chuoiTB = "";
                    if (string.IsNullOrEmpty(phimCapNhat.TenPhim))
                        chuoiTB = "Tên phim phải khác rỗng";
                    if (phimCapNhat.ThoiLuong <= 0)
                        chuoiTB += "Thời lượng phim phải > 0";

                    if (string.IsNullOrEmpty(chuoiTB))
                    {
                        int kq = _context.SaveChanges();
                        if (kq > 0)
                        {
                            tb.Maso = 0;
                            chuoiTB = "Lưu thông tin phim mới thành công";
                        }
                        else
                            chuoiTB = "Lưu thông tin phim mới không thành công";
                    }
                    tb.NoiDung = chuoiTB;
                }
            }
            catch (Exception ex) {
                tb.NoiDung = "Lỗi: " + ex.Message;
            }
            return Task.FromResult(tb);
        }



        public override Task<Output.Types.Phims> TimPhim(Input.Types.TimPhim request, ServerCallContext context)
        {
            var theLoais = _context.TheLoaiPhims.ToList();
            List<Models.Phim> dsPhimTimKiem = new List<Models.Phim>();
            if (!string.IsNullOrEmpty(request.KeyWord))
            {
                var tukhoas = request.KeyWord.Split(new string[] { "," },
                                    StringSplitOptions.RemoveEmptyEntries);
                if (tukhoas.Count() > 0)
                {
                    foreach (var tk in tukhoas)
                    {
                        var phimTimKiem = _context.Phims.Where(x => x.TenPhim.Contains(tk)
                            || x.TenGoc.Contains(tk) || x.DaoDien.Contains(tk)
                            || x.DienVien.Contains(tk)).ToList();
                        if (phimTimKiem != null && phimTimKiem.Count() > 0)
                            dsPhimTimKiem.AddRange(phimTimKiem);
                    }
                }
            }

            float numberpage = (float)dsPhimTimKiem.Count() / request.PageSize;
            int pageCount = (int)Math.Ceiling(numberpage);
            int currentPage = request.CurrentPage;
            if (currentPage > pageCount) currentPage = pageCount;

            //var xepHangs = _context.XepHangPhims.ToList();
            dsPhimTimKiem = dsPhimTimKiem.Skip((currentPage - 1) * request.PageSize)
                                         .Take(request.PageSize).ToList();

            Output.Types.Phims responseData = new Output.Types.Phims();
            if (dsPhimTimKiem.Count > 0)
            {
                var dsPhim = dsPhimTimKiem
                .Select(x => new Output.Types.Phim
                {
                    //Gán giá trị cho các thuộc tính của phim
                    Id = x.Id,
                    TenPhim = x.TenPhim,
                    TenGoc = string.IsNullOrEmpty(x.TenGoc) ? "" : x.TenGoc,
                    NgayKhoiChieu = x.NgayKhoiChieu == null ? Timestamp.FromDateTimeOffset(new DateTime(1900, 1, 1)) : Timestamp.FromDateTimeOffset(x.NgayKhoiChieu.Value),
                    DanhSachTheLoaiId = x.DanhSachTheLoaiId,
                    DaoDien = x.DaoDien ?? "",
                    DienVien = x.DienVien ?? "",
                    XepHangPhimId = x.XepHangPhimId ?? 0,
                    NgonNgu = x.NgonNgu ?? "",
                    NhaSanXuat = x.NhaSanXuat ?? "",
                    NoiDung = x.NoiDung ?? "",
                    NuocSanXuat = x.NuocSanXuat ?? "",
                    Poster = x.Poster ?? "",
                    ThoiLuong = x.ThoiLuong,
                    Trailer = x.Trailer ?? ""
                });

                try
                {
                    responseData.Items.AddRange(dsPhim);
                    responseData.PageCount = pageCount;
                }
                catch (Exception ex)
                {
                    responseData.ThongBao = "Lỗi: " + ex.Message;
                }
            }
            else
            {
                responseData.ThongBao = "Lỗi: Không tìm thấy thông tin phim";
            }
            return Task.FromResult(responseData);
        }



        public override Task<Output.Types.Phims> DanhSachPhimDangChieu(Input.Types.Empty request, ServerCallContext context)
        {
            int SoNgayChieu = Utilities.NumberOfWeekShow * 7;
            var dsPhimDangChieu = _context.Phims.Where(x=> x.NgayKhoiChieu != null && (x.NgayKhoiChieu <= DateTime.Today && x.NgayKhoiChieu.Value.AddDays(SoNgayChieu) >= DateTime.Today)).ToList();
            Output.Types.Phims responseData = new Output.Types.Phims();
            if (dsPhimDangChieu.Count > 0)
            {
                var dsPhim = dsPhimDangChieu
                .Select(x => new Output.Types.Phim
                {
                    //Gán giá trị cho các thuộc tính của phim
                    Id = x.Id,
                    TenPhim = x.TenPhim,
                    TenGoc = string.IsNullOrEmpty(x.TenGoc) ? "" : x.TenGoc,
                    NgayKhoiChieu = x.NgayKhoiChieu == null ? Timestamp.FromDateTimeOffset(new DateTime(1900, 1, 1)) : Timestamp.FromDateTimeOffset(x.NgayKhoiChieu.Value),
                    DanhSachTheLoaiId = x.DanhSachTheLoaiId,
                    DaoDien = x.DaoDien ?? "",
                    DienVien = x.DienVien ?? "",
                    XepHangPhimId = x.XepHangPhimId ?? 0,
                    NgonNgu = x.NgonNgu ?? "",
                    NhaSanXuat = x.NhaSanXuat ?? "",
                    NoiDung = x.NoiDung ?? "",
                    NuocSanXuat = x.NuocSanXuat ?? "",
                    Poster = x.Poster ?? "",
                    ThoiLuong = x.ThoiLuong,
                    Trailer = x.Trailer ?? ""
                });

                try
                {
                    responseData.Items.AddRange(dsPhim);
                }
                catch (Exception ex)
                {
                    responseData.ThongBao = "Lỗi: " + ex.Message;
                }
            }
            else
            {
                responseData.ThongBao = "Lỗi: Không có dữ liệu.";
            }
            return Task.FromResult(responseData);
        }



        public override Task<Output.Types.Phims> DanhSachPhimSapChieu(Input.Types.Empty request, ServerCallContext context)
        {
            int SoNgayChieu = Utilities.NumberOfWeekShow * 21;
            var dsPhimSapChieu = _context.Phims.Where(x => x.NgayKhoiChieu != null && x.NgayKhoiChieu > DateTime.Today).ToList();
            Output.Types.Phims responseData = new Output.Types.Phims();
            if (dsPhimSapChieu.Count > 0)
            {
                var dsPhim = dsPhimSapChieu
                .Select(x => new Output.Types.Phim
                {
                    //Gán giá trị cho các thuộc tính của phim
                    Id = x.Id,
                    TenPhim = x.TenPhim,
                    TenGoc = string.IsNullOrEmpty(x.TenGoc) ? "" : x.TenGoc,
                    NgayKhoiChieu = x.NgayKhoiChieu == null ? Timestamp.FromDateTimeOffset(new DateTime(1900, 1, 1)) : Timestamp.FromDateTimeOffset(x.NgayKhoiChieu.Value),
                    DanhSachTheLoaiId = x.DanhSachTheLoaiId,
                    DaoDien = x.DaoDien ?? "",
                    DienVien = x.DienVien ?? "",
                    XepHangPhimId = x.XepHangPhimId ?? 0,
                    NgonNgu = x.NgonNgu ?? "",
                    NhaSanXuat = x.NhaSanXuat ?? "",
                    NoiDung = x.NoiDung ?? "",
                    NuocSanXuat = x.NuocSanXuat ?? "",
                    Poster = x.Poster ?? "",
                    ThoiLuong = x.ThoiLuong,
                    Trailer = x.Trailer ?? ""
                });

                try
                {
                    responseData.Items.AddRange(dsPhim);
                }
                catch (Exception ex)
                {
                    responseData.ThongBao = "Lỗi: " + ex.Message;
                }
            }
            else
            {
                responseData.ThongBao = "Lỗi: Không có dữ liệu.";
            }
            return Task.FromResult(responseData);
        }
    }
}
