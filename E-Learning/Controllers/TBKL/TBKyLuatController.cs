using E_Learning.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_Learning.Controllers.TBKL
{
    public class TBKyLuatController : Controller
    {
        ELEARNINGEntities db = new ELEARNINGEntities();
        int Idquyen = MyAuthentication.IDQuyen;
        String ControllerName = "TBKyLuat";
        // GET: TBKyLuat
        public ActionResult Index(int? page, int? thang, int? nam, string search)
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            ViewBag.QUYENCN = ListQuyen;

            if (search == null) search = "";
            ViewBag.search = search;
            
            var res = (from a in db.TB_KyLuat
                       select new TBKyLuatValidation
                       {
                           ID = a.ID,
                           TB_TieuDe = a.TB_TieuDe,
                           TB_Nam = a.TB_Nam,
                           TB_Thang = a.TB_Thang,
                           TB_File = a.TB_File
                       }).ToList().OrderByDescending(x => x.TB_Nam).ThenByDescending(x=>x.TB_Thang);
            if (thang != null) res = res.Where(x => x.TB_Thang == thang).ToList().OrderByDescending(x => x.TB_Nam).ThenByDescending(x => x.TB_Thang);
            if (nam != null) res = res.Where(x => x.TB_Nam == nam).ToList().OrderByDescending(x => x.TB_Nam).ThenByDescending(x => x.TB_Thang);
            if(search != null) res =res.Where(x=>x.TB_TieuDe.Contains(search)).ToList().OrderByDescending(x => x.TB_Nam).ThenByDescending(x => x.TB_Thang);

            var selec = new List<SelectListItem>();
            for (int i = 1; i <= 12; i++)
            {
                var se = new SelectListItem { Text =  i.ToString(), Value = i.ToString() };
                selec.Add(se);
            }
            ViewBag.Thang = new SelectList(selec, "Text", "Value");

            var selecYear = new List<SelectListItem>();
            foreach (var item in Enumerable.Range(2020, (DateTime.Now.Year - 2020) + 1))
            {
                var se = new SelectListItem { Text =  item.ToString(), Value =item.ToString() };
                selecYear.Add(se);
            }
            ViewBag.Nam = new SelectList(selecYear, "Text", "Value");

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
            db.Configuration.ProxyCreationEnabled = false;

            return PartialView();
        }
        [HttpPost]
        public ActionResult Create(TBKyLuatValidation _DO)
        {
            try
            {
                //if (_DO.OrderBy == null) _DO.OrderBy = 0;
                string path = Server.MapPath("~/UploadedFiles/TBKyLuat/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                //Use Namespace called :  System.IO  
                string FileName = _DO.FileUpload != null ? "TBKL_" + DateTime.Now.ToString("yyyyMMddHHmmss") : "";

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
                        _DO.TB_File = "~/UploadedFiles/TBKyLuat/" + FileName;
                    }
                    if(_DO.TB_TieuDe != null && _DO.TB_Thang != null && _DO.TB_Nam != null)
                    {
                        var a = db.TB_KyLuat_insert(_DO.TB_TieuDe, _DO.TB_Thang, _DO.TB_Nam, _DO.TB_File);
                        TempData["msgSuccess"] = "<script>alert('Thêm mới thành công');</script>";
                    }
                    else
                    {
                        TempData["msgSuccess"] = "<script>alert('Vui lòng điền đầy đủ thông tin');</script>";
                    }
                  
                }
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Có lỗi khi thêm mới: " + e.Message + "');</script>";
            }
            //return View();
            return RedirectToAction("Index", "TBKyLuat");
        }
        public ActionResult Edit(int id)
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            if (!ListQuyen.Contains(CONSTKEY.EDIT))
            {
                TempData["msgError"] = "<script>alert('Bạn không có quyền thực hiện chức năng này');</script>";
                return RedirectToAction("", "Home");
            }
            var res = (from a in db.TB_KyLuat.Where(x => x.ID == id)
                       select new TBKyLuatValidation
                       {
                           ID = a.ID,
                           TB_TieuDe = a.TB_TieuDe,
                           TB_Nam = a.TB_Nam,
                           TB_Thang = a.TB_Thang,
                           TB_File = a.TB_File
                       }).ToList();
            TBKyLuatValidation DO = new TBKyLuatValidation();

            if (res.Count > 0)
            {
                foreach (var c in res)
                {
                  DO.ID =c.ID;
                  DO.TB_TieuDe = c.TB_TieuDe;
                  DO.TB_Thang = c.TB_Thang;
                  DO.TB_Nam = c.TB_Nam;
                  DO.TB_File = c.TB_File;
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
        public ActionResult Edit(TBKyLuatValidation _DO)
        {
            try
            {
                //if (_DO.OrderBy == null) _DO.OrderBy = 0/*;*/
                if (_DO.FileUpload != null)
                {
                    string path = Server.MapPath("~/UploadedFiles/TBKyLuat/");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    //Use Namespace called :  System.IO  
                    string FileName = _DO.FileUpload != null ? "TBKL_" + DateTime.Now.ToString("yyyyMMddHHmmss") : "";

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
                            _DO.TB_File = "~/UploadedFiles/TBKyLuat/" + FileName;
                        }
                        var a = db.TB_KyLuat_Update(_DO.ID, _DO.TB_TieuDe, _DO.TB_Thang,_DO.TB_Nam,_DO.TB_File);
                        TempData["msgSuccess"] = "<script>alert('Thêm mới thành công');</script>";
                    }
                }
                else
                {
                    var a = db.TB_KyLuat_Update(_DO.ID, _DO.TB_TieuDe, _DO.TB_Thang, _DO.TB_Nam, _DO.TB_File);
                }

                TempData["msgSuccess"] = "<script>alert('Cập nhập thành công');</script>";
            }
            catch (Exception e)
            {

                TempData["msgSuccess"] = "<script>alert('Cập nhập thất bại " + e.Message + " ');</script>";
            }

            return RedirectToAction("Index", "TBKyLuat");
        }
        public ActionResult Delete(int id)
        {
            try
            {
                db.TB_KyLuat_delete(id);
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Xóa dữ liệu thất bại');</script>";
            }
            return RedirectToAction("Index", "TBKyLuat");
        }

    }

}