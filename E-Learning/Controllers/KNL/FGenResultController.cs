using E_Learning.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_Learning.Controllers.KNL
{
    public class FGenResultController : Controller
    {
        ELEARNINGEntities db = new ELEARNINGEntities();
        int Idquyen = MyAuthentication.IDQuyen;
        String ControllerName = "FGenResult";
        // GET: FGenResult
        public ActionResult Index(int? page, int? IDPX, int? IDPB,int? IDNhom,int? IDTo,DateTime? dat)
        {
            if (page == null) page = 1;
            int pageSize = 20;
            int pageNumber = (page ?? 1);
            
            var firstDayOfMonth = (DateTime)dat;
            ViewBag.Month = firstDayOfMonth.ToString("yyyy-MM");
            List<PhongBan> dt = db.PhongBans.ToList();
            //if (ListQuyen.Contains(CONSTKEY.V_BP)) dt = dt.Where(x => x.IDPhongBan == idpb).ToList();
            ViewBag.IDPB = new SelectList(dt, "IDPhongBan", "TenPhongBan");

            var res = (from a in db.VitriKNL_search()
                       select new FGenResultValidation()
                       {
                           IDVT = a.IDVT,
                           TenViTri = a.TenViTri+"-"+a.TenTo +"-" +a.TenNhom +"-"+ a.TenPX,
                           TenPhongBan =a.TenPhongBan,
                           FilePath =a.FilePath,
                           IDPB =a.IDPB,
                           IDPX =a.IDPX,
                           IDNhom =a.IDNhom,
                           IDTo =a.IDTo
                       }).ToList();
            if (IDPB == null) IDPB = 0;
            if (IDPB != 0) res = res.Where(x => x.IDPB == IDPB).ToList();
            if (IDPX == null) IDPX = 0;
            if (IDPX != 0) res = res.Where(x => x.IDPX == IDPX).ToList();
            if (IDNhom == null) IDNhom = 0;
            if (IDNhom != 0) res = res.Where(x => x.IDNhom == IDNhom).ToList();
            if (IDTo == null) IDTo = 0;
            if (IDTo != 0) res = res.Where(x => x.IDTo == IDTo).ToList();


            for (var i=pageSize*((int)page-1) ;i< (page*pageSize>res.Count?res.Count: page*pageSize); i++)
            {
                    var kq = GetGenResult(res[i].IDVT, firstDayOfMonth).ToList();
                    res[i].SumNL = db.KhungNangLuc_SearchByIDVT(res[i].IDVT).Where(x => x.IsDanhGia == 1).Count();
                    res[i].Total = kq.Sum(x => x.Total);
                    if(res[i].Total > 0)
                    {
                        res[i].TileDat = Math.Round((Double)kq.Sum(x => x.TotalDat)*100/(Double)res[i].Total,2);
                        res[i].TileKDat = Math.Round((Double)kq.Sum(x => x.TotalKDat) * 100 / (Double)res[i].Total,2);
                        res[i].TileVuot = Math.Round((Double)kq.Sum(x => x.TotalVuot) * 100 / (Double)res[i].Total,2);
                        res[i].TileKDGia = 100- (res[i].TileDat+res[i].TileKDat+res[i].TileVuot);
                    }
            }
            

            return View(res.ToList().ToPagedList(pageNumber, pageSize));
        }
        public ActionResult FGenResultDetails(int? IDVT, DateTime dt)
        {
            var vt = db.VitriKNL_searchByIDVT(IDVT).FirstOrDefault();
            ViewBag.TenViTri = vt?.TenViTri ?? "";
            ViewBag.TenPB = vt?.TenPhongBan ?? "";
            ViewBag.ThangDG = (DateTime?)dt ?? default(DateTime);
            var res = GetGenResult(IDVT, dt);
            //var res = (from a in db.KhungNangLuc_SearchByIDVT(IDVT)
            //           select new FGenResultValidation
            //           {
            //               IDNL = a.IDNL,
            //               TenNL = a.TenNL,
            //               IDLoaiNL = a.IDLoaiNL,
            //               IDVT = a.IDVT,
            //               TenViTri = a.TenViTri,
            //               IDPB = a.IDPB,
            //               DinhMuc = a.IsDanhGia != 0 ? a.DinhMuc : 0,
            //               IsDanhGia = a.IsDanhGia,
            //           }).ToList().OrderBy(x => x.OrderBy);
            //foreach (var item in res)
            //{
            //    var kq = db.KNL_KQ_searchByIDNL(item.IDNL, dt).ToList();
            //    item.Total = kq.Count();
            //    item.TotalKDGia = kq.Where(x => x.DiemDG == null).Count();
            //    item.TotalDat = kq.Where(x => x.DiemDG == item.DinhMuc).Count();
            //    item.TotalVuot = kq.Where(x => x.DiemDG > item.DinhMuc).Count();
            //    item.TotalKDat = kq.Where(x => x.DiemDG < item.DinhMuc).Count();
            //    if(kq.Count() > 0)
            //    {
            //        item.TileDat = item.TotalDat *100 / item.Total;
            //        item.TileKDat = item.TotalKDat *100 / item.Total;
            //        item.TileKDGia = item.TotalKDGia*100 / item.Total;
            //        item.TileVuot = item.TotalVuot*100 / item.Total;
            //    }
            //}

            ViewBag.SumKDat = res.Where(x => x.TotalKDat != 0).Sum(x => x.TotalKDat);
            ViewBag.SumDat = res.Where(x => x.TotalDat != 0).Sum(x => x.TotalDat);
            ViewBag.SumVuot = res.Where(x => x.TotalVuot != 0).Sum(x => x.TotalVuot);
            ViewBag.SumKDGia = res.Where(x => x.TotalKDGia != 0).Sum(x => x.TotalKDGia);

            List<LoaiKNL> loaiNL = db.LoaiKNLs.Where(x => x.IDVT == IDVT && x.IDLoai != 1 && x.IDLoai != 2).OrderBy(x => x.OrderBy).ToList();
            ViewBag.IDLoaiNL = new SelectList(loaiNL, "IDLoai", "TenLoai");

            return View(res.ToList());
        }
        public List<FGenResultValidation> GetGenResult(int? IDVT, DateTime dt)
        {
            var vt = db.VitriKNL_searchByIDVT(IDVT).FirstOrDefault();
            ViewBag.TenViTri = vt?.TenViTri ?? "";
            ViewBag.TenPB = vt?.TenPhongBan ?? "";
            ViewBag.ThangDG = (DateTime?)dt ?? default(DateTime);
            var DataRes = new GenKNLValidation();

            var res = (from a in db.KhungNangLuc_SearchByIDVT(IDVT)
                       select new FGenResultValidation
                       {
                           IDNL = a.IDNL,
                           TenNL = a.TenNL,
                           IDLoaiNL = a.IDLoaiNL,
                           IDVT = a.IDVT,
                           TenViTri = a.TenViTri,
                           IDPB = a.IDPB,
                           DinhMuc = a.IsDanhGia != 0 ? a.DinhMuc : 0,
                           IsDanhGia = a.IsDanhGia,
                       }).ToList().OrderBy(x => x.OrderBy);
            foreach (var item in res)
            {
                var kq = db.KNL_KQ_searchByIDNL(item.IDNL, dt).ToList();
                item.Total = kq.Count();
                item.TotalKDGia = kq.Where(x => x.DiemDG == null).Count();
                item.TotalDat = kq.Where(x => x.DiemDG == item.DinhMuc).Count();
                item.TotalVuot = kq.Where(x => x.DiemDG > item.DinhMuc).Count();
                item.TotalKDat = kq.Where(x => x.DiemDG < item.DinhMuc).Count();
                if (kq.Count() > 0)
                {
                    item.TileDat = item.TotalDat * 100 / item.Total;
                    item.TileKDat = item.TotalKDat * 100 / item.Total;
                    item.TileKDGia = item.TotalKDGia * 100 / item.Total;
                    item.TileVuot = item.TotalVuot * 100 / item.Total;
                }
            }
            //DataRes.ListGenNL = res.ToList();
            //DataRes.TotalDG = res.Sum(x => x.Total);
            //DataRes.TotalDat = res.Sum(x => x.TotalDat);
            //DataRes.TotalKDat = res.Sum(x => x.TotalKDat);
            //DataRes.TotalVuot = res.Sum(x => x.TotalVuot);
            //DataRes.TotalKDGia = res.Sum(x => x.TotalKDGia);

            return res.ToList();
        }
    }
}