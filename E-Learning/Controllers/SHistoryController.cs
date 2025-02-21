using PagedList;
using E_Learning.Models;
using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using E_Learning.ModelsKCCD;

namespace E_Learning.Controllers
{
    public class SHistoryController : Controller
    {
        ELEARNINGEntities db_context = new ELEARNINGEntities();
        ELEARNINGEntities dbKCCCD = new ELEARNINGEntities();
        // GET: StudyHistory
        public ActionResult Index( int? page)
        {
            if (User.Identity.IsAuthenticated)
            {
               
                int id = MyAuthentication.ID;
                var res = (from h in db_context.XNHocTaps
                           join l in db_context.LopHocs on h.LHID equals l.IDLH
                           join n in db_context.NhanViens.Where(x => x.ID == id) on h.NVID equals n.ID
                           join p in db_context.PhongBans on h.PBID equals p.IDPhongBan
                           join v in db_context.Vitris on h.VTID equals v.IDViTri
                           select new ConfirmEStudyValidation
                           {
                               IDHT = h.IDHT,
                               PBID = (int)h.PBID,
                               TenPB = p.TenPhongBan,
                               NVID = n.ID,
                               MaNV = n.MaNV,
                               HoTenHV = n.HoTen,
                               VTID = (int)h.VTID,
                               TenVT = v.TenViTri,
                               LHID = l.IDLH,                               
                               TenLH = l.TenLH,
                               TenND = l.NoiDungDT.NoiDung,
                               LinhVuc = l.NoiDungDT.LinhVucDT.TenLVDT,
                               TGBDLH = (DateTime)l.TGBDLH,
                               TGKTLH = (DateTime)l.TGKTLH,
                               NgayTG = (DateTime)h.NgayTG,
                               NgayHT = (DateTime)h.NgayHT,
                               XNTG = (bool)h.XNTG,
                               XNHT = (bool)h.XNHT,
                               PPDaoTao = "Mở lớp tập trung (E-learning)"
                           }).OrderByDescending(x => x.TGBDLH).ToList();

                //var sugg = dbKCCCD.DeNghiKCCD_select("").ToList();

                var phongban = db_context.PhongBans.ToList();
                var nhanvien = db_context.NhanViens.ToList();
                var vitri = db_context.Vitris.ToList();
                var lvdt = db_context.LinhVucDTs.ToList();

                var sugg = (from a in dbKCCCD.DeNghiKCCD_selectKCCD("")
                           join b in phongban on a.PhongBanID equals b.IDPhongBan
                           join c in lvdt on a.LinhVucID equals c.IDLVDT
                           select new SuggestKCCDView
                           {
                               ID = a.ID,
                               TenNDDT = a.TenND,
                               NoiDungKCCDID = a.NoiDungKCCDID,
                               LinhVucID = a.LinhVucID,
                               TenLVDT = c.TenLVDT,
                               NhomNangLucID = a.NhomNangLucID,
                               TenNhomNL = a.TenNhomNL,
                               PhongBanID = a.PhongBanID,
                               TenPhongBan = b.TenPhongBan,
                               NgayTao = (DateTime)a.NgayTao,
                               TuNgay = (DateTime)a.TuNgay,
                               DenNgay = (DateTime)a.DenNgay,
                               NgayXN = a.NgayXN ?? DateTime.Now,
                               HoTen1 = nhanvien.Where(x => x.ID == a.HuongDan1).FirstOrDefault().HoTen,
                               ViTriID1 = a.ViTriID1,
                               TenViTri1 = vitri.Where(x => x.IDViTri == a.ViTriID1).FirstOrDefault().TenViTri,
                               MaNV1 = nhanvien.Where(x => x.ID == a.HuongDan1).FirstOrDefault().MaNV,
                               HoTen2 = nhanvien.Where(x => x.ID == a.HuongDan2).FirstOrDefault().HoTen,
                               ViTriID2 = a.ViTriID2,
                               TenViTri2 = vitri.Where(x => x.IDViTri == a.ViTriID2).FirstOrDefault().TenViTri,
                               MaNV2 = nhanvien.Where(x => x.ID == a.HuongDan2).FirstOrDefault().MaNV,
                               HuongDan1 = a.HuongDan1,
                               HuongDan2 = a.HuongDan2,
                               TinhTrang = a.TinhTrang,
                           }).ToList();

                var phieuxn = dbKCCCD.PhieuXacNhanKCCDs.ToList();
                //var nhanvien = db_context.NhanViens.Where(x => x.IDTinhTrangLV == 1).ToList();
                //var vitri = db_context.Vitris.ToList();
                //var phongban = db_context.PhongBans.ToList();

                var res1 = (from a in phieuxn.Where(x => x.HocVienID == id && x.IDTinhTrang == 2)
                           join b in nhanvien on a.HocVienID equals b.ID
                           join c in vitri on b.IDViTri equals c.IDViTri
                           join d in phongban on b.IDPhongBan equals d.IDPhongBan
                           join e in sugg on a.DeNghiDTID equals e.ID
                           select new ConfirmEStudyValidation
                           {
                               //IDHT = h.IDHT,
                               //PBID = (int)h.PBID,
                               TenPB = d.TenPhongBan,
                               NVID = b.ID,
                               MaNV = b.MaNV,
                               HoTenHV = b.HoTen,
                               //VTID = (int)h.VTID,
                               TenVT = c.TenViTri,
                               //LHID = l.IDLH,
                               //TenLH = e.TenND,
                               TenND = e.TenNDDT,
                               LinhVuc = e.TenLVDT,
                               TGBDLH = (DateTime)e.TuNgay,
                               TGKTLH = (DateTime)e.DenNgay,
                               NgayTG = (DateTime)e.TuNgay,
                               NgayHT = (DateTime)e.NgayXN,
                               XNTG = e.TinhTrang == 2 ? true : false,
                               XNHT = e.TinhTrang ==2? true:false,
                               PPDaoTao = "Kèm cặp, chỉ dẫn",
                               IDPPDaoTao = 1
                           }).ToList();

                res.AddRange(res1);

                if (page == null) page = 1;
                int pageSize = 20;
                int pageNumber = (page ?? 1);
                return PartialView(res.ToList().ToPagedList(pageNumber, pageSize));
            }
            else
            {
                return RedirectToAction("", "Login");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db_context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}