using PagedList;
using E_Learning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_Learning.Controllers
{
    public class EmloyeeManageController : Controller
    {
        ELEARNINGEntities db_context = new ELEARNINGEntities();
        // GET: EmloyeeManage
        public ActionResult Index(int? page)
        {
            var res = from a in db_context.NhanVien_select()
                      select new EmployeeValidation
                      {
                          ID = a.ID,
                          MaNV = a.MaNV,
                          HoTen = a.HoTen,
                          IDPhongBan = (int)a.IDPhongBan,
                          IsGV = (bool)a.IsGV ,
                      };
            if (page == null) page = 1;
            int pageSize = 20;
            int pageNumber = (page ?? 1);
            return View(res.ToList().ToPagedList(pageNumber, pageSize));
        }
    }
}