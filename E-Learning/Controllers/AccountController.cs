using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using E_Learning.Models;
using PagedList;
using E_Learning.Common;
using System.Text.RegularExpressions;

namespace E_Learning.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        ELEARNINGEntities db = new ELEARNINGEntities();
        int Idquyen = MyAuthentication.IDQuyen;
        String ControllerName = "Account";
        public ActionResult Index(int? page,string search)
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

            var res = (from a in db.TaiKhoan_select(search)
                      select new EmployeeValidation
                      {
                          ID = a.ID,
                          HoTen = a.HoTen,
                          MaNV = a.MaNV,
                          PhongBan = a.TenPhongBan,
                          TenQuyen = a.TenQuyen
                      }).OrderBy(x=>x.ID);

            if (page == null) page = 1;
            int pageSize = 50;
            int pageNumber = (page ?? 1);
            return View(res.ToList().ToPagedList(pageNumber, pageSize));
           
        }

        public ActionResult GetData(JqueryDatatableParam param, int? IDPB)
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            if (!ListQuyen.Contains(CONSTKEY.V))
            {
                return Json("Unauthorized", JsonRequestBehavior.AllowGet);
            }
            string  search = "";
            var employees = (from a in db.TaiKhoan_select(search)
                             select new EmployeeValidation
                             {
                                 ID = a.ID,
                                 HoTen = a.HoTen,
                                 MaNV = a.MaNV,
                                 PhongBan = a.TenPhongBan,
                                 TenQuyen =a.TenQuyen,
                                 IDPhongBan =(int)a.IDPhongBan

                                 //PhongBan = a.TenPhongBan,
                                 //TenQuyen = a.TenQuyen
                             }).ToList();
            //if(IDPB != null) employees = employees.Where(x=>x.IDPhongBan == IDPB).ToList();
            //employees.ToList().ForEach(x => x. = x.StartDate.ToString("dd'/'MM'/'yyyy"));

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                employees = employees.Where(x => x.HoTen.ToLower().Contains(param.sSearch.ToLower())
                                              || x.MaNV.ToLower().Contains(param.sSearch.ToLower())).ToList();
            }

            var sortColumnIndex = Convert.ToInt32(HttpContext.Request.QueryString["iSortCol_0"]);
            var sortDirection = HttpContext.Request.QueryString["sSortDir_0"];

            if (sortColumnIndex == 2)
            {
                employees = sortDirection == "asc" ? employees.OrderBy(c => c.MaNV).ToList() : employees.OrderByDescending(c => c.MaNV).ToList();
            }
            else if (sortColumnIndex == 3)
            {
                employees = sortDirection == "asc" ? employees.OrderBy(c => c.HoTen).ToList() : employees.OrderByDescending(c => c.HoTen).ToList();
            }
            else if (sortColumnIndex == 4)
            {
                employees = sortDirection == "asc" ? employees.OrderBy(c => c.IDPhongBan).ToList() : employees.OrderByDescending(c => c.IDPhongBan).ToList();
            }
            else if(sortColumnIndex == 5)
            {
                employees = sortDirection == "asc" ? employees.OrderBy(c => c.TenQuyen).ToList() : employees.OrderByDescending(c => c.TenQuyen).ToList();
            }
            else
            {
                employees = sortDirection == "asc" ? employees.OrderBy(c => c.MaNV).ToList() : employees.OrderByDescending(c => c.MaNV).ToList();
            }

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


        public ActionResult Resetpass(int id)
        {
            var res = (from dm in db.TaiKhoan_ID(id)
                       select new LoginValidation
                       {
                           ID = dm.ID,
                           HoTen=dm.HoTen
                       }).ToList();
            LoginValidation DO = new LoginValidation();
            if (res.Count > 0)
            {
                foreach (var dm in res)
                {
                    DO.ID = dm.ID;
                    DO.HoTen = dm.HoTen;
                }
            }
            else
            {
                return HttpNotFound();
            }
            return PartialView(DO);
        }

        [HttpPost]
        public ActionResult Resetpass(LoginValidation _DO)
        {
            try
            {
                db.TaiKhoan_Update(_DO.ID, Encryptor.MD5Hash(_DO.MatKhau));
                TempData["msgSuccess"] = "<script>alert('Cập nhập thành công');</script>";
            }
            catch (Exception e)
            {
                TempData["msgSuccess"] = "<script>alert('Cập nhập thất bại: " + e.Message + "');</script>";
            }
            return RedirectToAction("Index", "Account");
        }

        public ActionResult ResetListPass()
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            if (!ListQuyen.Contains(CONSTKEY.R_PASS))
            {
                TempData["msgError"] = "<script>alert('Bạn không có quyền thực hiện chức năng này');</script>";
                return RedirectToAction("", "Home");
            }
            //db.Configuration.ProxyCreationEnabled = false;
            return PartialView();
        }

        [HttpPost]
        public ActionResult ResetListPass(LoginValidation _DO)
        {
            try
            {
                if (!String.IsNullOrEmpty(_DO.MaNV))
                {
                    //Regex.Replace(_DO.NVDG, @"[^0-9a-zA-Z]+", " ");
                    string tx = Regex.Replace(_DO.MaNV, @"[^0-9a-zA-Z]+", " ");
                    string[] NVS = tx.Split(new char[] { ' ' });
                    foreach (var item in NVS)
                    {
                        var aa = db.NhanViens.Where(x => x.MaNV == item).FirstOrDefault();
                        if (aa != null)
                        {
                            db.TaiKhoan_Update(aa.ID, Encryptor.MD5Hash(_DO.MatKhau));
                        }
                    }
                }
                //db.TaiKhoan_Update(_DO.ID, Encryptor.MD5Hash(_DO.MatKhau));
                TempData["msgSuccess"] = "<script>alert('Cập nhập thành công');</script>";
            }
            catch (Exception e)
            {
                TempData["msgSuccess"] = "<script>alert('Cập nhập thất bại: " + e.Message + "');</script>";
            }
            return RedirectToAction("Index", "Account");
        }

        public ActionResult Edit(int id)
        {
            List<Quyen> pq = db.Quyens.ToList();
            ViewBag.PList = new SelectList(pq, "IDQuyen", "TenQuyen");

            List<KNL_Quyen> quyen = db.KNL_Quyen.ToList();
            ViewBag.QuyenList = new SelectList(quyen, "IDQuyen", "TenQuyen");

            List<PhongBan> dn = db.PhongBans.ToList();
            ViewBag.DNList = new SelectList(dn, "IDDN", "TenDN");

            var res = (from a in db.TaiKhoan_ID(id)
                       select new LoginValidation
                       {
                           ID = a.ID,
                           MaNV = a.MaNV,
                           HoTen = a.HoTen,
                           IDQuyen = (int)a.IDQuyen,
                           IDQuyenKNL = (int?)a.IDQuyenKNL ?? 0
                       }).ToList();
            LoginValidation DO = new LoginValidation();
            if (res.Count > 0)
            {
                foreach (var nv in res)
                {
                    DO.ID = nv.ID;
                    DO.MaNV = nv.MaNV;
                    DO.HoTen = nv.HoTen;
                    DO.IDQuyen = nv.IDQuyen;
                    DO.IDQuyenKNL = nv.IDQuyenKNL;
                }
            }
            else
            {
                return HttpNotFound();
            }
            return PartialView(DO);
        }

        [HttpPost]
        public ActionResult Edit(LoginValidation _DO)
        {
            try
            {
                //db.Nhanvien_update_QuyenKNL(_DO.MaNV, _DO.IDQuyenKNL);
                db.TK_Update_Quyen(_DO.ID, _DO.IDQuyen);
                TempData["msgSuccess"] = "<script>alert('Cập nhập thành công');</script>";
            }
            catch (Exception e)
            {
                TempData["msgSuccess"] = "<script>alert('Cập nhập thất bại:" + e.Message + "');</script>";
            }

            return RedirectToAction("Index", "Account");
        }

    }
}