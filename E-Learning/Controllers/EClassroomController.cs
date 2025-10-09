using PagedList;
using E_Learning.Models;
using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using ExcelDataReader;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace E_Learning.Controllers
{
    public class EClassroomController : Controller
    {
        ELEARNINGEntities db_context = new ELEARNINGEntities();
        // GET: EClassroom
        public ActionResult Index(int ? page)
        {
            if (User.Identity.IsAuthenticated)
            {
                int id = MyAuthentication.ID;
                var res = (from h in db_context.XNHocTaps.Where(x=>x.NVID == id)
                           join l in db_context.LopHocs.Where(x=>x.TinhTrang == 1 || x.NCDT_ID == null) on h.LHID equals l.IDLH
                           join n in db_context.NhanViens.Where(x => x.ID == id) on h.NVID equals n.ID
                           join p in db_context.PhongBans on h.NhanVien.IDPhongBan equals p.IDPhongBan
                           select new EClassroomValidation
                           {
                               IDHT = h.IDHT,
                               PBID = p.IDPhongBan,
                               TenPB = p.TenPhongBan,
                               NVID = n.ID,
                               MaNV = n.MaNV,
                               HoTenHV = n.HoTen,
                               TenVT = n.Vitri.TenViTri,
                               LHID = l.IDLH,
                               MaLH = l.MaLH,
                               TenLH = l.TenLH,
                               TenND = l.NoiDungDT.NoiDung,
                               LinhVuc = l.NoiDungDT.LinhVucDT.TenLVDT,
                               VideoLH = l.NoiDungDT.VideoND,
                               ImageLH = l.NoiDungDT.ImageND,
                               TGBDLH = (DateTime)l.TGBDLH,
                               TGKTLH = (DateTime)l.TGKTLH,
                               NgayTG = h.NgayTG != null?(DateTime)h.NgayTG:default(DateTime),
                               NgayHT = h.NgayHT != null?(DateTime)h.NgayHT:default(DateTime),
                               XNTG = (bool)h.XNTG,
                               XNHT = (bool)h.XNHT,
                               //ToChucThi=(bool)l.ToChucThi
                               ToChucThi = true
                           }).OrderBy(x => x.TGBDLH).ToList();

                return View(res.ToList());
            }
            else
            {
                //Chưa đăng nhập
                return RedirectToAction("", "Login");
            }
                
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