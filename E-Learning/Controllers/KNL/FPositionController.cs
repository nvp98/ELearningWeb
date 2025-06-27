using E_Learning.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using ClosedXML.Excel;
using ExcelDataReader;
using System.Data;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using System.Drawing;
using System.Data.OleDb;
using Rotativa;
using Rotativa.Options;
using Font = iTextSharp.text.Font;
using static iTextSharp.text.Font;
using iTextSharp.text;
using System.Data.SqlClient;
using WebGrease.Activities;
using System.Configuration;
using DocumentFormat.OpenXml.Office.Word;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Web.UI;
using ClosedXML.Excel.Drawings;
using Syncfusion.CompoundFile.DocIO.Native;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Data.Entity;
using E_Learning.Common;
using E_Learning.ModelsDTTH;
using iTextSharp.tool.xml.html;
using System.Text;
using iTextSharp.text.pdf.qrcode;

namespace E_Learning.Controllers
{
    public class FPositionController : Controller
    {
        ELEARNINGEntities db = new ELEARNINGEntities();
        int Idquyen = MyAuthentication.IDQuyen;
        String ControllerName = "FPosition";
        // GET: FPosition
        public ActionResult Index(int? page, string search, string searchVT, int? IDPB,int? IDPX,int? IDNhom ,int? IDKhoi,int? IDTo)
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
            if (searchVT == null) searchVT = "";
            ViewBag.searchVT = searchVT;
            int idpb = MyAuthentication.IDPhongban;
            var manv = MyAuthentication.Username;
            var ThangDG = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            if (IDPB == null) IDPB = 0;

            var res = (from a in db.VitriKNL_Select(IDPB)
                       select new ViTriKNLValidation
                       {
                           IDVT = a.IDVT,
                           TenViTri = a.TenViTri,
                           IDPB = a.IDPhongBan,
                           TenPhongBan = a.TenPhongBan,
                           MaViTri = a.MaViTri,
                           IDKhoi = a.IDKhoi,
                           TenKhoi = a.TenKhoi,
                           IDPX = a.IDPX,
                           TenPX = a.TenPX,
                           IDNhom = a.IDNhom,
                           TenNhom = a.TenNhom,
                           IDTo = a.IDTo,
                           TenTo = a.TenTo,
                           FilePath = a.FilePath,
                           CountNV = a.SLNV ==null ? 0:a.SLNV,
                           CountKNL = a.SLNL,
                           CountDGTC = a.SLDGTC,
                           CountNVDDG = a.SLNVDDG,
                           TinhTrang = a.TinhTrang,
                           CountSLNDDT = db.SH_ViTri_NDDT.Where(x=>x.Vitri_ID == a.IDVT).Count()
                       }).OrderBy(x => x.IDPB).ThenBy(x=>x.IDPX).ThenBy(x => x.IDNhom).ThenBy(x => x.IDTo).ToList();
            if(!ListQuyen.Contains(CONSTKEY.LOCK)) res = res.Where(x=>x.TinhTrang != 0).ToList();
            //foreach (var k in res)
            //{
            //    //var listNVVT = KQKNLThang.Where(x => x.IDVTKNL == k.IDVT).ToList();
            //    k.CountNVDDG = KQKNLThang.Where(x => x.IDVTKNL == k.IDVT).Count();
            //}

            List<PhongBan> dt = db.PhongBans.ToList();
            if (ListQuyen.Contains(CONSTKEY.V_BP)) dt = dt.Where(x => x.IDPhongBan == idpb).ToList();
            ViewBag.IDPB = new SelectList(dt, "IDPhongBan", "TenPhongBan");

            if (IDPB == null) IDPB = 0;
            if (IDPB != 0) res = res.Where(x => x.IDPB == IDPB).ToList();
            if (IDPX != null) res = res.Where(x => x.IDPX == IDPX).ToList();
            if (IDKhoi != null) res = res.Where(x => x.IDKhoi == IDKhoi).ToList();
            if (IDNhom != null) res = res.Where(x => x.IDNhom == IDNhom).ToList();
            if (IDTo != null) res = res.Where(x => x.IDTo == IDTo).ToList();
            if (!String.IsNullOrEmpty(search)) res = res.Where(x => x.MaViTri == search).ToList();
            if (!String.IsNullOrEmpty(searchVT)) res = res.Where(x => x.TenViTri?.IndexOf(searchVT, StringComparison.OrdinalIgnoreCase) >= 0).ToList();

            if (ListQuyen.Contains(CONSTKEY.V_BP)) res = res.Where(x => x.IDPB == idpb).ToList();
            //if(Idquyen != 1 && IdquyenKNL ==0 ) res = res.Where(x => x.IDPB == 0).ToList();

            ViewBag.SumNV = res.Select(x => x.CountNV).Sum();

            ViewBag.SumNVDDG = res.Select(x => x.CountNVDDG).Sum();

            List<KNL_Khoi> khoi = db.KNL_Khoi.ToList();
            ViewBag.IDKhoi = new SelectList(khoi, "ID", "TenKhoi");

            List<KNL_PhanXuong> px = db.KNL_PhanXuong.ToList();
            ViewBag.IDPX = new SelectList(px, "ID", "TenPX");
            if (page == null) page = 1;
            int pageSize = 50;
            int pageNumber = (page ?? 1);
            return View(res.ToList().ToPagedList(pageNumber, pageSize));
        }
        //public int GetListNVDDG(int? IDVT, DateTime? ThangDG)
        //{
        //    //var model = db.NhanViens.Where(x=>x.IDVTKNL ==IDVT && x.IDTinhTrangLV ==1).ToList();
        //    int count = (from aa in db.NhanViens.Where(x => x.IDVTKNL == a.IDVT && x.IDTinhTrangLV == 1) join ab in db.KNL_KQ on aa.ID equals ab.IDNV into ls from ab in ls.Take(1) select aa).ToList().Count;
        //    //if(model.Count > 0)
        //    //{
        //    //    count = (from aa in db.NhanViens.Where(x => x.IDVTKNL == a.IDVT && x.IDTinhTrangLV == 1) join ab in db.KNL_KQ on aa.ID equals ab.IDNV into ls from ab in ls.Take(1) select aa).ToList().Count;
        //    //    //foreach (var x in model)
        //    //    //{
        //    //    //  coun  (from aa in db.NhanViens.Where(x => x.IDVTKNL == a.IDVT && x.IDTinhTrangLV == 1) join ab in db.KNL_KQ on aa.ID equals ab.IDNV into ls from ab in ls.Take(1) select aa).ToList().Count
        //    //    //}
        //    //}
        //    if (model == null)
        //        return 0;
        //    return model.IDVT;
        //}
        public int GetIDVT(string TenVT)
        {
            var model = db.ViTriKNLs.Where(x => x.TenViTri == TenVT).SingleOrDefault();
            if (model == null)
                return 0;
            return model.IDVT;
        }
        public int GetIDVTKNL(string TenVT,int? idpb)
        {
            var model = db.ViTriKNLs.Where(x => x.TenViTri == TenVT &&x.IDPB ==idpb).SingleOrDefault();
            if (model == null)
                return 0;
            return model.IDVT;
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

            List<PhongBan> dt = db.PhongBans.ToList();
            ViewBag.IDPB = new SelectList(dt, "IDPhongBan", "TenPhongBan");

            return PartialView();
        }

        [HttpPost]
        public ActionResult Create(ViTriKNLValidation _DO, FormCollection collection)
        {
            try
            {
               if(_DO.TenViTri !=null &&_DO.IDPB != null && GetIDVT(_DO.TenViTri.Trim())==0)
                {
                    //var aa = db.VitriKNL_insert(_DO.TenViTri, _DO.MaViTri, _DO.IDPB);
                }
                var ListVT = new List<ViTriKNLValidation>();
                foreach (var key in collection.AllKeys)
                {
                    if (key.Split('_')[0] == "TenVTKNL")
                    {
                        ListVT.Add(new ViTriKNLValidation() { TenViTri = collection[key], MaViTri = collection["MaViTri_" + key.Split('_')[1]] });
                    }
                }
               foreach (var item in ListVT)
                {
                    int idvt = GetIDVTKNL(item.TenViTri, _DO.IDPB);
                    if (idvt == 0)
                    {
                        //var k = db.VitriKNL_insert(item.TenViTri, _DO.MaViTri, _DO.IDPB);
                    }
                    else
                    {
                        //var k = db.VitriKNL_update(idvt,item.TenViTri, _DO.MaViTri, _DO.IDPB);
                    }
                }
                //for (int i=2;i<collection.Count - 1; i++)
                //{
                //    if (collection["TenViTri_"+i] != null)
                //    {
                //        var a = collection["TenViTri_" + i].Trim().ToString();
                //        if(GetIDVT(a) == 0)
                //        {
                //            var k = db.VitriKNL_insert(a,_DO.MaViTri, _DO.IDPB);
                //        }
                        
                //    }
                //}
                
                //db.VitriKNL_insert(_DO.TenViTri,_DO.IDPB);
                TempData["msgSuccess"] = "<script>alert('Thêm mới thành công');</script>";
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Có lỗi khi thêm mới: " + e.Message + "');</script>";
            }
            //return View();
            return RedirectToAction("Index", "FPosition");
        }

        public ActionResult SyncVT()
        {
            int idpb = MyAuthentication.IDPhongban;
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            if (!ListQuyen.Contains(CONSTKEY.ED_KNL))
            {
                TempData["msgError"] = "<script>alert('Bạn không có quyền thực hiện chức năng này');</script>";
                return RedirectToAction("", "Home");
            }
            db.Configuration.ProxyCreationEnabled = false;

            List<PhongBan> dt = db.PhongBans.ToList();
            ViewBag.IDPB = new SelectList(dt, "IDPhongBan", "TenPhongBan");

            return PartialView();
        }
        [HttpPost]
        public ActionResult SyncVT(ViTriKNLValidation _DO)
        {
            try
            {
                int dtc = 0;
                string msg = "";
                if (_DO.IDPB != null && _DO.IDNhom != null)
                {
                    var a = db.KNL_Nhom.Where(x => x.IDPhongBan == _DO.IDPB && x.IDNhom == _DO.IDNhom).FirstOrDefault();
                    var lsVT = db.ViTriKNLs.Where(x => x.IDPB == _DO.IDPB && x.IDNhom == _DO.IDNhom).ToList();
                    foreach(var item in lsVT)
                    {
                        db.VitriKNL_update_VT(item.IDVT, a.IDKhoi, a.IDPhanXuong);
                        dtc++;
                    }
                }
                else if (_DO.IDPB != null && _DO.IDTo != null)
                {
                    var a = db.KNL_To.Where(x => x.IDPhongBan == _DO.IDPB && x.IDTo == _DO.IDTo).FirstOrDefault();
                    var lsVT = db.ViTriKNLs.Where(x => x.IDPB == _DO.IDPB && x.IDTo == _DO.IDTo).ToList();
                    foreach (var item in lsVT)
                    {
                        db.VitriKNL_update_VT(item.IDVT, a.IDKhoi, a.IDPhanXuong);
                        dtc++;
                    }
                }
                else if(_DO.IDPB != null && _DO.IDTo == null && _DO.IDNhom == null)
                {
                    var lsVT = db.ViTriKNLs.Where(x => x.IDPB == _DO.IDPB && (x.IDTo != null || x.IDNhom != null)).ToList();
                    foreach(var item in lsVT)
                    {
                        if(item.IDNhom != null)
                        {
                            var a = db.KNL_Nhom.Where(x => x.IDPhongBan == _DO.IDPB && x.IDNhom == item.IDNhom).FirstOrDefault();
                            if(a != null)
                            {
                                db.VitriKNL_update_VT(item.IDVT, a.IDKhoi, a.IDPhanXuong);
                                dtc++;
                            }
                        }
                        else if (item.IDTo != null)
                        {
                            var a = db.KNL_To.Where(x => x.IDPhongBan == _DO.IDPB && x.IDTo == item.IDTo).FirstOrDefault();
                            if (a != null)
                            {
                                db.VitriKNL_update_VT(item.IDVT, a.IDKhoi, a.IDPhanXuong);
                                dtc++;
                            }
                        }
                    }
                }

                if (dtc != 0)
                {
                    msg = "Cập nhật thông tin được " + dtc + " Vị trí";
                }
                TempData["msgSuccess"] = "<script>alert('" + msg + "');</script>";
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Có lỗi khi thêm mới: " + e.Message + "');</script>";
            }
            //return View();
            return RedirectToAction("Index", "FPosition", new { IDPB = _DO.IDPB,IDNhom =_DO.IDNhom,IDTo =_DO.IDTo });
        }


        public ActionResult Edit(int id)
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            if (!ListQuyen.Contains(CONSTKEY.EDIT))
            {
                TempData["msgError"] = "<script>alert('Bạn không có quyền thực hiện chức năng này');</script>";
                return RedirectToAction("", "Home");
            }
            var res = (from a in db.ViTriKNLs.Where(x=>x.IDVT==id)
                       join d in db.PhongBans
                      on a.IDPB equals d.IDPhongBan
                       join e in db.KNL_PhanXuong
                        on a.IDPX equals e.ID into ul
                       from e in ul.DefaultIfEmpty()
                       join f in db.KNL_Nhom
                       on a.IDNhom equals f.IDNhom into uls
                       from f in uls.DefaultIfEmpty()
                       join g in db.KNL_To
                       on a.IDTo equals g.IDTo into ulk
                       from g in ulk.DefaultIfEmpty()
                       join h in db.KNL_Khoi
                       on a.IDKhoi equals h.ID into ulg
                       from h in ulg.DefaultIfEmpty()
                       select new ViTriKNLValidation
                       {
                           IDVT = a.IDVT,
                           TenViTri = a.TenViTri,
                           IDPB = a.IDPB,
                           TenPhongBan = d.TenPhongBan,
                           MaViTri = a.MaViTri,
                           IDKhoi = a.IDKhoi,
                           TenKhoi = h.TenKhoi,
                           IDPX = a.IDPX,
                           TenPX = e.TenPX,
                           IDNhom = a.IDNhom,
                           TenNhom = f.TenNhom,
                           IDTo = a.IDTo,
                           TenTo = g.TenTo,
                           FilePath = a.FilePath
                       }).ToList();
            ViTriKNLValidation DO = new ViTriKNLValidation();

            if (res.Count > 0)
            {
                foreach (var c in res)
                {
                    DO.IDVT = c.IDVT;
                    DO.IDPB = c.IDPB;
                    DO.TenViTri = c.TenViTri;
                    DO.TenPhongBan = c.TenPhongBan;
                    DO.MaViTri = c.MaViTri;
                    DO.IDKhoi = c.IDKhoi;
                    DO.TenKhoi = c.TenKhoi;
                    DO.IDPX = c.IDPX;
                    DO.TenPX = c.TenPX;
                    DO.IDNhom = c.IDNhom;
                    DO.TenNhom = c.TenNhom;
                    DO.TenTo = c.TenTo;
                    DO.FilePath = c.FilePath;
                    DO.IDTo = c.IDTo;
                    DO.IDNhom = c.IDNhom;
                }

                db.Configuration.ProxyCreationEnabled = false;
                List<PhongBan> dt = db.PhongBans.ToList();
                ViewBag.IDPB = new SelectList(dt, "IDPhongBan", "TenPhongBan",DO.IDPB);

                List<KNL_PhanXuong> px = db.KNL_PhanXuong.Where(x => x.IDPhongBan == DO.IDPB).ToList();
                ViewBag.IDPX = new SelectList(px, "ID", "TenPX", DO.IDPX);
                List<KNL_Nhom> nhom = db.KNL_Nhom.Where(x => x.IDPhongBan == DO.IDPB).ToList();
                ViewBag.IDNhom = new SelectList(nhom, "IDNhom", "TenNhom", DO.IDNhom);
                List<KNL_Khoi> khoi = db.KNL_Khoi.ToList();
                ViewBag.IDKhoi = new SelectList(khoi, "ID", "TenKhoi", DO.IDKhoi);
                List<KNL_To> to = db.KNL_To.Where(x => x.IDPhongBan == DO.IDPB).ToList();
                ViewBag.IDTo = new SelectList(to, "IDTo", "TenTo", DO.IDTo);

            }
            else
            {
                return HttpNotFound();
            }
            return PartialView(DO);
        }
        [HttpPost]
        public ActionResult Edit(ViTriKNLValidation _DO,int? IDPB)
        {
            try
            {

                db.VitriKNL_update(_DO.IDVT, _DO.TenViTri, _DO.MaViTri, _DO.IDPB, _DO.IDKhoi, _DO.IDPX, _DO.IDNhom, _DO.IDTo, _DO.FilePath);

                TempData["msgSuccess"] = "<script>alert('Cập nhập thành công');</script>";
            }
            catch (Exception e)
            {

                TempData["msgSuccess"] = "<script>alert('Cập nhập thất bại " + e.Message + " ');</script>";
            }

            return RedirectToAction("Index", "FPosition", new { IDPB = IDPB, IDKhoi = _DO.IDKhoi, IDPX = _DO.IDPX, IDTo = _DO.IDTo, IDNhom = _DO.IDNhom });
        }
        public List<ViTriKNLValidation> getListSetAuto(ViTriKNL vt)
        {
            var vtSelectDefault = new List<ViTriKNLValidation>();

            if (checkMVT2(vt.MaViTri) == "PT" && vt.IDNhom != null)
            {
                var lsNhom = db.ViTriKNLs.Where(x => x.IDNhom == vt.IDNhom).ToList();
                foreach (var item in lsNhom)
                {
                    if (checkMVT3(item.MaViTri) != "TPT" && checkMVT2(item.MaViTri) != "PT")
                    {
                        vtSelectDefault.Add(new ViTriKNLValidation { IDVT = item.IDVT, TenViTri = item.TenViTri });
                    }
                }
            }
            else if (checkMVT2(vt.MaViTri) == "TT" && vt.IDTo != null)
            {
                var lsTo = db.ViTriKNLs.Where(x => x.IDTo == vt.IDTo).ToList();
                foreach (var item in lsTo)
                {
                    if (checkMVT2(item.MaViTri) != "TT")
                    {
                        vtSelectDefault.Add(new ViTriKNLValidation { IDVT = item.IDVT, TenViTri = item.TenViTri });
                    }
                }
            }
            return vtSelectDefault;
        }

        public int? CountListSetAuto(ViTriKNL vt)
        {
            var vtSelectDefault = new List<ViTriKNLValidation>();

            if (checkMVT2(vt.MaViTri) == "PT" && vt.IDNhom != null)
            {
                var lsNhom = db.ViTriKNLs.Where(x => x.IDNhom == vt.IDNhom).ToList();
                foreach (var item in lsNhom)
                {
                    if (checkMVT3(item.MaViTri) != "TPT" && checkMVT2(item.MaViTri) != "PT")
                    {
                        vtSelectDefault.Add(new ViTriKNLValidation { IDVT = item.IDVT, TenViTri = item.TenViTri });
                    }
                }
            }
            else if (checkMVT2(vt.MaViTri) == "TT" && vt.IDTo != null)
            {
                var lsTo = db.ViTriKNLs.Where(x => x.IDTo == vt.IDTo).ToList();
                foreach (var item in lsTo)
                {
                    if (checkMVT2(item.MaViTri) != "TT")
                    {
                        vtSelectDefault.Add(new ViTriKNLValidation { IDVT = item.IDVT, TenViTri = item.TenViTri });
                    }
                }
            }
            return vtSelectDefault.Count();
        }

        public ActionResult PerCheck(int id,string controll)
        {

            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            if (!ListQuyen.Contains(CONSTKEY.ED_KNL))
            {
                TempData["msgError"] = "<script>alert('Bạn không có quyền thực hiện chức năng này');</script>";
                return RedirectToAction("", "Home");
            }
            ViewBag.QUYENCN = ListQuyen;

            var res = (from a in db.ViTriKNLs.Where(x => x.IDVT == id)
                       join d in db.PhongBans
                      on a.IDPB equals d.IDPhongBan
                       join e in db.KNL_PhanXuong
                        on a.IDPX equals e.ID into ul
                       from e in ul.DefaultIfEmpty()
                       join f in db.KNL_Nhom
                       on a.IDNhom equals f.IDNhom into uls
                       from f in uls.DefaultIfEmpty()
                       join g in db.KNL_To
                       on a.IDTo equals g.IDTo into ulk
                       from g in ulk.DefaultIfEmpty()
                       join h in db.KNL_Khoi
                       on a.IDKhoi equals h.ID into ulg
                       from h in ulg.DefaultIfEmpty()
                       select new ViTriKNLValidation
                       {
                           IDVT = a.IDVT,
                           TenViTri = a.TenViTri,
                           IDPB = a.IDPB,
                           TenPhongBan = d.TenPhongBan,
                           MaViTri = controll,
                           IDKhoi = a.IDKhoi,
                           TenKhoi = h.TenKhoi,
                           IDPX = a.IDPX,
                           TenPX = e.TenPX,
                           IDNhom = a.IDNhom,
                           TenNhom = f.TenNhom,
                           IDTo = a.IDTo,
                           TenTo = g.TenTo,
                           FilePath = a.FilePath
                       }).ToList();
            ViTriKNLValidation DO = new ViTriKNLValidation();

            if (res.Count > 0)
            {
                foreach (var c in res)
                {
                    DO.IDVT = c.IDVT;
                    DO.IDPB = c.IDPB;
                    DO.TenViTri = c.TenViTri;
                    DO.TenPhongBan = c.TenPhongBan;
                    DO.MaViTri = c.MaViTri;
                    DO.IDKhoi = c.IDKhoi;
                    DO.TenKhoi = c.TenKhoi;
                    DO.IDPX = c.IDPX;
                    DO.TenPX = c.TenPX;
                    DO.IDNhom = c.IDNhom;
                    DO.TenNhom = c.TenNhom;
                    DO.TenTo = c.TenTo;
                    DO.FilePath = c.FilePath;
                    DO.IDTo = c.IDTo;
                    DO.IDNhom = c.IDNhom;
                }

                db.Configuration.ProxyCreationEnabled = false;
                List<PhongBan> dt = db.PhongBans.Where(x => x.IDPhongBan == DO.IDPB).ToList();
                ViewBag.IDPB = new SelectList(dt, "IDPhongBan", "TenPhongBan", DO.IDPB);
                ViewBag.IDPB2 = new SelectList(dt, "IDPhongBan", "TenPhongBan");

                List<KNL_PhanXuong> px = db.KNL_PhanXuong.Where(x => x.IDPhongBan == DO.IDPB).ToList();
                ViewBag.IDPX1 = new SelectList(px, "ID", "TenPX");
                ViewBag.IDPX2 = new SelectList(px, "ID", "TenPX");

                List<KNL_Nhom> nhom = db.KNL_Nhom.Where(x => x.IDPhongBan == DO.IDPB).ToList();
                ViewBag.IDNhom1 = new SelectList(nhom, "IDNhom", "TenNhom");
                ViewBag.IDNhom2 = new SelectList(nhom, "IDNhom", "TenNhom");

                List<KNL_To> to = db.KNL_To.Where(x => x.IDPhongBan == DO.IDPB).ToList();
                ViewBag.IDTo1 = new SelectList(to, "IDTo", "TenTo");
                ViewBag.IDTo2 = new SelectList(to, "IDTo", "TenTo");

                var vt = (from a in db.ViTriKNLs.Where(x => x.IDPB == DO.IDPB)
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

                ViewBag.Selected = new SelectList(vt, "IDVT", "TenViTri");
                ViewBag.Selec = new SelectList(vt, "IDVT", "TenViTri");

                var kvt = vt.Where(x => x.IDVT == id).ToList();
                ViewBag.IDKNL = new SelectList(kvt, "IDVT", "TenViTri", id);
                //ViewBag.Selected = new SelectList(vt, "IDVT", "TenViTri");

                //var lsVT = db.KNL_DG.Where(x => x.IDVTDG == id).ToList();

                var vtSelect = vt.Where(x => x.IDVTParent == id);
                //foreach(var item in lsVT)
                //{
                //    var vta = vt.Where(x=>x.IDVT ==item.IDVTDDG).FirstOrDefault();
                //    vtSelect.Add(new ViTriKNLValidation {IDVT = item.ID, TenViTri = vta.TenViTri + "-" + vta.TenPX + "-" + vta.TenNhom + "-" + vta.TenTo, });
                //}
                //ViewBag.Selec = new SelectList(vtSelect, "IDVT", "TenViTri");
                ViewBag.SelecIDPB = DO.IDPB;

                List<KNLDGiaTCValidation> lsVTTT = (from a in db.KNL_DGiaTC.Where(x => x.IDVT == id && x.IDVTDGTT != null)
                                                    join b in db.ViTriKNLs on a.IDVTDGTT equals b.IDVT
                                                    join e in db.KNL_PhanXuong
                                                    on b.IDPX equals e.ID into ul
                                                    from e in ul.DefaultIfEmpty()
                                                    join f in db.KNL_Nhom
                                                    on b.IDNhom equals f.IDNhom into uls
                                                    from f in uls.DefaultIfEmpty()
                                                    join g in db.KNL_To
                                                    on b.IDTo equals g.IDTo into ulk
                                                    from g in ulk.DefaultIfEmpty()
                                                    select new KNLDGiaTCValidation
                                                    {
                                                        ID = a.ID,
                                                        IDVT = (int)a.IDVT,
                                                        TenViTri = b.TenViTri + "-" + f.TenNhom + "-" + g.TenTo + "-" + e.TenPX,
                                                        MaViTri = b.MaViTri,
                                                        IDPB = b.IDPB,
                                                        IDVTDGTC = a.IDVTDGTC,
                                                        IDVTDGTT = a.IDVTDGTT
                                                    }).ToList();
                List<KNLDGiaTCValidation> lsVTTC = (from a in db.KNL_DGiaTC.Where(x => x.IDVT == id && x.IDVTDGTC != null)
                                                    join b in db.ViTriKNLs on a.IDVTDGTC equals b.IDVT
                                                    join e in db.KNL_PhanXuong
                                                    on b.IDPX equals e.ID into ul
                                                    from e in ul.DefaultIfEmpty()
                                                    join f in db.KNL_Nhom
                                                    on b.IDNhom equals f.IDNhom into uls
                                                    from f in uls.DefaultIfEmpty()
                                                    join g in db.KNL_To
                                                    on b.IDTo equals g.IDTo into ulk
                                                    from g in ulk.DefaultIfEmpty()
                                                    select new KNLDGiaTCValidation
                                                    {
                                                        ID = a.ID,
                                                        IDVT = (int)a.IDVT,
                                                        TenViTri = b.TenViTri + "-" + f.TenNhom + "-" + g.TenTo + "-" + e.TenPX,
                                                        MaViTri = b.MaViTri,
                                                        IDPB = b.IDPB,
                                                        IDVTDGTC = a.IDVTDGTC,
                                                        IDVTDGTT = a.IDVTDGTT
                                                    }).ToList();

                ViewBag.SelecVTTT = new SelectList(lsVTTT, "ID", "TenViTri");
                ViewBag.SelecVTTC = new SelectList(lsVTTC, "ID", "TenViTri");

                var a1 = db.ViTriKNLs.Where(x => x.IDVT == id).FirstOrDefault();
                var lsAuto = getListSetAuto(a1);
                ViewBag.SelecDefault = new SelectList(lsAuto, "IDVT", "TenViTri");
                ViewBag.CountVT = lsAuto.Count;

            }
            else
            {
                return HttpNotFound();
            }
            return PartialView(DO);
        }
        [HttpPost]
        public ActionResult PerCheck(DSachDGValidation _DO, int? IDPB, FormCollection collection)
        {
            var vt = db.ViTriKNLs.Where(x => x.IDVT == _DO.IDKNL).FirstOrDefault();
            var controll = collection["MaViTri"];
            try
            {
                if (_DO.Selec != null) //ĐG truc tiep
                {
                    for (int i = 0; i < _DO.Selec.Length; i++)
                    {
                        if (GetKNL_DGiaTT(_DO.IDKNL, ToNullableInt(_DO.Selec[i])) == 0)
                        {
                            db.KNLDGiaTC_insert(_DO.IDKNL, ToNullableInt(_DO.Selec[i]), null);
                        }
                    }
                }

                if (_DO.Selected != null) //Xem lai ket qua
                {
                    for (int i = 0; i < _DO.Selected.Length; i++)
                    {
                        //db.DSachDG_insert(_DO.Selected[i], manv, _DO.IDKNL);
                        if (GetKNL_DGiaTC(_DO.IDKNL, ToNullableInt(_DO.Selected[i])) == 0)
                        {
                            db.KNLDGiaTC_insert(_DO.IDKNL, null, ToNullableInt(_DO.Selected[i]));
                        }
                    }
                }
                int? px1 = ToNullableInt(collection["IDPX1"]);
                int? nhom1 = ToNullableInt(collection["IDNhom1"]);
                int? to1 = ToNullableInt(collection["IDTo1"]);
                if (px1 != null)
                {
                    var a1 = db.ViTriKNLs.Where(x => x.IDPX == px1).ToList();
                    foreach (var item in a1)
                    {
                        if (GetKNL_DGiaTT(_DO.IDKNL, item.IDVT) == 0)
                        {
                            db.KNLDGiaTC_insert(_DO.IDKNL, item.IDVT, null);
                        }
                    }
                }
                if (nhom1 != null)
                {
                    var a1 = db.ViTriKNLs.Where(x => x.IDNhom == nhom1).ToList();
                    foreach (var item in a1)
                    {
                        if (GetKNL_DGiaTC(_DO.IDKNL, item.IDVT) == 0)
                        {
                            db.KNLDGiaTC_insert(_DO.IDKNL, item.IDVT, null);
                        }
                    }
                }
                if (to1 != null)
                {
                    var a1 = db.ViTriKNLs.Where(x => x.IDTo == to1).ToList();
                    foreach (var item in a1)
                    {
                        if (GetKNL_DGiaTC(_DO.IDKNL, item.IDVT) == 0)
                        {
                            db.KNLDGiaTC_insert(_DO.IDKNL, item.IDVT, null);
                        }
                    }
                }
                int? px2 = ToNullableInt(collection["IDPX2"]);
                int? nhom2 = ToNullableInt(collection["IDNhom2"]);
                int? to2 = ToNullableInt(collection["IDTo2"]);
                int? pb2 = ToNullableInt(collection["IDPB2"]);
                if (pb2 != null)
                {
                    var a1 = db.ViTriKNLs.Where(x => x.IDPB == pb2).ToList();
                    foreach (var item in a1)
                    {
                        if (GetKNL_DGiaTC(_DO.IDKNL, item.IDVT) == 0 && _DO.IDKNL != item.IDVT)
                        {
                            db.KNLDGiaTC_insert(_DO.IDKNL, null, item.IDVT);
                        }
                    }
                }
                if (px2 != null)
                {
                    var a1 = db.ViTriKNLs.Where(x => x.IDPX == px2).ToList();
                    foreach (var item in a1)
                    {
                        if (GetKNL_DGiaTC(_DO.IDKNL, item.IDVT) == 0)
                        {
                            db.KNLDGiaTC_insert(_DO.IDKNL, null, item.IDVT);
                        }
                    }
                }
                if (nhom2 != null)
                {
                    var a1 = db.ViTriKNLs.Where(x => x.IDNhom == nhom2).ToList();
                    foreach (var item in a1)
                    {
                        if (GetKNL_DGiaTC(_DO.IDKNL, item.IDVT) == 0)
                        {
                            db.KNLDGiaTC_insert(_DO.IDKNL, null, item.IDVT);
                        }
                    }
                }
                if (to2 != null)
                {
                    var a1 = db.ViTriKNLs.Where(x => x.IDTo == to2).ToList();
                    foreach (var item in a1)
                    {
                        if (GetKNL_DGiaTC(_DO.IDKNL, item.IDVT) == 0)
                        {
                            db.KNLDGiaTC_insert(_DO.IDKNL, null, item.IDVT);
                        }
                    }
                }
                //db.VitriKNL_update(_DO.IDVT, _DO.TenViTri, _DO.MaViTri, _DO.IDPB, _DO.IDKhoi, _DO.IDPX, _DO.IDNhom, _DO.IDTo, _DO.FilePath);

                TempData["msgSuccess"] = "<script>alert('Cập nhập thành công');</script>";
            }
            catch (Exception e)
            {

                TempData["msgSuccess"] = "<script>alert('Cập nhập thất bại " + e.Message + " ');</script>";
            }
            if(controll == "SetVTPermision")  return RedirectToAction("SetVTPermision", "FPosition", new { IDPB = IDPB });
            return RedirectToAction("Index", "FPosition", new { IDPB = IDPB, IDKhoi = vt.IDKhoi, IDPX = vt.IDPX, IDTo = vt.IDTo, IDNhom = vt.IDNhom });
        }
        public ActionResult Delete(int id, int? IDPB)
        {
            try
            {
                var aa = db.NhanViens.Where(x => x.IDVTKNL == id ).ToList();
                var ab = db.KNL_LSDG.Where(x => x.VTID == id).ToList();
                var ac = db.KNL_NVKiemNhiem.Where(x => x.IDVTKN == id).ToList();
                if (aa.Count == 0 && ab.Count == 0 && ac.Count == 0) { db.VitriKNL_delete(id); db.QT_PhanQuyen.Where(x => x.IDVTKNL == id).ToList().RemoveAll(x => x.IDVTKNL == id); db.SaveChanges(); }
                else TempData["msgError"] = "<script>alert('Xóa dữ liệu thất bại');</script>";
                //if (aa.Count > 0)
                //{
                //    TempData["msgError"] = "<script>alert('Xóa dữ liệu thất bại');</script>";
                //    //foreach (var item in aa)
                //    //{
                //    //    db.Nhanvien_update_IDKNL(item.MaNV, null);
                //    //}
                //}
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Xóa dữ liệu thất bại');</script>";
            }
            return RedirectToAction("Index", "FPosition", new { IDPB = IDPB });
        }

        public ActionResult Hide(int id, int? IDPB)
        {
            try
            {
                db.NhanViens.Where(x => x.IDVTKNL == id).ToList().ForEach(i => i.IDVTKNL = null);
                db.KNL_NVKiemNhiem.Where(x => x.IDVTKN == id).ToList().RemoveAll(x => x.IDVTKN == id);
                db.SaveChanges();
                db.VitriKNL_update_TinhTrang(id,0);
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Xóa dữ liệu thất bại');</script>";
            }
            return RedirectToAction("Index", "FPosition", new { IDPB = IDPB });
        }

        public ActionResult Show(int id, int? IDPB)
        {
            try
            {
                db.VitriKNL_update_TinhTrang(id, 1);
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Xóa dữ liệu thất bại');</script>";
            }
            return RedirectToAction("Index", "FPosition", new { IDPB = IDPB });
        }

        public int GetIDNVKN(int? IDNV, int? IDVTKN)
        {
            var model = db.KNL_NVKiemNhiem.Where(x => x.IDNV == IDNV && x.IDVTKN == IDVTKN).SingleOrDefault();
            if (model == null)
                return 0;
            return model.ID;
        }
        public int GetKNLDG(int? IDVTDG, int? IDVTDDG)
        {
            var model = db.KNL_DG.Where(x => x.IDVTDG == IDVTDG && x.IDVTDDG == IDVTDDG).SingleOrDefault();
            if (model == null)
                return 0;
            return model.ID;
        }

        public int GetKNL_DGiaTT(int? IDVT, int? IDVTDGTT)
        {
            var model = db.KNL_DGiaTC.Where(x => x.IDVTDGTT == IDVTDGTT && x.IDVT == IDVT).SingleOrDefault();
            if (model == null)
                return 0;
            return model.ID;
        }

        public int GetKNL_DGiaTC(int? IDVT, int? IDVTDGTC)
        {
            var model = db.KNL_DGiaTC.Where(x => x.IDVTDGTC == IDVTDGTC && x.IDVT == IDVT).SingleOrDefault();
            if (model == null)
                return 0;
            return model.ID;
        }
        public ActionResult UpdateNV(int? id, int? IDPB)
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            if (!ListQuyen.Contains(CONSTKEY.U_KNL))
            {
                TempData["msgError"] = "<script>alert('Bạn không có quyền thực hiện chức năng này');</script>";
                return RedirectToAction("", "Home");
            }
            ViewBag.QUYENCN = ListQuyen;
            //int IDPB = MyAuthentication.IDPhongban;
            string manv = MyAuthentication.Username;

            var aa = db.NhanViens.Where(x => x.IDTinhTrangLV == 1).ToList();

            //List<EmployeeValidation> nv1 = db.NhanViens.Where(x => x.IDTinhTrangLV == 1 && x.IDPhongBan ==IDPB).Select(x => new EmployeeValidation { MaNV = x.MaNV, HoTen = x.MaNV + " - " + x.HoTen }).ToList();
            List<EmployeeValidation> nv1 = aa.Where(x => x.IDTinhTrangLV == 1 && x.IDPhongBan == IDPB).Select(x => new EmployeeValidation { MaNV = x.MaNV, HoTen = x.MaNV + " - " + x.HoTen }).ToList();
            //List<EmployeeValidation> nv = new List<EmployeeValidation>();
            //foreach (var i in nv1)
            //{
            //    var nvSe = db.DSachDGs.Where(x => x.MaNV == i.MaNV).FirstOrDefault();
            //    if (nvSe == null)
            //    {
            //        nv.Add(i);
            //    }
            //}
            var nv2 = aa.Where(x => x.IDTinhTrangLV == 1 && x.IDVTKNL == id).Select(x => new EmployeeValidation { MaNV = x.MaNV, HoTen = x.MaNV + " - " + x.HoTen }).ToList();
            ViewBag.Selec = new SelectList(nv2, "MaNV", "HoTen");
            ViewBag.Selected = new SelectList(nv1, "MaNV", "HoTen");

            var nv3 = aa.Select(x => new EmployeeValidation { ID = x.ID, HoTen = x.MaNV + " - " + x.HoTen }).ToList();
            ViewBag.SelectedKN = new SelectList(nv3, "ID", "HoTen");

            var nvkn = (from a in db.KNL_NVKiemNhiem.Where(x => x.IDVTKN == id)
                        join h in db.NhanViens
                        on a.IDNV equals h.ID into ulg
                        from h in ulg.DefaultIfEmpty()
                        select new EmployeeValidation { ID = a.ID, HoTen = h.MaNV + " - " + h.HoTen }).ToList();
            //var nvkn = aa.Where(x => x.IDTinhTrangLV == 1 && x.IDVTKN == id).Select(x => new EmployeeValidation { MaNV = x.MaNV, HoTen = x.MaNV + " - " + x.HoTen }).ToList();
            ViewBag.SelecKN = new SelectList(nvkn, "ID", "HoTen");
            //List<ViTriKNL> knl = db.ViTriKNLs.Where(x => x.IDPB == IDPB).ToList();

            //var b = db.QuyenKNLs.Where(x => x.MaNV == manv).Select(x => x.IDVT).ToList();
            //List<ViTriKNL> vt = new List<ViTriKNL>();
            //foreach (var item in b)
            //{
            //    var vt2 = db.ViTriKNLs.Where(x => x.IDPB == IDPB && x.IDVT == item).FirstOrDefault();
            //    if (vt2 != null)
            //    {
            //        vt.Add(vt2);
            //    }
            //}
            //ViewBag.IDKNL = new SelectList(vt, "IDVT", "TenViTri");
            List<ViTriKNL> vt = db.ViTriKNLs.Where(x => x.IDVT == id).ToList();
            ViewBag.IDKNL = new SelectList(vt, "IDVT", "TenViTri", id);
            List<PhongBan> dt = db.PhongBans.Where(x => x.IDPhongBan == IDPB).ToList();
            ViewBag.IDPB = new SelectList(dt, "IDPhongBan", "TenPhongBan", IDPB);
            ViewBag.SelecIDPB = IDPB;
            ViewBag.SelecIDVT = id;

            return PartialView();
        }

        [HttpPost]
        public ActionResult UpdateNV(DSachDGValidation _DO, int? IDPB)
        {
            var vt = new ViTriKNL();
            try
            {
                //string manv = MyAuthentication.Username;
                vt = db.ViTriKNLs.Where(x => x.IDVT == _DO.IDKNL).FirstOrDefault();
                if (!String.IsNullOrEmpty(_DO.NVDG))
                {
                    
                    //Regex.Replace(_DO.NVDG, @"[^0-9a-zA-Z]+", " ");
                    string tx = Regex.Replace(_DO.NVDG, @"[^0-9a-zA-Z]+", " ");
                    string[] NVS = tx.Split(new char[] { ' ' });
                    foreach (var item in NVS)
                    {
                        var aa = db.NhanViens.Where(x => x.MaNV == item && x.IDTinhTrangLV ==1).Count();
                        if (aa > 0)
                        {
                            db.Nhanvien_update_IDKNL(item, _DO.IDKNL);
                        }
                    }
                }
                if (_DO.Selected != null)
                {
                    for (int i = 0; i < _DO.Selected.Length; i++)
                    {
                        //db.DSachDG_insert(_DO.Selected[i], manv, _DO.IDKNL);
                        if (_DO.Selected[i] != null && _DO.IDKNL != null)
                        {
                            //var mavt = db.ViTriKNLs.Where(x => x.IDVT == _DO.IDKNL).FirstOrDefault()?.MaViTri;
                            //db.Nhanvien_update_ViTri(_DO.Selected[i], mavt);
                            db.Nhanvien_update_IDKNL(_DO.Selected[i], _DO.IDKNL);
                        }
                    }
                }
                if (_DO.SelectedKN != null)
                {
                    for (int i = 0; i < _DO.SelectedKN.Length; i++)
                    {
                        //db.DSachDG_insert(_DO.Selected[i], manv, _DO.IDKNL);
                        int? idkn = GetIDNVKN(_DO.SelectedKN[i], _DO.IDKNL);
                        if (idkn == 0)
                        {
                            //var mavt = db.ViTriKNLs.Where(x => x.IDVT == _DO.IDKNL).FirstOrDefault()?.MaViTri;
                            //db.Nhanvien_update_ViTri(_DO.Selected[i], mavt);
                            db.KNLNVKiemNhiem_insert(_DO.SelectedKN[i], _DO.IDKNL);
                            //db.Nhanvien_update_IDVTKN(_DO.SelectedKN[i], _DO.IDKNL);
                        }
                        else
                        {
                            db.KNLNVKiemNhiem_update(idkn, _DO.SelectedKN[i], _DO.IDKNL);
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
            return RedirectToAction("Index", "FPosition", new { IDPB = IDPB, IDKhoi = vt.IDKhoi, IDPX = vt.IDPX, IDTo = vt.IDTo, IDNhom = vt.IDNhom });
        }
        public ActionResult DeleteNVIDKNL(string manv, int? IDPB)
        {
            try
            {
                db.Nhanvien_update_IDKNL(manv, null);
            }
            catch (Exception e)
            {
                TempData["msgSuccess"] = "<script>alert('Xóa dữ liệu thất bại: " + e.Message + "');</script>";
            }
            return RedirectToAction("Index", "FPosition", new { IDPB = IDPB });
        }

        public ActionResult DeleteIDKNL(string id)
        {
            try
            {
                db.Nhanvien_update_IDKNL(id, null);
            }
            catch (Exception e)
            {
                TempData["msgSuccess"] = "<script>alert('Xóa dữ liệu thất bại: " + e.Message + "');</script>";
            }
            return RedirectToAction("ListNVKNL", "FPosition");
        }

        public ActionResult DeleteKNLDG(int? IDVTDG, int? IDPB)
        {
            try
            {
                db.KNLDG_delete(IDVTDG);
            }
            catch (Exception e)
            {
                TempData["msgSuccess"] = "<script>alert('Xóa dữ liệu thất bại: " + e.Message + "');</script>";
            }
            return RedirectToAction("Index", "FPosition", new { IDPB = IDPB });
        }
        public ActionResult DeleteNVKN(int? ID)
        {
            try
            {
                db.KNLNVKiemNhiem_delete(ID);
                TempData["msgSuccess"] = "<script>alert('Xóa thành công');</script>";
            }
            catch (Exception e)
            {
                TempData["msgSuccess"] = "<script>alert('Xóa dữ liệu thất bại: " + e.Message + "');</script>";
            }
            return RedirectToAction("UpdateNVKN", "FPosition");
        }

        public ActionResult UpdateNVKN()
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            //if (!ListQuyen.Contains(CONSTKEY.U_KNL))
            //{
            //    TempData["msgError"] = "<script>alert('Bạn không có quyền thực hiện chức năng này');</script>";
            //    return RedirectToAction("", "Home");
            //}
            ViewBag.QUYENCN = ListQuyen;
            var vt = (from a in db.ViTriKNLs
                      join b in db.PhongBans on a.IDPB equals b.IDPhongBan into ulp
                      from b in ulp.DefaultIfEmpty()
                      join e in db.KNL_PhanXuong on a.IDPX equals e.ID into ul
                      from e in ul.DefaultIfEmpty()
                      join f in db.KNL_Nhom on a.IDNhom equals f.IDNhom into uls
                      from f in uls.DefaultIfEmpty()
                      join g in db.KNL_To on a.IDTo equals g.IDTo into ulk
                      from g in ulk.DefaultIfEmpty()
                      select new ViTriKNLValidation
                      {
                          IDVT = a.IDVT,
                          TenViTri = a.TenViTri + "-" + f.TenNhom + "-" + g.TenTo + "-" + e.TenPX + "-" + b.TenPhongBan,
                          IDNhom = a.IDNhom,
                          IDTo = a.IDTo,
                          MaViTri = a.MaViTri,
                          IDPX = a.IDPX,
                          IDVTParent = a.IDVTParent
                      });

            var res = (from a in db.KNL_NVKiemNhiem
                       join b in db.NhanViens on a.IDNV equals b.ID into ulkh
                       from b in ulkh.DefaultIfEmpty()
                       join v in vt on b.IDVTKNL equals v.IDVT into ulkhv
                       from v in ulkhv.DefaultIfEmpty()
                       join kn in vt on a.IDVTKN equals kn.IDVT into ulkhk
                       from kn in ulkhk.DefaultIfEmpty()
                       select new NVKNVTValidation
                       {
                           ID = a.ID,
                           IDNV = b.ID,
                           TenNV = b.HoTen,
                           MaNV = b.MaNV,
                           IDVT = b.IDVTKNL,
                           TenVT = v.TenViTri,
                           IDVTKN = a.IDVTKN,
                           TenVTKN = kn.TenViTri,
                       }).ToList();
            NVKNVTValidation DO = new NVKNVTValidation();

            //if (res.Count > 0)
            //{
            foreach (var c in res)
            {
                DO.ID = c.ID;
                DO.IDNV = c.IDNV;
                DO.TenNV = c.TenNV;
                DO.MaNV = c.MaNV;
                DO.IDVT = c.IDVT;
                DO.TenVT = c.TenVT;
                DO.IDVTKN = c.IDVTKN;
                DO.TenVTKN = c.TenVTKN;
            }
            //int idpb = MyAuthentication.IDPhongban;
            db.Configuration.ProxyCreationEnabled = false;

            //}
            //else
            //{
            //    return HttpNotFound();
            //}
            return View(res.ToList());
        }

        public ActionResult CreateNVKN()
        {
            var aa = db.NhanViens.Where(x => x.IDTinhTrangLV == 1).ToList();

            var nv3 = aa.Select(x => new EmployeeValidation { ID = x.ID, HoTen = x.MaNV + " - " + x.HoTen }).ToList();
            ViewBag.IDNV = new SelectList(nv3, "ID", "HoTen");

            var vt = (from a in db.ViTriKNLs
                      join b in db.PhongBans on a.IDPB equals b.IDPhongBan into ulp
                      from b in ulp.DefaultIfEmpty()
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
                          TenViTri = a.TenViTri  + "-" + f.TenNhom + "-" + g.TenTo  + "-" + e.TenPX + "-" + b.TenPhongBan,
                          IDNhom = a.IDNhom,
                          IDTo = a.IDTo,
                          MaViTri = a.MaViTri,
                          IDPX = a.IDPX,
                          IDVTParent = a.IDVTParent
                      }).ToList();
            //var kvt = vt.Where(x => x.IDVT == id).ToList();
            //ViewBag.IDKNL = new SelectList(kvt, "IDVT", "TenViTri", id);
            ViewBag.Selected = new SelectList(vt, "IDVT", "TenViTri");

            return PartialView();
        }
        [HttpPost]
        public ActionResult CreateNVKN(NVKNSelect _DO)
        {
            try
            {
                if (_DO.Selected != null && _DO.IDNV != null)
                {
                    foreach (var item in _DO.Selected)
                    {
                        var aa = GetIDNVKN(_DO.IDNV, item);
                        if (aa == 0)
                        {
                            db.KNLNVKiemNhiem_insert(_DO.IDNV, item);
                        }
                    }
                    TempData["msgSuccess"] = "<script>alert('Thêm mới thành công');</script>";
                }
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Có lỗi khi thêm mới: " + e.Message + "');</script>";
            }
            return RedirectToAction("UpdateNVKN", "FPosition");
        }

        public ActionResult ShowSetVTPermision(int? IDVT, string TenPB, string TenVT)
        {
            ViewBag.TenPB = TenPB;
            ViewBag.TenVT = TenVT;
            List<KNLDGiaTCValidation> lsVTTT = (from a in db.KNL_DGiaTC.Where(x => x.IDVT == IDVT && x.IDVTDGTT != null)
                                                join b in db.ViTriKNLs on a.IDVTDGTT equals b.IDVT
                                                join e in db.KNL_PhanXuong
                                                on b.IDPX equals e.ID into ul
                                                from e in ul.DefaultIfEmpty()
                                                join f in db.KNL_Nhom
                                                on b.IDNhom equals f.IDNhom into uls
                                                from f in uls.DefaultIfEmpty()
                                                join g in db.KNL_To
                                                on b.IDTo equals g.IDTo into ulk
                                                from g in ulk.DefaultIfEmpty()
                                                select new KNLDGiaTCValidation
                                                {
                                                    IDVT = (int)a.IDVT,
                                                    TenViTri = b.TenViTri + "-" + e.TenPX + "-" + f.TenNhom + "-" + g.TenTo,
                                                    MaViTri = b.MaViTri,
                                                    IDPB = b.IDPB,
                                                    IDVTDGTC = a.IDVTDGTC,
                                                    IDVTDGTT = a.IDVTDGTT
                                                }).ToList();
            List<KNLDGiaTCValidation> lsVTTC = (from a in db.KNL_DGiaTC.Where(x => x.IDVT == IDVT && x.IDVTDGTC != null)
                                                join b in db.ViTriKNLs on a.IDVTDGTC equals b.IDVT
                                                join e in db.KNL_PhanXuong
                                                on b.IDPX equals e.ID into ul
                                                from e in ul.DefaultIfEmpty()
                                                join f in db.KNL_Nhom
                                                on b.IDNhom equals f.IDNhom into uls
                                                from f in uls.DefaultIfEmpty()
                                                join g in db.KNL_To
                                                on b.IDTo equals g.IDTo into ulk
                                                from g in ulk.DefaultIfEmpty()
                                                select new KNLDGiaTCValidation
                                                {
                                                    IDVT = (int)a.IDVT,
                                                    TenViTri = b.TenViTri + "-" + e.TenPX + "-" + f.TenNhom + "-" + g.TenTo,
                                                    MaViTri = b.MaViTri,
                                                    IDPB = b.IDPB,
                                                    IDVTDGTC = a.IDVTDGTC,
                                                    IDVTDGTT = a.IDVTDGTT
                                                }).ToList();
            var a1 = db.ViTriKNLs.Where(x => x.IDVT == IDVT).FirstOrDefault();
            var lsAuto = getListSetAuto(a1);
            ViewBag.ShowVTAuto = lsAuto;
            ViewBag.ShowVTTT = lsVTTT;
            ViewBag.ShowVTTC = lsVTTC;

            return PartialView();
        }

        public ActionResult SetVTPermision(int? page, string search, int? IDPB, int? IDPX, int? IDNhom, int? IDKhoi, int? IDTo)
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            if (!ListQuyen.Contains("PER_KNL"))
            {
                TempData["msgError"] = "<script>alert('Bạn không có quyền thực hiện chức năng này');</script>";
                return RedirectToAction("", "Home");
            }
            List<PhongBan> dt = db.PhongBans.ToList();
            ViewBag.IDPB = new SelectList(dt, "IDPhongBan", "TenPhongBan");
            List<KNL_Khoi> khoi = db.KNL_Khoi.ToList();
            ViewBag.IDKhoi = new SelectList(khoi, "ID", "TenKhoi");
            if (IDPB == null) IDPB = 0;

            List<SetPermisionKNLValidation> res = new List<SetPermisionKNLValidation>();
            if(IDPB != 0)
            {
                res = (from a in db.ViTriKNLs.Where(x=>x.IDPB == IDPB)
                       join e in db.KNL_PhanXuong
                       on a.IDPX equals e.ID into ul
                       from e in ul.DefaultIfEmpty()
                       join f in db.KNL_Nhom
                       on a.IDNhom equals f.IDNhom into uls
                       from f in uls.DefaultIfEmpty()
                       join g in db.KNL_To
                       on a.IDTo equals g.IDTo into ulk
                       from g in ulk.DefaultIfEmpty()
                       join h in db.PhongBans
                       on a.IDPB equals h.IDPhongBan into ulkp
                       from h in ulkp.DefaultIfEmpty()
                       select new SetPermisionKNLValidation
                       {
                           IDVT = a.IDVT,
                           TenViTri = a.TenViTri + "-" + f.TenNhom + "-" + g.TenTo + "-" + e.TenPX + "-" + h.MaPB,
                           MaViTri = a.MaViTri,
                           IDPB = a.IDPB,
                           IDPX = a.IDPX,
                           IDKhoi = a.IDKhoi,
                           IDNhom = a.IDNhom,
                           IDTo = a.IDTo,
                           //ListVTTT = (from aa in n join ll in db.KNL_DGiaTC on aa.IDVT equals ll.IDVT select new ),
                           //ListVTSet = new List<ViTriKNLValidation>(),
                           CountDGTT = db.KNL_DGiaTC.Where(x => x.IDVT == a.IDVT && x.IDVTDGTT != null).Count(),
                           CountDGTC = db.KNL_DGiaTC.Where(x => x.IDVT == a.IDVT && x.IDVTDGTC != null).Count(),
                       }).ToList();
            }
            else
            {
                res = (from a in db.ViTriKNLs
                       join e in db.KNL_PhanXuong
                       on a.IDPX equals e.ID into ul
                       from e in ul.DefaultIfEmpty()
                       join f in db.KNL_Nhom
                       on a.IDNhom equals f.IDNhom into uls
                       from f in uls.DefaultIfEmpty()
                       join g in db.KNL_To
                       on a.IDTo equals g.IDTo into ulk
                       from g in ulk.DefaultIfEmpty()
                       join h in db.PhongBans
                       on a.IDPB equals h.IDPhongBan into ulkp
                       from h in ulkp.DefaultIfEmpty()
                       select new SetPermisionKNLValidation
                       {
                           IDVT = a.IDVT,
                           TenViTri = a.TenViTri + "-" + f.TenNhom + "-" + g.TenTo + "-" + e.TenPX + "-" + h.MaPB,
                           MaViTri = a.MaViTri,
                           IDPB = a.IDPB,
                           IDPX = a.IDPX,
                           IDKhoi = a.IDKhoi,
                           IDNhom = a.IDNhom,
                           IDTo = a.IDTo,
                           //ListVTTT = (from aa in n join ll in db.KNL_DGiaTC on aa.IDVT equals ll.IDVT select new ),
                           //ListVTSet = new List<ViTriKNLValidation>(),
                           CountDGTT = db.KNL_DGiaTC.Where(x => x.IDVT == a.IDVT && x.IDVTDGTT != null).Count(),
                           CountDGTC = db.KNL_DGiaTC.Where(x => x.IDVT == a.IDVT && x.IDVTDGTC != null).Count(),
                       }).ToList();
            }
            res = res.Where(x => x.CountDGTT != 0 && x.CountDGTC != 0).ToList();
            if (IDPB == null) IDPB = 0;
            if (IDPB != 0) res = res.Where(x => x.IDPB == IDPB).ToList();
            if (IDPX != null) res = res.Where(x => x.IDPX == IDPX).ToList();
            if (IDKhoi != null) res = res.Where(x => x.IDKhoi == IDKhoi).ToList();
            if (IDNhom != null) res = res.Where(x => x.IDNhom == IDNhom).ToList();
            if (IDTo != null) res = res.Where(x => x.IDTo == IDTo).ToList();

            if (page == null) page = 1;
            int pageSize = 50;
            int pageNumber = (page ?? 1);
            return View(res.ToList().ToPagedList(pageNumber, pageSize));
        }

        public ActionResult ListNVKNL(int? page, string search, int? IDPB, int? IDPX, int? IDNhom, int? IDKhoi, int? IDTo)
        {
            if (page == null) page = 1;
            int pageSize = 20;
            int pageNumber = (page ?? 1);

            int idpb = MyAuthentication.IDPhongban;
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            ViewBag.QUYENCN = ListQuyen;
            var ListNV = new List<FCheckValidation>();
            if (search == null) search = "";
            ViewBag.search = search;

            List<KNL_Khoi> khoi = db.KNL_Khoi.ToList();
            ViewBag.IDKhoi = new SelectList(khoi, "ID", "TenKhoi");
            try
            {
                if (IDPB == null) IDPB = 0;
                //if (IDPB != 0) res = res.Where(x => x.IDPB == IDPB).ToList();
                var res = (from a in db.NhanVien_searchByVTKNL(IDPB)
                           select new FCheckValidation
                           {
                               MaNV = a.MaNV,
                               IDNV = a.ID,
                               IDVT = a.IDVT,
                               TenVT = a.TenViTri + "-" + a.TenNhom + "-" + a.TenTo + "-" + a.MaPX + "-" + a.MaPB,
                               TenNV = a.HoTen,
                               IDNhom = a.IDNhom,
                               IDPB = a.IDPB,
                               IDKip = a.IDKip,
                               TenKip = a.TenKip,
                               MaViTri = a.MaViTri,
                               TenPB = a.TenPhongBan,
                               IDPX = a.IDPX,
                               IDTo = a.IDTo,
                           }).ToList();
                if (IDPX != null) res = res.Where(x => x.IDPX == IDPX).ToList();
                //if (IDKhoi != null) res = res.Where(x => x.IDKhoi == IDKhoi).ToList();
                if (IDNhom != null) res = res.Where(x => x.IDNhom == IDNhom).ToList();
                if (IDTo != null) res = res.Where(x => x.IDTo == IDTo).ToList();
                if (!String.IsNullOrEmpty(search)) res = res.Where(x => x.MaNV == search || x.TenNV.ToLower() == search.ToLower()).ToList();

                if (ListQuyen.Contains("VIEW_BP"))
                {
                    res = res.Where(x => x.IDPB == idpb).ToList();
                }
                if (!User.Identity.IsAuthenticated)
                {
                    ListNV = new List<FCheckValidation>();
                }
                else ListNV = res;
                int paz = pageNumber * pageSize > res.Count ? res.Count - (pageNumber - 1) * pageSize : pageSize;
                foreach (var item in res.GetRange(pageSize * (pageNumber - 1), paz))
                {
                    //var KQ = db.KNL_KQ.Where(x => x.IDNV == item.IDNV).OrderByDescending(i => i.NgayDG).ToList();
                    //var KQ = db.KNL_KQ_SelectNV(item.IDNV).ToList();
                    //var lastCheck = KQ.FirstOrDefault();
                    //if (lastCheck != null)
                    //{
                    //    var KQDG = db.KNL_KQ_Select(item.IDNV, lastCheck?.ThangDG).Where(x => x.IDVT == item.IDVT).ToList();
                    //    if (KQDG.Count > 0)
                    //    {
                    //        item.NgayDG = KQ.FirstOrDefault()?.NgayDG != null ? String.Format("{0:dd/MM/yyyy}", KQ.FirstOrDefault()?.NgayDG) : "";
                    //        item.Total = lastCheck.ThangDG != null ? db.KNL_KQ_searchByIDNV(item.IDNV, lastCheck?.ThangDG, item.IDVT).Count() - KQDG.Where(x => x.IsDanhGia == 0 && x.DiemDG == null).Count() : 0;
                    //        item.TotalDat = KQDG != null ? KQDG.Where(x => x.DiemDG == x.DinhMuc && x.DiemDG != null).Count() : 0;
                    //        item.TotalKDat = KQDG != null ? KQDG.Where(x => x.DiemDG < x.DinhMuc && x.DiemDG != null).Count() : 0;
                    //        item.TotalVuot = KQDG != null ? KQDG.Where(x => x.DiemDG > x.DinhMuc && x.DiemDG != null).Count() : 0;
                    //        item.TotalKDGia = KQDG != null ? KQDG.Where(x => x.IsDanhGia == 1 && x.DiemDG == null).Count() : 0;
                    //        item.ThangDG = lastCheck?.ThangDG.ToString();
                    //    }
                    //}
                    var KQ = db.KNL_LSDG.Where(x=>x.NVID == item.IDNV).OrderByDescending(i => i.NgayDGGN).FirstOrDefault();
                    if(KQ != null)
                    {
                        item.NgayDG = String.Format("{0:dd/MM/yyyy}", KQ?.NgayDGGN);
                        item.Total = KQ.TONGNL;
                        item.TotalDat =KQ.DAT;
                        item.TotalKDat = KQ.KDAT;
                        item.TotalVuot = KQ.VUOT;
                        item.TotalKDGia = KQ.KDGia;
                        item.TotalChuaDG = KQ.CHUADG;
                        item.ThangDG = KQ.ThangDG.ToString();
                        item.NgayCanhBao = KQ.KDAT > 0 ? (((DateTime)KQ.NgayDGGN).AddMonths(6) - DateTime.Now).Days : -1000;
                        item.NgayHanDG = KQ.KDAT > 0 ? ((DateTime)KQ.NgayDGGN).AddMonths(6) : default;
                    }
                    //add QT/HD
                    var qthd = db.QT_PhanQuyen.Where(x => x.IDVTKNL == item.IDVT && x.QT_NoiDungQT.NgayHieuLuc < DateTime.Now && (x.QT_NoiDungQT.NgayHetHieuLuc == null || x.QT_NoiDungQT.NgayHetHieuLuc == default || x.QT_NoiDungQT.NgayHetHieuLuc > DateTime.Now)).ToList();

                    item.TotalQTHD =qthd.Where(x=>x.QT_NoiDungQT.TinhTrang ==1).Count();
                    if(item.TotalQTHD > 0){
                        int dem = 0;
                        foreach(var x in qthd) {
                          var al=  db.QT_BaiKiemTra_CheckQT(item.IDNV, x.QTHDID, DateTime.Now);
                            if (al.Count() > 0)
                            {
                                dem++;
                            }
                        }
                        item.TotalHTQTHD =dem;
                    }
                    //var checkkt = db.QT_BaiKiemTra.Where(x => x.IDNV == item.IDNV && x.QTHDID == item.).OrderByDescending(x => x.LanKT).ToList();


                }
            }
            catch (Exception e)
            {
                TempData["msgSuccess"] = "<script>alert('Có lỗi xảy ra: " + e.Message + "');</script>";
            }

            List<PhongBan> dt = db.PhongBans.ToList();
            if (ListQuyen.Contains("VIEW_BP")) dt = dt.Where(x => x.IDPhongBan == idpb).ToList();
            ViewBag.IDPB = new SelectList(dt, "IDPhongBan", "TenPhongBan");

            return View(ListNV.ToList().ToPagedList(pageNumber, pageSize));
        }

        public ActionResult ChangeNVGroup()
        {

            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            ViewBag.QUYENCN = ListQuyen;
            //if (!ListQuyen.Contains(CONSTKEY.V))
            //{
            //    TempData["msgError"] = "<script>alert('Bạn không có quyền truy cập chức năng này');</script>";
            //    return RedirectToAction("", "Home");
            //}

            var aa = db.NhanViens.Where(x => x.IDTinhTrangLV == 1).ToList();

            var nv3 = aa.Select(x => new EmployeeValidation { MaNV = x.MaNV, HoTen = x.MaNV + " - " + x.HoTen }).ToList();
            ViewBag.Selected = new SelectList(nv3, "MaNV", "HoTen");

            var vt = (from a in db.ViTriKNLs
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

            //List<ViTriKNL> vt = db.ViTriKNLs.ToList();
            ViewBag.IDVT = new SelectList(vt, "IDVT", "TenViTri");
            List<PhongBan> dt = db.PhongBans.ToList();
            ViewBag.IDPB = new SelectList(dt, "IDPhongBan", "TenPhongBan");
            List<KNL_Khoi> khoi = db.KNL_Khoi.ToList();
            ViewBag.IDKhoi = new SelectList(khoi, "ID", "TenKhoi");

            List<KNL_PhanXuong> px = db.KNL_PhanXuong.ToList();
            ViewBag.IDPX = new SelectList(px, "ID", "TenPX");
            List<KNL_To> to = db.KNL_To.ToList();
            ViewBag.IDTo = new SelectList(to, "IDTo", "TenTo");

            List<KNL_Nhom> n = db.KNL_Nhom.ToList();
            ViewBag.IDNhom = new SelectList(n, "IDNhom", "TenNhom");


            return PartialView();
        }
        [HttpPost]
        public ActionResult ChangeNVGroup(DSachDGValidation _DO, FormCollection collection)
        {

            try
            {
                //if (_DO.MaNV != null)
                //{
                //    //var aa = db.KNLTo_insert(_DO.TenTo, _DO.MaTo, (int?)_DO.IDPhongBan, (int?)_DO.IDPhanXuong, _DO.IDKhoi);
                //}
                var ListVT = new List<ViTriKNLValidation>();
                foreach (var key in collection.AllKeys)
                {
                    if (key.Split('_')[0] == "IDVT")
                    {
                        ListVT.Add(new ViTriKNLValidation() { MaViTri = collection["IDVT_" + key.Split('_')[1]], TenViTri = collection["NVDG_" + key.Split('_')[1]] });
                    }
                }
                foreach (var item in ListVT)
                {
                    //int idvt = GetIDTo(item.TenViTri.Trim());
                    if (!String.IsNullOrEmpty(item.TenViTri) && !String.IsNullOrEmpty(item.MaViTri))
                    {
                        //Regex.Replace(_DO.NVDG, @"[^0-9a-zA-Z]+", " ");
                        string tx = Regex.Replace(item.TenViTri, @"[^0-9a-zA-Z]+", " ");
                        string[] NVS = tx.Split(new char[] { ' ' });
                        foreach (var item1 in NVS)
                        {
                            var aa = db.NhanViens.Where(x => x.MaNV == item1 && x.IDTinhTrangLV ==1).Count();
                            if (aa > 0)
                            {
                                int? mvt = ToNullableInt(item.MaViTri);
                                db.Nhanvien_update_IDKNL(item1, mvt);
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
            return RedirectToAction("ListNVKNL", "FPosition");
        }

        public ActionResult CopyMapingData()
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            if (!ListQuyen.Contains(CONSTKEY.ED_KNL))
            {
                TempData["msgError"] = "<script>alert('Bạn không có quyền thực hiện chức năng này');</script>";
                return RedirectToAction("", "Home");
            }
            int idpb = MyAuthentication.IDPhongban;
            //var IdquyenKNL = MyAuthentication.IDQuyenKNL;
            var vt = (from a in db.VitriKNL_search()
                      select new ViTriKNLValidation
                      {
                          IDVT = a.IDVT,
                          TenViTri = a.TenViTri + "-" + a.MaPB + "-" + a.TenPX + "-" + a.TenNhom + "-" + a.TenTo,
                          IDNhom = a.IDNhom,
                          IDTo = a.IDTo,
                          MaViTri = a.MaViTri,
                          IDPX = a.IDPX,
                          IDPB = a.IDPB,
                      }).ToList();

            List<PhongBan> dt = db.PhongBans.ToList();
            if (ListQuyen.Contains(CONSTKEY.V_BP))
            {
                dt = dt.Where(x => x.IDPhongBan == idpb).ToList();
                vt = vt.Where(x => x.IDPB == idpb).ToList();
            }

            ViewBag.IDKNL = new SelectList(vt, "IDVT", "TenViTri");
            ViewBag.Selected = new SelectList(vt, "IDVT", "TenViTri");

            ViewBag.IDPB = new SelectList(dt, "IDPhongBan", "TenPhongBan");
            ViewBag.IDDS = new SelectList(dt, "IDPhongBan", "TenPhongBan");

            ViewBag.IDPB = new SelectList(dt, "IDPhongBan", "TenPhongBan");
            ViewBag.IDPB2 = new SelectList(dt, "IDPhongBan", "TenPhongBan");

            List<KNL_PhanXuong> px = db.KNL_PhanXuong.ToList();
            ViewBag.IDPX1 = new SelectList(px, "ID", "TenPX");
            ViewBag.IDPX2 = new SelectList(px, "ID", "TenPX");

            List<KNL_Nhom> nhom = db.KNL_Nhom.ToList();
            ViewBag.IDNhom1 = new SelectList(nhom, "IDNhom", "TenNhom");
            ViewBag.IDNhom2 = new SelectList(nhom, "IDNhom", "TenNhom");

            List<KNL_To> to = db.KNL_To.ToList();
            ViewBag.IDTo1 = new SelectList(to, "IDTo", "TenTo");
            ViewBag.IDTo2 = new SelectList(to, "IDTo", "TenTo");


            return PartialView();
        }

        [HttpPost]
        public ActionResult CopyMapingData(DSachDGValidation _DO, FormCollection collection)
        {
            try
            {

                if (!String.IsNullOrEmpty(_DO.ViTriNguon) && !String.IsNullOrEmpty(_DO.ViTriDen))
                {

                    //Regex.Replace(_DO.NVDG, @"[^0-9a-zA-Z]+", " ");
                    string tx = Regex.Replace(_DO.ViTriNguon, @"[^0-9a-zA-Z]+", " ");
                    string txd = Regex.Replace(_DO.ViTriDen, @"[^0-9a-zA-Z]+", " ");
                    string[] VTN = tx.Split(new char[] { ' ' });
                    string[] VTD = txd.Split(new char[] { ' ' });
                    int k = 0;
                    foreach (var item in VTD)
                    {
                        int? IDN = ToNullableInt(VTN[k]);

                        var lsKNL = db.KhungNangLucs.Where(x => x.IDVT == IDN).ToList();
                        var lsLoaiKNL = db.LoaiKNLs.Where(x => x.IDVT == IDN).ToList();

                        int? i = ToNullableInt(item);
                        if(i != null)
                        {
                            var vtN = db.ViTriKNLs.Where(x => x.IDVT == i).FirstOrDefault();
                            var vtLoaiN = db.LoaiKNLs.Where(x => x.IDVT == i).ToList();
                            if (i != null)
                            {
                                db.KhungNangLuc_delete_VT(i);
                                db.LoaiKNL_delete_VT(i);
                            }

                            if (lsLoaiKNL.Count > 0)
                            {
                                foreach (var a in lsLoaiKNL)
                                {
                                    db.LoaiKNL_insert(a.TenLoai, i, a.OrderBy);
                                }
                            }
                            if (lsKNL.Count > 0)
                            {
                                foreach (var data in lsKNL)
                                {
                                    if (data.IDLoaiNL != 1 && data.IDLoaiNL != 2)
                                    {
                                        var a = db.LoaiKNLs.Where(x => x.IDLoai == data.IDLoaiNL).FirstOrDefault();
                                        int? b = GetIDLoaiNL(a.TenLoai, i);
                                        db.KhungNangLuc_insert(data.TenNL, b, i, vtN.IDPB, data.DinhMuc, data.IsDanhGia, data.OrderBy,null);
                                    }
                                    else
                                    {
                                        db.KhungNangLuc_insert(data.TenNL, data.IDLoaiNL, i, vtN.IDPB, data.DinhMuc, data.IsDanhGia, data.OrderBy,null);
                                    }

                                }
                            }
                        }
                        k++;
                    }
                }

                if (_DO.IDKNL != null && _DO.Selected != null)
                {
                    var lsKNL = db.KhungNangLucs.Where(x => x.IDVT == _DO.IDKNL).ToList();
                    var lsLoaiKNL = db.LoaiKNLs.Where(x => x.IDVT == _DO.IDKNL).ToList();
                    foreach (var item in _DO.Selected)
                    {
                        int? i = ToNullableInt(item);
                        var vtN = db.ViTriKNLs.Where(x => x.IDVT == i).FirstOrDefault();
                        var vtLoaiN = db.LoaiKNLs.Where(x => x.IDVT == i).ToList();
                        if (i != null)
                        {
                            db.KhungNangLuc_delete_VT(i);
                            db.LoaiKNL_delete_VT(i);
                        }

                        if (lsLoaiKNL.Count > 0)
                        {
                            foreach (var a in lsLoaiKNL)
                            {
                                db.LoaiKNL_insert(a.TenLoai, i, a.OrderBy);
                            }
                        }
                        if (lsKNL.Count > 0)
                        {
                            foreach (var data in lsKNL)
                            {
                                if (data.IDLoaiNL != 1 && data.IDLoaiNL != 2)
                                {
                                    var a = db.LoaiKNLs.Where(x => x.IDLoai == data.IDLoaiNL).FirstOrDefault();
                                    int? b = GetIDLoaiNL(a.TenLoai, i);
                                    db.KhungNangLuc_insert(data.TenNL, b, i, vtN.IDPB, data.DinhMuc, data.IsDanhGia, data.OrderBy,null);
                                }
                                else
                                {
                                    db.KhungNangLuc_insert(data.TenNL, data.IDLoaiNL, i, vtN.IDPB, data.DinhMuc, data.IsDanhGia, data.OrderBy,null);
                                }

                            }
                        }
                    }
                }

                int? pb1 = ToNullableInt(collection["IDPB"]);
                int? px1 = ToNullableInt(collection["IDPX1"]);
                int? nhom1 = ToNullableInt(collection["IDNhom1"]);
                int? to1 = ToNullableInt(collection["IDTo1"]);
                int? pb2 = ToNullableInt(collection["IDDS"]);
                int? px2 = ToNullableInt(collection["IDPX2"]);

                if (pb1 != null && pb2 != null && px1 != null){
                    var px = db.KNL_PhanXuong.Where(x => x.ID == px1).FirstOrDefault();
                   
                    if (nhom1 != null)
                    {
                        var vt = db.ViTriKNLs.Where(x => x.IDNhom == nhom1).ToList();
                        var nhom = db.KNL_Nhom.Where(x => x.IDNhom == nhom1).FirstOrDefault();
                        // set new values
                        nhom.IDPhongBan = pb2;
                        nhom.IDPhanXuong = px2;
                        db.SaveChanges();
                        foreach (var item in vt)
                        {
                            db.VitriKNL_update(item.IDVT,item.TenViTri,item.MaViTri,pb2,item.IDKhoi, px2, item.IDNhom,item.IDTo,item.FilePath);
                        }
                    }
                    if (to1 != null)
                    {
                        var vt = db.ViTriKNLs.Where(x => x.IDTo == to1).ToList();
                        var to = db.KNL_To.Where(x => x.IDTo == to1).FirstOrDefault();
                        // set new values
                        to.IDPhongBan = pb2;
                        to.IDPhanXuong = px2;
                        db.SaveChanges();
                        foreach (var item in vt)
                        {
                            db.VitriKNL_update(item.IDVT, item.TenViTri, item.MaViTri, pb2, item.IDKhoi, px2, item.IDNhom, item.IDTo, item.FilePath);
                        }
                    }
                    if (to1 == null && nhom1 == null)
                    {
                        px.IDPhongBan = pb2;
                        db.SaveChanges();
                        var vt = db.ViTriKNLs.Where(x => x.IDPX == px1).ToList();
                        // update to
                        var lsTo = vt.Where(x => x.IDTo != null && x.IDTo != 0).ToList();
                        foreach (var t in lsTo)
                        {
                            var to = db.KNL_To.Where(x => x.IDTo == t.IDTo).FirstOrDefault();
                            to.IDPhongBan = pb2;
                            db.SaveChanges();
                        }
                        // update nhom
                        var lsNhom = vt.Where(x => x.IDNhom != null && x.IDNhom != 0).ToList();
                        foreach (var t in lsNhom)
                        {
                            var nhom = db.KNL_Nhom.Where(x => x.IDNhom == t.IDNhom).FirstOrDefault();
                            nhom.IDPhongBan = pb2;
                            //t.IDPB = pb2;
                            db.SaveChanges();
                        }
                        db.SaveChanges();
                        foreach (var item in vt)
                        {
                            db.VitriKNL_update(item.IDVT, item.TenViTri, item.MaViTri, pb2, item.IDKhoi, px2, item.IDNhom, item.IDTo, item.FilePath);
                        }
                    }

                }
                else if(pb1 != null && pb2 != null && px1 == null)
                {
                    if (nhom1 != null)
                    {
                        var vt = db.ViTriKNLs.Where(x => x.IDNhom == nhom1).ToList();
                        var nhom = db.KNL_Nhom.Where(x => x.IDNhom == nhom1).FirstOrDefault();
                        // set new values
                        nhom.IDPhongBan = pb2;
                        nhom.IDPhanXuong = px2;
                        db.SaveChanges();
                        foreach (var item in vt)
                        {
                            db.VitriKNL_update(item.IDVT, item.TenViTri, item.MaViTri, pb2, item.IDKhoi, px2, item.IDNhom, item.IDTo, item.FilePath);
                        }
                    }
                    if (to1 != null)
                    {
                        var vt = db.ViTriKNLs.Where(x => x.IDTo == to1).ToList();
                        var to = db.KNL_To.Where(x => x.IDTo == to1).FirstOrDefault();
                        // set new values
                        to.IDPhongBan = pb2;
                        to.IDPhanXuong = px2;
                        db.SaveChanges();
                        foreach (var item in vt)
                        {
                            db.VitriKNL_update(item.IDVT, item.TenViTri, item.MaViTri, pb2, item.IDKhoi, px2, item.IDNhom, item.IDTo, item.FilePath);
                        }
                    }
                }



                TempData["msgSuccess"] = "<script>alert('Copy Dữ liệu thành công');</script>";
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Có lỗi xảy ra: " + e.Message + "');</script>";
            }
            //return View();
            return RedirectToAction("Index", "FPosition");
        }

        [HttpPost]
        public ActionResult CreateVT(string TenVT,string MaVT,int? IDPB, int? IDK, int? IDPX, int? IDNhom, int? IDTo, string FilePath)
        {
            try
            {
                if(TenVT != "") {
                    db.VitriKNL_insert(TenVT, MaVT, IDPB, IDK, IDPX, IDNhom, IDTo, FilePath);
                }
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Thêm mới thất bại');</script>";
            }
            return RedirectToAction("Index", "FPosition");
        }
        public ActionResult UploadFile(int? id)
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            if (!ListQuyen.Contains(CONSTKEY.ED_KNL))
            {
                TempData["msgError"] = "<script>alert('Bạn không có quyền thực hiện chức năng này');</script>";
                return RedirectToAction("", "Home");
            }
            ViewBag.QUYENCN = ListQuyen;
            var res = (from a in db.ViTriKNLs.Where(x => x.IDVT == id)
                       select new ViTriKNLValidation
                       {
                           IDVT = a.IDVT,
                           TenViTri = a.TenViTri,
                           MaViTri = a.MaViTri,
                           FilePath = a.FilePath,
                           IDPB = a.IDPB,
                           IDPX = a.IDPX,
                           IDNhom = a.IDNhom,
                           IDTo = a.IDTo,
                           IDKhoi = a.IDKhoi
                       }).ToList();
            ViTriKNLValidation DO = new ViTriKNLValidation();

            if (res.Count > 0)
            {
                foreach (var c in res)
                {
                    DO.IDVT = c.IDVT;
                    DO.TenViTri = c.TenViTri;
                    DO.MaViTri = c.MaViTri;
                    DO.FilePath = c.FilePath;
                    DO.IDPB = c.IDPB;
                    DO.IDKhoi = c.IDKhoi;
                    DO.IDPX = c.IDPX;
                    DO.IDNhom = c.IDNhom;
                    DO.IDTo = c.IDTo;
                }
            }
            else
            {
                return HttpNotFound();
            }
            return PartialView(DO);
        }

        [HttpPost]
        public ActionResult UploadFile(ViTriKNLValidation _DO,int? IDPB)
        {

            try
            {
                string path = Server.MapPath("~/FileJD/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                //Use Namespace called :  System.IO  
                string FileName = _DO.FileUpload != null ? DateTime.Now.ToString("yyyyMMddHHmmss") + "_"+"File" +_DO.IDVT.ToString() : "";

                //To Get File Extension  
                string FileExtension = _DO.FileUpload != null ? Path.GetExtension(_DO.FileUpload.FileName) : "";
                ////Add Current Date To Attached File Name  
                if (FileExtension != ".pdf")
                {
                    TempData["msgError"] = "<script>alert('Vui lòng chọn đúng định dạng file PDF');</script>";
                    //return View();
                }
                else
                {
                    if (_DO.FileUpload != null)
                    {
                        FileName = FileName.Trim() + FileExtension;
                        _DO.FileUpload.SaveAs(path + FileName);
                        _DO.FilePath = "~/FileJD/" + FileName;
                    }
                    var a = db.VitriKNL_update_file(_DO.IDVT, _DO.FilePath);
                    TempData["msgSuccess"] = "<script>alert('Thêm mới thành công');</script>";
                }
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Có lỗi khi thêm mới: " + e.Message + "');</script>";
            }

            return RedirectToAction("Index", "FPosition", new { IDPB = _DO.IDPB, IDKhoi = _DO.IDKhoi, IDPX = _DO.IDPX, IDTo = _DO.IDTo, IDNhom = _DO.IDNhom });
        }

        public JsonResult AddVT(string TenVT, string MaVT, int? IDPB, int? IDK, int? IDPX, int? IDNhom, int? IDTo, string FilePath)
        {
            if (TenVT != "")
            {
                db.VitriKNL_insert(TenVT, MaVT, IDPB, IDK, IDPX, IDNhom, IDTo, FilePath);
            }
            //RedirectToAction("Index", "ViTriKNL");
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateKNLDefault()
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            ViewBag.QUYENCN = ListQuyen;
            if (!ListQuyen.Contains(CONSTKEY.DE_KNL))
            {
                TempData["msgError"] = "<script>alert('Bạn không có quyền thực hiện chức năng này');</script>";
                return RedirectToAction("", "Home");
            }
            var res = (from a in db.KhungNangLucs.Where(x => x.IDPB == 0)
                       join b in db.LoaiKNLs
                        on a.IDLoaiNL equals b.IDLoai
                       select new KhungNangLucValidation
                       {
                           IDNL = a.IDNL,
                           TenNL = a.TenNL,
                           IDLoaiNL = a.IDLoaiNL,
                           TenLoaiNL = b.TenLoai,
                           IDVT = a.IDVT,
                           DinhMuc = a.DinhMuc,
                       }).ToList();
            KhungNangLucValidation DO = new KhungNangLucValidation();

            //if (res.Count > 0)
            //{
            foreach (var c in res)
            {
                DO.IDVT = c.IDVT;
                DO.IDPB = c.IDPB;
                DO.TenViTri = c.TenViTri;
                DO.TenPhongBan = c.TenPhongBan;
                DO.TenNL = c.TenNL;
                DO.IsDanhGia = c.IsDanhGia;
                DO.DanhGia = c.DanhGia;
                DO.DinhMuc = c.DinhMuc;

            }
            //int idpb = MyAuthentication.IDPhongban;
            db.Configuration.ProxyCreationEnabled = false;

            List<LoaiKNL> loaiNL = db.LoaiKNLs.Where(x => x.IDLoai == 1 || x.IDLoai == 2).ToList();
            ViewBag.IDLoaiNL = new SelectList(loaiNL, "IDLoai", "TenLoai");

            //}
            //else
            //{
            //    return HttpNotFound();
            //}
            return View(res.ToList());
        }
        [HttpPost]
        public ActionResult CreateKNLDefault(FormCollection collection, List<KhungNangLucValidation> ListKNL)
        {

            try
            {

                var pb = ToNullableInt(collection["IDPB"]);
                var vt = ToNullableInt(collection["IDVT"]);
                var Item = new List<KeyValuePair<string, string>>();
                var ListChung = new List<AddNL>();
                var ListQly = new List<AddNL>();
                var ListLoaiUpdate = new List<AddNL>();
                var ListCmonNew = new List<AddNL>();

                foreach (var key in collection.AllKeys)
                {

                    Item.Add(new KeyValuePair<string, string>(key, collection[key]));
                    if (key.Split('_')[0] == "nlchung")
                    {
                        ListChung.Add(new AddNL() { NLuc = collection[key], DinhMuc = null, IsDanhGia = null });
                    }
                    if (key.Split('_')[0] == "nlqly")
                    {
                        ListQly.Add(new AddNL() { NLuc = collection[key], DinhMuc = null, IsDanhGia = null });
                    }

                }
                //var addChung = Item.Where(x => x.Key.Split('_')[0] == "nlchung").ToList();
                if (ListKNL?.Count > 0 && ListKNL != null)
                {

                    foreach (var K in ListKNL)
                    {
                        if (K.IDNL != 0)
                        {
                            var a = db.KhungNangLuc_update(K.IDNL, K.TenNL, K.IDLoaiNL, null, 0, null, null,null,null);
                        }

                    }
                }


                foreach (var item in ListChung)
                {
                    if (item.NLuc != null)
                    {
                        var aa = db.KhungNangLuc_insert(item.NLuc, 1, null, 0, null, null,1,null);
                    }
                }

                foreach (var item in ListQly)
                {
                    if (item.NLuc != null)
                    {
                        var aa = db.KhungNangLuc_insert(item.NLuc, 2, null, 0, null, null,1,null);
                    }
                }


                //db.VitriKNL_insert(_DO.TenViTri,_DO.IDPB);
                TempData["msgSuccess"] = "<script>alert('Cập nhật thành công');</script>";
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Có lỗi khi thêm mới: " + e.Message + "');</script>";
            }
            return RedirectToAction("Index", "FPosition");
        }

        public ActionResult CreateKNL(int id,int? IDPB)
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            ViewBag.QUYENCN = ListQuyen;
            if (!ListQuyen.Contains(CONSTKEY.ED_KNL))
            {
                TempData["msgError"] = "<script>alert('Bạn không có quyền thực hiện chức năng này');</script>";
                return RedirectToAction("", "Home");
            }
            var def = (from a in db.KhungNangLucs.Where(x => x.IDPB == 0 && x.IsDuyet == null)
                       join b in db.LoaiKNLs
                        on a.IDLoaiNL equals b.IDLoai
                       select new KhungNangLucValidation
                       {
                           IDNL = a.IDNL,
                           TenNL = a.TenNL,
                           IDLoaiNL = a.IDLoaiNL,
                           TenLoaiNL = b.TenLoai,
                           IDVT = a.IDVT,
                           TenViTri = "",
                           IDPB = a.IDPB,
                           TenPhongBan = "",
                           DinhMuc = a.DinhMuc,
                           IsDanhGia = a.IsDanhGia,
                           DanhGia = a.IsDanhGia == 1 ? false : true,
                       }).ToList();

            var chqly = db.KhungNangLucs.Where(x => x.IDVT == id && (x.IDLoaiNL == 1 || x.IDLoaiNL == 2) && x.IsDuyet == null).ToList();
            if (chqly.Count == 0)
            {
                foreach (var item in def)
                {
                    if (item.TenNL != null && item.IDLoaiNL == 1)
                    {
                        var aa = db.KhungNangLuc_insert(item.TenNL, 1, id, IDPB, null, 1,1,null);
                    }
                    else if (item.TenNL != null && item.IDLoaiNL == 2)
                    {
                        var aa = db.KhungNangLuc_insert(item.TenNL, 2, id, IDPB, null, 1,1,null);
                    }
                }
            }
            if ((chqly.Count != def.Where(x => x.IDLoaiNL == 1 || x.IDLoaiNL == 2).Count()) && chqly.Count != 0)
            {
                foreach (var item in chqly)
                {
                    //db.KhungNangLuc_delete(item.IDNL);
                    item.IsDuyet = 2;// năng lực cũ ẩn
                }
                db.SaveChanges();
                foreach (var item in def)
                {
                    if (item.TenNL != null && item.IDLoaiNL == 1)
                    {
                        var aa = db.KhungNangLuc_insert(item.TenNL, 1, id, IDPB, null, 1, 1, null);
                    }
                    else if (item.TenNL != null && item.IDLoaiNL == 2)
                    {
                        var aa = db.KhungNangLuc_insert(item.TenNL, 2, id, IDPB, null, 1, 1, null);
                    }
                }
            }

            var res = (from a in db.KhungNangLucs.Where(x=>x.IDVT ==id && x.IsDuyet == null)
                      join b in db.LoaiKNLs
                       on a.IDLoaiNL equals b.IDLoai
                      join c in db.ViTriKNLs
                      on a.IDVT equals c.IDVT
                      join d in db.PhongBans
                      on c.IDPB equals d.IDPhongBan
                      select new KhungNangLucValidation
                      {
                          IDNL = a.IDNL,
                          TenNL = a.TenNL,
                          IDLoaiNL = a.IDLoaiNL,
                          TenLoaiNL = b.TenLoai,
                          IDVT = a.IDVT,
                          TenViTri = c.TenViTri,
                          IDPB = a.IDPB,
                          TenPhongBan = d.TenPhongBan,
                          DinhMuc = a.DinhMuc,
                          IsDanhGia = a.IsDanhGia,
                          DanhGia =a.IsDanhGia ==1?false:true,
                          OrderBy = a.OrderBy,
                          OrderByLoai = b.OrderBy
                      }).ToList().OrderBy(x => x.OrderBy);
            KhungNangLucValidation DO = new KhungNangLucValidation();

            //if (res.Count > 0)
            //{
                foreach (var c in res)
                {
                    DO.IDVT = c.IDVT;
                    DO.IDPB = c.IDPB;
                    DO.TenViTri = c.TenViTri;
                    DO.TenPhongBan = c.TenPhongBan;
                    DO.TenNL = c.TenNL;
                    DO.IsDanhGia = c.IsDanhGia;
                    DO.DanhGia = c.DanhGia;
                    DO.DinhMuc = c.DinhMuc;
                    DO.OrderBy = c.OrderBy;
                    DO.OrderByLoai = c.OrderByLoai;
                }
                //int idpb = MyAuthentication.IDPhongban;
                db.Configuration.ProxyCreationEnabled = false;
                List<PhongBan> dt = db.PhongBans.Where(x=>x.IDPhongBan == IDPB).ToList();
                ViewBag.IDPB = new SelectList(dt, "IDPhongBan", "TenPhongBan");

            var vt = (from a in db.ViTriKNLs.Where(x => x.IDVT == id)
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
                          TenViTri = a.TenViTri + "-" + g.TenTo + "-" + f.TenNhom +"-"+ e.TenPX,
                          IDNhom = a.IDNhom,
                          IDTo = a.IDTo,
                          MaViTri = a.MaViTri,
                          IDPX = a.IDPX
                      }).ToList();
           
                //ViewBag.IDKNL = new SelectList(kvt, "IDVT", "TenViTri", id);

                //List<ViTriKNL> vt = db.ViTriKNLs.Where(x=>x.IDVT ==id).ToList();
                ViewBag.IDVT = new SelectList(vt, "IDVT", "TenViTri");
                List<LoaiKNL> loaiNL = db.LoaiKNLs.Where(x => x.IDVT == id && x.IDLoai !=1 && x.IDLoai !=2).OrderBy(x => x.OrderBy).ToList();
                ViewBag.IDLoaiNL = new SelectList(loaiNL, "IDLoai", "TenLoai");
                ViewBag.SLIDVT = id;
            ViewBag.PBIDD = IDPB;


            //}
            //else
            //{
            //    return HttpNotFound();
            //}
            return View(res.ToList());
        }
        [HttpPost]
        public ActionResult CreateKNL(FormCollection collection, List<KhungNangLucValidation> ListKNL)
        {

            try
            {
                
                var pb = ToNullableInt(collection["IDPB"]);
                var vt = ToNullableInt(collection["IDVT"]);
                var Item = new List<KeyValuePair<string, string>>();
                var ListChung = new List<AddNL>();
                var ListQly = new List<AddNL>();
                var ListLoaiUpdate = new List<AddNL>();
                var ListCmonNew = new List<AddNL>();

                List<LoaiKNL> loaiNL = db.LoaiKNLs.Where(x => x.IDVT == vt && x.IDLoai != 1 && x.IDLoai != 2).ToList();
                foreach (var loai in loaiNL)
                {
                    var u = collection["Loai_" + loai.IDLoai];
                    int orDer = ToNullableInt(collection["OrderLoai_" + loai.IDLoai]) ?? 1;
                    var up = db.LoaiKNL_update(loai.IDLoai, u, vt, orDer);
                }
                foreach (var key in collection.AllKeys)
                {

                    Item.Add(new KeyValuePair<string, string>(key, collection[key]));
                    if(key.Split('_')[0] == "nlchung")
                    {
                        ListChung.Add(new AddNL() { NLuc = collection[key], DinhMuc = collection["dmchung_" + key.Split('_')[1]], IsDanhGia = collection["IsDanhGiachung_" + key.Split('_')[1]].Split(',')[0] });
                    }
                    if (key.Split('_')[0] == "nlqly")
                    {
                        ListQly.Add(new AddNL() { NLuc = collection[key], DinhMuc = collection["dmqly_" + key.Split('_')[1]], IsDanhGia = collection["IsDanhGiaqly_" + key.Split('_')[1]].Split(',')[0] });
                    }
                    if (key.Split('_')[0] == "nlLoai")
                    {
                        ListLoaiUpdate.Add(new AddNL() { NLuc = collection[key], DinhMuc = collection["dmLoai_" + key.Split('_')[1]+ "_cmonLoai_"+ key.Split('_')[3]], IsDanhGia = collection["IsDanhGiaLoai_" + key.Split('_')[1] + "_cmonLoai_" + key.Split('_')[3]].Split(',')[0],IDLoai = key.Split('_')[3].ToString() });
                    }
                    if (key.Split('_')[0] == "nlcmon" && key.Split('_')[2] == "cmon")
                    {
                        var aa = collection["loai_cmon_" + key.Split('_')[3]];
                        int idloai = GetIDLoaiNL(aa, vt);
                        if (idloai == 0)
                        {
                            var am = db.LoaiKNL_insert(aa, vt, 1);
                            idloai = GetIDLoaiNL(aa.ToString(), vt);
                            
                        }
                        var dmNL = collection["dmcmon_" + key.Split('_')[1] + "_cmon_" + key.Split('_')[3]];
                        var isdg = collection["IsDanhGiacmon_" + key.Split('_')[1] + "_cmon_" + key.Split('_')[3]].Split(',')[0];

                        ListCmonNew.Add(new AddNL() { NLuc = collection[key], DinhMuc = dmNL, IsDanhGia = isdg, IDLoai = idloai.ToString() });
                    }

                }
                //var addChung = Item.Where(x => x.Key.Split('_')[0] == "nlchung").ToList();
                if (ListKNL?.Count > 0&& ListKNL !=null)
                {
                    
                    foreach (var K in ListKNL)
                    {
                        if(K.IDNL != 0)
                        {
                            var a = db.KhungNangLuc_update(K.IDNL, K.TenNL, K.IDLoaiNL, K.IDVT, K.IDPB, ToNullableInt(K.DinhMuc.ToString()), GetDanhGia(K.DanhGia.ToString()), K.OrderBy,null);
                        }

                    }
                }
               
                
                foreach (var item in ListLoaiUpdate)
                {
                    if (item.NLuc != null && pb != null && vt != null)
                    {
                        var aa = db.KhungNangLuc_insert(item.NLuc, ToNullableInt(item.IDLoai), vt, pb, ToNullableInt(item.DinhMuc), GetDanhGia(item.IsDanhGia),1,null);
                    }
                }

                foreach (var item in ListChung)
                {
                    if (item.NLuc != null && pb != null && vt != null)
                    {
                        var aa = db.KhungNangLuc_insert(item.NLuc, 1, vt, pb, ToNullableInt(item.DinhMuc), GetDanhGia(item.IsDanhGia),1,null);
                    }
                }

                foreach (var item in ListQly)
                {
                    if (item.NLuc != null && pb != null && vt != null)
                    {
                        var aa = db.KhungNangLuc_insert(item.NLuc, 2, vt, pb, ToNullableInt(item.DinhMuc), GetDanhGia(item.IsDanhGia),1,null);
                    }
                }
                foreach (var item in ListCmonNew)
                {
                    if (item.NLuc != null && pb != null && vt != null)
                    {
                        var aa = db.KhungNangLuc_insert(item.NLuc, ToNullableInt(item.IDLoai), vt, pb, ToNullableInt(item.DinhMuc), GetDanhGia(item.IsDanhGia),1,null);
                    }
                }


                //db.VitriKNL_insert(_DO.TenViTri,_DO.IDPB);
                TempData["msgSuccess"] = "<script>alert('Cập nhật thành công');</script>";
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Có lỗi khi thêm mới: " + e.Message + "');</script>";
            }
            return RedirectToAction("CreateKNL", "FPosition", new { id = collection["IDVT"], IDPB = collection["IDPB"] });
        }

        public ActionResult UpdateDuyetKNL(int id, int? IDPB, int? IdDuyet)
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            ViewBag.QUYENCN = ListQuyen;
            //if (!ListQuyen.Contains(CONSTKEY.ED_KNL))
            //{
            //    TempData["msgError"] = "<script>alert('Bạn không có quyền thực hiện chức năng này');</script>";
            //    return RedirectToAction("", "Home");
            //}


            var res = (from a in db.KhungNangLucs.Where(x => x.IDVT == id && x.IsDuyet == 1)
                       join b in db.LoaiKNLs
                        on a.IDLoaiNL equals b.IDLoai
                       join c in db.ViTriKNLs
                       on a.IDVT equals c.IDVT
                       join d in db.PhongBans
                       on a.IDPB equals d.IDPhongBan
                       select new KhungNangLucValidation
                       {
                           IDNL = a.IDNL,
                           TenNL = a.TenNL,
                           IDLoaiNL = a.IDLoaiNL,
                           TenLoaiNL = b.TenLoai,
                           IDVT = a.IDVT,
                           TenViTri = c.TenViTri,
                           IDPB = a.IDPB,
                           TenPhongBan = d.TenPhongBan,
                           DinhMuc = a.DinhMuc,
                           IsDanhGia = a.IsDanhGia,
                           DanhGia = a.IsDanhGia == 1 ? false : true,
                           OrderBy = a.OrderBy,
                           OrderByLoai = b.OrderBy
                       }).ToList().OrderBy(x => x.OrderBy);
            KhungNangLucValidation DO = new KhungNangLucValidation();

            //if (res.Count > 0)
            //{
            foreach (var c in res)
            {
                DO.IDVT = c.IDVT;
                DO.IDPB = c.IDPB;
                DO.TenViTri = c.TenViTri;
                DO.TenPhongBan = c.TenPhongBan;
                DO.TenNL = c.TenNL;
                DO.IsDanhGia = c.IsDanhGia;
                DO.DanhGia = c.DanhGia;
                DO.DinhMuc = c.DinhMuc;
                DO.OrderBy = c.OrderBy;
                DO.OrderByLoai = c.OrderByLoai;
            }
            //int idpb = MyAuthentication.IDPhongban;
            db.Configuration.ProxyCreationEnabled = false;
            List<PhongBan> dt = db.PhongBans.Where(x => x.IDPhongBan == IDPB).ToList();
            ViewBag.IDPB = new SelectList(dt, "IDPhongBan", "TenPhongBan");

            var vt = (from a in db.ViTriKNLs.Where(x => x.IDVT == id)
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
                          TenViTri = a.TenViTri + "-" + g.TenTo + "-" + f.TenNhom + "-" + e.TenPX,
                          IDNhom = a.IDNhom,
                          IDTo = a.IDTo,
                          MaViTri = a.MaViTri,
                          IDPX = a.IDPX
                      }).ToList();

            //ViewBag.IDKNL = new SelectList(kvt, "IDVT", "TenViTri", id);

            //List<ViTriKNL> vt = db.ViTriKNLs.Where(x=>x.IDVT ==id).ToList();
            ViewBag.IDVT = new SelectList(vt, "IDVT", "TenViTri");
            List<LoaiKNL> loaiNL = db.LoaiKNLs.Where(x => x.IDVT == id && x.IDLoai != 1 && x.IDLoai != 2).OrderBy(x => x.OrderBy).ToList();
            ViewBag.IDLoaiNL = new SelectList(loaiNL, "IDLoai", "TenLoai");
            ViewBag.SLIDVT = id;
            ViewBag.PBIDD = IDPB;
            ViewBag.IDDuyet = IdDuyet;

            //}
            //else
            //{
            //    return HttpNotFound();
            //}
            return View(res.ToList());
        }
        [HttpPost]
        public ActionResult UpdateDuyetKNL(FormCollection collection, List<KhungNangLucValidation> ListKNL)
        {

            try
            {

                var pb = ToNullableInt(collection["IDPB"]);
                var vt = ToNullableInt(collection["IDVT"]);
                var idduyet = ToNullableInt(collection["IDDuyet"]);
                var Item = new List<KeyValuePair<string, string>>();
                var ListChung = new List<AddNL>();
                var ListQly = new List<AddNL>();
                var ListLoaiUpdate = new List<AddNL>();
                var ListCmonNew = new List<AddNL>();

                List<LoaiKNL> loaiNL = db.LoaiKNLs.Where(x => x.IDVT == vt && x.IDLoai != 1 && x.IDLoai != 2).ToList();
                foreach (var loai in loaiNL)
                {
                    var u = collection["Loai_" + loai.IDLoai];
                    int orDer = ToNullableInt(collection["OrderLoai_" + loai.IDLoai]) ?? 1;
                    var up = db.LoaiKNL_update(loai.IDLoai, u, vt, orDer);
                }
                foreach (var key in collection.AllKeys)
                {

                    Item.Add(new KeyValuePair<string, string>(key, collection[key]));
                    if (key.Split('_')[0] == "nlchung")
                    {
                        ListChung.Add(new AddNL() { NLuc = collection[key], DinhMuc = collection["dmchung_" + key.Split('_')[1]], IsDanhGia = collection["IsDanhGiachung_" + key.Split('_')[1]].Split(',')[0] });
                    }
                    if (key.Split('_')[0] == "nlqly")
                    {
                        ListQly.Add(new AddNL() { NLuc = collection[key], DinhMuc = collection["dmqly_" + key.Split('_')[1]], IsDanhGia = collection["IsDanhGiaqly_" + key.Split('_')[1]].Split(',')[0] });
                    }
                    if (key.Split('_')[0] == "nlLoai")
                    {
                        ListLoaiUpdate.Add(new AddNL() { NLuc = collection[key], DinhMuc = collection["dmLoai_" + key.Split('_')[1] + "_cmonLoai_" + key.Split('_')[3]], IsDanhGia = collection["IsDanhGiaLoai_" + key.Split('_')[1] + "_cmonLoai_" + key.Split('_')[3]].Split(',')[0], IDLoai = key.Split('_')[3].ToString() });
                    }
                    if (key.Split('_')[0] == "nlcmon" && key.Split('_')[2] == "cmon")
                    {
                        var aa = collection["loai_cmon_" + key.Split('_')[3]];
                        int idloai = GetIDLoaiNL(aa, vt);
                        if (idloai == 0)
                        {
                            var am = db.LoaiKNL_insert(aa, vt, 1);
                            idloai = GetIDLoaiNL(aa.ToString(), vt);

                        }
                        var dmNL = collection["dmcmon_" + key.Split('_')[1] + "_cmon_" + key.Split('_')[3]];
                        var isdg = collection["IsDanhGiacmon_" + key.Split('_')[1] + "_cmon_" + key.Split('_')[3]].Split(',')[0];

                        ListCmonNew.Add(new AddNL() { NLuc = collection[key], DinhMuc = dmNL, IsDanhGia = isdg, IDLoai = idloai.ToString() });
                    }

                }
                //var addChung = Item.Where(x => x.Key.Split('_')[0] == "nlchung").ToList();
                if (ListKNL?.Count > 0 && ListKNL != null)
                {

                    foreach (var K in ListKNL)
                    {
                        if (K.IDNL != 0)
                        {
                            var a = db.KhungNangLuc_update(K.IDNL, K.TenNL, K.IDLoaiNL, K.IDVT, K.IDPB, ToNullableInt(K.DinhMuc.ToString()), GetDanhGia(K.DanhGia.ToString()), K.OrderBy, 1);
                        }

                    }
                }


                foreach (var item in ListLoaiUpdate)
                {
                    if (item.NLuc != null && pb != null && vt != null)
                    {
                        var aa = db.KhungNangLuc_insert(item.NLuc, ToNullableInt(item.IDLoai), vt, pb, ToNullableInt(item.DinhMuc), GetDanhGia(item.IsDanhGia), 1, 1);
                    }
                }

                foreach (var item in ListChung)
                {
                    if (item.NLuc != null && pb != null && vt != null)
                    {
                        var aa = db.KhungNangLuc_insert(item.NLuc, 1, vt, pb, ToNullableInt(item.DinhMuc), GetDanhGia(item.IsDanhGia), 1, 1);
                    }
                }

                foreach (var item in ListQly)
                {
                    if (item.NLuc != null && pb != null && vt != null)
                    {
                        var aa = db.KhungNangLuc_insert(item.NLuc, 2, vt, pb, ToNullableInt(item.DinhMuc), GetDanhGia(item.IsDanhGia), 1, 1);
                    }
                }
                foreach (var item in ListCmonNew)
                {
                    if (item.NLuc != null && pb != null && vt != null)
                    {
                        var aa = db.KhungNangLuc_insert(item.NLuc, ToNullableInt(item.IDLoai), vt, pb, ToNullableInt(item.DinhMuc), GetDanhGia(item.IsDanhGia), 1, 1);
                    }
                }

                // luu lại file pdf
                KNL_PheDuyetKNL pheduyet = db.KNL_PheDuyetKNL.Where(x => x.ID == idduyet).FirstOrDefault();
                string filepath = ExportViewToPdf(pheduyet.ID, vt);
                if (filepath != null)
                {
                    pheduyet.File_KNL = filepath;
                }
                db.SaveChanges();

                //db.VitriKNL_insert(_DO.TenViTri,_DO.IDPB);
                TempData["msgSuccess"] = "<script>alert('Cập nhật thành công');</script>";
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Có lỗi khi thêm mới: " + e.Message + "');</script>";
            }
            return RedirectToAction("UpdateDuyetKNL", "FPosition", new { id = collection["IDVT"], IDPB = collection["IDPB"], IdDuyet = collection["IDDuyet"] });
        }

        public ActionResult UpdateBangKNL(int id, int? IDPB)
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            ViewBag.QUYENCN = ListQuyen;
            //if (!ListQuyen.Contains(CONSTKEY.ED_KNL))
            //{
            //    TempData["msgError"] = "<script>alert('Bạn không có quyền thực hiện chức năng này');</script>";
            //    return RedirectToAction("", "Home");
            //}

            var res = (from a in db.KhungNangLucs.Where(x => x.IDVT == id && x.IsDuyet == 0)
                       join b in db.LoaiKNLs
                        on a.IDLoaiNL equals b.IDLoai
                       join c in db.ViTriKNLs
                       on a.IDVT equals c.IDVT
                       join d in db.PhongBans
                       on a.IDPB equals d.IDPhongBan
                       select new KhungNangLucValidation
                       {
                           IDNL = a.IDNL,
                           TenNL = a.TenNL,
                           IDLoaiNL = a.IDLoaiNL,
                           TenLoaiNL = b.TenLoai,
                           IDVT = a.IDVT,
                           TenViTri = c.TenViTri,
                           IDPB = a.IDPB,
                           TenPhongBan = d.TenPhongBan,
                           DinhMuc = a.DinhMuc,
                           IsDanhGia = a.IsDanhGia,
                           DanhGia = a.IsDanhGia == 1 ? false : true,
                           OrderBy = a.OrderBy,
                           OrderByLoai = b.OrderBy
                       }).ToList().OrderBy(x => x.OrderBy);
            KhungNangLucValidation DO = new KhungNangLucValidation();

            //if (res.Count > 0)
            //{
            foreach (var c in res)
            {
                DO.IDVT = c.IDVT;
                DO.IDPB = c.IDPB;
                DO.TenViTri = c.TenViTri;
                DO.TenPhongBan = c.TenPhongBan;
                DO.TenNL = c.TenNL;
                DO.IsDanhGia = c.IsDanhGia;
                DO.DanhGia = c.DanhGia;
                DO.DinhMuc = c.DinhMuc;
                DO.OrderBy = c.OrderBy;
                DO.OrderByLoai = c.OrderByLoai;
            }
            //int idpb = MyAuthentication.IDPhongban;
            db.Configuration.ProxyCreationEnabled = false;
            List<PhongBan> dt = db.PhongBans.Where(x => x.IDPhongBan == IDPB).ToList();
            ViewBag.IDPB = new SelectList(dt, "IDPhongBan", "TenPhongBan");

            var vt = (from a in db.ViTriKNLs.Where(x => x.IDVT == id)
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
                          TenViTri = a.TenViTri + "-" + g.TenTo + "-" + f.TenNhom + "-" + e.TenPX,
                          IDNhom = a.IDNhom,
                          IDTo = a.IDTo,
                          MaViTri = a.MaViTri,
                          IDPX = a.IDPX
                      }).ToList();

            //ViewBag.IDKNL = new SelectList(kvt, "IDVT", "TenViTri", id);

            //List<ViTriKNL> vt = db.ViTriKNLs.Where(x=>x.IDVT ==id).ToList();
            ViewBag.IDVT = new SelectList(vt, "IDVT", "TenViTri");
            List<LoaiKNL> loaiNL = db.LoaiKNLs.Where(x => x.IDVT == id && x.IDLoai != 1 && x.IDLoai != 2).OrderBy(x => x.OrderBy).ToList();
            ViewBag.IDLoaiNL = new SelectList(loaiNL, "IDLoai", "TenLoai");
            ViewBag.SLIDVT = id;
            ViewBag.PBIDD = IDPB;


            //}
            //else
            //{
            //    return HttpNotFound();
            //}
            return View(res.ToList());
        }
        [HttpPost]
        public ActionResult UpdateBangKNL(FormCollection collection, List<KhungNangLucValidation> ListKNL)
        {

            try
            {

                var pb = ToNullableInt(collection["IDPB"]);
                var vt = ToNullableInt(collection["IDVT"]);
                var Item = new List<KeyValuePair<string, string>>();
                var ListChung = new List<AddNL>();
                var ListQly = new List<AddNL>();
                var ListLoaiUpdate = new List<AddNL>();
                var ListCmonNew = new List<AddNL>();

                List<LoaiKNL> loaiNL = db.LoaiKNLs.Where(x => x.IDVT == vt && x.IDLoai != 1 && x.IDLoai != 2).ToList();
                foreach (var loai in loaiNL)
                {
                    var u = collection["Loai_" + loai.IDLoai];
                    int orDer = ToNullableInt(collection["OrderLoai_" + loai.IDLoai]) ?? 1;
                    var up = db.LoaiKNL_update(loai.IDLoai, u, vt, orDer);
                }
                foreach (var key in collection.AllKeys)
                {

                    Item.Add(new KeyValuePair<string, string>(key, collection[key]));
                    if (key.Split('_')[0] == "nlchung")
                    {
                        ListChung.Add(new AddNL() { NLuc = collection[key], DinhMuc = collection["dmchung_" + key.Split('_')[1]], IsDanhGia = collection["IsDanhGiachung_" + key.Split('_')[1]].Split(',')[0] });
                    }
                    if (key.Split('_')[0] == "nlqly")
                    {
                        ListQly.Add(new AddNL() { NLuc = collection[key], DinhMuc = collection["dmqly_" + key.Split('_')[1]], IsDanhGia = collection["IsDanhGiaqly_" + key.Split('_')[1]].Split(',')[0] });
                    }
                    if (key.Split('_')[0] == "nlLoai")
                    {
                        ListLoaiUpdate.Add(new AddNL() { NLuc = collection[key], DinhMuc = collection["dmLoai_" + key.Split('_')[1] + "_cmonLoai_" + key.Split('_')[3]], IsDanhGia = collection["IsDanhGiaLoai_" + key.Split('_')[1] + "_cmonLoai_" + key.Split('_')[3]].Split(',')[0], IDLoai = key.Split('_')[3].ToString() });
                    }
                    if (key.Split('_')[0] == "nlcmon" && key.Split('_')[2] == "cmon")
                    {
                        var aa = collection["loai_cmon_" + key.Split('_')[3]];
                        int idloai = GetIDLoaiNL(aa, vt);
                        if (idloai == 0)
                        {
                            var am = db.LoaiKNL_insert(aa, vt, 1);
                            idloai = GetIDLoaiNL(aa.ToString(), vt);

                        }
                        var dmNL = collection["dmcmon_" + key.Split('_')[1] + "_cmon_" + key.Split('_')[3]];
                        var isdg = collection["IsDanhGiacmon_" + key.Split('_')[1] + "_cmon_" + key.Split('_')[3]].Split(',')[0];

                        ListCmonNew.Add(new AddNL() { NLuc = collection[key], DinhMuc = dmNL, IsDanhGia = isdg, IDLoai = idloai.ToString() });
                    }

                }
                //var addChung = Item.Where(x => x.Key.Split('_')[0] == "nlchung").ToList();
                if (ListKNL?.Count > 0 && ListKNL != null)
                {

                    foreach (var K in ListKNL)
                    {
                        if (K.IDNL != 0)
                        {
                            var a = db.KhungNangLuc_update(K.IDNL, K.TenNL, K.IDLoaiNL, K.IDVT, K.IDPB, ToNullableInt(K.DinhMuc.ToString()), GetDanhGia(K.DanhGia.ToString()), K.OrderBy, 0);
                        }

                    }
                }


                foreach (var item in ListLoaiUpdate)
                {
                    if (item.NLuc != null && pb != null && vt != null)
                    {
                        var aa = db.KhungNangLuc_insert(item.NLuc, ToNullableInt(item.IDLoai), vt, pb, ToNullableInt(item.DinhMuc), GetDanhGia(item.IsDanhGia), 1, 0);
                    }
                }

                foreach (var item in ListChung)
                {
                    if (item.NLuc != null && pb != null && vt != null)
                    {
                        var aa = db.KhungNangLuc_insert(item.NLuc, 1, vt, pb, ToNullableInt(item.DinhMuc), GetDanhGia(item.IsDanhGia), 1, 0);
                    }
                }

                foreach (var item in ListQly)
                {
                    if (item.NLuc != null && pb != null && vt != null)
                    {
                        var aa = db.KhungNangLuc_insert(item.NLuc, 2, vt, pb, ToNullableInt(item.DinhMuc), GetDanhGia(item.IsDanhGia), 1, 0);
                    }
                }
                foreach (var item in ListCmonNew)
                {
                    if (item.NLuc != null && pb != null && vt != null)
                    {
                        var aa = db.KhungNangLuc_insert(item.NLuc, ToNullableInt(item.IDLoai), vt, pb, ToNullableInt(item.DinhMuc), GetDanhGia(item.IsDanhGia), 1, 0);
                    }
                }


                //db.VitriKNL_insert(_DO.TenViTri,_DO.IDPB);
                TempData["msgSuccess"] = "<script>alert('Cập nhật thành công');</script>";
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Có lỗi khi thêm mới: " + e.Message + "');</script>";
            }
            return RedirectToAction("UpdateBangKNL", "FPosition", new { id = collection["IDVT"], IDPB = collection["IDPB"] });
        }
        public ActionResult CreateVTKNL(int? IDPB)
        {
            int idpb = MyAuthentication.IDPhongban;
            var Idquyen = MyAuthentication.IDQuyen;
            var res = (from a in db.ViTriKNLs
                       join d in db.PhongBans
                       on a.IDPB equals d.IDPhongBan
                       select new ViTriKNLValidation
                       {
                           IDVT =a.IDVT,
                           IDPB =a.IDPB,
                           MaViTri =a.MaViTri,
                           TenPhongBan =d.TenPhongBan,
                           TenViTri=a.TenViTri
                       }).ToList();

            //if (IDPB == null) IDPB = 0;
            //if (IDPB != 0) res = res.Where(x => x.IDPB == IDPB).ToList();
            //else res = res.Where(x => x.IDPB == idpb).ToList();

            //var dsquyen = db.QuyenKNLs.Where(x => x.MaNV == MyAuthentication.Username && x.isDanhGia == 1).ToList();

            //var cb = dsquyen.Count > 0 ? dsquyen.FirstOrDefault().IDCB : 0;
            //var isAll = dsquyen.Count > 0 ? dsquyen.FirstOrDefault().isAll : 0;

            db.Configuration.ProxyCreationEnabled = false;
            //int? idns = GetIDPNS("P. Nhân sự");
            //if (idns != idpb && Idquyen != 6 && isAll != 1)
            //{
            //    List<PhongBan> dt = db.PhongBans.Where(x => x.IDPhongBan == idpb).ToList();
            //    ViewBag.IDPB = new SelectList(dt, "IDPhongBan", "TenPhongBan", idpb);
            //}
            //else
            //{
            //    List<PhongBan> dt = db.PhongBans.ToList();
            //    ViewBag.IDPB = new SelectList(dt, "IDPhongBan", "TenPhongBan",idpb);
            //}
           
            //if (Idquyen == 6 || (Idquyen == 5 && idns == idpb) || idpb == 3 || (Idquyen == 1 && idns == idpb) || isAll == 1)
            //{
            //    if (IDPB == null) IDPB = 0;
            //    if (IDPB != 0) res = res.Where(x => x.IDPB == IDPB).ToList();
            //    else res = res.ToList();
            //    //res1 = res1.ToList();
            //}
            //else if ((Idquyen == 5 && idns != idpb) || cb == 2 || cb == 3 || (Idquyen == 1 && idns != idpb))
            //{
            //    res = res.Where(x => x.IDPB == idpb).ToList();
            //}
            //else if (dsquyen.Count > 0)
            //{
            //    var res1 = new List<ViTriKNLValidation>();
            //    foreach (var item in aa)
            //    {
            //        var kk = res.Where(x => x.IDVT == item).FirstOrDefault();
            //        if (kk != null)
            //        {
            //            res1.Add(kk);
            //        }
            //    }
            //    res = res1;
            //}
            //else res = new List<ViTriKNLValidation>();

            //List<ViTriKNL> vt = db.ViTriKNLs.Where(x => x.IDVT == id).ToList();
            //ViewBag.IDVT = new SelectList(vt, "IDVT", "TenViTri");
            //List<LoaiKNL> loaiNL = db.LoaiKNLs.Where(x => x.IDVT == id && x.IDLoai != 1 && x.IDLoai != 2).ToList();
            //ViewBag.IDLoaiNL = new SelectList(loaiNL, "IDLoai", "TenLoai");
            //ViewBag.SLIDVT = id;

            //}
            //else
            //{
            //    return HttpNotFound();
            //}
            return View(res.ToList());
        }

        [HttpPost]
        public ActionResult CreateVTKNL(FormCollection collection, List<ViTriKNLValidation> ListVTKNL)
        {

            try
            {

                var pb = ToNullableInt(collection["IDPB"]);
                var Item = new List<KeyValuePair<string, string>>();
                var ListVT = new List<ViTriKNLValidation>();

                foreach (var key in collection.AllKeys)
                {

                    Item.Add(new KeyValuePair<string, string>(key, collection[key]));
                    if (key.Split('_')[0] == "tenvt")
                    {
                        ListVT.Add(new ViTriKNLValidation() { TenViTri = collection[key], MaViTri = collection["mavt_" + key.Split('_')[1]]});
                    }

                }
                if (ListVTKNL?.Count > 0 && ListVTKNL != null)
                {

                    foreach (var K in ListVTKNL)
                    {
                        if (K.IDVT != null || K.IDVT != 0)
                        {
                            //var a = db.VitriKNL_update(K.IDVT, K.TenViTri, K.MaViTri, K.IDPB);
                        }

                    }
                }

                foreach (var item in ListVT)
                {
                    if (item.TenViTri != null && pb != null )
                    {
                        //var aa = db.VitriKNL_insert(item.TenViTri, item.MaViTri, pb);
                    }
                }
                

                //db.VitriKNL_insert(_DO.TenViTri,_DO.IDPB);
                TempData["msgSuccess"] = "<script>alert('Thêm mới thành công');</script>";
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Có lỗi khi thêm mới: " + e.Message + "');</script>";
            }
            return RedirectToAction("Index", "FPosition");
        }

        public ActionResult DeleteVTKNL(int id)
        {
            try
            {
                db.KhungNangLuc_delete(id);
            }
            catch (Exception e)
            {
                TempData["msgSuccess"] = "<script>alert('Xóa dữ liệu thất bại: " + e.Message + "');</script>";
            }
            return RedirectToAction("CreateVTKNL", "FPosition");
        }
        public ActionResult DeleteKNLDefault(int id, int? IDVT)
        {
            try
            {
                db.KhungNangLuc_delete(id);
            }
            catch (Exception e)
            {
                TempData["msgSuccess"] = "<script>alert('Xóa dữ liệu thất bại: " + e.Message + "');</script>";
            }
            return RedirectToAction("CreateKNLDefault", "FPosition");
        }
        public ActionResult DeleteKNL(int id,int? IDVT,int? IDPB)
        {
            try
            {
                // ẩn khung năng lực
                var knl = db.KhungNangLucs.FirstOrDefault(x => x.IDNL == id);
                knl.IsDuyet = 2; //năng lực cũ
                db.SaveChanges();
                //db.KhungNangLuc_delete(id);
            }
            catch (Exception e)
            {
                TempData["msgSuccess"] = "<script>alert('Xóa dữ liệu thất bại: " + e.Message + "');</script>";
            }
            return RedirectToAction("CreateKNL", "FPosition", new { id =IDVT, IDPB = IDPB });
        }

        public ActionResult DeleteLoaiKNL(int id, int? IDVT, int? IDPB)
        {
            try
            {
                var loaiKNL = db.LoaiKNLs.FirstOrDefault(x=>x.IDLoai ==id);
                loaiKNL.IDVT = null;
                db.SaveChanges();

                var aLi = db.KhungNangLucs.Where(x => x.IDVT == IDVT && x.IDLoaiNL == id).ToList();
                if (aLi.Count > 0)
                {
                    foreach (var item in aLi)
                    {
                        //db.KhungNangLuc_delete(item.IDNL);
                        item.IsDuyet = 2; //năng lực cũ
                    }
                    db.SaveChanges();
                }
                //db.LoaiKNL_delete(id);

            }
            catch (Exception e)
            {
                TempData["msgSuccess"] = "<script>alert('Xóa dữ liệu thất bại: " + e.Message + "');</script>";
            }
            return RedirectToAction("CreateKNL", "FPosition", new { id = IDVT, IDPB = IDPB });
        }
        public int GetIDNangLuc(string tenNL, int? IDBP,int? IDLoaiNL)
        {
            var model = db.KhungNangLucs.Where(x => x.TenNL == tenNL && x.IDPB == IDBP && x.IDLoaiNL ==IDLoaiNL).FirstOrDefault();
            if (model == null)
                return 0;
            return model.IDNL;
        }
        public int? ToNullableInt(string s)
        {
            int i;
            if (int.TryParse(s, out i)) return i;
            return null;
        }
        public int GetDanhGia(string TenDG)
        {
            if (TenDG == "True"||TenDG =="true") return 0;
            else return 1;
        }
        public int GetIDLoaiNL(string tenloai, int? idvt)
        {
            var model = db.LoaiKNLs.Where(x => x.TenLoai == tenloai && x.IDVT == idvt).FirstOrDefault();
            if (model == null)
                return 0;
            return model.IDLoai;
        }
        public int? GetIDPNS(string tenpb)
        {
            var model = db.PhongBans.Where(x => x.TenPhongBan == tenpb).FirstOrDefault();
            if (model == null)
                return null;
            return model.IDPhongBan;
        }
        public JsonResult CheckNV(string lsnv)
        {
            var ListNV = new List<EmployeeValidation>();
            if (!String.IsNullOrEmpty(lsnv))
            {
                //Regex.Replace(_DO.NVDG, @"[^0-9a-zA-Z]+", " ");
                string tx = Regex.Replace(lsnv, @"[^0-9a-zA-Z]+", " ");
                string[] NVS = tx.Split(new char[] { ' ' });
                
                foreach (var item in NVS)
                {
                    var aa = db.NhanViens.Where(x => x.MaNV == item && x.IDTinhTrangLV ==1).ToList();
                    if (aa.Count > 0)
                    {
                        ListNV.Add(new EmployeeValidation { MaNV = aa.FirstOrDefault().MaNV, HoTen = aa.FirstOrDefault().MaNV + " - " + aa.FirstOrDefault().HoTen });
                    }
                }
            }
            return Json(ListNV, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CheckVT(string lsnv)
        {
            var ListNV = new List<EmployeeValidation>();
            if (!String.IsNullOrEmpty(lsnv))
            {
                //Regex.Replace(_DO.NVDG, @"[^0-9a-zA-Z]+", " ");
                string tx = Regex.Replace(lsnv, @"[^0-9a-zA-Z]+", " ");
                string[] NVS = tx.Split(new char[] { ' ' });

                foreach (var item in NVS)
                {
                    int IDVT = Int32.TryParse(item, out IDVT) ? IDVT : 0;
                    var aa = db.ViTriKNLs.Where(x => x.IDVT == IDVT).ToList();
                    if (aa.Count > 0)
                    {
                        ListNV.Add(new EmployeeValidation { ID = aa.FirstOrDefault().IDVT, HoTen = aa.FirstOrDefault().TenViTri });
                    }
                }
            }
            return Json(ListNV, JsonRequestBehavior.AllowGet);
        }



        public JsonResult CheckListVT(string lsVTN,string lsVTD)
        {
            var ListNV = new List<EmployeeValidation>();
            if (!String.IsNullOrEmpty(lsVTN) && !String.IsNullOrEmpty(lsVTD))
            {
                string tx = Regex.Replace(lsVTN, @"[^0-9a-zA-Z]+", " ");
                string txd = Regex.Replace(lsVTD, @"[^0-9a-zA-Z]+", " ");
                string[] VTN = tx.Split(new char[] { ' ' });
                string[] VTD = txd.Split(new char[] { ' ' });

                //Regex.Replace(_DO.NVDG, @"[^0-9a-zA-Z]+", " ");
                //string tx = Regex.Replace(lsVTN, @"[^0-9a-zA-Z]+", " ");
                //string[] NVS = tx.Split(new char[] { ' ' });
                int k = 0;
                foreach (var item in VTD)
                {
                    ListNV.Add(new EmployeeValidation { MaNV = VTN[k], HoTen = item });
                    //var aa = db.NhanViens.Where(x => x.MaNV == item && x.IDTinhTrangLV == 1).ToList();
                    //int? i = ToNullableInt(item);
                    //if (i != null)
                    //{
                    //    ListNV.Add(new EmployeeValidation { MaNV = , HoTen = aa.FirstOrDefault().MaNV + " - " + aa.FirstOrDefault().HoTen });
                    //}
                    k++;
                }
            }
            return Json(ListNV, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteNVIDKNLJSON(string manv)
        {
            try
            {
                var aa = db.NhanViens.Where(x => x.MaNV == manv).ToList();
                if(aa.Count > 0)
                {
                    db.Nhanvien_update_IDKNL(manv, null);
                }
                
            }
            catch (Exception e)
            {
                TempData["msgSuccess"] = "<script>alert('Xóa dữ liệu thất bại: " + e.Message + "');</script>";
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getNVJSON(int? id)
        {
            //try
            //{
            //    var nv2 = db.NhanViens.Where(x => x.IDTinhTrangLV == 1 && x.IDVTKNL == id).Select(x => new EmployeeValidation { MaNV = x.MaNV, HoTen = x.MaNV + " - " + x.HoTen }).ToList();
            //    return Json(nv2, JsonRequestBehavior.AllowGet);
            //}
            //catch (Exception e)
            //{
            //    TempData["msgSuccess"] = "<script>alert('Xóa dữ liệu thất bại: " + e.Message + "');</script>";
            //}
            //return Json(nv2, JsonRequestBehavior.AllowGet);
            var nv2 = db.NhanViens.Where(x => x.IDTinhTrangLV == 1 && x.IDVTKNL == id).Select(x => new EmployeeValidation { MaNV = x.MaNV, HoTen = x.MaNV + " - " + x.HoTen }).ToList();
            return Json(nv2, JsonRequestBehavior.AllowGet);
        }
        //public JsonResult DeleteKNLDGJSON(int? ID)
        //{
        //    try
        //    {
        //        db.KNLDG_delete(ID);

        //    }
        //    catch (Exception e)
        //    {
        //        TempData["msgSuccess"] = "<script>alert('Xóa dữ liệu thất bại: " + e.Message + "');</script>";
        //    }
        //    return Json(true, JsonRequestBehavior.AllowGet);
        //}
        public JsonResult DeleteNVKNJSON(int? idvt, int? ID)
        {
            db.KNLNVKiemNhiem_delete(ID);
            var nvkn = (from a in db.KNL_NVKiemNhiem.Where(x => x.IDVTKN == idvt)
                        join h in db.NhanViens
                        on a.IDNV equals h.ID into ulg
                        from h in ulg.DefaultIfEmpty()
                        select new EmployeeValidation { ID = a.ID, HoTen = h.MaNV + " - " + h.HoTen }).ToList();
            return Json(nvkn, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteVTTT(int? ID)
        {
            try
            {
                db.KNLDGiaTC_delete(ID);
                //db.KNLDG_delete(ID);
                //db.VitriKNL_update_VTParent(ID, null);
            }
            catch (Exception e)
            {
                TempData["msgSuccess"] = "<script>alert('Xóa dữ liệu thất bại: " + e.Message + "');</script>";
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteVTTC(int? ID)
        {
            try
            {
                db.KNLDGiaTC_delete(ID);
                //db.KNLDG_delete(ID);
                //db.VitriKNL_update_VTParent(ID, null);
            }
            catch (Exception e)
            {
                TempData["msgSuccess"] = "<script>alert('Xóa dữ liệu thất bại: " + e.Message + "');</script>";
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getVTTTJSON(int? id, int? IDPB)
        {

            List<KNLDGiaTCValidation> vt = (from a in db.KNL_DGiaTC.Where(x => x.IDVT == id && x.IDVTDGTT != null)
                                            join b in db.ViTriKNLs on a.IDVTDGTT equals b.IDVT
                                            join e in db.KNL_PhanXuong
                                            on b.IDPX equals e.ID into ul
                                            from e in ul.DefaultIfEmpty()
                                            join f in db.KNL_Nhom
                                            on b.IDNhom equals f.IDNhom into uls
                                            from f in uls.DefaultIfEmpty()
                                            join g in db.KNL_To
                                            on b.IDTo equals g.IDTo into ulk
                                            from g in ulk.DefaultIfEmpty()
                                            select new KNLDGiaTCValidation
                                            {
                                                ID = a.ID,
                                                IDVT = (int)a.IDVT,
                                                TenViTri = b.TenViTri + "-" + e.TenPX + "-" + f.TenNhom + "-" + g.TenTo,
                                                MaViTri = b.MaViTri,
                                                IDPB = b.IDPB,
                                                IDVTDGTC = a.IDVTDGTC,
                                                IDVTDGTT = a.IDVTDGTT
                                            }).ToList();

            return Json(vt, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getVTTCJSON(int? id, int? IDPB)
        {


            List<KNLDGiaTCValidation> vt = (from a in db.KNL_DGiaTC.Where(x => x.IDVT == id && x.IDVTDGTC != null)
                                            join b in db.ViTriKNLs on a.IDVTDGTC equals b.IDVT
                                            join e in db.KNL_PhanXuong
                                            on b.IDPX equals e.ID into ul
                                            from e in ul.DefaultIfEmpty()
                                            join f in db.KNL_Nhom
                                            on b.IDNhom equals f.IDNhom into uls
                                            from f in uls.DefaultIfEmpty()
                                            join g in db.KNL_To
                                            on b.IDTo equals g.IDTo into ulk
                                            from g in ulk.DefaultIfEmpty()
                                            select new KNLDGiaTCValidation
                                            {
                                                ID = a.ID,
                                                IDVT = (int)a.IDVT,
                                                TenViTri = b.TenViTri + "-" + e.TenPX + "-" + f.TenNhom + "-" + g.TenTo,
                                                MaViTri = b.MaViTri,
                                                IDPB = b.IDPB,
                                                IDVTDGTC = a.IDVTDGTC,
                                                IDVTDGTT = a.IDVTDGTT
                                            }).ToList();
            return Json(vt, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult getKNLDGJSON(int? id,int? IDPB)
        //{
        //    var vt = (from a in db.ViTriKNLs.Where(x => x.IDPB == IDPB)
        //              join e in db.KNL_PhanXuong
        //              on a.IDPX equals e.ID into ul
        //              from e in ul.DefaultIfEmpty()
        //              join f in db.KNL_Nhom
        //              on a.IDNhom equals f.IDNhom into uls
        //              from f in uls.DefaultIfEmpty()
        //              join g in db.KNL_To
        //              on a.IDTo equals g.IDTo into ulk
        //              from g in ulk.DefaultIfEmpty()
        //              select new ViTriKNLValidation
        //              {
        //                  IDVT = a.IDVT,
        //                  TenViTri = a.TenViTri + "-" + e.TenPX + "-" + f.TenNhom + "-" + g.TenTo,
        //              }).ToList();
        //    var lsVT = db.KNL_DG.Where(x => x.IDVTDG == id).ToList();

        //    var vtSelect = new List<ViTriKNLValidation>();
        //    foreach (var item in lsVT)
        //    {
        //        var vta = vt.Where(x => x.IDVT == item.IDVTDDG).FirstOrDefault();
        //        vtSelect.Add(new ViTriKNLValidation { IDVT = item.ID, TenViTri = vta.TenViTri + "-" + vta.TenPX + "-" + vta.TenNhom + "-" + vta.TenTo, });
        //    }
        //    return Json(vtSelect, JsonRequestBehavior.AllowGet);
        //}

        public JsonResult GetVTKNL(int? IDPB)
        {
            var vt = (from a in db.VitriKNL_search()
                      select new ViTriKNLValidation
                      {
                          IDVT = a.IDVT,
                          TenViTri = a.TenViTri + "-" + a.MaPB + "-" + a.TenPX + "-" + a.TenNhom + "-" + a.TenTo,
                          IDNhom = a.IDNhom,
                          IDTo = a.IDTo,
                          MaViTri = a.MaViTri,
                          IDPX = a.IDPX,
                          IDPB =a.IDPB
                      }).ToList();
            if (IDPB != null) vt = vt.Where(x => x.IDPB == IDPB).ToList();
            ViewBag.IDVT = new SelectList(vt, "IDVT", "TenViTri");

            return Json(vt, JsonRequestBehavior.AllowGet);
        }
        public string checkMVT2(string mvt)
        {
            if (mvt is null) return "";
            if (mvt.Length <2) return "";
            else return mvt.Substring(0, 2);
            //switch (mvt.Substring(0, 2))
            //{
            //    case "PT":
            //        return "PT";
            //    case "TT":
            //        return "TT";
            //    case "TP":
            //        return "TP";
            //    default:
            //        return "";
            //}
        }
        public string checkMVT3(string mvt)
        {
            if (mvt is null) return "";
            if (mvt.Length <3) return "";
            else return mvt.Substring(0, 3);
        }

        public ActionResult ExportToExcel()
        {
            try
            {
                string fileNamemau = AppDomain.CurrentDomain.BaseDirectory + @"App_Data\ThongKeNhapKNL.xlsx";
                string fileNamemaunew = AppDomain.CurrentDomain.BaseDirectory + @"App_Data\ThongKeNhapKNL_Temp.xlsx";
                XLWorkbook Workbook = new XLWorkbook(fileNamemau);
                IXLWorksheet Worksheet = Workbook.Worksheet("ThongKe_KNL");
                List<ExportViTriKNL> DataKNL = GetViTriKNL();
                int row = 5;
                if (DataKNL.Count > 0)
                {
                    foreach (var data in DataKNL)
                    {
                        Worksheet.Cell(row, "A").Value = row - 4;
                        Worksheet.Cell(row, "A").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        Worksheet.Cell(row, "A").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "A").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "B").Value = data.TenViTri;
                        Worksheet.Cell(row, "B").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "B").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "B").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "C").Value = data.MaViTri;
                        Worksheet.Cell(row, "C").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        Worksheet.Cell(row, "C").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "C").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "D").Value = data.TenPhongBan;
                        Worksheet.Cell(row, "D").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "D").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "D").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "E").Value = data.TenKhoi;
                        Worksheet.Cell(row, "E").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "E").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "E").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "F").Value = data.TenPX;
                        Worksheet.Cell(row, "F").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "F").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "F").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "G").Value = data.TenNhom;
                        Worksheet.Cell(row, "G").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "G").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "G").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "H").Value = data.TenTo;
                        Worksheet.Cell(row, "H").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "H").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "H").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "I").Value = data.CountChung;
                        Worksheet.Cell(row, "I").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        Worksheet.Cell(row, "I").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "I").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "J").Value = data.SLDinhMucChung;
                        Worksheet.Cell(row, "J").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        Worksheet.Cell(row, "J").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "J").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        Worksheet.Cell(row, "J").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "K").Value = data.SLisDanhGiaChung;
                        Worksheet.Cell(row, "K").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        Worksheet.Cell(row, "K").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "K").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "L").Value = data.CountCMon;
                        Worksheet.Cell(row, "L").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        Worksheet.Cell(row, "L").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "L").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "M").Value = data.SLDinhMucCMon;
                        Worksheet.Cell(row, "M").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        Worksheet.Cell(row, "M").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "M").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        Worksheet.Cell(row, "M").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "N").Value = data.SLisDanhGiaCMon;
                        Worksheet.Cell(row, "N").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        Worksheet.Cell(row, "N").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "N").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "O").Value = data.CountQLy;
                        Worksheet.Cell(row, "O").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        Worksheet.Cell(row, "O").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "O").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "P").Value = data.SLDinhMucQLy;
                        Worksheet.Cell(row, "P").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        Worksheet.Cell(row, "P").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "P").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        Worksheet.Cell(row, "P").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "Q").Value = data.SLisDanhGiaQLy;
                        Worksheet.Cell(row, "Q").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        Worksheet.Cell(row, "Q").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "Q").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "R").Value = data.CountTotal;
                        Worksheet.Cell(row, "R").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        Worksheet.Cell(row, "R").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "R").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "S").Value = data.NhapBMTCV;
                        Worksheet.Cell(row, "S").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        Worksheet.Cell(row, "S").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "S").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "T").Value = data.CountNV;
                        Worksheet.Cell(row, "T").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        Worksheet.Cell(row, "T").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "T").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "U").Value = data.CountVTTT;
                        Worksheet.Cell(row, "U").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        Worksheet.Cell(row, "U").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "U").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "V").Value = data.CountVTXem;
                        Worksheet.Cell(row, "V").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        Worksheet.Cell(row, "V").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "V").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "W").Value = data.CountNVDGTT;
                        Worksheet.Cell(row, "W").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        Worksheet.Cell(row, "W").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "W").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "X").Value = data.SLNVDuocDG;
                        Worksheet.Cell(row, "X").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        Worksheet.Cell(row, "X").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "X").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "Y").Value = data.CountNV - data.SLNVDuocDG;
                        Worksheet.Cell(row, "Y").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        Worksheet.Cell(row, "Y").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "Y").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "Z").Value = data.CountVTAuto;
                        Worksheet.Cell(row, "Z").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        Worksheet.Cell(row, "Z").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "Z").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;


                        Worksheet.Cell(row, "AA").Value = data.IDVT;
                        Worksheet.Cell(row, "AA").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        Worksheet.Cell(row, "AA").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "AA").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "AB").Value = data.TinhTrang ==0?"Hết hiệu lực":"Còn hiệu lực";
                        Worksheet.Cell(row, "AB").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        Worksheet.Cell(row, "AB").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "AB").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        row = row + 1;
                    }

                    Workbook.SaveAs(fileNamemaunew);
                    byte[] fileBytes = System.IO.File.ReadAllBytes(fileNamemaunew);
                    string fileName = "ThongKe_KNL - " + DateTime.Now.Date.ToString("dd/MM/yyyy") + ".xlsx";
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
                }
                else
                {

                    //Worksheet.Cell("A1").Value = "Ngày xuất file: " + DateTime.Now.Date.ToString("dd/MM/yyyy");
                    Workbook.SaveAs(fileNamemaunew);
                    byte[] fileBytes = System.IO.File.ReadAllBytes(fileNamemaunew);
                    string fileName = "ThongKe_KNL - " + DateTime.Now.Date.ToString("dd/MM/yyyy") + ".xlsx";
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = "<script>alert('" + ex + "');window.location.href = '/Everyday/'</script>";
                return View(TempData);
            }
        }
        private List<ExportViTriKNL> GetViTriKNL()
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            //var listKNL = db.KhungNangLucs.ToList();
           
            var fir = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            //var KQKNLThang = db.KNL_KQ_SelectThang(fir).ToList();
            var KQKNLThang = db.KNL_KQ_SelectDDG().ToList();

            var listVT = db.ViTriKNLs.ToList();
            if (ListQuyen.Contains(CONSTKEY.V_BP)) listVT = listVT.Where(x => x.IDPB == MyAuthentication.IDPhongban).ToList();
            var res = (from a in db.ViTriKNLs
                       join d in db.PhongBans
                       on a.IDPB equals d.IDPhongBan
                       join e in db.KNL_PhanXuong
                        on a.IDPX equals e.ID into ul
                       from e in ul.DefaultIfEmpty()
                       join f in db.KNL_Nhom
                       on a.IDNhom equals f.IDNhom into uls
                       from f in uls.DefaultIfEmpty()
                       join g in db.KNL_To
                       on a.IDTo equals g.IDTo into ulk
                       from g in ulk.DefaultIfEmpty()
                       join h in db.KNL_Khoi
                       on a.IDKhoi equals h.ID into ulkl
                       from h in ulkl.DefaultIfEmpty()
                       select new ExportViTriKNL
                       {
                           IDVT = a.IDVT,
                           TenViTri = a.TenViTri,
                           IDPB = a.IDPB,
                           TenPhongBan = d.TenPhongBan,
                           MaViTri = a.MaViTri,
                           TenKhoi = h.TenKhoi,
                           TenPX = e.TenPX,
                           TenNhom = f.TenNhom,
                           TenTo = g.TenTo,
                           TinhTrang = a.TinhTrang,
                           NhapBMTCV = a.FilePath != null ? "Đã Nhập" : "Chưa Nhập",
                           CountChung = db.KhungNangLucs.Where(x => x.IDVT == a.IDVT && x.IDLoaiNL == 1).Count(),
                           CountQLy = db.KhungNangLucs.Where(x => x.IDVT == a.IDVT && x.IDLoaiNL == 2).Count(),
                           CountCMon = db.KhungNangLucs.Where(x => x.IDVT == a.IDVT && x.IDLoaiNL != 2 && x.IDLoaiNL != 1).Count(),
                           CountTotal = db.KhungNangLucs.Where(x => x.IDVT == a.IDVT).Count(),
                           SLDinhMucChung = db.KhungNangLucs.Where(x => x.IDVT == a.IDVT && x.IDLoaiNL == 1 && x.DinhMuc != null && x.IsDanhGia == 1).Count(),
                           SLDinhMucCMon = db.KhungNangLucs.Where(x => x.IDVT == a.IDVT && x.IDLoaiNL != 2 && x.IDLoaiNL != 1 && x.DinhMuc != null && x.IsDanhGia == 1).Count(),
                           SLDinhMucQLy = db.KhungNangLucs.Where(x => x.IDVT == a.IDVT && x.IDLoaiNL == 2 && x.DinhMuc != null && x.IsDanhGia == 1).Count(),
                           SLisDanhGiaChung = db.KhungNangLucs.Where(x => x.IDVT == a.IDVT && x.IDLoaiNL == 1 && x.IsDanhGia == 0).Count(),
                           SLisDanhGiaCMon = db.KhungNangLucs.Where(x => x.IDVT == a.IDVT && x.IDLoaiNL != 2 && x.IDLoaiNL != 1 && x.IsDanhGia == 0).Count(),
                           SLisDanhGiaQLy = db.KhungNangLucs.Where(x => x.IDVT == a.IDVT && x.IDLoaiNL == 2 && x.IsDanhGia == 0).Count(),
                           CountNV = db.NhanViens.Where(x => x.IDVTKNL == a.IDVT && x.IDTinhTrangLV == 1).Count(),
                           CountVTTT = db.KNL_DGiaTC.Where(x => x.IDVT == a.IDVT && x.IDVTDGTT != null).Count(),
                           CountVTXem = db.KNL_DGiaTC.Where(x => x.IDVT == a.IDVT && x.IDVTDGTC != null).Count(),
                           CountNVDGTT = (from aa in db.NhanViens join ab in db.KNL_DGiaTC.Where(x => x.IDVT == a.IDVT && x.IDVTDGTT != null) on aa.IDVTKNL equals ab.IDVTDGTT select aa).ToList().Count,
                           //SLNVDuocDG = (from aa in db.NhanViens.Where(x => x.IDVTKNL == a.IDVT && x.IDTinhTrangLV == 1) join ab in db.KNL_KQ.Where(x=>x.ThangDG ==fir) on aa.ID equals ab.IDNV into ls from ab in ls.Take(1) select aa).ToList().Count,
                           //CountVTAuto =  CountListSetAuto(a)
                       }).OrderBy(x => x.IDPB).ToList();
            int i = 0;
            foreach(var item in res)
            {
                var vt = listVT.Where(x => x.IDVT == item.IDVT).FirstOrDefault();
                item.CountVTAuto = getListSetAuto(vt).Count();
                item.SLNVDuocDG = KQKNLThang.Where(x => x.IDVTKNL == item.IDVT).Count();
                i++;
            }
            return res;
        }
        public JsonResult GetDataNVTable(int? IDPB)
        {
            int idpb = MyAuthentication.IDPhongban;
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            var ListNV = new List<FCheckValidation>();
            try
            {
                if (IDPB == null) IDPB = 0;
                //if (IDPB != 0) res = res.Where(x => x.IDPB == IDPB).ToList();
                var res = (from a in db.NhanVien_searchByVTKNL(IDPB)
                           select new FCheckValidation
                           {
                               MaNV = a.MaNV,
                               IDNV = a.ID,
                               IDVT = a.IDVT,
                               TenVT = a.TenViTri + "-" + a.TenNhom + "-" + a.TenTo + "-" + a.TenPX + "-" + a.MaPB,
                               TenNV = a.HoTen,
                               IDNhom = a.IDNhom,
                               IDPB = a.IDPB,
                               IDKip = a.IDKip,
                               TenKip = a.TenKip,
                               MaViTri = a.MaViTri,
                               TenPB = a.TenPhongBan
                           }).ToList();
                
                if (ListQuyen.Contains("VIEW_BP"))
                {
                    res = res.Where(x => x.IDPB == idpb).ToList();
                }
                if (!User.Identity.IsAuthenticated)
                {
                    ListNV = new List<FCheckValidation>();
                }
                else ListNV = res;
                foreach (var item in res)
                {
                    //var KQ = db.KNL_KQ.Where(x => x.IDNV == item.IDNV).OrderByDescending(i => i.NgayDG).ToList();
                    var KQ = db.KNL_KQ_SelectNV(item.IDNV).ToList();
                    var lastCheck = KQ.FirstOrDefault();
                    if (lastCheck != null)
                    {
                        var KQDG = db.KNL_KQ_Select(item.IDNV, lastCheck?.ThangDG).ToList();
                        item.NgayDG = KQ.FirstOrDefault()?.NgayDG != null ? String.Format("{0:dd/MM/yyyy}", KQ.FirstOrDefault()?.NgayDG) : "";
                        item.Total = lastCheck.ThangDG != null ? db.KNL_KQ_searchByIDNV(item.IDNV,lastCheck?.ThangDG,item.IDVT).Count() : 0;
                        item.TotalDat = KQDG != null ? KQDG.Where(x => x.DiemDG == x.DinhMuc && x.DiemDG != null).Count() : 0;
                        item.TotalKDat = KQDG != null ? KQDG.Where(x => x.DiemDG < x.DinhMuc && x.DiemDG != null).Count() : 0;
                        item.TotalVuot = KQDG != null ? KQDG.Where(x => x.DiemDG > x.DinhMuc && x.DiemDG != null).Count() : 0;
                        item.TotalKDGia = KQDG != null ? KQDG.Where(x => x.IsDanhGia == 0 || x.DiemDG == null).Count() : 0;
                        item.ThangDG = lastCheck?.ThangDG.ToString();
                    }
                }
            }
            catch (Exception e)
            {
                TempData["msgSuccess"] = "<script>alert('Có lỗi xảy ra: " + e.Message + "');</script>";
            }
            var jsonResult = Json(ListNV, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
            //return Json(ListNV, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDataVTKNL(int? IDPB)
        {
            if (IDPB == null) IDPB = 0;
            List<ViTriKNLValidation> px = (from a in db.ViTriKNLs
                                           join e in db.KNL_PhanXuong
                                           on a.IDPX equals e.ID into ul
                                           from e in ul.DefaultIfEmpty()
                                           join f in db.KNL_Nhom
                                           on a.IDNhom equals f.IDNhom into uls
                                           from f in uls.DefaultIfEmpty()
                                           join g in db.KNL_To
                                           on a.IDTo equals g.IDTo into ulk
                                           from g in ulk.DefaultIfEmpty()
                                           join h in db.PhongBans 
                                           on a.IDPB equals h.IDPhongBan
                                           select new ViTriKNLValidation
                                           {
                                               IDVT = a.IDVT,
                                               TenViTri = a.TenViTri + "-" + f.TenNhom + "-" + g.TenTo + "-" + e.TenPX + "-"+h.MaPB,
                                               IDNhom = a.IDNhom,
                                               IDTo = a.IDTo,
                                               MaViTri = a.MaViTri,
                                               IDPX = a.IDPX,
                                               IDVTParent = a.IDVTParent,
                                               IDPB = a.IDPB
                                           }).ToList();
            if (IDPB != 0) px = px.Where(x => x.IDPB == IDPB).ToList();
            else px = new List<ViTriKNLValidation>();
            return Json(px, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSetPermision(int? IDPB)
        {
            if (IDPB == null) IDPB = 0;
            //List<SetPermisionKNLValidation> res = new List<SetPermisionKNLValidation>();

            List<ViTriKNLValidation> n = (from a in db.ViTriKNLs
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
                                              MaViTri = a.MaViTri,
                                              IDPB = a.IDPB
                                          }).ToList();

            List<SetPermisionKNLValidation> px = (from a in db.ViTriKNLs
                                                  join e in db.KNL_PhanXuong
                                                  on a.IDPX equals e.ID into ul
                                                  from e in ul.DefaultIfEmpty()
                                                  join f in db.KNL_Nhom
                                                  on a.IDNhom equals f.IDNhom into uls
                                                  from f in uls.DefaultIfEmpty()
                                                  join g in db.KNL_To
                                                  on a.IDTo equals g.IDTo into ulk
                                                  from g in ulk.DefaultIfEmpty()
                                                  join b in db.KNL_DGiaTC
                                                  on a.IDVT equals b.IDVT into ulkv
                                                  from b in ulkv.DefaultIfEmpty()
                                                  select new SetPermisionKNLValidation
                                                  {
                                                      IDVT = a.IDVT,
                                                      TenViTri = a.TenViTri + "-" + e.TenPX + "-" + f.TenNhom + "-" + g.TenTo,
                                                      MaViTri = a.MaViTri,
                                                      IDPB = a.IDPB,
                                                      //ListVTTT = (from aa in n join ll in db.KNL_DGiaTC on aa.IDVT equals ll.IDVT select new ),
                                                      //ListVTSet = n.ToList(),
                                                  }).ToList();
            foreach (var item in px)
            {
                var aa = db.KNL_DGiaTC.Where(x => x.IDVT == item.IDVT).ToList();
                if (aa.Count > 0)
                {
                    var TT = new List<ViTriKNLValidation>();
                    var TC = new List<ViTriKNLValidation>();
                    foreach (var vt in aa)
                    {
                        if (vt.IDVTDGTT != null)
                        {
                            var na = n.Where(x => x.IDVT == vt.IDVTDGTT).FirstOrDefault();
                            TT.Add(na);
                        }
                        else if (vt.IDVTDGTC != null)
                        {
                            var na = n.Where(x => x.IDVT == vt.IDVTDGTC).FirstOrDefault();
                            TT.Add(na);
                        }
                    }
                    item.ListVTTT = TT;
                    item.ListVTSet = TC;
                }
            }
            if (IDPB != 0) px = px.Where(x => x.IDPB == IDPB).ToList();
            //else px = new List<SetPermisionKNLValidation>();
            return Json(px, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExportToExcelNV()
        {
            try
            {
                string fileNamemau = AppDomain.CurrentDomain.BaseDirectory + @"App_Data\ThongKeNV_KNL.xlsx";
                string fileNamemaunew = AppDomain.CurrentDomain.BaseDirectory + @"App_Data\ThongKeNV_KNL_Temp.xlsx";
                XLWorkbook Workbook = new XLWorkbook(fileNamemau);
                IXLWorksheet Worksheet = Workbook.Worksheet("NhanVien_KNL");
                List<ExportNhanVienKNL> DataKNL = GetNhanVienKNL();
                int row = 5;
                if (DataKNL.Count > 0)
                {
                    foreach (var data in DataKNL)
                    {
                        Worksheet.Cell(row, "A").Value = row - 4;
                        Worksheet.Cell(row, "A").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        Worksheet.Cell(row, "A").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "A").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "B").Value = data.MaNV;
                        Worksheet.Cell(row, "B").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "B").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "B").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "C").Value = data.TenNV;
                        Worksheet.Cell(row, "C").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "C").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "C").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "D").Value = data.TenKip;
                        Worksheet.Cell(row, "D").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "D").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "D").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "E").Value = data.TenViTri;
                        Worksheet.Cell(row, "E").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "E").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "E").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        

                        Worksheet.Cell(row, "F").Value = data.IDVT;
                        Worksheet.Cell(row, "F").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "F").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "F").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        Worksheet.Cell(row, "F").Style.Fill.BackgroundColor = XLColor.Yellow;

                        Worksheet.Cell(row, "G").Value = data.TenTo;
                        Worksheet.Cell(row, "G").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "G").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "G").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "H").Value = data.TenNhom;
                        Worksheet.Cell(row, "H").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "H").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "H").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "I").Value = data.TenPX;
                        Worksheet.Cell(row, "I").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "I").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "I").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "J").Value = data.TenPhongBan;
                        Worksheet.Cell(row, "J").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "J").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "J").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        Worksheet.Cell(row, "J").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;


                        row = row + 1;
                    }

                    Workbook.SaveAs(fileNamemaunew);
                    Workbook.Dispose();
                    byte[] fileBytes = System.IO.File.ReadAllBytes(fileNamemaunew);
                    string fileName = "NhanVien_KNL - " + DateTime.Now.Date.ToString("dd/MM/yyyy") + ".xlsx";
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
                }
                else
                {

                    //Worksheet.Cell("A1").Value = "Ngày xuất file: " + DateTime.Now.Date.ToString("dd/MM/yyyy");
                    Workbook.SaveAs(fileNamemaunew);
                    Workbook.Dispose();
                    byte[] fileBytes = System.IO.File.ReadAllBytes(fileNamemaunew);
                    string fileName = "NhanVien_KNL - " + DateTime.Now.Date.ToString("dd/MM/yyyy") + ".xlsx";
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
                }

            }
            catch (Exception ex)
            {
                //TempData["msg"] = "<script>alert('" + ex + "');window.location.href = '/Everyday/'</script>";
                //return View(TempData);
                TempData["msgError"] = "<script>alert('" + ex + "');window.location.href = '/Everyday/'</script>";
                //TempData["msg"] = "<script>alert('" + ex + "');window.location.href = '/Everyday/'</script>";
                //return View(TempData);
                return RedirectToAction("ListNVKNL", "FPosition");
            }
        }

        public ActionResult ExportToExcelNV1(int? IDPB,int? IDPX, int? IDTo, int? IDNhom)
        {
            try
            {
                string fileNamemau = AppDomain.CurrentDomain.BaseDirectory + @"App_Data\ThongKeKQua_KNL.xlsx";
                string fileNamemaunew = AppDomain.CurrentDomain.BaseDirectory + @"App_Data\ThongKeKQua_KNL_Temp.xlsx";
                XLWorkbook Workbook = new XLWorkbook(fileNamemau);
                IXLWorksheet Worksheet = Workbook.Worksheet("NhanVien_KNL");
                List<ExportNhanVienKQKNL> DataKNL = GetNhanVienKNL1(IDPB, IDPX, IDTo,IDNhom);
                int row = 5;
                if (DataKNL.Count > 0)
                {
                    foreach (var data in DataKNL)
                    {
                        Worksheet.Cell(row, "A").Value = row - 4;
                        Worksheet.Cell(row, "A").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        Worksheet.Cell(row, "A").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "A").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "B").Value = data.MaNV;
                        Worksheet.Cell(row, "B").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "B").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "B").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "C").Value = data.TenNV;
                        Worksheet.Cell(row, "C").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "C").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "C").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "D").Value = data.TenKip;
                        Worksheet.Cell(row, "D").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "D").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "D").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "E").Value = data.TenViTri;
                        Worksheet.Cell(row, "E").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "E").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "E").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;


                        Worksheet.Cell(row, "F").Value = data.IDVT;
                        Worksheet.Cell(row, "F").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "F").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "F").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        Worksheet.Cell(row, "F").Style.Fill.BackgroundColor = XLColor.Yellow;

                        Worksheet.Cell(row, "G").Value = data.TenPhongBan;
                        Worksheet.Cell(row, "G").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "G").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "G").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "H").Value = data.TenPX;
                        Worksheet.Cell(row, "H").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "H").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "H").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "I").Value = data.TenNhom;
                        Worksheet.Cell(row, "I").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "I").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "I").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "J").Value = data.TenTo;
                        Worksheet.Cell(row, "J").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "J").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "J").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        Worksheet.Cell(row, "J").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "K").Value = data.TotalNL;
                        Worksheet.Cell(row, "K").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "K").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "K").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        Worksheet.Cell(row, "K").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "L").Value = data.KDGIA;
                        Worksheet.Cell(row, "L").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "L").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "L").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        Worksheet.Cell(row, "L").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "M").Value = data.TotalDocKNL;
                        Worksheet.Cell(row, "M").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "M").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "M").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        Worksheet.Cell(row, "M").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "N").Value = data.KDATTu;
                        Worksheet.Cell(row, "N").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "N").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "N").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        Worksheet.Cell(row, "N").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "O").Value = data.DATTu;
                        Worksheet.Cell(row, "O").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "O").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "O").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        Worksheet.Cell(row, "O").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "P").Value = data.VUOTTu;
                        Worksheet.Cell(row, "P").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "P").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "P").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        Worksheet.Cell(row, "P").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "Q").Value = data.CHUADGTu;
                        Worksheet.Cell(row, "Q").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "Q").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "Q").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        Worksheet.Cell(row, "Q").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "R").Value = data.NgayDGTu != null?"'"+ data.NgayDGTu.Value.ToString("dd/MM/yyyy"):"";
                        Worksheet.Cell(row, "R").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "R").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "R").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        Worksheet.Cell(row, "R").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "S").Value = data.KDATCap1;
                        Worksheet.Cell(row, "S").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "S").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "S").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        Worksheet.Cell(row, "S").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "T").Value = data.DATCap1;
                        Worksheet.Cell(row, "T").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "T").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "T").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        Worksheet.Cell(row, "T").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "U").Value = data.VUOTCap1;
                        Worksheet.Cell(row, "U").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "U").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "U").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        Worksheet.Cell(row, "U").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "V").Value = data.CHUADGCap1;
                        Worksheet.Cell(row, "V").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "V").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "V").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        Worksheet.Cell(row, "V").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "W").Value =data.NgayDGCap1 != null ? "'" + data.NgayDGCap1.Value.ToString("dd/MM/yyyy") : "";
                        Worksheet.Cell(row, "W").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "W").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "W").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        Worksheet.Cell(row, "W").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "X").Value = data.KDAT;
                        Worksheet.Cell(row, "X").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "X").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "X").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        Worksheet.Cell(row, "X").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "Y").Value = data.SLQuaHanKDAT;
                        Worksheet.Cell(row, "Y").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "Y").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "Y").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        Worksheet.Cell(row, "Y").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "Z").Value = data.DAT;
                        Worksheet.Cell(row, "Z").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "Z").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "Z").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        Worksheet.Cell(row, "Z").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "AA").Value = data.SLQuaHanDAT;
                        Worksheet.Cell(row, "AA").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "AA").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "AA").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        Worksheet.Cell(row, "AA").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "AB").Value = data.VUOT;
                        Worksheet.Cell(row, "AB").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "AB").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "AB").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        Worksheet.Cell(row, "AB").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "AC").Value = data.SLQuaHanVuot;
                        Worksheet.Cell(row, "AC").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "AC").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "AC").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        Worksheet.Cell(row, "AC").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "AD").Value = data.CHUADG;
                        Worksheet.Cell(row, "AD").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "AD").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "AD").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        Worksheet.Cell(row, "AD").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "AE").Value = data.SLQuaHanChuaDG;
                        Worksheet.Cell(row, "AE").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "AE").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "AE").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        Worksheet.Cell(row, "AE").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "AF").Value = data.NgayDGHN != null ? "'" + data.NgayDGHN.Value.ToString("dd/MM/yyyy") : "";
                        Worksheet.Cell(row, "AF").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "AF").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "AF").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        Worksheet.Cell(row, "AF").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        row = row + 1;
                    }

                    Workbook.SaveAs(fileNamemaunew);
                    Workbook.Dispose();
                    byte[] fileBytes = System.IO.File.ReadAllBytes(fileNamemaunew);
                    string fileName = "KetQua_KNL - " + DateTime.Now.Date.ToString("dd/MM/yyyy") + ".xlsx";
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
                }
                else
                {

                    //Worksheet.Cell("A1").Value = "Ngày xuất file: " + DateTime.Now.Date.ToString("dd/MM/yyyy");
                    Workbook.SaveAs(fileNamemaunew);
                    Workbook.Dispose();
                    byte[] fileBytes = System.IO.File.ReadAllBytes(fileNamemaunew);
                    string fileName = "KetQua_KNL - " + DateTime.Now.Date.ToString("dd/MM/yyyy") + ".xlsx";
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
                }

            }
            catch (Exception ex)
            {
                TempData["msg"] = "<script>alert('" + ex + "');window.location.href = '/Everyday/'</script>";
                return View(TempData);
            }
        }

        public ActionResult ExportToExcelViTri()
        {
            try
            {
                string fileNamemau = AppDomain.CurrentDomain.BaseDirectory + @"App_Data\DS_ViTriKNL.xlsx";
                string fileNamemaunew = AppDomain.CurrentDomain.BaseDirectory + @"App_Data\DS_ViTriKNL_Temp.xlsx";
                XLWorkbook Workbook = new XLWorkbook(fileNamemau);
                IXLWorksheet Worksheet = Workbook.Worksheet("DSVT_KNL");
                List<ExportViTriKNL> DataKNL = GetViTriKNL();
                int row = 5;
                if (DataKNL.Count > 0)
                {
                    foreach (var data in DataKNL)
                    {
                        Worksheet.Cell(row, "A").Value = row - 4;
                        Worksheet.Cell(row, "A").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        Worksheet.Cell(row, "A").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "A").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "B").Value = data.TenViTri;
                        Worksheet.Cell(row, "B").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "B").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "B").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "C").Value = data.IDVT;
                        Worksheet.Cell(row, "C").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        Worksheet.Cell(row, "C").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "C").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        Worksheet.Cell(row, "C").Style.Fill.BackgroundColor = XLColor.Yellow;

                        Worksheet.Cell(row, "D").Value = data.MaViTri;
                        Worksheet.Cell(row, "D").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "D").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "D").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "E").Value = data.TenPhongBan;
                        Worksheet.Cell(row, "E").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "E").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "E").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "F").Value = data.TenKhoi;
                        Worksheet.Cell(row, "F").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "F").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "F").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "G").Value = data.TenPX;
                        Worksheet.Cell(row, "G").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "G").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "G").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "H").Value = data.TenNhom;
                        Worksheet.Cell(row, "H").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "H").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "H").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "I").Value = data.TenTo;
                        Worksheet.Cell(row, "I").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        Worksheet.Cell(row, "I").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "I").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "J").Value = data.TinhTrang == 0 ? "Hết hiệu lực" : "Còn hiệu lực";
                        Worksheet.Cell(row, "J").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        Worksheet.Cell(row, "J").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "J").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        row = row + 1;
                    }

                    Workbook.SaveAs(fileNamemaunew);
                    byte[] fileBytes = System.IO.File.ReadAllBytes(fileNamemaunew);
                    string fileName = "DS_ViTriKNL - " + DateTime.Now.Date.ToString("dd/MM/yyyy") + ".xlsx";
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
                }
                else
                {

                    //Worksheet.Cell("A1").Value = "Ngày xuất file: " + DateTime.Now.Date.ToString("dd/MM/yyyy");
                    Workbook.SaveAs(fileNamemaunew);
                    byte[] fileBytes = System.IO.File.ReadAllBytes(fileNamemaunew);
                    string fileName = "DS_ViTriKNL - " + DateTime.Now.Date.ToString("dd/MM/yyyy") + ".xlsx";
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = "<script>alert('" + ex + "');window.location.href = '/Everyday/'</script>";
                return View(TempData);
            }
        }

        public ActionResult ExportToExcelBoPhan()
        {
            try
            {
                string fileNamemau = AppDomain.CurrentDomain.BaseDirectory + @"App_Data\ThongKeBoPhan_KNL.xlsx";
                string fileNamemaunew = AppDomain.CurrentDomain.BaseDirectory + @"App_Data\ThongKeBoPhan_KNL_Temp.xlsx";
                XLWorkbook Workbook = new XLWorkbook(fileNamemau);
                IXLWorksheet Worksheet = Workbook.Worksheet("BoPhan_KNL");
                List<ExportBoPhanKNL> DataKNL = GetBoPhanKNL();
                int row = 3;
                if (DataKNL.Count > 0)
                {
                    foreach (var data in DataKNL)
                    {
                        Worksheet.Cell(row, "A").Value = row - 2;
                        Worksheet.Cell(row, "A").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        Worksheet.Cell(row, "A").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "A").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "B").Value = data.TenPhongBan;
                        Worksheet.Cell(row, "B").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "B").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "B").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "C").Value = data.MaPhongBan;
                        Worksheet.Cell(row, "C").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "C").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "C").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "D").Value = data.TotalVT;
                        Worksheet.Cell(row, "D").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "D").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "D").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        Worksheet.Cell(row, "E").Value = data.VTDDG;
                        Worksheet.Cell(row, "E").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "E").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "E").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;


                        Worksheet.Cell(row, "F").Value = data.TotalNV;
                        Worksheet.Cell(row, "F").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "F").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "F").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        //Worksheet.Cell(row, "F").Style.Fill.BackgroundColor = XLColor.Yellow;

                        Worksheet.Cell(row, "G").Value = data.NVDDG;
                        Worksheet.Cell(row, "G").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        Worksheet.Cell(row, "G").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        Worksheet.Cell(row, "G").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;


                        row = row + 1;
                    }

                    Workbook.SaveAs(fileNamemaunew);
                    byte[] fileBytes = System.IO.File.ReadAllBytes(fileNamemaunew);
                    string fileName = "BoPhan_KNL - " + DateTime.Now.Date.ToString("dd/MM/yyyy") + ".xlsx";
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
                }
                else
                {

                    //Worksheet.Cell("A1").Value = "Ngày xuất file: " + DateTime.Now.Date.ToString("dd/MM/yyyy");
                    Workbook.SaveAs(fileNamemaunew);
                    byte[] fileBytes = System.IO.File.ReadAllBytes(fileNamemaunew);
                    string fileName = "BoPhan_KNL - " + DateTime.Now.Date.ToString("dd/MM/yyyy") + ".xlsx";
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
                }

            }
            catch (Exception ex)
            {
                TempData["msg"] = "<script>alert('" + ex + "');window.location.href = '/Everyday/'</script>";
                return View(TempData);
            }
        }

        private List<ExportNhanVienKNL> GetNhanVienKNL()
        {
            //var res = (from a in db.NhanViens.Where(x=>x.IDTinhTrangLV ==1)
            //           join b in db.ViTriKNLs on a.IDVTKNL equals b.IDVT into ulb
            //           from b in ulb.DefaultIfEmpty()
            //           join d in db.PhongBans
            //           on b.IDPB equals d.IDPhongBan
            //           join e in db.KNL_PhanXuong
            //            on b.IDPX equals e.ID into ul
            //           from e in ul.DefaultIfEmpty()
            //           join f in db.KNL_Nhom
            //           on b.IDNhom equals f.IDNhom into uls
            //           from f in uls.DefaultIfEmpty()
            //           join g in db.KNL_To
            //           on b.IDTo equals g.IDTo into ulk
            //           from g in ulk.DefaultIfEmpty()
            //           join h in db.Kips
            //           on a.IDKip equals h.IDKip into ulkh
            //           from h in ulkh.DefaultIfEmpty()
            //           select new ExportNhanVienKNL
            //           {
            //               MaNV = a.MaNV,
            //               IDNV = a.ID,
            //               IDVT = b.IDVT,
            //               TenViTri = b.TenViTri,
            //               TenNV = a.HoTen,
            //               IDNhom = b.IDNhom,
            //               TenNhom =f.TenNhom,
            //               IDTo =b.IDTo,
            //               TenTo =g.TenTo,
            //               IDPB = b.IDPB,
            //               IDPX =b.IDPX,
            //               TenPX =e.TenPX,
            //               IDKip = a.IDKip,
            //               TenKip = h.TenKip,
            //               MaViTri = b.MaViTri,
            //               TenPhongBan = d.TenPhongBan,
            //           }).ToList();

            var res = (from a in db.NhanVien_Export_ALL()
                       select new ExportNhanVienKNL
                       {
                           MaNV = a.MaNV,
                           IDNV = a.ID,
                           IDVT = a.IDVT,
                           TenViTri = a.TenViTri,
                           TenNV = a.HoTen,
                           //IDNhom = a.IDNhom,
                           TenNhom = a.TenNhom,
                           //IDTo = b.IDTo,
                           TenTo = a.TenTo,
                           //IDPB = a.IDPB,
                           //IDPX = b.IDPX,
                           TenPX = a.TenPX,
                           IDKip = a.IDKip,
                           TenKip = a.TenKip,
                           MaViTri = a.MaViTri,
                           TenPhongBan = a.TenPhongBan,
                       }).ToList();
            return res;
        }

        private List<ExportBoPhanKNL> GetBoPhanKNL()
        {
            var res = (from a in db.PhongBan_ExportKNL()
                       select new ExportBoPhanKNL
                       {
                           TenPhongBan = a.TenPhongBan,
                           MaPhongBan =a.MaPB,
                           TotalVT = a.ToTalVT,
                           VTDDG =a.VTDDG,
                           TotalNV =a.TotalNV,
                           NVDDG =a.NVDDG
                       }).ToList();
            return res;
        }

        private List<ExportNhanVienKQKNL> GetNhanVienKNL1(int? IDPB, int? IDPX, int? IDTo, int? IDNhom)
        {
            if (IDPB == null) IDPB = 0;
            var allKQ = db.KNL_KQ
                        .Where(x => x.NgayDG.HasValue && x.ThangDG.HasValue)
                        .Select(x => new {
                            x.IDNV,
                            x.VTID,
                            x.ThangDG,
                            x.DiemDG,
                            x.DiemDM,
                            x.NgayDG
                        })
                        .ToList();
            var nhanVien = db.NhanVien_ExportKQKNL(IDPB).ToList();
            var docKNL = db.KNL_DocBangKNL.ToList();
            var res = (from a in nhanVien
                       let kq = allKQ.Where(x => x.IDNV == a.ID &&
                               x.VTID == a.IDVT &&
                               x.ThangDG.HasValue &&
                               a.NgayDG.HasValue &&
                               x.ThangDG.Value.Year == a.NgayDG.Value.Year &&
                               x.ThangDG.Value.Month == a.NgayDG.Value.Month).ToList()
                       let c = docKNL.Where(x => x.IDNV == a.ID && x.ID_ViTriKNL == a.IDVT)
                       select new ExportNhanVienKQKNL
                       {
                           MaNV = a.MaNV,
                           IDNV = a.ID,
                           IDVT = a.IDVT,
                           TenViTri = a.TenViTri,
                           TenNV = a.HoTen,
                           IDNhom = a.IDNhom,
                           TenNhom = a.TenNhom,
                           IDTo = a.IDTo,
                           TenTo = a.TenTo,
                           IDPX = a.IDPX,
                           TenPX = a.TenPX,
                           IDKip = a.IDKip,
                           TenKip = a.TenKip,
                           TenPhongBan = a.TenPhongBan,
                           TotalNL = a.TotalNL,
                           DAT = a?.DAT ?? 0,
                           SLQuaHanDAT = kq?.Count(x => x.DiemDG == x.DiemDM && x.NgayDG.Value.AddMonths(6) <= DateTime.Now),
                           VUOT = a?.VUOT ?? 0,
                           SLQuaHanVuot = kq?.Count(x => x.DiemDG > x.DiemDM && x.NgayDG.Value.AddMonths(6) <= DateTime.Now),
                           KDAT = a?.KDAT ?? 0,
                           SLQuaHanKDAT = kq?.Count(x => x.DiemDG < x.DiemDM && x.NgayDG.Value.AddMonths(3) <= DateTime.Now),
                           KDGIA = a?.NODG ?? 0,
                           CHUADG = a?.CHUADG ?? 0,
                           NgayDGHN = a?.NgayDG,
                           TotalDocKNL = c?.Count(),
                           KDATTu = a?.KDATTUDG ?? 0,
                           DATTu = a?.DATTUDG ?? 0,
                           VUOTTu = a?.VUOTTUDG ?? 0,
                           CHUADGTu = a?.CHUADGTuDG ?? 0,
                           NgayDGTu = a?.NgayTuDGGN,
                           KDATCap1 = a?.KDATTUDGLan1 ?? 0,
                           DATCap1 = a?.DATTUDGLan1 ?? 0,
                           VUOTCap1 = a?.VUOTTUDGLan1 ?? 0,
                           CHUADGCap1 = a?.CHUADGTuDGLan1 ?? 0,
                           NgayDGCap1 = a?.NgayDGGNLan1,
                       }).ToList();
            if (IDPX != null) res = res.Where(x => x.IDPX == IDPX).ToList();
            if (IDNhom != null) res = res.Where(x => x.IDNhom == IDNhom).ToList();
            if (IDTo != null) res = res.Where(x => x.IDTo == IDTo).ToList();
            return res;
        }


        public ActionResult ImportExcel()
        {
            db.Configuration.ProxyCreationEnabled = false;
            //List<NoiDungDT> dt = db.NoiDungDTs.ToList();
            //ViewBag.IDND = new SelectList(dt, "IDND", "NoiDung");

            //List<DanhSachDA> ds = db_context.DanhSachDAs.ToList();
            //ViewBag.IDDAĐung = new SelectList(ds, "IDDSĐA", "TenĐA");

            //List<CTLVDT> lvct = db_context.CTLVDTs.ToList();
            //ViewBag.IDCTLVDT = new SelectList(lvct, "IDCTLVDT", "TenCTLVDT");

            return PartialView();
        }
        [HttpPost]
        public ActionResult ImportExcel(FCheckValidation _DO)
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

                    try
                    {
                        for (int i = 4; i < dt.Rows.Count; i++)
                        {
                            string MaNV = dt.Rows[i][1].ToString().Trim();
                            var check = db.NhanViens.Where(x => x.MaNV == MaNV && x.IDTinhTrangLV == 1).Count();
                            int? IDVT = ToNullableInt(dt.Rows[i][4].ToString().Trim());

                            if (IDVT != null && check>0) db.Nhanvien_update_IDKNL(MaNV, IDVT);
                            //db_context.CauHoi_insert(NoiDungCH, DapAnA, DapAnB, DapAnC, DapAnD, Convert.ToInt32(DapAnDung), _DO.IDND, MyAuthentication.ID);

                        }

                        TempData["msgSuccess"] = "<script>alert('Import dữ liệu thành công');</script>";
                    }
                    catch (Exception ex)
                    {
                        TempData["msgError"] = "<script>alert('Có lỗi khi Upload: " + ex + "');</script>";
                    }
                }
                else
                {
                    TempData["msgSuccess"] = "<script>alert('Vui lòng nhập file Import');</script>";
                }
            }
            else
            {
                TempData["msgSuccess"] = "<script>alert('Vui lòng nhập file Import');</script>";
            }

            return RedirectToAction("ListNVKNL", "FPosition");
        }
        public FileResult DownloadExcel()
        {
            string path = "/App_Data/BM_NVKNL.xlsx";
            return File(path, "application/vnd.ms-excel", "BM_NVKNL.xlsx");
        }

        public ActionResult FBMView(int? IDVT)
        {
            var vt = db.VitriKNL_searchByIDVT(IDVT).FirstOrDefault();
            ViewBag.TenViTri = vt?.TenViTri ?? "";
            ViewBag.IDVT = IDVT;
            ViewBag.TenPB = vt?.TenPhongBan ?? "";

            var res = (from a in db.KhungNangLuc_SearchByIDVT(IDVT).Where(x=>x.IsDuyet == 1)
                       select new FValueValidation
                       {
                           IDNL = a.IDNL,
                           TenNL = a.TenNL,
                           IDLoaiNL = a.IDLoaiNL,
                           IDVT = a.IDVT,
                           TenViTri = a.TenViTri,
                           IDPB = a.IDPB,
                           DinhMuc = a.IsDanhGia != 0 ? a.DinhMuc : 0,
                           IsDanhGia = a.IsDanhGia,
                       }).ToList().OrderBy(x => x.OrderBy);

            List<LoaiKNL> loaiNL = db.LoaiKNLs.Where(x => x.IDVT == IDVT && x.IDLoai != 1 && x.IDLoai != 2).OrderBy(x => x.OrderBy).ToList();
            ViewBag.IDLoaiNL = new SelectList(loaiNL, "IDLoai", "TenLoai");

            return View(res.ToList());
        }


        public ActionResult ExportExcelNVien(object sender, EventArgs e,int? IDPB)
        {
            
            if (IDPB is null) IDPB = 0;
            //string constr = ConfigurationManager.ConnectionStrings["ELEARNINGEntities"].ConnectionString;
            string constr = ConfigurationManager.AppSettings["LinkExport"];
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("NhanVien_ExportExcel", con)
                {
                    CommandType = CommandType.StoredProcedure
                })
                {
                    cmd.Parameters.AddWithValue("@IDPB", IDPB);
                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {
                        cmd.Connection = con;
                        sda.SelectCommand = cmd;
                        using (DataTable dt = new DataTable())
                        {
                            sda.Fill(dt);
                            using (XLWorkbook wb = new XLWorkbook())
                            {
                                wb.Worksheets.Add(dt, "NhanVien");

                                Response.Clear();
                                Response.Buffer = true;
                                Response.Charset = "";
                                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                                Response.AddHeader("content-disposition", "attachment;filename=DanhSachNhanVien.xlsx");
                                using (MemoryStream MyMemoryStream = new MemoryStream())
                                {
                                    wb.SaveAs(MyMemoryStream);
                                    MyMemoryStream.WriteTo(Response.OutputStream);
                                    Response.Flush();
                                    Response.End();
                                }
                            }
                        }
                    }
                    //cmd.Dispose();
                }
            }
            return View(TempData);
        }

        public ActionResult ExportExcelNoQuery(object sender, EventArgs e, string StoreProcuduce, string nameFileExcel)
        {
            //string constr = ConfigurationManager.ConnectionStrings["ELEARNINGEntities"].ConnectionString;
            string constr = ConfigurationManager.AppSettings["LinkExport"];
            string fileExport = "attachment;filename="+ nameFileExcel + ".xlsx";
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(StoreProcuduce, con)
                {
                    CommandType = CommandType.StoredProcedure
                })
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {
                        cmd.Connection = con;
                        sda.SelectCommand = cmd;
                        using (DataTable dt = new DataTable())
                        {
                            sda.Fill(dt);
                            using (XLWorkbook wb = new XLWorkbook())
                            {
                                wb.Worksheets.Add(dt, "DanhSach");

                                Response.Clear();
                                Response.Buffer = true;
                                Response.Charset = "";
                                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                                Response.AddHeader("content-disposition", fileExport);
                                using (MemoryStream MyMemoryStream = new MemoryStream())
                                {
                                    wb.SaveAs(MyMemoryStream);
                                    MyMemoryStream.WriteTo(Response.OutputStream);
                                    Response.Flush();
                                    Response.End();
                                }
                            }
                        }
                    }
                }
            }
            return View(TempData);
        }


        public ActionResult ExportToBMKNL(int? IDPB, int? IDPX, int? IDTo, int? IDNhom, int? IDVT)
        {
            try
            {
                string fileNamemau = AppDomain.CurrentDomain.BaseDirectory + @"App_Data\HPDQ_QT04BM12.xlsx";
                string fileNamemaunew = AppDomain.CurrentDomain.BaseDirectory + @"App_Data\HPDQ_QT04BM12_Temp.xlsx";
                XLWorkbook Workbook = new XLWorkbook(fileNamemau);
                IXLWorksheet Worksheet = Workbook.Worksheet("KNL");

                if (IDPB == null) IDPB = 0;
                var listVT = db.ViTriKNLs.Where(x => x.IDPB == IDPB).ToList();
                if (IDPX != null) listVT = listVT.Where(x => x.IDPX == IDPX).ToList();
                if (IDTo != null) listVT = listVT.Where(x => x.IDTo == IDTo).ToList();
                if (IDNhom != null) listVT = listVT.Where(x => x.IDNhom == IDNhom).ToList();
                if (IDVT != null) listVT = db.ViTriKNLs.Where(x => x.IDVT == IDVT).ToList();
                int row = 7;
                if (listVT.Count > 0)
                {
                    var tem = 0;
                    foreach(var vt in listVT)
                    {
                        var dataKNL = GetDataExportKNL(vt.IDVT);
                        if (dataKNL != null)
                        {
                            if (tem > 0)
                            {
                                var addressPicture = Worksheet.Pictures;

                                var imagePath = AppDomain.CurrentDomain.BaseDirectory + @"Content\assets\images\logoExport.png";

                                var image = Worksheet.AddPicture(imagePath)
                                    .MoveTo(Worksheet.Cell("A" + row))
                                    .Scale(0.3); // optional: resize picture
                                var row4 = row + 3;
                                Worksheet.Range("A" + row + ":F" + row4).Merge();
                                Worksheet.Range("G" + row).Value = Worksheet.Cell("G1");
                                Worksheet.Range("G" + row + ":I" + row4).Merge();
                                Worksheet.Rows(row, row4).Height = 16;
                                row = row + 4;

                                IXLRow headerRow1 = Worksheet.Row(5);
                                IXLRow headerRow2 = Worksheet.Row(6);
                                Worksheet.Range("A" + row).Value = headerRow1;
                                Worksheet.Row(row).Height = 16;
                                row = row + 1;
                                Worksheet.Range("A" + row).Value = headerRow2;
                                //Worksheet.Row(row).Height = 16;
                                var row1 = row - 1;
                                //Worksheet.Range("A" + row + ":I" + row).Merge();
                                //Worksheet.Range("A" + row1 + ":I" + row1).Merge();
                                Worksheet.Range("A" + row1 + ":I" + row).Merge();

                                row = row + 1;

                            }
                            var rowstart = row;
                            Worksheet.Cell(row, "A").Value = "1. Thông tin chung ";
                            Worksheet.Cell(row, "A").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                            Worksheet.Cell(row, "A").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                           

                            //Worksheet.Cell(row, "A").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            Worksheet.Range("A" + row + ":I" + row).Merge();
                            row = row + 1;
                            Worksheet.Cell(row, "A").Value = "1.1. Vị trí công việc: "+dataKNL.TenViTri;
                            Worksheet.Cell(row, "A").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                            Worksheet.Cell(row, "A").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                            //Worksheet.Cell(row, "A").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            Worksheet.Range("A"+row+":I"+row).Merge();
                            row = row + 1;
                            Worksheet.Cell(row, "A").Value = "1.2. Bộ phận: " + dataKNL.TenPhongBan;
                            Worksheet.Cell(row, "A").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                            Worksheet.Cell(row, "A").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                            //Worksheet.Cell(row, "A").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            Worksheet.Range("A" + row + ":I" + row).Merge();
                            row = row + 1;
                            Worksheet.Cell(row, "A").Value = "1.3. Lần cập nhật thứ: 1 ";
                            Worksheet.Cell(row, "A").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                            Worksheet.Cell(row, "A").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                            //Worksheet.Cell(row, "A").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            Worksheet.Range("A" + row + ":I" + row).Merge();
                            row = row + 1;
                            Worksheet.Cell(row, "A").Value = "2. Khung năng lực chi tiết ";
                            Worksheet.Cell(row, "A").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                            Worksheet.Cell(row, "A").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                            //Worksheet.Cell(row, "A").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            Worksheet.Range("A" + row + ":I" + row).Merge();
                            Worksheet.Row(row).Height = 16;
                            var row2 = row - 4;
                            Worksheet.Range("A" + row2 + ":A" + row).Style.Font.Bold = true;
                            row = row + 1;

                            if (tem > 0)
                            {
                                IXLRow headerRow1 = Worksheet.Row(12);
                                IXLRow headerRow2 = Worksheet.Row(13);
                                Worksheet.Range("A" + row).Value = headerRow1;
                                Worksheet.Row(row).Height = 39.75;
                                row = row + 1;
                                Worksheet.Range("A" + row).Value = headerRow2;
                                Worksheet.Row(row).Height = 39.75;
                                var row1 = row - 1;
                                Worksheet.Range("A" + row1 + ":A" + row).Merge();
                                Worksheet.Range("B" + row1 + ":B" + row).Merge();

                                row = row + 1;

                            }
                            else { row = row + 2; }

                            Worksheet.Cell(row, "A").Value = "I";
                            Worksheet.Cell(row, "A").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            Worksheet.Cell(row, "A").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                            Worksheet.Cell(row, "A").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            Worksheet.Cell(row, "A").Style.Font.Bold = true;

                            Worksheet.Cell(row, "B").Value = "I.Năng lực chung";
                            Worksheet.Cell(row, "B").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                            Worksheet.Cell(row, "B").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                            Worksheet.Cell(row, "B").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            Worksheet.Cell(row, "B").Style.Font.Bold = true;

                            Worksheet.Row(row).Height = 16;
                            Worksheet.Range("B" + row + ":I" + row).Merge();
                            Worksheet.Range("B" + row + ":I" + row).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            row = row + 1;
                            var b = 0;
                            foreach(var data in dataKNL.LSFValue.Where(x=>x.IDLoaiNL ==1))
                            {
                                Worksheet.Cell(row, "A").Value = b+1;
                                Worksheet.Cell(row, "A").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                Worksheet.Cell(row, "A").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                                Worksheet.Cell(row, "A").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                                Worksheet.Cell(row, "B").Value = data.TenNL;
                                Worksheet.Cell(row, "B").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                                Worksheet.Cell(row, "B").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                                Worksheet.Cell(row, "B").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                                Worksheet.Cell(row, "C").Value = data.DinhMuc ==0?"X":"";
                                Worksheet.Cell(row, "C").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                Worksheet.Cell(row, "C").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                                Worksheet.Cell(row, "C").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                                Worksheet.Cell(row, "D").Value = data.DinhMuc == 1 ? "X" : "";
                                Worksheet.Cell(row, "D").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                Worksheet.Cell(row, "D").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                                Worksheet.Cell(row, "D").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                                Worksheet.Cell(row, "E").Value = data.DinhMuc == 2 ? "X" : "";
                                Worksheet.Cell(row, "E").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                Worksheet.Cell(row, "E").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                                Worksheet.Cell(row, "E").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                                Worksheet.Cell(row, "F").Value = data.DinhMuc == 3 ? "X" : "";
                                Worksheet.Cell(row, "F").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                Worksheet.Cell(row, "F").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                                Worksheet.Cell(row, "F").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                                Worksheet.Cell(row, "G").Value = data.DinhMuc == 4 ? "X" : "";
                                Worksheet.Cell(row, "G").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                Worksheet.Cell(row, "G").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                                Worksheet.Cell(row, "G").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                                Worksheet.Cell(row, "H").Value = data.DinhMuc == 5 ? "X" : "";
                                Worksheet.Cell(row, "H").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                Worksheet.Cell(row, "H").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                                Worksheet.Cell(row, "H").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                                Worksheet.Cell(row, "I").Value = data.DinhMuc == 6 ? "X" : "";
                                Worksheet.Cell(row, "I").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                Worksheet.Cell(row, "I").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                                Worksheet.Cell(row, "I").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                                //Worksheet.Row(row).Height = 16;
                                row =row + 1;
                                b= b + 1;

                            }
                            Worksheet.Cell(row, "A").Value = "II";
                            Worksheet.Cell(row, "A").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            Worksheet.Cell(row, "A").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                            Worksheet.Cell(row, "A").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            Worksheet.Cell(row, "A").Style.Font.Bold = true;

                            Worksheet.Cell(row, "B").Value = "Năng lực chuyên môn";
                            Worksheet.Cell(row, "B").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                            Worksheet.Cell(row, "B").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                            Worksheet.Cell(row, "B").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            Worksheet.Range("B" + row + ":I" + row).Merge();
                            Worksheet.Range("B" + row + ":I" + row).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                            Worksheet.Row(row).Height = 16;
                            Worksheet.Cell(row, "B").Style.Font.Bold = true;
                            row = row + 1;
                            var k = 0;
                            foreach(var loaiNL in dataKNL.LSLoaiKNL)
                            {
                                Worksheet.Cell(row, "A").Value = k + 1;
                                Worksheet.Cell(row, "A").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                Worksheet.Cell(row, "A").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                                Worksheet.Cell(row, "A").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                                Worksheet.Cell(row, "A").Style.Font.Bold = true;

                                Worksheet.Cell(row, "B").Value = loaiNL.TenLoai;
                                Worksheet.Cell(row, "B").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                                Worksheet.Cell(row, "B").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                                Worksheet.Cell(row, "B").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                                Worksheet.Range("B" + row + ":I" + row).Merge();
                                Worksheet.Range("B" + row + ":I" + row).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                                Worksheet.Cell(row, "B").Style.Font.Bold = true;
                                row = row + 1;
                                var de = 0;
                                foreach(var data in dataKNL.LSFValue.Where(x=>x.IDLoaiNL == loaiNL.IDLoai))
                                {
                                    Worksheet.Cell(row, "A").Value = (k+1)+"."+ (de + 1);
                                    Worksheet.Cell(row, "A").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                    Worksheet.Cell(row, "A").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                                    Worksheet.Cell(row, "A").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                                    Worksheet.Cell(row, "B").Value = data.TenNL;
                                    Worksheet.Cell(row, "B").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                                    Worksheet.Cell(row, "B").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                                    Worksheet.Cell(row, "B").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                                    Worksheet.Cell(row, "C").Value = data.DinhMuc == 0 ? "X" : "";
                                    Worksheet.Cell(row, "C").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                    Worksheet.Cell(row, "C").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                                    Worksheet.Cell(row, "C").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                                    Worksheet.Cell(row, "D").Value = data.DinhMuc == 1 ? "X" : "";
                                    Worksheet.Cell(row, "D").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                    Worksheet.Cell(row, "D").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                                    Worksheet.Cell(row, "D").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                                    Worksheet.Cell(row, "E").Value = data.DinhMuc == 2 ? "X" : "";
                                    Worksheet.Cell(row, "E").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                    Worksheet.Cell(row, "E").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                                    Worksheet.Cell(row, "E").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                                    Worksheet.Cell(row, "F").Value = data.DinhMuc == 3 ? "X" : "";
                                    Worksheet.Cell(row, "F").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                    Worksheet.Cell(row, "F").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                                    Worksheet.Cell(row, "F").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                                    Worksheet.Cell(row, "G").Value = data.DinhMuc == 4 ? "X" : "";
                                    Worksheet.Cell(row, "G").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                    Worksheet.Cell(row, "G").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                                    Worksheet.Cell(row, "G").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                                    Worksheet.Cell(row, "H").Value = data.DinhMuc == 5 ? "X" : "";
                                    Worksheet.Cell(row, "H").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                    Worksheet.Cell(row, "H").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                                    Worksheet.Cell(row, "H").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                                    Worksheet.Cell(row, "I").Value = data.DinhMuc == 6 ? "X" : "";
                                    Worksheet.Cell(row, "I").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                    Worksheet.Cell(row, "I").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                                    Worksheet.Cell(row, "I").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                                    //Worksheet.Row(row).Height = 16;
                                    row = row + 1;
                                    de++;
                                }
                                k++;
                            }
                            Worksheet.Cell(row, "A").Value = "III";
                            Worksheet.Cell(row, "A").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            Worksheet.Cell(row, "A").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                            Worksheet.Cell(row, "A").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            Worksheet.Cell(row, "A").Style.Font.Bold = true;

                            Worksheet.Cell(row, "B").Value = "Năng lực quản lý";
                            Worksheet.Cell(row, "B").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                            Worksheet.Cell(row, "B").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                            Worksheet.Cell(row, "B").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            Worksheet.Row(row).Height = 16;
                            Worksheet.Range("B" + row + ":I" + row).Merge();
                            Worksheet.Range("B" + row + ":I" + row).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            Worksheet.Cell(row, "B").Style.Font.Bold = true;
                            row = row + 1;
                            var ll = 0;
                            foreach (var data in dataKNL.LSFValue.Where(x => x.IDLoaiNL == 2))
                            {
                                Worksheet.Cell(row, "A").Value = ll + 1;
                                Worksheet.Cell(row, "A").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                Worksheet.Cell(row, "A").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                                Worksheet.Cell(row, "A").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                                Worksheet.Cell(row, "B").Value = data.TenNL;
                                Worksheet.Cell(row, "B").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                                Worksheet.Cell(row, "B").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                                Worksheet.Cell(row, "B").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                                Worksheet.Cell(row, "C").Value = data.DinhMuc == 0 ? "X" : "";
                                Worksheet.Cell(row, "C").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                Worksheet.Cell(row, "C").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                                Worksheet.Cell(row, "C").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                                Worksheet.Cell(row, "D").Value = data.DinhMuc == 1 ? "X" : "";
                                Worksheet.Cell(row, "D").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                Worksheet.Cell(row, "D").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                                Worksheet.Cell(row, "D").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                                Worksheet.Cell(row, "E").Value = data.DinhMuc == 2 ? "X" : "";
                                Worksheet.Cell(row, "E").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                Worksheet.Cell(row, "E").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                                Worksheet.Cell(row, "E").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                                Worksheet.Cell(row, "F").Value = data.DinhMuc == 3 ? "X" : "";
                                Worksheet.Cell(row, "F").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                Worksheet.Cell(row, "F").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                                Worksheet.Cell(row, "F").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                                Worksheet.Cell(row, "G").Value = data.DinhMuc == 4 ? "X" : "";
                                Worksheet.Cell(row, "G").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                Worksheet.Cell(row, "G").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                                Worksheet.Cell(row, "G").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                                Worksheet.Cell(row, "H").Value = data.DinhMuc == 5 ? "X" : "";
                                Worksheet.Cell(row, "H").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                Worksheet.Cell(row, "H").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                                Worksheet.Cell(row, "H").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                                Worksheet.Cell(row, "I").Value = data.DinhMuc == 6 ? "X" : "";
                                Worksheet.Cell(row, "I").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                Worksheet.Cell(row, "I").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                                Worksheet.Cell(row, "I").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;


                                //Worksheet.Row(row).Height = 16;
                                row = row + 1;
                                ll = ll + 1;

                            }

                            row = row + 1;
                            Worksheet.Cell(row, "C").Value = "Quảng Ngãi, ngày…… tháng…… năm………";
                            Worksheet.Cell(row, "C").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            Worksheet.Cell(row, "C").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                            //Worksheet.Cell(row, "C").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            Worksheet.Cell(row, "C").Style.Font.Italic = true;
                            Worksheet.Range("C" + row + ":I" + row).Merge();
                            row = row + 1;
                            Worksheet.Cell(row, "C").Value = "T/PBP";
                            Worksheet.Cell(row, "C").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            Worksheet.Cell(row, "C").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                            //Worksheet.Cell(row, "C").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            Worksheet.Cell(row, "C").Style.Font.Bold = true;
                            Worksheet.Range("C" + row + ":I" + row).Merge();

                            Worksheet.Range("A" + rowstart + ":I" + row).Style.Font.FontSize = 13;
                        }
                        tem = tem + 1;
                        row = row + 5;
                    }

                    Workbook.SaveAs(fileNamemaunew);
                    byte[] fileBytes = System.IO.File.ReadAllBytes(fileNamemaunew);
                    string fileName = "BMKNL - " + DateTime.Now.Date.ToString("dd/MM/yyyy") + ".xlsx";
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
                }
                else
                {
                    byte[] fileBytes = System.IO.File.ReadAllBytes(fileNamemaunew);
                    string fileName = "BMKNL - " + DateTime.Now.Date.ToString("dd/MM/yyyy") + ".xlsx";
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
                }

            }
            catch (Exception ex)
            {
                TempData["msg"] = "<script>alert('" + ex + "');window.location.href = '/Everyday/'</script>";
                return View(TempData);
            }
        }

        private FExportKNLValidation GetDataExportKNL( int? IDVT)
        {
            var res = new FExportKNLValidation();
            var vt = db.VitriKNL_searchByIDVT(IDVT).FirstOrDefault();

            var FKNL = (from a in db.KhungNangLuc_SearchByIDVT(IDVT).Where(x=>x.IsDuyet == 1)
                       select new FValueValidation
                       {
                           IDNL = a.IDNL,
                           TenNL = a.TenNL,
                           IDLoaiNL = a.IDLoaiNL,
                           IDVT = a.IDVT,
                           TenViTri = vt.TenViTri+vt.TenTo != null? "-"+vt.TenTo:""+ vt.TenNhom != null ?"-"+ vt.TenNhom:"" + vt.TenPX!= null? "-" +vt.TenPX:"",
                           IDPB = a.IDPB,
                           DinhMuc = a.IsDanhGia != 0 ? a.DinhMuc : 0,
                           IsDanhGia = a.IsDanhGia,
                       }).ToList().OrderBy(x => x.OrderBy);
            var distinctIDLoaiNLs = FKNL.Where(x => x.IDLoaiNL != 1 && x.IDLoaiNL != 2)
                   .Select(x => x.IDLoaiNL)
                   .Distinct()
                   .ToList();

            List<LoaiKNL> loaiNL = db.LoaiKNLs.Where(x => distinctIDLoaiNLs.Contains(x.IDLoai)).OrderBy(x => x.OrderBy).ToList();
            res.LSLoaiKNL = loaiNL;
            res.LSFValue = FKNL.ToList();
            //res.TenViTri = vt.TenViTri + "-" + vt.TenTo + "-" + vt.TenNhom + "-" + vt.TenPX;
            res.TenViTri = vt.TenViTri;
            if(!String.IsNullOrEmpty(vt.TenTo)) res.TenViTri += "-"+vt.TenTo;
            if (!String.IsNullOrEmpty(vt.TenNhom)) res.TenViTri += "-" + vt.TenNhom;
            if (!String.IsNullOrEmpty(vt.TenPX)) res.TenViTri += "-" + vt.TenPX;
            res.TenPhongBan = vt.TenPhongBan;

            return res;
        }

        public ActionResult GetDataNVExport(JqueryDatatableParam param, int? IDPB,int? IDPX, int?  IDNhom, int? IDTo)
        {
            if (IDPB == null) IDPB = 0;
            var employees = (from a in db.NhanVien_ExportKQKNL(IDPB)
                       select new ExportNhanVienKQKNL
                       {
                           MaNV = a.MaNV,
                           IDNV = a.ID,
                           IDVT = a.IDVT,
                           TenViTri = a.TenViTri,
                           TenNV = a.HoTen,
                           IDNhom = a.IDNhom,
                           TenNhom = a.TenNhom,
                           IDTo = a.IDTo,
                           TenTo = a.TenTo,
                           IDPX = a.IDPX,
                           TenPX = a.TenPX,
                           IDKip = a.IDKip,
                           TenKip = a.TenKip,
                           TenPhongBan = a.TenPhongBan,
                           TotalNL = a.TotalNL,
                           DAT = a.DAT,
                           VUOT = a.VUOT,
                           KDAT = a.KDAT,
                           KDGIA = a.NODG,
                           CHUADG = a.CHUADG,
                           NgayDG = a.NgayDG == null ? "" : String.Format("{0:dd/MM/yyyy}", a?.NgayDG),
                           HanDG = a.KDAT > 0?"Cần đánh giá lại sau " + (((DateTime)a.NgayDG).AddMonths(6) - DateTime.Now).Days + " ngày tới" : ""  ,
                           NgayHanDG = a.KDAT > 0 ? String.Format("{0:dd/MM/yyyy}", ((DateTime)a.NgayDG).AddMonths(6)) : "",
                       }).ToList();

            if (IDPX != null) employees = employees.Where(x => x.IDPX == IDPX).ToList();
            if (IDNhom != null) employees = employees.Where(x => x.IDNhom == IDNhom).ToList();
            if (IDTo != null) employees = employees.Where(x => x.IDTo == IDTo).ToList();

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                employees = employees.Where(x => x.TenNV.ToLower().Contains(param.sSearch.ToLower())
                                              || x.MaNV.ToLower().Contains(param.sSearch.ToLower())).ToList();
            }

            var sortColumnIndex = Convert.ToInt32(HttpContext.Request.QueryString["iSortCol_0"]);
            var sortDirection = HttpContext.Request.QueryString["sSortDir_0"];

            if (sortColumnIndex == 0)
            {
                employees = sortDirection == "asc" ? employees.OrderBy(c => c.TenNV).ToList() : employees.OrderByDescending(c => c.TenNV).ToList();
            }
            else if (sortColumnIndex == 1)
            {
                employees = sortDirection == "asc" ? employees.OrderBy(c => c.MaNV).ToList() : employees.OrderByDescending(c => c.MaNV).ToList();
            }
            else if (sortColumnIndex == 5)
            {
                employees = sortDirection == "asc" ? employees.OrderBy(c => c.IDPB).ToList() : employees.OrderByDescending(c => c.IDPB).ToList();
            }
            //else
            //{
            //    Func<NhanVien, string> orderingFunction = e => sortColumnIndex == 0 ? e.HoTen :
            //                                                   sortColumnIndex == 1 ? e.MaNV :
            //                                                   e.DiaChi;

            //    //employees = sortDirection == "asc" ? employees.OrderBy(orderingFunction).ToList() : employees.OrderByDescending(orderingFunction).ToList();
            //}

            var displayResult = employees.Skip(param.iDisplayStart)
             .Take(param.iDisplayLength).ToList();
            var totalRecords = employees.Count();

            var jsonResult = Json(new
            {
                param.sEcho,
                iTotalRecords = totalRecords,
                iTotalDisplayRecords = totalRecords,
                aaData = displayResult,
            }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
            //return Json(new
            //{
            //    param.sEcho,
            //    iTotalRecords = totalRecords,
            //    iTotalDisplayRecords = totalRecords,
            //    aaData = displayResult,
            //}, JsonRequestBehavior.AllowGet);

        }
        public ActionResult GetDataNVVTKNL(JqueryDatatableParam param)
        {
            var employees = (from a in db.NhanVien_Export_ALL()
                       select new ExportNhanVienKNL
                       {
                           MaNV = a.MaNV,
                           IDNV = a.ID,
                           IDVT = a.IDVT,
                           TenViTri = a.TenViTri,
                           TenNV = a.HoTen,
                           //IDNhom = a.IDNhom,
                           TenNhom = a.TenNhom,
                           //IDTo = b.IDTo,
                           TenTo = a.TenTo,
                           //IDPB = a.IDPB,
                           //IDPX = b.IDPX,
                           TenPX = a.TenPX,
                           IDKip = a.IDKip,
                           TenKip = a.TenKip,
                           MaViTri = a.MaViTri,
                           TenPhongBan = a.TenPhongBan,
                           TenPhongBanKNL =a.TenPBKNL
                       }).ToList();

            var displayResult = employees.Skip(param.iDisplayStart)
             .Take(param.iDisplayLength).ToList();
            var totalRecords = employees.Count();

            var jsonResult = Json(new
            {
                param.sEcho,
                iTotalRecords = totalRecords,
                iTotalDisplayRecords = totalRecords,
                aaData = displayResult,
            }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;

        }

        public ActionResult ChangePositon(int? page, string search, int? IDPB, int? IDPX, int? IDNhom, int? IDKhoi, int? IDTo)
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
            int idpb = MyAuthentication.IDPhongban;
            var manv = MyAuthentication.Username;
            var ThangDG = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            if (IDPB == null) IDPB = 0;


            var res = (from a in db.VitriKNL_Select(IDPB).Where(x=>x.IDPhongBan ==IDPB)
                       select new ViTriKNLValidation
                       {
                           IDVT = a.IDVT,
                           TenViTri = a.TenViTri,
                           IDPB = a.IDPhongBan,
                           TenPhongBan = a.TenPhongBan,
                           MaViTri = a.MaViTri,
                           IDKhoi = a.IDKhoi,
                           TenKhoi = a.TenKhoi,
                           IDPX = a.IDPX,
                           TenPX = a.TenPX,
                           IDNhom = a.IDNhom,
                           TenNhom = a.TenNhom,
                           IDTo = a.IDTo,
                           TenTo = a.TenTo,
                           FilePath = a.FilePath,
                           CountNV = a.SLNV == null ? 0 : a.SLNV,
                           CountKNL = a.SLNL,
                           CountDGTC = a.SLDGTC,
                           CountNVDDG = a.SLNVDDG
                       }).OrderBy(x => x.IDPB).ToList();
            //foreach (var k in res)
            //{
            //    //var listNVVT = KQKNLThang.Where(x => x.IDVTKNL == k.IDVT).ToList();
            //    k.CountNVDDG = KQKNLThang.Where(x => x.IDVTKNL == k.IDVT).Count();
            //}

            List<PhongBan> dt = db.PhongBans.ToList();
            //if (ListQuyen.Contains(CONSTKEY.V_BP)) dt = dt.Where(x => x.IDPhongBan == idpb).ToList();
            ViewBag.IDPB = new SelectList(dt, "IDPhongBan", "TenPhongBan");

            if (IDPB == null) IDPB = 0;
            if (IDPB != 0) res = res.Where(x => x.IDPB == IDPB).ToList();
            if (IDPX != null) res = res.Where(x => x.IDPX == IDPX).ToList();
            if (IDKhoi != null) res = res.Where(x => x.IDKhoi == IDKhoi).ToList();
            if (IDNhom != null) res = res.Where(x => x.IDNhom == IDNhom).ToList();
            if (IDTo != null) res = res.Where(x => x.IDTo == IDTo).ToList();
            if (!String.IsNullOrEmpty(search)) res = res.Where(x => x.MaViTri == search).ToList();

            //if (ListQuyen.Contains(CONSTKEY.V_BP)) res = res.Where(x => x.IDPB == idpb).ToList();
            //if(Idquyen != 1 && IdquyenKNL ==0 ) res = res.Where(x => x.IDPB == 0).ToList();

            ViewBag.SumNV = res.Select(x => x.CountNV).Sum();

            ViewBag.SumNVDDG = res.Select(x => x.CountNVDDG).Sum();

            List<KNL_Khoi> khoi = db.KNL_Khoi.ToList();
            ViewBag.IDKhoi = new SelectList(khoi, "ID", "TenKhoi");

            List<KNL_PhanXuong> px = db.KNL_PhanXuong.Where(x=>x.IDPhongBan == IDPB ).ToList();
            ViewBag.IDPX = new SelectList(px, "ID", "TenPX");

            List<KNL_Nhom> nhom = db.KNL_Nhom.Where(x => x.IDPhongBan == IDPB).ToList();
            ViewBag.IDNhom = new SelectList(nhom, "IDNhom", "TenNhom");

            List<KNL_To> tos = db.KNL_To.Where(x => x.IDPhongBan == IDPB).ToList();
            ViewBag.IDTo = new SelectList(tos, "IDTo", "TenTo");

            if (page == null) page = 1;
            int pageSize = 50;
            int pageNumber = (page ?? 1);
            return View(res.ToList().ToPagedList(pageNumber, pageSize));
        }
        [HttpPost]
        public ActionResult ChangePositon( FormCollection collection, List<ViTriKNLValidation> ListKQ)
        {

            try
            {
                //if (_DO.MaNV != null)
                //{
                //    //var aa = db.KNLTo_insert(_DO.TenTo, _DO.MaTo, (int?)_DO.IDPhongBan, (int?)_DO.IDPhanXuong, _DO.IDKhoi);
                //}
                var ListVT = new List<ViTriKNLValidation>();
                foreach (var key in collection.AllKeys)
                {
                    if (key.Split('_')[0] == "IDPB")
                    {
                        ListVT.Add(new ViTriKNLValidation() { TenPhongBan = collection["IDPB_" + key.Split('_')[1]], TenKhoi = collection["IDKhoi_" + key.Split('_')[1]], TenPX = collection["IDPX_" + key.Split('_')[1]], TenNhom = collection["IDNhom_" + key.Split('_')[1]], TenTo = collection["IDTo_" + key.Split('_')[1]], MaViTri =  key.Split('_')[1] });
                    }
                }
                foreach (var item in ListVT)
                {
                    //int idvt = GetIDTo(item.TenViTri.Trim());
                    if (!String.IsNullOrEmpty(item.MaViTri) && !String.IsNullOrEmpty(item.TenPhongBan))
                    {
                        //Regex.Replace(_DO.NVDG, @"[^0-9a-zA-Z]+", " ");
                        //string tx = Regex.Replace(item.TenViTri, @"[^0-9a-zA-Z]+", " ");
                        //string[] NVS = tx.Split(new char[] { ' ' });
                        int IDVT = Int32.TryParse(item.MaViTri, out IDVT) ? IDVT : 0;
                        int IDPB = Int32.TryParse(item.TenPhongBan, out IDPB) ? IDPB : 0;
                        int IDPX = Int32.TryParse(item.TenPX, out IDPX) ? IDPX : 0;
                        int IDNhom = Int32.TryParse(item.TenNhom, out IDNhom) ? IDNhom : 0;
                        int IDTo = Int32.TryParse(item.TenTo, out IDTo) ? IDTo : 0;
                        int IDK = Int32.TryParse(item.TenKhoi, out IDK) ? IDK : 0;

                        var aa = db.ViTriKNLs.Where(x => x.IDVT == IDVT).ToList();
                        if (aa.Count > 0)
                        {
                            var vt = aa.FirstOrDefault();
                            if(IDPX != 0)
                            {
                                var checkKhoi = db.KNL_PhanXuong.Where(x => x.ID == IDPX).FirstOrDefault();
                                IDK =  Int32.TryParse(checkKhoi.IDKhoi.ToString(), out IDK) ? IDK : 0;
                            }
                            db.VitriKNL_update(vt.IDVT, vt.TenViTri, vt.MaViTri, IDPB, IDK, IDPX, IDNhom, IDTo, vt.FilePath);
                            //ListNV.Add(new EmployeeValidation { ID = aa.FirstOrDefault().IDVT, HoTen = aa.FirstOrDefault().TenViTri });
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
            return RedirectToAction("ChangePositon", "FPosition");
            //return RedirectToAction("ChangePosition", "FPosition", new { IDPB = urlParameters.Get("IDPB"), IDKhoi = urlParameters.Get("IDKhoi"), IDPX = urlParameters.Get("IDPX"), IDTo = urlParameters.Get("IDTo"), IDNhom = urlParameters.Get("IDNhom") });
        }

        public ActionResult TrinhKyKNL()
        {
            int idpb = MyAuthentication.IDPhongban;
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            if (!ListQuyen.Contains(CONSTKEY.U_KNL))
            {
                TempData["msgError"] = "<script>alert('Bạn không có quyền thực hiện chức năng này');</script>";
                return RedirectToAction("", "Home");
            }
            db.Configuration.ProxyCreationEnabled = false;

            List<PhongBan> dt = db.PhongBans.ToList();
            if (ListQuyen.Contains(CONSTKEY.V_BP))
            {
                dt = dt.Where(x => x.IDPhongBan == idpb).ToList();
            }
            ViewBag.IDPB = new SelectList(dt, "IDPhongBan", "TenPhongBan");
            // trình ký
            var nhanvien = db.NhanViens.Where(x => x.IDTinhTrangLV == 1 && x.IDPhongBan == MyAuthentication.IDPhongban).Select(x => new EmployeeValidation { ID = x.ID, HoTen = x.MaNV + " - " + x.HoTen, IDPhongBan = (int)x.IDPhongBan }).ToList();
            ViewBag.NguoiDuyet = new SelectList(nhanvien, "ID", "HoTen");

            return PartialView();
        }
        [HttpPost]
        public ActionResult TrinhKyKNL(ViTriKNLValidation _DO, FormCollection form)
        {
            try
            {
                int dtc = 0;
                string msg = "";
                var nguoiduyet = form.GetValue("NguoiDuyet");
                if(nguoiduyet == null)
                {
                    TempData["msgSuccess"] = "<script>alert('Vui lòng chọn người duyệt KNL');</script>";
                    return RedirectToAction("Index", "FPosition", new { IDPB = _DO.IDPB, IDNhom = _DO.IDNhom, IDTo = _DO.IDTo });
                }
                if(_DO.IDPB == null && _DO.IDNhom == null && _DO.IDTo == null)
                {
                    TempData["msgSuccess"] = "<script>alert('Vui lòng chọn ít nhất 1 Bộ phận/Xưởng/Tổ/Nhóm');</script>";
                    return RedirectToAction("Index", "FPosition", new { IDPB = _DO.IDPB, IDNhom = _DO.IDNhom, IDTo = _DO.IDTo });
                }
                int IDNguoiDuyet = int.Parse(nguoiduyet.AttemptedValue.ToString());
                if(_DO.IDPB != null && _DO.IDNhom != null)
                {
                    var lsVT = db.ViTriKNLs.Where(x => x.IDPB == _DO.IDPB && x.IDNhom == _DO.IDNhom && x.MaViTri != "TBP" && x.MaViTri != "PBP").ToList();
                    foreach (var item in lsVT)
                    {
                        // xóa dữ liệu trùng trình ký trước chưa được duyệt
                        var isDuplicate = db.KNL_PheDuyetKNL.Where(x => x.IDVT == item.IDVT && x.TinhTrang == 0).ToList();
                        if (isDuplicate.Count != 0)
                        {
                            db.KNL_PheDuyetKNL.RemoveRange(isDuplicate);
                            db.SaveChanges();
                        }
                        var a = new KNL_PheDuyetKNL()
                        {
                            IDVT = item.IDVT,
                            ID_NguoiTao = MyAuthentication.ID,
                            ID_NguoiDuyet = IDNguoiDuyet,
                            NgayTrinhKy = DateTime.Now,
                            TinhTrang = 0
                        };
                        db.KNL_PheDuyetKNL.Add(a);
                        db.SaveChanges();
                        // update đọc bảng KNL Delete
                        db.Database.ExecuteSqlCommand("DELETE FROM KNL_DocBangKNL WHERE ID_ViTriKNL = {0}", item.IDVT);
                        // Cập nhật lại trạng thái phê duyệt năng lực
                        var knl = db.KhungNangLuc_SearchByIDVT(item.IDVT).ToList();
                        var knl1 = knl.Where(x => x.IsDuyet == 1).ToList();
                        var knl12 = knl.Where(x => x.IsDuyet == null).ToList();
                        var Knlxoa = knl.Where(x => x.IsDuyet == 0).ToList();
                        foreach (var nl in knl1) // cập nhật bảng knl đã ký trước đó về bảng cũ
                        {
                            db.KhungNangLuc_update(nl.IDNL, nl.TenNL, nl.IDLoaiNL, nl.IDVT, nl.IDPB, nl.DinhMuc, nl.IsDanhGia, nl.OrderBy, 2);
                        }
                        foreach (var nl in knl12) // tạo bảng lưu knl trình ký
                        {
                            db.KhungNangLuc_insert(nl.TenNL, nl.IDLoaiNL, nl.IDVT, nl.IDPB, nl.DinhMuc, nl.IsDanhGia, nl.OrderBy, 0);
                        }
                        foreach (var nl in Knlxoa) // xóa KNL đang lưu
                        {
                            db.KhungNangLuc_delete(nl.IDNL);
                        }
                        dtc++;
                    }

                } 
                else if (_DO.IDPB != null && _DO.IDTo != null)
                {
                    var lsVT = db.ViTriKNLs.Where(x => x.IDPB == _DO.IDPB && x.IDTo == _DO.IDTo && x.MaViTri != "TBP" && x.MaViTri != "PBP").ToList();
                    foreach (var item in lsVT)
                    {
                        // xóa dữ liệu trình ký trước chưa được duyệt
                        var isDuplicate = db.KNL_PheDuyetKNL.Where(x => x.IDVT == item.IDVT && x.TinhTrang == 0).ToList();
                        if (isDuplicate.Count != 0)
                        {
                            db.KNL_PheDuyetKNL.RemoveRange(isDuplicate);
                            db.SaveChanges();
                        }
                        var a = new KNL_PheDuyetKNL()
                        {
                            IDVT = item.IDVT,
                            ID_NguoiTao = MyAuthentication.ID,
                            ID_NguoiDuyet = IDNguoiDuyet,
                            NgayTrinhKy = DateTime.Now,
                            TinhTrang = 0
                        };
                        db.KNL_PheDuyetKNL.Add(a);
                        db.SaveChanges();
                        // Cập nhật lại trạng thái phê duyệt năng lực
                        var knl = db.KhungNangLuc_SearchByIDVT(item.IDVT).ToList();
                        var knl1 = knl.Where(x => x.IsDuyet == 1).ToList();
                        var knl12 = knl.Where(x => x.IsDuyet == null).ToList();
                        var Knlxoa = knl.Where(x => x.IsDuyet == 0).ToList();
                        foreach (var nl in knl1) // cập nhật bảng knl đã ký trước đó về bảng cũ
                        {
                            db.KhungNangLuc_update(nl.IDNL, nl.TenNL, nl.IDLoaiNL, nl.IDVT, nl.IDPB, nl.DinhMuc, nl.IsDanhGia, nl.OrderBy, 2);
                        }
                        foreach (var nl in knl12) // tạo bảng lưu knl trình ký
                        {
                            db.KhungNangLuc_insert(nl.TenNL, nl.IDLoaiNL, nl.IDVT, nl.IDPB, nl.DinhMuc, nl.IsDanhGia, nl.OrderBy, 0);
                        }
                        foreach (var nl in Knlxoa) // xóa KNL đang lưu
                        {
                            db.KhungNangLuc_delete(nl.IDNL);
                        }
                        dtc++;
                    }

                }
                else if (_DO.IDPB != null && _DO.IDPX != null)
                {
                    var lsVT = db.ViTriKNLs.Where(x => x.IDPB == _DO.IDPB && x.IDPX == _DO.IDPX && x.MaViTri != "TBP" && x.MaViTri != "PBP").ToList();
                    foreach (var item in lsVT)
                    {
                        // xóa dữ liệu trình ký trước chưa được duyệt
                        var isDuplicate = db.KNL_PheDuyetKNL.Where(x => x.IDVT == item.IDVT && x.TinhTrang == 0).ToList();
                        if (isDuplicate.Count != 0)
                        {
                            db.KNL_PheDuyetKNL.RemoveRange(isDuplicate);
                            db.SaveChanges();
                        }
                        var a = new KNL_PheDuyetKNL()
                        {
                            IDVT = item.IDVT,
                            ID_NguoiDuyet = IDNguoiDuyet,
                            ID_NguoiTao = MyAuthentication.ID,
                            NgayTrinhKy = DateTime.Now,
                            TinhTrang = 0
                        };
                        db.KNL_PheDuyetKNL.Add(a);
                        db.SaveChanges();
                        // Cập nhật lại trạng thái phê duyệt năng lực
                        var knl = db.KhungNangLuc_SearchByIDVT(item.IDVT).ToList();
                        var knl1 = knl.Where(x => x.IsDuyet == 1).ToList();
                        var knl12 = knl.Where(x => x.IsDuyet == null).ToList();
                        var Knlxoa = knl.Where(x => x.IsDuyet == 0).ToList();
                        foreach (var nl in knl1) // cập nhật bảng knl đã ký trước đó về bảng cũ
                        {
                            db.KhungNangLuc_update(nl.IDNL, nl.TenNL, nl.IDLoaiNL, nl.IDVT, nl.IDPB, nl.DinhMuc, nl.IsDanhGia, nl.OrderBy, 2);
                        }
                        foreach (var nl in knl12) // tạo bảng lưu knl trình ký
                        {
                            db.KhungNangLuc_insert(nl.TenNL, nl.IDLoaiNL, nl.IDVT, nl.IDPB, nl.DinhMuc, nl.IsDanhGia, nl.OrderBy, 0);
                        }
                        foreach (var nl in Knlxoa) // xóa KNL đang lưu
                        {
                            db.KhungNangLuc_delete(nl.IDNL);
                        }
                        dtc++;
                    }

                } else if(_DO.IDPB != null)
                {
                    var lsVT = db.ViTriKNLs.Where(x => x.IDPB == _DO.IDPB && x.MaViTri != "TBP" && x.MaViTri != "PBP").ToList();
                    foreach (var item in lsVT)
                    {
                        // xóa dữ liệu trình ký trước chưa được duyệt
                        var isDuplicate = db.KNL_PheDuyetKNL.Where(x => x.IDVT == item.IDVT && x.TinhTrang == 0).ToList();
                        if (isDuplicate.Count != 0)
                        {
                            db.KNL_PheDuyetKNL.RemoveRange(isDuplicate);
                            db.SaveChanges();
                        }
                        var a = new KNL_PheDuyetKNL()
                        {
                            IDVT = item.IDVT,
                            ID_NguoiTao = MyAuthentication.ID,
                            ID_NguoiDuyet = IDNguoiDuyet,
                            NgayTrinhKy = DateTime.Now,
                            TinhTrang = 0
                        };
                        db.KNL_PheDuyetKNL.Add(a);
                        db.SaveChanges();
                        // Cập nhật lại trạng thái phê duyệt năng lực
                        var knl = db.KhungNangLuc_SearchByIDVT(item.IDVT).ToList();
                        var knl1 = knl.Where(x => x.IsDuyet == 1).ToList();
                        var knl12 = knl.Where(x => x.IsDuyet == null).ToList();
                        var Knlxoa = knl.Where(x => x.IsDuyet == 0).ToList();
                        foreach (var nl in knl1) // cập nhật bảng knl đã ký trước đó về bảng cũ
                        {
                            db.KhungNangLuc_update(nl.IDNL, nl.TenNL, nl.IDLoaiNL, nl.IDVT, nl.IDPB, nl.DinhMuc, nl.IsDanhGia, nl.OrderBy, 2);
                        }
                        foreach (var nl in knl12) // tạo bảng lưu knl trình ký
                        {
                            db.KhungNangLuc_insert(nl.TenNL, nl.IDLoaiNL, nl.IDVT, nl.IDPB, nl.DinhMuc, nl.IsDanhGia, nl.OrderBy, 0);
                        }
                        foreach (var nl in Knlxoa) // xóa KNL đang lưu
                        {
                            db.KhungNangLuc_delete(nl.IDNL);
                        }
                        dtc++;
                    }
                }

                if (dtc != 0)
                {
                    msg = "Đã trình ký " + dtc + " Vị trí";
                }
                TempData["msgSuccess"] = "<script>alert('" + msg + "');</script>";
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Có lỗi khi thêm mới: " + e.Message + "');</script>";
            }
            //return View();
            return RedirectToAction("Index", "FPosition", new { IDPB = _DO.IDPB, IDNhom = _DO.IDNhom, IDTo = _DO.IDTo });
        }

        public ActionResult TrinhKyBangKNL(int? IDVT ,int? IDPB)
        {
            int idpb = MyAuthentication.IDPhongban;
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            if (!ListQuyen.Contains(CONSTKEY.U_KNL))
            {
                TempData["msgError"] = "<script>alert('Bạn không có quyền thực hiện chức năng này');</script>";
                return RedirectToAction("", "Home");
            }
            db.Configuration.ProxyCreationEnabled = false;

            ViewBag.IDPB = IDPB;
            ViewBag.IDVT = IDVT;
            // trình ký
            var nhanvien = db.NhanViens.Where(x => x.IDTinhTrangLV == 1 && x.IDPhongBan == MyAuthentication.IDPhongban).Select(x => new EmployeeValidation { ID = x.ID, HoTen = x.MaNV + " - " + x.HoTen, IDPhongBan = (int)x.IDPhongBan }).ToList();
            ViewBag.NguoiDuyet = new SelectList(nhanvien, "ID", "HoTen");

            return PartialView();
        }
        [HttpPost]
        public ActionResult TrinhKyBangKNL(ViTriKNLValidation _DO, FormCollection form)
        {
            try
            {
                int dtc = 0;
                string msg = "";
                var nguoiduyet = form.GetValue("NguoiDuyet");
                if (nguoiduyet == null)
                {
                    TempData["msgSuccess"] = "<script>alert('Vui lòng chọn người duyệt KNL');</script>";
                    return RedirectToAction("CreateKNL", "FPosition", new { id = _DO.IDVT, IDPB = _DO.IDPB });
                }
                //if (_DO.IDPB == null && _DO.IDNhom == null && _DO.IDTo == null)
                //{
                //    TempData["msgSuccess"] = "<script>alert('Vui lòng chọn ít nhất 1 Bộ phận/Xưởng/Tổ/Nhóm');</script>";
                //    return RedirectToAction("Index", "FPosition", new { IDPB = _DO.IDPB, IDNhom = _DO.IDNhom, IDTo = _DO.IDTo });
                //}
                int IDNguoiDuyet = int.Parse(nguoiduyet.AttemptedValue.ToString());
                if (_DO.IDVT != 0)
                {
                    var lsVT = db.ViTriKNLs.Where(x => x.IDVT == _DO.IDVT).FirstOrDefault();
                    // xóa dữ liệu trùng trình ký trước chưa được duyệt
                    var isDuplicate = db.KNL_PheDuyetKNL.Where(x => x.IDVT == lsVT.IDVT && x.TinhTrang == 0).ToList();
                    if (isDuplicate.Count != 0)
                    {
                        db.KNL_PheDuyetKNL.RemoveRange(isDuplicate);
                        db.SaveChanges();
                    }
                    var a = new KNL_PheDuyetKNL()
                    {
                        IDVT = _DO.IDVT,
                        ID_NguoiTao = MyAuthentication.ID,
                        ID_NguoiDuyet = IDNguoiDuyet,
                        NgayTrinhKy = DateTime.Now,
                        TinhTrang = 0
                    };
                    db.KNL_PheDuyetKNL.Add(a);
                    db.SaveChanges();
                    // Cập nhật lại trạng thái phê duyệt năng lực
                    var knl = db.KhungNangLuc_SearchByIDVT(_DO.IDVT).ToList();
                    var knl1 = knl.Where(x => x.IsDuyet == 1).ToList();
                    var knl12 = knl.Where(x => x.IsDuyet == null).ToList();
                    var Knlxoa = knl.Where(x => x.IsDuyet == 0).ToList();
                    // update đọc bảng KNL Delete
                    db.Database.ExecuteSqlCommand("DELETE FROM KNL_DocBangKNL WHERE ID_ViTriKNL = {0}", _DO.IDVT);
                    foreach (var nl in knl1) // cập nhật bảng knl đã ký trước đó về bảng cũ
                    {
                        db.KhungNangLuc_update(nl.IDNL, nl.TenNL, nl.IDLoaiNL, nl.IDVT, nl.IDPB, nl.DinhMuc, nl.IsDanhGia, nl.OrderBy, 2);
                    }
                    foreach (var nl in knl12) // tạo bảng lưu knl trình ký
                    {
                        db.KhungNangLuc_insert( nl.TenNL, nl.IDLoaiNL, nl.IDVT, nl.IDPB, nl.DinhMuc, nl.IsDanhGia, nl.OrderBy, 0);
                    }
                    foreach (var nl in Knlxoa) // xóa KNL đang lưu
                    {
                        db.KhungNangLuc_delete(nl.IDNL);
                    }
                    dtc++;

                }

                if (dtc != 0)
                {
                    msg = "Đã trình ký " + dtc + " Vị trí";
                }
                TempData["msgSuccess"] = "<script>alert('" + msg + "');</script>";
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Có lỗi khi thêm mới: " + e.Message + "');</script>";
            }
            //return View();
            return RedirectToAction("CreateKNL", "FPosition", new { id = _DO.IDVT, IDPB = _DO.IDPB });
        }

        public ActionResult HistoryBangKNL(int? page, int? IDVT)
        {
            //var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            //ViewBag.QUYENCN = ListQuyen;
            //if (!ListQuyen.Contains(CONSTKEY.V))
            //{
            //    TempData["msgError"] = "<script>alert('Bạn không có quyền truy cập chức năng này');</script>";
            //    return RedirectToAction("", "Home");
            //}
            var vt = db.ViTriKNLs.Where(x => x.IDVT == IDVT).FirstOrDefault();
            ViewBag.ViTri = vt.TenViTri;
            int idpb = MyAuthentication.IDPhongban;
            var manv = MyAuthentication.Username;

            var res = (from a in db.KNL_PheDuyetKNL.Where(x=>x.IDVT == IDVT)
                       select new KNL_PheDuyetKNLView
                       {
                           ID = a.ID,
                           IDVT = IDVT,
                           TenViTri = vt.TenViTri,
                           ID_NguoiDuyet = a.ID_NguoiDuyet,
                           HoTenNguoiDuyet = a.NhanVien.MaNV +" - "+ a.NhanVien.HoTen,
                           NgayDuyet = a.NgayDuyet,
                           NgayTrinhKy =a.NgayTrinhKy,
                           TinhTrang = a.TinhTrang,
                           File_KNL=a.File_KNL
                       }).OrderByDescending(x=>x.NgayTrinhKy).ToList();

            if (page == null) page = 1;
            int pageSize = 50;
            int pageNumber = (page ?? 1);
            return View(res.ToList().ToPagedList(pageNumber, pageSize));
        }
        public ActionResult PheDuyetKNL(int? page)
        {
            //var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            //ViewBag.QUYENCN = ListQuyen;
            //if (!ListQuyen.Contains(CONSTKEY.V))
            //{
            //    TempData["msgError"] = "<script>alert('Bạn không có quyền truy cập chức năng này');</script>";
            //    return RedirectToAction("", "Home");
            //}
            // check chữ ký
            var nhanvien = db.NhanViens.Where(x => x.ID == MyAuthentication.ID).FirstOrDefault();
            if (string.IsNullOrEmpty(nhanvien.ChuKy))
            {
                TempData["msgSuccess"] = "<script>alert('Chưa có chữ ký vui lòng cập nhật chữ ký ');</script>";
                return new RedirectResult("~/Login/CapNhatChuKy");
            }
            int idpb = MyAuthentication.IDPhongban;
            var manv = MyAuthentication.Username;

            var res = (from a in db.KNL_PheDuyetKNL.Where(x => x.ID_NguoiDuyet == MyAuthentication.ID)
                       join b in db.ViTriKNLs on a.IDVT equals b.IDVT
                       let maxNgayTrinhKy = db.KNL_PheDuyetKNL
                                 .Where(y => y.IDVT == a.IDVT && y.ID_NguoiDuyet == a.ID_NguoiDuyet)
                                 .Max(y => y.NgayTrinhKy)
                       select new KNL_PheDuyetKNLView
                       {
                           ID = a.ID,
                           IDVT = b.IDVT,
                           TenViTri = b.TenViTri,
                           ID_NguoiDuyet = a.ID_NguoiDuyet,
                           HoTenNguoiDuyet = a.NhanVien.MaNV + " - " + a.NhanVien.HoTen,
                           ID_NguoiTao = a.ID_NguoiTao,
                           HoTenNguoiTao = a.NhanVien1.MaNV + " - " + a.NhanVien1.HoTen,
                           NgayDuyet = a.NgayDuyet,
                           NgayTrinhKy = a.NgayTrinhKy,
                           TinhTrang = a.TinhTrang,
                           File_KNL = a.File_KNL,
                           IDPB = b.IDPB,
                           IsLatest = a.NgayTrinhKy == maxNgayTrinhKy // Đánh dấu bản ghi mới nhất
                       }).OrderByDescending(x=>x.NgayTrinhKy).ThenBy(x => x.TinhTrang).ToList();

            if (page == null) page = 1;
            int pageSize = 100;
            int pageNumber = (page ?? 1);
            return View(res.ToList().ToPagedList(pageNumber, pageSize));
        }

        public ActionResult PheDuyetAll()
        {

            int idpb = MyAuthentication.IDPhongban;
            var manv = MyAuthentication.Username;
            var listduyet = db.KNL_PheDuyetKNL.Where(x => x.ID_NguoiDuyet == MyAuthentication.ID && x.TinhTrang == 0).ToList();
            if(listduyet.Count == 0)
            {
                TempData["msgSuccess"] = "<script>alert('Không tồn tại dữ liệu phê duyệt');</script>";
                return RedirectToAction("PheDuyetKNL", "FPosition");
            }
            foreach(var item in listduyet)
            {
                item.NgayDuyet = DateTime.Now;
                item.TinhTrang = 1;
              
                // thêm bảng KNL ký vào table KhungNangLuc
                var knl = db.KhungNangLuc_SearchByIDVT(item.IDVT).Where(x => x.IsDuyet == 0).ToList();
                foreach (var nl in knl)
                {
                    db.KhungNangLuc_update(nl.IDNL, nl.TenNL, nl.IDLoaiNL, nl.IDVT, nl.IDPB, nl.DinhMuc, nl.IsDanhGia, nl.OrderBy, 1);
                }
                string filepath = ExportViewToPdf(item.ID, item.IDVT);
                if (filepath != null)
                {
                    item.File_KNL = filepath;
                }
                db.SaveChanges();
            }

            TempData["msgSuccess"] = "<script>alert('Thành công');</script>";
            return RedirectToAction("PheDuyetKNL", "FPosition");
        }

        [HttpPost]
        public ActionResult ProcessSelected(List<int> selectedItems)
        {
            if (selectedItems != null && selectedItems.Any())
            {

                //// Xử lý danh sách ID được chọn
                foreach (var id in selectedItems)
                {
                    var sH_KyDuyetNCDT = db.KNL_PheDuyetKNL.Where(x => x.IDVT == id && x.ID_NguoiDuyet == MyAuthentication.ID && x.TinhTrang == 0).FirstOrDefault();
                    if (sH_KyDuyetNCDT == null)
                    {
                        return HttpNotFound();
                    }
                    sH_KyDuyetNCDT.NgayDuyet = DateTime.Now;
                    sH_KyDuyetNCDT.TinhTrang = 1;
                   
                    // thêm bảng KNL ký vào table KhungNangLuc
                    var knl = db.KhungNangLuc_SearchByIDVT(id).Where(x => x.IsDuyet == 0).ToList();
                    foreach (var nl in knl)
                    {
                        db.KhungNangLuc_update(nl.IDNL, nl.TenNL, nl.IDLoaiNL, nl.IDVT, nl.IDPB, nl.DinhMuc, nl.IsDanhGia, nl.OrderBy, 1);
                    }
                    string filepath = ExportViewToPdf(sH_KyDuyetNCDT.ID, sH_KyDuyetNCDT.IDVT);
                    if (filepath != null)
                    {
                        sH_KyDuyetNCDT.File_KNL = filepath;
                    }
                    db.SaveChanges();
                }
                //_context.SaveChanges();
            }

            TempData["msgSuccess"] = "<script>alert('Thành công');</script>";
            return RedirectToAction("PheDuyetKNL", "FPosition");
        }

        public ActionResult PheDuyetBangKNL(int? IDVT)
        {

            int idpb = MyAuthentication.IDPhongban;
            var manv = MyAuthentication.Username;
            var listduyet = db.KNL_PheDuyetKNL.Where(x => x.ID_NguoiDuyet == MyAuthentication.ID && x.TinhTrang == 0 && x.IDVT == IDVT).ToList();
            if (listduyet.Count == 0)
            {
                TempData["msgSuccess"] = "<script>alert('Không tồn tại dữ liệu phê duyệt');</script>";
                return RedirectToAction("PheDuyetKNL", "FPosition");
            }
            foreach (var item in listduyet)
            {
                item.NgayDuyet = DateTime.Now;
                item.TinhTrang = 1;
               
                // thêm bảng KNL ký vào table KhungNangLuc
                var knl = db.KhungNangLuc_SearchByIDVT(IDVT).Where(x=>x.IsDuyet ==0).ToList();
                foreach (var nl in knl)
                {
                    db.KhungNangLuc_update(nl.IDNL,nl.TenNL,nl.IDLoaiNL,nl.IDVT,nl.IDPB,nl.DinhMuc,nl.IsDanhGia,nl.OrderBy,1);
                }
                string filepath = ExportViewToPdf(item.ID, IDVT);
                if (filepath != null)
                {
                    item.File_KNL = filepath;
                }
                db.SaveChanges();
            }

            TempData["msgSuccess"] = "<script>alert('Thành công');</script>";
            return RedirectToAction("PheDuyetKNL", "FPosition");
        }

        public string ExportViewToPdf(int id,int? IDVT)
        {

            // Lấy dữ liệu từ cơ sở dữ liệu (ví dụ)
            var vt = db.VitriKNL_searchByIDVT(IDVT).FirstOrDefault();
            ViewBag.TenViTri = vt?.TenViTri ?? "";
            ViewBag.IDVT = IDVT;
            ViewBag.TenPB = vt?.TenPhongBan ?? "";
            ViewBag.ID = id;

            var res = (from a in db.KhungNangLuc_SearchByIDVT(IDVT)
                       select new FValueValidation
                       {
                           IDNL = a.IDNL,
                           TenNL = a.TenNL,
                           IDLoaiNL = a.IDLoaiNL,
                           IDVT = a.IDVT,
                           TenViTri = a.TenViTri,
                           IDPB = a.IDPB,
                           DinhMuc = a.IsDanhGia != 0 ? a.DinhMuc : 0,
                           IsDanhGia = a.IsDanhGia,
                           IsDuyet = a.IsDuyet
                       }).Where(x=>x.IsDuyet == 1).ToList().OrderBy(x => x.OrderBy);
            var distinctIDLoaiNLs = res.Where(x => x.IDLoaiNL != 1 && x.IDLoaiNL != 2)
                   .Select(x => x.IDLoaiNL)
                   .Distinct()
                   .ToList();

            List<LoaiKNL> loaiNL = db.LoaiKNLs.Where(x => distinctIDLoaiNLs.Contains(x.IDLoai)).OrderBy(x => x.OrderBy).ToList();
            ViewBag.IDLoaiNL = new SelectList(loaiNL, "IDLoai", "TenLoai");

            // Kết xuất View thành HTML
            string htmlContent = PdfHelper.RenderViewToString(this.ControllerContext, "ExportView", res.ToList());

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
                var document = new iTextSharp.text.Document(PageSize.A4, 30, 30, 30, 30);
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
                string filename = $"{"KhungNangLuc_" +  vt?.IDVT + "_" + DateTime.Now.ToString("yyyyMMddHHmm")}.pdf";
                var folderPath = Server.MapPath("~/FileKNL/");

                // Lưu file vào server và trả về path

                byte[] pdfData = memoryStream.ToArray();
                string pathsave = SavePdfToFile(pdfData, folderPath, filename);
                return pathsave;
                //return NoContent();
                //return File(pdfData, "application/pdf", filename);
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
            return $"/FileKNL/{fileName}"; // Nếu wwwroot/pdfs là thư mục lưu trữ công khai
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}