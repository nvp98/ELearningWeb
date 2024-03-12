using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using E_Learning.Models;
using E_Learning.Common;
using PagedList;

namespace E_Learning.Controllers
{
    public class ManageExamQuestionGroupController : Controller
    {
        ELEARNINGEntities db_context = new ELEARNINGEntities();
        // GET: ManageExamQuestion
        //public ActionResult Index(int? page, int id)
        //{
        //    var res = (from b in db_context.BoDeThis.Where(x => x.IDND == id)
        //               join n in db_context.NoiDungDTs on b.IDND equals n.IDND
        //               select new ManageExamQuestionGroupValidation
        //               {
        //                   IDBDeThi = b.IDBDeThi,
        //                   IDND = (int)b.IDND,
        //                   TenNoiDung = n.NoiDung,
        //                   MaBDeThi = b.MaBDeThi,
        //                   TenBDeThi = b.TenBDeThi,
        //               }).ToList();

        //    if (page == null) page = 1;
        //    int pageSize = 20;
        //    int pageNumber = (page ?? 1);
        //    return View(res.ToList().ToPagedList(pageNumber, pageSize));
        //}

        //public ActionResult Create(int id)
        //{
        //    var lastRecord = (from b in db_context.BoDeThis orderby b.IDBDeThi descending select b).FirstOrDefault();
        //    if (lastRecord == null)
        //    {
        //        ViewBag.MaBDeThi = "ĐT0000" + 1;
        //    }
        //    else if (Convert.ToInt32(lastRecord.IDBDeThi) < 9)
        //    {
        //        ViewBag.MaBDeThi = "ĐT0000" + (Convert.ToInt32(lastRecord.IDBDeThi) + 1);
        //    }
        //    else if (Convert.ToInt32(lastRecord.IDBDeThi) < 99)
        //    {
        //        ViewBag.MaBDeThi = "ĐT000" + (Convert.ToInt32(lastRecord.IDBDeThi) + 1);
        //    }
        //    else if (Convert.ToInt32(lastRecord.IDBDeThi) < 999)
        //    {
        //        ViewBag.MaBDeThi = "ĐT00" + (Convert.ToInt32(lastRecord.IDBDeThi) + 1);
        //    }
        //    else if (Convert.ToInt32(lastRecord.IDBDeThi) < 9999)
        //    {
        //        ViewBag.MaBDeThi = "ĐT0" + (Convert.ToInt32(lastRecord.IDBDeThi) + 1);
        //    }
        //    else
        //    {
        //        ViewBag.MaBDeThi = "ĐT" + (Convert.ToInt32(lastRecord.IDBDeThi) + 1);
        //    }

        //    db_context.Configuration.ProxyCreationEnabled = false;
        //    List<NoiDungDT> nd = db_context.NoiDungDTs.Where(x => x.IDND == id).ToList();
        //    ViewBag.NDList = new SelectList(nd, "IDND", "NoiDung");

        //    return PartialView();
        //}

        //[HttpPost]
        //public ActionResult Create(ManageExamQuestionGroupValidation _DO)
        //{

        //    try
        //    {
        //        //Check trùng mã Đề Thi
        //        if (IsBDTAvailable(_DO.MaBDeThi) == false)
        //        {
        //            db_context.BoDeThi_insert(_DO.IDND, _DO.MaBDeThi, _DO.TenBDeThi);
        //            TempData["msgSuccess"] = "<script>alert('Thêm mới thành công');</script>";
        //        }
        //        else
        //        {
        //            TempData["msgSuccess"] = "<script>alert('Bộ Đề thi đã tồn tại');</script>";
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        TempData["msgError"] = "<script>alert('Có lỗi khi thêm mới: " + e.Message + "');</script>";
        //    }
        //    //return View();
        //    return RedirectToAction("Index", "ManageExamQuestionGroup", new { id = _DO.IDND });
        //}

        //public bool IsBDTAvailable(string MaBDeThi)
        //{
        //    var IsCheck = (from k in db_context.BoDeThis
        //                   where (k.MaBDeThi.ToLower() == MaBDeThi)
        //                   select new { k.MaBDeThi }).FirstOrDefault();
        //    bool status;
        //    if (IsCheck != null)
        //    {
        //        status = true;
        //    }
        //    else
        //    {
        //        status = false;
        //    }
        //    return status;
        //}



        //public ActionResult Edit(int id)
        //{
        //    var res = (from c in db_context.BoDeThi_searchByID(id)
        //               select new ManageExamQuestionGroupValidation
        //               {
        //                   IDBDeThi = c.IDBDeThi,
        //                   IDND = (int)c.IDND,
        //                   MaBDeThi = c.MaBDeThi,
        //                   TenBDeThi = c.TenBDeThi,
        //               }).ToList();
        //    ManageExamQuestionGroupValidation DO = new ManageExamQuestionGroupValidation();

        //    if (res.Count > 0)
        //    {
        //        foreach (var co in res)
        //        {
        //            DO.IDBDeThi = co.IDBDeThi;
        //            DO.IDND = co.IDND;
        //            DO.MaBDeThi = co.MaBDeThi;
        //            DO.TenBDeThi = co.TenBDeThi;
        //        }
        //        db_context.Configuration.ProxyCreationEnabled = false;
        //        var idn = db_context.BoDeThis.Where(x => x.IDBDeThi == id).Select(g => g.IDND).FirstOrDefault();
                
        //        List<NoiDungDT> nd = db_context.NoiDungDTs.Where(x => x.IDND == idn).ToList();
        //        ViewBag.NDList = new SelectList(nd, "IDND", "NoiDung");
        //    }
        //    else
        //    {
        //        return HttpNotFound();
        //    }
        //    return PartialView(DO);
        //}


        //[HttpPost]

        //public ActionResult Edit(ManageExamQuestionGroupValidation _DO)
        //{
        //    try
        //    {

        //        db_context.BoDeThi_update(_DO.IDBDeThi, _DO.IDND, _DO.MaBDeThi, _DO.TenBDeThi);

        //        TempData["msgSuccess"] = "<script>alert('Cập nhập thành công');</script>";
        //    }
        //    catch (Exception e)
        //    {
        //        TempData["msgSuccess"] = "<script>alert('Cập nhập thất bại " + e.Message + " ');</script>";
        //    }

        //    return RedirectToAction("Index", "ManageExamQuestionGroup", new { id = _DO.IDND });
        //}




        public ActionResult Delete(int id)
        {
            //var idb = db_context.BoDeThis.Where(x => x.IDBDeThi == id).Select(g => g.IDND).FirstOrDefault();
            //try
            //{
            //    db_context.BoDeThi_delete(id);
            //}
            //catch (Exception e)
            //{
            //    TempData["msgSuccess"] = "<script>alert('Xóa dữ liệu thất bại: " + e.Message + "');</script>";
            //}
            return RedirectToAction("Index", "ManageExamQuestionGroup", new { id = id });
        }

    }
}