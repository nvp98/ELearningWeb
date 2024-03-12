using E_Learning.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_Learning.Controllers
{
    public class FGroupController : Controller
    {
        ELEARNINGEntities db = new ELEARNINGEntities();
        int Idquyen = MyAuthentication.IDQuyen;
        String ControllerName = "FGroup";
        // GET: NhomKNL
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

            var res = (from a in db.KNL_Nhom
                       join d in db.PhongBans
                       on a.IDPhongBan equals d.IDPhongBan into uli
                       from d in uli.DefaultIfEmpty()
                       join e in db.KNL_PhanXuong
                       on a.IDPhanXuong equals e.ID into ul
                       from e in ul.DefaultIfEmpty()
                       join f in db.KNL_Khoi
                       on a.IDKhoi equals f.ID into ulis
                       from f in ulis.DefaultIfEmpty()
                       select new NhomKNLValidation
                       {
                           IDPhongBan = a.IDPhongBan,
                            TenPhongBan= d.TenPhongBan,
                           IDNhom = a.IDNhom,
                           MaNhom = a.MaNhom,
                           TenNhom = a.TenNhom,
                           IDPhanXuong =a.IDPhanXuong,
                           TenPhanXuong =e.TenPX,
                           IDKhoi = a.IDKhoi,
                           TenKhoi = f.TenKhoi
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
        public int GetIDNhom(string TenNhom,int? IDPB)
        {
            var model = db.KNL_Nhom.Where(x => x.TenNhom == TenNhom && x.IDPhongBan ==IDPB).SingleOrDefault();
            if (model == null)
                return 0;
            return model.IDNhom;
        }
        [HttpPost]
        public ActionResult Create(NhomKNLValidation _DO, FormCollection collection)
        {
            //try
            //{
            //    if (_DO.TenNhom != null && GetIDNhom(_DO.TenNhom.Trim()) == 0)
            //    {
            //        var aa = db.KNLNhom_insert(_DO.TenNhom, _DO.MaNhom, (int?)_DO.IDPhongBan,(int?)_DO.IDPhanXuong,_DO.IDKhoi);
            //    }
            //    TempData["msgSuccess"] = "<script>alert('Thêm mới thành công');</script>";
            //}
            //catch (Exception e)
            //{
            //    TempData["msgError"] = "<script>alert('Có lỗi khi thêm mới: " + e.Message + "');</script>";
            //}

            try
            {
                if (!String.IsNullOrEmpty(_DO.TenNhom))
                {
                    var aa = db.KNLNhom_insert(_DO.TenNhom, _DO.MaNhom, (int?)_DO.IDPhongBan, (int?)_DO.IDPhanXuong, _DO.IDKhoi);
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
                        var aa = db.KNLNhom_insert(item.TenViTri, item.MaViTri, (int?)_DO.IDPhongBan, (int?)_DO.IDPhanXuong, _DO.IDKhoi);
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
            return RedirectToAction("Index", "FGroup");
        }
        public ActionResult Edit(int id)
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            if (!ListQuyen.Contains(CONSTKEY.EDIT))
            {
                TempData["msgError"] = "<script>alert('Bạn không có quyền thực hiện chức năng này');</script>";
                return RedirectToAction("", "Home");
            }
            var res = (from a in db.KNL_Nhom.Where(x => x.IDNhom == id)
                       join d in db.PhongBans
                      on a.IDPhongBan equals d.IDPhongBan into uli
                       from d in uli.DefaultIfEmpty()
                       join e in db.KNL_PhanXuong
                       on a.IDPhanXuong equals e.ID into ul
                       from e in ul.DefaultIfEmpty()
                       join f in db.KNL_Khoi
                       on a.IDKhoi equals f.ID into ulis
                       from f in ulis.DefaultIfEmpty()
                       select new NhomKNLValidation
                       {
                           IDPhongBan = a.IDPhongBan,
                           TenPhongBan = d.TenPhongBan,
                           IDNhom = a.IDNhom,
                           MaNhom = a.MaNhom,
                           TenNhom = a.TenNhom,
                           IDPhanXuong = a.IDPhanXuong,
                           TenPhanXuong = e.TenPX,
                           IDKhoi = a.IDKhoi,
                           TenKhoi = f.TenKhoi
                       }).ToList();
            NhomKNLValidation DO = new NhomKNLValidation();

            if (res.Count > 0)
            {
                foreach (var c in res)
                {
                    DO.IDPhongBan = c.IDPhongBan;
                    DO.MaNhom = c.MaNhom;
                    DO.TenPhongBan = c.TenPhongBan;
                    DO.TenNhom = c.TenNhom;
                    DO.IDNhom = c.IDNhom;
                    DO.IDPhanXuong = c.IDPhanXuong;
                    DO.TenPhanXuong = c.TenPhanXuong;
                    DO.IDKhoi = c.IDKhoi;
                    DO.TenKhoi = c.TenKhoi;
                }

                db.Configuration.ProxyCreationEnabled = false;
                List<PhongBan> dt = db.PhongBans.ToList();
                ViewBag.IDPhongBan = new SelectList(dt, "IDPhongBan", "TenPhongBan", DO.IDPhongBan);
                List<KNL_PhanXuong> px = db.KNL_PhanXuong.Where(x=>x.IDPhongBan ==DO.IDPhongBan).ToList();
                ViewBag.IDPhanXuong = new SelectList(px, "ID", "TenPX",DO.IDPhanXuong);

                List<KNL_Khoi> khoi = db.KNL_Khoi.ToList();
                ViewBag.IDKhoi = new SelectList(khoi, "ID", "TenKhoi", DO.IDKhoi);

            }
            else
            {
                return HttpNotFound();
            }
            return PartialView(DO);
        }
        [HttpPost]
        public ActionResult Edit(NhomKNLValidation _DO)
        {
            try
            {

                db.KNLNhom_update(_DO.IDNhom, _DO.TenNhom, _DO.MaNhom, (int?)_DO.IDPhongBan, (int?)_DO.IDPhanXuong,_DO.IDKhoi);

                TempData["msgSuccess"] = "<script>alert('Cập nhập thành công');</script>";
            }
            catch (Exception e)
            {

                TempData["msgSuccess"] = "<script>alert('Cập nhập thất bại " + e.Message + " ');</script>";
            }

            return RedirectToAction("Index", "FGroup");
        }
        public ActionResult Delete(int id)
        {
            try
            {
                var a = db.ViTriKNLs.Where(x => x.IDNhom == id).ToList();
                if (a.Count == 0) db.KNLNhom_delete(id);
                else TempData["msgError"] = "<script>alert('Xóa dữ liệu thất bại');</script>";
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Xóa dữ liệu thất bại');</script>";
            }
            return RedirectToAction("Index", "FGroup");
        }
        public JsonResult GetPX(int? IDPB, int? IDKhoi)
        {
            if (IDKhoi == null) IDKhoi = 0;
            if (IDPB == null) IDPB = 0;
            List<PhanXuongValidation> px = (from b in db.KNL_PhanXuong.Where(x=>x.IDPhongBan ==IDPB)
                                 select new PhanXuongValidation()
                                 {
                                    TenPX =b.TenPX,
                                    MaPX =b.TenPX,
                                    ID =b.ID,
                                    IDPhongBan =b.IDPhongBan,
                                    IDKhoi =b.IDKhoi
                                 }).ToList();
            if (IDKhoi != 0) px=px.Where(x => x.IDKhoi == IDKhoi).ToList();
            //if (IDPB != 0) px = px.Where(x => x.IDPhongBan == IDPB).ToList();
            return Json(px, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetNhom(int? IDPB,int? IDKhoi,int? IDPX)
        {
            if (IDKhoi == null) IDKhoi = 0;
            if (IDPX == null) IDPX = 0;
            if (IDPB == null) IDPB = 0;
            List<NhomKNLValidation> px = (from b in db.KNL_Nhom.Where(x=>x.IDPhongBan ==IDPB)
                                            select new NhomKNLValidation()
                                            {
                                                TenNhom =b.TenNhom,
                                                MaNhom = b.MaNhom,
                                                IDNhom = b.IDNhom,
                                                IDPhongBan = b.IDPhongBan,
                                                IDPhanXuong =b.IDPhanXuong,
                                                IDKhoi = b.IDKhoi
                                            }).ToList();
            //if (IDPB != 0) px = px.Where(x => x.IDPhongBan == IDPB).ToList();
            if (IDKhoi != 0) px = px.Where(x => x.IDKhoi == IDKhoi).ToList();
            if (IDPX != 0) px = px.Where(x => x.IDPhanXuong == IDPX).ToList();
            if (IDPB != 0 && IDPX == 0 && IDKhoi == 0) px = px.Where(x => x.IDPhongBan == IDPB && x.IDPhanXuong == null && x.IDKhoi == null).ToList();
            return Json(px, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetTo(int? IDPX,int? IDKhoi,int? IDPB)
        {
            if(IDPX == null)IDPX = 0;
            if (IDKhoi == null) IDKhoi = 0;
            if (IDPB == null) IDPB = 0;
            List<ToKNLValidation> px = (from b in db.KNL_To.Where(x=>x.IDPhongBan ==IDPB)
                                          select new ToKNLValidation()
                                          {
                                              MaTo = b.MaTo,
                                              IDKhoi = b.IDKhoi,
                                              IDPhongBan = b.IDPhongBan,
                                              IDPhanXuong = b.IDPhanXuong,
                                              IDTo =b.IDTo,
                                              TenTo =b.TenTo,
                                          }).ToList();
            if (IDPX != 0) px = px.Where(x => x.IDPhanXuong == IDPX).ToList();
            if (IDKhoi != 0) px = px.Where(x => x.IDKhoi == IDKhoi).ToList();
            //if (IDPB != 0) px = px.Where(x => x.IDPhongBan == IDPB).ToList();
            if (IDPB != 0 && IDPX ==0 && IDKhoi ==0) px = px.Where(x => x.IDPhongBan == IDPB && x.IDPhanXuong == null && x.IDKhoi ==null).ToList();
            //if (IDPB != 0 && IDPX == 0 && IDKhoi == 0) px = px.Where(x => x.IDPhongBan == IDPB && x.IDPhanXuong == null && x.IDKhoi == null).ToList();
            return Json(px, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetVTKNL(int? IDPX, int? IDKhoi, int? IDPB,int? IDNhom, int? IDTo)
        {
            if (IDPX == null) IDPX = 0;
            if (IDKhoi == null) IDKhoi = 0;
            if (IDPB == null) IDPB = 0;
            if (IDNhom == null) IDNhom = 0;
            if (IDTo == null) IDTo = 0;
            List<ViTriKNLValidation> px = (from b in db.ViTriKNLs
                                        select new ViTriKNLValidation()
                                        {
                                            IDVT =b.IDVT,
                                            MaViTri =b.MaViTri,
                                            TenViTri =b.TenViTri,
                                            IDPB =b.IDPB,
                                            IDKhoi =b.IDKhoi,
                                            IDNhom =b.IDNhom,
                                            IDPX =b.IDPX,
                                            IDTo =b.IDTo
                                        }).ToList();
            if (IDPX != 0) px = px.Where(x => x.IDPX == IDPX).ToList();
            if (IDKhoi != 0) px = px.Where(x => x.IDKhoi == IDKhoi).ToList();
            if (IDPB != 0) px = px.Where(x => x.IDPB == IDPB).ToList();
            if (IDNhom != 0) px = px.Where(x => x.IDNhom == IDNhom).ToList();
            if (IDTo != 0) px = px.Where(x => x.IDTo == IDTo).ToList();
            return Json(px, JsonRequestBehavior.AllowGet);
        }


    }
}