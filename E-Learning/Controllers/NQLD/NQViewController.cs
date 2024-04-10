using E_Learning.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_Learning.Controllers.NQLD
{
    public class NQViewController : Controller
    {
        ELEARNINGEntities db_context = new ELEARNINGEntities();
        int Idquyen = MyAuthentication.IDQuyen;
        String ControllerName = "NQView";
        // GET: NQView
        public ActionResult Index(int id)
        {
            var res = (from n in db_context.NoiDungDTs.Where(x=>x.IDND == id && x.isNQ ==1)
                       select new ManageETContentValidation
                       {
                           IDND = n.IDND,
                           MaND = n.MaND,
                           NoiDung = n.NoiDung,
                           ImageND = n.ImageND,
                           VideoND = n.VideoND,
                       }).ToList();

            return View(res);
        }
        public ActionResult ListNQ(int? page, string search, int? IDNQLD)
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            ViewBag.QUYENCN = ListQuyen;
            //if (!ListQuyen.Contains(CONSTKEY.V))
            //{
            //    TempData["msgError"] = "<script>alert('Bạn không có quyền truy cập chức năng này');</script>";
            //    return RedirectToAction("", "Home");
            //}
            if (search == null) search = "";
            ViewBag.search = search;

            if (IDNQLD == null) IDNQLD = 0;
            int NVID = MyAuthentication.ID;

            var res = (from a in db_context.NoiDungDTs.Where(x => x.isNQ == 1)
                       join b in db_context.NQ_KetQua.Where(x=>x.IDNV == NVID) on a.IDND equals b.NDDTID into ul
                       from b in ul.DefaultIfEmpty()
                       select new NoiQuyKQView
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
                           XNTG = b.XNTG,
                           XNHT = b.XNHT,
                           XNHTFile = b.XNHTFile,
                           NgayHT = b.NgayHT,
                           NgayTG = b.NgayTG,
                           TinhTrang = b.TinhTrang,
                       }).OrderBy(x => x.isOrder).ToList();

            List<NoiDungDT> ctlvdt = db_context.NoiDungDTs.Where(x => x.isNQ == 1).OrderBy(x => x.isOrder).ToList();
            ViewBag.IDNQLD = new SelectList(ctlvdt, "IDND", "NoiDung");
            if (IDNQLD != 0) res = res.Where(x => x.IDND == IDNQLD).ToList();

            if (page == null) page = 1;
            int pageSize = 50;
            int pageNumber = (page ?? 1);
            return View(res.ToList().ToPagedList(pageNumber, pageSize));
        }
    }
}