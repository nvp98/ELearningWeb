using DocumentFormat.OpenXml.Vml;
using E_Learning.Models;
using Microsoft.Ajax.Utilities;
using PagedList;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
            var aa = db.KNL_NVKiemNhiem.Where(x => x.IDNV == idnv).ToList();
            var res = new List<FCheckValidation>();
            var resView = new List<FCheckValidation>();
            // Đánh giá cá nhân
            var resNV = new List<FCheckValidation>();
            if (vt == null && aa.Count == 0) res = new List<FCheckValidation>();
            if (vt == null)
            {
                //var aa = db.KNL_NVKiemNhiem.Where(x => x.IDNV == nv.ID).ToList();
                if (aa.Count > 0)
                {
                    foreach (var item in aa)
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
                if (aa.Count > 0)
                {
                    foreach (var item in aa)
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

                resNV = (from a in db.NhanVien_SelectKQKNL(idpb).Where(x => x.ID == MyAuthentication.ID)
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
                             TotalDat =  db.KNL_DocBangKNL.Count(x => x.IDNV == a.ID && x.ID_ViTriKNL == a.IDVT),
                             Total = db.KhungNangLuc_Total(a.IDVT).FirstOrDefault() != null? db.KhungNangLuc_Total(a.IDVT).FirstOrDefault().Total:0, // tổng NL duyệt
                             NgayTuDG = a?.NgayTuDG != null ? String.Format("{0:dd/MM/yyyy}", a?.NgayTuDG) : "",
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
        public int CheckDGiaNV()
        {
            string manv = MyAuthentication.Username;
            int idpb = MyAuthentication.IDPhongban;
            int? IDVTKNL = MyAuthentication.IDVTKNL;
            if (IDVTKNL == 0 || manv ==null || idpb ==0) return 0;

            var res = new List<FCheckValidation>();
            var resView = new List<FCheckValidation>();
            var nv = db.NhanViens.Where(x => x.MaNV == manv).FirstOrDefault();
            var vt = db.ViTriKNLs.Where(x => x.IDVT == IDVTKNL).FirstOrDefault();
            if (nv ==null || vt == null) return 0;
            if(vt != null &&  nv != null)
            {
                resView = getListUerView(vt, idpb, nv);
                res = getListUser(vt, idpb, nv);
                if (res.Count != 0 || resView.Count != 0) return 1;
            }
            var aa = db.KNL_NVKiemNhiem.Where(x => x.IDNV == nv.ID).ToList();
            if (aa.Count > 0)
            {
                foreach (var item in aa)
                {
                    var vt1 = db.ViTriKNLs.Where(x => x.IDVT == item.IDVTKN).FirstOrDefault();
                    if(vt1 != null)
                    {
                        var res1 = getListUser(vt1, vt1.IDPB, nv);
                        var resV = getListUerView(vt1, vt1.IDPB, nv);
                        resView.AddRange(resV);
                        res.AddRange(res1);
                    }
                    
                }
            }
            if (resView.Count == 0 && res.Count == 0)
            {
                return 0;
            }
            return 1;
        }
        public List<FCheckValidation> getListUser( ViTriKNL vt ,int? idpb,NhanVien nv)
        {
            var vt2 = checkMVT2(vt.MaViTri);
            var vt3 = checkMVT3(vt.MaViTri);
            var res = new List<FCheckValidation>();
            if(idpb ==null) idpb = 0;
            //var res1 = (from a in db.NhanVien_SelectKQKNL(idpb)
            //            select new FCheckValidation
            //            {
            //                MaNV = a.MaNV,
            //                IDNV = a.ID,
            //                IDVT = a.IDVT,
            //                TenVT = a.TenViTri,
            //                TenNV = a.HoTen,
            //                //IDNhom = b.IDNhom,
            //                //IDPX = b.IDPX,
            //                IDKip = a.IDKip,
            //                TenKip = a.TenKip,
            //                //MaViTri = a.MaViTri,
            //                fileBMTCV = a.FilePath,
            //                NgayDG =a?.NgayDG != null? String.Format("{0:dd/MM/yyyy}", a?.NgayDG):"",
            //                Total = db.KhungNangLuc_Total(a.IDVT) != null?1:0
            //            }).ToList();

            if (vt.IDNhom != null && vt2 == "PT")
            {
                //res = res1.Where(x => x.IDNhom == vt.IDNhom && x.IDVT != vt.IDVT).ToList();
                res  = (from a in db.NhanVien_SelectKQKNL(idpb).Where(x => x.IDVT != vt.IDVT)
                        join b in db.ViTriKNLs.Where(x => x.IDNhom == vt.IDNhom) on a.IDVT equals b.IDVT
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
                               Total = db.KhungNangLuc_Total(a.IDVT) != null ? 1 : 0
                           }).ToList();
                //res = (from a in res1.Where(x=>x.IDVT != vt.IDVT)
                //      join b in db.ViTriKNLs.Where(x=> x.IDNhom == vt.IDNhom) on a.IDVT equals b.IDVT 
                //      select a).ToList();
            }
            else if (vt.IDTo != null && vt2 == "TT")
            {
                res = (from a in db.NhanVien_SelectKQKNL(idpb).Where(x => x.IDVT != vt.IDVT && (x.IDKip == nv.IDKip || (x.IDKip != 1 && x.IDKip != 2 & x.IDKip != 3)))
                       join b in db.ViTriKNLs.Where(x => x.IDTo == vt.IDTo) on a.IDVT equals b.IDVT
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
                           Total = db.KhungNangLuc_Total(a.IDVT) != null ? 1 : 0
                       }).ToList();
                //res = res1.Where(x => x.IDTo == vt.IDTo && x.IDVT != vt.IDVT).ToList();
                //res = (from a in res1.Where(x => x.IDVT != vt.IDVT && (x.IDKip ==nv.IDKip || (x.IDKip != 1 && x.IDKip != 2 & x.IDKip != 3)))
                //       join b in db.ViTriKNLs.Where(x =>  x.IDTo == vt.IDTo) on a.IDVT equals b.IDVT
                //       select a).ToList();
            }

            List<KNLDGiaTCValidation> lsVTTT = (from a in db.KNLDGiaTC_select(vt.IDVT).Where(x=> x.IDVTDGTT != null)
                                                select new KNLDGiaTCValidation
                                                {
                                                    ID = a.ID,
                                                    IDVT = (int)a.IDVT,
                                                    TenViTri = a.TenViTri,
                                                    MaViTri = a.MaViTri,
                                                    IDPB = a.IDPhongBan,
                                                    IDVTDGTC = a.IDVTDGTC,
                                                    IDVTDGTT = a.IDVTDGTT
                                                }).ToList();
            if (lsVTTT.Count > 0)
            {
                var resVT=new List<FCheckValidation>();
                var aa = (from a in db.NhanVien_SelectKQKNL(idpb)
                          join b in lsVTTT on a.IDVT equals b.IDVTDGTT
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
                              Total = db.KhungNangLuc_Total(a.IDVT) != null ? 1 : 0
                          }).ToList();
                if (vt2 == "TK"||vt2 =="TT" || vt2 =="PK" || vt2 =="TP")
                {
                    aa = aa.Where(x =>  x.IDKip == nv.IDKip || (x.IDKip != 1 && x.IDKip != 2 & x.IDKip != 3)).ToList();
                }
                aa.ForEach(res.Add);
            }
            return res;
        }

        public List<FCheckValidation> getListUerView(ViTriKNL vt, int? idpb, NhanVien nv)
        {
            var vt2 = checkMVT2(vt.MaViTri);
            var vt3 = checkMVT3(vt.MaViTri);
            var res = new List<FCheckValidation>();
            if (idpb == null) idpb = 0;

            //var res1 = (from a in db.NhanVien_SelectKQKNL(idpb)
            //            select new FCheckValidation
            //            {
            //                MaNV = a.MaNV,
            //                IDNV = a.ID,
            //                IDVT = a.IDVT,
            //                TenVT = a.TenViTri,
            //                TenNV = a.HoTen,
            //                //IDNhom = b.IDNhom,
            //                //IDPX = b.IDPX,
            //                IDKip = a.IDKip,
            //                TenKip = a.TenKip,
            //                //MaViTri = a.MaViTri,
            //                fileBMTCV = a.FilePath,
            //                NgayDG = a?.NgayDG != null ? String.Format("{0:dd/MM/yyyy}", a?.NgayDG) : "",
            //                Total = db.KhungNangLuc_Total(a.IDVT) != null ? 1 : 0
            //            }).ToList();
 
            List<KNLDGiaTCValidation> lsVTTC = (from a in db.KNLDGiaTC_select(vt.IDVT).Where(x => x.IDVTDGTC != null)
                                                select new KNLDGiaTCValidation
                                                {
                                                    ID = a.ID,
                                                    IDVT = (int)a.IDVT,
                                                    TenViTri = a.TenViTri,
                                                    MaViTri = a.MaViTri,
                                                    IDPB = a.IDPhongBan,
                                                    IDVTDGTC = a.IDVTDGTC,
                                                    IDVTDGTT = a.IDVTDGTT
                                                }).ToList();
            if (lsVTTC.Count > 0)
            {
                var resVT = new List<FCheckValidation>();
                var aa = (from a in db.NhanVien_SelectKQKNL(idpb)
                          join b in lsVTTC on a.IDVT equals b.IDVTDGTC
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
                              Total = db.KhungNangLuc_Total(a.IDVT) != null ? 1 : 0
                          }).ToList();
                if (vt2 == "TK" || vt2 == "TT" || vt2 == "PK" || vt2 == "TP")
                {
                    aa = aa.Where(x => x.IDKip == nv.IDKip || (x.IDKip != 1 && x.IDKip != 2 & x.IDKip != 3)).ToList();
                }
                aa.ForEach(res.Add);
            }
            return res;
        }

        public ActionResult Value(int? IDNV, DateTime dt,string capDG)
        {
            if(IDNV ==null) IDNV = 0;
            var nv = (from a in db.NhanVien_selectByIDNV(IDNV)
                      select new FCheckValidation
                      {
                          TenNV = a.HoTen,
                          TenVT = a.TenViTri,
                          IDVT =a.IDVT,
                          IDNV =a.ID,
                          IDPB =a.IDPB,
                      }).FirstOrDefault();
            ViewBag.TenNV = nv.TenNV??"";
            ViewBag.TenVT = nv.TenVT??"";
            ViewBag.ThangDG = (DateTime?)dt ?? default(DateTime);

            //var kqprev = db.KNL_KQ.Where(x => x.IDNV == IDNV && x.ThangDG == dt).ToList();
            var kqprev = db.KNL_KQ_Select(IDNV, dt).ToList();
            // Xoa Bang KNL cũ trong cùng Tháng
            if(kqprev.Count > 0)
            {
                var listDe = kqprev.Where(x => x.IDVT != nv.IDVT).ToList();
                if (listDe.Count > 0)
                {
                    foreach( var x in listDe)
                    {
                        var k = db.KNL_KQ_delete(x.IDKQ);
                    }
                }
            }
            else if (kqprev.Count == 0)
            {
                //var ngay = db.KNL_KQ.Where(x => x.IDNV == IDNV).OrderByDescending(i => i.NgayDG).FirstOrDefault();
                var ngay = db.KNL_KQ_SelectNV(IDNV).FirstOrDefault();
                //var a = dt.AddMonths(-1);
                var a = ngay != null ? ngay.ThangDG : dt;
                //var kqt = db.KNL_KQ.Where(x => x.IDNV == IDNV && x.ThangDG == a).ToList();
                var kqt = db.KNL_KQ_Select(IDNV, a).ToList();
                //var Fnew = db.KhungNangLucs.Where(x => x.IDVT == nv.IDVT && (x.IDLoaiNL == 1 || x.IDLoaiNL == 2 || (x.IDLoaiNL != 1 && x.IDLoaiNL != 2 && x.IsDanhGia == 1))).ToList();
                var Fnew = db.KhungNangLuc_SearchByIDVT(nv.IDVT).Where(x => x.IsDuyet ==1 && (x.IDLoaiNL == 1 || x.IDLoaiNL == 2 || (x.IDLoaiNL != 1 && x.IDLoaiNL != 2 && x.IsDanhGia == 1))).ToList();
                if (kqt.Count > 0 && Fnew.Count > 0)
                {
                    foreach (var KQ in Fnew)
                    {
                        var aa = kqt.FirstOrDefault(x => x.IDNL == KQ.IDNL);
                        if (aa != null)
                        {
                            db.KNL_KQ_insert(aa.IDNV, aa.IDNL, aa.IDNVDG, aa.DiemDG, dt, aa.NgayDG, CheckKQID(aa.DiemDG, aa.DinhMuc, aa.IsDanhGia), KQ.DinhMuc, KQ.IDVT, aa.Note, null, null);
                            var knlkq = db.KNL_KQ_Select(aa.IDNV,dt).ToList();
                            var kqua = knlkq.FirstOrDefault(x => x.IDNL == aa.IDNL);
                            kqua.DiemTuDG = aa.DiemTuDG;
                            kqua.NgayTuDG = aa.NgayTuDG;
                            kqua.IDNguoiDG_Lan1 = aa.IDNguoiDG_Lan1;
                            kqua.DiemDG_Lan1 = aa.DiemDG_Lan1;
                            kqua.NgayDG_Lan1 = aa.NgayDG_Lan1;
                            
                        }    
                        
                    }
                    db.SaveChanges();
                }
            }

            var res = (from a in db.KNL_KQ_searchByIDNV(IDNV, dt, nv.IDVT).Where(x =>x.IsDuyet ==1)
                      select new FValueValidation
                      {
                          IDNV = (int?)nv.IDNV ?? null,
                          TenNV = nv.TenNV ?? "",
                          IDNL = a.IDNL,
                          TenNL = a.TenNL,
                          IDLoaiNL = a.IDLoaiNL,
                          TenLoaiNL = a.TenLoai,
                          IDVT = a.IDVT,
                          TenViTri = a.TenViTri,
                          IDPB = a.IDPB,
                          TenPhongBan = a.TenPhongBan,
                          DinhMuc = a.IsDanhGia != 0 ? a.DinhMuc : 0,
                          IsDanhGia = a.IsDanhGia,
                          DiemDG = capDG =="1"?a.DiemDG_Lan1:a.IDNV == MyAuthentication.ID?a.DiemTuDG: a.DiemDG,
                          IDKQ = (int?)a.IDKQ ?? null,
                          Note = a.Note,
                          ThangDG = (DateTime?)dt ?? default(DateTime),
                          NgayDG = (DateTime?)a.NgayDG ?? default(DateTime),
                          StrNgayDG = a.NgayDG != null ? a.NgayDG.Value.ToString("dd/MM/yyyy") : "",
                          OrderBy = a.OrderBy,
                          OrderByLoai = a.orByLoai,
                          //ColorKQ = a.DiemDG < a.DinhMuc ? "bg-danger" : "bg-success",
                          ColorKQ = "bg-light",
                          IDNVDG = a.IDNVDG,
                          TenNVDG = a.HoTen,
                          NgayCanhBao = a.DiemDG < a.DinhMuc ? (((DateTime)a.NgayDG).AddMonths(3) -DateTime.Now).Days : a.DiemDG >= a.DinhMuc? (((DateTime)a.NgayDG).AddMonths(6) - DateTime.Now).Days : - 1000,
                          NgayHanDG = a.DiemDG < a.DinhMuc ? ((DateTime)a.NgayDG).AddMonths(3): a.DiemDG >= a.DinhMuc? ((DateTime)a.NgayDG).AddMonths(6) : default(DateTime),
                          DiemCBNVDG = a.DiemTuDG,
                          NgayCBNVDG = a.NgayTuDG,
                          DiemDGLan1 = a.DiemDG_Lan1,
                          NgayDGLan1 = a.NgayDG_Lan1,
                          DiemDuyetDG = a.DiemDG,
                          capDG = capDG
                      }).ToList().OrderBy(x => x.OrderBy);


            var distinctIDLoaiNLs = res.Where(x => x.IDLoaiNL != 1 && x.IDLoaiNL != 2)
                   .Select(x => x.IDLoaiNL)
                   .Distinct()
                   .ToList();

            List<LoaiKNL> loaiNL = db.LoaiKNLs.Where(x => distinctIDLoaiNLs.Contains(x.IDLoai)).OrderBy(x => x.OrderBy).ToList();
            ViewBag.IDLoaiNL = new SelectList(loaiNL, "IDLoai", "TenLoai");

            string manv = MyAuthentication.Username;
            var nvndg = db.NhanViens.Where(x => x.MaNV == manv).FirstOrDefault();

            var vt = db.ViTriKNLs.Where(x => x.IDVT == nvndg.IDVTKNL).FirstOrDefault();
            List<FCheckValidation> user = getListUser(vt, nv.IDPB, nvndg);
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
            user = user.DistinctBy(x => x.MaNV).ToList();
            ViewBag.LSUser = new SelectList(user, "IDNV", "TenNV",IDNV);

            //if (res.Count > 0)
            //{

            //}
            //else
            //{
            //    return HttpNotFound();
            //}
            return View(res.ToList());
        }
        [HttpPost]
        public ActionResult Value(List<FValueValidation> ListKQ)
        {
            try
            {
                string manv = MyAuthentication.Username;
                var nv = db.NhanViens.Where(x => x.MaNV == manv).FirstOrDefault();

                foreach (var KQ in ListKQ)
                {
                    var firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    int idkq = GetIDKQuaKNL(KQ.ThangDG, KQ.IDNV, KQ.IDNL);
                    var diemkq = GetDiemKQuaKNL(KQ.ThangDG, KQ.IDNV, KQ.IDNL, KQ.Note);
                    if(KQ.IsDanhGia == 0) // Không đánh giá NL này
                    {
                        if (idkq == 0)
                        {
                            db.KNL_KQ_insert(KQ.IDNV, KQ.IDNL, null, null, KQ.ThangDG, null, CheckKQID(KQ.DiemDG, KQ.DinhMuc, KQ.IsDanhGia), KQ.DinhMuc, KQ.IDVT, null, null, null);
                        }
                        else // cập nhật các giá trị về null nếu có
                        {
                            var KNLKQ = db.KNL_KQ.Find(KQ.IDKQ);
                            KNLKQ.IDNVDG = null;
                            KNLKQ.DiemDG = null;
                            KNLKQ.NgayDG = null;
                            KNLKQ.Note = null;
                            KNLKQ.DiemTuDG = null;
                            KNLKQ.NgayTuDG = null;
                            KNLKQ.IDNguoiDG_Lan1 = null;
                            KNLKQ.DiemDG_Lan1 = null;
                            KNLKQ.NgayDG_Lan1 = null;
                            db.SaveChanges();
                        }
                       
                    }
                    else // các NL được đánh giá
                    {
                        if (idkq == 0) // chưa có thêm mới
                        {
                            if(KQ.DiemDG != null && KQ.DiemDG != 0) // có điểm đánh giá
                            {
                                if(KQ.IDNV == nv.ID) // tự đánh giá
                                {
                                    db.KNL_KQ_insert(KQ.IDNV, KQ.IDNL, null, null, KQ.ThangDG, null, CheckKQID(KQ.DiemDG, KQ.DinhMuc, KQ.IsDanhGia), KQ.DinhMuc, KQ.IDVT, KQ.Note,KQ.DiemDG,DateTime.Now);
                                }
                                else if (KQ.capDG =="1") // đánh giá lần 1
                                {
                                    var danhgia = new KNL_KQ()
                                    {
                                        IDNV = KQ.IDNV,
                                        IDNL = KQ.IDNL,
                                        IDNVDG = nv.ID,
                                        DiemDG = KQ.DiemDG,
                                        ThangDG = KQ.ThangDG,
                                        NgayDG = DateTime.Now,
                                        Note = KQ.Note,
                                        DiemDM = KQ.DinhMuc,
                                        KQID = CheckKQID(KQ.DiemDG, KQ.DinhMuc, KQ.IsDanhGia),
                                        VTID = KQ.IDVT,
                                        NgayTuDG = null,
                                        DiemTuDG = null,
                                        IDNguoiDG_Lan1 = nv.ID,
                                        DiemDG_Lan1 = KQ.DiemDG,
                                        NgayDG_Lan1 = DateTime.Now
                                    };
                                    db.KNL_KQ.Add(danhgia);
                                    db.SaveChanges();
                                }
                                else // kiểm duyệt kết quả
                                {
                                    db.KNL_KQ_insert(KQ.IDNV, KQ.IDNL, nv.ID, KQ.DiemDG, KQ.ThangDG, DateTime.Now, CheckKQID(KQ.DiemDG, KQ.DinhMuc, KQ.IsDanhGia), KQ.DinhMuc, KQ.IDVT, KQ.Note,null,null);
                                    //var kqua = db.KNL_KQ.Where(x => x.IDNV == KQ.IDNV && x.IDNL == KQ.IDNL && x.ThangDG == KQ.ThangDG).FirstOrDefault();
                                    //kqua.
                                }
                                //db.KNL_KQ_insert(KQ.IDNV, KQ.IDNL, nv.ID, KQ.DiemDG, KQ.ThangDG, DateTime.Now, CheckKQID(KQ.DiemDG, KQ.DinhMuc, KQ.IsDanhGia), KQ.DinhMuc, KQ.IDVT, KQ.Note);
                            }
                            else
                            {
                                db.KNL_KQ_insert(KQ.IDNV, KQ.IDNL, null, null, KQ.ThangDG, null, CheckKQID(KQ.DiemDG, KQ.DinhMuc, KQ.IsDanhGia), KQ.DinhMuc, KQ.IDVT, null, null, null);
                                //db.KNL_KQ_insert(KQ.IDNV, KQ.IDNL, null, null, KQ.ThangDG, null, CheckKQID(KQ.DiemDG, KQ.DinhMuc, KQ.IsDanhGia), KQ.DinhMuc, KQ.IDVT, null);
                            }

                        }
                        else if (diemkq != KQ.DiemDG) // đã có dữ liệu đánh giá điểm khác điểm cũ
                        {
                            if (KQ.DiemDG != null && KQ.DiemDG != 0)
                            {
                                if(KQ.IDNV == nv.ID) // tự đánh giá
                                {
                                    db.KNL_KQ_update_TuDG(KQ.IDKQ, KQ.DiemDG, DateTime.Now);
                                }
                                else if (KQ.capDG == "1") // đánh giá lần 1
                                {
                                    var danhgia = db.KNL_KQ.Find(KQ.IDKQ);
                                    danhgia.DiemDM = KQ.DinhMuc;
                                    danhgia.VTID = KQ.IDVT;
                                    danhgia.KQID = CheckKQID(KQ.DiemDG, KQ.DinhMuc, KQ.IsDanhGia);

                                    //danhgia.IDNVDG = nv.ID;
                                    //danhgia.DiemDG = KQ.DiemDG;
                                    //danhgia.NgayDG = DateTime.Now;
                                    //danhgia.Note = KQ.Note;
                                    danhgia.IDNguoiDG_Lan1 = nv.ID;
                                    danhgia.DiemDG_Lan1 = KQ.DiemDG;
                                    danhgia.NgayDG_Lan1 = DateTime.Now;
                                    db.SaveChanges();
                                }
                                else // kiểm duyệt kết quả
                                {
                                    db.KNL_KQ_update(KQ.IDKQ, KQ.IDNV, KQ.IDNL, nv.ID, KQ.DiemDG, KQ.ThangDG, DateTime.Now, CheckKQID(KQ.DiemDG, KQ.DinhMuc, KQ.IsDanhGia), KQ.DinhMuc, KQ.IDVT, KQ.Note);
                                }
                                
                            }
                            else
                            {
                                if(KQ.IDNV == nv.ID)
                                {
                                    db.KNL_KQ_update_TuDG(KQ.IDKQ, null, null);
                                }
                                else if (KQ.capDG == "1") // đánh giá lần 1
                                {
                                    var danhgia = db.KNL_KQ.Find(KQ.IDKQ);
                                    danhgia.DiemDM = KQ.DinhMuc;
                                    danhgia.VTID = KQ.IDVT;
                                    danhgia.KQID = CheckKQID(KQ.DiemDG, KQ.DinhMuc, KQ.IsDanhGia);

                                    danhgia.IDNVDG = null;
                                    danhgia.DiemDG = null;
                                    danhgia.NgayDG = null;
                                    danhgia.Note = null;
                                    danhgia.IDNguoiDG_Lan1 = null;
                                    danhgia.DiemDG_Lan1 = null;
                                    danhgia.NgayDG_Lan1 = null;
                                    db.SaveChanges();
                                }
                                else db.KNL_KQ_update(KQ.IDKQ, KQ.IDNV, KQ.IDNL, null, null, KQ.ThangDG, null, CheckKQID(KQ.DiemDG, KQ.DinhMuc, KQ.IsDanhGia), KQ.DinhMuc, KQ.IDVT, null);
                            }
                        }
                        else if(diemkq == KQ.DiemDG && KQ.CapNhatDG == true) // tích chọn cập nhật điểm đang có
                        {
                            if(KQ.IDNV == nv.ID)
                            {
                                db.KNL_KQ_update_TuDG(KQ.IDKQ, KQ.DiemDG, DateTime.Now);
                            }
                            else if (KQ.capDG == "1") // đánh giá lần 1
                            {
                                var danhgia = db.KNL_KQ.Find(KQ.IDKQ);
                                danhgia.DiemDM = KQ.DinhMuc;
                                danhgia.VTID = KQ.IDVT;
                                danhgia.KQID = CheckKQID(KQ.DiemDG, KQ.DinhMuc, KQ.IsDanhGia);

                                //danhgia.IDNVDG = nv.ID;
                                //danhgia.DiemDG = KQ.DiemDG;
                                //danhgia.NgayDG = DateTime.Now;
                                //danhgia.Note = KQ.Note;
                                danhgia.IDNguoiDG_Lan1 = nv.ID;
                                danhgia.DiemDG_Lan1 = KQ.DiemDG;
                                danhgia.NgayDG_Lan1 = DateTime.Now;
                                db.SaveChanges();
                            }
                            else db.KNL_KQ_update(KQ.IDKQ, KQ.IDNV, KQ.IDNL, nv.ID, KQ.DiemDG, KQ.ThangDG, DateTime.Now, CheckKQID(KQ.DiemDG, KQ.DinhMuc, KQ.IsDanhGia), KQ.DinhMuc, KQ.IDVT, KQ.Note);
                        }
                    }
                    

                }
                // cập nhật lịch sử
                var nvdg = ListKQ.FirstOrDefault();
                var GtriLS = db.KNL_LSDG.FirstOrDefault(x => x.NVID == nvdg.IDNV && x.ThangDG == nvdg.ThangDG && x.VTID == nvdg.IDVT);
                int? TONGNL = ListKQ.Count();
               
                int? DAT = ListKQ.Where(x=>x.DiemDG == x.DinhMuc).Count();
                int? KDAT = ListKQ.Where(x => x.DiemDG < x.DinhMuc).Count();
                int? VUOT = ListKQ.Where(x => x.DiemDG > x.DinhMuc).Count();
                int? KDGIA = ListKQ.Where(x => x.IsDanhGia == 0).Count();
                int? CHUADG = TONGNL - (DAT + KDAT + VUOT + KDGIA);
                int? DATTu = ListKQ.Where(x => x.DiemDG == x.DinhMuc || x.DiemCBNVDG == x.DinhMuc).Count();
                int? KDATTu = ListKQ.Where(x => x.DiemDG < x.DinhMuc || x.DiemCBNVDG < x.DinhMuc).Count();
                int? VUOTTu = ListKQ.Where(x => x.DiemDG > x.DinhMuc || x.DiemCBNVDG > x.DinhMuc).Count();
                int? KDGIATu = KDGIA;
                int? CHUADGTu = TONGNL - (DATTu + KDATTu + VUOTTu + KDGIATu);
                // đánh giá lần 1
                int? DATLan1 = ListKQ.Where(x => x.DiemDG == x.DinhMuc || x.DiemDGLan1 == x.DinhMuc).Count();
                int? KDATLan1 = ListKQ.Where(x => x.DiemDG < x.DinhMuc || x.DiemDGLan1 < x.DinhMuc).Count();
                int? VUOTLan1 = ListKQ.Where(x => x.DiemDG > x.DinhMuc || x.DiemDGLan1 > x.DinhMuc).Count();
                int? KDGIALan1 = KDGIA;
                int? CHUADGLan1 = TONGNL - (DATLan1 + KDATLan1 + VUOTLan1 + KDGIALan1);

                if (GtriLS == null)
                {
                    if(nvdg.IDNV == nv.ID) // tự đánh giá
                    {
                        db.KNL_LSDG_insert(nvdg.IDNV, nvdg.IDVT, nvdg.ThangDG, null, 0, 0, 0, 0, 0, TONGNL,DateTime.Now);
                        var a = db.KNL_LSDG.FirstOrDefault(x => x.NVID == nvdg.IDNV && x.ThangDG == nvdg.ThangDG && x.VTID == nvdg.IDVT);
                        if(a != null)
                        {
                            a.DATTUDG = DATTu;
                            a.KDATTUDG = KDATTu;
                            a.VUOTTUDG = VUOTTu;
                            a.KDGiaTuDG = KDGIATu;
                            a.CHUADGTuDG = CHUADGTu;
                            
                        }   
                        db.SaveChanges();
                    }
                    else if (ListKQ[0].capDG =="1") // đánh giá lần 1
                    {
                        db.KNL_LSDG_insert(nvdg.IDNV, nvdg.IDVT, nvdg.ThangDG, null, 0, 0, 0, 0, 0, TONGNL, DateTime.Now);
                        var a = db.KNL_LSDG.FirstOrDefault(x => x.NVID == nvdg.IDNV && x.ThangDG == nvdg.ThangDG && x.VTID == nvdg.IDVT);
                        if (a != null)
                        {
                            a.DATTUDGLan1 = DATLan1;
                            a.KDATTUDGLan1 = KDATLan1;
                            a.VUOTTUDGLan1 = VUOTLan1;
                            a.KDGiaTuDGLan1 = KDGIALan1;
                            a.CHUADGTuDGLan1 = KDGIALan1;
                            a.NgayDGGNLan1 = DateTime.Now;
                        }
                        db.SaveChanges();
                    }
                    else
                    {
                        db.KNL_LSDG_insert(nvdg.IDNV, nvdg.IDVT, nvdg.ThangDG, DateTime.Now, DAT, KDAT, VUOT, KDGIA, CHUADG, TONGNL,null);
                    }
                    
                }
                else // update LSDGia
                {
                    if(nvdg.IDNV == nv.ID)
                    {
                        //db.KNL_LSDG_update(GtriLS.IDLS, nvdg.IDNV, nvdg.IDVT, nvdg.ThangDG, GtriLS.NgayDGGN, DAT, KDAT, VUOT, KDGIA, CHUADG, TONGNL,DateTime.Now);
                        var a = db.KNL_LSDG.FirstOrDefault(x => x.IDLS == GtriLS.IDLS);
                        if (a != null)
                        {
                            a.DATTUDG = DATTu;
                            a.KDATTUDG = KDATTu;
                            a.VUOTTUDG = VUOTTu;
                            a.KDGiaTuDG = KDGIATu;
                            a.CHUADGTuDG = CHUADGTu;
                        }
                        db.SaveChanges();
                    }
                    else if (ListKQ[0].capDG == "1") // đánh giá lần 1
                    {
                        var a = db.KNL_LSDG.FirstOrDefault(x => x.IDLS == GtriLS.IDLS);
                        if (a != null)
                        {
                            a.DATTUDGLan1 = DATLan1;
                            a.KDATTUDGLan1 = KDATLan1;
                            a.VUOTTUDGLan1 = VUOTLan1;
                            a.KDGiaTuDGLan1 = KDGIALan1;
                            a.CHUADGTuDGLan1 = KDGIALan1;
                            a.NgayDGGNLan1 = DateTime.Now;
                        }
                        db.SaveChanges();
                    }
                    else
                    {
                        db.KNL_LSDG_update(GtriLS.IDLS, nvdg.IDNV, nvdg.IDVT, nvdg.ThangDG, DateTime.Now, DAT, KDAT, VUOT, KDGIA, CHUADG, TONGNL, GtriLS.NgayTuDGGN);
                    }
                   
                }

                TempData["msgSuccess"] = "<script>alert('Cập nhập thành công');</script>";
            }
            catch (Exception e)
            {

                TempData["msgSuccess"] = "<script>alert('Cập nhập thất bại " + e.Message + " ');</script>";
            }

            return RedirectToAction("Value", "FCheck", new { IDNV =ListKQ[0].IDNV ,dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), capDG = ListKQ[0].capDG });
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

            var res = (from a in db.KNL_KQ_searchByIDNV(IDNV, DateTime.Now, nv.IDVT).Where(x=>x.IsDuyet == 1)
                       join b in db.KNL_DocBangKNL on a.IDNL equals b.ID_NangLuc into uli from b in uli.DefaultIfEmpty()
                       select new FValueValidation
                       {
                           IDNV = (int?)nv.IDNV ?? null,
                           TenNV = nv.TenNV ?? "",
                           IDNL = a.IDNL,
                           TenNL = a.TenNL,
                           IDLoaiNL = a.IDLoaiNL,
                           TenLoaiNL = a.TenLoai,
                           IDVT = a.IDVT,
                           TenViTri = a.TenViTri,
                           IDPB = a.IDPB,
                           TenPhongBan = a.TenPhongBan,
                           DinhMuc = a.IsDanhGia != 0 ? a.DinhMuc : 0,
                           IsDanhGia = a.IsDanhGia,
                           OrderBy = a.OrderBy,
                           OrderByLoai = a.orByLoai,
                           NgayCanhBao = a.DiemDG < a.DinhMuc ? (((DateTime)a.NgayDG).AddMonths(6) - DateTime.Now).Days : -1000,
                           NgayHanDG = a.DiemDG < a.DinhMuc ? ((DateTime)a.NgayDG).AddMonths(6) : default(DateTime),
                           CapNhatDG = b?.TinhTrang == 1?true:false,
                       }).ToList().OrderBy(x => x.OrderBy);
            var distinctIDLoaiNLs = res.Where(x => x.IDLoaiNL != 1 && x.IDLoaiNL != 2)
                   .Select(x => x.IDLoaiNL)
                   .Distinct()
                   .ToList();

            List<LoaiKNL> loaiNL = db.LoaiKNLs.Where(x => distinctIDLoaiNLs.Contains(x.IDLoai)).OrderBy(x => x.OrderBy).ToList();
            ViewBag.IDLoaiNL = new SelectList(loaiNL, "IDLoai", "TenLoai");

            string manv = MyAuthentication.Username;
            var nvndg = db.NhanViens.Where(x => x.MaNV == manv).FirstOrDefault();

            var vt = db.ViTriKNLs.Where(x => x.IDVT == nvndg.IDVTKNL).FirstOrDefault();

            return View(res.ToList());
        }
        [HttpPost]
        public ActionResult ReadKNL(List<FValueValidation> ListKQ)
        {
            try
            {
                string manv = MyAuthentication.Username;
                var nv = db.NhanViens.FirstOrDefault(x => x.MaNV == manv);
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
            if (DiemDG == DiemDM && IsDG ==1) return 1;
            else if (DiemDG < DiemDM && IsDG == 1) return 2;
            else if (DiemDG > DiemDM && IsDG == 1) return 3;
            else if (IsDG ==0) return 4;
            else if(DiemDG is null && IsDG == 1) return 5;
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