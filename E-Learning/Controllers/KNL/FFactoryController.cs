using E_Learning.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_Learning.Controllers
{
    public class FFactoryController : Controller
    {
        ELEARNINGEntities db = new ELEARNINGEntities();
        int Idquyen = MyAuthentication.IDQuyen;
        String ControllerName = "FFactory";
        // GET: FFactory
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
            if (IDPB == null) IDPB = 0;
            int idpb = MyAuthentication.IDPhongban;
            var manv = MyAuthentication.Username;

            var res = (from a in db.KNL_PhanXuong
                       join d in db.PhongBans
                       on a.IDPhongBan equals d.IDPhongBan into uli
                       from d in uli.DefaultIfEmpty()
                       join e in db.KNL_Khoi
                       on a.IDKhoi equals e.ID into ulis
                       from e in ulis.DefaultIfEmpty()
                       select new PhanXuongValidation
                       {
                           IDPhongBan = a.IDPhongBan,
                           TenPhongBan = d.TenPhongBan,
                           ID =a.ID,
                           MaPX =a.MaPX,
                           TenPX =a.TenPX,
                           IDKhoi =a.IDKhoi,
                           TenKhoi =e.TenKhoi
                       }).ToList();

            List<PhongBan> dt = db.PhongBans.ToList();
            ViewBag.IDPB = new SelectList(dt, "IDPhongBan", "TenPhongBan");
            if (IDPB != 0) res = res.Where(x => x.IDPhongBan == IDPB).ToList();

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

            List<PhongBan> dt = db.PhongBans.ToList();
            ViewBag.IDPhongBan = new SelectList(dt, "IDPhongBan", "TenPhongBan");

            List<KNL_Khoi> khoi = db.KNL_Khoi.ToList();
            ViewBag.IDKhoi = new SelectList(khoi, "ID", "TenKhoi");

            return PartialView();
        }
        public int GetIDPX(string TenPX,int? IDPB)
        {
            var model = db.KNL_PhanXuong.Where(x => x.TenPX == TenPX && x.IDPhongBan ==IDPB).SingleOrDefault();
            if (model == null)
                return 0;
            return model.ID;
        }
        [HttpPost]
        public ActionResult Create(PhanXuongValidation _DO, FormCollection collection)
        {
            //try
            //{
            //    if (_DO.TenPX != null )
            //    {
            //        var aa = db.KNLPhanXuong_insert(_DO.MaPX, _DO.TenPX,(int?)_DO.IDPhongBan,_DO.IDKhoi);
            //    }
            //    TempData["msgSuccess"] = "<script>alert('Thêm mới thành công');</script>";
            //}
            //catch (Exception e)
            //{
            //    TempData["msgError"] = "<script>alert('Có lỗi khi thêm mới: " + e.Message + "');</script>";
            //}
            try
            {
                if (!String.IsNullOrEmpty(_DO.TenPX))
                {
                    var aa = db.KNLPhanXuong_insert(_DO.MaPX, _DO.TenPX, (int?)_DO.IDPhongBan, _DO.IDKhoi);
                }
                var ListVT = new List<ViTriKNLValidation>();
                foreach (var key in collection.AllKeys)
                {
                    if (key.Split('_')[0] == "TenVTKNL")
                    {
                        ListVT.Add(new ViTriKNLValidation() { TenViTri = collection[key], MaViTri = collection["MaViTri_" + key.Split('_')[1]] });
                    }
                }
                foreach (var item in ListVT)
                {
                    //int idvt = GetIDNhom(item.TenViTri.Trim());
                    if (!String.IsNullOrEmpty(item.TenViTri))
                    {
                        var aa = db.KNLPhanXuong_insert(item.TenViTri, item.MaViTri, (int?)_DO.IDPhongBan, _DO.IDKhoi);
                    }
                }
                TempData["msgSuccess"] = "<script>alert('Thêm mới thành công');</script>";
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Có lỗi khi thêm mới: " + e.Message + "');</script>";
            }
            //return View();
            return RedirectToAction("Index", "FFactory");
        }
        public ActionResult Edit(int id)
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            if (!ListQuyen.Contains(CONSTKEY.EDIT))
            {
                TempData["msgError"] = "<script>alert('Bạn không có quyền thực hiện chức năng này');</script>";
                return RedirectToAction("", "Home");
            }
            var res = (from a in db.KNL_PhanXuong.Where(x => x.ID == id)
                       join d in db.PhongBans
                        on a.IDPhongBan equals d.IDPhongBan into uli
                       from d in uli.DefaultIfEmpty()
                       join e in db.KNL_Khoi
                       on a.IDKhoi equals e.ID into ulis
                       from e in ulis.DefaultIfEmpty()
                       select new PhanXuongValidation
                       {
                           IDPhongBan = a.IDPhongBan,
                           TenPhongBan = d.TenPhongBan,
                           ID = a.ID,
                           MaPX = a.MaPX,
                           TenPX = a.TenPX,
                           IDKhoi = a.IDKhoi,
                           TenKhoi = e.TenKhoi
                       }).ToList();
            PhanXuongValidation DO = new PhanXuongValidation();

            if (res.Count > 0)
            {
                foreach (var c in res)
                {
                    DO.IDPhongBan = c.IDPhongBan;
                    DO.MaPX = c.MaPX;
                    DO.TenPhongBan = c.TenPhongBan;
                    DO.TenPX = c.TenPX;
                    DO.ID = c.ID;
                    DO.IDKhoi = c.IDKhoi;
                    DO.TenKhoi = c.TenKhoi;
                }

                db.Configuration.ProxyCreationEnabled = false;
                List<PhongBan> dt = db.PhongBans.ToList();
                ViewBag.IDPhongBan = new SelectList(dt, "IDPhongBan", "TenPhongBan", DO.IDPhongBan);
                List<KNL_Khoi> khoi = db.KNL_Khoi.ToList();
                ViewBag.IDKhoi = new SelectList(khoi, "ID", "TenKhoi",DO.IDKhoi);

            }
            else
            {
                return HttpNotFound();
            }
            return PartialView(DO);
        }
        [HttpPost]
        public ActionResult Edit(PhanXuongValidation _DO)
        {
            try
            {

                db.KNLPhanXuong_update(_DO.ID, _DO.MaPX,_DO.TenPX, (int?)_DO.IDPhongBan,_DO.IDKhoi);

                TempData["msgSuccess"] = "<script>alert('Cập nhập thành công');</script>";
            }
            catch (Exception e)
            {

                TempData["msgSuccess"] = "<script>alert('Cập nhập thất bại " + e.Message + " ');</script>";
            }

            return RedirectToAction("Index", "FFactory");
        }
        public ActionResult Delete(int id)
        {
            try
            {
                var a = db.ViTriKNLs.Where(x=>x.IDPX == id).ToList();
                var b = db.KNL_To.Where(x=>x.IDPhanXuong ==id).ToList();
                var c = db.KNL_Nhom.Where(x => x.IDPhanXuong == id).ToList();
                if (a.Count == 0 && b.Count ==0 && c.Count ==0) db.KNLPhanXuong_delete(id);
                else TempData["msgError"] = "<script>alert('Xóa dữ liệu thất bại');</script>";
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Xóa dữ liệu thất bại');</script>";
            }
            return RedirectToAction("Index", "FFactory");
        }
    }
}