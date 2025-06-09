using E_Learning.Models;
using E_Learning.ModelsDMST;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace E_Learning.Controllers.DMST
{
    public class RenewLinhVucController : Controller
    {
        ELEARNINGEntities db = new ELEARNINGEntities();
        int Idquyen = MyAuthentication.IDQuyen;
        String ControllerName = "RenewLinhVuc";
        // GET: RenewLinhVuc
        public ActionResult Index(int? page, string linhVucFilter)
        {
            int? filterLinhVucID = null;
            if (!string.IsNullOrEmpty(linhVucFilter))
            {
                int parsed;
                if (int.TryParse(linhVucFilter, out parsed))
                    filterLinhVucID = parsed;
            }

            var linhVucsQuery = db.DMST_LinhVuc.Where(x => x.TrangThai == 1).AsQueryable();
            if (filterLinhVucID.HasValue)
            {
                linhVucsQuery = linhVucsQuery.Where(lv => lv.ID == filterLinhVucID.Value);
            }

            var allLinhVucs = linhVucsQuery.ToList();

            var data = allLinhVucs.Select(lv =>
            {
                var xuLyLinhVucs = db.DMST_XuLyLinhVuc
                    .Where(x => x.ID_LinhVuc == lv.ID)
                    .ToList();

                var phongBans = xuLyLinhVucs
                    .GroupBy(x => x.ID_PhongBanXL)
                    .Select(pbGroup =>
                    {
                        var tenPhongBan = db.PhongBans
                            .Where(pb => pb.IDPhongBan == pbGroup.Key)
                            .Select(pb => pb.TenPhongBan)
                            .FirstOrDefault();

                        var nhanViens = pbGroup
                            .GroupBy(x => x.ID_NhanVienXL)
                            .Select(nvGroup =>
                            {
                                var hoTen = db.NhanViens
                                    .Where(nv => nv.ID == nvGroup.Key)
                                    .Select(nv => nv.HoTen)
                                    .FirstOrDefault();

                                var chucVuNames = nvGroup
                                    .Select(x => db.DMST_ChucVu
                                        .Where(cv => cv.ID == x.ID_ChucVuXL)
                                        .Select(cv => cv.TenChucVu)
                                        .FirstOrDefault())
                                    .Where(cv => !string.IsNullOrEmpty(cv))
                                    .Distinct()
                                    .ToList();

                                return new NhanVienXuLyViewModel
                                {
                                    HoTen = hoTen,
                                    ChucVus = chucVuNames
                                };
                            }).ToList();

                        return new PhongBanXuLyViewModel
                        {
                            TenPhongBan = tenPhongBan,
                            NhanViens = nhanViens
                        };
                    }).ToList();

                return new XuLyLinhVucViewModel
                {
                    ID_LinhVuc = lv.ID,
                    TenLinhVuc = lv.TenLinhVuc,
                    PhongBans = phongBans
                };
            }).ToList();

            ViewBag.DSLinhVuc = new SelectList(db.DMST_LinhVuc
                .OrderBy(x => x.ID)
                .Select(pb => new SelectListItem
                {
                    Value = pb.ID.ToString(),
                    Text = pb.TenLinhVuc
                }).ToList(), "Value", "Text", linhVucFilter);

            ViewBag.linhVucFilter = linhVucFilter;

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            var pagedList = data.OrderBy(x => x.ID_LinhVuc).ToPagedList(pageNumber, pageSize);

            return View(pagedList);
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

            return PartialView();
        }

        [HttpPost]
        public ActionResult Create(DMST_LinhVuc DTO)
        {
            try
            {
                var data = new DMST_LinhVuc
                {
                    TenLinhVuc = DTO.TenLinhVuc,
                    TrangThai = 1
                };

                db.DMST_LinhVuc.Add(data);
                db.SaveChanges();

                TempData["msgSuccess"] = "<script>alert('Thêm mới thành công');</script>";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["msgError"] = "<script>alert('Lỗi khi thêm mới: " + ex.Message + "');</script>";
                return RedirectToAction("Index");
            }
        }

        public ActionResult Edit(int id)
        {
            var linhVuc = db.DMST_LinhVuc.FirstOrDefault(x => x.ID == id);
            if (linhVuc == null)
            {
                return HttpNotFound();
            }

            var dsXuLy = db.DMST_XuLyLinhVuc
                .Where(x => x.ID_LinhVuc == id)
                .Select(x => new XuLyItem
                {
                    ID_PhongBanXL = (int) x.ID_PhongBanXL,
                    ID_NhanVienXL = (int) x.ID_NhanVienXL,
                    ID_ChucVuXL = (int) x.ID_ChucVuXL
                }).ToList();

            ViewBag.XuLyRows = dsXuLy;

            ViewBag.DSPhongBan = db.PhongBans
                .Select(pb => new SelectListItem
                {
                    Value = pb.IDPhongBan.ToString(),
                    Text = pb.TenPhongBan
                }).OrderBy(x => x.Text).ToList();

            ViewBag.DSChucVu = db.DMST_ChucVu
                .OrderBy(cv => cv.ID)
                .Select(cv => new SelectListItem
                {
                    Value = cv.ID.ToString(),
                    Text = cv.TenChucVu + (cv.MaToChuc == "1" ? " - Ban ĐMST Công ty" : " - Tiểu ban ĐMST Bộ phận")
                }).ToList();

            ViewBag.TenLinhVuc = linhVuc.TenLinhVuc;

            return PartialView(linhVuc);
        }

        public ActionResult RenderRowPartial()
        {
            ViewBag.DSPhongBan = db.PhongBans.Select(pb => new SelectListItem
            {
                Value = pb.IDPhongBan.ToString(),
                Text = pb.TenPhongBan
            }).OrderBy(x => x.Text).ToList();

            ViewBag.DSChucVu = db.DMST_ChucVu
            .OrderBy(cv => cv.ID)
            .Select(cv => new SelectListItem
            {
                Value = cv.ID.ToString(),
                Text = cv.TenChucVu + (cv.MaToChuc == "1" ? " - Ban ĐMST Công ty" : " - Tiểu ban ĐMST Bộ phận")
            }).ToList();

            return PartialView("_XuLyLinhVucRow");
        }

        public JsonResult GetNhanVienTheoPhongBan(int idPB)
        {
            var ds = db.NhanViens
                .Where(nv => nv.IDPhongBan == idPB && nv.IDTinhTrangLV == 1)
                .Select(nv => new SelectListItem
                {
                    Value = nv.ID.ToString(),
                    Text = nv.MaNV + " - " + nv.HoTen
                }).ToList();

            return Json(ds, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult InsertXuLyLinhVuc(XuLyLinhVucViewModel input)
        {
            if (input == null || input.XuLyList == null || !input.XuLyList.Any())
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Dữ liệu không hợp lệ.");
            }

            var idLinhVuc = input.ID_LinhVuc;

            var existingRecords = db.DMST_XuLyLinhVuc
                .Where(x => x.ID_LinhVuc == idLinhVuc)
                .ToList();

            var viewKeys = input.XuLyList
                .Select(x => new { x.ID_NhanVienXL, x.ID_ChucVuXL, x.ID_PhongBanXL })
                .ToList();

            foreach (var row in viewKeys)
            {
                bool isExists = existingRecords.Any(x =>
                    x.ID_NhanVienXL == row.ID_NhanVienXL &&
                    x.ID_ChucVuXL == row.ID_ChucVuXL &&
                    x.ID_PhongBanXL == row.ID_PhongBanXL);

                if (!isExists)
                {
                    var record = new DMST_XuLyLinhVuc
                    {
                        ID_LinhVuc = idLinhVuc,
                        ID_PhongBanXL = row.ID_PhongBanXL,
                        ID_NhanVienXL = row.ID_NhanVienXL,
                        ID_ChucVuXL = row.ID_ChucVuXL
                    };

                    db.DMST_XuLyLinhVuc.Add(record);
                }
            }

            var viewSet = new HashSet<string>(
                viewKeys.Select(x => $"{x.ID_PhongBanXL}_{x.ID_NhanVienXL}_{x.ID_ChucVuXL}")
            );

            var toDelete = existingRecords.Where(x =>
                !viewSet.Contains($"{x.ID_PhongBanXL}_{x.ID_NhanVienXL}_{x.ID_ChucVuXL}")
            ).ToList();

            if (toDelete.Any())
            {
                db.DMST_XuLyLinhVuc.RemoveRange(toDelete);
            }

            db.SaveChanges();
            return Json(new { success = true });
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var linhVuc = db.DMST_LinhVuc.FirstOrDefault(x => x.ID == id);
            if (linhVuc == null)
            {
                return HttpNotFound();
            }

            linhVuc.TrangThai = 0;
            db.SaveChanges();

            return Json(new { success = true });
        }
    }
}