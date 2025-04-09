using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_Learning.Controllers.KhenThuong
{
    public class HonorController : Controller
    {
        // GET: Honor
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetOverviewByYear(int year)
        {
            var data = new Dictionary<int, object>
            {
                { 2020, new { TongSoDeTai = 980, TongGiaTriLamLoi = 1850, TongSoTienThuong = 32, SoCaNhanKhenThuong = 245, SoDonViKhenThuong = 65 } },
                { 2021, new { TongSoDeTai = 1050, TongGiaTriLamLoi = 2000, TongSoTienThuong = 36, SoCaNhanKhenThuong = 278, SoDonViKhenThuong = 72 } },
                { 2022, new { TongSoDeTai = 1120, TongGiaTriLamLoi = 2200, TongSoTienThuong = 42, SoCaNhanKhenThuong = 295, SoDonViKhenThuong = 82 } },
                { 2023, new { TongSoDeTai = 1245, TongGiaTriLamLoi = 250, TongSoTienThuong = 45, SoCaNhanKhenThuong = 324, SoDonViKhenThuong = 78 } }
            };

            if (data.ContainsKey(year))
            {
                return Json(data[year], JsonRequestBehavior.AllowGet);
            }

            return Json(null, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetCollectiveChartData(int year)
        {
            var chartData = new Dictionary<int, object>
            {
                {
                    2020, new {
                        Labels = new[] { "P. CNTT & CĐS", "NM. HRC1", "NM. CĐ2", "NM. HRC2", "NM. TKVV" },
                        Data = new[] { 45, 32, 28, 20, 15 }
                    }
                },
                {
                    2021, new {
                        Labels = new[] { "P. CNTT & CĐS", "NM. HRC1", "NM. CĐ2", "NM. HRC2", "NM. TKVV" },
                        Data = new[] { 52, 38, 31, 25, 18 }
                    }
                },
                {
                    2022, new {
                        Labels = new[] { "P. CNTT & CĐS", "NM. HRC1", "NM. CĐ2", "NM. HRC2", "NM. TKVV" },
                        Data = new[] { 60, 42, 36, 30, 22 }
                    }
                },
                {
                    2023, new {
                        Labels = new[] { "P. CNTT & CĐS", "NM. HRC1", "NM. CĐ2", "NM. HRC2", "NM. TKVV" },
                        Data = new[] { 68, 48, 40, 35, 25 }
                    }
                }
            };

            if (chartData.ContainsKey(year))
            {
                return Json(chartData[year], JsonRequestBehavior.AllowGet);
            }

            return Json(null, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetAchievements()
        {
            var data = new List<object>
            {
                new { id = "NV001", name = "Nguyễn Văn A", department = "NM. CĐ1", project = "Giải pháp A", reward = "Có đổi mới sáng tạo", year = "2023" },
                new { id = "NV002", name = "Trần Thị B", department = "NM. TKVV", project = "Nghiên cứu B", reward = "Có đổi mới sáng tạo", year = "2023" },
                new { id = "NV003", name = "Lê Văn C", department = "P. ATMT", project = "Vật liệu C", reward = "Có đổi mới sáng tạo", year = "2023" },
                new { id = "NV004", name = "Phạm Thị D", department = "P. CNTT & CĐS", project = "Giải pháp xử lý D", reward = "Có đổi mới sáng tạo", year = "2023" },
                new { id = "NV005", name = "Hoàng Văn E", department = "NM. HRC2", project = "Năng lượng tái tạo", reward = "Có đổi mới sáng tạo", year = "2023" },
                new { id = "NV006", name = "Vũ Thị F", department = "NM. CĐ1", project = "Blockchain", reward = "Có đổi mới sáng tạo", year = "2022" },
                new { id = "NV007", name = "Đặng Văn G", department = "NM. TKVV", project = "Nghiên cứu E", reward = "Có đổi mới sáng tạo", year = "2022" },
                new { id = "NV008", name = "Bùi Thị H", department = "P. ATMT", project = "Đề tài F", reward = "Có đổi mới sáng tạo", year = "2022" },
                new { id = "NV009", name = "Ngô Văn I", department = "P. CNTT & CĐS", project = "Giám sát G", reward = "Có đổi mới sáng tạo", year = "2022" },
                new { id = "NV010", name = "Đỗ Thị K", department = "NM. HRC2", project = "Hiệu suất H", reward = "Có đổi mới sáng tạo", year = "2022" },
                new { id = "NV011", name = "Mai Văn L", department = "NM. CĐ1", project = "Hệ thống K", reward = "Có đổi mới sáng tạo", year = "2021" },
                new { id = "NV012", name = "Lý Thị M", department = "NM. TKVV", project = "Công nghệ L", reward = "Có đổi mới sáng tạo", year = "2021" },
                new { id = "NV013", name = "Trương Văn N", department = "P. ATMT", project = "Vật liệu M", reward = "Có đổi mới sáng tạo", year = "2021" },
                new { id = "NV014", name = "Hồ Thị O", department = "P. CNTT & CĐS", project = "Xử lý N", reward = "Có đổi mới sáng tạo", year = "2021" },
                new { id = "NV015", name = "Phan Văn P", department = "NM. HRC2", project = "Năng lượng Z", reward = "Có đổi mới sáng tạo", year = "2021" },
                new { id = "NV016", name = "Võ Thị Q", department = "NM. CĐ1", project = "AI", reward = "Có đổi mới sáng tạo", year = "2020" },
                new { id = "NV017", name = "Chu Văn R", department = "NM. TKVV", project = "Nghiên cứu O", reward = "Có đổi mới sáng tạo", year = "2020" },
                new { id = "NV018", name = "Lâm Thị S", department = "P. ATMT", project = "Đề tài ABC", reward = "Có đổi mới sáng tạo", year = "2020" },
                new { id = "NV019", name = "Tô Văn T", department = "P. CNTT & CĐS", project = "Giảm thiểu CO2", reward = "Có đổi mới sáng tạo", year = "2020" },
                new { id = "NV020", name = "Hà Thị U", department = "NM. HRC2", project = "Lưu trữ năng lượng", reward = "Có đổi mới sáng tạo", year = "2020" }
            };

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetStatisticsData(int year)
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
    }
}