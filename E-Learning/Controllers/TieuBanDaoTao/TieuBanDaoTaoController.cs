using E_Learning.Models;
using E_Learning.ModelsTieuBanDaoTao;
using OfficeOpenXml;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                              ViTriTieuBan_ID = (int)tvTieuBan.ViTriTieuBan_ID
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

                    foreach (var tv in model.ThanhVienList)
                    {

                        var nhanVien = db.NhanViens.Where(x => x.ID == tv.Id).FirstOrDefault();
                        
                        var thanhVien = new BDT_ThanhVienTieuBan
                        {
                            TieuBan_ID = tieuBanId,
                            NhanVien_ID = tv.Id,
                            ViTriTieuBan_ID = tv.ViTri,
                            ViTriKNL_ID = nhanVien.IDVTKNL,
                            NgayCapNhat = DateTime.Now,
                            NgayDenHanCapNhatLai = DateTime.Now.AddMonths(6),
                            TrangThai = 0
                        };
                        db.BDT_ThanhVienTieuBan.Add(thanhVien);
                    }

                    db.SaveChanges();
                    tran.Commit();

                    TempData["msgSuccess"] = "<script>alert('Cập nhật thành công!');</script>";
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

        public ActionResult Edit(int id)
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

        public ActionResult Delete(int id)
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            if (!ListQuyen.Contains(CONSTKEY.DEL))
            {
                TempData["msgError"] = "<script>alert('Bạn không có quyền thực hiện chức năng này');</script>";
                return RedirectToAction("", "Home");
            }
            try
            {
                var record = db.BDT_ThanhVienTieuBan.SingleOrDefault(x => x.ID == id);

                if (record == null)
                {
                    TempData["msgError"] = "<script>alert('Không tìm thấy dữ liệu cần xóa');</script>";
                    return RedirectToAction("Index");
                }

                db.BDT_ThanhVienTieuBan.Remove(record);
                db.SaveChanges();

                TempData["msgSuccess"] = "<script>alert('Xóa thành viên khỏi tiểu ban thành công!');</script>";
                return RedirectToAction("Index");
            }
            catch
            {
                TempData["msgError"] = "<script>alert('Có lỗi xảy ra khi xóa.');</script>";
                return RedirectToAction("Index");
            }
        }

        public ActionResult TrinhKy()
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            //if (!ListQuyen.Contains(CONSTKEY.VIEW_ALL))
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
                              NguoiTrinhKy = db.NhanViens.Where(x => x.ID == lichSu.NguoiTrinhKy_ID).FirstOrDefault().HoTen
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

                var lichSuList = db.BDT_LichSuPheDuyet
                    .Where(x => ids.Contains((int)x.ThanhVienTieuBan_ID) && x.TrangThai == 1)
                    .ToList();

                foreach (var item in lichSuList)
                {
                    item.NgayPheDuyet = ngayHienTai;
                    item.TrangThai = 2;
                }

                var thanhVienList = db.BDT_ThanhVienTieuBan
                    .Where(x => ids.Contains(x.ID) && x.TrangThai == 1)
                    .ToList();

                foreach (var item in thanhVienList)
                {
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

            phongBanFilter = phongBanFilter.HasValue ? phongBanFilter : MyAuthentication.IDPhongban;

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
                              ViTriTieuBan_ID = (int)tvTieuBan.ViTriTieuBan_ID
                          }).Where(x => x.PhongBanID == phongBanFilter && x.TrangThai == 2)
                          .OrderBy(x => x.ViTriTieuBan_ID);

            using (var package = new ExcelPackage())
            {
                var ws = package.Workbook.Worksheets.Add("ThanhVienTieuBan");

                ws.Cells[1, 1].Value = "STT";
                ws.Cells[1, 2].Value = "Mã vị trí KNL";
                ws.Cells[1, 3].Value = "Thành viên";
                ws.Cells[1, 4].Value = "Vị trí KNL";
                ws.Cells[1, 5].Value = "Vị trí trong tiểu ban";
                ws.Cells[1, 6].Value = "Ngày cập nhật gần nhất";
                ws.Cells[1, 7].Value = "Ngày đến hạn cập nhật lại";
                ws.Cells[1, 8].Value = "Trạng thái";

                int row = 2;
                int stt = 1;
                foreach (var item in result)
                {
                    ws.Cells[row, 1].Value = stt++;
                    ws.Cells[row, 2].Value = item.MaViTriKNL;
                    ws.Cells[row, 3].Value = item.HoTen;
                    ws.Cells[row, 4].Value = item.TenViTriKNL;
                    ws.Cells[row, 5].Value = item.TenViTriTieuBan;
                    ws.Cells[row, 6].Value = item.NgayCapNhatGanNhat.ToString("dd/MM/yyyy");
                    ws.Cells[row, 7].Value = item.NgayDenHanCapNhat.ToString("dd/MM/yyyy");
                    ws.Cells[row, 8].Value = item.TrangThai == 0 ? "Chưa trình ký" : (item.TrangThai == 1 ? "Đang trình ký" : (item.TrangThai == 2 ? "Đang hiệu lực" : "Hết hiệu lực"));

                    row++;
                }

                ws.Cells[1, 1, row - 1, 8].AutoFitColumns();
                ws.Cells[1, 1, 1, 8].Style.Font.Bold = true;

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                var maPhongBan = db.PhongBans
                            .Where(p => p.IDPhongBan == phongBanFilter)
                            .Select(p => p.MaPB)
                            .FirstOrDefault();

                string fileName = "DanhSachTieuBan_" + maPhongBan + ".xlsx";
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                return File(stream, contentType, fileName);
            }
        }
    }
}