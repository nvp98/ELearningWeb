using E_Learning.Models;
using E_Learning.ModelsDMST;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
    }
}