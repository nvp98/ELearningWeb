using DocumentFormat.OpenXml.Bibliography;
using E_Learning.Models;
using E_Learning.ModelsQTHD;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_Learning.Controllers.QTHD
{

    public class ListTestQTController : Controller
    {
        ELEARNINGEntities db = new ELEARNINGEntities();
        int Idquyen = MyAuthentication.IDQuyen;
        String ControllerName = "ListTestQT";
       
        // GET: ListTestQT
        public ActionResult Index(int? page, int? IDNV,int? IDVTKNL)
        {

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

            string manv = MyAuthentication.Username;
            string TenNV = MyAuthentication.TenNV;
            var res = (from a in db.QT_PhanQuyen.Where(x=>x.IDVTKNL == IDVTKNL)
                       join b in db.QT_NoiDungQT.Where(x=>x.TinhTrang==1) on a.QTHDID equals b.IDQTHD
                       join c in db.QT_DinhKy on a.DKID equals c.IDDK
                       select new ListTestQTView
                       {
                           IDPhanQuyen = a.IDPhanQuyen,
                           QTHDID = a.QTHDID,
                           IDVTKNL = a.IDVTKNL,
                           List_FileQT = db.QT_FileQT.Where(x => x.QTHDID == a.QTHDID).OrderBy(x => x.OrderByID).ToList(),
                           List_VanBanLQ = (from aa in db.QT_VanBanLQ.Where(x => x.QTHDID == a.QTHDID)
                                            join d in db.QT_NoiDungQT
                                            on aa.IDQT_LienQuan equals d.IDQTHD
                                            select new NoiDungQTLienQuan
                                            {
                                                IDQTHD = d.IDQTHD,
                                                TenQTHD = d.MaHieu + "-" + d.TenQTHD,
                                                TinhTrangHL = d.NgayHieuLuc < DateTime.Now && (d.NgayHetHieuLuc == null || d.NgayHetHieuLuc == default || d.NgayHetHieuLuc > DateTime.Now) ? 1 : 0,
                                            }).Where(x=> x.TinhTrangHL ==1).OrderBy(x => x.IDQTHD).ToList(),
                           SL_CauHoi = db.QT_CauHoiQT.Where(x => x.QTHDID == a.QTHDID).Count(),
                           IDNV = IDNV,
                           TenNV = TenNV,
                           DKID = a.DKID,
                           NoiDungQT = b,
                           QT_DinhKy = c,
                           TinhTrangKT =1,
                           TinhTrang =0,
                           TinhTrangHL =b.TinhTrang,
                       }).ToList();

            //check QT còn hiệu lực
            res = res.Where(x => x.NoiDungQT.NgayHieuLuc < DateTime.Now && (x.NoiDungQT.NgayHetHieuLuc ==null || x.NoiDungQT.NgayHetHieuLuc == default|| x.NoiDungQT.NgayHetHieuLuc > DateTime.Now)).ToList();

            // tình trạng làm bài thi
            foreach (var item in res)
            {
                int aa = (int)item.QT_DinhKy.MaDinhKy;

                DateTime NgayKTTT = (DateTime)item.NoiDungQT.NgayHieuLuc;
                var checkkt = db.QT_BaiKiemTra.Where(x => x.IDNV == IDNV && x.QTHDID == item.QTHDID).OrderByDescending(x => x.LanKT).ToList();
                if (checkkt.Count != 0)
                {
                    item.NgayKTTiep = (DateTime)checkkt.FirstOrDefault().NgayKTTT;
                    item.DiemSo = checkkt.Where(x => x.TinhTrang == 1).FirstOrDefault()?.Diem ?? 0;
                    DateTime nht = (DateTime)checkkt.FirstOrDefault().NgayHT;

                    if (item.NgayKTTiep < DateTime.Now && aa != 0) //quá hạn
                    {
                        item.TinhTrangKT = 0;
                        //item.NgayHT = checkkt.Where(x => x.TinhTrang == 1).FirstOrDefault()?.NgayHT ?? default;
                    }
                    else if (checkkt.Where(x => x.TinhTrang == 1).Count() > 0 && (item.NgayKTTiep >= DateTime.Now || aa == 0)) //Đạt
                    {
                        item.TinhTrangKT = 1;
                        item.TinhTrang = checkkt.Where(x => x.TinhTrang == 1).Count() > 0 ? 1 : 0;
                        item.NgayHT = checkkt.Where(x => x.TinhTrang == 1).FirstOrDefault()?.NgayHT ?? default;
                    }
                    else if (checkkt.Count < 3 || nht.AddDays(1) <DateTime.Now) //Làm lại bài thi
                    {
                        item.TinhTrangKT = 2;
                    }
                    else  //làm sau 24h
                    {
                        item.TinhTrangKT = 3;
                    }
                }
                else
                {
                    item.TinhTrangKT = 0;
                    item.NgayKTTiep = new DateTime(1970, 01, 01);
                }

            }

            if (page == null) page = 1;
            int pageSize = 50;
            int pageNumber = (page ?? 1);
            return View(res.ToList().ToPagedList(pageNumber, pageSize));
        }

        public ActionResult ListQTView(int? page, int? IDNV, int? IDVTKNL)
        {
            var nv = db.NhanViens.Where(x => x.ID == IDNV).FirstOrDefault();
            var vt = db.ViTriKNLs.Where(x => x.IDVT == IDVTKNL).FirstOrDefault();
            string manv = nv.MaNV;
            string TenNV = nv.HoTen;
            ViewBag.TenNV = TenNV;
            ViewBag.MaVTCV = vt.MaViTri +"-"+ vt.TenViTri;
            var res = (from a in db.QT_PhanQuyen.Where(x => x.IDVTKNL == IDVTKNL)
                       join b in db.QT_NoiDungQT.Where(x => x.TinhTrang == 1) on a.QTHDID equals b.IDQTHD
                       join c in db.QT_DinhKy on a.DKID equals c.IDDK
                       select new ListTestQTView
                       {
                           IDPhanQuyen = a.IDPhanQuyen,
                           QTHDID = a.QTHDID,
                           IDVTKNL = a.IDVTKNL,
                           List_FileQT = db.QT_FileQT.Where(x => x.QTHDID == a.QTHDID).OrderBy(x => x.OrderByID).ToList(),
                           List_VanBanLQ = (from aa in db.QT_VanBanLQ.Where(x => x.QTHDID == a.QTHDID)
                                            join d in db.QT_NoiDungQT
                                            on aa.IDQT_LienQuan equals d.IDQTHD
                                            select new NoiDungQTLienQuan
                                            {
                                                IDQTHD = d.IDQTHD,
                                                TenQTHD = d.MaHieu + "-" + d.TenQTHD,
                                                TinhTrangHL = d.NgayHieuLuc < DateTime.Now && (d.NgayHetHieuLuc == null || d.NgayHetHieuLuc == default || d.NgayHetHieuLuc > DateTime.Now) ? 1 : 0,
                                            }).Where(x=>x.TinhTrangHL ==1).OrderBy(x => x.IDQTHD).ToList(),
                           SL_CauHoi = db.QT_CauHoiQT.Where(x => x.QTHDID == a.QTHDID).Count(),
                           IDNV = IDNV,
                           TenNV = TenNV,
                           DKID = a.DKID,
                           NoiDungQT = b,
                           QT_DinhKy = c,
                           TinhTrangKT = 0,
                           TinhTrang = 0,
                           TinhTrangHL = b.TinhTrang,
                       }).ToList();

            //check QT còn hiệu lực
            res = res.Where(x => x.NoiDungQT.NgayHieuLuc < DateTime.Now && (x.NoiDungQT.NgayHetHieuLuc == null || x.NoiDungQT.NgayHetHieuLuc == default || x.NoiDungQT.NgayHetHieuLuc > DateTime.Now)).ToList();

            foreach (var item in res)
            {
                int aa = (int)item.QT_DinhKy.MaDinhKy;
               
                    DateTime NgayKTTT = (DateTime)item.NoiDungQT.NgayHieuLuc;
                    var checkkt = db.QT_BaiKiemTra.Where(x => x.IDNV == IDNV && x.QTHDID == item.QTHDID).OrderByDescending(x => x.LanKT).ToList();
                    if (checkkt.Count != 0)
                    {
                        item.NgayKTTiep = (DateTime)checkkt.FirstOrDefault().NgayKTTT;
                        item.DiemSo = checkkt.Where(x => x.TinhTrang == 1).FirstOrDefault()?.Diem ?? 0;

                        if (item.NgayKTTiep < DateTime.Now && aa != 0) //quá hạn
                        {
                            item.TinhTrangKT = 0;
                            //item.NgayHT = checkkt.Where(x => x.TinhTrang == 1).FirstOrDefault()?.NgayHT ?? default;
                        }
                        else if(checkkt.Where(x => x.TinhTrang == 1).Count() >0 && (item.NgayKTTiep >= DateTime.Now || aa == 0)) //Đạt
                        {
                            item.TinhTrangKT = 1;
                            item.TinhTrang = checkkt.Where(x => x.TinhTrang == 1).Count() > 0 ? 1 : 0;
                            item.NgayHT = checkkt.Where(x => x.TinhTrang == 1).FirstOrDefault()?.NgayHT ?? default;
                        }
                        else if(checkkt.Count < 3 ) //Làm lại bài thi
                        {
                            item.TinhTrangKT = 2;
                        }
                        else  //làm sau 24h
                        {
                            item.TinhTrangKT = 3;
                        }
                    }
                    else
                    {
                        item.TinhTrangKT = 0;
                        item.NgayKTTiep = new DateTime(1970, 01, 01);
                    }
                
            }

            if (page == null) page = 1;
            int pageSize = 50;
            int pageNumber = (page ?? 1);
            return View(res.ToList().ToPagedList(pageNumber, pageSize));
        }

        public ActionResult TestQT(int? IDNV, int?IDQTHD, int? IDQuyen)
        {
            string manv = MyAuthentication.Username;
            string TenNV = MyAuthentication.TenNV;
            int? IDVTKNL = MyAuthentication.IDVTKNL;

            int NVID = MyAuthentication.ID;
            var lanthi = db.QT_BaiKiemTra.Where(x => x.IDNV == NVID && x.QTHDID == IDQTHD ).OrderByDescending(x=>x.LanKT).ToList();
            if (lanthi.Count > 0)
            {
                if (lanthi.Count >= 3 && lanthi.FirstOrDefault().NgayKTTT >= DateTime.Now)
                {
                    DateTime aa = (DateTime)lanthi.FirstOrDefault().NgayHT;
                    if (aa.AddDays(1) >= DateTime.Now)
                    {
                        TempData["msgSuccess"] = "<script>alert('Bạn đã thi quá 3 lần. Vui lòng làm lại bài thi sau 24h');</script>";
                        return RedirectToAction("Index", "ListTestQT", new { IDNV = IDNV, IDVTKNL = IDVTKNL });
                    }
                    
                }
                else if ((lanthi.Where(x => x.TinhTrang == 1).Count() > 0) && lanthi.FirstOrDefault().NgayKTTT >= DateTime.Now)
                {
                    TempData["msgSuccess"] = "<script>alert('Bạn đã thi đạt');</script>";
                    return RedirectToAction("Index", "ListTestQT", new { IDNV = IDNV, IDVTKNL = IDVTKNL });
                }
            }

            Random random = new Random();
            var res = (from a in db.QT_NoiDungQT.Where(x => x.IDQTHD == IDQTHD)
                       select new ListTestQTView
                       {
                           QTHDID = a.IDQTHD,
                           IDVTKNL = IDVTKNL,
                          
                           List_FileQT = db.QT_FileQT.Where(x => x.QTHDID == a.IDQTHD).OrderBy(x => x.OrderByID).ToList(),
                           List_VanBanLQ = (from aa in db.QT_VanBanLQ.Where(x => x.QTHDID == a.IDQTHD)
                                            join d in db.QT_NoiDungQT
                                            on aa.IDQT_LienQuan equals d.IDQTHD
                                            select new NoiDungQTLienQuan
                                            {
                                                IDQTHD = d.IDQTHD,
                                                TenQTHD = d.MaHieu + "-" + d.TenQTHD,
                                                TinhTrangHL = d.NgayHieuLuc < DateTime.Now && (d.NgayHetHieuLuc == null || d.NgayHetHieuLuc == default || d.NgayHetHieuLuc > DateTime.Now) ? 1 : 0,
                                            }).Where(x=>x.TinhTrangHL ==1).OrderBy(x => x.IDQTHD).ToList(),
                           SL_CauHoi = db.QT_CauHoiQT.Where(x => x.QTHDID == a.IDQTHD).Count(),
                           List_CauHoiQT = (from ch in db.QT_CauHoiQT.Where(x => x.QTHDID == a.IDQTHD)
                                            join da in db.DanhSachDAs on ch.IDDAĐung equals da.IDDSĐA
                                            select new CauHoiQTView
                                            {
                                                IDCH = ch.IDCH,
                                                DapAnA =ch.DapAnA,
                                                DapAnB =ch.DapAnB,
                                                DapAnC=ch.DapAnC,
                                                DapAnD=ch.DapAnD,
                                                IDDAĐung =ch.IDDAĐung,
                                                NoiDungCH =ch.NoiDungCH,
                                                DapAn = da.TenĐA,
                                                Diem = Math.Round((double) 10/ db.QT_CauHoiQT.Where(x=> x.QTHDID == a.IDQTHD).Count(),1),
                                            }).ToList(),
                           IDNV = IDNV,
                           TenNV = TenNV,
                           NoiDungQT = a,
                           DKID = db.QT_PhanQuyen.Where(x=>x.IDVTKNL == IDVTKNL && x.QTHDID == a.IDQTHD).FirstOrDefault().DKID,
                           TinhTrang = a.TinhTrang,
                       }).ToList();
            res.FirstOrDefault().List_CauHoiQT = res.FirstOrDefault().List_CauHoiQT.OrderBy(x => random.Next()).ToList();
            return View(res.FirstOrDefault());
        }

        public ActionResult Confirm(ListTestQTView ListQ)
        {
            if (User.Identity.IsAuthenticated)
            {
                ObjectParameter IDKTout = new ObjectParameter("IDKT", typeof(int));
                int i = 0;
                string msg = "";
                int IDBaiThi = 0;
                var qt = db.QT_NoiDungQT.Where(x => x.IDQTHD == ListQ.QTHDID).FirstOrDefault();
                var dk = (int)db.QT_DinhKy.Where(x => x.IDDK == ListQ.DKID).FirstOrDefault().MaDinhKy;


                if (ListQ.List_CauHoiQT.Count > 0)
                {
                    foreach (var Q in ListQ.List_CauHoiQT)
                    {
                        if (i == 0)
                        {
                            var lanthi = db.QT_BaiKiemTra.Where(x => x.IDNV == ListQ.IDNV && x.QTHDID == ListQ.QTHDID).OrderByDescending(x=>x.LanKT).ToList();
                            if (lanthi.Count !=0)
                            {
                                if(lanthi.FirstOrDefault().NgayKTTT >= DateTime.Now || dk == 0)
                                {
                                    DateTime ngay = (DateTime)lanthi.FirstOrDefault().NgayHT;
                                    if (lanthi.Where(x => x.TinhTrang == 1).Count() > 0)
                                    {
                                        TempData["msgSuccess"] = "<script>alert('Bạn đã thi đạt ở kỳ sát hạch này');</script>";
                                        return RedirectToAction("Index", "ListTestQT", new { IDNV = ListQ.IDNV, IDVTKNL = ListQ.IDVTKNL });

                                    }
                                    else if (lanthi.Count >= 3 && ngay.AddDays(1) >= DateTime.Now && lanthi.Where(x => x.TinhTrang == 1).Count() ==0)
                                    {
                                        TempData["msgSuccess"] = "<script>alert('Bạn đã thi quá 3 lần. Vui lòng làm lại bài thi sau 24h');</script>";
                                        return RedirectToAction("Index", "ListTestQT", new { IDNV = ListQ.IDNV, IDVTKNL = ListQ.IDVTKNL });

                                    }
                                    else
                                    {
                                        DateTime NHL = (DateTime)qt.NgayHieuLuc;
                                        int luotKt = (int)lanthi.FirstOrDefault().LuotKiemTra;

                                        if (lanthi.Count >= 3 && ngay.AddDays(1) < DateTime.Now)
                                        {
                                            //delete BaiKiemTra cũ
                                            foreach (var ii in lanthi)
                                            {
                                                db.QT_CTBaiKiemTra_delete(ii.IDKT);
                                            }
                                            db.QT_BaiKiemTra_delete(ListQ.IDNV, ListQ.QTHDID);
                                        }
                                        if (lanthi.Count >= 3)
                                        {
                                            db.QT_BaiKiemTra_insert(ListQ.IDNV, ListQ.QTHDID, 0, DateTime.Now, lanthi.FirstOrDefault().NgayKTTT, 1, 0, luotKt, IDKTout);
                                        }
                                        else
                                        {
                                            db.QT_BaiKiemTra_insert(ListQ.IDNV, ListQ.QTHDID, 0, DateTime.Now, lanthi.FirstOrDefault().NgayKTTT, (lanthi.Count + 1), 0, luotKt, IDKTout);
                                        }
                                        //db.QT_BaiKiemTra_insert(ListQ.IDNV, ListQ.QTHDID, 0, DateTime.Now, lanthi.FirstOrDefault().NgayKTTT, (lanthi.Count + 1), 0, luotKt, IDKTout);
                                        IDBaiThi = Convert.ToInt32(IDKTout.Value);
                                        if (Q.IDDAĐung == Q.Answer)
                                            db.QT_CTBaiKiemTra_insert(IDBaiThi, Q.IDCH, Q.IDDAĐung, Q.Answer, Q.Diem);
                                        else
                                            db.QT_CTBaiKiemTra_insert(IDBaiThi, Q.IDCH, Q.IDDAĐung, Q.Answer, 0);
                                        i++;
                                    }

                                }
                                else if(lanthi.FirstOrDefault().NgayKTTT < DateTime.Now)
                                {
                                    DateTime NHL = (DateTime)qt.NgayHieuLuc;
                                    DateTime NgayKTTT = (DateTime)qt.NgayHieuLuc;
                                    int luotKt = 0;
                                    var checkkt = db.QT_BaiKiemTra.Where(x => x.IDNV == ListQ.IDNV && x.QTHDID == ListQ.QTHDID).OrderByDescending(x => x.NgayKTTT).ToList();

                                    if (checkkt.Count != 0)
                                    {
                                        luotKt = (int)checkkt.FirstOrDefault().LuotKiemTra;
                                    }

                                    //tinh NgayKTTT
                                    int Countthang = MonthDifference(DateTime.Now, NHL);
                                    int countCir = 0;
                                    if (dk != 0)
                                    {
                                        countCir = Countthang / dk;
                                    }

                                    //delete BaiKiemTra cũ
                                    foreach (var ii in lanthi)
                                    {
                                        db.QT_CTBaiKiemTra_delete(ii.IDKT);
                                    }
                                    db.QT_BaiKiemTra_delete(ListQ.IDNV, ListQ.QTHDID);

                                    //db.QT_BaiKiemTra_insert(ListQ.IDNV, ListQ.QTHDID, 0, DateTime.Now, NgayKTTT.AddMonths(dk * (countCir + 1)),1, 0, luotKt + 1, IDKTout);
                                    db.QT_BaiKiemTra_insert(ListQ.IDNV, ListQ.QTHDID, 0, DateTime.Now, DateTime.Now.AddMonths(dk), 1, 0, luotKt + 1, IDKTout);

                                    IDBaiThi = Convert.ToInt32(IDKTout.Value);
                                    if (Q.IDDAĐung == Q.Answer)
                                        db.QT_CTBaiKiemTra_insert(IDBaiThi, Q.IDCH, Q.IDDAĐung, Q.Answer, Q.Diem);
                                    else
                                        db.QT_CTBaiKiemTra_insert(IDBaiThi, Q.IDCH, Q.IDDAĐung, Q.Answer, 0);
                                    i++;
                                }
                            }
                            else
                            {
                                DateTime NHL = (DateTime)qt.NgayHieuLuc;
                                int luotKt = 0;

                                //tinh NgayKTTT
                                int Countthang = MonthDifference(DateTime.Now, NHL);
                                int countCir = 0;
                                if(dk != 0)
                                {
                                    countCir = Countthang / dk;
                                }
                                //db.QT_BaiKiemTra_insert(ListQ.IDNV, ListQ.QTHDID, 0, DateTime.Now, NHL.AddMonths(dk * (countCir + 1)), 1, 0, 1, IDKTout);
                                db.QT_BaiKiemTra_insert(ListQ.IDNV, ListQ.QTHDID, 0, DateTime.Now, DateTime.Now.AddMonths(dk), 1, 0, 1, IDKTout);
                                //if (lanthi.Count >= 3)
                                //{
                                //    db.QT_BaiKiemTra_insert(ListQ.IDNV, ListQ.QTHDID, 0, DateTime.Now, NHL.AddMonths(dk * (countCir + 1)), 0, 0, luotKt+1, IDKTout);
                                //}
                                //else
                                //{
                                //    db.QT_BaiKiemTra_insert(ListQ.IDNV, ListQ.QTHDID, 0, DateTime.Now, NHL.AddMonths(dk * (countCir + 1)), (lanthi.Count + 1), 0, luotKt+1, IDKTout);
                                //}

                                IDBaiThi = Convert.ToInt32(IDKTout.Value);
                                if (Q.IDDAĐung == Q.Answer)
                                    db.QT_CTBaiKiemTra_insert(IDBaiThi, Q.IDCH, Q.IDDAĐung, Q.Answer, Q.Diem);
                                else
                                    db.QT_CTBaiKiemTra_insert(IDBaiThi, Q.IDCH, Q.IDDAĐung, Q.Answer, 0);
                                i++;

                            }
                        }
                        else
                        {
                            if (Q.IDDAĐung == Q.Answer)
                                db.QT_CTBaiKiemTra_insert(IDBaiThi, Q.IDCH, Q.IDDAĐung, Q.Answer, Q.Diem);
                            else
                                db.QT_CTBaiKiemTra_insert(IDBaiThi, Q.IDCH, Q.IDDAĐung, Q.Answer, 0);
                        }
                    }
                    double diemso = Math.Round((double)db.QT_CTBaiKiemTra.Where(x => x.IDKiemTra == IDBaiThi).Sum(x => x.Diem), 1) ;
                    var bt = db.QT_BaiKiemTra.Where(x => x.IDKT == IDBaiThi).SingleOrDefault();
                    if(diemso > 10 ) diemso = 10;
                    if (diemso >= qt.DiemChuan)
                    {
                        db.QT_BaiKiemTra_UpdateDiem(diemso, 1, IDBaiThi);
                        msg = "<script>alert('Chúc mừng bạn đã hoàn thành bài thi với số điểm là: " + diemso + "');</script>";
                    }
                    else
                    {
                        db.QT_BaiKiemTra_UpdateDiem(diemso, 0, IDBaiThi);
                        msg = "<script>alert('Bài thi của bạn đạt số điểm là: " + diemso + ". Bạn cần tham gia thi lại');</script>";
                    }


                }
                TempData["msgSuccess"] = msg;
                return RedirectToAction("Index", "ListTestQT",new { IDNV = ListQ.IDNV , IDVTKNL = ListQ.IDVTKNL});
            }
            else { return RedirectToAction("", "Login"); }

        }
        public int MonthDifference(DateTime lValue, DateTime rValue)
        {
            return (lValue.Month - rValue.Month) + 12 * (lValue.Year - rValue.Year);
        }

    }

}