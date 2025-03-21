using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using E_Learning.Common;
using E_Learning.Models;
using E_Learning.ModelsDTTH;
using iTextSharp.text;
using PagedList;

namespace E_Learning.Controllers.DaoTaoTH
{
    public class PheDuyetPhieuController : Controller
    {
        private ELEARNINGEntities db = new ELEARNINGEntities();
        int Idquyen = MyAuthentication.IDQuyen;
        String ControllerName = "PheDuyetPhieu";

        // GET: PheDuyetPhieu
        public ActionResult Index(int? LoaiKyDuyet)
        {
            return View(db.SH_KyDuyetNCDT.ToList());
        }
        public ActionResult Index_NCDT(int? page)
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            ViewBag.QUYENCN = ListQuyen;
            //if (!ListQuyen.Contains(CONSTKEY.V))
            //{
            //    TempData["msgError"] = "<script>alert('Bạn không có quyền truy cập chức năng này');</script>";
            //    return RedirectToAction("", "Home");
            //}
            // check chữ ký
            var nhanvien = db.NhanViens.Where(x => x.ID == MyAuthentication.ID).FirstOrDefault();
            if (string.IsNullOrEmpty(nhanvien.ChuKy))
            {
                TempData["msgSuccess"] = "<script>alert('Chưa có chữ ký vui lòng cập nhật chữ ký ');</script>";
                return new RedirectResult("~/Login/CapNhatChuKy");
            }

            var valuesToCheck = db.SH_NhuCauDT.Where(x=>x.TinhTrang != 0).Select(k => k.ID).ToList();
            var res = (from a in db.SH_KyDuyetNCDT.Where(x => x.NguoiDuyet_ID == MyAuthentication.ID && valuesToCheck.Contains((int)x.NCDT_ID))
                       select new SH_KyDuyetNCDTView
                       {
                           ID = a.ID,
                           CapDuyet = a.CapDuyet,
                           NguoiDuyet_ID = a.NguoiDuyet_ID,
                           GhiChu = a.GhiChu,
                           NCDT_ID = a.NCDT_ID,
                           NgayDuyet = a.NgayDuyet,
                           TinhTrangDuyet = a.TinhTrangDuyet,
                       }).ToList();
            foreach (var item in res)
            {
                var kyduyet = db.SH_KyDuyetNCDT.Where(x => x.NCDT_ID == item.NCDT_ID).ToList();
                if (kyduyet.Count() > 1) // > 1 cấp duyệt 
                {
                    var checkky = kyduyet.Where(x => x.CapDuyet < item.CapDuyet).ToList(); // check các cấp duyệt nhỏ hơn
                    if (checkky.Count() != 0 )
                    {
                        if(checkky.Where(x=>x.TinhTrangDuyet == 0 || x.TinhTrangDuyet == 2).Count() != 0) // nếu chưa duyệt hoặc hủy thì ẩn
                        {
                            res = res.Where(x=>x.ID != item.ID).ToList();
                        }

                    }
                }
                   
               
            }
            if (page == null) page = 1;
            int pageSize = 100;
            int pageNumber = (page ?? 1);
            return View(res.ToList().ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Index_CTDT(int? page)
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            ViewBag.QUYENCN = ListQuyen;
            //if (!ListQuyen.Contains(CONSTKEY.V))
            //{
            //    TempData["msgError"] = "<script>alert('Bạn không có quyền truy cập chức năng này');</script>";
            //    return RedirectToAction("", "Home");
            //}
            // check chữ ký
            var nhanvien = db.NhanViens.Where(x => x.ID == MyAuthentication.ID).FirstOrDefault();
            if (string.IsNullOrEmpty(nhanvien.ChuKy))
            {
                TempData["msgSuccess"] = "<script>alert('Chưa có chữ ký vui lòng cập nhật chữ ký ');</script>";
                return new RedirectResult("~/Login/CapNhatChuKy");
            }

            var valuesToCheck = db.SH_ChuongTrinhDT.Where(x => (x.TinhTrang == 2 || x.TinhTrang == 1 || x.TinhTrang ==3 )).Select(k => k.IDCTDT).ToList();
            var res = (from a in db.SH_KyDuyetCTDT.Where(x => (x.ID_NguoiKiemTra == MyAuthentication.ID ||x.ID_TPBP == MyAuthentication.ID || x.ID_PCHN == MyAuthentication.ID) && valuesToCheck.Contains((int)x.ID_CTDT) && (x.ID_NguoiDuyetNDDT == null||(x.NgayDuyetNDDT != null && x.ID_NguoiDuyetNDDT != null)))
                       select new SH_KyDuyetCTDTView
                       {
                           ID = a.ID,
                           ID_NguoiTao = a.ID_NguoiTao,
                           NgayTao = a.NgayTao,
                           ID_NguoiKiemTra = a.ID_NguoiKiemTra,
                           NgayKTDuyet = a.NgayKTDuyet,
                           ID_TPBP = a.ID_TPBP,
                           NgayTPBP = a.NgayTPBP,
                           ID_PCHN = a.ID_PCHN,
                           NgayPCHN = a.NgayPCHN,
                           ID_NguoiDuyetNDDT = a.ID_NguoiDuyetNDDT,
                           NgayDuyetNDDT= a.NgayDuyetNDDT,
                           ID_CTDT = a.ID_CTDT,
                           NoiDungCTDT = db.SH_ChuongTrinhDT.Where(x=>x.IDCTDT == a.ID_CTDT).FirstOrDefault().TenChuongTrinhDT,
                           NoiDungTrichYeu = db.SH_ChuongTrinhDT.Where(x => x.IDCTDT == a.ID_CTDT).FirstOrDefault().NoiDungTrichYeu,
                           ID_TinhTrangCTDT = db.SH_ChuongTrinhDT.Where(x=>x.IDCTDT == a.ID_CTDT).FirstOrDefault().TinhTrang,
                           IsDuyet =0
                       }).OrderByDescending(x => x.NgayTao).ToList();
            int IDNV = MyAuthentication.ID;
            foreach (var item in res)
            {
                if (item.ID_NguoiKiemTra == IDNV)
                {
                    var nv = db.NhanViens.Select(x => new { x.ID, x.HoTen, x.MaNV }).ToList();
                    item.TenNguoiTao = db.NhanViens.Where(x => x.ID == item.ID_NguoiTao).FirstOrDefault().HoTen;
                    item.TenNguoiKiemTra = nv.Where(x => x.ID == item.ID_NguoiKiemTra).FirstOrDefault().MaNV + "-" + nv.Where(x => x.ID == item.ID_NguoiKiemTra).FirstOrDefault().HoTen;
                    item.TenTPBP = nv.Where(x => x.ID == item.ID_TPBP).FirstOrDefault()?.MaNV + "-" + nv.Where(x => x.ID == item.ID_TPBP).FirstOrDefault()?.HoTen;
                    item.TenPCHN = nv.Where(x => x.ID == item.ID_PCHN).FirstOrDefault()?.MaNV + "-" + nv.Where(x => x.ID == item.ID_PCHN).FirstOrDefault()?.HoTen;
                    if (item.NgayKTDuyet != null)
                    {
                        item.IsDuyet = 1;
                    }
                    else item.IsDuyet = 0;
                }
                else if (item.ID_TPBP == IDNV && item.NgayKTDuyet != null)
                {
                    var nv = db.NhanViens.Select(x => new { x.ID, x.HoTen,x.MaNV }).ToList();
                    item.TenNguoiTao = nv.Where(x => x.ID == item.ID_NguoiTao).FirstOrDefault().MaNV +"-" + nv.Where(x => x.ID == item.ID_NguoiTao).FirstOrDefault().HoTen;
                    item.TenNguoiKiemTra = nv.Where(x => x.ID == item.ID_NguoiKiemTra).FirstOrDefault().MaNV + "-" + nv.Where(x => x.ID == item.ID_NguoiKiemTra).FirstOrDefault().HoTen;
                    item.TenTPBP = nv.Where(x => x.ID == item.ID_TPBP).FirstOrDefault()?.MaNV + "-" + nv.Where(x => x.ID == item.ID_TPBP).FirstOrDefault()?.HoTen;
                    item.TenPCHN = nv.Where(x => x.ID == item.ID_PCHN).FirstOrDefault()?.MaNV + "-" + nv.Where(x => x.ID == item.ID_PCHN).FirstOrDefault()?.HoTen;
                    item.IsDuyet = 1;
                    if (item.NgayTPBP != null)
                    {
                        item.IsDuyet = 1;
                    }
                    else item.IsDuyet = 0;
                }
                else if (item.ID_PCHN == IDNV && (item.NgayKTDuyet != null && item.NgayTPBP != null))
                {
                    var nv = db.NhanViens.Select(x => new { x.ID, x.HoTen, x.MaNV }).ToList();
                    item.TenNguoiTao = nv.Where(x => x.ID == item.ID_NguoiTao).FirstOrDefault().MaNV + "-" + nv.Where(x => x.ID == item.ID_NguoiTao).FirstOrDefault().HoTen;
                    item.TenNguoiKiemTra = nv.Where(x => x.ID == item.ID_NguoiKiemTra).FirstOrDefault().MaNV + "-" + nv.Where(x => x.ID == item.ID_NguoiKiemTra).FirstOrDefault().HoTen;
                    item.TenTPBP = nv.Where(x => x.ID == item.ID_TPBP).FirstOrDefault()?.MaNV + "-" + nv.Where(x => x.ID == item.ID_TPBP).FirstOrDefault()?.HoTen;
                    item.TenPCHN = nv.Where(x => x.ID == item.ID_PCHN).FirstOrDefault()?.MaNV + "-" + nv.Where(x => x.ID == item.ID_PCHN).FirstOrDefault()?.HoTen;
                    if (item.NgayPCHN != null)
                    {
                        item.IsDuyet = 1;
                    }
                    else item.IsDuyet = 0;
                }
                else
                {
                    res = res.Where(x=>x.ID != item.ID).ToList();
                };
            }
            if (page == null) page = 1;
            int pageSize = 100;
            int pageNumber = (page ?? 1);
            return View(res.ToList().ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Index_NDDT(int? page)
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            ViewBag.QUYENCN = ListQuyen;
            //if (!ListQuyen.Contains(CONSTKEY.V))
            //{
            //    TempData["msgError"] = "<script>alert('Bạn không có quyền truy cập chức năng này');</script>";
            //    return RedirectToAction("", "Home");
            //}
            // check chữ ký
            var nhanvien = db.NhanViens.Where(x => x.ID == MyAuthentication.ID).FirstOrDefault();
            if (string.IsNullOrEmpty(nhanvien.ChuKy))
            {
                TempData["msgSuccess"] = "<script>alert('Chưa có chữ ký vui lòng cập nhật chữ ký ');</script>";
                return new RedirectResult("~/Login/CapNhatChuKy");
            }

            var valuesToCheck = db.SH_ChuongTrinhDT.Where(x => x.NoiDungDT.IsDelete == true && x.TinhTrang ==2).Select(k => k.IDCTDT).ToList();
            var res = (from a in db.SH_KyDuyetCTDT.Where(x => ((x.ID_NguoiDuyetNDDT == MyAuthentication.ID && x.NgayDangNDDT != null) ||(x.ID_NguoiTao == MyAuthentication.ID &&x.NgayDuyetNDDT != null) || x.ID_NguoiDangNDDT == MyAuthentication.ID) && valuesToCheck.Contains((int)x.ID_CTDT))
                       join b in db.SH_ChuongTrinhDT on a.ID_CTDT equals b.IDCTDT
                       join c in db.NoiDungDTs on b.ID_NoiDungDT equals c.IDND
                       select new SH_KyDuyetCTDTView
                       {
                           ID = a.ID,
                           ID_NguoiTao = a.ID_NguoiTao,
                           NgayTao = a.NgayTao,
                           TenNguoiTao = db.NhanViens.Where(x=>x.ID == a.ID_NguoiTao).Select(x => x.HoTen).FirstOrDefault(),
                           ID_NguoiKiemTra = a.ID_NguoiKiemTra,
                           NgayKTDuyet = a.NgayKTDuyet,
                           ID_TPBP = a.ID_TPBP,
                           NgayTPBP = a.NgayTPBP,
                           ID_PCHN = a.ID_PCHN,
                           NgayPCHN = a.NgayPCHN,
                           ID_NguoiDuyetNDDT = a.ID_NguoiDuyetNDDT,
                           TenNguoiDuyetNDDT = db.NhanViens.Where(x => x.ID == a.ID_NguoiDuyetNDDT).Select(x=>x.HoTen).FirstOrDefault(),
                           NgayDuyetNDDT = a.NgayDuyetNDDT,
                           ID_CTDT = a.ID_CTDT,
                           NoiDungCTDT = b.TenChuongTrinhDT,
                           NoiDungTrichYeu = b.NoiDungTrichYeu,
                           ID_TinhTrangCTDT = b.TinhTrang,
                           noidungdt = c,
                           IsDuyet = 0,
                           ID_NguoiDangNDDT = a.ID_NguoiDangNDDT,
                           NgayDangNDDT = a.NgayDangNDDT,
                           TenNguoiDangNDDT = db.NhanViens.Where(x => x.ID == a.ID_NguoiDangNDDT).Select(x => x.HoTen).FirstOrDefault(),
                           CapDuyet = a.ID_NguoiDangNDDT == MyAuthentication.ID?1:2
                       }).OrderByDescending(x => x.NgayTao).ToList();
            int IDNV = MyAuthentication.ID;
            if (page == null) page = 1;
            int pageSize = 100;
            int pageNumber = (page ?? 1);
            return View(res.ToList().ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Index_TCDT(int? page)
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            ViewBag.QUYENCN = ListQuyen;
            //if (!ListQuyen.Contains(CONSTKEY.V))
            //{
            //    TempData["msgError"] = "<script>alert('Bạn không có quyền truy cập chức năng này');</script>";
            //    return RedirectToAction("", "Home");
            //}
            // check chữ ký
            var nhanvien = db.NhanViens.Where(x => x.ID == MyAuthentication.ID).FirstOrDefault();
            if (string.IsNullOrEmpty(nhanvien.ChuKy))
            {
                TempData["msgSuccess"] = "<script>alert('Chưa có chữ ký vui lòng cập nhật chữ ký ');</script>";
                return new RedirectResult("~/Login/CapNhatChuKy");
            }
            var res = (from a in db.LopHocs.Where(x => x.NguoiKiemTra_ID == MyAuthentication.ID && x.TinhTrang != 0)
                       join b in db.NhanViens on a.NguoiTao_ID equals b.ID
                       select new SH_KyDuyetNCDTView
                       {
                           ID = a.IDLH,
                           //CapDuyet = a.CapDuyet,
                           NguoiDuyet_ID = a.NguoiKiemTra_ID,
                           //GhiChu = a.GhiChu,
                           //NCDT_ID = a.NCDT_ID,
                           ID_Duyet = a.IDLH,
                           NgayDuyet = a.NgayKiemTra,
                           TinhTrangDuyet = a.NgayKiemTra != null?1:0,
                           HoTen_NguoiTao = b.MaNV +" - " + b.HoTen,
                           NgayTao = a.NgayTao
                       }).OrderByDescending(x => x.NgayTao).ToList();

            if (page == null) page = 1;
            int pageSize = 100;
            int pageNumber = (page ?? 1);
            return View(res.ToList().ToPagedList(pageNumber, pageSize));
        }

        public ActionResult XuLyPhieuNCDT(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SH_NhuCauDT nhuCauDT = db.SH_NhuCauDT.Find(id);
            if (nhuCauDT == null)
            {
                return HttpNotFound();
            }
            var IDPB = MyAuthentication.IDPhongban;
            ViewBag.IDPhanLoaiDT = new SelectList(db.SH_PhanLoaiNDDT, "ID", "TenPhanLoaiDT", nhuCauDT.PhanLoaiNCDT_ID);
            ViewBag.IDPhongBan = db.PhongBans.Where(x => x.IDPhongBan == IDPB).FirstOrDefault().TenPhongBan;
            //ViewBag.IDNoiDungDT = new SelectList(db.NoiDungDTs.Where(x=>x.IDND == nhuCauDT.NoiDungDT_ID), "IDND", "NoiDung",nhuCauDT.NoiDungDT_ID);
            ViewBag.IDNoiDungDT = new SelectList(db.NoiDungDTs.Where(x => x.IDND == nhuCauDT.NoiDungDT_ID).Select(x => new
            {
                Value = x.IDND,
                Text = x.IDND + " - " + db.SH_PhanLoaiNDDT.Where(a => x.IDND == nhuCauDT.NoiDungDT_ID).FirstOrDefault().TenPhanLoaiDT + " - " + x.NoiDung // Concatenate fields here
            }),
            "Value",
            "Text", nhuCauDT.NoiDungDT_ID);
            ViewBag.IDDKNhacLai = new SelectList(db.SH_DinhKy, "MaDK", "DKNhacLai", nhuCauDT.MaDinhKy);

            ViewBag.LoaiNCDT = nhuCauDT.PhanLoaiNCDT_ID;
            ViewBag.LoaiHinh_DT = db.SH_PhanLoaiNCDT.Where(x => x.IDLoai == nhuCauDT.PhanLoaiNCDT_ID).FirstOrDefault().LoaiHinhDT_ID;
            ViewBag.Nam = db.SH_QuyDaoTao.First().AD_Nam;
            ViewBag.Quy = db.SH_QuyDaoTao.First().AD_Quy;
            ViewBag.PhuongPhapDT_ID = new SelectList(db.SH_PhuongPhapDT.Where(x => x.ID == nhuCauDT.PhuongPhapDT_ID), "ID", "TenPhuongPhapDT", nhuCauDT.PhuongPhapDT_ID);
            var nv2 = db.NhanViens.Where(x => x.IDTinhTrangLV == 1 && x.IDPhongBan == IDPB).Select(x => new EmployeeValidation { ID = x.ID, HoTen = x.MaNV + " - " + x.HoTen + "-" + x.Vitri.TenViTri }).ToList();
            ViewBag.Selec = new SelectList(nv2, "ID", "HoTen", nhuCauDT.SH_ChiTiet_NCDT.FirstOrDefault().GiangVien_ID);

            var ls = new List<SH_ViTri_NDDTView>();

            var data = db.VitriKNL_search();
            var vt = db.SH_ViTri_NDDT.Where(x => x.NCDT_ID == null || x.NCDT_ID == nhuCauDT.ID).ToList();
            ls = (from a in vt.Where(x => x.NoiDungDT_ID == nhuCauDT.NoiDungDT_ID && x.PhuongPhapDT_ID == nhuCauDT.PhuongPhapDT_ID)
                  join b in data on a.Vitri_ID equals b.IDVT
                  select new SH_ViTri_NDDTView()
                  {
                      ID = a.ID,
                      TenViTri = b.IDVT + "-" + b.TenViTri + "-" + b.MaPB + "-" + b.TenPX + "-" + b.TenNhom + "-" + b.TenTo,
                      Tinhtrang = a.NCDT_ID == nhuCauDT.ID ? 1 : 0
                  }).ToList();
            ViewBag.DSVitri = ls;

            // trình ký
            var trinhky = db.SH_KyDuyetNCDT.Where(x => x.NCDT_ID == nhuCauDT.ID).ToList();
            var nhanvien = db.NhanViens.Where(x => x.IDTinhTrangLV == 1).Select(x => new EmployeeValidation { ID = x.ID, HoTen = x.MaNV + " - " + x.HoTen, IDPhongBan = (int)x.IDPhongBan }).ToList();

            ViewBag.BPSD = new SelectList(nhanvien.Where(x => x.IDPhongBan == MyAuthentication.IDPhongban), "ID", "HoTen", trinhky.Where(x => x.CapDuyet == 1).FirstOrDefault()?.NguoiDuyet_ID);
            ViewBag.PCHN = new SelectList(nhanvien, "ID", "HoTen", trinhky.Where(x => x.CapDuyet == 2).FirstOrDefault()?.NguoiDuyet_ID);
            var phongns = db.PhongBans.Where(x => x.MaPB.Contains("P.NS")).First();
            var bangiamdoc = db.PhongBans.Where(x => x.MaPB.Contains("BGD")).First();
            ViewBag.PNS = new SelectList(nhanvien.Where(x => x.IDPhongBan == phongns.IDPhongBan), "ID", "HoTen", trinhky.Where(x => x.CapDuyet == 3).FirstOrDefault()?.NguoiDuyet_ID);
            ViewBag.BGD = new SelectList(nhanvien.Where(x => x.IDPhongBan == bangiamdoc.IDPhongBan), "ID", "HoTen", trinhky.Where(x => x.CapDuyet == 4).FirstOrDefault()?.NguoiDuyet_ID);

            var noiDungDTs = (from a in db.SH_NhuCauDT.Where(x => x.ID == id)
                              join b in db.SH_ChiTiet_NCDT on a.ID equals b.NhuCauDT_ID
                              join c in db.PhongBans on a.BoPhanLNC_ID equals c.IDPhongBan into uli
                              from c in uli.DefaultIfEmpty()
                              join d in db.NhanViens.Where(x => x.IDTinhTrangLV == 1) on a.NguoiTao_ID equals d.ID
                              select new NhuCauDTTHView
                              {
                                  ID_NCDT = a.ID,
                                  NoiDungDT_ID = a.NoiDungDT_ID,
                                  TenNoiDungDT = a.NoiDungDT.NoiDung,
                                  Quy = a.Quy,
                                  Nam = a.Nam,
                                  BoPhanLNC_ID = a.BoPhanLNC_ID,
                                  TenBoPhan_LNC = c.TenPhongBan,
                                  NguoiTao = d.MaNV + "-" + d.HoTen,
                                  TinhTrang = a.TinhTrang,
                                  FileDinhKem = a.FileDinhKem,
                                  TrinhKy_ID = a.MaTrinhKy,
                                  PhuongPhapDT_ID = a.PhuongPhapDT_ID,
                                  NgayTao = (DateTime)a.NgayTao,
                                  chiTietNhuCauDTTHView = new ChiTietNhuCauDTTHView
                                  {
                                      SoLuongNguoi = b.SoLuongNguoi,
                                      DonViDT = b.DonViDT,
                                      DiaDiemDT = b.DiaDiemDT,
                                      ThoiGianDT = b.ThoiGian_DT,
                                      GhiChu = b.GhiChu,
                                      ThoiLuong = b.ThoiLuong_DT,
                                      DoiTuongDT = b.DoiTuongDT,
                                      TenViTri = b.GiangVien_Vitri,
                                      TenGiangVien = b.GiangVien_HoTen,
                                      Nhom_ID = a.NoiDungDT.IDNhomNL,
                                      GiangVien_ID = b.GiangVien_ID,
                                  },
                              }).OrderBy(x => x.ID_NCDT).FirstOrDefault();

            return View(noiDungDTs);
        }

        public ActionResult XuLyPhieuCTDT(int? id)
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
                                  IDPhuongPhapDT = a.IDPhuongPhapDT,
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
                                                            TongSoCau = d.TongSoCau,
                                                            fileDe = d.FileDeThi
                                                        }).ToList(),
                                  sH_KyDuyetCTDTs = db.SH_KyDuyetCTDT.Where(x => x.ID_CTDT == id).FirstOrDefault()
                              }).OrderBy(x => x.NgayTao).FirstOrDefault();

            return View(noiDungDTs);
        }
        public ActionResult XuLyNDDT(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SH_ChuongTrinhDT chuongtrinh = db.SH_ChuongTrinhDT.Where(x=>x.IDCTDT == id).FirstOrDefault();
            ViewBag.ID_ChuongtrinhDT = id;
            NoiDungDT noiDungDT = db.NoiDungDTs.Find(chuongtrinh.ID_NoiDungDT);
            if (noiDungDT == null)
            {
                return HttpNotFound();
            }
            //ViewBag.LVDTID = new SelectList(db.LinhVucDTs, "IDLVDT", "TenLVDT", noiDungDT.LVDTID);
            ViewBag.LVDTID = new SelectList(db.LinhVucDTs, "IDLVDT", "TenLVDT", noiDungDT.LVDTID);
            ViewBag.IDNguonGV = new SelectList(db.SH_NguonGV, "ID", "TenNguonGV", noiDungDT.IDNguonGV);
            ViewBag.IDPhanLoaiDT = new SelectList(db.SH_PhanLoaiNDDT, "ID", "TenPhanLoaiDT", noiDungDT.IDPhanLoaiDT);
            ViewBag.IDPPDT = new SelectList(db.SH_PhuongPhapDT, "ID", "TenPhuongPhapDT", noiDungDT.IDPhuongPhapDT);
            ViewBag.IDNhomNL = new SelectList(db.NhomNLKCCDs, "ID", "NoiDung", noiDungDT.IDNhomNL);
            ViewBag.IDHoatDongDT = new SelectList(db.SH_HoatDongDT, "ID", "TenHoatDong", noiDungDT.IDHoatDongDT);
            ViewBag.IDPhuongPhapDT = new SelectList(db.SH_PhuongPhapDT, "ID", "TenPhuongPhapDT", noiDungDT.IDPhuongPhapDT);
            return PartialView(noiDungDT);
        }

        // POST: NoiDungDTTH/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult XuLyNDDT(NoiDungDT noiDungDT,int? ID_CTDT, int? capduyet)
        {
            if (ModelState.IsValid)
            {
                var kyduyet = db.SH_KyDuyetCTDT.Where(x => x.ID_CTDT == ID_CTDT).FirstOrDefault();
                var existingRecord = db.NoiDungDTs.Find(noiDungDT.IDND); // Tìm bản ghi hiện có dựa vào khóa chính
                if (existingRecord != null)
                {
                    // Cập nhật từng trường nếu nó khác null
                    if (noiDungDT.NoiDung != null)
                        existingRecord.NoiDung = noiDungDT.NoiDung;

                    if (noiDungDT.VideoND != null)
                        existingRecord.VideoND = noiDungDT.VideoND;

                    if (noiDungDT.ImageND != null)
                        existingRecord.ImageND = noiDungDT.ImageND;
                    if (noiDungDT.FileDinhKem != null)
                        existingRecord.FileDinhKem = noiDungDT.FileDinhKem;
                    existingRecord.LVDTID = noiDungDT.LVDTID;
                    existingRecord.IDNguonGV = noiDungDT.IDNguonGV;
                    existingRecord.IDHoatDongDT = noiDungDT.IDHoatDongDT;
                    existingRecord.IDNhomNL = noiDungDT.IDNhomNL;
                    existingRecord.IDPhuongPhapDT = noiDungDT.IDPhuongPhapDT;
                    existingRecord.IDPhanLoaiDT = noiDungDT.IDPhanLoaiDT;
                    // Thêm logic cho các trường khác nếu cần
                    db.SaveChanges(); // Lưu thay đổi
                }
                if (kyduyet.ID_NguoiDuyetNDDT == MyAuthentication.ID)
                {
                    // xử lý xác nhận PNS xác nhận NDDT
                    kyduyet.NgayDuyetNDDT = DateTime.Now;
                    // Tác giả xác nhận nội dung
                    //existingRecord.IsDelete = false;
                    db.SaveChanges();
                }
                if(kyduyet.ID_NguoiDangNDDT == MyAuthentication.ID)
                {
                    kyduyet.NgayDangNDDT = DateTime.Now;
                    db.SaveChanges();
                }
                TempData["msgSuccess"] = "<script>alert('Thành công ');</script>";
                return RedirectToAction("Index_NDDT");
            }
            ViewBag.LVDTID = new SelectList(db.LinhVucDTs, "IDLVDT", "TenLVDT", noiDungDT.LVDTID);
            return RedirectToAction("Index_NDDT");
        }

        public ActionResult XacNhanNDDT(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var kyduyet = db.SH_KyDuyetCTDT.Where(x => x.ID_CTDT == id).FirstOrDefault();
            if (kyduyet.ID_NguoiDuyetNDDT == MyAuthentication.ID)
            {
                kyduyet.NgayDuyetNDDT = DateTime.Now;
                db.SaveChanges();
            }
            TempData["msgSuccess"] = "<script>alert('Thành công ');</script>";
            return RedirectToAction("Index_NDDT");
        }


        public ActionResult XacNhanPhieu(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            var sH_KyDuyetNCDT = db.SH_KyDuyetNCDT.Where(x=>x.NCDT_ID == id && x.NguoiDuyet_ID == MyAuthentication.ID).ToList();
            if (sH_KyDuyetNCDT == null)
            {
                return HttpNotFound();
            }
            if(sH_KyDuyetNCDT.Count() > 1)
            {
                foreach (var item in sH_KyDuyetNCDT)
                {
                    item.NgayDuyet = DateTime.Now;
                    item.TinhTrangDuyet = 1;
                }
                db.SaveChanges();
            }
            else
            {
                var aa = sH_KyDuyetNCDT.FirstOrDefault();
                aa.NgayDuyet = DateTime.Now;
                aa.TinhTrangDuyet = 1;
                db.SaveChanges();
            }
            // kiểm tra và update SH_NCDT
            var checkDuyet = db.SH_KyDuyetNCDT.Where(x => x.NCDT_ID == id && x.TinhTrangDuyet != 1).ToList();
            if(checkDuyet.Count == 0)
            {
                var ncdt = db.SH_NhuCauDT.Where(x=>x.ID == id).FirstOrDefault();
                ncdt.TinhTrang = 1;
                db.SaveChanges();
            }

            TempData["msgSuccess"] = "<script>alert('Xác nhận thành công ');</script>";
            return RedirectToAction("Index_NCDT", "PheDuyetPhieu");
        }

        public ActionResult KhongXacNhanPhieu(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var sH_KyDuyetNCDT = db.SH_KyDuyetNCDT.Where(x => x.NCDT_ID == id && x.NguoiDuyet_ID == MyAuthentication.ID).FirstOrDefault();
            if (sH_KyDuyetNCDT == null)
            {
                return HttpNotFound();
            }
            sH_KyDuyetNCDT.NgayDuyet = DateTime.Now;
            sH_KyDuyetNCDT.TinhTrangDuyet = 2; // hủy
            db.SaveChanges();
            // kiểm tra và update SH_NCDT về hủy
            var ncdt = db.SH_NhuCauDT.Where(x => x.ID == id).FirstOrDefault();
            if (ncdt != null)
            {
                ncdt.TinhTrang = 3; // hủy NCĐT
                db.SaveChanges();
            }
            TempData["msgSuccess"] = "<script>alert('Xác nhận thành công ');</script>";
            return RedirectToAction("Index_NCDT", "PheDuyetPhieu");
        }

        public ActionResult XacNhanPhieuCTDT(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            int IDNV = MyAuthentication.ID;
            var chuongtrinh = db.SH_ChuongTrinhDT.Where(x => x.IDCTDT == id).FirstOrDefault();
            var sH_KyDuyetCTDT = db.SH_KyDuyetCTDT.Where(x => x.ID_CTDT == chuongtrinh.IDCTDT).FirstOrDefault();
            var nddt = db.NoiDungDTs.FirstOrDefault(x => x.IDND == chuongtrinh.ID_NoiDungDT);
            if (sH_KyDuyetCTDT.ID_NguoiKiemTra == IDNV)
            {
                sH_KyDuyetCTDT.NgayKTDuyet = DateTime.Now;
                db.SaveChanges();
            }
            if (sH_KyDuyetCTDT.ID_TPBP == IDNV)
            {
                sH_KyDuyetCTDT.NgayTPBP = DateTime.Now;
                db.SaveChanges();
            }
            if (sH_KyDuyetCTDT.ID_PCHN == IDNV)
            {
                sH_KyDuyetCTDT.NgayPCHN = DateTime.Now;
                db.SaveChanges();
            }
            if ((sH_KyDuyetCTDT.NgayKTDuyet != null && sH_KyDuyetCTDT.ID_TPBP == null && sH_KyDuyetCTDT.ID_PCHN == null)
               || (sH_KyDuyetCTDT.NgayKTDuyet != null && sH_KyDuyetCTDT.ID_TPBP != null && sH_KyDuyetCTDT.ID_PCHN == null)
               || (sH_KyDuyetCTDT.NgayKTDuyet != null && sH_KyDuyetCTDT.ID_TPBP == null && sH_KyDuyetCTDT.ID_PCHN != null)
               || (sH_KyDuyetCTDT.NgayKTDuyet != null && sH_KyDuyetCTDT.ID_TPBP != null && sH_KyDuyetCTDT.ID_PCHN != null)
                ) // Hoàn thành
            {
                nddt.IsDelete = false;
                chuongtrinh.TinhTrang = 1;
                db.SaveChanges();
            }

            if ((sH_KyDuyetCTDT.NgayKTDuyet == null && sH_KyDuyetCTDT.ID_TPBP != null && sH_KyDuyetCTDT.ID_PCHN == null)
              || (sH_KyDuyetCTDT.NgayKTDuyet != null && sH_KyDuyetCTDT.ID_TPBP != null && sH_KyDuyetCTDT.ID_PCHN == null)
              || (sH_KyDuyetCTDT.NgayKTDuyet == null && sH_KyDuyetCTDT.ID_TPBP != null && sH_KyDuyetCTDT.ID_PCHN != null)
              || (sH_KyDuyetCTDT.NgayKTDuyet != null && sH_KyDuyetCTDT.ID_TPBP != null && sH_KyDuyetCTDT.ID_PCHN != null)
               ) // Hoàn thành
            {
                nddt.IsDelete = false;
                chuongtrinh.TinhTrang = 1;

                db.SaveChanges();
            }
            if ((sH_KyDuyetCTDT.NgayKTDuyet == null && sH_KyDuyetCTDT.ID_TPBP == null && sH_KyDuyetCTDT.ID_PCHN != null)
               || (sH_KyDuyetCTDT.NgayKTDuyet != null && sH_KyDuyetCTDT.ID_TPBP == null && sH_KyDuyetCTDT.ID_PCHN != null)
               || (sH_KyDuyetCTDT.NgayKTDuyet == null && sH_KyDuyetCTDT.ID_TPBP != null && sH_KyDuyetCTDT.ID_PCHN != null)
               || (sH_KyDuyetCTDT.NgayKTDuyet != null && sH_KyDuyetCTDT.ID_TPBP != null && sH_KyDuyetCTDT.ID_PCHN != null)
                ) // Hoàn thành
            {
                nddt.IsDelete = false;
                chuongtrinh.TinhTrang = 1;
                db.SaveChanges();
            }


            TempData["msgSuccess"] = "<script>alert('Xác nhận thành công ');</script>";
            return RedirectToAction("Index_CTDT", "PheDuyetPhieu");
        }


        public ActionResult KhongXacNhanPhieuCTDT(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            int IDNV = MyAuthentication.ID;
            var chuongtrinh = db.SH_ChuongTrinhDT.Where(x => x.IDCTDT == id).FirstOrDefault();
            var sH_KyDuyetCTDT = db.SH_KyDuyetCTDT.Where(x => x.ID == chuongtrinh.IDCTDT).FirstOrDefault();
            chuongtrinh.TinhTrang = 3;// không duyệt 
            db.SaveChanges();

            TempData["msgSuccess"] = "<script>alert('Xác nhận thành công ');</script>";
            return RedirectToAction("Index_CTDT", "PheDuyetPhieu");
        }

        public ActionResult XacNhanPhieuTCDT(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            int IDNV = MyAuthentication.ID;
            var lophoc = db.LopHocs.Where(x => x.IDLH == id).FirstOrDefault();
            lophoc.NgayKiemTra = DateTime.Now;
            lophoc.TinhTrang = 1;
            // set tinh trạng XNHT
            var listXNHT = db.XNHocTaps.Where(x=>x.LHID == id).ToList();
            foreach (var item in listXNHT)
            {
                item.TinhTrang = 1;
            }

            db.SaveChanges();

            TempData["msgSuccess"] = "<script>alert('Xác nhận thành công ');</script>";
            return RedirectToAction("Index_TCDT", "PheDuyetPhieu");
        }

        public ActionResult KhongXacNhanPhieuTCDT(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            int IDNV = MyAuthentication.ID;
            var lophoc = db.LopHocs.Where(x => x.IDLH == id).FirstOrDefault();
            lophoc.NgayKiemTra = DateTime.Now;
            lophoc.TinhTrang = 1;
            // set tinh trạng XNHT
            var listXNHT = db.XNHocTaps.Where(x => x.LHID == id).ToList();
            foreach (var item in listXNHT)
            {
                item.TinhTrang = 3; // từ chối
            }

            db.SaveChanges();

            TempData["msgSuccess"] = "<script>alert('Từ chối thành công ');</script>";
            return RedirectToAction("Index_TCDT", "PheDuyetPhieu");
        }

        // GET: PheDuyetPhieu/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SH_KyDuyetNCDT sH_KyDuyetNCDT = db.SH_KyDuyetNCDT.Find(id);
            if (sH_KyDuyetNCDT == null)
            {
                return HttpNotFound();
            }
            return View(sH_KyDuyetNCDT);
        }

        // GET: PheDuyetPhieu/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PheDuyetPhieu/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,NCDT_ID,NguoiDuyet_ID,CapDuyet,TinhTrangDuyet,NgayDuyet")] SH_KyDuyetNCDT sH_KyDuyetNCDT)
        {
            if (ModelState.IsValid)
            {
                db.SH_KyDuyetNCDT.Add(sH_KyDuyetNCDT);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(sH_KyDuyetNCDT);
        }

        // GET: PheDuyetPhieu/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SH_KyDuyetNCDT sH_KyDuyetNCDT = db.SH_KyDuyetNCDT.Find(id);
            if (sH_KyDuyetNCDT == null)
            {
                return HttpNotFound();
            }
            return View(sH_KyDuyetNCDT);
        }

        // POST: PheDuyetPhieu/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,NCDT_ID,NguoiDuyet_ID,CapDuyet,TinhTrangDuyet,NgayDuyet")] SH_KyDuyetNCDT sH_KyDuyetNCDT)
        {
            if (ModelState.IsValid)
            {
                //db.Entry(sH_KyDuyetNCDT).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(sH_KyDuyetNCDT);
        }

        [HttpPost]
        public ActionResult ProcessSelected(List<int> selectedItems)
        {
            if (selectedItems != null && selectedItems.Any())
            {

                //// Xử lý danh sách ID được chọn
                foreach (var id in selectedItems)
                {
                    var sH_KyDuyetNCDT = db.SH_KyDuyetNCDT.Where(x => x.NCDT_ID == id && x.NguoiDuyet_ID == MyAuthentication.ID).ToList();
                    if (sH_KyDuyetNCDT == null)
                    {
                        return HttpNotFound();
                    }
                    if (sH_KyDuyetNCDT.Count() > 1)
                    {
                        foreach (var item in sH_KyDuyetNCDT)
                        {
                            item.NgayDuyet = DateTime.Now;
                            item.TinhTrangDuyet = 1;
                        }
                        db.SaveChanges();
                    }
                    else
                    {
                        var aa = sH_KyDuyetNCDT.FirstOrDefault();
                        aa.NgayDuyet = DateTime.Now;
                        aa.TinhTrangDuyet = 1;
                        db.SaveChanges();
                    }
                    // kiểm tra và update SH_NCDT
                    var checkDuyet = db.SH_KyDuyetNCDT.Where(x => x.NCDT_ID == id && x.TinhTrangDuyet != 1).ToList();
                    if (checkDuyet.Count == 0)
                    {
                        var ncdt = db.SH_NhuCauDT.Where(x => x.ID == id).FirstOrDefault();
                        ncdt.TinhTrang = 1;
                        db.SaveChanges();
                    }
                }
                //_context.SaveChanges();
            }
            TempData["msgSuccess"] = "<script>alert('Phê duyệt thành công ');</script>";
            return RedirectToAction("Index_NCDT", "PheDuyetPhieu"); // Quay lại trang danh sách
        }

        [HttpPost]
        public ActionResult ProcessSelectedCTDT(List<int> selectedItems)
        {
            if (selectedItems != null && selectedItems.Any())
            {
                int IDNV = MyAuthentication.ID;
                //// Xử lý danh sách ID được chọn
                foreach (var id in selectedItems)
                {
                    var sH_KyDuyetCTDT = db.SH_KyDuyetCTDT.Where(x => x.ID == id).FirstOrDefault();
                    var chuongtrinh = db.SH_ChuongTrinhDT.Where(x => x.IDCTDT == sH_KyDuyetCTDT.ID_CTDT).FirstOrDefault();
                    var nddt = db.NoiDungDTs.FirstOrDefault(x=>x.IDND == chuongtrinh.ID_NoiDungDT);    
                    if (sH_KyDuyetCTDT.ID_NguoiKiemTra == IDNV)
                    {
                        sH_KyDuyetCTDT.NgayKTDuyet = DateTime.Now;
                        db.SaveChanges();
                    }
                    if (sH_KyDuyetCTDT.ID_TPBP == IDNV)
                    {
                        sH_KyDuyetCTDT.NgayTPBP = DateTime.Now;
                        db.SaveChanges();
                    }
                    if (sH_KyDuyetCTDT.ID_PCHN == IDNV)
                    {
                        sH_KyDuyetCTDT.NgayPCHN = DateTime.Now;
                        db.SaveChanges();
                    }
                    if ((sH_KyDuyetCTDT.NgayKTDuyet != null && sH_KyDuyetCTDT.ID_TPBP == null && sH_KyDuyetCTDT.ID_PCHN == null)
                || (sH_KyDuyetCTDT.NgayKTDuyet != null && sH_KyDuyetCTDT.ID_TPBP != null && sH_KyDuyetCTDT.ID_PCHN == null)
                || (sH_KyDuyetCTDT.NgayKTDuyet != null && sH_KyDuyetCTDT.ID_TPBP == null && sH_KyDuyetCTDT.ID_PCHN != null)
                || (sH_KyDuyetCTDT.NgayKTDuyet != null && sH_KyDuyetCTDT.ID_TPBP != null && sH_KyDuyetCTDT.ID_PCHN != null)
                 ) // Hoàn thành
                    {
                        nddt.IsDelete = false;
                        chuongtrinh.TinhTrang = 1;
                        db.SaveChanges();
                    }

                    if ((sH_KyDuyetCTDT.NgayKTDuyet == null && sH_KyDuyetCTDT.ID_TPBP != null && sH_KyDuyetCTDT.ID_PCHN == null)
                      || (sH_KyDuyetCTDT.NgayKTDuyet != null && sH_KyDuyetCTDT.ID_TPBP != null && sH_KyDuyetCTDT.ID_PCHN == null)
                      || (sH_KyDuyetCTDT.NgayKTDuyet == null && sH_KyDuyetCTDT.ID_TPBP != null && sH_KyDuyetCTDT.ID_PCHN != null)
                      || (sH_KyDuyetCTDT.NgayKTDuyet != null && sH_KyDuyetCTDT.ID_TPBP != null && sH_KyDuyetCTDT.ID_PCHN != null)
                       ) // Hoàn thành
                    {
                        nddt.IsDelete = false;
                        chuongtrinh.TinhTrang = 1;
                        
                        db.SaveChanges();
                    }
                    if ((sH_KyDuyetCTDT.NgayKTDuyet == null && sH_KyDuyetCTDT.ID_TPBP == null && sH_KyDuyetCTDT.ID_PCHN != null)
                       || (sH_KyDuyetCTDT.NgayKTDuyet != null && sH_KyDuyetCTDT.ID_TPBP == null && sH_KyDuyetCTDT.ID_PCHN != null)
                       || (sH_KyDuyetCTDT.NgayKTDuyet == null && sH_KyDuyetCTDT.ID_TPBP != null && sH_KyDuyetCTDT.ID_PCHN != null)
                       || (sH_KyDuyetCTDT.NgayKTDuyet != null && sH_KyDuyetCTDT.ID_TPBP != null && sH_KyDuyetCTDT.ID_PCHN != null)
                        ) // Hoàn thành
                    {
                        nddt.IsDelete = false;
                        chuongtrinh.TinhTrang = 1;
                        db.SaveChanges();
                    }
                }
            }
            TempData["msgSuccess"] = "<script>alert('Phê duyệt thành công ');</script>";
            return RedirectToAction("Index_CTDT", "PheDuyetPhieu"); // Quay lại trang danh sách
        }

        [HttpPost]
        public ActionResult ProcessSelectedTCDT(List<int> selectedItems)
        {
            if (selectedItems != null && selectedItems.Any())
            {
                int IDNV = MyAuthentication.ID;
                //// Xử lý danh sách ID được chọn
                foreach (var id in selectedItems)
                {
                    var sH_KyDuyetCTDT = db.LopHocs.Where(x => x.IDLH == id).FirstOrDefault();
                    sH_KyDuyetCTDT.NgayKiemTra = DateTime.Now;
                    sH_KyDuyetCTDT.TinhTrang = 1;
                    db.SaveChanges();
                }
            }
            TempData["msgSuccess"] = "<script>alert('Phê duyệt thành công ');</script>";
            return RedirectToAction("Index_TCDT", "PheDuyetPhieu"); // Quay lại trang danh sách
        }

        // GET: PheDuyetPhieu/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SH_KyDuyetNCDT sH_KyDuyetNCDT = db.SH_KyDuyetNCDT.Find(id);
            if (sH_KyDuyetNCDT == null)
            {
                return HttpNotFound();
            }
            return View(sH_KyDuyetNCDT);
        }

        // POST: PheDuyetPhieu/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SH_KyDuyetNCDT sH_KyDuyetNCDT = db.SH_KyDuyetNCDT.Find(id);
            db.SH_KyDuyetNCDT.Remove(sH_KyDuyetNCDT);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
