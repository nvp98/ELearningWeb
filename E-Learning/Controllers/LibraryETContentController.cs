using PagedList;
using E_Learning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_Learning.Controllers
{
    public class LibraryETContentController : Controller
    {
        ELEARNINGEntities db_context = new ELEARNINGEntities();
        //GET: LibraryCourses
        public ActionResult Index(int id, int? page)
        {
            if (User.Identity.IsAuthenticated)
            {
                var res = (from a in db_context.NoiDungDTs
                           join ct in db_context.CTLVDTs on a.IDCTLVDT equals ct.IDCTLVDT
                           join lv in db_context.LinhVucDTs on a.LVDTID equals lv.IDLVDT
                           join p in db_context.PhongBans on a.BPLID equals p.IDPhongBan
                           join l in db_context.LopHocs on a.IDND equals l.NDID into NoiDungSLLH
                           select new ManageETContentValidation
                           {
                               IDND = a.IDND,
                               MaND = a.MaND,
                               NoiDung = a.NoiDung,
                               VideoND = a.VideoND,
                               ImageND = a.ImageND,
                               LVDTID = lv.IDLVDT,
                               LinhVuc = lv.TenLVDT,
                               IDCTLVDT = ct.IDCTLVDT,
                               LVChiTiet = ct.TenCTLVDT,
                               BPLNC = p.TenPhongBan,
                               ThoiLuongDT = (int)a.ThoiLuongDT,
                               SLLH = NoiDungSLLH.Count(),
                               NgayTao = a.NgayTao
                           }).Where(x => x.IDCTLVDT == id).ToList();
                if (page == null) page = 1;
                int pageSize = 20;
                int pageNumber = (page ?? 1);
                return View(res.ToList().ToPagedList(pageNumber, pageSize));
            }
            else
            {
                return RedirectToAction("", "Login");
            }
        }
    }
}