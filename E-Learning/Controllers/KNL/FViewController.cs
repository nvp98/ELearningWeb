using E_Learning.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_Learning.Controllers.KNL
{
    public class FViewController : Controller
    {
        ELEARNINGEntities db = new ELEARNINGEntities();
        // GET: FView
        public ActionResult Index(int? page,int? IDVT)
        {
            //var res = new List<FCheckValidation>();
            var res = (from a in db.NhanViens.Where(x => x.IDTinhTrangLV ==1)
                        join b in db.ViTriKNLs.Where(x=>x.IDVT == IDVT) on a.IDVTKNL equals b.IDVT
                        join d in db.PhongBans
                        on b.IDPB equals d.IDPhongBan
                        join e in db.KNL_PhanXuong
                         on b.IDPX equals e.ID into ul
                        from e in ul.DefaultIfEmpty()
                        join f in db.KNL_Nhom
                        on b.IDNhom equals f.IDNhom into uls
                        from f in uls.DefaultIfEmpty()
                        join g in db.KNL_To
                        on b.IDTo equals g.IDTo into ulk
                        from g in ulk.DefaultIfEmpty()
                        join h in db.Kips
                        on a.IDKip equals h.IDKip into ulkh
                        from h in ulkh.DefaultIfEmpty()
                        select new FViewValidation
                        {
                            MaNV = a.MaNV,
                            IDNV = a.ID,
                            IDVT = b.IDVT,
                            TenVT = b.TenViTri + "-" + f.TenNhom + "-" + g.TenTo + "-" + e.TenPX + "-" + d.MaPB,
                            TenNV = a.HoTen,
                            IDNhom = b.IDNhom,
                            IDPX = b.IDPX,
                            IDKip = a.IDKip,
                            TenKip = h.TenKip,
                            MaViTri = b.MaViTri,
                            fileBMTCV = b.FilePath,
                        }).ToList();
            foreach (var item in res)
            {
                //var KQ = db.KNL_KQ.Where(x => x.IDNV == item.IDNV).OrderByDescending(i => i.NgayDG).ToList();
                //var lastCheck = KQ.FirstOrDefault();
                var KQ = db.KNL_KQ_SelectNV(item.IDNV).ToList();
                var lastCheck = KQ.FirstOrDefault();
                int? IDVTT = 0;
                var KQDG = db.KNL_KQ_Select(item.IDNV, lastCheck?.ThangDG).Where(x => x.IDVT == item.IDVT).ToList();
                IDVTT = KQDG.LastOrDefault()?.IDVT;
                KQDG = KQDG.Where(x => x.IDVT == IDVTT).ToList();

                if (lastCheck != null  && KQDG.Count>0)
                {
                    item.NgayDG = KQ.FirstOrDefault()?.NgayDG != null ? String.Format("{0:dd/MM/yyyy}", KQ.FirstOrDefault()?.NgayDG) : "";
                    item.Total = lastCheck.ThangDG != null ? db.KNL_KQ_searchByIDNV(item.IDNV, lastCheck?.ThangDG, IDVTT).Count() - KQDG.Where(x => x.IsDanhGia == 0 && x.DiemDG == null).Count() : 0;
                    item.TotalDat = KQDG != null ? KQDG.Where(x => x.DiemDG == x.DinhMuc && x.DiemDG != null).Count() : 0;
                    item.TotalKDat = KQDG != null ? KQDG.Where(x => x.DiemDG < x.DinhMuc && x.DiemDG != null).Count() : 0;
                    item.TotalVuot = KQDG != null ? KQDG.Where(x => x.DiemDG > x.DinhMuc && x.DiemDG != null).Count() : 0;
                    item.TotalKDGia = KQDG != null ? KQDG.Where(x => x.IsDanhGia == 1 && x.DiemDG == null).Count() : 0;
                    item.ThangDG = lastCheck?.ThangDG;
                }
                
            }
            ViewBag.TenVT = res.FirstOrDefault()?.TenVT;
            //Session["ListUser"] = res;
            if (page == null) page = 1;
            int pageSize = res.Count() > 0 ? res.Count() : 50;
            int pageNumber = (page ?? 1);
            return View(res.ToList().ToPagedList(pageNumber, pageSize));
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