# 🚀 AIHub OS | Task Tracker API (Internal Use)
*Developer:* Nguyễn Hoàng Thiện  
*Team:* Backend Internal – AIHub OS  
*Ngày bắt đầu:* 10/10/2025  
*Deadline:* 13/10/2025  
*Phí task:* 300.000đ  

---

## 🎯 Mục tiêu
Xây dựng RESTful API quản lý công việc nội bộ (AIHub Task Tracker),  
được frontend (Tuấn & Phú) sử dụng để hiển thị danh sách task trong dashboard.

---

## 🧩 Cấu trúc thư mục
AIHubTaskTracker/
├── Controllers/
│   └── TasksController.cs
├── Models/
│   └── TaskItem.cs
├── Data/
│   └── AppDbContext.cs
├── Program.cs
├── appsettings.json
├── README.md
└── AIHubTaskTracker.sln
---

## 🗃️ Bảng dữ liệu
| Cột | Kiểu dữ liệu | Ghi chú |
|------|----------------|----------|
| Id | int (auto) | Khóa chính |
| MemberName | string | Tên thành viên |
| TaskTitle | string | Tiêu đề công việc |
| Status | string | Pending / In Progress / Done |
| Deadline | datetime | Hạn hoàn thành |
| CreatedAt | datetime | Tự động ghi thời gian tạo |

---

## 🌐 Endpoint REST API
| Method | Endpoint | Mô tả | Ví dụ Request |
|---------|-----------|--------|----------------|
| GET | /tasks | Lấy danh sách task | — |
| GET | /tasks/{id} | Lấy chi tiết 1 task | /tasks/1 |
| POST | /tasks | Thêm task mới | JSON body |
| PUT | /tasks/{id} | Cập nhật trạng thái | /tasks/1 |
| DELETE | /tasks/{id} | Xóa task | /tasks/1 |

---

## 🧠 Ví dụ JSON
```json
[
  {
    "Id": 1,
    "MemberName": "Nguyễn Thành Tuấn",
    "TaskTitle": "Tích hợp API Dashboard",
    "Status": "In Progress",
    "Deadline": "2025-10-15",
    "CreatedAt": "2025-10-10"
  }
]



⚙️ Công nghệ sử dụng
 • Framework: ASP.NET Core 6 / 8


 • Database: SQL Server / JSON File


 • Tools: Visual Studio / Postman / GitHub


 • CORS: Cho phép frontend kết nối


 • API Test: Postman, Swagger UI





📈 Hướng mở rộng
 • Thêm đăng nhập nội bộ (JWT token)


 • Phân quyền Leader / Member


 • Dashboard thống kê số task hoàn thành





✅ Kết quả bàn giao
 • Link demo (Host / Local)


 • Link GitHub / Google Drive


 • Video demo API (Postman hoặc trình duyệt)


File .zip dự phòng (nếu có)