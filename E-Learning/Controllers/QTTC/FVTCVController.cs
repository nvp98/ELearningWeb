using E_Learning.Models;
using E_Learning.ModelsQTTC;
using OfficeOpenXml;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_Learning.Controllers.QTTC
{
    public class FVTCVController : Controller
    {
        ELEARNINGEntities db = new ELEARNINGEntities();
        int Idquyen = MyAuthentication.IDQuyen;
        String ControllerName = "FVTCV";
        // GET: FVTCV
        public ActionResult Index(int? page, string search, int? IDVTCV)
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
            if (IDVTCV == null) IDVTCV = 0;

            var res = (from a in db.DV_ViTri
                       select new VTCVValidation
                       {
                           IDVTCV = a.IDVT,
                           MaVTCV = a.MaViTri,
                           TenVTCV = a.TenViTri,
                           CapQuanLy = a.CapQuanLy,
                           TrangThai = (long)a.TrangThai
                       }).ToList();

            List<DV_ViTri> dt = db.DV_ViTri.ToList();
            ViewBag.IDVTCV = new SelectList(dt, "IDVT", "TenViTri");
            if (IDVTCV != 0) res = res.Where(x => x.IDVTCV == IDVTCV).ToList();
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

            List<DV_ViTri> dt = db.DV_ViTri.ToList();
            ViewBag.IDVTCV = new SelectList(dt, "IDVT", "TenViTri");

            return PartialView();
        }

        public int GetIDDVTC(string TenVTCV)
        {
            if (string.IsNullOrWhiteSpace(TenVTCV))
                return 0;

            var model = db.DV_ViTri.FirstOrDefault(x => x.TenViTri == TenVTCV);

            return model?.IDVT ?? 0;
        }

        [HttpPost]
        public ActionResult Create(VTCVValidation _DO)
        {
            List<DV_CapC2> dt = db.DV_CapC2.ToList();
            try
            {
                if (!string.IsNullOrWhiteSpace(_DO.MaVTCV) && !string.IsNullOrWhiteSpace(_DO.TenVTCV) && GetIDDVTC(_DO.TenVTCV.Trim()) == 0)
                {
                    if (!dt.Any(d => d.MaDVTC == _DO.MaVTCV))
                    {
                        db.DV_ViTri_Insert(_DO.MaVTCV.Trim(), _DO.TenVTCV.Trim(), _DO.CapQuanLy, 1);
                    }
                    //else
                    //{
                    //    int id = dt.Find(item => item.MaDVTC == _DO.MaVTCV).ID;
                    //    db.Cap2_update_KNL(id, _DO.MaVTCV.Trim(), _DO.TenVTCV.Trim(), 1);
                    //}
                    TempData["msgSuccess"] = "<script>alert('Thêm mới thành công');</script>";
                }
                else
                {
                    TempData["msgSuccess"] = "<script>alert('Mã VTCV và Tên VTCV không được để trống');</script>";
                }
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Có lỗi khi thêm mới: " + e.Message + "');</script>";
            }

            return RedirectToAction("Index", "FVTCV");
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

            List<DV_ViTri> dt = db.DV_ViTri.ToList();

            try
            {
                using (var stream = new MemoryStream())
                {
                    fileObj.FileExcel.InputStream.CopyTo(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[11];
                        int rowCount = worksheet.Dimension.Rows;

                        {
                            for (int row = 2; row <= rowCount; row++) // Skip header
                            {
                                string maVTCV = worksheet.Cells[row, 2].Value?.ToString().Trim();
                                string tenVTCV = worksheet.Cells[row, 3].Value?.ToString().Trim();
                                string capQuanLy = worksheet.Cells[row, 4].Value?.ToString().Trim();
                                string trangThaiInFile= worksheet.Cells[row, 5].Value?.ToString().Trim();
                                int trangThaiInDb = trangThaiInFile.Equals("Trong định biên") ? 1 : (trangThaiInFile.Equals("Ngoài định biên") ? 2 : 0);

                                if (!dt.Any(d => d.MaViTri == maVTCV))
                                {
                                    db.DV_ViTri_Insert(maVTCV, tenVTCV, capQuanLy, trangThaiInDb);
                                }
                                //else
                                //{
                                //    int id = dt.Find(item => item.MaDVTC == maVTCV).ID;
                                //    db.Cap2_update_KNL(id, maVTCV, tenVTCV, 1);
                                //}
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