using E_Learning.Controllers.QTHD;
using E_Learning.Models;
using E_Learning.ModelsQTHD;
using ExcelDataReader;
using PagedList;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace E_Learning.Controllers.KCCD
{
    public class CauHoiKCCDController : Controller
    {
        // GET: CauHoiKCCD
        ELEARNINGEntities db_context = new ELEARNINGEntities();
        int Idquyen = MyAuthentication.IDQuyen;
        String ControllerName = "CauHoiKCCD";
        public ActionResult Index(int? page, int? KCCDID, int? IDDT)
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
                var res = (from c in db_context.KCCD_CauHoi.Where(x=>x.KCCDID ==KCCDID && x.DeThiID ==IDDT)
                           join b in db_context.NoiDungDTKCCDs on c.KCCDID equals b.ID
                           join d in db_context.DanhSachDAs on c.IDDAĐung equals d.IDDSĐA
                           join e in db_context.KCCD_DeThi on c.DeThiID equals e.ID into ul
                           from e in ul.DefaultIfEmpty()
                           select new CauHoiKCCDView
                           {
                               IDCH = c.IDCH,
                               NoiDungCH = c.NoiDungCH,
                               DapAnA = c.DapAnA,
                               DapAnB = c.DapAnB,
                               DapAnC = c.DapAnC,
                               DapAnD = c.DapAnD,
                               IDDAĐung = (int)c.IDDAĐung,
                               DapAnĐung = d.TenĐA,
                               DeThiID = c.DeThiID,
                               TenDeThi = e.TenDe,
                               KCCDID = c.KCCDID,
                               TenNoiDung = b.TenND
                           }).ToList();

                if (page == null) page = 1;
                int pageSize = 20;
                int pageNumber = (page ?? 1);
                //if (MyAuthentication.IDQuyen == 2 || MyAuthentication.IDQuyen == 3)
                //{
                //    res = res.Where(x => x.GVID == id).ToList();
                //}
                //if (ListQuyen.Contains(CONSTKEY.V_GV)) res = res.Where(x => x.GVID == id).ToList();
                //List<NoiDungDT> dt = db_context.NoiDungDTs.ToList();
                //if (IDND == null) IDND = 0;
                //if (IDND != 0) { res = res.Where(x => x.IDND == IDND).ToList(); }
                //ViewBag.IDND = new SelectList(dt, "IDND", "NoiDung", IDND);
                return View(res.ToList().ToPagedList(pageNumber, pageSize));

            }
            else { return RedirectToAction("", "Login"); }
        }

        public ActionResult EditCH(int id)
        {
            var res = (from c in db_context.KCCD_CauHoi.Where(x => x.IDCH == id)
                       join d in db_context.DanhSachDAs on c.IDDAĐung equals d.IDDSĐA
                       select new CauHoiKCCDView
                       {
                           IDCH = c.IDCH,
                           NoiDungCH = c.NoiDungCH,
                           DapAnA = c.DapAnA,
                           DapAnB = c.DapAnB,
                           DapAnC = c.DapAnC,
                           DapAnD = c.DapAnD,
                           IDDAĐung = (int)c.IDDAĐung,
                           DapAnĐung = d.TenĐA,
                           DeThiID = c.DeThiID,
                           KCCDID = c.KCCDID,
                       }).FirstOrDefault();


            if (res != null)
            {
                List<DanhSachDA> ds = db_context.DanhSachDAs.ToList();
                ViewBag.IDDAĐung = new SelectList(ds, "IDDSĐA", "TenĐA", res.IDDAĐung);
            }
            else
            {
                return HttpNotFound();
            }
            return PartialView(res);
        }
        [HttpPost]
        public ActionResult EditCH(CauHoiKCCDView _DO)
        {
            try
            {

                db_context.KCCD_CauHoi_Update(_DO.IDCH, _DO.NoiDungCH, _DO.DapAnA, _DO.DapAnB, _DO.DapAnC, _DO.DapAnD, _DO.IDDAĐung,_DO.KCCDID,_DO.DeThiID);

                TempData["msgSuccess"] = "<script>alert('Cập nhập thành công');</script>";
            }
            catch (Exception e)
            {

                TempData["msgSuccess"] = "<script>alert('Cập nhập thất bại " + e.Message + " ');</script>";
            }

            return RedirectToAction("Index", "CauHoiKCCD", new { KCCDID = _DO.KCCDID , IDDT = _DO.DeThiID});
        }

        public ActionResult DeleteCH(int id, int? KCCDID, int? DeThiID )
        {
            try
            {
                db_context.KCCD_CauHoi_delete(id);
                TempData["msgSuccess"] = "<script>alert('Xóa dữ liệu thành công');</script>";
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Xóa dữ liệu thất bại');</script>";
            }
            return RedirectToAction("Index", "CauHoiKCCD", new { KCCDID = KCCDID, IDDT = DeThiID });
        }

        public ActionResult DeleteAll(int? KCCDID, int? DeThiID)
        {
            try
            {
               if(KCCDID != null && DeThiID != null)
                {
                    var a = db_context.KCCD_CauHoi.Where(x => x.KCCDID == KCCDID && x.DeThiID == DeThiID).ToList();
                    if(a.Count >0)
                    {
                        foreach( var n in a)
                        {
                            var ak = db_context.KCCD_CTBaiThi.Where(x => x.IDCauHoi == n.IDCH).FirstOrDefault();
                            if (ak != null) db_context.KCCD_CTBaiThi_delete(ak.IDCT);
                            db_context.KCCD_CauHoi_delete(n.IDCH);  
                        }
                    }
                }
                TempData["msgSuccess"] = "<script>alert('Xóa dữ liệu thành công');</script>";
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Xóa dữ liệu thất bại');</script>";
            }
            return RedirectToAction("Index", "CauHoiKCCD", new { KCCDID = KCCDID, IDDT = DeThiID });
        }


        public ActionResult DeThi(int? page, int? KCCDID)
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
                var res = (from c in db_context.KCCD_DeThi.Where(x=>x.KCCDID == KCCDID)
                           join b in db_context.NoiDungDTKCCDs on c.KCCDID equals b.ID
                           select new DeThiKCCDView
                           {
                              ID = c.ID,
                              MaDe =c.MaDe,
                              TenDe =c.TenDe,
                              DiemChuan =c.DiemChuan,
                              KCCDID= c.KCCDID,
                              TenND =b.TenND,
                              TongSoCau = db_context.KCCD_CauHoi.Where(x=>x.DeThiID ==c.ID).Count(),
                           }).ToList();
                ViewBag.NoiDungKCCD = db_context.NoiDungDTKCCDs.Where(x => x.ID == KCCDID).FirstOrDefault().TenND;
                if (page == null) page = 1;
                int pageSize = 20;
                int pageNumber = (page ?? 1);
                //if (MyAuthentication.IDQuyen == 2 || MyAuthentication.IDQuyen == 3)
                //{
                //    res = res.Where(x => x.GVID == id).ToList();
                //}
                //if (ListQuyen.Contains(CONSTKEY.V_GV)) res = res.Where(x => x.GVID == id).ToList();
                //List<NoiDungDT> dt = db_context.NoiDungDTs.ToList();
                //if (IDND == null) IDND = 0;
                //if (IDND != 0) { res = res.Where(x => x.IDND == IDND).ToList(); }
                //ViewBag.IDND = new SelectList(dt, "IDND", "NoiDung", IDND);
                return View(res.ToList().ToPagedList(pageNumber, pageSize));

            }
            else { return RedirectToAction("", "Login"); }
        }
        public ActionResult EditDT(int id)
        {
            db_context.Configuration.ProxyCreationEnabled = false;
            var res = (from c in db_context.KCCD_DeThi.Where(x => x.ID == id)
                       join b in db_context.NoiDungDTKCCDs on c.KCCDID equals b.ID
                       select new DeThiKCCDView
                       {
                           ID = c.ID,
                           MaDe = c.MaDe,
                           TenDe = c.TenDe,
                           DiemChuan = c.DiemChuan,
                           KCCDID = c.KCCDID,
                           TenND = b.TenND,
                           //TongSoCau = db_context.KCCD_CauHoi.Where(x => x.DeThiID == c.ID).Count(),
                       }).FirstOrDefault();
            List<NoiDungDTKCCD> bd = db_context.NoiDungDTKCCDs.Where(x => x.ID == res.KCCDID).ToList();
            ViewBag.KCCDID = new SelectList(bd, "ID", "TenND", res.KCCDID);
            return PartialView(res);

        }
        [HttpPost]
        public ActionResult EditDT(DeThiKCCDView _DO)
        {
            try
            {
                if(_DO.ID != 0 && _DO.KCCDID != null)
                {
                    db_context.KCCD_DeThi_update(_DO.ID, _DO.MaDe, _DO.TenDe, _DO.DiemChuan, _DO.KCCDID);
                }
                //dbKCCCD.NoiDungDTKCCD_update(_DO.ID, _DO.TenND, _DO.NhomNLID, _DO.LVDTID, _DO.PhongBanID, _DO.NgayTao);

                TempData["msgSuccess"] = "<script>alert('Cập nhập thành công');</script>";
            }
            catch (Exception e)
            {

                TempData["msgSuccess"] = "<script>alert('Cập nhập thất bại " + e.Message + " ');</script>";
            }
            return RedirectToAction("DeThi", "CauHoiKCCD", new { KCCDID = _DO.KCCDID });
        }

        public ActionResult DeleteDT(int id, int? KCCDID)
        {
            try
            {
                //var a = db_context.KCCD_CauHoi.Where(x=>x.DeThiID ==id).ToList();
                //if(a.Count > 0)
                //{
                //    foreach( var u in a)
                //    {

                //    }
                //}
                db_context.KCCD_DeThi_delete(id);
                //db_context.DeNghiKCCD_delete(id);
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Xóa dữ liệu thất bại');</script>";
            }
            return RedirectToAction("DeThi", "CauHoiKCCD", new { KCCDID = KCCDID });
        }

        public ActionResult ImportExcel(int? KCCDID)
        {
            db_context.Configuration.ProxyCreationEnabled = false;
            List<NoiDungDTKCCD> bd = db_context.NoiDungDTKCCDs.Where(x=>x.ID == KCCDID).ToList();
            ViewBag.KCCDID = new SelectList(bd, "ID", "TenND", KCCDID);
            return PartialView();
        }
        [HttpPost]
        public ActionResult ImportExcel(DeThiKCCDView _DO)
        {

            ObjectParameter IDDTout = new ObjectParameter("ID", typeof(int));
            int IDDT = 0;
            if (_DO.KCCDID != null && _DO.MaDe != "" && _DO.TenDe != "")
            {
                db_context.KCCD_DeThi_insert(_DO.MaDe, _DO.TenDe, 7, _DO.KCCDID,IDDTout);
                IDDT = Convert.ToInt32(IDDTout.Value);
            }
         

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
                            int DA = new NoiDungQTController().CheckDA(DapAnDung);

                            db_context.KCCD_CauHoi_insert(NoiDungCH, DapAnA, DapAnB, DapAnC, DapAnD, DA, _DO.KCCDID,IDDT);

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

            return RedirectToAction("DeThi", "CauHoiKCCD", new { KCCDID  = _DO.KCCDID});
        }

    }
}