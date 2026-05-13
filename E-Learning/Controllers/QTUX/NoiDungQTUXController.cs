using E_Learning.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_Learning.Controllers.QTUX
{
    public class NoiDungQTUXController : Controller
    {
        ELEARNINGEntities db_context = new ELEARNINGEntities();
        int Idquyen = MyAuthentication.IDQuyen;
        String ControllerName = "NoiDungQTUX";

        // GET: NoiDungQTUX
        public ActionResult Index(int? page, string search, int? IDQTUX)
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

            if (IDQTUX == null) IDQTUX = 0;

            var res = (from a in db_context.NoiDungDTs.Where(x => x.isNQ == 2)
                       join dk in db_context.QTUX_DinhKy on a.IDND equals dk.IDNoiDungQTUX into dkGroup
                       from dk in dkGroup.DefaultIfEmpty()
                       select new NoiQuyUXView
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
                           SLHT = db_context.QTUX_KetQua.Where(x => x.NoiDungDT.IDND == a.IDND && x.XNHT == 1).Count(),
                           SLHTFile = db_context.QTUX_KetQua.Where(x => x.NoiDungDT.IDND == a.IDND && x.XNHTFile == 1).Count(),
                           SLHoanThanhThi = db_context.BaiThis.Where(x => x.IDND == a.IDND && x.TinhTrang == true).Count(),
                           DinhKy = dk.DinhKy
                       }).OrderBy(x => x.isOrder).ToList();

            List<NoiDungDT> ctlvdt = db_context.NoiDungDTs.Where(x => x.isNQ == 2).OrderBy(x => x.isOrder).ToList();
            ViewBag.IDQTUX = new SelectList(ctlvdt, "IDND", "NoiDung");
            if (IDQTUX != 0) res = res.Where(x => x.IDND == IDQTUX).ToList();

            if (page == null) page = 1;
            int pageSize = 50;
            int pageNumber = (page ?? 1);
            return View(res.ToList().ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Create()
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            if (!ListQuyen.Contains(CONSTKEY.ADD))
            {
                TempData["msgError"] = "<script>alert('Bạn không có quyền thực hiện chức năng này');</script>";
                return RedirectToAction("", "Home");
            }
            var lastRecord = (from c in db_context.NoiDungDTs orderby c.IDND descending select c).FirstOrDefault();
            if (lastRecord == null)
            {
                ViewBag.MaND = "NDUX0000" + 1;
            }
            else if (Convert.ToInt32(lastRecord.IDND) < 9)
            {
                ViewBag.MaND = "NDUX0000" + (Convert.ToInt32(lastRecord.IDND) + 1);
            }
            else if (Convert.ToInt32(lastRecord.IDND) < 99)
            {
                ViewBag.MaND = "NDUX000" + (Convert.ToInt32(lastRecord.IDND) + 1);
            }
            else if (Convert.ToInt32(lastRecord.IDND) < 999)
            {
                ViewBag.MaND = "NDUX00" + (Convert.ToInt32(lastRecord.IDND) + 1);
            }
            else if (Convert.ToInt32(lastRecord.IDND) < 9999)
            {
                ViewBag.MaND = "NDUX0" + (Convert.ToInt32(lastRecord.IDND) + 1);
            }
            else
            {
                ViewBag.MaND = "NDUX" + (Convert.ToInt32(lastRecord.IDND) + 1);
            }

            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(NoiQuyUXView _DO, int? DinhKy)
        {
            try
            {
                string path = Server.MapPath("~/UploadedFiles/EduPro/");

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string FileName = _DO.PDFEduFile != null ? DateTime.Now.ToString("ddMMyyHHmmss") + _DO.MaND : "";
                string FileExtension = _DO.PDFEduFile != null ? Path.GetExtension(_DO.PDFEduFile.FileName) : "";

                if (_DO.PDFEduFile != null)
                {
                    FileName = FileName.Trim() + FileExtension;
                    _DO.PDFEduFile.SaveAs(path + FileName);
                    _DO.FileDinhKem = "/UploadedFiles/EduPro/" + FileName;
                }

                if (IsNDAvailable(_DO.MaND) == false)
                {
                    // Tạo trực tiếp NoiDungDT với isNQ=2 (QTUX)
                    var noiDung = new NoiDungDT
                    {
                        MaND = _DO.MaND,
                        NoiDung = _DO.NoiDung,
                        VideoND = _DO.VideoND,
                        ImageND = _DO.ImageND,
                        ThoiLuongDT = _DO.ThoiLuongDT,
                        FileDinhKem = _DO.FileDinhKem,
                        NgayTao = DateTime.Now,
                        isOrder = _DO.isOrder,
                        isNQ = 2  // ← QTUX = 2
                    };
                    db_context.NoiDungDTs.Add(noiDung);
                    db_context.SaveChanges();

                    // Lưu định kỳ nếu có
                    if (DinhKy.HasValue && DinhKy.Value > 0)
                    {
                        var dinhKyRecord = new QTUX_DinhKy
                        {
                            IDNoiDungQTUX = noiDung.IDND,
                            DinhKy = DinhKy.Value,
                            NgayTao = DateTime.Now
                        };
                        db_context.QTUX_DinhKy.Add(dinhKyRecord);
                        db_context.SaveChanges();
                    }

                    TempData["msgSuccess"] = "<script>alert('Thêm mới thành công');</script>";
                }
                else
                {
                    TempData["msgSuccess"] = "<script>alert('Nội dung đã tồn tại');</script>";
                }
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Có lỗi khi thêm mới: " + e.Message + "');</script>";
            }
            return RedirectToAction("Index", "NoiDungQTUX");
        }

        public bool IsNDAvailable(string MaND)
        {
            var IsCheck = (from k in db_context.NoiDungDTs
                           where (k.MaND.ToLower() == MaND)
                           select new { k.MaND }).FirstOrDefault();
            bool status;
            if (IsCheck != null)
            {
                status = true;
            }
            else
            {
                status = false;
            }
            return status;
        }

        public ActionResult Edit(int id)
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            if (!ListQuyen.Contains(CONSTKEY.EDIT))
            {
                TempData["msgError"] = "<script>alert('Bạn không có quyền thực hiện chức năng này');</script>";
                return RedirectToAction("", "Home");
            }
            var res = (from a in db_context.NoiDungDTs.Where(x => x.isNQ == 2 && x.IDND == id)
                       select new NoiQuyUXView
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
                       }).ToList();

            NoiQuyUXView DO = new NoiQuyUXView();

            if (res.Count > 0)
            {
                foreach (var co in res)
                {
                    DO.IDND = co.IDND;
                    DO.MaND = co.MaND;
                    DO.NoiDung = co.NoiDung;
                    DO.VideoND = co.VideoND;
                    DO.ImageND = co.ImageND;
                    DO.ThoiLuongDT = co.ThoiLuongDT;
                    DO.FileDinhKem = co.FileDinhKem;
                    DO.NgayTao = co.NgayTao;
                    DO.isOrder = co.isOrder;
                }
                ViewBag.NgayTao = DO.NgayTao?.ToString("yyyy-MM-dd");

                // Lấy định kỳ hiện tại
                var dinhKy = db_context.QTUX_DinhKy.FirstOrDefault(x => x.IDNoiDungQTUX == id);
                ViewBag.CurrentDinhKy = dinhKy?.DinhKy ?? 0;
            }
            else
            {
                return HttpNotFound();
            }
            return PartialView(DO);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(NoiQuyUXView _DO, int? DinhKy)
        {
            try
            {
                string path = Server.MapPath("~/UploadedFiles/EduPro/");

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string FileName = _DO.PDFEduFile != null ? DateTime.Now.ToString("ddMMyyHHmmss") + _DO.MaND : "";
                string FileExtension = _DO.PDFEduFile != null ? Path.GetExtension(_DO.PDFEduFile.FileName) : "";

                if (_DO.PDFEduFile != null)
                {
                    FileName = FileName.Trim() + FileExtension;
                    _DO.PDFEduFile.SaveAs(path + FileName);
                    _DO.FileDinhKem = "/UploadedFiles/EduPro/" + FileName;
                }

                // Cập nhật trực tiếp NoiDungDT
                var noiDung = db_context.NoiDungDTs.FirstOrDefault(x => x.IDND == _DO.IDND && x.isNQ == 2);
                if(_DO.FileDinhKem == null)
                {
                    _DO.FileDinhKem = noiDung.FileDinhKem;
                }
                if (noiDung != null)
                {
                    noiDung.MaND = _DO.MaND;
                    noiDung.NoiDung = _DO.NoiDung;
                    noiDung.VideoND = _DO.VideoND;
                    noiDung.ImageND = _DO.ImageND;
                    noiDung.ThoiLuongDT = _DO.ThoiLuongDT;
                    noiDung.FileDinhKem = _DO.FileDinhKem;
                    noiDung.isOrder = _DO.isOrder;
                    db_context.SaveChanges();

                    // Cập nhật định kỳ
                    var dinhKyRecord = db_context.QTUX_DinhKy.FirstOrDefault(x => x.IDNoiDungQTUX == _DO.IDND);
                    if (DinhKy.HasValue && DinhKy.Value > 0)
                    {
                        if (dinhKyRecord != null)
                        {
                            // Cập nhật
                            dinhKyRecord.DinhKy = DinhKy.Value;
                            db_context.SaveChanges();
                        }
                        else
                        {
                            // Tạo mới
                            var newDinhKy = new QTUX_DinhKy
                            {
                                IDNoiDungQTUX = _DO.IDND,
                                DinhKy = DinhKy.Value,
                                NgayTao = DateTime.Now
                            };
                            db_context.QTUX_DinhKy.Add(newDinhKy);
                            db_context.SaveChanges();
                        }
                    }
                    else
                    {
                        // Xóa định kỳ nếu chọn "Không lặp lại"
                        if (dinhKyRecord != null)
                        {
                            db_context.QTUX_DinhKy.Remove(dinhKyRecord);
                            db_context.SaveChanges();
                        }
                    }
                }

                TempData["msgSuccess"] = "<script>alert('Cập nhập thành công');</script>";
            }
            catch (Exception e)
            {
                TempData["msgSuccess"] = "<script>alert('Cập nhập thất bại " + e.Message + " ');</script>";
            }

            return RedirectToAction("Index", "NoiDungQTUX");
        }

        public ActionResult Delete(int id)
        {
            try
            {
                db_context.NoiDungDT_delete(id);
            }
            catch (Exception e)
            {
                TempData["msgSuccess"] = "<script>alert('Xóa dữ liệu thất bại: " + e.Message + "');</script>";
            }
            return RedirectToAction("Index", "NoiDungQTUX");
        }

        // ========== QUẢN LÝ ĐỀ THI VÀ CÂU HỎI ==========

        // Danh sách đề thi cho nội dung QTUX
        public ActionResult ManageExams(int? page, int? IDND)
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            if (!ListQuyen.Contains(CONSTKEY.V))
            {
                TempData["msgError"] = "<script>alert('Bạn không có quyền truy cập chức năng này');</script>";
                return RedirectToAction("", "Home");
            }

            if (IDND == null)
            {
                TempData["msgError"] = "<script>alert('Vui lòng chọn nội dung');</script>";
                return RedirectToAction("Index");
            }

            ViewBag.IDND = IDND;
            var noiDung = db_context.NoiDungDTs.FirstOrDefault(x => x.IDND == IDND && x.isNQ == 2);
            if (noiDung == null)
            {
                return HttpNotFound();
            }
            ViewBag.TenND = noiDung.NoiDung;

            var res = db_context.DeThis.Where(x => x.IDND == IDND).ToList();

            if (page == null) page = 1;
            int pageSize = 50;
            int pageNumber = (page ?? 1);
            return View(res.ToPagedList(pageNumber, pageSize));
        }

        // Tạo đề thi mới
        public ActionResult CreateExam(int IDND)
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            if (!ListQuyen.Contains(CONSTKEY.ADD))
            {
                TempData["msgError"] = "<script>alert('Bạn không có quyền thực hiện chức năng này');</script>";
                return RedirectToAction("ManageExams", new { IDND = IDND });
            }

            var noiDung = db_context.NoiDungDTs.FirstOrDefault(x => x.IDND == IDND && x.isNQ == 2);
            if (noiDung == null)
            {
                return HttpNotFound();
            }

            ViewBag.IDND = IDND;
            ViewBag.TenND = noiDung.NoiDung;

            var lastRecord = db_context.DeThis.OrderByDescending(x => x.IDDeThi).FirstOrDefault();
            string maDe = "DE" + (lastRecord != null ? (lastRecord.IDDeThi + 1).ToString().PadLeft(4, '0') : "0001");
            ViewBag.MaDe = maDe;

            return PartialView();
        }

        [HttpPost]
        public ActionResult CreateExam(int IDND, string MaDe, string TenDe, double? DiemChuan, int? ThoiGianLamBai)
        {
            try
            {
                var deThi = new DeThi
                {
                    MaDe = MaDe,
                    TenDe = TenDe,
                    IDND = IDND,
                    DiemChuan = DiemChuan ?? 5.0,
                    ThoiGianLamBai = ThoiGianLamBai ?? 30
                };
                db_context.DeThis.Add(deThi);
                db_context.SaveChanges();
                TempData["msgSuccess"] = "<script>alert('Tạo đề thi thành công');</script>";
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Lỗi: " + e.Message + "');</script>";
            }
            return RedirectToAction("ManageExams", new { IDND = IDND });
        }

        // Quản lý câu hỏi của đề thi
        public ActionResult ManageQuestions(int? page, int? IDDeThi)
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            if (!ListQuyen.Contains(CONSTKEY.V))
            {
                TempData["msgError"] = "<script>alert('Bạn không có quyền truy cập chức năng này');</script>";
                return RedirectToAction("", "Home");
            }

            if (IDDeThi == null)
            {
                return RedirectToAction("Index");
            }

            var deThi = db_context.DeThis.FirstOrDefault(x => x.IDDeThi == IDDeThi);
            if (deThi == null)
            {
                return HttpNotFound();
            }

            ViewBag.IDDeThi = IDDeThi;
            ViewBag.TenDeThi = deThi.TenDe;
            ViewBag.IDND = deThi.IDND;

            var res = (from chdt in db_context.CauHoiDeThis.Where(x => x.IDDeThi == IDDeThi)
                       join ch in db_context.CauHois on chdt.IDCauHoi equals ch.IDCH
                       select new CauHoiViewModel
                       {
                           IDCauHoiDeThi = chdt.IDCauHoiDeThi,
                           IDCauHoi = ch.IDCH,
                           NoiDungCauHoi = ch.NoiDungCH,
                           DapAnA = ch.DapAnA,
                           DapAnB = ch.DapAnB,
                           DapAnC = ch.DapAnC,
                           DapAnD = ch.DapAnD,
                           IDDAĐung = ch.IDDAĐung,
                           Diem = chdt.Diem
                       }).OrderBy(x => x.IDCauHoiDeThi).ToList();

            if (page == null) page = 1;
            int pageSize = 50;
            int pageNumber = (page ?? 1);
            return View(res.ToPagedList(pageNumber, pageSize));
        }

        // Thêm câu hỏi vào đề thi
        public ActionResult CreateQuestion(int IDDeThi)
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            if (!ListQuyen.Contains(CONSTKEY.ADD))
            {
                TempData["msgError"] = "<script>alert('Bạn không có quyền thực hiện chức năng này');</script>";
                return RedirectToAction("ManageQuestions", new { IDDeThi = IDDeThi });
            }

            var deThi = db_context.DeThis.FirstOrDefault(x => x.IDDeThi == IDDeThi);
            if (deThi == null)
            {
                return HttpNotFound();
            }

            ViewBag.IDDeThi = IDDeThi;
            ViewBag.TenDeThi = deThi.TenDe;
            return PartialView();
        }

        [HttpPost]
        public ActionResult CreateQuestion(int IDDeThi, string NoiDungCauHoi, string DapAnA, string DapAnB,
                                          string DapAnC, string DapAnD, int IDDAĐung, double Diem)
        {
            try
            {
                // Tạo câu hỏi mới
                var cauHoi = new CauHoi
                {
                    NoiDungCH = NoiDungCauHoi,
                    DapAnA = DapAnA,
                    DapAnB = DapAnB,
                    DapAnC = DapAnC,
                    DapAnD = DapAnD,
                    IDDAĐung = IDDAĐung,
                    IDND = db_context.DeThis.FirstOrDefault(x => x.IDDeThi == IDDeThi)?.IDND
                };
                db_context.CauHois.Add(cauHoi);
                db_context.SaveChanges();

                // Liên kết câu hỏi với đề thi
                var cauHoiDeThi = new CauHoiDeThi
                {
                    IDCauHoi = cauHoi.IDCH,
                    IDDeThi = IDDeThi,
                    Diem = Diem
                };
                db_context.CauHoiDeThis.Add(cauHoiDeThi);
                db_context.SaveChanges();

                TempData["msgSuccess"] = "<script>alert('Thêm câu hỏi thành công');</script>";
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Lỗi: " + e.Message + "');</script>";
            }
            return RedirectToAction("ManageQuestions", new { IDDeThi = IDDeThi });
        }

        // Sửa câu hỏi
        public ActionResult EditQuestion(int IDCauHoiDeThi, int IDDeThi)
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            if (!ListQuyen.Contains(CONSTKEY.EDIT))
            {
                TempData["msgError"] = "<script>alert('Bạn không có quyền thực hiện chức năng này');</script>";
                return RedirectToAction("ManageQuestions", new { IDDeThi = IDDeThi });
            }

            var chdt = db_context.CauHoiDeThis.FirstOrDefault(x => x.IDCauHoiDeThi == IDCauHoiDeThi);
            if (chdt == null)
            {
                return HttpNotFound();
            }

            var cauHoi = db_context.CauHois.FirstOrDefault(x => x.IDCH == chdt.IDCauHoi);
            ViewBag.IDDeThi = IDDeThi;
            ViewBag.IDCauHoiDeThi = IDCauHoiDeThi;
            ViewBag.TenDeThi = db_context.DeThis.FirstOrDefault(x => x.IDDeThi == IDDeThi)?.TenDe;

            var model = new CauHoiViewModel
            {
                IDCauHoiDeThi = chdt.IDCauHoiDeThi,
                IDCauHoi = cauHoi.IDCH,
                NoiDungCauHoi = cauHoi.NoiDungCH,
                DapAnA = cauHoi.DapAnA,
                DapAnB = cauHoi.DapAnB,
                DapAnC = cauHoi.DapAnC,
                DapAnD = cauHoi.DapAnD,
                IDDAĐung = cauHoi.IDDAĐung,
                Diem = chdt.Diem
            };

            return PartialView(model);
        }

        [HttpPost]
        public ActionResult EditQuestion(int IDCauHoi, int IDCauHoiDeThi, int IDDeThi, string NoiDungCauHoi,
                                        string DapAnA, string DapAnB, string DapAnC, string DapAnD,
                                        int IDDAĐung, double Diem)
        {
            try
            {
                var cauHoi = db_context.CauHois.FirstOrDefault(x => x.IDCH == IDCauHoi);
                if (cauHoi != null)
                {
                    cauHoi.NoiDungCH = NoiDungCauHoi;
                    cauHoi.DapAnA = DapAnA;
                    cauHoi.DapAnB = DapAnB;
                    cauHoi.DapAnC = DapAnC;
                    cauHoi.DapAnD = DapAnD;
                    cauHoi.IDDAĐung = IDDAĐung;
                    db_context.SaveChanges();
                }

                var chdt = db_context.CauHoiDeThis.FirstOrDefault(x => x.IDCauHoiDeThi == IDCauHoiDeThi);
                if (chdt != null)
                {
                    chdt.Diem = Diem;
                    db_context.SaveChanges();
                }

                TempData["msgSuccess"] = "<script>alert('Cập nhật câu hỏi thành công');</script>";
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Lỗi: " + e.Message + "');</script>";
            }
            return RedirectToAction("ManageQuestions", new { IDDeThi = IDDeThi });
        }

        // Xóa câu hỏi
        [HttpPost]
        public ActionResult DeleteQuestion(int IDCauHoiDeThi, int IDDeThi)
        {
            try
            {
                var chdt = db_context.CauHoiDeThis.FirstOrDefault(x => x.IDCauHoiDeThi == IDCauHoiDeThi);
                if (chdt != null)
                {
                    db_context.CauHoiDeThis.Remove(chdt);
                    db_context.SaveChanges();
                    TempData["msgSuccess"] = "<script>alert('Xóa câu hỏi thành công');</script>";
                }
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Lỗi: " + e.Message + "');</script>";
            }
            return RedirectToAction("ManageQuestions", new { IDDeThi = IDDeThi });
        }
    }
}
