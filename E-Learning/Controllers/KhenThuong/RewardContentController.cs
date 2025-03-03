using E_Learning.Models;
using E_Learning.ModelsQLKhenThuong;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace E_Learning.Controllers.KhenThuong
{
    public class RewardContentController : Controller
    {
        ELEARNINGEntities db = new ELEARNINGEntities();
        int Idquyen = MyAuthentication.IDQuyen;
        String ControllerName = "RewardContent";
        // GET: RewardContent
        public ActionResult Index(int? page, string search, int? IDND)
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
            if (IDND == null) IDND = 0;

            var res = (from a in db.KT_NoiDungThuong
                       select new KhenThuongDTO
                       {
                           ID = a.ID,
                           IDLoaiKhenThuong = a.ID_LoaiKhenThuong != null ? (int)a.ID_LoaiKhenThuong : 0,
                           NoiDungKhenThuong = a.NoiDungKhenThuong,
                           SoQuyetDinh = a.SoQuyetDinh,
                           NgayQuyetDinh = (DateTime) a.NgayQuyetDinh,
                           GiaTriLamLoi = a.GiaTriLamLoi != null ? (decimal) a.GiaTriLamLoi : 0,
                           TongTienThuong = a.TongTienThuong != null ? (decimal) a.TongTienThuong : 0
                       });

            List<KT_NoiDungThuong> dt = db.KT_NoiDungThuong.ToList();
            // dropdown
            ViewBag.IDND = new SelectList(db.KT_NoiDungThuong, "ID", "NoiDungKhenThuong");

            int pageSize = 20;
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

            List<KT_LoaiThuong> listLoaiThuong = db.KT_LoaiThuong.ToList();
            ViewBag.LoaiThuong = new SelectList(listLoaiThuong, "ID_Loai", "TenLoaiThuong");

            return PartialView();
        }

        [HttpPost]
        public ActionResult Create(KhenThuongDTO DTO)
        {
            try
            {
                var data = new KT_NoiDungThuong()
                {
                    SoQuyetDinh = DTO.SoQuyetDinh,
                    NoiDungKhenThuong = DTO.NoiDungKhenThuong,
                    ID_LoaiKhenThuong = DTO.IDLoaiKhenThuong,
                    NgayQuyetDinh = DTO.NgayQuyetDinh
                };

                db.KT_NoiDungThuong.Add(data);

                db.SaveChanges();
                TempData["msgSuccess"] = "<script>alert('Thêm mới thành công');</script>";
            } catch (Exception ex)
            {
                TempData["msgError"] = "<script>alert('Có lỗi khi thêm mới: " + ex.Message + "');</script>";
            }
            
            return RedirectToAction("Index", "RewardContent");
        }

        public ActionResult Edit(int id)
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            if (!ListQuyen.Contains(CONSTKEY.EDIT))
            {
                TempData["msgError"] = "<script>alert('Bạn không có quyền thực hiện chức năng này');</script>";
                return RedirectToAction("", "Home");
            }
            var data = db.KT_NoiDungThuong.Where(x => x.ID == id).SingleOrDefault();
            var DTO = new KhenThuongDTO()
            {
                NoiDungKhenThuong = data.NoiDungKhenThuong,
                SoQuyetDinh = data.SoQuyetDinh,
                NgayQuyetDinh = data.NgayQuyetDinh,
                IDLoaiKhenThuong = (int) data.ID_LoaiKhenThuong
            };
            List<KT_LoaiThuong> listLoaiThuong = db.KT_LoaiThuong.ToList();
            ViewBag.LoaiThuong = new SelectList(listLoaiThuong, "ID_Loai", "TenLoaiThuong", data.ID_LoaiKhenThuong);

            return PartialView(DTO);
        }

        [HttpPost]
        public ActionResult Edit(KhenThuongDTO DTO)
        {
            try
            {
                var data = db.KT_NoiDungThuong.Where(x => x.ID == DTO.ID).SingleOrDefault();
                data.NoiDungKhenThuong = DTO.NoiDungKhenThuong;
                data.SoQuyetDinh = DTO.SoQuyetDinh;
                data.NgayQuyetDinh = DTO.NgayQuyetDinh;
                data.ID_LoaiKhenThuong = DTO.IDLoaiKhenThuong;

                db.SaveChanges();
                TempData["msgSuccess"] = "<script>alert('Cập nhập thành công');</script>";
            }
            catch (Exception e)
            {
                TempData["msgSuccess"] = "<script>alert('Cập nhập thất bại " + e.Message + " ');</script>";
            }

            return RedirectToAction("Index", "RewardContent");
        }

        public ActionResult Delete(int id)
        {
            try
            {
                var data = db.KT_NoiDungThuong.Where(x => x.ID == id).SingleOrDefault();
                db.KT_NoiDungThuong.Remove(data);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                TempData["msgSuccess"] = "<script>alert('Xóa thất bại " + e.Message + " ');</script>";
            }
            return RedirectToAction("Index", "RewardContent");
        }
    }
}