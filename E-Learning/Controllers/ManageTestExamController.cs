using PagedList;
using E_Learning.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.IO;
using System.Linq;
using ExcelDataReader;
using ClosedXML.Excel;
using System.Globalization;
using System.Data;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using E_Learning.Controllers;
using iTextSharp.tool.xml.html;
using iTextSharp.text.html;

namespace E_Learning
{
    public class ManageTestExamController : Controller
    {
        ELEARNINGEntities db_context = new ELEARNINGEntities();
        int Idquyen = MyAuthentication.IDQuyen;
        String ControllerName = "ManageTestExam";
        // GET: ManageExam
        public ActionResult Index(int? page, string search, int? IDND)
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            ViewBag.QUYENCN = ListQuyen;
            //if (!ListQuyen.Contains(CONSTKEY.V))
            //{
            //    TempData["msgError"] = "<script>alert('Bạn không có quyền truy cập chức năng này');</script>";
            //    return RedirectToAction("", "Home");
            //}
            if (User.Identity.IsAuthenticated)
            {
                int id = MyAuthentication.ID;
                //if (MyAuthentication.IDQuyen == 4)
                if(!ListQuyen.Contains(CONSTKEY.V))
                {
                    //return RedirectToAction("", "Login");
                    TempData["msgError"] = "<script>alert('Bạn không có quyền truy cập chức năng này');</script>";
                    return RedirectToAction("", "Home");
                }
                else
                {
                    if (IDND == null) IDND = 0;

                    //var res = (from b in db_context.DeThis
                    //           join nd in db_context.NoiDungDTs on b.IDND equals nd.IDND
                    //           join nv in db_context.NhanViens on b.GVID equals nv.ID
                    //           select new ManageTestExamValidation
                    //           {
                    //               IDDeThi = b.IDDeThi,
                    //               MaDe = b.MaDe,
                    //               TenDe = b.TenDe,
                    //               DiemChuan = (double)b.DiemChuan,
                    //               TongSoCau = (int)b.TongSoCau,
                    //               ThoiGianLamBai = (int)b.ThoiGianLamBai,
                    //               IDND = (int)b.IDND,
                    //               TenND = nd.NoiDung,
                    //               GVID = nv.ID,
                    //               HoTen = nv.HoTen,
                    //               IsGV = (bool)nv.IsGV
                    //           }).OrderByDescending(x => x.IDDeThi).ToList();

                    if (search == null) search = "";
                    ViewBag.search = search;

                    var res = (from b in db_context.NoiDungDT_ListContent(search)
                               join nd in db_context.NoiDungDTs on b.IDND equals nd.IDND
                               join dt in db_context.DeThis on b.IDND equals dt.IDND
                               join nv in db_context.NhanViens on dt.GVID equals nv.ID
                               select new ManageTestExamValidation
                               {
                                   IDDeThi = dt.IDDeThi,
                                   MaDe = dt.MaDe,
                                   TenDe = dt.TenDe,
                                   DiemChuan = (double)dt.DiemChuan,
                                   TongSoCau = (int)dt.TongSoCau,
                                   ThoiGianLamBai = (int)dt.ThoiGianLamBai,
                                   IDND = (int)b.IDND,
                                   TenND = nd.NoiDung,
                                   GVID = nv.ID,
                                   HoTen = nv.HoTen,
                                   IsGV = (bool)nv.IsGV
                               }).OrderByDescending(x => x.IDDeThi).ToList();
                    //if (MyAuthentication.IDQuyen == 2 || MyAuthentication.IDQuyen == 3)
                    //{
                    //    res = res.Where(x => x.GVID == id ).ToList();
                    //}
                    if (ListQuyen.Contains(CONSTKEY.V_GV)) res = res.Where(x => x.GVID == id).ToList();
                    List<NoiDungDT> lh = db_context.NoiDungDTs.ToList();
                    ViewBag.IDND = new SelectList(lh, "IDND", "NoiDung");

                    if (IDND != 0) res = res.Where(x => x.IDND == IDND).ToList();

                    if (page == null) page = 1;
                    int pageSize = 20;
                    int pageNumber = (page ?? 1);
                    return View(res.ToList().ToPagedList(pageNumber, pageSize));
                }    
            } else
            {
                return RedirectToAction("", "Login");
            }    
               
        }
        public ActionResult Create()
        {
            db_context.Configuration.ProxyCreationEnabled = false;
            List<NoiDungDT> bd = db_context.NoiDungDTs.ToList();
            ViewBag.BDList = new SelectList(bd, "IDND", "NoiDung");
            return PartialView();

        }
        [HttpPost]
        public ActionResult Create(ManageTestExamValidation _DO)
        {
            try
            {
                db_context.DeThi_insert(_DO.MaDe, _DO.TenDe, _DO.DiemChuan, _DO.ThoiGianLamBai, _DO.IDND, MyAuthentication.ID, _DO.TongSoCau);
                TempData["msgSuccess"] = "<script>alert('Thêm mới thành công');</script>";
            }

            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Có lỗi khi thêm mới: " + e.Message + "');</script>";
            }
            return RedirectToAction("Index", "ManageTestExam");
        }
        public bool IsDAAvailable(int IDDSĐA)
        {
            var IsCheck = (from c in db_context.DanhSachDAs
                           where (c.IDDSĐA == IDDSĐA)
                           select new { c.IDDSĐA }).FirstOrDefault();
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
        public bool IsBTAvailable(string MaDe)
        {
            var IsCheck = (from b in db_context.DeThis
                           where (b.MaDe.ToLower() == MaDe)
                           select new { b.MaDe }).FirstOrDefault();
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
            var res = (from c in db_context.DeThi_searchByID(id)
                       select new ManageTestExamValidation
                       {
                           IDDeThi = c.IDDeThi,
                           MaDe = c.MaDe,
                           TenDe = c.TenDe,
                           DiemChuan = (double)c.DiemChuan,
                           TongSoCau = (int)c.TongSoCau,
                           ThoiGianLamBai = (int)c.ThoiGianLamBai,
                           IDND = (int)c.IDND,
                           GVID = (int)c.GVID
                       }).ToList();
            ManageTestExamValidation DO = new ManageTestExamValidation();

            if (res.Count > 0)
            {
                foreach (var co in res)
                {
                    DO.IDDeThi = co.IDDeThi;
                    DO.MaDe = co.MaDe;
                    DO.TenDe = co.TenDe;
                    DO.DiemChuan = (double)co.DiemChuan;
                    DO.TongSoCau = (int)co.TongSoCau;
                    DO.ThoiGianLamBai = (int)co.ThoiGianLamBai;
                    DO.GVID = (int)co.GVID;
                    DO.IDND = (int)co.IDND;
                }
                db_context.Configuration.ProxyCreationEnabled = false;
                List<NoiDungDT> bd = db_context.NoiDungDTs.ToList();
                ViewBag.BDList = new SelectList(bd, "IDND", "NoiDung", DO.IDND);
            }
            else
            {
                return HttpNotFound();
            }
            return PartialView(DO);
        }
        [HttpPost]
        public ActionResult Edit(ManageTestExamValidation _DO)
        {

            try
            {

                db_context.DeThi_Update(_DO.IDDeThi, _DO.MaDe, _DO.TenDe, _DO.DiemChuan, _DO.ThoiGianLamBai, _DO.IDND, _DO.TongSoCau);

                TempData["msgSuccess"] = "<script>alert('Cập nhập thành công');</script>";
            }
            catch (Exception e)
            {
                TempData["msgSuccess"] = "<script>alert('Cập nhập thất bại " + e.Message + " ');</script>";
            }


            return RedirectToAction("Index", "ManageTestExam");
        }

        public ActionResult Delete(int id)
        {

            try
            {
                var m_Class = db_context.LopHocs.Where(x => x.IDDeThi == id).FirstOrDefault();
                if (m_Class == null)
                {
                    db_context.DeThi_delete(id);
                }
                else
                {
                    TempData["msgSuccess"] = "<script>alert('Đề thi đã được gắn vào lớp học');</script>";
                }
            }
            catch (Exception e)
            {
                TempData["msgSuccess"] = "<script>alert('Xóa dữ liệu thất bại: " + e.Message + "');</script>";
            }
            return RedirectToAction("Index", "ManageTestExam");
        }
        public ActionResult Question(int id)
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            ViewBag.QUYENCN = ListQuyen;
            if (!ListQuyen.Contains(CONSTKEY.V))
            {
                TempData["msgError"] = "<script>alert('Bạn không có quyền thực hiện chức năng này');</script>";
                return RedirectToAction("", "Home");
            }
            var model = (from a in db_context.CauHoiDeThis.Where(x => x.IDDeThi == id)
                         join b in db_context.CauHois on a.IDCauHoi equals b.IDCH
                         join c in db_context.DanhSachDAs on b.IDDAĐung equals c.IDDSĐA
                         select new ManageQuestionValidation
                         {
                             IDCH = (int)a.IDCauHoi,
                             NoiDungCH = b.NoiDungCH,
                             DapAnA = b.DapAnA,
                             DapAnB = b.DapAnB,
                             DapAnC = b.DapAnC,
                             DapAnD = b.DapAnD,
                             DapAnĐung = c.TenĐA,
                             Diem = (double)a.Diem,
                             IDCauHoiDeThi=a.IDCauHoiDeThi

                         }).ToList();

            return View(model);
        }

        public ActionResult ViewQuestion(int id)
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            if (!ListQuyen.Contains(CONSTKEY.V))
            {
                TempData["msgError"] = "<script>alert('Bạn không có quyền thực hiện chức năng này');</script>";
                return RedirectToAction("", "Home");
            }
            var model = (from a in db_context.CauHoiDeThis.Where(x => x.IDDeThi == id)
                         join b in db_context.CauHois on a.IDCauHoi equals b.IDCH
                         join c in db_context.DanhSachDAs on b.IDDAĐung equals c.IDDSĐA
                         select new ManageQuestionValidation
                         {
                             IDCH = (int)a.IDCauHoi,
                             NoiDungCH = b.NoiDungCH,
                             DapAnA = b.DapAnA,
                             DapAnB = b.DapAnB,
                             DapAnC = b.DapAnC,
                             DapAnD = b.DapAnD,
                             DapAnĐung = c.TenĐA,
                             Diem = (double)a.Diem,
                             IDCauHoiDeThi = a.IDCauHoiDeThi
                         }).ToList();

            return View(model);
        }
        public string StripHTML(string html)
        {
            if (string.IsNullOrWhiteSpace(html)) return "";

            // could be stored in static variable
            var regex = new Regex("<[^>]+>|\\s{2}", RegexOptions.IgnoreCase);
            return System.Web.HttpUtility.HtmlDecode(regex.Replace(html, ""));
        }
        public ActionResult AddQuestion(int id)
        {

            
            
            var dethi = db_context.DeThis.Where(x => x.IDDeThi == id).FirstOrDefault();
            var ND = db_context.NoiDungDTs.Where(x => x.IDND == dethi.IDND).FirstOrDefault();
            //var lch = db_context.CauHois.Where(x => x.IDND == ND.IDND).ToList();
            var lch = (from a in db_context.CauHoi_DeThi_list(id).Where(x=>x.IDND== dethi.IDND)
                       select new CauHoi
                       {
                           IDCH = a.IDCH,
                           NoiDungCH = StripHTML(a.NoiDungCH)
        }
                       ).ToList();

            db_context.Configuration.ProxyCreationEnabled = false;
            ViewBag.CHList = new SelectList(lch,  "IDCH", "NoiDungCH");

            db_context.Configuration.ProxyCreationEnabled = false;
            List<NoiDungDT> nddt = db_context.NoiDungDTs.Where(x => x.IDND == ND.IDND).ToList();
            ViewBag.IDND = new SelectList(nddt, "IDND", "NoiDung", ND.IDND);


            return PartialView();
        }
        [HttpPost]
        public ActionResult AddQuestion(CauHoiDeThiValidation _DO, int? id)
        {
            var SLCau = db_context.CauHoiDeThis.Where(x => x.IDDeThi == id).Count();
            var IDDT = db_context.DeThis.Where(x => x.IDDeThi == id).SingleOrDefault();
            var Tong = db_context.CauHoiDeThis.Where(x => x.IDDeThi == id).Sum(x => x.Diem);
            if(Tong==null) Tong = 0;
            var TongDiem = _DO.Diem + Tong;
            try
            {
                if (SLCau < IDDT.TongSoCau && TongDiem <= 10)
                {
                    db_context.CauHoiDeThi_insert(_DO.IDCauHoi, id, _DO.Diem);

                    TempData["msgSuccess"] = "<script>alert('Thêm mới thành công');</script>";
                }
                else if (SLCau >= IDDT.TongSoCau && TongDiem <= 10)
                {
                    TempData["msgSuccess"] = "<script>alert('Vui lòng xem lại Tổng số câu hỏi');</script>";
                }
                else if (TongDiem > 10 && SLCau < IDDT.TongSoCau)
                {
                    TempData["msgSuccess"] = "<script>alert('Vui lòng xem lại Tổng số điểm');</script>";
                }
                else if (SLCau >= IDDT.TongSoCau && TongDiem > 10)
                {
                    TempData["msgSuccess"] = "<script>alert('Vui lòng xem lại Tổng số câu hỏi và số điểm');</script>";
                }

            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Có lỗi khi thêm mới: " + e.Message + "');</script>";
            }
            //return View();
            return RedirectToAction("Question", "ManageTestExam", new { id = id });
        }
        public ActionResult DeleteQuestion(int? id)
        {
            var idb = db_context.CauHoiDeThis.Where(x => x.IDCauHoiDeThi == id).Select(g => g.IDDeThi).FirstOrDefault();
            try
            {
                db_context.CauHoiDeThi_delete(id);
            }
            catch (Exception e)
            {
                TempData["msgSuccess"] = "<script>alert('Xóa dữ liệu thất bại: " + e.Message + "');</script>";
            }
            return RedirectToAction("Question", "ManageTestExam", new { id = idb });
        }
    }
}