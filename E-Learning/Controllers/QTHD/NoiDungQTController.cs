using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using E_Learning.Models;
using E_Learning.ModelsKCCD;
using E_Learning.ModelsQTHD;
using ExcelDataReader;
using iTextSharp.text.pdf.codec;
using Microsoft.Ajax.Utilities;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace E_Learning.Controllers.QTHD
{
    public class NoiDungQTController : Controller
    {
        ELEARNINGEntities db = new ELEARNINGEntities();
        ELEARNINGEntities dbKCCCD = new ELEARNINGEntities();
        int Idquyen = MyAuthentication.IDQuyen;
        String ControllerName = "NoiDungQT";
        // GET: NoiDungQT
        public ActionResult Index(int? page, string search, int? IDPB, int? IDLVDT,int? IDQT,int? IDTinhTrang, int? IDMahieu)
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

            if (IDQT == null) IDQT = 0;
            if (IDMahieu == null) IDMahieu = 0;

            var res = (from a in db.QT_NoiDungQT
                       join d in db.PhongBans
                       on a.IDPhongBan equals d.IDPhongBan
                       join e in db.LinhVucDTs
                       on a.IDLVDT equals e.IDLVDT into ul
                       from e in ul.DefaultIfEmpty()
                       select new NoiDungQTView
                       {
                           IDQTHD = a.IDQTHD,
                           IDLVDT = a.IDLVDT,
                           TenLVDT = e.TenLVDT,
                           IDPhongBan = a.IDPhongBan,
                           TenPhongBan = d.TenPhongBan,
                           DiemChuan = a.DiemChuan,
                           IDLoaiQTHD = a.IDLoaiQTHD,
                           TenLoaiQTHD = a.QT_LoaiQT.TenLoai,
                           LanCapNhat = a.LanCapNhat,
                           MaHieu = a.MaHieu,
                           NgayHieuLuc = a.NgayHieuLuc??default,
                           NgayHetHieuLuc = a.NgayHetHieuLuc ?? default,
                           NoiDungCapNhat = a.NoiDungCapNhat,
                           TenQTHD = a.TenQTHD,
                           TinhTrang = a.TinhTrang,
                           List_FileQT = db.QT_FileQT.Where(x=>x.QTHDID == a.IDQTHD).OrderBy(x=>x.OrderByID).ToList(),
                           List_VanBanLQ = (from a in db.QT_VanBanLQ.Where(x => x.QTHDID == a.IDQTHD)
                                            join d in db.QT_NoiDungQT
                                            on a.IDQT_LienQuan equals d.IDQTHD
                                            select new NoiDungQTLienQuan
                                            {
                                                IDQTHD = d.IDQTHD,
                                                TenQTHD = d.MaHieu +"-"+ d.TenQTHD,
                                                TinhTrangHL = d.NgayHieuLuc < DateTime.Now && (d.NgayHetHieuLuc == null || d.NgayHetHieuLuc == default || d.NgayHetHieuLuc > DateTime.Now) ? 1 : 0,
                                                TinhTrangQT =d.TinhTrang
                                            }).OrderBy(x => x.IDQTHD).ToList(),
                           SL_CauHoi = db.QT_CauHoiQT.Where(x=>x.QTHDID == a.IDQTHD).Count(),
                           SL_VTCVLQ =db.QT_PhanQuyen.Where(x=>x.QTHDID == a.IDQTHD).Count(),
                           SL_NVHT = db.QT_BaiKiemTra.Where(x=>x.QTHDID ==a.IDQTHD && x.TinhTrang ==1 && x.NgayKTTT >= DateTime.Now).Count(),
                           Total_SL_NV = (from a in db.QT_PhanQuyen.Where(x => x.QTHDID == a.IDQTHD)
                                          join d in db.NhanViens
                                          on a.IDVTKNL equals d.IDVTKNL
                                          select new {}
                                         ).Count(),
                           TinhTrangHL = a.NgayHieuLuc < DateTime.Now && (a.NgayHetHieuLuc == null || a.NgayHetHieuLuc == default || a.NgayHetHieuLuc > DateTime.Now)?1:a.NgayHieuLuc>DateTime.Now?2: 0,      
                       }).OrderBy(x => x.IDQTHD).ToList();
            //if (!ListQuyen.Contains(CONSTKEY.VIEW_ALL)) res = res.Where(x => x.PhongBanID == MyAuthentication.IDPhongban).ToList();

            List<PhongBan> dt = db.PhongBans.ToList();
            ViewBag.IDPB = new SelectList(dt, "IDPhongBan", "TenPhongBan", IDPB);
            List<LinhVucDT> lv = db.LinhVucDTs.ToList();
            ViewBag.LVDTID = new SelectList(lv, "IDLVDT", "TenLVDT", IDLVDT);

            List<QT_NoiDungQT> qt = db.QT_NoiDungQT.ToList();
            ViewBag.NoiDungQT = new SelectList(qt, "IDQTHD","TenQTHD" , IDQT);
            ViewBag.MaHieuQT = new SelectList(qt, "IDQTHD", "MaHieu", IDMahieu);

            //if (IDPB != 0) res = res.Where(x => x.PhongBanID == IDPB).ToList();
            if (IDQT != 0) res = res.Where(x => x.IDQTHD == IDQT).ToList();
            if (IDMahieu != 0) res = res.Where(x => x.IDQTHD == IDMahieu).ToList();
            if (IDTinhTrang != null) res = res.Where(x => x.TinhTrang == IDTinhTrang).ToList();
            if (IDPB != 0) res = res.Where(x => x.IDPhongBan == IDPB).ToList();
            if (IDLVDT != 0) res = res.Where(x => x.IDLVDT == IDLVDT).ToList();

            if (page == null) page = 1;
            int pageSize = 50;
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
            //db.Configuration.ProxyCreationEnabled = false;

            db.Configuration.ProxyCreationEnabled = false;
            List<PhongBan> dt = db.PhongBans.ToList();
            ViewBag.PhongBanID = new SelectList(dt, "IDPhongBan", "TenPhongBan");

            List<LinhVucDT> lv = db.LinhVucDTs.ToList();
            ViewBag.LVDTID = new SelectList(lv, "IDLVDT", "TenLVDT");

            List<QT_LoaiQT> loaiQT = db.QT_LoaiQT.ToList();
            ViewBag.IDLoaiQTHD = new SelectList(loaiQT, "IDLoai", "TenLoai");

            var NDQT = db.QT_NoiDungQT.ToList();
            List<NoiDungQTView> noidungQT = NDQT.Where(x => x.NgayHieuLuc < DateTime.Now && (x.NgayHetHieuLuc == null || x.NgayHetHieuLuc == default || x.NgayHetHieuLuc > DateTime.Now)).Select(x => new NoiDungQTView { IDQTHD = x.IDQTHD, TenQTHD = x.MaHieu + " - " + x.TenQTHD }).ToList();
            ViewBag.SelectedQT = new SelectList(noidungQT, "IDQTHD", "TenQTHD");

            var aa = db.QT_VanBanLQ.ToList();
            List<NoiDungQTView> qtvb = (from a in aa
                                        join b in NDQT on a.IDQT_LienQuan equals b.IDQTHD
                                        select new NoiDungQTView { IDQTHD = a.IDVB, TenQTHD = b.MaHieu + " - " + b.TenQTHD }).ToList();
            ViewBag.Selec = new SelectList(qtvb, "IDQTHD", "TenQTHD");

            return View();
        }

        [HttpPost]
        public ActionResult Create(IEnumerable<HttpPostedFileBase> files, NoiDungQTView _DO, FormCollection collection)
        {
            try
            {
                string msg = "";
                int dem = 0;
                int flag = 0;
                //Cap nhat Noi Dung QT
                if (_DO.MaHieu != "" && _DO.TenQTHD != "")
                {
                    var a = db.QT_NoiDungQT.Where(x => x.MaHieu == _DO.MaHieu || x.TenQTHD == _DO.TenQTHD).ToList();
                    if (a.Count() == 0)
                    {
                        DateTime? dt = null;
                        if (_DO.isActive == true) _DO.TinhTrang = 1; else _DO.TinhTrang = 0;
                        if (_DO.NgayHetHieuLuc != null && _DO.NgayHetHieuLuc != default) dt = _DO.NgayHetHieuLuc;
                        var b = db.QT_NoiDungQT_insert(_DO.MaHieu, _DO.TenQTHD, _DO.IDLoaiQTHD, _DO.IDPhongBan, _DO.IDLVDT, _DO.NgayHieuLuc, dt, 7, _DO.LanCapNhat, _DO.NoiDungCapNhat, DateTime.Now, _DO.TinhTrang);
                        flag = 1;
                    }

                }
                var aa = db.QT_NoiDungQT.Where(x => x.MaHieu == _DO.MaHieu ).ToList();
                if (aa.Count() != 0 && flag == 1) _DO.IDQTHD = aa.FirstOrDefault().IDQTHD;
                else
                {
                    TempData["msgSuccess"] = "<script>alert('Nội dung Quy trình đã tồn tại');</script>";
                    return RedirectToAction("Index", "NoiDungQT");
                }
                //update file
                //var listFile = db.QT_FileQT.Where(x => x.QTHDID == _DO.IDQTHD).ToList();
                //if (listFile != null)
                //{
                //    foreach (var item in listFile)
                //    {
                //        int? OrderbyID = ToNullableInt(collection["t_" + item.IDFile]);
                //        var a = db.QT_FileQT_UpdateOrderBy(item.IDFile, OrderbyID);
                //    }
                //}
                //upload file QT
                if (files != null)
                {
                    string path = Server.MapPath("~/FileQTHD/");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    int coun = 0;
                    foreach (var file in files)
                    {
                        if (file != null)
                        {
                            //Use Namespace called :  System.IO  
                            string FileName = file.FileName;
                            string FileNameSave = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + Path.GetFileNameWithoutExtension(FileName);
                            //To Get File Extension  
                            string FileExtension = file != null ? Path.GetExtension(FileName) : "";
                            //Add Current Date To Attached File Name  
                            if (FileExtension != ".pdf")
                            {
                                //TempData["msgError"] = "<script>alert('Vui lòng chọn đúng định dạng file PDF');</script>";
                                //return View();
                            }
                            else
                            {
                                string filePathSave = "";
                                int? OrderbyID = 0;
                                if (file != null)
                                {
                                    FileNameSave = FileNameSave.Trim() + FileExtension;
                                    file.SaveAs(path + FileNameSave);
                                    filePathSave = "~/FileQTHD/" + FileNameSave;
                                    OrderbyID = ToNullableInt(collection["tt_" + coun]);

                                }
                                var a = db.QT_FileQT_insert(FileName, _DO.IDQTHD, filePathSave, OrderbyID);
                                //TempData["msgSuccess"] = "<script>alert('Thêm mới thành công');</script>";
                                dem++;
                            }
                        }
                        coun++;
                    }
                }
                msg = "Thêm mới được" + dem + "QT/HD";
                // VB QT liên quan
                if (_DO.SelectedQT != null)
                {
                    for (int i = 0; i < _DO.SelectedQT.Length; i++)
                    {
                        int selc = _DO.SelectedQT[i];
                        var checkVBLQ = db.QT_VanBanLQ.Where(x => x.QTHDID == _DO.IDQTHD && x.IDQT_LienQuan == selc).ToList();
                        if (checkVBLQ.Count == 0)
                        {
                            var a = db.QT_QT_VanBanLQ_insert(_DO.IDQTHD, _DO.SelectedQT[i]);
                        }
                    }
                }
                // Upload câu hỏi QT
                if (Request != null)
                {
                    string filePath = string.Empty;
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
                            //TempData["msg"] = "<script>alert('Vui lòng chọn đúng định dạng file Excel');</script>";
                            return View();
                        }
                        DataSet result = reader.AsDataSet();
                        DataTable dt = result.Tables[0];
                        reader.Close();
                        int co = 0;
                        try
                        {

                            for (int i = 5; i < dt.Rows.Count; i++)
                            {
                                string NDCH = dt.Rows[i][1].ToString().Trim();
                                string DA_A = dt.Rows[i][2].ToString().Trim();
                                string DA_B = dt.Rows[i][3].ToString().Trim();
                                string DA_C = dt.Rows[i][4].ToString().Trim();
                                string DA_D = dt.Rows[i][5].ToString().Trim();
                                string DA_DUNG = dt.Rows[i][6].ToString().Trim();

                                int DA = CheckDA(DA_DUNG);

                                if (NDCH != "" && DA != 0)
                                {
                                    var a = db.QT_CauHoiQT_insert(NDCH, DA_A, DA_B, DA_C, DA_D, DA, _DO.IDQTHD);
                                    co++;
                                }

                            }

                            msg = msg + "\n - Import được" + co + " Câu hỏi";
                        }
                        catch (Exception ex)
                        {
                            //TempData["msgError"] = "<script>alert('Kiểm tra lại dòng : " + co + "');</script>";
                        }
                    }
                }
                //db.VitriKNL_update(_DO.IDVT, _DO.TenViTri, _DO.MaViTri, _DO.IDPB, _DO.IDKhoi, _DO.IDPX, _DO.IDNhom, _DO.IDTo, _DO.FilePath);

                TempData["msgSuccess"] = "<script>alert('Cập nhật thành công');</script>";
            }
            catch (Exception e)
            {

                TempData["msgSuccess"] = "<script>alert('Cập nhập thất bại " + e.Message + " ');</script>";
            }

            return RedirectToAction("Index", "NoiDungQT");
        }

        public ActionResult Edit(int id)
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            if (!ListQuyen.Contains(CONSTKEY.EDIT))
            {
                TempData["msgError"] = "<script>alert('Bạn không có quyền thực hiện chức năng này');</script>";
                return RedirectToAction("", "Home");
            }
            var res = (from a in db.QT_NoiDungQT.Where(x=>x.IDQTHD == id)
                       join d in db.PhongBans
                       on a.IDPhongBan equals d.IDPhongBan
                       join e in db.LinhVucDTs
                       on a.IDLVDT equals e.IDLVDT into ul
                       from e in ul.DefaultIfEmpty()
                       select new NoiDungQTView
                       {
                           IDQTHD = a.IDQTHD,
                           IDLVDT = a.IDLVDT,
                           TenLVDT = e.TenLVDT,
                           IDPhongBan = a.IDPhongBan,
                           TenPhongBan = d.TenPhongBan,
                           DiemChuan = a.DiemChuan,
                           IDLoaiQTHD = a.IDLoaiQTHD,
                           TenLoaiQTHD = a.QT_LoaiQT.TenLoai,
                           LanCapNhat = a.LanCapNhat,
                           MaHieu = a.MaHieu,
                           NgayHieuLuc = a.NgayHieuLuc ?? DateTime.Now,
                           NgayHetHieuLuc = a.NgayHetHieuLuc ?? default,
                           NoiDungCapNhat = a.NoiDungCapNhat,
                           TenQTHD = a.TenQTHD,
                           TinhTrang = a.TinhTrang,
                           isActive = a.TinhTrang ==0?false:true,
                           SL_CauHoi = db.QT_CauHoiQT.Where(x=>x.QTHDID ==a.IDQTHD).Count(),
                           List_FileQT = db.QT_FileQT.Where(x => x.QTHDID == a.IDQTHD).OrderBy(x => x.OrderByID).ToList(),
                           List_VanBanLQ = (from a in db.QT_VanBanLQ.Where(x => x.QTHDID == a.IDQTHD)
                                            join d in db.QT_NoiDungQT
                                            on a.IDQT_LienQuan equals d.IDQTHD
                                            select new NoiDungQTLienQuan
                                            {
                                                IDQTHD = d.IDQTHD,
                                                TenQTHD = d.TenQTHD,
                                            }).OrderBy(x => x.IDQTHD).ToList(),
                           LanCapNhatOld =a.LanCapNhat
                       }).ToList();
            NoiDungQTView DO = new NoiDungQTView();

            if (res.Count > 0)
            {
                foreach (var c in res)
                {
                    DO.IDQTHD = c.IDQTHD;
                    DO.IDLVDT = c.IDLVDT;
                    DO.TenLVDT = c.TenLVDT;
                    DO.TenPhongBan = c.TenPhongBan;
                    DO.MaHieu = c.MaHieu;
                    DO.IDLoaiQTHD = c.IDLoaiQTHD;
                    DO.IDPhongBan = c.IDPhongBan;
                    DO.TenQTHD = c.TenQTHD;
                    DO.isActive =c.isActive;
                    DO.NgayHieuLuc = c.NgayHieuLuc;
                    DO.NgayHetHieuLuc = c.NgayHetHieuLuc;
                    DO.LanCapNhat =c.LanCapNhat;
                    DO.NoiDungCapNhat = c.NoiDungCapNhat;
                    DO.DiemChuan = c.DiemChuan;
                    DO.List_FileQT = c.List_FileQT; 
                    DO.SL_VTCVLQ = c.SL_VTCVLQ;
                    DO.List_VanBanLQ = c.List_VanBanLQ;
                    DO.SelectedQT = c.SelectedQT;
                    DO.SL_CauHoi =c.SL_CauHoi;
                    DO.LanCapNhatOld = c.LanCapNhatOld;
                }

                db.Configuration.ProxyCreationEnabled = false;
                List<PhongBan> dt = db.PhongBans.ToList();
                ViewBag.PhongBanID = new SelectList(dt, "IDPhongBan", "TenPhongBan", DO.IDPhongBan);

                List<LinhVucDT> lv = db.LinhVucDTs.ToList();
                ViewBag.LVDTID = new SelectList(lv, "IDLVDT", "TenLVDT",DO.IDLVDT);

                List<QT_LoaiQT> loaiQT = db.QT_LoaiQT.ToList();
                ViewBag.IDLoaiQTHD = new SelectList(loaiQT, "IDLoai", "TenLoai", DO.IDLoaiQTHD);

                var NDQT = db.QT_NoiDungQT.Where(x => x.IDQTHD != id).ToList();
                List <NoiDungQTView> noidungQT = NDQT.Where(x=> x.NgayHieuLuc < DateTime.Now && (x.NgayHetHieuLuc == null || x.NgayHetHieuLuc == default || x.NgayHetHieuLuc > DateTime.Now)).Select(x => new NoiDungQTView { IDQTHD = x.IDQTHD, TenQTHD = x.MaHieu + " - " + x.TenQTHD }).ToList();
                ViewBag.SelectedQT = new SelectList(noidungQT, "IDQTHD", "TenQTHD");

                var aa = db.QT_VanBanLQ.Where(x => x.QTHDID == DO.IDQTHD).ToList();
                List<NoiDungQTView> qtvb = (from a in aa
                                          join b in NDQT on a.IDQT_LienQuan equals b.IDQTHD 
                                          select new NoiDungQTView { IDQTHD = a.IDVB, TenQTHD = b.MaHieu + " - " + b.TenQTHD }).ToList();
                ViewBag.Selec = new SelectList(qtvb, "IDQTHD", "TenQTHD");

                //ViewBag.NgayHL = DO.NgayHieuLuc.ToString("yyyy-MM-dd");
                //ViewBag.NgayHHL = DO.NgayHetHieuLuc.ToString("yyyy-MM-dd");

                //List<KNL_PhanXuong> px = db.KNL_PhanXuong.Where(x => x.IDPhongBan == DO.IDPB).ToList();
                //ViewBag.IDPX = new SelectList(px, "ID", "TenPX", DO.IDPX);
                //List<KNL_Nhom> nhom = db.KNL_Nhom.Where(x => x.IDPhongBan == DO.IDPB).ToList();
                //ViewBag.IDNhom = new SelectList(nhom, "IDNhom", "TenNhom", DO.IDNhom);
                //List<KNL_Khoi> khoi = db.KNL_Khoi.ToList();
                //ViewBag.IDKhoi = new SelectList(khoi, "ID", "TenKhoi", DO.IDKhoi);
                //List<KNL_To> to = db.KNL_To.Where(x => x.IDPhongBan == DO.IDPB).ToList();
                //ViewBag.IDTo = new SelectList(to, "IDTo", "TenTo", DO.IDTo);

            }
            else
            {
                return HttpNotFound();
            }
            return PartialView(DO);
        }
        [HttpPost]
        public ActionResult Edit( IEnumerable<HttpPostedFileBase> files, NoiDungQTView _DO, FormCollection collection)
        {
            try
            {
                string msg = "";
                int dem = 0;
                //Cap nhat Noi Dung QT
                if(_DO != null)
                {
                    DateTime? dt = null;
                    if (_DO.isActive == true) _DO.TinhTrang = 1; else _DO.TinhTrang = 0;
                    if (_DO.NgayHetHieuLuc != null && _DO.NgayHetHieuLuc != default) dt= _DO.NgayHetHieuLuc;
                    //Cập nhật QT delete BaiKiemTra 
                    if (_DO.LanCapNhat != _DO.LanCapNhatOld)
                    {
                       
                        var ls = db.QT_BaiKiemTra.Where(x => x.QTHDID == _DO.IDQTHD).ToList();
                        if (ls.Count > 0)
                        {
                            foreach(var ss in ls)
                            {
                                db.QT_CTBaiKiemTra_delete(ss.IDKT);
                            }
                        }
                        db.QT_BaiKiemTra_deleteQTID(_DO.IDQTHD);
                    }
                    var a = db.QT_NoiDungQT_update(_DO.IDQTHD, _DO.MaHieu, _DO.TenQTHD, _DO.IDLoaiQTHD, _DO.IDPhongBan, _DO.IDLVDT, _DO.NgayHieuLuc, dt, _DO.DiemChuan, _DO.LanCapNhat, _DO.NoiDungCapNhat, DateTime.Now, _DO.TinhTrang);
                }
                //update file
                var listFile = db.QT_FileQT.Where(x => x.QTHDID == _DO.IDQTHD).ToList();
                if (listFile != null)
                {
                    foreach (var item in listFile)
                    {
                        int? OrderbyID = ToNullableInt(collection["t_" + item.IDFile]);
                        var a = db.QT_FileQT_UpdateOrderBy(item.IDFile, OrderbyID);
                    }
                }
                //upload file QT
                if (files != null)
                {
                    string path = Server.MapPath("~/FileQTHD/");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    int coun = 0;
                    foreach (var file in files)
                    {
                        if(file != null)
                        {
                            //Use Namespace called :  System.IO  
                            string FileName = file.FileName;
                            string FileNameSave = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + Path.GetFileNameWithoutExtension(FileName);
                            //To Get File Extension  
                            string FileExtension = file != null ? Path.GetExtension(FileName) : "";
                            //Add Current Date To Attached File Name  
                            if (FileExtension != ".pdf")
                            {
                                //TempData["msgError"] = "<script>alert('Vui lòng chọn đúng định dạng file PDF');</script>";
                                //return View();
                            }
                            else
                            {
                                string filePathSave = "";
                                int? OrderbyID = 0;
                                if (file != null)
                                {
                                    FileNameSave = FileNameSave.Trim() + FileExtension;
                                    file.SaveAs(path + FileNameSave);
                                    filePathSave = "~/FileQTHD/" + FileNameSave;
                                    OrderbyID = ToNullableInt(collection["tt_" + coun]);

                                }
                                var a = db.QT_FileQT_insert(FileName,_DO.IDQTHD,filePathSave,OrderbyID);
                                //TempData["msgSuccess"] = "<script>alert('Thêm mới thành công');</script>";
                                dem++;
                            }
                        }
                        coun++;
                    }
                }
                msg = "Thêm mới được" + dem + "QT/HD";
                // VB QT liên quan
                if (_DO.SelectedQT != null)
                {
                    for (int i = 0; i < _DO.SelectedQT.Length; i++)
                    {
                        int selc = _DO.SelectedQT[i];
                        var checkVBLQ = db.QT_VanBanLQ.Where(x=>x.QTHDID == _DO.IDQTHD && x.IDQT_LienQuan == selc).ToList();   
                        if(checkVBLQ.Count == 0)
                        {
                            var a = db.QT_QT_VanBanLQ_insert(_DO.IDQTHD, _DO.SelectedQT[i]);
                        }
                    }
                }
                // Upload câu hỏi QT
                if (Request != null)
                {
                    string filePath = string.Empty;
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
                            //TempData["msg"] = "<script>alert('Vui lòng chọn đúng định dạng file Excel');</script>";
                            return View();
                        }
                        DataSet result = reader.AsDataSet();
                        DataTable dt = result.Tables[0];
                        reader.Close();
                        int co = 0;
                        try
                        {

                            for (int i = 5; i < dt.Rows.Count; i++)
                            {
                                string NDCH = dt.Rows[i][1].ToString().Trim();
                                string DA_A = dt.Rows[i][2].ToString().Trim();
                                string DA_B = dt.Rows[i][3].ToString().Trim();
                                string DA_C = dt.Rows[i][4].ToString().Trim();
                                string DA_D = dt.Rows[i][5].ToString().Trim();
                                string DA_DUNG = dt.Rows[i][6].ToString().Trim();

                                int DA = CheckDA(DA_DUNG);

                                if (NDCH != "" && DA != 0)
                                {
                                    var a = db.QT_CauHoiQT_insert(NDCH, DA_A,DA_B,DA_C,DA_D,DA,_DO.IDQTHD);
                                    co++;
                                }

                            }

                            msg = msg + "\n - Import được" + co + " Câu hỏi";
                        }
                        catch (Exception ex)
                        {
                            //TempData["msgError"] = "<script>alert('Kiểm tra lại dòng : " + co + "');</script>";
                        }
                    }
                }
                //db.VitriKNL_update(_DO.IDVT, _DO.TenViTri, _DO.MaViTri, _DO.IDPB, _DO.IDKhoi, _DO.IDPX, _DO.IDNhom, _DO.IDTo, _DO.FilePath);

                TempData["msgSuccess"] = "<script>alert('Cập nhật thành công');</script>";
            }
            catch (Exception e)
            {

                TempData["msgSuccess"] = "<script>alert('Cập nhập thất bại " + e.Message + " ');</script>";
            }

            return RedirectToAction("Index", "NoiDungQT");
        }

        public ActionResult EditFileQT(int id)
        {
            //int idpb = MyAuthentication.IDPhongban;
            //var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            //if (!ListQuyen.Contains(CONSTKEY.ED_KNL))
            //{
            //    TempData["msgError"] = "<script>alert('Bạn không có quyền thực hiện chức năng này');</script>";
            //    return RedirectToAction("", "Home");
            //}
            //db.Configuration.ProxyCreationEnabled = false;
            var qt = db.QT_NoiDungQT.Where(x => x.IDQTHD == id).FirstOrDefault();
            ViewBag.NoiDungQT =qt;
            var res = db.QT_FileQT.Where(x => x.QTHDID == id).ToList();

            NoiDungQTView a = new NoiDungQTView();
            a.IDQTHD = qt.IDQTHD;
            a.List_FileQT = res;
           

            return PartialView(a);
        }
        [HttpPost]
        public ActionResult EditFileQT(IEnumerable<HttpPostedFileBase> files, NoiDungQTView _DO, FormCollection collection)
        {
            try
            {

                //update file
                var listFile = db.QT_FileQT.Where(x => x.QTHDID == _DO.IDQTHD).ToList();
                if (listFile != null)
                {
                    foreach(var item in listFile)
                    {
                       int? OrderbyID = ToNullableInt(collection["t_" + item.IDFile]);
                       var a = db.QT_FileQT_UpdateOrderBy(item.IDFile, OrderbyID);
                    }
                }

                string msg = "";
                int dem = 0;
                //upload file QT
                if (files != null)
                {
                    string path = Server.MapPath("~/FileQTHD/");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    int coun = 0;
                    foreach (var file in files)
                    {
                        if (file != null)
                        {
                            //Use Namespace called :  System.IO  
                            string FileName = file.FileName;
                            string FileNameSave = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + Path.GetFileNameWithoutExtension(FileName);
                            //To Get File Extension  
                            string FileExtension = file != null ? Path.GetExtension(FileName) : "";
                            //Add Current Date To Attached File Name  
                            if (FileExtension != ".pdf")
                            {
                                //TempData["msgError"] = "<script>alert('Vui lòng chọn đúng định dạng file PDF');</script>";
                                //return View();
                            }
                            else
                            {
                                string filePathSave = "";
                                int? OrderbyID = 0;
                                if (file != null)
                                {
                                    FileNameSave = FileNameSave.Trim() + FileExtension;
                                    file.SaveAs(path + FileNameSave);
                                    filePathSave = "~/FileQTHD/" + FileNameSave;
                                    OrderbyID = ToNullableInt(collection["tt_" + coun]);

                                }
                                var a = db.QT_FileQT_insert(FileName, _DO.IDQTHD, filePathSave, OrderbyID);
                                //TempData["msgSuccess"] = "<script>alert('Thêm mới thành công');</script>";
                                dem++;
                            }
                        }
                        coun++;
                    }
                }
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Có lỗi khi thêm mới: " + e.Message + "');</script>";
            }
            //return View();
            return RedirectToAction("Index", "NoiDungQT");
        }

        public JsonResult DeleteFileQT(int id)
        {
            try
            {
                var aa = db.QT_FileQT.Where(x => x.IDFile == id).FirstOrDefault();
                if (aa != null)
                {
                    //Directory.Delete(aa.FilePDF, true);
                    db.QT_FileQT_delete(id);
                }
                //var listFile = db.QT_FileQT.Where(x => x.QTHDID == idqt).ToList();
                //return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                TempData["msgSuccess"] = "<script>alert('Xóa dữ liệu thất bại: " + e.Message + "');</script>";
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public int CountPhanQuyen(int? IDVT)
        {
            var a = db.QT_PhanQuyen.Where(x => x.IDVTKNL ==IDVT).ToList();
            return a.Count();
        }

        public JsonResult DeletePhanQuyenQT(int id)
        {
            try
            {
                var aa = db.QT_PhanQuyen.Where(x => x.IDPhanQuyen == id).FirstOrDefault();
                if (aa != null)
                {
                    //Directory.Delete(aa.FilePDF, true);
                    db.QT_PhanQuyen_delete(id);
                }
                //var listFile = db.QT_FileQT.Where(x => x.QTHDID == idqt).ToList();
                //return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                TempData["msgSuccess"] = "<script>alert('Xóa dữ liệu thất bại: " + e.Message + "');</script>";
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteFileVB(int id)
        {
            try
            {
                var aa = db.QT_VanBanLQ.Where(x => x.IDVB == id).FirstOrDefault();
                if (aa != null)
                {
                    //Directory.Delete(aa.FilePDF, true);
                    db.QT_QT_VanBanLQ_delete(id);
                }
                //var listFile = db.QT_FileQT.Where(x => x.QTHDID == idqt).ToList();
                //return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                TempData["msgSuccess"] = "<script>alert('Xóa dữ liệu thất bại: " + e.Message + "');</script>";
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ImportExcel()
        {
            db.Configuration.ProxyCreationEnabled = false;
            //List<NoiDungDT> dt = db.NoiDungDTs.ToList();
            //ViewBag.IDND = new SelectList(dt, "IDND", "NoiDung");

            //List<DanhSachDA> ds = db_context.DanhSachDAs.ToList();
            //ViewBag.IDDAĐung = new SelectList(ds, "IDDSĐA", "TenĐA");

            //List<CTLVDT> lvct = db_context.CTLVDTs.ToList();
            //ViewBag.IDCTLVDT = new SelectList(lvct, "IDCTLVDT", "TenCTLVDT");

            return PartialView();
        }
        [HttpPost]
        public ActionResult ImportExcel(FCheckValidation _DO)
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
                    int co = 0;
                    try
                    {
                        
                        for (int i = 1; i < dt.Rows.Count; i++)
                        {
                            string MaHieu = dt.Rows[i][1].ToString().Trim();
                            string TenTaiLieu = dt.Rows[i][2].ToString().Trim();
                            string TenLoai = dt.Rows[i][3].ToString().Trim();
                            string TenLVDT = dt.Rows[i][4].ToString().Trim();
                            string TenBP = dt.Rows[i][5].ToString().Trim();
                            string TenPhoBien = dt.Rows[i][6].ToString().Trim();
                            int? IDLoai = db.QT_LoaiQT.Where(x=>x.TenLoai == TenLoai).FirstOrDefault().IDLoai;
                            int? IDLV = GetLVDT(TenLVDT);
                            int? IDBP = db.PhongBans.Where(x => x.TenPhongBan == TenBP).FirstOrDefault().IDPhongBan;

                            int? IDTT = 0;
                            if(TenPhoBien == "Phổ biến") { IDTT = 1; }

                            string Ngayhieuluc = dt.Rows[i][7].ToString().Trim();
                            var d = DateTime.ParseExact(Ngayhieuluc, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                            if (IDBP != null && IDLoai != null && IDLV != 0 && TenTaiLieu !=""&& MaHieu != "")
                            {
                                var checkMH = db.QT_NoiDungQT.Where(x=>x.MaHieu == MaHieu).FirstOrDefault();
                                if(checkMH == null)
                                {
                                    var a = db.QT_NoiDungQT_insertExcel(MaHieu, TenTaiLieu, IDLoai, IDBP, IDLV, 7, DateTime.Now, IDTT,d);
                                    co++;
                                }
                                
                            }

                        }

                        TempData["msgSuccess"] = "<script>alert('Import thành công: " + co + " dòng dữ liệu');</script>";
                    }
                    catch (Exception ex)
                    {
                        TempData["msgError"] = "<script>alert('Kiểm tra lại dòng : " + co + "');</script>";
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

            return RedirectToAction("Index", "NoiDungQT");
        }

        public FileResult DownloadExcel()
        {
            string path = "/App_Data/BM_NoiDungQT.xlsx";
            return File(path, "application/vnd.ms-excel", "BM_NoiDungQT.xlsx");
        }


        public int GetLVDT(string TenLV)
        {
            var model = db.LinhVucDTs.Where(x => x.TenLVDT.Contains(TenLV)).SingleOrDefault();
            if (model == null)
                return 0;
            return model.IDLVDT;
        }

        public ActionResult VanBanLQView(int id)
        {
            //var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            //if (!ListQuyen.Contains(CONSTKEY.EDIT))
            //{
            //    TempData["msgError"] = "<script>alert('Bạn không có quyền thực hiện chức năng này');</script>";
            //    return RedirectToAction("", "Home");
            //}
            var res = (from a in db.QT_NoiDungQT.Where(x => x.IDQTHD == id)
                       join d in db.PhongBans
                       on a.IDPhongBan equals d.IDPhongBan
                       join e in db.LinhVucDTs
                       on a.IDLVDT equals e.IDLVDT into ul
                       from e in ul.DefaultIfEmpty()
                       select new NoiDungQTView
                       {
                           IDQTHD = a.IDQTHD,
                           IDLVDT = a.IDLVDT,
                           TenLVDT = e.TenLVDT,
                           IDPhongBan = a.IDPhongBan,
                           TenPhongBan = d.TenPhongBan,
                           DiemChuan = a.DiemChuan,
                           IDLoaiQTHD = a.IDLoaiQTHD,
                           TenLoaiQTHD = a.QT_LoaiQT.TenLoai,
                           LanCapNhat = a.LanCapNhat,
                           MaHieu = a.MaHieu,
                           NgayHieuLuc = a.NgayHieuLuc ?? DateTime.Now,
                           NgayHetHieuLuc = a.NgayHetHieuLuc ?? default,
                           NoiDungCapNhat = a.NoiDungCapNhat,
                           TenQTHD = a.TenQTHD,
                           TinhTrang = a.TinhTrang,
                           isActive = a.TinhTrang == 0 ? false : true,
                           SL_CauHoi = db.QT_CauHoiQT.Where(x => x.QTHDID == a.IDQTHD).Count(),
                           List_FileQT = db.QT_FileQT.Where(x => x.QTHDID == a.IDQTHD).OrderBy(x => x.OrderByID).ToList(),
                           List_VanBanLQ = (from a in db.QT_VanBanLQ.Where(x => x.QTHDID == a.IDQTHD)
                                            join d in db.QT_NoiDungQT
                                            on a.IDQT_LienQuan equals d.IDQTHD
                                            select new NoiDungQTLienQuan
                                            {
                                                IDQTHD = d.IDQTHD,
                                                TenQTHD = d.TenQTHD,
                                                TinhTrangHL = d.NgayHieuLuc < DateTime.Now && (d.NgayHetHieuLuc == null || d.NgayHetHieuLuc == default || d.NgayHetHieuLuc > DateTime.Now) ? 1 : 0,
                                                TinhTrangQT = d.TinhTrang
                                            }).Where(x=>x.TinhTrangHL ==1 && x.TinhTrangQT ==1).OrderBy(x => x.IDQTHD).ToList(),
                       }).ToList();
            NoiDungQTView DO = new NoiDungQTView();

            if (res.Count > 0)
            {
                foreach (var c in res)
                {
                    DO.IDQTHD = c.IDQTHD;
                    DO.IDLVDT = c.IDLVDT;
                    DO.TenLVDT = c.TenLVDT;
                    DO.TenPhongBan = c.TenPhongBan;
                    DO.MaHieu = c.MaHieu;
                    DO.IDLoaiQTHD = c.IDLoaiQTHD;
                    DO.IDPhongBan = c.IDPhongBan;
                    DO.TenQTHD = c.TenQTHD;
                    DO.isActive = c.isActive;
                    DO.NgayHieuLuc = c.NgayHieuLuc;
                    DO.NgayHetHieuLuc = c.NgayHetHieuLuc;
                    DO.LanCapNhat = c.LanCapNhat;
                    DO.NoiDungCapNhat = c.NoiDungCapNhat;
                    DO.DiemChuan = c.DiemChuan;
                    DO.List_FileQT = c.List_FileQT;
                    DO.SL_VTCVLQ = c.SL_VTCVLQ;
                    DO.List_VanBanLQ = c.List_VanBanLQ;
                    DO.SelectedQT = c.SelectedQT;
                    DO.SL_CauHoi = c.SL_CauHoi;
                }

                db.Configuration.ProxyCreationEnabled = false;
                List<PhongBan> dt = db.PhongBans.ToList();
                ViewBag.PhongBanID = new SelectList(dt, "IDPhongBan", "TenPhongBan", DO.IDPhongBan);

                List<LinhVucDT> lv = db.LinhVucDTs.ToList();
                ViewBag.LVDTID = new SelectList(lv, "IDLVDT", "TenLVDT", DO.IDLVDT);

                List<QT_LoaiQT> loaiQT = db.QT_LoaiQT.ToList();
                ViewBag.IDLoai = new SelectList(loaiQT, "IDLoai", "TenLoai", DO.IDLoaiQTHD);

                List<NoiDungQTView> noidungQT = db.QT_NoiDungQT.Where(x => x.IDQTHD != id).Select(x => new NoiDungQTView { IDQTHD = x.IDQTHD, TenQTHD = x.MaHieu + " - " + x.TenQTHD }).ToList();
                ViewBag.SelectedQT = new SelectList(noidungQT, "IDQTHD", "TenQTHD");

                var aa = db.QT_VanBanLQ.Where(x => x.QTHDID == DO.IDQTHD).ToList();
                List<NoiDungQTView> qtvb = (from a in aa
                                            join b in noidungQT on a.IDQT_LienQuan equals b.IDQTHD
                                            select new NoiDungQTView { IDQTHD = a.IDVB, TenQTHD = b.MaHieu + " - " + b.TenQTHD }).ToList();
                ViewBag.Selec = new SelectList(qtvb, "IDQTHD", "TenQTHD");

                //ViewBag.NgayHL = DO.NgayHieuLuc.ToString("yyyy-MM-dd");
                //ViewBag.NgayHHL = DO.NgayHetHieuLuc.ToString("yyyy-MM-dd");

                //List<KNL_PhanXuong> px = db.KNL_PhanXuong.Where(x => x.IDPhongBan == DO.IDPB).ToList();
                //ViewBag.IDPX = new SelectList(px, "ID", "TenPX", DO.IDPX);
                //List<KNL_Nhom> nhom = db.KNL_Nhom.Where(x => x.IDPhongBan == DO.IDPB).ToList();
                //ViewBag.IDNhom = new SelectList(nhom, "IDNhom", "TenNhom", DO.IDNhom);
                //List<KNL_Khoi> khoi = db.KNL_Khoi.ToList();
                //ViewBag.IDKhoi = new SelectList(khoi, "ID", "TenKhoi", DO.IDKhoi);
                //List<KNL_To> to = db.KNL_To.Where(x => x.IDPhongBan == DO.IDPB).ToList();
                //ViewBag.IDTo = new SelectList(to, "IDTo", "TenTo", DO.IDTo);

            }
            else
            {
                return HttpNotFound();
            }
            return PartialView(DO);
        }

        public ActionResult PhanQuyenQT(int? IDVT)
        {
            //var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            //if (!ListQuyen.Contains(CONSTKEY.EDIT))
            //{
            //    TempData["msgError"] = "<script>alert('Bạn không có quyền thực hiện chức năng này');</script>";
            //    return RedirectToAction("", "Home");
            //}
            var res = (from a in db.ViTriKNLs.Where(x => x.IDVT == IDVT)
                       select new PhanQuyenView
                       {
                           IDVTKNL = a.IDVT,
                           TenVTKNL = a.TenViTri,
                           ListNDQT = (from aa in db.QT_PhanQuyen.Where(x => x.IDVTKNL == a.IDVT)
                                            join d in db.QT_NoiDungQT
                                            on aa.QTHDID equals d.IDQTHD
                                            select new NoiDungPhanQuyenView
                                            {
                                                IDQTHD = d.IDQTHD,
                                                TenQTHD = d.MaHieu+"-"+d.TenQTHD,
                                                IDDinhKy = aa.DKID,
                                                TenDinhKy =aa.QT_DinhKy.TenDinhKy,
                                                IDPhanQuyen =aa.IDPhanQuyen,
                                                IDVTKNL =aa.IDVTKNL,
                                                MaHieu = d.MaHieu,
                                                TinhTrangQT = d.TinhTrang,
                                                TinhTrangHL = d.NgayHieuLuc < DateTime.Now && (d.NgayHetHieuLuc == null || d.NgayHetHieuLuc == default || d.NgayHetHieuLuc > DateTime.Now) ? 1 : 0,
                                            }).ToList(),
                       }).ToList();
            PhanQuyenView DO = new PhanQuyenView();

            if (res.Count > 0)
            {
                foreach (var c in res)
                {
                   DO.IDVTKNL = c.IDVTKNL;
                   DO.TenVTKNL = c.TenVTKNL;
                   DO.ListNDQT = c.ListNDQT;
                }

                List<NoiDungQTView> noidungQT = db.QT_NoiDungQT.Where(x=>x.TinhTrang ==1 && x.NgayHieuLuc < DateTime.Now && (x.NgayHetHieuLuc == null || x.NgayHetHieuLuc == default || x.NgayHetHieuLuc > DateTime.Now)).Select(x => new NoiDungQTView { IDQTHD = x.IDQTHD, TenQTHD = x.MaHieu + " - " + x.TenQTHD }).ToList();
                ViewBag.SelectedQT = new SelectList(noidungQT, "IDQTHD", "TenQTHD");

                //db.Configuration.ProxyCreationEnabled = false;
                //List<PhongBan> dt = db.PhongBans.ToList();
                //ViewBag.PhongBanID = new SelectList(dt, "IDPhongBan", "TenPhongBan", DO.IDPhongBan);

                List<QT_DinhKy> lv = db.QT_DinhKy.ToList();
                ViewBag.IDDinhKy = new SelectList(lv, "IDDK", "TenDinhKy");


            }
            else
            {
                return HttpNotFound();
            }
            return PartialView(DO);
        }

        [HttpPost]
        public ActionResult PhanQuyenQT(PhanQuyenView _DO, FormCollection collection)
        {
            var vt = db.ViTriKNLs.Where(x => x.IDVT == _DO.IDVTKNL).FirstOrDefault();
            try
            {
                //update QT
                var listFile = db.QT_PhanQuyen.Where(x => x.IDVTKNL == _DO.IDVTKNL).ToList();
                if (listFile != null)
                {
                    foreach (var item in listFile)
                    {
                        int? IDDK = ToNullableInt(collection["qt_" + item.QTHDID]);
                        if (IDDK == null) { IDDK = 0; }
                        var a = db.QT_PhanQuyen_update(item.IDPhanQuyen,item.QTHDID,item.IDVTKNL,IDDK);
                    }
                }

                // Danh sach QT
                foreach (var key in collection.AllKeys)
                {
                    if (key.Split('_')[0] == "QuyTrinh")
                    {
                        int? IDDK = ToNullableInt(collection["DinhKy_" + key.Split('_')[1]]);
                        int? IDQTHD = ToNullableInt(collection[key]);
                        var checkq = db.QT_PhanQuyen.Where(x => x.IDVTKNL == _DO.IDVTKNL && x.QTHDID == IDQTHD).ToList();
                        if(checkq.Count() == 0 && IDQTHD != null && IDDK != null)
                        {
                            var aa = db.QT_PhanQuyen_insert(IDQTHD, _DO.IDVTKNL, IDDK);
                        }


                    }
                }

                TempData["msgSuccess"] = "<script>alert('Thêm mới thành công');</script>";
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Có lỗi khi thêm mới: " + e.Message + "');</script>";
            }
            //return View();
            return RedirectToAction("Index", "FPosition", new { IDPB = vt.IDPB, IDKhoi = vt.IDKhoi, IDPX = vt.IDPX, IDTo = vt.IDTo, IDNhom = vt.IDNhom });
        }


        public ActionResult EditVTCV(int? IDQTHD)
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            if (!ListQuyen.Contains(CONSTKEY.P_QTHD))
            {
                TempData["msgError"] = "<script>alert('Bạn không có quyền thực hiện chức năng này');</script>";
                return RedirectToAction("", "Home");
            }


            var listND = db.QT_NoiDungQT.ToList();
            var listPQ = db.QT_PhanQuyen.ToList();
            var listVT = db.VitriKNL_search();
            var res = (from a in listND.Where(x => x.IDQTHD == IDQTHD)
                       select new PhanQuyenView
                       {
                           QTHDID = a.IDQTHD,
                           TenQTHD = a.MaHieu+"-"+a.TenQTHD,
                           ListVTKNL = (from aa in listPQ.Where(x => x.QTHDID == a.IDQTHD)
                                        join d in listVT
                                        on aa.IDVTKNL equals d.IDVT
                                        select new ViTriPhanQuyenView
                                       {
                                           IDVT = d.IDVT,
                                           TenViTri = d.TenViTri + "-" + d.TenTo + "-" + d.TenNhom + "-" + d.TenPX + d.TenPhongBan,
                                           MaViTri = d.MaViTri,
                                           IDDinhKy =aa?.DKID ?? 0,
                                           TenDinhKy =aa?.QT_DinhKy?.TenDinhKy??"",
                                           IDPhanQuyen =aa.IDPhanQuyen,
                                           MaHieu =a.MaHieu,
                                           QTHDID =a.IDQTHD,
                                           TenQTHD = a.MaHieu + "-" + a.TenQTHD,
                                       }).ToList(),
                       }).ToList();
            PhanQuyenView DO = new PhanQuyenView();

            if (res.Count > 0)
            {
                foreach (var c in res)
                {
                    DO.QTHDID = c.QTHDID;
                    DO.TenQTHD = c.TenQTHD;
                    DO.ListVTKNL = c.ListVTKNL;
                }


                //var vt = (from a in db.VitriKNL_search()
                //          select new ViTriKNLValidation
                //          {
                //              IDVT = a.IDVT,
                //              TenViTri = a.TenViTri + "-" + a.TenPhongBan + "-" + a.TenPX + "-" + a.TenNhom + "-" + a.TenTo,
                //              IDNhom = a.IDNhom,
                //              IDTo = a.IDTo,
                //              MaViTri = a.MaViTri,
                //              IDPX = a.IDPX,
                //              IDVTParent = a.IDVTParent
                //          }).ToList();

                //ViewBag.SelectedVT = new SelectList(vt, "IDVT", "TenViTri");

                //List<ViTriKNLValidation> noidungQT = db.ViTriKNLs.Select(x => new ViTriKNLValidation { IDVT = x.IDVT, TenViTri = x.MaViTri + " - " + x.TenViTri }).ToList();
                //ViewBag.SelectedQT = new SelectList(noidungQT, "IDQTHD", "TenQTHD");

                List<QT_DinhKy> lv = db.QT_DinhKy.ToList();
                ViewBag.IDDinhKy = new SelectList(lv, "IDDK", "TenDinhKy");

                db.Configuration.ProxyCreationEnabled = false;

                
                List<PhongBan> dt = db.PhongBans.ToList();
                if (ListQuyen.Contains(CONSTKEY.V_BP))
                {
                    dt = dt.Where(x => x.IDPhongBan == MyAuthentication.IDPhongban).ToList();
                }
                ViewBag.IDPB = new SelectList(dt, "IDPhongBan", "TenPhongBan");

                //List<KNL_PhanXuong> px = db.KNL_PhanXuong.ToList();
                //ViewBag.IDPX = new SelectList(px, "ID", "TenPX");

                //List<KNL_Nhom> nhom = db.KNL_Nhom.ToList();
                //ViewBag.IDNhom = new SelectList(nhom, "IDNhom", "TenNhom");

                //List<KNL_To> to = db.KNL_To.ToList();
                //ViewBag.IDTo = new SelectList(to, "IDTo", "TenTo");

                List<KNL_Khoi> khoi = db.KNL_Khoi.ToList();
                ViewBag.IDKhoi = new SelectList(khoi, "ID", "TenKhoi");

            }
            else
            {
                return HttpNotFound();
            }
            return PartialView(DO);
        }

        [HttpPost]
        public ActionResult EditVTCV(PhanQuyenView _DO, FormCollection collection)
        {
            //var vt = db.ViTriKNLs.Where(x => x.IDVT == _DO.IDVTKNL).FirstOrDefault();
            try
            {
                //update QT
                var listFile = db.QT_PhanQuyen.Where(x => x.QTHDID == _DO.QTHDID).ToList();
                if (listFile != null)
                {
                    foreach (var item in listFile)
                    {
                        int? IDDK = ToNullableInt(collection["qt_" + item.IDVTKNL]);
                        if(IDDK == null) { IDDK = 0; }
                        var a = db.QT_PhanQuyen_update(item.IDPhanQuyen, item.QTHDID, item.IDVTKNL, IDDK);
                    }
                }

                // Danh sach QT
                foreach (var key in collection.AllKeys)
                {
                    if (key.Split('_')[0] == "QuyTrinh")
                    {
                        int? IDDK = ToNullableInt(collection["DinhKy_" + key.Split('_')[1]]);
                        int? IDVT = ToNullableInt(collection[key]);
                        var checkq = db.QT_PhanQuyen.Where(x => x.IDVTKNL == IDVT && x.QTHDID == _DO.QTHDID).ToList();
                        if (checkq.Count() == 0 && IDVT != null && IDDK != null)
                        {
                            var aa = db.QT_PhanQuyen_insert(_DO.QTHDID, IDVT, IDDK);
                        }


                    }
                }

                TempData["msgSuccess"] = "<script>alert('Cập nhật thành công');</script>";
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Có lỗi khi thêm mới: " + e.Message + "');</script>";
            }
            //return View();
            return RedirectToAction("Index", "NoiDungQT");
        }


        public int? ToNullableInt(string s)
        {
            int i;
            if (int.TryParse(s, out i)) return i;
            return null;
        }

        public int CheckDA(string s)
        {
           int re =0;
           if(s =="1"||s=="A"||s=="a") re = 1;
           else if(s=="2"||s=="B"||s=="b") re= 2;
           else if (s == "3" || s == "C" || s == "c") re= 3;
           else if (s == "4" || s == "D" || s == "d") re= 4;
           return re;

        }

        public List<NoiDungQTLienQuan> checkListNDQT(int? IDQT)
        { 
                 var res = (from a in db.QT_VanBanLQ.Where(x=>x.QTHDID ==IDQT)
                            join d in db.QT_NoiDungQT
                            on a.IDQT_LienQuan equals d.IDQTHD
                            select new NoiDungQTLienQuan
                            {
                                IDQTHD = d.IDQTHD,
                                TenQTHD = d.TenQTHD,
                            }).OrderBy(x => x.IDQTHD).ToList();
            return res;
        }

        public ActionResult ExportData()
        {
            try
            {
                string fileNamemau = AppDomain.CurrentDomain.BaseDirectory + @"App_Data\DS_NoiDungQTHD.xlsx";
                string fileNamemaunew = AppDomain.CurrentDomain.BaseDirectory + @"App_Data\DS_NoiDungQTHD_Temp.xlsx";
                XLWorkbook Workbook = new XLWorkbook(fileNamemau);
                IXLWorksheet Worksheet = Workbook.Worksheet("NDQTHD");
                List<NoiDungQTView> DataKNL = ListNDDT();
                int row = 2;
                if (DataKNL.Count > 0)
                {
                    foreach (var data in DataKNL)
                    {
                        Worksheet.Cell(row, "A").Value = row - 1;
                        Worksheet.Cell(row, "A").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        Worksheet.Cell(row, "A").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "A").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "B").Value = data.MaHieu;
                        Worksheet.Cell(row, "B").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "B").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "B").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "C").Value = data.TenQTHD;
                        Worksheet.Cell(row, "C").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "C").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "C").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "D").Value = data.TenLoaiQTHD;
                        Worksheet.Cell(row, "D").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "D").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "D").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "E").Value = data.TenLVDT;
                        Worksheet.Cell(row, "E").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "E").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "E").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "F").Value = data.TenPhongBan;
                        Worksheet.Cell(row, "F").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "F").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "F").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;


                        Worksheet.Cell(row, "G").Value = data.TinhTrang ==1?"Phổ biến":"Chưa phổ biến";
                        Worksheet.Cell(row, "G").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        Worksheet.Cell(row, "G").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "G").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        //Worksheet.Cell(row, "F").Style.Fill.BackgroundColor = XLColor.Yellow;

                        Worksheet.Cell(row, "H").Value = data.NgayHieuLuc;
                        Worksheet.Cell(row, "H").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        Worksheet.Cell(row, "H").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "H").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        Worksheet.Cell(row, "H").Style.DateFormat.Format = "dd/MM/yyyy";

                        Worksheet.Cell(row, "I").Value = data.SL_CauHoi;
                        Worksheet.Cell(row, "I").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        Worksheet.Cell(row, "I").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "I").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "J").Value = data.TinhTrangHL ==1?"Còn hiệu lực":data.TinhTrangHL ==2?"Chưa hiệu lực":"Hết hiệu lực";
                        Worksheet.Cell(row, "J").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        Worksheet.Cell(row, "J").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "J").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        row = row + 1;
                    }

                    Workbook.SaveAs(fileNamemaunew);
                    byte[] fileBytes = System.IO.File.ReadAllBytes(fileNamemaunew);
                    string fileName = "ThongKe_DS_NoiDungQTHD - " + DateTime.Now.Date.ToString("dd/MM/yyyy") + ".xlsx";
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
                }
                else
                {

                    //Worksheet.Cell("A1").Value = "Ngày xuất file: " + DateTime.Now.Date.ToString("dd/MM/yyyy");
                    Workbook.SaveAs(fileNamemaunew);
                    byte[] fileBytes = System.IO.File.ReadAllBytes(fileNamemaunew);
                    string fileName = "ThongKe_DS_NoiDungQTHD - " + DateTime.Now.Date.ToString("dd/MM/yyyy") + ".xlsx";
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
                }

            }
            catch (Exception ex)
            {
                TempData["msg"] = "<script>alert('" + ex + "');window.location.href = '/Everyday/'</script>";
                return View(TempData);
            }
        }
        public List<NoiDungQTView> ListNDDT()
        {
            //var phongban = db.PhongBans.ToList();
            //var lvdt = db.LinhVucDTs.ToList();

            var res = (from a in db.QT_NoiDungQT
                       join d in db.PhongBans
                       on a.IDPhongBan equals d.IDPhongBan
                       join e in db.LinhVucDTs
                       on a.IDLVDT equals e.IDLVDT into ul
                       from e in ul.DefaultIfEmpty()
                       select new NoiDungQTView
                       {
                           IDQTHD = a.IDQTHD,
                           IDLVDT = a.IDLVDT,
                           TenLVDT = e.TenLVDT,
                           IDPhongBan = a.IDPhongBan,
                           TenPhongBan = d.TenPhongBan,
                           DiemChuan = a.DiemChuan,
                           IDLoaiQTHD = a.IDLoaiQTHD,
                           TenLoaiQTHD = a.QT_LoaiQT.TenLoai,
                           LanCapNhat = a.LanCapNhat,
                           MaHieu = a.MaHieu,
                           NgayHieuLuc = a.NgayHieuLuc ?? default,
                           NgayHetHieuLuc = a.NgayHetHieuLuc ?? default,
                           NoiDungCapNhat = a.NoiDungCapNhat,
                           TenQTHD = a.TenQTHD,
                           TinhTrang = a.TinhTrang,
                           TinhTrangHL = a.NgayHieuLuc < DateTime.Now && (a.NgayHetHieuLuc == null || a.NgayHetHieuLuc == default || a.NgayHetHieuLuc > DateTime.Now) ? 1 : a.NgayHieuLuc > DateTime.Now?2:0,
                           //List_FileQT = db.QT_FileQT.Where(x => x.QTHDID == a.IDQTHD).OrderBy(x => x.OrderByID).ToList(),
                           //List_VanBanLQ = (from a in db.QT_VanBanLQ.Where(x => x.QTHDID == a.IDQTHD)
                           //                 join d in db.QT_NoiDungQT
                           //                 on a.IDQT_LienQuan equals d.IDQTHD
                           //                 select new NoiDungQTLienQuan
                           //                 {
                           //                     IDQTHD = d.IDQTHD,
                           //                     TenQTHD = d.MaHieu + "-" + d.TenQTHD,
                           //                 }).OrderBy(x => x.IDQTHD).ToList(),
                           SL_CauHoi = db.QT_CauHoiQT.Where(x => x.QTHDID == a.IDQTHD).Count(),
                           //SL_VTCVLQ = db.QT_PhanQuyen.Where(x => x.QTHDID == a.IDQTHD).Count(),
                           //SL_NVHT = db.QT_BaiKiemTra.Where(x => x.QTHDID == a.IDQTHD && x.TinhTrang == 1 && x.NgayKTTT >= DateTime.Now).Count()
                       }).OrderBy(x => x.IDQTHD).ToList();
            return res;
        }


    }
}