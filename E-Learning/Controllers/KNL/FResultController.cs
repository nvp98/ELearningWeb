using E_Learning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_Learning.Controllers.KNL
{
    public class FResultController : Controller
    {
        ELEARNINGEntities db = new ELEARNINGEntities();
        // GET: FResult
        public ActionResult Index(int? IDNV, DateTime begind, DateTime endd)
        {
            var nv = db.NhanViens.Where(x => x.ID == IDNV).FirstOrDefault();
            var vt = db.VitriKNL_searchByIDVT(nv.IDVTKNL).FirstOrDefault();
            ViewBag.IDVT = nv.IDVTKNL;
            ViewBag.HoTen = nv.MaNV +"-"+ nv.HoTen;
            ViewBag.TenVT = vt?.TenViTri;
            ViewBag.BMTCV = vt?.FilePath;
            List<FResultValidation> KQua = new List<FResultValidation>();
            foreach(DateTime day in EachMont(begind, endd))
            {
                var firstDayOfMonth = new DateTime(day.Year, day.Month, 1);
                //var kqThang = db.KNL_KQ_Select(IDNV, firstDayOfMonth).ToList();
                var kqThang = db.KNL_LSDG_Select(IDNV, firstDayOfMonth).ToList();

                //var knl = new List<FValueValidation>();
                int? IDVTT = 0;
                //if (kqThang.Count > 0)
                //{
                //    knl = (from k in kqThang
                //              select new FValueValidation
                //              {
                //                  DinhMuc = k.DinhMuc,
                //                  DiemDG = k.DiemDG,
                //                  IDNL = k.IDNL,
                //                  ColorKQ = k.DinhMuc < k.DiemDG && k.DiemDG != null ? "VUOT" : k.DinhMuc == k.DiemDG ? "DAT" : "KHONG",
                //                  IDVT =k.IDVT,
                //                  TenViTri = db.VitriKNL_searchByIDVT(k.IDVT).FirstOrDefault()?.TenViTri,
                //                  Note = db.VitriKNL_searchByIDVT(k.IDVT).FirstOrDefault()?.FilePath,
                //                  IsDanhGia = k.IsDanhGia,
                //              }).ToList();
                //    IDVTT = knl.LastOrDefault()?.IDVT;
                //    knl = knl.Where(x=>x.IDVT == IDVTT).ToList();
                //}
                //FResultValidation a = new FResultValidation()
                //{
                //    IDNV = IDNV,
                //    MaNV = nv.MaNV,
                //    HoTen = nv.MaNV + "-" + nv.HoTen,
                //    DGThang = day.Month.ToString() + "/" + day.Year.ToString(),
                //    DGThangDate = firstDayOfMonth,
                //    Total = knl.Where(x=>x.IsDanhGia ==1).Count(),
                //    TotalDat = knl.Where(x=> x.DiemDG == x.DinhMuc && x.DiemDG != null).Count(),
                //    TotalVuot =knl.Where(x=> x.DiemDG > x.DinhMuc && x.DiemDG != null).Count(),
                //    TotalKDat =knl.Where(x => x.DiemDG < x.DinhMuc && x.DiemDG != null).Count(),
                //    TotalKDGia =knl.Where(x=> x.IsDanhGia == 1 && x.DiemDG == null).Count(),
                //    IDVT = knl.LastOrDefault()?.IDVT,
                //    FilePath = knl.FirstOrDefault()?.Note,
                //    TenViTri = knl.LastOrDefault()?.TenViTri,
                //};
                FResultValidation a = new FResultValidation()
                {
                    IDNV = IDNV,
                    MaNV = nv.MaNV,
                    HoTen = nv.MaNV + "-" + nv.HoTen,
                    DGThang = day.Month.ToString() + "/" + day.Year.ToString(),
                    DGThangDate = firstDayOfMonth,
                    Total = 0,
                    TotalDat = 0,
                    TotalVuot = 0,
                    TotalKDat = 0, 
                    TotalKDGia = 0,
                    TotalChuaDGia = 0,
                    TotalDatTu = 0,
                    TotalVuotTu = 0,
                    TotalKDatTu = 0,
                    TotalKDGiaTu = 0,
                    TotalChuaDGiaTu = 0,
                    TotalDatTuLan1 = 0,
                    TotalVuotTuLan1 = 0,
                    TotalKDatTuLan1 = 0,
                    TotalKDGiaTuLan1 = 0,
                    TotalChuaDGiaTuLan1 = 0,
                    IDVT = 0,
                    //FilePath = kqThang?.FirstOrDefault().FilePath,
                    //TenViTri = kqThang?.FirstOrDefault().TenViTri,
                };
                if(kqThang.Count > 0)
                {
                    a.Total = kqThang?.FirstOrDefault().TONGNL;
                    a.TotalDat = kqThang?.FirstOrDefault().DAT;
                    a.TotalVuot = kqThang?.FirstOrDefault().VUOT;
                    a.TotalKDat = kqThang?.FirstOrDefault().KDAT;
                    a.TotalKDGia = kqThang?.FirstOrDefault().KDGia;
                    a.TotalChuaDGia = kqThang?.FirstOrDefault().CHUADG;
                    a.IDVT = kqThang?.FirstOrDefault().VTID;
                    a.FilePath = kqThang?.FirstOrDefault().FilePath;
                    a.TenViTri = kqThang?.FirstOrDefault().TenViTri;
                    a.TotalDatTu = kqThang?.FirstOrDefault().DATTUDG;
                    a.TotalVuotTu = kqThang?.FirstOrDefault().VUOTTUDG;
                    a.TotalKDatTu = kqThang?.FirstOrDefault().KDATTUDG;
                    a.TotalKDGiaTu = kqThang?.FirstOrDefault().KDGiaTuDG;
                    a.TotalChuaDGiaTu = kqThang?.FirstOrDefault().CHUADGTuDG;

                    a.TotalDatTuLan1 = kqThang?.FirstOrDefault().DATTUDGLan1;
                    a.TotalVuotTuLan1 = kqThang?.FirstOrDefault().VUOTTUDGLan1;
                    a.TotalKDatTuLan1 = kqThang?.FirstOrDefault().KDATTUDGLan1;
                    a.TotalKDGiaTuLan1 = kqThang?.FirstOrDefault().KDGiaTuDGLan1;
                    a.TotalChuaDGiaTuLan1 = kqThang?.FirstOrDefault().CHUADGTuDGLan1;

                }
                KQua.Add(a);
            }
            return View(KQua);
        }

        public ActionResult FView(int? IDNV, DateTime? dt,int? IDVT)
        {
            var nv = (from a in db.NhanViens.Where(x => x.ID == IDNV)
                      join b in db.ViTriKNLs on a.IDVTKNL equals b.IDVT
                      select new FCheckValidation
                      {
                          TenNV = a.HoTen,
                          TenVT = b.TenViTri,
                          IDVT = b.IDVT,
                          IDNV = a.ID,
                          IDPB = a.IDPhongBan,
                      }).FirstOrDefault();
            ViewBag.TenNV = nv.TenNV ?? "";
            ViewBag.TenVT = nv.TenVT ?? "";
            ViewBag.ThangDG = (DateTime?)dt ?? default(DateTime);

            var res = (from a in db.KNL_KQ_searchByIDNV(IDNV, dt, IDVT).Where(x=>x.ThangDG == dt && x.IDNV != x.IDNVDG)
                       select new FValueValidation
                       {
                           IDNV = (int?)nv.IDNV ?? null,
                           TenNV = nv.TenNV ?? "",
                           IDNL = a.IDNL,
                           TenNL = a.TenNL,
                           IDLoaiNL = a.IDLoaiNL,
                           TenLoaiNL = a.TenLoai,
                           IDVT = a.IDVT,
                           TenViTri = a.TenViTri,
                           IDPB = a.IDPB,
                           TenPhongBan = a.TenPhongBan,
                           DinhMuc = a.IsDanhGia != 0 ? a.DinhMuc : 0,
                           IsDanhGia = a.IsDanhGia,
                           DiemDG = a.DiemDG,
                           IDKQ = (int?)a.IDKQ ?? null,
                           Note = a.Note,
                           ThangDG = (DateTime?)dt ?? default(DateTime),
                           NgayDG = (DateTime?)a.NgayDG ?? default(DateTime),
                           StrNgayDG = a.NgayDG != null ? a.NgayDG.Value.ToString("dd/MM/yyyy") : "",
                           OrderBy = a.OrderBy,
                           OrderByLoai = a.orByLoai,
                           ColorKQ = a.DiemDG < a.DinhMuc ? "bg-danger" : "bg-success",
                           IDNVDG = a.IDNVDG,
                           TenNVDG = a.HoTen
                       }).ToList().OrderBy(x => x.OrderBy);

            List<LoaiKNL> loaiNL = db.LoaiKNLs.Where(x => x.IDVT == IDVT && x.IDLoai != 1 && x.IDLoai != 2).OrderBy(x => x.OrderBy).ToList();
            ViewBag.IDLoaiNL = new SelectList(loaiNL, "IDLoai", "TenLoai");

            return View(res.ToList());
        }


        public static IEnumerable<DateTime> EachMont(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddMonths(1))
                yield return day;
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