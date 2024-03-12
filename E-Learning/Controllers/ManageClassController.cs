using PagedList;
using E_Learning.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.IO;
using System.Linq;
using ExcelDataReader;
using ClosedXML.Excel;
using System.Globalization;
using System.Data;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace E_Learning.Controllers
{
    public class ManageClassController : Controller
    {
        ELEARNINGEntities db_context = new ELEARNINGEntities();
        int Idquyen = MyAuthentication.IDQuyen;
        String ControllerName = "ManageClass";
        //GET: Class
        public ActionResult Index(int? page, int?  IDLH, int? IDPB, int? IDND)
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            ViewBag.QUYENCN = ListQuyen;
            //if (!ListQuyen.Contains(CONSTKEY.V))
            //{
            //    TempData["msgError"] = "<script>alert('Bạn không có quyền truy cập chức năng này');</script>";
            //    return RedirectToAction("", "Home");
            //}
            if (User.Identity.IsAuthenticated)
            {

                int id = MyAuthentication.ID;
                //if (MyAuthentication.IDQuyen == 4)
                if (!ListQuyen.Contains(CONSTKEY.V))
                {
                    TempData["msgError"] = "<script>alert('Bạn không có quyền truy cập chức năng này');</script>";
                    return RedirectToAction("", "Home");
                }
                else
                {
                    if (IDLH == null) IDLH = 0;
                    if (IDPB == null) IDPB = 0;
                    if (IDND == null) IDND = 0;
                    var res = new List<ManageClassValidation>();
                    if (IDPB != 0)
                    {
                        res = (from l in db_context.LopHocs
                               join n in db_context.NoiDungDTs on l.NDID equals n.IDND
                               join g in db_context.NhanViens on l.GVID equals g.ID
                               join hv in db_context.XNHocTaps on l.IDLH equals hv.LHID into LopHocHV
                               join d in db_context.DeThis on l.IDDeThi equals d.IDDeThi into uli
                               from d in uli.DefaultIfEmpty()
                               join x in db_context.BaiThis.Where(x => x.TinhTrang == true) on l.IDLH equals x.IDLH into LopHocHVHT
                               select new ManageClassValidation
                               {
                                   IDLH = l.IDLH,
                                   MaLH = l.MaLH,
                                   TenLH = l.TenLH,
                                   NDID = n.IDND,
                                   MaND = n.MaND,
                                   TenND = n.NoiDung,
                                   LinhVuc = n.LinhVucDT.TenLVDT,
                                   VideoLH = n.VideoND,
                                   ImageLH = n.ImageND,
                                   QuyDT = (int)l.QuyDT,
                                   NamDT = (int)l.NamDT,
                                   TGBDLH = (DateTime)l.TGBDLH,
                                   TGKTLH = (DateTime)l.TGKTLH,
                                   IDDeThi = (int)l.IDDeThi,
                                   TenDeThi = d.TenDe,
                                   GVID = g.ID,
                                   TenGV = g.HoTen,
                                   MaGV = g.MaNV,
                                   TenPhongBan = g.PhongBan.TenPhongBan,
                                   IsGV = g.IsGV,
                                   SLHT = LopHocHVHT.Count(),
                                   TSLHV = LopHocHV.Count(),
                                   IDPB = g.IDPhongBan
                               }).Where(x=>x.IDPB == IDPB).ToList();
                    }
                    else
                    {
                        res = (from l in db_context.LopHocs
                               join n in db_context.NoiDungDTs on l.NDID equals n.IDND
                               join g in db_context.NhanViens on l.GVID equals g.ID
                               join hv in db_context.XNHocTaps on l.IDLH equals hv.LHID into LopHocHV
                               join d in db_context.DeThis on l.IDDeThi equals d.IDDeThi into uli
                               from d in uli.DefaultIfEmpty()
                               join x in db_context.BaiThis.Where(x => x.TinhTrang == true) on l.IDLH equals x.IDLH into LopHocHVHT
                               select new ManageClassValidation
                               {
                                   IDLH = l.IDLH,
                                   MaLH = l.MaLH,
                                   TenLH = l.TenLH,
                                   NDID = n.IDND,
                                   MaND = n.MaND,
                                   TenND = n.NoiDung,
                                   LinhVuc = n.LinhVucDT.TenLVDT,
                                   VideoLH = n.VideoND,
                                   ImageLH = n.ImageND,
                                   QuyDT = (int)l.QuyDT,
                                   NamDT = (int)l.NamDT,
                                   TGBDLH = (DateTime)l.TGBDLH,
                                   TGKTLH = (DateTime)l.TGKTLH,
                                   IDDeThi = (int)l.IDDeThi,
                                   TenDeThi = d.TenDe,
                                   GVID = g.ID,
                                   TenGV = g.HoTen,
                                   MaGV = g.MaNV,
                                   TenPhongBan = g.PhongBan.TenPhongBan,
                                   IsGV = g.IsGV,
                                   SLHT = LopHocHVHT.Count(),
                                   TSLHV = LopHocHV.Count(),
                                   IDPB = g.IDPhongBan
                               }).ToList();
                    }

                    //if (MyAuthentication.IDQuyen == 2 || MyAuthentication.IDQuyen == 3)
                    if(ListQuyen.Contains(CONSTKEY.V_GV))
                    {
                        res = res.Where(x => x.GVID == id).ToList();
                    }

                    List<PhongBan> dt = db_context.PhongBans.ToList();
                    ViewBag.IDPB = new SelectList(dt, "IDPhongBan", "TenPhongBan");

                    List<LopHoc> lh = db_context.LopHocs.ToList();
                    ViewBag.IDLH = new SelectList(lh, "IDLH", "TenLH");

                    List<NoiDungDT> nd = db_context.NoiDungDTs.ToList();
                    ViewBag.IDND = new SelectList(nd, "IDND", "NoiDung");

                    //if (IDPB != 0) res = res.Where(x => x.IDPB == IDPB).ToList();
                    if (IDLH != 0) res = res.Where(x => x.IDLH == IDLH).ToList();
                    if (IDND != 0) res = res.Where(x => x.NDID == IDND).ToList();
                    if (page == null) page = 1;
                    int pageSize = 20;
                    int pageNumber = (page ?? 1);
                    return View(res.ToList().OrderByDescending(x=>x.TGBDLH).ToPagedList(pageNumber, pageSize));
                }

            }
            else
            {
                return RedirectToAction("", "Login");
            }
            
        }

        public ActionResult Create()
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            if (!ListQuyen.Contains(CONSTKEY.ADD))
            {
                TempData["msgError"] = "<script>alert('Bạn không có quyền thực hiện chức năng này');</script>";
                return RedirectToAction("", "Home");
            }

            List<NoiDungDT> nd = db_context.NoiDungDTs.ToList();
            ViewBag.NDList = new SelectList(nd, "IDND", "NoiDung");

            var lastRecord = (from c in db_context.LopHocs orderby c.IDLH descending select c).FirstOrDefault();
            if (lastRecord == null)
            {
                ViewBag.MaLH = "LH0000" + 1;
            }
            else if (Convert.ToInt32(lastRecord.IDLH) < 9)
            {
                ViewBag.MaLH = "LH0000" + (Convert.ToInt32(lastRecord.IDLH) + 1);
            }
            else if (Convert.ToInt32(lastRecord.IDLH) < 99)
            {
                ViewBag.MaLH = "LH000" + (Convert.ToInt32(lastRecord.IDLH) + 1);
            }
            else if (Convert.ToInt32(lastRecord.IDLH) < 999)
            {
                ViewBag.MaLH = "LH00" + (Convert.ToInt32(lastRecord.IDLH) + 1);
            }
            else if (Convert.ToInt32(lastRecord.IDLH) < 9999)
            {
                ViewBag.MaLH = "LH0" + (Convert.ToInt32(lastRecord.IDLH) + 1);
            }
            else
            {
                ViewBag.MaLH = "LH" + (Convert.ToInt32(lastRecord.IDLH) + 1);
            }

            return PartialView();
        }

        [HttpPost]
        public ActionResult Create(ManageClassValidation _DO)
        {
            if (User.Identity.IsAuthenticated)
            {

                var GVID = MyAuthentication.ID;
                //var GVID = id;
                try
                {
                    // Check trùng mã lớp học
                    if (IsLHAvailable(_DO.MaLH) == false)
                    {

                        db_context.LopHoc_insert(_DO.MaLH, _DO.TenLH, _DO.NDID, _DO.QuyDT, _DO.NamDT, _DO.TGBDLH, _DO.TGKTLH, GVID, _DO.IDDeThi);
                        // Import Excel


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
                                        var MaLH = _DO.MaLH.ToString();
                                        var LHID = db_context.LopHocs.Where(x => x.MaLH == MaLH).Select(g => g.IDLH).FirstOrDefault();
                                        if (IsLHAvailable(MaLH) == true)
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
                                                    if (IsHVAvailable(MaLH, MaNV) == false)
                                                    {
                                                        if (CheckMaNV(MaNV))
                                                        {
                                                            InsertDSHocTap(NVID, LHID, NgayTG, NgayHT, XNTG, XNHT, PBID, VTID);
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
                                        else
                                        {
                                            TempData["msgError"] = "<script>alert('Mã lớp học không đúng. Vui lòng kiểm tra lại.');</script>";
                                        }
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

                        // Import Excel

                        TempData["msgSuccess"] = "<script>alert('Thêm mới thành công');</script>";
                    }
                    else
                    {
                        TempData["msgSuccess"] = "<script>alert('Lớp học đã tồn tại');</script>";
                    }
                }
                catch (Exception e)
                {
                    TempData["msgError"] = "<script>alert('Có lỗi khi thêm mới lớp học: " + e.Message + "');</script>";
                }
                return RedirectToAction("Index", "ManageClass");
            }
            else { return RedirectToAction("", "Login"); }
        }




        public bool IsLHAvailable(string MaLH)
        {
            var IsCheck = (from l in db_context.LopHocs
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


        public bool IsHVAvailable(string MaLH, string MaNV)
        {
            var IsCheck = (from h in db_context.XNHocTaps
                           join l in db_context.LopHocs
                           on h.LHID equals l.IDLH
                           join n in db_context.NhanViens
                           on h.NVID equals n.ID
                           where (l.MaLH.ToLower() == MaLH && n.MaNV.ToLower() == MaNV)
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


        public void InsertDSHocTap(int NVID, int LHID, DateTime NgayTG, DateTime NgayHT, bool XNTG, bool XNHT, int? PBID, int? VTID)
        {
            db_context.XNHocTap_insert(NVID, LHID, NgayTG, NgayHT, XNTG, XNHT, PBID, VTID);
        }

        public ActionResult Edit(int id)
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            if (!ListQuyen.Contains(CONSTKEY.EDIT))
            {
                TempData["msgError"] = "<script>alert('Bạn không có quyền thực hiện chức năng này');</script>";
                return RedirectToAction("", "Home");
            }
            var res = (from l in db_context.LopHoc_searchByID(id)
                       select new ManageClassValidation
                       {
                           IDLH = l.IDLH,
                           MaLH = l.MaLH,
                           TenLH = l.TenLH,
                           NDID = (int)l.NDID,
                           QuyDT = (int)l.QuyDT,
                           NamDT = (int)l.NamDT,
                           TGBDLH = (DateTime)l.TGBDLH,
                           TGKTLH = (DateTime)l.TGKTLH,
                           IDDeThi = (int)l.IDDeThi
                       }).ToList();
            ManageClassValidation DO = new ManageClassValidation();

            if (res.Count > 0)
            {
                foreach (var co in res)
                {
                    DO.IDLH = co.IDLH;
                    DO.MaLH = co.MaLH;
                    DO.TenLH = co.TenLH;
                    DO.NDID = co.NDID;
                    DO.QuyDT = co.QuyDT;
                    DO.NamDT = co.NamDT;
                    DO.TGBDLH = co.TGBDLH;
                    DO.TGKTLH = co.TGKTLH;
                    DO.IDDeThi = co.IDDeThi;
                }

                List<NoiDungDT> nd = db_context.NoiDungDTs.ToList();
                ViewBag.NDList = new SelectList(nd, "IDND", "NoiDung");

                List<DeThi> dt = db_context.DeThis.Where(x => x.IDND == DO.NDID).ToList();
                ViewBag.DeThiList = new SelectList(dt, "IDDeThi", "TenDe", DO.IDDeThi);

                ViewBag.TGBDLH = DO.TGBDLH.ToString("yyyy-MM-dd");
                ViewBag.TGKTLH = DO.TGKTLH.ToString("yyyy-MM-dd");
            }
            else
            {
                return HttpNotFound();
            }
            return PartialView(DO);
        }

        [HttpPost]

        public ActionResult Edit(ManageClassValidation _DO)
        {
            //var idgv = db_context.LopHocs.Where(x => x.IDLH == _DO.IDLH).Select(g => g.GVID).FirstOrDefault();
            try
            {
                db_context.LopHoc_update(_DO.IDLH, _DO.MaLH, _DO.TenLH, _DO.NDID, _DO.QuyDT, _DO.NamDT, _DO.TGBDLH, _DO.TGKTLH, _DO.IDDeThi);

                TempData["msgSuccess"] = "<script>alert('Cập nhập thành công');</script>";
            }
            catch (Exception e)
            {
                TempData["msgSuccess"] = "<script>alert('Cập nhập thất bại " + e.Message + " ');</script>";
            }

            return RedirectToAction("Index", "ManageClass"/*, new { id = idgv}*/);
        }



        public ActionResult Delete(int id)
        {
            //var idgv = db_context.LopHocs.Where(x => x.IDLH == id).Select(g => g.GVID).FirstOrDefault();
            try
            {
                db_context.LopHoc_delete(id);
            }
            catch (Exception e)
            {
                TempData["msgSuccess"] = "<script>alert('Xóa dữ liệu thất bại: " + e.Message + "');</script>";
            }
            return RedirectToAction("Index", "ManageClass"/*, new { id = idgv}*/);
        }


        public ActionResult ExportToExcelLH()
        {
            try
            {
                string fileNameMau = AppDomain.CurrentDomain.BaseDirectory + @"App_Data\BM_LH.xlsx";
                string fileNamemaunew = AppDomain.CurrentDomain.BaseDirectory + @"App_Data\BM_LH_Temp.xlsx";
                XLWorkbook Workbook = new XLWorkbook(fileNameMau);
                IXLWorksheet Worksheet = Workbook.Worksheet("LH");
                List<ManageClassValidation> DataLH = GetDSLopHoc();
                int row = 4;
                if (DataLH.Count > 0)
                {
                    foreach (var data in DataLH)
                    {
                        Worksheet.Cell(row, "A").Value = row - 3;
                        Worksheet.Cell(row, "A").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        Worksheet.Cell(row, "A").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "A").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "B").Value = data.MaLH;
                        Worksheet.Cell(row, "B").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "B").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "B").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "C").Value = data.TenLH;
                        Worksheet.Cell(row, "C").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "C").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "C").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "D").Value = data.LinhVuc;
                        Worksheet.Cell(row, "D").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "D").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "D").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "E").Value = data.TenND;
                        Worksheet.Cell(row, "E").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "E").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "E").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "F").Value = data.QuyDT;
                        Worksheet.Cell(row, "F").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        Worksheet.Cell(row, "F").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "F").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "G").Value = data.NamDT;
                        Worksheet.Cell(row, "G").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "G").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "G").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "H").Value = data.TGBDLH;
                        Worksheet.Cell(row, "H").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        Worksheet.Cell(row, "H").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "H").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "I").Value = data.TGKTLH;
                        Worksheet.Cell(row, "I").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        Worksheet.Cell(row, "I").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "I").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "J").Value = data.TenDeThi;
                        Worksheet.Cell(row, "J").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "J").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "J").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        Worksheet.Cell(row, "J").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "K").Value = data.SLHT;
                        Worksheet.Cell(row, "K").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        Worksheet.Cell(row, "K").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "K").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "L").Value = data.TenGV;
                        Worksheet.Cell(row, "L").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "L").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "L").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "M").Value = data.TenPhongBan;
                        Worksheet.Cell(row, "M").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "M").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "M").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        Worksheet.Cell(row, "M").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        row = row + 1;
                    }
                }

                Workbook.SaveAs(fileNamemaunew);
                byte[] fileBytes = System.IO.File.ReadAllBytes(fileNamemaunew);
                //byte[] fileBytes = System.IO.File.ReadAllBytes(fileNameMau);
                string fileName = "DSLopHoc_" + DateTime.Now.Date.ToString("dd/MM/yyyy") + ".xlsx";
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            catch (Exception ex)
            {
                TempData["msg"] = "<script>alert('" + ex + "');window.location.href = '/ManageClass'</script>";
                return RedirectToAction("Index", "ManageClass");
            }

        }
        private List<ManageClassValidation> GetDSLopHoc()
        {
            var res = (from l in db_context.LopHocs
                       join n in db_context.NoiDungDTs on l.NDID equals n.IDND
                       join g in db_context.NhanViens on l.GVID equals g.ID
                       join hv in db_context.XNHocTaps on l.IDLH equals hv.LHID into LopHocHV
                       join d in db_context.DeThis on l.IDDeThi equals d.IDDeThi
                       join x in db_context.BaiThis.Where(x => x.TinhTrang == true) on l.IDLH equals x.IDLH into LopHocHVHT
                       select new ManageClassValidation
                       {
                           IDLH = l.IDLH,
                           MaLH = l.MaLH,
                           TenLH = l.TenLH,
                           NDID = n.IDND,
                           MaND = n.MaND,
                           TenND = n.NoiDung,
                           LinhVuc = n.LinhVucDT.TenLVDT,
                           VideoLH = n.VideoND,
                           ImageLH = n.ImageND,
                           QuyDT = (int)l.QuyDT,
                           NamDT = (int)l.NamDT,
                           TGBDLH = (DateTime)l.TGBDLH,
                           TGKTLH = (DateTime)l.TGKTLH,
                           IDDeThi = (int)l.IDDeThi,
                           TenDeThi = d.TenDe,
                           GVID = g.ID,
                           TenGV = g.HoTen,
                           MaGV = g.MaNV,
                           TenPhongBan = g.PhongBan.TenPhongBan,
                           IsGV = g.IsGV,
                           SLHT = LopHocHVHT.Count(),
                           TSLHV = LopHocHV.Count(),
                           IDPB = g.IDPhongBan
                       }).OrderBy(x => x.IDPB).ToList();
            return res;
        }


        public ActionResult ExportToExcel()
        {
            try
            {
                string fileNameMau = AppDomain.CurrentDomain.BaseDirectory + @"App_Data\BM_DSHT.xlsx";
                XLWorkbook Workbook = new XLWorkbook(fileNameMau);
                IXLWorksheet Worksheet = Workbook.Worksheet("DSHT");

                byte[] fileBytes = System.IO.File.ReadAllBytes(fileNameMau);
                string fileName = "BM_DSHT.xlsx";
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            catch (Exception ex)
            {
                TempData["msg"] = "<script>alert('" + ex + "');window.location.href = '/ManageClass'</script>";
                return RedirectToAction("Index", "ManageClass");
            }

        }



        public JsonResult GetDeThi(int id)
        {
            List<ManageTestExamValidation> DeThiList = (from d in db_context.DeThis.Where(x => x.IDND == id)
                                                        join b in db_context.NoiDungDTs on d.IDND equals b.IDND
                                                        select new ManageTestExamValidation()
                                                        {
                                                            IDDeThi = d.IDDeThi,
                                                            TenDe = d.TenDe
                                                        }).ToList();

            return Json(DeThiList, JsonRequestBehavior.AllowGet);
        }




    }
}


