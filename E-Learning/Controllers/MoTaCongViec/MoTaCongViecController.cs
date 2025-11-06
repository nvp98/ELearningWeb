using E_Learning.Models;
using E_Learning.ModelsBangMTCV;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace E_Learning.Controllers.MoTaCongViec
{
    public class MoTaCongViecController : Controller
    {
        private readonly ELEARNINGEntities db = new ELEARNINGEntities();
        // GET: MoTaCongViec
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LamBaiKiemTra(int? id)
        {
            int userId = MyAuthentication.ID;

            if (!id.HasValue)
            {
                var deChinhThuc = db.MTCV_DeKiemTra.FirstOrDefault(x => x.LaDeChinhThuc == true);
                if (deChinhThuc == null)
                {
                    return HttpNotFound();
                }
                id = deChinhThuc.ID;
            }

            var de = db.MTCV_DeKiemTra.FirstOrDefault(x => x.ID == id.Value);
            if (de == null)
            {
                return HttpNotFound();
            }

            ViewBag.NgayDenHan = de.NgayDenHan;
            bool hetHan = (de.NgayDenHan.HasValue && DateTime.Now.Date > de.NgayDenHan.Value);
            ViewBag.HetHan = hetHan;

            var dsQuanLy = db.NhanViens
                .Where(nv => nv.IDPhongBan == MyAuthentication.IDPhongban && nv.IDTinhTrangLV == 1)
                .Select(nv => new SelectListItem
                {
                    Value = nv.ID.ToString(),
                    Text = nv.MaNV + " - " + nv.HoTen
                })
                .ToList();

            ViewBag.DanhSachQuanLy = dsQuanLy;

            var baiLam = db.MTCV_BaiLam.FirstOrDefault(x => x.DeKiemTraID == id.Value && x.NhanVienID == userId);

            if (baiLam != null)
            {
                var baiLamChiTiet = (
                    from ct in db.MTCV_BaiLamChiTiet
                    join q in db.MTCV_CauHoi on ct.CauHoiID equals q.ID
                    where ct.BaiLamID == baiLam.ID
                    select new CauHoiModel
                    {
                        CauHoiID = q.ID,
                        NoiDung = q.NoiDung,
                        LoaiCauHoi = q.LoaiCauHoi,
                        LuaChonList = db.MTCV_TracNghiem
                            .Where(c => c.CauHoiID == q.ID)
                            .Select(c => new LuaChonModel
                            {
                                LuaChonID = c.ID,
                                NoiDung = c.NoiDung
                            }).ToList(),
                        CauTraLoi = ct.CauTraLoi,
                        LaTracNghiem = (bool)ct.LaTracNghiem
                    }).ToList();

                ViewBag.DeKiemTraID = id.Value;
                ViewBag.DaLamBai = true;
                return View(baiLamChiTiet);
            }
            else
            {
                var model = db.MTCV_CauHoi
                    .Where(q => q.DeKiemTraID == id.Value)
                    .Select(q => new CauHoiModel
                    {
                        CauHoiID = q.ID,
                        NoiDung = q.NoiDung,
                        LoaiCauHoi = q.LoaiCauHoi,
                        LuaChonList = db.MTCV_TracNghiem
                            .Where(c => c.CauHoiID == q.ID)
                            .Select(c => new LuaChonModel
                            {
                                LuaChonID = c.ID,
                                NoiDung = c.NoiDung
                            }).ToList()
                    }).ToList();

                ViewBag.DeKiemTraID = id.Value;
                ViewBag.DaLamBai = false;
                return View(model);
            }
        }

        [HttpPost]
        public ActionResult NopBai(int deKiemTraID, int nguoiChamID, FormCollection form)
        {
            int userId = MyAuthentication.ID;

            bool daLam = db.MTCV_BaiLam.Any(x => x.DeKiemTraID == deKiemTraID && x.NhanVienID == userId);
            if (daLam)
            {
                return new HttpStatusCodeResult(403, "Bạn đã làm bài kiểm tra. Không thể làm lại!");
            }

            string nguoiChamStr = form["nguoiChamID"];

            var baiLam = new MTCV_BaiLam
            {
                DeKiemTraID = deKiemTraID,
                NhanVienID = userId,
                NgayLam = DateTime.Now,
                NguoiChamID = nguoiChamID
            };
            db.MTCV_BaiLam.Add(baiLam);
            db.SaveChanges();

            var cauHoiList = db.MTCV_CauHoi.Where(x => x.DeKiemTraID == deKiemTraID).ToList();

            foreach (var q in cauHoiList)
            {
                if (q.LoaiCauHoi == "TracNghiem")
                {
                    string formKey = $"q_{q.ID}";
                    string luaChonIDStr = form[formKey];
                    int? luaChonID = string.IsNullOrEmpty(luaChonIDStr) ? (int?)null : int.Parse(luaChonIDStr);

                    bool? dungSai = null;
                    if (luaChonID.HasValue)
                    {
                        var dapAnDung = db.MTCV_TracNghiem
                            .FirstOrDefault(x => x.ID == luaChonID && x.LaDapAnDung == true);
                        dungSai = (dapAnDung != null);
                    }

                    db.MTCV_BaiLamChiTiet.Add(new MTCV_BaiLamChiTiet
                    {
                        BaiLamID = baiLam.ID,
                        CauHoiID = q.ID,
                        CauTraLoi = luaChonIDStr,
                        LaTracNghiem = true,
                        DungSai = dungSai
                    });
                }
                else if (q.LoaiCauHoi == "TuLuan")
                {
                    string formKey = $"essay_{q.ID}";
                    string traLoi = form[formKey];

                    db.MTCV_BaiLamChiTiet.Add(new MTCV_BaiLamChiTiet
                    {
                        BaiLamID = baiLam.ID,
                        CauHoiID = q.ID,
                        CauTraLoi = traLoi,
                        LaTracNghiem = false,
                        DungSai = null
                    });
                }
            }

            db.SaveChanges();
            return RedirectToAction("HoanThanhKiemTra");
        }

        public ActionResult HoanThanhKiemTra(int? id)
        {
            int userId = MyAuthentication.ID;
            if (!id.HasValue)
            {
                var deChinhThuc = db.MTCV_DeKiemTra.FirstOrDefault(x => x.LaDeChinhThuc == true);
                if (deChinhThuc == null)
                {
                    return HttpNotFound();
                }
                id = deChinhThuc.ID;
            }

            var baiLam = db.MTCV_BaiLam.FirstOrDefault(x => x.DeKiemTraID == id && x.NhanVienID == userId);

            ViewBag.DaLamBai = baiLam != null;
            return View();
        }

        public ActionResult TaoDeKiemTra()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LuuDe()
        {
            try
            {
                Request.InputStream.Position = 0;
                string json = new System.IO.StreamReader(Request.InputStream).ReadToEnd();

                var model = JsonConvert.DeserializeObject<DeKiemTraModel>(json);

                if (model == null)
                {
                    return Json(new { success = false, message = "Dữ liệu không hợp lệ!" });
                }

                var de = new MTCV_DeKiemTra
                {
                    TenDe = model.TenDe,
                    NgayTao = DateTime.Now,
                    NgayDenHan = model.NgayDenHan,
                    LaDeChinhThuc = model.LaDeChinhThuc
                };
                db.MTCV_DeKiemTra.Add(de);
                db.SaveChanges();

                if (model.LaDeChinhThuc)
                {
                    var otherDeList = db.MTCV_DeKiemTra
                        .Where(x => x.ID != de.ID && (x.LaDeChinhThuc == true || x.LaDeChinhThuc == null))
                        .ToList();

                    foreach (var otherDe in otherDeList)
                    {
                        otherDe.LaDeChinhThuc = false;
                    }

                    de.LaDeChinhThuc = true;

                    db.SaveChanges();
                }

                foreach (var cauHoi in model.CauHoiList)
                {
                    var q = new MTCV_CauHoi
                    {
                        DeKiemTraID = de.ID,
                        LoaiCauHoi = cauHoi.LoaiCauHoi,
                        NoiDung = cauHoi.NoiDung,
                    };
                    db.MTCV_CauHoi.Add(q);
                    db.SaveChanges();

                    if (cauHoi.LoaiCauHoi == "TracNghiem" && cauHoi.LuaChonList != null)
                    {
                        foreach (var lc in cauHoi.LuaChonList)
                        {
                            var l = new MTCV_TracNghiem
                            {
                                CauHoiID = q.ID,
                                NoiDung = lc.NoiDung,
                                LaDapAnDung = lc.LaDapAnDung
                            };
                            db.MTCV_TracNghiem.Add(l);
                        }
                        db.SaveChanges();
                    }
                }

                return Json(new { success = true, message = "Đã lưu đề kiểm tra thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public ActionResult ChamDiem()
        {
            int nguoiChamID = MyAuthentication.ID;

            var data = (
                from bl in db.MTCV_BaiLam
                join nv in db.NhanViens on bl.NhanVienID equals nv.ID
                join vt in db.ViTriKNLs on nv.IDVTKNL equals vt.IDVT into gj
                from vt in gj.DefaultIfEmpty()
                join de in db.MTCV_DeKiemTra on bl.DeKiemTraID equals de.ID
                where bl.NguoiChamID == nguoiChamID
                      && de.LaDeChinhThuc == true

                select new ChamDiemViewModel
                {
                    NhanVienID = nv.ID,
                    HoTen = nv.HoTen,
                    MaNhanVien = nv.MaNV,
                    ViTriCongViec = vt != null ? vt.TenViTri : "",
                    KetQuaTracNghiem = (
                        from ct in db.MTCV_BaiLamChiTiet
                        where ct.BaiLamID == bl.ID && ct.LaTracNghiem == true
                        let tong = db.MTCV_BaiLamChiTiet.Count(x => x.BaiLamID == bl.ID && x.LaTracNghiem == true)
                        let dung = db.MTCV_BaiLamChiTiet.Count(x => x.BaiLamID == bl.ID && x.LaTracNghiem == true && x.DungSai == true)
                        select dung.ToString() + "/" + tong.ToString()
                    ).FirstOrDefault(),

                    CauHoiTuLuan = (
                        from ct in db.MTCV_BaiLamChiTiet
                        join q in db.MTCV_CauHoi on ct.CauHoiID equals q.ID
                        where ct.BaiLamID == bl.ID && ct.LaTracNghiem == false
                        select new ChamDiemTuLuanModel
                        {
                            CauHoi = q.NoiDung,
                            CauTraLoi = ct.CauTraLoi,
                            Diem = (decimal) ct.Diem
                        }
                    ).ToList()
                }
            ).ToList();

            return View(data);
        }

        public ActionResult DanhSachDe()
        {
            var model = db.MTCV_DeKiemTra
            .Select(d => new DeKiemTraModel
            {
                ID = d.ID,
                TenDe = d.TenDe,
                NgayTao = d.NgayTao,
                NgayDenHan = d.NgayDenHan,
                LaDeChinhThuc = (bool)d.LaDeChinhThuc
            })
            .OrderByDescending(x => x.NgayTao)
            .ToList();

            return View(model);
        }

        public ActionResult ChiTietDe(int id)
        {
            var de = db.MTCV_DeKiemTra.FirstOrDefault(x => x.ID == id);
            if (de == null)
            {
                return HttpNotFound();
            }

            var model = new DeKiemTraModel
            {
                ID = de.ID,
                TenDe = de.TenDe,
                NgayTao = de.NgayTao,
                LaDeChinhThuc = (bool)de.LaDeChinhThuc,
                NgayDenHan = de.NgayDenHan,
                CauHoiList = db.MTCV_CauHoi
                    .Where(q => q.DeKiemTraID == de.ID)
                    .Select(q => new CauHoiModel
                    {
                        CauHoiID = q.ID,
                        NoiDung = q.NoiDung,
                        LoaiCauHoi = q.LoaiCauHoi,
                        LuaChonList = db.MTCV_TracNghiem
                            .Where(c => c.CauHoiID == q.ID)
                            .Select(c => new LuaChonModel
                            {
                                LuaChonID = c.ID,
                                NoiDung = c.NoiDung,
                                LaDapAnDung = (bool)c.LaDapAnDung
                            }).ToList()
                    }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult DatLamDeChinhThuc(int id)
        {
            var de = db.MTCV_DeKiemTra.FirstOrDefault(x => x.ID == id);
            if (de == null)
            {
                return HttpNotFound();
            }

            var all = db.MTCV_DeKiemTra.ToList();
            foreach (var d in all)
            {
                d.LaDeChinhThuc = false;
            }

            de.LaDeChinhThuc = true;
            db.SaveChanges();

            return RedirectToAction("DanhSachDe");
        }

        [HttpPost]
        public ActionResult LuuKetQuaChamDiem(List<ChamDiemSaveModel> data)
        {
            if (data == null || data.Count == 0)
            {
                return Json(new { success = false, message = "Không có dữ liệu gửi lên." });
            }

            try
            {
                foreach (var group in data.GroupBy(x => x.NhanVienID))
                {
                    int nhanVienID = group.Key;

                    var baiLam = (
                        from bl in db.MTCV_BaiLam
                        join dk in db.MTCV_DeKiemTra on bl.DeKiemTraID equals dk.ID
                        where bl.NhanVienID == nhanVienID
                              && dk.LaDeChinhThuc == true
                        select bl
                    ).FirstOrDefault();

                    if (baiLam == null)
                    {
                        continue;
                    }

                    var chiTietList = db.MTCV_BaiLamChiTiet
                                        .Where(ct => ct.BaiLamID == baiLam.ID)
                                        .ToList();

                    decimal tongDiem = 0;

                    foreach (var ct in chiTietList.Where(x => x.LaTracNghiem == true))
                    {
                        ct.Diem = (ct.DungSai == true) ? 1 : 0;
                        tongDiem += ct.Diem ?? 0;
                    }

                    var tuLuanList = chiTietList.Where(x => x.LaTracNghiem == false).ToList();

                    int index = 0;
                    foreach (var item in group)
                    {
                        if (index < tuLuanList.Count)
                        {
                            var ctTuLuan = tuLuanList[index];
                            ctTuLuan.Diem = item.Diem;
                            tongDiem += item.Diem;
                        }
                        index++;
                    }

                    baiLam.TongDiem = tongDiem;
                }

                db.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult CapNhatNgayDenHan(int id, DateTime ngayDenHan)
        {
            var de = db.MTCV_DeKiemTra.Find(id);
            if (de == null)
            {
                return Json(new { success = false, message = "Không tìm thấy đề kiểm tra." });
            }

            de.NgayDenHan = ngayDenHan;
            db.SaveChanges();

            return Json(new { success = true, message = "Cập nhật ngày đến hạn thành công." });
        }

        public ActionResult KetQuaLamBai()
        {
            int userId = MyAuthentication.ID;

            ViewBag.CoDuLieuLamBai = true;

            var deChinhThuc = db.MTCV_DeKiemTra.FirstOrDefault(x => x.LaDeChinhThuc == true);
            if (deChinhThuc == null)
            {
                ViewBag.CoDuLieuLamBai = false;
                return View();
            }

            var baiLam = db.MTCV_BaiLam.FirstOrDefault(x => x.NhanVienID == userId && x.DeKiemTraID == deChinhThuc.ID);
            if (baiLam == null)
            {
                ViewBag.CoDuLieuLamBai = false;
                return View();
            }

            var chiTiet = (
                from ct in db.MTCV_BaiLamChiTiet
                join bl in db.MTCV_BaiLam on ct.BaiLamID equals bl.ID
                join dk in db.MTCV_DeKiemTra on bl.DeKiemTraID equals dk.ID
                join q in db.MTCV_CauHoi on ct.CauHoiID equals q.ID
                where dk.LaDeChinhThuc == true && bl.NhanVienID == userId
                select new KetQuaChiTietModel
                {
                    CauHoi = q.NoiDung,

                    CauTraLoi = (ct.LaTracNghiem == true)
                        ? (from tn in db.MTCV_TracNghiem
                           where tn.ID.ToString() == ct.CauTraLoi
                           select tn.NoiDung).FirstOrDefault()
                        : ct.CauTraLoi,

                    LaTracNghiem = (bool)ct.LaTracNghiem,
                    DungSai = ct.DungSai ?? false,
                    Diem = ct.Diem
                }
            )
            .ToList();

            if (!chiTiet.Any())
            {
                ViewBag.CoDuLieuLamBai = false;
                return View();
            }

            var model = new KetQuaBaiLamViewModel
            {
                TenDe = deChinhThuc.TenDe,
                NgayLam = baiLam.NgayLam ?? DateTime.Now,
                TongDiem = baiLam.TongDiem ?? 0,
                ChiTietList = chiTiet
            };

            return View(model);
        }
    }
}