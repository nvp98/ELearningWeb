using DocumentFormat.OpenXml.Office2010.Excel;
using E_Learning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_Learning.Controllers.QTUX
{
    public class QTUXUserController : Controller
    {
        ELEARNINGEntities db_context = new ELEARNINGEntities();

        // GET: QTUXUser - Xem video/tài liệu Quy Tắc Ứng Xử
        public ActionResult Index(EClassroomValidation _DO, int? id)
        {
            if (User.Identity.IsAuthenticated)
            {
                int? NVID = MyAuthentication.ID;

                var kq = db_context.QTUX_KetQua.Where(x => x.NhanVien.ID == NVID && x.NoiDungDT.IDND == id).ToList();
                if (kq.Count == 0 && NVID != null && id != null)
                {
                    //db_context.QTUX_KetQua__insert(id, 1, null, null, NVID, DateTime.Now, null, 0);
                }

                var res = (from n in db_context.NoiDungDTs.Where(x => x.IDND == id && x.isNQ == 2)
                           join b in db_context.QTUX_KetQua.Where(x => x.NhanVien.ID == NVID && x.NoiDungDT.IDND == id) on n.IDND equals b.NoiDungDT.IDND into ul
                           from b in ul.DefaultIfEmpty()
                           select new NoiQuyUXKQView
                           {
                               IDND = n.IDND,
                               MaND = n.MaND,
                               NoiDung = n.NoiDung,
                               ImageND = n.ImageND,
                               VideoND = n.VideoND,
                               XNHT = b.XNHT ?? 0,
                               XNTG = b.XNTG,
                               XNHTFile = b.XNHTFile,
                               FileDinhKem = n.FileDinhKem,
                               TinhTrang = b.TinhTrang
                           }).ToList();

                return View(res);
            }
            else
            {
                return RedirectToAction("", "Login");
            }
        }

        // Hoàn thành xem video
        [HttpPost]
        public ActionResult CompleteLesson(int id)
        {
            try
            {
                int NVID = MyAuthentication.ID;
                var record = db_context.QTUX_KetQua.Where(x => x.NhanVien.ID == NVID && x.NoiDungDT.IDND == id).FirstOrDefault();
                if (record != null)
                {
                    record.XNHT = 1;
                    record.NgayHT = DateTime.Now;
                    record.TinhTrang = 1;
                    db_context.SaveChanges();
                    TempData["msgSuccess"] = "<script>alert('Hoàn thành xem video thành công!');</script>";
                }
                // neu record null thi co the insert ban ghi moi vao ket qua voi XNHT = 1, ngayht = now, tinhtrang = 1
                else
                {
                    db_context.QTUX_KetQua__insert(id, NVID, 0, 1, 0, null, DateTime.Now, 0, null);
                    TempData["msgSuccess"] = "<script>alert('Hoàn thành xem video thành công!');</script>";
                }
            }
            catch (Exception ex)
            {
                TempData["msgError"] = "<script>alert('Lỗi: " + ex.Message + "');</script>";
            }
            return RedirectToAction("ListQTUX", "QTUXView");
        }

        // Hoàn thành xem tài liệu
        [HttpGet]
        public ActionResult CompleteFile(int id, string link)
        {
            try
            {
                int NVID = MyAuthentication.ID;
                var record = db_context.QTUX_KetQua.Where(x => x.NhanVien.ID == NVID && x.NoiDungDT.IDND == id).FirstOrDefault();
                if (record != null)
                {
                    record.XNHTFile = 1;
                    //record.NgayTG = DateTime.Now;
                    db_context.SaveChanges();
                }
                else
                {
                    db_context.QTUX_KetQua__insert(id, NVID, 0, 0, 1, null, DateTime.Now, 0, null);
                    //TempData["msgSuccess"] = "<script>alert('Hoàn thành xem video thành công!');</script>";
                }
            }
            catch (Exception ex)
            {
                TempData["msgError"] = "<script>alert('Lỗi: " + ex.Message + "');</script>";
            }
            return Redirect(link);
        }

        // Trang quay lại sau khi hoàn thành
        public ActionResult AfterComplete(int id)
        {
            TempData["msgSuccess"] = "<script>alert('Bạn đã hoàn thành xem quy tắc ứng xử!');</script>";
            return RedirectToAction("ListQTUX", "QTUXView");
        }
    }
}
