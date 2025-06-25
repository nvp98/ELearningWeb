using ClosedXML.Excel;
using E_Learning.Models;
using E_Learning.ModelsTieuBanDaoTao;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_Learning.Controllers.TieuBanDaoTao
{
    public class TieuBanDaoTaoController : Controller
    {
        private ELEARNINGEntities db = new ELEARNINGEntities();
        int Idquyen = MyAuthentication.IDQuyen;
        String ControllerName = "TieuBanDaoTao";
        // GET: TieuBanDaoTao
        public ActionResult Index(int? viTriTieuBanFilter, int? phongBanFilter, string searchName, int? page)
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            ViewBag.QUYENCN = ListQuyen;

            if (!ListQuyen.Contains(CONSTKEY.VIEW_ALL))
            {
                phongBanFilter = MyAuthentication.IDPhongban;
                ModelState.Remove("phongBanFilter");
            }
            db.Configuration.ProxyCreationEnabled = false;

            var result = (from nv in db.NhanViens
                          join vtKNL in db.ViTriKNLs on nv.IDVTKNL equals vtKNL.IDVT
                          join tvTieuBan in db.BDT_ThanhVienTieuBan on nv.ID equals tvTieuBan.NhanVien_ID
                          join vtTieuBan in db.BDT_ViTriTieuBan on tvTieuBan.ViTriTieuBan_ID equals vtTieuBan.ID
                          join tieuBan in db.BDT_TieuBan on tvTieuBan.TieuBan_ID equals tieuBan.ID
                          select new ThanhVienTieuBanInfo
                          {
                              Id = tvTieuBan.ID,
                              MaViTriKNL = (int)tvTieuBan.ViTriKNL_ID,
                              TenViTriKNL = vtKNL.TenViTri,
                              HoTen = nv.HoTen,
                              TenViTriTieuBan = vtTieuBan.TenViTri,
                              NgayCapNhatGanNhat = (DateTime)tvTieuBan.NgayCapNhat,
                              NgayDenHanCapNhat = (DateTime)tvTieuBan.NgayDenHanCapNhatLai,
                              TrangThai = (int)tvTieuBan.TrangThai,
                              PhongBanID = (int)tieuBan.PhongBan_ID,
                              ViTriTieuBan_ID = (int)tvTieuBan.ViTriTieuBan_ID,
                              HoTenNguoiThem = tvTieuBan.NhanVienThem_ID != null ? db.NhanViens.Where(nv => nv.ID == tvTieuBan.NhanVienThem_ID).Select(nv =>
                              nv.HoTen).FirstOrDefault() ?? "" : "",
                              HoTenNguoiSua = tvTieuBan.NhanVienSua_ID != null ? db.NhanViens.Where(nv => nv.ID == tvTieuBan.NhanVienSua_ID).Select(nv => nv.HoTen).FirstOrDefault() ?? "" : ""
                          });

            if (phongBanFilter.HasValue && phongBanFilter != 0)
            {
                result = result.Where(x => x.PhongBanID == phongBanFilter.Value);
            }
            else
            {
                phongBanFilter = MyAuthentication.IDPhongban;
                result = result.Where(x => x.PhongBanID == MyAuthentication.IDPhongban);
            }

            var danhSachPhongBan = db.PhongBans
                .Select(p => new { p.IDPhongBan, p.TenPhongBan })
                .OrderBy(p => p.TenPhongBan)
                .ToList();

            ViewBag.DSPhongBan = new SelectList(danhSachPhongBan, "IDPhongBan", "TenPhongBan", phongBanFilter);

            if (viTriTieuBanFilter.HasValue && viTriTieuBanFilter.Value != 0)
            {
                result = result.Where(x => x.ViTriTieuBan_ID == viTriTieuBanFilter.Value);
            }

            if (!string.IsNullOrEmpty(searchName))
            {
                result = result.Where(x => x.HoTen.Contains(searchName));
            }

            var now = DateTime.Now;

            var expiredItems = db.BDT_ThanhVienTieuBan
                .Where(x => x.NgayDenHanCapNhatLai < now && x.TrangThai != 3)
                .ToList();

            foreach (var item in expiredItems)
            {
                item.TrangThai = 3;
            }
            db.SaveChanges();

            var danhSachViTri = db.BDT_ViTriTieuBan
                .Select(v => new { v.ID, v.TenViTri })
                .ToList();
            ViewBag.DSViTriTieuBan = new SelectList(danhSachViTri, "ID", "TenViTri", viTriTieuBanFilter);

            ViewBag.SearchName = searchName;

            int pageSize = 20;
            int pageNumber = (page ?? 1);

            var pagedResult = result.OrderBy(x => x.ViTriTieuBan_ID).ToPagedList(pageNumber, pageSize);

            return View(pagedResult);
        }

        public ActionResult Create()
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            //if (!ListQuyen.Contains(CONSTKEY.ADD))
            //{
            //    TempData["msgError"] = "<script>alert('Bạn không có quyền thực hiện chức năng này');</script>";
            //    return RedirectToAction("", "Home");
            //}
            db.Configuration.ProxyCreationEnabled = false;

            var username = MyAuthentication.Username;
            if (username == null)
            {
                TempData["msgError"] = "<script>alert('Phiên đăng nhập đã hết hạn, vui lòng đăng nhập lại');</script>";
                return RedirectToAction("Index", "Login");
            }

            var tenPhongBan = db.PhongBans
                            .Where(p => p.IDPhongBan == MyAuthentication.IDPhongban)
                            .Select(p => p.TenPhongBan)
                            .FirstOrDefault();

            ViewBag.TenBoPhan = tenPhongBan;

            var danhSachNhanVien = db.NhanViens
                .Where(nv => nv.IDPhongBan == MyAuthentication.IDPhongban && nv.IDTinhTrangLV == 1 )
                .Select(nv => new { nv.ID, nv.HoTen })
                .OrderBy(nv => nv.HoTen)
                .ToList();

            ViewBag.DSThanhVien = new SelectList(danhSachNhanVien, "ID", "HoTen");

            var danhSachViTriTieuBan = db.BDT_ViTriTieuBan
                .Select(vt => new { vt.ID, vt.TenViTri})
                .ToList();

            ViewBag.DSViTriTieuBan = new SelectList(danhSachViTriTieuBan, "ID", "TenViTri");

            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ThanhVienTieuBanViewModel model)
        {
            using (var tran = db.Database.BeginTransaction())
            {
                try
                {
                    int idPhongBan = MyAuthentication.IDPhongban;

                    var tenPhongBan = db.PhongBans
                                        .Where(x => x.IDPhongBan == idPhongBan)
                                        .Select(x => x.TenPhongBan)
                                        .FirstOrDefault();

                    var dataTieuBan = db.BDT_TieuBan
                                        .FirstOrDefault(x => x.PhongBan_ID == idPhongBan);

                    int tieuBanId;

                    if (dataTieuBan != null)
                    {
                        tieuBanId = dataTieuBan.ID;
                    }
                    else
                    {
                        var tieuBan = new BDT_TieuBan
                        {
                            TenTieuBan = "Tiểu ban đào tạo " + tenPhongBan,
                            PhongBan_ID = idPhongBan
                        };

                        db.BDT_TieuBan.Add(tieuBan);
                        db.SaveChanges();

                        tieuBanId = tieuBan.ID;
                    }

                    List<string> trungLapNhanVien = new List<string>();

                    foreach (var tv in model.ThanhVienList)
                    {
                        bool isExist = db.BDT_ThanhVienTieuBan.Any(x =>
                            x.NhanVien_ID == tv.Id &&
                            x.ViTriTieuBan_ID == tv.ViTri
                        );

                        if (isExist)
                        {
                            var hoTenNV = db.NhanViens
                                            .Where(x => x.ID == tv.Id)
                                            .Select(x => x.HoTen)
                                            .FirstOrDefault();
                            trungLapNhanVien.Add(hoTenNV);
                            continue;
                        }

                        var nhanVien = db.NhanViens.FirstOrDefault(x => x.ID == tv.Id);

                        var thanhVien = new BDT_ThanhVienTieuBan
                        {
                            TieuBan_ID = tieuBanId,
                            NhanVien_ID = tv.Id,
                            ViTriTieuBan_ID = tv.ViTri,
                            ViTriKNL_ID = nhanVien.IDVTKNL,
                            NgayCapNhat = DateTime.Now,
                            NgayDenHanCapNhatLai = DateTime.Now.AddMonths(6),
                            TrangThai = 0,
                            NhanVienThem_ID = MyAuthentication.ID
                        };

                        db.BDT_ThanhVienTieuBan.Add(thanhVien);
                    }

                    db.SaveChanges();
                    tran.Commit();

                    if (trungLapNhanVien.Any())
                    {
                        string message = "Các nhân viên sau đã tồn tại ở vị trí tương ứng, không được thêm:\n- " +
                                         string.Join("\n- ", trungLapNhanVien);
                        TempData["msgError"] = $"<script>alert(`{message}`);</script>";
                    }
                    else
                    {
                        TempData["msgSuccess"] = "<script>alert('Cập nhật thành công!');</script>";
                    }

                    return RedirectToAction("Index");
                }
                catch
                {
                    tran.Rollback();
                    TempData["msgError"] = "<script>alert('Lỗi khi lưu dữ liệu.');</script>";
                    return View(model);
                }
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

            var username = MyAuthentication.Username;
            if (username == null)
            {
                TempData["msgError"] = "<script>alert('Phiên đăng nhập đã hết hạn, vui lòng đăng nhập lại');</script>";
                return RedirectToAction("Index", "Login");
            }

            if (phongBanFilter > 0 && phongBanFilter != MyAuthentication.IDPhongban)
            {
                return Json(new { error = true, message = "Không thể chỉnh sửa ở bộ phận khác!" }, JsonRequestBehavior.AllowGet);
            }

            var tenPhongBan = db.PhongBans
                            .Where(p => p.IDPhongBan == MyAuthentication.IDPhongban)
                            .Select(p => p.TenPhongBan)
                            .FirstOrDefault();

            ViewBag.TenBoPhan = tenPhongBan;

            var data = db.BDT_ThanhVienTieuBan.Where(x => x.ID == id).SingleOrDefault();
            if (data == null)
            {
                TempData["msgError"] = "<script>alert('Không tìm thấy dữ liệu');</script>";
                return RedirectToAction("", "Home");
            }
            var nhanVien = db.NhanViens.Where(x =>x.ID == data.NhanVien_ID).FirstOrDefault();
            var viTriTieuBan = db.BDT_ViTriTieuBan.Where(x => x.ID == data.ViTriTieuBan_ID).FirstOrDefault();

            var DTO = new ThanhVienTieuBanInfo()
            {
                Id = nhanVien.ID,
                HoTen = nhanVien.HoTen,
                TenViTriTieuBan =  viTriTieuBan.TenViTri,
                ViTriTieuBan_ID = viTriTieuBan.ID
            };

            ViewBag.NhanVienDangChon = DTO.HoTen;

            var danhSachViTriTieuBan = db.BDT_ViTriTieuBan
                .Select(vt => new { vt.ID, vt.TenViTri })
                .ToList();

            ViewBag.DSViTriTieuBan = new SelectList(danhSachViTriTieuBan, "ID", "TenViTri", DTO.ViTriTieuBan_ID);


            return PartialView(DTO);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ThanhVienTieuBanInfo model)
        {
            try
            {
                var data = db.BDT_ThanhVienTieuBan
                             .SingleOrDefault(x => x.ID == model.Id);

                if (data != null)
                {
                    data.ViTriTieuBan_ID = model.ViTriTieuBan_ID;
                    data.NgayCapNhat = DateTime.Now;
                    data.NgayDenHanCapNhatLai = DateTime.Now.AddMonths(6);
                    data.NhanVienSua_ID = MyAuthentication.ID;
                    db.SaveChanges();

                    TempData["msgSuccess"] = "<script>alert('Cập nhật thành công!');</script>";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["msgError"] = "<script>alert('Không tìm thấy dữ liệu để cập nhật');</script>";
                    return RedirectToAction("Index");
                }
            }
            catch
            {
                TempData["msgError"] = "<script>alert('Lỗi khi lưu dữ liệu.');</script>";
                return View(model);
            }
        }

        [HttpPost]
        public JsonResult Delete(int id, int? phongBanFilter)
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            if (!ListQuyen.Contains(CONSTKEY.DEL))
            {
                return Json(new { success = false, message = "Bạn không có quyền thực hiện chức năng này!" });
            }

            if (phongBanFilter > 0 && phongBanFilter != MyAuthentication.IDPhongban)
            {
                return Json(new { success = false, message = "Không thể xóa ở bộ phận khác!" });
            }

            try
            {
                var record = db.BDT_ThanhVienTieuBan.SingleOrDefault(x => x.ID == id);
                if (record == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy dữ liệu cần xóa!" });
                }

                db.BDT_ThanhVienTieuBan.Remove(record);
                db.SaveChanges();

                return Json(new { success = true, message = "Xóa thành viên khỏi tiểu ban thành công!" });
            }
            catch
            {
                return Json(new { success = false, message = "Có lỗi xảy ra khi xóa!" });
            }
        }

        public ActionResult TrinhKy(int? phongBanFilter)
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            //if (!ListQuyen.Contains(CONSTKEY.VIEW_ALL))
            //{
            //    TempData["msgError"] = "<script>alert('Bạn không có quyền thực hiện chức năng này');</script>";
            //    return RedirectToAction("", "Home");
            //}

            if (phongBanFilter > 0 && phongBanFilter != MyAuthentication.IDPhongban)
            {
                return Json(new { error = true, message = "Không thể trình ký ở bộ phận khác." }, JsonRequestBehavior.AllowGet);
            }

            var username = MyAuthentication.Username;
            if (username == null)
            {
                TempData["msgError"] = "<script>alert('Phiên đăng nhập đã hết hạn, vui lòng đăng nhập lại');</script>";
                return RedirectToAction("Index", "Login");
            }

            var danhSachTruongPho = (from nv in db.NhanViens
                                     join vt in db.ViTriKNLs on nv.IDVTKNL equals vt.IDVT
                                     where (vt.MaViTri == "TBP" || vt.MaViTri == "PBP")
                                           && vt.IDPB == MyAuthentication.IDPhongban
                                     select new
                                     {
                                         nv.ID,
                                         nv.HoTen
                                     }).ToList();
            ViewBag.DanhSachTruongPho = new SelectList(danhSachTruongPho, "ID", "HoTen");

            var tenPhongBan = db.PhongBans
                            .Where(p => p.IDPhongBan == MyAuthentication.IDPhongban)
                            .Select(p => p.TenPhongBan)
                            .FirstOrDefault();

            ViewBag.TenBoPhan = tenPhongBan;

            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TrinhKy(string SelectedIds, int NguoiTrinhKy)
        {
            var username = MyAuthentication.Username;
            if (username == null)
            {
                TempData["msgError"] = "<script>alert('Phiên đăng nhập đã hết hạn, vui lòng đăng nhập lại');</script>";
                return RedirectToAction("Index", "Login");
            }

            if (string.IsNullOrEmpty(SelectedIds))
            {
                TempData["msgError"] = "<script>alert('Chưa có vị trí nào được chọn.');</script>";
                return RedirectToAction("Index");
            }

            var idList = SelectedIds.Split(',').Select(int.Parse).ToList();
            var ngayTrinhKy = DateTime.Now;

            foreach (var id in idList)
            {
                var thanhVien = db.BDT_ThanhVienTieuBan.FirstOrDefault(x => x.ID == id);
                if (thanhVien != null)
                {
                    var record = new BDT_LichSuPheDuyet
                    {
                        TieuBan_ID = thanhVien.TieuBan_ID,
                        NgayTrinhKy = ngayTrinhKy,
                        TrangThai = 1, // đang trình ký
                        NguoiPheDuyet_ID = NguoiTrinhKy,
                        NguoiTrinhKy_ID = MyAuthentication.ID,
                        ThanhVienTieuBan_ID = thanhVien.ID
                    };
                    db.BDT_LichSuPheDuyet.Add(record);

                    thanhVien.TrangThai = 1; // đang trình ký
                    thanhVien.NgayCapNhat = DateTime.Now;
                    thanhVien.NgayDenHanCapNhatLai = DateTime.Now.AddMonths(6);
                }
            }

            db.SaveChanges();
            TempData["msgSuccess"] = "<script>alert('Trình ký thành công!');</script>";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public JsonResult HuyTrinhKy(int id)
        {
            var thanhVien = db.BDT_ThanhVienTieuBan.FirstOrDefault(x => x.ID == id && x.TrangThai == 1);
            if (thanhVien == null)
            {
                return Json(new { success = false });
            }

            var lichSuTrinhKy = db.BDT_LichSuPheDuyet.FirstOrDefault(x => x.ThanhVienTieuBan_ID == id);
            if (lichSuTrinhKy != null)
            {
                db.BDT_LichSuPheDuyet.Remove(lichSuTrinhKy);
            }

            thanhVien.TrangThai = 0;

            db.SaveChanges();

            return Json(new { success = true });
        }
        
        public ActionResult PheDuyetTrinhKy(int? viTriTieuBanFilter, string searchName, int? page)
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            if (!ListQuyen.Contains(CONSTKEY.DUYET))
            {
                TempData["msgError"] = "<script>alert('Bạn không có quyền thực hiện chức năng này');</script>";
                return RedirectToAction("", "Home");
            }

            var tenPhongBan = db.PhongBans
                            .Where(p => p.IDPhongBan == MyAuthentication.IDPhongban)
                            .Select(p => p.TenPhongBan)
                            .FirstOrDefault();

            ViewBag.TenBoPhan = tenPhongBan;

            var danhSachViTri = db.BDT_ViTriTieuBan
                .Select(v => new { v.ID, v.TenViTri })
                .ToList();
            ViewBag.DSViTriTieuBan = new SelectList(danhSachViTri, "ID", "TenViTri", viTriTieuBanFilter);

            ViewBag.SearchName = searchName;

            var result = (from nv in db.NhanViens
                          join vtKNL in db.ViTriKNLs on nv.IDVTKNL equals vtKNL.IDVT
                          join tvTieuBan in db.BDT_ThanhVienTieuBan on nv.ID equals tvTieuBan.NhanVien_ID
                          join vtTieuBan in db.BDT_ViTriTieuBan on tvTieuBan.ViTriTieuBan_ID equals vtTieuBan.ID
                          join tieuBan in db.BDT_TieuBan on tvTieuBan.TieuBan_ID equals tieuBan.ID
                          join lichSu in db.BDT_LichSuPheDuyet on tvTieuBan.ID equals lichSu.ThanhVienTieuBan_ID
                          where
                             lichSu.NguoiPheDuyet_ID == MyAuthentication.ID
                          //&& lichSu.TrangThai == 1
                          select new ThanhVienTieuBanInfo
                          {
                              Id = tvTieuBan.ID,
                              MaViTriKNL = (int)tvTieuBan.ViTriKNL_ID,
                              TenViTriKNL = vtKNL.TenViTri,
                              HoTen = nv.HoTen,
                              TenViTriTieuBan = vtTieuBan.TenViTri,
                              NgayCapNhatGanNhat = (DateTime)tvTieuBan.NgayCapNhat,
                              NgayDenHanCapNhat = (DateTime)tvTieuBan.NgayDenHanCapNhatLai,
                              TrangThai = (int)tvTieuBan.TrangThai,
                              PhongBanID = (int)tieuBan.PhongBan_ID,
                              ViTriTieuBan_ID = (int)tvTieuBan.ViTriTieuBan_ID,
                              NgayPheDuyet = lichSu.NgayPheDuyet,
                              HoTenNguoiTrinhKy = db.NhanViens.Where(x => x.ID == lichSu.NguoiTrinhKy_ID).FirstOrDefault().HoTen
                          });

            if (viTriTieuBanFilter.HasValue && viTriTieuBanFilter.Value != 0)
            {
                result = result.Where(x => x.ViTriTieuBan_ID == viTriTieuBanFilter.Value);
            }

            if (!string.IsNullOrEmpty(searchName))
            {
                result = result.Where(x => x.HoTen.Contains(searchName));
            }

            int pageSize = 20;
            int pageNumber = (page ?? 1);

            var pagedResult = result.OrderBy(x => x.TrangThai).ToPagedList(pageNumber, pageSize);

            return View(pagedResult);
        }

        [HttpPost]
        public JsonResult PheDuyet(List<int> ids)
        {
            try
            {
                var ngayHienTai = DateTime.Now;

                var thanhVienList = db.BDT_ThanhVienTieuBan
                    .Where(x => ids.Contains(x.ID) && x.TrangThai == 1)
                    .ToList();

                foreach (var thanhVienMoi in thanhVienList)
                {
                    var thanhVienCu = db.BDT_ThanhVienTieuBan
                        .FirstOrDefault(x =>
                            x.NhanVien_ID == thanhVienMoi.NhanVien_ID &&
                            !ids.Contains(x.ID)
                        );

                    if (thanhVienCu != null)
                    {
                        var lichSuCu = db.BDT_LichSuPheDuyet
                            .Where(x => x.ThanhVienTieuBan_ID == thanhVienCu.ID)
                            .ToList();
                        db.BDT_LichSuPheDuyet.RemoveRange(lichSuCu);

                        db.BDT_ThanhVienTieuBan.Remove(thanhVienCu);
                    }

                    thanhVienMoi.TrangThai = 2;
                }

                var lichSuList = db.BDT_LichSuPheDuyet
                    .Where(x => ids.Contains((int)x.ThanhVienTieuBan_ID) && x.TrangThai == 1)
                    .ToList();

                foreach (var item in lichSuList)
                {
                    item.NgayPheDuyet = ngayHienTai;
                    item.TrangThai = 2;
                }

                db.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public ActionResult ExportExcel(int? phongBanFilter)
        {
            var username = MyAuthentication.Username;
            if (username == null)
            {
                TempData["msgError"] = "<script>alert('Phiên đăng nhập đã hết hạn, vui lòng đăng nhập lại');</script>";
                return RedirectToAction("Index", "Login");
            }

            phongBanFilter = phongBanFilter.HasValue && phongBanFilter > 0 ? phongBanFilter : MyAuthentication.IDPhongban;

            var result = (from nv in db.NhanViens
                          join vtKNL in db.ViTriKNLs on nv.IDVTKNL equals vtKNL.IDVT
                          join tvTieuBan in db.BDT_ThanhVienTieuBan on nv.ID equals tvTieuBan.NhanVien_ID
                          join vtTieuBan in db.BDT_ViTriTieuBan on tvTieuBan.ViTriTieuBan_ID equals vtTieuBan.ID
                          join tieuBan in db.BDT_TieuBan on tvTieuBan.TieuBan_ID equals tieuBan.ID
                          where tieuBan.PhongBan_ID == phongBanFilter

                          // LEFT JOIN với LichSuPheDuyet
                          join lichSuGroup in db.BDT_LichSuPheDuyet
                              on tvTieuBan.ID equals lichSuGroup.ThanhVienTieuBan_ID into lichSuLeftJoin
                          from lichSu in lichSuLeftJoin.DefaultIfEmpty()

                          orderby tvTieuBan.ViTriTieuBan_ID
                          select new ThanhVienTieuBanInfo
                          {
                              Id = tvTieuBan.ID,
                              MaViTriKNL = (int)tvTieuBan.ViTriKNL_ID,
                              TenViTriKNL = vtKNL.TenViTri,
                              MaNhanVien = nv.MaNV,
                              HoTen = nv.HoTen,
                              TenViTriTieuBan = vtTieuBan.TenViTri,
                              NgayCapNhatGanNhat = (DateTime)tvTieuBan.NgayCapNhat,
                              NgayDenHanCapNhat = (DateTime)tvTieuBan.NgayDenHanCapNhatLai,
                              TrangThai = (int)tvTieuBan.TrangThai,
                              MaNhanVienNguoiPheDuyet = lichSu != null
                                  ? db.NhanViens.Where(x => x.ID == lichSu.NguoiPheDuyet_ID).Select(x => x.MaNV).FirstOrDefault()
                                  : "",
                              HoTenNguoiPheDuyet = lichSu != null
                                  ? db.NhanViens.Where(x => x.ID == lichSu.NguoiPheDuyet_ID).Select(x => x.HoTen).FirstOrDefault()
                                  : "",
                          }).ToList();


            var templatePath = Server.MapPath("~/App_Data/DanhSachTieuBan_Template.xlsx");

            using (var workbook = new XLWorkbook(templatePath))
            {
                var worksheet = workbook.Worksheet(1);

                int startRow = 2;
                int stt = 1;
                foreach (var item in result)
                {
                    worksheet.Cell(startRow, 1).Value = stt++;
                    worksheet.Cell(startRow, 2).Value = item.MaNhanVien;
                    worksheet.Cell(startRow, 3).Value = item.HoTen;
                    worksheet.Cell(startRow, 4).Value = item.TenViTriTieuBan;
                    worksheet.Cell(startRow, 5).Value = item.TenViTriKNL;
                    worksheet.Cell(startRow, 6).Value = item.MaNhanVienNguoiPheDuyet;
                    worksheet.Cell(startRow, 7).Value = item.HoTenNguoiPheDuyet;
                    worksheet.Cell(startRow, 8).Value = item.TrangThai == 0 ? "Chưa trình ký" : (item.TrangThai == 1 ? "Đang trình ký" : item.TrangThai == 2 ? "Đang hiệu lực" : "Hết hiệu lực");
                    worksheet.Cell(startRow, 9).Value = item.NgayCapNhatGanNhat.ToString("dd/MM/yyyy");
                    worksheet.Cell(startRow, 10).Value = item.NgayDenHanCapNhat.ToString("dd/MM/yyyy");

                    startRow++;
                }

                worksheet.Columns().AdjustToContents();

                var maPhongBan = db.PhongBans
                    .Where(p => p.IDPhongBan == phongBanFilter)
                    .Select(p => p.MaPB)
                    .FirstOrDefault();

                string fileName = "DanhSachTieuBan_" + maPhongBan + ".xlsx";
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Position = 0;
                    return File(stream.ToArray(), contentType, fileName);
                }
            }
        }

        [HttpPost]
        public ActionResult Import(HttpPostedFileBase file)
        {
            if (file == null || file.ContentLength == 0)
            {
                TempData["msgError"] = "<script>alert('Vui lòng chọn file hợp lệ.');</script>";
                return RedirectToAction("Index");
            }

            try
            {
                using (var workbook = new XLWorkbook(file.InputStream))
                {
                    var worksheet = workbook.Worksheet(1);
                    var rows = worksheet.RangeUsed().RowsUsed().Skip(1);

                    if (rows.Any(row => row.Cell(8).GetString().Trim() == "Đang hiệu lực"))
                    {
                        TempData["msgError"] = "<script>alert('Dữ liệu file Excel không hợp lệ ở cột trạng thái.');</script>";
                        return RedirectToAction("Index");
                    }

                    var tieuBanID = db.BDT_TieuBan
                        .Where(x => x.PhongBan_ID == MyAuthentication.IDPhongban)
                        .Select(x => x.ID)
                        .FirstOrDefault();

                    foreach (var row in rows)
                    {
                        string maNV = row.Cell(2).GetString().Trim();
                        string viTriTieuBanStr = row.Cell(4).GetString().Trim();
                        string maNguoiPheDuyet = row.Cell(6).GetString().Trim();
                        string trangThaiStr = row.Cell(8).GetString().Trim();

                        var nhanVien = db.NhanViens.FirstOrDefault(x => x.MaNV == maNV);
                        if (nhanVien == null) continue;

                        int viTriTieuBanID = viTriTieuBanStr == "Trưởng tiểu ban" ? 1 :
                                             viTriTieuBanStr == "Phó tiểu ban" ? 2 :
                                             viTriTieuBanStr == "Thành viên thường trực" ? 3 : 4;

                        int trangThai = trangThaiStr == "Chưa trình ký" ? 0 : 1;

                        var tonTaiThanhVien = db.BDT_ThanhVienTieuBan
                            .Where(x => x.TieuBan_ID == tieuBanID && x.NhanVien_ID == nhanVien.ID)
                            .ToList();

                        bool daTonTai = tonTaiThanhVien.Any();

                        if (!daTonTai ||
                            !tonTaiThanhVien.Any(x => x.ViTriTieuBan_ID == viTriTieuBanID) ||
                            !tonTaiThanhVien.Any(x => x.TrangThai == trangThai))
                        {
                            var thanhVien = new BDT_ThanhVienTieuBan
                            {
                                TieuBan_ID = tieuBanID,
                                NhanVien_ID = nhanVien.ID,
                                ViTriKNL_ID = nhanVien.IDVTKNL,
                                ViTriTieuBan_ID = viTriTieuBanID,
                                TrangThai = trangThai,
                                NgayCapNhat = DateTime.Now,
                                NgayDenHanCapNhatLai = DateTime.Now.AddMonths(6),
                                NhanVienThem_ID = MyAuthentication.ID,
                            };

                            db.BDT_ThanhVienTieuBan.Add(thanhVien);
                            db.SaveChanges();

                            if (trangThai == 1)
                            {
                                var nguoiPheDuyet = db.NhanViens.FirstOrDefault(x => x.MaNV == maNguoiPheDuyet);
                                if (nguoiPheDuyet != null)
                                {
                                    var lichSu = new BDT_LichSuPheDuyet
                                    {
                                        TieuBan_ID = tieuBanID,
                                        NguoiTrinhKy_ID = MyAuthentication.ID,
                                        NguoiPheDuyet_ID = nguoiPheDuyet.ID,
                                        NgayTrinhKy = DateTime.Now,
                                        TrangThai = 1,
                                        ThanhVienTieuBan_ID = thanhVien.ID,
                                    };
                                    db.BDT_LichSuPheDuyet.Add(lichSu);
                                    db.SaveChanges();
                                }
                            }
                        }
                    }
                }

                TempData["msgSuccess"] = "<script>alert('Import dữ liệu thành công!');</script>";
            }
            catch (Exception ex)
            {
                TempData["msgError"] = $"<script>alert('Lỗi: {ex.Message}');</script>";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult XoaThanhVienTieuBan(List<int> ids)
        {
            if (ids == null || !ids.Any())
            {
                return Json(new { success = false, message = "Danh sách ID rỗng." });
            }

            try
            {
                var lichSu = db.BDT_LichSuPheDuyet
                    .Where(x => ids.Contains((int) x.ThanhVienTieuBan_ID))
                    .ToList();
                db.BDT_LichSuPheDuyet.RemoveRange(lichSu);

                var thanhViens = db.BDT_ThanhVienTieuBan
                    .Where(x => ids.Contains(x.ID))
                    .ToList();
                db.BDT_ThanhVienTieuBan.RemoveRange(thanhViens);

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