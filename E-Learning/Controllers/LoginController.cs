using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using E_Learning.Common;
using E_Learning.Models;

namespace E_Learning.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        ELEARNINGEntities _db = new ELEARNINGEntities();
        public ActionResult Index()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            Session.Abandon();
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult LoginUser(LoginValidation u)
        {
            if(u.MaNV!="" && u.MatKhau!="" && u.MaNV!=null && u.MatKhau!=null)
            {     
                string mk = Common.Encryptor.MD5Hash(u.MatKhau);
                NhanVien user = _db.NhanViens.Where(x => x.MaNV == u.MaNV && x.MatKhau == mk && x.IDTinhTrangLV == 1).FirstOrDefault();
                if (user != null)
                {
                    string Cookie = string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8}", user.ID, user.MaNV, user.HoTen, user.IDPhongBan, user.IDQuyen,user.IDViTri,user.IDQuyenKNL,user.IDVTKNL,user.MaViTri);
                    FormsAuthentication.SetAuthCookie(Cookie, false);
                    return RedirectToAction("Index", "EClassroom");

                }
                else
                {
                    TempData["msglg"] = "<script>alert('Tài khoản hoặc mật khẩu không đúng, liên hệ B.CNTT nếu bạn quên mật khẩu')</script>";
                    return RedirectToAction("", "Login");
                }
            }
            else {
                TempData["msglg"] = "<script>alert('Vui lòng nhập tài khoản và mật khẩu')</script>";
                return RedirectToAction("", "Login");
            }
        }


        public ActionResult Logout()
        {

            FormsAuthentication.SignOut();
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Index", "Login");

        }
        public ActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ChangePassword(LoginValidation model)
        {
            if (User.Identity.IsAuthenticated)
            {
                string mk = Encryptor.MD5Hash(model.MatKhauCu);
               
                NhanVien user = _db.NhanViens.SingleOrDefault(x => x.MaNV == MyAuthentication.Username && x.MatKhau == mk);
                if (user != null)
                {
                    //user.MatKhau = model.MatKhau;
                    mk = Encryptor.MD5Hash(model.MatKhau);
                    user.MatKhau = mk;
                    _db.SaveChanges();
                    Session.Clear();
                    Session.Abandon();
                    //TempData["msg"] = "<script>alert('Cập nhập thành công')</script>";
                    ViewBag.Message = "<script>alert('Thay đổi mật khẩu thành công');window.location.href = '/Login</script>";
                    //Page.ClientScript.RegisterClientScriptBlock(GetType(), "alerta", "alert('Save records with success')", true);
                    //return RedirectToAction("Index", "Login");
                    return View();
                }
                else
                {
                    ViewBag.Message = "<script>alert('Mật khẩu cũ không đúng, vui lòng nhập lại')</script>";
                    //TempData["msg"] = "<script>alert('Mật khẩu cũ không đúng, vui lòng nhập lại')</script>";
                    return View();
                }
            }
            else
            {
                ViewBag.Message = "<script>alert('Lỗi thay đổi mật khẩu')</script>";
                //TempData["msg"] = "<script>alert('Lỗi thay đổi mật khẩu')</script>";
                return View();
            }

        }
    }
}