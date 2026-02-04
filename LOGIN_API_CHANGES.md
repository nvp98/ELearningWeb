# Cập nhật Login Controller - Hỗ trợ đăng nhập qua API

## Tóm tắt thay đổi

Đã cập nhật `LoginController.cs` để hỗ trợ đăng nhập qua API ngoài việc đăng nhập từ database cục bộ.

## Chi tiết thay đổi

### 1. Thêm các using directives cần thiết

```csharp
using System.Configuration;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;
```

### 2. Cập nhật phương thức `LoginUser` (HttpPost)

**Quy trình đăng nhập (được thực hiện theo thứ tự):**

- Bước 1: Thử đăng nhập từ database cục bộ (như trước)
- Bước 2: Nếu không tìm thấy user cục bộ, thử đăng nhập qua API
- Bước 3: Nếu API trả về thành công, kiểm tra user có tồn tại trong database không
- Bước 4: Nếu user tồn tại và đang hoạt động (IDTinhTrangLV == 1), tạo session và chuyển hướng
- Bước 5: Nếu tất cả đều thất bại, hiển thị thông báo lỗi

### 3. Thêm hai phương thức riêng tư (Private Methods)

#### a) `GetTokenFromAPI(string username, string password) : string`

**Chức năng:** Lấy token từ API bằng cách gửi yêu cầu POST tới endpoint được cấu hình trong `LinkToken`

**Quy trình:**

- Đọc URL từ Web.config: `ConfigurationManager.AppSettings["LinkToken"]`
- Gửi POST request với JSON body chứa username và password
- Parse response JSON và trích xuất token từ `data.tokenLogin`
- Trả về token hoặc chuỗi rỗng nếu có lỗi

**Xử lý lỗi:**

- WebException: Xảy ra khi không thể kết nối tới API
- Exception: Lỗi chung khác
- Tất cả lỗi đều được ghi vào Debug output

#### b) `LoginViaAPI(string username, string password) : APILoginResult`

**Chức năng:** Thực hiện đăng nhập qua API và trả về kết quả chi tiết

**Trả về đối tượng `APILoginResult` với thông tin:**

- `Success`: Boolean - Đăng nhập có thành công không
- `Token`: String - Token từ API
- `Message`: String - Thông báo mô tả kết quả

### 4. Thêm lớp `APILoginResult` (Private Class)

```csharp
private class APILoginResult
{
    public bool Success { get; set; }
    public string Token { get; set; }
    public string Message { get; set; }
}
```

## Cấu hình cần thiết trong Web.config

```xml
<appSettings>
    <add key="LinkToken" value="https://hr.hoaphatdungquat.vn/hpdq_api/api/User/GetToken" />
    <!-- Các cấu hình khác... -->
</appSettings>
```

## Luồng xử lý toàn bộ

```
LoginUser (nhận MaNV và MatKhau)
  ↓
Kiểm tra input có rỗng không
  ↓
Thử tìm user từ database cục bộ với MD5 password
  ├─ Thành công: Tạo session, chuyển hướng về Home
  └─ Thất bại: Chuyển sang API
      ↓
      Gọi LoginViaAPI
        ├─ Lấy token từ API
        ├─ Nếu token rỗng: Trả về Success=false
        └─ Nếu có token: Trả về Success=true, Token
      ↓
      Kiểm tra kết quả API
      ├─ Success=true:
      │   ├─ Tìm user trong database với MaNV
      │   └─ Nếu found & active: Tạo session, chuyển hướng
      └─ Success=false: Hiển thị lỗi
```

## Ưu điểm của giải pháp này

1. **Tương thích ngược**: Vẫn hỗ trợ đăng nhập cục bộ
2. **Linh hoạt**: Cho phép sử dụng API như một giải pháp dự phòng
3. **An toàn**: Không lưu token vào database, chỉ xác thực khi có yêu cầu
4. **Dễ bảo trì**: Code được tổ chức rõ ràng với các method riêng biệt
5. **Xử lý lỗi**: Có try-catch để xử lý các ngoại lệ mạng

## Các bước triển khai

1. Xác nhận `LinkToken` đã được cấu hình chính xác trong Web.config
2. Kiểm tra API endpoint có hoạt động không
3. Test đăng nhập với cả database cục bộ và API
4. Kiểm tra log Debug để xem các thông báo lỗi nếu có

## Ghi chú

- Timeout cho API request được đặt là 30 giây
- Nếu API không phản hồi, hệ thống sẽ quay trở lại sử dụng database cục bộ
- Token từ API được lưu trong `APILoginResult.Token` để có thể sử dụng sau này nếu cần
