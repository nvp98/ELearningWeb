using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing.ChartDrawing;
using E_Learning.Models;
using E_Learning.ModelsQTHD;
using Microsoft.Ajax.Utilities;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_Learning.Controllers.QTHD
{
    public class ResultQTHDController : Controller
    {
        ELEARNINGEntities db = new ELEARNINGEntities();
        int Idquyen = MyAuthentication.IDQuyen;
        String ControllerName = "ResultQTHD";
        // GET: ResultQTHD
        public ActionResult Index(int? page, string search, int? IDQT, int? IDMahieu)
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, "CauHoiQT");
            ViewBag.QUYENCN = ListQuyen;
            if (!ListQuyen.Contains(CONSTKEY.V))
            {
                TempData["msgError"] = "<script>alert('Bạn không có quyền truy cập chức năng này');</script>";
                return RedirectToAction("", "Home");
            }
            if (search == null) search = "";
            ViewBag.search = search;

            if (IDQT == null) IDQT = 0;
            if (IDMahieu == null) IDMahieu = 0;

            var res = (from a in db.QT_CauHoiQT
                       join d in db.QT_NoiDungQT
                       on a.QTHDID equals d.IDQTHD
                       join e in db.QT_CTBaiKiemTra
                       on a.IDCH equals e.IDCauHoi into ul
                       from e in ul.DefaultIfEmpty()
                       select new CauHoiQTListView
                       {
                           IDCH =a.IDCH,
                           NoiDungCH =a.NoiDungCH,
                           DapAnA =a.DapAnA,
                           DapAnB =a.DapAnB,
                           DapAnC =a.DapAnC,
                           DapAnD =a.DapAnD,
                           QTHDID =a.QTHDID,
                           TenQT=d.MaHieu+ " - "+ d.TenQTHD,
                           MaHieu =d.MaHieu,
                           CountBaiThi = ul.Count(),
                           CountDat = ul.Where(x => x.Diem != 0).Count(),
                           CountKhongDat = ul.Where(x => x.Diem == 0).Count(),
                       }).ToList().DistinctBy(x=>x.IDCH);

            //if (!ListQuyen.Contains(CONSTKEY.VIEW_ALL)) res = res.Where(x => x.PhongBanID == MyAuthentication.IDPhongban).ToList();


            List<NoiDungQTView> qt = (from a in  db.QT_NoiDungQT select new NoiDungQTView {IDQTHD =a.IDQTHD,TenQTHD =a.MaHieu+" - "+a.TenQTHD,MaHieu =a.MaHieu}).ToList();
            ViewBag.NoiDungQT = new SelectList(qt, "IDQTHD", "TenQTHD", IDQT);
            ViewBag.MaHieuQT = new SelectList(qt, "IDQTHD", "MaHieu", IDMahieu);

            //if (IDPB != 0) res = res.Where(x => x.PhongBanID == IDPB).ToList();
            if (IDQT != 0) res = res.Where(x => x.QTHDID == IDQT).ToList();
            if (IDMahieu != 0) res = res.Where(x => x.QTHDID == IDMahieu).ToList();

            if (page == null) page = 1;
            int pageSize = 50;
            int pageNumber = (page ?? 1);
            return View(res.ToList().ToPagedList(pageNumber, pageSize));
        }

        public ActionResult ListNV(int? page, string search, int? IDPB, int? IDTinhTrang, int? IDPX, int? IDNhom, int? IDKhoi, int? IDTo, int? IDQT, int? IDMahieu)
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


            List<KNL_Khoi> khoi = db.KNL_Khoi.ToList();
            ViewBag.IDKhoi = new SelectList(khoi, "ID", "TenKhoi");

            List<PhongBan> dt = db.PhongBans.ToList();
            if (ListQuyen.Contains("VIEW_BP")) dt = dt.Where(x => x.IDPhongBan == MyAuthentication.IDPhongban).ToList();
            ViewBag.IDPB = new SelectList(dt, "IDPhongBan", "TenPhongBan");

            if (IDQT == null) IDQT = 0;
            if (IDMahieu == null) IDMahieu = 0;
            if (IDPB == null) IDPB = 0;
            if (ListQuyen.Contains("VIEW_BP")) IDPB = MyAuthentication.IDPhongban;
            //var res = (from a in db.NhanVien_searchByVTKNL(0)
            //           join b in db.ViTriKNLs
            //           on a.IDVT equals b.IDVT
            //           join c in db.QT_PhanQuyen
            //           on b.IDVT equals c.IDVTKNL
            //           select new EmployeeValidation
            //           {
            //              ID =a.ID,
            //              HoTen =a.HoTen,
            //              MaNV =a.MaNV,

            //           }).ToList().DistinctBy(x => x.MaNV);

            var res = (from a in db.QT_Result_searchByIDQT(IDPB)
                       select new ResultQTHDView
                       {
                           IDNV = a.IDNV,
                           HoTen = a.HoTen,
                           MaNV = a.MaNV,
                           diem = a.Diem,
                           IDDK = a.DKID,
                           IDQTHD =a.QTHDID,
                           IDVTKNL =a.IDVTKNL,
                           MaHieu = a.MaHieu,
                           NgaHT =a.NgayHT,
                           NgayKTTT = a.NgayKTTT,
                           //NgayKT =a.NgayKT,
                           TenQTHD =a.TenQTHD,
                           TenVTKNL = a.TenViTri,
                           TinhTrang = a.TinhTrang,
                           TenPhongBan = a.TenPhongBan,
                           IDPB = a.IDPhongBan,
                           MaViTri =a.MaViTri,
                           NgayHieuLuc =a.NgayHieuLuc,
                           NgayHetHieuLuc  =a.NgayHetHieuLuc,
                           TinhTrangHL  = a.NgayHieuLuc < DateTime.Now && (a.NgayHetHieuLuc == null || a.NgayHetHieuLuc == default || a.NgayHetHieuLuc > DateTime.Now) ? 1 : a.NgayHieuLuc > DateTime.Now ? 2 : 0,
                           TinhTrangKT = a.DKID != 1 && DateTime.Now > a.NgayKTTT ? 1 : 0
                       }).ToList().Where(x=>x.TinhTrangHL ==1).OrderBy(x=>x.MaNV);


            //if (!ListQuyen.Contains(CONSTKEY.VIEW_ALL)) res = res.Where(x => x.PhongBanID == MyAuthentication.IDPhongban).ToList();

            //if (IDPB != 0) res = res.Where(x => x.IDPB == IDPB).ToList();
            List<NoiDungQTView> qt = (from a in db.QT_NoiDungQT select new NoiDungQTView { IDQTHD = a.IDQTHD, TenQTHD = a.MaHieu + " - " + a.TenQTHD, MaHieu = a.MaHieu }).ToList();
            ViewBag.NoiDungQT = new SelectList(qt, "IDQTHD", "TenQTHD", IDQT);
            ViewBag.MaHieuQT = new SelectList(qt, "IDQTHD", "MaHieu", IDMahieu);

            if (IDPB != 0) res = res.Where(x => x.IDPB == IDPB).ToList().OrderBy(x => x.MaNV);
            if (search != "") res = res.Where(x => x.MaNV == search).ToList().OrderBy(x=>x.MaNV);
            if (IDMahieu != 0) res = res.Where(x => x.IDQTHD == IDMahieu).ToList().OrderBy(x => x.MaNV);
            if (IDQT != 0) res = res.Where(x => x.IDQTHD == IDQT).ToList().OrderBy(x => x.MaNV);
            if (IDTinhTrang != null && IDTinhTrang ==1) res = res.Where(x => x.TinhTrang == IDTinhTrang && x.TinhTrangKT ==0).ToList().OrderBy(x => x.MaNV);
            else if(IDTinhTrang != null && IDTinhTrang == 0) res = res.Where(x => x.TinhTrang == IDTinhTrang).ToList().OrderBy(x => x.MaNV);
            else if(IDTinhTrang ==3 && IDTinhTrang != null) res = res.Where(x => x.TinhTrang == null).ToList().OrderBy(x => x.MaNV);

            if (page == null) page = 1;
            int pageSize = 50;
            int pageNumber = (page ?? 1);
            return View(res.ToList().ToPagedList(pageNumber, pageSize));
        }


        public ActionResult ExportData(int? IDPB, int? IDQT, int? IDMahieu)
        {
            try
            {
                var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
                string fileNamemau = AppDomain.CurrentDomain.BaseDirectory + @"App_Data\DS_KetQuaQTHD.xlsx";
                string fileNamemaunew = AppDomain.CurrentDomain.BaseDirectory + @"App_Data\DS_KetQuaQTHD_Temp.xlsx";
                XLWorkbook Workbook = new XLWorkbook(fileNamemau);
                IXLWorksheet Worksheet = Workbook.Worksheet("Data");

                if (IDPB == null) IDPB = 0;
                if (ListQuyen.Contains("VIEW_BP")) IDPB = MyAuthentication.IDPhongban;
                List<ResultQTHDView> DataKNL = ListResult(IDPB,IDQT,IDMahieu);
                int row = 2;
                if (DataKNL.Count > 0)
                {
                    foreach (var data in DataKNL)
                    {
                        int dk = (int)db.QT_DinhKy.Where(x => x.IDDK == data.IDDK).FirstOrDefault().MaDinhKy;
                        Worksheet.Cell(row, "A").Value = row - 1;
                        Worksheet.Cell(row, "A").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        Worksheet.Cell(row, "A").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "A").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "B").Value = data.MaNV;
                        Worksheet.Cell(row, "B").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "B").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "B").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "C").Value = data.HoTen;
                        Worksheet.Cell(row, "C").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "C").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "C").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "D").Value = data.TenPhongBan;
                        Worksheet.Cell(row, "D").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "D").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "D").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "E").Value = data.TenVTKNL;
                        Worksheet.Cell(row, "E").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "E").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "E").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "F").Value = data.MaHieu+" - "+ data.TenQTHD;
                        Worksheet.Cell(row, "F").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "F").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "F").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;


                        Worksheet.Cell(row, "G").Value = data.TinhTrang == 1 && data.TinhTrangKT ==0 ? "Hoàn thành" : data.TinhTrangKT == 1?  "Chưa làm lại bài thi":"Chưa hoàn thành";
                        Worksheet.Cell(row, "G").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        Worksheet.Cell(row, "G").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "G").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        //Worksheet.Cell(row, "F").Style.Fill.BackgroundColor = XLColor.Yellow;

                        Worksheet.Cell(row, "H").Value = data.diem != null && data.NgaHT != null && data.TinhTrangKT ==0 ?data.diem.ToString():"";
                        Worksheet.Cell(row, "H").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        Worksheet.Cell(row, "H").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "H").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        //Worksheet.Cell(row, "H").Style.DateFormat.Format = "dd/MM/yyyy";

                        Worksheet.Cell(row, "I").Value = data.NgaHT != null ?data.NgaHT.ToString():"";
                        Worksheet.Cell(row, "I").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        Worksheet.Cell(row, "I").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "I").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        Worksheet.Cell(row, "I").Style.DateFormat.Format = "dd/MM/yyyy";

                        Worksheet.Cell(row, "J").Value = data.NgaHT != null ? ((DateTime)data.NgaHT).AddMonths(dk).ToString() : "";
                        Worksheet.Cell(row, "J").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        Worksheet.Cell(row, "J").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "J").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        Worksheet.Cell(row, "J").Style.DateFormat.Format = "dd/MM/yyyy";

                        row = row + 1;
                    }

                    Workbook.SaveAs(fileNamemaunew);
                    byte[] fileBytes = System.IO.File.ReadAllBytes(fileNamemaunew);
                    string fileName = "ThongKe_DS_ResultQTHD - " + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
                }
                else
                {

                    //Worksheet.Cell("A1").Value = "Ngày xuất file: " + DateTime.Now.Date.ToString("dd/MM/yyyy");
                    Workbook.SaveAs(fileNamemaunew);
                    byte[] fileBytes = System.IO.File.ReadAllBytes(fileNamemaunew);
                    string fileName = "ThongKe_DS_ResultQTHD - " + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
                }

            }
            catch (Exception ex)
            {
                TempData["msg"] = "<script>alert('" + ex + "');window.location.href = '/Everyday/'</script>";
                return View(TempData);
            }
        }
        public List<ResultQTHDView> ListResult(int? IDPB, int? IDQT, int? IDMahieu)
        {
            //var phongban = db.PhongBans.ToList();
            //var lvdt = db.LinhVucDTs.ToList();
            if (IDQT == null) IDQT = 0;
            if (IDMahieu == null) IDMahieu = 0;

            var res = (from a in db.QT_Result_searchByIDQT(IDPB)
                       select new ResultQTHDView
                       {
                           IDNV = a.IDNV,
                           HoTen = a.HoTen,
                           MaNV = a.MaNV,
                           diem = a.Diem,
                           IDDK = a.DKID,
                           IDQTHD = a.QTHDID,
                           IDVTKNL = a.IDVTKNL,
                           MaHieu = a.MaHieu,
                           NgaHT = a.NgayHT,
                           NgayKTTT = a.NgayKTTT,
                           TenQTHD = a.TenQTHD,
                           TenVTKNL = a.TenViTri,
                           TinhTrang = a.TinhTrang,
                           TenPhongBan = a.TenPhongBan,
                           IDPB = a.IDPhongBan,
                           MaViTri = a.MaViTri,
                           NgayHieuLuc = a.NgayHieuLuc,
                           NgayHetHieuLuc = a.NgayHetHieuLuc,
                           TinhTrangHL = a.NgayHieuLuc < DateTime.Now && (a.NgayHetHieuLuc == null || a.NgayHetHieuLuc == default || a.NgayHetHieuLuc > DateTime.Now) ? 1 : a.NgayHieuLuc > DateTime.Now ? 2 : 0,
                           TinhTrangKT = a.DKID != 1 && a.NgayHT != null && DateTime.Now > ((DateTime)a.NgayHT).AddMonths((int)db.QT_DinhKy.Where(x=>x.IDDK == a.DKID).FirstOrDefault().MaDinhKy) ?1:0 
                       }).ToList().Where(x => x.TinhTrangHL == 1).OrderBy(x => x.MaNV).OrderBy(x=>x.IDPB);

            if (IDMahieu != 0) res = res.Where(x => x.IDQTHD == IDMahieu).ToList().OrderBy(x => x.MaNV).OrderBy(x => x.IDPB);
            if (IDQT != 0) res = res.Where(x => x.IDQTHD == IDQT).ToList().OrderBy(x => x.MaNV).OrderBy(x => x.IDPB);

            return res.ToList();
        }


    }
}