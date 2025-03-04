using E_Learning.Models;
using E_Learning.ModelsQLKhenThuong;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_Learning.Controllers.KhenThuong
{
    public class RewardTypeController : Controller
    {
        ELEARNINGEntities db = new ELEARNINGEntities();
        int Idquyen = MyAuthentication.IDQuyen;
        String ControllerName = "RewardType";
        // GET: RewardType
        public ActionResult Index(int? page, string search, int? IDLoai)
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
            if (IDLoai == null) IDLoai = 0;

            var res = (from a in db.KT_LoaiThuong
                       select new LoaiKhenThuongDTO
                       {
                           ID = a.ID_Loai,
                           TenLoaiThuong = a.TenLoaiThuong
                       }).ToList();

            List<KT_LoaiThuong> dt = db.KT_LoaiThuong.ToList();

            // dropdown
            ViewBag.IDLoaiThuong = new SelectList(dt, "ID_Loai", "TenLoaiThuong", IDLoai);

            if (IDLoai != 0) res = res.Where(x => x.ID == IDLoai).ToList();

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(res.OrderBy(a => a.ID).ToPagedList(pageNumber, pageSize));
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

            return PartialView();
        }

        [HttpPost]
        public ActionResult Create(LoaiKhenThuongDTO DTO)
        {
            try
            {
                var data = new KT_LoaiThuong()
                {
                    TenLoaiThuong = DTO.TenLoaiThuong
                };

                db.KT_LoaiThuong.Add(data);

                db.SaveChanges();
                TempData["msgSuccess"] = "<script>alert('Thêm mới thành công');</script>";
            }
            catch (Exception ex)
            {
                TempData["msgError"] = "<script>alert('Có lỗi khi thêm mới: " + ex.Message + "');</script>";
            }

            return RedirectToAction("Index", "RewardType");
        }

        public ActionResult Edit(int id)
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            if (!ListQuyen.Contains(CONSTKEY.EDIT))
            {
                TempData["msgError"] = "<script>alert('Bạn không có quyền thực hiện chức năng này');</script>";
                return RedirectToAction("", "Home");
            }
            var data = db.KT_LoaiThuong.Where(x => x.ID_Loai == id).SingleOrDefault();
            var DTO = new LoaiKhenThuongDTO()
            {
                TenLoaiThuong = data.TenLoaiThuong
            };

            return PartialView(DTO);
        }

        [HttpPost]
        public ActionResult Edit(LoaiKhenThuongDTO DTO)
        {
            try
            {
                var data = db.KT_LoaiThuong.Where(x => x.ID_Loai == DTO.ID).SingleOrDefault();
                data.TenLoaiThuong = DTO.TenLoaiThuong;

                db.SaveChanges();
                TempData["msgSuccess"] = "<script>alert('Cập nhập thành công');</script>";
            }
            catch (Exception e)
            {
                TempData["msgSuccess"] = "<script>alert('Cập nhập thất bại " + e.Message + " ');</script>";
            }

            return RedirectToAction("Index", "RewardType");
        }

        public ActionResult Delete(int id)
        {
            try
            {
                var data = db.KT_LoaiThuong.Where(x => x.ID_Loai == id).SingleOrDefault();
                db.KT_LoaiThuong.Remove(data);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                TempData["msgSuccess"] = "<script>alert('Xóa thất bại " + e.Message + " ');</script>";
            }
            return RedirectToAction("Index", "RewardType");
        }
    }
}