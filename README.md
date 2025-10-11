# AIHubTaskTracker

Hệ thống quản lý task cho AIHub: quản lý Members, Task, KPI, Report, và TaskLog.

---

## 1. Yêu cầu

- .NET 8 SDK
- PostgreSQL 15+
- Entity Framework Core 8

---

## 2. Clone project

```bash
git clone https://github.com/hoangthienisme/AIHubTaskTracker.git
cd AIHubTaskTracker

3. Chạy project

dotnet build
dotnet run
API mặc định chạy tại: http://localhost:8080

4. API ví dụ
Member

GET /members : Lấy tất cả member

POST /members : Tạo member mới

{
  "name": "Nguyễn Văn B",
  "email": "vanB@example.com"
}
Task
GET /tasks : Lấy tất cả task

POST /tasks :

{
  "taskTitle": "Tích hợp API Dashboard",
  "status": "In Progress",
  "deadline": "2025-10-15T12:00:00Z",
  "assignedMemberId": 1
}
Report
POST /reports
{
  "memberId": 1,
  "reportTitle": "Báo cáo tiến độ Task",
  "summary": "Trong tuần này, 80% task được hoàn thành đúng hạn.",
  "createdAt": "2025-10-11T11:53:11Z",
  "tasksCompleted": 10,
  "tasksPending": 2
}
KPI

GET /kpis : Lấy tất cả KPI

POST /kpis

{
  "memberId": 1,
  "tasksCompleted": 1,
  "efficiency":90,
  "month": "2025-10-11T12:39:01.354Z"
}
TaskLog
GET /tasklogs : Lấy lịch sử thay đổi task

5. Swagger UI
Mở: http://localhost:8080/swagger/index.html để test API nhanh.

6. Lưu ý
Sử dụng UTC cho tất cả DateTime (timestamp with time zone)

