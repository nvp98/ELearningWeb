using E_Learning.Models;
using E_Learning.ModelsQTTC;
using OfficeOpenXml;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace E_Learning.Controllers.QTTC
{
    public class FGroupQTTCController : Controller
    {
        ELEARNINGEntities db = new ELEARNINGEntities();
        int Idquyen = MyAuthentication.IDQuyen;
        String ControllerName = "FGroupQTTC";
        // GET: FGroupQTTC
        public ActionResult Index(int? page, string search, int? IDDVTC)
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
            if (IDDVTC == null) IDDVTC = 0;

            var res = (from a in db.DV_CapC7
                       select new DVTCValidation
                       {
                           IDDVTC = a.ID,
                           MaDVTC = a.MaDVTC,
                           TenDVTC = a.TenDVTC,
                           TrangThai = (long)a.TrangThai
                       }).ToList();

            List<DV_CapC7> dt = db.DV_CapC7.ToList();
            ViewBag.IDDVTC = new SelectList(dt, "ID", "TenDVTC");
            if (IDDVTC != 0) res = res.Where(x => x.IDDVTC == IDDVTC).ToList();
            if (page == null) page = 1;
            int pageSize = res.Count();
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

            List<DV_CapC7> dt = db.DV_CapC7.ToList();
            ViewBag.IDDVTC = new SelectList(dt, "ID", "TenDVTC");

            return PartialView();
        }
        public int GetIDDVTC(string TenDVTC)
        {
            var model = db.DV_CapC7.Where(x => x.TenDVTC == TenDVTC).SingleOrDefault();
            if (model == null)
                return 0;
            return model.ID;
        }

        [HttpPost]
        public ActionResult Create(DVTCValidation _DO)
        {
            try
            {
                if (_DO.TenDVTC != null && GetIDDVTC(_DO.TenDVTC.Trim()) == 0)
                {
                    var aa = db.Cap7_insert_KNL(_DO.MaDVTC, _DO.TenDVTC);
                }

                TempData["msgSuccess"] = "<script>alert('Thêm mới thành công');</script>";
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Có lỗi khi thêm mới: " + e.Message + "');</script>";
            }

            return RedirectToAction("Index", "FGroupQTTC");
        }
        public ActionResult Edit(int id)
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            if (!ListQuyen.Contains(CONSTKEY.EDIT))
            {
                TempData["msgError"] = "<script>alert('Bạn không có quyền thực hiện chức năng này');</script>";
                return RedirectToAction("", "Home");
            }
            var res = (from a in db.DV_CapC7.Where(x => x.ID == id)
                       select new DVTCValidation
                       {
                           IDDVTC = a.ID,
                           MaDVTC = a.MaDVTC,
                           TenDVTC = a.TenDVTC,
                           TrangThai = (long)a.TrangThai
                       }).ToList();
            DVTCValidation DO = new DVTCValidation();

            if (res.Count > 0)
            {
                foreach (var c in res)
                {
                    DO.IDDVTC = c.IDDVTC;
                    DO.MaDVTC = c.MaDVTC;
                    DO.TenDVTC = c.TenDVTC;
                    DO.TrangThai = c.TrangThai;
                }

                db.Configuration.ProxyCreationEnabled = false;
                List<DV_CapC7> dt = db.DV_CapC7.ToList();
            }
            else
            {
                return HttpNotFound();
            }
            return PartialView(DO);
        }
        [HttpPost]
        public ActionResult Edit(DVTCValidation _DO)
        {
            try
            {

                db.Cap7_update_KNL(_DO.IDDVTC, _DO.MaDVTC, _DO.TenDVTC, 1);

                TempData["msgSuccess"] = "<script>alert('Cập nhật thành công');</script>";

            }
            catch (Exception e)
            {

                TempData["msgSuccess"] = "<script>alert('Cập nhật thất bại " + e.Message + " ');</script>";
            }

            return RedirectToAction("Index", "FGroupQTTC");
        }
        public ActionResult Delete(int id)
        {
            try
            {
                db.Cap7_delete_KNL(id);
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Xóa dữ liệu thất bại');</script>";
            }
            return RedirectToAction("Index", "FGroupQTTC");
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
            return RedirectToAction("Index", "FGroupQTTC");
        }
        [HttpPost]
        public ActionResult ImportExcel(ImportExcelModel fileObj)
        {
            db.Configuration.ProxyCreationEnabled = false;

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            if (fileObj.FileExcel == null)
            {
                return Json(new { message = "Vui lòng chọn file Excel!" });
            }

            try
            {
                using (var stream = new MemoryStream())
                {
                    fileObj.FileExcel.InputStream.CopyTo(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                        int rowCount = worksheet.Dimension.Rows;

                        {
                            for (int row = 2; row <= rowCount; row++) // Skip header
                            {
                                string maDVTC = worksheet.Cells[row, 2].Value?.ToString().Trim();
                                string tenDVTC = worksheet.Cells[row, 3].Value?.ToString().Trim();

                                db.Cap7_insert_KNL(maDVTC, tenDVTC);
                            }
                        }
                    }
                }

                return Json(new { message = "Import dữ liệu thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { message = "Lỗi import: " + ex.Message });
            }
        }
    }
}