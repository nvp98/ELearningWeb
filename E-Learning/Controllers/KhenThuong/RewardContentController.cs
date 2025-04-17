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
        public ActionResult Index(int? page, string search, int? IDND)
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
                           NgayQuyetDinh = (DateTime) a.NgayQuyetDinh,
                           GiaTriLamLoi = a.GiaTriLamLoi != null ? (decimal) a.GiaTriLamLoi : 0,
                           TongTienThuong = a.TongTienThuong != null ? (decimal) a.TongTienThuong : 0,
                           LoaiKhenThuong = db.KT_LoaiThuong.Where(x => x.ID_Loai == a.ID_LoaiKhenThuong).FirstOrDefault().TenLoaiThuong,
                           BannerBase64 = a.BannerImageBase64
                       });

            if (IDND != 0) res = res.Where(a => a.ID == IDND);

            List<KT_NoiDungThuong> dt = db.KT_NoiDungThuong.ToList();
            // dropdown
            ViewBag.IDND = new SelectList(db.KT_NoiDungThuong, "ID", "NoiDungKhenThuong");

            int pageSize = 20;
            int pageNumber = (page ?? 1);

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
            var binaryReader = new BinaryReader(DTO.BannerUpload.InputStream);
            byte[] fileData = binaryReader.ReadBytes(DTO.BannerUpload.ContentLength);
            string base64Data = Convert.ToBase64String(fileData);
            string fullBase64Image = $"data:{DTO.BannerUpload.ContentType};base64,{base64Data}";
            try
            {
                var data = new KT_NoiDungThuong()
                {
                    SoQuyetDinh = DTO.SoQuyetDinh,
                    NoiDungKhenThuong = DTO.NoiDungKhenThuong,
                    ID_LoaiKhenThuong = DTO.IDLoaiKhenThuong,
                    NgayQuyetDinh = DTO.NgayQuyetDinh,
                    BannerImage = binaryReader.ReadBytes(DTO.BannerUpload.ContentLength),
                    BannerImageBase64 = fullBase64Image,
                    GiaTriLamLoi = DTO.GiaTriLamLoi,
                    TongTienThuong = DTO.TongTienThuong
                };

                db.KT_NoiDungThuong.Add(data);

                db.SaveChanges();
                TempData["msgSuccess"] = "<script>alert('Thêm mới thành công');</script>";
            } catch (Exception ex)
            {
                TempData["msgError"] = "<script>alert('Có lỗi khi thêm mới: " + ex.Message + "');</script>";
            }
            
            return RedirectToAction("Index", "RewardContent");
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
                BannerBase64 = data.BannerImageBase64,
                NgayQuyetDinh = data.NgayQuyetDinh,
                GiaTriLamLoi = data.GiaTriLamLoi != null ? data.GiaTriLamLoi : 0,
                TongTienThuong = data.TongTienThuong != null ? data.TongTienThuong : 0
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
                var data = db.KT_NoiDungThuong.Where(x => x.ID == DTO.ID).SingleOrDefault();
                data.NoiDungKhenThuong = DTO.NoiDungKhenThuong;
                data.SoQuyetDinh = DTO.SoQuyetDinh;
                data.NgayQuyetDinh = DTO.NgayQuyetDinh;
                data.GiaTriLamLoi = DTO.GiaTriLamLoi;
                data.TongTienThuong = DTO.TongTienThuong;

                var dataKT_DanhSachKhenThuong = db.KT_DanhSachKhenThuong.Where(x => x.ID_NoiDungThuong == DTO.ID).ToList();

                foreach (var item in dataKT_DanhSachKhenThuong)
                {
                    item.Nam = DTO.NgayQuyetDinh.Value.Year;
                    item.Thang = DTO.NgayQuyetDinh.Value.Month;
                }

                if (DTO.BannerUpload != null && DTO.BannerUpload.ContentLength > 0)
                {
                    using (var binaryReader = new BinaryReader(DTO.BannerUpload.InputStream))
                    {
                        data.BannerImage = binaryReader.ReadBytes(DTO.BannerUpload.ContentLength);

                        string base64String = "data:" + DTO.BannerUpload.ContentType + ";base64," + Convert.ToBase64String(data.BannerImage);
                        data.BannerImageBase64 = base64String;
                    }
                }

                db.SaveChanges();
                TempData["msgSuccess"] = "<script>alert('Cập nhật thành công');</script>";
            }
            catch (Exception e)
            {
                TempData["msgSuccess"] = "<script>alert('Cập nhật thất bại " + e.Message + " ');</script>";
            }

            return RedirectToAction("Index", "RewardContent");
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

        public static string GetImageMimeType(byte[] bytes)
        {
            if (bytes == null || bytes.Length < 4) return "application/octet-stream";

            if (bytes[0] == 0xFF && bytes[1] == 0xD8) return "image/jpeg";
            if (bytes[0] == 0x89 && bytes[1] == 0x50 &&
                bytes[2] == 0x4E && bytes[3] == 0x47) return "image/png";
            if (bytes[0] == 0x47 && bytes[1] == 0x49 &&
                bytes[2] == 0x46) return "image/gif";
            if (bytes[0] == 0x42 && bytes[1] == 0x4D) return "image/bmp";

            return "application/octet-stream";
        }

    }
}