using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using E_Learning.Models;

namespace E_Learning.Controllers
{
    public class LibraryFieldController : Controller
    {
        ELEARNINGEntities db_context = new ELEARNINGEntities();
        int Idquyen = MyAuthentication.IDQuyen;
        String ControllerName = "LibraryField";
        // GET: LibraryField
        public ActionResult Index(int? page, int? IDNDDT, int? IDCTLVDT)
        {
            if (User.Identity.IsAuthenticated)
            {
                var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
                ViewBag.QUYENCN = ListQuyen;
                if (!ListQuyen.Contains(CONSTKEY.V))
                {
                    TempData["msgError"] = "<script>alert('Bạn không có quyền truy cập chức năng này');</script>";
                    return RedirectToAction("", "Home");
                }
                if (IDNDDT == null) IDNDDT = 0;
                if (IDCTLVDT == null) IDCTLVDT = 0;
                var res = (from lv in db_context.LinhVucDTs
                           join lvct in db_context.CTLVDTs on lv.IDLVDT equals lvct.LVDTID into LinhVucTSCT
                           join nd in db_context.NoiDungDTs on lv.IDLVDT equals nd.LVDTID into LinhVucTSND
                           select new FieldValidation
                           {
                               IDLVDT = lv.IDLVDT,
                               TenLVDT = lv.TenLVDT,
                               SLLVCT = LinhVucTSCT.Count(),
                               SLNDĐT = LinhVucTSND.Count(),
                           });

                List<NoiDungDT_select_Result> ctlvdt = db_context.NoiDungDT_select().ToList();
                ViewBag.IDNDDT = new SelectList(ctlvdt, "IDND", "NoiDung");
                List<CTLVDT> ctl = db_context.CTLVDTs.ToList();
                ViewBag.CTLVDTID = new SelectList(ctl, "IDCTLVDT", "TenCTLVDT");

                if (IDNDDT != 0) return RedirectToAction("Index", "TLesson",new { id = IDNDDT });
                if (IDCTLVDT != 0) return RedirectToAction("Index", "LibraryETContent", new { id = IDCTLVDT });



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