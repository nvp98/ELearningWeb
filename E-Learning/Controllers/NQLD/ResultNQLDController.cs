using ClosedXML.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using E_Learning.Models;
using E_Learning.ModelsNQLD;
using E_Learning.ModelsQTHD;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_Learning.Controllers.NQLD
{
    public class ResultNQLDController : Controller
    {
        ELEARNINGEntities db = new ELEARNINGEntities();
        int Idquyen = MyAuthentication.IDQuyen;
        String ControllerName = "ResultNQLD";
        // GET: ResultNQLD
        public ActionResult Index(int? page, string search, int? IDND, int? IDNV, int? IDPB )
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
            if (IDND == null) IDND = 0;
            if (IDNV == null) IDNV = 0;
            if (IDPB == null) IDPB = 0;
            var res = (from a in db.NhanViens.Where(x=>x.IDTinhTrangLV ==1)
                       join d in db.PhongBans
                       on a.IDPhongBan equals d.IDPhongBan
                       join b in db.Vitris on a.IDViTri equals b.IDViTri
                       select new ResultNQLDView
                       {
                           IDNV = a.ID,
                           HoTen = a.HoTen,
                           IDPhongBan =d.IDPhongBan,
                           PhongBan =d.TenPhongBan,
                           MaNV = a.MaNV,
                           
                           ListKQuaNQLD = (from aa in db.NoiDungDTs.Where(x => x.isNQ  == 1)
                                           join k in db.NQ_KetQua.Where(x=>x.IDNV == a.ID) on aa.IDND equals k.NDDTID into ul
                                           from k in ul.DefaultIfEmpty()
                                           select new KQuaNQ
                                            {
                                                IDChuong = aa.isOrder,
                                                NDDTID = aa.IDND,
                                                TinhTrang = k.TinhTrang,
                                                NgayHT = k.NgayHT,
                                                NgayTG = k.NgayTG,
                                                XNHT = k.XNHT,
                                                XNHTFile = k.XNHTFile,
                                                XNTG = k.XNTG,
                                            }).OrderBy(x => x.IDChuong).ToList(),
                       }).OrderBy(x => x.IDPhongBan).ToList();

            List<NoiDungDT> ctlvdt = db.NoiDungDTs.Where(x => x.isNQ == 1).OrderBy(x => x.isOrder).ToList();
            ViewBag.IDNQLD = new SelectList(ctlvdt, "IDND", "NoiDung");

            List<PhongBan> phongban = db.PhongBans.ToList();
            ViewBag.IDPB = new SelectList(phongban, "IDPhongBan", "TenPhongBan");


            List<NhanVien> nhanvien = db.NhanViens.Where(x => x.IDTinhTrangLV == 1).ToList();
            var nv3 = nhanvien.Select(x => new EmployeeValidation { ID = x.ID, HoTen = x.MaNV + " - " + x.HoTen }).ToList();
            ViewBag.LisNV = new SelectList(nv3, "ID", "HoTen");

            if (IDNV != 0) {
                res = res.Where(x => x.IDNV == IDNV).ToList();
                ViewBag.LisNV = new SelectList(nv3, "ID", "HoTen", IDNV);
            }
            if (IDPB != 0) { ViewBag.IDPB = new SelectList(phongban, "IDPhongBan", "TenPhongBan",IDPB); res = res.Where(x => x.IDPhongBan == IDPB).ToList(); }
            if (search != "") res = res.Where(x => x.MaNV == search).ToList();

            //if (IDND != 0) res = res.Where(x => x.ND == IDNQLD).ToList();
            List<string> columnHeaders = new List<string> { };
            var listND = db.NoiDungDTs.Where(x => x.isNQ == 1).OrderBy(x=>x.isOrder).ToList();
            foreach (var item in listND)
            {
                columnHeaders.Add("Chương "+item.isOrder);
            }
            ViewBag.ColumHeader = columnHeaders;
            if (page == null) page = 1;
            int pageSize = 50;
            int pageNumber = (page ?? 1);
            return View(res.ToList().ToPagedList(pageNumber, pageSize));
        }
        public ActionResult ResultView(int? page, string search, int? IDND, int? IDNV)
        {
            var res = (from a in db.NoiDungDTs.Where(x => x.isNQ == 1)
                       join b in db.NQ_KetQua.Where(x => x.IDNV == IDNV) on a.IDND equals b.NDDTID into ul
                       from b in ul.DefaultIfEmpty()
                       select new NoiQuyKQView
                       {
                           IDND = a.IDND,
                           MaND = a.MaND,
                           NoiDung = a.NoiDung,
                           VideoND = a.VideoND,
                           ImageND = a.ImageND,
                           ThoiLuongDT = (int)a.ThoiLuongDT,
                           FileDinhKem = a.FileDinhKem,
                           NgayTao = a.NgayTao,
                           isOrder = a.isOrder,
                           XNTG = b.XNTG,
                           XNHT = b.XNHT,
                           XNHTFile = b.XNHTFile,
                           NgayHT = b.NgayHT,
                           NgayTG = b.NgayTG,
                           TinhTrang = b.TinhTrang,
                           TTNV = (from aa in db.NhanViens.Where(x => x.IDTinhTrangLV == 1 && x.ID == IDNV)
                                   join d in db.PhongBans
                                    on aa.IDPhongBan equals d.IDPhongBan
                                   join bb in db.Vitris on aa.IDViTri equals bb.IDViTri
                                   select new ResultNQLDView
                                   {
                                             MaNV = aa.MaNV,
                                             HoTen =aa.HoTen,
                                             IDNV =aa.ID,
                                             PhongBan =d.TenPhongBan
                                           }).FirstOrDefault(),
                       }).OrderBy(x => x.isOrder).ToList();

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
                string fileNamemau = AppDomain.CurrentDomain.BaseDirectory + @"App_Data\DS_KetQuaNQLD.xlsx";
                string fileNamemaunew = AppDomain.CurrentDomain.BaseDirectory + @"App_Data\DS_KetQuaNQLD_Temp.xlsx";
                XLWorkbook Workbook = new XLWorkbook(fileNamemau);
                IXLWorksheet Worksheet = Workbook.Worksheet("Data");

                //header
                List<string> columnHeaders = new List<string> { };
                var listND = db.NoiDungDTs.Where(x => x.isNQ == 1).OrderBy(x => x.isOrder).ToList();
                int dem = 6;
                foreach (var item in listND)
                {
                    //columnHeaders.Add(item.NoiDung);
                    Worksheet.Cell(1, dem).Value = item.NoiDung;
                    Worksheet.Cell(2, dem).Value = "Tình trạng";
                    dem++;
                    Worksheet.Cell(2, dem).Value = "Ngày Hoàn thành";
                    dem++;
                }


                //if (IDPB == null) IDPB = 0;
                //if (ListQuyen.Contains("VIEW_BP")) IDPB = MyAuthentication.IDPhongban;
                List<ResultNQLDView> DataKNL = ListResult(IDPB, IDQT, IDMahieu);
                int row = 3;
                if (DataKNL.Count > 0)
                {
                  
                    foreach (var data in DataKNL)
                    {
                        Worksheet.Cell(row, "A").Value = row - 2;
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

                        Worksheet.Cell(row, "D").Value = data.PhongBan;
                        Worksheet.Cell(row, "D").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "D").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "D").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "E").Value = data.TenViTri;
                        Worksheet.Cell(row, "E").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "E").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "E").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        int col = 6;
                        foreach (var item in data.ListKQuaNQLD)
                        {
                           
                            Worksheet.Cell(row,col).Value =  item.TinhTrang == 1?"Đã hoàn thành": "Chưa hoàn thành" ;
                            Worksheet.Cell(row, col).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                            Worksheet.Cell(row, col).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                            Worksheet.Cell(row, col).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            col++;
                            Worksheet.Cell(row, col).Value = item.TinhTrang == 1 ? item.NgayHT.ToString() : "";
                            Worksheet.Cell(row, col).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                            Worksheet.Cell(row, col).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                            Worksheet.Cell(row, col).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            Worksheet.Cell(row, col).Style.DateFormat.Format = "dd/MM/yyyy";
                            col++;
                        }

                        row = row + 1;
                    }

                    Workbook.SaveAs(fileNamemaunew);
                    byte[] fileBytes = System.IO.File.ReadAllBytes(fileNamemaunew);
                    string fileName = "ThongKe_DS_KetQuaNQLD - " + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
                }
                else
                {

                    //Worksheet.Cell("A1").Value = "Ngày xuất file: " + DateTime.Now.Date.ToString("dd/MM/yyyy");
                    Workbook.SaveAs(fileNamemaunew);
                    byte[] fileBytes = System.IO.File.ReadAllBytes(fileNamemaunew);
                    string fileName = "ThongKe_DS_KetQuaNQLD - " + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
                }

            }
            catch (Exception ex)
            {
                TempData["msg"] = "<script>alert('" + ex + "');window.location.href = '/Everyday/'</script>";
                return View(TempData);
            }
        }
        public List<ResultNQLDView> ListResult(int? IDPB, int? IDQT, int? IDMahieu)
        {
            //var phongban = db.PhongBans.ToList();
            //var lvdt = db.LinhVucDTs.ToList();
            if (IDQT == null) IDQT = 0;
            if (IDMahieu == null) IDMahieu = 0;

            var res = (from a in db.NhanViens.Where(x => x.IDTinhTrangLV == 1)
                       join d in db.PhongBans
                       on a.IDPhongBan equals d.IDPhongBan
                       join b in db.Vitris on a.IDViTri equals b.IDViTri
                       select new ResultNQLDView
                       {
                           IDNV = a.ID,
                           HoTen = a.HoTen,
                           IDPhongBan = d.IDPhongBan,
                           PhongBan = d.TenPhongBan,
                           MaNV = a.MaNV,
                           TenViTri = b.TenViTri,
                           ListKQuaNQLD = (from aa in db.NoiDungDTs.Where(x => x.isNQ == 1)
                                           join k in db.NQ_KetQua.Where(x => x.IDNV == a.ID) on aa.IDND equals k.NDDTID into ul
                                           from k in ul.DefaultIfEmpty()
                                           select new KQuaNQ
                                           {
                                               IDChuong = aa.isOrder,
                                               NDDTID = aa.IDND,
                                               TinhTrang = k.TinhTrang,
                                               NgayHT = k.NgayHT,
                                               NgayTG = k.NgayTG,
                                               XNHT = k.XNHT,
                                               XNHTFile = k.XNHTFile,
                                               XNTG = k.XNTG,
                                           }).OrderBy(x => x.IDChuong).ToList(),
                       }).OrderBy(x => x.IDPhongBan).ToList();

            //if (IDMahieu != 0) res = res.Where(x => x.IDQTHD == IDMahieu).ToList().OrderBy(x => x.MaNV).OrderBy(x => x.IDPB);
            //if (IDQT != 0) res = res.Where(x => x.IDQTHD == IDQT).ToList().OrderBy(x => x.MaNV).OrderBy(x => x.IDPB);

            return res.ToList();
        }

    }
}