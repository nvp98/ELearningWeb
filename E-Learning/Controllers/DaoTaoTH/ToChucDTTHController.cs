using ClosedXML.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using E_Learning.Models;
using E_Learning.ModelsDTTH;
using E_Learning.Services;
using ExcelDataReader;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace E_Learning.Controllers.DaoTaoTH
{
    public class ToChucDTTHController : Controller
    {
        ELEARNINGEntities db_context = new ELEARNINGEntities();
        int Idquyen = MyAuthentication.IDQuyen;
        String ControllerName = "ToChucDTTH";
        // GET: ToChucDTTH
        public ActionResult Index(int? page,string search, int? NCDT_ID, int? PhuongPhapDT_ID,int? PhanLoaiNCDT_ID)
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            ViewBag.QUYENCN = ListQuyen;
            if (User.Identity.IsAuthenticated)
            {

                int id = MyAuthentication.ID;
                //if (MyAuthentication.IDQuyen == 4)
                //if (!ListQuyen.Contains(CONSTKEY.V))
                //{
                //    TempData["msgError"] = "<script>alert('Bạn không có quyền truy cập chức năng này');</script>";
                //    return RedirectToAction("", "Home");
                //}
                var res = new List<ManageClassValidation>();
                res = (from l in db_context.LopHocs.Where(x => (search == null || x.TenLH.Contains(search)) &&
                              (NCDT_ID == null || x.NCDT_ID == NCDT_ID))
                       join b in db_context.SH_NhuCauDT.Where(x=> (PhanLoaiNCDT_ID == null || x.PhanLoaiNCDT_ID == PhanLoaiNCDT_ID) && (PhuongPhapDT_ID == null || x.PhuongPhapDT_ID == PhuongPhapDT_ID)) on l.NCDT_ID equals b.ID
                       join n in db_context.NoiDungDTs on l.NDID equals n.IDND
                       //join g in db_context.NhanViens on l.GVID equals g.ID
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
                           TinhTrang = l.TinhTrang,
                           DiaDiemDT =l.DiaDiemDT,
                           ThoiLuongDT =l.ThoiLuongDT,
                           NoiDungTrichYeu = l.NoiDungTrichYeu,
                           //GVID = g.ID,
                           //TenGV = g.HoTen,
                           //MaGV = g.MaNV,
                           //TenPhongBan = g.PhongBan.TenPhongBan,
                           //IsGV = g.IsGV,
                           //SLHT = LopHocHVHT.Count(),
                           TSLHV = LopHocHV.Count(),
                           //IDPB = g.IDPhongBan
                       }).ToList();

                //if (ListQuyen.Contains(CONSTKEY.V_GV))
                //{
                //    res = res.Where(x => x.GVID == id).ToList();
                //}

                ViewBag.PhuongPhapDT_ID = Convert.ToInt32(Request.QueryString["PhuongPhapDT_ID"]);
                ViewBag.PhanLoaiNCDT_ID = Convert.ToInt32(Request.QueryString["PhanLoaiNCDT_ID"]);


                List<PhongBan> dt = db_context.PhongBans.ToList();
                ViewBag.IDPB = new SelectList(dt, "IDPhongBan", "TenPhongBan");

                List<LopHoc> lh = db_context.LopHocs.ToList();
                ViewBag.IDLH = new SelectList(lh, "IDLH", "TenLH");

                List<NoiDungDT> nd = db_context.NoiDungDTs.ToList();
                ViewBag.IDND = new SelectList(nd, "IDND", "NoiDung");

                if (page == null) page = 1;
                int pageSize = 50;
                int pageNumber = (page ?? 1);
                return View(res.ToList().OrderByDescending(x => x.TGBDLH).ToPagedList(pageNumber, pageSize));

            }
            else
            {
                return RedirectToAction("", "Login");
            }
        }

        public ActionResult Create(int? NCDT_ID, int? PhuongPhapDT_ID, int? PhanLoaiNCDT_ID)
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            //if (!ListQuyen.Contains(CONSTKEY.ADD))
            //{
            //    TempData["msgError"] = "<script>alert('Bạn không có quyền thực hiện chức năng này');</script>";
            //    return RedirectToAction("", "Home");
            //}
            //ViewBag.IDPhanLoaiDT = new SelectList(db_context.SH_PhanLoaiNDDT, "ID", "TenPhanLoaiDT",PhanLoaiNCDT_ID);
            ViewBag.PhuongPhapDT_ID = PhuongPhapDT_ID;
            ViewBag.PhanLoaiNCDT_ID = PhanLoaiNCDT_ID;
            ViewBag.LoaiNCDT = PhanLoaiNCDT_ID;
            ViewBag.LoaiHinh_DT = db_context.SH_PhanLoaiNCDT.Where(x => x.IDLoai == PhanLoaiNCDT_ID).FirstOrDefault().LoaiHinhDT_ID;
            List<NhuCauDTTHView> ncdt = (from a in db_context.SH_NhuCauDT.Where(x => (PhanLoaiNCDT_ID == null || x.PhanLoaiNCDT_ID == PhanLoaiNCDT_ID) && (PhuongPhapDT_ID == null || x.PhuongPhapDT_ID == PhuongPhapDT_ID) && x.BoPhanLNC_ID == MyAuthentication.IDPhongban)
                                         select new NhuCauDTTHView
                                         {
                                             ID_NCDT = a.ID,
                                             TenNoiDungDT = "Mã NCĐT: "+a.ID+" - "+ a.NoiDungDT.NoiDung
                                         }).ToList();

            ViewBag.NCDT_ID = new SelectList(ncdt, "ID_NCDT", "TenNoiDungDT");
            var IDPB = MyAuthentication.IDPhongban;
            ViewBag.Nam = db_context.SH_QuyDaoTao.First().AD_Nam;
            ViewBag.Quy = db_context.SH_QuyDaoTao.First().AD_Quy;
            ViewBag.BoPhan_ID = new SelectList(db_context.PhongBans.Where(x => x.IDPhongBan == IDPB), "IDPhongBan", "TenPhongBan", IDPB);

            // trình ký
            var nhanvien = db_context.NhanViens.Where(x => x.IDTinhTrangLV == 1).Select(x => new EmployeeValidation { ID = x.ID, HoTen = x.MaNV + " - " + x.HoTen, IDPhongBan = (int)x.IDPhongBan }).ToList();

            ViewBag.ID_NguoiKiemTra = new SelectList(nhanvien.Where(x => x.IDPhongBan == MyAuthentication.IDPhongban), "ID", "HoTen");
            ViewBag.ID_NhanVien = new SelectList(nhanvien, "ID", "HoTen");



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
        [ValidateAntiForgeryToken]
        public ActionResult Create(ManageClassValidation tochucdt, FormCollection form, IEnumerable<HttpPostedFileBase> files, HttpPostedFileBase fileUpload, string action)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    // Thêm lớp học
                    var ncdt = db_context.SH_NhuCauDT.Where(x => x.ID == tochucdt.NCDT_ID).FirstOrDefault();
                    var nam = db_context.SH_QuyDaoTao.First().AD_Nam;
                    var quy = db_context.SH_QuyDaoTao.First().AD_Quy;
                    var lophoc = new LopHoc()
                    {
                        MaLH = tochucdt.MaLH,
                        TenLH = tochucdt.TenLH,
                        NDID = ncdt.NoiDungDT_ID,
                        QuyDT = quy,
                        NamDT = nam,
                        NCDT_ID = tochucdt.NCDT_ID,
                        NoiDungTrichYeu = tochucdt.NoiDungTrichYeu,
                        TGBDLH = tochucdt.TGBDLH,
                        TGKTLH = tochucdt.TGKTLH,
                        ThoiLuongDT = tochucdt.ThoiLuongDT,
                        DiaDiemDT = tochucdt.DiaDiemDT,
                        IDDeThi = tochucdt.IDDeThi,
                        //ToChucThi
                        CTDT_ID = tochucdt.CTDT_ID,
                        BoPhan_ID = MyAuthentication.IDPhongban,
                        NguoiTao_ID = MyAuthentication.ID,
                        NguoiKiemTra_ID = tochucdt.NguoiKiemTra_ID,
                        TinhTrang = 2,// đang trình ký
                        NgayTao = DateTime.Now,
                    };
                    db_context.LopHocs.Add(lophoc);
                    db_context.SaveChanges();
                    // lưu thông tin giảng viên
                    var chitietCTDT = new SH_ChiTietTCDT()
                    {
                        CTDT_ID = lophoc.IDLH,
                        ID_GVCty = tochucdt.GVID,
                        HoTenGV = tochucdt.chiTietToChucDTTH.HoTenGV,
                        ViTriCV_GV = tochucdt.chiTietToChucDTTH.ViTriCV_GV,
                        DonVi_GV = tochucdt.chiTietToChucDTTH.ViTriCV_GV,
                        MaNV_GV = tochucdt.chiTietToChucDTTH.MaNV_GV
                    };
                    if (tochucdt.chiTietToChucDTTH.ID_GVCty != 0)
                    {
                        var gv = db_context.NhanViens.Where(x=>x.ID == tochucdt.chiTietToChucDTTH.ID_GVCty).FirstOrDefault();
                        chitietCTDT.HoTenGV = gv.HoTen;
                        chitietCTDT.MaNV_GV =gv.MaNV;
                        chitietCTDT.ViTriCV_GV = gv.Vitri.TenViTri;
                        chitietCTDT.DonVi_GV = gv.PhongBan.TenPhongBan;
                    }
                    db_context.SH_ChiTietTCDT.Add(chitietCTDT);
                    db_context.SaveChanges();
                    // thêm học viên vào lớp
                    if (tochucdt.IsAll)
                    {
                        var dsvt = db_context.SH_ViTri_NDDT.Where(x => x.NCDT_ID == tochucdt.NCDT_ID).ToList();
                        foreach (var item in dsvt)
                        {
                            var nhanvien = db_context.NhanViens.Where(x => x.IDVTKNL == item.Vitri_ID && x.IDTinhTrangLV == 1).ToList();
                            foreach (var item1 in nhanvien)
                            {
                                var checknv = db_context.XNHocTaps.Where(x => x.NVID == item1.ID && x.LHID == lophoc.IDLH).FirstOrDefault();
                                if (checknv == null)
                                {
                                    var XNHT = new XNHocTap()
                                    {
                                        LHID = lophoc.IDLH,
                                        NVID = item1.ID,
                                        PBID = item1.IDPhongBan,
                                        VTID = item1.IDViTri,
                                        NgayTG = default,
                                        NgayHT = default,
                                        XNHT = false,
                                        XNTG = false
                                    };
                                    db_context.XNHocTaps.Add(XNHT);
                                    db_context.SaveChanges();
                                }
                            }
                        }
                        var dsnv = db_context.SH_KetQuaDaoTao.Where(x => x.NCDT_ID == tochucdt.NCDT_ID).ToList();
                        foreach (var item in dsnv)
                        {
                            var nhanvien = db_context.NhanViens.Where(x => x.ID == item.NhanVien_ID && x.IDTinhTrangLV == 1).FirstOrDefault();
                            if (nhanvien != null)
                            {
                                var checknv = db_context.XNHocTaps.Where(x => x.NVID == nhanvien.ID && x.LHID == lophoc.IDLH).FirstOrDefault();
                                if (checknv == null)
                                {
                                    var XNHT = new XNHocTap()
                                    {
                                        LHID = lophoc.IDLH,
                                        NVID = nhanvien.ID,
                                        PBID = nhanvien.IDPhongBan,
                                        VTID = nhanvien.IDViTri,
                                        NgayTG = default(DateTime),
                                        NgayHT = default(DateTime),
                                        XNHT =false,
                                        XNTG = false
                                    };
                                    db_context.XNHocTaps.Add(XNHT);
                                    db_context.SaveChanges();
                                }
                            }

                        }
                    }
                    // check ds học viên có học viên để mở lớp
                    if (!String.IsNullOrEmpty(tochucdt.DSHocVien))
                    {
                        //Regex.Replace(_DO.NVDG, @"[^0-9a-zA-Z]+", " ");
                        string tx = Regex.Replace(tochucdt.DSHocVien, @"[^0-9a-zA-Z]+", " ");
                        string[] NVS = tx.Split(new char[] { ' ' });
                        foreach (var item in NVS)
                        {
                            var aa = db_context.NhanViens.Where(x => x.MaNV == item).FirstOrDefault();
                            if (aa != null)
                            {
                                var checktrung = db_context.XNHocTaps.Where(x => x.NVID == aa.ID && x.LHID == lophoc.IDLH).ToList();
                                if (checktrung.Count() == 0)
                                {
                                    var XNHT = new XNHocTap()
                                    {
                                        LHID = lophoc.IDLH,
                                        NVID = aa.ID,
                                        PBID = aa.IDPhongBan,
                                        VTID = aa.IDViTri,
                                        NgayTG = default(DateTime),
                                        NgayHT = default(DateTime),
                                        XNHT = false,
                                        XNTG = false
                                    };
                                    db_context.XNHocTaps.Add(XNHT);
                                    db_context.SaveChanges();
                                }
                            }
                        }
                        db_context.SaveChanges();
                    }
                    // thêm học viên từ DS excel
                    if (fileUpload != null && fileUpload.ContentLength > 0)
                    {
                        //lưu câu hỏi
                        Stream stream = fileUpload.InputStream;

                        IExcelDataReader reader = null;
                        if (fileUpload.FileName.EndsWith(".xls"))
                        {
                            reader = ExcelReaderFactory.CreateBinaryReader(stream);
                        }
                        else if (fileUpload.FileName.EndsWith(".xlsx"))
                        {
                            reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                        }
                        else
                        {
                            TempData["msg"] = "<script>alert('Vui lòng chọn đúng định dạng file Excel');</script>";
                            RedirectToAction("Index");
                        }
                        DataSet result = reader.AsDataSet();
                        DataTable dt = result.Tables[0];
                        reader.Close();

                        for (int i = 1; i < dt.Rows.Count; i++)
                        {
                            string manv = dt.Rows[i][1].ToString().Trim();
                            var nv = db_context.NhanViens.Where(x=>x.MaNV == manv).FirstOrDefault();
                            if(nv != null)
                            {
                                var checktrung = db_context.XNHocTaps.Where(x => x.NVID == nv.ID && x.LHID == lophoc.IDLH).ToList();
                                if (checktrung.Count() == 0)
                                {
                                    var XNHT = new XNHocTap()
                                    {
                                        LHID = lophoc.IDLH,
                                        NVID = nv.ID,
                                        PBID = nv.IDPhongBan,
                                        VTID = nv.IDViTri,
                                        NgayTG = default(DateTime),
                                        NgayHT = default(DateTime),
                                        XNHT = false,
                                        XNTG = false
                                    };
                                    db_context.XNHocTaps.Add(XNHT);
                                    db_context.SaveChanges();
                                }
                            }
                        }
                    }

                    // Thêm thông tin trình ký
                    if (action == "Lưu")
                    {
                        lophoc.TinhTrang = 0; // Đang lưu
                        db_context.SaveChanges();
                    }
                    
                    TempData["msgSuccess"] = "<script>alert('Thêm mới thành công ');</script>";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["msgSuccess"] = "<script>alert('Cập nhập thất bại " + ex.Message + " ');</script>";
            }
            return RedirectToAction("Index","ToChucDTTH");
        }

        public JsonResult GetDSHocVien(int? ID_NCDT)
        {
            var ListNV = new List<EmployeeValidation>();
            if (ID_NCDT != null)
            {
                var dsvt = db_context.SH_ViTri_NDDT.Where(x => x.NCDT_ID == ID_NCDT).ToList();
                foreach (var item in dsvt)
                {
                    var nhanvien = db_context.NhanViens.Where(x=>x.IDVTKNL == item.Vitri_ID && x.IDTinhTrangLV ==1).ToList();
                    foreach (var item1 in nhanvien)
                    {
                        var checknv = ListNV.Where(x=>x.ID == item1.ID).FirstOrDefault();
                        if(checknv ==null)ListNV.Add(new EmployeeValidation { MaNV = item1.MaNV,HoTen = item1.MaNV + " - " + item1.HoTen,ID =item1.ID });
                    }
                }
                var dsnv = db_context.SH_KetQuaDaoTao.Where(x => x.NCDT_ID == ID_NCDT).ToList();
                foreach (var item in dsnv)
                {
                    var nhanvien = db_context.NhanViens.Where(x => x.ID == item.NhanVien_ID && x.IDTinhTrangLV ==1).FirstOrDefault();
                    if(nhanvien != null)
                    {
                        var checknv = ListNV.Where(x => x.ID == item.NhanVien_ID).FirstOrDefault();
                        if (checknv == null) ListNV.Add(new EmployeeValidation { MaNV = nhanvien.MaNV, HoTen = nhanvien.MaNV +" - "+ nhanvien.HoTen, ID = nhanvien.ID });
                    }
                    
                }
            }
            return Json(ListNV, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSLHocVien(int? ID_NCDT)
        {
            int ListNV = 0;
            if (ID_NCDT != null)
            {
                var dsvt = db_context.SH_ViTri_NDDT.Where(x => x.NCDT_ID == ID_NCDT).ToList();
                foreach (var item in dsvt)
                {
                    var nhanvien = db_context.NhanViens.Where(x => x.IDVTKNL == item.Vitri_ID && x.IDTinhTrangLV == 1).ToList();
                    foreach (var item1 in nhanvien)
                    {
                        ListNV++;
                    }
                }
                var dsnv = db_context.SH_KetQuaDaoTao.Where(x => x.NCDT_ID == ID_NCDT).ToList();
                foreach (var item in dsnv)
                {
                    var nhanvien = db_context.NhanViens.Where(x => x.ID == item.NhanVien_ID && x.IDTinhTrangLV == 1).FirstOrDefault();
                    if (nhanvien != null)
                    {
                        ListNV++;
                    }

                }
            }
            return Json(ListNV, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCTDT(int? ID_NCDT)
        {
            var ListNV = new List<SH_ChuongTrinhDT>();
            if (ID_NCDT != null)
            {
                var ncdt = db_context.SH_NhuCauDT.Where(x=>x.ID == ID_NCDT).FirstOrDefault();
                ListNV = db_context.SH_ChuongTrinhDT.Where(x => x.ID_NoiDungDT == ncdt.NoiDungDT_ID).ToList();
               
            }
            return Json(ListNV, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDeThi(int? ID_NCDT)
        {
            var ListNV = new List<CauHoiDeThiCTDTTH>();
            if (ID_NCDT != null)
            {
                var ncdt = db_context.SH_NhuCauDT.Where(x => x.ID == ID_NCDT).FirstOrDefault();
                ListNV = db_context.DeThis.Where(x => x.IDND == ncdt.NoiDungDT_ID).Select(x => new CauHoiDeThiCTDTTH { ID = x.IDDeThi, TenDeThi = x.TenDe,MaDeThi = x.MaDe,TongSoCau = x.TongSoCau,ThoiGianLamBai = x.ThoiGianLamBai,DiemChuan = x.DiemChuan }).ToList();
            }
            return Json(ListNV, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult UploadExcel(HttpPostedFileBase file, int? NCDT_ID)
        {
            if (file == null || file.ContentLength == 0)
            {
                return Json(new { success = false, message = "Vui lòng chọn file" });
            }

            List<List<string>> excelData = new List<List<string>>();

            try
            {
                using (var stream = new MemoryStream())
                {
                    file.InputStream.CopyTo(stream);
                    using (var workbook = new XLWorkbook(stream))
                    {
                        var worksheet = workbook.Worksheets.FirstOrDefault();
                        if (worksheet == null)
                            return Json(new { success = false, message = "Không tìm thấy dữ liệu" });

                        int rowCount = worksheet.LastRowUsed().RowNumber();
                        int colCount = worksheet.LastColumnUsed().ColumnNumber();

                        for (int row = 2; row <= rowCount; row++)
                        {
                            List<string> rowData = new List<string>();
                            for (int col = 1; col <= colCount; col++)
                            {
                                rowData.Add(worksheet.Cell(row, col).GetValue<string>().Trim());
                                // Check nhân viên
                                if (NCDT_ID != null)
                                {
                                    var manv = rowData[0];
                                    var nvcheck = db_context.NhanViens.Where(x => x.MaNV == manv).FirstOrDefault();
                                    if (nvcheck != null)
                                    {
                                        var dsvt = db_context.SH_ViTri_NDDT.Where(x => x.NCDT_ID == NCDT_ID).ToList();
                                        foreach (var item in dsvt)
                                        {
                                            var nhanvien = db_context.NhanViens.Where(x => x.IDVTKNL == item.Vitri_ID && x.IDTinhTrangLV == 1).ToList();
                                            foreach (var item1 in nhanvien)
                                            {
                                                if (nvcheck.ID == item1.ID)
                                                {
                                                    rowData.Add("Nhân viên đã có trong NCĐT");
                                                }
                                            }
                                        }
                                        var dsnv = db_context.SH_KetQuaDaoTao.Where(x => x.NCDT_ID == NCDT_ID).ToList();
                                        foreach (var item in dsnv)
                                        {
                                            var nhanvien = db_context.NhanViens.Where(x => x.ID == item.NhanVien_ID && x.IDTinhTrangLV == 1).FirstOrDefault();
                                            if (nhanvien != null)
                                            {
                                                if (nvcheck.ID == nhanvien.ID)
                                                {
                                                    rowData.Add("Nhân viên đã có trong NCĐT");
                                                }
                                            }

                                        }
                                    }
                                }
                            }
                            

                            excelData.Add(rowData);
                        }
                    }
                }

                return Json(excelData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi xử lý file: " + ex.Message });
            }
        }

        public ActionResult Edit(int? PhuongPhapDT_ID, int? PhanLoaiNCDT_ID, int? IDLH)
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);

            var res = db_context.LopHocs.Find(IDLH);
            List<NhuCauDTTHView> ncdtData = (from a in db_context.SH_NhuCauDT.Where(x => (PhanLoaiNCDT_ID == null || x.PhanLoaiNCDT_ID == PhanLoaiNCDT_ID) 
                                             && (PhuongPhapDT_ID == null || x.PhuongPhapDT_ID == PhuongPhapDT_ID) && x.BoPhanLNC_ID == MyAuthentication.IDPhongban)
                                   select new NhuCauDTTHView
                                   {
                                       ID_NCDT = a.ID,
                                       TenNoiDungDT = "Mã NCĐT: " + a.ID + " - " + a.NoiDungDT.NoiDung
                                   }).ToList();

            var data = new ManageClassValidation();
            if (res != null)
            {
                data.MaLH = res.MaLH;
                data.TenLH = res.TenLH;
                data.NoiDungTrichYeu = res.NoiDungTrichYeu;
                data.TGBDLH = res.TGBDLH.Value;
                data.TGKTLH = res.TGKTLH.Value;
                data.DiaDiemDT = res.DiaDiemDT;
                data.ThoiLuongDT = res.ThoiLuongDT;
                data.NCDT_ID = (int) res.NCDT_ID;
                data.QuyDT = (int) res.QuyDT;
                data.IDDeThi = (int) res.IDDeThi;
                data.NguoiKiemTra_ID = res.NguoiKiemTra_ID;
                data.IDLH = res.IDLH;
            }

            ViewBag.NCDT_DATA = new SelectList(ncdtData, "ID_NCDT", "TenNoiDungDT", data.NCDT_ID);

            var listDeThi = new List<CauHoiDeThiCTDTTH>();

            if (data.NCDT_ID != null)
            {
                var ncdt = db_context.SH_NhuCauDT.FirstOrDefault(x => x.ID == data.NCDT_ID);

                if (ncdt != null)
                {
                    listDeThi = (from x in db_context.DeThis
                                 where x.IDND == ncdt.NoiDungDT_ID
                                 select new CauHoiDeThiCTDTTH
                                 {
                                     ID = x.IDDeThi,
                                     TenDeThi = string.Concat("Điểm chuẩn: ", x.DiemChuan, "-", x.TenDe),
                                 }).ToList();
                }
            }

            ViewBag.DETHI_DATA = new SelectList(listDeThi, "ID", "TenDeThi", data.IDDeThi);

            var listNhanVien = db_context.NhanViens.Where(x => x.IDTinhTrangLV == 1)
                .Select(x => new EmployeeValidation { ID = x.ID, HoTen = x.MaNV + " - " + x.HoTen, IDPhongBan = (int)x.IDPhongBan })
                .ToList();

            int IDGVCty = db_context.SH_ChiTietTCDT.Where(x => x.CTDT_ID == data.IDLH).SingleOrDefault().ID_GVCty.Value;

            ViewBag.ID_NhanVien = new SelectList(listNhanVien, "ID", "HoTen", IDGVCty);
            ViewBag.ID_NguoiKiemTra = new SelectList(listNhanVien.Where(x => x.IDPhongBan == MyAuthentication.IDPhongban), "ID", "HoTen", data.NguoiKiemTra_ID);
            ViewBag.PhuongPhapDT_ID = PhuongPhapDT_ID;
            ViewBag.PhanLoaiNCDT_ID = PhanLoaiNCDT_ID;
            ViewBag.LoaiNCDT = PhanLoaiNCDT_ID;
            ViewBag.LoaiHinh_DT = db_context.SH_PhanLoaiNCDT.Where(x => x.IDLoai == PhanLoaiNCDT_ID).FirstOrDefault().LoaiHinhDT_ID;
            
            var IDPB = MyAuthentication.IDPhongban;
            ViewBag.Nam = db_context.SH_QuyDaoTao.First().AD_Nam;
            ViewBag.Quy = db_context.SH_QuyDaoTao.First().AD_Quy;
            ViewBag.BoPhan_ID = new SelectList(db_context.PhongBans.Where(x => x.IDPhongBan == IDPB), "IDPhongBan", "TenPhongBan", IDPB);
            ViewBag.MaLH = data.MaLH;
            
            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ManageClassValidation DTO, HttpPostedFileBase fileUpload, string action, int? PhuongPhapDT_ID, int? PhanLoaiNCDT_ID)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var lopHocExisting = db_context.LopHocs.Where(x => x.MaLH == DTO.MaLH).FirstOrDefault();
                    if (lopHocExisting != null)
                    {
                        lopHocExisting.TenLH = DTO.TenLH;
                        lopHocExisting.NCDT_ID = DTO.NCDT_ID;
                        lopHocExisting.NoiDungTrichYeu = DTO.NoiDungTrichYeu;
                        lopHocExisting.TGBDLH = DTO.TGBDLH;
                        lopHocExisting.TGKTLH = DTO.TGKTLH;
                        lopHocExisting.ThoiLuongDT = DTO.ThoiLuongDT;
                        lopHocExisting.DiaDiemDT = DTO.DiaDiemDT;
                        var nhuCauDT = db_context.SH_NhuCauDT.Where(x => x.ID == lopHocExisting.NCDT_ID).FirstOrDefault();
                        lopHocExisting.NDID = nhuCauDT.NoiDungDT_ID;

                        int IDGVCty = DTO.chiTietToChucDTTH.ID_GVCty.Value;
                        lopHocExisting.GVID = IDGVCty;
                        var chiTietTCDTInDB = db_context.SH_ChiTietTCDT.Where(x => x.CTDT_ID == lopHocExisting.IDLH).SingleOrDefault();
                        var nhanVienInDb = db_context.NhanViens.Where(x => x.ID == IDGVCty).SingleOrDefault();

                        chiTietTCDTInDB.ID_GVCty = IDGVCty;
                        chiTietTCDTInDB.MaNV_GV = nhanVienInDb.MaNV;
                        chiTietTCDTInDB.HoTenGV = nhanVienInDb.HoTen;
                        var viTriCongViecInDb = db_context.Vitris.Where(x => x.IDViTri == nhanVienInDb.IDViTri).SingleOrDefault();
                        chiTietTCDTInDB.ViTriCV_GV = viTriCongViecInDb.TenViTri;
                        var phongBanInDb = db_context.PhongBans.Where(x => x.IDPhongBan == nhanVienInDb.IDPhongBan).SingleOrDefault();
                        chiTietTCDTInDB.DonVi_GV = phongBanInDb.TenPhongBan;

                        lopHocExisting.NguoiKiemTra_ID = DTO.NguoiKiemTra_ID;
                        lopHocExisting.NguoiTao_ID = lopHocExisting.NguoiTao_ID;
                        lopHocExisting.NgayTao = lopHocExisting.NgayTao;
                        lopHocExisting.IDDeThi = DTO.IDDeThi;

                        if (!String.IsNullOrEmpty(DTO.DSHocVien))
                        {
                            string tx = Regex.Replace(DTO.DSHocVien, @"[^0-9a-zA-Z]+", " ");
                            string[] NVS = tx.Split(new char[] { ' ' });
                            foreach (var item in NVS)
                            {
                                var nhanVien = db_context.NhanViens.Where(x => x.MaNV == item).FirstOrDefault();
                                if (nhanVien != null)
                                {
                                    var checkDuplicate = db_context.XNHocTaps.Where(x => x.NVID == nhanVien.ID && x.LHID == lopHocExisting.IDLH).ToList();
                                    if (checkDuplicate.Count() == 0)
                                    {
                                        var XNHT = new XNHocTap()
                                        {
                                            LHID = lopHocExisting.IDLH,
                                            NVID = nhanVien.ID,
                                            PBID = nhanVien.IDPhongBan,
                                            VTID = nhanVien.IDViTri,
                                            NgayTG = default(DateTime),
                                            NgayHT = default(DateTime),
                                            XNHT = false,
                                            XNTG = false
                                        };
                                        db_context.XNHocTaps.Add(XNHT);
                                    }
                                }
                            }
                            db_context.SaveChanges();
                        }
                    }
                    
                    // Thêm thông tin trình ký
                    if (action == "Cập nhật")
                    {
                        db_context.SaveChanges();
                    }

                    TempData["msgSuccess"] = "<script>alert('Cập nhật thành công ');</script>";
                    
                    return RedirectToAction("Index", new { PhuongPhapDT_ID = PhuongPhapDT_ID, PhanLoaiNCDT_ID = PhanLoaiNCDT_ID });
                }
            }
            catch (Exception ex)
            {
                TempData["msgSuccess"] = "<script>alert('Cập nhật thất bại " + ex.Message + " ');</script>";
            }
            
            return RedirectToAction("Index", "ToChucDTTH");
        }

        public JsonResult CheckLSNV(int IDLH)
        {
            var ListNV = new List<EmployeeValidation>();

            var XNHocTap = db_context.XNHocTaps.Where(x => x.LHID == IDLH).ToList();

            if (XNHocTap.Any())
            {
                foreach (var item in XNHocTap)
                {
                    var nhanViens = db_context.NhanViens.Where(x => x.ID == item.NVID).ToList();
                    foreach (var nv in nhanViens)
                    {
                        ListNV.Add(new EmployeeValidation
                        {
                            MaNV = nv.MaNV,
                            HoTen = nv.MaNV + " - " + nv.HoTen
                        });
                    }
                }
            }

            return Json(ListNV, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewEdit()
        {
            return View();
        }
    }
}