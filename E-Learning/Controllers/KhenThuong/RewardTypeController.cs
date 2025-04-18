using E_Learning.Models;
using E_Learning.ModelsQLKhenThuong;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_Learning.Controllers.KhenThuong
{
    public class RewardTypeController : Controller
    {
        ELEARNINGEntities db = new ELEARNINGEntities();
        int Idquyen = MyAuthentication.IDQuyen;
        String ControllerName = "RewardType";
        // GET: RewardType
        public ActionResult Index(int? page, string search, int? IDLoai, string kieuThuong = "all", string highlightDeTai = null,
                  string highlightIDs = null, string highlightDonVis = null)
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            ViewBag.QUYENCN = ListQuyen;
            if (!ListQuyen.Contains(CONSTKEY.V))
            {
                TempData["msgError"] = "<script>alert('Bạn không có quyền truy cập chức năng này');</script>";
                return RedirectToAction("", "Home");
            }

            if (search == null) search = "";
            ViewBag.search = search;
            ViewBag.kieuThuong = kieuThuong;
            ViewBag.IDLoai = IDLoai;

            int pageSize = 4;
            int pageNumber = (page ?? 1);

            var baseQuery = db.KT_DanhSachKhenThuong.AsQueryable();

            if (kieuThuong == "ca-nhan")
            {
                baseQuery = baseQuery.Where(x => x.MNV != null);
            }
            else if (kieuThuong == "tap-the")
            {
                baseQuery = baseQuery.Where(x => x.MNV == null);
            }

            if (IDLoai.HasValue && IDLoai > 0)
            {
                baseQuery = baseQuery.Where(x => x.ID_NoiDungThuong == IDLoai.Value);
            }

            var allData = baseQuery.Select(dskt => new LoaiKhenThuongDTO
            {
                ID = dskt.ID,
                TenDeTai = dskt.NoiDungKhenThuong,
                MaNV = dskt.MNV,
                HoTen = dskt.HoTen,
                TenPhongBan = dskt.DonVi,
                TapThe = dskt.TapThe
            }).ToList();

            if (!string.IsNullOrEmpty(search))
            {
                allData = allData.Where(x =>
                    (x.HoTen + " " + x.MaNV + " " + x.TenPhongBan).ToLower().Contains(search.ToLower())
                ).ToList();
            }

            var distinctDeTais = allData
                .GroupBy(x => x.TenDeTai)
                .Select(g => g.Key)
                .ToList();

            if (!string.IsNullOrEmpty(highlightDeTai))
            {
                var index = distinctDeTais.IndexOf(highlightDeTai);
                if (index >= 0)
                {
                    pageNumber = (index / pageSize) + 1;
                }
                ViewBag.HighlightDeTai = highlightDeTai;
            }

            var highlightIDList = string.IsNullOrEmpty(highlightIDs)
               ? new List<int>()
               : highlightIDs.Split(',').Select(int.Parse).ToList();
            ViewBag.HighlightIDs = highlightIDList;

            var highlightDonViList = string.IsNullOrEmpty(highlightDonVis)
                ? new List<string>()
                : highlightDonVis.Split(',').ToList();
            ViewBag.HighlightDonVis = highlightDonViList;

            var pagedDeTais = distinctDeTais
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var filteredData = allData
                .Where(x => pagedDeTais.Contains(x.TenDeTai))
                .OrderBy(x => x.TenDeTai)
                .ThenBy(x => x.ID)
                .ToList();

            var pagedList = new StaticPagedList<LoaiKhenThuongDTO>(
                filteredData, pageNumber, pageSize, distinctDeTais.Count()
            );

            var noiDungList = db.KT_NoiDungThuong
               .Select(x => new { x.ID, x.NoiDungKhenThuong })
               .OrderBy(x => x.NoiDungKhenThuong)
               .ToList();

            ViewBag.NoiDungThuongList = new SelectList(noiDungList, "ID", "NoiDungKhenThuong", IDLoai);

            return View(pagedList);
        }

        public ActionResult Create()
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            if (!ListQuyen.Contains(CONSTKEY.ADD))
            {
                TempData["msgError"] = "<script>alert('Bạn không có quyền thực hiện chức năng này');</script>";
                return RedirectToAction("", "Home");
            }
            db.Configuration.ProxyCreationEnabled = false;

            var noiDungList = db.KT_NoiDungThuong
               .OrderBy(x => x.NoiDungKhenThuong)
               .Select(x => new { x.ID, x.NoiDungKhenThuong })
               .ToList();

            ViewBag.NoiDungThuongList = new SelectList(noiDungList, "ID", "NoiDungKhenThuong");

            ViewBag.ListPhongBan = db.PhongBans.Where(x => x.MaPB != null).OrderBy(x => x.MaPB).Select(x => new { x.MaPB }).ToList();

            return PartialView();
        }

        [HttpPost]
        public ActionResult Create(LoaiKhenThuongDTO DTO)
        {
            try
            {
                var data = new KT_LoaiThuong()
                {
                    TenLoaiThuong = DTO.TenLoaiThuong
                };

                db.KT_LoaiThuong.Add(data);

                db.SaveChanges();
                TempData["msgSuccess"] = "<script>alert('Thêm mới thành công');</script>";
            }
            catch (Exception ex)
            {
                TempData["msgError"] = "<script>alert('Có lỗi khi thêm mới: " + ex.Message + "');</script>";
            }

            return RedirectToAction("Index", "RewardType");
        }

        public ActionResult Edit(int id)
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            if (!ListQuyen.Contains(CONSTKEY.EDIT))
            {
                TempData["msgError"] = "<script>alert('Bạn không có quyền thực hiện chức năng này');</script>";
                return RedirectToAction("", "Home");
            }
            var data = db.KT_LoaiThuong.Where(x => x.ID_Loai == id).SingleOrDefault();
            var DTO = new LoaiKhenThuongDTO()
            {
                TenLoaiThuong = data.TenLoaiThuong
            };

            return PartialView(DTO);
        }

        [HttpPost]
        public ActionResult Edit(LoaiKhenThuongDTO DTO)
        {
            try
            {
                var data = db.KT_LoaiThuong.Where(x => x.ID_Loai == DTO.ID).SingleOrDefault();
                data.TenLoaiThuong = DTO.TenLoaiThuong;

                db.SaveChanges();
                TempData["msgSuccess"] = "<script>alert('Cập nhập thành công');</script>";
            }
            catch (Exception e)
            {
                TempData["msgSuccess"] = "<script>alert('Cập nhập thất bại " + e.Message + " ');</script>";
            }

            return RedirectToAction("Index", "RewardType");
        }

        public ActionResult Delete(int id)
        {
            try
            {
                var data = db.KT_LoaiThuong.Where(x => x.ID_Loai == id).SingleOrDefault();
                db.KT_LoaiThuong.Remove(data);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                TempData["msgSuccess"] = "<script>alert('Xóa thất bại " + e.Message + " ');</script>";
            }
            return RedirectToAction("Index", "RewardType");
        }

        [HttpPost]
        public ActionResult LoadDanhSachNhanVien(string maNV)
        {
            if (string.IsNullOrWhiteSpace(maNV))
                return Json(new { success = false, message = "Mã nhân viên không được để trống." });

            var danhSachMaNV = maNV.Split(new[] { '\n', '\r', ',' }, StringSplitOptions.RemoveEmptyEntries)
                                   .Select(m => m.Trim())
                                   .ToList();

            var danhSachNhanVien = (from nv in db.NhanViens
                                    join pb in db.PhongBans on nv.IDPhongBan equals pb.IDPhongBan into joined
                                    from pb in joined.DefaultIfEmpty()
                                    where danhSachMaNV.Contains(nv.MaNV)
                                    select new
                                    {
                                        nv.MaNV,
                                        nv.HoTen,
                                        TenPhongBan = pb != null ? pb.MaPB : "(Chưa có phòng ban)"
                                    }).ToList();

            return Json(new { success = true, data = danhSachNhanVien });
        }

        [HttpPost]
        public JsonResult GetDanhSachPhongBan(string[] selectedMaPBs)
        {
            if (selectedMaPBs == null || selectedMaPBs.Length == 0)
            {
                return Json(new { success = false, message = "Không có mã phòng ban nào được chọn." });
            }

            var phongBanList = db.PhongBans
                .Where(pb => selectedMaPBs.Contains(pb.MaPB))
                .Select(pb => new
                {
                    pb.MaPB,
                    pb.TenPhongBan
                })
                .ToList();

            return Json(new { success = true, data = phongBanList });
        }

        [HttpPost]
        public ActionResult SubmitCaNhanKhenThuong(int idNoiDungThuong, List<LoaiKhenThuongDTO> danhSachNV)
        {
            if (idNoiDungThuong <= 0 || danhSachNV == null || !danhSachNV.Any())
            {
                return Json(new { success = false, message = "Xảy ra lỗi" });
            }

            var dataNoiDungThuong = db.KT_NoiDungThuong.FirstOrDefault(x => x.ID == idNoiDungThuong);
            if (dataNoiDungThuong == null)
            {
                return Json(new { success = false, message = "Không tìm thấy nội dung thưởng" });
            }

            int? nam = dataNoiDungThuong.NgayQuyetDinh?.Year;
            int? thang = dataNoiDungThuong.NgayQuyetDinh?.Month;
            string tenDeTai = dataNoiDungThuong.NoiDungKhenThuong;

            try
            {
                var existingMNVs = db.KT_DanhSachKhenThuong
                    .Where(x => x.ID_NoiDungThuong == idNoiDungThuong)
                    .Select(x => x.MNV)
                    .ToList();

                var checkDuplicate = danhSachNV
                    .Where(nv => existingMNVs.Contains(nv.MaNV))
                    .Select(nv => nv.MaNV)
                    .ToList();

                if (checkDuplicate.Any())
                {
                    string danhSachTrung = string.Join(", ", checkDuplicate);
                    return Json(new
                    {
                        success = false,
                        message = $"Các mã nhân viên đã tồn tại: {danhSachTrung}"
                    });
                }

                var danhSachInsert = danhSachNV.Select(nv => new KT_DanhSachKhenThuong
                {
                    MNV = nv.MaNV,
                    HoTen = nv.HoTen,
                    DonVi = nv.TenPhongBan,
                    ID_NoiDungThuong = idNoiDungThuong,
                    Thang = thang,
                    Nam = nam,
                    NoiDungKhenThuong = tenDeTai
                }).ToList();

                db.KT_DanhSachKhenThuong.AddRange(danhSachInsert);
                db.SaveChanges();

                var danhSachID = danhSachInsert.Select(x => x.ID).ToList();

                return Json(new
                {
                    success = true,
                    tenDeTai = tenDeTai,
                    danhSachID = danhSachID
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        
        [HttpPost]
        public JsonResult SubmitDonViKhenThuong(List<string> maPBList, int idNoiDungThuong, string tapThe)
        {
            var dataKhenThuong = db.KT_NoiDungThuong
                .FirstOrDefault(x => x.ID == idNoiDungThuong);

            try
            {
                if (maPBList != null && maPBList.Any())
                {
                    List<KT_DanhSachKhenThuong> newEntities = new List<KT_DanhSachKhenThuong>();

                    foreach (var maPB in maPBList)
                    {
                        var entity = new KT_DanhSachKhenThuong
                        {
                            DonVi = maPB,
                            ID_NoiDungThuong = idNoiDungThuong,
                            NoiDungKhenThuong = dataKhenThuong.NoiDungKhenThuong,
                            Thang = dataKhenThuong.NgayQuyetDinh?.Month,
                            Nam = dataKhenThuong.NgayQuyetDinh?.Year,
                            TapThe = tapThe
                        };

                        db.KT_DanhSachKhenThuong.Add(entity);
                        newEntities.Add(entity);
                    }

                    db.SaveChanges();

                    var addedIDs = newEntities.Select(x => x.ID).ToList();

                    return Json(new
                    {
                        success = true,
                        message = "Lưu thành công!",
                        tenDeTai = dataKhenThuong.NoiDungKhenThuong,
                        danhSachID = addedIDs
                    });
                }

                return Json(new { success = false, message = "Danh sách đơn vị trống!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        [HttpPost]
        public ActionResult XoaKhenThuong(int id)
        {
            try
            {
                var item = db.KT_DanhSachKhenThuong.FirstOrDefault(x => x.ID == id);
                if (item == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy dữ liệu" });
                }

                db.KT_DanhSachKhenThuong.Remove(item);
                db.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}