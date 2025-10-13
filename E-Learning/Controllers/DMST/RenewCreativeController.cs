using E_Learning.Models;
using E_Learning.ModelsDMST;
using PagedList;
using System;
using System.IO;
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

        public ActionResult Create()
        {
            var model = new DeTaiDMSTView
            {
                LinhVucList = db.DMST_LinhVuc
                       .Select(lv => new SelectListItem
                       {
                           Value = lv.ID.ToString(),
                           Text = lv.TenLinhVuc
                       })
                       .ToList(),

                PhongBanList = db.PhongBans
                        .Select(pb => new SelectListItem
                        {
                            Value = pb.IDPhongBan.ToString(),
                            Text = pb.TenPhongBan
                        })
                        .OrderBy(x => x.Text)
                        .ToList(),
                PhamViApDung = 1
            };

            string tenNhanVien = db.NhanViens
                .Where(x => x.ID == MyAuthentication.ID)
                .Select(x => x.HoTenKhongDau)
                .FirstOrDefault();

            ViewBag.Username = MyAuthentication.Username + " – " + tenNhanVien;

            return View(model);
        }

        [HttpGet]
        public JsonResult GetDanhSachDeTaiJson_(int page = 1, string keyword = "")
        {
            int pageSize = 20;

            var dsKhenThuongRaw = (from k in db.KT_DanhSachKhenThuong
                                   select new { k.NoiDungKhenThuong, k.DonVi }).ToList();

            var dsKhenThuong = dsKhenThuongRaw
                .GroupBy(x => x.NoiDungKhenThuong)
                .Select(g => new DeTaiListItemViewModel
                {
                    TenDeTai = g.Key,
                    BoPhanThamGia = string.Join(" - ", g.Select(x => x.DonVi)),
                    BoPhanApDung = "Toàn công ty",
                    TrangThai = 2
                }).ToList();


            var dsDeTaiRaw = (from d in db.DMST_DeTai
                              join dp in db.DMST_DeTai_PhongBanThamGia on d.ID equals dp.DeTaiID
                              join p in db.PhongBans on dp.PhongBanID equals p.IDPhongBan
                              join da in db.DMST_DeTai_PhongBanApDung on d.ID equals da.DeTaiID into apDung
                              from da in apDung.DefaultIfEmpty()
                              join pbApDung in db.PhongBans on da.PhongBanID equals pbApDung.IDPhongBan into pbApDungJoin
                              from pbApDung in pbApDungJoin.DefaultIfEmpty()
                              select new
                              {
                                  d.TenDeTai,
                                  d.TrangThai,
                                  BoPhanThamGia = p.MaPB,
                                  BoPhanApDung = pbApDung != null ? pbApDung.MaPB : "Toàn công ty"
                              }).ToList();

            var dsDeTai = dsDeTaiRaw
                .GroupBy(x => new { x.TenDeTai, x.TrangThai })
                .Select(g => new DeTaiListItemViewModel
                {
                    TenDeTai = g.Key.TenDeTai,
                    BoPhanThamGia = string.Join(" - ", g.Select(x => x.BoPhanThamGia)),
                    BoPhanApDung = string.Join(" - ", g.Select(x => x.BoPhanApDung).Distinct()),
                    TrangThai = g.Key.TrangThai
                }).ToList();

            var danhSach = dsKhenThuong.Union(dsDeTai).ToList();

            if (!string.IsNullOrEmpty(keyword))
            {
                danhSach = danhSach.Where(x => x.TenDeTai.ToLower().Contains(keyword.ToLower())).ToList();
            }

            int totalItems = danhSach.Count;
            var pageData = danhSach.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            int totalPages = (int) Math.Ceiling((double)totalItems / pageSize);

            return Json(new
            {
                Data = pageData,
                CurrentPage = page,
                TotalPages = totalPages
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetDanhSachDeTaiJson(int page = 1, string keyword = "")
        {
            int pageSize = 20;

            var dsKhenThuongRaw = (from k in db.KT_DanhSachKhenThuong
                                   select new { k.NoiDungKhenThuong, k.DonVi }).ToList();

            var dsKhenThuong = dsKhenThuongRaw
                .GroupBy(x => x.NoiDungKhenThuong)
                .Select(g => new DeTaiListItemViewModel
                {
                    TenDeTai = g.Key,
                    BoPhanThamGia = string.Join(" - ", g.Select(x => x.DonVi)),
                    BoPhanApDung = "Toàn công ty",
                    TrangThai = 2,
                    TepDinhKem = null
                }).ToList();

            var dsDeTaiRaw = (from d in db.DMST_DeTai
                              join dp in db.DMST_DeTai_PhongBanThamGia on d.ID equals dp.DeTaiID
                              join p in db.PhongBans on dp.PhongBanID equals p.IDPhongBan
                              join da in db.DMST_DeTai_PhongBanApDung on d.ID equals da.DeTaiID into apDung
                              from da in apDung.DefaultIfEmpty()
                              join pbApDung in db.PhongBans on da.PhongBanID equals pbApDung.IDPhongBan into pbApDungJoin
                              from pbApDung in pbApDungJoin.DefaultIfEmpty()
                              select new
                              {
                                  d.TenDeTai,
                                  d.TrangThai,
                                  BoPhanThamGia = p.MaPB,
                                  BoPhanApDung = pbApDung != null ? pbApDung.MaPB : "Toàn công ty",
                                  d.TepDinhKem
                              }).ToList();

            var dsDeTai = dsDeTaiRaw
                .GroupBy(x => new { x.TenDeTai, x.TrangThai })
                .Select(g => new DeTaiListItemViewModel
                {
                    TenDeTai = g.Key.TenDeTai,
                    BoPhanThamGia = string.Join(" - ", g.Select(x => x.BoPhanThamGia)),
                    BoPhanApDung = string.Join(" - ", g.Select(x => x.BoPhanApDung).Distinct()),
                    TrangThai = g.Key.TrangThai,
                    TepDinhKem = g.Select(x => x.TepDinhKem).FirstOrDefault(f => !string.IsNullOrEmpty(f))
                }).ToList();

            var danhSach = dsKhenThuong.Union(dsDeTai).ToList();

            if (!string.IsNullOrEmpty(keyword))
            {
                danhSach = danhSach.Where(x => x.TenDeTai.ToLower().Contains(keyword.ToLower())).ToList();
            }

            int totalItems = danhSach.Count;
            var pageData = danhSach.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            return Json(new
            {
                Data = pageData,
                CurrentPage = page,
                TotalPages = totalPages
            }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DeTaiDMSTView model)
        {
            if (ModelState.IsValid)
            {
                string filePathInDb = null;

                if (model.TepDinhKem != null && model.TepDinhKem.ContentLength > 0)
                {
                    var uploadDir = Server.MapPath("~/Uploads/DoiMoiSangTao/");
                    if (!Directory.Exists(uploadDir))
                    {
                        Directory.CreateDirectory(uploadDir);
                    }

                    string fileName = Path.GetFileNameWithoutExtension(model.TepDinhKem.FileName);
                    string extension = Path.GetExtension(model.TepDinhKem.FileName);
                    string safeFileName = fileName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + extension;

                    string path = Path.Combine(uploadDir, safeFileName);
                    model.TepDinhKem.SaveAs(path);

                    filePathInDb = "/Uploads/DoiMoiSangTao/" + safeFileName;
                }

                var deTai = new DMST_DeTai
                {
                    TenDeTai = model.TenDeTai,
                    LinhVucID = model.LinhVucID,
                    PhamViApDung = model.PhamViApDung,
                    MoTaNgan = model.MoTaNgan,
                    NguoiDangKyID = MyAuthentication.ID,
                    TepDinhKem = filePathInDb,
                    TrangThai = 0,
                    NgayTao = DateTime.Now,
                };
                db.DMST_DeTai.Add(deTai);
                db.SaveChanges();

                if (model.PhongBanThamGiaIDs != null && model.PhongBanThamGiaIDs.Any())
                {
                    foreach (var pbid in model.PhongBanThamGiaIDs)
                    {
                        db.DMST_DeTai_PhongBanThamGia.Add(new DMST_DeTai_PhongBanThamGia
                        {
                            DeTaiID = deTai.ID,
                            PhongBanID = pbid
                        });

                    }
                    db.SaveChanges();
                }

                if (model.PhongBanApDungIDs != null && model.PhongBanApDungIDs.Any())
                {
                    foreach (var pbid in model.PhongBanApDungIDs)
                    {
                        db.DMST_DeTai_PhongBanApDung.Add(new DMST_DeTai_PhongBanApDung
                        {
                            DeTaiID = deTai.ID,
                            PhongBanID = pbid
                        });
                    }
                    db.SaveChanges();
                }

                return RedirectToAction("Create");
            }

            // Reload if ModelState invalid
            model.LinhVucList = db.DMST_LinhVuc
                                  .Select(lv => new SelectListItem
                                  {
                                      Value = lv.ID.ToString(),
                                      Text = lv.TenLinhVuc
                                  })
                                  .ToList();
            model.PhongBanList = db.PhongBans
                                  .Select(pb => new SelectListItem
                                  {
                                      Value = pb.IDPhongBan.ToString(),
                                      Text = pb.TenPhongBan
                                  })
                                  .ToList();

            return View(model);
        }

        [HttpGet]
        public JsonResult GetDeTaiCuaToi()
        {
            int currentUserId = MyAuthentication.ID;

            var deTaiCuaToi = (from d in db.DMST_DeTai
                               where d.NguoiDangKyID == currentUserId
                               select new
                               {
                                   d.TenDeTai,
                                   d.TrangThai
                               }).ToList();

            return Json(new
            {
                Data = deTaiCuaToi
            }, JsonRequestBehavior.AllowGet);
        }
    }
}