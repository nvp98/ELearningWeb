using E_Learning.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_Learning.Controllers
{
    public class EGuideController : Controller
    {
        ELEARNINGEntities db = new ELEARNINGEntities();
        int Idquyen = MyAuthentication.IDQuyen;
        String ControllerName = "EGuide";
        // GET: EGuide
        public ActionResult Index()
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            ViewBag.QUYENCN = ListQuyen;

            var res = (from a in db.HDSDs
                       select new EGuideValidation
                       {
                           ID = a.ID,
                           MoTa = a.MoTa,
                           FilePath = a.FilePath,
                           OrderBy =a.OrderBy
                       }).ToList().OrderBy(x=>x.OrderBy);

            return View(res.ToList());
        }

        public ActionResult Create()
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            if (!ListQuyen.Contains(CONSTKEY.ADD))
            {
                TempData["msgError"] = "<script>alert('Bạn không có quyền thực hiện chức năng này');</script>";
                return RedirectToAction("", "Home");
            }
            db.Configuration.ProxyCreationEnabled = false;

            return PartialView();
        }
        [HttpPost]
        public ActionResult Create(EGuideValidation _DO)
        {
            try
            {
                if (_DO.OrderBy == null) _DO.OrderBy = 0;
                string path = Server.MapPath("~/UploadedFiles/HDSD/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                //Use Namespace called :  System.IO  
                string FileName = _DO.FileUpload != null ? "HDSD_" + DateTime.Now.ToString("yyyyMMddHHmmss") : "";

                //To Get File Extension  
                string FileExtension = _DO.FileUpload != null ? Path.GetExtension(_DO.FileUpload.FileName) : "";
                ////Add Current Date To Attached File Name  
                if (FileExtension != ".pdf")
                {
                    TempData["msgError"] = "<script>alert('Vui lòng chọn đúng định dạng file PDF');</script>";
                    //return View();
                }
                else
                {
                    if (_DO.FileUpload != null)
                    {
                        FileName = FileName.Trim() + FileExtension;
                        _DO.FileUpload.SaveAs(path + FileName);
                        _DO.FilePath = "~/UploadedFiles/HDSD/" + FileName;
                    }
                    var a = db.HDSD_insert(_DO.MoTa, _DO.FilePath,_DO.OrderBy);
                    TempData["msgSuccess"] = "<script>alert('Thêm mới thành công');</script>";
                }
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Có lỗi khi thêm mới: " + e.Message + "');</script>";
            }
            //return View();
            return RedirectToAction("Index", "EGuide");
        }
        public ActionResult Edit(int id)
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            if (!ListQuyen.Contains(CONSTKEY.EDIT))
            {
                TempData["msgError"] = "<script>alert('Bạn không có quyền thực hiện chức năng này');</script>";
                return RedirectToAction("", "Home");
            }
            var res = (from a in db.HDSDs.Where(x => x.ID == id)
                       select new EGuideValidation
                       {
                           ID = a.ID,
                           MoTa = a.MoTa,
                           FilePath = a.FilePath,
                           OrderBy =a.OrderBy
                       }).ToList();
            EGuideValidation DO = new EGuideValidation();

            if (res.Count > 0)
            {
                foreach (var c in res)
                {
                    DO.ID = c.ID;
                    DO.MoTa = c.MoTa;
                    DO.FilePath = c.FilePath;
                    DO.OrderBy = c.OrderBy;
                }

                db.Configuration.ProxyCreationEnabled = false;

            }
            else
            {
                return HttpNotFound();
            }
            return PartialView(DO);
        }
        [HttpPost]
        public ActionResult Edit(EGuideValidation _DO)
        {
            try
            {
                if (_DO.OrderBy == null) _DO.OrderBy = 0;
                if(_DO.FileUpload != null)
                {
                    string path = Server.MapPath("~/UploadedFiles/HDSD/");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    //Use Namespace called :  System.IO  
                    string FileName = _DO.FileUpload != null ? "HDSD_" + _DO.ID : "";

                    //To Get File Extension  
                    string FileExtension = _DO.FileUpload != null ? Path.GetExtension(_DO.FileUpload.FileName) : "";
                    ////Add Current Date To Attached File Name  
                    if (FileExtension != ".pdf")
                    {
                        TempData["msgError"] = "<script>alert('Vui lòng chọn đúng định dạng file PDF');</script>";
                        //return View();
                    }
                    else
                    {
                        if (_DO.FileUpload != null)
                        {
                            FileName = FileName.Trim() + FileExtension;
                            _DO.FileUpload.SaveAs(path + FileName);
                            _DO.FilePath = "~/UploadedFiles/HDSD/" + FileName;
                        }
                        var a = db.HDSD_update(_DO.ID,_DO.MoTa, _DO.FilePath, _DO.OrderBy);
                        TempData["msgSuccess"] = "<script>alert('Thêm mới thành công');</script>";
                    }
                }
                else
                {
                    var a = db.HDSD_update(_DO.ID,_DO.MoTa, _DO.FilePath, _DO.OrderBy);
                }

                TempData["msgSuccess"] = "<script>alert('Cập nhập thành công');</script>";
            }
            catch (Exception e)
            {

                TempData["msgSuccess"] = "<script>alert('Cập nhập thất bại " + e.Message + " ');</script>";
            }

            return RedirectToAction("Index", "EGuide");
        }
        public ActionResult Delete(int id)
        {
            try
            {
                db.HDSD_delete(id);
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Xóa dữ liệu thất bại');</script>";
            }
            return RedirectToAction("Index", "EGuide");
        }

    }
}