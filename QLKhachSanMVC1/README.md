# Hệ thống quản lý khách sạn - Frontend ASP.NET MVC

Frontend ASP.NET MVC cho hệ thống quản lý khách sạn, sử dụng Razor Views và JavaScript để gọi API .NET 8.

## 🚀 Tính năng

- **Quản lý loại phòng**: CRUD đầy đủ cho loại phòng
- **Quản lý phòng**: Quản lý thông tin phòng và trạng thái
- **Quản lý khách hàng**: Thông tin khách hàng
- **Quản lý nhân viên**: Thông tin nhân viên
- **Quản lý dịch vụ**: Các dịch vụ khách sạn
- **Đặt phòng**: Form đặt phòng với nhiều phòng và dịch vụ
- **Thanh toán**: Quản lý thanh toán
- **Dashboard**: Thống kê tổng quan

## 📋 Yêu cầu hệ thống

- .NET 8 SDK
- ASP.NET MVC
- API Backend chạy tại `https://localhost:7158`

## 🛠️ Cài đặt

### 1. Cấu trúc thư mục

```
HotelManagement/
├── Controllers/
│   ├── HomeController.cs
│   ├── LoaiPhongController.cs
│   ├── PhongController.cs
│   ├── KhachHangController.cs
│   ├── DatPhongController.cs
│   ├── DichVuController.cs
│   └── ThanhToanController.cs
├── Views/
│   ├── Shared/
│   │   └── _Layout.cshtml
│   ├── Home/
│   │   └── Index.cshtml
│   ├── LoaiPhong/
│   │   ├── Index.cshtml
│   │   ├── Create.cshtml
│   │   ├── Edit.cshtml
│   │   └── Details.cshtml
│   └── DatPhong/
│       ├── Index.cshtml
│       ├── Create.cshtml
│       └── Details.cshtml
├── wwwroot/
│   ├── css/
│   │   └── site.css
│   └── js/
│       ├── site.js
│       ├── loaiPhong.js
│       └── datPhong.js
└── README.md
```

### 2. Cấu hình API

Trong file `wwwroot/js/site.js`, cập nhật URL API:

```javascript
const API_CONFIG = {
    baseUrl: 'https://localhost:7158/api', // Thay đổi URL API nếu cần
    timeout: 30000
};
```

### 3. Cấu hình CORS (nếu cần)

Nếu gặp lỗi CORS, thêm vào API backend:

```csharp
// Trong Program.cs hoặc Startup.cs
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("https://localhost:5001", "http://localhost:5000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

app.UseCors("AllowFrontend");
```

## 🎯 Sử dụng

### 1. Chạy ứng dụng

```bash
dotnet run
```

Truy cập: `https://localhost:5001` hoặc `http://localhost:5000`

### 2. Đảm bảo API Backend đang chạy

API phải chạy tại `https://localhost:7158` với các endpoints:

- `GET /api/LoaiPhong` - Danh sách loại phòng
- `POST /api/LoaiPhong` - Tạo loại phòng
- `PUT /api/LoaiPhong/{id}` - Cập nhật loại phòng
- `DELETE /api/LoaiPhong/{id}` - Xóa loại phòng
- `GET /api/DatPhong` - Danh sách đặt phòng
- `POST /api/DatPhong` - Tạo đặt phòng
- `PUT /api/DatPhong/{id}/checkin` - Check-in
- `PUT /api/DatPhong/{id}/checkout` - Check-out
- Và các endpoints khác...

## 📱 Giao diện

### Dashboard
- Thống kê tổng quan: tổng phòng, phòng trống, đặt phòng hôm nay, doanh thu
- Danh sách đặt phòng gần đây
- Biểu đồ trạng thái phòng

### Quản lý loại phòng
- Danh sách loại phòng với tìm kiếm và phân trang
- Form tạo/sửa loại phòng với validation
- Chi tiết loại phòng

### Đặt phòng
- Form đặt phòng với:
  - Chọn khách hàng và nhân viên
  - Thêm nhiều phòng (dynamic rows)
  - Thêm nhiều dịch vụ (dynamic rows)
  - Tính toán tổng tiền tự động
- Quản lý trạng thái: Check-in, Check-out, Hủy

## 🔧 Tùy chỉnh

### 1. Thêm resource mới

Để thêm quản lý resource mới (ví dụ: NhanVien):

1. **Tạo Controller:**
```csharp
public class NhanVienController : Controller
{
    public IActionResult Index() => View();
    public IActionResult Create() => View();
    public IActionResult Edit(int id) { ViewBag.Id = id; return View(); }
    public IActionResult Details(int id) { ViewBag.Id = id; return View(); }
}
```

2. **Tạo Views:** Copy từ `LoaiPhong` và sửa tên
3. **Tạo JavaScript:** Copy từ `loaiPhong.js` và sửa endpoint API
4. **Thêm vào navbar:** Cập nhật `_Layout.cshtml`

### 2. Thay đổi API URL

Sửa trong `wwwroot/js/site.js`:

```javascript
const API_CONFIG = {
    baseUrl: 'https://your-api-url.com/api',
    timeout: 30000
};
```

### 3. Thêm authentication

Nếu cần authentication, thêm vào `site.js`:

```javascript
// Thêm token vào headers
const token = localStorage.getItem('token');
options.headers = {
    'Authorization': `Bearer ${token}`,
    'Content-Type': 'application/json',
    ...options.headers
};
```

## 🐛 Xử lý lỗi

### Lỗi CORS
```
Access to fetch at 'https://localhost:7158/api/...' from origin 'https://localhost:5001' has been blocked by CORS policy
```
**Giải pháp:** Cấu hình CORS trong API backend (xem phần Cấu hình CORS)

### Lỗi 404 API
```
GET https://localhost:7158/api/LoaiPhong 404 (Not Found)
```
**Giải pháp:** Kiểm tra API backend có đang chạy và có endpoint này không

### Lỗi validation
Form validation được xử lý tự động với HTML5 validation và JavaScript

## 📚 API Endpoints cần thiết

### LoaiPhong
- `GET /api/LoaiPhong` - Danh sách
- `GET /api/LoaiPhong/{id}` - Chi tiết
- `POST /api/LoaiPhong` - Tạo mới
- `PUT /api/LoaiPhong/{id}` - Cập nhật
- `DELETE /api/LoaiPhong/{id}` - Xóa

### DatPhong
- `GET /api/DatPhong` - Danh sách
- `GET /api/DatPhong/{id}` - Chi tiết
- `POST /api/DatPhong` - Tạo mới
- `PUT /api/DatPhong/{id}/checkin` - Check-in
- `PUT /api/DatPhong/{id}/checkout` - Check-out
- `DELETE /api/DatPhong/{id}` - Hủy đặt phòng

### Các resource khác
- `GET /api/Phong` - Danh sách phòng
- `GET /api/KhachHang` - Danh sách khách hàng
- `GET /api/NhanVien` - Danh sách nhân viên
- `GET /api/DichVu` - Danh sách dịch vụ

## 🎨 Styling

- **Bootstrap 5**: Giao diện responsive
- **FontAwesome**: Icons
- **Toastr**: Thông báo
- **Custom CSS**: Styling bổ sung trong `site.css`

## 📝 Ghi chú

- Tất cả dữ liệu được lấy từ API backend
- Form validation sử dụng HTML5 và JavaScript
- Responsive design cho mobile và desktop
- Loading states và error handling đầy đủ
- Có thể dễ dàng mở rộng cho các resource mới

## 🤝 Đóng góp

1. Fork project
2. Tạo feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to branch (`git push origin feature/AmazingFeature`)
5. Mở Pull Request

## 📄 License

Distributed under the MIT License. See `LICENSE` for more information.
