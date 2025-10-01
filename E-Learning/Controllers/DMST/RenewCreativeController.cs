using E_Learning.Models;
using E_Learning.ModelsDMST;
using PagedList;
using System;
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
                           ID_NhanVienDaiDien = (int) dt.ID_NhanVienDaiDien,
                           ID_LinhVuc = (int) dt.ID_LinhVuc
                       };

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            var pagedList = data.OrderBy(x => x.TenYTuong).ToPagedList(pageNumber, pageSize);

            return View(pagedList);
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
                        .ToList()
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(DeTaiDMSTView model)
        {
            if (ModelState.IsValid)
            {
                var deTai = new DMST_DeTai
                {
                    TenDeTai = model.TenDeTai,
                    LinhVucID = model.LinhVucID,
                    PhamViApDung = model.PhamViApDung,
                    MoTaNgan = model.MoTaNgan,
                    NguoiDangKyID = model.NguoiDangKyID,
                    //FileDinhKem = model.FileDinhKem != null ? model.FileDinhKem.FileName : null,
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

    }
}