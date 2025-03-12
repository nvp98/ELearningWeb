using DocumentFormat.OpenXml.Drawing.Charts;
using E_Learning.Models;
using E_Learning.ModelsDTTH;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E_Learning.Services
{
    public class ChuongTrinhDTService
    {
        private readonly ELEARNINGEntities _context;

        public ChuongTrinhDTService(ELEARNINGEntities context)
        {
            _context = context;
        }

        public SH_ChuongTrinhDT ThemChuongTrinhDT(ChuongTrinhDTTHView chuongtrinh, string filePathSave)
        {
            try
            {
                var parent = new SH_ChuongTrinhDT()
                {
                  TenChuongTrinhDT = chuongtrinh.TenChuongTrinhDT,
                  NoiDungTrichYeu = chuongtrinh.NoiDungTrichYeu,
                  ThoiLuongDT = chuongtrinh.ThoiLuongDT,
                  FileDinhKem = chuongtrinh.FileDinhKem,
                  IDPhuongPhapDT = chuongtrinh.IDPhuongPhapDT,
                  IDPhongBan = chuongtrinh.IDPhongBan,
                  IsDelete = false,
                  FileCTDT = chuongtrinh.FileCTDT,
                  IDKiemTra = chuongtrinh.IDKiemTra,
                  //ID_LVDT = chuongtrinh.ID_LVDT,
                  //ID_NhomNL = chuongtrinh.ID_NhomNL,
                  DoiTuongDT = chuongtrinh.DoiTuongDT,
                  NgayTao= DateTime.Now,
                  ID_NguoiTao = chuongtrinh.ID_NguoiTao,
                  ID_NoiDungDT = chuongtrinh.ID_NoiDungDT,
                  TinhTrang =2,
                };

                _context.SH_ChuongTrinhDT.Add(parent);
                _context.SaveChanges(); // .NET Framework không hỗ trợ SaveChangesAsync()
                // Thêm chi tiết CTĐT 
                if (parent.IDCTDT != 0 && chuongtrinh.sH_ChiTietCTDTs.Count != 0)
                {
                    foreach (var item in chuongtrinh.sH_ChiTietCTDTs)
                    {
                        if(!string.IsNullOrWhiteSpace(item.NoiDungDT) && item.ThoiGianDT != null && !string.IsNullOrWhiteSpace(item.YeuCau))
                        {
                            var chitiet = new SH_ChiTietCTDT()
                            {
                                ID_ChuongTrinhDT = parent.IDCTDT,
                                NoiDungDT = item.NoiDungDT,
                                YeuCau = item.YeuCau,
                                ThoiGianDT = item.ThoiGianDT,
                                GhiChu = item.GhiChu
                            };
                            _context.SH_ChiTietCTDT.Add(chitiet);
                            _context.SaveChanges();
                        }
                    }
                }
                else return null;
                return parent;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi lưu dữ liệu: " + ex.Message);
                return null;
            }
        }

        public SH_ChuongTrinhDT CapNhatChuongTrinhDT(ChuongTrinhDTTHView chuongtrinh)
        {
            try
            {
                SH_ChuongTrinhDT ct = _context.SH_ChuongTrinhDT.Where(x=>x.IDCTDT == chuongtrinh.IDCTDT).FirstOrDefault();
                if(ct == null) return null;
                //ct.TenChuongTrinhDT = chuongtrinh.TenChuongTrinhDT;
                ct.NoiDungTrichYeu = chuongtrinh.NoiDungTrichYeu;
                ct.ThoiLuongDT = chuongtrinh.ThoiLuongDT;
                ct.FileDinhKem = chuongtrinh.FileDinhKem;
                ct.DoiTuongDT = chuongtrinh.DoiTuongDT;
                ct.IDKiemTra = chuongtrinh.IDKiemTra;
                //IsDelete = false,
                if (chuongtrinh.FileCTDT != null)
                {
                    ct.FileCTDT = chuongtrinh.FileCTDT;
                }
                //FileCTDT = chuongtrinh.FileCTDT,
                //TinhTrang = 2,
                _context.SaveChanges();
                // Cập nhật chi tiết CTĐT 
                if (ct.IDCTDT != 0 && chuongtrinh.sH_ChiTietCTDTs.Count != 0)
                {
                    foreach (var item in chuongtrinh.sH_ChiTietCTDTs)
                    {
                        //xóa dữ liệu đã có
                        if(string.IsNullOrWhiteSpace(item.NoiDungDT) && item.ThoiGianDT == null && string.IsNullOrWhiteSpace(item.YeuCau) && item.ID != 0)
                        {
                            _context.SH_ChiTietCTDT.Remove(item);
                            _context.SaveChanges();
                        }
                        if (!string.IsNullOrWhiteSpace(item.NoiDungDT) && item.ThoiGianDT != null && !string.IsNullOrWhiteSpace(item.YeuCau))
                        {
                            // cập nhật dữ liệu đã có
                            if(item.ID != 0)
                            {
                                var chitietcapnhat = _context.SH_ChiTietCTDT.Where(x=>x.ID == item.ID).FirstOrDefault();
                                chitietcapnhat.NoiDungDT = item.NoiDungDT;
                                chitietcapnhat.YeuCau = item.YeuCau;
                                chitietcapnhat.ThoiGianDT = item.ThoiGianDT;
                                chitietcapnhat.GhiChu = item.GhiChu;
                            }
                            else
                            {
                                // thêm mới
                                var chitiet = new SH_ChiTietCTDT()
                                {
                                    ID_ChuongTrinhDT = ct.IDCTDT,
                                    NoiDungDT = item.NoiDungDT,
                                    YeuCau = item.YeuCau,
                                    ThoiGianDT = item.ThoiGianDT,
                                    GhiChu = item.GhiChu
                                };
                                _context.SH_ChiTietCTDT.Add(chitiet);
                            }
                           
                            _context.SaveChanges();
                        }
                    }
                }
                //else return null;
                return ct;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi lưu dữ liệu: " + ex.Message);
                return null;
            }
        }


        //public int ThemCauHoiDeThiCTDT(CauHoiDeThiCTDTTH cauhoi, string filePathSave)
        //{

        //}
        public bool ThemTrinhKyChuongTrinhDT(ChuongTrinhDTTHView chuongtrinh)
        {
            try
            {
                var parent = new SH_KyDuyetCTDT()
                {
                   ID_CTDT = chuongtrinh.IDCTDT,
                    ID_NguoiTao = MyAuthentication.ID,
                    NgayTao = DateTime.Now,
                    ID_NguoiKiemTra = chuongtrinh.sH_KyDuyetCTDTs.ID_NguoiKiemTra,
                    NgayKTDuyet = null,
                    ID_TPBP = chuongtrinh.sH_KyDuyetCTDTs.ID_TPBP,
                    NgayTPBP = null,
                    ID_PCHN = chuongtrinh.sH_KyDuyetCTDTs.ID_PCHN,
                    NgayPCHN = null,
                    ID_NguoiDuyetNDDT = chuongtrinh.sH_KyDuyetCTDTs.ID_NguoiDuyetNDDT,
                    NgayDuyetNDDT = null,
                    ID_NguoiDangNDDT = chuongtrinh.sH_KyDuyetCTDTs.ID_NguoiDangNDDT,
                    NgayDangNDDT = null
                };

                _context.SH_KyDuyetCTDT.Add(parent);
                _context.SaveChanges(); // .NET Framework không hỗ trợ SaveChangesAsync()
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi lưu dữ liệu: " + ex.Message);
                return false;
            }
        }

    }
}