using E_Learning.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_Learning.Controllers.DMST
{
    public class RenewLinhVucController : Controller
    {
        ELEARNINGEntities db = new ELEARNINGEntities();
        int Idquyen = MyAuthentication.IDQuyen;
        String ControllerName = "RenewLinhVuc";
        // GET: RenewLinhVuc
        // Demo data

        public ActionResult Index( string search, int page = 1)
        {
            var data = db.LinhVucDTs.AsQueryable();
            if (!string.IsNullOrEmpty(search))
                data = data.Where(x => x.TenLVDT.Contains(search));

            var paged = data.OrderBy(x => x.IDLVDT).ToPagedList(page, 1);

            if (Request.IsAjaxRequest())
                return PartialView("_ListLinhVuc", paged);

            return View(paged);
        }

        public ActionResult CreatePartial()
        {
            return PartialView("_Create", new LinhVucDT());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LinhVucDT model)
        {
            if (ModelState.IsValid)
            {
                //db.LinhVucDTs.Add(model);
                //db.SaveChanges();
                return Json(new { success = true });
            }
            return PartialView("_Create", model);
        }

        public ActionResult EditPartial(int id)
        {
            var model = db.LinhVucDTs.Find(id);
            return PartialView("_Edit", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(LinhVucDT model)
        {
            if (ModelState.IsValid)
            {
                //db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                //db.SaveChanges();
                return Json(new { success = true });
            }
            return PartialView("_Edit", model);
        }


    }
}