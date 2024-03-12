using DocumentFormat.OpenXml.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.Office2010.Excel;
using E_Learning.Models;
using E_Learning.ModelsKCCD;
using ExcelDataReader;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_Learning.Controllers.KCCD
{
    public class ConfirmKCCDController : Controller
    {
        ELEARNINGEntities db = new ELEARNINGEntities();
        ELEARNINGEntities dbKCCCD = new ELEARNINGEntities();
        int Idquyen = MyAuthentication.IDQuyen;
        String ControllerName = "ConfirmKCCD";
        // GET: ConfirmKCCD
        public ActionResult Index(int? page, int? SuggestID, int? IDLVDT)
        {
            //if (search == null) search = "";
            //ViewBag.search = search;
            //if (IDPB == null) IDPB = 0;
            //if (IDLVDT == null) IDLVDT = 0;
            ViewBag.SuggestID = SuggestID;
            //var sugg = dbKCCCD.DeNghiKCCD_select("").Where(x=>x.ID == SuggestID).FirstOrDefault();

            //var phieuxn =dbKCCCD.PhieuXacNhanKCCDs.ToList();
            //var nhanvien = db.NhanViens.Where(x=>x.IDTinhTrangLV ==1).ToList();
            //var vitri = db.Vitris.ToList();
            //var phongban = db.PhongBans.ToList();

            var sugg = new SuggestKCCDController().ListSuggest("").Where(x => x.ID == SuggestID).FirstOrDefault();
            ViewBag.IDTinhTrangPhieu = sugg.TinhTrang;
            ViewBag.IDHuongDan1 = sugg.HuongDan1;
            ViewBag.NoiDungDT = sugg.TenNDDT;

            var res = (from a in db.PhieuXacNhanKCCD_select(0,SuggestID,0)
                       //join b in db.NhanViens on a.HocVienID equals b.ID
                       //join c in vitri on b.IDViTri equals c.IDViTri
                       //join d in phongBan on a.IDPhong equals d.IDPhongBan
                       select new ConfirmKCCDView
                       {
                           ID = a.ID,
                           DeNghiDTID = a.DeNghiDTID,
                           NoiDungDT = a.TenND,
                           HocVienID = a.HocVienID,
                           HoTenHV = a.HoTen,
                           MaNV = a.MaNV,
                           VitriHV = a.TenViTri,
                           PhongBanHV = db.NhanViens.Where(x => x.ID == a.HocVienID).FirstOrDefault().PhongBan.TenPhongBan,
                           HVTruocDatDuoc =a.HVTruocDatDuoc,
                           HVTruocCanCaiThien =a.HVTruocCanCaiThien,
                           HVSauDatDuoc =a.HVSauDatDuoc,
                           HVSauCanCaiThien =a.HVSauCanCaiThien,
                           GVThucHanhSauDT = a.GVThucHanhSauDT,
                           GVLyThuyetSauDT =a.GVLyThuyetSauDT,
                           GVNhanXetLTSauDT = a.GVNhanXetLTSauDT,
                           GVNhanXetTHSauDT = a.GVNhanXetTHSauDT,
                           GVThucHanhTruocDT =a.GVThucHanhTruocDT,
                           GVLyThuyetTruocDT =a.GVLyThuyetTruocDT,
                           GVNhanXetLTTruocDT =a.GVNhanXetLTTruocDT,
                           GVNhanXetTHTruocDT =a.GVNhanXetTHTruocDT,
                           GVKetLuanYKienKhac =a.GVKetLuanYKienKhac,
                           GVKetLuan =a.GVKetLuan,
                           HVDeXuat =a.HVDeXuat,
                           HVDeXuatKhac =a.HVDeXuatKhac,
                           HVNgayXacNhan = (DateTime)a.HVNgayXacNhan,
                           IDTinhTrang =a.IDTinhTrang,
                           HVDanhGia = a.IDTinhTrang != 0 ? a.IDTinhTrang == 2 ? "Hoàn thành" : "HV đã xác nhận" : "Chưa hoàn thành",
                           TinhTrangThi = a.TinhTrangThi
                       }).ToList();
            
            //List<PhongBan> dt = db.PhongBans.ToList();
            //ViewBag.IDPB = new SelectList(dt, "IDPhongBan", "TenPhongBan", IDPB);
            //List<LinhVucDT> lv = db.LinhVucDTs.ToList();
            //ViewBag.LVDTID = new SelectList(lv, "IDLVDT", "TenLVDT", IDLVDT);

            //if (IDPB != 0) res = res.Where(x => x.PhongBanID == IDPB).ToList();
            //if (IDLVDT != 0) res = res.Where(x => x.LinhVucID == IDLVDT).ToList();

            if (page == null) page = 1;
            int pageSize = 50;
            int pageNumber = (page ?? 1);
            return View(res.ToList().ToPagedList(pageNumber, pageSize));
        }

        // GET: ConfirmKCCD/Details/5
        public ActionResult Details(int id,int? SuggetID)
        {

            //var phieuxn = dbKCCCD.PhieuXacNhanKCCDs.Where(x=>x.ID ==id).ToList();
            //var nhanvien = db.NhanViens.ToList();
            //var vitri = db.Vitris.ToList();
            //var phongban = db.PhongBans.ToList();

            //var lvdt = db.LinhVucDTs.ToList();
            
            var sugg = new SuggestKCCDController().ListSuggest("").Where(x => x.ID == SuggetID).FirstOrDefault();

            ViewBag.PhieuDN = sugg;  
            var res = (from a in db.PhieuXacNhanKCCD_select(0,0,id)
                       //join b in db.NhanViens on a.HocVienID equals b.ID
                       select new ConfirmKCCDView
                       {
                           ID = a.ID,
                           DeNghiDTID = a.DeNghiDTID,
                           NoiDungDT = a.TenND,
                           HocVienID = a.HocVienID,
                           HoTenHV = a.HoTen,
                           MaNV = a.MaNV,
                           VitriHV = a.TenViTri,
                           PhongBanHV = db.NhanViens.Where(x=>x.ID == a.HocVienID).FirstOrDefault().PhongBan.TenPhongBan,
                           PhongBanPhieu = a.TenPhongBan,
                           HVTruocDatDuoc = a.HVTruocDatDuoc,
                           HVTruocCanCaiThien = a.HVTruocCanCaiThien,
                           HVSauDatDuoc = a.HVSauDatDuoc,
                           HVSauCanCaiThien = a.HVSauCanCaiThien,
                           GVThucHanhSauDT = a.GVThucHanhSauDT,
                           GVLyThuyetSauDT = a.GVLyThuyetSauDT,
                           GVNhanXetLTSauDT = a.GVNhanXetLTSauDT,
                           GVNhanXetTHSauDT = a.GVNhanXetTHSauDT,
                           GVThucHanhTruocDT = a.GVThucHanhTruocDT,
                           GVLyThuyetTruocDT = a.GVLyThuyetTruocDT,
                           GVNhanXetLTTruocDT = a.GVNhanXetLTTruocDT,
                           GVNhanXetTHTruocDT = a.GVNhanXetTHTruocDT,
                           GVKetLuanYKienKhac = a.GVKetLuanYKienKhac,
                           GVKetLuan = a.GVKetLuan,
                           HVDeXuat = a.HVDeXuat,
                           HVDeXuatKhac = a.HVDeXuatKhac,
                           HVNgayXacNhan = (DateTime)a.HVNgayXacNhan,
                           IDTinhTrang = a.IDTinhTrang,
                           HVDanhGia = a.IDTinhTrang != 0 ? a.IDTinhTrang == 2 ? "Hoàn thành" : "HV đã xác nhận" : "Chưa hoàn thành",
                           ThoiLuongDT = ((Convert.ToDateTime(sugg.DenNgay) - Convert.ToDateTime(sugg.TuNgay))).Days +1,
                       }).FirstOrDefault();

            return PartialView(res);
        }

        // GET: ConfirmKCCD/Create
        public ActionResult Create(int? SuggestID)
        {
            var denghi = dbKCCCD.DeNghiKCCDs.Where(x=>x.ID == SuggestID).FirstOrDefault();
            var nv3 = db.NhanViens.Where(x => x.IDPhongBan == MyAuthentication.IDPhongban && x.IDTinhTrangLV == 1 && x.ID != denghi.HuongDan1 && x.ID != denghi.HuongDan2).Select(x => new EmployeeValidation { ID = x.ID, HoTen = x.MaNV + " - " + x.HoTen + " - " + x.MaViTri }).ToList();
            var nvAll = db.NhanViens.Where(x => x.IDPhongBan != 3 && x.IDPhongBan != MyAuthentication.IDPhongban && x.IDTinhTrangLV == 1 && x.ID != denghi.HuongDan1 && x.ID != denghi.HuongDan2).Select(x => new EmployeeValidation { ID = x.ID, HoTen = x.MaNV + " - " + x.HoTen + " - " + x.MaViTri }).ToList();
            ViewBag.NhanVienID = new SelectList(nv3, "ID", "HoTen");
            ViewBag.NhanVienAll = new SelectList(nvAll, "ID", "HoTen");
            ViewBag.SuggestID = SuggestID;
            return PartialView();
        }

        // POST: ConfirmKCCD/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection,ConfirmKCCDView _DO)
        {
            try
            {
                if (_DO.HocVienID != null && _DO.DeNghiDTID != null && _DO.isOutBP == false)
                {
                    var phieuxn = dbKCCCD.PhieuXacNhanKCCDs.ToList();
                    var a =  phieuxn.Where(x=>x.DeNghiDTID == _DO.DeNghiDTID && x.HocVienID == _DO.HocVienID).FirstOrDefault();
                    var IDNDDN = dbKCCCD.DeNghiKCCDs.Where(x => x.ID == _DO.DeNghiDTID ).FirstOrDefault().NoiDungKCCDID;
                    var checkdn = (from k in phieuxn.Where(x => x.HocVienID == _DO.HocVienID)
                                   select new ConfirmKCCDView
                                   {
                                       ID = k.ID,
                                       DeNghiDTID = k.DeNghiKCCD.NoiDungKCCDID,
                                       HocVienID = k.HocVienID,
                                       HVNgayXacNhan = (DateTime)k.HVNgayXacNhan,
                                       IDTinhTrang = k.IDTinhTrang,
                                   }).Where(x=>x.DeNghiDTID == IDNDDN).ToList().OrderByDescending(x=>x.HVNgayXacNhan);

                    var dn = db.DeNghiKCCDs.Where(x => x.ID == _DO.DeNghiDTID).FirstOrDefault();
                    int thi = dn.isKiemTra == 1 ? 0 : -1; // nếu có thi thì 0 không thi -1
                    if (checkdn.Count() == 0)
                    {
                        if(a == null)
                        {
                            _DO.HVNgayXacNhan = DateTime.Now;
                           
                            dbKCCCD.PhieuXacNhanKCCD_insert(_DO.DeNghiDTID, _DO.HocVienID, null, null, null, null, _DO.GVLyThuyetTruocDT, _DO.GVThucHanhTruocDT, _DO.GVNhanXetLTTruocDT, _DO.GVNhanXetTHTruocDT, _DO.GVLyThuyetSauDT, _DO.GVThucHanhSauDT, _DO.GVNhanXetLTSauDT, _DO.GVNhanXetTHSauDT, _DO.GVKetLuan, _DO.GVKetLuanYKienKhac, null, null, _DO.HVNgayXacNhan, 0,thi);
                            TempData["msgSuccess"] = "<script>alert('Thêm mới thành công');</script>";
                        }
                    }
                    else if(checkdn.Count() != 0)
                    {
                        DateTime ngay = (DateTime)checkdn.FirstOrDefault().HVNgayXacNhan;
                        ngay = ngay.AddMonths(3);
                        if (ngay < DateTime.Now && a ==null)
                        {
                            _DO.HVNgayXacNhan = DateTime.Now;
                            dbKCCCD.PhieuXacNhanKCCD_insert(_DO.DeNghiDTID, _DO.HocVienID, null, null, null, null, _DO.GVLyThuyetTruocDT, _DO.GVThucHanhTruocDT, _DO.GVNhanXetLTTruocDT, _DO.GVNhanXetTHTruocDT, _DO.GVLyThuyetSauDT, _DO.GVThucHanhSauDT, _DO.GVNhanXetLTSauDT, _DO.GVNhanXetTHSauDT, _DO.GVKetLuan, _DO.GVKetLuanYKienKhac, null, null, _DO.HVNgayXacNhan, 0, thi);
                            TempData["msgSuccess"] = "<script>alert('Thêm mới thành công');</script>";
                        }
                        else if(ngay >= DateTime.Now)
                        {
                            TempData["msgSuccess"] = "<script>alert('Học viên đã được đào tạo nội dung này trong 3 tháng gần nhất');</script>";
                        }
                    }

                }
                else if(_DO.DeNghiDTID != null && _DO.isOutBP == true && _DO.HocVienIDOutBP != null)
                {
                    var phieuxn = dbKCCCD.PhieuXacNhanKCCDs.ToList();
                    var a = phieuxn.Where(x => x.DeNghiDTID == _DO.DeNghiDTID && x.HocVienID == _DO.HocVienIDOutBP).FirstOrDefault();
                    var IDNDDN = dbKCCCD.DeNghiKCCDs.Where(x => x.ID == _DO.DeNghiDTID).FirstOrDefault().NoiDungKCCDID;
                    var checkdn = (from k in phieuxn.Where(x => x.HocVienID == _DO.HocVienIDOutBP)
                                   select new ConfirmKCCDView
                                   {
                                       ID = k.ID,
                                       DeNghiDTID = k.DeNghiKCCD.NoiDungKCCDID,
                                       HocVienID = k.HocVienID,
                                       HVNgayXacNhan = (DateTime)k.HVNgayXacNhan,
                                       IDTinhTrang = k.IDTinhTrang,
                                   }).Where(x => x.DeNghiDTID == IDNDDN).ToList().OrderByDescending(x => x.HVNgayXacNhan);
                    var dn = db.DeNghiKCCDs.Where(x => x.ID == _DO.DeNghiDTID).FirstOrDefault();
                    int thi = dn.isKiemTra == 1 ? 0 : -1;
                    if (checkdn.Count() == 0)
                    {
                        if (a == null)
                        {
                            _DO.HVNgayXacNhan = DateTime.Now;
                            dbKCCCD.PhieuXacNhanKCCD_insert(_DO.DeNghiDTID, _DO.HocVienIDOutBP, null, null, null, null, _DO.GVLyThuyetTruocDT, _DO.GVThucHanhTruocDT, _DO.GVNhanXetLTTruocDT, _DO.GVNhanXetTHTruocDT, _DO.GVLyThuyetSauDT, _DO.GVThucHanhSauDT, _DO.GVNhanXetLTSauDT, _DO.GVNhanXetTHSauDT, _DO.GVKetLuan, _DO.GVKetLuanYKienKhac, null, null, _DO.HVNgayXacNhan, 0, thi);
                            TempData["msgSuccess"] = "<script>alert('Thêm mới thành công');</script>";
                        }
                    }
                    else if (checkdn.Count() != 0)
                    {
                        DateTime ngay = (DateTime)checkdn.FirstOrDefault().HVNgayXacNhan;
                        ngay = ngay.AddMonths(3);
                        if (ngay < DateTime.Now && a == null)
                        {
                            _DO.HVNgayXacNhan = DateTime.Now;
                            dbKCCCD.PhieuXacNhanKCCD_insert(_DO.DeNghiDTID, _DO.HocVienIDOutBP, null, null, null, null, _DO.GVLyThuyetTruocDT, _DO.GVThucHanhTruocDT, _DO.GVNhanXetLTTruocDT, _DO.GVNhanXetTHTruocDT, _DO.GVLyThuyetSauDT, _DO.GVThucHanhSauDT, _DO.GVNhanXetLTSauDT, _DO.GVNhanXetTHSauDT, _DO.GVKetLuan, _DO.GVKetLuanYKienKhac, null, null, _DO.HVNgayXacNhan, 0, thi);
                            TempData["msgSuccess"] = "<script>alert('Thêm mới thành công');</script>";
                        }
                        else if (ngay >= DateTime.Now)
                        {
                            TempData["msgSuccess"] = "<script>alert('Học viên đã được đào tạo nội dung này trong 3 tháng gần nhất');</script>";
                        }
                    }
                }

                return RedirectToAction("Index",new { SuggestID = _DO.DeNghiDTID });
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Có lỗi khi thêm mới: " + e.Message + "');</script>";
                return RedirectToAction("Index", new { SuggestID = _DO.DeNghiDTID });
            }
        }

        // GET: ConfirmKCCD/Edit/5
        public ActionResult Edit(int id)
        {

            var res = (from a in db.PhieuXacNhanKCCD_select(0,0,id)
                       select new ConfirmKCCDView
                       {
                           ID = a.ID,
                           DeNghiDTID = a.DeNghiDTID,
                           HocVienID = a.HocVienID,
                           HoTenHV = a.HoTen,
                           MaNV =a.MaNV,
                           VitriHV = a.TenViTri,
                           PhongBanHV = a.TenPhongBan,
                           HVTruocDatDuoc = a.HVTruocDatDuoc,
                           HVTruocCanCaiThien = a.HVTruocCanCaiThien,
                           HVSauDatDuoc = a.HVSauDatDuoc,
                           HVSauCanCaiThien = a.HVSauCanCaiThien,
                           GVThucHanhSauDT = a.GVThucHanhSauDT,
                           GVLyThuyetSauDT = a.GVLyThuyetSauDT,
                           GVNhanXetLTSauDT = a.GVNhanXetLTSauDT,
                           GVNhanXetTHSauDT = a.GVNhanXetTHSauDT,
                           GVThucHanhTruocDT = a.GVThucHanhTruocDT,
                           GVLyThuyetTruocDT = a.GVLyThuyetTruocDT,
                           GVNhanXetLTTruocDT = a.GVNhanXetLTTruocDT,
                           GVNhanXetTHTruocDT = a.GVNhanXetTHTruocDT,
                           GVKetLuanYKienKhac = a.GVKetLuanYKienKhac,
                           GVKetLuan = a.GVKetLuan,
                           HVDeXuat = a.HVDeXuat,
                           HVDeXuatKhac = a.HVDeXuatKhac,
                           HVNgayXacNhan = (DateTime)a.HVNgayXacNhan,
                           IDTinhTrang = a.IDTinhTrang
                       }).FirstOrDefault();

            //SuggestKCCDView DO = new SuggestKCCDView();

            var nv3 = db.NhanViens.Where(x => x.ID == res.HocVienID).Select(x => new EmployeeValidation { ID = x.ID, HoTen = x.MaNV + " - " + x.HoTen }).ToList();
            ViewBag.NhanVienID = new SelectList(nv3, "ID", "HoTen",res.HocVienID);

            return PartialView(res);
        }

        // POST: ConfirmKCCD/Edit/5
        [HttpPost]
        public ActionResult Edit(ConfirmKCCDView _DO)
        {
            try
            {
                if (_DO.HocVienID != null && _DO.DeNghiDTID != null)
                {
                   
                        _DO.HVNgayXacNhan = DateTime.Now;
                        dbKCCCD.PhieuXacNhanKCCD_update(_DO.ID,_DO.DeNghiDTID, _DO.HocVienID, _DO.HVTruocDatDuoc,_DO.HVTruocCanCaiThien, _DO.HVSauDatDuoc, _DO.HVSauCanCaiThien, _DO.GVLyThuyetTruocDT, _DO.GVThucHanhTruocDT, _DO.GVNhanXetLTTruocDT, _DO.GVNhanXetTHTruocDT, _DO.GVLyThuyetSauDT, _DO.GVThucHanhSauDT, _DO.GVNhanXetLTSauDT, _DO.GVNhanXetTHSauDT, _DO.GVKetLuan, _DO.GVKetLuanYKienKhac,_DO.HVDeXuat, _DO.HVDeXuatKhac, _DO.HVNgayXacNhan, _DO.IDTinhTrang);
                }

                TempData["msgSuccess"] = "<script>alert('Thêm mới thành công');</script>";

                return RedirectToAction("Index", new { SuggestID = _DO.DeNghiDTID });
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Có lỗi khi thêm mới: " + e.Message + "');</script>";
                return RedirectToAction("Index", new { SuggestID = _DO.DeNghiDTID });
            }
        }

        // GET: ConfirmKCCD/Delete/5
        public ActionResult Delete(int id,int? IDDN)
        {
            try
            {
                dbKCCCD.PhieuXacNhanKCCD_delete(id);
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Xóa dữ liệu thất bại');</script>";
            }
            return RedirectToAction("Index",new { SuggestID = IDDN });
        }

        // POST: ConfirmKCCD/Delete/5
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
            string path = "/App_Data/BM_DSHV_KCCD.xlsx";
            return File(path, "application/vnd.ms-excel", "BM_DSHV_KCCD.xlsx");
        }

        public ActionResult ImportExcel(int? SuggestID)
        {
            ViewBag.SuggestID = SuggestID;
            return PartialView();
        }
        [HttpPost]

        public ActionResult ImportExcel(ConfirmKCCDView _DO)
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

                            for (int i = 3; i < dt.Rows.Count; i++)
                            {

                                if (dt.Rows[i] != null)
                                {

                                    var HocVienID = GetIDNV(dt.Rows[i][1].ToString());
                                    
                                    if (HocVienID != 0 )
                                    {
                                        _DO.HocVienID = HocVienID;
                                        _DO.HVNgayXacNhan = DateTime.Now;
                                        _DO.GVLyThuyetTruocDT = ToNullableInt(dt.Rows[i][4].ToString());
                                        _DO.GVNhanXetLTTruocDT = dt.Rows[i][5].ToString();
                                        _DO.GVThucHanhTruocDT = ToNullableInt(dt.Rows[i][6].ToString());
                                        _DO.GVNhanXetTHTruocDT = dt.Rows[i][7].ToString();
                                        _DO.GVLyThuyetSauDT = ToNullableInt(dt.Rows[i][8].ToString());
                                        _DO.GVNhanXetLTSauDT = dt.Rows[i][9].ToString();
                                        _DO.GVThucHanhSauDT = ToNullableInt(dt.Rows[i][10].ToString());
                                        _DO.GVNhanXetTHSauDT = dt.Rows[i][11].ToString();
                                        _DO.GVKetLuan = dt.Rows[i][12].ToString() == "Đạt" ? 1 : 2;
                                        _DO.GVKetLuanYKienKhac = dt.Rows[i][13].ToString();



                                        var phieuxn = dbKCCCD.PhieuXacNhanKCCDs.ToList();
                                        var a = phieuxn.Where(x => x.DeNghiDTID == _DO.DeNghiDTID && x.HocVienID == _DO.HocVienID).FirstOrDefault();
                                        var IDNDDN = dbKCCCD.DeNghiKCCDs.Where(x => x.ID == _DO.DeNghiDTID).FirstOrDefault().NoiDungKCCDID;
                                        var checkdn = (from k in phieuxn.Where(x => x.HocVienID == _DO.HocVienID)
                                                       select new ConfirmKCCDView
                                                       {
                                                           ID = k.ID,
                                                           DeNghiDTID = k.DeNghiKCCD.NoiDungKCCDID,
                                                           HocVienID = k.HocVienID,
                                                           HVNgayXacNhan = (DateTime)k.HVNgayXacNhan,
                                                           IDTinhTrang = k.IDTinhTrang,
                                                       }).ToList().Where(x => x.DeNghiDTID == IDNDDN).OrderByDescending(x => x.HVNgayXacNhan);

                                        //var a = dbKCCCD.PhieuXacNhanKCCDs.Where(x => x.DeNghiDTID == _DO.DeNghiDTID && x.HocVienID == _DO.HocVienID).FirstOrDefault();
                                        //if (a == null)
                                        //{
                                        //    _DO.HVNgayXacNhan = DateTime.Now;
                                        //    dbKCCCD.PhieuXacNhanKCCD_insert(_DO.DeNghiDTID, _DO.HocVienID, null, null, null, null, _DO.GVLyThuyetTruocDT, _DO.GVThucHanhTruocDT, _DO.GVNhanXetLTTruocDT, _DO.GVNhanXetTHTruocDT, _DO.GVLyThuyetSauDT, _DO.GVThucHanhSauDT, _DO.GVNhanXetLTSauDT, _DO.GVNhanXetTHSauDT, _DO.GVKetLuan, _DO.GVKetLuanYKienKhac, null, null, _DO.HVNgayXacNhan, 0);
                                        //    dtc++;
                                        //}
                                        //else
                                        //{
                                        //    DateTime aa = (DateTime)a.HVNgayXacNhan;
                                        //    aa = aa.AddMonths(3);
                                        //    if (aa < DateTime.Now)
                                        //    {
                                        //        _DO.HVNgayXacNhan = DateTime.Now;
                                        //        dbKCCCD.PhieuXacNhanKCCD_insert(_DO.DeNghiDTID, _DO.HocVienID, null, null, null, null, _DO.GVLyThuyetTruocDT, _DO.GVThucHanhTruocDT, _DO.GVNhanXetLTTruocDT, _DO.GVNhanXetTHTruocDT, _DO.GVLyThuyetSauDT, _DO.GVThucHanhSauDT, _DO.GVNhanXetLTSauDT, _DO.GVNhanXetTHSauDT, _DO.GVKetLuan, _DO.GVKetLuanYKienKhac, null, null, _DO.HVNgayXacNhan, 0);
                                        //        dtc++;
                                        //    }
                                        //}
                                        var dn = db.DeNghiKCCDs.Where(x => x.ID == _DO.DeNghiDTID).FirstOrDefault();
                                        int thi = dn.isKiemTra == 1 ? 0 : -1;
                                        if (checkdn.Count() == 0)
                                        {
                                            if (a == null)
                                            {
                                                _DO.HVNgayXacNhan = DateTime.Now;
                                                if (_DO.GVLyThuyetTruocDT != null && _DO.GVLyThuyetSauDT >= 5 && _DO.GVThucHanhTruocDT != null && _DO.GVThucHanhSauDT >= 5 && _DO.GVNhanXetLTTruocDT != "" && _DO.GVNhanXetLTSauDT != "" && _DO.GVNhanXetTHTruocDT != "" && _DO.GVNhanXetTHSauDT != "")
                                                {
                                                    dbKCCCD.PhieuXacNhanKCCD_insert(_DO.DeNghiDTID, _DO.HocVienID, null, null, null, null, _DO.GVLyThuyetTruocDT, _DO.GVThucHanhTruocDT, _DO.GVNhanXetLTTruocDT, _DO.GVNhanXetTHTruocDT, _DO.GVLyThuyetSauDT, _DO.GVThucHanhSauDT, _DO.GVNhanXetLTSauDT, _DO.GVNhanXetTHSauDT, _DO.GVKetLuan, _DO.GVKetLuanYKienKhac, null, null, _DO.HVNgayXacNhan, 0, thi);
                                                    dtc++;
                                                }
                                                    
                                            }
                                        }
                                        else if (checkdn.Count() != 0)
                                        {
                                            DateTime ngay = (DateTime)checkdn.FirstOrDefault().HVNgayXacNhan;
                                            ngay = ngay.AddMonths(3);
                                            if (ngay < DateTime.Now && a == null)
                                            {
                                                _DO.HVNgayXacNhan = DateTime.Now;
                                                if(_DO.GVLyThuyetTruocDT != null && _DO.GVLyThuyetSauDT >=5 && _DO.GVThucHanhTruocDT != null && _DO.GVThucHanhSauDT >=5  && _DO.GVNhanXetLTTruocDT != "" && _DO.GVNhanXetLTSauDT != "" && _DO.GVNhanXetTHTruocDT != "" && _DO.GVNhanXetTHSauDT != "")
                                                {
                                                    dbKCCCD.PhieuXacNhanKCCD_insert(_DO.DeNghiDTID, _DO.HocVienID, null, null, null, null, _DO.GVLyThuyetTruocDT, _DO.GVThucHanhTruocDT, _DO.GVNhanXetLTTruocDT, _DO.GVNhanXetTHTruocDT, _DO.GVLyThuyetSauDT, _DO.GVThucHanhSauDT, _DO.GVNhanXetLTSauDT, _DO.GVNhanXetTHSauDT, _DO.GVKetLuan, _DO.GVKetLuanYKienKhac, null, null, _DO.HVNgayXacNhan, 0, thi);
                                                    dtc++;
                                                }
                                                   
                                            }
                                            //else if (ngay >= DateTime.Now)
                                            //{
                                            //    TempData["msgSuccess"] = "<script>alert('Học viên đã được đào tạo nội dung này trong 3 tháng gần nhất');</script>";
                                            //}
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

            return RedirectToAction("Index", new { SuggestID = _DO.DeNghiDTID });
        }

        public int GetIDNV(string MaNV)
        {
            var model = db.NhanViens.Where(x => x.MaNV == MaNV && x.IDTinhTrangLV ==1 && x.IDPhongBan == MyAuthentication.IDPhongban).SingleOrDefault();
            if (model == null)
                return 0;
            return model.ID;
        }
        public float? ToNullableInt(string s)
        {
            float i;
            if (float.TryParse(s, out i)) return i;
            return null;
        }

    }
}
