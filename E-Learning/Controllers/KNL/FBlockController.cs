using E_Learning.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_Learning.Controllers
{
    public class FBlockController : Controller
    {
        ELEARNINGEntities db = new ELEARNINGEntities();
        int Idquyen = MyAuthentication.IDQuyen;
        String ControllerName = "FBlock";
        // GET: FBlock
        public ActionResult Index(int? page, string search, int? IDPB)
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            ViewBag.QUYENCN = ListQuyen;
            if (!ListQuyen.Contains(CONSTKEY.V))
            {
                TempData["msgError"] = "<script>alert('Bạn không có quyền truy cập chức năng này');</script>";
                return RedirectToAction("", "Home");
            }
            if (search == null) search = "";
            ViewBag.search = search;
            int idpb = MyAuthentication.IDPhongban;
            var manv = MyAuthentication.Username;

            var res = (from a in db.KNL_Khoi
                       select new KhoiKNLValidation
                       {
                           ID = a.ID,
                           MaKhoi = a.MaKhoi,
                           TenKhoi = a.TenKhoi
                       }).ToList();

            List<PhongBan> dt = db.PhongBans.ToList();
            ViewBag.IDPB = new SelectList(dt, "IDPhongBan", "TenPhongBan");

            if (page == null) page = 1;
            int pageSize = 50;
            int pageNumber = (page ?? 1);
            return View(res.ToList().ToPagedList(pageNumber, pageSize));
        }
        public ActionResult Create()
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            if (!ListQuyen.Contains(CONSTKEY.ADD))
            {
                TempData["msgError"] = "<script>alert('Bạn không có quyền thực hiện chức năng này');</script>";
                return RedirectToAction("", "Home");
            }
            db.Configuration.ProxyCreationEnabled = false;
            //var quyen = ViewBag.CREATE;
            //if (quyen != "1") return RedirectToAction("Index", "Home");
            List<PhongBan> dt = db.PhongBans.ToList();
            ViewBag.IDPB = new SelectList(dt, "IDPhongBan", "TenPhongBan");

            return PartialView();
        }
        public int GetIDKhoi(string TenPB)
        {
            var model = db.KNL_Khoi.Where(x => x.TenKhoi == TenPB).SingleOrDefault();
            if (model == null)
                return 0;
            return model.ID;
        }
        [HttpPost]
        public ActionResult Create(KhoiKNLValidation _DO, FormCollection collection)
        {
            try
            {
                if (_DO.TenKhoi != null && GetIDKhoi(_DO.TenKhoi.Trim()) == 0)
                {
                    var aa = db.KNLKhoi_insert(_DO.MaKhoi, _DO.TenKhoi);
                }
                TempData["msgSuccess"] = "<script>alert('Thêm mới thành công');</script>";
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Có lỗi khi thêm mới: " + e.Message + "');</script>";
            }
            //return View();
            return RedirectToAction("Index", "FBlock");
        }
        public ActionResult Edit(int id)
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            if (!ListQuyen.Contains(CONSTKEY.EDIT))
            {
                TempData["msgError"] = "<script>alert('Bạn không có quyền thực hiện chức năng này');</script>";
                return RedirectToAction("", "Home");
            }

            var res = (from a in db.KNL_Khoi.Where(x => x.ID == id)
                       select new KhoiKNLValidation
                       {
                           ID = a.ID,
                           MaKhoi = a.MaKhoi,
                           TenKhoi = a.TenKhoi
                       }).ToList();
            KhoiKNLValidation DO = new KhoiKNLValidation();

            if (res.Count > 0)
            {
                foreach (var c in res)
                {
                    DO.ID = c.ID;
                    DO.MaKhoi = c.MaKhoi;
                    DO.TenKhoi = c.TenKhoi;
                }

                db.Configuration.ProxyCreationEnabled = false;
                List<PhongBan> dt = db.PhongBans.ToList();
                //ViewBag.IDPB = new SelectList(dt, "IDPhongBan", "TenPhongBan", DO.IDPB);

            }
            else
            {
                return HttpNotFound();
            }
            return PartialView(DO);
        }
        [HttpPost]
        public ActionResult Edit(KhoiKNLValidation _DO)
        {
            try
            {

                db.KNLKhoi_update(_DO.ID, _DO.MaKhoi, _DO.TenKhoi);

                TempData["msgSuccess"] = "<script>alert('Cập nhập thành công');</script>";
            }
            catch (Exception e)
            {

                TempData["msgSuccess"] = "<script>alert('Cập nhập thất bại " + e.Message + " ');</script>";
            }

            return RedirectToAction("Index", "FBlock");
        }
        public ActionResult Delete(int id)
        {
            try
            {
                db.KNLKhoi_delete(id);
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Xóa dữ liệu thất bại');</script>";
            }
            return RedirectToAction("Index", "FBlock");
        }
    }
}