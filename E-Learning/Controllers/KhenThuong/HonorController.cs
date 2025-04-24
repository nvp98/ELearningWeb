using E_Learning.Models;
using E_Learning.ModelsQLKhenThuong;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_Learning.Controllers.KhenThuong
{
    public class HonorController : Controller
    {
        private readonly ELEARNINGEntities _context;

        public HonorController()
        {
            _context = new ELEARNINGEntities();
        }

        // GET: Honor
        public ActionResult Index()
        {
            var departments = _context.KT_DanhSachKhenThuong
                                .Where(x => x.MNV != null)
                                .Select(x => x.DonVi)
                                .Distinct()
                                .ToList();

            ViewBag.Departments = departments;

            List<CaNhanXepHang> danhSachCaNhan = _context.KT_DanhSachKhenThuong
                                    .Where(x => x.MNV != null)
                                    .GroupBy(x => new { x.MNV, x.HoTen, x.DonVi })
                                    .Select(g => new CaNhanXepHang
                                    {
                                        MNV = g.Key.MNV,
                                        HoTen = g.Key.HoTen,
                                        DonVi = g.Key.DonVi,
                                        SoLuongDeTai = g.Count(x => x.NoiDungKhenThuong != null)
                                    })
                                    .OrderByDescending(x => x.SoLuongDeTai)
                                    .ToList();

            ViewBag.DanhSachCaNhan = danhSachCaNhan;

            List<DonViXepHang> danhSachDonVi = _context.KT_DanhSachKhenThuong
                                    .Where(k => !string.IsNullOrEmpty(k.DonVi))
                                    .GroupBy(k => k.DonVi)
                                    .Select(g => new DonViXepHang
                                    {
                                        DonVi = g.Key,
                                        SoLuongDeTai = g.Select(k => k.NoiDungKhenThuong).Distinct().Count()
                                    })
                                    .OrderByDescending(x => x.SoLuongDeTai)
                                    .OrderByDescending(x => x.SoLuongDeTai)
                                    .ToList();
            
            ViewBag.DanhSachDonVi = danhSachDonVi;

            var today = DateTime.Today;
            var currentMonth = today.Month;
            var currentYear = today.Year;

            DateTime firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
            DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            DayOfWeek firstDayOfWeek = DayOfWeek.Monday;

            int offset = ((int)firstDayOfMonth.DayOfWeek - (int)firstDayOfWeek + 7) % 7;

            int weekOfMonth = ((today.Day + offset - 1) / 7) + 1;

            ViewBag.TuanHienTai = weekOfMonth;
            ViewBag.ThangHienTai = currentMonth;

            int daysToSubtract = (int)today.DayOfWeek == 0 ? 6 : (int)today.DayOfWeek - 1;
            DateTime startOfWeek = today.AddDays(-daysToSubtract);
            DateTime endOfWeek = startOfWeek.AddDays(6);

            if (startOfWeek.Month != currentMonth || startOfWeek.Year != currentYear)
            {
                startOfWeek = new DateTime(currentYear, currentMonth, 1);
                endOfWeek = startOfWeek.AddDays(6 - ((int)startOfWeek.DayOfWeek == 0 ? 6 : (int)startOfWeek.DayOfWeek - 1));
            }

            ViewBag.NgayBatDauTuan = startOfWeek.ToString("dd'/'MM");
            ViewBag.NgayKetThucTuan = endOfWeek.ToString("dd'/'MM");
            ViewBag.NgayBatDauThang = firstDayOfMonth.ToString("dd'/'MM");
            ViewBag.NgayKetThucThang = lastDayOfMonth.ToString("dd'/'MM");

            var khenThuongThangRaw = (from nd in _context.KT_NoiDungThuong
                                      join ds in _context.KT_DanhSachKhenThuong
                                      on nd.ID equals ds.ID_NoiDungThuong
                                      where nd.NgayQuyetDinh.HasValue &&
                                            nd.NgayQuyetDinh.Value.Month == currentMonth &&
                                            nd.NgayQuyetDinh.Value.Year == currentYear
                                      select new NoiDungKhenThuongDTO
                                      {
                                          NoiDungKhenThuong = nd.NoiDungKhenThuong,
                                          //BannerBase64 = nd.BannerImageBase64,
                                          DonVi = ds.DonVi,
                                          BannerImage = nd.BannerImage
                                      }).ToList();

            var khenThuongThang = khenThuongThangRaw
                .GroupBy(x => new { x.NoiDungKhenThuong, x.BannerImage})
                .Select(g => new NoiDungKhenThuongDTO
                {
                    NoiDungKhenThuong = g.Key.NoiDungKhenThuong,
                    BannerImage = g.Key.BannerImage,
                    DonVi = string.Join(", ", g
                                .Select(x => x.DonVi)
                                .Where(dv => !string.IsNullOrEmpty(dv))
                                .Distinct())
                })
                .ToList();

            ViewBag.KhenThuongThang = khenThuongThang;

            if (startOfWeek.Month != currentMonth)
            {
                startOfWeek = new DateTime(currentYear, currentMonth, 1);
            }

            if (endOfWeek.Month != currentMonth)
            {
                endOfWeek = new DateTime(currentYear, currentMonth, DateTime.DaysInMonth(currentYear, currentMonth));
            }

            var khenThuongTuanRaw = (from ds in _context.KT_NoiDungThuong
                                     join nd in _context.KT_DanhSachKhenThuong
                                     on ds.ID equals nd.ID_NoiDungThuong
                                     where ds.NgayQuyetDinh.HasValue &&
                                           ds.NgayQuyetDinh.Value >= startOfWeek &&
                                           ds.NgayQuyetDinh.Value <= endOfWeek
                                     select new NoiDungKhenThuongDTO
                                     {
                                         NoiDungKhenThuong = ds.NoiDungKhenThuong,
                                         //BannerBase64 = ds.BannerImageBase64,
                                         DonVi = nd.DonVi,
                                         BannerImage = ds.BannerImage
                                     }).ToList();

            var khenThuongTuan = khenThuongTuanRaw
                .GroupBy(x => new { x.NoiDungKhenThuong, x.BannerImage })
                .Select(g => new NoiDungKhenThuongDTO
                {
                    NoiDungKhenThuong = g.Key.NoiDungKhenThuong,
                    BannerImage = g.Key.BannerImage,
                    DonVi = string.Join(", ", g
                                .Select(x => x.DonVi)
                                .Where(dv => !string.IsNullOrEmpty(dv))
                                .Distinct())
                })
                .ToList();

            ViewBag.KhenThuongTuan = khenThuongTuan;

            var tongQuanKhenThuongThang = from ndt in _context.KT_NoiDungThuong
                             join ds in _context.KT_DanhSachKhenThuong
                             on ndt.ID equals ds.ID_NoiDungThuong
                             where ndt.NgayQuyetDinh.HasValue &&
                                   ndt.NgayQuyetDinh.Value.Month == currentMonth &&
                                   ndt.NgayQuyetDinh.Value.Year == currentYear
                             select new
                             {
                                 ndt.ID,
                                 ndt.NoiDungKhenThuong,
                                 ds.MNV,
                                 ds.DonVi,
                                 ds.GiaTriThuong
                             };

            var tongNoiDungThuongThang = tongQuanKhenThuongThang.Select(x => x.NoiDungKhenThuong).Distinct().Count();
            var tongNhanVienThuongThang = tongQuanKhenThuongThang.Where(x => x.MNV != null).Select(x => x.MNV).Distinct().Count();
            var tongDonViThuongThang = tongQuanKhenThuongThang.Where(x => x.DonVi != null).Select(x => x.DonVi).Distinct().Count();
            //var tongGiaTriThuongThang = tongQuanKhenThuongThang.ToList().Sum(x => x.GiaTriThuong ?? 0);


            var tongGiaTriThuongThang = _context.KT_NoiDungThuong
                .Where(x => x.NgayQuyetDinh.HasValue &&
                            x.NgayQuyetDinh.Value.Month == currentMonth &&
                            x.NgayQuyetDinh.Value.Year == currentYear)
                .Sum(x => (decimal?)x.TongTienThuong) ?? 0;


            ViewBag.TongNoiDungThuongThang = tongNoiDungThuongThang;
            ViewBag.TongNhanVienThuongThang = tongNhanVienThuongThang;
            ViewBag.TongDonViThuongThang = tongDonViThuongThang;
            ViewBag.TongGiaTriThuongThang = tongGiaTriThuongThang.ToString("N0"); ;

            var tongQuanKhenThuongTuan = from noiDungThuong in _context.KT_NoiDungThuong
                        join danhSachKhenThuong in _context.KT_DanhSachKhenThuong
                        on noiDungThuong.ID equals danhSachKhenThuong.ID_NoiDungThuong
                        where noiDungThuong.NgayQuyetDinh >= startOfWeek
                              && noiDungThuong.NgayQuyetDinh < endOfWeek
                        select new
                        {
                            NoiDungKhenThuong = noiDungThuong.NoiDungKhenThuong,
                            MNV = danhSachKhenThuong.MNV,
                            DonVi = danhSachKhenThuong.DonVi,
                            GiaTriThuong = danhSachKhenThuong.GiaTriThuong
                        };

            var tongNoiDungThuongTuan = tongQuanKhenThuongTuan.Select(x => x.NoiDungKhenThuong).Distinct().Count();
            var tongNhanVienThuongTuan = tongQuanKhenThuongTuan.Where(x => x.MNV != null).Select(x => x.MNV).Distinct().Count();
            var tongDonViThuongTuan = tongQuanKhenThuongTuan.Where(x => x.DonVi != null).Select(x => x.DonVi).Distinct().Count();
            //var tongGiaTriThuongTuan = tongQuanKhenThuongTuan.ToList().Sum(x => x.GiaTriThuong ?? 0);

            var tongGiaTriThuongTuan = _context.KT_NoiDungThuong
                .Where(x => x.NgayQuyetDinh.HasValue &&
                            x.NgayQuyetDinh >= startOfWeek &&
                            x.NgayQuyetDinh < endOfWeek)
                .Sum(x => (decimal?)x.TongTienThuong) ?? 0;


            ViewBag.TongNoiDungThuongTuan = tongNoiDungThuongTuan;
            ViewBag.TongNhanVienThuongTuan = tongNhanVienThuongTuan;
            ViewBag.TongDonViThuongTuan = tongDonViThuongTuan;
            ViewBag.TongGiaTriThuongTuan = tongGiaTriThuongTuan.ToString("N0"); ;


            return View();
        }

        [HttpGet]
        public JsonResult GetOverviewByYear_(int year)
        {
            var data = _context.KT_DanhSachKhenThuong
                .Where(x => x.Nam == year)
                .GroupBy(x => x.Nam)
                .Select(g => new
                {
                    TongSoDeTai = g.Select(x => x.NoiDungKhenThuong).Distinct().Count(),
                    SoCaNhanKhenThuong = g.Where(x => x.MNV != null).Select(x => x.MNV).Distinct().Count(),
                    SoDonViKhenThuong = g.Where(x => x.DonVi != null).Select(x => x.DonVi).Distinct().Count()
                })
                .FirstOrDefault();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetOverviewByYear(int year)
        {
            var data = _context.KT_DanhSachKhenThuong
                .Where(x => x.Nam == year)
                .GroupBy(x => x.Nam)
                .Select(g => new
                {
                    TongSoDeTai = g.Select(x => x.NoiDungKhenThuong).Distinct().Count(),
                    SoCaNhanKhenThuong = g.Where(x => x.MNV != null).Select(x => x.MNV).Distinct().Count(),
                    SoDonViKhenThuong = g.Where(x => x.DonVi != null).Select(x => x.DonVi).Distinct().Count()
                })
                .FirstOrDefault();

            var tongTienThuongVaLamLoi = _context.KT_NoiDungThuong
                .Where(x => x.Nam == year)
                .GroupBy(x => 1)
                .Select(g => new
                {
                    TongGiaTriLamLoi = Math.Round(g.Sum(x => x.GiaTriLamLoi ?? 0) / (decimal) 1_000_000_000.0, 0),
                    TongSoTienThuong = Math.Round(g.Sum(x => x.TongTienThuong ?? 0) / (decimal) 1_000_000.0, 0),
                })
                .FirstOrDefault();

            var result = new
            {
                TongSoDeTai = data?.TongSoDeTai ?? 0,
                SoCaNhanKhenThuong = data?.SoCaNhanKhenThuong ?? 0,
                SoDonViKhenThuong = data?.SoDonViKhenThuong ?? 0,
                TongSoTienThuong = tongTienThuongVaLamLoi?.TongSoTienThuong ?? 0,
                TongGiaTriLamLoi = tongTienThuongVaLamLoi?.TongGiaTriLamLoi ?? 0
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetCollectiveChartData(int year)
        {
            var thongKe = _context.KT_DanhSachKhenThuong
                .Where(x => x.Nam == year)
                .GroupBy(x => x.DonVi)
                .Select(g => new
                {
                    DonVi = g.Key,
                    SoLuong = g.Select(x => x.NoiDungKhenThuong).Distinct().Count()
                    //SoLuong = g.Distinct().Count()
                })
                .OrderByDescending(g => g.SoLuong)
                .ToList();

            var labels = thongKe.Select(x => x.DonVi).ToArray();
            var data = thongKe.Select(x => x.SoLuong).ToArray();

            var chartData = new
            {
                Labels = labels,
                Data = data
            };

            return Json(chartData, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetAchievements()
        {
            var data = _context.KT_DanhSachKhenThuong.Select(x => new
            {
                id = x.MNV,
                name = x.HoTen,
                department = x.DonVi,
                project = x.NoiDungKhenThuong,
                reward = "Có đổi mới sáng tạo",
                year = x.Nam.ToString()
            }).Where(x => x.id != null).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetStatisticsData(int year)
        {
            var data_01 = _context.KT_DanhSachKhenThuong
                .Where(x => x.Nam == year)
                .GroupBy(x => x.Nam)
                .Select(g => new
                {
                    soCaNhanDuocThuong = g.Where(x => x.MNV != null).Select(x => x.MNV).Distinct().Count(),
                    soTapTheDuocThuong = g.Select(x => x.DonVi).Distinct().Count(),
                    soDeTaiDuocThuong = g.Select(x => x.NoiDungKhenThuong).Distinct().Count(),
                })
                .FirstOrDefault();

            var data_02 = _context.KT_NoiDungThuong
                .Where(x => x.Nam == year)
                .GroupBy(x => 1)
                .Select(g => new
                {
                    tongSoTienThuong = Math.Round(g.Sum(x => x.TongTienThuong ?? 0) / (decimal)1_000_000.0, 0)
                })
                .FirstOrDefault();

            var data = new
            {
                individuals = data_01?.soCaNhanDuocThuong ?? 0,
                teams = data_01?.soTapTheDuocThuong ?? 0,
                contents = data_01?.soDeTaiDuocThuong ?? 0,
                bonus = data_02?.tongSoTienThuong ?? 0
            };

            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}