using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls.WebParts;
using ClosedXML.Excel;
using CrystalDecisions.ReportAppServer.DataDefModel;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using E_Learning.Models;
using E_Learning.ModelsDTTH;
using E_Learning.ModelsQTHD;
using ExcelDataReader;
using PagedList;
using DataSet = System.Data.DataSet;
using EntityState = System.Data.Entity.EntityState;

namespace E_Learning.Controllers.DaoTaoTH
{
    public class NoiDungDTTHController : Controller
    {
        private ELEARNINGEntities db = new ELEARNINGEntities();
        int Idquyen = MyAuthentication.IDQuyen;
        String ControllerName = "NoiDungDTTH";

        // GET: NoiDungDTTH
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
            //var students = db.NoiDungDTs.Where(x=>x.IDPhanLoaiDT != null)
            //          .Include(s => s.LinhVucDT ).Include(x=>x.SH_PhanLoaiNDDT)
            //          .ToList();
            var noiDung = (from a in db.NoiDungDTs.Where(x => x.isNQ == null 
                              && (search == null || x.NoiDung.Contains(search)) 
                              && (IDLoaiDT == null || x.IDPhanLoaiDT == IDLoaiDT))
                              join d in db.SH_PhuongPhapDT
                              on a.IDPhuongPhapDT equals d.ID into uli
                              from d in uli.DefaultIfEmpty()
                              select new NoiDungDTTHView
                              {
                                  IDND = a.IDND,
                                  IDNguonGV = a.IDNguonGV,
                                  TenNguonGV = a.SH_NguonGV.TenNguonGV,
                                  TenLVDT = a.LinhVucDT.TenLVDT,
                                  TenLoaiDT = a.SH_PhanLoaiNDDT.TenPhanLoaiDT,
                                  IDNhomNL = a.IDNhomNL,
                                  TenNhomNL = a.NhomNLKCCD.NoiDung,
                                  IDPPDT = a.IDPhuongPhapDT,
                                  TenPPDT = a.IDPhuongPhapDT != null && a.IDPhuongPhapDT != 0?d.TenPhuongPhapDT : "",
                                  BPLID = a.BPLID,
                                  FileDinhKem = a.FileDinhKem,
                                  IDCTLVDT = a.IDCTLVDT,
                                  IDHoatDongDT = a.IDHoatDongDT,
                                  IDPhanLoaiDT = a.IDPhanLoaiDT,
                                  ImageND = a.ImageND,
                                  isNQ = a.isNQ,
                                  isOrder = a.isOrder,
                                  LVDTID = a.LVDTID,
                                  MaND = a.MaND,
                                  NgayTao = a.NgayTao,
                                  NoiDung = a.NoiDung,
                                  TenHoatDongDT = a.SH_HoatDongDT.TenHoatDong,
                                  VideoND = a.VideoND,
                                  ThoiLuongDT = a.ThoiLuongDT,
                                  SLViTri = db.SH_ViTri_NDDT.Where(x=>x.NoiDungDT_ID == a.IDND).Count(),
                              }).ToList();
            ViewBag.IDPhanLoaiDT = new SelectList(db.SH_PhanLoaiNDDT, "ID", "TenPhanLoaiDT");
            ViewBag.search = search;
            if (page == null) page = 1;
            int pageSize = 50;
            int pageNumber = (page ?? 1);
            return View(noiDung.ToList().ToPagedList(pageNumber, pageSize));
        }

        // GET: NoiDungDTTH/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NoiDungDT noiDungDT = db.NoiDungDTs.Find(id);
            if (noiDungDT == null)
            {
                return HttpNotFound();
            }
            return View(noiDungDT);
        }

        // GET: NoiDungDTTH/Create
        public ActionResult Create()
        {
            ViewBag.LVDTID = new SelectList(db.LinhVucDTs, "IDLVDT", "TenLVDT");
            ViewBag.IDNguonGV = new SelectList(db.SH_NguonGV, "ID", "TenNguonGV");
            ViewBag.IDPhanLoaiDT = new SelectList(db.SH_PhanLoaiNDDT, "ID", "TenPhanLoaiDT");
            ViewBag.IDPPDT = new SelectList(db.SH_PhuongPhapDT, "ID", "TenPhuongPhapDT");
            ViewBag.IDNhomNL = new SelectList(db.NhomNLKCCDs, "ID", "NoiDung");
            ViewBag.IDHoatDongDT = new SelectList(db.SH_HoatDongDT, "ID", "TenHoatDong");
            ViewBag.IDPhuongPhapDT = new SelectList(db.SH_PhuongPhapDT, "ID", "TenPhuongPhapDT");
            return PartialView();
        }

        // POST: NoiDungDTTH/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( NoiDungDT noiDungDT)
        {
            if (ModelState.IsValid)
            {
                noiDungDT.IsDelete = false;
                db.NoiDungDTs.Add(noiDungDT);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.LVDTID = new SelectList(db.LinhVucDTs, "IDLVDT", "TenLVDT");
            return View(noiDungDT);
        }

        // GET: NoiDungDTTH/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NoiDungDT noiDungDT = db.NoiDungDTs.Find(id);
            if (noiDungDT == null)
            {
                return HttpNotFound();
            }
            //ViewBag.LVDTID = new SelectList(db.LinhVucDTs, "IDLVDT", "TenLVDT", noiDungDT.LVDTID);
            ViewBag.LVDTID = new SelectList(db.LinhVucDTs, "IDLVDT", "TenLVDT", noiDungDT.LVDTID);
            ViewBag.IDNguonGV = new SelectList(db.SH_NguonGV, "ID", "TenNguonGV", noiDungDT.IDNguonGV);
            ViewBag.IDPhanLoaiDT = new SelectList(db.SH_PhanLoaiNDDT, "ID", "TenPhanLoaiDT", noiDungDT.IDPhanLoaiDT);
            ViewBag.IDPPDT = new SelectList(db.SH_PhuongPhapDT, "ID", "TenPhuongPhapDT", noiDungDT.IDPhuongPhapDT);
            ViewBag.IDNhomNL = new SelectList(db.NhomNLKCCDs, "ID", "NoiDung", noiDungDT.IDNhomNL);
            ViewBag.IDHoatDongDT = new SelectList(db.SH_HoatDongDT, "ID", "TenHoatDong", noiDungDT.IDHoatDongDT);
            ViewBag.IDPhuongPhapDT = new SelectList(db.SH_PhuongPhapDT, "ID", "TenPhuongPhapDT", noiDungDT.IDHoatDongDT);
            return PartialView(noiDungDT);
        }

        // POST: NoiDungDTTH/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(NoiDungDT noiDungDT)
        {
            if (ModelState.IsValid)
            {
                var existingRecord = db.NoiDungDTs.Find(noiDungDT.IDND); // Tìm bản ghi hiện có dựa vào khóa chính
                if (existingRecord != null)
                {
                    // Cập nhật từng trường nếu nó khác null
                    if (noiDungDT.NoiDung != null)
                        existingRecord.NoiDung = noiDungDT.NoiDung;

                    if (noiDungDT.VideoND != null)
                        existingRecord.VideoND = noiDungDT.VideoND;

                    if (noiDungDT.ImageND != null)
                        existingRecord.ImageND = noiDungDT.ImageND;
                    if (noiDungDT.FileDinhKem != null)
                        existingRecord.FileDinhKem = noiDungDT.FileDinhKem;
                    existingRecord.LVDTID = noiDungDT.LVDTID;
                    existingRecord.IDNguonGV = noiDungDT.IDNguonGV;
                    existingRecord.IDHoatDongDT = noiDungDT.IDHoatDongDT;
                    existingRecord.IDNhomNL = noiDungDT.IDNhomNL;
                    existingRecord.IDPhuongPhapDT = noiDungDT.IDPhuongPhapDT;
                    existingRecord.IDPhanLoaiDT = noiDungDT.IDPhanLoaiDT;
                    // Thêm logic cho các trường khác nếu cần
                    db.SaveChanges(); // Lưu thay đổi
                }
                //db.Entry(noiDungDT).State = EntityState.Modified;
                //db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.LVDTID = new SelectList(db.LinhVucDTs, "IDLVDT", "TenLVDT", noiDungDT.LVDTID);
            return View(noiDungDT);
        }

        public ActionResult PhanQuyenVT_NDDT(int? IDVT)
        {
            //var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            //if (!ListQuyen.Contains(CONSTKEY.EDIT))
            //{
            //    TempData["msgError"] = "<script>alert('Bạn không có quyền thực hiện chức năng này');</script>";
            //    return RedirectToAction("", "Home");
            //}
            ViewBag.IDVT = IDVT;

            ViewBag.PhuongPhapDT = new SelectList(db.SH_PhuongPhapDT, "ID", "TenPhuongPhapDT");
            ViewBag.DSNoiDungDT = new SelectList(db.NoiDungDTs, "IDND", "NoiDung");
            ViewBag.DSNhomND = new SelectList(db.SH_PhanLoaiNDDT.Where(x=>x.LoaiHinhDT_ID ==1), "ID", "TenPhanLoaiDT");

            return PartialView();
        }

        [HttpPost]
        public ActionResult PhanQuyenVT_NDDT(SH_ViTri_NDDTView DO, FormCollection collection, int? IDVT)
        {
            try
            {
                // Danh sach QT
                foreach (var key in collection.AllKeys)
                {
                    if (key.Split('_')[0] == "noidung")
                    {

                        int? IDPPDT1 = ToNullableInt(collection["phuongphap1_" + key.Split('_')[1]]);
                        int? IDPPDT2 = ToNullableInt(collection["phuongphap2_" + key.Split('_')[1]]);
                        int? IDND = ToNullableInt(collection[key]);
                        var nddt = db.NoiDungDTs.Where(x => x.IDND == IDND).FirstOrDefault();

                        var isDuplicate = db.SH_ViTri_NDDT.Where(u => u.NoiDungDT_ID == IDND && u.Vitri_ID == IDVT).ToList();
                        if (isDuplicate.Count() < 2 && (IDPPDT1 != null || IDPPDT2 != null))
                        {
                            bool checkVT1 = db.SH_ViTri_NDDT.Any(u => u.NoiDungDT_ID == IDND && u.Vitri_ID == IDVT && u.PhuongPhapDT_ID == IDPPDT1);
                            if (IDPPDT1 != null && !checkVT1)
                            {
                                // Thêm một bản ghi mới
                                var newRecord = new SH_ViTri_NDDT { NoiDungDT_ID = IDND, Vitri_ID = IDVT, PhuongPhapDT_ID = IDPPDT1 };
                                db.SH_ViTri_NDDT.Add(newRecord);

                                // Thực hiện lưu thay đổi vào cơ sở dữ liệu
                                db.SaveChanges();
                            }
                            bool checkVT2 = db.SH_ViTri_NDDT.Any(u => u.NoiDungDT_ID == IDND && u.Vitri_ID == IDVT && u.PhuongPhapDT_ID == IDPPDT2);
                            if (IDPPDT2 != null && !checkVT2)
                            {
                                // Thêm một bản ghi mới
                                var newRecord = new SH_ViTri_NDDT { NoiDungDT_ID = IDND, Vitri_ID = IDVT, PhuongPhapDT_ID = IDPPDT2 };
                                db.SH_ViTri_NDDT.Add(newRecord);

                                // Thực hiện lưu thay đổi vào cơ sở dữ liệu
                                db.SaveChanges();
                            }
                        }


                    }
                }

                TempData["msgSuccess"] = "<script>alert('Thêm mới thành công');</script>";
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Có lỗi khi thêm mới: " + e.Message + "');</script>";
            }
            //return View();
            return RedirectToAction("DS_NoiDung", "NoiDungDTTH", new { IDVT = IDVT });
        }
        public int? ToNullableInt(string s)
        {
            int i;
            if (int.TryParse(s, out i)) return i;
            return null;
        }

        // POST: NoiDungDTTH/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            NoiDungDT noiDungDT = db.NoiDungDTs.Find(id);
            db.NoiDungDTs.Remove(noiDungDT);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        // Phân quyền vị trí
        public ActionResult DS_ViTri(int? IDND)
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, "FPosition");
            ViewBag.QUYENCN = ListQuyen;
            //if (!ListQuyen.Contains(CONSTKEY.PER))
            //{
            //    TempData["msgError"] = "<script>alert('Bạn không có quyền thực hiện chức năng này');</script>";
            //    return RedirectToAction("", "Home");
            //}

            var nd =  db.NoiDungDTs.Where(x=>x.IDND == IDND).FirstOrDefault();
            ViewBag.NoiDung = nd.NoiDung;

            List<SH_PhuongPhapDT> pp = db.SH_PhuongPhapDT.ToList();
            if (nd.IDPhuongPhapDT != null && nd.IDPhuongPhapDT != 0)
            {
                pp = pp.Where(x => x.ID == nd.IDPhuongPhapDT).ToList();
            }
            ViewBag.PhuongPhapDT_ID = new SelectList(pp, "ID", "TenPhuongPhapDT");

            List<SH_DinhKy> dk = db.SH_DinhKy.ToList();
            ViewBag.DinhKy_ID = new SelectList(dk, "MaDK", "DKNhacLai");

            return View();
        }

        public JsonResult GetDSViTri(int IDND)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            List<SH_ViTri_NDDTView> res = (from a in db.SH_ViTri_NDDT.Where(x => x.NoiDungDT_ID == IDND)
                                           join b in db.ViTriKNLs on a.Vitri_ID equals b.IDVT
                                           join e in db.KNL_PhanXuong
                                           on b.IDPX equals e.ID into ul
                                           from e in ul.DefaultIfEmpty()
                                           join f in db.KNL_Nhom
                                           on b.IDNhom equals f.IDNhom into uls
                                           from f in uls.DefaultIfEmpty()
                                           join g in db.KNL_To
                                           on b.IDTo equals g.IDTo into ulk
                                           from g in ulk.DefaultIfEmpty()
                                           join h in db.PhongBans
                                           on b.IDPB equals h.IDPhongBan into ulks
                                           from h in ulks.DefaultIfEmpty()
                                           join n in db.SH_PhuongPhapDT
                                           on a.PhuongPhapDT_ID equals n.ID into ulksn
                                           from n in ulksn.DefaultIfEmpty()
                                           select new SH_ViTri_NDDTView
                                            {
                                                ID = a.ID,
                                                NoiDungDT_ID = IDND,
                                                ViTri_ID = b.IDVT,
                                                TenViTri = b.TenViTri + "-" + e.TenPX + "-" + f.TenNhom + "-" + g.TenTo,
                                                TenPhongBan = h.TenPhongBan,
                                                PhuongPhapDT_ID = a.PhuongPhapDT_ID,
                                                TenPhuongPhapDT = n.TenPhuongPhapDT
                                           }).OrderBy(x=>x.ViTri_ID).ToList();
            var jsonResult = Json(res, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
            //return res;
        }

        public FileResult DownloadExcel()
        {
            string path = "/App_Data/BM_NDĐTTH.xlsx";
            return File(path, "application/vnd.ms-excel", "BM_NDĐTTH.xlsx");
        }
        public ActionResult ImportExcel()
        {
            return PartialView();
        }
        [HttpPost]
        public ActionResult ImportExcel(NoiDungDTTHView _DO)
        {
            string filePath = string.Empty;
            if (Request != null)
            {
                HttpPostedFileBase file = Request.Files["FileUpload"];
                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    string path = Server.MapPath("~/UploadedFiles/");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    filePath = path + Path.GetFileName(DateTime.Now.ToString("yyyyMMddHHmm") + "-" + file.FileName);

                    file.SaveAs(filePath);
                    Stream stream = file.InputStream;
                    // We return the interface, so that  
                    IExcelDataReader reader = null;
                    if (file.FileName.EndsWith(".xls"))
                    {
                        reader = ExcelReaderFactory.CreateBinaryReader(stream);
                    }
                    else if (file.FileName.EndsWith(".xlsx"))
                    {
                        reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                    }
                    else
                    {
                        TempData["msg"] = "<script>alert('Vui lòng chọn đúng định dạng file Excel');</script>";
                        return View();
                    }
                    DataSet result = reader.AsDataSet();
                    DataTable dt = result.Tables[0];
                    reader.Close();
                    int dtc = 0, dtrung = 0;
                    if (dt.Rows.Count > 0)
                    {
                        try
                        {

                            for (int i = 1; i < dt.Rows.Count; i++)
                            {

                                if (dt.Rows[i] != null)
                                {

                                    var IDGV = GetNguonGV(dt.Rows[i][1].ToString());
                                    var IDLVDT = GetLVDT(dt.Rows[i][2].ToString());
                                    var IDHoatDongDT = GetHoatDongDT(dt.Rows[i][3].ToString());
                                    var IDNhom = GetIDNhom(dt.Rows[i][4].ToString());
                                    var IDPPDT = GetPPDT(dt.Rows[i][5].ToString());
                                    var IDloai = GetLoaiDT(dt.Rows[i][6].ToString());
                                    //var IDTacGia = GetIDTacGia(dt.Rows[i][7].ToString());
                                    
                                    if (IDGV != 0 && IDNhom != 0 && IDLVDT != 0 && dt.Rows[i][0].ToString() != ""  && IDloai != 0 && IDHoatDongDT != 0)
                                    {
                                        var nddt = new NoiDungDT { 
                                            NoiDung = dt.Rows[i][0].ToString(),
                                            IDNguonGV = IDGV,
                                            IDHoatDongDT = IDHoatDongDT,
                                            LVDTID = IDLVDT,
                                            IDNhomNL=IDNhom,
                                            IDPhanLoaiDT =IDloai,
                                            IsDelete = false,
                                            IDPhuongPhapDT = IDPPDT,
                                        };
                                        db.NoiDungDTs.Add(nddt);
                                        db.SaveChanges();
                                        dtc++;
                                    }
                                    else
                                    {
                                        dtrung++;
                                    }
                                }

                            }
                            string msg = "";
                            if (dtc != 0 && dtrung != 0)
                            {
                                msg = "Import được " + dtc + " dòng dữ liệu, " + "Có " + dtrung + " dòng trùng Mã NV cập nhập nội dung";
                            }
                            else if (dtc != 0 && dtrung == 0)
                            {
                                msg = "Import được " + dtc + " dòng dữ liệu";
                            }
                            else if (dtc == 0 && dtrung != 0)
                            {
                                msg = "Có " + dtrung + " dòng trùng Mã NV cập nhập nội dung";
                            }
                            else { msg = "File import không có dữ liệu"; }


                            TempData["msgSuccess"] = "<script>alert('" + msg + "');</script>";

                        }
                        catch (Exception ex)
                        {
                            TempData["msgError"] = "<script>alert('File import nội dung không đúng. Vui lòng kiểm tra lại nội dung');</script>";
                        }
                    }
                    else
                    {
                        TempData["msgError"] = "<script>alert('File import không đúng định dạng. Vui lòng kiểm tra biểu mẫu file import');</script>";
                    }

                }
                else
                {
                    TempData["msgError"] = "<script>alert('Vui lòng nhập file Import');</script>";
                }
            }
            else
            {
                TempData["msgError"] = "<script>alert('Vui lòng nhập file Import');</script>";
            }

            return RedirectToAction("Index",new { IDLoaiDT =1});
        }

        public ActionResult DongBoNoiDungDT()
        {
            // đồng bộ QT/HD
            var noidungQT = db.QT_NoiDungQT.ToList();
            foreach (var item in noidungQT)
            {
                var checkND = db.NoiDungDTs.Where(x => x.MaND == item.MaHieu).FirstOrDefault();
                if(checkND == null)
                {
                    var nd = new NoiDungDT()
                    {
                        MaND = item.MaHieu,
                        NoiDung= item.TenQTHD,
                        NgayTao= item.NgayCapNhat,
                        IDNguonGV = 1,
                        LVDTID = item.IDLVDT,
                        IDHoatDongDT =2,
                        IDNhomNL =1,
                        IDPhanLoaiDT = 9,
                        IsDelete = false,
                    };
                    db.NoiDungDTs.Add(nd);
                }
                else
                {
                    checkND.MaND = item.MaHieu;
                    checkND.NoiDung = item.TenQTHD;
                    checkND.LVDTID = item.IDLVDT;
                    checkND.NgayTao =item.NgayCapNhat;
                    checkND.IsDelete = item.TinhTrang == 1 ? false : true;
                }
               
                // Video E-Learning
                var noidungVideo = db.NoiDungDTs.Where(x=>x.VideoND != null && x.isNQ != 1).ToList();
                foreach (var item1 in noidungVideo)
                {
                    item1.IDNguonGV = 1;
                    item1.IDPhuongPhapDT = 1;
                    item1.IDHoatDongDT = 2;
                    item1.IDPhanLoaiDT = 1;
                    if(item1.IDNhomNL == null) item1.IDNhomNL = 1;
                }
                db.SaveChanges();
            }
            //db.SaveChanges();
            TempData["msgSuccess"] = "<script>alert('Đồng bộ dữ liệu thành công ');</script>";
            return RedirectToAction("Index","NoiDungDTTH",new { IDLoaiDT =1});
        }

        public int GetNguonGV(string TenPB)
        {
            var model = db.SH_NguonGV.Where(x => x.TenNguonGV == TenPB).SingleOrDefault();
            if (model == null)
                return 0;
            return model.ID;
        }
        public int GetIDNhom(string TenNhom)
        {
            var model = db.NhomNLKCCDs.Where(x => x.NoiDung == TenNhom).SingleOrDefault();
            if (model == null)
                return 0;
            return model.ID;
        }
        public int GetLVDT(string TenLV)
        {
            var model = db.LinhVucDTs.Where(x => x.TenLVDT.Contains(TenLV)).SingleOrDefault();
            if (model == null)
                return 0;
            return model.IDLVDT;
        }
        public int GetHoatDongDT(string TenHD)
        {
            var model = db.SH_HoatDongDT.Where(x => x.TenHoatDong.Contains(TenHD)).SingleOrDefault();
            if (model == null)
                return 0;
            return model.ID;
        }
        public int GetLoaiDT(string TenNhom)
        {
            var model = db.SH_PhanLoaiNDDT.Where(x => x.TenPhanLoaiDT == TenNhom).SingleOrDefault();
            if (model == null)
                return 0;
            return model.ID;
        }
        public int? GetPPDT(string TenNhom)
        {
            var model = db.SH_PhuongPhapDT.Where(x => x.TenPhuongPhapDT == TenNhom).SingleOrDefault();
            if (model == null)
                return null;
            return model.ID;
        }
        public int GetIDTacGia(string MaNV)
        {
            var model = db.NhanViens.Where(x => x.IDTinhTrangLV == 1 && x.MaNV ==MaNV).SingleOrDefault();
            if (model == null)
                return 0;
            return model.ID;
        }


        public ActionResult ExportExcel(int IDPhanLoaiDT)
        {
            try
            {
                string fileNamemau = AppDomain.CurrentDomain.BaseDirectory + @"App_Data\DS_NoiDungDTTH.xlsx";
                string fileNamemaunew = AppDomain.CurrentDomain.BaseDirectory + @"App_Data\DS_NoiDungDTTH_Temp.xlsx";
                XLWorkbook Workbook = new XLWorkbook(fileNamemau);
                IXLWorksheet Worksheet = Workbook.Worksheet("NDDTTH");
                List<NoiDungDTTHView> DataKNL = ListNDDT(IDPhanLoaiDT);
                int row = 2;
                if (DataKNL.Count > 0)
                {
                    foreach (var data in DataKNL)
                    {
                        Worksheet.Cell(row, "A").Value = row - 1;
                        Worksheet.Cell(row, "A").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        Worksheet.Cell(row, "A").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "A").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "B").Value = data.IDND;
                        Worksheet.Cell(row, "B").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "B").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "B").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "C").Value = data.NoiDung;
                        Worksheet.Cell(row, "C").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "C").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "C").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "D").Value = data.TenNguonGV;
                        Worksheet.Cell(row, "D").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "D").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "D").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "E").Value = data.TenLVDT;
                        Worksheet.Cell(row, "E").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "E").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "E").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "F").Value = data.TenHoatDongDT;
                        Worksheet.Cell(row, "F").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "F").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "F").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;


                        Worksheet.Cell(row, "G").Value = data.TenNhomNL;
                        Worksheet.Cell(row, "G").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        Worksheet.Cell(row, "G").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "G").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        //Worksheet.Cell(row, "F").Style.Fill.BackgroundColor = XLColor.Yellow;

                        Worksheet.Cell(row, "H").Value = data.TenPPDT;
                        Worksheet.Cell(row, "H").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        Worksheet.Cell(row, "H").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "H").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        //Worksheet.Cell(row, "H").Style.DateFormat.Format = "dd/MM/yyyy";

                        Worksheet.Cell(row, "I").Value = data.TenLoaiDT;
                        Worksheet.Cell(row, "I").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        Worksheet.Cell(row, "I").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "I").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        row = row + 1;
                    }

                    Workbook.SaveAs(fileNamemaunew);
                    byte[] fileBytes = System.IO.File.ReadAllBytes(fileNamemaunew);
                    string fileName = "ThongKe_DS_NoiDungDTTH - " + DateTime.Now.Date.ToString("dd/MM/yyyy") + ".xlsx";
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
                }
                else
                {

                    //Worksheet.Cell("A1").Value = "Ngày xuất file: " + DateTime.Now.Date.ToString("dd/MM/yyyy");
                    Workbook.SaveAs(fileNamemaunew);
                    byte[] fileBytes = System.IO.File.ReadAllBytes(fileNamemaunew);
                    string fileName = "ThongKe_DS_NoiDungDTTH - " + DateTime.Now.Date.ToString("dd/MM/yyyy") + ".xlsx";
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
                }

            }
            catch (Exception ex)
            {
                TempData["msg"] = "<script>alert('" + ex + "');window.location.href = '/Everyday/'</script>";
                return View(TempData);
            }
        }
        public List<NoiDungDTTHView> ListNDDT(int IDLoaiDT)
        {
            var noiDungDTs = (from a in db.NoiDungDTs.Where(x => (IDLoaiDT == null || x.IDPhanLoaiDT == IDLoaiDT))
                              join d in db.SH_PhuongPhapDT
                              on a.IDPhuongPhapDT equals d.ID into uli
                              from d in uli.DefaultIfEmpty()
                              select new NoiDungDTTHView
                              {
                                  IDND = a.IDND,
                                  IDNguonGV = a.IDNguonGV,
                                  TenNguonGV = a.SH_NguonGV.TenNguonGV,
                                  TenLVDT = a.LinhVucDT.TenLVDT,
                                  TenLoaiDT = a.SH_PhanLoaiNDDT.TenPhanLoaiDT,
                                  IDNhomNL = a.IDNhomNL,
                                  TenNhomNL = a.NhomNLKCCD.NoiDung,
                                  IDPPDT = a.IDPhuongPhapDT,
                                  TenPPDT = a.IDPhuongPhapDT != null && a.IDPhuongPhapDT != 0 ? db.SH_PhuongPhapDT.FirstOrDefault().TenPhuongPhapDT : "",
                                  BPLID = a.BPLID,
                                  FileDinhKem = a.FileDinhKem,
                                  IDCTLVDT = a.IDCTLVDT,
                                  IDHoatDongDT = a.IDHoatDongDT,
                                  IDPhanLoaiDT = a.IDPhanLoaiDT,
                                  ImageND = a.ImageND,
                                  isNQ = a.isNQ,
                                  isOrder = a.isOrder,
                                  LVDTID = a.LVDTID,
                                  MaND = a.MaND,
                                  NgayTao = a.NgayTao,
                                  NoiDung = a.NoiDung,
                                  TenHoatDongDT = a.SH_HoatDongDT.TenHoatDong,
                                  VideoND = a.VideoND,
                                  ThoiLuongDT = a.ThoiLuongDT,
                                  SLViTri = db.SH_ViTri_NDDT.Where(x => x.NoiDungDT_ID == a.IDND).Count(),
                              }).ToList();
            return noiDungDTs;
        }

        public ActionResult Process()
        {
            return PartialView();
        }
        [HttpPost]
        public ActionResult Process(NoiDungDTTHView _DO)
        {
            string filePath = string.Empty;
            if (Request != null)
            {
                HttpPostedFileBase file = Request.Files["FileUpload"];
                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    string path = Server.MapPath("~/UploadedFiles/");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    filePath = path + Path.GetFileName(DateTime.Now.ToString("yyyyMMddHHmm") + "-" + file.FileName);

                    file.SaveAs(filePath);
                    Stream stream = file.InputStream;
                    // We return the interface, so that  
                    IExcelDataReader reader = null;
                    if (file.FileName.EndsWith(".xls"))
                    {
                        reader = ExcelReaderFactory.CreateBinaryReader(stream);
                    }
                    else if (file.FileName.EndsWith(".xlsx"))
                    {
                        reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                    }
                    else
                    {
                        TempData["msg"] = "<script>alert('Vui lòng chọn đúng định dạng file Excel');</script>";
                        return View();
                    }
                    DataSet result = reader.AsDataSet();
                    DataTable dt = result.Tables[0];
                    reader.Close();
                    int dtc = 0, dtrung = 0;
                    if (dt.Rows.Count > 0)
                    {
                        try
                        {

                            for (int i = 1; i < dt.Rows.Count; i++)
                            {
                                if (dt.Rows[i] != null)
                                {

                                    var IDND = int.Parse(dt.Rows[i][1].ToString());

                                    // Duyệt qua từng cột trong dòng
                                    for (int j = 3; j < dt.Columns.Count; j++)
                                    {
                                        var vt = dt.Rows[i][j].ToString();
                                        if (dt.Rows[i][j].ToString() == "") break;
                                        var IDVT = dt.Rows[i][j].ToString() != ""? int.Parse(dt.Rows[i][j].ToString()) :0;
                                        
                                        if (ModelState.IsValid)
                                        {
                                            var checkquyen = db.SH_ViTri_NDDT.Where(x => x.Vitri_ID == IDVT && x.NoiDungDT_ID == IDND).ToList();
                                            if(checkquyen.Count == 0)
                                            {
                                                SH_ViTri_NDDT pq = new SH_ViTri_NDDT();
                                                pq.NoiDungDT_ID = IDND;
                                                pq.Vitri_ID = IDVT;
                                                db.SH_ViTri_NDDT.Add(pq);
                                                db.SaveChanges();
                                                dtc++;
                                            }
                                           
                                        }

                                        //ViewBag.LVDTID = new SelectList(db.LinhVucDTs, "IDLVDT", "TenLVDT");
                                        //return View(noiDungDT);
                                    }

                                    //if (IDGV != 0 && IDNhom != 0 && IDLVDT != 0 && dt.Rows[i][0].ToString() != "" && IDloai != 0 && IDHoatDongDT != 0)
                                    //{
                                    //    var aa = db.NoiDungDTTH_insert(dt.Rows[i][0].ToString(), IDGV, IDHoatDongDT, IDLVDT, IDPPDT, IDNhom, IDTacGia, IDloai);
                                    //    dtc++;
                                    //}
                                    //else
                                    //{
                                    //    dtrung++;
                                    //}
                                }

                            }
                            string msg = "";
                            if (dtc != 0 && dtrung != 0)
                            {
                                msg = "Import được " + dtc + " dòng dữ liệu, " + "Có " + dtrung + " dòng trùng Mã NV cập nhập nội dung";
                            }
                            else if (dtc != 0 && dtrung == 0)
                            {
                                msg = "Import được " + dtc + " dòng dữ liệu";
                            }
                            else if (dtc == 0 && dtrung != 0)
                            {
                                msg = "Có " + dtrung + " dòng trùng Mã NV cập nhập nội dung";
                            }
                            else { msg = "File import không có dữ liệu"; }


                            TempData["msgSuccess"] = "<script>alert('" + msg + "');</script>";

                        }
                        catch (Exception ex)
                        {
                            TempData["msgError"] = "<script>alert('File import nội dung không đúng. Vui lòng kiểm tra lại nội dung');</script>";
                        }
                    }
                    else
                    {
                        TempData["msgError"] = "<script>alert('File import không đúng định dạng. Vui lòng kiểm tra biểu mẫu file import');</script>";
                    }

                }
                else
                {
                    TempData["msgError"] = "<script>alert('Vui lòng nhập file Import');</script>";
                }
            }
            else
            {
                TempData["msgError"] = "<script>alert('Vui lòng nhập file Import');</script>";
            }

            return RedirectToAction("Index", new { IDLoaiDT = 1 });
        }


        public JsonResult GetNDDT( int? IDLoai)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var nd = new List<NoiDungDT>();
            if (IDLoai != null)
            {
              nd = db.NoiDungDTs.Where(x=>x.IDPhanLoaiDT == IDLoai).ToList();
            }
            return Json(nd, JsonRequestBehavior.AllowGet);
            //RedirectToAction("Index", "ViTriKNL");

        }

        public JsonResult GetPhuongPhapDT(int? IDND)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var nd = new NoiDungDT();
            var pp = new List<SH_PhuongPhapDT>();
            if (IDND != null)
            {
                nd = db.NoiDungDTs.Where(x => x.IDND == IDND ).FirstOrDefault();
                if(nd.IDPhuongPhapDT != null && nd.IDPhuongPhapDT != 0)
                {
                    pp = db.SH_PhuongPhapDT.Where(x=>x.ID == nd.IDPhuongPhapDT).ToList();
                }
                else
                {
                    pp = db.SH_PhuongPhapDT.Where(x => x.ID != 1).ToList();
                }
            }
            return Json(pp, JsonRequestBehavior.AllowGet);
            //RedirectToAction("Index", "ViTriKNL");

        }
        //public JsonResult GetViTriND(int? DSMaVT)
        //{
        //    db.Configuration.ProxyCreationEnabled = false;
        //    var nd = new List<ViTriKNL>();
        //    if (DSMaVT != null)
        //    {
        //        nd = db.NoiDungDTs.Where(x => x.IDPhanLoaiDT == DSMaVT).ToList();
        //    }
        //    return Json(nd, JsonRequestBehavior.AllowGet);
        //    //RedirectToAction("Index", "ViTriKNL");

        //}

        public ActionResult AddViTri(int? IDND)
        {
            db.Configuration.ProxyCreationEnabled = false;

            List<PhongBan> dt = db.PhongBans.ToList();
            ViewBag.IDPB = new SelectList(dt, "IDPhongBan", "TenPhongBan");
            var nd = db.NoiDungDTs.Where(x=>x.IDND == IDND).FirstOrDefault();
            
            List<SH_PhuongPhapDT> pp = db.SH_PhuongPhapDT.ToList();
            if (nd.IDPhuongPhapDT != null && nd.IDPhuongPhapDT != 0)
            {
                pp = pp.Where(x=>x.ID == nd.IDPhuongPhapDT).ToList();
            }
            ViewBag.PhuongPhapDT_ID = new SelectList(pp, "ID", "TenPhuongPhapDT");

            //List<SH_DinhKy> dk = db.SH_DinhKy.ToList();
            //ViewBag.DinhKy_ID = new SelectList(dk, "MaDK", "DKNhacLai");

            var vt = (from a in db.VitriKNL_search()
                      select new ViTriKNLValidation
                      {
                          IDVT = a.IDVT,
                          TenViTri = a.IDVT+"-"+ a.TenViTri + "-" + a.MaPB + "-" + a.TenPX + "-" + a.TenNhom + "-" + a.TenTo,
                          IDNhom = a.IDNhom,
                          IDTo = a.IDTo,
                          MaViTri = a.MaViTri,
                          IDPX = a.IDPX,
                          IDPB = a.IDPB,
                      }).ToList();
            ViewBag.Selected = new SelectList(vt, "IDVT", "TenViTri");
            ViewBag.NDIDS = IDND;
            return PartialView();
        }

        [HttpPost]
        public ActionResult AddViTri(SH_ViTri_NDDTView _DO)
        {
            try
            {
                if (!String.IsNullOrEmpty(_DO.IDVTCopy))
                {
                    //Regex.Replace(_DO.NVDG, @"[^0-9a-zA-Z]+", " ");
                    string tx = Regex.Replace(_DO.IDVTCopy, @"[^0-9a-zA-Z]+", " ");
                    string[] NVS = tx.Split(new char[] { ' ' });

                    foreach (var item in NVS)
                    {
                        int intValue = int.TryParse(item, out int result) ? result : 0;

                        if (intValue != 0)
                        {
                            var vt = db.ViTriKNLs.Where(x => x.IDVT == intValue).ToList();
                            if (vt.Count > 0)
                            {
                               
                                bool isDuplicate = db.SH_ViTri_NDDT.Any(u => u.NoiDungDT_ID == _DO.NoiDungDT_ID && u.Vitri_ID == intValue && u.PhuongPhapDT_ID == _DO.PhuongPhapDT_ID);
                                if (!isDuplicate)
                                {
                                    // Thêm một bản ghi mới
                                    var newRecord = new SH_ViTri_NDDT { NoiDungDT_ID = _DO.NoiDungDT_ID, Vitri_ID = intValue, PhuongPhapDT_ID = _DO.PhuongPhapDT_ID };
                                    db.SH_ViTri_NDDT.Add(newRecord);

                                    // Thực hiện lưu thay đổi vào cơ sở dữ liệu
                                    db.SaveChanges();
                                }
                            }
                            
                        }
                    }
                }
                if (_DO.Selected != null)
                {
                    for (int i = 0; i < _DO.Selected.Length; i++)
                    {
                        //db.DSachDG_insert(_DO.Selected[i], manv, _DO.IDKNL);
                        if (_DO.Selected[i] != null )
                        {
                            int intValue = int.TryParse(_DO.Selected[i], out int result) ? result : 0;
                            // Thêm một bản ghi mới
                            if(intValue != 0)
                            {
                                var vt = db.ViTriKNLs.Where(x => x.IDVT == intValue).ToList();
                                if (vt.Count > 0)
                                {
                                  
                                    bool isDuplicate = db.SH_ViTri_NDDT.Any(u => u.NoiDungDT_ID == _DO.NoiDungDT_ID && u.Vitri_ID == intValue);

                                    if (!isDuplicate)
                                    {
                                        var newRecord = new SH_ViTri_NDDT { NoiDungDT_ID = _DO.NoiDungDT_ID, Vitri_ID = intValue, PhuongPhapDT_ID = _DO.PhuongPhapDT_ID };
                                        // Kiểm tra trùng lặp email
                                        db.SH_ViTri_NDDT.Add(newRecord);

                                        // Thực hiện lưu thay đổi vào cơ sở dữ liệu
                                        db.SaveChanges();
                                    }
                                }
                            }
                            
                        }
                    }
                }

                TempData["msgSuccess"] = "<script>alert('Cập nhật thành công');</script>";
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Có lỗi khi cập nhật: " + e.Message + "');</script>";
            }
            //return View();
            return RedirectToAction("DS_ViTri", "NoiDungDTTH", new { IDND = _DO.NoiDungDT_ID });
        }


        public JsonResult Luu(int ID, int? IDPP, int? DKID)
        {
            SH_ViTri_NDDT pq = db.SH_ViTri_NDDT.Find(ID);
            bool isDuplicate = db.SH_ViTri_NDDT.Any(u => u.NoiDungDT_ID == pq.NoiDungDT_ID && u.Vitri_ID == pq.Vitri_ID && u.PhuongPhapDT_ID == IDPP);
            if (!isDuplicate)
            {
                pq.PhuongPhapDT_ID = IDPP;
                db.SaveChanges();
            }

            //return RedirectToAction("Index");
            var result = new { status = "OK", message = "Request successful!" };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Xoa(int ID)
        {
            SH_ViTri_NDDT pq = db.SH_ViTri_NDDT.Find(ID);
            var mess = "";
            if (pq.NCDT_ID == null){  // check nếu chưa lập NCĐT
                db.SH_ViTri_NDDT.Remove(pq);
                db.SaveChanges();
                mess = "Xóa thành công";
            }
            else
            {
                mess = "Không thành công! Vị trí đã được lập Nhu cầu đào tạo";
            }
            var result = new { status = "OK", message = mess };
            //return RedirectToAction("Index");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckVT(string dsvt)
        {
            var ListNV = new List<ViTriKNLValidation>();
            if (!String.IsNullOrEmpty(dsvt))
            {
                //Regex.Replace(_DO.NVDG, @"[^0-9a-zA-Z]+", " ");
                string tx = Regex.Replace(dsvt, @"[^0-9a-zA-Z]+", " ");
                string[] NVS = tx.Split(new char[] { ' ' });

                foreach (var item in NVS)
                {
                    int intValue = int.TryParse(item, out int result) ? result : 0;
                    
                    if (intValue != 0)
                    {
                        var aa = db.ViTriKNLs.Where(x => x.IDVT == intValue).ToList();
                        if (aa.Count > 0) ListNV.Add(new ViTriKNLValidation { IDVT = aa.FirstOrDefault().IDVT, TenViTri = aa.FirstOrDefault().TenViTri });
                    }
                }
            }
            return Json(ListNV, JsonRequestBehavior.AllowGet);
        }


        public ActionResult DS_NoiDung(int? IDVT)
        {
            //var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            //if (!ListQuyen.Contains(CONSTKEY.PER))
            //{
            //    TempData["msgError"] = "<script>alert('Bạn không có quyền thực hiện chức năng này');</script>";
            //    return RedirectToAction("", "Home");
            //}

            var nd = db.VitriKNL_search().Where(x => x.IDVT == IDVT).FirstOrDefault();
            ViewBag.ViTriKNL = nd.IDVT + "-" + nd.TenViTri + "-" + nd.MaPB + "-" + nd.TenPX + "-" + nd.TenNhom + "-" + nd.TenTo;

            List<SH_PhuongPhapDT> pp = db.SH_PhuongPhapDT.ToList();
            //if (nd.IDPhuongPhapDT != null && nd.IDPhuongPhapDT != 0)
            //{
            //    pp = pp.Where(x => x.ID == nd.IDPhuongPhapDT).ToList();
            //}
            ViewBag.PhuongPhapDT_ID = new SelectList(pp, "ID", "TenPhuongPhapDT");

            //List<SH_DinhKy> dk = db.SH_DinhKy.ToList();
            //ViewBag.DinhKy_ID = new SelectList(dk, "MaDK", "DKNhacLai");

            return View();
        }

        public JsonResult GetDSNoiDung(int IDVT)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            List<SH_ViTri_NDDTView> res = (from a in db.SH_ViTri_NDDT.Where(x => x.Vitri_ID == IDVT)
                                           join b in db.NoiDungDTs on a.NoiDungDT_ID equals b.IDND
                                           join c in db.SH_PhanLoaiNDDT
                                           on b.IDPhanLoaiDT equals c.ID into ul from c in ul.DefaultIfEmpty()
                                           join n in db.SH_PhuongPhapDT
                                           on a.PhuongPhapDT_ID equals n.ID into ulksn from n in ulksn.DefaultIfEmpty()
                                           select new SH_ViTri_NDDTView
                                           {
                                               ID = a.ID,
                                               NoiDungDT_ID = a.NoiDungDT_ID,
                                               ViTri_ID = IDVT,
                                               //TenViTri = b.TenViTri + "-" + e.TenPX + "-" + f.TenNhom + "-" + g.TenTo,
                                               TenNoiDungDT = b.NoiDung,
                                               //TenPhongBan = h.TenPhongBan,
                                               PhuongPhapDT_ID = a.PhuongPhapDT_ID,
                                               TenPhuongPhapDT = n.TenPhuongPhapDT,
                                               IDPhanLoaiDT = b.IDPhanLoaiDT,
                                               TenLoaiDT = c.TenPhanLoaiDT
                                           }).ToList();
            var jsonResult = Json(res, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public ActionResult AddNoiDung(int? IDVT)
        {
            db.Configuration.ProxyCreationEnabled = false;

            //List<PhongBan> dt = db.PhongBans.ToList();
            //ViewBag.IDPB = new SelectList(dt, "IDPhongBan", "TenPhongBan");
            var vt = db.ViTriKNLs.Where(x => x.IDVT == IDVT).FirstOrDefault();

            List<SH_PhuongPhapDT> pp = db.SH_PhuongPhapDT.ToList();
            //if (nd.IDPhuongPhapDT != null && nd.IDPhuongPhapDT != 0)
            //{
            //    pp = pp.Where(x => x.ID == nd.IDPhuongPhapDT).ToList();
            //}
            ViewBag.PhuongPhapDT_ID = new SelectList(pp, "ID", "TenPhuongPhapDT");

            //List<SH_DinhKy> dk = db.SH_DinhKy.ToList();
            //ViewBag.DinhKy_ID = new SelectList(dk, "MaDK", "DKNhacLai");

            var nd = (from a in db.NoiDungDTs
                      select new NoiDungDTTHView
                      {
                          IDND =a.IDND,
                          NoiDung = a.IDND + "-" + a.NoiDung,
                         
                      }).ToList();
            ViewBag.Selected = new SelectList(nd, "IDND", "NoiDung");
            ViewBag.VTID = IDVT;
            return PartialView();
        }

        [HttpPost]
        public ActionResult AddNoiDung(SH_ViTri_NDDTView _DO)
        {
            try
            {
                if (!String.IsNullOrEmpty(_DO.IDNDCopy))
                {
                    //Regex.Replace(_DO.NVDG, @"[^0-9a-zA-Z]+", " ");
                    string tx = Regex.Replace(_DO.IDNDCopy, @"[^0-9a-zA-Z]+", " ");
                    string[] NVS = tx.Split(new char[] { ' ' });

                    foreach (var item in NVS)
                    {
                        int intValue = int.TryParse(item, out int result) ? result : 0;

                        if (intValue != 0)
                        {
                            var aa = db.NoiDungDTs.Where(x => x.IDND == intValue).ToList();
                            if (aa.Count > 0)
                            {
                                // Thêm một bản ghi mới
                                var newRecord = new SH_ViTri_NDDT { NoiDungDT_ID = intValue, Vitri_ID = _DO.ViTri_ID, PhuongPhapDT_ID = _DO.PhuongPhapDT_ID };
                                bool isDuplicate = db.SH_ViTri_NDDT.Any(u => u.NoiDungDT_ID == newRecord.NoiDungDT_ID && u.Vitri_ID == newRecord.Vitri_ID);

                                if (!isDuplicate)
                                {
                                    db.SH_ViTri_NDDT.Add(newRecord);
                                    // Thực hiện lưu thay đổi vào cơ sở dữ liệu
                                    db.SaveChanges();
                                }
                            }
                        }
                    }
                }
                if (_DO.Selected != null)
                {
                    for (int i = 0; i < _DO.Selected.Length; i++)
                    {
                        //db.DSachDG_insert(_DO.Selected[i], manv, _DO.IDKNL);
                        if (_DO.Selected[i] != null)
                        {
                            int intValue = int.TryParse(_DO.Selected[i], out int result) ? result : 0;
                            // Thêm một bản ghi mới
                            if (intValue != 0)
                            {
                                var aa = db.NoiDungDTs.Where(x => x.IDND == intValue).ToList();
                                if (aa.Count > 0)
                                {
                                    var newRecord = new SH_ViTri_NDDT { NoiDungDT_ID = intValue, Vitri_ID = _DO.ViTri_ID, PhuongPhapDT_ID = _DO.PhuongPhapDT_ID };
                                    // Kiểm tra trùng lặp email
                                    bool isDuplicate = db.SH_ViTri_NDDT.Any(u => u.NoiDungDT_ID == newRecord.NoiDungDT_ID && u.Vitri_ID == newRecord.Vitri_ID);
                                    if (!isDuplicate)
                                    {
                                        db.SH_ViTri_NDDT.Add(newRecord);
                                        // Thực hiện lưu thay đổi vào cơ sở dữ liệu
                                        db.SaveChanges();
                                    }
                                }
                            }

                        }
                    }
                }

                TempData["msgSuccess"] = "<script>alert('Cập nhật thành công');</script>";
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Có lỗi khi cập nhật: " + e.Message + "');</script>";
            }
            //return View();
            return RedirectToAction("DS_NoiDung", "NoiDungDTTH", new { IDVT = _DO.ViTri_ID });
        }
        public JsonResult CheckND(string dsnd)
        {
            var ListNV = new List<NoiDungDTTHView>();
            if (!String.IsNullOrEmpty(dsnd))
            {
                //Regex.Replace(_DO.NVDG, @"[^0-9a-zA-Z]+", " ");
                string tx = Regex.Replace(dsnd, @"[^0-9a-zA-Z]+", " ");
                string[] NVS = tx.Split(new char[] { ' ' });

                foreach (var item in NVS)
                {
                    int intValue = int.TryParse(item, out int result) ? result : 0;

                    if (intValue != 0)
                    {
                        var aa = db.NoiDungDTs.Where(x => x.IDND == intValue).ToList();
                        if(aa.Count>0) ListNV.Add(new NoiDungDTTHView { IDND = aa.FirstOrDefault().IDND, NoiDung = aa.FirstOrDefault().NoiDung });
                    }
                }
            }
            return Json(ListNV, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckViTri(string dsnd)
        {
            var ListNV = new List<ViTriKNLValidation>();
            if (!String.IsNullOrEmpty(dsnd))
            {
                //Regex.Replace(_DO.NVDG, @"[^0-9a-zA-Z]+", " ");
                string tx = Regex.Replace(dsnd, @"[^0-9a-zA-Z]+", " ");
                string[] NVS = tx.Split(new char[] { ' ' });

                foreach (var item in NVS)
                {
                    int intValue = int.TryParse(item, out int result) ? result : 0;

                    if (intValue != 0)
                    {
                        var aa = db.VitriKNL_search().Where(x => x.IDVT == intValue).ToList();
                        if (aa.Count > 0) ListNV.Add(new ViTriKNLValidation { IDVT = aa.FirstOrDefault().IDVT, TenViTri = aa.FirstOrDefault().TenViTri + "-" + aa.FirstOrDefault().MaPB + "-" + aa.FirstOrDefault().TenPX + "-" + aa.FirstOrDefault().TenNhom + "-" + aa.FirstOrDefault().TenTo, });
                    }
                }
            }
            return Json(ListNV, JsonRequestBehavior.AllowGet);
        }


        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
    }
}
