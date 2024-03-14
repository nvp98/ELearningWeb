using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2010.Excel;
using E_Learning.Models;
using E_Learning.ModelsKCCD;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace E_Learning.Controllers.KCCD
{
    public class SuggestKCCDController : Controller
    {
        ELEARNINGEntities db = new ELEARNINGEntities();
        ELEARNINGEntities dbKCCCD = new ELEARNINGEntities();
        int Idquyen = MyAuthentication.IDQuyen;
        String ControllerName = "SuggestKCCD";
        // GET: SuggestKCCD
        public ActionResult Index(int? page, string search,int? IDLVDT,int? IDNDDT, int? IDPB,  int? IDTinhTrang,DateTime? begind, DateTime? endd)
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            ViewBag.QUYENCN = ListQuyen;
            //if (!ListQuyen.Contains(CONSTKEY.V))
            //{
            //    TempData["msgError"] = "<script>alert('Bạn không có quyền truy cập chức năng này');</script>";
            //    return RedirectToAction("", "Home");
            //}

            if (search == null) search = "";
            ViewBag.search = search;
            if (IDPB == null) IDPB = 0;
            if (IDTinhTrang == null) IDTinhTrang = 0;
            if (IDNDDT == null) IDNDDT = 0;
            //if (IDLVDT == null) IDLVDT = 0;
            if (begind == null) begind = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1); ;
            if (endd == null) endd = DateTime.Now;
            //if (IDLVDT == null) IDLVDT = 0;
            var phieunx = dbKCCCD.PhieuXacNhanKCCDs.ToList();
            //var begindd = begind;
            //var end = endd;
            //var begindd = TuNgay?.Date + new TimeSpan(0, 00, 00);
            //var endd = DenNgay?.Date + new TimeSpan(23, 59, 59);
            //begind = begind?.Date + new TimeSpan(0, 00, 00);
            //endd = endd?.Date + new TimeSpan(23, 59, 59);

            //var phongban = db.PhongBans.ToList();
            //var nhanvien = db.NhanViens.ToList();
            //var vitri = db.Vitris.ToList();
            //var lvdt = db.LinhVucDTs.ToList();
            //var denghi = dbKCCCD.DeNghiKCCD_selectKCCD(search).Where(x=>x.TuNgay >=begind && x.DenNgay <= endd).ToList();

            //if (ListQuyen.Contains(CONSTKEY.V_BP)) denghi = denghi.Where(x => x.PhongBanID == MyAuthentication.IDPhongban).ToList();
            //else if (ListQuyen.Contains(CONSTKEY.VIEW_ALL)) denghi.ToList();
            //else denghi = denghi.Where(x => x.HuongDan1 == MyAuthentication.ID).ToList();

            var res = new List<SuggestKCCDView>();
            if(!ListQuyen.Contains(CONSTKEY.V_BP) && !ListQuyen.Contains(CONSTKEY.VIEW_ALL))
            {
                res = (from a in db.DeNghiKCCD_select(search).Where(x => x.HuongDan1 == MyAuthentication.ID && x.TuNgay >= begind && x.DenNgay <= endd)
                           //join b in phongban on a.PhongBanID equals b.IDPhongBan
                           //join c in lvdt on a.LinhVucID equals c.IDLVDT
                       select new SuggestKCCDView
                       {
                           ID = a.ID,
                           TenNDDT = a.TenND,
                           NoiDungKCCDID = a.NoiDungKCCDID,
                           LinhVucID = a.LinhVucID,
                           TenLVDT = a.TenLVDT,
                           NhomNangLucID = a.NhomNangLucID,
                           TenNhomNL = a.TenNhomNL,
                           PhongBanID = a.PhongBanID,
                           TenPhongBan = a.TenPhongBan,
                           NgayTao = (DateTime)a.NgayTao,
                           TuNgay = (DateTime)a.TuNgay,
                           DenNgay = (DateTime)a.DenNgay,
                           NgayXN = a.NgayXN ?? DateTime.Now,
                           HoTen1 = a.HoTen1,
                           ViTriID1 = a.ViTriID1,
                           TenViTri1 = a.VT1,
                           MaNV1 = a.MaNV1,
                           HoTen2 = a.HoTen2,
                           ViTriID2 = a.ViTriID2,
                           TenViTri2 = a.VT2,
                           MaNV2 = a.MaNV2,
                           HuongDan1 = a.HuongDan1,
                           HuongDan2 = a.HuongDan2,
                           TinhTrang = a.TinhTrang,
                           SLHV = phieunx.Where(x => x.DeNghiDTID == a.ID).Count(),
                           SLHVHT = phieunx.Where(x => x.DeNghiDTID == a.ID && x.IDTinhTrang != 0).Count(),
                           DeThiID =a.DeThiID,
                           isKiemTra =a.isKiemTra ==1?true:false,
                       }).ToList();
            }
            else if(ListQuyen.Contains(CONSTKEY.V_BP))
            {
                res = (from a in db.DeNghiKCCD_select(search).Where(x =>  x.PhongBanID == MyAuthentication.IDPhongban && x.TuNgay >= begind && x.DenNgay <= endd)
                           //join b in phongban on a.PhongBanID equals b.IDPhongBan
                           //join c in lvdt on a.LinhVucID equals c.IDLVDT
                       select new SuggestKCCDView
                       {
                           ID = a.ID,
                           TenNDDT = a.TenND,
                           NoiDungKCCDID = a.NoiDungKCCDID,
                           LinhVucID = a.LinhVucID,
                           TenLVDT = a.TenLVDT,
                           NhomNangLucID = a.NhomNangLucID,
                           TenNhomNL = a.TenNhomNL,
                           PhongBanID = a.PhongBanID,
                           TenPhongBan = a.TenPhongBan,
                           NgayTao = (DateTime)a.NgayTao,
                           TuNgay = (DateTime)a.TuNgay,
                           DenNgay = (DateTime)a.DenNgay,
                           NgayXN = a.NgayXN ?? DateTime.Now,
                           HoTen1 = a.HoTen1,
                           ViTriID1 = a.ViTriID1,
                           TenViTri1 = a.VT1,
                           MaNV1 = a.MaNV1,
                           HoTen2 = a.HoTen2,
                           ViTriID2 = a.ViTriID2,
                           TenViTri2 = a.VT2,
                           MaNV2 = a.MaNV2,
                           HuongDan1 = a.HuongDan1,
                           HuongDan2 = a.HuongDan2,
                           TinhTrang = a.TinhTrang,
                           SLHV = phieunx.Where(x => x.DeNghiDTID == a.ID).Count(),
                           SLHVHT = phieunx.Where(x => x.DeNghiDTID == a.ID && x.IDTinhTrang != 0).Count(),
                           DeThiID = a.DeThiID,
                           isKiemTra = a.isKiemTra == 1 ? true : false,
                       }).ToList();
            }
            else if(ListQuyen.Contains(CONSTKEY.VIEW_ALL))
            {
                res = (from a in db.DeNghiKCCD_select(search).Where(x=> x.TuNgay >= begind && x.DenNgay <= endd)
                           //join b in phongban on a.PhongBanID equals b.IDPhongBan
                           //join c in lvdt on a.LinhVucID equals c.IDLVDT
                       select new SuggestKCCDView
                       {
                           ID = a.ID,
                           TenNDDT = a.TenND,
                           NoiDungKCCDID = a.NoiDungKCCDID,
                           LinhVucID = a.LinhVucID,
                           TenLVDT = a.TenLVDT,
                           NhomNangLucID = a.NhomNangLucID,
                           TenNhomNL = a.TenNhomNL,
                           PhongBanID = a.PhongBanID,
                           TenPhongBan = a.TenPhongBan,
                           NgayTao = (DateTime)a.NgayTao,
                           TuNgay = (DateTime)a.TuNgay,
                           DenNgay = (DateTime)a.DenNgay,
                           NgayXN = a.NgayXN ?? DateTime.Now,
                           HoTen1 = a.HoTen1,
                           ViTriID1 = a.ViTriID1,
                           TenViTri1 = a.VT1,
                           MaNV1 = a.MaNV1,
                           HoTen2 = a.HoTen2,
                           ViTriID2 = a.ViTriID2,
                           TenViTri2 = a.VT2,
                           MaNV2 = a.MaNV2,
                           HuongDan1 = a.HuongDan1,
                           HuongDan2 = a.HuongDan2,
                           TinhTrang = a.TinhTrang,
                           SLHV = phieunx.Where(x => x.DeNghiDTID == a.ID).Count(),
                           SLHVHT = phieunx.Where(x => x.DeNghiDTID == a.ID && x.IDTinhTrang != 0).Count(),
                           DeThiID = a.DeThiID,
                           isKiemTra = a.isKiemTra == 1 ? true : false,
                       }).ToList();
            }

            List<PhongBan> dt = db.PhongBans.ToList();
            ViewBag.IDPB = new SelectList(dt, "IDPhongBan", "TenPhongBan", IDPB);
            ViewBag.begind = begind?.ToString("yyyy-MM-dd");
            ViewBag.endd = endd?.ToString("yyyy-MM-dd");


            List<NoiDungDTKCCD> nd = dbKCCCD.NoiDungDTKCCDs.ToList();
            if (ListQuyen.Contains(CONSTKEY.V_BP))
            {
                nd = nd.Where(x=>x.PhongBanID == MyAuthentication.IDPhongban).ToList();
            }
            ViewBag.IDNDDT = new SelectList(nd, "ID", "TenND");

            //List<LinhVucDT> lv = db.LinhVucDTs.ToList();
            //ViewBag.LVDTID = new SelectList(lv, "IDLVDT", "TenLVDT", IDLVDT);

            //List<LinhVucDT> lv = db.LinhVucDTs.ToList();
            //ViewBag.LVDTID = new SelectList(lv, "IDLVDT", "TenLVDT", IDLVDT);

            if (begind != null) res = res.Where(x => x.TuNgay >= begind).ToList();
            if (endd != null) res = res.Where(x => x.DenNgay <= endd).ToList();
            if (IDPB != 0) res = res.Where(x => x.PhongBanID == IDPB).ToList();
            if (IDTinhTrang != 0) res = res.Where(x => x.TinhTrang == IDTinhTrang).ToList();
            //if (IDLVDT != 0) res = res.Where(x => x.LinhVucID == IDLVDT).ToList();
            if (IDNDDT != 0) res = res.Where(x => x.NoiDungKCCDID == IDNDDT).ToList();

            if (page == null) page = 1;
            int pageSize =50;
            int pageNumber = (page ?? 1);
            return View(res.ToList().ToPagedList(pageNumber, pageSize));
        }

        // GET: SuggestKCCD/Details/5
        public ActionResult Approve(int? page, string search, int? IDPB , int? IDTinhTrang, int? NhanVienID)
        {
            if (search == null) search = "";
            ViewBag.search = search;
            if (IDPB == null) IDPB = 0;
            //if (IDLVDT == null) IDLVDT = 0;
            if (NhanVienID == null) NhanVienID = 0;
            if(IDTinhTrang == null ) IDTinhTrang = 0;
            //if (begind == null) begind = DateTime.Now.AddDays(-7);
            //if (endd == null) endd = DateTime.Now;
            var phieunx = dbKCCCD.PhieuXacNhanKCCDs.ToList();


            //var phongban = db.PhongBans.ToList();
            //var nhanvien = db.NhanViens.ToList();
            //var vitri = db.Vitris.ToList();
            //var lvdt = db.LinhVucDTs.ToList();
            //begind = begind?.Date + new TimeSpan(0, 00, 00);
            //endd = endd?.Date + new TimeSpan(23, 59, 59);

            var res = (from a in dbKCCCD.DeNghiKCCD_select(search).Where(x=>x.HuongDan2 == MyAuthentication.ID )
                       //join b in phongban on a.PhongBanID equals b.IDPhongBan
                       //join c in lvdt on a.LinhVucID equals c.IDLVDT
                       select new SuggestKCCDView
                       {
                           ID = a.ID,
                           TenNDDT = a.TenND,
                           NoiDungKCCDID = a.NoiDungKCCDID,
                           LinhVucID = a.LinhVucID,
                           TenLVDT = a.TenLVDT,
                           NhomNangLucID = a.NhomNangLucID,
                           TenNhomNL = a.TenNhomNL,
                           PhongBanID = a.PhongBanID,
                           TenPhongBan = a.TenPhongBan,
                           NgayTao = (DateTime)a.NgayTao,
                           TuNgay = (DateTime)a.TuNgay,
                           DenNgay = (DateTime)a.DenNgay,
                           NgayXN = a.NgayXN ?? DateTime.Now,
                           HoTen1 = a.HoTen1,
                           ViTriID1 = a.ViTriID1,
                           TenViTri1 = a.VT1,
                           MaNV1 = a.MaNV1,
                           HoTen2 = a.HoTen2,
                           ViTriID2 = a.ViTriID2,
                           TenViTri2 = a.VT2,
                           MaNV2 = a.MaNV2,
                           HuongDan1 = a.HuongDan1,
                           HuongDan2 = a.HuongDan2,
                           TinhTrang = a.TinhTrang,
                           SLHV = phieunx.Where(x => x.DeNghiDTID == a.ID).Count(),
                           SLHVHT = phieunx.Where(x => x.DeNghiDTID == a.ID && x.IDTinhTrang != 0).Count(),
                           TinhTrangHVXN = phieunx.Where(x => x.DeNghiDTID == a.ID).Count() == phieunx.Where(x => x.DeNghiDTID == a.ID && x.IDTinhTrang ==1).Count() && phieunx.Where(x => x.DeNghiDTID == a.ID).Count() != 0 ? 1:0,
                           TinhTrangThi = a.isKiemTra == 1 ? phieunx.Where(x=> x.DeNghiDTID == a.ID &&(x.TinhTrangThi ==1 ||x.TinhTrangThi ==3)).Count() == phieunx.Where(x => x.DeNghiDTID == a.ID).Count() ?1:0:1
                       }).ToList();

            List<PhongBan> dt = db.PhongBans.ToList();
            ViewBag.IDPB = new SelectList(dt, "IDPhongBan", "TenPhongBan", IDPB);
            //List<LinhVucDT> lv = db.LinhVucDTs.ToList();
            //ViewBag.LVDTID = new SelectList(lv, "IDLVDT", "TenLVDT", IDLVDT);
           var nv = db.NhanViens.Where(x=>x.IDPhongBan == MyAuthentication.IDPhongban).Select(x => new EmployeeValidation { ID = x.ID, HoTen = x.MaNV + " - " + x.HoTen + " - " + x.MaViTri }).ToList();
            ViewBag.NhanVienID = new SelectList(nv, "ID", "HoTen");

            List<SelectListItem> abc = new List<SelectListItem> {
                                          new SelectListItem { Value = "1" , Text = "Chưa Duyệt" },
                                          new SelectListItem { Value = "2" , Text = "Đã duyệt" },
                                          new SelectListItem { Value = "3" , Text = "Từ chối" },
                                       };
            ViewBag.IDTinhTrang = new SelectList(abc, "Value", "Text",IDTinhTrang);
            //ViewBag.begind = begind?.ToString("yyyy-MM-dd");
            //ViewBag.endd = endd?.ToString("yyyy-MM-dd");

            if (IDPB != 0) res = res.Where(x => x.PhongBanID == IDPB).ToList();
            //if (IDLVDT != 0) res = res.Where(x => x.LinhVucID == IDLVDT).ToList();
            if (NhanVienID != 0) res = res.Where(x => x.HuongDan1 == NhanVienID).ToList();

            if (IDTinhTrang != 0)
            {
                res = res.Where(x => x.TinhTrang == IDTinhTrang).ToList();
            }
            //else
            //{
            //    //if (begind != null) res = res.Where(x => x.TuNgay >= begind).ToList();
            //    //if (endd != null) res = res.Where(x => x.DenNgay <= endd).ToList();
            //}

            //if (IDTinhTrang == 2) res = res.Where(x => x.TinhTrang == IDTinhTrang).ToList();


            if (page == null) page = 1;
            int pageSize = 50;
            int pageNumber = (page ?? 1);
            return View(res.ToList().OrderBy(x=>x.TinhTrang).ToPagedList(pageNumber, pageSize));
        }
        public ActionResult ApproveXN(int id)
        {
            try
            {
                if(id != 0) 
                {
                    
                    dbKCCCD.DeNghiKCCD_XacNhan(id,2, DateTime.Now);
                    dbKCCCD.PhieuXacNhanKCCD_XacNhan(id,DateTime.Now,2);
                    TempData["msgSuccess"] = "<script>alert('Phê duyệt thành công');</script>";
                }
                
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Xóa dữ liệu thất bại');</script>";
            }
            return RedirectToAction("Approve", "SuggestKCCD");
        }

        public ActionResult CancelPhieu(int id)
        {
            try
            {
                if (id != 0)
                {

                    dbKCCCD.DeNghiKCCD_XacNhan(id, 3, DateTime.Now);
                    dbKCCCD.PhieuXacNhanKCCD_XacNhan(id, DateTime.Now, 3);
                    TempData["msgSuccess"] = "<script>alert('Hủy đề nghị thành công');</script>";
                }

            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Xóa dữ liệu thất bại');</script>";
            }
            return RedirectToAction("Approve", "SuggestKCCD");
        }

        // GET: SuggestKCCD/Create
        public ActionResult Create()
        {
            //var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            //if (!ListQuyen.Contains(CONSTKEY.ADD))
            //{
            //    TempData["msgError"] = "<script>alert('Bạn không có quyền thực hiện chức năng này');</script>";
            //    return RedirectToAction("", "Home");
            //}
            //List<PhongBan> dt = db.PhongBans.ToList();
            //ViewBag.PhongBanID = new SelectList(dt, "IDPhongBan", "TenPhongBan");

            //List<LinhVucDT> lv = db.LinhVucDTs.ToList();
            //ViewBag.LVDTID = new SelectList(lv, "IDLVDT", "TenLVDT");

            //List<KCCD_DeThi> dt = db.KCCD_DeThi.ToList();
            //ViewBag.DeThiID = new SelectList(dt, "ID", "TenDe");

            var nv3 = db.NhanViens.Where(x=>x.IDPhongBan == MyAuthentication.IDPhongban && x.IDTinhTrangLV ==1 && x.ID != MyAuthentication.ID && x.Vitri.TenViTri.Substring(0,1) != "N").Select(x => new EmployeeValidation { ID = x.ID, HoTen = x.MaNV + " - " + x.HoTen + " - " + x.MaViTri }).ToList();
            ViewBag.NhanVienID = new SelectList(nv3, "ID", "HoTen");

            List<NoiDungDTKCCD> nd = dbKCCCD.NoiDungDTKCCDs.Where(x=>x.PhongBanID == MyAuthentication.IDPhongban).ToList();
            ViewBag.IDNDDT = new SelectList(nd, "ID", "TenND");

            return PartialView();
        }

        // POST: SuggestKCCD/Create
        [HttpPost]
        public ActionResult Create(SuggestKCCDView _DO, FormCollection collection)
        {
            try
            {
                if (_DO.NoiDungKCCDID != null && _DO.HuongDan2 != null )
                {
                    var Date7 = _DO.DenNgay.AddDays(5);
                    if(Date7 >= DateTime.Now && DateTime.Now >= _DO.DenNgay && _DO.TuNgay <= _DO.DenNgay)
                    {
                        var nv2 = db.NhanViens.Where(x => x.ID == _DO.HuongDan2).FirstOrDefault();
                        _DO.NgayTao = DateTime.Now;
                        int isthi = _DO.isKiemTra == true ? 1 : 0;
                        int? iddt = _DO.isKiemTra == true ? _DO.DeThiID : null;
                        var aa = dbKCCCD.DeNghiKCCD_insert(_DO.NoiDungKCCDID, _DO.LinhVucID, _DO.NhomNangLucID, MyAuthentication.IDPhongban, MyAuthentication.ID, MyAuthentication.IDViTri, _DO.HuongDan2, nv2.IDViTri, _DO.NgayTao, 1, _DO.TuNgay, _DO.DenNgay, isthi,iddt);
                        TempData["msgSuccess"] = "<script>alert('Thêm mới thành công');</script>";
                    }
                    else 
                    {
                        TempData["msgSuccess"] = "<script>alert('Đề nghị được lập sau khi kết thúc đào tạo và trong vòng 5 ngày kể từ ngày kết thúc');</script>";
                    }
                    //else if(DateTime.Now > Date7)
                    //{
                    //    TempData["msgSuccess"] = "<script>alert('Ngày tạo phiếu đề nghị không vượt quá 5 ngày kể từ lúc kết thúc đào tạo');</script>";
                    //}
                }
                else
                {
                    TempData["msgSuccess"] = "<script>alert('Hãy kiểm tra đã nhập đủ thông tin trước khi xác nhận');</script>";
                }

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Có lỗi khi thêm mới: " + e.Message + "');</script>";
                return RedirectToAction("Index");
            }
        }

        // GET: SuggestKCCD/Edit/5
        public ActionResult Edit(int id)
        {
            //var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            //if (!ListQuyen.Contains(CONSTKEY.EDIT))
            //{
            //    TempData["msgError"] = "<script>alert('Bạn không có quyền thực hiện chức năng này');</script>";
            //    return RedirectToAction("", "Home");
            //}
            string search = "";

            //var phongban = db.PhongBans.ToList();
            //var nhanvien = db.NhanViens.ToList();
            //var vitri = db.Vitris.ToList();
            //var lvdt = db.LinhVucDTs.ToList();

            var res = (from a in dbKCCCD.DeNghiKCCD_select(search).Where(x => x.ID == id)
                       //join b in phongban on a.PhongBanID equals b.IDPhongBan
                       //join c in lvdt on a.LinhVucID equals c.IDLVDT
                       select new SuggestKCCDView
                       {
                           ID = a.ID,
                           TenNDDT = a.TenND,
                           NoiDungKCCDID = a.NoiDungKCCDID,
                           LinhVucID = a.LinhVucID,
                           TenLVDT = a.TenLVDT,
                           NhomNangLucID = a.NhomNangLucID,
                           TenNhomNL = a.TenNhomNL,
                           PhongBanID = a.PhongBanID,
                           TenPhongBan =a.TenPhongBan,
                           NgayTao = (DateTime)a.NgayTao,
                           TuNgay = (DateTime)a.TuNgay,
                           DenNgay = (DateTime)a.DenNgay,
                           NgayXN = a.NgayXN ?? DateTime.Now,
                           HoTen1 = a.HoTen1,
                           ViTriID1 = a.ViTriID1,
                           TenViTri1 = a.VT1,
                           MaNV1 =a.MaNV1,
                           HoTen2 = a.HoTen2,
                           ViTriID2 = a.ViTriID2,
                           TenViTri2 = a.VT2,
                           MaNV2 = a.MaNV2,
                           HuongDan1 = a.HuongDan1,
                           HuongDan2 = a.HuongDan2,
                           TinhTrang = a.TinhTrang,
                           DeThiID = a.DeThiID,
                           isKiemTra = a.isKiemTra ==1?true:false,
                       }).FirstOrDefault();
            //SuggestKCCDView DO = new SuggestKCCDView();

            db.Configuration.ProxyCreationEnabled = false;
            List<PhongBan> dt = db.PhongBans.ToList();
            ViewBag.PhongBanID = new SelectList(dt, "IDPhongBan", "TenPhongBan", res.PhongBanID);

            List<LinhVucDT> lv = db.LinhVucDTs.ToList();
            ViewBag.LVDTID = new SelectList(lv, "IDLVDT", "TenLVDT", res.LinhVucID);

            List<NhomNLKCCD> nhom = dbKCCCD.NhomNLKCCDs.ToList();
            ViewBag.NhomNLID = new SelectList(nhom, "ID", "NoiDung", res.NhomNangLucID);

            List<KCCD_DeThi> dethi = dbKCCCD.KCCD_DeThi.ToList();
            ViewBag.IDDeThi = new SelectList(dethi, "ID", "TenDe", res.DeThiID);

            var ListNV = db.NhanViens.Where(x => x.IDTinhTrangLV == 1).ToList();
            if (res.HuongDan1 == MyAuthentication.ID)
            {
                ListNV = ListNV.Where(x => x.IDPhongBan == MyAuthentication.IDPhongban && x.ID != MyAuthentication.ID).ToList();
            }
            else
            {
                ListNV = ListNV.Where(x => x.ID == res.HuongDan2).ToList();
            }
            var nv3 = ListNV.Select(x => new EmployeeValidation { ID = x.ID, HoTen = x.MaNV + " - " + x.HoTen }).ToList();
          
            ViewBag.NhanVienID = new SelectList(nv3, "ID", "HoTen", res.HuongDan2);

            List<NoiDungDTKCCD> nd = dbKCCCD.NoiDungDTKCCDs.ToList();
            if (res.HuongDan1 == MyAuthentication.ID)
            {
                nd = nd.Where(x => x.PhongBanID == MyAuthentication.IDPhongban).ToList();
            }
            else
            {
                nd = nd.Where(x=>x.ID == res.NoiDungKCCDID).ToList();
            }
            ViewBag.IDNDDT = new SelectList(nd, "ID", "TenND",res.NoiDungKCCDID);

            ViewBag.TuNgay = res.TuNgay.ToString("yyyy-MM-dd");
            ViewBag.DenNgay = res.DenNgay.ToString("yyyy-MM-dd");

            //if (res.Count > 0)
            //{
            //    foreach (var c in res)
            //    {
            //        DO.ID = c.ID;
            //        DO.TenNDDT = c.TenNDDT;
            //        DO.LinhVucID = c.LinhVucID;
            //        DO.TenLVDT = c.TenLVDT;
            //        DO.NhomNangLucID = c.NhomNangLucID;
            //        DO.TenNhomNL = c.TenNhomNL;
            //        DO.PhongBanID = c.PhongBanID;
            //        DO.TenPhongBan = c.TenPhongBan;
            //        DO.NgayTao = c.NgayTao;
            //        DO.MaNV1 = c.MaNV1;
            //        DO.
            //    }

            //    db.Configuration.ProxyCreationEnabled = false;
            //    List<PhongBan> dt = db.PhongBans.ToList();
            //    ViewBag.PhongBanID = new SelectList(dt, "IDPhongBan", "TenPhongBan", DO.PhongBanID);

            //    List<LinhVucDT> lv = db.LinhVucDTs.ToList();
            //    ViewBag.LVDTID = new SelectList(lv, "IDLVDT", "TenLVDT", DO.LVDTID);

            //    List<NhomNLKCCD> nhom = dbKCCCD.NhomNLKCCDs.ToList();
            //    ViewBag.NhomNLID = new SelectList(nhom, "ID", "NoiDung", DO.NhomNLID);

            //}
            //else
            //{
            //    return HttpNotFound();
            //}
            return PartialView(res);
        }

        // POST: SuggestKCCD/Edit/5
        [HttpPost]
        public ActionResult Edit(SuggestKCCDView _DO)
        {
            try
            {
                //if (_DO.NoiDungKCCDID != null && _DO.TuNgay != null && _DO.DenNgay != null)
                //{
                //    var nv2 = db.NhanViens.Where(x => x.ID == _DO.HuongDan2).FirstOrDefault();
                //    _DO.NgayTao = DateTime.Now;
                //    var aa = dbKCCCD.DeNghiKCCD_update(_DO.ID,_DO.NoiDungKCCDID, _DO.LinhVucID, _DO.NhomNangLucID, _DO.PhongBanID, _DO.HuongDan1, _DO.ViTriID1, _DO.HuongDan2, nv2.IDViTri, _DO.NgayTao, 1, _DO.TuNgay, _DO.DenNgay);
                //}

                //TempData["msgSuccess"] = "<script>alert('Thêm mới thành công');</script>";

                var phieuxn = dbKCCCD.PhieuXacNhanKCCDs.Where(x=>x.DeNghiDTID ==_DO.ID && x.IDTinhTrang ==1).ToList();


                if (_DO.NoiDungKCCDID != null && _DO.HuongDan2 != null && phieuxn.Count() ==0 && _DO.HuongDan1 == MyAuthentication.ID)
                {
                    var Date7 = _DO.DenNgay.AddDays(5);
                    if (Date7 >= DateTime.Now && DateTime.Now >= _DO.DenNgay && _DO.TuNgay <= _DO.DenNgay)
                    {
                        var nv2 = db.NhanViens.Where(x => x.ID == _DO.HuongDan2).FirstOrDefault();
                        _DO.NgayTao = DateTime.Now;
                        int isthi = _DO.isKiemTra == true ? 1 : 0;
                        int? iddt = _DO.isKiemTra == true ? _DO.DeThiID : null;
                        var aa = dbKCCCD.DeNghiKCCD_update(_DO.ID, _DO.NoiDungKCCDID, _DO.LinhVucID, _DO.NhomNangLucID, _DO.PhongBanID, _DO.HuongDan1, _DO.ViTriID1, _DO.HuongDan2, nv2.IDViTri, _DO.NgayTao, 1, _DO.TuNgay, _DO.DenNgay,isthi,iddt);
                        //var aa = dbKCCCD.DeNghiKCCD_update(_DO.NoiDungKCCDID, _DO.LinhVucID, _DO.NhomNangLucID, MyAuthentication.IDPhongban, MyAuthentication.ID, MyAuthentication.IDViTri, _DO.HuongDan2, nv2.IDViTri, _DO.NgayTao, 1, _DO.TuNgay, _DO.DenNgay);
                        TempData["msgSuccess"] = "<script>alert('Cập nhật thành công');</script>";
                    }
                    else
                    {
                        TempData["msgSuccess"] = "<script>alert('TG bắt đầu không được sau TG kết thúc và Đề nghị được lập trong vòng 5 ngày kể từ ngày kết thúc');</script>";
                    }
                    //else if(DateTime.Now > Date7)
                    //{
                    //    TempData["msgSuccess"] = "<script>alert('Ngày tạo phiếu đề nghị không vượt quá 5 ngày kể từ lúc kết thúc đào tạo');</script>";
                    //}
                }
                else if (_DO.HuongDan1 != MyAuthentication.ID)
                {
                    TempData["msgSuccess"] = "<script>alert('Bạn không có quyền thay đổi thông tin');</script>";
                }
                else if(phieuxn.Count() != 0)
                {
                    TempData["msgSuccess"] = "<script>alert('Đề nghị đã được lưu, không được thay đổi thông tin');</script>";
                }
                else
                {
                    TempData["msgSuccess"] = "<script>alert('Hãy kiểm tra đã nhập đủ thông tin trước khi xác nhận');</script>";
                }

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Có lỗi khi thêm mới: " + e.Message + "');</script>";
                return RedirectToAction("Index");
            }
        }

        // GET: SuggestKCCD/Delete/5
        public ActionResult Delete(int id)
        {
            try
            {
                dbKCCCD.DeNghiKCCD_delete(id);
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Xóa dữ liệu thất bại');</script>";
            }
            return RedirectToAction("Index");
        }


        public ActionResult ExportData()
        {
            return PartialView();
        }

        // POST: SuggestKCCD/Create
        [HttpPost]
        public ActionResult ExportData(SuggestKCCDView _DO)
        {
            try
            {
                string fileNamemau = AppDomain.CurrentDomain.BaseDirectory + @"App_Data\DS_HocVienKCCD.xlsx";
                string fileNamemaunew = AppDomain.CurrentDomain.BaseDirectory + @"App_Data\DS_HocVienKCCD_Temp.xlsx";
                XLWorkbook Workbook = new XLWorkbook(fileNamemau);
                IXLWorksheet Worksheet = Workbook.Worksheet("DSHV");
                List<ConfirmKCCDExport> DataKNL = ListPhieuXN(_DO.TuNgay,_DO.DenNgay);
                int row = 4;
                if (DataKNL.Count > 0)
                {
                    foreach (var data in DataKNL)
                    {
                        Worksheet.Cell(row, "A").Value = row - 3;
                        Worksheet.Cell(row, "A").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        Worksheet.Cell(row, "A").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "A").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "B").Value = data.MaNV;
                        Worksheet.Cell(row, "B").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "B").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "B").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "C").Value = data.HoTenHV;
                        Worksheet.Cell(row, "C").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "C").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "C").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "D").Value = data.TenPhongBan;
                        Worksheet.Cell(row, "D").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "D").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "D").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "E").Value = data.NoiDungID;
                        Worksheet.Cell(row, "E").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "E").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "E").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "F").Value = data.NoiDungDT;
                        Worksheet.Cell(row, "F").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "F").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "F").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;


                        Worksheet.Cell(row, "G").Value = data.ThoiLuongDT;
                        Worksheet.Cell(row, "G").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        Worksheet.Cell(row, "G").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "G").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        //Worksheet.Cell(row, "F").Style.Fill.BackgroundColor = XLColor.Yellow;

                        Worksheet.Cell(row, "H").Value = data.TenLVDT;
                        Worksheet.Cell(row, "H").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "H").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "H").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "I").Value = data.TuNgay.ToString("MM/dd/yyyy");
                        Worksheet.Cell(row, "I").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "I").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "I").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "J").Value = data.DenNgay.ToString("MM/dd/yyyy");
                        Worksheet.Cell(row, "J").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "J").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "J").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "K").Value = data.NgayXN.ToString("MM/dd/yyyy");
                        Worksheet.Cell(row, "K").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "K").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "K").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "L").Value = data.MaNV1;
                        Worksheet.Cell(row, "L").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "L").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "L").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "M").Value = data.HoTen1;
                        Worksheet.Cell(row, "M").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "M").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "M").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "N").Value = data.MaNV2;
                        Worksheet.Cell(row, "N").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "N").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "N").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "O").Value = data.HoTen2;
                        Worksheet.Cell(row, "O").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "O").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "O").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "P").Value = data.GVKetLuanString;
                        Worksheet.Cell(row, "P").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "P").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "P").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;


                        row = row + 1;
                    }

                    Workbook.SaveAs(fileNamemaunew);
                    byte[] fileBytes = System.IO.File.ReadAllBytes(fileNamemaunew);
                    string fileName = "DS_HocVienKCCD - " + DateTime.Now.Date.ToString("dd/MM/yyyy") + ".xlsx";
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
                }
                else
                {

                    //Worksheet.Cell("A1").Value = "Ngày xuất file: " + DateTime.Now.Date.ToString("dd/MM/yyyy");
                    Workbook.SaveAs(fileNamemaunew);
                    byte[] fileBytes = System.IO.File.ReadAllBytes(fileNamemaunew);
                    string fileName = "DS_HocVienKCCD - " + DateTime.Now.Date.ToString("dd/MM/yyyy") + ".xlsx";
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
                }

            }
            catch (Exception ex)
            {
                TempData["msg"] = "<script>alert('" + ex + "');window.location.href = '/Everyday/'</script>";
                return View(TempData);
            }
        }

        public List<ConfirmKCCDExport> ListPhieuXN(DateTime TuNgay, DateTime DenNgay)
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);

            var begindd = TuNgay.Date + new TimeSpan(0, 00, 00);
            var endd = DenNgay.Date + new TimeSpan(23, 59, 59);

            //var sugg = dbKCCCD.DeNghiKCCD_select("").ToList();
            var phieuxn = dbKCCCD.PhieuXacNhanKCCDs.ToList();
            var nhanvien = db.NhanViens.ToList();
            //var phongban = db.PhongBans.ToList();

            var sugg = new SuggestKCCDController().ListSuggest("").ToList();

            var res = (from a in phieuxn.Where(x => x.IDTinhTrang == 2 && x.HVNgayXacNhan >= begindd && x.HVNgayXacNhan <= endd)
                       join b in nhanvien on a.HocVienID equals b.ID
                       join e in sugg on a.DeNghiDTID equals e.ID
                       select new ConfirmKCCDExport
                       {
                           ID = a.ID,
                           DeNghiDTID = a.DeNghiDTID,
                           NoiDungDT = e.TenNDDT,
                           NoiDungID = e.NoiDungKCCDID,
                           TenPhongBan = db.NhanViens.Where(x=>x.ID ==a.HocVienID).FirstOrDefault().PhongBan.TenPhongBan,
                           ThoiLuongDT = ((Convert.ToDateTime(e.DenNgay) - Convert.ToDateTime(e.TuNgay))).Days +1,
                           TenLVDT = e.TenLVDT,
                           TuNgay = (DateTime)e.TuNgay,
                           DenNgay = (DateTime)e.DenNgay,
                           NgayXN = (DateTime)e.NgayXN,
                           HocVienID = a.HocVienID,
                           HoTenHV = b.HoTen,
                           MaNV = b.MaNV,
                           MaNV1 = e.MaNV1,
                           HoTen1 = e.HoTen1,
                           GVKetLuanString = a.GVKetLuan ==1?"Đạt":"Không đạt",
                           IDTinhTrang = a.IDTinhTrang,
                           HuongDan1 =e.HuongDan1,
                           PhongBanID =e.PhongBanID,
                           HoTen2 = e.HoTen2,
                           MaNV2  = e.MaNV2,
                       }).ToList();
            if (ListQuyen.Contains(CONSTKEY.V_BP)) res = res.Where(x => x.PhongBanID == MyAuthentication.IDPhongban).ToList();
            else if (ListQuyen.Contains(CONSTKEY.VIEW_ALL)) res.ToList();
            else res = res.Where(x => x.HuongDan1 == MyAuthentication.ID).ToList();

            return res;
        }

        public int CheckApprove()
        {
            var a = dbKCCCD.DeNghiKCCDs.Where(x=>x.HuongDan2 == MyAuthentication.ID).ToList();
            if (a.Count == 0 )
            {
                return 0;
            }
            return 1;
        }

        public int CheckHVXN(int? sugID)
        {
            var a = dbKCCCD.PhieuXacNhanKCCDs.Where(x => x.DeNghiDTID == sugID && x.IDTinhTrang != 0).ToList();
            if (a.Count == 0)
            {
                return 0;
            }
            return 1;
        }

        public List<SuggestKCCDView> ListSuggest(string search)
        {
            if(search == null) search = "";

            //var phongban = db.PhongBans.ToList();
            //var nhanvien = db.NhanViens.ToList();
            //var vitri = db.Vitris.ToList();
            //var lvdt = db.LinhVucDTs.ToList();

            var res = (from a in dbKCCCD.DeNghiKCCD_select(search)
                       //join b in phongban on a.PhongBanID equals b.IDPhongBan
                       //join c in lvdt on a.LinhVucID equals c.IDLVDT
                       select new SuggestKCCDView
                       {
                           ID = a.ID,
                           TenNDDT = a.TenND,
                           NoiDungKCCDID = a.NoiDungKCCDID,
                           LinhVucID = a.LinhVucID,
                           TenLVDT = a.TenLVDT,
                           NhomNangLucID = a.NhomNangLucID,
                           TenNhomNL = a.TenNhomNL,
                           PhongBanID = a.PhongBanID,
                           TenPhongBan = a.TenPhongBan,
                           NgayTao = (DateTime)a.NgayTao,
                           TuNgay = (DateTime)a.TuNgay,
                           DenNgay = (DateTime)a.DenNgay,
                           NgayXN = a.NgayXN ?? DateTime.Now,
                           HoTen1 = a.HoTen1,
                           ViTriID1 = a.ViTriID1,
                           TenViTri1 = a.VT1,
                           MaNV1 = a.MaNV1,
                           HoTen2 = a.HoTen2,
                           ViTriID2 = a.ViTriID2,
                           TenViTri2 = a.VT2,
                           MaNV2 = a.MaNV2,
                           HuongDan1 = a.HuongDan1,
                           HuongDan2 = a.HuongDan2,
                           TinhTrang = a.TinhTrang,
                           DeThiID = a.DeThiID,
                           isKiemTra= a.isKiemTra ==1?true:false,
                       }).ToList();
            return res;
        }
        public JsonResult GetNoiDungDT(int id)
        {
            dbKCCCD.Configuration.ProxyCreationEnabled = false;
            //var gcl = dbKCCCD.NoiDungDTKCCD_selectKCCD("").Where(x=>x.ID == id).FirstOrDefault();
            var phongban = db.PhongBans.ToList();
            var lvdt = db.LinhVucDTs.ToList();

            var res = (from a in dbKCCCD.NoiDungDTKCCD_selectKCCD("").Where(x => x.ID == id)
                       join b in phongban on a.PhongBanID equals b.IDPhongBan
                       join c in lvdt on a.LVDTID equals c.IDLVDT
                       select new NoiDungDTKCCDView
                       {
                           ID = a.ID,
                           LVDTID = (int)a.LVDTID,
                           TenLVDT = c.TenLVDT,
                           NhomNLID = (int)a.NhomNLID,
                           TenNhomNL = a.NoiDung,
                           PhongBanID = (int)a.PhongBanID,
                           TenPhongBan = b.TenPhongBan,
                           NgayTao = (DateTime)a.NgayTao,
                           TenND = a.TenND
                       }).FirstOrDefault();
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDeThi(int id)
        {
            //var res = db.KCCD_DeThi.Where(x => x.KCCDID == id).ToList();
            List<ManageTestExamValidation> DeThiList = (from d in db.KCCD_DeThi.Where(x => x.KCCDID == id)
                                                        select new ManageTestExamValidation()
                                                        {
                                                            IDDeThi = d.ID,
                                                            TenDe = d.TenDe,
                                                            MaDe =d.MaDe,
                                                        }).ToList();
            return Json(DeThiList, JsonRequestBehavior.AllowGet);
        }

    }
}
