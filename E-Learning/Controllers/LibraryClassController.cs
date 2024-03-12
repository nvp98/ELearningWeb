using PagedList;
using E_Learning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_Learning.Controllers
{
    public class LibraryClassController : Controller
    {
        ELEARNINGEntities db_context = new ELEARNINGEntities();
        // GET: LibraryCourses
        public ActionResult Index(int id , int? page)
        {
            var res = (from l in db_context.LopHocs
                       join n in db_context.NoiDungDTs
                       on l.NDID equals n.IDND
                       select new ManageClassValidation
                       {
                           IDLH = l.IDLH,
                           MaLH = l.MaLH,
                           TenLH = l.TenLH,
                           NDID = n.IDND,
                           MaND = n.MaND,
                           TenND = n.NoiDung,
                           LinhVuc = n.LinhVucDT.TenLVDT,
                           VideoLH = n.VideoND,
                           ImageLH = n.ImageND,
                           QuyDT = (int)l.QuyDT,
                           NamDT = (int)l.NamDT,
                           TGBDLH = (DateTime)l.TGBDLH,
                           TGKTLH = (DateTime)l.TGKTLH,
                       }).Where(x => x.NDID == id).ToList();
            if (page == null) page = 1;
            int pageSize = 20;
            int pageNumber = (page ?? 1);
            return View(res.ToList().ToPagedList(pageNumber, pageSize));
        }
    }
}