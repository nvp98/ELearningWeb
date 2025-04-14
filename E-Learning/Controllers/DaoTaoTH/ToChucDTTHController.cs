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
using System.Data.Entity;
using System.Globalization;
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
        public ActionResult Index(int? page,string search, int? NCDT_ID, int? PhuongPhapDT_ID,int? PhanLoaiNCDT_ID , int? IDLH, int? IDPB, int? IDND)
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            ViewBag.QUYENCN = ListQuyen;
            if (User.Identity.IsAuthenticated)
            {

                int id = MyAuthentication.ID;
                if (!ListQuyen.Contains(CONSTKEY.V))
                {
                    TempData["msgError"] = "<script>alert('Bạn không có quyền truy cập chức năng này');</script>";
                    return RedirectToAction("", "Home");
                }
                var res = new List<ManageClassValidation>();
                res = (from l in db_context.LopHocs.Where(x => (search == null || x.TenLH.Contains(search)) &&
                              (NCDT_ID == null || x.NCDT_ID == NCDT_ID) && (IDLH == null || x.IDLH == IDLH) && (IDPB == null || x.BoPhan_ID == IDPB))
                       join b in db_context.SH_NhuCauDT.Where(x=> (PhanLoaiNCDT_ID == null || x.PhanLoaiNCDT_ID == PhanLoaiNCDT_ID) && (PhuongPhapDT_ID == null || x.PhuongPhapDT_ID == PhuongPhapDT_ID)) on l.NCDT_ID equals b.ID
                       join n in db_context.NoiDungDTs.Where(x=>(IDND == null || x.IDND == IDND)) on l.NDID equals n.IDND
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
                           NguoiTao_ID = l.NguoiTao_ID,

                           //GVID = g.ID,
                           //TenGV = g.HoTen,
                           //MaGV = g.MaNV,
                           //TenPhongBan = g.PhongBan.TenPhongBan,
                           //IsGV = g.IsGV,
                           //SLHT = LopHocHVHT.Count(),
                           TSLHV = LopHocHV.Count(),
                           IDPB = l.BoPhan_ID,
                           TenBoPhan = db_context.PhongBans.FirstOrDefault(x => x.IDPhongBan == l.BoPhan_ID).TenPhongBan,
                           TenNguoiTao = db_context.NhanViens.FirstOrDefault(x=>x.ID == l.NguoiTao_ID).HoTen,
                           TenNguoiKiemTra = db_context.NhanViens.FirstOrDefault(x => x.ID == l.NguoiKiemTra_ID).HoTen,
                           CTDT_ID = l.CTDT_ID,
                           TenGV = db_context.SH_ChiTietTCDT.FirstOrDefault(x => x.CTDT_ID == l.IDLH).MaNV_GV +" - "+ db_context.SH_ChiTietTCDT.FirstOrDefault(x=>x.CTDT_ID == l.IDLH).HoTenGV 
                       }).ToList();

                if (!ListQuyen.Contains(CONSTKEY.VIEW_ALL) && !ListQuyen.Contains(CONSTKEY.V_BP)) res = res.Where(x => x.NguoiTao_ID == MyAuthentication.ID).ToList();
                else if (ListQuyen.Contains(CONSTKEY.V_BP)) res = res.Where(x => x.IDPB == MyAuthentication.IDPhongban).ToList();

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
            List<NhuCauDTTHView> ncdt = (from a in db_context.SH_NhuCauDT.Where(x => (PhanLoaiNCDT_ID == null || x.PhanLoaiNCDT_ID == PhanLoaiNCDT_ID) && (PhuongPhapDT_ID == null || x.PhuongPhapDT_ID == PhuongPhapDT_ID) && x.BoPhanLNC_ID == MyAuthentication.IDPhongban && x.TinhTrang ==1)
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
            ViewBag.NhomNLID = new SelectList(db_context.NhomNLKCCDs.ToList(), "ID", "NoiDung");
            ViewBag.LVDTID = new SelectList(db_context.LinhVucDTs.ToList(), "IDLVDT", "TenLVDT");


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
        public ActionResult Create(ManageClassValidation tochucdt, FormCollection form, IEnumerable<HttpPostedFileBase> files, HttpPostedFileBase fileUpload, string action,int? PhuongPhapDT_ID, int? PhanLoaiNCDT_ID)
        {

            try
            {
                if (ModelState.IsValid || true)
                {

                    // check điều kiện Giáo viên
                    if(tochucdt.chiTietToChucDTTH.ID_GVCty == null && tochucdt.chiTietToChucDTTH.HoTenGV == null) {
                        TempData["msg"] = "<script>alert('Vui lòng điền đầy đủ thông tin giảng viên');</script>";
                        return RedirectToAction("Index", "ToChucDTTH", new { PhuongPhapDT_ID = PhuongPhapDT_ID, PhanLoaiNCDT_ID = PhanLoaiNCDT_ID });
                    }
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
                        IsCoCTDT = tochucdt.IsCoCTDT
                    };
                    db_context.LopHocs.Add(lophoc);
                    db_context.SaveChanges();
                    // lưu thông tin giảng viên
                    var chitietCTDT = new SH_ChiTietTCDT()
                    {
                        CTDT_ID = lophoc.IDLH,
                        ID_GVCty = tochucdt.chiTietToChucDTTH.ID_GVCty,
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
                        lophoc.GVID = gv.ID; // lưu IDGV vào LopHoc
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
                                //var checknv = db_context.XNHocTaps.Where(x => x.NVID == item1.ID && x.LHID == lophoc.IDLH).FirstOrDefault();
                                //var checkDKnv = (from a in db_context.XNHocTaps.Where(x => x.NVID == item1.ID && x.IDND == ncdt.NoiDungDT_ID && x.ID_PhuongPhapDT == ncdt.PhuongPhapDT_ID )
                                //                 join b in db_context.LopHocs.Where(x=>x.TinhTrang ==4) on a.LHID equals b.IDLH
                                //                 select a
                                //                ).ToList(); // đang nộp hồ sơ
                                //var checkNVNop = (from a in db_context.XNHocTaps.Where(x => x.NVID == item1.ID && x.IDND == ncdt.NoiDungDT_ID && x.ID_PhuongPhapDT == ncdt.PhuongPhapDT_ID && x.KetLuan == 1)
                                //                 join b in db_context.LopHocs.Where(x => x.TinhTrang == 5) on a.LHID equals b.IDLH
                                //                 join c in db_context.SH_HoSoDaoTao.Where(x=>x.NgayKTThucTe.Value.AddMonths(dinhky) < DateTime.Now) on b.IDLH equals c.ID
                                //                 select a
                                //                ).ToList(); // đã duyệt hồ sơ
                                bool check = CheckDKHocVien(item1.ID, lophoc.IDLH, ncdt.NoiDungDT_ID, ncdt.PhuongPhapDT_ID, ncdt.ID);
                                if (check)
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
                                        XNTG = false,
                                        IDND = ncdt.NoiDungDT_ID,
                                        ID_PhuongPhapDT = ncdt.PhuongPhapDT_ID
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
                                //var checknv = db_context.XNHocTaps.Where(x => x.NVID == nhanvien.ID && x.LHID == lophoc.IDLH).FirstOrDefault();
                                bool check = CheckDKHocVien(nhanvien.ID, lophoc.IDLH, ncdt.NoiDungDT_ID, ncdt.PhuongPhapDT_ID, ncdt.ID);
                                if (check)
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
                                        XNTG = false,
                                        IDND = ncdt.NoiDungDT_ID,
                                        ID_PhuongPhapDT = ncdt.PhuongPhapDT_ID
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
                                //var checktrung = db_context.XNHocTaps.Where(x => x.NVID == aa.ID && x.LHID == lophoc.IDLH).ToList();
                                bool check = CheckDKHocVien(aa.ID, lophoc.IDLH, ncdt.NoiDungDT_ID, ncdt.PhuongPhapDT_ID, ncdt.ID);
                                if (check)
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
                                        XNTG = false,
                                        IDND = ncdt.NoiDungDT_ID,
                                        ID_PhuongPhapDT = ncdt.PhuongPhapDT_ID
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
                            return RedirectToAction("Index", "ToChucDTTH", new { PhuongPhapDT_ID = PhuongPhapDT_ID, PhanLoaiNCDT_ID = PhanLoaiNCDT_ID });
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
                                //var checktrung = db_context.XNHocTaps.Where(x => x.NVID == nv.ID && x.LHID == lophoc.IDLH).ToList();
                                bool check = CheckDKHocVien(nv.ID, lophoc.IDLH, ncdt.NoiDungDT_ID, ncdt.PhuongPhapDT_ID, ncdt.ID);
                                if (check)
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
                                        XNTG = false,
                                        IDND = ncdt.NoiDungDT_ID,
                                        ID_PhuongPhapDT = ncdt.PhuongPhapDT_ID
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
                    return RedirectToAction("Index", "ToChucDTTH", new { PhuongPhapDT_ID = PhuongPhapDT_ID, PhanLoaiNCDT_ID = PhanLoaiNCDT_ID });
                }
            }
            catch (Exception ex)
            {
                TempData["msgSuccess"] = "<script>alert('Cập nhập thất bại " + ex.Message + " ');</script>";
            }
            return RedirectToAction("Index","ToChucDTTH", new { PhuongPhapDT_ID = PhuongPhapDT_ID, PhanLoaiNCDT_ID = PhanLoaiNCDT_ID });
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
            var ListNV = new List<ChuongTrinhDTTHView>();
            if (ID_NCDT != null)
            {
                var ncdt = db_context.SH_NhuCauDT.Where(x=>x.ID == ID_NCDT).FirstOrDefault();
                ListNV = (from a in db_context.SH_ChuongTrinhDT.Where(x => x.ID_NoiDungDT == ncdt.NoiDungDT_ID)
                          select new ChuongTrinhDTTHView
                          {
                              IDCTDT = a.IDCTDT,
                              TenChuongTrinhDT = a.TenChuongTrinhDT,
                              NoiDungTrichYeu =a.NoiDungTrichYeu
                          }).ToList();
               
            }
            return Json(ListNV, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetChiTietNoiDungDT(int? NCDT_ID)
        {
            //var lsIDLoaiND = new List<int?> { 6, 7, 8 }; // list ID PhanLoaiNDDT thuê ngoài được lượt bỏ
            db_context.Configuration.ProxyCreationEnabled = false;
            var ls = new NoiDungDTTHView();
            if (NCDT_ID != null)
            {
                var ncdt = db_context.SH_NhuCauDT.Where(x=>x.ID ==NCDT_ID).FirstOrDefault();    
                ls = (from a in db_context.NoiDungDTs.Where(x => x.IDND == ncdt.NoiDungDT_ID)
                      select new NoiDungDTTHView()
                      {
                          IDND = a.IDND,
                          NoiDung = a.IDND + "-" + a.MaND + "-" + a.NoiDung,
                          LVDTID = a.LVDTID,
                          TenLVDT = a.LinhVucDT.TenLVDT,
                          IDNhomNL = a.IDNhomNL,
                          TenNhomNL = a.NhomNLKCCD.NoiDung
                      }).FirstOrDefault();
            }
            return Json(ls, JsonRequestBehavior.AllowGet);
        }
        public bool CheckDKHocVien(int? IDNV, int? IDLH, int? IDND, int? IDPhuongPhap,int? NCDT_ID )
        {
            int dinhky = db_context.SH_NhuCauDT.FirstOrDefault(x => x.ID == NCDT_ID)?.MaDinhKy ?? 0;
            var thoiGianSoSanh = DateTime.Now.AddMonths(-dinhky);
            var checknv = db_context.XNHocTaps.Where(x => x.NVID == IDNV && x.LHID == IDLH).FirstOrDefault(); // check hv đã có trong lớp
            if (checknv != null) return false;

            //var checkDKnv = (from a in db_context.XNHocTaps.Where(x => x.NVID == IDNV && x.IDND == IDND && x.ID_PhuongPhapDT == IDPhuongPhap)
            //                 join b in db_context.LopHocs.Where(x => x.TinhTrang != 3 && x.TinhTrang != 5 && x.TinhTrang != 6) on a.LHID equals b.IDLH
            //                 select a
            //                ).ToList(); // hồ sơ chưa xử lý
            var checkDKnv = db_context.XNHocTaps
                            .Where(x => x.NVID == IDNV
                                     && x.IDND == IDND
                                     && x.ID_PhuongPhapDT == IDPhuongPhap
                                     && db_context.LopHocs.Any(b => b.IDLH == x.LHID
                                                                    && b.TinhTrang != 3
                                                                    && b.TinhTrang != 5
                                                                    && b.TinhTrang != 6))
                            .AsNoTracking() // Tối ưu hiệu năng nếu không cần cập nhật
                            .ToList();
            if (checkDKnv.Count != 0) return false;
            //var checkNVNop = (from a in db_context.XNHocTaps.Where(x => x.NVID == IDNV && x.IDND == IDND && x.ID_PhuongPhapDT == IDPhuongPhap && x.KetLuan == 1)
            //                  join b in db_context.LopHocs.Where(x => x.TinhTrang == 5) on a.LHID equals b.IDLH
            //                  join c in db_context.SH_HoSoDaoTao.Where(x=>(x.NgayKTThucTe != null && x.NgayKTThucTe > thoiGianSoSanh && dinhky != 0) || dinhky == 0) on b.IDLH equals c.LHID
            //                  //join c in db_context.SH_HoSoDaoTao.Where(x =>(x.NgayKTThucTe != null && x.NgayKTThucTe.Value.AddMonths(dinhky) > DateTime.Now && dinhky != 0)|| dinhky == 0) on b.IDLH equals c.LHID
            //                  select a
            //                ).ToList(); // đã duyệt hồ sơ
            var checkNVNop = (from a in db_context.XNHocTaps
                              where a.NVID == IDNV
                                 && a.IDND == IDND
                                 && a.ID_PhuongPhapDT == IDPhuongPhap
                                 && a.KetLuan == 1
                                 && db_context.LopHocs.Any(b => b.TinhTrang == 5 && b.IDLH == a.LHID)
                                 && db_context.SH_HoSoDaoTao.Any(c =>
                                        c.LHID == a.LHID &&
                                        ((c.NgayKTThucTe != null && c.NgayKTThucTe > thoiGianSoSanh && dinhky != 0) || dinhky == 0))
                              select a
                 ).AsNoTracking().ToList();
            if (checkNVNop.Count != 0) return false;
            //var checkNVNopKDAT = (from a in db_context.XNHocTaps.Where(x => x.NVID == IDNV && x.IDND == IDND && x.ID_PhuongPhapDT == IDPhuongPhap && x.KetLuan != 1)
            //                  join b in db_context.LopHocs.Where(x => x.TinhTrang == 5) on a.LHID equals b.IDLH
            //                  //join c in db_context.SH_HoSoDaoTao.Where(x => (x.NgayKTThucTe != null && x.NgayKTThucTe.Value.AddMonths(dinhky) > DateTime.Now && dinhky != 0) || dinhky == 0) on b.IDLH equals c.ID
            //                  select a
            //               ).ToList(); // đã duyệt hồ sơ và không đạt
            var checkNVNopKDAT = (from a in db_context.XNHocTaps
                                  where a.NVID == IDNV
                                     && a.IDND == IDND
                                     && a.ID_PhuongPhapDT == IDPhuongPhap
                                     && a.KetLuan != 1
                                     && db_context.LopHocs.Any(b => b.TinhTrang == 5 && b.IDLH == a.LHID)
                                     && db_context.SH_HoSoDaoTao.Any(c =>
                                            c.LHID == a.LHID &&
                                            ((c.NgayKTThucTe != null && c.NgayKTThucTe > thoiGianSoSanh && dinhky != 0) || dinhky == 0))
                                  select a)
                     .AsNoTracking() // Tối ưu hiệu suất
                     .ToList();
            if (checkNVNop.Count != 0) return false;

            return true;

            //if (checknv == null ) return true;
            //else return false;
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
        public JsonResult GetDeThiCTDT(int? ID_CTDT, int? ID_NCDT)
        {
            var ListNV = new List<CauHoiDeThiCTDTTH>();
            if (ID_CTDT != null)
            {
                ListNV = db_context.DeThis.Where(x => x.CTDT_ID == ID_CTDT).Select(x => new CauHoiDeThiCTDTTH { ID = x.IDDeThi, TenDeThi = x.TenDe, MaDeThi = x.MaDe, TongSoCau = x.TongSoCau, ThoiGianLamBai = x.ThoiGianLamBai, DiemChuan = x.DiemChuan }).ToList();
            }
            else
            {
                var ncdt = db_context.SH_NhuCauDT.Where(x => x.ID == ID_NCDT).FirstOrDefault();
                ListNV = db_context.DeThis.Where(x => x.IDND == ncdt.NoiDungDT_ID).Select(x => new CauHoiDeThiCTDTTH { ID = x.IDDeThi, TenDeThi = x.TenDe, MaDeThi = x.MaDe, TongSoCau = x.TongSoCau, ThoiGianLamBai = x.ThoiGianLamBai, DiemChuan = x.DiemChuan }).ToList();
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

            var res = db_context.LopHocs.FirstOrDefault(x=>x.IDLH ==IDLH);
            List<NhuCauDTTHView> ncdtData = (from a in db_context.SH_NhuCauDT.Where(x => x.ID == res.NCDT_ID)
                                   select new NhuCauDTTHView
                                   {
                                       ID_NCDT = a.ID,
                                       TenNoiDungDT = "Mã NCĐT: " + a.ID + " - " + a.NoiDungDT.NoiDung
                                   }).ToList();
            var IDPB = res.BoPhan_ID;
            var data = new ManageClassValidation();
            if (res != null)
            {
                var chitietGV = db_context.SH_ChiTietTCDT.FirstOrDefault(x => x.CTDT_ID == res.IDLH);
                data.MaLH = res.MaLH;
                data.TenLH = res.TenLH;
                data.NoiDungTrichYeu = res.NoiDungTrichYeu;
                data.TGBDLH = res.TGBDLH.Value;
                data.TGKTLH = res.TGKTLH.Value;
                data.DiaDiemDT = res.DiaDiemDT;
                data.ThoiLuongDT = res.ThoiLuongDT;
                data.NCDT_ID = (int) res.NCDT_ID;
                data.QuyDT = (int) res.QuyDT;
                data.NamDT = (int) res.NamDT;
                data.IDDeThi = (int) res.IDDeThi;
                data.NguoiKiemTra_ID = res.NguoiKiemTra_ID;
                data.IDLH = res.IDLH;
                data.IsCoCTDT = res.IsCoCTDT;
                data.CTDT_ID = res.CTDT_ID;
                data.NDID = (int)res.NDID;
                data.IsAll = true;
                if(chitietGV != null)
                {
                    data.chiTietToChucDTTH = new ChiTietToChucDTTHView() { 
                    DonVi_GV = chitietGV.DonVi_GV,
                    HoTenGV = chitietGV.HoTenGV,
                    MaNV_GV = chitietGV.MaNV_GV,
                    ViTriCV_GV = chitietGV.ViTriCV_GV,
                    };
                }
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

            ViewBag.DETHI_DATA = new SelectList(listDeThi, "ID", "TenDeThi", data?.IDDeThi);

            var listNhanVien = db_context.NhanViens.Where(x => x.IDTinhTrangLV == 1)
                .Select(x => new EmployeeValidation { ID = x.ID, HoTen = x.MaNV + " - " + x.HoTen, IDPhongBan = (int)x.IDPhongBan })
                .ToList();

            int IDGVCty = db_context.SH_ChiTietTCDT.Where(x => x.CTDT_ID == data.IDLH).SingleOrDefault()?.ID_GVCty??0;

            ViewBag.ID_NhanVien = new SelectList(listNhanVien, "ID", "HoTen", IDGVCty);
            ViewBag.ID_NguoiKiemTra = new SelectList(listNhanVien.Where(x => x.IDPhongBan == IDPB), "ID", "HoTen", data?.NguoiKiemTra_ID);
            ViewBag.PhuongPhapDT_ID = PhuongPhapDT_ID;
            ViewBag.PhanLoaiNCDT_ID = PhanLoaiNCDT_ID;
            ViewBag.LoaiNCDT = PhanLoaiNCDT_ID;
            ViewBag.LoaiHinh_DT = db_context.SH_PhanLoaiNCDT.Where(x => x.IDLoai == PhanLoaiNCDT_ID).FirstOrDefault().LoaiHinhDT_ID;
            ViewBag.ChuongTrinhDT_ID = new SelectList(db_context.SH_ChuongTrinhDT.Where(x=>x.ID_NoiDungDT == data.NDID), "IDCTDT", "TenChuongTrinhDT", data?.CTDT_ID);

           
            ViewBag.Nam = res.NamDT;
            ViewBag.Quy = res.QuyDT;
            ViewBag.BoPhan_ID = new SelectList(db_context.PhongBans.Where(x => x.IDPhongBan == IDPB), "IDPhongBan", "TenPhongBan", IDPB);
            ViewBag.MaLH = data?.MaLH;
            
            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ManageClassValidation DTO, HttpPostedFileBase fileUpload, string action, int? PhuongPhapDT_ID, int? PhanLoaiNCDT_ID)
        {
            try
            {
                if (ModelState.IsValid || true)
                {
                    var lopHocExisting = db_context.LopHocs.Where(x => x.IDLH == DTO.IDLH).FirstOrDefault();
                    if (lopHocExisting != null)
                    {
                        lopHocExisting.TenLH = DTO.TenLH;
                        lopHocExisting.NCDT_ID = DTO.NCDT_ID;
                        lopHocExisting.NoiDungTrichYeu = DTO.NoiDungTrichYeu;
                        lopHocExisting.TGBDLH = DTO.TGBDLH;
                        lopHocExisting.TGKTLH = DTO.TGKTLH;
                        lopHocExisting.ThoiLuongDT = DTO.ThoiLuongDT;
                        lopHocExisting.DiaDiemDT = DTO.DiaDiemDT;
                        lopHocExisting.CTDT_ID =DTO.CTDT_ID;
                        var nhuCauDT = db_context.SH_NhuCauDT.Where(x => x.ID == lopHocExisting.NCDT_ID).FirstOrDefault();
                        lopHocExisting.NDID = nhuCauDT.NoiDungDT_ID;

                        int? IDGVCty = DTO.chiTietToChucDTTH.ID_GVCty;
                        lopHocExisting.GVID = IDGVCty;

                        var chiTietTCDTInDB = db_context.SH_ChiTietTCDT.Where(x => x.CTDT_ID == lopHocExisting.IDLH).SingleOrDefault();
                        var nhanVienInDb = db_context.NhanViens.Where(x => x.ID == IDGVCty).SingleOrDefault();


                        if (nhanVienInDb != null)
                        {
                            chiTietTCDTInDB.ID_GVCty = IDGVCty;
                            chiTietTCDTInDB.MaNV_GV = nhanVienInDb != null ? nhanVienInDb.MaNV : "";
                            chiTietTCDTInDB.HoTenGV = nhanVienInDb != null ? nhanVienInDb.HoTen : "";
                            var viTriCongViecInDb = db_context.Vitris.Where(x => x.IDViTri == nhanVienInDb.IDViTri).SingleOrDefault();
                            chiTietTCDTInDB.ViTriCV_GV = viTriCongViecInDb != null ? viTriCongViecInDb.TenViTri : "";
                            var phongBanInDb = db_context.PhongBans.Where(x => x.IDPhongBan == nhanVienInDb.IDPhongBan).SingleOrDefault();
                            chiTietTCDTInDB.DonVi_GV = phongBanInDb != null ? phongBanInDb.TenPhongBan : "";
                            lopHocExisting.GVID = IDGVCty; // lưu IDGV vào LopHoc
                        }
                        else
                        {
                            lopHocExisting.GVID = 0;
                            chiTietTCDTInDB.ID_GVCty = 0;
                            chiTietTCDTInDB.MaNV_GV = "";
                            chiTietTCDTInDB.HoTenGV = DTO.chiTietToChucDTTH?.HoTenGV ?? "";
                            chiTietTCDTInDB.ViTriCV_GV = DTO.chiTietToChucDTTH?.ViTriCV_GV ?? "";
                            chiTietTCDTInDB.DonVi_GV = DTO.chiTietToChucDTTH?.DonVi_GV ?? "";
                        }


                        lopHocExisting.NguoiKiemTra_ID = DTO.NguoiKiemTra_ID;
                        lopHocExisting.NguoiTao_ID = lopHocExisting.NguoiTao_ID;
                        lopHocExisting.NgayTao = lopHocExisting.NgayTao;
                        lopHocExisting.IDDeThi = DTO.IDDeThi;
                        lopHocExisting.IsCoCTDT = DTO.IsCoCTDT;
                    }
                    var ncdt = db_context.SH_NhuCauDT.FirstOrDefault(x => x.ID == DTO.NCDT_ID);
                    // thêm học viên vào lớp
                    if (DTO.IsAll)
                    {
                        var dsvt = db_context.SH_ViTri_NDDT.Where(x => x.NCDT_ID == DTO.NCDT_ID).ToList();
                       
                        foreach (var item in dsvt)
                        {
                            var nhanvien = db_context.NhanViens.Where(x => x.IDVTKNL == item.Vitri_ID && x.IDTinhTrangLV == 1).ToList();
                            foreach (var item1 in nhanvien)
                            {
                                //var checknv = db_context.XNHocTaps.Where(x => x.NVID == item1.ID && x.LHID == DTO.IDLH).FirstOrDefault();
                                bool check = CheckDKHocVien(item1.ID, DTO.IDLH, ncdt.NoiDungDT_ID, ncdt.PhuongPhapDT_ID, ncdt.ID);
                                if (check)
                                {
                                    var XNHT = new XNHocTap()
                                    {
                                        LHID = DTO.IDLH,
                                        NVID = item1.ID,
                                        PBID = item1.IDPhongBan,
                                        VTID = item1.IDViTri,
                                        NgayTG = default(DateTime),
                                        NgayHT = default(DateTime),
                                        XNHT = false,
                                        XNTG = false,
                                        IDND = ncdt.NoiDungDT_ID,
                                        ID_PhuongPhapDT = ncdt.PhuongPhapDT_ID
                                    };
                                    db_context.XNHocTaps.Add(XNHT);
                                    db_context.SaveChanges();
                                }
                            }
                        }
                        var dsnv = db_context.SH_KetQuaDaoTao.Where(x => x.NCDT_ID == DTO.NCDT_ID).ToList();
                        foreach (var item in dsnv)
                        {
                            var nhanvien = db_context.NhanViens.Where(x => x.ID == item.NhanVien_ID && x.IDTinhTrangLV == 1).FirstOrDefault();
                            if (nhanvien != null)
                            {
                                //var checknv = db_context.XNHocTaps.Where(x => x.NVID == nhanvien.ID && x.LHID == DTO.IDLH).FirstOrDefault();
                                bool check = CheckDKHocVien(nhanvien.ID, DTO.IDLH, ncdt.NoiDungDT_ID, ncdt.PhuongPhapDT_ID, ncdt.ID);
                                if (check)
                                {
                                    var XNHT = new XNHocTap()
                                    {
                                        LHID = DTO.IDLH,
                                        NVID = nhanvien.ID,
                                        PBID = nhanvien.IDPhongBan,
                                        VTID = nhanvien.IDViTri,
                                        NgayTG = default(DateTime),
                                        NgayHT = default(DateTime),
                                        XNHT = false,
                                        XNTG = false,
                                        IDND = ncdt.NoiDungDT_ID,
                                        ID_PhuongPhapDT = ncdt.PhuongPhapDT_ID
                                    };
                                    db_context.XNHocTaps.Add(XNHT);
                                    db_context.SaveChanges();
                                }
                            }

                        }
                    }
                    // check ds học viên có học viên để mở lớp
                    if (!String.IsNullOrEmpty(DTO.DSHocVien))
                    {
                        //Regex.Replace(_DO.NVDG, @"[^0-9a-zA-Z]+", " ");
                        string tx = Regex.Replace(DTO.DSHocVien, @"[^0-9a-zA-Z]+", " ");
                        string[] NVS = tx.Split(new char[] { ' ' });
                        foreach (var item in NVS)
                        {
                            var aa = db_context.NhanViens.Where(x => x.MaNV == item).FirstOrDefault();
                            if (aa != null)
                            {
                                //var checktrung = db_context.XNHocTaps.Where(x => x.NVID == aa.ID && x.LHID == DTO.IDLH).ToList();
                                bool check = CheckDKHocVien(aa.ID, DTO.IDLH, ncdt.NoiDungDT_ID, ncdt.PhuongPhapDT_ID, ncdt.ID);
                                if (check)
                                {
                                    var XNHT = new XNHocTap()
                                    {
                                        LHID = DTO.IDLH,
                                        NVID = aa.ID,
                                        PBID = aa.IDPhongBan,
                                        VTID = aa.IDViTri,
                                        NgayTG = default(DateTime),
                                        NgayHT = default(DateTime),
                                        XNHT = false,
                                        XNTG = false,
                                        IDND = ncdt.NoiDungDT_ID,
                                        ID_PhuongPhapDT = ncdt.PhuongPhapDT_ID
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
                            var nv = db_context.NhanViens.Where(x => x.MaNV == manv).FirstOrDefault();
                            if (nv != null)
                            {
                                //var checktrung = db_context.XNHocTaps.Where(x => x.NVID == nv.ID && x.LHID == DTO.IDLH).ToList();
                                bool check = CheckDKHocVien(nv.ID, DTO.IDLH, ncdt.NoiDungDT_ID, ncdt.PhuongPhapDT_ID, ncdt.ID);
                                if (check)
                                {
                                    var XNHT = new XNHocTap()
                                    {
                                        LHID = DTO.IDLH,
                                        NVID = nv.ID,
                                        PBID = nv.IDPhongBan,
                                        VTID = nv.IDViTri,
                                        NgayTG = default(DateTime),
                                        NgayHT = default(DateTime),
                                        XNHT = false,
                                        XNTG = false,
                                        IDND = ncdt.NoiDungDT_ID,
                                        ID_PhuongPhapDT = ncdt.PhuongPhapDT_ID
                                    };
                                    db_context.XNHocTaps.Add(XNHT);
                                    db_context.SaveChanges();
                                }
                            }
                        }
                    }

                    // Thêm thông tin trình ký
                    if (action == "Cập nhật")
                    {
                        lopHocExisting.TinhTrang = 0; // Đang lưu
                        lopHocExisting.NgayKiemTra = null;
                        db_context.SaveChanges();
                    }
                    else if(action == "Trình ký tổ chức đào tạo")
                    {
                        lopHocExisting.TinhTrang = 2; // Đang trình ký
                        lopHocExisting.NgayKiemTra = null;
                        db_context.SaveChanges();
                    }
                    // kiểm tra có nộp hồ sơ chưa thì xóa
                    var checkhoso = db_context.SH_HoSoDaoTao.FirstOrDefault(x => x.LHID == lopHocExisting.IDLH);
                    if(checkhoso != null)
                    {
                        db_context.SH_HoSoDaoTao.Remove(checkhoso);
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
            
            return RedirectToAction("Index", "ToChucDTTH", new { PhuongPhapDT_ID = PhuongPhapDT_ID, PhanLoaiNCDT_ID = PhanLoaiNCDT_ID });
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

        public ActionResult ViewEdit(int? IDLH)
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);

            var res = db_context.LopHocs.Find(IDLH);
            List<NhuCauDTTHView> ncdtData = (from a in db_context.SH_NhuCauDT.Where(x => x.ID == res.NCDT_ID)
                                             select new NhuCauDTTHView
                                             {
                                                 ID_NCDT = a.ID,
                                                 TenNoiDungDT = "Mã NCĐT: " + a.ID + " - " + a.NoiDungDT.NoiDung
                                             }).ToList();

            var data = new ManageClassValidation();
            if (res != null)
            {
                var chitietGV = db_context.SH_ChiTietTCDT.FirstOrDefault(x => x.CTDT_ID == res.IDLH);
                data.MaLH = res.MaLH;
                data.TenLH = res.TenLH;
                data.NoiDungTrichYeu = res.NoiDungTrichYeu;
                data.TGBDLH = res.TGBDLH.Value;
                data.TGKTLH = res.TGKTLH.Value;
                data.DiaDiemDT = res.DiaDiemDT;
                data.ThoiLuongDT = res.ThoiLuongDT;
                data.NCDT_ID = (int)res.NCDT_ID;
                data.QuyDT = (int)res.QuyDT;
                data.IDDeThi = (int)res.IDDeThi;
                data.NguoiKiemTra_ID = res.NguoiKiemTra_ID;
                data.IDLH = res.IDLH;
                data.IsCoCTDT = res.IsCoCTDT;
                data.CTDT_ID = res.CTDT_ID;
                data.NDID = (int)res.NDID;
                data.NguoiTao_ID = res.NguoiTao_ID;
                data.NgayKiemTra = res.NgayKiemTra;
                if (chitietGV != null)
                {
                    data.chiTietToChucDTTH = new ChiTietToChucDTTHView()
                    {
                        DonVi_GV = chitietGV.DonVi_GV,
                        HoTenGV = chitietGV.HoTenGV,
                        MaNV_GV = chitietGV.MaNV_GV,
                        ViTriCV_GV = chitietGV.ViTriCV_GV,
                    };
                }
            }

            ViewBag.NCDT_DATA = new SelectList(ncdtData, "ID_NCDT", "TenNoiDungDT", data.NCDT_ID);
            //ViewBag.HocVien = (from a in  db_context.XNHocTaps.Where(x=>x.LHID == data.IDLH)
            //                  join b in db_context.NhanViens on a.NVID equals b.ID 
            //                  select b).ToList();

            ViewBag.HocVien = (from h in db_context.XNHocTaps.Where(x => x.LHID == data.IDLH)
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
                             TLDT = l.NoiDungDT.ThoiLuongDT ?? 0,
                             NgayTG = (DateTime)(h.NgayTG ?? default(DateTime)),
                             NgayHT = (DateTime)(h.NgayHT ?? default(DateTime)),
                             XNTG = (bool)h.XNTG,
                             XNHT = (bool)h.XNHT,
                             DiemOnline = h.DiemOnline,
                             DiemLyThuyet = h.DiemLyThuyet,
                             DiemThucHanh = h.DiemThucHanh,
                             DiemVanDap = h.DiemVanDap,
                             KetLuan = h.KetLuan,
                             LyDoKhongTGia = h.LyDoKhongTGia,
                             TinhTrang = h.TinhTrang,
                         }).ToList();


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

            ViewBag.NoiDung = db_context.NoiDungDTs.Where(x=>x.IDND == data.NDID).FirstOrDefault();

            var listNhanVien = db_context.NhanViens.Where(x => x.IDTinhTrangLV == 1)
                .Select(x => new EmployeeValidation { ID = x.ID, HoTen = x.MaNV + " - " + x.HoTen, IDPhongBan = (int)x.IDPhongBan })
                .ToList();

            int IDGVCty = db_context.SH_ChiTietTCDT.Where(x => x.CTDT_ID == data.IDLH).SingleOrDefault()?.ID_GVCty ?? 0;

            ViewBag.NguoiTao = db_context.NhanViens.FirstOrDefault(x=>x.ID == data.NguoiTao_ID);
            ViewBag.NguoiKiemTra = db_context.NhanViens.FirstOrDefault(x=>x.ID == data.NguoiKiemTra_ID);
            //ViewBag.PhuongPhapDT_ID = PhuongPhapDT_ID;
            //ViewBag.PhanLoaiNCDT_ID = PhanLoaiNCDT_ID;
            //ViewBag.LoaiNCDT = PhanLoaiNCDT_ID;
            //ViewBag.LoaiHinh_DT = db_context.SH_PhanLoaiNCDT.Where(x => x.IDLoai == PhanLoaiNCDT_ID).FirstOrDefault().LoaiHinhDT_ID;
            ViewBag.ChuongTrinhDT_ID = new SelectList(db_context.SH_ChuongTrinhDT.Where(x => x.ID_NoiDungDT == data.NDID), "IDCTDT", "TenChuongTrinhDT", data.CTDT_ID);

            var IDPB = MyAuthentication.IDPhongban;
            ViewBag.Nam = db_context.SH_QuyDaoTao.First().AD_Nam;
            ViewBag.Quy = db_context.SH_QuyDaoTao.First().AD_Quy;
            ViewBag.BoPhan_ID = db_context.PhongBans.FirstOrDefault(x => x.IDPhongBan == IDPB).TenPhongBan;
            ViewBag.MaLH = data.MaLH;

            return View(data);
        }

        public ActionResult XLHS(int id, int? page, int? IDPhongBan, string search) {
            List<PhongBan> pb = db_context.PhongBans.ToList();
            ViewBag.PBList = new SelectList(pb, "IDPhongBan", "TenPhongBan", IDPhongBan);
            if (IDPhongBan == null) IDPhongBan = 0;
            if (search == null) search = "";
            ViewBag.search = search;

            var model = (from h in db_context.XNHocTaps.Where(x => x.LHID == id && (search == "" || search == null || x.NhanVien.MaNV.Contains(search)) && (IDPhongBan == 0 || IDPhongBan == null || x.PBID == IDPhongBan))
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
                             TLDT = l.NoiDungDT.ThoiLuongDT ?? 0,
                             NgayTG = (DateTime)(h.NgayTG ?? default(DateTime)),
                             NgayHT = (DateTime)(h.NgayHT ?? default(DateTime)),
                             XNTG = (bool)h.XNTG,
                             XNHT = (bool)h.XNHT,
                             DiemOnline = h.DiemOnline,
                             DiemLyThuyet = h.DiemLyThuyet,
                             DiemThucHanh =h.DiemThucHanh,
                             DiemVanDap =h.DiemVanDap,
                             KetLuan = h.KetLuan,
                             LyDoKhongTGia =h.LyDoKhongTGia,
                             TinhTrang = h.TinhTrang,
                         }).ToList();
            ViewBag.ID_NguoiTao = db_context.LopHocs.FirstOrDefault(x=>x.IDLH == id).NguoiTao_ID;
            var lh = db_context.LopHocs.FirstOrDefault(x => x.IDLH == id).TinhTrang;
            List<int> numbers = new List<int> { 0, 2, 3, 5};
            ViewBag.TinhTrangLH =numbers.Contains(lh??0) == true ?0:1;
            if (page == null) page = 1;
            int pageSize = 50;
            int pageNumber = (page ?? 1);
            return View(model.ToPagedList(pageNumber, pageSize)); 
        }

        public ActionResult ExportToExcel(int? id)
        {
            var data = (from h in db_context.XNHocTaps.Where(x => x.LHID == id)
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
                            TLDT = l.NoiDungDT.ThoiLuongDT ?? 0,
                            NgayTG = (DateTime)(h.NgayTG ?? default(DateTime)),
                            NgayHT = (DateTime)(h.NgayHT ?? default(DateTime)),
                            XNTG = (bool)h.XNTG,
                            XNHT = (bool)h.XNHT,
                            DiemOnline = h.DiemOnline,
                            DiemLyThuyet = h.DiemLyThuyet,
                            DiemThucHanh = h.DiemThucHanh,
                            DiemVanDap = h.DiemVanDap,
                            KetLuan = h.KetLuan,
                            LyDoKhongTGia = h.LyDoKhongTGia,
                            TinhTrang = h.TinhTrang,
                        }).ToList();
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("LopHoc");
                //Header
                worksheet.Cell(1, 1).Value = "STT";
                worksheet.Cell(1, 2).Value = "Mã học tập";
                worksheet.Cell(1, 3).Value = "Mã Nhân viên";
                worksheet.Cell(1, 4).Value = "Họ và tên";
                worksheet.Cell(1, 5).Value = "Vị trí công việc";
                worksheet.Cell(1, 6).Value = "Bộ phận";
                worksheet.Cell(1, 7).Value = "Tên lớp học";
                worksheet.Cell(1, 8).Value = "Điểm Online";
                worksheet.Cell(1, 9).Value = "Điểm lý thuyết";
                worksheet.Cell(1, 10).Value = "Điểm thực hành";
                worksheet.Cell(1, 11).Value = "Điểm vấn đáp";
                worksheet.Cell(1, 12).Value = "Kết luận Đạt";
                worksheet.Cell(1, 13).Value = "Kết luận Không Đạt";
                worksheet.Cell(1, 14).Value = "Kết luận Không tham gia";
                worksheet.Cell(1, 15).Value = "Lý do Không tham gia";
                //worksheet.Cell(1, 16).Value = "Địa điểm đào tạo dự kiến";
                //worksheet.Cell(1, 16).Value = "Người tạo";
                //worksheet.Cell(1, 17).Value = "Tình trạng";
                //worksheet.Cell(1, 18).Value = "Nhóm nhu cầu đào tạo";
                //worksheet.Cell(1, 19).Value = "Ghi chú";
                //value
                //worksheet.Cell(2, 1).Value = 1;
                //worksheet.Cell(2, 2).Value = "John Doe";
                //worksheet.Cell(2, 3).Value = 30;
                int row = 2; int stt = 1;
                foreach (var item in data)
                {
                    worksheet.Cell(row, 1).Value = stt;
                    worksheet.Cell(row, 2).Value = item.IDHT;
                    worksheet.Cell(row, 3).Value = item.MaNV;
                    worksheet.Cell(row, 4).Value = item.HoTenHV;
                    worksheet.Cell(row, 5).Value = item.TenVT;
                    worksheet.Cell(row, 6).Value = item.TenPB;
                    worksheet.Cell(row, 7).Value = item.TenLH;
                    worksheet.Cell(row, 8).Value = item.DiemOnline;
                    worksheet.Cell(row, 9).Value = item.DiemLyThuyet;
                    worksheet.Cell(row, 10).Value = item.DiemThucHanh;
                    worksheet.Cell(row, 11).Value = item.DiemVanDap;
                    worksheet.Cell(row, 12).Value = item.KetLuan ==1?"X":"";
                    worksheet.Cell(row, 13).Value = item.KetLuan == 2 ? "X" : "";
                    worksheet.Cell(row, 14).Value = item.KetLuan == 3 ? "X" : "";
                    worksheet.Cell(row, 15).Value = item.LyDoKhongTGia;
                    //worksheet.Cell(row, 15).Value = item.chiTietNhuCauDTTHView.DiaDiemDT;
                    //worksheet.Cell(row, 19).Value = item.chiTietNhuCauDTTHView.GhiChu;
                    row++; stt++;
                }

                var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0; // Reset con trỏ stream về đầu
                string filename = "DanhSachHocVien_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".xlsx";

                return File(stream, System.Net.Mime.MediaTypeNames.Application.Octet, filename);
            }
        }

        public ActionResult ImportXuLyExcel(int? id)
        {
            ViewBag.LopHoc = db_context.LopHocs.FirstOrDefault(x => x.IDLH == id);
            var nhanvien = db_context.NhanViens.Where(x => x.IDTinhTrangLV == 1).Select(x => new EmployeeValidation { ID = x.ID, HoTen = x.MaNV + " - " + x.HoTen, IDPhongBan = (int)x.IDPhongBan }).ToList();

            ViewBag.GiangVien = new SelectList(nhanvien, "ID", "HoTen");
            return PartialView();
        }

        [HttpPost]

        public ActionResult ImportXuLyExcel(HoSoDaoTaoTH _DO, int id)
        {
            var LHID = id;
            string filePath = string.Empty;
            // Kiểm tra thông tin
            if(_DO.NgayBDThucTe == null || _DO.NgayKTThucTe == null)
            {
                TempData["msg"] = "<script>alert('Vui lòng chọn ngày bắt đầu và kết thúc');</script>";
                return RedirectToAction("XLHS", "ToChucDTTH", new { id = LHID });
            }
            if (_DO.ID_GiangVien == null && _DO.HoTenGV == null)
            {
                TempData["msg"] = "<script>alert('Vui lòng bổ sung thông tin giảng viên thực tế');</script>";
                return RedirectToAction("XLHS", "ToChucDTTH", new { id = LHID });
            }
            if(_DO.ID_GiangVien != null)
            {
                var gv = db_context.NhanViens.FirstOrDefault(x=>x.ID == _DO.ID_GiangVien);
                _DO.HoTenGV = gv.HoTen;
                _DO.MaGiangVien = gv.MaNV;
                _DO.DonViGV = gv.PhongBan.TenPhongBan;
            }

            var lophoc = db_context.LopHocs.FirstOrDefault(x => x.IDLH == id);
            lophoc.TinhTrang = 4; // set tình trạng lớp thành đang trình hồ sơ
            db_context.SaveChanges();
            var ncdt = db_context.SH_NhuCauDT.FirstOrDefault(x => x.ID == lophoc.NCDT_ID);
            // file data học viên
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
                                    int MaXNHT = int.Parse(dt.Rows[i][1]?.ToString());
                                    var MaNV = dt.Rows[i][2].ToString();
                                    double? diemOnline = double.TryParse(dt.Rows[i][7]?.ToString(), out double s) ? (double?)s : null;
                                    double? diemLyThuyet = double.TryParse(dt.Rows[i][8]?.ToString(), out double ss) ? (double?)ss : null;
                                    double? diemThuchanh = double.TryParse(dt.Rows[i][9]?.ToString(), out double sx) ? (double?)sx : null;
                                    double? diemVanDap = double.TryParse(dt.Rows[i][10]?.ToString(), out double sl) ? (double?)sl : null;
                                    var KetLuanDat = dt.Rows[i][11]?.ToString();
                                    var KetLuanKDat = dt.Rows[i][12]?.ToString();
                                    var KetLuanKTGia = dt.Rows[i][13]?.ToString();
                                    var LyDo = dt.Rows[i][14]?.ToString();
                                    var nhanvien = db_context.NhanViens.FirstOrDefault(x => x.MaNV == MaNV);
                                    if(nhanvien != null)
                                    {
                                        var xnht = db_context.XNHocTaps.FirstOrDefault(x => x.IDHT == MaXNHT && x.NVID == nhanvien.ID);
                                        if(xnht != null)
                                        {
                                            xnht.DiemOnline = diemOnline;
                                            xnht.DiemLyThuyet = diemLyThuyet;
                                            xnht.DiemThucHanh = diemThuchanh;
                                            xnht.DiemVanDap = diemVanDap;
                                            xnht.LyDoKhongTGia = LyDo;
                                            if(KetLuanDat == "X" || KetLuanDat == "x")
                                            {
                                                xnht.KetLuan = 1;
                                            }
                                            else if (KetLuanKDat == "X" || KetLuanKDat == "x")
                                            {
                                                xnht.KetLuan = 2;
                                            }
                                            else if (KetLuanKTGia == "X" || KetLuanKTGia == "x")
                                            {
                                                xnht.KetLuan = 3;
                                            }
                                            xnht.IDND = ncdt.NoiDungDT_ID;
                                            xnht.ID_PhuongPhapDT = ncdt.PhuongPhapDT_ID;
                                        }
                                    }
                                }
                            }
                            db_context.SaveChanges();
                            string msg = "";

                        }
                        catch (Exception ex)
                        {
                            TempData["msgError"] = "<script>alert('Vui lòng kiểm tra lại file danh sách nhân viên');</script>";
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

            //File đính kèm
            if(_DO.fileScanHoSoViews != null)
            {
                var filehoso = db_context.SH_FileScanHoSo.Where(x => x.IDLH == lophoc.IDLH).ToList();
                string filedinhkem = null;
                string path = Server.MapPath("~/FileHoSo/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                foreach (var item in _DO.fileScanHoSoViews)
                {
                   if(item.FileDinhKem != null )
                    {
                        //Use Namespace called :  System.IO  
                        string FileName = item.FileDinhKem.FileName;
                        string FileNameSave = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + item.TenFile;
                        //To Get File Extension  
                        string FileExtension = item.FileDinhKem != null ? Path.GetExtension(FileName) : "";
                        //Add Current Date To Attached File Name  
                        if (FileExtension == ".pdf")
                        {
                            FileNameSave = FileNameSave.Trim() + FileExtension;
                            item.FileDinhKem.SaveAs(path + FileNameSave);
                            filedinhkem = "~/FileHoSo/" + FileNameSave;
                            if (string.IsNullOrEmpty(item.TenFile))
                            {
                                item.TenFile = FileName;
                            }
                            var hoso = new SH_FileScanHoSo() { FileDinhKem = filedinhkem, TenFile = item.TenFile, IDLH = lophoc.IDLH };
                            db_context.SH_FileScanHoSo.Add(hoso);
                            db_context.SaveChanges();
                        }
                        //else
                        //{
                        //    if (item.FileDinhKem != null)
                        //    {
                        //        FileNameSave = FileNameSave.Trim() + FileExtension;
                        //        item.FileDinhKem.SaveAs(path + FileNameSave);
                        //        filedinhkem = "~/FileHoSo/" + FileNameSave;
                        //    }
                        //}
                        //FileNameSave = FileNameSave.Trim() + FileExtension;
                        //item.FileDinhKem.SaveAs(path + FileNameSave);
                        //filedinhkem = "~/FileHoSo/" + FileNameSave;
                        //var hoso = new SH_FileScanHoSo() { FileDinhKem = filedinhkem, TenFile = item.TenFile, IDLH = lophoc.IDLH };
                        //db_context.SH_FileScanHoSo.Add(hoso);
                        //db_context.SaveChanges();
                    }
                }
            }
            // trình ký hồ sơ
            var checkky = db_context.SH_HoSoDaoTao.FirstOrDefault(x => x.LHID == lophoc.IDLH);
            if(checkky != null) { // xóa hồ sơ
                db_context.SH_HoSoDaoTao.Remove(checkky);
            }
            var trinhkyhoso = new SH_HoSoDaoTao()
            {
                LHID = lophoc.IDLH,
                NgayBDThucTe = _DO.NgayBDThucTe,
                NgayKTThucTe = _DO.NgayKTThucTe,
                ID_GiangVien = _DO.ID_GiangVien,
                HoTenGV = _DO.HoTenGV,
                MaGiangVien = _DO.MaGiangVien,
                DonViGV = _DO.DonViGV,
                ThoiLuongDT = _DO.ThoiLuongDT,
                ID_NguoiNopHS = MyAuthentication.ID,
                NgayNopHS = DateTime.Now,
                TinhTrang = 0
            };
           
            db_context.SH_HoSoDaoTao.Add(trinhkyhoso);
            db_context.SaveChanges();
            TempData["msgError"] = "<script>alert('Nộp hồ sơ thành công');</script>";
            return RedirectToAction("XLHS", "ToChucDTTH", new { id = LHID });
        }

    }
}