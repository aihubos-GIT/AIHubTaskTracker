# AIHUB BACKEND CORE SYSTEM (Task Tracker)
## 1. Giới thiệu Dự án (Project Overview)

Hệ thống Backend cốt lõi quản lý Task nội bộ (PostgreSQL) và tự động đồng bộ hóa các thay đổi với ClickUp.

  * **Runtime:** .NET 8.0.
  * **Database:** PostgreSQL (Npgsql).
  * **Authentication:** JWT Bearer.
  * **Tích hợp:** ClickUp API, Telegram API.

-----

## 2\. Thiết lập Môi trường Phát triển (Local Setup)

### 2.1. Yêu cầu hệ thống (Prerequisites)

  * .NET SDK 8.0 trở lên.
  * PostgreSQL Database.
  * API Token của ClickUp (BẮT BUỘC có quyền `write` và `read`).

### 2.2. Cấu hình Bí mật và Khởi chạy

Các thông tin nhạy cảm được lấy từ `appsettings.json`.

| Cấu hình | Mô tả | Giá trị Mẫu |
| :--- | :--- | :--- |
| `ConnectionStrings:DefaultConnection` | Chuỗi kết nối PostgreSQL. | `Host=dpg-d3jt3hndiees738ocffg-a.oregon-postgres.render.com;...` |
| `Jwt:Key` | Khóa bí mật JWT. | `ThisIsASecretKeyForJWTToken123!!` |
| `ClickUp:ApiToken` | Token xác thực ClickUp (Cần quyền `write` mới tạo được task). | `pk_101536042_B00ZMXRQ1HPDYHL0K3QSAHHWK18CK0WA` |
| `ClickUp:ListId` | ID Danh sách ClickUp mục tiêu. | `901812388988` |

**Các bước Khởi chạy:**

1.  **Clone Repository** và **`dotnet restore`**.
2.  **Cập nhật Database:** `dotnet ef database update`.
3.  **Chạy ứng dụng:** `dotnet run` (Ứng dụng chạy trên cổng được cấu hình trong Environment Variable `PORT` hoặc mặc định là `8080`).

-----

## 3\. Hướng dẫn Tái sử dụng Module (Self-Service Integration)

### 3.1. Đồng bộ Task với ClickUp

Hệ thống tự động hóa quá trình gán tag và chuyển đổi dữ liệu.

  * **Tự động gán Tag:** Mọi task mới đều được tự động gán tag **`[AIHUB_BACKEND]`**.
  * **Chuyển đổi Deadline:** Tự động chuyển đổi `DateTime` sang Unix Timestamp (mili giây) theo yêu cầu của ClickUp.

| Endpoint | Method | Chức năng & Tái sử dụng |
| :--- | :--- | :--- |
| `/api/v1/tasks` | **POST** | Tạo task mới, lưu vào DB, **gọi ClickUp API** và **lưu lại ID ClickUp** cho các thao tác sau. |
| `/api/v1/tasks/{id}` | **PUT** | Cập nhật task nội bộ và đồng bộ thay đổi (Title, Status, Deadline) lên ClickUp. **Gửi báo cáo Telegram** nếu trạng thái chuyển sang `"Completed"`. |
| `/api/v1/tasks/{id}` | **DELETE** | Xóa task khỏi DB, sau đó gọi API xóa task khỏi ClickUp. |

### 3.2. Bảo mật (Dùng cho Frontend/Admin)

  * **Hashing Mật khẩu:** Sử dụng module `PasswordHelper` (BCrypt.Net) để băm mật khẩu.
  * **JWT:** Xác thực bằng token JWT (được kiểm tra bởi `JwtBlacklistMiddleware` và các tham số xác thực đã cấu hình).

-----

## 4\. Testing và Documentation

1.  **API Documentation:** Truy cập `/swagger` để xem và kiểm thử tất cả các endpoints.
2.  **Testing Core Logic:** Sử dụng dự án `Service.Tests` để chạy Unit Tests và xác nhận các module core hoạt động đúng.

-----

**END OF README.md**
