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
            if (!lsQuyen.Contains(CONSTKEY.V)) lsQuyen = new List<string>();
            return lsQuyen;
        }
        public List<String> GetPermisionControll(int? Idquyen)
        {
            //var IdControll = db.ListControllers.Where(x => x.Controller == ControllerName).Select(x => x.ID).FirstOrDefault();
            var lsQuyen = (from a in db.QuyenDetails.Where(x => x.IDQuyen == Idquyen && x.IDQuyenCN == 1 && x.isActive == 1)
                           join b in db.ListControllers.Where(x => x.isActive == 1) on a.IDController equals b.ID
                           select b.Controller).ToList();
            return lsQuyen;
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
    }
}