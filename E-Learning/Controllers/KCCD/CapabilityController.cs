using E_Learning.ModelsKCCD;
using E_Learning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace E_Learning.Controllers.KCCD
{
    public class CapabilityController : Controller
    {
        ELEARNINGEntities db = new ELEARNINGEntities();
        ELEARNINGEntities dbKCCCD = new ELEARNINGEntities();
        int Idquyen = MyAuthentication.IDQuyen;
        String ControllerName = "Capability";
        // GET: Capability
        public ActionResult Index(int? page, string search, int? IDPB, int? IDLVDT)
        {
            //var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            //ViewBag.QUYENCN = ListQuyen;
            //if (!ListQuyen.Contains(CONSTKEY.V))
            //{
            //    TempData["msgError"] = "<script>alert('Bạn không có quyền truy cập chức năng này');</script>";
            //    return RedirectToAction("", "Home");
            //}

            //if (search == null) search = "";
            //ViewBag.search = search;
            //if (IDPB == null) IDPB = 0;
            //if (IDLVDT == null) IDLVDT = 0;

            //var res = (from a in dbKCCCD.NoiDungDTKCCD_select(search)
            //           select new NoiDungDTKCCDView
            //           {
            //               ID = a.ID,
            //               LVDTID =(int)a.LVDTID,
            //               TenLVDT = a.TenLVDT,
            //               NhomNLID =(int)a.NhomNLID,
            //               TenNhomNL = a.TenND,
            //               PhongBanID =(int)a.PhongBanID,
            //               TenPhongBan = a.TenPhongBan,
            //               NgayTao =(DateTime)a.NgayTao,
            //           }).ToList();

            //List<PhongBan> dt = db.PhongBans.ToList();
            //ViewBag.IDPB = new SelectList(dt, "IDPhongBan", "TenPhongBan");
            //if (IDPB != 0) res = res.Where(x => x.PhongBanID == IDPB).ToList();
            //if (IDLVDT != 0) res = res.Where(x => x.LVDTID == IDLVDT).ToList();

            if (page == null) page = 1;
            int pageSize = 50;
            int pageNumber = (page ?? 1);
            return View();
        }

        // GET: Capability/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Capability/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Capability/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Capability/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Capability/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Capability/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Capability/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
