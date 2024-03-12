using DocumentFormat.OpenXml.Bibliography;
using E_Learning.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_Learning.Controllers
{
    public class FNestController : Controller
    {
        ELEARNINGEntities db = new ELEARNINGEntities();
        int Idquyen = MyAuthentication.IDQuyen;
        String ControllerName = "FNest";
        // GET: FNest
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

            var res = (from a in db.KNL_To
                       join d in db.PhongBans
                       on a.IDPhongBan equals d.IDPhongBan into uli
                       from d in uli.DefaultIfEmpty()
                       join e in db.KNL_PhanXuong
                       on a.IDPhanXuong equals e.ID into ul
                       from e in ul.DefaultIfEmpty()
                       join f in db.KNL_Khoi
                       on a.IDKhoi equals f.ID into ulis
                       from f in ulis.DefaultIfEmpty()
                       select new ToKNLValidation
                       {
                           IDPhongBan = a.IDPhongBan,
                           TenPhongBan = d.TenPhongBan,
                           IDKhoi = a.IDKhoi,
                           TenKhoi = f.TenKhoi,
                           IDPhanXuong = a.IDPhanXuong,
                           TenPhanXuong = e.TenPX,
                           TenTo =a.TenTo,
                           MaTo =a.MaTo,
                           IDTo =a.IDTo,
                       }).ToList();

            List<PhongBan> dt = db.PhongBans.ToList();
            ViewBag.IDPB = new SelectList(dt, "IDPhongBan", "TenPhongBan");
            if (IDPB == null) IDPB = 0;
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

            List<KNL_PhanXuong> px = new List<KNL_PhanXuong>();
            ViewBag.IDPhanXuong = new SelectList(px, "ID", "TenPX");

            List<KNL_Khoi> khoi = db.KNL_Khoi.ToList();
            ViewBag.IDKhoi = new SelectList(khoi, "ID", "TenKhoi");

            return PartialView();
        }
        public int GetIDTo(string TenTo,int? IDPB,int? IDPX)
        {
            var model = db.KNL_To.Where(x => x.TenTo == TenTo && x.IDPhongBan ==IDPB && x.IDPhanXuong ==IDPX).SingleOrDefault();
            if (model == null)
                return 0;
            return model.IDTo;
        }
        [HttpPost]
        public ActionResult Create(ToKNLValidation _DO, FormCollection collection)
        {
            //try
            //{
            //    if (_DO.TenTo != null && GetIDTo(_DO.TenTo.Trim()) == 0)
            //    {
            //        var aa = db.KNLTo_insert(_DO.TenTo, _DO.MaTo, (int?)_DO.IDPhongBan, (int?)_DO.IDPhanXuong,_DO.IDKhoi);
            //    }
            //    TempData["msgSuccess"] = "<script>alert('Thêm mới thành công');</script>";
            //}
            //catch (Exception e)
            //{
            //    TempData["msgError"] = "<script>alert('Có lỗi khi thêm mới: " + e.Message + "');</script>";
            //}

            try
            {
                if (!String.IsNullOrEmpty(_DO.TenTo) )
                {
                    var aa = db.KNLTo_insert(_DO.TenTo, _DO.MaTo, (int?)_DO.IDPhongBan, (int?)_DO.IDPhanXuong, _DO.IDKhoi);
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
                    //int idvt = GetIDTo(item.TenViTri.Trim());
                    if (!String.IsNullOrEmpty(item.TenViTri))
                    {
                        var aa = db.KNLTo_insert(item.TenViTri, item.MaViTri, (int?)_DO.IDPhongBan, (int?)_DO.IDPhanXuong, _DO.IDKhoi);
                        //var k = db.VitriKNL_insert(item.TenViTri, _DO.MaViTri, _DO.IDPB);
                    }
                    else
                    {
                        //var k = db.VitriKNL_update(idvt,item.TenViTri, _DO.MaViTri, _DO.IDPB);
                    }
                }
                //for (int i=2;i<collection.Count - 1; i++)
                //{
                //    if (collection["TenViTri_"+i] != null)
                //    {
                //        var a = collection["TenViTri_" + i].Trim().ToString();
                //        if(GetIDVT(a) == 0)
                //        {
                //            var k = db.VitriKNL_insert(a,_DO.MaViTri, _DO.IDPB);
                //        }

                //    }
                //}

                //db.VitriKNL_insert(_DO.TenViTri,_DO.IDPB);
                TempData["msgSuccess"] = "<script>alert('Thêm mới thành công');</script>";
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Có lỗi khi thêm mới: " + e.Message + "');</script>";
            }
            //return View();
            return RedirectToAction("Index", "FNest");
        }
        public ActionResult Edit(int id)
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            if (!ListQuyen.Contains(CONSTKEY.EDIT))
            {
                TempData["msgError"] = "<script>alert('Bạn không có quyền thực hiện chức năng này');</script>";
                return RedirectToAction("", "Home");
            }
            var res = (from a in db.KNL_To.Where(x => x.IDTo == id)
                       join d in db.PhongBans
                       on a.IDPhongBan equals d.IDPhongBan into uli
                       from d in uli.DefaultIfEmpty()
                       join e in db.KNL_PhanXuong
                       on a.IDPhanXuong equals e.ID into ul
                       from e in ul.DefaultIfEmpty()
                       join f in db.KNL_Khoi
                       on a.IDKhoi equals f.ID into ulis
                       from f in ulis.DefaultIfEmpty()
                       select new ToKNLValidation
                       {
                           IDPhongBan = a.IDPhongBan,
                           TenPhongBan = d.TenPhongBan,
                           IDKhoi = a.IDKhoi,
                           TenKhoi = f.TenKhoi,
                           IDPhanXuong = a.IDPhanXuong,
                           TenPhanXuong = e.TenPX,
                           TenTo = a.TenTo,
                           MaTo = a.MaTo,
                           IDTo = a.IDTo
                       }).ToList();
            ToKNLValidation DO = new ToKNLValidation();

            if (res.Count > 0)
            {
                foreach (var c in res)
                {
                    DO.IDPhongBan = c.IDPhongBan;
                    DO.MaTo = c.MaTo;
                    DO.TenPhongBan = c.TenPhongBan;
                    DO.TenKhoi = c.TenKhoi;
                    DO.IDKhoi = c.IDKhoi;
                    DO.IDPhanXuong = c.IDPhanXuong;
                    DO.TenPhanXuong = c.TenPhanXuong;
                    DO.TenTo = c.TenTo;
                    DO.IDTo = c.IDTo;
                }

                db.Configuration.ProxyCreationEnabled = false;
                List<PhongBan> dt = db.PhongBans.ToList();
                ViewBag.IDPhongBan = new SelectList(dt, "IDPhongBan", "TenPhongBan", DO.IDPhongBan);
                List<KNL_PhanXuong> px = db.KNL_PhanXuong.Where(x => x.IDPhongBan == DO.IDPhongBan).ToList();
                ViewBag.IDPhanXuong = new SelectList(px, "ID", "TenPX", DO.IDPhanXuong);
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
        public ActionResult Edit(ToKNLValidation _DO)
        {
            try
            {

                db.KNLTo_update(_DO.IDTo, _DO.TenTo, _DO.MaTo, (int?)_DO.IDPhongBan, (int?)_DO.IDPhanXuong, _DO.IDKhoi);

                TempData["msgSuccess"] = "<script>alert('Cập nhập thành công');</script>";
            }
            catch (Exception e)
            {

                TempData["msgSuccess"] = "<script>alert('Cập nhập thất bại " + e.Message + " ');</script>";
            }

            return RedirectToAction("Index", "FNest");
        }
        public ActionResult Delete(int id)
        {
            try
            {
                var a = db.ViTriKNLs.Where(x => x.IDTo == id).ToList();
                if (a.Count ==0) db.KNLTo_delete(id);
                else TempData["msgError"] = "<script>alert('Xóa dữ liệu thất bại');</script>";
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Xóa dữ liệu thất bại');</script>";
            }
            return RedirectToAction("Index", "FNest");
        }
    }
}