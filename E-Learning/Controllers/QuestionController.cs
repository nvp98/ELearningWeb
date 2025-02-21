using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using E_Learning.Models;
using E_Learning.Common;
using ExcelDataReader;
using ClosedXML.Excel;
using System.Data.Entity.Core.Objects;
using System.Globalization;
using PagedList;
using System.Data;
using System.Web.Hosting;

namespace E_Learning.Controllers
{
    public class QuestionController : Controller
    {
        // GET: Question
        ELEARNINGEntities db_context = new ELEARNINGEntities();
        int Idquyen = MyAuthentication.IDQuyen;
        String ControllerName = "Question";
        public ActionResult Index(int? page, int? IDND)
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            ViewBag.QUYENCN = ListQuyen;
            if (!ListQuyen.Contains(CONSTKEY.V))
            {
                TempData["msgError"] = "<script>alert('Bạn không có quyền truy cập chức năng này');</script>";
                return RedirectToAction("", "Home");
            }
            if (User.Identity.IsAuthenticated)
            {
                int id = MyAuthentication.ID;
                var res = (from c in db_context.CauHois
                           join d in db_context.DanhSachDAs on c.IDDAĐung equals d.IDDSĐA
                           select new ManageQuestionValidation
                           {
                               IDCH = c.IDCH,
                               NoiDungCH = c.NoiDungCH,
                               DapAnA = c.DapAnA,
                               DapAnB = c.DapAnB,
                               DapAnC = c.DapAnC,
                               DapAnD = c.DapAnD,
                               IDDAĐung = (int)c.IDDAĐung,
                               DapAnĐung = d.TenĐA,
                               IDND = (int)c.IDND,
                               GVID = (int)c.GVID,
                           }).ToList();

                if (page == null) page = 1;
                int pageSize = 20;
                int pageNumber = (page ?? 1);
                //if (MyAuthentication.IDQuyen == 2 || MyAuthentication.IDQuyen == 3)
                //{
                //    res = res.Where(x => x.GVID == id).ToList();
                //}
                if (ListQuyen.Contains(CONSTKEY.V_GV)) res = res.Where(x => x.GVID == id).ToList();
                List<NoiDungDT> dt = db_context.NoiDungDTs.ToList();
                if (IDND == null) IDND = 0;
                if (IDND != 0) { res = res.Where(x => x.IDND == IDND).ToList(); }
                ViewBag.IDND = new SelectList(dt, "IDND", "NoiDung", IDND);
                return View(res.ToList().ToPagedList(pageNumber, pageSize));

            }
            else { return RedirectToAction("", "Login"); }            
            
        }
        public JsonResult GetCTLVDT(int IDLVDT)
        {
            db_context.Configuration.ProxyCreationEnabled = false;
            List<CTLVDT> IDHuyen = db_context.CTLVDTs.Where(x => x.LVDTID == IDLVDT).ToList();
            CTLVDT h = new CTLVDT();
            h.IDCTLVDT = 0;
            h.TenCTLVDT = "--Chọn Lĩnh vực chi tiết--";
            h.LVDTID = IDLVDT;
            IDHuyen.Add(h);

            return Json(IDHuyen.OrderBy(x => x.IDCTLVDT).ToList(), JsonRequestBehavior.AllowGet);
        }
        public ActionResult Create()
        {
            var lastRecord = (from b in db_context.CauHois orderby b.IDCH descending select b).FirstOrDefault();
            if (lastRecord == null)
            {
                ViewBag.MaCH = "CH0000" + 1;
            }
            else if (Convert.ToInt32(lastRecord.IDCH) < 9)
            {
                ViewBag.MaCH = "CH0000" + (Convert.ToInt32(lastRecord.IDCH) + 1);
            }
            else if (Convert.ToInt32(lastRecord.IDCH) < 99)
            {
                ViewBag.MaCH = "CH000" + (Convert.ToInt32(lastRecord.IDCH) + 1);
            }
            else if (Convert.ToInt32(lastRecord.IDCH) < 999)
            {
                ViewBag.MaCH = "CH00" + (Convert.ToInt32(lastRecord.IDCH) + 1);
            }
            else if (Convert.ToInt32(lastRecord.IDCH) < 9999)
            {
                ViewBag.MaCH = "CH0" + (Convert.ToInt32(lastRecord.IDCH) + 1);
            }
            else
            {
                ViewBag.MaCH = "CH" + (Convert.ToInt32(lastRecord.IDCH) + 1);
            }

            db_context.Configuration.ProxyCreationEnabled = false;
            List<NoiDungDT> dt = db_context.NoiDungDTs.ToList();
            ViewBag.IDND = new SelectList(dt, "IDND", "NoiDung");

            List<DanhSachDA> ds = db_context.DanhSachDAs.ToList();
            ViewBag.DSList = new SelectList(ds, "IDDSĐA", "TenĐA");

            //var lCTLVDT = new List<CTLVDT>();
            //CTLVDT h = new CTLVDT();
            //h.LVDTID = 0;
            //h.TenCTLVDT = "--Chọn Lĩnh vực chi tiết--";
            //h.IDCTLVDT = 0;
            //lCTLVDT.Add(h);
            //ViewBag.IDCTLVDT = new SelectList(lCTLVDT, "IDCTLVDT", "TenCTLVDT", 0);

            return PartialView();
        }

        [HttpPost]
        public ActionResult Create(ManageQuestionValidation _DO)
        {
            try
            {
                db_context.CauHoi_insert(_DO.NoiDungCH, _DO.DapAnA, _DO.DapAnB, _DO.DapAnC, _DO.DapAnD, _DO.IDDAĐung,_DO.IDND,MyAuthentication.ID);
                TempData["msgSuccess"] = "<script>alert('Thêm mới thành công');</script>";
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Có lỗi khi thêm mới: " + e.Message + "');</script>";
            }
            //return View();
            return RedirectToAction("Index", "Question");
        }

        public ActionResult Upload()
        {
            var file = Request.Files["file"];
            //string extension = Path.GetExtension(file.FileName);
            //string fileid = Guid.NewGuid().ToString();
            //fileid = Path.ChangeExtension(fileid, extension);
            var filename1 = Path.GetFileName(file.FileName);
            string location = Server.MapPath(@"~\UploadedFiles\ImagesUpload\" + filename1);
            file.SaveAs(location);

            return Json(new { location = Url.Content("/UploadedFiles/ImagesUpload/" + filename1) }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit(int id)
        {
            var res = (from c in db_context.CauHoi_searchByID(id)
                       join d in db_context.DanhSachDAs on c.IDDAĐung equals d.IDDSĐA
                     
                       select new ManageQuestionValidation
                       {
                           IDCH = c.IDCH,
                           NoiDungCH = c.NoiDungCH,
                           DapAnA = c.DapAnA,
                           DapAnB = c.DapAnB,
                           DapAnC = c.DapAnC,
                           DapAnD = c.DapAnD,
                           IDDAĐung = (int)c.IDDAĐung,
                           DapAnĐung = d.TenĐA,
                           IDND=(int)c.IDND

                       }).ToList();
            ManageQuestionValidation DO = new ManageQuestionValidation();

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
                    DO.IDND = (int)c.IDND;

                }

                db_context.Configuration.ProxyCreationEnabled = false;
                List<NoiDungDT> dt = db_context.NoiDungDTs.ToList();
                ViewBag.IDND = new SelectList(dt, "IDND", "NoiDung", DO.IDND);

                List<DanhSachDA> ds = db_context.DanhSachDAs.ToList();
                ViewBag.IDDAĐung = new SelectList(ds, "IDDSĐA", "TenĐA", DO.IDDAĐung);

                //List<CTLVDT> lvct = db_context.CTLVDTs.Where(x=>x.LVDTID==DO.IDLVDT).ToList();
                //ViewBag.IDCTLVDT = new SelectList(lvct, "IDCTLVDT", "TenCTLVDT", DO.IDCTLVDT);

            }
            else
            {
                return HttpNotFound();
            }
            return PartialView(DO);
        }
        [HttpPost]
        public ActionResult Edit(ManageQuestionValidation _DO)
        {
            try
            {

                db_context.CauHoi_update(_DO.IDCH, _DO.NoiDungCH, _DO.DapAnA, _DO.DapAnB, _DO.DapAnC, _DO.DapAnD, _DO.IDDAĐung,_DO.IDND);

                TempData["msgSuccess"] = "<script>alert('Cập nhập thành công');</script>";
            }
            catch (Exception e)
            {

                TempData["msgSuccess"] = "<script>alert('Cập nhập thất bại " + e.Message + " ');</script>";
            }

            return RedirectToAction("Index", "Question");
        }
        public FileResult DownloadExcel()
        {
            string path = "/App_Data/Template_Question.xlsx";
            return File(path, "application/vnd.ms-excel", "Template_Question.xlsx");
        }
        public ActionResult Delete(int id)
        {
            try
            {
                db_context.CauHoi_delete(id);
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Xóa dữ liệu thất bại');</script>";
            }
            return RedirectToAction("Index", "Question");
        }
        public JsonResult GetNDĐT(int IDCTLVDT)
        {
            db_context.Configuration.ProxyCreationEnabled = false;
            List<NoiDungDT> IDNoiDung = db_context.NoiDungDTs.Where(x => x.IDCTLVDT == IDCTLVDT).ToList();
            NoiDungDT n = new NoiDungDT();
            n.IDND = 0;
            n.NoiDung = "--Chọn Nội dung--";
            n.IDCTLVDT = IDCTLVDT;
            IDNoiDung.Add(n);

            return Json(IDNoiDung.OrderBy(x => x.IDND).ToList(), JsonRequestBehavior.AllowGet);
        }
        public ActionResult ImportExcel()
        {
            db_context.Configuration.ProxyCreationEnabled = false;
            List<NoiDungDT> dt = db_context.NoiDungDTs.ToList();
            ViewBag.IDND = new SelectList(dt, "IDND", "NoiDung");

            //List<DanhSachDA> ds = db_context.DanhSachDAs.ToList();
            //ViewBag.IDDAĐung = new SelectList(ds, "IDDSĐA", "TenĐA");

            //List<CTLVDT> lvct = db_context.CTLVDTs.ToList();
            //ViewBag.IDCTLVDT = new SelectList(lvct, "IDCTLVDT", "TenCTLVDT");

            return PartialView();
        }
        [HttpPost]
        public ActionResult ImportExcel(ManageQuestionValidation _DO)
        {
            string filePath = string.Empty;
            if (Request != null)
            {
                HttpPostedFileBase file = Request.Files["FileUpload"];
                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    string path = Server.MapPath("~/UploadedFiles/");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    filePath = path + Path.GetFileName(DateTime.Now.ToString("yyyyMMddHHmm") + "-" + file.FileName);

                    file.SaveAs(filePath);
                    Stream stream = file.InputStream;

                    IExcelDataReader reader = null;
                    if (file.FileName.EndsWith(".xls"))
                    {
                        reader = ExcelReaderFactory.CreateBinaryReader(stream);
                    }
                    else if (file.FileName.EndsWith(".xlsx"))
                    {
                        reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                    }
                    else
                    {
                        TempData["msg"] = "<script>alert('Vui lòng chọn đúng định dạng file Excel');</script>";
                        return View();
                    }
                    DataSet result = reader.AsDataSet();
                    DataTable dt = result.Tables[0];
                    reader.Close();

                    try
                    {
                        for (int i = 5; i < dt.Rows.Count; i++)
                        {
                            string NoiDungCH = dt.Rows[i][1].ToString().Trim();

                            string DapAnA = dt.Rows[i][2].ToString().Trim();

                            string DapAnB = dt.Rows[i][3].ToString().Trim();

                            string DapAnC = dt.Rows[i][4].ToString().Trim();

                            string DapAnD = dt.Rows[i][5].ToString().Trim();

                            string DapAnDung = dt.Rows[i][6].ToString().Trim();

                            db_context.CauHoi_insert(NoiDungCH, DapAnA, DapAnB, DapAnC, DapAnD, Convert.ToInt32(DapAnDung),_DO.IDND,MyAuthentication.ID);

                        }

                        TempData["msgSuccess"] = "<script>alert('Import dữ liệu thành công');</script>";
                    }
                    catch (Exception ex)
                    {
                        TempData["msgError"] = "<script>alert('Có lỗi khi Upload: " + ex + "');</script>";
                    }
                }
                else
                {
                    TempData["msgSuccess"] = "<script>alert('Vui lòng nhập file Import');</script>";
                }
            }
            else
            {
                TempData["msgSuccess"] = "<script>alert('Vui lòng nhập file Import');</script>";
            }

            return RedirectToAction("Index", "Question");
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