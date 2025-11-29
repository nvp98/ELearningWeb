using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using E_Learning.Common;
using E_Learning.Models;
using Newtonsoft.Json.Linq;

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
            if (u.MaNV != "" && u.MatKhau != "" && u.MaNV != null && u.MatKhau != null)
            {
                // Thử login từ database cục bộ
                string mk = Common.Encryptor.MD5Hash(u.MatKhau);
                NhanVien user = _db.NhanViens.Where(x => x.MaNV == u.MaNV && x.MatKhau == mk && x.IDTinhTrangLV == 1).FirstOrDefault();
                if (user != null)
                {
                    string Cookie = string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8}", user.ID, user.MaNV, user.HoTen, user.IDPhongBan, user.IDQuyen, user.IDViTri, user.IDQuyenKNL, user.IDVTKNL, user.MaViTri);
                    FormsAuthentication.SetAuthCookie(Cookie, false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    // Nếu login cục bộ không thành công, thử login qua API
                    var apiLoginResult = LoginViaAPI(u.MaNV, u.MatKhau);
                    if (apiLoginResult.Success)
                    {
                        // Tìm hoặc tạo user từ thông tin API
                        user = _db.NhanViens.Where(x => x.MaNV == u.MaNV).FirstOrDefault();
                        if (user != null && user.IDTinhTrangLV == 1)
                        {
                            string Cookie = string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8}", user.ID, user.MaNV, user.HoTen, user.IDPhongBan, user.IDQuyen, user.IDViTri, user.IDQuyenKNL, user.IDVTKNL, user.MaViTri);
                            FormsAuthentication.SetAuthCookie(Cookie, false);
                            return RedirectToAction("Index", "Home");
                        }
                    }

                    TempData["msglg"] = "<script>alert('Tài khoản hoặc mật khẩu không đúng, liên hệ B.CNTT nếu bạn quên mật khẩu')</script>";
                    return RedirectToAction("", "Login");
                }
            }
            else
            {
                TempData["msglg"] = "<script>alert('Vui lòng nhập tài khoản và mật khẩu')</script>";
                return RedirectToAction("", "Login");
            }
        }


        /// <summary>
        /// Lấy token từ API (LinkToken)
        /// </summary>
        /// <param name="username">Tên đăng nhập</param>
        /// <param name="password">Mật khẩu</param>
        /// <returns>Token hoặc rỗng nếu lỗi</returns>
        private string GetTokenFromAPI(string username, string password)
        {
            try
            {
                string url = ConfigurationManager.AppSettings["LinkToken"];
                if (string.IsNullOrEmpty(url))
                    return "";

                var httpRequest = (HttpWebRequest)WebRequest.Create(url);
                httpRequest.Method = "POST";
                httpRequest.ContentType = "application/json";
                httpRequest.Timeout = 30000;

                var data = @"{
                              ""username"":""" + username + @""",
                              ""password"":""" + password + @"""
                            }";

                using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
                {
                    streamWriter.Write(data);
                }

                WebResponse httpResponse = httpRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    JObject json = JObject.Parse(result);
                    var checkdata = json["data"].ToString();

                    // Kiểm tra xem có thành công không
                    if (checkdata != "")
                    {
                        var token = json["data"]["tokenLogin"]?.ToString();
                        return token ?? "";
                    }
                }
            }
            catch (WebException webex)
            {
                // Ghi log lỗi nếu cần
                System.Diagnostics.Debug.WriteLine("GetTokenFromAPI Error: " + webex.Message);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("GetTokenFromAPI Exception: " + ex.Message);
            }

            return "";
        }

        /// <summary>
        /// Lớp kết quả đăng nhập API
        /// </summary>
        private class APILoginResult
        {
            public bool Success { get; set; }
            public string Token { get; set; }
            public string Message { get; set; }
        }

        /// <summary>
        /// Đăng nhập qua API
        /// </summary>
        /// <param name="username">Tên đăng nhập</param>
        /// <param name="password">Mật khẩu</param>
        /// <returns>Kết quả đăng nhập</returns>
        private APILoginResult LoginViaAPI(string username, string password)
        {
            var result = new APILoginResult { Success = false };

            try
            {
                string token = GetTokenFromAPI(username, password);
                if (string.IsNullOrEmpty(token))
                {
                    result.Message = "Không thể lấy token từ API";
                    return result;
                }

                // Nếu có token, coi như đăng nhập thành công
                // Có thể thêm logic kiểm tra token với API khác tại đây
                result.Success = true;
                result.Token = token;
                result.Message = "Đăng nhập API thành công";
            }
            catch (Exception ex)
            {
                result.Message = "Lỗi đăng nhập API: " + ex.Message;
                System.Diagnostics.Debug.WriteLine("LoginViaAPI Exception: " + ex.Message);
            }

            return result;
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

        public ActionResult CapNhatChuKy()
        {
            var nv = _db.NhanViens.Where(x => x.ID == MyAuthentication.ID).FirstOrDefault();
            ChuKyView ck = new ChuKyView();
            ck.ChuKy = nv.ChuKy;
            ck.IDNV = nv.ID;
            return View(ck);
        }
        [HttpPost]
        public ActionResult CapNhatChuKy(ChuKyView model)
        {
            if (User.Identity.IsAuthenticated)
            {
                string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
                var nv = _db.NhanViens.Where(x => x.ID == model.IDNV).FirstOrDefault();
                string filePathSave = null;
                //upload file NCDT
                if (model.FileChuKy != null)
                {
                    string path = Server.MapPath("~/FileChuKy/");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    //Use Namespace called :  System.IO  
                    string FileName = model.FileChuKy.FileName;
                    string FileNameSave = nv.MaNV + "_" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    //To Get File Extension  
                    string FileExtension = model.FileChuKy != null ? Path.GetExtension(FileName) : "";
                    //Add Current Date To Attached File Name  
                    if (!_allowedExtensions.Contains(FileExtension?.ToLower()))
                    {
                        ViewBag.Message = "<script>alert('Chọn đúng định dạng hình ảnh  \".jpg\", \".jpeg\", \".png\", \".gif\", \".bmp\"')</script>";
                        //TempData["msg"] = "<script>alert('Lỗi thay đổi mật khẩu')</script>";
                        //return View();
                    }
                    else
                    {
                        FileNameSave = FileNameSave.Trim() + FileExtension;
                        model.FileChuKy.SaveAs(path + FileNameSave);
                        filePathSave = "~/FileChuKy/" + FileNameSave;
                        nv.ChuKy = filePathSave;
                        _db.SaveChanges();
                        ViewBag.Message = "<script>alert('Cập nhật chữ ký thành công')</script>";
                        //TempData["msg"] = "<script>alert('Lỗi thay đổi mật khẩu')</script>";

                    }
                }
                return CapNhatChuKy();
            }
            else
            {
                ViewBag.Message = "<script>alert('Lỗi thay đổi mật khẩu')</script>";
                //TempData["msg"] = "<script>alert('Lỗi thay đổi mật khẩu')</script>";
                return CapNhatChuKy();
            }

        }
    }
}