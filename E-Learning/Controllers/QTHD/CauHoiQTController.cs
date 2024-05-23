using DocumentFormat.OpenXml.Office2010.Excel;
using E_Learning.Models;
using E_Learning.ModelsQTHD;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_Learning.Controllers.QTHD
{
    public class CauHoiQTController : Controller
    {
        ELEARNINGEntities db = new ELEARNINGEntities();
        String ControllerName = "CauHoiQT";
        int Idquyen = MyAuthentication.IDQuyen;
        // GET: CauHoiQT
        public ActionResult Index(int? IDQTHD)
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            ViewBag.QUYENCN = ListQuyen;
            if (!ListQuyen.Contains(CONSTKEY.V))
            {
                TempData["msgError"] = "<script>alert('Bạn không có quyền thực hiện chức năng này');</script>";
                return RedirectToAction("", "Home");
            }
            var aa = db.QT_NoiDungQT.Where(x => x.IDQTHD == IDQTHD).FirstOrDefault();
            ViewBag.TenQT= aa.MaHieu +"-"+ aa.TenQTHD;
            ViewBag.IDQTHD = aa.IDQTHD;
            var model = (from a in db.QT_CauHoiQT.Where(x => x.QTHDID == IDQTHD)
                         join c in db.DanhSachDAs on a.IDDAĐung equals c.IDDSĐA
                         select new CauHoiQTView
                         {
                             IDCH = (int)a.IDCH,
                             NoiDungCH =a.NoiDungCH,
                             DapAnA = a.DapAnA,
                             DapAnB = a.DapAnB,
                             DapAnC = a.DapAnC,
                             DapAnD = a.DapAnD,
                             IDDAĐung = a.IDDAĐung,
                             DapAn =c.TenĐA,
                             TenQT =a.QT_NoiDungQT.TenQTHD,
                             MaHieu =a.QT_NoiDungQT.MaHieu,
                             QTHDID =a.QTHDID,
                         }).ToList();

            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var res = (from c in db.QT_CauHoiQT.Where(x=>x.IDCH ==id)
                       join d in db.DanhSachDAs on c.IDDAĐung equals d.IDDSĐA

                       select new CauHoiQTView
                       {
                           IDCH = c.IDCH,
                           NoiDungCH = c.NoiDungCH,
                           DapAnA = c.DapAnA,
                           DapAnB = c.DapAnB,
                           DapAnC = c.DapAnC,
                           DapAnD = c.DapAnD,
                           IDDAĐung = (int)c.IDDAĐung,
                           DapAn = d.TenĐA,
                           TenQT = c.QT_NoiDungQT.TenQTHD,
                           MaHieu = c.QT_NoiDungQT.MaHieu,
                           QTHDID =c.QTHDID
                       }).ToList();
            CauHoiQTView DO = new CauHoiQTView();

            if (res.Count > 0)
            {
                foreach (var c in res)
                {
                    DO.IDCH = c.IDCH;
                    DO.NoiDungCH = c.NoiDungCH;
                    DO.DapAnA = c.DapAnA;
                    DO.DapAnB = c.DapAnB;
                    DO.DapAnC = c.DapAnC;
                    DO.DapAnD = c.DapAnD;
                    DO.IDDAĐung = (int)c.IDDAĐung;
                    DO.DapAn =c.DapAn;
                    DO.QTHDID = c.QTHDID;   
                }

                //List<CTLVDT> lvct = db_context.CTLVDTs.Where(x=>x.LVDTID==DO.IDLVDT).ToList();
                //ViewBag.IDCTLVDT = new SelectList(lvct, "IDCTLVDT", "TenCTLVDT", DO.IDCTLVDT);
                List<DanhSachDA> ds = db.DanhSachDAs.ToList();
                ViewBag.IDDAĐung = new SelectList(ds, "IDDSĐA", "TenĐA", DO.IDDAĐung);
            }
            else
            {
                return HttpNotFound();
            }
            return PartialView(DO);
        }
        [HttpPost]
        public ActionResult Edit(CauHoiQTView _DO)
        {
            try
            {

                db.QT_CauHoiQT_update(_DO.IDCH, _DO.NoiDungCH, _DO.DapAnA, _DO.DapAnB, _DO.DapAnC, _DO.DapAnD, _DO.IDDAĐung, _DO.QTHDID);

                TempData["msgSuccess"] = "<script>alert('Cập nhập thành công');</script>";
            }
            catch (Exception e)
            {

                TempData["msgSuccess"] = "<script>alert('Cập nhập thất bại " + e.Message + " ');</script>";
            }

            return RedirectToAction("Index", "CauHoiQT",new { IDQTHD = _DO.QTHDID});
        }

        public ActionResult Delete(int id,int? IDQTHD)
        {
            try
            {
                db.QT_CauHoiQT_delete(id);
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Xóa dữ liệu thất bại');</script>";
            }
            return RedirectToAction("Index", "CauHoiQT", new { IDQTHD = IDQTHD });
        }

        public ActionResult DeleteAll(int? IDQTHD)
        {
            try
            {
                db.QT_CauHoiQT_deleteAll(IDQTHD);
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Xóa dữ liệu thất bại');</script>";
            }
            return RedirectToAction("Index", "CauHoiQT", new { IDQTHD = IDQTHD });
        }
        //xoa tat ca ket qua va cau hoi
        public ActionResult DeleteAllKetQua(int? IDQTHD)
        {
            try
            {
                var a = db.QT_BaiKiemTra.Where(x => x.QTHDID == IDQTHD).ToList();
                if (a.Count() > 0)
                {
                    foreach (var item in a)
                    {
                        //delete chi tiết bài thi (bỏ logic)
                        //db.QT_CTBaiKiemTra_delete(item.IDKT);
                    }
                    // cập nhật tình trạng làm bài thi
                    db.QT_BaiKiemTra_UpdateDeThi(0, 0, IDQTHD); 
                }
              
                //db.QT_BaiKiemTra_deleteQTID(IDQTHD);
                db.QT_CauHoiQT_deleteAll(IDQTHD);
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Xóa dữ liệu thất bại');</script>";
            }
            return RedirectToAction("Index", "CauHoiQT", new { IDQTHD = IDQTHD });
        }

    }
}