using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Core.Objects;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using E_Learning.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace E_Learning.Controllers
{
    
    public class EmployeeAPIController : Controller
    {
        // GET: Employee
        ELEARNINGEntities _db = new ELEARNINGEntities();
        int Idquyen = MyAuthentication.IDQuyen;
        String ControllerName = "Account";
        public ActionResult Index()
        {

            return View();
        }

        public String GetToken()
        {
            string url = ConfigurationManager.AppSettings["LinkToken"];
            string username = ConfigurationManager.AppSettings["Username"];
            string password = ConfigurationManager.AppSettings["Password"];
            var httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.Method = "POST";
            httpRequest.ContentType = "application/json";
            var data = @"{
                          ""username"":""" + username + @""",
                          ""password"":""" + password + @"""
                        }";
            using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
            {
                streamWriter.Write(data);
            }
            var token = "";
            try
            {
                WebResponse httpResponse = httpRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {

                    var result = streamReader.ReadToEnd();
                    JObject json = JObject.Parse(result);
                    var a = json["data"]["tokenLogin"].ToString();
                    token = a;
                }
            }
            catch (WebException webex)
            {
                WebResponse errResp = webex.Response;
                using (Stream respStream = errResp.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(respStream);
                    string text = reader.ReadToEnd();
                }
            }
            return token;
        }

        List<Employees_API.data> GetAPI()
        {
            var GetTo = GetToken();
            string link = ConfigurationManager.AppSettings["LinkAPI"];
            using (WebClient webClient = new WebClient())
            {
                var token = "Bearer " + GetTo;

                webClient.Encoding = Encoding.UTF8;
                webClient.Headers.Add("Authorization", token);
                var json = webClient.DownloadString(link);
                //Getdata(json);
                //Json khong dung chuan
                //=>Nen cat chuoi, cat chuoi the nay cu chuoi qua
                json = json.Remove(0, 36);
                json = json.Replace("}]}","}]");
                var list = JsonConvert.DeserializeObject<List<Employees_API.data>>(json);
                return list.ToList();
            }

        }
        public string Getdata(string Content)
        {
            string pattern = "[^data$]]";
            Regex Title = new Regex(pattern);
            Match m = Title.Match(Content);
            if (m.Success)
                return m.ToString();
            return "";
        }
        public int GetIDPhongBan(string TenPB)
        {
            var pb = _db.PhongBans.Where(x => x.TenPhongBan == TenPB).SingleOrDefault();
            //if (pb != null &&  pb.MaPB != maphongban) _db.PhongBan_update_KNL(pb.IDPhongBan, TenPB, maphongban);
            var model = _db.PhongBans.Where(x => x.TenPhongBan == TenPB).SingleOrDefault();
            if (model == null)
                return 0;
            //if(TenPB != model.TenPhongBan) _db.PhongBan_update_KNL(model.IDPhongBan, TenPB, maphongban);
            return model.IDPhongBan;
        }
        public int GetIDViTri(string TenViTri)
        {
            var model = _db.Vitris.Where(x => x.TenViTri == TenViTri).SingleOrDefault();
            if (model == null)
                return 0;
            return model.IDViTri;
        }
        public static string convertToUnSign(string s)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }
        public ActionResult Sync()
        {
            var ListQuyen = new HomeController().GetPermisionCN(Idquyen, ControllerName);
            if (!ListQuyen.Contains(CONSTKEY.SYC))
            {
                TempData["msgError"] = "<script>alert('Bạn không có quyền thực hiện chức năng này');</script>";
                return RedirectToAction("", "Home");
            }
            string MaNV, sMaNV;
            int IDViTri, IDPhongBan;
            int dtc = 0;
            string msg = "";
            if (User.Identity.IsAuthenticated)
            {
               
                List<NhanVien> LNV = _db.NhanViens.ToList();
                     
                    List<Employees_API.data> listNV = GetAPI();
                    if (listNV.Count > 0)
                    {
                        foreach (var item in listNV)
                        {
                            if (item.manv != null)
                            {
                                MaNV = item.manv;
                                sMaNV = MaNV.Substring(0, 4);

                                var rsnv = LNV.Where(x => x.MaNV == MaNV).FirstOrDefault();
                                if (rsnv == null)
                                {
                                    if (sMaNV == "HPDQ")
                                    {
                                        ObjectParameter IDPhongBanout = new ObjectParameter("IDPhongBan", typeof(int));
                                        ObjectParameter IDViTriout = new ObjectParameter("IDViTri", typeof(int));
                                        IDPhongBan = GetIDPhongBan(item.phongban);
                                        if (IDPhongBan == 0)
                                        {
                                            _db.PhongBan_insert(IDPhongBanout, item.phongban);
                                            IDPhongBan = Convert.ToInt32(IDPhongBanout.Value);
                                        }
                                        IDViTri = GetIDViTri(item.vitri);
                                        if (IDViTri == 0)
                                        {
                                            _db.Vitri_insert(IDViTriout, item.vitri);
                                            IDViTri = Convert.ToInt32(IDViTriout.Value);
                                        }

                                    _db.Nhanvien_insert_API(MaNV, item.hoten, convertToUnSign(item.hoten), item.diachi, item.sodienthoai, DateTime.ParseExact(item.ngayvaolam, "dd/MM/yyyy", CultureInfo.InvariantCulture), IDPhongBan, item.tinhtranglamviec, IDViTri, item.mavitri, int.Parse(item.makip));

                                }
                                }
                                else
                                {
                                    if (sMaNV == "HPDQ")
                                    {
                                        //if (item.tinhtranglamviec == 0)
                                        //{
                                        //    _db.Nhanvien_update_IDKNL(MaNV, null);
                                        //}
                                        ObjectParameter IDPhongBanout = new ObjectParameter("IDPhongBan", typeof(int));
                                        ObjectParameter IDViTriout = new ObjectParameter("IDViTri", typeof(int));
                                        IDPhongBan = GetIDPhongBan(item.phongban);
                                        if (IDPhongBan == 0)
                                        {
                                            _db.PhongBan_insert(IDPhongBanout, item.phongban);
                                            IDPhongBan = Convert.ToInt32(IDPhongBanout.Value);
                                        }
                                        IDViTri = GetIDViTri(item.vitri);
                                        if (IDViTri == 0)
                                        {
                                            _db.Vitri_insert(IDViTriout, item.vitri);
                                            IDViTri = Convert.ToInt32(IDViTriout.Value);
                                        }
                                        if(rsnv.IDPhongBan!=IDPhongBan || rsnv.IDViTri!=IDViTri || rsnv.IDTinhTrangLV!=item.tinhtranglamviec || rsnv.MaViTri != item.mavitri || rsnv.IDKip != int.Parse(item.makip))
                                        {
                                        if (item.tinhtranglamviec == 0)
                                        {
                                            _db.Nhanvien_update_IDKNL(MaNV, null);
                                        }
                                        _db.Nhanvien_update_API(MaNV, item.diachi, item.sodienthoai, IDPhongBan, item.tinhtranglamviec, IDViTri, item.mavitri, int.Parse(item.makip));
                                        dtc++;
                                        }    
                                        
                                    }
                                }
                            }

                        }
                        if (dtc != 0)
                        {
                            msg = "Cập nhật thông tin được " + dtc + " nhân viên";
                        }
                        TempData["msgSuccess"] = "<script>alert('" + msg + "');</script>";
                        return RedirectToAction("Index", "Account");
                    }
                
            }
            else
            {
                return RedirectToAction("", "Login");
            }
            return View();
        }
    }
}