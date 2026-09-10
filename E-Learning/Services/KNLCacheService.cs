using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace E_Learning.Services
{
    // DTO nhỏ gọn — chỉ chứa field KNL module thực sự dùng, tránh circular ref từ virtual nav properties
    public class NhanVienCacheDto
    {
        public int ID { get; set; }
        public string MaNV { get; set; }
        public string HoTen { get; set; }
        public int? IDPhongBan { get; set; }
        public int? IDVTKNL { get; set; }
        public int? IDTinhTrangLV { get; set; }
        public int? IDKip { get; set; }
        public int? IDQuyen { get; set; }
        public int? IDQuyenKNL { get; set; }
        public string MaViTri { get; set; }
    }

    public static class KNLCacheService
    {
        private static readonly Lazy<ConnectionMultiplexer> _lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            string conn = ConfigurationManager.AppSettings["RedisConnection"] ?? "localhost:6379,abortConnect=false";
            return ConnectionMultiplexer.Connect(conn);
        });

        private static IDatabase Cache
        {
            get
            {
                try { return _lazyConnection.Value.GetDatabase(); }
                catch { return null; }
            }
        }

        // ── Cache-aside generic (List) ────────────────────────────────────────
        // Chỉ bọc try/catch quanh thao tác Redis — lỗi từ fetch() (DB) phải được
        // ném ra ngoài nguyên trạng để caller (vd. retry loop) xử lý, không nuốt rồi gọi lại fetch() lần 2.
        private static List<T> GetOrSet<T>(string key, Func<List<T>> fetch, TimeSpan ttl)
        {
            IDatabase db = null;
            try
            {
                db = Cache;
                if (db != null)
                {
                    var cached = db.StringGet(key);
                    if (cached.HasValue)
                        return JsonConvert.DeserializeObject<List<T>>(cached);
                }
            }
            catch
            {
                db = null;
            }

            var value = fetch();

            if (db != null && value != null)
            {
                try { db.StringSet(key, JsonConvert.SerializeObject(value), ttl); }
                catch { /* ghi cache lỗi — bỏ qua, không ảnh hưởng kết quả trả về */ }
            }

            return value;
        }

        // ── Cache-aside scalar (single object) ───────────────────────────────
        private static T GetOrSetSingle<T>(string key, Func<T> fetch, TimeSpan ttl) where T : class
        {
            IDatabase db = null;
            try
            {
                db = Cache;
                if (db != null)
                {
                    var cached = db.StringGet(key);
                    if (cached.HasValue)
                        return JsonConvert.DeserializeObject<T>(cached);
                }
            }
            catch
            {
                db = null;
            }

            var value = fetch();

            if (db != null && value != null)
            {
                try { db.StringSet(key, JsonConvert.SerializeObject(value), ttl); }
                catch { /* ghi cache lỗi — bỏ qua, không ảnh hưởng kết quả trả về */ }
            }

            return value;
        }

        // ── Invalidate ────────────────────────────────────────────────────────
        public static void Invalidate(params string[] keys)
        {
            try
            {
                var db = Cache;
                if (db == null) return;
                foreach (var key in keys)
                    if (key != null) db.KeyDelete(key);
            }
            catch { }
        }

        // ── Key builders ─────────────────────────────────────────────────────
        public static string KeyKQTheoQuy(int nam, int quy, int? idnv)      => $"knl:kq:{nam}:{quy}:{idnv}";
        public static string KeyLSDGTheoQuy(int nam, int? quy, int? idnv)   => $"knl:lsdg:{nam}:{quy}:{idnv}";
        public static string KeyDocBang(int idvt, int idnv)                 => $"knl:docbang:{idvt}:{idnv}";
        public static string KeyNVDanhGiaTT(int idvt)                       => $"knl:nvdgtt:{idvt}";
        public static string KeyNVDanhGiaTC(int idvt)                       => $"knl:nvdgtc:{idvt}";
        public static string KeyGenResult(int idvt, string thang)           => $"knl:genresult:{idvt}:{thang}";
        public static string KeyViTriDetail(int? idvt)                      => $"knl:vitri:detail:{idvt}";
        public static string KeyLoaiNL(int idvt)                            => $"knl:loainl:{idvt}";
        public static string KeyNVByMaNV(string manv)                       => $"nv:manv:{manv}";
        public static string KeyNVById(int id)                              => $"nv:id:{id}";
        public static string KeyNVListByIDVT(int idvt)                      => $"nv:idvt:{idvt}";

        // ── KNL evaluation cache ──────────────────────────────────────────────

        public static List<T> GetKQTheoQuy<T>(int nam, int quy, int? idnv, Func<List<T>> fetch)
            => GetOrSet(KeyKQTheoQuy(nam, quy, idnv), fetch, TimeSpan.FromMinutes(15));

        public static List<T> GetLSDGTheoQuy<T>(int nam, int? quy, int? idnv, Func<List<T>> fetch)
            => GetOrSet(KeyLSDGTheoQuy(nam, quy, idnv), fetch, TimeSpan.FromMinutes(15));

        public static List<T> GetDocBangKNL<T>(int idvt, int idnv, Func<List<T>> fetch)
            => GetOrSet(KeyDocBang(idvt, idnv), fetch, TimeSpan.FromHours(1));

        public static List<T> GetNVDanhGiaTT<T>(int idvt, Func<List<T>> fetch)
            => GetOrSet(KeyNVDanhGiaTT(idvt), fetch, TimeSpan.FromMinutes(10));

        public static List<T> GetNVDanhGiaTC<T>(int idvt, Func<List<T>> fetch)
            => GetOrSet(KeyNVDanhGiaTC(idvt), fetch, TimeSpan.FromMinutes(10));

        public static List<T> GetGenResult<T>(int idvt, string thang, Func<List<T>> fetch)
            => GetOrSet(KeyGenResult(idvt, thang), fetch, TimeSpan.FromMinutes(30));

        // ── NhanVien cache (TTL 30 phút) ─────────────────────────────────────

        /// <summary>Lấy NhanVien theo MaNV — dùng cho user đang đăng nhập</summary>
        public static NhanVienCacheDto GetNhanVienByMaNV(string manv, Func<NhanVienCacheDto> fetch)
            => GetOrSetSingle(KeyNVByMaNV(manv), fetch, TimeSpan.FromMinutes(30));

        /// <summary>Lấy NhanVien theo ID — dùng khi mở form đánh giá cho NV khác</summary>
        public static NhanVienCacheDto GetNhanVienById(int id, Func<NhanVienCacheDto> fetch)
            => GetOrSetSingle(KeyNVById(id), fetch, TimeSpan.FromMinutes(30));

        /// <summary>Lấy danh sách NhanVien đang làm việc theo vị trí KNL</summary>
        public static List<NhanVienCacheDto> GetNhanVienListByIDVT(int idvt, Func<List<NhanVienCacheDto>> fetch)
            => GetOrSet(KeyNVListByIDVT(idvt), fetch, TimeSpan.FromMinutes(30));
    }
}
