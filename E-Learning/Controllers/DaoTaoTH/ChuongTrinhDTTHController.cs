using E_Learning.Models;
using E_Learning.ModelsDTTH;
using E_Learning.Services;
using ExcelDataReader;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls.WebParts;

namespace E_Learning.Controllers.DaoTaoTH
{
    public class ChuongTrinhDTTHController : Controller
    {
        private ELEARNINGEntities db = new ELEARNINGEntities();
        int Idquyen = MyAuthentication.IDQuyen;
        String ControllerName = "ChuongTrinhDTTH";
        private readonly ChuongTrinhDTService _chuongtrinhDTService;
        public ChuongTrinhDTTHController()
        {
            _chuongtrinhDTService = new ChuongTrinhDTService(new ELEARNINGEntities()); // Tạo instance thủ công
        }
        // GET: ChuongTrinhDTTH
        public ActionResult Index(int? page, string search, int? IDPhuongPhapDT, int? ID_NoiDungDT)
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            ViewBag.QUYENCN = ListQuyen;
            if (!ListQuyen.Contains(CONSTKEY.V))
            {
                TempData["msgError"] = "<script>alert('Bạn không có quyền truy cập chức năng này');</script>";
                return RedirectToAction("", "Home");
            }
            var noiDungDTs = (from a in db.SH_ChuongTrinhDT.Where(x =>
                              //&& (search == null || x.NoiDung.Contains(search))
                              (IDPhuongPhapDT == null || x.IDPhuongPhapDT == IDPhuongPhapDT) &&
                              (ID_NoiDungDT == null || x.ID_NoiDungDT == ID_NoiDungDT))
                              select new ChuongTrinhDTTHView
                              {
                                  IDCTDT = a.IDCTDT,
                                  TenChuongTrinhDT = a.TenChuongTrinhDT,
                                  NoiDungTrichYeu = a.NoiDungTrichYeu,
                                  ThoiLuongDT = a.ThoiLuongDT,
                                  FileDinhKem = a.FileDinhKem,
                                  IDPhuongPhapDT = a.IDPhuongPhapDT,
                                  //TenPPDT = b.TenPhuongPhapDT,
                                  IDPhongBan =a.IDPhongBan,
                                  TenPhongBan = a.PhongBan.TenPhongBan,
                                  IsDelete = a.IsDelete,
                                  FileCTDT = a.FileCTDT,
                                  IDKiemTra=a.IDKiemTra,
                                  DoiTuongDT = a.DoiTuongDT,
                                  ID_NguoiTao = a.ID_NguoiTao,
                                  HoTen_NT = a.NhanVien.HoTen,
                                  MaNhanVien_NT = a.NhanVien.MaNV,
                                  ID_NoiDungDT = a.ID_NoiDungDT,
                                  TenNoiDungDT = a.NoiDungDT.NoiDung,
                                  NgayTao = a.NgayTao,
                                  TenNhomNL = a.NoiDungDT.NhomNLKCCD.NoiDung,
                                  TenLVDT =a.NoiDungDT.LinhVucDT.TenLVDT,
                                  TinhTrang = a.TinhTrang
                              }).OrderBy(x => x.NgayTao).ToList();
            if (!ListQuyen.Contains(CONSTKEY.VIEW_ALL) && !ListQuyen.Contains(CONSTKEY.V_BP)) noiDungDTs = noiDungDTs.Where(x => x.ID_NguoiTao == MyAuthentication.ID).ToList();
            else if (ListQuyen.Contains(CONSTKEY.V_BP)) noiDungDTs = noiDungDTs.Where(x => x.IDPhongBan == MyAuthentication.IDPhongban).ToList();

            ViewBag.ID_NoiDungDT = new SelectList(db.NoiDungDTs, "IDND", "NoiDung", ID_NoiDungDT);

            if (page == null) page = 1;
            int pageSize = 50;
            int pageNumber = (page ?? 1);
            return View(noiDungDTs.ToList().ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Create(int? ID_NCDT)
        {
            var ncdt = db.SH_NhuCauDT.Where(x=>x.ID == ID_NCDT).FirstOrDefault();
            var IDPB = MyAuthentication.IDPhongban;

            //ViewBag.IDPPDT = new SelectList(db.SH_PhuongPhapDT, "ID", "TenPhuongPhapDT");
            ViewBag.IDPhongBan = new SelectList(db.PhongBans.Where(x => x.IDPhongBan == IDPB), "IDPhongBan", "TenPhongBan", IDPB);
            
            ViewBag.IDNoiDungDT = new SelectList(db.NoiDungDTs.Where(x => x.IDPhuongPhapDT == 2 || x.IDPhuongPhapDT == 3).ToList(), "IDND", "NoiDung",ncdt?.NoiDungDT_ID);
            var idnhom = ncdt?.NoiDungDT.IDNhomNL;
            ViewBag.NhomNLID = new SelectList(db.NhomNLKCCDs.ToList(), "ID", "NoiDung",idnhom);
            ViewBag.LVDTID = new SelectList(db.LinhVucDTs.ToList(), "IDLVDT", "TenLVDT",ncdt?.NoiDungDT.LVDTID);
            //var ppdt = db.SH_PhuongPhapDT.ToList();
            ViewBag.PhuongPhapDT_ID = new SelectList(db.SH_PhuongPhapDT.Where(x=>x.ID == 2 || x.ID ==3 ), "ID", "TenPhuongPhapDT",ncdt?.PhuongPhapDT_ID);
            ViewBag.PhanLoaiNDDT = new SelectList(db.SH_PhanLoaiNDDT.ToList(), "ID", "TenPhanLoaiDT");

            // trình ký
            var nhanvien = db.NhanViens.Where(x => x.IDTinhTrangLV == 1).Select(x => new EmployeeValidation { ID = x.ID, HoTen = x.MaNV + " - " + x.HoTen, IDPhongBan = (int)x.IDPhongBan }).ToList();

            ViewBag.ID_NguoiKiemTra = new SelectList(nhanvien.Where(x => x.IDPhongBan == MyAuthentication.IDPhongban), "ID", "HoTen");
            ViewBag.ID_TPBP = new SelectList(nhanvien.Where(x => x.IDPhongBan == MyAuthentication.IDPhongban), "ID", "HoTen");
            ViewBag.ID_PCHN = new SelectList(nhanvien, "ID", "HoTen");
            //ViewBag.DoiTuong_DT = ncdt?.SH_ChiTiet_NCDT.FirstOrDefault().DoiTuongDT;
            //ViewBag.ThangDT = ncdt?.SH_ChiTiet_NCDT.FirstOrDefault().ThoiGian_DT;

            var model = new ChuongTrinhDTTHView();
            model.DoiTuongDT = ncdt?.SH_ChiTiet_NCDT.FirstOrDefault().DoiTuongDT;
            model.ThoiLuongDT = ncdt?.SH_ChiTiet_NCDT.FirstOrDefault().ThoiGian_DT;

            return PartialView(model);
        }

        // POST: NoiDungDTTH/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ChuongTrinhDTTHView chuongtrinh, FormCollection form, IEnumerable<HttpPostedFileBase> files, string action)
        {

            try
            {
                //foreach (var key in ModelState.Keys)
                //{
                //    foreach (var error in ModelState[key].Errors)
                //    {
                //        Console.WriteLine($"Lỗi tại {key}: {error.ErrorMessage}");
                //    }
                //}
                if (ModelState.IsValid || true)
                {
                    string filePathSave = null;
                    int countSl = 0;

                   // upload file CTDT
                    if (chuongtrinh.File != null)
                    {
                        string path = Server.MapPath("~/FileCTDTTH/");
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        //Use Namespace called :  System.IO  
                        string FileName = chuongtrinh.File.FileName;
                        string FileNameSave = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + Path.GetFileNameWithoutExtension(FileName);
                        //To Get File Extension  
                        string FileExtension = chuongtrinh.File != null ? Path.GetExtension(FileName) : "";
                        //Add Current Date To Attached File Name  
                        if (FileExtension != ".pdf")
                        {
                            //TempData["msgError"] = "<script>alert('Vui lòng chọn đúng định dạng file PDF');</script>";
                            //return View();
                        }
                        else
                        {
                            if (chuongtrinh.File != null)
                            {
                                FileNameSave = FileNameSave.Trim() + FileExtension;
                                chuongtrinh.File.SaveAs(path + FileNameSave);
                                filePathSave = "~/FileCTDTTH/" + FileNameSave;
                            }
                        }
                    }
                    chuongtrinh.FileCTDT = filePathSave;
                    var nddt = db.NoiDungDTs.Where(x => x.IDND == chuongtrinh.ID_NoiDungDT).FirstOrDefault();
                    chuongtrinh.TenChuongTrinhDT = nddt.NoiDung;
                    SH_ChuongTrinhDT chuongtrinhnew = _chuongtrinhDTService.ThemChuongTrinhDT(chuongtrinh, filePathSave);
                   
                    // Thêm Câu hỏi đề thi vào
                    if (chuongtrinh.cauHoiDeThiCTDTTHs.Count != 0)
                    {
                        foreach (var item in chuongtrinh.cauHoiDeThiCTDTTHs)
                        {
                            
                            if(item.TenDeThi == null && item.MaDeThi == null && item.FileDeThi == null && item.FileScanDeThi == null && item.DiemChuan == null )
                            {
                                continue;
                            }
                            // lưu đề thi vào
                            var dethi = new DeThi()
                            {
                                MaDe = item.MaDeThi,
                                TenDe = item.TenDeThi,
                                DiemChuan = item.DiemChuan,
                                GVID = MyAuthentication.ID,
                                TongSoCau = item.TongSoCau,
                                ThoiGianLamBai =item.ThoiGianLamBai,
                                CTDT_ID = chuongtrinhnew.IDCTDT,
                                IDND = chuongtrinhnew.ID_NoiDungDT
                            };
                           

                            if (item.MaDeThi == "DETHIGIAY" && item.FileScanDeThi != null)
                            {
                                string fileDeThi = null;
                                string path = Server.MapPath("~/FileCTDTTH/");
                                if (!Directory.Exists(path))
                                {
                                    Directory.CreateDirectory(path);
                                }
                                //Use Namespace called :  System.IO  
                                string FileName = item.FileScanDeThi.FileName;
                                string FileNameSave = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + Path.GetFileNameWithoutExtension(FileName);
                                //To Get File Extension  
                                string FileExtension = item.FileScanDeThi != null ? Path.GetExtension(FileName) : "";
                                //Add Current Date To Attached File Name  
                                if (FileExtension != ".pdf")
                                {
                                    //TempData["msgError"] = "<script>alert('Vui lòng chọn đúng định dạng file PDF');</script>";
                                    //return View();
                                }
                                else
                                {
                                    if (item.FileScanDeThi != null)
                                    {
                                        FileNameSave = FileNameSave.Trim() + FileExtension;
                                        item.FileScanDeThi.SaveAs(path + FileNameSave);
                                        fileDeThi = "~/FileCTDTTH/" + FileNameSave;
                                    }
                                }
                                dethi.FileDeThi = fileDeThi;
                                db.DeThis.Add(dethi);
                                db.SaveChanges();
                            }
                            else
                            {
                                db.DeThis.Add(dethi);
                                db.SaveChanges();
                                //lưu câu hỏi
                                Stream stream = item.FileDeThi.InputStream;

                                IExcelDataReader reader = null;
                                if (item.FileDeThi.FileName.EndsWith(".xls"))
                                {
                                    reader = ExcelReaderFactory.CreateBinaryReader(stream);
                                }
                                else if (item.FileDeThi.FileName.EndsWith(".xlsx"))
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

                                for (int i = 5; i < dt.Rows.Count; i++)
                                {
                                    string NoiDungCH = dt.Rows[i][1].ToString().Trim();

                                    string DapAnA = dt.Rows[i][2].ToString().Trim();

                                    string DapAnB = dt.Rows[i][3].ToString().Trim();

                                    string DapAnC = dt.Rows[i][4].ToString().Trim();

                                    string DapAnD = dt.Rows[i][5].ToString().Trim();

                                    string DapAnDung = dt.Rows[i][6].ToString().Trim();
                                    if (DapAnDung == "A" || DapAnDung == "a")
                                    {
                                        DapAnDung = "1";
                                    }
                                    else if (DapAnDung == "B" || DapAnDung == "b")
                                    {
                                        DapAnDung = "2";
                                    }
                                    else if (DapAnDung == "C" || DapAnDung == "c")
                                    {
                                        DapAnDung = "3";
                                    }
                                    else if (DapAnDung == "D" || DapAnDung == "d")
                                    {
                                        DapAnDung = "4";
                                    }
                                    // db.CauHoi_insert(NoiDungCH, DapAnA, DapAnB, DapAnC, DapAnD, Convert.ToInt32(DapAnDung), chuongtrinhnew.ID_NoiDungDT, MyAuthentication.ID);
                                    var ch = new CauHoi()
                                    {
                                        NoiDungCH = NoiDungCH,
                                        DapAnA = DapAnA,
                                        DapAnB = DapAnB,
                                        DapAnC = DapAnC,
                                        DapAnD = DapAnD,
                                        IDDAĐung = Convert.ToInt32(DapAnDung),
                                        IDND = chuongtrinhnew.ID_NoiDungDT,
                                        GVID = MyAuthentication.ID
                                    };
                                    db.CauHois.Add(ch);
                                    db.SaveChanges();
                                    // lưu câu hỏi đề thi
                                    string ketQua = (10 / (dt.Rows.Count - 5)).ToString("F1");
                                    double diemso = 10 / (dt.Rows.Count - 5);
                                    db.CauHoiDeThi_insert(ch.IDCH, dethi.IDDeThi, double.Parse(ketQua));
                                }
                            }
                           
                        }
                    }
                    // Thêm thông tin trình ký
                    chuongtrinh.IDCTDT = chuongtrinhnew.IDCTDT;
                    bool kyduyetctdt = _chuongtrinhDTService.ThemTrinhKyChuongTrinhDT(chuongtrinh);
                    if (action == "Lưu")
                    {
                        var kyduyet = db.SH_ChuongTrinhDT.Where(x => x.IDCTDT == chuongtrinhnew.IDCTDT).FirstOrDefault();
                        kyduyet.TinhTrang = 0; // Đang lưu
                        db.SaveChanges();
                    }

                    TempData["msgSuccess"] = "<script>alert('Thêm mới thành công ');</script>";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["msgSuccess"] = "<script>alert('Cập nhập thất bại " + ex.Message + " ');</script>";
            }
            return RedirectToAction("Index");
        }

        public ActionResult CreateNew()
        {
            var IDPB = MyAuthentication.IDPhongban;

            //ViewBag.IDPPDT = new SelectList(db.SH_PhuongPhapDT, "ID", "TenPhuongPhapDT");
            ViewBag.IDPhongBan = new SelectList(db.PhongBans.Where(x => x.IDPhongBan == IDPB), "IDPhongBan", "TenPhongBan", IDPB);

            //ViewBag.IDNoiDungDT = new SelectList(db.NoiDungDTs.Where(x => x.IDPhuongPhapDT == 2 || x.IDPhuongPhapDT == 3).ToList(), "IDND", "NoiDung");
            ViewBag.NhomNLID = new SelectList(db.NhomNLKCCDs.ToList(), "ID", "NoiDung");
            ViewBag.LVDTID = new SelectList(db.LinhVucDTs.ToList(), "IDLVDT", "TenLVDT");
            ViewBag.IDNguonGV = new SelectList(db.SH_NguonGV.ToList(), "ID", "TenNguonGV");
            ViewBag.IDHoatDongDT = new SelectList(db.SH_HoatDongDT.ToList(), "ID", "TenHoatDong");
            ViewBag.IDPhanLoaiDT = new SelectList(db.SH_PhanLoaiNDDT.ToList(), "ID", "TenPhanLoaiDT");
            ViewBag.PhuongPhapDT_ID = new SelectList(db.SH_PhuongPhapDT.Where(x => x.ID == 1), "ID", "TenPhuongPhapDT");

            // trình ký
            var nhanvien = db.NhanViens.Where(x => x.IDTinhTrangLV == 1).Select(x => new EmployeeValidation { ID = x.ID, HoTen = x.MaNV + " - " + x.HoTen, IDPhongBan = (int)x.IDPhongBan }).ToList();

            ViewBag.ID_NguoiKiemTra = new SelectList(nhanvien.Where(x => x.IDPhongBan == MyAuthentication.IDPhongban), "ID", "HoTen");
            ViewBag.ID_TPBP = new SelectList(nhanvien.Where(x => x.IDPhongBan == MyAuthentication.IDPhongban), "ID", "HoTen");
            ViewBag.ID_PCHN = new SelectList(nhanvien, "ID", "HoTen");
            //ViewBag.DoiTuong_DT = ncdt?.SH_ChiTiet_NCDT.FirstOrDefault().DoiTuongDT;
            //ViewBag.ThangDT = ncdt?.SH_ChiTiet_NCDT.FirstOrDefault().ThoiGian_DT;

            var model = new ChuongTrinhDTTHView();

            return PartialView(model);
        }

        // POST: NoiDungDTTH/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateNew(ChuongTrinhDTTHView chuongtrinh, FormCollection form, IEnumerable<HttpPostedFileBase> files, string action)
        {

            try
            {
                //foreach (var key in ModelState.Keys)
                //{
                //    foreach (var error in ModelState[key].Errors)
                //    {
                //        Console.WriteLine($"Lỗi tại {key}: {error.ErrorMessage}");
                //    }
                //}
                if (ModelState.IsValid)
                {
                    string filePathSave = null;
                    int countSl = 0;
                    // thêm mới NDĐT
                    if(chuongtrinh.TenNoiDungDT != null)
                    {
                        var ndd = new NoiDungDT
                        {
                            NoiDung = chuongtrinh.TenNoiDungDT,
                            IDNguonGV = chuongtrinh.noiDungDT.IDNguonGV,
                            IDHoatDongDT = chuongtrinh.noiDungDT.IDHoatDongDT,
                            LVDTID = chuongtrinh.ID_LVDT,
                            IDNhomNL = chuongtrinh.ID_NhomNL,
                            IDPhanLoaiDT = chuongtrinh.noiDungDT.IDPhanLoaiDT,
                            IsDelete = true, // Nội dung chưa được online
                            IDPhuongPhapDT = chuongtrinh.IDPhuongPhapDT,
                            BPLID = MyAuthentication.IDPhongban,
                            VideoND = chuongtrinh.noiDungDT.VideoND,
                            ImageND = chuongtrinh.noiDungDT.ImageND,
                        };
                        db.NoiDungDTs.Add(ndd);
                        db.SaveChanges();
                        chuongtrinh.ID_NoiDungDT = ndd.IDND; // set IDND cho CTDT
                    }
                    // upload file CTDT
                    if (chuongtrinh.File != null)
                    {
                        string path = Server.MapPath("~/FileCTDTTH/");
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        //Use Namespace called :  System.IO  
                        string FileName = chuongtrinh.File.FileName;
                        string FileNameSave = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + Path.GetFileNameWithoutExtension(FileName);
                        //To Get File Extension  
                        string FileExtension = chuongtrinh.File != null ? Path.GetExtension(FileName) : "";
                        //Add Current Date To Attached File Name  
                        if (FileExtension != ".pdf")
                        {
                            //TempData["msgError"] = "<script>alert('Vui lòng chọn đúng định dạng file PDF');</script>";
                            //return View();
                        }
                        else
                        {
                            if (chuongtrinh.File != null)
                            {
                                FileNameSave = FileNameSave.Trim() + FileExtension;
                                chuongtrinh.File.SaveAs(path + FileNameSave);
                                filePathSave = "~/FileCTDTTH/" + FileNameSave;
                            }
                        }
                    }
                    chuongtrinh.FileCTDT = filePathSave;
                    var nddt = db.NoiDungDTs.Where(x => x.IDND == chuongtrinh.ID_NoiDungDT).FirstOrDefault();
                    chuongtrinh.TenChuongTrinhDT = nddt.NoiDung;
                    SH_ChuongTrinhDT chuongtrinhnew = _chuongtrinhDTService.ThemChuongTrinhDT(chuongtrinh, filePathSave);

                    // Thêm Câu hỏi đề thi vào
                    if (chuongtrinh.cauHoiDeThiCTDTTHs.Count != 0)
                    {
                        foreach (var item in chuongtrinh.cauHoiDeThiCTDTTHs)
                        {

                            if (item.TenDeThi == null || item.MaDeThi == null || item.FileDeThi == null || item.DiemChuan == null)
                            {
                                continue;
                            }
                            // lưu đề thi vào
                            var dethi = new DeThi()
                            {
                                MaDe = item.MaDeThi,
                                TenDe = item.TenDeThi,
                                DiemChuan = item.DiemChuan,
                                GVID = MyAuthentication.ID,
                                TongSoCau = item.TongSoCau,
                                ThoiGianLamBai = item.ThoiGianLamBai,
                                CTDT_ID = chuongtrinhnew.IDCTDT,
                                IDND = chuongtrinhnew.ID_NoiDungDT
                            };
                            db.DeThis.Add(dethi);
                            db.SaveChanges();
                            //lưu câu hỏi
                            Stream stream = item.FileDeThi.InputStream;

                            IExcelDataReader reader = null;
                            if (item.FileDeThi.FileName.EndsWith(".xls"))
                            {
                                reader = ExcelReaderFactory.CreateBinaryReader(stream);
                            }
                            else if (item.FileDeThi.FileName.EndsWith(".xlsx"))
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

                            for (int i = 5; i < dt.Rows.Count; i++)
                            {
                                string NoiDungCH = dt.Rows[i][1].ToString().Trim();

                                string DapAnA = dt.Rows[i][2].ToString().Trim();

                                string DapAnB = dt.Rows[i][3].ToString().Trim();

                                string DapAnC = dt.Rows[i][4].ToString().Trim();

                                string DapAnD = dt.Rows[i][5].ToString().Trim();

                                string DapAnDung = dt.Rows[i][6].ToString().Trim();
                                if (DapAnDung == "A" || DapAnDung == "a")
                                {
                                    DapAnDung = "1";
                                }
                                else if (DapAnDung == "B" || DapAnDung == "b")
                                {
                                    DapAnDung = "2";
                                }
                                else if (DapAnDung == "C" || DapAnDung == "c")
                                {
                                    DapAnDung = "3";
                                }
                                else if (DapAnDung == "D" || DapAnDung == "d")
                                {
                                    DapAnDung = "4";
                                }
                                // db.CauHoi_insert(NoiDungCH, DapAnA, DapAnB, DapAnC, DapAnD, Convert.ToInt32(DapAnDung), chuongtrinhnew.ID_NoiDungDT, MyAuthentication.ID);
                                var ch = new CauHoi()
                                {
                                    NoiDungCH = NoiDungCH,
                                    DapAnA = DapAnA,
                                    DapAnB = DapAnB,
                                    DapAnC = DapAnC,
                                    DapAnD = DapAnD,
                                    IDDAĐung = Convert.ToInt32(DapAnDung),
                                    IDND = chuongtrinhnew.ID_NoiDungDT,
                                    GVID = MyAuthentication.ID
                                };
                                db.CauHois.Add(ch);
                                db.SaveChanges();
                                // lưu câu hỏi đề thi
                                string ketQua = (10 / (dt.Rows.Count - 5)).ToString("F1");
                                double diemso = 10 / (dt.Rows.Count - 5);
                                db.CauHoiDeThi_insert(ch.IDCH, dethi.IDDeThi, double.Parse(ketQua));
                            }
                        }
                    }
                    // Thêm thông tin trình ký
                    chuongtrinh.IDCTDT = chuongtrinhnew.IDCTDT;
                    bool kyduyetctdt = _chuongtrinhDTService.ThemTrinhKyChuongTrinhDT(chuongtrinh);
                    if (action == "Lưu")
                    {
                        var kyduyet = db.SH_ChuongTrinhDT.Where(x => x.IDCTDT == chuongtrinhnew.IDCTDT).FirstOrDefault();
                        kyduyet.TinhTrang = 0; // Đang lưu
                        db.SaveChanges();
                    }

                    TempData["msgSuccess"] = "<script>alert('Thêm mới thành công ');</script>";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["msgSuccess"] = "<script>alert('Cập nhập thất bại " + ex.Message + " ');</script>";
            }
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SH_ChuongTrinhDT chuongtrinh = db.SH_ChuongTrinhDT.Find(id);
            if (chuongtrinh == null)
            {
                return HttpNotFound();
            }
            var IDPB = MyAuthentication.IDPhongban;
            ViewBag.IDPhongBan = new SelectList(db.PhongBans.Where(x => x.IDPhongBan == IDPB), "IDPhongBan", "TenPhongBan", IDPB);
            //ViewBag.IDNoiDungDT = new SelectList(db.NoiDungDTs.Where(x=>x.IDND == nhuCauDT.NoiDungDT_ID), "IDND", "NoiDung",nhuCauDT.NoiDungDT_ID);
            ViewBag.IDNoiDungDT = new SelectList(db.NoiDungDTs.Where(x => x.IDND == chuongtrinh.ID_NoiDungDT).Select(x => new
            {
                Value = x.IDND,
                Text = x.IDND + " - " + db.SH_PhanLoaiNDDT.Where(a => x.IDND == chuongtrinh.ID_NoiDungDT).FirstOrDefault().TenPhanLoaiDT + " - " + x.NoiDung // Concatenate fields here
            }),
            "Value",
            "Text", chuongtrinh.ID_NoiDungDT);
            var noidung = db.NoiDungDTs.Where(x => x.IDND == chuongtrinh.ID_NoiDungDT).FirstOrDefault();
            ViewBag.PhanLoaiNDDT = new SelectList(db.SH_PhanLoaiNDDT.ToList(), "ID", "TenPhanLoaiDT", noidung.IDPhanLoaiDT);
            ViewBag.NhomNLID = new SelectList(db.NhomNLKCCDs.ToList(), "ID", "NoiDung", chuongtrinh.NoiDungDT.IDNhomNL);
            ViewBag.LVDTID = new SelectList(db.LinhVucDTs.ToList(), "IDLVDT", "TenLVDT", chuongtrinh?.NoiDungDT.LVDTID);

            ViewBag.PhuongPhapDT_ID = new SelectList(db.SH_PhuongPhapDT.Where(x => x.ID == chuongtrinh.IDPhuongPhapDT), "ID", "TenPhuongPhapDT", chuongtrinh.IDPhuongPhapDT);
            var nv2 = db.NhanViens.Where(x => x.IDTinhTrangLV == 1 && x.IDPhongBan == IDPB).Select(x => new EmployeeValidation { ID = x.ID, HoTen = x.MaNV + " - " + x.HoTen + "-" + x.Vitri.TenViTri }).ToList();
            //ViewBag.Selec = new SelectList(nv2, "ID", "HoTen", nhuCauDT.SH_ChiTiet_NCDT.FirstOrDefault().GiangVien_ID);

            // trình ký
            var trinhky = db.SH_KyDuyetCTDT.Where(x => x.ID_CTDT == chuongtrinh.IDCTDT).FirstOrDefault();
            var nhanvien = db.NhanViens.Where(x => x.IDTinhTrangLV == 1).Select(x => new EmployeeValidation { ID = x.ID, HoTen = x.MaNV + " - " + x.HoTen, IDPhongBan = (int)x.IDPhongBan }).ToList();

            ViewBag.ID_NguoiKiemTra = new SelectList(nhanvien.Where(x => x.IDPhongBan == MyAuthentication.IDPhongban), "ID", "HoTen",trinhky.ID_NguoiKiemTra);
            ViewBag.ID_TPBP = new SelectList(nhanvien.Where(x => x.IDPhongBan == MyAuthentication.IDPhongban), "ID", "HoTen",trinhky.ID_TPBP);
            ViewBag.ID_PCHN = new SelectList(nhanvien, "ID", "HoTen",trinhky.ID_PCHN);

            var noiDungDTs = (from a in db.SH_ChuongTrinhDT.Where(x => x.IDCTDT == id)
                              select new ChuongTrinhDTTHView
                              {
                                  IDCTDT = a.IDCTDT,
                                  TenChuongTrinhDT = a.TenChuongTrinhDT,
                                  NoiDungTrichYeu = a.NoiDungTrichYeu,
                                  ThoiLuongDT = a.ThoiLuongDT,
                                  FileDinhKem = a.FileDinhKem,
                                  IDPhuongPhapDT = a.IDPhuongPhapDT,
                                  IDPhongBan = a.IDPhongBan,
                                  TenPhongBan = a.PhongBan.TenPhongBan,
                                  IsDelete = a.IsDelete,
                                  FileCTDT = a.FileCTDT,
                                  IDKiemTra = a.IDKiemTra,
                                  DoiTuongDT = a.DoiTuongDT,
                                  ID_NguoiTao = a.ID_NguoiTao,
                                  HoTen_NT = a.NhanVien.HoTen,
                                  MaNhanVien_NT = a.NhanVien.MaNV,
                                  ID_NoiDungDT = a.ID_NoiDungDT,
                                  TenNoiDungDT = a.NoiDungDT.NoiDung,
                                  NgayTao = a.NgayTao,
                                  TenNhomNL = a.NoiDungDT.NhomNLKCCD.NoiDung,
                                  TenLVDT = a.NoiDungDT.LinhVucDT.TenLVDT,
                                  TinhTrang = a.TinhTrang,
                                  sH_ChiTietCTDTs = db.SH_ChiTietCTDT.Where(x=>x.ID_ChuongTrinhDT == id).ToList(),
                                  cauHoiDeThiCTDTTHs = (from d in db.DeThis.Where(x=>x.CTDT_ID ==id) select new CauHoiDeThiCTDTTH
                                  {
                                      ID = d.IDDeThi,
                                      MaDeThi = d.MaDe,
                                      TenDeThi = d.TenDe,
                                      DiemChuan = d.DiemChuan,
                                      ThoiGianLamBai = d.ThoiGianLamBai,
                                      TongSoCau =d.TongSoCau,
                                      fileDe = d.FileDeThi
                                  }).ToList(),
                              }).OrderBy(x => x.NgayTao).FirstOrDefault();

            return View(noiDungDTs);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ChuongTrinhDTTHView chuongtrinh, FormCollection form, IEnumerable<HttpPostedFileBase> files, string action)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string filePathSave = null;
                    int countSl = 0;

                    // upload file CTDT
                    if (chuongtrinh.File != null)
                    {
                        string path = Server.MapPath("~/FileCTDTTH/");
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        //Use Namespace called :  System.IO  
                        string FileName = chuongtrinh.File.FileName;
                        string FileNameSave = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + Path.GetFileNameWithoutExtension(FileName);
                        //To Get File Extension  
                        string FileExtension = chuongtrinh.File != null ? Path.GetExtension(FileName) : "";
                        //Add Current Date To Attached File Name  
                        if (FileExtension != ".pdf")
                        {
                            //TempData["msgError"] = "<script>alert('Vui lòng chọn đúng định dạng file PDF');</script>";
                            //return View();
                        }
                        else
                        {
                            if (chuongtrinh.File != null)
                            {
                                FileNameSave = FileNameSave.Trim() + FileExtension;
                                chuongtrinh.File.SaveAs(path + FileNameSave);
                                filePathSave = "~/FileCTDTTH/" + FileNameSave;
                            }
                        }
                    }
                    chuongtrinh.FileCTDT = filePathSave;
                    var nddt = db.NoiDungDTs.Where(x => x.IDND == chuongtrinh.ID_NoiDungDT).FirstOrDefault();
                    chuongtrinh.TenChuongTrinhDT = nddt.NoiDung;
                    SH_ChuongTrinhDT chuongtrinhupdate = _chuongtrinhDTService.CapNhatChuongTrinhDT(chuongtrinh);


                    // Thêm Câu hỏi đề thi vào
                    if (chuongtrinh.cauHoiDeThiCTDTTHs.Count != 0)
                    {
                        foreach (var item in chuongtrinh.cauHoiDeThiCTDTTHs)
                        {
                            if(item.TenDeThi == null && item.MaDeThi == null && item.FileDeThi == null && item.FileScanDeThi == null && item.DiemChuan == null && item.ID != 0) // xóa bộ đề thi
                            {
                                var ch = db.CauHoiDeThis.Where(x => x.IDDeThi == item.ID).ToList();
                                if(ch.Count > 0)
                                {
                                    foreach (var item1 in ch)
                                    {
                                        var h = db.CauHois.Where(x => x.IDCH == item1.IDCauHoi).FirstOrDefault();
                                        db.CauHois.Remove(h);
                                    }
                                    db.CauHoiDeThis.RemoveRange(ch);
                                    var det = db.DeThis.Where(x => x.IDDeThi == item.ID).FirstOrDefault();
                                    db.DeThis.Remove(det);
                                    db.SaveChanges();
                                }
                                
                            }
                            else if(item.ID != 0 ) // cập nhật lại đề thi
                            {
                                // lưu lại nội dung đề thi
                                var det = db.DeThis.Where(x => x.IDDeThi == item.ID).FirstOrDefault();
                                det.TenDe = item.TenDeThi;
                                det.MaDe = item.MaDeThi;
                                det.DiemChuan = item.DiemChuan;
                                det.ThoiGianLamBai = item.ThoiGianLamBai;
                                det.TongSoCau = item.TongSoCau;
                                // cập nhật đề thi giấy
                                if (item.MaDeThi == "DETHIGIAY" && item.FileScanDeThi != null)
                                {
                                    string fileDeThi = null;
                                    string path = Server.MapPath("~/FileCTDTTH/");
                                    if (!Directory.Exists(path))
                                    {
                                        Directory.CreateDirectory(path);
                                    }
                                    //Use Namespace called :  System.IO  
                                    string FileName = item.FileScanDeThi.FileName;
                                    string FileNameSave = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + Path.GetFileNameWithoutExtension(FileName);
                                    //To Get File Extension  
                                    string FileExtension = item.FileScanDeThi != null ? Path.GetExtension(FileName) : "";
                                    //Add Current Date To Attached File Name  
                                    if (FileExtension != ".pdf")
                                    {
                                        //TempData["msgError"] = "<script>alert('Vui lòng chọn đúng định dạng file PDF');</script>";
                                        //return View();
                                    }
                                    else
                                    {
                                        if (item.FileScanDeThi != null)
                                        {
                                            FileNameSave = FileNameSave.Trim() + FileExtension;
                                            item.FileScanDeThi.SaveAs(path + FileNameSave);
                                            fileDeThi = "~/FileCTDTTH/" + FileNameSave;
                                        }
                                    }
                                    det.FileDeThi = fileDeThi;
                                    db.SaveChanges();
                                }
                                if (item.FileDeThi != null) // cập nhật lại bộ câu hỏi mới cho đề thi
                                {
                                    // xóa CauHoiDeThi cũ
                                    var ch = db.CauHoiDeThis.Where(x => x.IDDeThi == item.ID).ToList();
                                    db.CauHoiDeThis.RemoveRange(ch);
                                    //lưu câu hỏi
                                    Stream streamt = item.FileDeThi.InputStream;

                                    IExcelDataReader readert = null;
                                    if (item.FileDeThi.FileName.EndsWith(".xls"))
                                    {
                                        readert = ExcelReaderFactory.CreateBinaryReader(streamt);
                                    }
                                    else if (item.FileDeThi.FileName.EndsWith(".xlsx"))
                                    {
                                        readert = ExcelReaderFactory.CreateOpenXmlReader(streamt);
                                    }
                                    else
                                    {
                                        TempData["msg"] = "<script>alert('Vui lòng chọn đúng định dạng file Excel');</script>";
                                        RedirectToAction("Index");
                                    }
                                    DataSet resultt = readert.AsDataSet();
                                    DataTable dtt = resultt.Tables[0];
                                    readert.Close();

                                    for (int i = 5; i < dtt.Rows.Count; i++)
                                    {
                                        string NoiDungCH = dtt.Rows[i][1].ToString().Trim();

                                        string DapAnA = dtt.Rows[i][2].ToString().Trim();

                                        string DapAnB = dtt.Rows[i][3].ToString().Trim();

                                        string DapAnC = dtt.Rows[i][4].ToString().Trim();

                                        string DapAnD = dtt.Rows[i][5].ToString().Trim();

                                        string DapAnDung = dtt.Rows[i][6].ToString().Trim();
                                        if (DapAnDung == "A" || DapAnDung == "a")
                                        {
                                            DapAnDung = "1";
                                        }
                                        else if (DapAnDung == "B" || DapAnDung == "b")
                                        {
                                            DapAnDung = "2";
                                        }
                                        else if (DapAnDung == "C" || DapAnDung == "c")
                                        {
                                            DapAnDung = "3";
                                        }
                                        else if (DapAnDung == "D" || DapAnDung == "d")
                                        {
                                            DapAnDung = "4";
                                        }
                                        // db.CauHoi_insert(NoiDungCH, DapAnA, DapAnB, DapAnC, DapAnD, Convert.ToInt32(DapAnDung), chuongtrinhnew.ID_NoiDungDT, MyAuthentication.ID);
                                        var chh = new CauHoi()
                                        {
                                            NoiDungCH = NoiDungCH,
                                            DapAnA = DapAnA,
                                            DapAnB = DapAnB,
                                            DapAnC = DapAnC,
                                            DapAnD = DapAnD,
                                            IDDAĐung = Convert.ToInt32(DapAnDung),
                                            IDND = nddt.IDND,
                                            GVID = MyAuthentication.ID
                                        };
                                        db.CauHois.Add(chh);
                                        db.SaveChanges();
                                        // lưu câu hỏi đề thi
                                        string ketQua = (10 / (dtt.Rows.Count - 5)).ToString("F1");
                                        double diemso = 10 / (dtt.Rows.Count - 5);
                                        db.CauHoiDeThi_insert(chh.IDCH, item.ID, double.Parse(ketQua));
                                    }
                                }
                            }
                            else // thêm mới
                            {
                                if ((item.TenDeThi == null || item.MaDeThi == null || item.FileDeThi == null || item.DiemChuan == null))
                                {
                                    continue;
                                }

                                // lưu đề thi vào
                                var dethi = new DeThi()
                                {
                                    MaDe = item.MaDeThi,
                                    TenDe = item.TenDeThi,
                                    DiemChuan = item.DiemChuan,
                                    GVID = MyAuthentication.ID,
                                    TongSoCau = item.TongSoCau,
                                    ThoiGianLamBai = item.ThoiGianLamBai,
                                    CTDT_ID = chuongtrinh.IDCTDT,
                                    IDND = chuongtrinh.ID_NoiDungDT
                                };
                               

                                if (item.MaDeThi == "DETHIGIAY" && item.FileScanDeThi != null)
                                {
                                    string fileDeThi = null;
                                    string path = Server.MapPath("~/FileCTDTTH/");
                                    if (!Directory.Exists(path))
                                    {
                                        Directory.CreateDirectory(path);
                                    }
                                    //Use Namespace called :  System.IO  
                                    string FileName = item.FileScanDeThi.FileName;
                                    string FileNameSave = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + Path.GetFileNameWithoutExtension(FileName);
                                    //To Get File Extension  
                                    string FileExtension = item.FileScanDeThi != null ? Path.GetExtension(FileName) : "";
                                    //Add Current Date To Attached File Name  
                                    if (FileExtension != ".pdf")
                                    {
                                        //TempData["msgError"] = "<script>alert('Vui lòng chọn đúng định dạng file PDF');</script>";
                                        //return View();
                                    }
                                    else
                                    {
                                        if (item.FileScanDeThi != null)
                                        {
                                            FileNameSave = FileNameSave.Trim() + FileExtension;
                                            item.FileScanDeThi.SaveAs(path + FileNameSave);
                                            fileDeThi = "~/FileCTDTTH/" + FileNameSave;
                                        }
                                    }
                                    dethi.FileDeThi = fileDeThi;
                                    db.DeThis.Add(dethi);
                                    db.SaveChanges();
                                }
                                else
                                {
                                    db.DeThis.Add(dethi);
                                    db.SaveChanges();
                                    //lưu câu hỏi
                                    Stream stream = item.FileDeThi.InputStream;

                                    IExcelDataReader reader = null;
                                    if (item.FileDeThi.FileName.EndsWith(".xls"))
                                    {
                                        reader = ExcelReaderFactory.CreateBinaryReader(stream);
                                    }
                                    else if (item.FileDeThi.FileName.EndsWith(".xlsx"))
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

                                    for (int i = 5; i < dt.Rows.Count; i++)
                                    {
                                        string NoiDungCH = dt.Rows[i][1].ToString().Trim();

                                        string DapAnA = dt.Rows[i][2].ToString().Trim();

                                        string DapAnB = dt.Rows[i][3].ToString().Trim();

                                        string DapAnC = dt.Rows[i][4].ToString().Trim();

                                        string DapAnD = dt.Rows[i][5].ToString().Trim();

                                        string DapAnDung = dt.Rows[i][6].ToString().Trim();
                                        if (DapAnDung == "A" || DapAnDung == "a")
                                        {
                                            DapAnDung = "1";
                                        }
                                        else if (DapAnDung == "B" || DapAnDung == "b")
                                        {
                                            DapAnDung = "2";
                                        }
                                        else if (DapAnDung == "C" || DapAnDung == "c")
                                        {
                                            DapAnDung = "3";
                                        }
                                        else if (DapAnDung == "D" || DapAnDung == "d")
                                        {
                                            DapAnDung = "4";
                                        }
                                        // db.CauHoi_insert(NoiDungCH, DapAnA, DapAnB, DapAnC, DapAnD, Convert.ToInt32(DapAnDung), chuongtrinhnew.ID_NoiDungDT, MyAuthentication.ID);
                                        var ch = new CauHoi()
                                        {
                                            NoiDungCH = NoiDungCH,
                                            DapAnA = DapAnA,
                                            DapAnB = DapAnB,
                                            DapAnC = DapAnC,
                                            DapAnD = DapAnD,
                                            IDDAĐung = Convert.ToInt32(DapAnDung),
                                            IDND = chuongtrinh.ID_NoiDungDT,
                                            GVID = MyAuthentication.ID
                                        };
                                        db.CauHois.Add(ch);
                                        db.SaveChanges();
                                        // lưu câu hỏi đề thi
                                        string ketQua = (10 / (dt.Rows.Count - 5)).ToString("F1");
                                        double diemso = 10 / (dt.Rows.Count - 5);
                                        db.CauHoiDeThi_insert(ch.IDCH, dethi.IDDeThi, double.Parse(ketQua));
                                    }
                                }
                                
                            }

                            // Thêm thông tin trình ký
                            // check xóa trình ký cũ
                            List<SH_KyDuyetCTDT> kycu = db.SH_KyDuyetCTDT.Where(x=>x.ID_CTDT == chuongtrinh.IDCTDT).ToList();
                            db.SH_KyDuyetCTDT.RemoveRange(kycu);
                            db.SaveChanges();
                            bool kyduyetctdt = _chuongtrinhDTService.ThemTrinhKyChuongTrinhDT(chuongtrinh);
                            if (action == "Lưu")
                            {
                                var kyduyet = db.SH_ChuongTrinhDT.Where(x => x.IDCTDT == chuongtrinh.IDCTDT).FirstOrDefault();
                                kyduyet.TinhTrang = 0; // Đang lưu
                                db.SaveChanges();
                            }
                            else // trình ký lại
                            {
                                var kyduyet = db.SH_ChuongTrinhDT.Where(x => x.IDCTDT == chuongtrinh.IDCTDT).FirstOrDefault();
                                kyduyet.TinhTrang = 2; // Đang trình ký
                                db.SaveChanges();
                            }


                        }
                    }

                    TempData["msgSuccess"] = "<script>alert('Cập nhật thành công ');</script>";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["msgSuccess"] = "<script>alert('Cập nhập thất bại " + ex.Message + " ');</script>";
            }
            return RedirectToAction("Index");
        }

        public ActionResult Edit_View(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SH_ChuongTrinhDT chuongtrinh = db.SH_ChuongTrinhDT.Find(id);
            if (chuongtrinh == null)
            {
                return HttpNotFound();
            }
            var IDPB = MyAuthentication.IDPhongban;
            ViewBag.IDPhongBan = new SelectList(db.PhongBans.Where(x => x.IDPhongBan == IDPB), "IDPhongBan", "TenPhongBan", IDPB);
            //ViewBag.IDNoiDungDT = new SelectList(db.NoiDungDTs.Where(x=>x.IDND == nhuCauDT.NoiDungDT_ID), "IDND", "NoiDung",nhuCauDT.NoiDungDT_ID);
            ViewBag.IDNoiDungDT = new SelectList(db.NoiDungDTs.Where(x => x.IDND == chuongtrinh.ID_NoiDungDT).Select(x => new
            {
                Value = x.IDND,
                Text = x.IDND + " - " + db.SH_PhanLoaiNDDT.Where(a => x.IDND == chuongtrinh.ID_NoiDungDT).FirstOrDefault().TenPhanLoaiDT + " - " + x.NoiDung // Concatenate fields here
            }),
            "Value",
            "Text", chuongtrinh.ID_NoiDungDT);
            ViewBag.NhomNLID = new SelectList(db.NhomNLKCCDs.ToList(), "ID", "NoiDung", chuongtrinh.NoiDungDT.IDNhomNL);
            ViewBag.LVDTID = new SelectList(db.LinhVucDTs.ToList(), "IDLVDT", "TenLVDT", chuongtrinh?.NoiDungDT.LVDTID);

            ViewBag.PhuongPhapDT_ID = new SelectList(db.SH_PhuongPhapDT.Where(x => x.ID == chuongtrinh.IDPhuongPhapDT), "ID", "TenPhuongPhapDT", chuongtrinh.IDPhuongPhapDT);
            var nv2 = db.NhanViens.Where(x => x.IDTinhTrangLV == 1 && x.IDPhongBan == IDPB).Select(x => new EmployeeValidation { ID = x.ID, HoTen = x.MaNV + " - " + x.HoTen + "-" + x.Vitri.TenViTri }).ToList();
            //ViewBag.Selec = new SelectList(nv2, "ID", "HoTen", nhuCauDT.SH_ChiTiet_NCDT.FirstOrDefault().GiangVien_ID);

            // trình ký
            var trinhky = db.SH_KyDuyetCTDT.Where(x => x.ID_CTDT == chuongtrinh.IDCTDT).FirstOrDefault();
            var nhanvien = db.NhanViens.Where(x => x.IDTinhTrangLV == 1).Select(x => new EmployeeValidation { ID = x.ID, HoTen = x.MaNV + " - " + x.HoTen, IDPhongBan = (int)x.IDPhongBan }).ToList();

            ViewBag.ID_NguoiKiemTra = new SelectList(nhanvien.Where(x => x.IDPhongBan == MyAuthentication.IDPhongban), "ID", "HoTen", trinhky.ID_NguoiKiemTra);
            ViewBag.ID_TPBP = new SelectList(nhanvien.Where(x => x.IDPhongBan == MyAuthentication.IDPhongban), "ID", "HoTen", trinhky.ID_TPBP);
            ViewBag.ID_PCHN = new SelectList(nhanvien, "ID", "HoTen", trinhky.ID_PCHN);

            var noiDungDTs = (from a in db.SH_ChuongTrinhDT.Where(x => x.IDCTDT == id)
                              //join b in db.SH_PhuongPhapDT on a.IDPhuongPhapDT equals b.ID
                              select new ChuongTrinhDTTHView
                              {
                                  IDCTDT = a.IDCTDT,
                                  TenChuongTrinhDT = a.TenChuongTrinhDT,
                                  NoiDungTrichYeu = a.NoiDungTrichYeu,
                                  ThoiLuongDT = a.ThoiLuongDT,
                                  FileDinhKem = a.FileDinhKem,
                                  //IDPhuongPhapDT = a.IDPhuongPhapDT,
                                  //TenPPDT = b.TenPhuongPhapDT,
                                  IDPhongBan = a.IDPhongBan,
                                  TenPhongBan = a.PhongBan.TenPhongBan,
                                  IsDelete = a.IsDelete,
                                  FileCTDT = a.FileCTDT,
                                  IDKiemTra = a.IDKiemTra,
                                  DoiTuongDT = a.DoiTuongDT,
                                  ID_NguoiTao = a.ID_NguoiTao,
                                  HoTen_NT = a.NhanVien.HoTen,
                                  MaNhanVien_NT = a.NhanVien.MaNV,
                                  ID_NoiDungDT = a.ID_NoiDungDT,
                                  TenNoiDungDT = a.NoiDungDT.NoiDung,
                                  NgayTao = a.NgayTao,
                                  TenNhomNL = a.NoiDungDT.NhomNLKCCD.NoiDung,
                                  TenLVDT = a.NoiDungDT.LinhVucDT.TenLVDT,
                                  TinhTrang = a.TinhTrang,
                                  sH_ChiTietCTDTs = db.SH_ChiTietCTDT.Where(x => x.ID_ChuongTrinhDT == id).ToList(),
                                  cauHoiDeThiCTDTTHs = (from d in db.DeThis.Where(x => x.CTDT_ID == id)
                                                        select new CauHoiDeThiCTDTTH
                                                        {
                                                            ID = d.IDDeThi,
                                                            MaDeThi = d.MaDe,
                                                            TenDeThi = d.TenDe,
                                                            DiemChuan = d.DiemChuan,
                                                            ThoiGianLamBai = d.ThoiGianLamBai,
                                                            TongSoCau = d.TongSoCau
                                                        }).ToList(),
                                  sH_KyDuyetCTDTs =db.SH_KyDuyetCTDT.Where(x=>x.ID_CTDT == id).FirstOrDefault()
                              }).OrderBy(x => x.NgayTao).FirstOrDefault();

            return View(noiDungDTs);
        }

        public ActionResult Question(int id)
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            ViewBag.QUYENCN = ListQuyen;
            //if (!ListQuyen.Contains(CONSTKEY.V))
            //{
            //    TempData["msgError"] = "<script>alert('Bạn không có quyền thực hiện chức năng này');</script>";
            //    return RedirectToAction("", "Home");
            //}
            var model = (from a in db.CauHoiDeThis.Where(x => x.IDDeThi == id)
                         join b in db.CauHois on a.IDCauHoi equals b.IDCH
                         join c in db.DanhSachDAs on b.IDDAĐung equals c.IDDSĐA
                         select new ManageQuestionValidation
                         {
                             IDCH = (int)a.IDCauHoi,
                             NoiDungCH = b.NoiDungCH,
                             DapAnA = b.DapAnA,
                             DapAnB = b.DapAnB,
                             DapAnC = b.DapAnC,
                             DapAnD = b.DapAnD,
                             DapAnĐung = c.TenĐA,
                             Diem = (double)a.Diem,
                             IDCauHoiDeThi = a.IDCauHoiDeThi

                         }).ToList();

            return View(model);
        }


        public JsonResult GetDSNoiDungDT(int? NhomNDDT_ID)
        {
            //var lsIDLoaiND = new List<int?> { 6, 7, 8 }; // list ID PhanLoaiNDDT thuê ngoài được lượt bỏ
            db.Configuration.ProxyCreationEnabled = false;
            var ls = new List<NoiDungDTTHView>();
            if (NhomNDDT_ID != null)
            {
                ls = (from a in db.NoiDungDTs.Where(x=>x.IDPhanLoaiDT == NhomNDDT_ID)
                      select new NoiDungDTTHView()
                      {
                          IDND = a.IDND,
                          NoiDung = a.IDND + "-" + a.MaND + "-" + a.NoiDung,
                          LVDTID = a.LVDTID,
                          TenLVDT = a.LinhVucDT.TenLVDT,
                          IDNhomNL = a.IDNhomNL,
                          TenNhomNL = a.NhomNLKCCD.NoiDung
                      }).ToList();
            }
            return Json(ls, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetChiTietNoiDungDT(int? NoiDungDT_ID)
        {
            //var lsIDLoaiND = new List<int?> { 6, 7, 8 }; // list ID PhanLoaiNDDT thuê ngoài được lượt bỏ
            db.Configuration.ProxyCreationEnabled = false;
            var ls = new NoiDungDTTHView();
            if (NoiDungDT_ID != null)
            {
                ls = (from a in db.NoiDungDTs.Where(x => x.IDND == NoiDungDT_ID)
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



    }
}