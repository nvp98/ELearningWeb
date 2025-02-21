using E_Learning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_Learning.Controllers
{
    public class TLessonController : Controller
    {
        ELEARNINGEntities db_context = new ELEARNINGEntities();
        // GET: TLesson
        public ActionResult Index(int id)
        {
            var res = (from n in db_context.NoiDungDTs
                       join l in db_context.LinhVucDTs on n.LVDTID equals l.IDLVDT
                      select new ManageETContentValidation
                      {
                          IDND = n.IDND,
                          MaND = n.MaND,
                          NoiDung = n.NoiDung,
                          LinhVuc = l.TenLVDT,
                          ImageND = n.ImageND,
                          VideoND = n.VideoND,
                       }).Where(x => x.IDND == id).ToList();

            return View(res);
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