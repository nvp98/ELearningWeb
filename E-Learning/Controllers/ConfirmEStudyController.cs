using PagedList;
using E_Learning.Models;
using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using ExcelDataReader;
using ClosedXML.Excel;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;
using System.Net;
using iTextSharp.tool.xml;
using Rotativa;
using Rotativa.Options;
using DocumentFormat.OpenXml.Presentation;
using Font = iTextSharp.text.Font;
using static iTextSharp.text.Font;




//using Font = iTextSharp.text.Font;

namespace E_Learning.Controllers
{
    public class ConfirmEStudyController : Controller
    {
        ELEARNINGEntities db_context = new ELEARNINGEntities();
        // GET: ConfirmEStudy


        private List<SelectListItem> getYNOptions()
        {
            List<SelectListItem> yn = new List<SelectListItem>();
            yn.Add(new SelectListItem() { Text = "True", Value = true.ToString(), Selected = true });
            yn.Add(new SelectListItem() { Text = "False", Value = false.ToString(), Selected = true });
            return yn;
        }
        public ActionResult Index(int id, int? page, int? IDPhongBan, string search)
        {
            ViewBag.YNList = getYNOptions();
            List<PhongBan> pb = db_context.PhongBans.ToList();
            ViewBag.PBList = new SelectList(pb, "IDPhongBan", "TenPhongBan", IDPhongBan);
            if (IDPhongBan == null) IDPhongBan = 0;
            if (search == null) search = "";
            ViewBag.search = search;
            if (page == null) page = 1;
            int pageSize = 50;
            int pageNumber = (page ?? 1);
            return View(GetlistConfirmEStudy(id, IDPhongBan, search).ToPagedList(pageNumber, pageSize));
        }

        List<ConfirmEStudyValidation> GetlistConfirmEStudy(int? id, int? IDPhongBan, string search)
        {
            if (IDPhongBan == null) IDPhongBan = 0;
            if (search == "")
            {
                if (IDPhongBan != 0)
                {
                    var model = (from h in db_context.XNHocTaps.Where(x => x.NhanVien.IDPhongBan == IDPhongBan)
                                 join l in db_context.LopHocs on h.LHID equals l.IDLH
                                 join n in db_context.NhanViens on h.NVID equals n.ID
                                 join p in db_context.PhongBans on h.PBID equals p.IDPhongBan
                                 join v in db_context.Vitris on h.VTID equals v.IDViTri
                                 select new ConfirmEStudyValidation()
                                 {
                                     IDHT = h.IDHT,
                                     PBID = (int)h.PBID,
                                     NVID = n.ID,
                                     MaNV = n.MaNV,
                                     HoTenHV = n.HoTen,
                                     TenPB = p.TenPhongBan,
                                     VTID = (int)h.VTID,
                                     TenVT = v.TenViTri,
                                     LHID = l.IDLH,
                                     TenLH = l.TenLH,
                                     TGBDLH = (DateTime)l.TGBDLH,
                                     TGKTLH = (DateTime)l.TGKTLH,
                                     LinhVuc = l.NoiDungDT.LinhVucDT.TenLVDT,
                                     TenND = l.NoiDungDT.NoiDung,
                                     TLDT = (int)l.NoiDungDT.ThoiLuongDT,
                                     NgayTG = (DateTime)h.NgayTG,
                                     NgayHT = (DateTime)h.NgayHT,
                                     XNTG = (bool)h.XNTG,
                                     XNHT = (bool)h.XNHT,
                                 }).Where(x => x.LHID == id).ToList();
                    return model.ToList();
                }
                else
                {
                    var model = (from h in db_context.XNHocTaps
                                 join l in db_context.LopHocs on h.LHID equals l.IDLH
                                 join n in db_context.NhanViens on h.NVID equals n.ID
                                 join p in db_context.PhongBans on h.PBID equals p.IDPhongBan
                                 join v in db_context.Vitris on h.VTID equals v.IDViTri
                                 select new ConfirmEStudyValidation()
                                 {
                                     IDHT = h.IDHT,
                                     PBID = (int)h.PBID,
                                     NVID = n.ID,
                                     MaNV = n.MaNV,
                                     HoTenHV = n.HoTen,
                                     TenPB = p.TenPhongBan,
                                     VTID = (int)h.VTID,
                                     TenVT = v.TenViTri,
                                     LHID = l.IDLH,
                                     TenLH = l.TenLH,
                                     TGBDLH = (DateTime)l.TGBDLH,
                                     TGKTLH = (DateTime)l.TGKTLH,
                                     LinhVuc = l.NoiDungDT.LinhVucDT.TenLVDT,
                                     TenND = l.NoiDungDT.NoiDung,
                                     TLDT = (int)l.NoiDungDT.ThoiLuongDT,
                                     NgayTG = (DateTime)h.NgayTG,
                                     NgayHT = (DateTime)h.NgayHT,
                                     XNTG = (bool)h.XNTG,
                                     XNHT = (bool)h.XNHT,
                                 }).Where(x => x.LHID == id).ToList();
                    return model.ToList();
                }
            }
            else
            {
                if (IDPhongBan != 0)
                {
                    var model = (from h in db_context.XNHocTaps.Where(x => x.NhanVien.IDPhongBan == IDPhongBan && (x.NhanVien.MaNV.Contains(search) || x.NhanVien.HoTen.Contains(search)))
                                 join l in db_context.LopHocs on h.LHID equals l.IDLH
                                 join n in db_context.NhanViens on h.NVID equals n.ID
                                 join p in db_context.PhongBans on h.PBID equals p.IDPhongBan
                                 join v in db_context.Vitris on h.VTID equals v.IDViTri
                                 select new ConfirmEStudyValidation()
                                 {
                                     IDHT = h.IDHT,
                                     PBID = (int)h.PBID,
                                     NVID = n.ID,
                                     MaNV = n.MaNV,
                                     HoTenHV = n.HoTen,
                                     TenPB = p.TenPhongBan,
                                     VTID = (int)h.VTID,
                                     TenVT = v.TenViTri,
                                     LHID = l.IDLH,
                                     TenLH = l.TenLH,
                                     TGBDLH = (DateTime)l.TGBDLH,
                                     TGKTLH = (DateTime)l.TGKTLH,
                                     LinhVuc = l.NoiDungDT.LinhVucDT.TenLVDT,
                                     TenND = l.NoiDungDT.NoiDung,
                                     TLDT = (int)l.NoiDungDT.ThoiLuongDT,
                                     NgayTG = (DateTime)h.NgayTG,
                                     NgayHT = (DateTime)h.NgayHT,
                                     XNTG = (bool)h.XNTG,
                                     XNHT = (bool)h.XNHT,
                                 }).Where(x => x.LHID == id).ToList();
                    return model.ToList();
                }
                else
                {
                    var model = (from h in db_context.XNHocTaps.Where(x => x.NhanVien.MaNV.Contains(search) || x.NhanVien.HoTen.Contains(search))
                                 join l in db_context.LopHocs on h.LHID equals l.IDLH
                                 join n in db_context.NhanViens on h.NVID equals n.ID
                                 join p in db_context.PhongBans on h.PBID equals p.IDPhongBan
                                 join v in db_context.Vitris on h.VTID equals v.IDViTri
                                 select new ConfirmEStudyValidation()
                                 {
                                     IDHT = h.IDHT,
                                     PBID = (int)h.PBID,
                                     NVID = n.ID,
                                     MaNV = n.MaNV,
                                     HoTenHV = n.HoTen,
                                     TenPB = p.TenPhongBan,
                                     VTID = (int)h.VTID,
                                     TenVT = v.TenViTri,
                                     LHID = l.IDLH,
                                     TenLH = l.TenLH,
                                     TGBDLH = (DateTime)l.TGBDLH,
                                     TGKTLH = (DateTime)l.TGKTLH,
                                     LinhVuc = l.NoiDungDT.LinhVucDT.TenLVDT,
                                     TenND = l.NoiDungDT.NoiDung,
                                     TLDT = (int)l.NoiDungDT.ThoiLuongDT,
                                     NgayTG = (DateTime)h.NgayTG,
                                     NgayHT = (DateTime)h.NgayHT,
                                     XNTG = (bool)h.XNTG,
                                     XNHT = (bool)h.XNHT,
                                 }).Where(x => x.LHID == id).ToList();
                    return model.ToList();
                }
            }
        }


        public ActionResult Create(int id)
        {
            //List<LopHoc> lh = db_context.LopHocs.ToList();
            //ViewBag.LHList = new SelectList(lh, "IDLH", "TenLH");

            db_context.Configuration.ProxyCreationEnabled = false;
            List<LopHoc> lh = db_context.LopHocs.Where(x => x.IDLH == id).ToList();
            ViewBag.LHList = new SelectList(lh, "IDLH", "TenLH");

            List<PhongBan> pb = db_context.PhongBans.ToList();
            ViewBag.PBList = new SelectList(pb, "IDPhongBan", "TenPhongBan");

            //List<NhanVien> mnv = db_context.NhanViens.ToList();
            //ViewBag.NVList = new SelectList(mnv, "ID", "MaNV");

            return PartialView();
        }

        [HttpPost]
        public ActionResult Create(ConfirmEStudyValidation _DO)
        {
            // _DO chỉ get IDNV và IDLH không get được Mã
            try
            {
                var MaNV = db_context.NhanViens.Where(x => x.ID == _DO.NVID).Select(g => g.MaNV).FirstOrDefault();
                var VTID = db_context.NhanViens.Where(x => x.ID == _DO.NVID).Select(g => g.IDViTri).FirstOrDefault();
                if (IsHVAvailable(_DO.LHID, MaNV) == false)
                {
                    db_context.XNHocTap_insert(_DO.NVID, _DO.LHID, _DO.NgayTG, _DO.NgayHT, _DO.XNTG, _DO.XNHT, _DO.PBID, VTID);
                    TempData["msgSuccess"] = "<script>alert('Thêm mới thành công');</script>";
                }
                else
                {
                    TempData["msgError"] = "<script>alert('Nhân viên đã tồn tại trong lớp');</script>";
                }

            }

            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Có lỗi khi thêm mới học viên: " + e.Message + "');</script>";
            }
            return RedirectToAction("Index", "ConfirmEStudy", new { id = _DO.LHID });
        }




        //  ------- Start Import Excel --------  //

        //Return ID để thêm học viên trong khi thêm lớp 


        public ActionResult ImportExcel()
        {
            return PartialView();
        }
        [HttpPost]

        public ActionResult ImportExcel(ConfirmEStudyValidation _DO, int id)
        {
            var LHID = id;
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

                            for (int i = 5; i < dt.Rows.Count; i++)
                            {

                                if (dt.Rows[i] != null)
                                {

                                    var MaNV = dt.Rows[i][0].ToString();
                                    var NVID = db_context.NhanViens.Where(x => x.MaNV == MaNV).Select(g => g.ID).FirstOrDefault();
                                    var PBID = db_context.NhanViens.Where(x => x.MaNV == MaNV).Select(g => g.IDPhongBan).FirstOrDefault();
                                    var VTID = db_context.NhanViens.Where(x => x.MaNV == MaNV).Select(g => g.IDViTri).FirstOrDefault();
                                    var NgayTG = DateTime.ParseExact("01/01/0001", "dd/MM/yyyy", CultureInfo.InvariantCulture);
                                    var NgayHT = DateTime.ParseExact("01/01/0001", "dd/MM/yyyy", CultureInfo.InvariantCulture);
                                    var XNTG = false;
                                    var XNHT = false;
                                    if (IsHVAvailable(LHID, MaNV) == false)
                                    {
                                        if (CheckMaNV(MaNV))
                                        {
                                            InsertDSHocTap(NVID, NgayTG, NgayHT, XNTG, XNHT, LHID, PBID, VTID);
                                            dtc++;
                                        }
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

            return RedirectToAction("Index", "ConfirmEStudy", new { id = LHID });
        }

        public bool CheckMaNV(string MaNV)
        {
            var IsCheck = db_context.NhanViens.Where(x => x.MaNV.ToLower() == MaNV.ToLower()).FirstOrDefault();
            bool status;
            if (IsCheck != null)
            {

                status = true;
            }
            else
            {

                status = false;
            }
            return status;
        }
        public bool IsLHAvailable(string MaLH)
        {
            var IsCheck = (from h in db_context.XNHocTaps
                           join l in db_context.LopHocs
                           on h.LHID equals l.IDLH
                           where (l.MaLH.ToLower() == MaLH)
                           select new { l.MaLH }).FirstOrDefault();
            bool status;
            if (IsCheck != null)
            {

                status = true;
            }
            else
            {

                status = false;
            }
            return status;
        }


        public bool IsNVAvailable(int NVID)
        {
            var IsCheck = (from h in db_context.XNHocTaps
                           join n in db_context.NhanViens
                           on h.NVID equals n.ID
                           where (n.ID == NVID)
                           select new { n.MaNV }).FirstOrDefault();
            bool status;
            if (IsCheck != null)
            {

                status = true;
            }
            else
            {

                status = false;
            }
            return status;
        }


        public bool IsHVAvailable(int LHID, string MaNV)
        {
            var IsCheck = (from h in db_context.XNHocTaps
                           join l in db_context.LopHocs
                           on h.LHID equals l.IDLH
                           join n in db_context.NhanViens
                           on h.NVID equals n.ID
                           where (l.IDLH == LHID && n.MaNV.ToLower() == MaNV)
                           select new { l.MaLH, n.MaNV }).FirstOrDefault();
            bool status;
            if (IsCheck != null)
            {

                status = true;
            }
            else
            {

                status = false;
            }
            return status;
        }


        public void InsertDSHocTap(int NVID, DateTime NgayTG, DateTime NgayHT, bool XNTG, bool XNHT, int LHID, int? PBID, int? VTID)
        {
            db_context.XNHocTap_insert(NVID, LHID, NgayTG, NgayHT, XNTG, XNHT, PBID, VTID);
        }



        public ActionResult Delete(int id)
        {
            var idl = db_context.XNHocTaps.Where(x => x.IDHT == id).Select(g => g.LHID).FirstOrDefault();
            try
            {
                db_context.XNHocTap_delete(id);
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Xóa dữ liệu thất bại: " + e.Message + "');</script>";
            }
            return RedirectToAction("Index", "ConfirmEStudy", new { id = idl });
        }

        public JsonResult GetNhanVien(int id)
        {
            db_context.Configuration.ProxyCreationEnabled = false;
            List<NhanVien> gnv = db_context.NhanViens.Where(x => x.IDPhongBan == id && x.IDTinhTrangLV == 1).ToList();
            return Json(gnv, JsonRequestBehavior.AllowGet);
        }

        public IList<ConfirmEStudyValidation> GetConfirmEStudies(int id)
        {

            var res = (from h in db_context.XNHocTaps
                       join l in db_context.LopHocs on h.LHID equals l.IDLH
                       join n in db_context.NhanViens on h.NVID equals n.ID
                       join p in db_context.PhongBans on h.PBID equals p.IDPhongBan
                       join v in db_context.Vitris on h.VTID equals v.IDViTri
                       select new ConfirmEStudyValidation
                       {
                           IDHT = h.IDHT,
                           PBID = (int)h.PBID,
                           NVID = n.ID,
                           MaNV = n.MaNV,
                           HoTenHV = n.HoTen,
                           TenPB = p.TenPhongBan,
                           VTID = (int)h.VTID,
                           TenVT = v.TenViTri,
                           LHID = l.IDLH,
                           TenLH = l.TenLH,
                           TGBDLH = (DateTime)l.TGBDLH,
                           TGKTLH = (DateTime)l.TGKTLH,
                           LinhVuc = l.NoiDungDT.LinhVucDT.TenLVDT,
                           TenND = l.NoiDungDT.NoiDung,
                           TLDT = (int)l.NoiDungDT.ThoiLuongDT,
                           NgayTG = (DateTime)h.NgayTG,
                           NgayHT = (DateTime)h.NgayHT,
                           XNTG = (bool)h.XNTG,
                           XNHT = (bool)h.XNHT,
                       }).Where(x => x.LHID == id).ToList();
            return res;
        }
        public ActionResult ExportToExcel(int id)
        {
            var LHID = id;
            try
            {

                string fileNameMau = AppDomain.CurrentDomain.BaseDirectory + @"App_Data\BM_XDSHT.xlsx";
                string fileNameMauTemp = AppDomain.CurrentDomain.BaseDirectory + @"App_Data\BM_XDSHT_temp.xlsx";
                XLWorkbook Workbook = new XLWorkbook(fileNameMau);
                IXLWorksheet Worksheet = Workbook.Worksheet("DSHT");

                IList<ConfirmEStudyValidation> ConfirmEStudy = GetConfirmEStudies(LHID);
                List<BaiThi> KQ = db_context.BaiThis.Where(x => x.IDLH == id).ToList();
                int i = 0;
                string KetQua = "";
                string diem = "";
                if (ConfirmEStudy.Count > 0)
                {



                    int row = 8, rowlast = 8, stt = 0;
                    foreach (var item in ConfirmEStudy)
                    {

                        Worksheet.Cell("C5").Value = item.LinhVuc;
                        Worksheet.Cell("C5" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        Worksheet.Cell("C5" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell("C5" + row).Style.Alignment.WrapText = true;


                        Worksheet.Cell("E5").Value = item.TenND;
                        Worksheet.Cell("E5" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        Worksheet.Cell("E5" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell("E5" + row).Style.Alignment.WrapText = true;


                        Worksheet.Cell("G5").Value = item.TLDT;
                        Worksheet.Cell("G5" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        Worksheet.Cell("G5" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell("G5" + row).Style.Alignment.WrapText = true;


                        Worksheet.Cell("C6").Value = item.TenLH;
                        Worksheet.Cell("C6" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        Worksheet.Cell("C6" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell("C6" + row).Style.Alignment.WrapText = true;


                        Worksheet.Cell("E6").Value ="'"+ item.TGBDLH.ToString("dd/MM/yyyy");
                        Worksheet.Cell("E6" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        Worksheet.Cell("E6" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        //Worksheet.Cell("E6" + row).Style.DateFormat.Format = "dd/MM/yyyy";


                        Worksheet.Cell("G6").Value = "'" + item.TGKTLH.ToString("dd/MM/yyyy");
                        Worksheet.Cell("G6" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        Worksheet.Cell("G6" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        //Worksheet.Cell("G6" + row).Style.DateFormat.Format = "dd/MM/yyyy";


                        row++; stt++;
                        rowlast = row + 1;

                        Worksheet.Cell("A" + row).Value = stt;
                        Worksheet.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        Worksheet.Cell("A" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell("A" + row).Style.Alignment.WrapText = true;

                        Worksheet.Cell("B" + row).Value = item.MaNV;
                        Worksheet.Cell("B" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        Worksheet.Cell("B" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell("B" + row).Style.Alignment.WrapText = true;

                        Worksheet.Cell("C" + row).Value = item.HoTenHV;
                        Worksheet.Cell("C" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        Worksheet.Cell("C" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell("C" + row).Style.Alignment.WrapText = true;

                        Worksheet.Cell("D" + row).Value = item.TenVT;
                        Worksheet.Cell("D" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        Worksheet.Cell("D" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell("D" + row).Style.Alignment.WrapText = true;


                        Worksheet.Cell("E" + row).Value = item.TenPB;
                        Worksheet.Cell("E" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        Worksheet.Cell("E" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell("E" + row).Style.Alignment.WrapText = true;


                        if (item.NgayTG.ToString("dd/MM/yyyy") == "01/01/0001")
                            Worksheet.Cell("F" + row).Value = "Chưa Tham Gia";
                        else
                            Worksheet.Cell("F" + row).Value = item.NgayTG;

                        Worksheet.Cell("F" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        Worksheet.Cell("F" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell("F" + row).Style.DateFormat.Format = "dd/MM/yyyy";

                        if (item.NgayHT.ToString("dd/MM/yyyy") == "01/01/0001")
                            Worksheet.Cell("G" + row).Value = "Chưa Hoàn Thành";
                        else
                            Worksheet.Cell("G" + row).Value = item.NgayHT;

                        Worksheet.Cell("G" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        Worksheet.Cell("G" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell("G" + row).Style.DateFormat.Format = "dd/MM/yyyy";

                        if (item.XNTG == false)
                            Worksheet.Cell("H" + row).Value = "Chưa Tham Gia";
                        else
                            Worksheet.Cell("H" + row).Value = "Đã Tham Gia";

                        Worksheet.Cell("H" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        Worksheet.Cell("H" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell("H" + row).Style.Alignment.WrapText = true;

                        if (item.XNHT == false)
                            Worksheet.Cell("I" + row).Value = "Chưa Hoàn Thành";
                        else
                            Worksheet.Cell("I" + row).Value = "Đã Hoàn Thành";

                        Worksheet.Cell("I" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        Worksheet.Cell("I" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell("I" + row).Style.Alignment.WrapText = true;

                        var ketquathi = KQ.Where(x=>x.IDNV==item.NVID).OrderBy(x=>x.NgayThi).ToList();
                        int c=ketquathi.Count();
                        if (c == 0)
                        {
                            KetQua = "Chưa hoàn thành";
                            diem = "";
                        }else 
                        {
                            var kqd = ketquathi.Where(x => x.TinhTrang == true).ToList();
                            if (c == 3 && kqd.Count() == 0)
                            {
                                KetQua = "Không đạt";
                                diem = ketquathi[c - 1].DiemSo.ToString();
                            }
                            else if (c == 3 && kqd.Count() > 0)
                            {
                                KetQua = "Đạt";
                                diem = kqd[0].DiemSo.ToString();
                            }
                            else if (c <3 && kqd.Count() > 0) {
                                KetQua = "Đạt";
                                diem = kqd[0].DiemSo.ToString();
                            }else if(c<3 && kqd.Count() ==0)
                            {
                                KetQua = "Chưa hoàn thành";
                                diem = ketquathi[c-1].DiemSo.ToString();
                            }
                            else
                            {
                                KetQua = "Không đạt";
                                //diem = "tào lao";
                               diem =  ketquathi[c - 1].DiemSo.ToString();
                            }    

                        }    

                        
                        Worksheet.Cell("J" + row).Value = KetQua;
                        Worksheet.Cell("J" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell("J" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell("J" + row).Style.Alignment.WrapText = true;

                        Worksheet.Cell("K" + row).Value = "'"+diem;
                        Worksheet.Cell("K" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell("K" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell("K" + row).Style.Alignment.WrapText = true;
                        i = 0; KetQua = ""; diem = "";

                         row = rowlast - 1;
                    }
                    Worksheet.Range("A9:K" + (row)).Style.Font.SetFontName("Times New Roman");
                    Worksheet.Range("A9:K" + (row)).Style.Font.SetFontSize(11);
                    Worksheet.Range("A9:K" + (row)).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    Worksheet.Range("A9:K" + (row)).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                    Worksheet.Column("D").AdjustToContents();
                    Workbook.SaveAs(fileNameMauTemp);
                    byte[] fileBytes = System.IO.File.ReadAllBytes(fileNameMauTemp);
                    string fileName = "DSHT.xlsx";
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
                }
                else
                {
                    TempData["msg"] = "<script>alert('Không có dữ liệu');window.location.href = '/ConfirmEStudy'</script>";
                    return RedirectToAction("Index", "ConfirmEStudy", new { id = LHID });
                }

            }
            catch (Exception ex)
            {
                TempData["msg"] = "<script>alert('" + ex + "');window.location.href = '/ConfirmEStudy'</script>";
                return RedirectToAction("Index", "ConfirmEStudy", new { id = LHID });
            }

        }

        public ActionResult TestView(int? LHID, int? IDNV)
        {
            if(LHID == null) LHID = 0;
            if(IDNV == null) IDNV = 0;
            var res = (from a in db_context.BaiThis.Where(x => x.IDNV == IDNV && x.IDLH == LHID)
                       join b in db_context.NhanViens on a.IDNV equals b.ID
                       join c in db_context.PhongBans on a.IDPhongBan equals c.IDPhongBan
                       join d in db_context.Vitris on a.IDViTri equals d.IDViTri
                       join e in db_context.NoiDungDTs on a.IDND equals e.IDND
                       join f in db_context.DeThis on a.IDDeThi equals f.IDDeThi
                           select new TestView
                           {
                               IDBaiThi = a.IDBaiThi,
                               IDLH = (int)a.IDLH,
                               IDDeThi = (int)a.IDDeThi,
                               IDND = (int)a.IDND,
                               IDNV = (int)a.IDNV,
                               IDPhongBan = (int)a.IDPhongBan,
                               IDViTri = (int)a.IDViTri,
                               DiemSo = (double)a.DiemSo,
                               NgayThi = (DateTime)a.NgayThi,
                               TinhTrang = (bool)a.TinhTrang,
                               LanThi = (int)a.LanThi,
                               MaNV=b.MaNV,
                               HoTen=b.HoTen,
                               TenPhongBan=c.TenPhongBan,
                               TenViTri=d.TenViTri,
                               //NgaySinh=(DateTime)b.NgaySinh,
                               NoiDungDT=e.NoiDung,
                               ThoiGianLamBai=(int)f.ThoiGianLamBai,
                               TenDeThi=f.TenDe


                           }).ToList();
            TestView _DO= new TestView();
            if(res.Count>0)
            {
                foreach(var item in res)
                {
                    _DO.IDBaiThi = item.IDBaiThi;
                    _DO.IDLH = item.IDLH;
                    _DO.IDDeThi = item.IDDeThi;
                    _DO.IDND = item.IDND;
                    _DO.IDNV = item.IDNV;
                    _DO.IDPhongBan = item.IDPhongBan;
                    _DO.IDViTri = item.IDViTri;
                    _DO.DiemSo =item.DiemSo;
                    _DO.NgayThi = item.NgayThi;
                    _DO.TinhTrang = item.TinhTrang;
                    _DO.LanThi=item.LanThi;
                    _DO.MaNV=item.MaNV;
                    _DO.HoTen=item.HoTen;
                    _DO.TenPhongBan=item.TenPhongBan;
                    _DO.TenViTri=item.TenViTri;
                    //_DO.NgaySinh = item.NgaySinh;
                    _DO.NoiDungDT=item.NoiDungDT;
                    _DO.ThoiGianLamBai=item.ThoiGianLamBai;
                    _DO.TenDeThi=item.TenDeThi;
                }
                return View(_DO);
            }
            else
            {
                return RedirectToAction("Index", "ConfirmEStudy", new { id = LHID });
            }    

           
        }
        public ActionResult ListPrintTestView(int id)
        {
            var model = GetConfirmEStudies(id);
            List<TestView> listPrint = new List<TestView>();
            foreach (var st in model) 
            { 
                if (st.LHID == null) st.LHID = 0;
                if (st.NVID == null) st.NVID = 0;
                var res = (from a in db_context.BaiThis.Where(x => x.IDNV == st.NVID && x.IDLH == st.LHID)
                           join b in db_context.NhanViens on a.IDNV equals b.ID
                           join c in db_context.PhongBans on a.IDPhongBan equals c.IDPhongBan
                           join d in db_context.Vitris on a.IDViTri equals d.IDViTri
                           join e in db_context.NoiDungDTs on a.IDND equals e.IDND
                           join f in db_context.DeThis on a.IDDeThi equals f.IDDeThi
                           select new TestView
                           {
                               IDBaiThi = a.IDBaiThi,
                               IDLH = (int)a.IDLH,
                               IDDeThi = (int)a.IDDeThi,
                               IDND = (int)a.IDND,
                               IDNV = (int)a.IDNV,
                               IDPhongBan = (int)a.IDPhongBan,
                               IDViTri = (int)a.IDViTri,
                               DiemSo = (double)a.DiemSo,
                               NgayThi = (DateTime)a.NgayThi,
                               TinhTrang = (bool)a.TinhTrang,
                               LanThi = (int)a.LanThi,
                               MaNV = b.MaNV,
                               HoTen = b.HoTen,
                               TenPhongBan = c.TenPhongBan,
                               TenViTri = d.TenViTri,
                               //NgaySinh=(DateTime)b.NgaySinh,
                               NoiDungDT = e.NoiDung,
                               ThoiGianLamBai = (int)f.ThoiGianLamBai,
                               TenDeThi = f.TenDe

                           }).OrderByDescending(x => x.IDBaiThi).FirstOrDefault();
                TestView _DO = new TestView();
                if (res != null)
                {
                    
                        _DO.IDBaiThi = res.IDBaiThi;
                        _DO.IDLH = res.IDLH;
                        _DO.IDDeThi = res.IDDeThi;
                        _DO.IDND = res.IDND;
                        _DO.IDNV = res.IDNV;
                        _DO.IDPhongBan = res.IDPhongBan;
                        _DO.IDViTri = res.IDViTri;
                        _DO.DiemSo = res.DiemSo;
                        _DO.NgayThi = res.NgayThi;
                        _DO.TinhTrang = res.TinhTrang;
                        _DO.LanThi = res.LanThi;
                        _DO.MaNV = res.MaNV;
                        _DO.HoTen = res.HoTen;
                        _DO.TenPhongBan = res.TenPhongBan;
                        _DO.TenViTri = res.TenViTri;
                        //_DO.NgaySinh = item.NgaySinh;
                        _DO.NoiDungDT = res.NoiDungDT;
                        _DO.ThoiGianLamBai = res.ThoiGianLamBai;
                        _DO.TenDeThi = res.TenDeThi;

                    listPrint.Add(_DO);
                }
            }
            return View(listPrint.ToList());
        }
        public ActionResult PrintTestView(int? LHID, int? IDNV)
        {
            if (LHID == null) LHID = 0;
            if (IDNV == null) IDNV = 0;
            var res = (from a in db_context.BaiThis.Where(x => x.IDNV == IDNV && x.IDLH == LHID)
                       join b in db_context.NhanViens on a.IDNV equals b.ID
                       join c in db_context.PhongBans on a.IDPhongBan equals c.IDPhongBan
                       join d in db_context.Vitris on a.IDViTri equals d.IDViTri
                       join e in db_context.NoiDungDTs on a.IDND equals e.IDND
                       join f in db_context.DeThis on a.IDDeThi equals f.IDDeThi
                       select new TestView
                       {
                           IDBaiThi = a.IDBaiThi,
                           IDLH = (int)a.IDLH,
                           IDDeThi = (int)a.IDDeThi,
                           IDND = (int)a.IDND,
                           IDNV = (int)a.IDNV,
                           IDPhongBan = (int)a.IDPhongBan,
                           IDViTri = (int)a.IDViTri,
                           DiemSo = (double)a.DiemSo,
                           NgayThi = (DateTime)a.NgayThi,
                           TinhTrang = (bool)a.TinhTrang,
                           LanThi = (int)a.LanThi,
                           MaNV = b.MaNV,
                           HoTen = b.HoTen,
                           TenPhongBan = c.TenPhongBan,
                           TenViTri = d.TenViTri,
                           //NgaySinh=(DateTime)b.NgaySinh,
                           NoiDungDT = e.NoiDung,
                           ThoiGianLamBai = (int)f.ThoiGianLamBai,
                           TenDeThi = f.TenDe


                       }).ToList();
            TestView _DO = new TestView();
            if (res.Count > 0)
            {
                foreach (var item in res)
                {
                    _DO.IDBaiThi = item.IDBaiThi;
                    _DO.IDLH = item.IDLH;
                    _DO.IDDeThi = item.IDDeThi;
                    _DO.IDND = item.IDND;
                    _DO.IDNV = item.IDNV;
                    _DO.IDPhongBan = item.IDPhongBan;
                    _DO.IDViTri = item.IDViTri;
                    _DO.DiemSo = item.DiemSo;
                    _DO.NgayThi = item.NgayThi;
                    _DO.TinhTrang = item.TinhTrang;
                    _DO.LanThi = item.LanThi;
                    _DO.MaNV = item.MaNV;
                    _DO.HoTen = item.HoTen;
                    _DO.TenPhongBan = item.TenPhongBan;
                    _DO.TenViTri = item.TenViTri;
                    //_DO.NgaySinh = item.NgaySinh;
                    _DO.NoiDungDT = item.NoiDungDT;
                    _DO.ThoiGianLamBai = item.ThoiGianLamBai;
                    _DO.TenDeThi = item.TenDeThi;
                }
                return View(_DO);
            }
            else
            {
                return RedirectToAction("Index", "ConfirmEStudy", new { id = LHID });
            }


        }

        public ActionResult PrintFBMView(int? IDVT)
        {
            var vt = db_context.VitriKNL_searchByIDVT(IDVT).FirstOrDefault();
            ViewBag.TenViTri = vt?.TenViTri ?? "";
            ViewBag.IDVT = IDVT;
            ViewBag.TenPB = vt?.TenPhongBan ?? "";

            var res = (from a in db_context.KhungNangLuc_SearchByIDVT(IDVT)
                       select new FValueValidation
                       {
                           IDNL = a.IDNL,
                           TenNL = a.TenNL,
                           IDLoaiNL = a.IDLoaiNL,
                           IDVT = a.IDVT,
                           TenViTri = vt.TenViTri + "-" + vt.TenTo + "-" + vt.TenNhom +"-" + vt.TenPX + "-" +vt.MaPB,
                           IDPB = a.IDPB,
                           DinhMuc = a.IsDanhGia != 0 ? a.DinhMuc : 0,
                           IsDanhGia = a.IsDanhGia,
                       }).OrderBy(x => x.OrderBy).ToList();

            List<LoaiKNL> loaiNL = db_context.LoaiKNLs.Where(x => x.IDVT == IDVT && x.IDLoai != 1 && x.IDLoai != 2).OrderBy(x => x.OrderBy).ToList();
            ViewBag.IDLoaiNL = new SelectList(loaiNL, "IDLoai", "TenLoai");

            List<FValueValidation> _DO = new List<FValueValidation>();
            if (res.Count > 0)
            {
                foreach (var item in res)
                {
                    //_DO. = item.IDBaiThi;
                    //_DO.IDLH = item.IDLH;
                    //_DO.IDDeThi = item.IDDeThi;
                    //_DO.IDND = item.IDND;
                    //_DO.IDNV = item.IDNV;
                    //_DO.IDPhongBan = item.IDPhongBan;
                    //_DO.IDViTri = item.IDViTri;
                    //_DO.DiemSo = item.DiemSo;
                    //_DO.NgayThi = item.NgayThi;
                    //_DO.TinhTrang = item.TinhTrang;
                    //_DO.LanThi = item.LanThi;
                    //_DO.MaNV = item.MaNV;
                    //_DO.HoTen = item.HoTen;
                    //_DO.TenPhongBan = item.TenPhongBan;
                    //_DO.TenViTri = item.TenViTri;
                    //_DO.NgaySinh = item.NgaySinh;
                    //_DO.NoiDungDT = item.NoiDungDT;
                    //_DO.ThoiGianLamBai = item.ThoiGianLamBai;
                    //_DO.TenDeThi = item.TenDeThi;
                }
                return View(res.ToList());
            }
            else
            {
                return RedirectToAction("FBMView", "FPosition", new { IDVT = IDVT });
            }


        }
        //public ActionResult ExportToPdf(int? LHID,int? IDNV)
        //{
        //    if (LHID == null) LHID = 0;
        //    if (IDNV == null) IDNV = 0;
        //    var res = (from a in db_context.BaiThis.Where(x => x.IDNV == IDNV && x.IDLH == LHID)
        //               join b in db_context.NhanViens on a.IDNV equals b.ID
        //               join c in db_context.PhongBans on a.IDPhongBan equals c.IDPhongBan
        //               join d in db_context.Vitris on a.IDViTri equals d.IDViTri
        //               join e in db_context.NoiDungDTs on a.IDND equals e.IDND
        //               join f in db_context.DeThis on a.IDDeThi equals f.IDDeThi
        //               select new TestView
        //               {
        //                   IDBaiThi = a.IDBaiThi,
        //                   IDLH = (int)a.IDLH,
        //                   IDDeThi = (int)a.IDDeThi,
        //                   IDND = (int)a.IDND,
        //                   IDNV = (int)a.IDNV,
        //                   IDPhongBan = (int)a.IDPhongBan,
        //                   IDViTri = (int)a.IDViTri,
        //                   DiemSo = (double)a.DiemSo,
        //                   NgayThi = (DateTime)a.NgayThi,
        //                   TinhTrang = (bool)a.TinhTrang,
        //                   LanThi = (int)a.LanThi,
        //                   MaNV = b.MaNV,
        //                   HoTen = b.HoTen,
        //                   TenPhongBan = c.TenPhongBan,
        //                   TenViTri = d.TenViTri,
        //                   //NgaySinh=(DateTime)b.NgaySinh,
        //                   NoiDungDT = e.NoiDung,
        //                   ThoiGianLamBai = (int)f.ThoiGianLamBai

        //               }).ToList();
        //    TestView _DO = new TestView();

        //    if (res.Count > 0)
        //    {
        //        int pdfRowIndex = 1;

        //        Document document = new Document(iTextSharp.text.PageSize.A4, 5f, 5f, 10f, 10f);
        //        PdfWriter writer = PdfWriter.GetInstance(document, Response.OutputStream);
        //        document.Open();

        //        Font font1 = FontFactory.GetFont(FontFactory.TIMES_ROMAN, 13);
        //        Font font2 = FontFactory.GetFont(FontFactory.TIMES_BOLD, 13);

        //        var logo = iTextSharp.text.Image.GetInstance("https://elearning.hoaphatdungquat.vn/Content/assets/images/logo-inverse.png");
        //        //logo.SetAbsolutePosition(0f,0f);
        //        logo.ScaleAbsoluteHeight(50);
        //        logo.ScaleAbsoluteWidth(219);
        //        document.Add(logo);


        //        float[] columnDefinitionSize = { 2F, 5F, 2F, 5F };
        //        PdfPTable table;
        //        PdfPCell cell;

        //        table = new PdfPTable(columnDefinitionSize)
        //        {
        //            WidthPercentage = 100
        //        };

        //        cell = new PdfPCell
        //        {
        //            BackgroundColor = new BaseColor(0xC0, 0xC0, 0xC0)
        //        };

        //        table.AddCell(new Phrase("STT", font1));
        //        table.AddCell(new Phrase("Nội Dung", font1));
        //        table.AddCell(new Phrase("Đáp án học viên", font1));
        //        table.AddCell(new Phrase("Kết quả", font1));
        //        table.HeaderRows = 1;

        //        foreach (var item in res)
        //        {
        //            table.AddCell(new Phrase(item.GiangVien, font2));
        //            table.AddCell(new Phrase(item.GiangVien, font2));
        //            table.AddCell(new Phrase(item.GiangVien, font2));
        //            table.AddCell(new Phrase(item.GiangVien, font2));

        //            pdfRowIndex++;
        //        }

        //        document.Add(table);
        //        document.Close();
        //        document.CloseDocument();
        //        document.Dispose();
        //        writer.Close();
        //        writer.Dispose();



        //        Response.ContentType = "application/pdf";
        //        Response.AddHeader("content-disposition", "attachment;filename=ImageExport.pdf");
        //        Response.Cache.SetCacheability(HttpCacheability.NoCache);
        //        Response.Write(document);
        //        Response.End();


        //    }
        //    return View();
        //}
        public ActionResult ExportToPdfs(int? LHID, int? IDNV)
        {

                WebClient MyWebClient = new WebClient();
                MyWebClient.Credentials = CredentialCache.DefaultCredentials;
            // Gets or sets the network credentials used to authenticate requests to Internet resources
                string URLWithHTTPandPort = Request.Url.GetLeftPart(UriPartial.Authority);
                Byte[] pageData = MyWebClient.DownloadData(URLWithHTTPandPort + "/ConfirmEStudy/PrintTestView?LHID=" + LHID + "&IDNV=" + IDNV);
                string pageHtml = Encoding.UTF8.GetString(pageData);
                bool isBool = isMessyCode(pageHtml); // Determine which code to use to read web page information
                if (!isBool)
                {
                    string pageHtml1 = Encoding.UTF8.GetString(pageData);
                    pageHtml = pageHtml1;
                }
                else
                {
                    string pageHtml2 = Encoding.Default.GetString(pageData);
                    pageHtml = pageHtml2;
                }
                Byte[] data = Encoding.UTF8.GetBytes(pageHtml);
                MemoryStream msInput = new MemoryStream(data);

                var output = new MemoryStream(); // this MemoryStream is closed by FileStreamResult

                        Document document = new Document(iTextSharp.text.PageSize.A4, 5f, 5f, 10f, 10f);
                        var writer = PdfWriter.GetInstance(document, output);
                        writer.CloseStream = false;
                        document.Open();

                        var xmlWorker = XMLWorkerHelper.GetInstance();
                        xmlWorker.ParseXHtml(writer, document, msInput, null, Encoding.UTF8);
                        document.Close();
                        output.Position = 0;

                        return new FileStreamResult(output, "application/pdf");
                


        }
        public ActionResult ExportToPdfss(int? LHID, int? IDNV)
        {

            WebClient MyWebClient = new WebClient();
            MyWebClient.Credentials = CredentialCache.DefaultCredentials;
            // Gets or sets the network credentials used to authenticate requests to Internet resources
            string URLWithHTTPandPort = Request.Url.GetLeftPart(UriPartial.Authority);
            Byte[] pageData = MyWebClient.DownloadData(URLWithHTTPandPort+"/ConfirmEStudy/PrintTestView?LHID=" + LHID + "&IDNV=" + IDNV);
            string pageHtml = Encoding.UTF8.GetString(pageData);

            StringReader sr = new StringReader(pageHtml);
            Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
            PdfWriter writer = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
            pdfDoc.Open();
            XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
            pdfDoc.Close();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=HTML.pdf");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(pdfDoc);
            Response.End();

            return  View();



        }

        
        public void tempFilePdf(int? LHID, int? IDNV, String path)
        {
            //var root = Server.MapPath("~/PDF/");
            //var pdfname = String.Format("{0}.pdf", Guid.NewGuid().ToString());
            //var path = Path.Combine(root, pdfname);
            //path = Path.GetFullPath(path);
            string html = Server.MapPath("~/Views/Shared/footer.html");
            //string html1 = Server.MapPath("~/Views/Shared/header.html");
            string footer = string.Format(
                        "--footer-html \"{0}\" " +
                        "--footer-spacing \"5\" " +
                        "--footer-font-size \"5\" " +
                        "--footer-line --encoding utf-8", html);
            //string footer = "--footer-right \"Page: [page] of [toPage]\" " + "--footer-center \"Tài liệu này thuộc sở hữu của Hòa Phát Dung Quất. Việc phát tán, sử dụng trái phép bị nghiêm cấm\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"Arial Unicode MS\" --encoding \"utf-8\"";
            var actionPDF = new ActionAsPdf("PrintTestView", new { LHID = LHID, IDNV = IDNV })
            {
                //FileName = "cv.pdf",
                ////PageOrientation = Orientation.Portrait,
                PageSize = Size.A4,
                PageMargins = new Margins(13, 5, 10, 5),
                //PageWidth = 297,
                //PageHeight = 210,
                ////IsLowQuality = true ,
                CustomSwitches = footer,
                MinimumFontSize = 20,
                //SaveOnServerPath = path
            };
            var applicationPDFData = actionPDF.BuildPdf(this.ControllerContext);
            System.IO.File.WriteAllBytes(path, applicationPDFData);

        }
        public void ListtempFilePdf(int? IDL, String path)
        {
            //var root = Server.MapPath("~/PDF/");
            //var pdfname = String.Format("{0}.pdf", Guid.NewGuid().ToString());
            //var path = Path.Combine(root, pdfname);
            //path = Path.GetFullPath(path);
            string html = Server.MapPath("~/Views/Shared/footer.html");
            //string html1 = Server.MapPath("~/Views/Shared/header.html");
            string footer = string.Format(
                        "--footer-html \"{0}\" " +
                        "--footer-spacing \"5\" " +
                        "--footer-font-size \"5\" " +
                        "--footer-line --encoding utf-8", html);
            var actionPDF = new ActionAsPdf("ListPrintTestView", new { id = IDL })
            {
                //FileName = "cv.pdf",
                ////PageOrientation = Orientation.Portrait,
                PageSize = Size.A4,
                PageMargins = new Margins(13, 5, 10, 5),
                //PageWidth = 297,
                //PageHeight = 210,
                ////IsLowQuality = true ,
                CustomSwitches = footer,
                MinimumFontSize = 20,
                //SaveOnServerPath = path
            };
            var applicationPDFData = actionPDF.BuildPdf(this.ControllerContext);
            System.IO.File.WriteAllBytes(path, applicationPDFData);

        }

        public ActionResult ExportToPdfsss(int? LHID, int? IDNV, int? IDL)
        {
            //string footer = "--footer-right \"Date: [date] [time]\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"";
            var root = Server.MapPath("~/PDF/");
            var pdfname = String.Format("{0}.pdf", Guid.NewGuid().ToString());
            var path = Path.Combine(root, pdfname);
            path = Path.GetFullPath(path);
            //var actionPDF = new ActionAsPdf("PrintTestView", new { LHID = LHID, IDNV = IDNV })
            //{
            //    //FileName = "cv.pdf",
            //    ////PageOrientation = Orientation.Portrait,
            //    PageSize = Size.A4,
            //    ////PageMargins = new Margins(10, 10, 10, 0),
            //    //PageWidth = 297,
            //    //PageHeight = 210,
            //    ////IsLowQuality = true ,
            //    CustomSwitches = footer,
            //    MinimumFontSize = 20,
            //    //SaveOnServerPath = path
            //};
            //var applicationPDFData = actionPDF.BuildPdf(this.ControllerContext);
            //System.IO.File.WriteAllBytes(path, applicationPDFData);
            //MemoryStream ms = new MemoryStream(abc);
            //RedirectToAction("tempFilePdf", new { LHID = LHID, IDNV = IDNV, path = path });

            //string WatermarkLocation = Server.MapPath("~/Content/assets/images/WaterMarkElearning.jpg");
            //path = root + "aa.pdf";
            string destinationpath = string.Empty;
            if (IDL == 0) { 
                tempFilePdf(LHID, IDNV, path);
                var tenLH = db_context.LopHocs.Where(x => x.IDLH == LHID).Select(x => x.TenLH).FirstOrDefault();
                // Destination File path
                destinationpath = "HPDQ" + IDNV + "_" + tenLH + ".pdf";
            } else
            {
                ListtempFilePdf(IDL, path);
                var tenLH = db_context.LopHocs.Where(x => x.IDLH == IDL).Select(x => x.TenLH).FirstOrDefault();
                destinationpath = "Lớp "+ tenLH + ".pdf";
            }
            //Source File Path
            string originalFile = path;

            

            // Read file from file location
            PdfReader reader = new PdfReader(originalFile);

            //define font size and style
            Font font = new Font(Font.FontFamily.HELVETICA, 16, Font.NORMAL, BaseColor.LIGHT_GRAY);

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (var pdfStamper = new PdfStamper(reader, memoryStream, '\0'))
                {
                    // Getting total number of pages of the Existing Document
                    int pageCount = reader.NumberOfPages;

                    // Create two New Layer for Watermark
                    PdfLayer layer = new PdfLayer("WatermarkLayer", pdfStamper.Writer);
                    //PdfLayer layer2 = new PdfLayer("WatermarkLayer2", pdfStamper.Writer);

                    // Loop through each Page

                    string layerwarkmarktxt = "E-Learing Hoa Phat Dung Quat - "+ @DateTime.Now.ToString()+ "        -        ";// define text for 
                    //string Layer2warkmarktxt = "Confidential";
                    for (int i = 1; i <= pageCount; i++)
                    {
                        // Getting the Page Size
                        Rectangle rect = reader.GetPageSize(i);
                        // Get the ContentByte object
                        PdfContentByte cb = pdfStamper.GetUnderContent(i);
                        // Tell the cb that the next commands should be "bound" to this new layer
                        // Start Layer
                        cb.BeginLayer(layer);
                        PdfGState gState = new PdfGState();
                        gState.FillOpacity = 0.1f; // define opacity level
                        cb.SetGState(gState);
                        // set font size and style for layer water mark text
                        cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED), 10);
                        List<string> watermarkList = new List<string>();
                        float singleWaterMarkWidth = cb.GetEffectiveStringWidth(layerwarkmarktxt, false);
                        float fontHeight = 10;
                        //Work out the Watermark for a Single Line on the Page based on the Page Width
                        float currentWaterMarkWidth = 0;
                        while (currentWaterMarkWidth + singleWaterMarkWidth < rect.Width)
                        {
                            watermarkList.Add(layerwarkmarktxt);
                            currentWaterMarkWidth = cb.GetEffectiveStringWidth(string.Join(" ", watermarkList), false);
                        }
                        //watermarkList.Add(layerwarkmarktxt);
                        //currentWaterMarkWidth = cb.GetEffectiveStringWidth(string.Join(" ", watermarkList), false);
                        //Fill the Page with Lines of Watermarks
                        float currentYPos = rect.Height;
                        //cb.BeginText();
                        //var font = new Font(Font.FontFamily.HELVETICA, 60, Font.NORMAL, BaseColor.LIGHT_GRAY);
                        //ColumnText.ShowTextAligned(pdfWriter.DirectContent, Element.ALIGN_CENTER, new Phrase("PAID", font), 300, 400, 45);
                        while (currentYPos > 0)
                        {
                            //ColumnText.ShowTextAligned(cb, Element.ALIGN_CENTER, new Phrase(string.Join(" ", watermarkList), font), 550, 400, 45);
                            ColumnText.ShowTextAligned(cb, Element.ALIGN_CENTER, new Phrase(string.Join(" ", watermarkList), font), 310, 420, 55);
                            currentYPos -= fontHeight;
                        }
                        cb.EndLayer();
                    }

                }
                reader.Close();
                // Save file to destination location if required
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
                //System.IO.File.WriteAllBytes(destinationpath, memoryStream.ToArray());
                return File(memoryStream.ToArray(), "application/pdf", destinationpath);
            }

            // send file to browse to open it from destination location.
            

        }


        public void tempFilePdfFBM(int? IDVT, String path)
        {
            //var root = Server.MapPath("~/PDF/");
            //var pdfname = String.Format("{0}.pdf", Guid.NewGuid().ToString());
            //var path = Path.Combine(root, pdfname);
            //path = Path.GetFullPath(path);
            string html = Server.MapPath("~/Views/Shared/footer.html");
            //string html1 = Server.MapPath("~/Views/Shared/header.html");
            string footer = string.Format(
                        "--footer-html \"{0}\" " +
                        "--footer-spacing \"5\" " +
                        "--footer-font-size \"5\" " +
                        "--footer-line --encoding utf-8", html);
            //string footer = "--footer-right \"Page: [page] of [toPage]\" " + "--footer-center \"Tài liệu này thuộc sở hữu của Hòa Phát Dung Quất. Việc phát tán, sử dụng trái phép bị nghiêm cấm\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"Arial Unicode MS\" --encoding \"utf-8\"";
            var actionPDF = new ActionAsPdf("PrintFBMView", new { IDVT = IDVT })
            {
                //FileName = "cv.pdf",
                ////PageOrientation = Orientation.Portrait,
                PageSize = Size.A4,
                PageMargins = new Margins(13, 5, 10, 5),
                //PageWidth = 297,
                //PageHeight = 210,
                ////IsLowQuality = true ,
                CustomSwitches = footer,
                MinimumFontSize = 20,
                //SaveOnServerPath = path
            };
            var applicationPDFData = actionPDF.BuildPdf(this.ControllerContext);
            System.IO.File.WriteAllBytes(path, applicationPDFData);

        }

        public ActionResult ExportToPdfFBMView(int? IDVT)
        {
            //string footer = "--footer-right \"Date: [date] [time]\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"";
            var root = Server.MapPath("~/PDF/");
            var pdfname = String.Format("{0}.pdf", Guid.NewGuid().ToString());
            var path = Path.Combine(root, pdfname);
            path = Path.GetFullPath(path);
            //var actionPDF = new ActionAsPdf("PrintTestView", new { LHID = LHID, IDNV = IDNV })
            //{
            //    //FileName = "cv.pdf",
            //    ////PageOrientation = Orientation.Portrait,
            //    PageSize = Size.A4,
            //    ////PageMargins = new Margins(10, 10, 10, 0),
            //    //PageWidth = 297,
            //    //PageHeight = 210,
            //    ////IsLowQuality = true ,
            //    CustomSwitches = footer,
            //    MinimumFontSize = 20,
            //    //SaveOnServerPath = path
            //};
            //var applicationPDFData = actionPDF.BuildPdf(this.ControllerContext);
            //System.IO.File.WriteAllBytes(path, applicationPDFData);
            //MemoryStream ms = new MemoryStream(abc);
            //RedirectToAction("tempFilePdf", new { LHID = LHID, IDNV = IDNV, path = path });

            //string WatermarkLocation = Server.MapPath("~/Content/assets/images/WaterMarkElearning.jpg");
            //path = root + "aa.pdf";
            string destinationpath = string.Empty;
            //if (IDL == 0)
            //{
            //    tempFilePdf(LHID, IDNV, path);
            //    var tenLH = db_context.LopHocs.Where(x => x.IDLH == LHID).Select(x => x.TenLH).FirstOrDefault();
            //    // Destination File path
            //    destinationpath = "HPDQ" + IDNV + "_" + tenLH + ".pdf";
            //}
            //else
            //{
            //    ListtempFilePdf(IDL, path);
            //    var tenLH = db_context.LopHocs.Where(x => x.IDLH == IDL).Select(x => x.TenLH).FirstOrDefault();
            //    destinationpath = "Lớp " + tenLH + ".pdf";
            //}

            tempFilePdfFBM(IDVT, path);
            var tenLH = db_context.ViTriKNLs.Where(x => x.IDVT == IDVT).Select(x => x.TenViTri).FirstOrDefault();
            // Destination File path
            destinationpath = "HPDQ_" + tenLH + ".pdf";

            //Source File Path
            string originalFile = path;



            // Read file from file location
            PdfReader reader = new PdfReader(originalFile);

            //define font size and style
            Font font = new Font(Font.FontFamily.HELVETICA, 16, Font.NORMAL, BaseColor.LIGHT_GRAY);

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (var pdfStamper = new PdfStamper(reader, memoryStream, '\0'))
                {
                    // Getting total number of pages of the Existing Document
                    int pageCount = reader.NumberOfPages;

                    // Create two New Layer for Watermark
                    PdfLayer layer = new PdfLayer("WatermarkLayer", pdfStamper.Writer);
                    //PdfLayer layer2 = new PdfLayer("WatermarkLayer2", pdfStamper.Writer);

                    // Loop through each Page

                    //string layerwarkmarktxt = "E-Learing Hoa Phat Dung Quat - " + @DateTime.Now.ToString() + "        -        ";// define text for 
                    //                                                                                                             //string Layer2warkmarktxt = "Confidential";
                    string layerwarkmarktxt = "";// define text for 
                                                                                                                                 //string Layer2warkmarktxt = "Confidential";
                    for (int i = 1; i <= pageCount; i++)
                    {
                        // Getting the Page Size
                        Rectangle rect = reader.GetPageSize(i);
                        // Get the ContentByte object
                        PdfContentByte cb = pdfStamper.GetUnderContent(i);
                        // Tell the cb that the next commands should be "bound" to this new layer
                        // Start Layer
                        cb.BeginLayer(layer);
                        PdfGState gState = new PdfGState();
                        gState.FillOpacity = 0.1f; // define opacity level
                        cb.SetGState(gState);
                        // set font size and style for layer water mark text
                        cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED), 10);
                        List<string> watermarkList = new List<string>();
                        float singleWaterMarkWidth = cb.GetEffectiveStringWidth(layerwarkmarktxt, false);
                        float fontHeight = 10;
                        //Work out the Watermark for a Single Line on the Page based on the Page Width
                        float currentWaterMarkWidth = 0;
                        while (currentWaterMarkWidth + singleWaterMarkWidth < rect.Width)
                        {
                            watermarkList.Add(layerwarkmarktxt);
                            currentWaterMarkWidth = cb.GetEffectiveStringWidth(string.Join(" ", watermarkList), false);
                        }
                        //watermarkList.Add(layerwarkmarktxt);
                        //currentWaterMarkWidth = cb.GetEffectiveStringWidth(string.Join(" ", watermarkList), false);
                        //Fill the Page with Lines of Watermarks
                        float currentYPos = rect.Height;
                        //cb.BeginText();
                        //var font = new Font(Font.FontFamily.HELVETICA, 60, Font.NORMAL, BaseColor.LIGHT_GRAY);
                        //ColumnText.ShowTextAligned(pdfWriter.DirectContent, Element.ALIGN_CENTER, new Phrase("PAID", font), 300, 400, 45);
                        while (currentYPos > 0)
                        {
                            //ColumnText.ShowTextAligned(cb, Element.ALIGN_CENTER, new Phrase(string.Join(" ", watermarkList), font), 550, 400, 45);
                            ColumnText.ShowTextAligned(cb, Element.ALIGN_CENTER, new Phrase(string.Join(" ", watermarkList), font), 310, 420, 55);
                            currentYPos -= fontHeight;
                        }
                        cb.EndLayer();
                    }

                }
                reader.Close();
                // Save file to destination location if required
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
                //System.IO.File.WriteAllBytes(destinationpath, memoryStream.ToArray());
                return File(memoryStream.ToArray(), "application/pdf", destinationpath);
            }

            // send file to browse to open it from destination location.


        }
        public bool isMessyCode(string txt)
        {
            var bytes = Encoding.UTF8.GetBytes(txt);            //239 191 189            
            for (var i = 0; i < bytes.Length; i++)
            {
                if (i < bytes.Length - 3)
                    if (bytes[i] == 239 && bytes[i + 1] == 191 && bytes[i + 2] == 189)
                    {
                        return true;
                    }
            }
            return false;
        }

    }
}