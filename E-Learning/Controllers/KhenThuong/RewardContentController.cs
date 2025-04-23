using E_Learning.Models;
using E_Learning.ModelsQLKhenThuong;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace E_Learning.Controllers.KhenThuong
{
    public class RewardContentController : Controller
    {
        ELEARNINGEntities db = new ELEARNINGEntities();
        int Idquyen = MyAuthentication.IDQuyen;
        String ControllerName = "RewardContent";
        // GET: RewardContent
        public ActionResult Index(int? page, string search, int? IDND, int? highlightID = null)
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            ViewBag.QUYENCN = ListQuyen;
            if (!ListQuyen.Contains(CONSTKEY.V))
            {
                TempData["msgError"] = "<script>alert('Bạn không có quyền truy cập chức năng này');</script>";
                return RedirectToAction("", "Home");
            }

            if (search == null) search = "";
            ViewBag.search = search;
            if (IDND == null) IDND = 0;

            var res = (from a in db.KT_NoiDungThuong
                       select new NoiDungKhenThuongDTO
                       {
                           ID = a.ID,
                           IDLoaiKhenThuong = a.ID_LoaiKhenThuong != null ? (int)a.ID_LoaiKhenThuong : 0,
                           NoiDungKhenThuong = a.NoiDungKhenThuong,
                           SoQuyetDinh = a.SoQuyetDinh,
                           NgayQuyetDinh = (DateTime)a.NgayQuyetDinh,
                           GiaTriLamLoi = a.GiaTriLamLoi != null ? (decimal)a.GiaTriLamLoi : 0,
                           TongTienThuong = a.TongTienThuong != null ? (decimal)a.TongTienThuong : 0,
                           LoaiKhenThuong = db.KT_LoaiThuong.Where(x => x.ID_Loai == a.ID_LoaiKhenThuong).FirstOrDefault().TenLoaiThuong,
                           BannerImage = a.BannerImage
                       });

            if (IDND != 0) res = res.Where(a => a.ID == IDND);

            ViewBag.IDND = new SelectList(db.KT_NoiDungThuong, "ID", "NoiDungKhenThuong");

            int pageSize = 20;
            int pageNumber = (page ?? 1);

            if (highlightID.HasValue)
            {
                var list = res.OrderBy(a => a.ID).ToList();
                int index = list.FindIndex(x => x.ID == highlightID.Value);
                if (index >= 0)
                {
                    pageNumber = (index / pageSize) + 1;
                    ViewBag.HighlightID = highlightID.Value;
                }
            }

            return View(res.OrderBy(a => a.ID).ToPagedList(pageNumber, pageSize));
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

            List<KT_LoaiThuong> listLoaiThuong = db.KT_LoaiThuong.ToList();
            ViewBag.LoaiThuong = new SelectList(listLoaiThuong, "ID_Loai", "TenLoaiThuong");

            return PartialView();
        }

        [HttpPost]
        public ActionResult Create(NoiDungKhenThuongDTO DTO)
        {
            try
            {
                string fileName = null;

                if (DTO.BannerUpload != null && DTO.BannerUpload.ContentLength > 0)
                {
                    string folderPath = Server.MapPath("~/Uploads/Banners/");
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    fileName = Guid.NewGuid().ToString() + Path.GetExtension(DTO.BannerUpload.FileName);
                    string fullPath = Path.Combine(folderPath, fileName);

                    DTO.BannerUpload.SaveAs(fullPath);
                }

                var data = new KT_NoiDungThuong
                {
                    SoQuyetDinh = DTO.SoQuyetDinh,
                    NoiDungKhenThuong = DTO.NoiDungKhenThuong,
                    ID_LoaiKhenThuong = DTO.IDLoaiKhenThuong,
                    NgayQuyetDinh = DTO.NgayQuyetDinh,
                    GiaTriLamLoi = DTO.GiaTriLamLoi,
                    TongTienThuong = DTO.TongTienThuong,
                    BannerImage = fileName != null ? "/Uploads/Banners/" + fileName : null
                };

                db.KT_NoiDungThuong.Add(data);
                db.SaveChanges();

                TempData["msgSuccess"] = "<script>alert('Thêm mới thành công');</script>";
                return RedirectToAction("Index", new { highlightID = data.ID });
            }
            catch (Exception ex)
            {
                TempData["msgError"] = "<script>alert('Lỗi khi thêm mới: " + ex.Message + "');</script>";
                return RedirectToAction("Index");
            }
        }

        public ActionResult Edit(int id)
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            if (!ListQuyen.Contains(CONSTKEY.EDIT))
            {
                TempData["msgError"] = "<script>alert('Bạn không có quyền thực hiện chức năng này');</script>";
                return RedirectToAction("", "Home");
            }
            var data = db.KT_NoiDungThuong.Where(x => x.ID == id).SingleOrDefault();
            var DTO = new NoiDungKhenThuongDTO()
            {
                NoiDungKhenThuong = data.NoiDungKhenThuong != null ? data.NoiDungKhenThuong : "",
                SoQuyetDinh = data.SoQuyetDinh != null ? data.SoQuyetDinh : "",
                //BannerBase64 = data.BannerImageBase64,
                NgayQuyetDinh = data.NgayQuyetDinh,
                GiaTriLamLoi = data.GiaTriLamLoi != null ? data.GiaTriLamLoi : 0,
                TongTienThuong = data.TongTienThuong != null ? data.TongTienThuong : 0,
                BannerImage = data.BannerImage
            };
            List<KT_LoaiThuong> listLoaiThuong = db.KT_LoaiThuong.ToList();
            ViewBag.LoaiThuong = new SelectList(listLoaiThuong, "ID_Loai", "TenLoaiThuong", data.ID_LoaiKhenThuong);

            return PartialView(DTO);
        }

        [HttpPost]
        public ActionResult Edit(NoiDungKhenThuongDTO DTO)
        {
            try
            {
                var data = db.KT_NoiDungThuong.SingleOrDefault(x => x.ID == DTO.ID);
                if (data == null)
                {
                    TempData["msgError"] = "<script>alert('Không tìm thấy dữ liệu để cập nhật');</script>";
                    return RedirectToAction("Index", "RewardContent");
                }

                data.NoiDungKhenThuong = DTO.NoiDungKhenThuong;
                data.SoQuyetDinh = DTO.SoQuyetDinh;
                data.NgayQuyetDinh = DTO.NgayQuyetDinh;
                data.GiaTriLamLoi = DTO.GiaTriLamLoi;
                data.TongTienThuong = DTO.TongTienThuong;

                var dataKT_DanhSachKhenThuong = db.KT_DanhSachKhenThuong
                    .Where(x => x.ID_NoiDungThuong == DTO.ID)
                    .ToList();

                foreach (var item in dataKT_DanhSachKhenThuong)
                {
                    if (DTO.NgayQuyetDinh.HasValue)
                    {
                        item.Nam = DTO.NgayQuyetDinh.Value.Year;
                        item.Thang = DTO.NgayQuyetDinh.Value.Month;
                    }
                    item.NoiDungKhenThuong = DTO.NoiDungKhenThuong;
                }

                if (DTO.BannerUpload != null && DTO.BannerUpload.ContentLength > 0)
                {
                    string folderPath = Server.MapPath("~/Uploads/Banners/");
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    if (!string.IsNullOrEmpty(data.BannerImage))
                    {
                        string oldImagePath = Server.MapPath(data.BannerImage);
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(DTO.BannerUpload.FileName);
                    string fullPath = Path.Combine(folderPath, fileName);
                    DTO.BannerUpload.SaveAs(fullPath);
                    data.BannerImage = "/Uploads/Banners/" + fileName;
                }

                db.SaveChanges();
                TempData["msgSuccess"] = "<script>alert('Cập nhật thành công');</script>";
                return RedirectToAction("Index", "RewardContent", new { highlightID = data.ID });
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Cập nhật thất bại: " + e.Message + "');</script>";
                return RedirectToAction("Index", "RewardContent");
            }
        }

        public ActionResult Delete(int id)
        {
            try
            {
                var data = db.KT_NoiDungThuong.Where(x => x.ID == id).SingleOrDefault();
                db.KT_NoiDungThuong.Remove(data);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                TempData["msgSuccess"] = "<script>alert('Xóa thất bại " + e.Message + " ');</script>";
            }
            return RedirectToAction("Index", "RewardContent");
        }
    }
}