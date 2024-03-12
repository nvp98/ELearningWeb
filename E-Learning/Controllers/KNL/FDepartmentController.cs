using DocumentFormat.OpenXml.Wordprocessing;
using E_Learning.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using static E_Learning.Controllers.FDepartmentController;

namespace E_Learning.Controllers
{
    public class FDepartmentController : Controller
    {
        ELEARNINGEntities db = new ELEARNINGEntities();
        int Idquyen = MyAuthentication.IDQuyen;
        String ControllerName = "FDepartment";
        // GET: FDepartment
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

            var res = (from a in db.PhongBans
                       select new PhongBanValidation
                       {
                           IDPhongBan = a.IDPhongBan,
                           TenPhongBan = a.TenPhongBan,
                           MaPB = a.MaPB
                       }).ToList();

            List<PhongBan> dt = db.PhongBans.ToList();
            ViewBag.IDPB = new SelectList(dt, "IDPhongBan", "TenPhongBan");
            if (IDPB != 0) res = res.Where(x => x.IDPhongBan == IDPB).ToList();
            if (page == null) page = 1;
            int pageSize = res.Count();
            int pageNumber = (page ?? 1);
            return View(res.ToList().ToPagedList(pageNumber, pageSize));
            //return View(res.ToList());
        }


        public ActionResult DataTable(int? IDPB)
        {
            List<PhongBan> dt = db.PhongBans.ToList();
            ViewBag.IDPB = new SelectList(dt, "IDPhongBan", "TenPhongBan");
            return View();
            //return View(res.ToList());
        }

        public ActionResult GetData(JqueryDatatableParam param,int? IDPB)
        {
            var employees = (from a in db.NhanViens
                             select new EmployeeValidation
                             {
                                 ID = a.ID,
                                 HoTen = a.HoTen,
                                 MaNV = a.MaNV,
                                 IDPhongBan =(int)a.IDPhongBan
                                 //PhongBan = a.TenPhongBan,
                                 //TenQuyen = a.TenQuyen
                             }).ToList();
            if (IDPB != null) employees = employees.Where(x => x.IDPhongBan == IDPB).ToList();
            //employees.ToList().ForEach(x => x. = x.StartDate.ToString("dd'/'MM'/'yyyy"));

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                employees = employees.Where(x => x.HoTen.ToLower().Contains(param.sSearch.ToLower())
                                              || x.MaNV.ToLower().Contains(param.sSearch.ToLower()) ).ToList();
            }

            var sortColumnIndex = Convert.ToInt32(HttpContext.Request.QueryString["iSortCol_0"]);
            var sortDirection = HttpContext.Request.QueryString["sSortDir_0"];

            if (sortColumnIndex == 0)
            {
                employees = sortDirection == "asc" ? employees.OrderBy(c => c.HoTen).ToList() : employees.OrderByDescending(c => c.HoTen).ToList();
            }
            else if (sortColumnIndex == 1)
            {
                employees = sortDirection == "asc" ? employees.OrderBy(c => c.MaNV).ToList() : employees.OrderByDescending(c => c.MaNV).ToList();
            }
            else if (sortColumnIndex == 5)
            {
                employees = sortDirection == "asc" ? employees.OrderBy(c => c.IDPhongBan).ToList() : employees.OrderByDescending(c => c.IDPhongBan).ToList();
            }
            //else
            //{
            //    Func<NhanVien, string> orderingFunction = e => sortColumnIndex == 0 ? e.HoTen :
            //                                                   sortColumnIndex == 1 ? e.MaNV :
            //                                                   e.DiaChi;

            //    //employees = sortDirection == "asc" ? employees.OrderBy(orderingFunction).ToList() : employees.OrderByDescending(orderingFunction).ToList();
            //}

            var displayResult = employees.Skip(param.iDisplayStart)
             .Take(param.iDisplayLength).ToList();
            var totalRecords = employees.Count();

            var jsonResult = Json(new
            {
                param.sEcho,
                iTotalRecords = totalRecords,
                iTotalDisplayRecords = totalRecords,
                aaData = displayResult,
            }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
            //return Json(new
            //{
            //    param.sEcho,
            //    iTotalRecords = totalRecords,
            //    iTotalDisplayRecords = totalRecords,
            //    aaData = displayResult,
            //}, JsonRequestBehavior.AllowGet);

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
            ViewBag.IDPB = new SelectList(dt, "IDPhongBan", "TenPhongBan");

            return PartialView();
        }
        public int GetIDPB(string TenPB)
        {
            var model = db.PhongBans.Where(x => x.TenPhongBan == TenPB).SingleOrDefault();
            if (model == null)
                return 0;
            return model.IDPhongBan;
        }



        [HttpPost]
        public ActionResult Create(PhongBanValidation _DO, FormCollection collection)
        {
            try
            {
                if (_DO.TenPhongBan != null && GetIDPB(_DO.TenPhongBan.Trim()) == 0)
                {
                    var aa = db.PhongBan_insert_KNL(_DO.TenPhongBan, _DO.MaPB);
                }
                //var ListVT = new List<ViTriKNLValidation>();
                //foreach (var key in collection.AllKeys)
                //{
                //    if (key.Split('_')[0] == "TenVTKNL")
                //    {
                //        ListVT.Add(new ViTriKNLValidation() { TenViTri = collection[key], MaViTri = collection["MaViTri_" + key.Split('_')[1]] });
                //    }
                //}
                //foreach (var item in ListVT)
                //{
                //    int idvt = GetIDVTKNL(item.TenViTri, _DO.IDPB);
                //    if (idvt == 0)
                //    {
                //        var k = db.VitriKNL_insert(item.TenViTri, _DO.MaViTri, _DO.IDPB);
                //    }
                //    else
                //    {
                //        var k = db.VitriKNL_update(idvt, item.TenViTri, _DO.MaViTri, _DO.IDPB);
                //    }
                //}
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
            return RedirectToAction("Index", "FDepartment");
        }
        public ActionResult Edit(int id)
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            if (!ListQuyen.Contains(CONSTKEY.EDIT))
            {
                TempData["msgError"] = "<script>alert('Bạn không có quyền thực hiện chức năng này');</script>";
                return RedirectToAction("", "Home");
            }
            var res = (from a in db.PhongBans.Where(x => x.IDPhongBan == id)
                       select new PhongBanValidation
                       {
                           IDPhongBan = a.IDPhongBan,
                           TenPhongBan = a.TenPhongBan,
                           MaPB = a.MaPB
                       }).ToList();
            PhongBanValidation DO = new PhongBanValidation();

            if (res.Count > 0)
            {
                foreach (var c in res)
                {
                    DO.IDPhongBan = c.IDPhongBan;
                    DO.MaPB = c.MaPB;
                    DO.TenPhongBan = c.TenPhongBan;
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
        public ActionResult Edit(PhongBanValidation _DO)
        {
            try
            {

                db.PhongBan_update_KNL(_DO.IDPhongBan, _DO.TenPhongBan, _DO.MaPB);

                TempData["msgSuccess"] = "<script>alert('Cập nhập thành công');</script>";
            }
            catch (Exception e)
            {

                TempData["msgSuccess"] = "<script>alert('Cập nhập thất bại " + e.Message + " ');</script>";
            }

            return RedirectToAction("Index", "FDepartment");
        }
        public ActionResult Delete(int id)
        {
            try
            {
                db.PhongBan_delete_KNL(id);
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Xóa dữ liệu thất bại');</script>";
            }
            return RedirectToAction("Index", "FDepartment");
        }

        public int GetIDController(string TenController)
        {
            var model = db.ListControllers.Where(x => x.Controller == TenController).SingleOrDefault();
            if (model == null)
                return 0;
            return model.ID;
        }
        public ActionResult Sync()
        {
            try
            {
                Assembly asm = Assembly.GetExecutingAssembly();

                var controllerTypes = from t in asm.GetExportedTypes()
                                      where typeof(IController).IsAssignableFrom(t)
                                      select t;

                foreach (var item in controllerTypes)
                {
                    int index = item.Name.IndexOf("Controller");
                    if (index != -1)
                    {
                        var name = item.Name.Remove(index);
                        int IDC = GetIDController(name);
                        if (IDC == 0)
                        {
                            db.Controller_insert(name, null, 1);
                        }
                    }
                }
                TempData["msgSuccess"] = "<script>alert('Đồng bộ dữ liệu thành công');</script>";
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Có lỗi khi đồng bộ dữ liệu: " + e.Message + "');</script>";
            }
            return RedirectToAction("Index", "FDepartment");
        }
    }
}