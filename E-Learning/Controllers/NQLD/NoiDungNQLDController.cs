using E_Learning.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_Learning.Controllers.NQLD
{
    public class NoiDungNQLDController : Controller
    {
        ELEARNINGEntities db_context = new ELEARNINGEntities();
        int Idquyen = MyAuthentication.IDQuyen;
        String ControllerName = "NoiDungNQLD";
        // GET: NoiDungNQLD
        public ActionResult Index(int? page, string search, int? IDNQLD)
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

            if (IDNQLD == null) IDNQLD = 0;

            var res = (from a in db_context.NoiDungDTs.Where(x=>x.isNQ == 1)
                       select new NoiQuyView
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
                           SLHT = db_context.NQ_KetQua.Where(x=>x.NDDTID == a.IDND && x.TinhTrang ==1).Count(),
                           SLHTFile = db_context.NQ_KetQua.Where(x => x.NDDTID == a.IDND && x.XNHTFile == 1).Count(),
                       }).OrderBy(x=>x.isOrder).ToList();

            List<NoiDungDT> ctlvdt = db_context.NoiDungDTs.Where(x=>x.isNQ ==1 ).OrderBy(x => x.isOrder).ToList();
            ViewBag.IDNQLD = new SelectList(ctlvdt, "IDND", "NoiDung");
            if(IDNQLD != 0) res = res.Where(x=>x.IDND == IDNQLD).ToList();

            if (page == null) page = 1;
            int pageSize = 50;
            int pageNumber = (page ?? 1);
            return View(res.ToList().ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Create()
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            if (!ListQuyen.Contains(CONSTKEY.ADD))
            {
                TempData["msgError"] = "<script>alert('Bạn không có quyền thực hiện chức năng này');</script>";
                return RedirectToAction("", "Home");
            }
            var lastRecord = (from c in db_context.NoiDungDTs orderby c.IDND descending select c).FirstOrDefault();
            if (lastRecord == null)
            {
                ViewBag.MaND = "NDĐT0000" + 1;
            }
            else if (Convert.ToInt32(lastRecord.IDND) < 9)
            {
                ViewBag.MaND = "NDĐT0000" + (Convert.ToInt32(lastRecord.IDND) + 1);
            }
            else if (Convert.ToInt32(lastRecord.IDND) < 99)
            {
                ViewBag.MaND = "NDĐT000" + (Convert.ToInt32(lastRecord.IDND) + 1);
            }
            else if (Convert.ToInt32(lastRecord.IDND) < 999)
            {
                ViewBag.MaND = "NDĐT00" + (Convert.ToInt32(lastRecord.IDND) + 1);
            }
            else if (Convert.ToInt32(lastRecord.IDND) < 9999)
            {
                ViewBag.MaND = "NDĐT0" + (Convert.ToInt32(lastRecord.IDND) + 1);
            }
            else
            {
                ViewBag.MaND = "NDĐT" + (Convert.ToInt32(lastRecord.IDND) + 1);
            }

            //List<LinhVucDT> lv = db_context.LinhVucDTs.ToList();
            //ViewBag.LVList = new SelectList(lv, "IDLVDT", "TenLVDT");


            //List<PhongBan> bp = db_context.PhongBans.ToList();
            //ViewBag.BPLList = new SelectList(bp, "IDPhongBan", "TenPhongBan");

            return PartialView();
        }

        [HttpPost]
        public ActionResult Create(NoiQuyView _DO)
        {

            try
            {
                string path = Server.MapPath("~/UploadedFiles/EduPro/");

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                //Use Namespace called :  System.IO  
                string FileName = _DO.PDFEduFile != null ? DateTime.Now.ToString("ddMMyyHHmmss")+ _DO.MaND : "";

                //To Get File Extension  
                string FileExtension = _DO.PDFEduFile != null ? Path.GetExtension(_DO.PDFEduFile.FileName) : "";

                ////Add Current Date To Attached File Name  
                if (_DO.PDFEduFile != null)
                {
                    FileName = FileName.Trim()  + FileExtension;
                    _DO.PDFEduFile.SaveAs(path + FileName);
                    _DO.FileDinhKem = "/UploadedFiles/EduPro/" + FileName;
                }
                //Check trùng mã Nội dung
                if (IsNDAvailable(_DO.MaND) == false)
                {
                    db_context.NoiDungDT_insert_NQLD(_DO.MaND, _DO.NoiDung, _DO.VideoND, _DO.ImageND,_DO.ThoiLuongDT, _DO.FileDinhKem, _DO.NgayTao,_DO.isOrder,1);
                    TempData["msgSuccess"] = "<script>alert('Thêm mới thành công');</script>";
                }
                else
                {
                    TempData["msgSuccess"] = "<script>alert('Chương trình đã tồn tại');</script>";
                }
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Có lỗi khi thêm mới: " + e.Message + "');</script>";
            }
            //return View();
            return RedirectToAction("Index", "NoiDungNQLD");
        }

        public bool IsNDAvailable(string MaND)
        {
            var IsCheck = (from k in db_context.NoiDungDTs
                           where (k.MaND.ToLower() == MaND)
                           select new { k.MaND }).FirstOrDefault();
            bool status;
            if (IsCheck != null)
            {

                status = true;
            }
            else
            {

                status = false;
            }
            return status;
        }

        public ActionResult Edit(int id)
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            if (!ListQuyen.Contains(CONSTKEY.EDIT))
            {
                TempData["msgError"] = "<script>alert('Bạn không có quyền thực hiện chức năng này');</script>";
                return RedirectToAction("", "Home");
            }
            var res = (from a in db_context.NoiDungDTs.Where(x => x.isNQ == 1 && x.IDND == id)
                       select new NoiQuyView
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
                       }).ToList();
            NoiQuyView DO = new NoiQuyView();

            if (res.Count > 0)
            {
                foreach (var co in res)
                {

                    DO.IDND = co.IDND;
                    DO.MaND = co.MaND;
                    DO.NoiDung = co.NoiDung;
                    DO.VideoND = co.VideoND;
                    DO.ImageND = co.ImageND;
                    DO.ThoiLuongDT = co.ThoiLuongDT;
                    DO.FileDinhKem = co.FileDinhKem;
                    DO.NgayTao = co.NgayTao;
                    DO.isOrder = co.isOrder;    
                }
               
                ViewBag.NgayTao = DO.NgayTao?.ToString("yyyy-MM-dd");
            }
            else
            {
                return HttpNotFound();
            }
            return PartialView(DO);
        }

        [HttpPost]
        public ActionResult Edit(NoiQuyView _DO)
        {
            try
            {
                string path = Server.MapPath("~/UploadedFiles/EduPro/");

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                //Use Namespace called :  System.IO  
                string FileName = _DO.PDFEduFile != null ? DateTime.Now.ToString("ddMMyyHHmmss")+ _DO.MaND : "";

                //To Get File Extension  
                string FileExtension = _DO.PDFEduFile != null ? Path.GetExtension(_DO.PDFEduFile.FileName) : "";


                //Add Current Date To Attached File Name  
                if (_DO.PDFEduFile != null)
                {
                    FileName = FileName.Trim() + FileExtension;
                    _DO.PDFEduFile.SaveAs(path + FileName);
                    _DO.FileDinhKem = "/UploadedFiles/EduPro/" + FileName;
                }

                db_context.NoiDungDT_updateNQLD(_DO.IDND, _DO.MaND, _DO.NoiDung, _DO.VideoND, _DO.ImageND, _DO.ThoiLuongDT, _DO.FileDinhKem, _DO.NgayTao,_DO.isOrder);

                TempData["msgSuccess"] = "<script>alert('Cập nhập thành công');</script>";
            }
            catch (Exception e)
            {
                TempData["msgSuccess"] = "<script>alert('Cập nhập thất bại " + e.Message + " ');</script>";
            }

            return RedirectToAction("Index", "NoiDungNQLD");
        }
        public ActionResult Delete(int id)
        {
            try
            {
                db_context.NoiDungDT_delete(id);
            }
            catch (Exception e)
            {
                TempData["msgSuccess"] = "<script>alert('Xóa dữ liệu thất bại: " + e.Message + "');</script>";
            }
            return RedirectToAction("Index", "NoiDungNQLD");
        }

    }
}