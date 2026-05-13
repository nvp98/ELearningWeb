using E_Learning.Models;
using E_Learning.ModelsQTUX;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_Learning.Controllers.QTUX
{
    public class QTUXTestController : Controller
    {
        ELEARNINGEntities db = new ELEARNINGEntities();
        int Idquyen = MyAuthentication.IDQuyen;
        String ControllerName = "QTUXTest";

        // GET: QTUXTest - Danh sách bài thi
        public ActionResult Index(int? page, int? IDND)
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            ViewBag.QUYENCN = ListQuyen;
            if (!ListQuyen.Contains(CONSTKEY.V))
            {
                TempData["msgError"] = "<script>alert('Bạn không có quyền truy cập chức năng này');</script>";
                return RedirectToAction("", "Home");
            }

            int NVID = MyAuthentication.ID;
            var res = (from nd in db.NoiDungDTs.Where(x => x.isNQ == 2)
                       join dt in db.DeThis.Where(x => x.IDND != null) on nd.IDND equals dt.IDND into ul
                       from dt in ul.DefaultIfEmpty()
                       where (IDND == null || nd.IDND == IDND)
                       select new NoiQuyUXKQView
                       {
                           IDND = nd.IDND,
                           MaND = nd.MaND,
                           NoiDung = nd.NoiDung,
                           ImageND = nd.ImageND,
                           isOrder = nd.isOrder,
                           IDDeThi = dt.IDDeThi,
                           TenDeThi = dt.TenDe,
                           LanThi = db.BaiThis.Where(x => x.IDND == nd.IDND && x.IDNV == NVID).Count(),
                       }).Distinct().ToList();

            List<NoiDungDT> ctlvdt = db.NoiDungDTs.Where(x => x.isNQ == 2).OrderBy(x => x.isOrder).ToList();
            ViewBag.IDND = new SelectList(ctlvdt, "IDND", "NoiDung", IDND);

            if (page == null) page = 1;
            int pageSize = 50;
            int pageNumber = (page ?? 1);
            return View(res.ToList().ToPagedList(pageNumber, pageSize));
        }

        // Trang chuẩn bị làm bài thi
        public ActionResult PrepareTest(int? deThiID)
        {
            if (deThiID == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            int NVID = MyAuthentication.ID;
            var deThi = db.DeThis.Find(deThiID);
            if (deThi == null)
            {
                return HttpNotFound();
            }

            // Lấy số câu hỏi
            var tongSoCau = db.CauHoiDeThis.Where(x => x.IDDeThi == deThiID).Count();

            ViewBag.TenDeThi = deThi.TenDe;
            ViewBag.DiemChuan = deThi.DiemChuan ?? 5.0;
            ViewBag.ThoiGianLamBai = deThi.ThoiGianLamBai ?? 30;
            ViewBag.TongSoCau = tongSoCau;

            return View(deThi);
        }

        // Làm bài thi
        public ActionResult DoTest(int? deThiID, int? baiThiID)
        {
            if (deThiID == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            int NVID = MyAuthentication.ID;
            var deThi = db.DeThis.Find(deThiID);
            if (deThi == null)
            {
                return HttpNotFound();
            }

            // Tạo bài thi mới nếu chưa có
            if (baiThiID == null)
            {
                var baiThi = new BaiThi
                {
                    IDND = deThi.IDND,
                    IDDeThi = deThiID,
                    IDNV = NVID,
                    NgayThi = DateTime.Now,
                    TinhTrang = null,
                    LanThi = (db.BaiThis.Where(x => x.IDND == deThi.IDND && x.IDNV == NVID).Max(x => (int?)x.LanThi) ?? 0) + 1
                };
                db.BaiThis.Add(baiThi);
                db.SaveChanges();
                baiThiID = baiThi.IDBaiThi;

                // Tạo CTBaiThi cho mỗi câu hỏi
                var cauHoiList = db.CauHoiDeThis.Where(x => x.IDDeThi == deThiID).ToList();
                foreach (var chdt in cauHoiList)
                {
                    var ctBaiThi = new CTBaiThi
                    {
                        IDBaiThi = baiThi.IDBaiThi,
                        IDCauHoi = chdt.IDCauHoi ?? 0,
                        IDDapAnDung = null,
                        IDDApAnNV = null,
                        Diem = null
                    };
                    db.CTBaiThis.Add(ctBaiThi);
                }
                db.SaveChanges();
            }

            var baiThiHienTai = db.BaiThis.Find(baiThiID);
            if (baiThiHienTai == null)
            {
                return HttpNotFound();
            }

            ViewBag.BaiThiID = baiThiID;
            ViewBag.DeThiID = deThiID;
            ViewBag.TenDeThi = deThi.TenDe;
            ViewBag.DiemChuan = deThi.DiemChuan ?? 5.0;
            ViewBag.ThoiGianLamBai = deThi.ThoiGianLamBai ?? 30;
            ViewBag.TongSoCau = db.CauHoiDeThis.Where(x => x.IDDeThi == deThiID).Count();

            // Lấy danh sách câu hỏi
            var cauHoiListDisplay = (from chdt in db.CauHoiDeThis.Where(x => x.IDDeThi == deThiID)
                                     join ch in db.CauHois on chdt.IDCauHoi equals ch.IDCH
                                     select new CauHoiViewModel
                                     {
                                         IDCauHoi = ch.IDCH,
                                         NoiDungCauHoi = ch.NoiDungCH,
                                         DapAnA = ch.DapAnA,
                                         DapAnB = ch.DapAnB,
                                         DapAnC = ch.DapAnC,
                                         DapAnD = ch.DapAnD,
                                         IDDAĐung = ch.IDDAĐung,
                                         Diem = chdt.Diem
                                     }).ToList();
            ViewBag.CauHoiList = cauHoiListDisplay;

            return View(deThi);
        }

        // Lưu câu trả lời
        [HttpPost]
        public JsonResult SaveAnswer(int baiThiID, int cauHoiID, int? dapAnID)
        {
            try
            {
                var existing = db.CTBaiThis.Where(x => x.IDBaiThi == baiThiID && x.IDCauHoi == cauHoiID).FirstOrDefault();
                if (existing != null)
                {
                    existing.IDDApAnNV = dapAnID;
                    db.SaveChanges();
                }
                else
                {
                    var ctBaiThi = new CTBaiThi
                    {
                        IDBaiThi = baiThiID,
                        IDCauHoi = cauHoiID,
                        IDDApAnNV = dapAnID
                    };
                    db.CTBaiThis.Add(ctBaiThi);
                    db.SaveChanges();
                }
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Nộp bài thi
        [HttpPost]
        public ActionResult SubmitTest(int baiThiID)
        {
            try
            {
                var baiThi = db.BaiThis.Find(baiThiID);
                if (baiThi == null)
                {
                    return HttpNotFound();
                }

                // Tính điểm
                double tongDiem = 0;
                var ctBaiThiList = db.CTBaiThis.Where(x => x.IDBaiThi == baiThiID).ToList();

                foreach (var ct in ctBaiThiList)
                {
                    var cauHoi = db.CauHois.Find(ct.IDCauHoi);
                    if (cauHoi != null)
                    {
                        // Lấy điểm từ CauHoiDeThi
                        var diemCauHoi = db.CauHoiDeThis
                            .Where(x => x.IDCauHoi == ct.IDCauHoi && x.IDDeThi == baiThi.IDDeThi)
                            .FirstOrDefault()?.Diem ?? 1.0;  // Mặc định 1 điểm

                        // Kiểm tra: Đáp án học viên == Đáp án đúng (IDDAĐung)
                        if (ct.IDDApAnNV == cauHoi.IDDAĐung)
                        {
                            ct.IDDapAnDung = cauHoi.IDDAĐung;
                            ct.Diem = (double?)diemCauHoi;
                            tongDiem += diemCauHoi;
                        }
                        else
                        {
                            ct.IDDapAnDung = cauHoi.IDDAĐung;
                            ct.Diem = 0;
                        }
                    }
                }
                db.SaveChanges();

                // Cập nhật kết quả bài thi
                var deThi = db.DeThis.Find(baiThi.IDDeThi);
                baiThi.DiemSo = tongDiem;
                baiThi.TinhTrang = tongDiem >= (deThi?.DiemChuan ?? 5.0) ? true : false;
                baiThi.NgayThi = DateTime.Now;
                db.SaveChanges();

                TempData["DiemSo"] = tongDiem;
                TempData["TinhTrang"] = baiThi.TinhTrang;
                return RedirectToAction("TestResult", new { baiThiID = baiThiID });
            }
            catch (Exception ex)
            {
                TempData["msgError"] = "<script>alert('Lỗi: " + ex.Message + "');</script>";
                return RedirectToAction("Index");
            }
        }

        // Kết quả bài thi
        public ActionResult TestResult(int? baiThiID)
        {
            if (baiThiID == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            var baiThi = db.BaiThis.Find(baiThiID);
            if (baiThi == null)
            {
                return HttpNotFound();
            }

            ViewBag.DiemSo = baiThi.DiemSo;
            ViewBag.TinhTrang = baiThi.TinhTrang == true ? "Đạt" : "Không đạt";
            ViewBag.LanThi = baiThi.LanThi;
            ViewBag.NgayThi = baiThi.NgayThi;

            var deThi = db.DeThis.Find(baiThi.IDDeThi);
            ViewBag.DiemChuan = deThi?.DiemChuan ?? 5.0;
            ViewBag.TenDeThi = deThi?.TenDe;

            // Lấy danh sách câu hỏi
            var cauHoiListDisplay = (from chdt in db.CauHoiDeThis.Where(x => x.IDDeThi == baiThi.IDDeThi)
                                     join ch in db.CauHois on chdt.IDCauHoi equals ch.IDCH
                                     select new CauHoiViewModel
                                     {
                                         IDCauHoi = ch.IDCH,
                                         NoiDungCauHoi = ch.NoiDungCH,
                                         DapAnA = ch.DapAnA,
                                         DapAnB = ch.DapAnB,
                                         DapAnC = ch.DapAnC,
                                         DapAnD = ch.DapAnD,
                                         IDDAĐung = ch.IDDAĐung,
                                         Diem = chdt.Diem
                                     }).ToList();
            ViewBag.CauHoiList = cauHoiListDisplay;

            var ctBaiThiList = db.CTBaiThis.Where(x => x.IDBaiThi == baiThiID).ToList();
            return View(ctBaiThiList);
        }
    }
}
