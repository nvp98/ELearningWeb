using E_Learning.Models;
using System;
using System.IO;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace E_Learning.Controllers
{
    public class STestController : Controller
    {
        ELEARNINGEntities db_context = new ELEARNINGEntities();
        // GET: STest
        public ActionResult Index(int LHID)
        {
            if (User.Identity.IsAuthenticated)
            {

                int NVID = MyAuthentication.ID;
                var lanthi = db_context.BaiThis.Where(x => x.IDNV == MyAuthentication.ID && x.IDLH == LHID).ToList();
                if (lanthi.Count >= 3)
                {
                    TempData["msgSuccess"] = "<script>alert('Bạn đã thi quá 3 lần');</script>";
                    return RedirectToAction("Index", "EClassroom");
                }else if((lanthi.Where(x=>x.TinhTrang==true).Count()>0))
                {
                    TempData["msgSuccess"] = "<script>alert('Bạn đã thi đạt');</script>";
                    return RedirectToAction("Index", "EClassroom");
                }
                else
                {
                    Random random = new Random();
                    var res = (from l in db_context.LopHocs.Where(x => x.IDLH == LHID)
                               join cd in db_context.CauHoiDeThis on l.IDDeThi equals cd.IDDeThi
                               join ch in db_context.CauHois on cd.IDCauHoi equals ch.IDCH
                               join da in db_context.DanhSachDAs on ch.IDDAĐung equals da.IDDSĐA
                               select new TestValidation
                               {
                                   IDCH = ch.IDCH,
                                   NoiDungCH = ch.NoiDungCH,
                                   DapAnA = ch.DapAnA,
                                   DapAnB = ch.DapAnB,
                                   DapAnC = ch.DapAnC,
                                   DapAnD = ch.DapAnD,
                                   IDDAĐung = (int)ch.IDDAĐung,
                                   DapAnĐung = da.TenĐA,
                                   Diem = (double)cd.Diem,
                                   IDLH = l.IDLH,
                                   IDDeThi = (int)l.IDDeThi,
                                   IDND = (int)l.NDID
                               }).ToList();
                   var res2 = res.OrderBy(x => random.Next()).ToList();
                    return View(res2.ToList());
                }



            }
            else
            {
                return RedirectToAction("", "Login");
            }
            
        }


        public ActionResult Submit(EClassroomValidation _DO, int id)
        {
            //Lấy LHID và NVID theo IDHT
            var IDHT = id;
            var NVID = db_context.XNHocTaps.Where(x => x.IDHT == IDHT).Select(g => g.NVID).FirstOrDefault();
            var PBID = db_context.XNHocTaps.Where(x => x.IDHT == IDHT).Select(g => g.PBID).FirstOrDefault();
            var VTID = db_context.XNHocTaps.Where(x => x.IDHT == IDHT).Select(g => g.VTID).FirstOrDefault();
            var LHID = db_context.XNHocTaps.Where(x => x.IDHT == IDHT).Select(g => g.LHID).FirstOrDefault();
            var NgayTG = db_context.XNHocTaps.Where(x => x.IDHT == IDHT).Select(g => g.NgayTG).FirstOrDefault();
            var XNTG = db_context.XNHocTaps.Where(x => x.IDHT == IDHT).Select(g => g.XNTG).FirstOrDefault();

            //db_context.XNHocTap_update(IDHT, NVID, PBID, VTID, LHID, NgayTG, DateTime.Today, XNTG, true);
            TempData["msgSuccess"] = "<script>alert('Chúc mừng bạn đã hoàn thành bài thi');</script>";

            return RedirectToAction("Index", "STest", new { id = LHID });
        }
        public ActionResult Confirm(List<TestValidation> ListQ)
        {
            if (User.Identity.IsAuthenticated)
            {
                ObjectParameter IDBaiThiout = new ObjectParameter("IDBaiThi", typeof(int));
                int i = 0;
                int IDBaiThi = 0;
                int IDDeThi = 0;
                if(ListQ.Count>0)
                {

                    foreach (var Q in ListQ)
                    {
                        if (i == 0){
                            var lanthi=db_context.BaiThis.Where(x=>x.IDNV==MyAuthentication.ID && x.IDDeThi==Q.IDDeThi&& x.IDLH==Q.IDLH).ToList();
                            if(lanthi.Count >= 3)
                            {
                                TempData["msgSuccess"] = "<script>alert('Bạn đã thi quá 3 lần');</script>";
                                return RedirectToAction("Index", "EClassroom");
                            }else if (lanthi.Where(x=>x.TinhTrang==true).Count()>0)
                            {
                                TempData["msgSuccess"] = "<script>alert('Bạn đã thi đạt');</script>";
                                return RedirectToAction("Index", "EClassroom");
                            }
                            else
                            {
                                db_context.BaiThi_insert(Q.IDLH, Q.IDDeThi, Q.IDND, MyAuthentication.ID, MyAuthentication.IDPhongban, MyAuthentication.IDViTri, 0, DateTime.Now, false, (lanthi.Count + 1), IDBaiThiout);
                                IDBaiThi = Convert.ToInt32(IDBaiThiout.Value);
                                IDDeThi = Q.IDDeThi;
                                if (Q.IDDAĐung == Q.Answer)
                                    db_context.CTBaiThi_insert(IDBaiThi, Q.IDCH, Q.IDDAĐung, Q.Answer, Q.Diem);
                                else
                                    db_context.CTBaiThi_insert(IDBaiThi, Q.IDCH, Q.IDDAĐung, Q.Answer, 0);
                                i++;

                            }    
                          
                            
                        }
                        else{
                            if (Q.IDDAĐung == Q.Answer)
                                db_context.CTBaiThi_insert(IDBaiThi, Q.IDCH, Q.IDDAĐung, Q.Answer, Q.Diem);
                            else
                                db_context.CTBaiThi_insert(IDBaiThi, Q.IDCH, Q.IDDAĐung, Q.Answer, 0);
                        }
                    }
                    double diemso = (double)db_context.CTBaiThis.Where(x => x.IDBaiThi == IDBaiThi).Sum(x => x.Diem);
                    var bt= db_context.DeThis.Where(x => x.IDDeThi== IDDeThi).SingleOrDefault();
                    if(diemso>=bt.DiemChuan)
                    {
                        db_context.BaiThi_Update(diemso, true, IDBaiThi);
                        TempData["msgSuccess"] = "<script>alert('Chúc mừng bạn đã hoàn thành bài thi với số điểm là: "+ diemso + "');</script>";
                    }    
                    else
                    {
                        db_context.BaiThi_Update(diemso, false, IDBaiThi);
                        TempData["msgSuccess"] = "<script>alert('Bài thi của bạn đạt số điểm là: " + diemso + ". Bạn cần tham gia thi lại');</script>";
                    }
                    // cập nhật lại điểm online
                    var xnht = db_context.XNHocTaps.FirstOrDefault(x => x.LHID == ListQ[0].IDLH && x.NVID == MyAuthentication.ID);
                    if(xnht != null)xnht.DiemOnline = diemso;
                    db_context.SaveChanges();
                    
                }    
                
                
                return RedirectToAction("Index", "EClassroom");
            }
            else { return RedirectToAction("", "Login"); }
            
        }

        public JsonResult AddEvent()
        {
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db_context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}