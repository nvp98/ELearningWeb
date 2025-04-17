using DocumentFormat.OpenXml.Office2010.Excel;
using E_Learning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_Learning.Controllers.NQLD
{
    public class NQLDUserController : Controller
    {
        ELEARNINGEntities db_context = new ELEARNINGEntities();
        // GET: NQLDUser
        public ActionResult Index(EClassroomValidation _DO, int? id)
        {
            //Lấy IDHT theo NVID và LHID
            if (User.Identity.IsAuthenticated)
            {
                int? NVID = MyAuthentication.ID;
                 
                var kq = db_context.NQ_KetQua.Where(x=>x.IDNV == NVID && x.NDDTID == id).ToList();
                if(kq.Count == 0 && NVID != null && id != null)
                {
                    db_context.NQ_KetQua__insert(id,1,null,null,NVID,DateTime.Now,null,0);
                }

                var res = (from n in db_context.NoiDungDTs.Where(x => x.IDND == id && x.isNQ == 1)
                           join b in db_context.NQ_KetQua.Where(x=> x.IDNV == NVID && x.NDDTID == id) on n.IDND equals b.NDDTID into ul
                           from b in ul.DefaultIfEmpty()
                           select new NoiQuyKQView
                           {
                               IDND = n.IDND,
                               MaND = n.MaND,
                               NoiDung = n.NoiDung,
                               ImageND = n.ImageND,
                               VideoND = n.VideoND,
                               XNHT = b.XNHT??0,
                               XNTG = b.XNTG,
                               XNHTFile = b.XNHTFile,
                               FileDinhKem = n.FileDinhKem,
                               isNQ = n.isNQ,
                               TinhTrang = b.TinhTrang
                           }).ToList();

                return View(res);
            }
            else
            {
                return RedirectToAction("", "Login");
            }

        }
        public ActionResult CompleteLesson( int id)
        {
            //Lấy LHID và NVID theo IDHT
            int? NVID = MyAuthentication.ID;
            var IDHT = id;
            var kq = db_context.NQ_KetQua.Where(x => x.IDNV == NVID && x.NDDTID == id).ToList();
            if (kq.Count != 0)
            {
                var a =kq.FirstOrDefault();
                db_context.NQ_KetQua_Update(a.ID, 1, DateTime.Now, 1);
            }
           
            TempData["msgSuccess"] = "<script>alert('Chúc mừng bạn đã hoàn thành nội quy này');</script>";

            return RedirectToAction("ListNQ", "NQView");
        }
        public ActionResult CompleteFile(int? id, string link)
        {
            int? NVID = MyAuthentication.ID;
            var IDHT = id;
            // Lấy domain từ yêu cầu hiện tại
            string domain = Request.Url.GetLeftPart(UriPartial.Authority);

            var kq = db_context.NQ_KetQua.Where(x => x.IDNV == NVID && x.NDDTID == id).ToList();
            if (kq.Count == 0 && NVID != null && id != null)
            {
                db_context.NQ_KetQua__insert(id, 1, null, 1, NVID, DateTime.Now, null, 0);
            }
            if (kq.Count != 0)
            {
                var a = kq.FirstOrDefault();
                db_context.NQ_KetQua_UpdateXNFile(a.ID, 1);
            }

            // Chuyển hướng người dùng đến một action khác trong cùng controller
            return Redirect(domain + link);

        }


    }
}