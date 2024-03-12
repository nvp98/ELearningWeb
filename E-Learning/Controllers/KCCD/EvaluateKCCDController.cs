using DocumentFormat.OpenXml.Wordprocessing;
using E_Learning.Models;
using E_Learning.ModelsKCCD;
using E_Learning.ModelsQTHD;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_Learning.Controllers.KCCD
{
    public class EvaluateKCCDController : Controller
    {
        ELEARNINGEntities db = new ELEARNINGEntities();
        ELEARNINGEntities dbKCCCD = new ELEARNINGEntities();
        int Idquyen = MyAuthentication.IDQuyen;
        String ControllerName = "EvaluateKCCD";
        // GET: EvaluateKCCD
        public ActionResult Index(int? page)
        {
            int IDNV = MyAuthentication.ID;

            var res = (from a in db.PhieuXacNhanKCCD_select(IDNV,0,0).Where(x => x.IDTinhTrang !=2)
                       select new ConfirmKCCDView
                       {
                           ID = a.ID,
                           DeNghiDTID = a.DeNghiDTID,
                           NoiDungDT = a.TenND,
                           HocVienID = a.HocVienID,
                           HoTenHV = a.HoTen,
                           MaNV = a.MaNV,
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
                           HVDeXuatKhac = a.HVDeXuat == 1 ? "Được đào tạo lại nội dung" : a.HVDeXuatKhac,
                           HVNgayXacNhan = (DateTime)a.HVNgayXacNhan,
                           IDTinhTrang = a.IDTinhTrang,
                           HVDanhGia = a.IDTinhTrang != 0 ? a.IDTinhTrang ==2? "Hoàn thành" : "HV đã xác nhận" : "Chưa hoàn thành",
                           TinhTrangThi = a.TinhTrangThi,
                           NoiDungKCCDID = db.DeNghiKCCDs.Where(x=>x.ID == a.DeNghiDTID).FirstOrDefault().ID,
                       }).ToList();

            if (page == null) page = 1;
            int pageSize = 50;
            int pageNumber = (page ?? 1);
            return PartialView(res.ToList().ToPagedList(pageNumber, pageSize));
        }

        // GET: EvaluateKCCD/Details/5
        public ActionResult Details(int id)
        {

            var res = (from a in db.PhieuXacNhanKCCD_select(0,0,id)
                       select new ConfirmKCCDView
                       {
                           ID = a.ID,
                           DeNghiDTID = a.DeNghiDTID,
                           HocVienID = a.HocVienID,
                           NoiDungDT = a.TenND,
                           HoTenHV = a.HoTen,
                           MaNV = a.MaNV,
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
                           HVDeXuatKhac = a.HVDeXuat ==1?"Được đào tạo lại nội dung": a.HVDeXuatKhac,
                           HVNgayXacNhan = (DateTime)a.HVNgayXacNhan,
                           IDTinhTrang = a.IDTinhTrang,
                           TinhTrangThi = a.TinhTrangThi
                       }).FirstOrDefault();
            ViewBag.NoiDungDT = res.NoiDungDT;

            //SuggestKCCDView DO = new SuggestKCCDView();

            var nv3 = db.NhanViens.Where(x => x.IDPhongBan == MyAuthentication.IDPhongban && x.IDTinhTrangLV == 1 && x.ID == res.HocVienID).Select(x => new EmployeeValidation { ID = x.ID, HoTen = x.MaNV + " - " + x.HoTen }).ToList();
            ViewBag.NhanVienID = new SelectList(nv3, "ID", "HoTen", res.HocVienID);
            return View(res);
        }
        [HttpPost]
        public ActionResult Details(ConfirmKCCDView _DO)
        {
            try
            {
                if (_DO.HocVienID != null )
                {
                    dbKCCCD.PhieuXacNhanKCCD_HV_update(_DO.ID, _DO.HocVienID, _DO.HVTruocDatDuoc, _DO.HVTruocCanCaiThien, _DO.HVSauDatDuoc, _DO.HVSauCanCaiThien,  _DO.HVDeXuat, _DO.HVDeXuatKhac, 1);
                }

                //TempData["msgSuccess"] = "<script>alert('Thêm mới thành công');</script>";

                return RedirectToAction("Index","EClassroom");
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Có lỗi khi thêm mới: " + e.Message + "');</script>";
                return RedirectToAction("Index", "EClassroom");
            }
        }


        public ActionResult TestKCCD(int? IDNV, int? NoiDungKCCDID, int? DeNghiID)
        {
            string manv = MyAuthentication.Username;
            string TenNV = MyAuthentication.TenNV;
            int? IDVTKNL = MyAuthentication.IDVTKNL;

            int NVID = MyAuthentication.ID;
            var lanthi = db.KCCD_BaiThi.Where(x => x.IDNV == NVID && x.KCCDID == NoiDungKCCDID && x.DeNghiID == DeNghiID).OrderByDescending(x => x.LanThi).ToList();
            if (lanthi.Count > 0)
            {
                if (lanthi.Count >= 3 )
                {
                    TempData["msgSuccess"] = "<script>alert('Bạn đã thi quá 3 lần.');</script>";
                    return RedirectToAction("Index", "EClassroom");

                }
                else if ((lanthi.Where(x => x.TinhTrang == 1).Count() > 0))
                {
                    TempData["msgSuccess"] = "<script>alert('Bạn đã thi đạt');</script>";
                    return RedirectToAction("Index", "EClassroom");
                }
            }

            Random random = new Random();
            var res = (from a in db.DeNghiKCCDs.Where(x => x.ID == DeNghiID)
                       select new BaiThiKCCDView
                       {
                           DeThiKCCDView = db.KCCD_DeThi.Where(x => x.ID == a.DeThiID).FirstOrDefault(),
                           List_CauHoiKCCD = (from ch in db.KCCD_CauHoi.Where(x => x.KCCDID == a.NoiDungKCCDID && x.DeThiID == a.DeThiID)
                                            join da in db.DanhSachDAs on ch.IDDAĐung equals da.IDDSĐA
                                            select new KCCD_CauHoiView
                                            {
                                                IDCH = ch.IDCH,
                                                DapAnA = ch.DapAnA,
                                                DapAnB = ch.DapAnB,
                                                DapAnC = ch.DapAnC,
                                                DapAnD = ch.DapAnD,
                                                IDDAĐung = ch.IDDAĐung,
                                                NoiDungCH = ch.NoiDungCH,
                                                DapAn = da.TenĐA,
                                                Diem = Math.Round((double)10 / db.KCCD_CauHoi.Where(x =>x.KCCDID == a.NoiDungKCCDID && x.DeThiID == a.DeThiID).Count(), 1),
                                            }).ToList(),
                           IDNV = IDNV,
                           TenNV = TenNV,
                           DeNghiID = DeNghiID,
                           KCCDID =NoiDungKCCDID,
                           DeThiID =a.DeThiID,
                           DiemChuan = db.KCCD_DeThi.Where(x => x.ID == a.DeThiID).FirstOrDefault().DiemChuan
                       }).ToList();
            
            res.FirstOrDefault().List_CauHoiKCCD = res.FirstOrDefault().List_CauHoiKCCD.OrderBy(x => random.Next()).ToList();
            return View(res.FirstOrDefault());
        }

        public ActionResult Confirm(BaiThiKCCDView ListQ)
        {
            if (User.Identity.IsAuthenticated)
            {
                ObjectParameter IDKTout = new ObjectParameter("IDBaiThi", typeof(int));
                int i = 0;
                string msg = "";
                int IDBaiThi = 0;
                //var qt = db.QT_NoiDungQT.Where(x => x.IDQTHD == ListQ.QTHDID).FirstOrDefault();

                var lanthi = db.KCCD_BaiThi.Where(x => x.IDNV == ListQ.IDNV && x.KCCDID == ListQ.KCCDID && x.DeNghiID == ListQ.DeNghiID).OrderByDescending(x => x.LanThi).ToList();
                if (ListQ.List_CauHoiKCCD.Count > 0)
                {
                    foreach (var Q in ListQ.List_CauHoiKCCD)
                    {
                        if (i == 0)
                        {
                           
                            if (lanthi.Count != 0)
                            {
                                if (lanthi.Count >= 3 ) // quá 3 lần
                                {
                                    TempData["msgSuccess"] = "<script>alert('Bạn đã thi quá 3 lần.');</script>";
                                    return RedirectToAction("Index", "EClassroom");
                                }
                                else // Thi lại
                                {
                                    db.KCCD_BaiThi_insert(ListQ.IDNV, ListQ.KCCDID, ListQ.DeNghiID,ListQ.DeThiID,0,DateTime.Now,lanthi.Count + 1 ,0, IDKTout);
                                    IDBaiThi = Convert.ToInt32(IDKTout.Value);
                                    if (Q.IDDAĐung == Q.Answer)
                                        db.KCCD_CTBaiThi_insert(IDBaiThi, Q.IDCH, Q.Answer, Q.IDDAĐung, Q.Diem);
                                    else
                                        db.KCCD_CTBaiThi_insert(IDBaiThi, Q.IDCH, Q.Answer, Q.IDDAĐung, Q.Diem);
                                    i++;
                                }
                            }
                            else // Chưa thi
                            {

                                db.KCCD_BaiThi_insert(ListQ.IDNV, ListQ.KCCDID, ListQ.DeNghiID, ListQ.DeThiID, 0, DateTime.Now, lanthi.Count + 1, 0, IDKTout);

                                IDBaiThi = Convert.ToInt32(IDKTout.Value);
                                if (Q.IDDAĐung == Q.Answer)
                                    db.KCCD_CTBaiThi_insert(IDBaiThi, Q.IDCH, Q.Answer, Q.IDDAĐung, Q.Diem);
                                else
                                    db.KCCD_CTBaiThi_insert(IDBaiThi, Q.IDCH, Q.Answer, Q.IDDAĐung, 0);
                                i++;

                            }
                        }
                        else // i != 0
                        {
                            if (Q.IDDAĐung == Q.Answer)
                                db.KCCD_CTBaiThi_insert(IDBaiThi, Q.IDCH,  Q.Answer, Q.IDDAĐung, Q.Diem);
                            else
                                db.KCCD_CTBaiThi_insert(IDBaiThi, Q.IDCH, Q.Answer, Q.IDDAĐung, 0);
                        }
                    }
                    double diemso = Math.Round((double)db.KCCD_CTBaiThi.Where(x => x.IDBT == IDBaiThi).Sum(x => x.Diem), 1);
                    //var bt = db.QT_BaiKiemTra.Where(x => x.IDKT == IDBaiThi).SingleOrDefault();
                    if (diemso > 10) diemso = 10;
                    if (diemso >= ListQ.DiemChuan)
                    {
                        db.KCCD_BaiThi_UpdateDiem(diemso, 1, IDBaiThi);
                        db.PhieuXacNhanKCCD_TinhTrangThi(ListQ.DeNghiID,ListQ.IDNV,1);
                        msg = "<script>alert('Chúc mừng bạn đã hoàn thành bài thi với số điểm là: " + diemso + "');</script>";
                    }
                    else
                    {
                        db.KCCD_BaiThi_UpdateDiem(diemso, 0, IDBaiThi);
                        if(lanthi.Count + 1 >= 3)
                        {
                            db.PhieuXacNhanKCCD_TinhTrangThi(ListQ.DeNghiID, ListQ.IDNV, 3);
                        }
                        else
                        {
                            db.PhieuXacNhanKCCD_TinhTrangThi(ListQ.DeNghiID, ListQ.IDNV, 2);
                        }
                        msg = "<script>alert('Bài thi của bạn đạt số điểm là: " + diemso + ". Bạn cần tham gia thi lại');</script>";
                    }


                }
                TempData["msgSuccess"] = msg;
                return RedirectToAction("Index", "EClassroom");
            }
            else { return RedirectToAction("", "Login"); }

        }


        // GET: EvaluateKCCD/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EvaluateKCCD/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: EvaluateKCCD/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: EvaluateKCCD/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: EvaluateKCCD/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: EvaluateKCCD/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
