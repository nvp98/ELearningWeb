using ClosedXML.Excel;
using E_Learning.Models;
using E_Learning.ModelsQTUX;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_Learning.Controllers.QTUX
{
    public class ResultQTUXController : Controller
    {
        ELEARNINGEntities db = new ELEARNINGEntities();
        int Idquyen = MyAuthentication.IDQuyen;
        String ControllerName = "ResultQTUX";

        // GET: ResultQTUX - Xem kết quả
        public ActionResult Index(int? page, string search, int? IDND, int? IDNV, int? IDPB)
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

            var res = (from a in db.NhanViens.Where(x => x.IDTinhTrangLV == 1)
                       join d in db.PhongBans on a.IDPhongBan equals d.IDPhongBan
                       join b in db.Vitris on a.IDViTri equals b.IDViTri
                       select new ResultQTUXView
                       {
                           IDNV = a.ID,
                           HoTen = a.HoTen,
                           IDPhongBan = d.IDPhongBan,
                           PhongBan = d.TenPhongBan,
                           MaNV = a.MaNV,
                           TenViTri = b.TenViTri,
                           ListKQuaQTUX = (from aa in db.NoiDungDTs.Where(x => x.isNQ == 2)
                                           join k in db.QTUX_KetQua.Where(x => x.NhanVien.ID == a.ID) on aa.IDND equals k.NoiDungDT.IDND into ul
                                           from k in ul.DefaultIfEmpty()
                                           select new KQuaQTUX
                                           {
                                               IDChuong = aa.isOrder,
                                               NDDTID = aa.IDND,
                                               TinhTrang = k.TinhTrang,
                                               NgayHT = k.NgayHT,
                                               NgayTG = k.NgayTG,
                                               XNHT = k.XNHT,
                                               XNHTFile = k.XNHTFile,
                                               XNTG = k.XNTG,
                                               // Thêm kết quả bài thi
                                               SoLanThi = db.BaiThis.Where(x => x.IDND == aa.IDND && x.IDNV == a.ID).Count(),
                                               DiemCaoNhat = db.BaiThis.Where(x => x.IDND == aa.IDND && x.IDNV == a.ID && x.TinhTrang == true).Max(x => x.DiemSo),
                                               TinhTrangThi = db.BaiThis.Where(x => x.IDND == aa.IDND && x.IDNV == a.ID && x.TinhTrang == true).Count(),
                                               NgayThiCuoi = db.BaiThis.Where(x => x.IDND == aa.IDND && x.IDNV == a.ID).Max(x => x.NgayThi),
                                           }).OrderBy(x => x.IDChuong).ToList(),
                       }).OrderBy(x => x.IDPhongBan).ToList();

            List<NoiDungDT> ctlvdt = db.NoiDungDTs.Where(x => x.isNQ == 2).OrderBy(x => x.isOrder).ToList();
            ViewBag.IDQTUX = new SelectList(ctlvdt, "IDND", "NoiDung");

            List<PhongBan> phongban = db.PhongBans.ToList();
            ViewBag.IDPB = new SelectList(phongban, "IDPhongBan", "TenPhongBan");

            List<NhanVien> nhanvien = db.NhanViens.Where(x => x.IDTinhTrangLV == 1).ToList();
            var nv3 = nhanvien.Select(x => new EmployeeValidation { ID = x.ID, HoTen = x.MaNV + " - " + x.HoTen }).ToList();
            ViewBag.LisNV = new SelectList(nv3, "ID", "HoTen");

            if (IDNV != 0)
            {
                res = res.Where(x => x.IDNV == IDNV).ToList();
                ViewBag.LisNV = new SelectList(nv3, "ID", "HoTen", IDNV);
            }
            if (IDPB != 0) { ViewBag.IDPB = new SelectList(phongban, "IDPhongBan", "TenPhongBan", IDPB); res = res.Where(x => x.IDPhongBan == IDPB).ToList(); }
            if (search != "") res = res.Where(x => x.MaNV == search).ToList();

            List<string> columnHeaders = new List<string> { };
            var listND = db.NoiDungDTs.Where(x => x.isNQ == 2).OrderBy(x => x.isOrder).ToList();
            foreach (var item in listND)
            {
                columnHeaders.Add("Chương " + item.isOrder);
            }
            ViewBag.ColumHeader = columnHeaders;

            if (page == null) page = 1;
            int pageSize = 50;
            int pageNumber = (page ?? 1);
            return View(res.ToList().ToPagedList(pageNumber, pageSize));
        }

        public ActionResult ResultView(int? page, string search, int? IDND, int? IDNV)
        {
            var res = (from a in db.NoiDungDTs.Where(x => x.isNQ == 2)
                       join b in db.QTUX_KetQua.Where(x => x.NhanVien.ID == IDNV) on a.IDND equals b.NoiDungDT.IDND into ul
                       from b in ul.DefaultIfEmpty()
                       select new NoiQuyUXKQView
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
                           // Thêm kết quả bài thi
                           LanThi = db.BaiThis.Where(x => x.IDND == a.IDND && x.IDNV == IDNV).Count(),
                           DiemSo = db.BaiThis.Where(x => x.IDND == a.IDND && x.IDNV == IDNV && x.TinhTrang == true).Max(x => x.DiemSo),
                           TinhTrangThi = db.BaiThis.Where(x => x.IDND == a.IDND && x.IDNV == IDNV && x.TinhTrang == true).Count(),
                           NgayThi = db.BaiThis.Where(x => x.IDND == a.IDND && x.IDNV == IDNV).Max(x => x.NgayThi),
                       }).OrderBy(x => x.isOrder).ToList();

            if (page == null) page = 1;
            int pageSize = 50;
            int pageNumber = (page ?? 1);
            return View(res.ToList().ToPagedList(pageNumber, pageSize));
        }
    }
}
