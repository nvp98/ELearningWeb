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
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using Font = iTextSharp.text.Font;
using System.Text;

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
                              //&& (search == null || x.NoiDung.Contains(search))
                              (IDLoaiDT == null || x.PhanLoaiNCDT_ID == IDLoaiDT))
                              join b in db.SH_ChiTiet_NCDT on a.ID equals b.NhuCauDT_ID
                              join c in db.PhongBans on a.BoPhanLNC_ID equals c.IDPhongBan into uli from c in uli.DefaultIfEmpty()
                              join d in db.NhanViens.Where(x=>x.IDTinhTrangLV ==1) on a.NguoiTao_ID equals d.ID
                              select new NhuCauDTTHView
                              {
                                 ID_NCDT = a.ID,
                                 NoiDungDT_ID = a.NoiDungDT_ID,
                                 TenNoiDungDT = a.NoiDungDT.NoiDung,
                                 Quy = a.Quy,
                                 Nam =a.Nam,
                                 BoPhanLNC_ID = a.BoPhanLNC_ID,
                                 TenBoPhan_LNC = c.TenPhongBan,
                                 NguoiTao = d.MaNV +"-" + d.HoTen,
                                 TinhTrang =a.TinhTrang,
                                 PhuongPhapDT_ID = a.PhuongPhapDT_ID,
                                 FileDinhKem = a.FileDinhKem,
                                 TenPPDT = db.SH_PhuongPhapDT.Where(x=>x.ID == a.PhuongPhapDT_ID).FirstOrDefault().TenPhuongPhapDT,
                                 chiTietNhuCauDTTHView = new ChiTietNhuCauDTTHView
                                  {
                                      SoLuongNguoi = b.SoLuongNguoi
                                  },
                                 PhanLoaiNCDT_ID = a.PhanLoaiNCDT_ID,
                                 ID_NguoiTao = (int)a.NguoiTao_ID
                              }).OrderBy(x => x.ID_NCDT).ToList();
            if (!ListQuyen.Contains(CONSTKEY.VIEW_ALL) && !ListQuyen.Contains(CONSTKEY.V_BP)) noiDungDTs = noiDungDTs.Where(x => x.ID_NguoiTao == MyAuthentication.ID).ToList();
            else if (ListQuyen.Contains(CONSTKEY.V_BP)) noiDungDTs = noiDungDTs.Where(x => x.BoPhanLNC_ID == MyAuthentication.IDPhongban).ToList();

            ViewBag.IDPhanLoaiDT = new SelectList(db.SH_PhanLoaiNCDT.Where(x=>x.IDLoai ==1 || x.IDLoai == 4), "IDLoai", "TenLoaiNCDT");

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
                if (ModelState.IsValid)
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
                            string[] NVS = tx.Split(new char[] { ' ' });
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


                    // check nhân viên
                    var gv = db.NhanViens.Where(x=>x.ID == nhucau.chiTietNhuCauDTTHView.GiangVien_ID).FirstOrDefault();

                    var child = new SH_ChiTiet_NCDT { NhuCauDT_ID = parentId, 
                        GhiChu = nhucau.chiTietNhuCauDTTHView.GhiChu,
                        SoLuongNguoi = countSl,
                        DoiTuongDT = nhucau.chiTietNhuCauDTTHView.DoiTuongDT,
                        GiangVien_ID = nhucau.chiTietNhuCauDTTHView.GiangVien_ID,
                        GiangVien_Vitri=nhucau.chiTietNhuCauDTTHView.TenViTri,
                        ThoiGian_DT =nhucau.chiTietNhuCauDTTHView.ThoiGianDT,
                        ThoiLuong_DT = nhucau.chiTietNhuCauDTTHView.ThoiLuong,
                        DiaDiemDT=nhucau.chiTietNhuCauDTTHView.DiaDiemDT,
                        DonViDT= nhucau.chiTietNhuCauDTTHView.DonViDT,
                        GiangVien_HoTen = nhucau.chiTietNhuCauDTTHView.TenGiangVien
                        //BoPhanDT_ID = gv.IDPhongBan
                    };
                   var loai =  db.SH_PhanLoaiNCDT.Where(x => x.IDLoai == nhucau.PhanLoaiNCDT_ID).FirstOrDefault().LoaiHinhDT_ID;
                    if(loai == 1)
                    {
                        child.DonViDT = gv?.PhongBan.TenPhongBan;
                        child.GiangVien_Vitri = gv?.Vitri.TenViTri;
                    }
                    db.SH_ChiTiet_NCDT.Add(child);
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
                TempData["msgSuccess"] = "<script>alert('Cập nhập thất bại " + ex.Message + " ');</script>";
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
           
                var data = db.VitriKNL_search();
                var vt = db.SH_ViTri_NDDT.Where(x=>x.NCDT_ID == null || x.NCDT_ID == nhuCauDT.ID).ToList();
                ls = (from a in vt.Where(x => x.NoiDungDT_ID == nhuCauDT.NoiDungDT_ID && x.PhuongPhapDT_ID == nhuCauDT.PhuongPhapDT_ID)
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
            SH_ChiTiet_NCDT chitiet = db.SH_ChiTiet_NCDT.Where(x=>x.NhuCauDT_ID == id).FirstOrDefault();
            db.SH_ChiTiet_NCDT.Remove(chitiet);
            SH_NhuCauDT noiDungDT = db.SH_NhuCauDT.Find(id);
            db.SH_NhuCauDT.Remove(noiDungDT);
            db.SaveChanges();
            return RedirectToAction("Index");
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
                    if (loai == 1)
                    {
                        chitietNCDT.DonViDT = gv?.PhongBan.TenPhongBan;
                        chitietNCDT.GiangVien_Vitri = gv?.Vitri.TenViTri;
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
                var vt = db.SH_ViTri_NDDT.Where(x=>x.NCDT_ID == null ).ToList();
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
            var mulQuyen = db.SH_PhanLoaiNCDT.Where(x=>x.IDLoai ==1 || x.IDLoai ==3).ToList();
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
            htmlContent = htmlContent.Replace("<head>", "")
                                      .Replace("</head>", "")
                                      .Replace("<meta>", "")
                                      .Replace("</meta>", "")
                                      .Replace("<style>", "")
                                      .Replace("</style>", "");


            // Tạo PDF từ HTML
            using (MemoryStream ms = new MemoryStream())
            {
                Document pdfDoc = new Document(PageSize.A4.Rotate());
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, ms);
                pdfDoc.Open();

                //// Khai báo font Unicode
                //BaseFont baseFont = BaseFont.CreateFont(
                //    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arialuni.ttf"),
                //    BaseFont.IDENTITY_H,
                //    BaseFont.EMBEDDED
                //);
                //Font vietnameseFont = new Font(baseFont, 12, Font.NORMAL);

                // Đảm bảo XMLWorkerHelper được sử dụng đúng cách
                using (StringReader sr = new StringReader(htmlContent))
                {
                    // Đảm bảo bạn đã cài XML Worker
                    XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                    //XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr, null, Encoding.UTF8, fontProvider);
                }

                pdfDoc.Close();

                // Trả về PDF
                return File(ms.ToArray(), "application/pdf", $"Report_{id}.pdf");
            }
        }


    }
}