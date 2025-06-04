using E_Learning.Models;
using E_Learning.ModelsDMST;
using E_Learning.ModelsTieuBanDaoTao;
using PagedList;
using System;
using System.Linq;
using System.Web.Mvc;

namespace E_Learning.Controllers.DMST
{
    public class TieuBanBoPhanController : Controller
    {
        ELEARNINGEntities db = new ELEARNINGEntities();
        int Idquyen = MyAuthentication.IDQuyen;
        String ControllerName = "TieuBanBoPhan";

        // GET: TieuBanBoPhan
        public ActionResult Index(int? page, string phongBanFilter, string viTriTieuBanFilter, string searchName)
        {
            var data = from tv in db.DMST_ThanhVienBan
                       join nv in db.NhanViens on tv.IDNV equals nv.ID
                       join pb in db.PhongBans on nv.IDPhongBan equals pb.IDPhongBan
                       join vt in db.ViTriKNLs on nv.IDVTKNL equals vt.IDVT
                       join cv in db.DMST_ChucVu on tv.ID_ChucVu equals cv.ID
                       where cv.MaToChuc.ToString() == "2"
                       select new ThanhVienView
                       {
                           ID = tv.ID,
                           MaNhanVien = nv.MaNV,
                           HoTen = nv.HoTen,
                           BoPhanID = pb.IDPhongBan,
                           BoPhan = pb.TenPhongBan,
                           ViTriCongViec = vt.TenViTri,
                           ChucVuID = cv.ID,
                           ChucVuTrongTieuBan = cv.TenChucVu,
                           Email = tv.Email
                       };

            if (!string.IsNullOrEmpty(phongBanFilter))
            {
                data = data.Where(x => x.BoPhanID.ToString() == phongBanFilter);
            }

            if (!string.IsNullOrEmpty(viTriTieuBanFilter))
            {
                data = data.Where(x => x.ChucVuID.ToString() == viTriTieuBanFilter);
            }

            if (!string.IsNullOrEmpty(searchName))
            {
                data = data.Where(x => x.HoTen.Contains(searchName) || x.MaNhanVien.Contains(searchName));
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            var pagedList = data.OrderBy(x => x.HoTen).ToPagedList(pageNumber, pageSize);

            ViewBag.phongBanFilter = phongBanFilter;
            ViewBag.viTriTieuBanFilter = viTriTieuBanFilter;
            ViewBag.SearchName = searchName;

            ViewBag.DSPhongBan = new SelectList(db.PhongBans.OrderBy(x => x.TenPhongBan), "IDPhongBan", "TenPhongBan", phongBanFilter);
            ViewBag.DSChucVu = new SelectList(db.DMST_ChucVu.Where(x => x.MaToChuc == "2").OrderBy(x => x.TenChucVu), "ID", "TenChucVu", viTriTieuBanFilter);

            return View(pagedList);
        }

        public ActionResult Create()
        {
            var dsPhongBan = db.PhongBans
               .Select(pb => new SelectListItem
               {
                   Value = pb.IDPhongBan.ToString(),
                   Text = pb.TenPhongBan.ToString(),
               }).OrderBy(x => x.Text).ToList();

            ViewBag.DSPhongBan = dsPhongBan;

            var dsNhanVien = db.NhanViens
                .Where(nv => nv.IDTinhTrangLV == 1)
               .Select(nv => new SelectListItem
               {
                   Value = nv.ID.ToString(),
                   Text = nv.MaNV + " - " + nv.HoTen,
               }).ToList();

            ViewBag.DSNhanVien = dsNhanVien;

            var dsChucVu = db.DMST_ChucVu
                .Where(cv => cv.MaToChuc == "2")
               .Select(cv => new SelectListItem
               {
                   Value = cv.ID.ToString(),
                   Text = cv.TenChucVu
               }).ToList();

            ViewBag.DSChucVu = dsChucVu;

            return PartialView();
        }

        public JsonResult GetNhanViensByPhongBan(int idPB)
        {
            var nhanViens = db.NhanViens
                    .Where(nv => nv.IDPhongBan == idPB && nv.IDTinhTrangLV == 1)
                    .Select(nv => new
                    {
                        Value = nv.ID,
                        Text = nv.MaNV + " - " + nv.HoTen
                    }).ToList();

            return Json(nhanViens, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ThanhVienView model)
        {
            try
            {
                if (model.ThanhVienList != null && model.ThanhVienList.Any())
                {
                    foreach (var tv in model.ThanhVienList)
                    {
                        var thanhVien = new DMST_ThanhVienBan
                        {
                            IDNV = tv.NhanVienID,
                            ID_ChucVu = tv.ChucVuID,
                            Email = tv.Email
                        };

                        db.DMST_ThanhVienBan.Add(thanhVien);
                    }

                    db.SaveChanges();
                    TempData["msgSuccess"] = "<script>alert('Cập nhật thành công!');</script>";
                }
                return RedirectToAction("Index");
            } catch
            {
                TempData["msgError"] = "<script>alert('Lỗi khi thêm mới');</script>";
                return RedirectToAction("Index");
            }
        }

        public ActionResult Edit(int id, int? phongBanFilter)
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            //if (!ListQuyen.Contains(CONSTKEY.EDIT))
            //{
            //    TempData["msgError"] = "<script>alert('Bạn không có quyền thực hiện chức năng này');</script>";
            //    return RedirectToAction("", "Home");
            //}

            var thanhVienTieuBan = db.DMST_ThanhVienBan.Where(x => x.ID == id).FirstOrDefault();
            var IDNhanVien = thanhVienTieuBan.IDNV;

            var tenPhongBan = db.PhongBans
                            .Where(p => p.IDPhongBan == db.NhanViens
                                .Where(nv => nv.ID == IDNhanVien)
                                .FirstOrDefault().IDPhongBan)
                            .Select(p => p.TenPhongBan)
                            .FirstOrDefault();

            ViewBag.TenBoPhan = tenPhongBan;

            var hoTenNhanVien = db.NhanViens.Where(x => x.ID == IDNhanVien).FirstOrDefault().HoTen;
            ViewBag.NhanVienDangChon = hoTenNhanVien;

            var danhSachViTriTieuBan = db.DMST_ChucVu
                .Select(vt => new { vt.ID, vt.TenChucVu })
                .ToList();

            ViewBag.DSViTriTieuBan = new SelectList(danhSachViTriTieuBan, "ID", "TenChucVu", thanhVienTieuBan.ID_ChucVu);

            ViewBag.Email = thanhVienTieuBan.Email;

            return PartialView();
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            if (!ListQuyen.Contains(CONSTKEY.DEL))
            {
                return Json(new { success = false, message = "Bạn không có quyền thực hiện chức năng này!" });
            }

            try
            {
                var record = db.DMST_ThanhVienBan.SingleOrDefault(x => x.ID == id);
                if (record == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy dữ liệu cần xóa!" });
                }

                db.DMST_ThanhVienBan.Remove(record);
                db.SaveChanges();

                return Json(new { success = true, message = "Xóa thành viên khỏi tiểu ban thành công!" });
            }
            catch
            {
                return Json(new { success = false, message = "Có lỗi xảy ra khi xóa!" });
            }
        }

    }
}