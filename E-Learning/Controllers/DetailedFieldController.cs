using PagedList;
using E_Learning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_Learning.Controllers
{
    public class DetailedFieldController : Controller
    {
        ELEARNINGEntities db_context = new ELEARNINGEntities();
        // GET: DetailedField
        public ActionResult Index(int id, int? page)
        {
            if (User.Identity.IsAuthenticated)
            {
                var res = (from lvct in db_context.CTLVDTs
                           join lv in db_context.LinhVucDTs on lvct.LVDTID equals lv.IDLVDT
                           join nd in db_context.NoiDungDTs on lvct.IDCTLVDT equals nd.IDCTLVDT into LinhVucTSND
                           select new DetailedFieldValidation
                           {
                               IDCTLVDT = lvct.IDCTLVDT,
                               TenCTLVDT = lvct.TenCTLVDT,
                               LVDTID = lv.IDLVDT,
                               TenLVDT = lv.TenLVDT,
                               SLND = LinhVucTSND.Count(),
                           }).Where(x => x.LVDTID == id).ToList();
                if (page == null) page = 1;
                int pageSize = 20;
                int pageNumber = (page ?? 1);
                return View(res.ToList().ToPagedList(pageNumber, pageSize));
            }
            else
            {
                return RedirectToAction("", "Login");
            }
        }
    }
}