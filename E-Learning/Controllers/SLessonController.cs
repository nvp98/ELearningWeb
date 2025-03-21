using E_Learning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_Learning.Controllers
{
    public class SLessonController : Controller
    {
        ELEARNINGEntities db_context = new ELEARNINGEntities();
        // GET: Lesson
        public ActionResult Index(EClassroomValidation _DO, int? LHID)
        {
            //Lấy IDHT theo NVID và LHID
            if (User.Identity.IsAuthenticated)
            {
               
                int NVID = MyAuthentication.ID;
                var IDHT = db_context.XNHocTaps.Where(x => x.NVID == NVID && x.LHID == _DO.LHID).Select(g => g.IDHT).FirstOrDefault();

                var res = (from h in db_context.XNHocTaps.Where(x => x.LHID == LHID)
                           join l in db_context.LopHocs on h.LHID equals l.IDLH
                           join n in db_context.NhanViens.Where(x => x.ID == NVID) on h.NVID equals n.ID
                           join p in db_context.PhongBans on h.NhanVien.IDPhongBan equals p.IDPhongBan
                           select new EClassroomValidation
                           {
                               IDHT = h.IDHT,
                               NVID = n.ID,
                               LHID = l.IDLH,
                               MaLH = l.MaLH,
                               TenLH = l.TenLH,
                               TenND = l.NoiDungDT.NoiDung,
                               LinhVuc = l.NoiDungDT.LinhVucDT.TenLVDT,
                               VideoLH = l.NoiDungDT.VideoND,
                               ImageLH = l.NoiDungDT.ImageND,
                               FileCTDT = l.NoiDungDT.FileDinhKem,
                               TGBDLH = (DateTime)l.TGBDLH,
                               TGKTLH = (DateTime)l.TGKTLH,
                               NgayTG = h.NgayTG != null?(DateTime)h.NgayTG:default(DateTime),
                               NgayHT = h.NgayHT != null?(DateTime)h.NgayHT:default(DateTime),
                               XNTG = (bool)h.XNTG,
                               XNHT = (bool)h.XNHT,
                           }).ToList();


                //Check Xác nhận tham gia và Update xác nhận tham gia và ngày tham gia 
                var XNTG = db_context.XNHocTaps.Where(x => x.IDHT == IDHT).Select(g => g.XNTG).FirstOrDefault();
                var PBID = db_context.XNHocTaps.Where(x => x.IDHT == IDHT).Select(g => g.PBID).FirstOrDefault();
                var VTID = db_context.XNHocTaps.Where(x => x.IDHT == IDHT).Select(g => g.VTID).FirstOrDefault();
                if (XNTG == false)
                {
                    db_context.XNHocTap_update(IDHT, NVID, LHID, DateTime.Today, _DO.NgayHT, true, _DO.XNHT, PBID, VTID);
                }
                else
                {
                    TempData["msgSuccess"] = "<script>alert('Bạn muốn tham gia lại bài học');</script>";
                }

                return View(res);
            }else
            {
                return RedirectToAction("", "Login");
            }    
        }


        public ActionResult CompleteLesson(EClassroomValidation _DO, int id)
        {
            //Lấy LHID và NVID theo IDHT
            var IDHT = id;
            var NVID = db_context.XNHocTaps.Where(x => x.IDHT == IDHT).Select(g => g.NVID).FirstOrDefault();
            var PBID = db_context.XNHocTaps.Where(x => x.IDHT == IDHT).Select(g => g.PBID).FirstOrDefault();
            var VTID = db_context.XNHocTaps.Where(x => x.IDHT == IDHT).Select(g => g.VTID).FirstOrDefault();
            var LHID = db_context.XNHocTaps.Where(x => x.IDHT == IDHT).Select(g => g.LHID).FirstOrDefault();
            var NgayTG = db_context.XNHocTaps.Where(x => x.IDHT == IDHT).Select(g => g.NgayTG).FirstOrDefault();
            var XNTG = db_context.XNHocTaps.Where(x => x.IDHT == IDHT).Select(g => g.XNTG).FirstOrDefault();
            db_context.XNHocTap_update(IDHT, NVID,LHID,NgayTG, DateTime.Today, XNTG, true,PBID,VTID);
            TempData["msgSuccess"] = "<script>alert('Chúc mừng bạn đã hoàn thành bài học');</script>";

            return RedirectToAction("Index", "EClassroom");
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