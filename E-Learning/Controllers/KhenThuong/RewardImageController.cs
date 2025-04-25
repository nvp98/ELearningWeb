using E_Learning.Models;
using E_Learning.ModelsQLKhenThuong;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_Learning.Controllers.KhenThuong
{
    public class RewardImageController : Controller
    {
        ELEARNINGEntities db = new ELEARNINGEntities();
        int Idquyen = MyAuthentication.IDQuyen;
        String ControllerName = "RewardImage";
        // GET: RewardImage
        public ActionResult Index(string loaiAvatar = "all", int page = 1, int pageSize = 10)
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            ViewBag.QUYENCN = ListQuyen;
            if (!ListQuyen.Contains(CONSTKEY.V))
            {
                TempData["msgError"] = "<script>alert('Bạn không có quyền truy cập chức năng này');</script>";
                return RedirectToAction("", "Home");
            }

            var avatars = db.KT_HinhAnh.AsQueryable();

            if (loaiAvatar == "ca-nhan")
            {
                avatars = avatars.Where(a => a.LoaiDoiTuong == "CaNhan");
            }
            else if (loaiAvatar == "don-vi")
            {
                avatars = avatars.Where(a => a.LoaiDoiTuong == "DonVi");
            }

            var pagedList = avatars.OrderByDescending(x => x.ID).ToPagedList(page, pageSize);
            ViewBag.LoaiAvatar = loaiAvatar;

            return View(pagedList);
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
        public JsonResult GetAllDonVi()
        {
            var list = db.PhongBans
                .Select(d => new { Ma = d.MaPB, Ten = d.TenPhongBan })
                .OrderBy(x => x.Ten)
                .ToList();

            return Json(list.Select(x => new { MaDonVi = x.Ma, TenDonVi = x.Ten }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult LayThongTinNhanVien(string maNV)
        {
            var nv = db.NhanViens
                .Where(n => n.MaNV == maNV)
                .Select(n => new { Ma = n.MaNV, Ten = n.HoTen })
                .FirstOrDefault();

            return Json(nv, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UploadAvatar(AvatarUpload model)
        {
            if (model.File == null || model.File.ContentLength == 0)
            {
                return Json(new { success = false, message = "Vui lòng chọn ảnh hợp lệ." });
            }

            if (string.IsNullOrEmpty(model.MaDoiTuong) || string.IsNullOrEmpty(model.LoaiDoiTuong))
            {
                return Json(new { success = false, message = "Thiếu thông tin đối tượng." });
            }

            try
            {
                var fileExt = Path.GetExtension(model.File.FileName);
                var safeFileName = $"{Guid.NewGuid()}{fileExt}";
                var relativePath = "/Uploads/Avatars/" + safeFileName;
                var fullPath = Server.MapPath(relativePath);

                var folder = Path.GetDirectoryName(fullPath);
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                model.File.SaveAs(fullPath);

                var existing = db.KT_HinhAnh.FirstOrDefault(a => a.MaDoiTuong == model.MaDoiTuong && a.LoaiDoiTuong == model.LoaiDoiTuong);

                if (existing != null)
                {
                    existing.AvatarPath = relativePath;
                }
                else
                {
                    db.KT_HinhAnh.Add(new KT_HinhAnh
                    {
                        MaDoiTuong = model.MaDoiTuong,
                        LoaiDoiTuong = model.LoaiDoiTuong,
                        AvatarPath = relativePath,
                    });
                }

                db.SaveChanges();

                return Json(new
                {
                    success = true,
                    message = "Upload avatar thành công!",
                    avatarUrl = Url.Content(relativePath)
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var avatar = db.KT_HinhAnh.Find(id);
            if (avatar == null)
            {
                return HttpNotFound();
            }

            var serverPath = Server.MapPath(avatar.AvatarPath);
            if (System.IO.File.Exists(serverPath))
            {
                System.IO.File.Delete(serverPath);
            }

            db.KT_HinhAnh.Remove(avatar);
            db.SaveChanges();

            TempData["SuccessMessage"] = "Xóa thành công.";
            return RedirectToAction("Index");
        }

    }
}