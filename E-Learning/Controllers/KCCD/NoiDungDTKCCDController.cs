using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2010.Excel;
using E_Learning.Models;
using E_Learning.ModelsKCCD;
using ExcelDataReader;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_Learning.Controllers.KCCD
{
    public class NoiDungDTKCCDController : Controller
    {
        ELEARNINGEntities db = new ELEARNINGEntities();
        ELEARNINGEntities dbKCCCD = new ELEARNINGEntities();
        int Idquyen = MyAuthentication.IDQuyen;
        String ControllerName = "NoiDungDTKCCD";
        // GET: NoiDungDTKCCD
        public ActionResult Index(int? page, string search, int? IDPB, int? IDLVDT)
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
            if (IDLVDT == null) IDLVDT = 0;
            //var phongban = db.PhongBans.ToList();
            //var lvdt = db.LinhVucDTs.ToList();
            var res = new List<NoiDungDTKCCDView>();
            if (IDPB != 0)
            {
                res = (from a in dbKCCCD.NoiDungDTKCCD_select(search).Where(x=>x.PhongBanID ==IDPB)
                       //join b in phongban on a.PhongBanID equals b.IDPhongBan
                       //join c in lvdt on a.LVDTID equals c.IDLVDT
                       select new NoiDungDTKCCDView
                       {
                           ID = a.ID,
                           LVDTID = (int)a.LVDTID,
                           TenLVDT = a.TenLVDT,
                           NhomNLID = (int)a.NhomNLID,
                           TenNhomNL = a.NoiDung,
                           PhongBanID = (int)a.PhongBanID,
                           TenPhongBan = a.TenPhongBan,
                           NgayTao = (DateTime)a.NgayTao,
                           TenND = a.TenND,
                           SLLH = a.SLLH,
                           SLCH = db.KCCD_CauHoi.Where(x=>x.KCCDID ==a.ID).Count(),
                           SLDT =db.KCCD_DeThi.Where(x => x.KCCDID == a.ID).Count(),
                       }).ToList();
            }
            else {
                res = (from a in dbKCCCD.NoiDungDTKCCD_select(search)
                       //join b in phongban on a.PhongBanID equals b.IDPhongBan
                       //join c in lvdt on a.LVDTID equals c.IDLVDT
                       select new NoiDungDTKCCDView
                       {
                           ID = a.ID,
                           LVDTID = (int)a.LVDTID,
                           TenLVDT = a.TenLVDT,
                           NhomNLID = (int)a.NhomNLID,
                           TenNhomNL = a.NoiDung,
                           PhongBanID = (int)a.PhongBanID,
                           TenPhongBan = a.TenPhongBan,
                           NgayTao = (DateTime)a.NgayTao,
                           TenND = a.TenND,
                           SLLH =a.SLLH,
                           SLCH = db.KCCD_CauHoi.Where(x => x.KCCDID == a.ID).Count(),
                           SLDT = db.KCCD_DeThi.Where(x => x.KCCDID == a.ID).Count(),
                       }).ToList();
            }

            //var res = (from a in dbKCCCD.NoiDungDTKCCD_select(search)
            //           select new NoiDungDTKCCDView
            //           {
            //               ID = a.ID,
            //               LVDTID = (int)a.LVDTID,
            //               TenLVDT = a.TenLVDT,
            //               NhomNLID = (int)a.NhomNLID,
            //               TenNhomNL = a.NoiDung,
            //               PhongBanID = (int)a.PhongBanID,
            //               TenPhongBan = a.TenPhongBan,
            //               NgayTao = (DateTime)a.NgayTao,
            //               TenND = a.TenND
            //           }).ToList();
            if (!ListQuyen.Contains(CONSTKEY.VIEW_ALL)) res = res.Where(x => x.PhongBanID == MyAuthentication.IDPhongban).ToList();

            List<PhongBan> dt = db.PhongBans.ToList();
            ViewBag.IDPB = new SelectList(dt, "IDPhongBan", "TenPhongBan",IDPB);
            List<LinhVucDT> lv = db.LinhVucDTs.ToList();
            ViewBag.LVDTID = new SelectList(lv, "IDLVDT", "TenLVDT",IDLVDT);

            if (IDPB != 0) res = res.Where(x => x.PhongBanID == IDPB).ToList();
            if (IDLVDT != 0) res = res.Where(x => x.LVDTID == IDLVDT).ToList();

            if (page == null) page = 1;
            int pageSize = 50;
            int pageNumber = (page ?? 1);
            return View(res.ToList().ToPagedList(pageNumber, pageSize));
        }

        // GET: NoiDungDTKCCD/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: NoiDungDTKCCD/Create
        public ActionResult Create()
        {
            //var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            //if (!ListQuyen.Contains(CONSTKEY.ADD))
            //{
            //    TempData["msgError"] = "<script>alert('Bạn không có quyền thực hiện chức năng này');</script>";
            //    return RedirectToAction("", "Home");
            //}
            //db.Configuration.ProxyCreationEnabled = false;

            List<PhongBan> dt = db.PhongBans.ToList();
            ViewBag.PhongBanID = new SelectList(dt, "IDPhongBan", "TenPhongBan");

            List<LinhVucDT> lv = db.LinhVucDTs.ToList();
            ViewBag.LVDTID = new SelectList(lv, "IDLVDT", "TenLVDT");

            List<NhomNLKCCD> nhom = dbKCCCD.NhomNLKCCDs.ToList();
            ViewBag.NhomNLID = new SelectList(nhom, "ID", "NoiDung",1);

            return PartialView();
        }

        // POST: NoiDungDTKCCD/Create
        [HttpPost]
        public ActionResult Create(NoiDungDTKCCDView _DO, FormCollection collection)
        {
            try
            {
                if (_DO.TenND != null && _DO.LVDTID != 0 )
                {
                    _DO.NgayTao = DateTime.Now;
                    var aa = dbKCCCD.NoiDungDTKCCD_insert(_DO.TenND,_DO.NhomNLID,_DO.LVDTID,_DO.PhongBanID,_DO.NgayTao);
                }
               
                TempData["msgSuccess"] = "<script>alert('Thêm mới thành công');</script>";

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Có lỗi khi thêm mới: " + e.Message + "');</script>";
                return RedirectToAction("Index");
            }
        }

        // GET: NoiDungDTKCCD/Edit/5
        public ActionResult Edit(int id)
        {
            //var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            //if (!ListQuyen.Contains(CONSTKEY.EDIT))
            //{
            //    TempData["msgError"] = "<script>alert('Bạn không có quyền thực hiện chức năng này');</script>";
            //    return RedirectToAction("", "Home");
            //}
            //var phongban = db.PhongBans.ToList();
            //var lvdt = db.LinhVucDTs.ToList();

            string search = "";
            var res = (from a in dbKCCCD.NoiDungDTKCCD_select(search).Where(x=>x.ID == id)
                       //join b in phongban on a.PhongBanID equals b.IDPhongBan 
                       //join c in lvdt on a.LVDTID equals c.IDLVDT
                       select new NoiDungDTKCCDView
                       {
                           ID = a.ID,
                           LVDTID = (int)a.LVDTID,
                           TenLVDT = a.TenLVDT,
                           NhomNLID = (int)a.NhomNLID,
                           TenNhomNL = a.NoiDung,
                           PhongBanID = (int)a.PhongBanID,
                           TenPhongBan = a.TenPhongBan,
                           NgayTao = (DateTime)a.NgayTao,
                           TenND = a.TenND
                       }).ToList();
            NoiDungDTKCCDView DO = new NoiDungDTKCCDView();

            if (res.Count > 0)
            {
                foreach (var c in res)
                {
                    DO.ID = c.ID;
                    DO.TenND = c.TenND; 
                    DO.LVDTID = c.LVDTID;
                    DO.TenLVDT = c.TenLVDT;
                    DO.NhomNLID = c.NhomNLID;
                    DO.TenNhomNL = c.TenNhomNL;
                    DO.PhongBanID = c.PhongBanID;
                    DO.TenPhongBan = c.TenPhongBan;
                    DO.NgayTao = c.NgayTao;
                }

                db.Configuration.ProxyCreationEnabled = false;
                List<PhongBan> dt = db.PhongBans.ToList();
                ViewBag.PhongBanID = new SelectList(dt, "IDPhongBan", "TenPhongBan",DO.PhongBanID);

                List<LinhVucDT> lv = db.LinhVucDTs.ToList();
                ViewBag.LVDTID = new SelectList(lv, "IDLVDT", "TenLVDT",DO.LVDTID);

                List<NhomNLKCCD> nhom = dbKCCCD.NhomNLKCCDs.ToList();
                ViewBag.NhomNLID = new SelectList(nhom, "ID", "NoiDung", DO.NhomNLID);

            }
            else
            {
                return HttpNotFound();
            }
            return PartialView(DO);
        }

        // POST: NoiDungDTKCCD/Edit/5
        [HttpPost]
        public ActionResult Edit(NoiDungDTKCCDView _DO)
        {
            try
            {
                _DO.NgayTao = DateTime.Now;
                dbKCCCD.NoiDungDTKCCD_update(_DO.ID,_DO.TenND, _DO.NhomNLID, _DO.LVDTID, _DO.PhongBanID, _DO.NgayTao);

                TempData["msgSuccess"] = "<script>alert('Cập nhập thành công');</script>";
            }
            catch (Exception e)
            {

                TempData["msgSuccess"] = "<script>alert('Cập nhập thất bại " + e.Message + " ');</script>";
            }
            return RedirectToAction("Index");
        }

        // GET: NoiDungDTKCCD/Delete/5
        public ActionResult Delete(int id)
        {
            try
            {
                dbKCCCD.NoiDungDTKCCD_delete(id);
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Xóa dữ liệu thất bại');</script>";
            }
            return RedirectToAction("Index");
        }

        // POST: NoiDungDTKCCD/Delete/5
        //[HttpPost]
        //public ActionResult Delete(int id, FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add delete logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}
        public FileResult DownloadExcel()
        {
            string path = "/App_Data/BM_NDĐT_KCCD.xlsx";
            return File(path, "application/vnd.ms-excel", "BM_NDĐT_KCCD.xlsx");
        }
        public ActionResult ImportExcel()
        {
            return PartialView();
        }
        [HttpPost]

        public ActionResult ImportExcel(NoiDungDTKCCDView _DO)
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
                    // We return the interface, so that  
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
                    int dtc = 0, dtrung = 0;
                    if (dt.Rows.Count > 0)
                    {
                        try
                        {

                            for (int i = 1; i < dt.Rows.Count; i++)
                            {

                                if (dt.Rows[i] != null)
                                {

                                    var IDPB = GetIDPB( dt.Rows[i][1].ToString());
                                    var IDNhom = GetIDNhom(dt.Rows[i][3].ToString());
                                    var IDLVDT = GetLVDT(dt.Rows[i][2].ToString());
                                    if(IDPB != 0 && IDNhom != 0 && IDLVDT != 0 && dt.Rows[i][0].ToString() != "")
                                    {
                                        _DO.NgayTao = DateTime.Now;
                                        //if(checkNDDT(dt.Rows[i][0].ToString(), IDPB, IDNhom, IDLVDT) == 0)
                                        //{
                                        //    var aa = dbKCCCD.NoiDungDTKCCD_insert(dt.Rows[i][0].ToString(), IDNhom, IDLVDT, IDPB, _DO.NgayTao);
                                        //    dtc++;
                                        //}
                                        //else
                                        //{
                                        //    dtrung++;
                                        //}
                                        var aa = dbKCCCD.NoiDungDTKCCD_insert(dt.Rows[i][0].ToString(), IDNhom, IDLVDT, IDPB, _DO.NgayTao);
                                        dtc++;
                                    }
                                    else
                                    {
                                        dtrung++;
                                    }
                                }

                            }
                            string msg = "";
                            if (dtc != 0 && dtrung != 0)
                            {
                                msg = "Import được " + dtc + " dòng dữ liệu, " + "Có " + dtrung + " dòng trùng Mã NV cập nhập nội dung";
                            }
                            else if (dtc != 0 && dtrung == 0)
                            {
                                msg = "Import được " + dtc + " dòng dữ liệu";
                            }
                            else if (dtc == 0 && dtrung != 0)
                            {
                                msg = "Có " + dtrung + " dòng trùng Mã NV cập nhập nội dung";
                            }
                            else { msg = "File import không có dữ liệu"; }


                            TempData["msgSuccess"] = "<script>alert('" + msg + "');</script>";

                        }
                        catch (Exception ex)
                        {
                            TempData["msgError"] = "<script>alert('File import nội dung không đúng. Vui lòng kiểm tra lại nội dung');</script>";
                        }
                    }
                    else
                    {
                        TempData["msgError"] = "<script>alert('File import không đúng định dạng. Vui lòng kiểm tra biểu mẫu file import');</script>";
                    }

                }
                else
                {
                    TempData["msgError"] = "<script>alert('Vui lòng nhập file Import');</script>";
                }
            }
            else
            {
                TempData["msgError"] = "<script>alert('Vui lòng nhập file Import');</script>";
            }

            return RedirectToAction("Index");
        }
        public int GetIDPB(string TenPB)
        {
            var model = db.PhongBans.Where(x => x.TenPhongBan==TenPB).SingleOrDefault();
            if (model == null)
                return 0;
            return model.IDPhongBan;
        }
        public int GetIDNhom(string TenNhom)
        {
            var model = dbKCCCD.NhomNLKCCDs.Where(x => x.NoiDung==TenNhom).SingleOrDefault();
            if (model == null)
                return 0;
            return model.ID;
        }
        public int GetLVDT(string TenLV)
        {
            var model = db.LinhVucDTs.Where(x => x.TenLVDT.Contains(TenLV)).SingleOrDefault();
            if (model == null)
                return 0;
            return model.IDLVDT;
        }

        //public ActionResult ExportData()
        //{
        //    return PartialView();
        //}


        public ActionResult ExportData()
        {
            try
            {
                string fileNamemau = AppDomain.CurrentDomain.BaseDirectory + @"App_Data\ThongKe_DS_NDĐT_KCCD.xlsx";
                string fileNamemaunew = AppDomain.CurrentDomain.BaseDirectory + @"App_Data\ThongKe_DS_NDĐT_KCCD_Temp.xlsx";
                XLWorkbook Workbook = new XLWorkbook(fileNamemau);
                IXLWorksheet Worksheet = Workbook.Worksheet("NDDT");
                List<NoiDungDTKCCDView> DataKNL = ListNDDT();
                int row = 2;
                if (DataKNL.Count > 0)
                {
                    foreach (var data in DataKNL)
                    {
                        Worksheet.Cell(row, "A").Value = row -1;
                        Worksheet.Cell(row, "A").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        Worksheet.Cell(row, "A").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "A").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "B").Value = data.ID;
                        Worksheet.Cell(row, "B").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "B").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "B").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "C").Value = data.TenND;
                        Worksheet.Cell(row, "C").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "C").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "C").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "D").Value = data.TenPhongBan;
                        Worksheet.Cell(row, "D").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "D").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "D").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "E").Value = data.TenLVDT;
                        Worksheet.Cell(row, "E").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "E").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "E").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "F").Value = data.TenNhomNL;
                        Worksheet.Cell(row, "F").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "F").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "F").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;


                        Worksheet.Cell(row, "G").Value = data.NgayTao.ToString("dd/MM/yyyy");
                        Worksheet.Cell(row, "G").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        Worksheet.Cell(row, "G").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "G").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        //Worksheet.Cell(row, "F").Style.Fill.BackgroundColor = XLColor.Yellow;

                        Worksheet.Cell(row, "H").Value = data.SLCH + " Câu hỏi/ " + data.SLDT + " Đề thi" ;
                        Worksheet.Cell(row, "H").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        Worksheet.Cell(row, "H").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "H").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        row = row + 1;
                    }

                    Workbook.SaveAs(fileNamemaunew);
                    byte[] fileBytes = System.IO.File.ReadAllBytes(fileNamemaunew);
                    string fileName = "ThongKe_DS_NDĐT_KCCD - " + DateTime.Now.Date.ToString("dd/MM/yyyy") + ".xlsx";
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
                }
                else
                {

                    //Worksheet.Cell("A1").Value = "Ngày xuất file: " + DateTime.Now.Date.ToString("dd/MM/yyyy");
                    Workbook.SaveAs(fileNamemaunew);
                    byte[] fileBytes = System.IO.File.ReadAllBytes(fileNamemaunew);
                    string fileName = "ThongKe_DS_NDĐT_KCCD - " + DateTime.Now.Date.ToString("dd/MM/yyyy") + ".xlsx";
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
                }

            }
            catch (Exception ex)
            {
                TempData["msg"] = "<script>alert('" + ex + "');window.location.href = '/Everyday/'</script>";
                return View(TempData);
            }
        }

        public List<NoiDungDTKCCDView> ListNDDT()
        {
            //var phongban = db.PhongBans.ToList();
            //var lvdt = db.LinhVucDTs.ToList();

            var res = (from a in dbKCCCD.NoiDungDTKCCD_select("")
                       //join b in phongban on a.PhongBanID equals b.IDPhongBan
                       //join c in lvdt on a.LVDTID equals c.IDLVDT
                       select new NoiDungDTKCCDView
                       {
                           ID = a.ID,
                           LVDTID = (int)a.LVDTID,
                           TenLVDT = a.TenLVDT,
                           NhomNLID = (int)a.NhomNLID,
                           TenNhomNL = a.NoiDung,
                           PhongBanID = (int)a.PhongBanID,
                           TenPhongBan = a.TenPhongBan,
                           NgayTao = (DateTime)a.NgayTao,
                           TenND = a.TenND,
                           SLCH = db.KCCD_CauHoi.Where(x => x.KCCDID == a.ID).Count(),
                           SLDT = db.KCCD_DeThi.Where(x => x.KCCDID == a.ID).Count(),
                       }).ToList();
            return res;
        }

        //public int checkNDDT(string TenND, int? IDPB, int? IDNhom, int? IDLVDT)
        //{
        //    var model = dbKCCCD.NoiDungDTKCCDs.Where(x => x.TenND == TenND && x.PhongBanID ==IDPB && x.NhomNLID == IDNhom && x.LVDTID == IDLVDT).SingleOrDefault();
        //    if (model == null)
        //        return 0;
        //    return model.ID;
        //}

    }
}
