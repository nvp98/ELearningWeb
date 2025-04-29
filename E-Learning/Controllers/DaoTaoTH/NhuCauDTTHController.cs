using Antlr.Runtime.Misc;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using E_Learning.Common;
using E_Learning.Models;
using E_Learning.ModelsDTTH;
using E_Learning.ModelsQTHD;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls.WebParts;
using iTextSharp.text;
using Font = iTextSharp.text.Font;
using System.Text;
using iText.Html2pdf;
using iText.Layout.Font;
using iText.Signatures; // Nếu xử lý thêm ký số
using Org.BouncyCastle.Crypto;
using iText.Kernel.Pdf.Event;
using iTextSharp.tool.xml.html;
using ClosedXML.Excel;
using ClosedXML;
using System.Web.UI;

namespace E_Learning.Controllers.DaoTaoTH
{
    public class NhuCauDTTHController : Controller
    {
        private ELEARNINGEntities db = new ELEARNINGEntities();
        int Idquyen = MyAuthentication.IDQuyen;
        String ControllerName = "NhuCauDTTH";
        // GET: NhuCauDTTH
        public ActionResult Index(int? page, string search, int? IDLoaiDT)
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            ViewBag.QUYENCN = ListQuyen;
            if (!ListQuyen.Contains(CONSTKEY.V))
            {
                TempData["msgError"] = "<script>alert('Bạn không có quyền truy cập chức năng này');</script>";
                return RedirectToAction("", "Home");
            }
            if (IDLoaiDT == null) IDLoaiDT = 1;
            var noiDungDTs = (from a in db.SH_NhuCauDT.Where(x =>
                              (search == null || x.NoiDungDT.NoiDung.Contains(search)) &&
                              (IDLoaiDT == null || x.PhanLoaiNCDT_ID == IDLoaiDT))
                              join b in db.SH_ChiTiet_NCDT on a.ID equals b.NhuCauDT_ID
                              join c in db.PhongBans on a.BoPhanLNC_ID equals c.IDPhongBan into uli from c in uli.DefaultIfEmpty()
                              join d in db.NhanViens on b.GiangVien_ID equals d.ID into ulis from d in ulis.DefaultIfEmpty()
                              select new NhuCauDTTHView
                              {
                                 ID_NCDT = a.ID,
                                 NoiDungDT_ID = a.NoiDungDT_ID,
                                 TenNoiDungDT = a.NoiDungDT.NoiDung,
                                 Quy = a.Quy,
                                 Nam =a.Nam,
                                 BoPhanLNC_ID = a.BoPhanLNC_ID,
                                 TenBoPhan_LNC = c.TenPhongBan,
                                 NguoiTao = a.NhanVien.MaNV +"-" + a.NhanVien.HoTen,
                                 TinhTrang =a.TinhTrang,
                                 PhuongPhapDT_ID = a.PhuongPhapDT_ID,
                                 FileDinhKem = a.FileDinhKem,
                                 TenPPDT = db.SH_PhuongPhapDT.Where(x=>x.ID == a.PhuongPhapDT_ID).FirstOrDefault().TenPhuongPhapDT,
                                 chiTietNhuCauDTTHView = new ChiTietNhuCauDTTHView
                                  {
                                      SoLuongNguoi = b.SoLuongNguoi,
                                      GiangVien_ID = b.GiangVien_ID,
                                      DiaDiemDT = b.DiaDiemDT,
                                      GhiChu = b.GhiChu,
                                      DonViDT =b.DonViDT,
                                      ThoiLuong =b.ThoiLuong_DT,
                                      ThoiGianDT =b.ThoiGian_DT,
                                      TenGiangVien= b.GiangVien_ID != null? d.HoTen: b.GiangVien_HoTen,
                                      TenViTri =b.GiangVien_Vitri,
                                      DoiTuongDT =b.DoiTuongDT,
                                      TenNhom = a.NoiDungDT.NhomNLKCCD.NoiDung,
                                      TenLVDT=a.NoiDungDT.LinhVucDT.TenLVDT,
                                      MaNV= d.MaNV,
                                  },
                                 PhanLoaiNCDT_ID = a.PhanLoaiNCDT_ID,
                                 ID_NguoiTao = (int)a.NguoiTao_ID,
                                 SLCauHoi = db.CauHois.Where(x=>x.IDND == a.NoiDungDT_ID).Count(),
                                 SLDeThi = db.DeThis.Where(x=>x.IDND == a.NoiDungDT_ID).Count()
                              }).OrderBy(x => x.ID_NCDT).ToList();
            if (!ListQuyen.Contains(CONSTKEY.VIEW_ALL) && !ListQuyen.Contains(CONSTKEY.V_BP)) noiDungDTs = noiDungDTs.Where(x => x.ID_NguoiTao == MyAuthentication.ID).ToList();
            else if (ListQuyen.Contains(CONSTKEY.V_BP)) noiDungDTs = noiDungDTs.Where(x => x.BoPhanLNC_ID == MyAuthentication.IDPhongban).ToList();

            ViewBag.IDPhanLoaiDT = new SelectList(db.SH_PhanLoaiNCDT.Where(x=>x.IDLoai ==1 || x.IDLoai == 4), "IDLoai", "TenLoaiNCDT");
            ViewBag.Search = search;
            if (page == null) page = 1;
            int pageSize = 50;
            int pageNumber = (page ?? 1);
            return View(noiDungDTs.ToList().ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Create(int? IDLoaiDT)
        {
            var IDPB = MyAuthentication.IDPhongban;
            ViewBag.IDPhanLoaiDT = new SelectList(db.SH_PhanLoaiNDDT, "ID", "TenPhanLoaiDT");
            ViewBag.IDPPDT = new SelectList(db.SH_PhuongPhapDT, "ID", "TenPhuongPhapDT");
            ViewBag.IDPhongBan = new SelectList(db.PhongBans.Where(x=>x.IDPhongBan ==IDPB), "IDPhongBan", "TenPhongBan",IDPB);
            //ViewBag.IDNoiDungDT = new SelectList(db.NoiDungDTs.Where(x=>x.isNQ == null).Select(x => new
            //{
            //    Value = x.IDND,
            //    Text = x.IDND + " - " + db.SH_PhanLoaiNDDT.Where(a=>a.ID == x.IDPhanLoaiDT).FirstOrDefault().TenPhanLoaiDT + " - " + x.NoiDung // Concatenate fields here
            //}),
            //"Value",
            //"Text");
          
            ViewBag.IDDKNhacLai = new SelectList(db.SH_DinhKy, "MaDK", "DKNhacLai");
            // thuê ngoài set mặc định loại nội dung thuê ngoài khác
            var loaiHinhDT_ID = db.SH_PhanLoaiNCDT.Where(x => x.IDLoai == IDLoaiDT).FirstOrDefault().LoaiHinhDT_ID;
            if(loaiHinhDT_ID ==2)
            {
                ViewBag.IDNoiDungDT = new SelectList(db.NoiDungDTs.Where(x=>x.IDPhanLoaiDT ==8).ToList(),"IDND","NoiDung");
            }
            else
            {
                ViewBag.IDNoiDungDT = new SelectList(Enumerable.Empty<SelectListItem>(),"Value","Text");
            }

            ViewBag.LoaiNCDT = IDLoaiDT;
            ViewBag.LoaiHinh_DT = db.SH_PhanLoaiNCDT.Where(x => x.IDLoai == IDLoaiDT).FirstOrDefault().LoaiHinhDT_ID;
            ViewBag.Nam = db.SH_QuyDaoTao.First().AD_Nam;
            ViewBag.Quy = db.SH_QuyDaoTao.First().AD_Quy;
            var ppdt = db.SH_PhuongPhapDT.ToList();
            if(IDLoaiDT != 1)
            {
                ppdt = ppdt.Where(x => x.ID != 1 || x.ID != 4).ToList();
            }
            ViewBag.PhuongPhapDT_ID = new SelectList(db.SH_PhuongPhapDT, "ID", "TenPhuongPhapDT");
            var nv2 = db.NhanViens.Where(x => x.IDTinhTrangLV == 1 && x.IDPhongBan ==IDPB).Select(x => new EmployeeValidation { ID =x.ID , HoTen = x.MaNV + " - " + x.HoTen + "-" + x.Vitri.TenViTri }).ToList();
            ViewBag.Selec = new SelectList(nv2, "ID", "HoTen");
            var vt = (from a in db.ViTriKNLs.Where(x => x.IDPB == IDPB)
                      join e in db.KNL_PhanXuong
                      on a.IDPX equals e.ID into ul
                      from e in ul.DefaultIfEmpty()
                      join f in db.KNL_Nhom
                      on a.IDNhom equals f.IDNhom into uls
                      from f in uls.DefaultIfEmpty()
                      join g in db.KNL_To
                      on a.IDTo equals g.IDTo into ulk
                      from g in ulk.DefaultIfEmpty()
                      select new ViTriKNLValidation
                      {
                          IDVT = a.IDVT,
                          TenViTri = a.TenViTri + "-" + e.TenPX + "-" + f.TenNhom + "-" + g.TenTo,
                          IDNhom = a.IDNhom,
                          IDTo = a.IDTo,
                          MaViTri = a.MaViTri,
                          IDPX = a.IDPX,
                          IDVTParent = a.IDVTParent
                      }).ToList();

            ViewBag.DSViTriKNL = new SelectList(vt, "IDVT", "TenViTri");
            
            // Tạo một danh sách các tùy chọn
            var categories = new List<SelectListItem>
    {
        new SelectListItem { Value = "Ban giám đốc", Text = "Ban giám đốc" },
        new SelectListItem { Value = "Trưởng bộ phận", Text = "Trưởng bộ phận" },
        new SelectListItem { Value = "Phòng nhân sự", Text = "Phòng nhân sự" }
    };

            // Gán danh sách vào ViewBag hoặc ViewModel
            ViewBag.DSNguoiNhan = new SelectList(categories, "Value", "Text");

            // trình ký
            var nhanvien = db.NhanViens.Where(x => x.IDTinhTrangLV == 1).Select(x => new EmployeeValidation { ID = x.ID, HoTen = x.MaNV + " - " + x.HoTen, IDPhongBan = (int)x.IDPhongBan }).ToList();

            ViewBag.BPSD = new SelectList(nhanvien.Where(x => x.IDPhongBan == MyAuthentication.IDPhongban), "ID", "HoTen");
            ViewBag.PCHN = new SelectList(nhanvien, "ID", "HoTen");
            var phongns = db.PhongBans.Where(x => x.MaPB.Contains("P.NS")).First();
            var bangiamdoc = db.PhongBans.Where(x => x.MaPB.Contains("BGD")).First();
            ViewBag.PNS = new SelectList(nhanvien.Where(x => x.IDPhongBan == phongns.IDPhongBan), "ID", "HoTen");
            ViewBag.BGD = new SelectList(nhanvien.Where(x => x.IDPhongBan == bangiamdoc.IDPhongBan), "ID", "HoTen");

            return PartialView();
        }

        // POST: NoiDungDTTH/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( NhuCauDTTHView nhucau, FormCollection form, IEnumerable<HttpPostedFileBase> files, string action)
        {
          
            try {
                if (ModelState.IsValid || true)
                {
                    var quy = db.SH_QuyDaoTao.First();
                    var selectedItems = form.GetValues("SelectedItems");
                    string filePathSave = null;
                    int countSl = 0;
                    //upload file NCDT
                    if (nhucau.File != null )
                    {
                        string path = Server.MapPath("~/FileNCDTTH/");
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        //Use Namespace called :  System.IO  
                        string FileName = nhucau.File.FileName;
                        string FileNameSave = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + Path.GetFileNameWithoutExtension(FileName);
                        //To Get File Extension  
                        string FileExtension = nhucau.File != null ? Path.GetExtension(FileName) : "";
                        //Add Current Date To Attached File Name  
                        if (FileExtension != ".pdf")
                        {
                            //TempData["msgError"] = "<script>alert('Vui lòng chọn đúng định dạng file PDF');</script>";
                            //return View();
                        }
                        else
                        {
                            if (nhucau.File != null)
                            {
                                FileNameSave = FileNameSave.Trim() + FileExtension;
                                nhucau.File.SaveAs(path + FileNameSave);
                                filePathSave = "~/FileNCDTTH/" + FileNameSave;
                            }
                        }
                    }
                    // Xứ lý dữ liệu khi NCĐT thuê ngoài 
                    if (nhucau.PhanLoaiNCDT_ID != 1)
                    {
                        var noidungdt = db.NoiDungDTs.Where(x=>x.IDND == nhucau.NoiDungDT_ID).FirstOrDefault();
                        if(noidungdt.IDPhuongPhapDT == null)
                        {
                            TempData["msgSuccess"] = "<script>alert('Chưa có Phương pháp đào tạo. Vui lòng kiểm tra lại nội dung đào tạo ');</script>";
                            return RedirectToAction("Index", new { IDLoaiDT = nhucau.PhanLoaiNCDT_ID });
                        }
                        nhucau.PhuongPhapDT_ID = noidungdt.IDPhuongPhapDT;
                    }


                    var parent = new SH_NhuCauDT { NoiDungDT_ID = nhucau.NoiDungDT_ID,
                        Nam = quy.AD_Nam,
                        Quy = quy.AD_Quy,
                        NguoiTao_ID = MyAuthentication.ID, 
                        PhanLoaiNCDT_ID = nhucau.PhanLoaiNCDT_ID,
                        MaDinhKy = nhucau.MaDinhKy,
                        BoPhanLNC_ID =MyAuthentication.IDPhongban,
                        NgayTao = DateTime.Now,
                        PhuongPhapDT_ID = nhucau.PhuongPhapDT_ID,
                        TinhTrang=0,
                        FileDinhKem = filePathSave
                    };
                    db.SH_NhuCauDT.Add(parent);
                    db.SaveChanges();
                    // Lấy Id của Parent mới tạo
                    int parentId = parent.ID;

                    // check nhân viên
                    var gv = db.NhanViens.Where(x => x.ID == nhucau.chiTietNhuCauDTTHView.GiangVien_ID).FirstOrDefault();
                    // thêm chi tiết NCĐT
                    var child = new SH_ChiTiet_NCDT
                    {
                        NhuCauDT_ID = parentId,
                        GhiChu = nhucau.chiTietNhuCauDTTHView.GhiChu,
                        SoLuongNguoi = countSl,
                        DoiTuongDT = nhucau.chiTietNhuCauDTTHView.DoiTuongDT,
                        GiangVien_ID = nhucau.chiTietNhuCauDTTHView.GiangVien_ID,
                        GiangVien_Vitri = nhucau.chiTietNhuCauDTTHView.TenViTri,
                        ThoiGian_DT = nhucau.chiTietNhuCauDTTHView.ThoiGianDT,
                        ThoiLuong_DT = nhucau.chiTietNhuCauDTTHView.ThoiLuong,
                        DiaDiemDT = nhucau.chiTietNhuCauDTTHView.DiaDiemDT,
                        DonViDT = nhucau.chiTietNhuCauDTTHView.DonViDT,
                        GiangVien_HoTen = nhucau.chiTietNhuCauDTTHView.TenGiangVien
                        //BoPhanDT_ID = gv.IDPhongBan
                    };
                    var loai = db.SH_PhanLoaiNCDT.Where(x => x.IDLoai == nhucau.PhanLoaiNCDT_ID).FirstOrDefault().LoaiHinhDT_ID;
                    if (gv != null)
                    {
                        child.DonViDT = gv?.PhongBan.TenPhongBan;
                        child.GiangVien_Vitri = gv?.Vitri.TenViTri;
                        child.GiangVien_HoTen = gv.HoTen;
                        child.GiangVien_ID = gv.ID;
                    }
                    db.SH_ChiTiet_NCDT.Add(child);
                    db.SaveChanges();

                  

                    // Thêm ds nhân viên khi chọn NCĐT thuê ngoài
                    if (nhucau.PhanLoaiNCDT_ID != 1)
                    {
                        if (!String.IsNullOrEmpty(nhucau.DSNhanVien))
                        {
                            //Regex.Replace(_DO.NVDG, @"[^0-9a-zA-Z]+", " ");
                            string tx = Regex.Replace(nhucau.DSNhanVien, @"[^0-9a-zA-Z]+", " ");
                            string[] NVS = tx.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (var item in NVS)
                            {
                                var aa = db.NhanViens.Where(x => x.MaNV == item).FirstOrDefault();
                                if (aa != null)
                                {
                                    var checktrung = db.SH_KetQuaDaoTao.Where(x => x.NhanVien_ID == aa.ID && x.NCDT_ID == parentId).ToList();
                                    if(checktrung.Count() == 0)
                                    {
                                        SH_KetQuaDaoTao a = new SH_KetQuaDaoTao()
                                        {
                                            NCDT_ID = parentId,
                                            NhanVien_ID = aa.ID,
                                        };
                                        db.SH_KetQuaDaoTao.Add(a);
                                        countSl = countSl + 1;
                                    }
                                }
                            }
                            db.SaveChanges();
                        }
                        if (!String.IsNullOrEmpty(nhucau.DSViTri))
                        {
                            //Regex.Replace(_DO.NVDG, @"[^0-9a-zA-Z]+", " ");
                            string tx = Regex.Replace(nhucau.DSViTri, @"[^0-9a-zA-Z]+", " ");
                            string[] NVS = tx.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (var item in NVS)
                            {
                                int idvt = int.Parse(item);
                                var aa = db.ViTriKNLs.Where(x => x.IDVT == idvt).FirstOrDefault();
                                if (aa != null)
                                {
                                    var vt = db.SH_ViTri_NDDT.Where(x=>x.Vitri_ID == idvt && x.NCDT_ID == parentId).ToList();
                                    if (vt.Count() == 0)
                                    {
                                        SH_ViTri_NDDT sH_ViTri_NDDT = new SH_ViTri_NDDT() { 
                                        NCDT_ID=parentId,
                                        Vitri_ID = idvt,
                                        NoiDungDT_ID = nhucau.NoiDungDT_ID,
                                        PhuongPhapDT_ID = nhucau.PhuongPhapDT_ID,
                                        };
                                        db.SH_ViTri_NDDT.Add(sH_ViTri_NDDT);
                                    }
                                    countSl = countSl + db.NhanViens.Where(x => x.IDVTKNL == idvt && x.IDTinhTrangLV == 1).Count();
                                }
                                
                            }
                            db.SaveChanges();
                        }

                    }


                    // Update Phan quyen
                    
                    if (selectedItems != null) // nội bộ
                    {
                        
                        foreach (var item in selectedItems)
                        {
                            // Xử lý từng giá trị đã chọn
                            var vt = db.SH_ViTri_NDDT.Find(int.Parse(item));
                            vt.NCDT_ID = parentId;
                            db.SaveChanges();
                            countSl = countSl + db.NhanViens.Where(x=>x.IDVTKNL == vt.Vitri_ID && x.IDTinhTrangLV ==1).Count();
                        }
                    }

                    // update số lượng Nhân viên
                    var chitiet = db.SH_ChiTiet_NCDT.FirstOrDefault(x=>x.NhuCauDT_ID == parentId);
                    chitiet.SoLuongNguoi = countSl;
                    db.SaveChanges();

                   
                    //duyệt BPSD
                    var CapDuyet = nhucau.CapDuyetView;
                    if (CapDuyet.BPSD != null)
                    {
                        SH_KyDuyetNCDT duyet = new SH_KyDuyetNCDT()
                        {
                            NCDT_ID = parentId,
                            NguoiDuyet_ID = CapDuyet.BPSD,
                            CapDuyet = 1,
                            TinhTrangDuyet = 0,
                        };
                        db.SH_KyDuyetNCDT.Add(duyet);
                        parent.MaTrinhKy = 1;
                        parent.TinhTrang = 2; // Đã trình ký
                        db.SaveChanges();
                    }
                    //duyệt PCHN
                    if (CapDuyet.PCHN != null)
                    {
                        SH_KyDuyetNCDT duyet = new SH_KyDuyetNCDT()
                        {
                            NCDT_ID = parentId,
                            NguoiDuyet_ID = CapDuyet.PCHN,
                            CapDuyet = 2,
                            TinhTrangDuyet = 0,
                        };
                        db.SH_KyDuyetNCDT.Add(duyet);
                        parent.MaTrinhKy = 2;
                        parent.TinhTrang = 2; // Đã trình ký
                        db.SaveChanges();
                    }
                    //duyệt PNS
                    if (CapDuyet.PNS != null)
                    {
                        SH_KyDuyetNCDT duyet = new SH_KyDuyetNCDT()
                        {
                            NCDT_ID = parentId,
                            NguoiDuyet_ID = CapDuyet.PNS,
                            CapDuyet = 3,
                            TinhTrangDuyet = 0,
                        };
                        db.SH_KyDuyetNCDT.Add(duyet);
                        parent.MaTrinhKy = 3;
                        parent.TinhTrang = 2; // Đã trình ký
                        db.SaveChanges();
                    }
                    //duyệt PNS
                    if (CapDuyet.BGD != null)
                    {
                        SH_KyDuyetNCDT duyet = new SH_KyDuyetNCDT()
                        {
                            NCDT_ID = parentId,
                            NguoiDuyet_ID = CapDuyet.BGD,
                            CapDuyet = 4,
                            TinhTrangDuyet = 0,
                        };
                        db.SH_KyDuyetNCDT.Add(duyet);
                        parent.MaTrinhKy = 4;
                        parent.TinhTrang = 2; // Đã trình ký
                        db.SaveChanges();
                        
                    }
                    // thêm thông tin trình ký
                    if (action == "Lưu")
                    {
                        parent.TinhTrang = 0; // Đang lưu
                        db.SaveChanges();
                    }

                    TempData["msgSuccess"] = "<script>alert('Thêm mới thành công ');</script>";
                    return RedirectToAction("Index",new {IDLoaiDT = nhucau.PhanLoaiNCDT_ID });
                }
            }
            catch (Exception ex)
            {
                TempData["msgSuccess"] = "<script>alert('Cập nhập thất bại " + ex.Message + "');</script>";
            }

            //ViewBag.LVDTID = new SelectList(db.LinhVucDTs, "IDLVDT", "TenLVDT");
            return RedirectToAction("Index",new { IDLoaiDT = nhucau.PhanLoaiNCDT_ID });
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SH_NhuCauDT nhuCauDT = db.SH_NhuCauDT.Find(id);
            if (nhuCauDT == null)
            {
                return HttpNotFound();
            }
            var IDPB = MyAuthentication.IDPhongban;
            ViewBag.IDPhanLoaiDT = new SelectList(db.SH_PhanLoaiNDDT, "ID", "TenPhanLoaiDT",nhuCauDT.PhanLoaiNCDT_ID);
            ViewBag.IDPhongBan = new SelectList(db.PhongBans.Where(x => x.IDPhongBan == IDPB), "IDPhongBan", "TenPhongBan", IDPB);
            //ViewBag.IDNoiDungDT = new SelectList(db.NoiDungDTs.Where(x=>x.IDND == nhuCauDT.NoiDungDT_ID), "IDND", "NoiDung",nhuCauDT.NoiDungDT_ID);
            ViewBag.IDNoiDungDT = new SelectList(db.NoiDungDTs.Where(x => x.IDND == nhuCauDT.NoiDungDT_ID).Select(x => new
            {
                Value = x.IDND,
                Text = x.IDND + " - " + db.SH_PhanLoaiNDDT.Where(a => x.IDND == nhuCauDT.NoiDungDT_ID).FirstOrDefault().TenPhanLoaiDT + " - " + x.NoiDung // Concatenate fields here
            }),
            "Value",
            "Text", nhuCauDT.NoiDungDT_ID);
            ViewBag.IDDKNhacLai = new SelectList(db.SH_DinhKy, "MaDK", "DKNhacLai",nhuCauDT.MaDinhKy);

            ViewBag.LoaiNCDT = nhuCauDT.PhanLoaiNCDT_ID;
            ViewBag.LoaiHinh_DT = db.SH_PhanLoaiNCDT.Where(x => x.IDLoai == nhuCauDT.PhanLoaiNCDT_ID).FirstOrDefault().LoaiHinhDT_ID;
            ViewBag.Nam = db.SH_QuyDaoTao.First().AD_Nam;
            ViewBag.Quy = db.SH_QuyDaoTao.First().AD_Quy;
            ViewBag.PhuongPhapDT_ID = new SelectList(db.SH_PhuongPhapDT.Where(x=>x.ID == nhuCauDT.PhuongPhapDT_ID), "ID", "TenPhuongPhapDT",nhuCauDT.PhuongPhapDT_ID);
            var nv2 = db.NhanViens.Where(x => x.IDTinhTrangLV == 1 && x.IDPhongBan == IDPB).Select(x => new EmployeeValidation { ID = x.ID, HoTen = x.MaNV + " - " + x.HoTen + "-" + x.Vitri.TenViTri }).ToList();
            ViewBag.Selec = new SelectList(nv2, "ID", "HoTen",nhuCauDT.SH_ChiTiet_NCDT.FirstOrDefault().GiangVien_ID);

            var ls = new List<SH_ViTri_NDDTView>();
           
                var data = db.VitriKNL_search().ToList();
                var vt = db.SH_ViTri_NDDT.Where(x=> x.NoiDungDT_ID == nhuCauDT.NoiDungDT_ID && x.PhuongPhapDT_ID == nhuCauDT.PhuongPhapDT_ID).ToList();
                if(nhuCauDT.PhanLoaiNCDT_ID == 1) // NCĐT nội bộ
                {
                    data = data.Where(x => x.IDPB == IDPB).ToList();
                }
                ls = (from a in vt
                      join b in data on a.Vitri_ID equals b.IDVT
                      select new SH_ViTri_NDDTView()
                      {
                          ID = a.ID,
                          TenViTri = b.IDVT + "-" + b.TenViTri + "-" + b.MaPB + "-" + b.TenPX + "-" + b.TenNhom + "-" + b.TenTo,
                          Tinhtrang = a.NCDT_ID == nhuCauDT.ID ?1:0
                      }).ToList();
            ViewBag.DSVitri = ls;

            // trình ký
            var trinhky = db.SH_KyDuyetNCDT.Where(x => x.NCDT_ID == nhuCauDT.ID).ToList();
            var nhanvien = db.NhanViens.Where(x => x.IDTinhTrangLV == 1).Select(x => new EmployeeValidation { ID = x.ID, HoTen = x.MaNV + " - " + x.HoTen, IDPhongBan = (int)x.IDPhongBan }).ToList();

            ViewBag.BPSD = new SelectList(nhanvien.Where(x => x.IDPhongBan == MyAuthentication.IDPhongban), "ID", "HoTen", trinhky.Where(x=>x.CapDuyet == 1).FirstOrDefault()?.NguoiDuyet_ID);
            ViewBag.PCHN = new SelectList(nhanvien, "ID", "HoTen", trinhky.Where(x => x.CapDuyet == 2).FirstOrDefault()?.NguoiDuyet_ID);
            var phongns = db.PhongBans.Where(x => x.MaPB.Contains("P.NS")).First();
            var bangiamdoc = db.PhongBans.Where(x => x.MaPB.Contains("BGD")).First();
            ViewBag.PNS = new SelectList(nhanvien.Where(x => x.IDPhongBan == phongns.IDPhongBan), "ID", "HoTen", trinhky.Where(x => x.CapDuyet == 3).FirstOrDefault()?.NguoiDuyet_ID);
            ViewBag.BGD = new SelectList(nhanvien.Where(x => x.IDPhongBan == bangiamdoc.IDPhongBan), "ID", "HoTen", trinhky.Where(x => x.CapDuyet == 4).FirstOrDefault()?.NguoiDuyet_ID);

            var noiDungDTs = (from a in db.SH_NhuCauDT.Where(x =>x.ID == id)
                              join b in db.SH_ChiTiet_NCDT on a.ID equals b.NhuCauDT_ID
                              join c in db.PhongBans on a.BoPhanLNC_ID equals c.IDPhongBan into uli
                              from c in uli.DefaultIfEmpty()
                              join d in db.NhanViens.Where(x => x.IDTinhTrangLV == 1) on a.NguoiTao_ID equals d.ID
                              select new NhuCauDTTHView
                              {
                                  ID_NCDT = a.ID,
                                  NoiDungDT_ID = a.NoiDungDT_ID,
                                  TenNoiDungDT = a.NoiDungDT.NoiDung,
                                  Quy = a.Quy,
                                  Nam = a.Nam,
                                  BoPhanLNC_ID = a.BoPhanLNC_ID,
                                  TenBoPhan_LNC = c.TenPhongBan,
                                  NguoiTao = d.MaNV + "-" + d.HoTen,
                                  TinhTrang = a.TinhTrang,
                                  FileDinhKem = a.FileDinhKem,
                                  chiTietNhuCauDTTHView = new ChiTietNhuCauDTTHView
                                  {
                                      SoLuongNguoi = b.SoLuongNguoi,
                                      DonViDT = b.DonViDT,
                                      DiaDiemDT = b.DiaDiemDT,
                                      ThoiGianDT =b.ThoiGian_DT,
                                      GhiChu = b.GhiChu,
                                      ThoiLuong = b.ThoiLuong_DT,
                                      DoiTuongDT = b.DoiTuongDT,
                                      TenViTri = b.GiangVien_Vitri,
                                      TenGiangVien = b.GiangVien_HoTen
                                  },
                              }).OrderBy(x => x.ID_NCDT).FirstOrDefault();

            return View(noiDungDTs);
        }

        public ActionResult Delete(int id)
        {
            SH_NhuCauDT noiDungDT = db.SH_NhuCauDT.Find(id);
            var loaiDT = noiDungDT.PhanLoaiNCDT_ID;
            // check trước xóa
            var tcdt = db.LopHocs.FirstOrDefault(x=>x.NCDT_ID ==  id);
            if(tcdt != null)
            {
                TempData["msgSuccess"] = "<script>alert('NCĐT đã được mở lớp, Không thể xóa! ');</script>";
                return RedirectToAction("Index", new { IDLoaiDT = loaiDT });
            }
            SH_ChiTiet_NCDT chitiet = db.SH_ChiTiet_NCDT.Where(x=>x.NhuCauDT_ID == id).FirstOrDefault();
            db.SH_ChiTiet_NCDT.Remove(chitiet);
          
            db.SH_NhuCauDT.Remove(noiDungDT);
            // cập nhật phân quyền ViTri_NDDT 
            List<SH_ViTri_NDDT> lsvt = db.SH_ViTri_NDDT.Where(x => x.NCDT_ID == id).ToList();
            foreach (var item in lsvt)
            {
                item.NCDT_ID = null;
                db.SaveChanges();
            }
            db.SaveChanges();
            TempData["msgSuccess"] = "<script>alert('Xóa thành công ');</script>";
            return RedirectToAction("Index", new { IDLoaiDT = loaiDT });
        }
        // POST: NoiDungDTTH/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(NhuCauDTTHView nhucau, FormCollection form, string action)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    List<SH_ViTri_NDDT> dsvitri = db.SH_ViTri_NDDT.Where(x=>x.NCDT_ID == nhucau.ID_NCDT).ToList();
                    if (nhucau.PhanLoaiNCDT_ID != 1)
                    {
                        //db.SH_ViTri_NDDT.RemoveRange(dsvitri);
                        //db.SaveChanges();
                    }
                    else
                    {
                        foreach (var item in dsvitri)
                        {
                            // Cập nhật kết quả chọn về null
                            item.NCDT_ID = null;
                            db.SaveChanges();
                        }
                    }
                    // nội bộ
                    var selectedItems = form.GetValues("SelectedItems");
                    // Update Phan quyen
                    int countSl = nhucau.chiTietNhuCauDTTHView?.SoLuongNguoi??0;
                    if (selectedItems != null)
                    {
                        countSl = 0;
                        foreach (var item in selectedItems)
                        {
                            // Xử lý từng giá trị đã chọn
                            var vt = db.SH_ViTri_NDDT.Find(int.Parse(item));
                            vt.NCDT_ID = nhucau.ID_NCDT;
                            db.SaveChanges();
                            countSl = countSl + db.NhanViens.Where(x => x.IDVTKNL == vt.Vitri_ID && x.IDTinhTrangLV == 1).Count();
                        }
                    }
                    //thuê ngoài
                    // Thêm ds nhân viên khi chọn NCĐT thuê ngoài
                    if (nhucau.PhanLoaiNCDT_ID != 1)
                    {
                        if (!String.IsNullOrEmpty(nhucau.DSNhanVien))
                        {
                            //Regex.Replace(_DO.NVDG, @"[^0-9a-zA-Z]+", " ");
                            string tx = Regex.Replace(nhucau.DSNhanVien, @"[^0-9a-zA-Z]+", " ");
                            string[] NVS = tx.Split(new char[] { ' ' });
                            foreach (var item in NVS)
                            {
                                var aa = db.NhanViens.Where(x => x.MaNV == item).FirstOrDefault();
                                if (aa != null)
                                {
                                    var checktrung = db.SH_KetQuaDaoTao.Where(x => x.NhanVien_ID == aa.ID && x.NCDT_ID == nhucau.ID_NCDT).ToList();
                                    if (checktrung.Count() == 0)
                                    {
                                        SH_KetQuaDaoTao a = new SH_KetQuaDaoTao()
                                        {
                                            NCDT_ID = nhucau.ID_NCDT,
                                            NhanVien_ID = aa.ID,
                                        };
                                        db.SH_KetQuaDaoTao.Add(a);
                                        countSl = countSl + 1;
                                    }
                                }
                            }
                            db.SaveChanges();
                        }
                        if (!String.IsNullOrEmpty(nhucau.DSViTri))
                        {
                            //Regex.Replace(_DO.NVDG, @"[^0-9a-zA-Z]+", " ");
                            string tx = Regex.Replace(nhucau.DSViTri, @"[^0-9a-zA-Z]+", " ");
                            string[] NVS = tx.Split(new char[] { ' ' });
                            foreach (var item in NVS)
                            {
                                int idvt = int.Parse(item);
                                var aa = db.ViTriKNLs.Where(x => x.IDVT == idvt).FirstOrDefault();
                                if (aa != null)
                                {
                                    var vt = db.SH_ViTri_NDDT.Where(x => x.Vitri_ID == idvt && x.NCDT_ID == nhucau.ID_NCDT).ToList();
                                    if (vt.Count() == 0)
                                    {
                                        SH_ViTri_NDDT sH_ViTri_NDDT = new SH_ViTri_NDDT()
                                        {
                                            NCDT_ID = nhucau.ID_NCDT,
                                            Vitri_ID = idvt,
                                            NoiDungDT_ID = nhucau.NoiDungDT_ID,
                                            PhuongPhapDT_ID = nhucau.PhuongPhapDT_ID,
                                        };
                                        db.SH_ViTri_NDDT.Add(sH_ViTri_NDDT);
                                    }
                                    countSl = countSl + db.NhanViens.Where(x => x.IDVTKNL == idvt && x.IDTinhTrangLV == 1).Count();
                                }

                            }
                            db.SaveChanges();
                        }

                    }



                    var nhucaudt = db.SH_NhuCauDT.FirstOrDefault(e => e.ID == nhucau.ID_NCDT);
                    string filePathSave = nhucaudt.FileDinhKem;
                    //upload file NCDT
                    if (nhucau.File != null)
                    {
                        string path = Server.MapPath("~/FileNCDTTH/");
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        //Use Namespace called :  System.IO  
                        string FileName = nhucau.File.FileName;
                        string FileNameSave = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + Path.GetFileNameWithoutExtension(FileName);
                        //To Get File Extension  
                        string FileExtension = nhucau.File != null ? Path.GetExtension(FileName) : "";
                        //Add Current Date To Attached File Name  
                        if (FileExtension != ".pdf")
                        {
                            //TempData["msgError"] = "<script>alert('Vui lòng chọn đúng định dạng file PDF');</script>";
                            //return View();
                        }
                        else
                        {
                            if (nhucau.File != null)
                            {
                                FileNameSave = FileNameSave.Trim() + FileExtension;
                                nhucau.File.SaveAs(path + FileNameSave);
                                filePathSave = "~/FileNCDTTH/" + FileNameSave;
                            }
                        }
                    }
                    if (nhucaudt != null)
                    {
                        nhucaudt.MaDinhKy = nhucau.MaDinhKy;
                        nhucaudt.FileDinhKem = filePathSave;
                        db.SaveChanges();
                    }
                    var chitietNCDT = db.SH_ChiTiet_NCDT.FirstOrDefault(e => e.NhuCauDT_ID == nhucau.ID_NCDT);
                    if(chitietNCDT != null)
                    {
                        chitietNCDT.DoiTuongDT = nhucau.chiTietNhuCauDTTHView.DoiTuongDT;
                        chitietNCDT.SoLuongNguoi = countSl;
                        chitietNCDT.GiangVien_ID = nhucau.chiTietNhuCauDTTHView.GiangVien_ID;
                        chitietNCDT.GiangVien_HoTen = nhucau.chiTietNhuCauDTTHView.TenGiangVien;
                        chitietNCDT.GiangVien_Vitri = nhucau.chiTietNhuCauDTTHView.TenViTri;
                        chitietNCDT.ThoiGian_DT = nhucau.chiTietNhuCauDTTHView.ThoiGianDT;
                        chitietNCDT.ThoiLuong_DT = nhucau.chiTietNhuCauDTTHView.ThoiLuong;
                        chitietNCDT.DiaDiemDT = nhucau.chiTietNhuCauDTTHView.DiaDiemDT;
                        chitietNCDT.GhiChu = nhucau.chiTietNhuCauDTTHView.GhiChu;
                        chitietNCDT.DonViDT = nhucau.chiTietNhuCauDTTHView.DonViDT;
                    }
                    // check nhân viên
                    var gv = db.NhanViens.Where(x => x.ID == nhucau.chiTietNhuCauDTTHView.GiangVien_ID).FirstOrDefault();
                    var loai = db.SH_PhanLoaiNCDT.Where(x => x.IDLoai == nhucau.PhanLoaiNCDT_ID).FirstOrDefault().LoaiHinhDT_ID;
                    if (gv != null)
                    {
                        chitietNCDT.DonViDT = gv?.PhongBan.TenPhongBan;
                        chitietNCDT.GiangVien_Vitri = gv?.Vitri.TenViTri;
                        chitietNCDT.GiangVien_HoTen = gv.HoTen;
                        chitietNCDT.GiangVien_ID = gv.ID;
                    }
                    db.SaveChanges();


                    // Thông tin ký duyệt
                    var kyduyet = db.SH_KyDuyetNCDT.Where(x => x.NCDT_ID == nhucau.ID_NCDT).ToList();
                    if (kyduyet.Count() != 0)
                    {
                        db.SH_KyDuyetNCDT.RemoveRange(kyduyet); // xóa thông tin cũ nếu có
                        db.SaveChanges();
                    }
                    //duyệt BPSD
                    var CapDuyet = nhucau.CapDuyetView;
                    if (CapDuyet.BPSD != null)
                    {
                        SH_KyDuyetNCDT duyet = new SH_KyDuyetNCDT()
                        {
                            NCDT_ID = nhucau.ID_NCDT,
                            NguoiDuyet_ID = CapDuyet.BPSD,
                            CapDuyet = 1,
                            TinhTrangDuyet = 0,
                        };
                        db.SH_KyDuyetNCDT.Add(duyet);
                        nhucaudt.MaTrinhKy = 1;
                        nhucaudt.TinhTrang = 2; // Đã trình ký
                        db.SaveChanges();
                    }
                    //duyệt PCHN
                    if (CapDuyet.PCHN != null)
                    {
                        SH_KyDuyetNCDT duyet = new SH_KyDuyetNCDT()
                        {
                            NCDT_ID = nhucau.ID_NCDT,
                            NguoiDuyet_ID = CapDuyet.PCHN,
                            CapDuyet = 2,
                            TinhTrangDuyet = 0,
                        };
                        db.SH_KyDuyetNCDT.Add(duyet);
                        nhucaudt.MaTrinhKy = 2;
                        nhucaudt.TinhTrang = 2; // Đã trình ký
                        db.SaveChanges();
                    }
                    //duyệt PNS
                    if (CapDuyet.PNS != null)
                    {
                        SH_KyDuyetNCDT duyet = new SH_KyDuyetNCDT()
                        {
                            NCDT_ID = nhucau.ID_NCDT,
                            NguoiDuyet_ID = CapDuyet.PNS,
                            CapDuyet = 3,
                            TinhTrangDuyet = 0,
                        };
                        db.SH_KyDuyetNCDT.Add(duyet);
                        nhucaudt.MaTrinhKy = 3;
                        nhucaudt.TinhTrang = 2; // Đã trình ký
                        db.SaveChanges();
                    }
                    //duyệt PNS
                    if (CapDuyet.BGD != null)
                    {
                        SH_KyDuyetNCDT duyet = new SH_KyDuyetNCDT()
                        {
                            NCDT_ID = nhucau.ID_NCDT,
                            NguoiDuyet_ID = CapDuyet.BGD,
                            CapDuyet = 4,
                            TinhTrangDuyet = 0,
                        };
                        db.SH_KyDuyetNCDT.Add(duyet);
                        nhucaudt.MaTrinhKy = 4;
                        nhucaudt.TinhTrang = 2; // Đã trình ký
                        db.SaveChanges();

                    }
                    // thêm thông tin trình ký
                    if (action == "Lưu")
                    {
                        nhucaudt.TinhTrang = 0; // Đang lưu
                        db.SaveChanges();
                    }


                    TempData["msgSuccess"] = "<script>alert('Cập nhật thành công ');</script>";
                    return RedirectToAction("Index", new { IDLoaiDT = nhucau.PhanLoaiNCDT_ID });
                }
            }
            catch (Exception ex)
            {
                TempData["msgSuccess"] = "<script>alert('Cập nhập thất bại " + ex.Message + " ');</script>";
            }
            return RedirectToAction("Index", new { IDLoaiDT = nhucau.PhanLoaiNCDT_ID });
        }

        public ActionResult Edit_View(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SH_NhuCauDT nhuCauDT = db.SH_NhuCauDT.Find(id);
            if (nhuCauDT == null)
            {
                return HttpNotFound();
            }
            //var IDPB = MyAuthentication.IDPhongban;
            ViewBag.IDPhanLoaiDT = new SelectList(db.SH_PhanLoaiNDDT, "ID", "TenPhanLoaiDT", nhuCauDT.PhanLoaiNCDT_ID);
            ViewBag.IDPhongBan = db.PhongBans.Where(x => x.IDPhongBan == nhuCauDT.BoPhanLNC_ID).FirstOrDefault().TenPhongBan;
            //ViewBag.IDNoiDungDT = new SelectList(db.NoiDungDTs.Where(x=>x.IDND == nhuCauDT.NoiDungDT_ID), "IDND", "NoiDung",nhuCauDT.NoiDungDT_ID);
            ViewBag.IDNoiDungDT = new SelectList(db.NoiDungDTs.Where(x => x.IDND == nhuCauDT.NoiDungDT_ID).Select(x => new
            {
                Value = x.IDND,
                Text = x.IDND + " - " + db.SH_PhanLoaiNDDT.Where(a => x.IDND == nhuCauDT.NoiDungDT_ID).FirstOrDefault().TenPhanLoaiDT + " - " + x.NoiDung // Concatenate fields here
            }),
            "Value",
            "Text", nhuCauDT.NoiDungDT_ID);
            ViewBag.IDDKNhacLai = new SelectList(db.SH_DinhKy, "MaDK", "DKNhacLai", nhuCauDT.MaDinhKy);

            ViewBag.LoaiNCDT = nhuCauDT.PhanLoaiNCDT_ID;
            ViewBag.LoaiHinh_DT = db.SH_PhanLoaiNCDT.Where(x => x.IDLoai == nhuCauDT.PhanLoaiNCDT_ID).FirstOrDefault().LoaiHinhDT_ID;
            ViewBag.Nam = nhuCauDT.Nam; // lấy từ ncđt
            ViewBag.Quy = nhuCauDT.Quy;
            ViewBag.PhuongPhapDT_ID = new SelectList(db.SH_PhuongPhapDT.Where(x => x.ID == nhuCauDT.PhuongPhapDT_ID), "ID", "TenPhuongPhapDT", nhuCauDT.PhuongPhapDT_ID);
            //var nv2 = db.NhanViens.Where(x => x.IDTinhTrangLV == 1 && x.IDPhongBan == IDPB).Select(x => new EmployeeValidation { ID = x.ID, HoTen = x.MaNV + " - " + x.HoTen + "-" + x.Vitri.TenViTri }).ToList();
            //ViewBag.Selec = new SelectList(nv2, "ID", "HoTen", nhuCauDT.SH_ChiTiet_NCDT.FirstOrDefault().GiangVien_ID);

            var ls = new List<SH_ViTri_NDDTView>();

            var data = db.VitriKNL_search();
            var vt = db.SH_ViTri_NDDT.Where(x => x.NCDT_ID == null || x.NCDT_ID == nhuCauDT.ID).ToList();
            ls = (from a in vt.Where(x => x.NoiDungDT_ID == nhuCauDT.NoiDungDT_ID && x.PhuongPhapDT_ID == nhuCauDT.PhuongPhapDT_ID)
                  join b in data on a.Vitri_ID equals b.IDVT
                  select new SH_ViTri_NDDTView()
                  {
                      ID = a.ID,
                      TenViTri = b.IDVT + "-" + b.TenViTri + "-" + b.MaPB + "-" + b.TenPX + "-" + b.TenNhom + "-" + b.TenTo,
                      Tinhtrang = a.NCDT_ID == nhuCauDT.ID ? 1 : 0
                  }).ToList();
            ViewBag.DSVitri = ls;

            // trình ký
            var trinhky = db.SH_KyDuyetNCDT.Where(x => x.NCDT_ID == nhuCauDT.ID).ToList();
            var nhanvien = db.NhanViens.Where(x => x.IDTinhTrangLV == 1).Select(x => new EmployeeValidation { ID = x.ID, HoTen = x.MaNV + " - " + x.HoTen, IDPhongBan = (int)x.IDPhongBan }).ToList();

            ViewBag.BPSD = new SelectList(nhanvien.Where(x => x.IDPhongBan == MyAuthentication.IDPhongban), "ID", "HoTen", trinhky.Where(x => x.CapDuyet == 1).FirstOrDefault()?.NguoiDuyet_ID);
            ViewBag.PCHN = new SelectList(nhanvien, "ID", "HoTen", trinhky.Where(x => x.CapDuyet == 2).FirstOrDefault()?.NguoiDuyet_ID);
            var phongns = db.PhongBans.Where(x => x.MaPB.Contains("P.NS")).First();
            var bangiamdoc = db.PhongBans.Where(x => x.MaPB.Contains("BGD")).First();
            ViewBag.PNS = new SelectList(nhanvien.Where(x => x.IDPhongBan == phongns.IDPhongBan), "ID", "HoTen", trinhky.Where(x => x.CapDuyet == 3).FirstOrDefault()?.NguoiDuyet_ID);
            ViewBag.BGD = new SelectList(nhanvien.Where(x => x.IDPhongBan == bangiamdoc.IDPhongBan), "ID", "HoTen", trinhky.Where(x => x.CapDuyet == 4).FirstOrDefault()?.NguoiDuyet_ID);

            var noiDungDTs = (from a in db.SH_NhuCauDT.Where(x => x.ID == id)
                              join b in db.SH_ChiTiet_NCDT on a.ID equals b.NhuCauDT_ID
                              join c in db.PhongBans on a.BoPhanLNC_ID equals c.IDPhongBan into uli
                              from c in uli.DefaultIfEmpty()
                              join d in db.NhanViens.Where(x => x.IDTinhTrangLV == 1) on a.NguoiTao_ID equals d.ID
                              select new NhuCauDTTHView
                              {
                                  ID_NCDT = a.ID,
                                  NoiDungDT_ID = a.NoiDungDT_ID,
                                  TenNoiDungDT = a.NoiDungDT.NoiDung,
                                  Quy = a.Quy,
                                  Nam = a.Nam,
                                  BoPhanLNC_ID = a.BoPhanLNC_ID,
                                  TenBoPhan_LNC = c.TenPhongBan,
                                  NguoiTao = d.MaNV + "-" + d.HoTen,
                                  TinhTrang = a.TinhTrang,
                                  FileDinhKem = a.FileDinhKem,
                                  TrinhKy_ID = a.MaTrinhKy,
                                  PhuongPhapDT_ID = a.PhuongPhapDT_ID,
                                  NgayTao = (DateTime)a.NgayTao,
                                  chiTietNhuCauDTTHView = new ChiTietNhuCauDTTHView
                                  {
                                      SoLuongNguoi = b.SoLuongNguoi,
                                      DonViDT = b.DonViDT,
                                      DiaDiemDT = b.DiaDiemDT,
                                      ThoiGianDT = b.ThoiGian_DT,
                                      GhiChu = b.GhiChu,
                                      ThoiLuong = b.ThoiLuong_DT,
                                      DoiTuongDT = b.DoiTuongDT,
                                      TenViTri = b.GiangVien_Vitri,
                                      TenGiangVien = b.GiangVien_HoTen,
                                      Nhom_ID = a.NoiDungDT.IDNhomNL,
                                      GiangVien_ID = b.GiangVien_ID,
                                  },
                              }).OrderBy(x => x.ID_NCDT).FirstOrDefault();

            return View(noiDungDTs);
        }

        //public ActionResult PrintView(int id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    SH_NhuCauDT nhuCauDT = db.SH_NhuCauDT.Find(id);
        //    if (nhuCauDT == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    //var IDPB = MyAuthentication.IDPhongban;
        //    ViewBag.IDPhanLoaiDT = new SelectList(db.SH_PhanLoaiNDDT, "ID", "TenPhanLoaiDT", nhuCauDT.PhanLoaiNCDT_ID);
        //    ViewBag.IDPhongBan = db.PhongBans.Where(x => x.IDPhongBan == nhuCauDT.BoPhanLNC_ID).FirstOrDefault().TenPhongBan;
        //    //ViewBag.IDNoiDungDT = new SelectList(db.NoiDungDTs.Where(x=>x.IDND == nhuCauDT.NoiDungDT_ID), "IDND", "NoiDung",nhuCauDT.NoiDungDT_ID);
        //    ViewBag.IDNoiDungDT = new SelectList(db.NoiDungDTs.Where(x => x.IDND == nhuCauDT.NoiDungDT_ID).Select(x => new
        //    {
        //        Value = x.IDND,
        //        Text = x.IDND + " - " + db.SH_PhanLoaiNDDT.Where(a => x.IDND == nhuCauDT.NoiDungDT_ID).FirstOrDefault().TenPhanLoaiDT + " - " + x.NoiDung // Concatenate fields here
        //    }),
        //    "Value",
        //    "Text", nhuCauDT.NoiDungDT_ID);
        //    ViewBag.IDDKNhacLai = new SelectList(db.SH_DinhKy, "MaDK", "DKNhacLai", nhuCauDT.MaDinhKy);

        //    ViewBag.LoaiNCDT = nhuCauDT.PhanLoaiNCDT_ID;
        //    ViewBag.LoaiHinh_DT = db.SH_PhanLoaiNCDT.Where(x => x.IDLoai == nhuCauDT.PhanLoaiNCDT_ID).FirstOrDefault().LoaiHinhDT_ID;
        //    ViewBag.Nam = db.SH_QuyDaoTao.First().AD_Nam;
        //    ViewBag.Quy = db.SH_QuyDaoTao.First().AD_Quy;
        //    ViewBag.PhuongPhapDT_ID = new SelectList(db.SH_PhuongPhapDT.Where(x => x.ID == nhuCauDT.PhuongPhapDT_ID), "ID", "TenPhuongPhapDT", nhuCauDT.PhuongPhapDT_ID);
        //    //var nv2 = db.NhanViens.Where(x => x.IDTinhTrangLV == 1 && x.IDPhongBan == IDPB).Select(x => new EmployeeValidation { ID = x.ID, HoTen = x.MaNV + " - " + x.HoTen + "-" + x.Vitri.TenViTri }).ToList();
        //    //ViewBag.Selec = new SelectList(nv2, "ID", "HoTen", nhuCauDT.SH_ChiTiet_NCDT.FirstOrDefault().GiangVien_ID);

        //    var ls = new List<SH_ViTri_NDDTView>();

        //    var data = db.VitriKNL_search();
        //    var vt = db.SH_ViTri_NDDT.Where(x => x.NCDT_ID == null || x.NCDT_ID == nhuCauDT.ID).ToList();
        //    ls = (from a in vt.Where(x => x.NoiDungDT_ID == nhuCauDT.NoiDungDT_ID && x.PhuongPhapDT_ID == nhuCauDT.PhuongPhapDT_ID)
        //          join b in data on a.Vitri_ID equals b.IDVT
        //          select new SH_ViTri_NDDTView()
        //          {
        //              ID = a.ID,
        //              TenViTri = b.IDVT + "-" + b.TenViTri + "-" + b.MaPB + "-" + b.TenPX + "-" + b.TenNhom + "-" + b.TenTo,
        //              Tinhtrang = a.NCDT_ID == nhuCauDT.ID ? 1 : 0
        //          }).ToList();
        //    ViewBag.DSVitri = ls;

        //    // trình ký
        //    var trinhky = db.SH_KyDuyetNCDT.Where(x => x.NCDT_ID == nhuCauDT.ID).ToList();
        //    var nhanvien = db.NhanViens.Where(x => x.IDTinhTrangLV == 1).Select(x => new EmployeeValidation { ID = x.ID, HoTen = x.MaNV + " - " + x.HoTen, IDPhongBan = (int)x.IDPhongBan }).ToList();

        //    ViewBag.BPSD = new SelectList(nhanvien.Where(x => x.IDPhongBan == MyAuthentication.IDPhongban), "ID", "HoTen", trinhky.Where(x => x.CapDuyet == 1).FirstOrDefault()?.NguoiDuyet_ID);
        //    ViewBag.PCHN = new SelectList(nhanvien, "ID", "HoTen", trinhky.Where(x => x.CapDuyet == 2).FirstOrDefault()?.NguoiDuyet_ID);
        //    var phongns = db.PhongBans.Where(x => x.MaPB.Contains("P.NS")).First();
        //    var bangiamdoc = db.PhongBans.Where(x => x.MaPB.Contains("BGD")).First();
        //    ViewBag.PNS = new SelectList(nhanvien.Where(x => x.IDPhongBan == phongns.IDPhongBan), "ID", "HoTen", trinhky.Where(x => x.CapDuyet == 3).FirstOrDefault()?.NguoiDuyet_ID);
        //    ViewBag.BGD = new SelectList(nhanvien.Where(x => x.IDPhongBan == bangiamdoc.IDPhongBan), "ID", "HoTen", trinhky.Where(x => x.CapDuyet == 4).FirstOrDefault()?.NguoiDuyet_ID);

        //    var noiDungDTs = (from a in db.SH_NhuCauDT.Where(x => x.ID == id)
        //                      join b in db.SH_ChiTiet_NCDT on a.ID equals b.NhuCauDT_ID
        //                      join c in db.PhongBans on a.BoPhanLNC_ID equals c.IDPhongBan into uli
        //                      from c in uli.DefaultIfEmpty()
        //                      join d in db.NhanViens.Where(x => x.IDTinhTrangLV == 1) on a.NguoiTao_ID equals d.ID
        //                      select new NhuCauDTTHView
        //                      {
        //                          ID_NCDT = a.ID,
        //                          NoiDungDT_ID = a.NoiDungDT_ID,
        //                          TenNoiDungDT = a.NoiDungDT.NoiDung,
        //                          Quy = a.Quy,
        //                          Nam = a.Nam,
        //                          BoPhanLNC_ID = a.BoPhanLNC_ID,
        //                          TenBoPhan_LNC = c.TenPhongBan,
        //                          NguoiTao = d.MaNV + "-" + d.HoTen,
        //                          TinhTrang = a.TinhTrang,
        //                          FileDinhKem = a.FileDinhKem,
        //                          TrinhKy_ID = a.MaTrinhKy,
        //                          PhuongPhapDT_ID = a.PhuongPhapDT_ID,
        //                          NgayTao = (DateTime)a.NgayTao,
        //                          chiTietNhuCauDTTHView = new ChiTietNhuCauDTTHView
        //                          {
        //                              SoLuongNguoi = b.SoLuongNguoi,
        //                              DonViDT = b.DonViDT,
        //                              DiaDiemDT = b.DiaDiemDT,
        //                              ThoiGianDT = b.ThoiGian_DT,
        //                              GhiChu = b.GhiChu,
        //                              ThoiLuong = b.ThoiLuong_DT,
        //                              DoiTuongDT = b.DoiTuongDT,
        //                              TenViTri = b.GiangVien_Vitri,
        //                              TenGiangVien = b.GiangVien_HoTen,
        //                              Nhom_ID = a.NoiDungDT.IDNhomNL,
        //                              GiangVien_ID = b.GiangVien_ID,
        //                          },
        //                      }).OrderBy(x => x.ID_NCDT).FirstOrDefault();

        //    return View(noiDungDTs);
        //}
        public ActionResult NCDT_ViewPRE(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SH_NhuCauDT nhuCauDT = db.SH_NhuCauDT.Find(id);
            if (nhuCauDT == null)
            {
                return HttpNotFound();
            }
            //var IDPB = MyAuthentication.IDPhongban;
            ViewBag.IDPhanLoaiDT = new SelectList(db.SH_PhanLoaiNDDT, "ID", "TenPhanLoaiDT", nhuCauDT.PhanLoaiNCDT_ID);
            ViewBag.IDPhongBan = db.PhongBans.Where(x => x.IDPhongBan == nhuCauDT.BoPhanLNC_ID).FirstOrDefault().TenPhongBan;
            //ViewBag.IDNoiDungDT = new SelectList(db.NoiDungDTs.Where(x=>x.IDND == nhuCauDT.NoiDungDT_ID), "IDND", "NoiDung",nhuCauDT.NoiDungDT_ID);
            ViewBag.IDNoiDungDT = new SelectList(db.NoiDungDTs.Where(x => x.IDND == nhuCauDT.NoiDungDT_ID).Select(x => new
            {
                Value = x.IDND,
                Text = x.IDND + " - " + db.SH_PhanLoaiNDDT.Where(a => x.IDND == nhuCauDT.NoiDungDT_ID).FirstOrDefault().TenPhanLoaiDT + " - " + x.NoiDung // Concatenate fields here
            }),
            "Value",
            "Text", nhuCauDT.NoiDungDT_ID);
            ViewBag.IDDKNhacLai = new SelectList(db.SH_DinhKy, "MaDK", "DKNhacLai", nhuCauDT.MaDinhKy);

            ViewBag.LoaiNCDT = nhuCauDT.PhanLoaiNCDT_ID;
            ViewBag.LoaiHinh_DT = db.SH_PhanLoaiNCDT.Where(x => x.IDLoai == nhuCauDT.PhanLoaiNCDT_ID).FirstOrDefault().LoaiHinhDT_ID;
            ViewBag.Nam = db.SH_QuyDaoTao.First().AD_Nam;
            ViewBag.Quy = db.SH_QuyDaoTao.First().AD_Quy;
            ViewBag.PhuongPhapDT_ID = new SelectList(db.SH_PhuongPhapDT.Where(x => x.ID == nhuCauDT.PhuongPhapDT_ID), "ID", "TenPhuongPhapDT", nhuCauDT.PhuongPhapDT_ID);
            //var nv2 = db.NhanViens.Where(x => x.IDTinhTrangLV == 1 && x.IDPhongBan == IDPB).Select(x => new EmployeeValidation { ID = x.ID, HoTen = x.MaNV + " - " + x.HoTen + "-" + x.Vitri.TenViTri }).ToList();
            //ViewBag.Selec = new SelectList(nv2, "ID", "HoTen", nhuCauDT.SH_ChiTiet_NCDT.FirstOrDefault().GiangVien_ID);

            var ls = new List<SH_ViTri_NDDTView>();

            var data = db.VitriKNL_search();
            var vt = db.SH_ViTri_NDDT.Where(x => x.NCDT_ID == null || x.NCDT_ID == nhuCauDT.ID).ToList();
            ls = (from a in vt.Where(x => x.NoiDungDT_ID == nhuCauDT.NoiDungDT_ID && x.PhuongPhapDT_ID == nhuCauDT.PhuongPhapDT_ID)
                  join b in data on a.Vitri_ID equals b.IDVT
                  select new SH_ViTri_NDDTView()
                  {
                      ID = a.ID,
                      TenViTri = b.IDVT + "-" + b.TenViTri + "-" + b.MaPB + "-" + b.TenPX + "-" + b.TenNhom + "-" + b.TenTo,
                      Tinhtrang = a.NCDT_ID == nhuCauDT.ID ? 1 : 0
                  }).ToList();
            ViewBag.DSVitri = ls;

            // trình ký
            var trinhky = db.SH_KyDuyetNCDT.Where(x => x.NCDT_ID == nhuCauDT.ID).ToList();
            var nhanvien = db.NhanViens.Where(x => x.IDTinhTrangLV == 1).Select(x => new EmployeeValidation { ID = x.ID, HoTen = x.MaNV + " - " + x.HoTen, IDPhongBan = (int)x.IDPhongBan }).ToList();

            ViewBag.BPSD = new SelectList(nhanvien.Where(x => x.IDPhongBan == MyAuthentication.IDPhongban), "ID", "HoTen", trinhky.Where(x => x.CapDuyet == 1).FirstOrDefault()?.NguoiDuyet_ID);
            ViewBag.PCHN = new SelectList(nhanvien, "ID", "HoTen", trinhky.Where(x => x.CapDuyet == 2).FirstOrDefault()?.NguoiDuyet_ID);
            var phongns = db.PhongBans.Where(x => x.MaPB.Contains("P.NS")).First();
            var bangiamdoc = db.PhongBans.Where(x => x.MaPB.Contains("BGD")).First();
            ViewBag.PNS = new SelectList(nhanvien.Where(x => x.IDPhongBan == phongns.IDPhongBan), "ID", "HoTen", trinhky.Where(x => x.CapDuyet == 3).FirstOrDefault()?.NguoiDuyet_ID);
            ViewBag.BGD = new SelectList(nhanvien.Where(x => x.IDPhongBan == bangiamdoc.IDPhongBan), "ID", "HoTen", trinhky.Where(x => x.CapDuyet == 4).FirstOrDefault()?.NguoiDuyet_ID);

            var noiDungDTs = (from a in db.SH_NhuCauDT.Where(x => x.ID == id)
                              join b in db.SH_ChiTiet_NCDT on a.ID equals b.NhuCauDT_ID
                              join c in db.PhongBans on a.BoPhanLNC_ID equals c.IDPhongBan into uli
                              from c in uli.DefaultIfEmpty()
                              join d in db.NhanViens.Where(x => x.IDTinhTrangLV == 1) on a.NguoiTao_ID equals d.ID
                              select new NhuCauDTTHView
                              {
                                  ID_NCDT = a.ID,
                                  NoiDungDT_ID = a.NoiDungDT_ID,
                                  TenNoiDungDT = a.NoiDungDT.NoiDung,
                                  Quy = a.Quy,
                                  Nam = a.Nam,
                                  BoPhanLNC_ID = a.BoPhanLNC_ID,
                                  TenBoPhan_LNC = c.TenPhongBan,
                                  NguoiTao = d.MaNV + "-" + d.HoTen,
                                  TinhTrang = a.TinhTrang,
                                  FileDinhKem = a.FileDinhKem,
                                  TrinhKy_ID = a.MaTrinhKy,
                                  PhuongPhapDT_ID = a.PhuongPhapDT_ID,
                                  NgayTao = (DateTime)a.NgayTao,
                                  chiTietNhuCauDTTHView = new ChiTietNhuCauDTTHView
                                  {
                                      SoLuongNguoi = b.SoLuongNguoi,
                                      DonViDT = b.DonViDT,
                                      DiaDiemDT = b.DiaDiemDT,
                                      ThoiGianDT = b.ThoiGian_DT,
                                      GhiChu = b.GhiChu,
                                      ThoiLuong = b.ThoiLuong_DT,
                                      DoiTuongDT = b.DoiTuongDT,
                                      TenViTri = b.GiangVien_Vitri,
                                      TenGiangVien = b.GiangVien_HoTen,
                                      Nhom_ID = a.NoiDungDT.IDNhomNL,
                                      GiangVien_ID = b.GiangVien_ID,
                                  },
                              }).OrderBy(x => x.ID_NCDT).FirstOrDefault();

            return View(noiDungDTs);
        }

        public ActionResult ExportToExcel()
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            ViewBag.QUYENCN = ListQuyen;
            var queryNC = db.SH_NhuCauDT.AsQueryable();
            if (!ListQuyen.Contains(CONSTKEY.VIEW_ALL) && !ListQuyen.Contains(CONSTKEY.V_BP))
            {
                queryNC = queryNC.Where(x => x.NguoiTao_ID == MyAuthentication.ID);
            }
            else if (ListQuyen.Contains(CONSTKEY.V_BP))
            {
                queryNC = queryNC.Where(x => x.BoPhanLNC_ID == MyAuthentication.IDPhongban);
            }
            var data = (from a in queryNC
                              join b in db.SH_ChiTiet_NCDT on a.ID equals b.NhuCauDT_ID
                              join c in db.PhongBans on a.BoPhanLNC_ID equals c.IDPhongBan into uli
                              from c in uli.DefaultIfEmpty()
                              join d in db.NhanViens on b.GiangVien_ID equals d.ID into ulis
                              from d in ulis.DefaultIfEmpty()
                              select new NhuCauDTTHView
                              {
                                  ID_NCDT = a.ID,
                                  NoiDungDT_ID = a.NoiDungDT_ID,
                                  TenNoiDungDT = a.NoiDungDT.NoiDung,
                                  Quy = a.Quy,
                                  Nam = a.Nam,
                                  BoPhanLNC_ID = a.BoPhanLNC_ID,
                                  TenBoPhan_LNC = c.TenPhongBan,
                                  NguoiTao = a.NhanVien.MaNV + "-" + a.NhanVien.HoTen,
                                  TinhTrang = a.TinhTrang,
                                  PhuongPhapDT_ID = a.PhuongPhapDT_ID,
                                  FileDinhKem = a.FileDinhKem,
                                  TenPPDT = db.SH_PhuongPhapDT.Where(x => x.ID == a.PhuongPhapDT_ID).FirstOrDefault().TenPhuongPhapDT,
                                  TenLoai_NCDT = a.SH_PhanLoaiNCDT.TenLoaiNCDT,
                                  chiTietNhuCauDTTHView = new ChiTietNhuCauDTTHView
                                  {
                                      SoLuongNguoi = b.SoLuongNguoi,
                                      GiangVien_ID = b.GiangVien_ID,
                                      DiaDiemDT = b.DiaDiemDT,
                                      GhiChu = b.GhiChu,
                                      DonViDT = b.DonViDT,
                                      ThoiLuong = b.ThoiLuong_DT,
                                      ThoiGianDT = b.ThoiGian_DT,
                                      TenGiangVien = b.GiangVien_ID != null ? d.HoTen : b.GiangVien_HoTen,
                                      TenViTri = b.GiangVien_Vitri,
                                      DoiTuongDT = b.DoiTuongDT,
                                      TenNhom = a.NoiDungDT.NhomNLKCCD.NoiDung,
                                      TenLVDT = a.NoiDungDT.LinhVucDT.TenLVDT,
                                      MaNV = d.MaNV,
                                  },
                                  PhanLoaiNCDT_ID = a.PhanLoaiNCDT_ID,
                                  ID_NguoiTao = (int)a.NguoiTao_ID
                              }).OrderBy(x => x.ID_NCDT).ToList();
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("NCĐT");
                //Header
                worksheet.Cell(1, 1).Value = "STT";
                worksheet.Cell(1, 2).Value = "Tên khóa đào tạo";
                worksheet.Cell(1, 3).Value = "Nhóm năng lực";
                worksheet.Cell(1, 4).Value = "Quý đào tạo";
                worksheet.Cell(1, 5).Value = "Bộ phận lập nhu cầu";
                worksheet.Cell(1, 6).Value = "Đối tượng tham gia đào tạo";
                worksheet.Cell(1, 7).Value = "Số lượng dự kiến (Người)";
                worksheet.Cell(1, 8).Value = "Mã giảng viên";
                worksheet.Cell(1, 9).Value = "Tên giảng viên";
                worksheet.Cell(1, 10).Value = "Đơn vị giảng viên";
                worksheet.Cell(1, 11).Value = "Phương pháp đào tạo";
                worksheet.Cell(1, 12).Value = "Lĩnh vực đào tạo";
                worksheet.Cell(1, 13).Value = "Thời gian bắt đầu đào tạo";
                worksheet.Cell(1, 14).Value = "Thời lượng đào tạo dự kiến";
                worksheet.Cell(1, 15).Value = "Địa điểm đào tạo dự kiến";
                worksheet.Cell(1, 16).Value = "Người tạo";
                worksheet.Cell(1, 17).Value = "Tình trạng";
                worksheet.Cell(1, 18).Value = "Nhóm nhu cầu đào tạo";
                worksheet.Cell(1, 19).Value = "Ghi chú";
                //value
                //worksheet.Cell(2, 1).Value = 1;
                //worksheet.Cell(2, 2).Value = "John Doe";
                //worksheet.Cell(2, 3).Value = 30;
                int row = 2; int stt = 1;
                foreach (var item in data)
                {
                    worksheet.Cell(row, 1).Value = stt;
                    worksheet.Cell(row, 2).Value = item.TenNoiDungDT;
                    worksheet.Cell(row, 3).Value = item.chiTietNhuCauDTTHView.TenNhom;
                    worksheet.Cell(row, 4).Value = "'" + item.Quy +" / " + item.Nam;
                    worksheet.Cell(row, 5).Value = item.TenBoPhan_LNC;
                    worksheet.Cell(row, 6).Value = item.chiTietNhuCauDTTHView.DoiTuongDT;
                    worksheet.Cell(row, 7).Value = item.chiTietNhuCauDTTHView.SoLuongNguoi;
                    worksheet.Cell(row, 8).Value = item.chiTietNhuCauDTTHView.MaNV;
                    worksheet.Cell(row, 9).Value = item.chiTietNhuCauDTTHView.TenGiangVien;
                    worksheet.Cell(row, 10).Value = item.chiTietNhuCauDTTHView.TenViTri;
                    worksheet.Cell(row, 11).Value = item.TenPPDT;
                    worksheet.Cell(row, 12).Value = item.chiTietNhuCauDTTHView.TenLVDT;
                    worksheet.Cell(row, 13).Value =  "Tháng " + item.chiTietNhuCauDTTHView.ThoiGianDT;
                    worksheet.Cell(row, 14).Value = item.chiTietNhuCauDTTHView.ThoiLuong + " Giờ";
                    worksheet.Cell(row, 15).Value = item.chiTietNhuCauDTTHView.DiaDiemDT;
                    worksheet.Cell(row, 16).Value = item.NguoiTao;
                    if(item.TinhTrang == 0)
                    {
                        worksheet.Cell(row, 17).Value = "Đang lưu";
                    }
                    else if(item.TinhTrang ==1)
                    {
                        worksheet.Cell(row, 17).Value = "Hoàn tất";
                    }
                    else if (item.TinhTrang ==2)
                    {
                        worksheet.Cell(row, 17).Value = "Đang trình ký";
                    }
                    else if(item.TinhTrang ==3)
                    {
                        worksheet.Cell(row, 17).Value = "Không phê duyệt";
                    }
                    worksheet.Cell(row, 18).Value = item.TenLoai_NCDT;
                    worksheet.Cell(row, 19).Value = item.chiTietNhuCauDTTHView.GhiChu;
                    row++; stt++;
                }

                var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0; // Reset con trỏ stream về đầu
                string filename = "DanhSachNhuCauDaoTao_"+DateTime.Now.ToString("ddMMyyHHmmss") + ".xlsx";

                return File(stream, System.Net.Mime.MediaTypeNames.Application.Octet, filename);
            }
        }

        public class PhuongPhapDTView
        {
            public int ID { get; set; }
            public string TenPhuongPhapDT { get; set; }
        }

        public JsonResult GetDSViTri(int? IDNoiDungDT,int? PhuongPhapDT_ID)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var ls = new List<SH_ViTri_NDDTView>();
            if(IDNoiDungDT != null && PhuongPhapDT_ID != null)
            {
                var data = db.VitriKNL_search().Where(x=>x.IDPB == MyAuthentication.IDPhongban);
                var vt = db.SH_ViTri_NDDT.Where(x=>x.NCDT_ID == null || x.NCDT_ID == 0 ).ToList();
                ls =  (from a in vt.Where(x => x.NoiDungDT_ID == IDNoiDungDT && x.PhuongPhapDT_ID == PhuongPhapDT_ID)
                     join b in data on a.Vitri_ID equals b.IDVT
                 select new SH_ViTri_NDDTView()
                 {
                     ID = a.ID,
                     TenViTri = b.IDVT + "-" + b.TenViTri + "-" + b.MaPB + "-" + b.TenPX + "-" + b.TenNhom + "-" + b.TenTo,
                 }).ToList();
            }
            return Json(ls, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDSNoiDungDT(int? PhuongPhapDT_ID)
        {
            //var lsIDLoaiND = new List<int?> { 6, 7, 8 }; // list ID PhanLoaiNDDT thuê ngoài được lượt bỏ
            db.Configuration.ProxyCreationEnabled = false;
            var ls = new List<NoiDungDTTHView>();
            if (PhuongPhapDT_ID != null)
            {
                ls = (from a in  db.VitriKNL_search().Where(x => x.IDPB == MyAuthentication.IDPhongban)
                           join b in db.SH_ViTri_NDDT.Where(x => x.PhuongPhapDT_ID == PhuongPhapDT_ID) on a.IDVT equals b.Vitri_ID 
                           join c in db.NoiDungDTs.Where(x=> !Constants.ID_NDDT_ThueNgoai.Contains(x.IDPhanLoaiDT)) on b.NoiDungDT_ID equals c.IDND
                           select new NoiDungDTTHView()
                           {
                               IDND = c.IDND,
                               NoiDung = c.IDND + "-" + c.MaND + "-" + c.NoiDung,
                           })
                           .GroupBy(x => x.IDND) // Group theo IDND
                           .Select(g => g.First()) // Lấy phần tử đầu tiên mỗi nhóm
                           .ToList();
            }
            return Json(ls, JsonRequestBehavior.AllowGet);
        }
        public List<SH_PhanLoaiNCDT> GetDSLoaiNCDT()
        {
            var mulQuyen = db.SH_PhanLoaiNCDT.Where(x=>x.IDLoai ==1 || x.IDLoai ==3).ToList();  // NCĐT nội bộ và NCĐT khác
            return mulQuyen;
        }

        public ActionResult TrinhKy(int? NCDTID)
        {
            var nhanvien = db.NhanViens.Where(x => x.IDTinhTrangLV == 1).Select(x => new EmployeeValidation { ID = x.ID, HoTen = x.MaNV + " - " + x.HoTen,IDPhongBan = (int)x.IDPhongBan }).ToList();

            ViewBag.BPSD = new SelectList(nhanvien.Where(x=>x.IDPhongBan == MyAuthentication.IDPhongban), "ID", "HoTen");
            ViewBag.PCHN = new SelectList(nhanvien, "ID", "HoTen");
            var phongns = db.PhongBans.Where(x=>x.MaPB.Contains("P.NS")).First();
            var bangiamdoc = db.PhongBans.Where(x => x.MaPB.Contains("BGD")).First();
            ViewBag.PNS = new SelectList(nhanvien.Where(x=>x.IDPhongBan == phongns.IDPhongBan), "ID", "HoTen");
            ViewBag.BGD = new SelectList(nhanvien.Where(x => x.IDPhongBan == bangiamdoc.IDPhongBan), "ID", "HoTen");
            ViewBag.IDNCDT = NCDTID;
            ViewBag.TrinhKy = new SelectList(db.SH_TrinhKy, "TenTrinhKy", "TenTrinhKy");
            var ncdt = db.SH_NhuCauDT.Where(x => x.ID == NCDTID).FirstOrDefault();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TrinhKy(CapDuyetView CapDuyet, int? IDNCDT)
        {

            if (IDNCDT == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var ncdt = db.SH_NhuCauDT.Where(x => x.ID == IDNCDT).FirstOrDefault();
            if (CapDuyet.BPSD == null && CapDuyet.PCHN == null && CapDuyet.PNS == null && CapDuyet.BGD == null )
            {
                TempData["msgSuccess"] = "<script>alert('Vui lòng chọn ít nhất 1 cấp ký duyệt ');</script>";
                return RedirectToAction("Index", new { IDLoaiDT = ncdt.PhanLoaiNCDT_ID });
            }
            var kyduyet = db.SH_KyDuyetNCDT.Where(x=>x.NCDT_ID == IDNCDT).ToList();
            if(kyduyet.Count() != 0)
            {
                db.SH_KyDuyetNCDT.RemoveRange(kyduyet);
                db.SaveChanges();
            }
            //duyệt BPSD
            if(CapDuyet.BPSD != null)
            {
                SH_KyDuyetNCDT duyet = new SH_KyDuyetNCDT()
                {
                    NCDT_ID = IDNCDT,
                    NguoiDuyet_ID = CapDuyet.BPSD,
                    CapDuyet = 1,
                    TinhTrangDuyet =0,
                };
                db.SH_KyDuyetNCDT.Add(duyet);
                db.SaveChanges();
            }
            //duyệt PCHN
            if (CapDuyet.PCHN != null)
            {
                SH_KyDuyetNCDT duyet = new SH_KyDuyetNCDT()
                {
                    NCDT_ID = IDNCDT,
                    NguoiDuyet_ID = CapDuyet.PCHN,
                    CapDuyet = 2,
                    TinhTrangDuyet = 0,
                };
                db.SH_KyDuyetNCDT.Add(duyet);
                db.SaveChanges();
            }
            //duyệt PNS
            if (CapDuyet.PNS != null)
            {
                SH_KyDuyetNCDT duyet = new SH_KyDuyetNCDT()
                {
                    NCDT_ID = IDNCDT,
                    NguoiDuyet_ID = CapDuyet.PNS,
                    CapDuyet = 3,
                    TinhTrangDuyet = 0,
                };
                db.SH_KyDuyetNCDT.Add(duyet);
                db.SaveChanges();
            }
            //duyệt PNS
            if (CapDuyet.BGD != null)
            {
                SH_KyDuyetNCDT duyet = new SH_KyDuyetNCDT()
                {
                    NCDT_ID = IDNCDT,
                    NguoiDuyet_ID = CapDuyet.BGD,
                    CapDuyet = 4,
                    TinhTrangDuyet = 0,
                };
                db.SH_KyDuyetNCDT.Add(duyet);
                db.SaveChanges();
            }
            ncdt.TinhTrang = 2; // Update tinhtrang đang trình ký
            db.SaveChanges();
            TempData["msgSuccess"] = "<script>alert('Trình ký thành công ');</script>";
            return RedirectToAction("Index", new { IDLoaiDT = ncdt.PhanLoaiNCDT_ID });
        }


        [HttpPost]
        public JsonResult DeleteNV(string item)
        {
            int? id = int.Parse(item);
            if (id != null)
            {
                SH_KetQuaDaoTao kq = db.SH_KetQuaDaoTao.Where(x=>x.ID ==id).FirstOrDefault();
                if (kq != null) { db.SH_KetQuaDaoTao.Remove(kq); db.SaveChanges(); return Json(new { success = true }); }
            }
            return Json(new { success = false });
        }
        [HttpPost]
        public JsonResult DeleteVT(string item)
        {
            int? id = int.Parse(item);
            if (id != null)
            {
                SH_ViTri_NDDT kq = db.SH_ViTri_NDDT.Where(x => x.ID == id).FirstOrDefault();
                if (kq != null) { db.SH_ViTri_NDDT.Remove(kq); db.SaveChanges(); return Json(new { success = true }); }
            }
            return Json(new { success = false });
        }

        public ActionResult ExportViewToPdf(int id)
        {
            // Lấy dữ liệu từ cơ sở dữ liệu (ví dụ)
            SH_NhuCauDT nhuCauDT = db.SH_NhuCauDT.Find(id);
            var noiDungDTs = (from a in db.SH_NhuCauDT.Where(x => x.ID == id)
                              join b in db.SH_ChiTiet_NCDT on a.ID equals b.NhuCauDT_ID
                              join c in db.PhongBans on a.BoPhanLNC_ID equals c.IDPhongBan into uli
                              from c in uli.DefaultIfEmpty()
                              join d in db.NhanViens.Where(x => x.IDTinhTrangLV == 1) on a.NguoiTao_ID equals d.ID
                              select new NhuCauDTTHView
                              {
                                  ID_NCDT = a.ID,
                                  NoiDungDT_ID = a.NoiDungDT_ID,
                                  TenNoiDungDT = a.NoiDungDT.NoiDung,
                                  Quy = a.Quy,
                                  Nam = a.Nam,
                                  BoPhanLNC_ID = a.BoPhanLNC_ID,
                                  TenBoPhan_LNC = c.TenPhongBan,
                                  NguoiTao = d.MaNV + "-" + d.HoTen,
                                  TinhTrang = a.TinhTrang,
                                  FileDinhKem = a.FileDinhKem,
                                  TrinhKy_ID = a.MaTrinhKy,
                                  PhuongPhapDT_ID = a.PhuongPhapDT_ID,
                                  NgayTao = (DateTime)a.NgayTao,
                                  chiTietNhuCauDTTHView = new ChiTietNhuCauDTTHView
                                  {
                                      SoLuongNguoi = b.SoLuongNguoi,
                                      DonViDT = b.DonViDT,
                                      DiaDiemDT = b.DiaDiemDT,
                                      ThoiGianDT = b.ThoiGian_DT,
                                      GhiChu = b.GhiChu,
                                      ThoiLuong = b.ThoiLuong_DT,
                                      DoiTuongDT = b.DoiTuongDT,
                                      TenViTri = b.GiangVien_Vitri,
                                      TenGiangVien = b.GiangVien_HoTen,
                                      Nhom_ID = a.NoiDungDT.IDNhomNL,
                                      GiangVien_ID = b.GiangVien_ID,
                                  },
                              }).OrderBy(x => x.ID_NCDT).FirstOrDefault();
            ViewBag.Nam = nhuCauDT.Nam; // lấy từ ncđt
            ViewBag.Quy = nhuCauDT.Quy;
            ViewBag.IDPhongBan = db.PhongBans.Where(x => x.IDPhongBan == nhuCauDT.BoPhanLNC_ID).FirstOrDefault().TenPhongBan;

            // Kết xuất View thành HTML
            string htmlContent = PdfHelper.RenderViewToString(this.ControllerContext, "ExportView", noiDungDTs);

            // Loại bỏ các thẻ head, meta, style trước khi truyền vào XMLWorker
            //htmlContent = htmlContent.Replace("<head>", "")
            //                          .Replace("</head>", "")
            //                          .Replace("<meta>", "")
            //                          .Replace("</meta>", "")
            //                          .Replace("<style>", "")
            //                          .Replace("</style>", "");
            
            using (var memoryStream = new MemoryStream())
            {
                // Tạo tài liệu PDF
                var document = new iTextSharp.text.Document(PageSize.A4.Rotate(), 30, 30, 30, 30);
                PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);

                document.Open();

                // Đăng ký font Unicode hỗ trợ tiếng Việt
                var fontProvider = new XMLWorkerFontProvider(XMLWorkerFontProvider.DONTLOOKFORFONTS);
                fontProvider.Register("C:/Windows/Fonts/times.ttf");     // Times New Roman Regular
                fontProvider.Register("C:/Windows/Fonts/timesbd.ttf");   // Times New Roman Bold
                fontProvider.Register("C:/Windows/Fonts/timesi.ttf");    // Times New Roman Italic
                fontProvider.Register("C:/Windows/Fonts/timesbi.ttf");   // Times New Roman Bold Italic

                // Cấu hình CSS Appliers sử dụng fontProvider
                CssAppliers cssAppliers = new CssAppliersImpl(fontProvider);

                // Chuyển chuỗi HTML thành MemoryStream
                byte[] byteArray = Encoding.UTF8.GetBytes(htmlContent);

                using (var sr = new MemoryStream(byteArray))
                {
                    // Parse HTML sang PDF
                    XMLWorkerHelper.GetInstance().ParseXHtml(writer, document, sr, null, Encoding.UTF8, fontProvider);
                }

                document.Close();
                string filename = $"{"NCĐT_"+noiDungDTs.TenBoPhan_LNC+"_" + DateTime.Now.ToString("yyyyMMddHHmm")}.pdf";
                // Lấy dữ liệu PDF và trả về file PDF
                byte[] pdfData = memoryStream.ToArray();
                return File(pdfData, "application/pdf", filename);
            }

        }
        public string SavePdfToFile(byte[] pdfBytes, string folderPath, string fileName)
        {
            // Kiểm tra và tạo thư mục nếu chưa tồn tại
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // Đường dẫn đầy đủ của file
            var filePath = Path.Combine(folderPath, fileName);

            // Lưu file từ mảng byte bằng FileStream
            //File.WriteAllBytes(filePath, pdfBytes);
            using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                stream.Write(pdfBytes, 0, pdfBytes.Length);
            }

            // Trả về đường dẫn tương đối hoặc URL để lưu vào database
            return $"/pdfs/{fileName}"; // Nếu wwwroot/pdfs là thư mục lưu trữ công khai
        }

        public ActionResult GetNhanViensByNCDT(int? id, int? page, string search = "")
        {
            if (!id.HasValue)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.IdNCDT = id;
            ViewBag.search = search;

            var nhanVienList = (from nv in db.NhanViens
                                join vtnd in db.SH_ViTri_NDDT on nv.IDVTKNL equals vtnd.Vitri_ID
                                join pb in db.PhongBans on nv.IDPhongBan equals pb.IDPhongBan
                                join vt in db.Vitris on nv.IDViTri equals vt.IDViTri
                                where vtnd.NCDT_ID == id && nv.IDTinhTrangLV == 1
                                select new NhanVienViewDTTH
                                {
                                    ID = nv.ID,
                                    MaNV = nv.MaNV,
                                    HoTen = nv.HoTen,
                                    ViTri = vt.TenViTri,
                                    TenPhongBan = pb.TenPhongBan
                                });

            if (!string.IsNullOrEmpty(search))
            {
                nhanVienList = nhanVienList.Where(x => x.MaNV.Contains(search) || x.HoTen.Contains(search));
            }

            int pageSize = 10;
            int pageNumber = page ?? 1;

            var pagedNhanVienList = nhanVienList.OrderBy(nv => nv.MaNV)
                                               .ToPagedList(pageNumber, pageSize);

            return View(pagedNhanVienList);
        }

        public ActionResult ExportNhanViensByNCDT(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction("Index", "Home");
            }

            var nhanVienList = (from nv in db.NhanViens
                                join vtnd in db.SH_ViTri_NDDT on nv.IDVTKNL equals vtnd.Vitri_ID
                                join pb in db.PhongBans on nv.IDPhongBan equals pb.IDPhongBan
                                join vt in db.Vitris on nv.IDViTri equals vt.IDViTri
                                where vtnd.NCDT_ID == id && nv.IDTinhTrangLV == 1
                                select new NhanVienViewDTTH
                                {
                                    ID = nv.ID,
                                    MaNV = nv.MaNV,
                                    HoTen = nv.HoTen,
                                    ViTri = vt.TenViTri,
                                    TenPhongBan = pb.TenPhongBan
                                }).ToList();

            using (var workbook = new ClosedXML.Excel.XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Danh sách nhân viên");

                worksheet.Cell(1, 1).Value = "STT";
                worksheet.Cell(1, 2).Value = "Mã NV";
                worksheet.Cell(1, 3).Value = "Họ Tên";
                worksheet.Cell(1, 4).Value = "Vị trí";
                worksheet.Cell(1, 5).Value = "Phòng ban";

                int row = 2;
                int stt = 1;
                foreach (var nv in nhanVienList)
                {
                    worksheet.Cell(row, 1).Value = stt++;
                    worksheet.Cell(row, 2).Value = nv.MaNV;
                    worksheet.Cell(row, 3).Value = nv.HoTen;
                    worksheet.Cell(row, 4).Value = nv.ViTri;
                    worksheet.Cell(row, 5).Value = nv.TenPhongBan;
                    row++;
                }

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Position = 0;
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DanhSachNhanVien.xlsx");
                }
            }
        }
    }
}