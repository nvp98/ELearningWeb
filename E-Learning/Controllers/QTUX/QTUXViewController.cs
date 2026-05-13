using E_Learning.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_Learning.Controllers.QTUX
{
    public class QTUXViewController : Controller
    {
        ELEARNINGEntities db_context = new ELEARNINGEntities();
        int Idquyen = MyAuthentication.IDQuyen;
        String ControllerName = "QTUXView";

        // GET: QTUXView - Hiển thị nội dung chi tiết
        public ActionResult Index(int id)
        {
            var res = (from n in db_context.NoiDungDTs.Where(x => x.IDND == id && x.isNQ == 2)
                       select new ManageETContentValidation
                       {
                           IDND = n.IDND,
                           MaND = n.MaND,
                           NoiDung = n.NoiDung,
                           ImageND = n.ImageND,
                           VideoND = n.VideoND,
                       }).ToList();

            return View(res);
        }

        // Danh sách Quy Tắc Ứng Xử cho học viên
        public ActionResult ListQTUX(int? page, string search, int? IDQTUX)
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            ViewBag.QUYENCN = ListQuyen;
            if (search == null) search = "";
            ViewBag.search = search;

            if (IDQTUX == null) IDQTUX = 0;
            int NVID = MyAuthentication.ID;

            // Lấy danh sách nội dung QTUX
            var noiDungList = db_context.NoiDungDTs.Where(x => x.isNQ == 2).OrderBy(x => x.isOrder).ToList();

            var res = new List<NoiQuyUXKQView>();

            foreach (var a in noiDungList)
            {
                // Lấy thông tin xem video/tài liệu
                var ketQua = db_context.QTUX_KetQua.FirstOrDefault(x => x.NoiDungDT.IDND == a.IDND && x.NhanVien.ID == NVID);

                // Lấy thông tin bài thi (đề thi đầu tiên của nội dung)
                var deThi = db_context.DeThis.FirstOrDefault(x => x.IDND == a.IDND);
                var baiThiGanNhat = deThi != null ? db_context.BaiThis
                    .Where(x => x.IDND == a.IDND && x.IDNV == NVID && x.IDDeThi == deThi.IDDeThi)
                    .OrderByDescending(x => x.IDBaiThi)
                    .FirstOrDefault() : null;

                var soLanThi = deThi != null ? db_context.BaiThis.Where(x => x.IDND == a.IDND && x.IDNV == NVID && x.IDDeThi == deThi.IDDeThi).Count() : 0;

                res.Add(new NoiQuyUXKQView
                {
                    IDND = a.IDND,
                    MaND = a.MaND,
                    NoiDung = a.NoiDung,
                    VideoND = a.VideoND,
                    ImageND = a.ImageND,
                    ThoiLuongDT = (int?)a.ThoiLuongDT ?? 0,
                    FileDinhKem = a.FileDinhKem,
                    NgayTao = a.NgayTao,
                    isOrder = a.isOrder,
                    XNTG = ketQua?.XNTG,
                    XNHT = ketQua?.XNHT,
                    XNHTFile = ketQua?.XNHTFile,
                    NgayHT = ketQua?.NgayHT,
                    NgayTG = ketQua?.NgayTG,
                    TinhTrang = ketQua?.TinhTrang,
                    // Thông tin bài thi
                    IDDeThi = deThi?.IDDeThi,
                    TenDeThi = deThi?.TenDe,
                    DiemSo = baiThiGanNhat?.DiemSo,
                    NgayThi = baiThiGanNhat?.NgayThi,
                    LanThi = soLanThi,
                    TinhTrangThi = baiThiGanNhat?.TinhTrang == true ? 1 : (baiThiGanNhat?.TinhTrang == false ? 0 : (int?)null)
                });
            }

            // Áp dụng bộ lọc
            if (IDQTUX != 0) res = res.Where(x => x.IDND == IDQTUX).ToList();

            List<NoiDungDT> ctlvdt = db_context.NoiDungDTs.Where(x => x.isNQ == 2).OrderBy(x => x.isOrder).ToList();
            ViewBag.IDQTUX = new SelectList(ctlvdt, "IDND", "NoiDung");

            if (page == null) page = 1;
            int pageSize = 50;
            int pageNumber = (page ?? 1);
            return View(res.ToList().ToPagedList(pageNumber, pageSize));
        }
    }
}
