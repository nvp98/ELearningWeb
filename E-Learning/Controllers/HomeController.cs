using E_Learning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_Learning.Controllers
{
    public class HomeController : Controller
    {
        ELEARNINGEntities db = new ELEARNINGEntities();
        public ActionResult Index()
        {
            return View();
        }
        public List<String> GetPermisionCN(int? Idquyen, string ControllerName)
        {
            //var IdControll = db.ListControllers.Where(x => x.Controller == ControllerName).Select(x => x.ID).FirstOrDefault();
            var lsQuyen = (from a in db.QuyenDetails.Where(x => x.IDQuyen == Idquyen && x.isActive == 1)
                           join b in db.QuyenCNs on a.IDQuyenCN equals b.ID
                           join c in db.ListControllers.Where(x => x.Controller == ControllerName && x.isActive == 1) on a.IDController equals c.ID
                           select b.MaQuyen).ToList();
            var mulQuyen = db.PhanQuyenHTs.Where(x => x.IDNV == MyAuthentication.ID).ToList();
            if (mulQuyen.Count > 0)
            {
                foreach (var item in mulQuyen)
                {
                    var lsq = (from a in db.QuyenDetails.Where(x => x.IDQuyen == item.IDQuyen && x.isActive == 1)
                               join b in db.QuyenCNs on a.IDQuyenCN equals b.ID
                               join c in db.ListControllers.Where(x => x.Controller == ControllerName && x.isActive == 1) on a.IDController equals c.ID
                               select b.MaQuyen).ToList();
                    lsQuyen.AddRange(lsq);
                }
            }
            if (!lsQuyen.Contains(CONSTKEY.V)) lsQuyen = new List<string>();
            return lsQuyen;
        }
        public List<String> GetPermisionControll(int? Idquyen)
        {
            //var IdControll = db.ListControllers.Where(x => x.Controller == ControllerName).Select(x => x.ID).FirstOrDefault();
            var lsQuyen = (from a in db.QuyenDetails.Where(x => x.IDQuyen == Idquyen && x.IDQuyenCN == 1 && x.isActive == 1)
                           join b in db.ListControllers.Where(x => x.isActive == 1) on a.IDController equals b.ID
                           select b.Controller).ToList();
            var mulQuyen = db.PhanQuyenHTs.Where(x=>x.IDNV == MyAuthentication.ID).ToList();
            if(mulQuyen.Count > 0)
            {
                foreach (var item in mulQuyen)
                {
                    var lsq = (from a in db.QuyenDetails.Where(x => x.IDQuyen == item.IDQuyen && x.IDQuyenCN == 1 && x.isActive == 1)
                               join b in db.ListControllers.Where(x => x.isActive == 1) on a.IDController equals b.ID
                               select b.Controller).ToList();
                    lsQuyen.AddRange(lsq);
                }
            }
            return lsQuyen;
        }

        public JsonResult Get_LichSu_KNL()
        {
            KNL_LSDG knl = new KNL_LSDG() { DAT = 0, KDGia = 0, CHUADG = 0, KDAT = 0, TONGNL = 0, VUOT = 0 };
            var res = db.KNL_LSDG.Where(x => x.NVID == MyAuthentication.ID).ToList();
            knl.NVID = MyAuthentication.ID;
            knl.VUOT = res.LastOrDefault()?.VUOT;
            knl.KDAT = res.LastOrDefault()?.KDAT;
            knl.DAT = res.LastOrDefault()?.DAT;
            knl.TONGNL = res.LastOrDefault()?.TONGNL;
            knl.ThangDG = res.LastOrDefault()?.ThangDG;
            knl.CHUADG = res.LastOrDefault()?.CHUADG;
            knl.KDGia = res.LastOrDefault()?.KDGia;
            return Json(knl, JsonRequestBehavior.AllowGet);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public JsonResult KeepSessionAlive()
        {
            return new JsonResult { Data = "Success" };
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