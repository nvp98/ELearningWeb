using E_Learning.Models;
using E_Learning.ModelsDMST;
using Newtonsoft.Json;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace E_Learning.Controllers.DMST
{
    public class RenewCreativeController : Controller
    {
        ELEARNINGEntities db = new ELEARNINGEntities();
        int Idquyen = MyAuthentication.IDQuyen;
        String ControllerName = "RenewCreative";
        // GET: RenewCreative
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ViewList(int? page)
        {
            var data = from dt in db.DMST_PhieuDangKy
                       select new DeTaiDMSTView
                       {
                           ID = dt.ID,
                           TenYTuong = dt.TenYTuong,
                           NoiDungYTuong = dt.NoiDungYTuong,
                           ViTriTrienKhai = dt.ViTriTrienKhai,
                           HieuQuaKyVong = dt.HieuQuaKyVong,
                           NgayBatDau = (DateTime) dt.NgayTao,
                           //NgayKetThuc = (DateTime) dt.DenNgay,
                           ID_NhanVienDaiDien = (int) dt.ID_NhanVienDaiDien
                       };

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            var pagedList = data.OrderBy(x => x.TenYTuong).ToPagedList(pageNumber, pageSize);

            return View(pagedList);
        }

        public ActionResult Create()
        {
            var dsLinhVuc = db.DMST_LinhVuc
                 .OrderBy(lv => lv.ID)
                 .Select(lv => new SelectListItem
                 {
                     Value = lv.ID.ToString(),
                     Text = lv.TenLinhVuc
                 })
                 .ToList();

            ViewBag.DSLinhVuc = dsLinhVuc;

            var model = new PhieuDangKyView
            {
                KinhPhiDuKien = new List<ChiPhiDuKienModel>(),
                NguoiThamGia = new List<NguoiThamGiaModel>()
            };

            var dsYTuong = db.DMST_PhieuDangKy
                 .OrderBy(lv => lv.ID)
                 .Select(lv => new SelectListItem
                 {
                     Value = lv.ID.ToString(),
                     Text = lv.TenYTuong
                 })
                 .ToList();

            ViewBag.DSYTuong = dsYTuong;

            return View();
        }

        [HttpGet]
        public JsonResult LayNhanVienTheoMa(string maNhanVien)
        {
            var nhanVien = db.NhanViens
                             .Where(nv => nv.MaNV == maNhanVien)
                             .Select(nv => new
                             {
                                 id = nv.ID,
                                 ma = nv.MaNV,
                                 ten = nv.HoTen
                             })
                             .FirstOrDefault();

            return Json(nhanVien, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitYTuong(PhieuDangKyView model)
        {
            try
            {
                var entity = new DMST_PhieuDangKy
                {
                    ID_LinhVuc = model.ID_LinhVuc,
                    ID_NguoiTao = MyAuthentication.ID,
                    NgayTao = DateTime.Now,
                    TinhTrang = model.TinhTrang,

                    TenYTuong = model.TenYTuong,
                    ID_NhanVienDaiDien = model.ID_NhanVienDaiDien,
                    NgayDeXuat = model.NgayDeXuat,

                    NoiDungYTuong = model.NoiDungYTuong,
                    LyDoThucHien = model.LyDoThucHien,
                    ViTriTrienKhai = model.ViTriTrienKhai,

                    TuNgay = model.TuNgay,
                    DenNgay = model.DenNgay,

                    CachThucThucHien = model.CachThucThucHien,
                    ThucHienKhi = model.ThucHienKhi,
                    CanThuNghiemThem = model.CanThuNghiemThem,
                    CanHoTro = model.CanHoTro,
                    HieuQuaKyVong = model.HieuQuaKyVong,
                    DeXuatNguoiBoPhan = model.DeXuatNguoiBoPhan
                };

                db.DMST_PhieuDangKy.Add(entity);
                db.SaveChanges();

                if (model.KinhPhiDuKien != null)
                {
                    foreach (var cp in model.KinhPhiDuKien)
                    {
                        var chiPhi = new DMST_KinhPhiDuKien
                        {
                            TenNguonLuc = cp.TenNguonLuc,
                            ChiPhi = cp.ChiPhi ?? 0,
                            NguonThongTin = cp.NguonThongTin,
                            GhiChu = cp.GhiChu,
                            ID_Phieu = entity.ID
                        };
                        db.DMST_KinhPhiDuKien.Add(chiPhi);
                    }
                    db.SaveChanges();
                }

                if (!string.IsNullOrEmpty(model.NguoiThamGiaJson))
                {
                    var ds = JsonConvert.DeserializeObject<List<NguoiThamGiaModel>>(model.NguoiThamGiaJson);
                    foreach (var item in ds)
                    {
                        db.DMST_NguoiThamGia.Add(new DMST_NguoiThamGia
                        {
                            ID_Phieu = entity.ID,
                            ID_NhanVien = item.ID_NhanVien,
                            VaiTro = item.VaiTro
                        });
                    }
                    db.SaveChanges();
                }

                TempData["msgSuccess"] = "<script>alert('Lưu ý tưởng thành công!');</script>";
                return RedirectToAction("Create");
            }
            catch (Exception ex)
            {
                TempData["msgError"] = "<script>alert('Có lỗi xảy ra: " + ex.Message + "');</script>";
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public JsonResult LayNguoiThucHienTheoPhieu(int idPhieu)
        {
            var ds = (from ntg in db.DMST_NguoiThamGia
                      join nv in db.NhanViens on ntg.ID_NhanVien equals nv.ID
                      where ntg.ID_Phieu == idPhieu
                      select new
                      {
                          id = ntg.ID_NhanVien,
                          ma = nv.MaNV,
                          ten = nv.HoTen
                      }).ToList();

            return Json(ds, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitKeHoachThucHien(PhieuDangKyView model)
        {
            try
            {
                var entity = new DMST_ChuanBiThucHien
                {
                    ID_Phieu = model.KeHoachThucHien.ID_Phieu,
                    NgayBatDauTrienKhai = (DateTime) model.KeHoachThucHien.NgayBatDauTrienKhai,
                    NgayDuKienHoanThanh = (DateTime) model.KeHoachThucHien.NgayDuKienHoanThanh,
                    MucTieuThucHien = model.KeHoachThucHien.MucTieuThucHien,
                    ChiPhiDuKien = model.KeHoachThucHien.ChiPhiDuKien,
                    ThuanLoiKhoKhan = model.KeHoachThucHien.ThuanLoiKhoKhan,
                    DeNghiHoTro = model.KeHoachThucHien.DeNghiHoTro
                };

                db.DMST_ChuanBiThucHien.Add(entity);
                db.SaveChanges();

                if (model.NhanVienThucHien != null)
                {
                    foreach (var item in model.NhanVienThucHien)
                    {
                        var row = new DMST_DanhSachThucHien
                        {
                            ID_Phieu = model.KeHoachThucHien.ID_Phieu,
                            NoiDungCongViec = item.NoiDungCongViec,
                            SoLuong = item.SoLuong,
                            ID_NhanVien = item.ID_NhanVien,
                            ThoiGian = item.ThoiGian,
                            GhiChu = item.GhiChu
                        };
                        db.DMST_DanhSachThucHien.Add(row);
                    }
                    db.SaveChanges();
                }

                TempData["msgSuccess"] = "<script>alert('Lưu kế hoạch thành công!');</script>";
                return RedirectToAction("Create");
            }
            catch (Exception ex)
            {
                TempData["msgError"] = "<script>alert('Có lỗi xảy ra: " + ex.Message + "');</script>";
                return RedirectToAction("Index");
            }
        }
    }
}