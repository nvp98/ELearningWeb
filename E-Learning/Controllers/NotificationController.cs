using DocumentFormat.OpenXml.Office2010.Excel;
using E_Learning.Models;
using E_Learning.ModelsQTHD;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_Learning.Controllers
{
    public class NotificationController : Controller
    {
        // GET: Notification
        ELEARNINGEntities db = new ELEARNINGEntities();
        int Idquyen = MyAuthentication.IDQuyen;
        String ControllerName = "Notification";
        public ActionResult Index(int? page)
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            ViewBag.QUYENCN = ListQuyen;
            if (!ListQuyen.Contains(CONSTKEY.V))
            {
                TempData["msgError"] = "<script>alert('Bạn không có quyền truy cập chức năng này');</script>";
                return RedirectToAction("", "Home");
            }

            var res = (from a in db.ThongBaos
                       select new NotificationView
                       {
                          ID = a.ID,
                          NgayTB = (DateTime)a.NgayTB,
                          NoiDungTB = a.NoiDungTB,
                          TinhTrang = a.TinhTrang
                       }).OrderBy(x => x.NgayTB).ToList();

            if (page == null) page = 1;
            int pageSize = 50;
            int pageNumber = (page ?? 1);
            return View(res.ToList().ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Create()
        {
            return PartialView();
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(NotificationView _DO)
        {
            try
            {
                db.ThongBao_insert(_DO.NoiDungTB,DateTime.Now,1);
                TempData["msgSuccess"] = "<script>alert('Thêm mới thành công');</script>";
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Có lỗi khi thêm mới: " + e.Message + "');</script>";
            }
            //return View();
            return RedirectToAction("Index", "Notification");
        }

        public ActionResult Edit(int id)
        {
            var res = (from c in db.ThongBaos.Where(x => x.ID == id)
                       select new NotificationView
                       {
                         ID = c.ID,
                         NoiDungTB=c.NoiDungTB,
                         NgayTB=(DateTime)c.NgayTB,
                         TinhTrang = c.TinhTrang,
                       }).ToList();
            NotificationView DO = new NotificationView();

            if (res.Count > 0)
            {
                foreach (var c in res)
                {
                   DO.NoiDungTB = c.NoiDungTB;
                    DO.ID = c.ID;
                    DO.NgayTB =c.NgayTB;
                    DO.TinhTrang = c.TinhTrang;
                }

                db.Configuration.ProxyCreationEnabled = false;

            }
            else
            {
                return HttpNotFound();
            }

            var selectListTT = new SelectList(
                                            new List<SelectListItem>
                                            {
                                new SelectListItem {Text = "Không Thông báo", Value = "0"},
                                new SelectListItem {Text = "Hoạt động", Value = "1"},
}, "Value", "Text", DO.TinhTrang);
            ViewBag.SelectList = selectListTT;

            return PartialView(DO);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(NotificationView _DO)
        {
            try
            {

                db.ThongBao_update(_DO.ID, _DO.NoiDungTB, DateTime.Now, _DO.TinhTrang);

                TempData["msgSuccess"] = "<script>alert('Cập nhập thành công');</script>";
            }
            catch (Exception e)
            {

                TempData["msgSuccess"] = "<script>alert('Cập nhập thất bại " + e.Message + " ');</script>";
            }

            return RedirectToAction("Index", "Notification");
        }
        public ActionResult Delete(int id)
        {
            try
            {
                db.ThongBao_delete(id);
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Xóa dữ liệu thất bại');</script>";
            }
            return RedirectToAction("Index", "Notification");
        }

        public ActionResult Details()
        {
            var res = (from c in db.ThongBaos.Where(x=>x.TinhTrang ==1)
                       select new NotificationView
                       {
                           ID = c.ID,
                           NoiDungTB = c.NoiDungTB,
                           NgayTB = (DateTime)c.NgayTB,
                           TinhTrang = c.TinhTrang,
                       }).OrderBy(x=>x.NgayTB).FirstOrDefault();
            return PartialView(res);
        }
    }
}