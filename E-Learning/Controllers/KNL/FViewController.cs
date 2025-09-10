using DocumentFormat.OpenXml.Wordprocessing;
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
            int month = DateTime.Now.Month; // hoặc date.Month
            int quy = (month - 1) / 3 + 1;
            int nam = DateTime.Now.Year;
            var kqQuy = db.KNL_LSDG_TheoQuy(nam,quy, null).Where(x=>x.IDVTKNL == IDVT).ToList();
            var nhanvien = db.NhanViens.Where(x => x.IDTinhTrangLV == 1 && x.IDVTKNL == IDVT).ToList();
            //var res = new List<FCheckValidation>();
            var res = (from a in nhanvien
                        join kq in kqQuy on a.ID equals kq.NVID into ulkh
                        from kq in ulkh.DefaultIfEmpty()
                        select new FResultValidation
                        {
                            IDNV = a.ID,
                            MaNV = a.MaNV,
                            HoTen = a.MaNV + "-" + a.HoTen,
                            DGQuy = quy, // có thể để 0 nếu muốn mặc định khác
                            DGNam = nam,
                            Total = kq.TONGNL ?? 0,
                            TotalDat = kq.DAT ?? 0,
                            TotalVuot = kq.VUOT ?? 0,
                            TotalKDat = kq.KDAT ?? 0,
                            TotalKDGia = kq.KDGia ?? 0,
                            TotalChuaDGia = kq.CHUADG ?? 0,
                            TotalDatTu = kq.DATTUDG ?? 0,
                            TotalVuotTu = kq.VUOTTUDG ?? 0,
                            TotalKDatTu = kq.KDATTUDG ?? 0,
                            TotalKDGiaTu = kq.KDGiaTuDG ?? 0,
                            TotalChuaDGiaTu = kq.CHUADGTuDG ?? 0,
                            TotalDatTuLan1 = kq.DATTUDGLan1 ?? 0,
                            TotalVuotTuLan1 = kq.VUOTTUDGLan1 ?? 0,
                            TotalKDatTuLan1 = kq.KDATTUDGLan1 ?? 0,
                            TotalKDGiaTuLan1 = kq.KDGiaTuDGLan1 ?? 0,
                            TotalChuaDGiaTuLan1 = kq.CHUADGTuDGLan1 ?? 0,
                            IDVT = kq.VTID ?? 0,
                            TenViTri = kq.TenViTri ?? "",
                            FilePath = kq.FilePath
                        }).ToList();

            //ViewBag.TenVT = res.FirstOrDefault()?.TenVT;
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