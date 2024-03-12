using E_Learning.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace E_Learning.Controllers.KNL
{
    public class FPermisionController : Controller
    {
        ELEARNINGEntities db = new ELEARNINGEntities();
        int Idquyen = MyAuthentication.IDQuyen;
        String ControllerName = "FPermision";
        // GET: FPermision
        public ActionResult Index(int? page, string search, int? IDPB)
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
            if (IDPB == null) IDPB = 0;

            var res = db.Quyens.ToList();

            if (page == null) page = 1;
            int pageSize = res.Count();
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
            //db.Configuration.ProxyCreationEnabled = false;
            return PartialView();
        }
        public int GetIDGropQuyen(string TenQuyen)
        {
            var model = db.Quyens.Where(x => x.TenQuyen == TenQuyen).SingleOrDefault();
            if (model == null)
                return 0;
            return model.IDQuyen;
        }

        [HttpPost]
        public ActionResult Create(GroupQuyenDetailsValidation _DO)
        {
            try
            {
                if (_DO.TenQuyen != null && GetIDGropQuyen(_DO.TenQuyen.Trim()) == 0)
                {
                    var aa = db.KNL_Quyen_insert(_DO.TenQuyen);
                }
               
                TempData["msgSuccess"] = "<script>alert('Thêm mới thành công');</script>";
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Có lỗi khi thêm mới: " + e.Message + "');</script>";
            }
            //return View();
            return RedirectToAction("Index", "FPermision");
        }

        public ActionResult Edit(int id)
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            if (!ListQuyen.Contains(CONSTKEY.EDIT))
            {
                TempData["msgError"] = "<script>alert('Bạn không có quyền thực hiện chức năng này');</script>";
                return RedirectToAction("", "Home");
            }
            var res = (from a in db.Quyens.Where(x => x.IDQuyen == id)
                       select new GroupQuyenDetailsValidation
                       {
                           IDQuyen = a.IDQuyen,
                           TenQuyen = a.TenQuyen,
                       }).ToList();

            var resCon = (from a in db.ListControllers.Where(x => x.isActive == 1)
                          select new ListControllerValidation
                          {
                              ID = a.ID,
                              Controller = a.Controller,
                              Mota = a.Mota,
                              isActive = a.isActive,
                              DSQuyenCN = a.DSQuyenCN,
                          }).OrderBy(x=>x.Mota).ToList();
            if (resCon.Count > 0)
            {
                foreach (var con in resCon)
                {
                    var lsCheck = new List<ItemCheck>();
                    var dsquyen = con.DSQuyenCN?.Split(',');

                    db.Configuration.ProxyCreationEnabled = false;
                    List<QuyenCN> dt = db.QuyenCNs.ToList();
                    foreach (var item in dt)
                    {
                        if (dsquyen == null)
                        {
                            //var a = new ItemCheck { Name = item.TenQuyenCN, IDCN = item.ID, isChecked = false };
                            //lsCheck.Add(a);
                        }
                        else
                        {
                            int pos = Array.IndexOf(dsquyen, item.ID.ToString());
                            if (pos > -1)
                            {
                                var IDQ = res[0].IDQuyen;
                                var check = db.QuyenDetails.Where(x => x.IDQuyen == IDQ && x.IDQuyenCN == item.ID && x.IDController == con.ID).FirstOrDefault();
                                if (check?.isActive == 1)
                                {
                                    var a = new ItemCheck { Name = item.TenQuyenCN, IDCN = item.ID, isChecked = true };
                                    lsCheck.Add(a);
                                }
                                else
                                {
                                    var a = new ItemCheck { Name = item.TenQuyenCN, IDCN = item.ID, isChecked = false };
                                    lsCheck.Add(a);
                                }

                            }
                            //else
                            //{
                            //    var a = new ItemCheck { Name = item.TenQuyenCN, IDCN = item.ID, isChecked = false };
                            //    lsCheck.Add(a);
                            //}
                        }
                    }
                    con.LSChecked = lsCheck.ToList();
                    con.isCheck = lsCheck.Where(x => x.isChecked == true).Count() == lsCheck.Count ? true : false; // check select all
                }
                res[0].ListController = resCon;
            }
            List<Quyen> dta = db.Quyens.ToList();
            ViewBag.SelectQuyen = new SelectList(dta, "IDQuyen", "TenQuyen", id);

            GroupQuyenDetailsValidation DO = new GroupQuyenDetailsValidation();
            if (res.Count > 0)
            {
                foreach (var c in res)
                {
                    DO.IDQuyen = c.IDQuyen;
                    DO.TenQuyen = c.TenQuyen;
                    DO.ListController = c.ListController;
                }
            }
            else
            {
                return HttpNotFound();
            }
            return View(DO);
        }
        [HttpPost]
        public ActionResult Edit(GroupQuyenDetailsValidation _DO)
        {
            try
            {
                if (_DO.ListController.Count > 0)
                {
                    foreach (var item in _DO.ListController)
                    {
                        if (item.LSChecked != null)
                        {
                            foreach (var chec in item.LSChecked)
                            {
                                int? isCh = chec.isChecked ? 1 : 0;
                                var idQ = GetIDQuyenDetail(_DO.IDQuyen, item.ID, chec.IDCN);
                                if (idQ == 0)
                                {
                                    db.QuyenDetails_insert(_DO.IDQuyen, item.ID, chec.IDCN, isCh);
                                }
                                else
                                {
                                    db.QuyenDetails_update(idQ, _DO.IDQuyen, item.ID, chec.IDCN, isCh);
                                }
                            }
                        }
                    }
                }
                //db.PhongBan_update_KNL(_DO.IDPhongBan, _DO.TenPhongBan, _DO.MaPB);

                TempData["msgSuccess"] = "<script>alert('Cập nhập thành công');</script>";
            }
            catch (Exception e)
            {

                TempData["msgSuccess"] = "<script>alert('Cập nhập thất bại " + e.Message + " ');</script>";
            }

            return RedirectToAction("Edit", "FPermision", new { id = _DO.IDQuyen });
        }


        public ActionResult ListUser(int id)
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            if (!ListQuyen.Contains(CONSTKEY.PER))
            {
                TempData["msgError"] = "<script>alert('Bạn không có quyền thực hiện chức năng này');</script>";
                return RedirectToAction("", "Home");
            }

            //List<EmployeeValidation> lsuser = GetListUser(id);
            //ViewBag.LSUSER = lsuser;

            //var res = (from a in db.NhanViens.Where(x=>x.IDQuyen ==id && x.IDTinhTrangLV ==1 )
            //           select new EmployeeValidation
            //           {
            //               ID =a.ID,
            //               HoTen =a.MaNV+"-"+a.HoTen,
            //               PhongBan =a.PhongBan.TenPhongBan,
            //               TenQuyen =a.Vitri.TenViTri,
            //               IDPhongBan =id
            //           }).ToList();

            List<Quyen> dta = db.Quyens.ToList();
            ViewBag.SelectQuyen = new SelectList(dta, "IDQuyen", "TenQuyen", id);

            //EmployeeValidation DO = new EmployeeValidation();
            //if (res.Count > 0)
            //{
            //    foreach (var c in res)
            //    {
            //        DO.ID = c.ID;
            //        DO.HoTen = c.HoTen;
            //        DO.PhongBan = c.PhongBan;
            //        DO.TenQuyen = c.TenQuyen;
            //        DO.IDPhongBan = c.IDPhongBan;
            //    }
            //}
            ////else
            ////{
            ////    return;
            ////}
            //int pageSize = res.Count()!=0?res.Count():50;
            //int pageNumber = 1;
            //return View(res.ToList());
            return View();
        }

        public ActionResult DeleteQuyen(int id, int? IDQuyen)
        {
            try
            {
                var aa = db.NhanViens.Where(x => x.ID == id).FirstOrDefault();
                if (aa != null)
                {
                    db.Nhanvien_update_Quyen(aa.MaNV, 4);
                }
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Xóa dữ liệu thất bại');</script>";
            }
            return RedirectToAction("ListUser", "FPermision", new { id = IDQuyen });
        }
        public ActionResult InsertQuyenCN(string TenQuyen,string MaQuyen)
        {
            try
            {
                if (TenQuyen != "" && MaQuyen != "")
                {
                    db.QuyenCN_insert(TenQuyen, MaQuyen);
                }
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Xóa dữ liệu thất bại');</script>";
            }
            return RedirectToAction("AdminPermision", "FPermision");
        }



        public ActionResult AdminPermision(int? page)
        {
            
            var res = (from a in db.ListControllers
                       select new ListControllerValidation
                       {
                           ID =a.ID,
                           Controller =a.Controller,
                           Mota =a.Mota,
                           isActive =a.isActive,
                           DSQuyenCN =a.DSQuyenCN,
                       }).ToList();
            foreach(var item in res)
            {
                var words = new List<string>();
                if (!String.IsNullOrEmpty(item.DSQuyenCN))
                {
                    var dsquyen = item.DSQuyenCN?.Split(',');
                    List<QuyenCN> dt = db.QuyenCNs.ToList();
                    foreach (var aa in dt)
                    {
                        int pos = Array.IndexOf(dsquyen, aa.ID.ToString());
                        if (pos > -1)
                        {
                            words.Add(aa.MaQuyen);
                        }
                    }
                }
                item.DSTenQuyen = string.Join(",", words);
            }

            if (page == null) page = 1;
            int pageSize = res.Count();
            int pageNumber = (page ?? 1);
            return View(res.ToList().ToPagedList(pageNumber, pageSize));
        }

        public ActionResult AddUserQuyen(int? id)
        {
            db.Configuration.ProxyCreationEnabled = false;
            List<Quyen> dta = db.Quyens.Where(x=>x.IDQuyen ==id).ToList();
            ViewBag.IDKNL = new SelectList(dta, "IDQuyen", "TenQuyen", id);

            var aa = db.NhanViens.Where(x => x.IDTinhTrangLV == 1).ToList();

            var nv3 = aa.Select(x => new EmployeeValidation { MaNV = x.MaNV, HoTen = x.MaNV + " - " + x.HoTen }).ToList();
            ViewBag.Selected = new SelectList(nv3, "MaNV", "HoTen");

            List<PhongBan> dt = db.PhongBans.ToList();
            ViewBag.IDPB = new SelectList(dt, "IDPhongBan", "TenPhongBan");

            return PartialView();
        }

        [HttpPost]
        public ActionResult AddUserQuyen(DSachDGValidation _DO)
        {
            try
            {
                if (!String.IsNullOrEmpty(_DO.NVDG))
                {
                    //Regex.Replace(_DO.NVDG, @"[^0-9a-zA-Z]+", " ");
                    string tx = Regex.Replace(_DO.NVDG, @"[^0-9a-zA-Z]+", " ");
                    string[] NVS = tx.Split(new char[] { ' ' });
                    foreach (var item in NVS)
                    {
                        var aa = db.NhanViens.Where(x => x.MaNV == item).Count();
                        if (aa > 0)
                        {
                            db.Nhanvien_update_Quyen(item, _DO.IDKNL);
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
                            //db.Nhanvien_update_QuyenKNL(_DO.Selected[i], _DO.IDKNL);
                            db.Nhanvien_update_Quyen(_DO.Selected[i], _DO.IDKNL);
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
            return RedirectToAction("ListUser", "FPermision", new { id = _DO.IDKNL });
        }

        public JsonResult CheckNV(int? IDPB)
        {
            var ListNV = new List<EmployeeValidation>();
                    var aa = db.NhanViens.Where(x => x.IDPhongBan == IDPB && x.IDTinhTrangLV ==1).ToList();
                    if (aa.Count > 0)
                    {
                        foreach(var item in aa)
                        {
                            ListNV.Add(new EmployeeValidation { MaNV = item.MaNV, HoTen = item.MaNV + " - " + item.HoTen });
                        }
                        
                    }
            return Json(ListNV, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetListUser(int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            List<EmployeeValidation> res = (from a in db.NhanViens.Where(x => x.IDQuyen == id && x.IDTinhTrangLV == 1)
                       select new EmployeeValidation
                       {
                           ID = a.ID,
                           HoTen = a.MaNV + "-" + a.HoTen,
                           PhongBan = a.PhongBan.TenPhongBan,
                           TenQuyen = a.Vitri.TenViTri,
                           IDPhongBan = id
                       }).ToList();
            var jsonResult = Json(res, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
            //return res;
        }

        public ActionResult EditController(int id)
        {

            var res = (from a in db.ListControllers.Where(x => x.ID == id)
                       select new ListControllerValidation
                       {
                           ID = a.ID,
                           Controller = a.Controller,
                           Mota = a.Mota,
                           isActive = a.isActive,
                           isCheck = a.isActive ==1?true:false,
                           DSQuyenCN = a.DSQuyenCN,
                       }).ToList();
            ListControllerValidation DO = new ListControllerValidation();

            if (res.Count > 0)
            {
                var lsCheck = new List<ItemCheck>();
                var dsquyen = res[0].DSQuyenCN?.Split(',');
                
                db.Configuration.ProxyCreationEnabled = false;
                List<QuyenCN> dt = db.QuyenCNs.ToList();
                foreach(var item in dt)
                {
                    if(dsquyen == null)
                    {
                        var a = new ItemCheck { Name = item.TenQuyenCN, IDCN = item.ID, isChecked = false };
                        lsCheck.Add(a);
                    }
                    else
                    {
                        int pos = Array.IndexOf(dsquyen, item.ID.ToString());
                        if (pos > -1)
                        {
                            var a = new ItemCheck { Name = item.TenQuyenCN, IDCN = item.ID, isChecked = true };
                            lsCheck.Add(a);
                        }
                        else
                        {
                            var a = new ItemCheck { Name = item.TenQuyenCN, IDCN = item.ID, isChecked = false };
                            lsCheck.Add(a);
                        }
                    }
                    
                }
                res[0].LSChecked = lsCheck.ToList();
                //ViewBag.Selected = new SelectList(dt, "ID", "TenQuyenCN");
                //ViewBag.Selec = new SelectList(dt, "ID", "TenQuyenCN");

                foreach (var c in res)
                {
                    DO.ID = c.ID;
                    DO.Mota = c.Mota;
                    DO.Controller = c.Controller;
                    DO.isActive = c.isActive;
                    DO.DSQuyenCN = c.DSQuyenCN;
                    DO.LSChecked = c.LSChecked;
                    DO.isCheck =c.isCheck;
                }
            }
            else
            {
                return HttpNotFound();
            }
            return PartialView(DO);
        }
        [HttpPost]
        public ActionResult EditController(ListControllerValidation _DO)
        {
            try
            {
                var words = new List<string>();
                var lsCN = db.QuyenDetails.Where(x => x.IDController == _DO.ID).ToList();
                foreach (var item in _DO.LSChecked)
                {
                    if (item.isChecked) { words.Add(item.IDCN.ToString());}
                    else
                    {
                        var a=lsCN.Where(x => x.IDQuyenCN == item.IDCN).Count();
                        if (a > 0) db.QuyenDetails_isActive(_DO.ID, item.IDCN, 0);
                    }
                }
                _DO.DSQuyenCN = string.Join(",", words);
                _DO.isActive = _DO.isCheck ? 1 : 0;
                db.ListController_update(_DO.ID, _DO.Controller, _DO.Mota, _DO.isActive, _DO.DSQuyenCN);
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Chỉnh sửa thất bại');</script>";
            }
            return RedirectToAction("AdminPermision", "FPermision");
        }
        public int GetIDController(string TenController)
        {
            var model = db.ListControllers.Where(x => x.Controller == TenController).SingleOrDefault();
            if (model == null)
                return 0;
            return model.ID;
        }

        public int GetIDQuyenDetail(int? IDQuyen,int? IDController, int? IDQuyenCN)
        {
            var model = db.QuyenDetails.Where(x => x.IDQuyen == IDQuyen&&x.IDController ==IDController&&x.IDQuyenCN ==IDQuyenCN).SingleOrDefault();
            if (model == null)
                return 0;
            return model.ID;
        }
        public ActionResult Sync()
        {
            try
            {
                Assembly asm = Assembly.GetExecutingAssembly();

                var controllerTypes = from t in asm.GetExportedTypes()
                                      where typeof(IController).IsAssignableFrom(t)
                                      select t;

                foreach (var item in controllerTypes)
                {
                    int index = item.Name.IndexOf("Controller");
                    if (index != -1)
                    {
                        var name = item.Name.Remove(index);
                        int IDC = GetIDController(name);
                        if (IDC == 0)
                        {
                            db.Controller_insert(name, null, 1);
                        }
                    }
                }
                TempData["msgSuccess"] = "<script>alert('Đồng bộ dữ liệu thành công');</script>";
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Có lỗi khi đồng bộ dữ liệu: " + e.Message + "');</script>";
            }
            return RedirectToAction("AdminPermision", "FPermision");
        }

        public JsonResult CheckLSNV(string lsnv)
        {
            var ListNV = new List<EmployeeValidation>();
            if (!String.IsNullOrEmpty(lsnv))
            {
                //Regex.Replace(_DO.NVDG, @"[^0-9a-zA-Z]+", " ");
                string tx = Regex.Replace(lsnv, @"[^0-9a-zA-Z]+", " ");
                string[] NVS = tx.Split(new char[] { ' ' });

                foreach (var item in NVS)
                {
                    var aa = db.NhanViens.Where(x => x.MaNV == item).ToList();
                    if (aa.Count > 0)
                    {
                        ListNV.Add(new EmployeeValidation { MaNV = aa.FirstOrDefault().MaNV, HoTen = aa.FirstOrDefault().MaNV + " - " + aa.FirstOrDefault().HoTen });
                    }
                }
            }
            return Json(ListNV, JsonRequestBehavior.AllowGet);
        }
    }
}