using ClosedXML.Excel;
using E_Learning.Models;
using ExcelDataReader;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using DocumentFormat.OpenXml.Wordprocessing;

namespace E_Learning.Controllers.DinhBien
{
    public class DinhBienController : Controller
    {
        ELEARNINGEntities db = new ELEARNINGEntities();
        ELEARNINGEntities dbKCCCD = new ELEARNINGEntities();
        int Idquyen = MyAuthentication.IDQuyen;
        String ControllerName = "DinhBien";
        // GET: DinhBien
        public ActionResult Index(int? page, string search, int? IDPB, int? IDPX, int? IDNhom, int? IDKhoi, int? IDTo)
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            ViewBag.QUYENCN = ListQuyen;
            //if (!ListQuyen.Contains(CONSTKEY.V))
            //{
            //    TempData["msgError"] = "<script>alert('Bạn không có quyền truy cập chức năng này');</script>";
            //    return RedirectToAction("", "Home");
            //}
            if (search == null) search = "";
            ViewBag.search = search;
            int idpb = MyAuthentication.IDPhongban;
            var manv = MyAuthentication.Username;
            
            List<KNL_Khoi> khoi = db.KNL_Khoi.ToList();
            ViewBag.IDKhoi = new SelectList(khoi, "ID", "TenKhoi");
            List<KNL_PhanXuong> px = db.KNL_PhanXuong.ToList();
            ViewBag.IDPX = new SelectList(px, "ID", "TenPX");
            if(IDPB == null) IDPB = 0;
            var listDB = db.DinhBienVTs.ToList();
            var res = (from a in db.VitriKNL_Select(IDPB)
                       join b in listDB
                       on a.IDVT equals b.IDVT into ul
                       from b in ul.DefaultIfEmpty()
                       select new DinhBienView
                       {
                           IDVT = a.IDVT,
                           TenViTri = a.TenViTri + "-" + a.TenNhom + "-" + a.TenTo + "-" + a.TenPX + "-" + a.TenPhongBan,
                           TenViTriSe =a.TenViTri??"",
                           IDPB = a.IDPhongBan,
                           TenPhongBan = a.TenPhongBan,
                           MaViTri = a.MaViTri,
                           IDKhoi = a.IDKhoi,
                           TenKhoi = a.TenKhoi,
                           IDPX = a.IDPX,
                           TenPX = a.TenPX,
                           IDNhom = a.IDNhom,
                           TenNhom = a.TenNhom,
                           IDTo = a.IDTo,
                           TenTo = a.TenTo,
                           FilePath = a.FilePath,
                           CountNV = a.SLNV == null ? 0 : a.SLNV,
                           CountKNL = a.SLNL,
                           DinhBienNS =b?.SLDinhBien,
                           NSTapSu = b?.NSTapSu == 1 ? "X" : "",
                           ChuaCoNS = b?.NSTrong == 1 ? "X" : "",
                           thoigianBoNhiem = b?.TGBoNhiem ?? default,
                           ghichu = b?.GhiChu,
                           NgayTao = b?.NgayTao ?? default,
                           ThuaThieu = b?.SLDinhBien != null && a?.SLNV != null ? (int)a?.SLNV - (int)b?.SLDinhBien : b?.SLDinhBien != null && a?.SLNV == null? 0 - (int)b?.SLDinhBien:  0,
                           NSVTKhac = b?.NSVTKhac ==1 ? "X":"",
                           IDDB = b?.ID,
                       }).Where(x=>x.DinhBienNS != null).OrderBy(x => x.IDPB).ToList();

            var listBP = (from a in listDB
                          join b in db.ViTriKNLs on a.IDVT equals b.IDVT
                          select new DinhBienView
                          {
                              IDPB = b.IDPB,
                          }).ToList().Select(a=>a.IDPB);
            List<PhongBan> dt = db.PhongBans.Where(s=>listBP.Contains(s.IDPhongBan)).ToList();
            //if (ListQuyen.Contains(CONSTKEY.V_BP)) dt = dt.Where(x => x.IDPhongBan == idpb).ToList();
            ViewBag.IDPB = new SelectList(dt, "IDPhongBan", "TenPhongBan");

            if (IDPB != 0) res = res.Where(x => x.IDPB == IDPB).ToList();
            if (IDPX != null) res = res.Where(x => x.IDPX == IDPX).ToList();
            if (IDKhoi != null) res = res.Where(x => x.IDKhoi == IDKhoi).ToList();
            if (IDNhom != null) res = res.Where(x => x.IDNhom == IDNhom).ToList();
            if (IDTo != null) res = res.Where(x => x.IDTo == IDTo).ToList();
            if (!String.IsNullOrEmpty(search)) res = res.Where(x => x.MaViTri.ToLower().Contains(search.ToLower())|| x.TenViTriSe.ToLower().Contains(search.ToLower())).ToList();

            if (ListQuyen.Contains(CONSTKEY.V_BP)) res = res.Where(x => x.IDPB == idpb).ToList();

            if (page == null) page = 1;
            int pageSize = 50;
            int pageNumber = (page ?? 1);
            return View(res.ToList().ToPagedList(pageNumber, pageSize));
        }
        public ActionResult Edit(int id)
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            if (!ListQuyen.Contains(CONSTKEY.EDIT))
            {
                TempData["msgError"] = "<script>alert('Bạn không có quyền thực hiện chức năng này');</script>";
                return RedirectToAction("", "Home");
            }
            var listDB = db.DinhBienVTs.ToList();
            var res = (from a in db.VitriKNL_Select(0).Where(x=>x.IDVT == id)
                       join b in listDB
                       on a.IDVT equals b.IDVT into ul
                       from b in ul.DefaultIfEmpty()
                       select new DinhBienView
                       {
                           IDVT = a.IDVT,
                           TenViTri = a.TenViTri + "-" + a.TenNhom + "-" + a.TenTo + "-" + a.TenPX + "-" + a.TenPhongBan,
                           TenViTriSe = a.TenViTri ?? "",
                           IDPB = a.IDPhongBan,
                           TenPhongBan = a.TenPhongBan,
                           MaViTri = a.MaViTri,
                           IDKhoi = a.IDKhoi,
                           TenKhoi = a.TenKhoi,
                           IDPX = a.IDPX,
                           TenPX = a.TenPX,
                           IDNhom = a.IDNhom,
                           TenNhom = a.TenNhom,
                           IDTo = a.IDTo,
                           TenTo = a.TenTo,
                           FilePath = a.FilePath,
                           CountNV = a.SLNV == null ? 0 : a.SLNV,
                           CountKNL = a.SLNL,
                           DinhBienNS = b?.SLDinhBien,
                           isNSTapSu = b?.NSTapSu == 1 ? true : false,
                           isChuaCoNS = b?.NSTrong == 1 ? true : false,
                           isNSVTKhac = b?.NSVTKhac == 1 ? true : false,
                           thoigianBoNhiem = b?.TGBoNhiem ?? default,
                           ghichu = b?.GhiChu,
                           NgayTao = b?.NgayTao ?? default,
                           ThuaThieu = b?.SLDinhBien != null && a?.SLNV != null ? (int)a?.SLNV - (int)b?.SLDinhBien : 0,
                           IDDB =b?.ID
                         
                       }).FirstOrDefault();
            return PartialView(res);
        }
        [HttpPost]
        public ActionResult Edit(DinhBienView _DO)
        {
            try
            {
                if(_DO.IDDB != 0 && _DO.IDDB != null && _DO.IDVT != 0)
                {
                    db.DinhBienVT_Update(_DO.IDDB, _DO.IDVT, _DO.DinhBienNS, _DO.isNSTapSu?1:0, _DO.isChuaCoNS?1:0, _DO.thoigianBoNhiem, _DO.isNSVTKhac ? 1 : 0, _DO.ghichu, DateTime.Now);
                }
                else if(_DO.IDVT != 0 && _DO.IDDB == null)
                {
                    db.DinhBienVT_insert( _DO.IDVT, _DO.DinhBienNS, _DO.isNSTapSu ? 1 : 0, _DO.isChuaCoNS ? 1 : 0, _DO.thoigianBoNhiem, _DO.isNSVTKhac ? 1 : 0, _DO.ghichu, DateTime.Now);
                }

                TempData["msgSuccess"] = "<script>alert('Cập nhập thành công');</script>";
            }
            catch (Exception e)
            {

                TempData["msgSuccess"] = "<script>alert('Cập nhập thất bại " + e.Message + " ');</script>";
            }

            return RedirectToAction("Index", "DinhBien",new { IDPB =_DO.IDPB});
        }

        public ActionResult Delete(int id, int? IDPB)
        {
            try
            {
                db.DinhBienVT_delete(id);
                TempData["msgError"] = "<script>alert('Xóa dữ liệu thành công');</script>";
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Xóa dữ liệu thất bại');</script>";
            }
            return RedirectToAction("Index", "DinhBien", new { IDPB = IDPB });
        }
        public ActionResult DowloadBMDinhBien()
        {
            try
            {
                string fileNamemau = AppDomain.CurrentDomain.BaseDirectory + @"App_Data\BM_DinhBienViTri.xlsx";
                string fileNamemaunew = AppDomain.CurrentDomain.BaseDirectory + @"App_Data\BM_DinhBienViTri_Temp.xlsx";
                XLWorkbook Workbook = new XLWorkbook(fileNamemau);
                IXLWorksheet Worksheet = Workbook.Worksheet("BM_DBVT");
                List<DinhBienView> DataKNL = GetViTriKNL();
                int row = 3;
                if (DataKNL.Count > 0)
                {
                    foreach (var data in DataKNL)
                    {
                        Worksheet.Cell(row, "A").Value = row - 2;
                        Worksheet.Cell(row, "A").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        Worksheet.Cell(row, "A").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "A").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "B").Value = data.TenViTri;
                        Worksheet.Cell(row, "B").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "B").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "B").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "C").Value = data.IDVT;
                        Worksheet.Cell(row, "C").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        Worksheet.Cell(row, "C").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "C").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "D").Value = data.MaViTri;
                        Worksheet.Cell(row, "D").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "D").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "D").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "E").Value = data.TenPhongBan;
                        Worksheet.Cell(row, "E").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "E").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "E").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "F").Value = data.TenKhoi;
                        Worksheet.Cell(row, "F").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "F").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "F").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "G").Value = data.TenPX;
                        Worksheet.Cell(row, "G").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "G").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "G").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "H").Value = data.TenNhom;
                        Worksheet.Cell(row, "H").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "H").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "H").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;


                        Worksheet.Cell(row, "I").Value = data.TenTo;
                        Worksheet.Cell(row, "I").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        Worksheet.Cell(row, "I").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "I").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "J").Value = data.CountKNL;
                        Worksheet.Cell(row, "J").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        Worksheet.Cell(row, "J").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "J").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "K").Value = data.NhapBMTCV;
                        Worksheet.Cell(row, "K").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        Worksheet.Cell(row, "K").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "K").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "M").Value = data.CountNV;
                        Worksheet.Cell(row, "M").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        Worksheet.Cell(row, "M").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "M").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        //Worksheet.Cell(row, "O").Value = data.CountNV != 0 ?"X":"";
                        //Worksheet.Cell(row, "O").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        //Worksheet.Cell(row, "O").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        //Worksheet.Cell(row, "O").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        row = row + 1;
                    }

                    Workbook.SaveAs(fileNamemaunew);
                    byte[] fileBytes = System.IO.File.ReadAllBytes(fileNamemaunew);
                    string fileName = "BM_DinhBienViTri- " + DateTime.Now.Date.ToString("dd/MM/yyyy") + ".xlsx";
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
                }
                else
                {

                    //Worksheet.Cell("A1").Value = "Ngày xuất file: " + DateTime.Now.Date.ToString("dd/MM/yyyy");
                    Workbook.SaveAs(fileNamemaunew);
                    byte[] fileBytes = System.IO.File.ReadAllBytes(fileNamemaunew);
                    string fileName = "BM_DinhBienViTri - " + DateTime.Now.Date.ToString("dd/MM/yyyy") + ".xlsx";
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = "<script>alert('" + ex + "');window.location.href = '/Everyday/'</script>";
                return View(TempData);
            }
        }
        private List<DinhBienView> GetViTriKNL()
        {
            var res = (from a in db.VitriKNL_Select(0)
                       select new DinhBienView
                       {
                           IDVT = a.IDVT,
                           TenViTri = a.TenViTri,
                           IDPB = a.IDPhongBan,
                           TenPhongBan = a.TenPhongBan,
                           MaViTri = a.MaViTri,
                           IDKhoi = a.IDKhoi,
                           TenKhoi = a.TenKhoi,
                           IDPX = a.IDPX,
                           TenPX = a.TenPX,
                           IDNhom = a.IDNhom,
                           TenNhom = a.TenNhom,
                           IDTo = a.IDTo,
                           TenTo = a.TenTo,
                           FilePath = a.FilePath,
                           CountNV = a.SLNV == null ? 0 : a.SLNV,
                           CountKNL = a.SLNL,
                           NhapBMTCV = a.FilePath != null?"Đã nhập":"Chưa nhập"
                       }).OrderBy(x => x.IDPB).ToList();
            return res;
        }

        [HttpPost]
        public ActionResult ImportExcel()
        {
            string filePath = string.Empty;
            if (Request != null)
            {
                HttpPostedFileBase file = Request.Files["FileUpload"];
                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    string path = Server.MapPath("~/UploadedFiles/");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    filePath = path + Path.GetFileName(DateTime.Now.ToString("yyyyMMddHHmm") + "-" + file.FileName);

                    file.SaveAs(filePath);
                    Stream stream = file.InputStream;

                    IExcelDataReader reader = null;
                    if (file.FileName.EndsWith(".xls"))
                    {
                        reader = ExcelReaderFactory.CreateBinaryReader(stream);
                    }
                    else if (file.FileName.EndsWith(".xlsx"))
                    {
                        reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                    }
                    else
                    {
                        TempData["msg"] = "<script>alert('Vui lòng chọn đúng định dạng file Excel');</script>";
                        return View();
                    }
                    DataSet result = reader.AsDataSet();
                    DataTable dt = result.Tables[0];
                    reader.Close();

                    try
                    {
                        for (int i = 2; i < dt.Rows.Count; i++)
                        {
                            string IDVT = dt.Rows[i][2].ToString().Trim();

                            string SLDinhBien = dt.Rows[i][11].ToString().Trim();

                            int NSTapSu = dt.Rows[i][14].ToString().Trim() != ""?1:0;

                            int NSTrong = dt.Rows[i][15].ToString().Trim() != "" ? 1 : 0;

                            string ngayBoNhiemStr = dt.Rows[i][16].ToString().Trim();
                            int NSVTkhac = dt.Rows[i][17].ToString().Trim() != "" ? 1 : 0;
                            string ghichu = dt.Rows[i][18].ToString().Trim();
                            if(IDVT != "")
                            {
                                int vtid = Int32.TryParse(IDVT, out vtid) ? vtid : 0;
                                int dinhbien = Int32.TryParse(SLDinhBien, out dinhbien) ? dinhbien : 0;
                                string dateFormat = "dd/MM/yyyy";

                                //var ngayBoNhiem = ngayBoNhiemStr != "" ? DateTime.ParseExact(ngayBoNhiemStr, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture) : default;
                                var ngayBoNhiem = DateTime.MinValue;
                                if (IsValidDateFormat(ngayBoNhiemStr, dateFormat) && ngayBoNhiemStr != "")
                                {
                                    ngayBoNhiem = DateTime.ParseExact(ngayBoNhiemStr, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                                }
                              
                               
                                if (vtid != 0 && (dinhbien != 0 || NSTapSu != 0 || NSTrong != 0 || NSVTkhac != 0 ))
                                {
                                    var a = db.DinhBienVTs.Where(x => x.IDVT == vtid).ToList();
                                   
                                    if (a.Count != 0 )
                                    {
                                        var dbvt = a.FirstOrDefault();
                                        var k = db.DinhBienVT_Update(dbvt.ID, vtid, dinhbien, NSTapSu, NSTrong, ngayBoNhiem,NSVTkhac, ghichu, DateTime.Now);
                                    }
                                    else
                                    {
                                        var k = db.DinhBienVT_insert( vtid, dinhbien, NSTapSu, NSTrong, ngayBoNhiem, NSVTkhac, ghichu, DateTime.Now);
                                    }
                                }
                            }
                           

                        }

                        TempData["msgSuccess"] = "<script>alert('Import dữ liệu thành công');</script>";
                    }
                    catch (Exception ex)
                    {
                        TempData["msgError"] = "<script>alert('Có lỗi khi Upload: " + ex + "');</script>";
                    }
                }
                else
                {
                    TempData["msgSuccess"] = "<script>alert('Vui lòng nhập file Import');</script>";
                }
            }
            else
            {
                TempData["msgSuccess"] = "<script>alert('Vui lòng nhập file Import');</script>";
            }

            return RedirectToAction("Index", "DinhBien");
        }
        static bool IsValidDateFormat(string input, string dateFormat)
        {
            DateTime parsedDate;
            return DateTime.TryParseExact(input, dateFormat, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out parsedDate);
        }
    }
}