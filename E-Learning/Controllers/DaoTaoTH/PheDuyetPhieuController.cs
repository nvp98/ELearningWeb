using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using E_Learning.Models;
using E_Learning.ModelsDTTH;
using PagedList;

namespace E_Learning.Controllers.DaoTaoTH
{
    public class PheDuyetPhieuController : Controller
    {
        private ELEARNINGEntities db = new ELEARNINGEntities();
        int Idquyen = MyAuthentication.IDQuyen;
        String ControllerName = "PheDuyetPhieu";

        // GET: PheDuyetPhieu
        public ActionResult Index(int? LoaiKyDuyet)
        {
            return View(db.SH_KyDuyetNCDT.ToList());
        }
        public ActionResult Index_NCDT(int? page)
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            ViewBag.QUYENCN = ListQuyen;
            //if (!ListQuyen.Contains(CONSTKEY.V))
            //{
            //    TempData["msgError"] = "<script>alert('Bạn không có quyền truy cập chức năng này');</script>";
            //    return RedirectToAction("", "Home");
            //}
            var valuesToCheck = db.SH_NhuCauDT.Where(x=>x.TinhTrang != 0).Select(k => k.ID).ToList();
            var res = (from a in db.SH_KyDuyetNCDT.Where(x => x.NguoiDuyet_ID == MyAuthentication.ID && valuesToCheck.Contains((int)x.NCDT_ID))
                       select new SH_KyDuyetNCDTView
                       {
                           ID = a.ID,
                           CapDuyet = a.CapDuyet,
                           NguoiDuyet_ID = a.NguoiDuyet_ID,
                           GhiChu = a.GhiChu,
                           NCDT_ID = a.NCDT_ID,
                           NgayDuyet = a.NgayDuyet,
                           TinhTrangDuyet = a.TinhTrangDuyet,
                       }).ToList();
            foreach (var item in res)
            {
                var kyduyet = db.SH_KyDuyetNCDT.Where(x => x.NCDT_ID == item.NCDT_ID).ToList();
                if (kyduyet.Count() > 1) // > 1 cấp duyệt 
                {
                    var checkky = kyduyet.Where(x => x.CapDuyet < item.CapDuyet).ToList(); // check các cấp duyệt nhỏ hơn
                    if (checkky.Count() != 0 )
                    {
                        if(checkky.Where(x=>x.TinhTrangDuyet == 0 || x.TinhTrangDuyet == 2).Count() != 0) // nếu chưa duyệt hoặc hủy thì ẩn
                        {
                            res = res.Where(x=>x.ID != item.ID).ToList();
                        }

                    }
                }
                   
               
            }
            if (page == null) page = 1;
            int pageSize = 100;
            int pageNumber = (page ?? 1);
            return View(res.ToList().ToPagedList(pageNumber, pageSize));
        }

        public ActionResult XuLyPhieuNCDT(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SH_NhuCauDT nhuCauDT = db.SH_NhuCauDT.Find(id);
            if (nhuCauDT == null)
            {
                return HttpNotFound();
            }
            var IDPB = MyAuthentication.IDPhongban;
            ViewBag.IDPhanLoaiDT = new SelectList(db.SH_PhanLoaiNDDT, "ID", "TenPhanLoaiDT", nhuCauDT.PhanLoaiNCDT_ID);
            ViewBag.IDPhongBan = db.PhongBans.Where(x => x.IDPhongBan == IDPB).FirstOrDefault().TenPhongBan;
            //ViewBag.IDNoiDungDT = new SelectList(db.NoiDungDTs.Where(x=>x.IDND == nhuCauDT.NoiDungDT_ID), "IDND", "NoiDung",nhuCauDT.NoiDungDT_ID);
            ViewBag.IDNoiDungDT = new SelectList(db.NoiDungDTs.Where(x => x.IDND == nhuCauDT.NoiDungDT_ID).Select(x => new
            {
                Value = x.IDND,
                Text = x.IDND + " - " + db.SH_PhanLoaiNDDT.Where(a => x.IDND == nhuCauDT.NoiDungDT_ID).FirstOrDefault().TenPhanLoaiDT + " - " + x.NoiDung // Concatenate fields here
            }),
            "Value",
            "Text", nhuCauDT.NoiDungDT_ID);
            ViewBag.IDDKNhacLai = new SelectList(db.SH_DinhKy, "MaDK", "DKNhacLai", nhuCauDT.MaDinhKy);

            ViewBag.LoaiNCDT = nhuCauDT.PhanLoaiNCDT_ID;
            ViewBag.LoaiHinh_DT = db.SH_PhanLoaiNCDT.Where(x => x.IDLoai == nhuCauDT.PhanLoaiNCDT_ID).FirstOrDefault().LoaiHinhDT_ID;
            ViewBag.Nam = db.SH_QuyDaoTao.First().AD_Nam;
            ViewBag.Quy = db.SH_QuyDaoTao.First().AD_Quy;
            ViewBag.PhuongPhapDT_ID = new SelectList(db.SH_PhuongPhapDT.Where(x => x.ID == nhuCauDT.PhuongPhapDT_ID), "ID", "TenPhuongPhapDT", nhuCauDT.PhuongPhapDT_ID);
            var nv2 = db.NhanViens.Where(x => x.IDTinhTrangLV == 1 && x.IDPhongBan == IDPB).Select(x => new EmployeeValidation { ID = x.ID, HoTen = x.MaNV + " - " + x.HoTen + "-" + x.Vitri.TenViTri }).ToList();
            ViewBag.Selec = new SelectList(nv2, "ID", "HoTen", nhuCauDT.SH_ChiTiet_NCDT.FirstOrDefault().GiangVien_ID);

            var ls = new List<SH_ViTri_NDDTView>();

            var data = db.VitriKNL_search();
            var vt = db.SH_ViTri_NDDT.Where(x => x.NCDT_ID == null || x.NCDT_ID == nhuCauDT.ID).ToList();
            ls = (from a in vt.Where(x => x.NoiDungDT_ID == nhuCauDT.NoiDungDT_ID && x.PhuongPhapDT_ID == nhuCauDT.PhuongPhapDT_ID)
                  join b in data on a.Vitri_ID equals b.IDVT
                  select new SH_ViTri_NDDTView()
                  {
                      ID = a.ID,
                      TenViTri = b.IDVT + "-" + b.TenViTri + "-" + b.MaPB + "-" + b.TenPX + "-" + b.TenNhom + "-" + b.TenTo,
                      Tinhtrang = a.NCDT_ID == nhuCauDT.ID ? 1 : 0
                  }).ToList();
            ViewBag.DSVitri = ls;

            // trình ký
            var trinhky = db.SH_KyDuyetNCDT.Where(x => x.NCDT_ID == nhuCauDT.ID).ToList();
            var nhanvien = db.NhanViens.Where(x => x.IDTinhTrangLV == 1).Select(x => new EmployeeValidation { ID = x.ID, HoTen = x.MaNV + " - " + x.HoTen, IDPhongBan = (int)x.IDPhongBan }).ToList();

            ViewBag.BPSD = new SelectList(nhanvien.Where(x => x.IDPhongBan == MyAuthentication.IDPhongban), "ID", "HoTen", trinhky.Where(x => x.CapDuyet == 1).FirstOrDefault()?.NguoiDuyet_ID);
            ViewBag.PCHN = new SelectList(nhanvien, "ID", "HoTen", trinhky.Where(x => x.CapDuyet == 2).FirstOrDefault()?.NguoiDuyet_ID);
            var phongns = db.PhongBans.Where(x => x.MaPB.Contains("P.NS")).First();
            var bangiamdoc = db.PhongBans.Where(x => x.MaPB.Contains("BGD")).First();
            ViewBag.PNS = new SelectList(nhanvien.Where(x => x.IDPhongBan == phongns.IDPhongBan), "ID", "HoTen", trinhky.Where(x => x.CapDuyet == 3).FirstOrDefault()?.NguoiDuyet_ID);
            ViewBag.BGD = new SelectList(nhanvien.Where(x => x.IDPhongBan == bangiamdoc.IDPhongBan), "ID", "HoTen", trinhky.Where(x => x.CapDuyet == 4).FirstOrDefault()?.NguoiDuyet_ID);

            var noiDungDTs = (from a in db.SH_NhuCauDT.Where(x => x.ID == id)
                              join b in db.SH_ChiTiet_NCDT on a.ID equals b.NhuCauDT_ID
                              join c in db.PhongBans on a.BoPhanLNC_ID equals c.IDPhongBan into uli
                              from c in uli.DefaultIfEmpty()
                              join d in db.NhanViens.Where(x => x.IDTinhTrangLV == 1) on a.NguoiTao_ID equals d.ID
                              select new NhuCauDTTHView
                              {
                                  ID_NCDT = a.ID,
                                  NoiDungDT_ID = a.NoiDungDT_ID,
                                  TenNoiDungDT = a.NoiDungDT.NoiDung,
                                  Quy = a.Quy,
                                  Nam = a.Nam,
                                  BoPhanLNC_ID = a.BoPhanLNC_ID,
                                  TenBoPhan_LNC = c.TenPhongBan,
                                  NguoiTao = d.MaNV + "-" + d.HoTen,
                                  TinhTrang = a.TinhTrang,
                                  FileDinhKem = a.FileDinhKem,
                                  TrinhKy_ID = a.MaTrinhKy,
                                  PhuongPhapDT_ID = a.PhuongPhapDT_ID,
                                  NgayTao = (DateTime)a.NgayTao,
                                  chiTietNhuCauDTTHView = new ChiTietNhuCauDTTHView
                                  {
                                      SoLuongNguoi = b.SoLuongNguoi,
                                      DonViDT = b.DonViDT,
                                      DiaDiemDT = b.DiaDiemDT,
                                      ThoiGianDT = b.ThoiGian_DT,
                                      GhiChu = b.GhiChu,
                                      ThoiLuong = b.ThoiLuong_DT,
                                      DoiTuongDT = b.DoiTuongDT,
                                      TenViTri = b.GiangVien_Vitri,
                                      TenGiangVien = b.GiangVien_HoTen,
                                      Nhom_ID = a.NoiDungDT.IDNhomNL,
                                      GiangVien_ID = b.GiangVien_ID,
                                  },
                              }).OrderBy(x => x.ID_NCDT).FirstOrDefault();

            return View(noiDungDTs);
        }

        public ActionResult XacNhanPhieu(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            var sH_KyDuyetNCDT = db.SH_KyDuyetNCDT.Where(x=>x.NCDT_ID == id && x.NguoiDuyet_ID == MyAuthentication.ID).ToList();
            if (sH_KyDuyetNCDT == null)
            {
                return HttpNotFound();
            }
            if(sH_KyDuyetNCDT.Count() > 1)
            {
                foreach (var item in sH_KyDuyetNCDT)
                {
                    item.NgayDuyet = DateTime.Now;
                    item.TinhTrangDuyet = 1;
                }
                db.SaveChanges();
            }
            else
            {
                var aa = sH_KyDuyetNCDT.FirstOrDefault();
                aa.NgayDuyet = DateTime.Now;
                aa.TinhTrangDuyet = 1;
                db.SaveChanges();
            }
            // kiểm tra và update SH_NCDT
            var checkDuyet = db.SH_KyDuyetNCDT.Where(x => x.NCDT_ID == id && x.TinhTrangDuyet != 1).ToList();
            if(checkDuyet.Count == 0)
            {
                var ncdt = db.SH_NhuCauDT.Where(x=>x.ID == id).FirstOrDefault();
                ncdt.TinhTrang = 1;
                db.SaveChanges();
            }

            TempData["msgSuccess"] = "<script>alert('Xác nhận thành công ');</script>";
            return RedirectToAction("Index_NCDT", "PheDuyetPhieu");
        }

        public ActionResult KhongXacNhanPhieu(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var sH_KyDuyetNCDT = db.SH_KyDuyetNCDT.Where(x => x.NCDT_ID == id && x.NguoiDuyet_ID == MyAuthentication.ID).FirstOrDefault();
            if (sH_KyDuyetNCDT == null)
            {
                return HttpNotFound();
            }
            sH_KyDuyetNCDT.NgayDuyet = DateTime.Now;
            sH_KyDuyetNCDT.TinhTrangDuyet = 2; // hủy
            db.SaveChanges();
            // kiểm tra và update SH_NCDT về hủy
            var ncdt = db.SH_NhuCauDT.Where(x => x.ID == id).FirstOrDefault();
            if (ncdt != null)
            {
                ncdt.TinhTrang = 3; // hủy NCĐT
                db.SaveChanges();
            }
            TempData["msgSuccess"] = "<script>alert('Xác nhận thành công ');</script>";
            return RedirectToAction("Index_NCDT", "PheDuyetPhieu");
        }

        // GET: PheDuyetPhieu/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SH_KyDuyetNCDT sH_KyDuyetNCDT = db.SH_KyDuyetNCDT.Find(id);
            if (sH_KyDuyetNCDT == null)
            {
                return HttpNotFound();
            }
            return View(sH_KyDuyetNCDT);
        }

        // GET: PheDuyetPhieu/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PheDuyetPhieu/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,NCDT_ID,NguoiDuyet_ID,CapDuyet,TinhTrangDuyet,NgayDuyet")] SH_KyDuyetNCDT sH_KyDuyetNCDT)
        {
            if (ModelState.IsValid)
            {
                db.SH_KyDuyetNCDT.Add(sH_KyDuyetNCDT);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(sH_KyDuyetNCDT);
        }

        // GET: PheDuyetPhieu/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SH_KyDuyetNCDT sH_KyDuyetNCDT = db.SH_KyDuyetNCDT.Find(id);
            if (sH_KyDuyetNCDT == null)
            {
                return HttpNotFound();
            }
            return View(sH_KyDuyetNCDT);
        }

        // POST: PheDuyetPhieu/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,NCDT_ID,NguoiDuyet_ID,CapDuyet,TinhTrangDuyet,NgayDuyet")] SH_KyDuyetNCDT sH_KyDuyetNCDT)
        {
            if (ModelState.IsValid)
            {
                //db.Entry(sH_KyDuyetNCDT).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(sH_KyDuyetNCDT);
        }

        // GET: PheDuyetPhieu/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SH_KyDuyetNCDT sH_KyDuyetNCDT = db.SH_KyDuyetNCDT.Find(id);
            if (sH_KyDuyetNCDT == null)
            {
                return HttpNotFound();
            }
            return View(sH_KyDuyetNCDT);
        }

        // POST: PheDuyetPhieu/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SH_KyDuyetNCDT sH_KyDuyetNCDT = db.SH_KyDuyetNCDT.Find(id);
            db.SH_KyDuyetNCDT.Remove(sH_KyDuyetNCDT);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
