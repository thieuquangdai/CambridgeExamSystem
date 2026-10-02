# Bắt đầu nhanh

1. Cài [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) và SQL Server (Windows: LocalDB đi kèm Visual Studio là đủ).
2. Mở `CambridgeExamSystem.sln` bằng Visual Studio 2022 (17.8+) hoặc VS Code / Rider.
3. Đặt mật khẩu admin (không commit vào Git):
   ```bash
   cd src/CambridgeExamSystem.Web
   dotnet user-secrets set "Seed:AdminPassword" "Admin@12345"
   ```
4. Nếu không dùng LocalDB, đặt connection string:
   ```bash
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost\SQLEXPRESS;Database=CambridgeExamDb;Trusted_Connection=True;TrustServerCertificate=True"
   ```
5. Chạy `dotnet run` (hoặc F5). Database, dữ liệu tham chiếu và đề mẫu được tạo tự động.
6. Đăng nhập `admin@cambridge.local` để quản trị, hoặc **Đăng ký** một tài khoản học sinh mới.

## Thử các tính năng

| Tính năng | Đường dẫn |
|-----------|-----------|
| Danh sách đề, làm bài, tự động lưu | `/Exam` → chọn đề → **Bắt đầu** |
| Tạm dừng và tiếp tục | Nút **Tạm dừng & thoát**, sau đó mở lại đề hoặc xem trang chủ |
| Kết quả & giải thích | Sau khi nộp bài, hoặc `/Result/History` |
| Bảng xếp hạng | `/Leaderboard` |
| Thành tích | `/Achievement` |
| Dịch & từ vựng | Bôi đen chữ tiếng Anh trong đề → **Dịch**, hoặc `/Translation` |
| Quản trị | `/Admin` (vai trò Admin/Teacher) |

## Lỗi thường gặp

| Lỗi | Cách xử lý |
|-----|-----------|
| `A network-related or instance-specific error` | Kiểm tra SQL Server đang chạy và connection string |
| Không có tài khoản admin | Chưa đặt `Seed:AdminPassword`; đặt rồi chạy lại ứng dụng |
| `Login failed for user` | Kiểm tra quyền `dbcreator`/`db_owner` của tài khoản SQL |
