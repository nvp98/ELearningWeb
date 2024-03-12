using E_Learning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_Learning.Controllers
{
    public class StatisticController : Controller
    {
        ELEARNINGEntities db = new ELEARNINGEntities();
        int Idquyen = MyAuthentication.IDQuyen;
        String ControllerName = "Statistic";
        // GET: Statistic
        public ActionResult Index()
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            ViewBag.QUYENCN = ListQuyen;
            if (!ListQuyen.Contains(CONSTKEY.V))
            {
                TempData["msgError"] = "<script>alert('Bạn không có quyền truy cập chức năng này');</script>";
                return RedirectToAction("", "Home");
            }

            StatisticValidation res = new StatisticValidation();
            //List<PhongBan> dt = db.PhongBans.Where(x=>x.MaPB !=""&&x.MaPB != null && x.MaPB != "Trùng lặp").ToList();
            //List<KNL_PhanXuong> dtpx = db.KNL_PhanXuong.Where(x => x.TenPX != "" && x.TenPX != null).ToList();
            //List<KNL_To> dtto = db.KNL_To.Where(x => x.TenTo != "" && x.TenTo != null).ToList();
            //List<KNL_Nhom> dtnhom = db.KNL_Nhom.Where(x => x.TenNhom != "" && x.TenNhom != null).ToList();
            var hv = db.TaiKhoan_select("").ToList();
            var lh = db.LopHoc_select().Count();
            var vtknl = db.VitriKNL_search().Count();
            var sldg = db.KNL_KQ.Count();
            var xnht = db.XNHocTap_select().ToList();
            float tlxnht = xnht.Where(x => x.XNHT == true).Count()*100/xnht.Count();
            var slch = db.CauHois.ToList();
            var sldt = db.DeThis.Where(x=>x.DiemChuan != 0 && x.TongSoCau != 0 && x.ThoiGianLamBai != 0).ToList();
            var slnddt = db.NoiDungDT_select().ToList();
            var gv = db.NhanViens.Where(x => x.IDQuyen == 2 || x.IDQuyen == 3).Count();
            res = new StatisticValidation()
            {
                SLHV = hv.Count(),
                SLuotDT = xnht.Count(),
                SLLH = lh,
                SLVTKNL = vtknl,
                SLDG = sldg,
                TLHT = tlxnht,
                SLCH = slch.Count(),
                SLDT =sldt.Count(),
                SLNDDT = slnddt.Count(),
                SLTGDT = (int?)slnddt.Sum(x=>x.ThoiLuongDT)??0,
                SLGV =gv
            };

            return View(res);
        }
        public JsonResult GetLVDT()
        {
            List<LinhVucDT> lh = db.LinhVucDTs.ToList();
            var slnddt = db.NoiDungDT_select().ToList();
            var res = (from a in lh
                       select new ChartLVDT { TenLV = a.TenLVDT, GiaTri = slnddt.Where(x => x.LVDTID == a.IDLVDT).Count(), TLGiaTri = slnddt.Where(x => x.LVDTID == a.IDLVDT).Count() * 100 / slnddt.Count() }).ToList();
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetNDDTBP()
        {
            ChartNDDT chartNDDT = new ChartNDDT();
            List<LinhVucDT> lvdt = db.LinhVucDTs.ToList();
            List<PhongBan> dt = db.PhongBans.Where(x => x.MaPB != "" && x.MaPB != null && x.MaPB != "Trùng lặp").ToList();
            var slnddt = db.NoiDungDT_select().ToList();
            List<PhongBan> rePB = new List<PhongBan>();
            foreach(var a in dt)
            {
                var nd = slnddt.Where(x => x.BPLID == a.IDPhongBan).ToList();
                if (nd.Count() > 0)
                {
                    rePB.Add(a);
                }
            }
            List<LVDT> ListLVDT = new List<LVDT>();
            foreach(var a in lvdt)
            {
                LVDT l = new LVDT();
                List<int> dskq = new List<int>();
                foreach(var pb in rePB)
                {
                    var nddt = slnddt.Where(x => x.BPLID == pb.IDPhongBan && x.LVDTID == a.IDLVDT).ToList();
                    dskq.Add(nddt.Count());
                }
                l.name = a.TenLVDT;
                l.data = dskq;
                ListLVDT.Add(l);
            }
            chartNDDT.ListTenBP = rePB.Select(x=>x.TenPhongBan).ToList();
            chartNDDT.ListLVDT = ListLVDT.ToList();
            return Json(chartNDDT, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetKQHT()
        {
            List<XNHocTap> ht = db.XNHocTaps.ToList();
            var res = new List<int>(new int[] { ht.Where(x=>x.XNTG ==true).Count(), ht.Where(x => x.XNTG == true && x.XNHT ==true).Count(), ht.Where(x => x.XNTG == false).Count() });
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetKQDT()
        {
            ChartKQHT chartKQDT = new ChartKQHT();
            List<BaiThi> bt = db.BaiThis.ToList();
            chartKQDT.Dat = bt.Where(x => x.TinhTrang == true).Count();
            chartKQDT.KhongDat = bt.Where(x => x.TinhTrang == false).Count();

            List<PhongBan> dt = db.PhongBans.Where(x => x.MaPB != "" && x.MaPB != null && x.MaPB != "Trùng lặp").ToList();
            //List<PhongBan> rePBDat = new List<PhongBan>();
            //List<PhongBan> rePBKDat = new List<PhongBan>();
            var kqdat = new List<KQDT>();
            var kqkdat = new List<KQDT>();
            foreach (var a in dt)
            {
                var nd = bt.Where(x => x.TinhTrang == true && x.IDPhongBan == a.IDPhongBan).ToList();
                var ndk = bt.Where(x => x.TinhTrang == false && x.IDPhongBan == a.IDPhongBan).ToList();
                if (nd.Count() > 0)
                {
                    var n = new KQDT { name = a.TenPhongBan, data = nd.Count() };
                    kqdat.Add(n);
                }
                if (ndk.Count() > 0)
                {
                    var n = new KQDT { name = a.TenPhongBan, data = ndk.Count() };
                    kqkdat.Add(n);
                }
            }
            chartKQDT.ListKQD = kqdat;
            chartKQDT.ListKQKD = kqkdat;

            return Json(chartKQDT, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReportKNL()
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            ViewBag.QUYENCN = ListQuyen;
            if (!ListQuyen.Contains(CONSTKEY.V))
            {
                TempData["msgError"] = "<script>alert('Bạn không có quyền truy cập chức năng này');</script>";
                return RedirectToAction("", "Home");
            }

            StatisticValidation res = new StatisticValidation();
            //List<PhongBan> dt = db.PhongBans.Where(x=>x.MaPB !=""&&x.MaPB != null && x.MaPB != "Trùng lặp").ToList();
            //List<KNL_PhanXuong> dtpx = db.KNL_PhanXuong.Where(x => x.TenPX != "" && x.TenPX != null).ToList();
            //List<KNL_To> dtto = db.KNL_To.Where(x => x.TenTo != "" && x.TenTo != null).ToList();
            //List<KNL_Nhom> dtnhom = db.KNL_Nhom.Where(x => x.TenNhom != "" && x.TenNhom != null).ToList();
            var hv = db.TaiKhoan_select("").ToList();
            var lh = db.LopHoc_select().Count();
            var vtknl = db.VitriKNL_search().Count();
            var sldg = db.KNL_KQ.Count();
            var xnht = db.XNHocTap_select().ToList();
            float tlxnht = xnht.Where(x => x.XNHT == true).Count() * 100 / xnht.Count();
            var slch = db.CauHois.ToList();
            var sldt = db.DeThis.Where(x => x.DiemChuan != 0 && x.TongSoCau != 0 && x.ThoiGianLamBai != 0).ToList();
            var slnddt = db.NoiDungDT_select().ToList();
            var gv = db.NhanViens.Where(x => x.IDQuyen == 2 || x.IDQuyen == 3).Count();
            res = new StatisticValidation()
            {
                SLHV = hv.Count(),
                SLuotDT = xnht.Count(),
                SLLH = lh,
                SLVTKNL = vtknl,
                SLDG = sldg,
                TLHT = tlxnht,
                SLCH = slch.Count(),
                SLDT = sldt.Count(),
                SLNDDT = slnddt.Count(),
                SLTGDT = (int?)slnddt.Sum(x => x.ThoiLuongDT) ?? 0,
                SLGV = gv
            };

            return View();
        }


    }
}