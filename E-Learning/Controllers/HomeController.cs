using E_Learning.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace E_Learning.Controllers
{
    public class HomeController : Controller
    {
        readonly ELEARNINGEntities db = new ELEARNINGEntities();
        readonly int Idquyen = MyAuthentication.IDQuyen;
        readonly String ControllerName = "Home";

        public ActionResult Index()
        {
            var banners = db.Banners
                    .Where(x => x.IsActive == true && x.Type == 1)
                    .OrderBy(x => x.SortOrder)
                    .ToList();

            return View(banners);
        }
        public List<String> GetPermisionCN(int? Idquyen, string ControllerName)
        {
            //var IdControll = db.ListControllers.Where(x => x.Controller == ControllerName).Select(x => x.ID).FirstOrDefault();
            var lsQuyen = (from a in db.QuyenDetails.Where(x => x.IDQuyen == Idquyen && x.isActive == 1)
                           join b in db.QuyenCNs on a.IDQuyenCN equals b.ID
                           join c in db.ListControllers.Where(x => x.Controller == ControllerName && x.isActive == 1) on a.IDController equals c.ID
                           select b.MaQuyen).ToList();
            var mulQuyen = db.PhanQuyenHTs.Where(x => x.IDNV == MyAuthentication.ID).ToList();
            if (mulQuyen.Count > 0)
            {
                foreach (var item in mulQuyen)
                {
                    var lsq = (from a in db.QuyenDetails.Where(x => x.IDQuyen == item.IDQuyen && x.isActive == 1)
                               join b in db.QuyenCNs on a.IDQuyenCN equals b.ID
                               join c in db.ListControllers.Where(x => x.Controller == ControllerName && x.isActive == 1) on a.IDController equals c.ID
                               select b.MaQuyen).ToList();
                    lsQuyen.AddRange(lsq);
                }
            }
            if (!lsQuyen.Contains(CONSTKEY.V)) lsQuyen = new List<string>();
            return lsQuyen;
        }
        public List<String> GetPermisionControll(int? Idquyen)
        {
            //var IdControll = db.ListControllers.Where(x => x.Controller == ControllerName).Select(x => x.ID).FirstOrDefault();
            var lsQuyen = (from a in db.QuyenDetails.Where(x => x.IDQuyen == Idquyen && x.IDQuyenCN == 1 && x.isActive == 1)
                           join b in db.ListControllers.Where(x => x.isActive == 1) on a.IDController equals b.ID
                           select b.Controller).ToList();
            var mulQuyen = db.PhanQuyenHTs.Where(x=>x.IDNV == MyAuthentication.ID).ToList();
            if(mulQuyen.Count > 0)
            {
                foreach (var item in mulQuyen)
                {
                    var lsq = (from a in db.QuyenDetails.Where(x => x.IDQuyen == item.IDQuyen && x.IDQuyenCN == 1 && x.isActive == 1)
                               join b in db.ListControllers.Where(x => x.isActive == 1) on a.IDController equals b.ID
                               select b.Controller).ToList();
                    lsQuyen.AddRange(lsq);
                }
            }
            return lsQuyen;
        }

        public JsonResult Get_LichSu_KNL()
        {
            KNL_LSDG knl = new KNL_LSDG() { DAT = 0, KDGia = 0, CHUADG = 0, KDAT = 0, TONGNL = 0, VUOT = 0 };
            var res = db.KNL_LSDG.Where(x => x.NVID == MyAuthentication.ID).ToList();
            knl.NVID = MyAuthentication.ID;
            knl.VUOT = res.LastOrDefault()?.VUOT;
            knl.KDAT = res.LastOrDefault()?.KDAT;
            knl.DAT = res.LastOrDefault()?.DAT;
            knl.TONGNL = res.LastOrDefault()?.TONGNL;
            knl.ThangDG = res.LastOrDefault()?.ThangDG;
            knl.CHUADG = res.LastOrDefault()?.CHUADG;
            knl.KDGia = res.LastOrDefault()?.KDGia;
            return Json(knl, JsonRequestBehavior.AllowGet);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public JsonResult KeepSessionAlive()
        {
            return new JsonResult { Data = "Success" };
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult ManageBanner()
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);

            if (!ListQuyen.Contains(CONSTKEY.V))
            {
                TempData["msgError"] = "<script>alert('Bạn không có quyền truy cập chức năng này');</script>";
                return RedirectToAction("", "Home");
            }

            var banners = db.Banners.OrderBy(x => x.SortOrder).ToList();
            return View(banners);
        }

        [HttpPost]
        public ActionResult AddBanner(List<HttpPostedFileBase> files, int type)
        {
            if (files == null || !files.Any(file => file != null && file.ContentLength > 0))
            {
                return RedirectToAction("ManageBanner");
            }

            int currentOrder = db.Banners.Any() ? db.Banners.Max(x => x.SortOrder) : 0;

            string uploadFolder = Server.MapPath("~/Uploads/Banners/");
            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }

            foreach (var file in files.Where(file => file != null && file.ContentLength > 0))
            {
                string ext = Path.GetExtension(file.FileName);
                string uniqueName = Guid.NewGuid().ToString("N") + ext;

                string fullPath = Path.Combine(uploadFolder, uniqueName);
                file.SaveAs(fullPath);

                currentOrder++;

                db.Banners.Add(new Banners
                {
                    BannerPath = "/Uploads/Banners/" + uniqueName,
                    SortOrder = currentOrder,
                    IsActive = true,
                    CreatedDate = DateTime.Now,
                    Type = type // 1 = Home page, 2 = Login page
                });
            }

            db.SaveChanges();
            TempData["msgSuccess"] = "<script>alert('Thêm thành công!');</script>";

            return RedirectToAction("ManageBanner");
        }

        public ActionResult MoveBanner(int id, string direction)
        {
            var item = db.Banners.Find(id);
            if (item == null)
            {
                return RedirectToAction("ManageBanner");
            }

            Banners swapItem = null;

            if (direction == "up")
            {
                swapItem = db.Banners
                              .Where(x => x.SortOrder < item.SortOrder)
                              .OrderByDescending(x => x.SortOrder)
                              .FirstOrDefault();
            }
            else if (direction == "down")
            {
                swapItem = db.Banners
                              .Where(x => x.SortOrder > item.SortOrder)
                              .OrderBy(x => x.SortOrder)
                              .FirstOrDefault();
            }

            if (swapItem != null)
            {
                int temp = item.SortOrder;
                item.SortOrder = swapItem.SortOrder;
                swapItem.SortOrder = temp;

                db.SaveChanges();
            }

            return RedirectToAction("ManageBanner");
        }

        public ActionResult DeleteBanner(int id)
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);

            if (!ListQuyen.Contains(CONSTKEY.V))
            {
                TempData["msgError"] = "<script>alert('Bạn không có quyền truy cập chức năng này');</script>";
                return RedirectToAction("", "Home");
            }

            var item = db.Banners.Find(id);
            if (item != null)
            {
                db.Banners.Remove(item);
                db.SaveChanges();
            }
            return RedirectToAction("ManageBanner");
        }
    }
}