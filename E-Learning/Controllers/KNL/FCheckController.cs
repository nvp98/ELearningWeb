using DocumentFormat.OpenXml.Vml;
using E_Learning.Models;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace E_Learning.Controllers.KNL
{
    public class FCheckController : Controller
    {
        // GET: FCheck
        ELEARNINGEntities db = new ELEARNINGEntities();
        public ActionResult Index(int? page, string search, int? IDPB)
        {
            //if (CheckDGiaNV() == 0)
            //{
            //    TempData["msgError"] = "<script>alert('Bạn không có quyền truy cập chức năng này');</script>";
            //    return RedirectToAction("", "Home");
            //}
            string manv = MyAuthentication.Username;
            int idnv = MyAuthentication.ID;
            int idpb = MyAuthentication.IDPhongban;
            int? IDVTKNL = MyAuthentication.IDVTKNL;
            if (search == null) search = "";
            ViewBag.search = search;
            var nv = db.NhanViens.Where(x => x.MaNV == manv).FirstOrDefault();
            var vt = db.ViTriKNLs.Where(x => x.IDVT == IDVTKNL).FirstOrDefault();
            var kiemnhiem = db.KNL_NVKiemNhiem.Where(x => x.IDNV == idnv).ToList();
            var res = new List<FCheckValidation>();
            var resView = new List<FCheckValidation>();

            // Đánh giá cá nhân
            var resNV = new List<FCheckValidation>();
            if (vt == null && kiemnhiem.Count == 0) res = new List<FCheckValidation>();
            if (vt == null)
            {
                //var aa = db.KNL_NVKiemNhiem.Where(x => x.IDNV == nv.ID).ToList();
                if (kiemnhiem.Count > 0)
                {
                    foreach (var item in kiemnhiem)
                    {
                        var vt1 = db.ViTriKNLs.FirstOrDefault(x => x.IDVT == item.IDVTKN);
                        if (vt1 != null)
                        {
                            var res1 = getListUser(vt1, vt1.IDPB, nv);
                            var resV = getListUerView(vt1, vt1.IDPB, nv);
                            resView.AddRange(resV);
                            res.AddRange(res1);
                        }

                    }
                }
                res = res.DistinctBy(x => x.MaNV).ToList();
                resView = resView.DistinctBy(x => x.MaNV).ToList();

            }
            else
            {
                resView = getListUerView(vt, idpb, nv);
                res = getListUser(vt, idpb, nv);
                //var aa = db.KNL_NVKiemNhiem.Where(x=>x.IDNV ==nv.ID).ToList();
                if (kiemnhiem.Count > 0)
                {
                    foreach (var item in kiemnhiem)
                    {
                        var vt1 = db.ViTriKNLs.FirstOrDefault(x => x.IDVT == item.IDVTKN);
                        if (vt1 != null)
                        {
                            var res1 = getListUser(vt1, vt1.IDPB, nv);
                            var resV = getListUerView(vt1, vt1.IDPB, nv);
                            resView.AddRange(resV);
                            res.AddRange(res1);
                        }

                    }
                }
                res = res.DistinctBy(x => x.MaNV).ToList();
                resView = resView.DistinctBy(x => x.MaNV).ToList();
                // Bổ sung Đánh giá cá nhân
                var tongNLDoc = db.KNL_DocBangKNL.Count(x => x.IDNV == nv.ID && x.ID_ViTriKNL == vt.IDVT);
                resNV = (from a in db.NhanVien_SelectKQKNL_V2(nv.ID,null,null,null)
                         select new FCheckValidation
                         {
                             MaNV = a.MaNV,
                             IDNV = a.ID,
                             IDVT = a.IDVT,
                             TenVT = a.TenViTri,
                             TenNV = a.HoTen,
                             //IDNhom = b.IDNhom,
                             //IDPX = b.IDPX,
                             IDKip = a.IDKip,
                             TenKip = a.TenKip,
                             //MaViTri = a.MaViTri,
                             fileBMTCV = a.FilePath,
                             NgayDG = a?.NgayDG != null ? String.Format("{0:dd/MM/yyyy}", a?.NgayDG) : "",
                             TotalDat =  tongNLDoc,
                             Total = a.TongNLDuyet, // tổng NL duyệt
                             NgayTuDG = a?.NgayTuDG != null ? String.Format("{0:dd/MM/yyyy}", a?.NgayTuDG) : "",
                             TinhTrang_DuyetKNL = a.TinhTrang_DuyetKNL
                         }).ToList();
            }

            ViewBag.ListUserView = resView;
            //Bổ sung Đánh giá cá nhân
            ViewBag.ListUserNV = resNV;

            //Session["ListUser"] = res;
            if (page == null) page = 1;
            int pageSize = res.Count() > 0 ? res.Count() : 50;
            int pageNumber = (page ?? 1);
            return View(res.ToList().ToPagedList(pageNumber, pageSize));
        }
        //public int CheckDGiaNV()
        //{
        //    string manv = MyAuthentication.Username;
        //    int idpb = MyAuthentication.IDPhongban;
        //    int? IDVTKNL = MyAuthentication.IDVTKNL;
        //    if (IDVTKNL == 0 || manv ==null || idpb ==0) return 0;

        //    var res = new List<FCheckValidation>();
        //    var resView = new List<FCheckValidation>();
        //    var nv = db.NhanViens.Where(x => x.MaNV == manv).FirstOrDefault();
        //    var vt = db.ViTriKNLs.Where(x => x.IDVT == IDVTKNL).FirstOrDefault();
        //    if (nv ==null || vt == null) return 0;
        //    if(vt != null &&  nv != null)
        //    {
        //        resView = getListUerView(vt, idpb, nv);
        //        res = getListUser(vt, idpb, nv);
        //        if (res.Count != 0 || resView.Count != 0) return 1;
        //    }
        //    var aa = db.KNL_NVKiemNhiem.Where(x => x.IDNV == nv.ID).ToList();
        //    if (aa.Count > 0)
        //    {
        //        foreach (var item in aa)
        //        {
        //            var vt1 = db.ViTriKNLs.Where(x => x.IDVT == item.IDVTKN).FirstOrDefault();
        //            if(vt1 != null)
        //            {
        //                var res1 = getListUser(vt1, vt1.IDPB, nv);
        //                var resV = getListUerView(vt1, vt1.IDPB, nv);
        //                resView.AddRange(resV);
        //                res.AddRange(res1);
        //            }
                    
        //        }
        //    }
        //    if (resView.Count == 0 && res.Count == 0)
        //    {
        //        return 0;
        //    }
        //    return 1;
        //}
        public List<FCheckValidation> getListUser( ViTriKNL vt ,int? idpb,NhanVien nv)
        {
            var vt2 = checkMVT2(vt.MaViTri);
            var vt3 = checkMVT3(vt.MaViTri);
            var res = (from a in db.KNL_GetNhanVienDanhGiaTT(vt.IDVT)
                                 select new FCheckValidation
                                 {
                                     MaNV = a.MaNV,
                                     IDNV = a.ID,
                                     IDVT = a.IDVT,
                                     TenVT = a.TenViTri,
                                     TenNV = a.HoTen,
                                     IDKip = a.IDKip,
                                     //TenKip = a.TenKip,
                                     fileBMTCV = a.FilePath,
                                     NgayDG = a?.NgayDG != null ? String.Format("{0:dd/MM/yyyy}", a?.NgayDG) : "",
                                     Total = a.TongNLDuyet,
                                     TinhTrang_DuyetKNL = a.TinhTrang_DuyetKNL,
                                     NgayTuDG = a?.NgayTuDG != null ? String.Format("{0:dd/MM/yyyy}", a?.NgayTuDG) : "",
                                 }).ToList();
            //if(idpb ==null) idpb = 0;

            //if (vt.IDNhom != null && vt2 == "PT")
            //{
            //    res  = (from a in db.NhanVien_SelectKQKNL_V2(null, null, vt.IDNhom,null)
            //            select new FCheckValidation
            //               {
            //                   MaNV = a.MaNV,
            //                   IDNV = a.ID,
            //                   IDVT = a.IDVT,
            //                   TenVT = a.TenViTri,
            //                   TenNV = a.HoTen,
            //                   IDKip = a.IDKip,
            //                   TenKip = a.TenKip,
            //                   fileBMTCV = a.FilePath,
            //                   NgayDG = a?.NgayDG != null ? String.Format("{0:dd/MM/yyyy}", a?.NgayDG) : "",
            //                   Total = a.TongNLDuyet,
            //                   TinhTrang_DuyetKNL = a.TinhTrang_DuyetKNL,
            //                   NgayTuDG = a?.NgayTuDG != null ? String.Format("{0:dd/MM/yyyy}", a?.NgayTuDG) : "",
            //            }).Where(x=>x.IDVT != vt.IDVT).ToList();
            //}
            //else if (vt.IDTo != null && vt2 == "TT")
            //{
            //    res = (from a in db.NhanVien_SelectKQKNL_V2(null,null,null,vt.IDTo)
            //           select new FCheckValidation
            //           {
            //               MaNV = a.MaNV,
            //               IDNV = a.ID,
            //               IDVT = a.IDVT,
            //               TenVT = a.TenViTri,
            //               TenNV = a.HoTen,
            //               IDKip = a.IDKip,
            //               TenKip = a.TenKip,
            //               fileBMTCV = a.FilePath,
            //               NgayDG = a?.NgayDG != null ? String.Format("{0:dd/MM/yyyy}", a?.NgayDG) : "",
            //               Total = a.TongNLDuyet,
            //               TinhTrang_DuyetKNL = a.TinhTrang_DuyetKNL,
            //               NgayTuDG = a?.NgayTuDG != null ? String.Format("{0:dd/MM/yyyy}", a?.NgayTuDG) : "",
            //           }).Where(x => x.IDVT != vt.IDVT).ToList();
            //}

            //List<KNLDGiaTCValidation> lsVTTT = (from a in db.KNLDGiaTC_select(vt.IDVT).Where(x=> x.IDVTDGTT != null)
            //                                    select new KNLDGiaTCValidation
            //                                    {
            //                                        ID = a.ID,
            //                                        IDVT = (int)a.IDVT,
            //                                        TenViTri = a.TenViTri,
            //                                        MaViTri = a.MaViTri,
            //                                        IDPB = a.IDPhongBan,
            //                                        IDVTDGTC = a.IDVTDGTC,
            //                                        IDVTDGTT = a.IDVTDGTT
            //                                    }).ToList();
            //if (lsVTTT.Count > 0)
            //{
            //    var resVT=new List<FCheckValidation>();
            //    var aa = new List<FCheckValidation>();
            //    //foreach (var item in lsVTTT)
            //    //{
            //    //    var nhanvienDG = (from a in db.NhanVien_SelectKQKNL_V2(null, item.IDVTDGTT,null,null)
            //    //                      select new FCheckValidation
            //    //                      {
            //    //                          MaNV = a.MaNV,
            //    //                          IDNV = a.ID,
            //    //                          IDVT = a.IDVT,
            //    //                          TenVT = a.TenViTri,
            //    //                          TenNV = a.HoTen,
            //    //                          IDKip = a.IDKip,
            //    //                          TenKip = a.TenKip,
            //    //                          fileBMTCV = a.FilePath,
            //    //                          NgayDG = a?.NgayDG != null ? String.Format("{0:dd/MM/yyyy}", a?.NgayDG) : "",
            //    //                          Total = a.TongNLDuyet,
            //    //                          TinhTrang_DuyetKNL = a.TinhTrang_DuyetKNL,
            //    //                          NgayTuDG = a?.NgayTuDG != null ? String.Format("{0:dd/MM/yyyy}", a?.NgayTuDG) : "",
            //    //                      });
            //    //    aa.AddRange(nhanvienDG);
            //    //}
            //    if (vt2 == "TK" || vt2 =="PK" || vt2 =="TP")
            //    {
            //        aa = aa.Where(x =>  x.IDKip == nv.IDKip || (x.IDKip != 1 && x.IDKip != 2 & x.IDKip != 3)).ToList();
            //    }
            //    var nhanvienDG = (from a in db.KNL_GetNhanVienDanhGiaTT(vt.IDVT)
            //                      select new FCheckValidation
            //                      {
            //                          MaNV = a.MaNV,
            //                          IDNV = a.ID,
            //                          IDVT = a.IDVT,
            //                          TenVT = a.TenViTri,
            //                          TenNV = a.HoTen,
            //                          IDKip = a.IDKip,
            //                          //TenKip = a.TenKip,
            //                          fileBMTCV = a.FilePath,
            //                          NgayDG = a?.NgayDG != null ? String.Format("{0:dd/MM/yyyy}", a?.NgayDG) : "",
            //                          Total = a.TongNLDuyet,
            //                          TinhTrang_DuyetKNL = a.TinhTrang_DuyetKNL,
            //                          NgayTuDG = a?.NgayTuDG != null ? String.Format("{0:dd/MM/yyyy}", a?.NgayTuDG) : "",
            //                      });
            //    aa.AddRange(nhanvienDG);
            //    if (vt2 == "TK" || vt2 == "PK" || vt2 == "TP")
            //    {
            //        aa = aa.Where(x => x.IDKip == nv.IDKip || (x.IDKip != 1 && x.IDKip != 2 & x.IDKip != 3)).ToList();
            //    }

            //    aa.ForEach(res.Add);
            //}
            if (vt2 == "TK" || vt2 == "PK" || vt2 == "TP")
            {
                res = res.Where(x => x.IDKip == nv.IDKip || (x.IDKip != 1 && x.IDKip != 2 & x.IDKip != 3)).ToList();
            }
            return res;
        }

        public List<FCheckValidation> getListUerView(ViTriKNL vt, int? idpb, NhanVien nv)
        {
            var vt2 = checkMVT2(vt.MaViTri);
            var vt3 = checkMVT3(vt.MaViTri);
            var res = (from a in db.KNL_GetNhanVienDanhGiaTC(vt.IDVT)
                          select new FCheckValidation
                          {
                              MaNV = a.MaNV,
                              IDNV = a.ID,
                              IDVT = a.IDVT,
                              TenVT = a.TenViTri,
                              TenNV = a.HoTen,
                              IDKip = a.IDKip,
                              //TenKip = a.TenKip,
                              fileBMTCV = a.FilePath,
                              NgayDG = a?.NgayDG != null ? String.Format("{0:dd/MM/yyyy}", a?.NgayDG) : "",
                              Total = a.TongNLDuyet,
                              TinhTrang_DuyetKNL = a.TinhTrang_DuyetKNL,
                              NgayTuDG = a?.NgayTuDG != null ? String.Format("{0:dd/MM/yyyy}", a?.NgayTuDG) : "",
                          }).ToList();
            //if (idpb == null) idpb = 0;


 
            //List<KNLDGiaTCValidation> lsVTTC = (from a in db.KNLDGiaTC_select(vt.IDVT).Where(x => x.IDVTDGTC != null)
            //                                    select new KNLDGiaTCValidation
            //                                    {
            //                                        ID = a.ID,
            //                                        IDVT = (int)a.IDVT,
            //                                        TenViTri = a.TenViTri,
            //                                        MaViTri = a.MaViTri,
            //                                        IDPB = a.IDPhongBan,
            //                                        IDVTDGTC = a.IDVTDGTC,
            //                                        IDVTDGTT = a.IDVTDGTT
            //                                    }).ToList();
         
            //if (lsVTTC.Count > 0)
            //{
            //    var resVT = (from a in db.KNL_GetNhanVienDanhGiaTC(vt.IDVT)
            //                 select new FCheckValidation
            //                 {
            //                     MaNV = a.MaNV,
            //                     IDNV = a.ID,
            //                     IDVT = a.IDVT,
            //                     TenVT = a.TenViTri,
            //                     TenNV = a.HoTen,
            //                     IDKip = a.IDKip,
            //                     //TenKip = a.TenKip,
            //                     fileBMTCV = a.FilePath,
            //                     NgayDG = a?.NgayDG != null ? String.Format("{0:dd/MM/yyyy}", a?.NgayDG) : "",
            //                     Total = a.TongNLDuyet,
            //                     TinhTrang_DuyetKNL = a.TinhTrang_DuyetKNL,
            //                     NgayTuDG = a?.NgayTuDG != null ? String.Format("{0:dd/MM/yyyy}", a?.NgayTuDG) : "",
            //                 });
            //    var aa = new List<FCheckValidation>();
            //    foreach (var item in lsVTTC)
            //    {
            //        var nhanvienDG = (from a in db.NhanVien_SelectKQKNL_V2(null,item.IDVTDGTC,null,null)
            //                  select new FCheckValidation
            //                  {
            //                      MaNV = a.MaNV,
            //                      IDNV = a.ID,
            //                      IDVT = a.IDVT,
            //                      TenVT = a.TenViTri,
            //                      TenNV = a.HoTen,
            //                      IDKip = a.IDKip,
            //                      TenKip = a.TenKip,
            //                      fileBMTCV = a.FilePath,
            //                      NgayDG = a?.NgayDG != null ? String.Format("{0:dd/MM/yyyy}", a?.NgayDG) : "",
            //                      Total = a.TongNLDuyet,
            //                      TinhTrang_DuyetKNL = a.TinhTrang_DuyetKNL,
            //                      NgayTuDG = a?.NgayTuDG != null ? String.Format("{0:dd/MM/yyyy}", a?.NgayTuDG) : "",
            //                  });
            //        aa.AddRange(nhanvienDG);
            //    }
            //    var nhanvienDG = (from a in db.KNL_GetNhanVienDanhGiaTC(vt.IDVT)
            //                      select new FCheckValidation
            //                      {
            //                          MaNV = a.MaNV,
            //                          IDNV = a.ID,
            //                          IDVT = a.IDVT,
            //                          TenVT = a.TenViTri,
            //                          TenNV = a.HoTen,
            //                          IDKip = a.IDKip,
            //                          //TenKip = a.TenKip,
            //                          fileBMTCV = a.FilePath,
            //                          NgayDG = a?.NgayDG != null ? String.Format("{0:dd/MM/yyyy}", a?.NgayDG) : "",
            //                          Total = a.TongNLDuyet,
            //                          TinhTrang_DuyetKNL = a.TinhTrang_DuyetKNL,
            //                          NgayTuDG = a?.NgayTuDG != null ? String.Format("{0:dd/MM/yyyy}", a?.NgayTuDG) : "",
            //                      });
            //    aa.AddRange(nhanvienDG);


            //    if (vt2 == "TK"  || vt2 == "PK" || vt2 == "TP")
            //    {
            //        aa = aa.Where(x => x.IDKip == nv.IDKip || (x.IDKip != 1 && x.IDKip != 2 & x.IDKip != 3)).ToList();
            //    }
            //    aa.ForEach(res.Add);
            //}
            if (vt2 == "TK" || vt2 == "PK" || vt2 == "TP")
            {
                res = res.Where(x => x.IDKip == nv.IDKip || (x.IDKip != 1 && x.IDKip != 2 & x.IDKip != 3)).ToList();
            }
            return res;
        }

        public int GetQuarter(DateTime date)
        {
            return (date.Month - 1) / 3 + 1;
        }

        public ActionResult Value(int? IDNV, DateTime dt,string capDG)
        {
            // check Quy
            int Quy = GetQuarter(dt);
            int Nam = dt.Year;
            // Thông tin nhân viên
            if (IDNV ==null) IDNV = 0;
            var nvv = db.NhanViens.FirstOrDefault(x => x.ID == IDNV && x.IDTinhTrangLV ==1);
            var vt = db.ViTriKNLs.FirstOrDefault(x => x.IDVT == nvv.IDVTKNL);
            ViewBag.TenNV = nvv.MaNV + " - "+ nvv.HoTen??"";
            ViewBag.TenVT = vt.TenViTri??"";
            ViewBag.ThangDG = (DateTime?)dt ?? default(DateTime);
            // Kiểm tra đánh giá cũ
            var kqprev = db.KNL_LSDG_TheoQuy(Nam,Quy,IDNV).ToList();
            // Xoa Bang KNL cũ trong cùng Tháng khác vị trí
            if(kqprev.Count > 0)
            {
                var listDgCu = kqprev.Where(x => x.VTID != vt.IDVT).ToList();
                if (listDgCu.Count > 0)
                {
                    foreach( var x in listDgCu)
                    {
                        var k = db.KNL_LSDG_delete(x.IDLS);
                        var m = db.KNL_KQ_LSDG_delete(x.IDLS);
                    }
                }
            }
            // Lấy DS Đánh giá chi tiết

            var joined =
                        from knl in db.KNL_DocBangKNL_IDNV(vt.IDVT, nvv.ID)                // TVF 1
                        join kq0 in db.KNL_KQ_TheoQuy(Nam, Quy, nvv.ID) on knl.IDNL equals kq0.IDNL
                            into gj
                        from kq in gj.DefaultIfEmpty()                                     // LEFT JOIN
                        select new { knl, kq };

            //var res = (from  knl in db.KNL_DocBangKNL_IDNV(vt.IDVT,nvv.ID)
            //          join kq in db.KNL_KQ_TheoQuy(Nam, Quy, nvv.ID) on knl.IDNL equals kq.IDNL
            //          into gj from kq in gj.DefaultIfEmpty() // LEFT JOIN
            //           select new FValueValidation
            //          {
            //              IDNV = (int?)nvv.ID ?? null,
            //              TenNV = nvv.HoTen ?? "",
            //              IDNL = kq.IDNL,
            //              TenNL = kq.TenNL,
            //              IDLoaiNL = kq.IDLoaiNL,
            //              //TenLoaiNL = kq.Ten,
            //              IDVT = kq.VTID,
            //              TenViTri = kq.TenViTri,
            //              //IDPB = a.IDPB,
            //              //TenPhongBan = a.TenPhongBan,
            //              DinhMuc = knl.IsDanhGia != 0 ? knl.DinhMuc : 0,
            //              IsDanhGia = knl.IsDanhGia,
            //              DiemDG = capDG =="1"?kq.DiemDG_Lan1:kq.IDNV == MyAuthentication.ID?kq.DiemTuDG: kq.DiemDG,
            //              IDKQ = (int?)kq.IDKQ ?? null,
            //              Note = kq.Note,
            //              ThangDG = (DateTime?)dt ?? default(DateTime),
            //              NgayDG = (DateTime?)kq.NgayDG ?? default(DateTime),
            //              StrNgayDG = kq.NgayDG != null ? kq.NgayDG.Value.ToString("dd/MM/yyyy") : "",
            //              OrderBy = knl.OrderBy,
            //              //OrderByLoai = a.orByLoai,
            //              ColorKQ = kq.DiemDG < knl.DinhMuc ? "bg-danger" : "bg-success",
            //              //ColorKQ = "bg-light",
            //              IDNVDG = kq.IDNVDG,
            //              TenNVDG = kq.TenNguoiDanhGia,
            //              NgayCanhBao = kq.DiemDG < knl.DinhMuc ? (((DateTime)kq.NgayDG).AddMonths(3) -DateTime.Now).Days : kq.DiemDG >= knl.DinhMuc? (((DateTime)kq.NgayDG).AddMonths(6) - DateTime.Now).Days : - 1000,
            //              NgayHanDG = kq.DiemDG < knl.DinhMuc ? ((DateTime)kq.NgayDG).AddMonths(3): kq.DiemDG >= knl.DinhMuc? ((DateTime)kq.NgayDG).AddMonths(6) : default(DateTime),
            //              DiemCBNVDG = kq.DiemTuDG,
            //              NgayCBNVDG = kq.NgayTuDG,
            //              DiemDGLan1 = kq.DiemDG_Lan1,
            //              NgayDGLan1 = kq.NgayDG_Lan1,
            //              DiemDuyetDG = kq.DiemDG,
            //              capDG = capDG
            //          }).ToList().OrderBy(x => x.OrderBy);
            var res = joined
    .AsEnumerable()    // chuyển sang LINQ to Objects để dùng AddMonths, ToString, DateTime.Now...
    .Select(x =>
    {
        var knl = x.knl;
        var kq = x.kq;   // có thể null

        // chọn điểm theo cấp đánh giá (tránh lồng 3 ngôi mơ hồ)
        int? diemChon;
        if (capDG == "1") diemChon = kq?.DiemDG_Lan1;
        else if (kq?.IDNV == MyAuthentication.ID) diemChon = kq?.DiemTuDG;
        else diemChon = kq?.DiemDG;

        var dimMuc = knl.IsDanhGia != 0 ? knl.DinhMuc : 0;
        var ngayDG = kq?.NgayDG; // DateTime?

        // tính hạn/cảnh báo (null-safe)
        DateTime? han3 = ngayDG?.AddMonths(3);
        DateTime? han6 = ngayDG?.AddMonths(6);
        int ngayCanhBao =
            (kq?.DiemDG != null && ngayDG != null)
                ? ((kq.DiemDG < dimMuc)
                        ? (int)(han3.Value - DateTime.Now).TotalDays
                        : (int)(han6.Value - DateTime.Now).TotalDays)
                : -1000;

        DateTime? ngayHanDG =
            (kq?.DiemDG != null && ngayDG != null)
                ? (kq.DiemDG < dimMuc ? han3 : han6)
                : (DateTime?)null;

        return new FValueValidation
        {
            IDNV = (int?)nvv.ID ?? null,
            TenNV = nvv.HoTen ?? "",
            IDNL = knl.IDNL,                    // lấy từ KN L (luôn có)
            TenNL = kq?.TenNL ?? knl.TenNL,      // kq có thì ưu tiên
            IDLoaiNL = kq?.IDLoaiNL ?? knl.IDLoaiNL,
            IDVT = kq?.VTID ?? knl.IDVT,
            TenViTri = kq?.TenViTri ?? "",
            DinhMuc = dimMuc,
            IsDanhGia = knl.IsDanhGia,
            DiemDG = diemChon,
            IDKQ = (int?)kq?.IDKQ ?? null,
            Note = kq?.Note,
            ThangDG = (DateTime?)dt ?? default(DateTime),
            NgayDG = ngayDG ?? default(DateTime),
            StrNgayDG = ngayDG?.ToString("dd/MM/yyyy") ?? "",
            OrderBy = knl.OrderBy,
            ColorKQ = (kq?.DiemDG ?? 0m) < dimMuc ? "bg-danger" : "bg-success",
            IDNVDG = kq?.IDNVDG,
            TenNVDG = kq?.TenNguoiDanhGia,
            NgayCanhBao = ngayCanhBao,
            NgayHanDG = ngayHanDG ?? default(DateTime),
            DiemCBNVDG = kq?.DiemTuDG,
            NgayCBNVDG = kq?.NgayTuDG,
            DiemDGLan1 = kq?.DiemDG_Lan1,
            NgayDGLan1 = kq?.NgayDG_Lan1,
            DiemDuyetDG = kq?.DiemDG,
            capDG = capDG
        };
    })
    .OrderBy(x => x.OrderBy)
    .ToList();

            List<int> danhSachGiuaLai = new List<int> { };
            foreach (var item in res)
            {
                if(item.IDKQ != null)
                {
                    danhSachGiuaLai.Add((int)item.IDKQ);
                }
            }
            var duLieuXoa = db.KNL_KQ
                    .Where(x => !danhSachGiuaLai.Contains(x.IDKQ) && x.IDNV == IDNV && x.ThangDG == dt)
                    .ToList();
            if(duLieuXoa.Count != 0)
            {
                db.KNL_KQ.RemoveRange(duLieuXoa);
                db.SaveChanges();
            }
            

            var distinctIDLoaiNLs = res.Where(x => x.IDLoaiNL != 1 && x.IDLoaiNL != 2)
                   .Select(x => x.IDLoaiNL)
                   .Distinct()
                   .ToList();

            List<LoaiKNL> loaiNL = db.LoaiKNLs.Where(x => distinctIDLoaiNLs.Contains(x.IDLoai)).OrderBy(x => x.OrderBy).ToList();
            ViewBag.IDLoaiNL = new SelectList(loaiNL, "IDLoai", "TenLoai");

            string manv = MyAuthentication.Username;
            var nvndg = db.NhanViens.Where(x => x.MaNV == manv).FirstOrDefault();

            var vtt = db.ViTriKNLs.Where(x => x.IDVT == nvndg.IDVTKNL).FirstOrDefault();
            List<FCheckValidation> user = getListUser(vtt, nvv.IDPhongBan, nvndg);
            var aaa = db.KNL_NVKiemNhiem.Where(x => x.IDNV == nvndg.ID).ToList();
            if (aaa.Count > 0)
            {
                foreach (var item in aaa)
                {
                    var vt1 = db.ViTriKNLs.Where(x => x.IDVT == item.IDVTKN).FirstOrDefault();
                    var res1 = getListUser(vt1, vt1.IDPB, nvndg);
                    user.AddRange(res1);
                }
            }
            user = user.Where(x=>x.IDNV == IDNV).DistinctBy(x => x.MaNV).ToList();
            ViewBag.LSUser = new SelectList(user, "IDNV", "TenNV",IDNV);
            return View(res.ToList());
        }
        [HttpPost]
        public ActionResult Value(List<FValueValidation> ListKQ)
        {
            try
            {
                string manv = MyAuthentication.Username;
                var nv = db.NhanViens.Where(x => x.MaNV == manv).FirstOrDefault();
                // Nhân viên được đánh giá
                int? IDNVDDG = ListKQ[0].IDNV;
                int? IDVTDDG = ListKQ[0].IDVT;

                int Quy = GetQuarter(DateTime.Now);
                int Nam = DateTime.Now.Year;
                // LSDG trong quý đó
                var LSDG = db.KNL_LSDG_TheoQuy(Nam, Quy, IDNVDDG).FirstOrDefault(x => x.VTID == IDVTDDG);
                // KNL_KQ chi tiết theo quý
                var KNL_KQCu = db.KNL_KQ_TheoQuy(Nam,Quy,IDNVDDG).Where(x=>x.VTID == IDVTDDG).ToList();
                var LSDG_New = new KNL_LSDG()
                {
                    NVID = IDNVDDG,
                    VTID = IDVTDDG,
                    Quy = Quy,
                    Nam = Nam
                };
                if (LSDG == null) // thêm LSDG quý đó
                {
                    db.KNL_LSDG.Add(LSDG_New);
                    db.SaveChanges();
                }
                int IDLS = LSDG == null?LSDG_New.IDLS:LSDG.IDLS;// lấy IDLS để lưu bảng con
                // thêm kết quả chi tiết đánh giá
                foreach (var item in ListKQ)
                {
                    var checkKQ = KNL_KQCu.FirstOrDefault(x => x.IDNL == item.IDNL);
                    int IDKQ = CheckKQID(item.DiemDG, item.DinhMuc, item.IsDanhGia);
                    if (checkKQ == null) // thêm mới
                    {
                        var KNL_KQ_New = new KNL_KQ() 
                        {
                            IDNV = item.IDNV,
                            IDNL = item.IDNL,
                            Quy = Quy,
                            Nam = Nam,
                            IDLS = IDLS,
                            VTID = item.IDVT,
                            DiemDM = item.DinhMuc
                        };
                        db.KNL_KQ.Add(KNL_KQ_New);
                        db.SaveChanges();
                        item.IDKQ = KNL_KQ_New.IDKQ; // gán lại IDKQ 
                    }
                    // update bảng KNL_KQ
                    var searchKQ = db.KNL_KQ.Find(item.IDKQ);
                    if(item.IDNV == nv.ID) // tự đánh giá
                    {
                        searchKQ.DiemTuDG = item.DiemDG;
                        searchKQ.NgayTuDG = DateTime.Now;
                    }
                    else if (item.capDG == "1") // Thẩm định kết quả
                    {
                        searchKQ.DiemDG_Lan1 = item.DiemDG;
                        searchKQ.NgayDG_Lan1 = DateTime.Now;
                        searchKQ.IDNguoiDG_Lan1 = nv.ID;

                    }
                    else // Phê duyệt kết quả
                    {
                        searchKQ.DiemDG = item.DiemDG;
                        searchKQ.NgayDG = DateTime.Now;
                        searchKQ.IDNVDG = nv.ID;
                        searchKQ.Note = item.Note;
                        searchKQ.KQID = IDKQ; //tính điểm
                    }
                    searchKQ.DiemDM = item.DinhMuc;
                    db.SaveChanges();
                }
               


                // Cập nhật kq đánh giá
                //foreach (var KQ in ListKQ)
                //    {
                //        var firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                //        //int idkq = GetIDKQuaKNL(KQ.ThangDG, KQ.IDNV, KQ.IDNL);
                //        var diemkq = GetDiemKQuaKNL(KQ.ThangDG, KQ.IDNV, KQ.IDNL, KQ.Note);
                //        if (KQ.IsDanhGia == 0) // Không đánh giá NL này
                //        {
                //            if (KQ.IDKQ == null) // chưa có kết quả đánh giá
                //            {
                //                db.KNL_KQ_insert(KQ.IDNV, KQ.IDNL, null, null, KQ.ThangDG, null, CheckKQID(KQ.DiemDG, KQ.DinhMuc, KQ.IsDanhGia), KQ.DinhMuc, KQ.IDVT, null, null, null);
                //            }
                //            else // cập nhật các giá trị về null nếu có
                //            {
                //                var KNLKQ = db.KNL_KQ.Find(KQ.IDKQ);
                //                KNLKQ.IDNVDG = null;
                //                KNLKQ.DiemDG = null;
                //                KNLKQ.NgayDG = null;
                //                KNLKQ.Note = null;
                //                KNLKQ.DiemTuDG = null;
                //                KNLKQ.NgayTuDG = null;
                //                KNLKQ.IDNguoiDG_Lan1 = null;
                //                KNLKQ.DiemDG_Lan1 = null;
                //                KNLKQ.NgayDG_Lan1 = null;
                //                db.SaveChanges();
                //            }

                //        }
                //        else // các NL được đánh giá
                //        {

                //            if (KQ.IDKQ == null) // chưa có thêm mới
                //            {
                //                if (KQ.DiemDG != null && KQ.DiemDG != 0) // có điểm đánh giá
                //                {
                //                    if (KQ.IDNV == nv.ID) // tự đánh giá
                //                    {
                //                        db.KNL_KQ_insert(KQ.IDNV, KQ.IDNL, null, null, KQ.ThangDG, null, CheckKQID(KQ.DiemDG, KQ.DinhMuc, KQ.IsDanhGia), KQ.DinhMuc, KQ.IDVT, KQ.Note, KQ.DiemDG, DateTime.Now);
                //                    }
                //                    else if (KQ.capDG == "1") // đánh giá lần 1
                //                    {
                //                        var danhgia = new KNL_KQ()
                //                        {
                //                            IDNV = KQ.IDNV,
                //                            IDNL = KQ.IDNL,
                //                            IDNVDG = nv.ID,
                //                            DiemDG = KQ.DiemDG,
                //                            ThangDG = KQ.ThangDG,
                //                            NgayDG = DateTime.Now,
                //                            Note = KQ.Note,
                //                            DiemDM = KQ.DinhMuc,
                //                            KQID = CheckKQID(KQ.DiemDG, KQ.DinhMuc, KQ.IsDanhGia),
                //                            VTID = KQ.IDVT,
                //                            NgayTuDG = null,
                //                            DiemTuDG = null,
                //                            IDNguoiDG_Lan1 = nv.ID,
                //                            DiemDG_Lan1 = KQ.DiemDG,
                //                            NgayDG_Lan1 = DateTime.Now
                //                        };
                //                        db.KNL_KQ.Add(danhgia);
                //                        db.SaveChanges();
                //                    }
                //                    else // kiểm duyệt kết quả
                //                    {
                //                        db.KNL_KQ_insert(KQ.IDNV, KQ.IDNL, nv.ID, KQ.DiemDG, KQ.ThangDG, DateTime.Now, CheckKQID(KQ.DiemDG, KQ.DinhMuc, KQ.IsDanhGia), KQ.DinhMuc, KQ.IDVT, KQ.Note, null, null);
                //                        //var kqua = db.KNL_KQ.Where(x => x.IDNV == KQ.IDNV && x.IDNL == KQ.IDNL && x.ThangDG == KQ.ThangDG).FirstOrDefault();
                //                        //kqua.
                //                    }
                //                    //db.KNL_KQ_insert(KQ.IDNV, KQ.IDNL, nv.ID, KQ.DiemDG, KQ.ThangDG, DateTime.Now, CheckKQID(KQ.DiemDG, KQ.DinhMuc, KQ.IsDanhGia), KQ.DinhMuc, KQ.IDVT, KQ.Note);
                //                }
                //                else // không có điểm
                //                {
                //                    db.KNL_KQ_insert(KQ.IDNV, KQ.IDNL, null, null, KQ.ThangDG, null, CheckKQID(KQ.DiemDG, KQ.DinhMuc, KQ.IsDanhGia), KQ.DinhMuc, KQ.IDVT, null, null, null);
                //                    //db.KNL_KQ_insert(KQ.IDNV, KQ.IDNL, null, null, KQ.ThangDG, null, CheckKQID(KQ.DiemDG, KQ.DinhMuc, KQ.IsDanhGia), KQ.DinhMuc, KQ.IDVT, null);
                //                }

                //            }
                //            else // đã có đánh giá
                //            {
                //                var danhgia = db.KNL_KQ.Find(KQ.IDKQ); // check danhgia
                //                if (KQ.DiemDG != null) // có điểm đánh giá
                //                {
                //                    if (KQ.IDNV == nv.ID) // tự đánh giá
                //                    {
                //                        if ((KQ.CapNhatDG == true && danhgia.DiemTuDG == KQ.DiemDG) || (danhgia.DiemTuDG != KQ.DiemDG)) // cập nhật đánh giá
                //                        {
                //                            db.KNL_KQ_update_TuDG(KQ.IDKQ, KQ.DiemDG, DateTime.Now);
                //                        }

                //                    }
                //                    else if (KQ.capDG == "1") // đánh giá lần 1
                //                    {
                //                        if ((KQ.CapNhatDG == true && danhgia.DiemDG_Lan1 == KQ.DiemDG) || (danhgia.DiemDG_Lan1 != KQ.DiemDG)) // cập nhật đánh giá lần 1
                //                        {
                //                            danhgia.DiemDM = KQ.DinhMuc;
                //                            danhgia.VTID = KQ.IDVT;
                //                            danhgia.KQID = CheckKQID(KQ.DiemDG, KQ.DinhMuc, KQ.IsDanhGia);

                //                            //danhgia.IDNVDG = nv.ID;
                //                            //danhgia.DiemDG = KQ.DiemDG;
                //                            //danhgia.NgayDG = DateTime.Now;
                //                            //danhgia.Note = KQ.Note;
                //                            danhgia.IDNguoiDG_Lan1 = nv.ID;
                //                            danhgia.DiemDG_Lan1 = KQ.DiemDG;
                //                            danhgia.NgayDG_Lan1 = DateTime.Now;
                //                            db.SaveChanges();
                //                        }

                //                    }
                //                    else
                //                    {
                //                        if ((KQ.CapNhatDG == true && danhgia.DiemDG == KQ.DiemDG) || (danhgia.DiemDG != KQ.DiemDG))
                //                        {
                //                            db.KNL_KQ_update(KQ.IDKQ, KQ.IDNV, KQ.IDNL, nv.ID, KQ.DiemDG, KQ.ThangDG, DateTime.Now, CheckKQID(KQ.DiemDG, KQ.DinhMuc, KQ.IsDanhGia), KQ.DinhMuc, KQ.IDVT, KQ.Note);
                //                        }

                //                    }


                //                }
                //                else // DiemDG = null chưa đánh giá
                //                {
                //                    if (KQ.IDNV == nv.ID)
                //                    {
                //                        db.KNL_KQ_update_TuDG(KQ.IDKQ, KQ.DiemDG, DateTime.Now);
                //                    }
                //                    else if (KQ.capDG == "1") // đánh giá lần 1
                //                    {
                //                        danhgia.DiemDM = KQ.DinhMuc;
                //                        danhgia.VTID = KQ.IDVT;
                //                        danhgia.KQID = CheckKQID(KQ.DiemDG, KQ.DinhMuc, KQ.IsDanhGia);

                //                        //danhgia.IDNVDG = nv.ID;
                //                        //danhgia.DiemDG = KQ.DiemDG;
                //                        //danhgia.NgayDG = DateTime.Now;
                //                        //danhgia.Note = KQ.Note;
                //                        danhgia.IDNguoiDG_Lan1 = null;
                //                        danhgia.DiemDG_Lan1 = KQ.DiemDG;
                //                        danhgia.NgayDG_Lan1 = DateTime.Now;
                //                        db.SaveChanges();
                //                    }
                //                    else db.KNL_KQ_update(KQ.IDKQ, KQ.IDNV, KQ.IDNL, null, KQ.DiemDG, KQ.ThangDG, null, CheckKQID(KQ.DiemDG, KQ.DinhMuc, KQ.IsDanhGia), KQ.DinhMuc, KQ.IDVT, KQ.Note);
                //                }
                //            }
                //        }
                //    }
                // cập nhật lịch sử đánh giá
                var nvdg = ListKQ.FirstOrDefault();
                var GtriLS = db.KNL_LSDG.Find(IDLS);
                //int? TONGNL = ListKQ.Count();
               
                //int? DAT = ListKQ.Where(x=>x.DiemDG == x.DinhMuc).Count();
                //int? KDAT = ListKQ.Where(x => x.DiemDG < x.DinhMuc).Count();
                //int? VUOT = ListKQ.Where(x => x.DiemDG > x.DinhMuc).Count();
                //int? KDGIA = ListKQ.Where(x => x.IsDanhGia == 0).Count();
                //int? CHUADG = TONGNL - (DAT + KDAT + VUOT + KDGIA);
                //int? DATTu = ListKQ.Where(x => x.DiemDG == x.DinhMuc || x.DiemCBNVDG == x.DinhMuc).Count();
                //int? KDATTu = ListKQ.Where(x => x.DiemDG < x.DinhMuc || x.DiemCBNVDG < x.DinhMuc).Count();
                //int? VUOTTu = ListKQ.Where(x => x.DiemDG > x.DinhMuc || x.DiemCBNVDG > x.DinhMuc).Count();
                //int? KDGIATu = KDGIA;
                //int? CHUADGTu = TONGNL - (DATTu + KDATTu + VUOTTu + KDGIATu);
                //// đánh giá lần 1
                //int? DATLan1 = ListKQ.Where(x => x.DiemDG == x.DinhMuc || x.DiemDGLan1 == x.DinhMuc).Count();
                //int? KDATLan1 = ListKQ.Where(x => x.DiemDG < x.DinhMuc || x.DiemDGLan1 < x.DinhMuc).Count();
                //int? VUOTLan1 = ListKQ.Where(x => x.DiemDG > x.DinhMuc || x.DiemDGLan1 > x.DinhMuc).Count();
                //int? KDGIALan1 = KDGIA;
                //int? CHUADGLan1 = TONGNL - (DATLan1 + KDATLan1 + VUOTLan1 + KDGIALan1);

                //if (GtriLS == null)
                //{
                    //if(nvdg.IDNV == nv.ID) // tự đánh giá
                    //{
                    //    db.KNL_LSDG_insert(nvdg.IDNV, nvdg.IDVT, nvdg.ThangDG, null, 0, 0, 0, 0, 0, TONGNL,DateTime.Now);
                    //    //var a = db.KNL_LSDG.FirstOrDefault(x => x.NVID == nvdg.IDNV && x.ThangDG == nvdg.ThangDG && x.VTID == nvdg.IDVT);
                    //    //if(a != null)
                    //    //{
                    //    //    a.DATTUDG = DATTu;
                    //    //    a.KDATTUDG = KDATTu;
                    //    //    a.VUOTTUDG = VUOTTu;
                    //    //    a.KDGiaTuDG = KDGIATu;
                    //    //    a.CHUADGTuDG = CHUADGTu;
                            
                    //    //}
                    //    GtriLS.DATTUDG = DATTu;
                    //    GtriLS.KDATTUDG = KDATTu;
                    //    GtriLS.VUOTTUDG = VUOTTu;
                    //    GtriLS.KDGiaTuDG = KDGIATu;
                    //    GtriLS.CHUADGTuDG = CHUADGTu;
                    //    db.SaveChanges();
                    //}
                    //else if (ListKQ[0].capDG =="1") // đánh giá lần 1
                    //{
                    //    db.KNL_LSDG_insert(nvdg.IDNV, nvdg.IDVT, nvdg.ThangDG, null, 0, 0, 0, 0, 0, TONGNL, DateTime.Now);
                    //    //var a = db.KNL_LSDG.FirstOrDefault(x => x.NVID == nvdg.IDNV && x.ThangDG == nvdg.ThangDG && x.VTID == nvdg.IDVT);
                    //    //if (a != null)
                    //    //{
                    //    //    a.DATTUDGLan1 = DATLan1;
                    //    //    a.KDATTUDGLan1 = KDATLan1;
                    //    //    a.VUOTTUDGLan1 = VUOTLan1;
                    //    //    a.KDGiaTuDGLan1 = KDGIALan1;
                    //    //    a.CHUADGTuDGLan1 = KDGIALan1;
                    //    //    a.NgayDGGNLan1 = DateTime.Now;
                    //    //}
                    //    GtriLS.DATTUDGLan1 = DATLan1;
                    //    GtriLS.KDATTUDGLan1 = KDATLan1;
                    //    GtriLS.VUOTTUDGLan1 = VUOTLan1;
                    //    GtriLS.KDGiaTuDGLan1 = KDGIALan1;
                    //    GtriLS.CHUADGTuDGLan1 = KDGIALan1;
                    //    GtriLS.NgayDGGNLan1 = DateTime.Now;
                    //    db.SaveChanges();
                    //}
                    //else
                    //{
                    //    db.KNL_LSDG_insert(nvdg.IDNV, nvdg.IDVT, nvdg.ThangDG, DateTime.Now, DAT, KDAT, VUOT, KDGIA, CHUADG, TONGNL,null);
                    //}
                    
                //}
                //else // update LSDGia
                //{
                //    if(nvdg.IDNV == nv.ID)
                //    {
                        //db.KNL_LSDG_update(GtriLS.IDLS, nvdg.IDNV, nvdg.IDVT, nvdg.ThangDG, GtriLS.NgayDGGN, DAT, KDAT, VUOT, KDGIA, CHUADG, TONGNL,DateTime.Now);
                        //var a = db.KNL_LSDG.FirstOrDefault(x => x.IDLS == GtriLS.IDLS);
                        //if (a != null)
                        //{
                        //    a.DATTUDG = DATTu;
                        //    a.KDATTUDG = KDATTu;
                        //    a.VUOTTUDG = VUOTTu;
                        //    a.KDGiaTuDG = KDGIATu;
                        //    a.CHUADGTuDG = CHUADGTu;
                        //}
                        //GtriLS.DATTUDG = DATTu;
                        //GtriLS.KDATTUDG = KDATTu;
                        //GtriLS.VUOTTUDG = VUOTTu;
                        //GtriLS.KDGiaTuDG = KDGIATu;
                        //GtriLS.CHUADGTuDG = CHUADGTu;
                        //GtriLS.NgayTuDGGN = DateTime.Now;
                        //db.SaveChanges();
                    //}
                    //else if (ListKQ[0].capDG == "1") // đánh giá lần 1
                    //{
                        //var a = db.KNL_LSDG.FirstOrDefault(x => x.IDLS == GtriLS.IDLS);
                        //if (a != null)
                        //{
                        //    a.DATTUDGLan1 = DATLan1;
                        //    a.KDATTUDGLan1 = KDATLan1;
                        //    a.VUOTTUDGLan1 = VUOTLan1;
                        //    a.KDGiaTuDGLan1 = KDGIALan1;
                        //    a.CHUADGTuDGLan1 = KDGIALan1;
                        //    a.NgayDGGNLan1 = DateTime.Now;
                        //}
                        //GtriLS.DATTUDGLan1 = DATLan1;
                        //GtriLS.KDATTUDGLan1 = KDATLan1;
                        //GtriLS.VUOTTUDGLan1 = VUOTLan1;
                        //GtriLS.KDGiaTuDGLan1 = KDGIALan1;
                        //GtriLS.CHUADGTuDGLan1 = CHUADGLan1;
                        //GtriLS.NgayDGGNLan1 = DateTime.Now;
                        //db.SaveChanges();
                    //}
                    //else // phê duyệt
                    //{
                        //GtriLS.DAT = DAT;
                        //GtriLS.KDAT = KDAT;
                        //GtriLS.VUOT = VUOT;
                        //GtriLS.KDGia = KDGIA;
                        //GtriLS.CHUADG = CHUADG;
                        //GtriLS.NgayDGGN = DateTime.Now;
                        //db.SaveChanges();
                        //db.KNL_LSDG_update(GtriLS.IDLS, nvdg.IDNV, nvdg.IDVT, nvdg.ThangDG, DateTime.Now, DAT, KDAT, VUOT, KDGIA, CHUADG, TONGNL, GtriLS.NgayTuDGGN);
                //    }
                   
                //}

                TempData["msgSuccess"] = "<script>alert('Đánh giá thành công');</script>";
            }
            catch (Exception e)
            {

                TempData["msgSuccess"] = "<script>alert('Cập nhập thất bại " + e.Message + " ');</script>";
            }

            return RedirectToAction("Value", "FCheck", new { IDNV =ListKQ[0].IDNV ,dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), capDG = ListKQ[0].capDG });
        }
        [HttpPost]
        public ActionResult ValueAjax()
        {
            try
            {
                string manv = MyAuthentication.Username;
                var nv = db.NhanViens.FirstOrDefault(x => x.MaNV == manv);
                Request.InputStream.Position = 0;
                using (var reader = new StreamReader(Request.InputStream)) // xử lý dữ liệu lớn
                {
                    var body = reader.ReadToEnd();

                    // Deserialize JSON list
                    var listKQ = JsonConvert.DeserializeObject<List<FValueDto>>(body);

                    if (listKQ == null)
                        return Json(new { success = false, message = "Không parse được JSON" });

                    // Lấy item đầu tiên
                    var firstItem = listKQ.FirstOrDefault();

                    int? IDNVDDG = firstItem.IDNV;
                    int? IDVTDDG = firstItem.IDVT;
                    int Quy = GetQuarter(DateTime.Now);
                    int Nam = DateTime.Now.Year;

                    var LSDG = db.KNL_LSDG_TheoQuy(Nam, Quy, IDNVDDG).FirstOrDefault(x => x.VTID == IDVTDDG);
                    var KNL_KQCu = db.KNL_KQ_TheoQuy(Nam, Quy, IDNVDDG).Where(x => x.VTID == IDVTDDG).ToList();

                    var LSDG_New = new KNL_LSDG()
                    {
                        NVID = IDNVDDG,
                        VTID = IDVTDDG,
                        Quy = Quy,
                        Nam = Nam
                    };

                    if (LSDG == null)
                    {
                        db.KNL_LSDG.Add(LSDG_New);
                        db.SaveChanges();
                    }

                    int IDLS = LSDG == null ? LSDG_New.IDLS : LSDG.IDLS;

                    foreach (var item in listKQ)
                    {
                        var checkKQ = KNL_KQCu.FirstOrDefault(x => x.IDNL == item.IDNL);
                        int IDKQ = CheckKQID(item.DiemDG, item.DinhMuc, item.IsDanhGia);

                        if (checkKQ == null) // thêm kết quả mới
                        {
                            var KNL_KQ_New = new KNL_KQ()
                            {
                                IDNV = item.IDNV,
                                IDNL = item.IDNL,
                                Quy = Quy,
                                Nam = Nam,
                                IDLS = IDLS,
                                VTID = item.IDVT,
                                DiemDM = item.DinhMuc
                            };
                            db.KNL_KQ.Add(KNL_KQ_New);
                            //db.SaveChanges();
                            //item.IDKQ = KNL_KQ_New.IDKQ;
                            if (item.IDNV == nv.ID)
                            {
                                KNL_KQ_New.DiemTuDG = item.DiemDG;
                                KNL_KQ_New.NgayTuDG = DateTime.Now;
                            }
                            else if (item.CapDG == "1")
                            {
                                KNL_KQ_New.DiemDG_Lan1 = item.DiemDG;
                                KNL_KQ_New.NgayDG_Lan1 = DateTime.Now;
                                KNL_KQ_New.IDNguoiDG_Lan1 = nv.ID;
                            }
                            else
                            {
                                KNL_KQ_New.DiemDG = item.DiemDG;
                                KNL_KQ_New.NgayDG = DateTime.Now;
                                KNL_KQ_New.IDNVDG = nv.ID;
                                KNL_KQ_New.Note = item.Note;
                                KNL_KQ_New.KQID = IDKQ;
                            }
                            KNL_KQ_New.DiemDM = item.DinhMuc;
                        }
                        else // update kq cũ
                        {
                            var searchKQ = db.KNL_KQ.Find(item.IDKQ);
                            if(searchKQ != null)
                            {
                                if (item.IDNV == nv.ID)
                                {
                                    searchKQ.DiemTuDG = item.DiemDG;
                                    searchKQ.NgayTuDG = DateTime.Now;
                                }
                                else if (item.CapDG == "1")
                                {
                                    searchKQ.DiemDG_Lan1 = item.DiemDG;
                                    searchKQ.NgayDG_Lan1 = DateTime.Now;
                                    searchKQ.IDNguoiDG_Lan1 = nv.ID;
                                }
                                else
                                {
                                    searchKQ.DiemDG = item.DiemDG;
                                    searchKQ.NgayDG = DateTime.Now;
                                    searchKQ.IDNVDG = nv.ID;
                                    searchKQ.Note = item.Note;
                                    searchKQ.KQID = IDKQ;
                                }
                                searchKQ.DiemDM = item.DinhMuc;
                            }
                        }

                        //var searchKQ = db.KNL_KQ.Find(item.IDKQ);
                        
                    }
                    db.SaveChanges();
                    return Json(new { success = true, message = "Đánh giá thành công" });

                }



            }
            catch (Exception e)
            {
                return Json(new { success = false, message = "Cập nhật thất bại: " + e.Message });
            }
        }

        public ActionResult ReadKNL(int? IDNV)
        {
            if (IDNV == null) IDNV = 0;
            var nv = (from a in db.NhanVien_selectByIDNV(IDNV)
                      select new FCheckValidation
                      {
                          TenNV = a.HoTen,
                          TenVT = a.TenViTri,
                          IDVT = a.IDVT,
                          IDNV = a.ID,
                          IDPB = a.IDPB,
                      }).FirstOrDefault();
            ViewBag.TenNV = nv.TenNV ?? "";
            ViewBag.TenVT = nv.TenVT ?? "";

            var res = (from a in db.KNL_DocBangKNL_IDNV(nv.IDVT,IDNV)
                       select new FValueValidation
                       {
                           IDNV = (int?)nv.IDNV ?? null,
                           TenNV = nv.TenNV ?? "",
                           IDNL = a.IDNL,
                           TenNL = a.TenNL,
                           IDLoaiNL = a.IDLoaiNL,
                           //TenLoaiNL = a.TenLoai,
                           IDVT = a.IDVT,
                           //TenViTri = a.TenViTri,
                           //IDPB = a.IDPB,
                           //TenPhongBan = a.TenPhongBan,
                           DinhMuc = a.IsDanhGia != 0 ? a.DinhMuc : 0,
                           IsDanhGia = a.IsDanhGia,
                           OrderBy = a.OrderBy,
                           OrderByLoai = a.OrderBy,
                           //NgayCanhBao = a.DiemDG < a.DinhMuc ? (((DateTime)a.NgayDG).AddMonths(6) - DateTime.Now).Days : -1000,
                           //NgayHanDG = a.DiemDG < a.DinhMuc ? ((DateTime)a.NgayDG).AddMonths(6) : default(DateTime),
                           CapNhatDG = a?.TinhTrang == 1 ? true : false,
                           //CapNhatDG =false
                       }).ToList().OrderBy(x => x.OrderBy);
            var distinctIDLoaiNLs = res.Where(x => x.IDLoaiNL != 1 && x.IDLoaiNL != 2)
                   .Select(x => x.IDLoaiNL)
                   .Distinct()
                   .ToList();

            List<LoaiKNL> loaiNL = db.LoaiKNLs.Where(x => distinctIDLoaiNLs.Contains(x.IDLoai)).OrderBy(x => x.OrderBy).ToList();
            ViewBag.IDLoaiNL = new SelectList(loaiNL, "IDLoai", "TenLoai");

            //string manv = MyAuthentication.Username;
            //var nvndg = db.NhanViens.Where(x => x.MaNV == manv).FirstOrDefault();

            //var vt = db.ViTriKNLs.Where(x => x.IDVT == nvndg.IDVTKNL).FirstOrDefault();

            return View(res.ToList());
        }
        [HttpPost]
        public ActionResult ReadKNL(List<FValueValidation> ListKQ)
        {
            try
            {
                string manv = MyAuthentication.Username;
                var nv = db.NhanViens.FirstOrDefault(x => x.MaNV == manv);
                var listDocCu = db.KNL_DocBangKNL.Where(x => x.IDNV == nv.ID).ToList(); // xóa lịch sử đọc cũ và lưu vào cái ds mới
                db.KNL_DocBangKNL.RemoveRange(listDocCu);
                db.SaveChanges();
                foreach (var KQ in ListKQ)
                {
                    if (KQ.CapNhatDG) {
                        var checkDoc = db.KNL_DocBangKNL.FirstOrDefault(x => x.ID_NangLuc == KQ.IDNL && x.IDNV == KQ.IDNV);
                        if (checkDoc == null)
                        {
                            KNL_DocBangKNL a = new KNL_DocBangKNL()
                            {
                                IDNV = KQ.IDNV,
                                ID_ViTriKNL = KQ.IDVT,
                                ID_NangLuc = KQ.IDNL,
                                NgayTao = DateTime.Now,
                                IsDelete = false,
                                TinhTrang = 1
                            };
                            db.KNL_DocBangKNL.Add(a);
                            db.SaveChanges();
                        }
                    }

                }

                TempData["msgSuccess"] = "<script>alert('Cập nhập thành công');</script>";
            }
            catch (Exception e)
            {

                TempData["msgSuccess"] = "<script>alert('Cập nhập thất bại " + e.Message + " ');</script>";
            }

            return RedirectToAction("ReadKNL", "FCheck", new { IDNV = ListKQ[0].IDNV});
        }

        public string checkMVT2(string mvt)
        {
            if (mvt == null) return "";
            else if (mvt.Length < 2) return "";
            else return mvt.Substring(0, 2);
        }
        public string checkMVT3(string mvt)
        {
            if (mvt == null) return "";
            else if (mvt?.Length < 3) return "";
            else return mvt.Substring(0, 3);
        }


        public int CheckKQID(int? DiemDG, int? DiemDM,int? IsDG)
        {
            if (DiemDG == DiemDM && IsDG ==1) return 1; // Đạt
            else if (DiemDG < DiemDM && IsDG == 1) return 2; // Không đạt
            else if (DiemDG > DiemDM && IsDG == 1) return 3; // vượt
            else if (IsDG ==0) return 4; // không đánh giá
            else if(DiemDG is null && IsDG == 1) return 5; // chưa đánh giá
            return 0;
        }

        public int? CountSLDG(int? IDNV, int? IDVT, DateTime? ThangDG,int? KQID)
        {
            var knlkq = db.KNL_KQ_searchByIDNV(IDNV, ThangDG,IDVT).ToList();
            if (KQID == null)
            {
               
                return knlkq.Count(); 
            }
            int? kq = knlkq.Count(x => x.IDKQ == KQID);
            return kq;
        }


        public int GetIDKQuaKNL(DateTime? dateDG, int? IDNV, int? IDNL)
        {
            var knlkq = db.KNL_KQ_searchByIDNL(IDNL,dateDG).ToList();
            var model = knlkq.FirstOrDefault(x => x.IDNV == IDNV);
            if (model == null)
                return 0;
            return model.IDKQ;
        }
        
        public int? GetDiemKQuaKNL(DateTime? dateDG, int? IDNV, int? IDNL, string note)
        {
            var knlkq = db.KNL_KQ_searchByIDNL(IDNL, dateDG).ToList();
            var model = knlkq.FirstOrDefault(x => x.IDNV == IDNV && x.Note == note);
            if (model == null)
                return null;
            return model.DiemDG;
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