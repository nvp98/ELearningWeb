using E_Learning.Models;
using E_Learning.ModelsDTTH;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Net;
using System.Data.Entity;
using ClosedXML.Excel;
using ExcelDataReader;
using System.IO;

namespace E_Learning.Controllers.DaoTaoTH
{
    public class HoSoDTTHController : Controller
    {
        ELEARNINGEntities db_context = new ELEARNINGEntities();
        int Idquyen = MyAuthentication.IDQuyen;
        String ControllerName = "HoSoDTTH";
        // GET: HoSoDTTH
        public ActionResult Index(int? page, string search, int? ID_TrangThai, int? ID_NoiDung,int? IDPhongBan)
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            ViewBag.QUYENCN = ListQuyen;
            if (!ListQuyen.Contains(CONSTKEY.V))
            {
                TempData["msgError"] = "<script>alert('Bạn không có quyền truy cập chức năng này');</script>";
                return RedirectToAction("", "Home");
            }
            if (search == null) search = "";
            //if (ID_TrangThai == null) ID_TrangThai = 0;
            var data = (from a in db_context.SH_HoSoDaoTao.Where(x=> ID_TrangThai ==null || x.TinhTrang == ID_TrangThai)
                        join b in db_context.LopHocs on a.LHID equals b.IDLH
                        join nc in db_context.SH_NhuCauDT on b.NCDT_ID equals nc.ID
                        join n in db_context.NoiDungDTs.Where(x=> search == ""|| x.NoiDung.Contains(search)) on b.NDID equals n.IDND
                        join c in db_context.NhanViens on a.ID_NguoiNopHS equals c.ID
                        join d in db_context.NhanViens on a.ID_NguoiXuLy equals d.ID into dGroup
                        from d in dGroup.DefaultIfEmpty() // LEFT JOIN
                        //join e in db_context.SH_FileScanHoSo on a.LHID equals e.IDLH into dj
                        //from e in dj.DefaultIfEmpty()
                        select new HoSoDaoTaoTH
                        {
                            ID = a.ID,
                            TenLopHoc = b.TenLH,
                            LHID = a.LHID,
                            ID_NguoiNopHS = a.ID_NguoiNopHS,
                            TenNguoiNop = string.Concat(c.MaNV, "-", c.HoTen),
                            NgayNopHS = a.NgayNopHS,
                            TinhTrang = a.TinhTrang,
                            ID_NguoiXuLy = a.ID_NguoiXuLy,
                            TenNguoiXuLy = d != null? string.Concat(d.MaNV, "-", d.HoTen):"",
                            NgayXuLy = a.NgayXuLy,
                            NgayBDThucTe = a.NgayBDThucTe,
                            NgayKTThucTe = a.NgayKTThucTe,
                            ThoiLuongDT = a.ThoiLuongDT,
                            manageClassValidation = 
                            new ManageClassValidation()
                            {
                                IDLH = b.IDLH,
                                MaLH = b.MaLH,
                                TenLH = b.TenLH,
                                TenND = n.NoiDung,
                                NDID = n.IDND,
                                NoiDungTrichYeu = b.NoiDungTrichYeu,
                                QuyDT = (int)b.QuyDT,
                                NamDT = (int)b.NamDT,
                                BoPhan_ID = b.BoPhan_ID,
                                TGBDLH = (DateTime)b.TGBDLH,
                                TGKTLH = (DateTime)b.TGKTLH,
                                ID_PPDT = nc.PhuongPhapDT_ID,
                                TenPPDT = db_context.SH_PhuongPhapDT.FirstOrDefault(x=>x.ID ==nc.PhuongPhapDT_ID).TenPhuongPhapDT,
                                TenGV = a.MaGiangVien +" - " + a.HoTenGV,
                                TenBoPhan = db_context.PhongBans.FirstOrDefault(x => x.IDPhongBan == b.BoPhan_ID).TenPhongBan,
                                TenNguoiTao = db_context.NhanViens.FirstOrDefault(x => x.ID == b.NguoiTao_ID).HoTen,
                                TenNguoiKiemTra = db_context.NhanViens.FirstOrDefault(x => x.ID == b.NguoiKiemTra_ID).HoTen,
                                LinhVuc = n.LinhVucDT.TenLVDT,
                                IDDeThi = b.IDDeThi??0,
                                TenDeThi = b.IDDeThi != null?db_context.DeThis.FirstOrDefault(x=>x.IDDeThi == b.IDDeThi).TenDe:""
                            }
                        }).ToList();
            if(ID_NoiDung != null)
            {
                data = data.Where(x=>x.manageClassValidation.NDID == ID_NoiDung).ToList();
            }

            if (IDPhongBan != null)
            {
                data = data.Where(x => x.manageClassValidation.BoPhan_ID == IDPhongBan).ToList();
            }

            if (!ListQuyen.Contains(CONSTKEY.VIEW_ALL) && !ListQuyen.Contains(CONSTKEY.V_BP)) data = data.Where(x => x.ID_NguoiNopHS == MyAuthentication.ID).ToList();
            else if (ListQuyen.Contains(CONSTKEY.V_BP)) data =  data.Where(x => x.manageClassValidation.BoPhan_ID == MyAuthentication.IDPhongban).ToList();

            List<PhongBan> pb = db_context.PhongBans.ToList();
            ViewBag.IDPhongBan = new SelectList(pb, "IDPhongBan", "TenPhongBan");

            List<NoiDungDT> lh = db_context.NoiDungDTs.ToList();
            ViewBag.IDNoiDung = new SelectList(lh, "IDND", "NoiDung");
            ViewBag.Search = search;
            if (page == null) page = 1;
            int pageSize = 100;
            int pageNumber = (page ?? 1);
            return View(data.ToList().OrderByDescending(x => x.NgayNopHS).ToPagedList(pageNumber, pageSize));
        }

        public ActionResult ViewEdit(int? IDLH)
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            ViewBag.QUYENCN = ListQuyen;
            var res = db_context.LopHocs.Find(IDLH);
            List<NhuCauDTTHView> ncdtData = (from a in db_context.SH_NhuCauDT.Where(x => x.ID == res.NCDT_ID)
                                             select new NhuCauDTTHView
                                             {
                                                 ID_NCDT = a.ID,
                                                 TenNoiDungDT = "Mã NCĐT: " + a.ID + " - " + a.NoiDungDT.NoiDung
                                             }).ToList();
            var IDPB = res.BoPhan_ID;
            var data = new ManageClassValidation();
            if (res != null)
            {
                var chitietGV = db_context.SH_ChiTietTCDT.FirstOrDefault(x => x.CTDT_ID == res.IDLH);
                data.MaLH = res.MaLH;
                data.TenLH = res.TenLH;
                data.NoiDungTrichYeu = res.NoiDungTrichYeu;
                data.TGBDLH = res.TGBDLH.Value;
                data.TGKTLH = res.TGKTLH.Value;
                data.DiaDiemDT = res.DiaDiemDT;
                data.ThoiLuongDT = res.ThoiLuongDT;
                data.NCDT_ID = (int)res.NCDT_ID;
                data.QuyDT = (int)res.QuyDT;
                data.IDDeThi = (int)res.IDDeThi;
                data.NguoiKiemTra_ID = res.NguoiKiemTra_ID;
                data.IDLH = res.IDLH;
                data.IsCoCTDT = res.IsCoCTDT;
                data.CTDT_ID = res.CTDT_ID;
                data.NDID = (int)res.NDID;
                data.NguoiTao_ID = res.NguoiTao_ID;
                data.NgayKiemTra = res.NgayKiemTra;
                data.TinhTrang = res.TinhTrang;
                if (chitietGV != null)
                {
                    data.chiTietToChucDTTH = new ChiTietToChucDTTHView()
                    {
                        DonVi_GV = chitietGV.DonVi_GV,
                        HoTenGV = chitietGV.HoTenGV,
                        MaNV_GV = chitietGV.MaNV_GV,
                        ViTriCV_GV = chitietGV.ViTriCV_GV,
                    };
                }
            }

            ViewBag.NCDT_DATA = new SelectList(ncdtData, "ID_NCDT", "TenNoiDungDT", data.NCDT_ID);
            //ViewBag.HocVien = (from a in  db_context.XNHocTaps.Where(x=>x.LHID == data.IDLH)
            //                  join b in db_context.NhanViens on a.NVID equals b.ID 
            //                  select b).ToList();

            ViewBag.HocVien = (from h in db_context.XNHocTaps.Where(x => x.LHID == data.IDLH)
                               join l in db_context.LopHocs on h.LHID equals l.IDLH
                               join n in db_context.NhanViens on h.NVID equals n.ID
                               join p in db_context.PhongBans on h.PBID equals p.IDPhongBan
                               join v in db_context.Vitris on h.VTID equals v.IDViTri
                               select new ConfirmEStudyValidation()
                               {
                                   IDHT = h.IDHT,
                                   PBID = (int)h.PBID,
                                   NVID = n.ID,
                                   MaNV = n.MaNV,
                                   HoTenHV = n.HoTen,
                                   TenPB = p.TenPhongBan,
                                   VTID = (int)h.VTID,
                                   TenVT = v.TenViTri,
                                   LHID = l.IDLH,
                                   TenLH = l.TenLH,
                                   TGBDLH = (DateTime)l.TGBDLH,
                                   TGKTLH = (DateTime)l.TGKTLH,
                                   LinhVuc = l.NoiDungDT.LinhVucDT.TenLVDT,
                                   TenND = l.NoiDungDT.NoiDung,
                                   TLDT = l.NoiDungDT.ThoiLuongDT ?? 0,
                                   NgayTG = (DateTime)(h.NgayTG ?? default(DateTime)),
                                   NgayHT = (DateTime)(h.NgayHT ?? default(DateTime)),
                                   XNTG = (bool)h.XNTG,
                                   XNHT = (bool)h.XNHT,
                                   DiemOnline = h.DiemOnline,
                                   DiemLyThuyet = h.DiemLyThuyet,
                                   DiemThucHanh = h.DiemThucHanh,
                                   DiemVanDap = h.DiemVanDap,
                                   KetLuan = h.KetLuan,
                                   LyDoKhongTGia = h.LyDoKhongTGia,
                                   TinhTrang = h.TinhTrang,
                               }).ToList();


            var listDeThi = new List<CauHoiDeThiCTDTTH>();

            if (data.NCDT_ID != null)
            {
                var ncdt = db_context.SH_NhuCauDT.FirstOrDefault(x => x.ID == data.NCDT_ID);

                if (ncdt != null)
                {
                    listDeThi = (from x in db_context.DeThis
                                 where x.IDND == ncdt.NoiDungDT_ID
                                 select new CauHoiDeThiCTDTTH
                                 {
                                     ID = x.IDDeThi,
                                     TenDeThi = string.Concat("Điểm chuẩn: ", x.DiemChuan, "-", x.TenDe),
                                 }).ToList();
                }
            }

            ViewBag.DETHI_DATA = new SelectList(listDeThi, "ID", "TenDeThi", data.IDDeThi);

            ViewBag.NoiDung = db_context.NoiDungDTs.Where(x => x.IDND == data.NDID).FirstOrDefault();

            var listNhanVien = db_context.NhanViens.Where(x => x.IDTinhTrangLV == 1)
                .Select(x => new EmployeeValidation { ID = x.ID, HoTen = x.MaNV + " - " + x.HoTen, IDPhongBan = (int)x.IDPhongBan })
                .ToList();

            int IDGVCty = db_context.SH_ChiTietTCDT.Where(x => x.CTDT_ID == data.IDLH).SingleOrDefault()?.ID_GVCty ?? 0;

            ViewBag.NguoiTao = db_context.NhanViens.FirstOrDefault(x => x.ID == data.NguoiTao_ID);
            ViewBag.NguoiKiemTra = db_context.NhanViens.FirstOrDefault(x => x.ID == data.NguoiKiemTra_ID);
            //ViewBag.PhuongPhapDT_ID = PhuongPhapDT_ID;
            //ViewBag.PhanLoaiNCDT_ID = PhanLoaiNCDT_ID;
            //ViewBag.LoaiNCDT = PhanLoaiNCDT_ID;
            //ViewBag.LoaiHinh_DT = db_context.SH_PhanLoaiNCDT.Where(x => x.IDLoai == PhanLoaiNCDT_ID).FirstOrDefault().LoaiHinhDT_ID;
            ViewBag.ChuongTrinhDT_ID = new SelectList(db_context.SH_ChuongTrinhDT.Where(x => x.ID_NoiDungDT == data.NDID), "IDCTDT", "TenChuongTrinhDT", data.CTDT_ID);

           
            ViewBag.Nam = db_context.SH_QuyDaoTao.First().AD_Nam;
            ViewBag.Quy = db_context.SH_QuyDaoTao.First().AD_Quy;
            ViewBag.BoPhan_ID = db_context.PhongBans.FirstOrDefault(x => x.IDPhongBan == IDPB).TenPhongBan;
            ViewBag.MaLH = data.MaLH;

            return View(data);
        }

        public ActionResult DSFileDinhKem(int? page, int? IDLH)
        {
            var data = (from a in db_context.SH_FileScanHoSo.Where(x => x.IDLH == IDLH) 
                        select new FileScanHoSoView
                        {
                            ID = a.ID,
                            IDLH= (int)a.IDLH,
                            LinkFile = a.FileDinhKem,
                            TenFile =a.TenFile
                        }
                        ).ToList();
            if (page == null) page = 1;
            int pageSize = 100;
            int pageNumber = (page ?? 1);
            return View(data.ToList().ToPagedList(pageNumber, pageSize));
        }

        public ActionResult XacNhanHS(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var data = db_context.SH_HoSoDaoTao.Where(x => x.LHID == id ).FirstOrDefault();
            if (data == null)
            {
                return HttpNotFound();
            }
            data.TinhTrang = 1;
            data.ID_NguoiXuLy = MyAuthentication.ID;
            data.NgayXuLy = DateTime.Now;
            // cập nhật tình trạng LopHoc
            var lophoc = db_context.LopHocs.FirstOrDefault(x => x.IDLH ==  id);
            lophoc.TinhTrang = 5;

            db_context.SaveChanges();
            TempData["msgSuccess"] = "<script>alert('Xác nhận thành công ');</script>";
            return RedirectToAction("Index", "HoSoDTTH");
        }

        public ActionResult KhongXacNhanHS(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var data = db_context.SH_HoSoDaoTao.Where(x => x.LHID == id).FirstOrDefault();
            if (data == null)
            {
                return HttpNotFound();
            }
            data.TinhTrang = 2;
            data.ID_NguoiXuLy = MyAuthentication.ID;
            data.NgayXuLy = DateTime.Now;
            // cập nhật tình trạng LopHoc
            var lophoc = db_context.LopHocs.FirstOrDefault(x => x.IDLH == id);
            lophoc.TinhTrang = 6;

            db_context.SaveChanges();
            TempData["msgSuccess"] = "<script>alert('Từ chối hồ sơ ');</script>";
            return RedirectToAction("Index", "HoSoDTTH");
        }

        public ActionResult ExportProcess()
        {
            //var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            //if (!ListQuyen.Contains(CONSTKEY.ADD))
            //{
            //    TempData["msgError"] = "<script>alert('Bạn không có quyền thực hiện chức năng này');</script>";
            //    return RedirectToAction("", "Home");
            //}


            return PartialView();
        }


        [HttpPost]
        public ActionResult ExportProcess(HoSoDaoTaoTH _DO, string SelectedMonth)
        {
            try
            {
                var startDate = ((DateTime)_DO.NgayBDThucTe).Date;
                var endDate = ((DateTime)_DO.NgayKTThucTe).Date.AddDays(1).AddTicks(-1);

                var data = (from h in db_context.XNHocTaps.AsNoTracking()
                            join hs in db_context.SH_HoSoDaoTao.Where(x=>x.TinhTrang == 1 && x.NgayXuLy >= startDate && x.NgayXuLy <= endDate) on h.LHID equals hs.LHID
                            join l in db_context.LopHocs.AsNoTracking() on h.LHID equals l.IDLH
                            join nc in db_context.SH_NhuCauDT.AsNoTracking() on l.NCDT_ID equals nc.ID
                            join pp in db_context.SH_PhuongPhapDT.AsNoTracking() on nc.PhuongPhapDT_ID equals pp.ID
                            join n in db_context.NhanViens.AsNoTracking() on h.NVID equals n.ID
                            join p in db_context.PhongBans.AsNoTracking() on h.PBID equals p.IDPhongBan
                            join pb in db_context.PhongBans.AsNoTracking() on l.BoPhan_ID equals pb.IDPhongBan
                            join nd in db_context.NoiDungDTs.AsNoTracking() on l.NDID equals nd.IDND 
                            select new ConfirmEStudyValidation()
                            {
                                IDHT = h.IDHT,
                                PBID = (int)h.PBID,
                                NVID = n.ID,
                                MaNV = n.MaNV,
                                HoTenHV = n.HoTen,
                                TenPB = p.TenPhongBan,
                                BoPhan_TCĐT = pb.TenPhongBan,
                                VTID = (int)h.VTID,
                                LHID = l.IDLH,
                                TenLH = l.TenLH,
                                TGBDLH = (DateTime)l.TGBDLH,
                                TGKTLH = (DateTime)l.TGKTLH,
                                LinhVuc = l.NoiDungDT.LinhVucDT.TenLVDT,
                                TenND = l.NoiDungDT.NoiDung,
                                TLDT = l.NoiDungDT.ThoiLuongDT ?? 0,
                                NgayTG = (DateTime)(h.NgayTG ?? default(DateTime)),
                                NgayHT = (DateTime)(h.NgayHT ?? default(DateTime)),
                                XNTG = (bool)h.XNTG,
                                XNHT = (bool)h.XNHT,
                                IDPPDaoTao = h.ID_PhuongPhapDT,
                                DiemOnline = h.DiemOnline,
                                DiemLyThuyet = h.DiemLyThuyet,
                                DiemThucHanh = h.DiemThucHanh,
                                DiemVanDap = h.DiemVanDap,
                                KetLuan = h.KetLuan,
                                LyDoKhongTGia = h.LyDoKhongTGia,
                                TinhTrang = h.TinhTrang,
                                hosodaotao = new HoSoDaoTaoTH
                                {
                                    HoTenGV = hs.HoTenGV,
                                    MaGiangVien = hs.MaGiangVien,
                                    DonViGV = hs.DonViGV,
                                    ThoiLuongDT = hs.ThoiLuongDT,
                                    NgayBDThucTe = hs.NgayBDThucTe,
                                    NgayKTThucTe = hs.NgayKTThucTe,
                                    NgayXuLy = hs.NgayXuLy,
                                    
                                },
                                noidungdt = new NoiDungDTTHView()
                                {
                                    IDND =nd.IDND,
                                    NoiDung =nd.NoiDung,
                                    IDNguonGV = nd.IDNguonGV,
                                    TenHoatDongDT =nd.SH_HoatDongDT.TenHoatDong,
                                    TenLVDT = nd.LinhVucDT.TenLVDT,
                                    TenNguonGV = nd.SH_NguonGV.TenNguonGV,
                                    TenPPDT = pp.TenPhuongPhapDT
                                }
                            }).ToList();
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("ToChucDaoTao");
                    //Header
                    worksheet.Cell(1, 1).Value = "STT";
                    worksheet.Cell(1, 2).Value = "BP Lập TCĐT";
                    worksheet.Cell(1, 3).Value = "Mã GV thực tế";
                    worksheet.Cell(1, 4).Value = "Tên GV thực tế";
                    worksheet.Cell(1, 5).Value = "Bộ phận GV";
                    worksheet.Cell(1, 6).Value = "Mã NDĐT";
                    worksheet.Cell(1, 7).Value = "Tên NDĐT";
                    worksheet.Cell(1, 8).Value = "Tên Lớp học";
                    worksheet.Cell(1, 9).Value = "TG Bắt đầu thực tế";
                    worksheet.Cell(1, 10).Value = "TG Kết thúc thực tế";
                    worksheet.Cell(1, 11).Value = "Thời lượng thực tế";
                    worksheet.Cell(1, 12).Value = "Hoạt động đào tạo";
                    worksheet.Cell(1, 13).Value = "Lĩnh Vực ĐT";
                    worksheet.Cell(1, 14).Value = "Mã học viên";
                    worksheet.Cell(1, 15).Value = "Tên học viên";
                    worksheet.Cell(1, 16).Value = "Bộ phận";
                    worksheet.Cell(1, 17).Value = "Kết quả ĐT (Đạt, Không đạt, Không tham gia)";
                    worksheet.Cell(1, 18).Value = "Phương Pháp ĐT";
                    worksheet.Cell(1, 19).Value = "Nguồn GV (Nội bộ/ thuê ngoài)";
                    worksheet.Cell(1, 20).Value = "Lý do không tham gia";
                    worksheet.Cell(1, 21).Value = "Ngày duyệt hồ sơ";

                    int row = 2; int stt = 1;
                    foreach (var item in data)
                    {
                        worksheet.Cell(row, 1).Value = stt;
                        worksheet.Cell(row, 2).Value = item.BoPhan_TCĐT;
                        worksheet.Cell(row, 3).Value = item.hosodaotao.MaGiangVien;
                        worksheet.Cell(row, 4).Value = item.hosodaotao.HoTenGV;
                        worksheet.Cell(row, 5).Value = item.hosodaotao.DonViGV;
                        worksheet.Cell(row, 6).Value = item.noidungdt.IDND;
                        worksheet.Cell(row, 7).Value = item.noidungdt.NoiDung;
                        worksheet.Cell(row, 8).Value = item.TenLH;
                        worksheet.Cell(row, 9).Value = item.hosodaotao.NgayBDThucTe.Value.ToString("dd/MM/yyyy");
                        worksheet.Cell(row, 10).Value = item.hosodaotao.NgayKTThucTe.Value.ToString("dd/MM/yyyy");
                        worksheet.Cell(row, 11).Value = item.hosodaotao.ThoiLuongDT;
                        worksheet.Cell(row, 12).Value = item.noidungdt.TenHoatDongDT;
                        worksheet.Cell(row, 13).Value = item.noidungdt.TenLVDT;
                        worksheet.Cell(row, 14).Value = item.MaNV;
                        worksheet.Cell(row, 15).Value = item.HoTenHV;
                        worksheet.Cell(row, 16).Value = item.TenPB;
                        worksheet.Cell(row, 17).Value = item.KetLuan ==1?"Đạt":item.KetLuan == 2?"Không đạt":"Không tham gia";
                        worksheet.Cell(row, 18).Value = item.noidungdt.TenPPDT;
                        worksheet.Cell(row, 19).Value = item.noidungdt.TenNguonGV;
                        worksheet.Cell(row, 20).Value = item.LyDoKhongTGia;
                        worksheet.Cell(row, 21).Value = item.hosodaotao.NgayXuLy.Value.ToString("dd/MM/yyyy");
                        row++; stt++;
                    }

                    var stream = new MemoryStream();
                    workbook.SaveAs(stream);
                    stream.Position = 0; // Reset con trỏ stream về đầu
                    string filename = "HoSoDaoTao_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".xlsx";

                    return File(stream, System.Net.Mime.MediaTypeNames.Application.Octet, filename);
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = "<script>alert('" + ex + "');window.location.href = '/ManageClass'</script>";
                return RedirectToAction("Index", "HoSoDTTH");
            }

        }

        public ActionResult HuyHoSo(int IDLH)
        {
            var hoSo = db_context.SH_HoSoDaoTao.Where(h => h.LHID == IDLH).FirstOrDefault();
            var lopHoc = db_context.LopHocs.Where(l => l.IDLH == IDLH).FirstOrDefault();
            if (hoSo.TinhTrang == 1)
            {
                hoSo.TinhTrang = 0;
                hoSo.NgayXuLy = null;
                hoSo.ID_NguoiXuLy = null;
                lopHoc.TinhTrang = 4;
            }
            else if (hoSo.TinhTrang == 0)
            {
                db_context.SH_HoSoDaoTao.Remove(hoSo);
                lopHoc.TinhTrang = 1;
            }

            db_context.SaveChanges();

            TempData["msgSuccess"] = "<script>alert('Gỡ hồ sơ thành công.');</script>";
            return RedirectToAction("Index");
        }
    }
}