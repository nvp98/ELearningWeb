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
        public ActionResult Index(int? IDNV, int? Nam)
        {
            var nv = db.NhanViens.Where(x => x.ID == IDNV).FirstOrDefault();
            var vt = db.VitriKNL_searchByIDVT(nv.IDVTKNL).FirstOrDefault();
            ViewBag.IDVT = nv.IDVTKNL;
            ViewBag.HoTen = nv.MaNV +"-"+ nv.HoTen;
            ViewBag.TenVT = vt?.TenViTri;
            ViewBag.BMTCV = vt?.FilePath;

            if (Nam == null) Nam = DateTime.Now.Year;
            var kqQuy = db.KNL_LSDG_TheoQuy(Nam,null, IDNV).ToList();

            List<FResultValidation> KQua = new List<FResultValidation>();
            for(var quy=1; quy<=4;quy++)
            {
                var item = kqQuy.Where(x=>x.Quy == quy).FirstOrDefault();
                var a = new FResultValidation
                {
                    IDNV = IDNV,
                    MaNV = nv.MaNV,
                    HoTen = nv.MaNV + "-" + nv.HoTen,
                    DGQuy = quy, // có thể để 0 nếu muốn mặc định khác
                    DGNam = Nam,
                    Total = item?.TONGNL ?? 0,
                    TotalDat = item?.DAT ?? 0,
                    TotalVuot = item?.VUOT ?? 0,
                    TotalKDat = item?.KDAT ?? 0,
                    TotalKDGia = item?.KDGia ?? 0,
                    TotalChuaDGia = item?.CHUADG ?? 0,
                    TotalDatTu = item?.DATTUDG ?? 0,
                    TotalVuotTu = item?.VUOTTUDG ?? 0,
                    TotalKDatTu = item?.KDATTUDG ?? 0,
                    TotalKDGiaTu = item?.KDGiaTuDG ?? 0,
                    TotalChuaDGiaTu = item?.CHUADGTuDG ?? 0,
                    TotalDatTuLan1 = item?.DATTUDGLan1 ?? 0,
                    TotalVuotTuLan1 = item?.VUOTTUDGLan1 ?? 0,
                    TotalKDatTuLan1 = item?.KDATTUDGLan1 ?? 0,
                    TotalKDGiaTuLan1 = item?.KDGiaTuDGLan1 ?? 0,
                    TotalChuaDGiaTuLan1 = item?.CHUADGTuDGLan1 ?? 0,
                    IDVT = item?.VTID ?? 0,
                    TenViTri = item?.TenViTri ?? "",
                    FilePath = item?.FilePath
                };

                KQua.Add(a);
            }
            return View(KQua);
        }

        public ActionResult FView(int? IDNV, int? Quy, int? Nam,int? IDVT)
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
            ViewBag.QuyDG = Quy +"/"+ Nam;

            var res = (from a in db.KNL_KQ_TheoQuy(Nam, Quy, IDNV).Where(x=>x.VTID == IDVT)
                       select new FValueValidation
                       {
                           IDNV = (int?)nv.IDNV ?? null,
                           TenNV = nv.TenNV ?? "",
                           IDNL = a.IDNL,
                           TenNL = a.TenNL,
                           IDLoaiNL = a.IDLoaiNL,
                           //TenLoaiNL = a.TenLoai,
                           IDVT = a.VTID,
                           TenViTri = a.TenViTri,
                           //IDPB = a.IDPB,
                           //TenPhongBan = a.TenPhongBan,
                           DinhMuc = a.IsDanhGia != 0 ? a.DiemDM : 0,
                           IsDanhGia = a.IsDanhGia,
                           DiemDG = a.DiemDG,
                           IDKQ = (int?)a.IDKQ ?? null,
                           Note = a.Note,
                           //ThangDG = (DateTime?)dt ?? default(DateTime),
                           NgayDG = (DateTime?)a.NgayDG ?? default(DateTime),
                           StrNgayDG = a.NgayDG != null ? a.NgayDG.Value.ToString("dd/MM/yyyy") : "",
                           StrNgayTuDG = a.NgayTuDG != null ? a.NgayTuDG.Value.ToString("dd/MM/yyyy") : "",
                           StrNgayDGLan1 = a.NgayDG_Lan1 != null ? a.NgayDG_Lan1.Value.ToString("dd/MM/yyyy") : "",
                           OrderBy = a.OrderBy,
                           //OrderByLoai = a.orByLoai,
                           ColorKQ = a.DiemDG < a.DiemDM ? "bg-danger" : "bg-success",
                           IDNVDG = a.IDNVDG,
                           TenNVDG = a.TenNguoiDanhGia
                       }).ToList().OrderBy(x => x.OrderBy);

            var distinctIDLoaiNLs = res.Where(x=>x.IDLoaiNL != 1 && x.IDLoaiNL != 2)
                    .Select(x => x.IDLoaiNL)
                    .Distinct()
                    .ToList();

            List<LoaiKNL> loaiNL = db.LoaiKNLs.Where(x => distinctIDLoaiNLs.Contains(x.IDLoai)).OrderBy(x => x.OrderBy).ToList();
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