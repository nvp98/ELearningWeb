using PagedList;
using E_Learning.Models;
using System;
using System.Data;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity.Core.Objects;
using System.Reflection;
using System.Globalization;

namespace E_Learning.Controllers
{
    public class ManageETContentController : Controller
    {
        ELEARNINGEntities db_context = new ELEARNINGEntities();
        int Idquyen = MyAuthentication.IDQuyen;
        String ControllerName = "ManageETContent";
        // GET: Courses
        public ActionResult Index(int? page, string search, int? IDLVDT , int? IDCTLVDT)
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

            if (IDCTLVDT == null) IDCTLVDT = 0;
            if (IDLVDT == null) IDLVDT = 0;

            var res = (from a in db_context.NoiDungDT_ListContent(search)
                      select new ManageETContentValidation
                      {
                          IDND = a.IDND,
                          MaND = a.MaND,
                          NoiDung = a.NoiDung,
                          VideoND = a.VideoND,
                          ImageND = a.ImageND,
                          LVDTID = (int)a.LVDTID,
                          LinhVuc = a.TenLVDT,
                          IDCTLVDT = (int)a.IDCTLVDT,
                          LVChiTiet = a.TenCTLVDT,
                          BPLNC = a.TenPhongBan,
                          ThoiLuongDT = (int)a.ThoiLuongDT,
                          FileDinhKem = a.FileDinhKem,
                          SLLH = a.SLLH,
                          NgayTao =a.NgayTao,
                      }).ToList();

            List<CTLVDT> ctlvdt = db_context.CTLVDTs.ToList();
            ViewBag.CTLVDTID = new SelectList(ctlvdt, "IDCTLVDT", "TenCTLVDT");

            List<LinhVucDT> lh = db_context.LinhVucDTs.ToList();
            ViewBag.IDLVDT = new SelectList(lh, "IDLVDT", "TenLVDT");

            if (IDCTLVDT != 0) res = res.Where(x => x.IDCTLVDT == IDCTLVDT).ToList();
            if (IDLVDT != 0) res = res.Where(x => x.LVDTID == IDLVDT).ToList();

            if (page == null) page = 1;
            int pageSize = 50;
            int pageNumber = (page ?? 1);
            return View(res.ToList().ToPagedList(pageNumber, pageSize));
        }


        public JsonResult GetLVChiTiet(int id)
        {
            db_context.Configuration.ProxyCreationEnabled = false;
            List<CTLVDT> gcl = db_context.CTLVDTs.Where(x => x.LVDTID == id).ToList();
            return Json(gcl, JsonRequestBehavior.AllowGet);
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

            List<LinhVucDT> lv = db_context.LinhVucDTs.ToList();
            ViewBag.LVList = new SelectList(lv, "IDLVDT", "TenLVDT");
            ViewBag.IDNhomNL = new SelectList(db_context.NhomNLKCCDs, "ID", "NoiDung");

            List<PhongBan> bp = db_context.PhongBans.ToList();
            ViewBag.BPLList = new SelectList(bp, "IDPhongBan", "TenPhongBan");

            return PartialView();
        }

        [HttpPost]
        public ActionResult Create(ManageETContentValidation _DO)
        {

            try
            {
                string path = Server.MapPath("~/UploadedFiles/EduPro/");

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                //Use Namespace called :  System.IO  
                string FileName = _DO.PDFEduFile != null ? _DO.MaND : "";

                //To Get File Extension  
                string FileExtension = _DO.PDFEduFile != null ? Path.GetExtension(_DO.PDFEduFile.FileName) : "";

                ////Add Current Date To Attached File Name  
                if (_DO.PDFEduFile != null)
                {
                    FileName = FileName.Trim() + FileExtension;
                    _DO.PDFEduFile.SaveAs(path + FileName);
                    _DO.FileDinhKem = "/UploadedFiles/EduPro/" + FileName;
                }
                //Check trùng mã Nội dung
                if (IsNDAvailable(_DO.MaND) == false)
                {
                    db_context.NoiDungDT_insert(_DO.MaND, _DO.NoiDung, _DO.VideoND, _DO.ImageND, _DO.BPLID, _DO.LVDTID, _DO.IDCTLVDT, _DO.ThoiLuongDT, _DO.FileDinhKem,_DO.NgayTao);
                    // cập nhật nhóm NL
                    var checknd = db_context.NoiDungDTs.Where(x=>x.MaND == _DO.MaND).FirstOrDefault();
                    if (checknd != null)
                    {
                        checknd.IDNguonGV = 1;
                        checknd.IDPhuongPhapDT = 1;
                        checknd.IDHoatDongDT = 2;
                        checknd.IDPhanLoaiDT = 1;
                        checknd.IDNhomNL = _DO.IDNhomNL;
                    }
                    db_context.SaveChanges();
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
            return RedirectToAction("Index", "ManageETContent");
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
            var res = (from c in db_context.NoiDungDT_searchByID(id)
                       select new ManageETContentValidation
                       {
                           IDND = c.IDND,
                           MaND = c.MaND,
                           NoiDung = c.NoiDung,
                           VideoND = c.VideoND,
                           ImageND = c.ImageND,
                           BPLID = (int)c.BPLID,
                           LVDTID = (int)c.LVDTID,
                           IDCTLVDT = (int)c.IDCTLVDT,
                           ThoiLuongDT = (int)c.ThoiLuongDT,
                           FileDinhKem = c.FileDinhKem,
                           NgayTao =c.NgayTao
                       }).ToList();
            ManageETContentValidation DO = new ManageETContentValidation();

            if (res.Count > 0)
            {
                foreach (var co in res)
                {

                    DO.IDND = co.IDND;
                    DO.MaND = co.MaND;
                    DO.NoiDung = co.NoiDung;
                    DO.VideoND = co.VideoND;
                    DO.ImageND = co.ImageND;
                    DO.BPLID = co.BPLID;
                    DO.LVDTID = co.LVDTID;
                    DO.IDCTLVDT = co.IDCTLVDT;
                    DO.ThoiLuongDT = co.ThoiLuongDT;
                    DO.FileDinhKem = co.FileDinhKem;
                    DO.NgayTao =co.NgayTao;
                }

                List<LinhVucDT> lv = db_context.LinhVucDTs.ToList();
                ViewBag.LVList = new SelectList(lv, "IDLVDT", "TenLVDT", DO.LVDTID);
                ViewBag.IDNhomNL = new SelectList(db_context.NhomNLKCCDs, "ID", "NoiDung");
                List<CTLVDT> CTLV = db_context.CTLVDTs.Where(x => x.LVDTID == DO.LVDTID).ToList();
                ViewBag.CTLVList = new SelectList(CTLV, "IDCTLVDT", "TenCTLVDT", DO.IDCTLVDT);

                List<PhongBan> bp = db_context.PhongBans.ToList();
                ViewBag.BPLList = new SelectList(bp, "IDPhongBan", "TenPhongBan");
               ViewBag.NgayTao =  DO.NgayTao?.ToString("yyyy-MM-dd");
            }
            else
            {
                return HttpNotFound();
            }
            return PartialView(DO);
        }

        [HttpPost]
        public ActionResult Edit(ManageETContentValidation _DO)
        {
            try
            {
                string path = Server.MapPath("~/UploadedFiles/EduPro/");

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                //Use Namespace called :  System.IO  
                string FileName = _DO.PDFEduFile != null ? _DO.MaND : "";

                //To Get File Extension  
                string FileExtension = _DO.PDFEduFile != null ? Path.GetExtension(_DO.PDFEduFile.FileName) : "";


                //Add Current Date To Attached File Name  
                if (_DO.PDFEduFile != null)
                {
                    FileName = FileName.Trim() + FileExtension;
                    _DO.PDFEduFile.SaveAs(path + FileName);
                    _DO.FileDinhKem = "/UploadedFiles/EduPro/" + FileName;
                }

                db_context.NoiDungDT_update(_DO.IDND, _DO.MaND, _DO.NoiDung, _DO.VideoND, _DO.ImageND, _DO.BPLID, _DO.LVDTID, _DO.IDCTLVDT, _DO.ThoiLuongDT, _DO.FileDinhKem,_DO.NgayTao);
                var checknd = db_context.NoiDungDTs.Where(x => x.IDND == _DO.IDND).FirstOrDefault();
                if (checknd != null)
                {
                    checknd.IDNhomNL = _DO.IDNhomNL;
                }
                db_context.SaveChanges();

                TempData["msgSuccess"] = "<script>alert('Cập nhập thành công');</script>";
            }
            catch (Exception e)
            {
                TempData["msgSuccess"] = "<script>alert('Cập nhập thất bại " + e.Message + " ');</script>";
            }

            return RedirectToAction("Index", "ManageETContent");
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
            return RedirectToAction("Index", "ManageETContent");
        }

    }

}