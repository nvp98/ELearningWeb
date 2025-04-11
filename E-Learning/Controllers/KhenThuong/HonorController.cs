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

            return View();
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
                    TongGiaTriLamLoi = Math.Round(g.Sum(x => x.GiaTriLamLoi ?? 0) / 1_000_000_000.0, 0),
                    TongSoTienThuong = Math.Round(g.Sum(x => x.GiaTriThuong ?? 0) / 1_000_000.0, 0),
                    SoCaNhanKhenThuong = g.Where(x => x.MNV != null).Select(x => x.MNV).Distinct().Count(),
                    SoDonViKhenThuong = g.Where(x => x.DonVi != null).Select(x => x.DonVi).Distinct().Count()
                })
                .FirstOrDefault();

            return Json(data, JsonRequestBehavior.AllowGet);
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
                    SoLuong = g.Count()
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
        public JsonResult GetStatisticsData1(int year)
        {

            var data = new Dictionary<int, object>
            {
                { 2020, new { individuals = 245, teams = 65, contents = 120, bonus = 32 } },
                { 2021, new { individuals = 278, teams = 72, contents = 145, bonus = 36 } },
                { 2022, new { individuals = 295, teams = 82, contents = 168, bonus = 42 } },
                { 2023, new { individuals = 324, teams = 78, contents = 195, bonus = 45 } }
            };

            if (data.ContainsKey(year))
            {
                return Json(data[year], JsonRequestBehavior.AllowGet);
            }

            return Json(null, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetStatisticsData(int year)
        {
            var data = _context.KT_DanhSachKhenThuong
                .Where(x => x.Nam == year)
                .GroupBy(x => x.Nam)
                .Select(g => new
                {
                    individuals = g.Select(x => x.HoTen).Distinct().Count(),
                    teams = g.Select(x => x.DonVi).Distinct().Count(),
                    contents = g.Select(x => x.NoiDungKhenThuong).Distinct().Count(),
                    bonus = Math.Round(g.Sum(x => x.GiaTriThuong ?? 0) / 1_000_000.0)
                })
                .FirstOrDefault();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

    }
}