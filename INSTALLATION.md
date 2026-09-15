# Hướng Dẫn Cài Đặt Chi Tiết

## 📋 Yêu Cầu Hệ Thống

### Phần Cứng
- **RAM**: Tối thiểu 4GB (8GB khuyến nghị)
- **Đĩa cứng**: 5GB dung lượng trống
- **Processor**: Intel Core i5 hoặc tương đương

### Phần Mềm
- **Visual Studio 2022 Community** (Free)
  - Link: https://visualstudio.microsoft.com/vs/community/
  - Cài đặt workload: ASP.NET and web development
  
- **SQL Server 2019/2022 Express** (Free)
  - Link: https://www.microsoft.com/en-us/sql-server/sql-server-downloads
  - Hoặc: SQL Server 2022 Developer Edition (Free)
  
- **SQL Server Management Studio** (Free)
  - Link: https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms
  
- **.NET Framework 4.7.2+**
  - Tự động cài khi cài Visual Studio

## 🛠️ Các Bước Cài Đặt

### Bước 1: Cài Đặt Visual Studio 2022

```
1. Tải Visual Studio Community từ: https://visualstudio.microsoft.com/vs/community/
2. Chạy installer
3. Chọn "ASP.NET and web development" workload
4. Chọn additional components:
   ✓ .NET Framework 4.7.2 SDK
   ✓ .NET Framework 4.7.2 targeting pack
5. Click "Install"
6. Đợi cài đặt xong (khoảng 30-45 phút)
7. Khởi động lại máy tính
```

### Bước 2: Cài Đặt SQL Server

```
1. Tải SQL Server 2022 Express từ: https://www.microsoft.com/en-us/sql-server/sql-server-downloads
2. Chạy installer
3. Chọn "Express" edition
4. Chấp nhận license terms
5. Chọn "Install SQL Server Express"
6. Chọn tùy chọn:
   ✓ Database Engine Services
   ✓ SQL Server Replication
7. Instance Name: SQLEXPRESS (mặc định)
8. Authentication mode: Windows Authentication hoặc Mixed
9. Click "Install"
10. Khởi động lại SQL Server
```

### Bước 3: Cài Đặt SQL Server Management Studio

```
1. Tải SSMS từ: https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms
2. Chạy installer
3. Chọn installation location
4. Click "Install"
5. Đợi cài đặt xong
```

### Bước 4: Giải Nén & Mở Dự Án

```
1. Giải nén file CambridgeExamSystem.zip
2. Mở Visual Studio 2022
3. File → Open → Project/Solution
4. Chọn file: CambridgeExamSystem.sln
5. Click "Open"
```

### Bước 5: Thiết Lập Database

#### 5.1 Mở SQL Server Management Studio
```
1. Mở SSMS
2. Server name: localhost\SQLEXPRESS (hoặc tên server của bạn)
3. Authentication: Windows Authentication
4. Click "Connect"
```

#### 5.2 Tạo Database Mới
```
1. Right-click "Databases" → New Database
2. Database name: CambridgeExamSystem
3. Click "OK"
```

#### 5.3 Chạy SQL Scripts
```
1. File → Open → SQL Script
2. Chọn file: Database/Schema/01_CreateTables.sql
3. Click "Open"
4. Click "Execute" (hoặc Ctrl+Shift+E)
5. Chờ completion
6. Lặp lại cho các file khác:
   - 02_CreateIndexes.sql
   - 03_CreateRelationships.sql
   - Tất cả file trong StoredProcedures/
   - Tất cả file trong Seeds/
```

**Thứ tự chạy SQL Scripts:**
```
1. Database/Schema/01_CreateTables.sql
2. Database/Schema/02_CreateIndexes.sql
3. Database/Schema/03_CreateRelationships.sql
4. Database/StoredProcedures/Exams/*.sql
5. Database/StoredProcedures/Tests/*.sql
6. Database/StoredProcedures/Leaderboard/*.sql
7. Database/StoredProcedures/Achievement/*.sql
8. Database/StoredProcedures/Users/*.sql
9. Database/StoredProcedures/Reports/*.sql
10. Database/Seeds/SeedLevels.sql
11. Database/Seeds/SeedQuestionTypes.sql
12. Database/Seeds/SeedAchievements.sql
13. Database/Seeds/SeedMotivationalMessages.sql
14. Database/Seeds/SampleData.sql
```

### Bước 6: Cấu Hình Connection String

```
1. Trong Visual Studio, mở file:
   CambridgeExamSystem.Web/Web.config

2. Tìm dòng:
   <connectionStrings>
       <add name="CambridgeDbContext" 
            connectionString="Server=YOUR_SERVER;Database=CambridgeExamSystem;Trusted_Connection=true;" 
            providerName="System.Data.SqlClient" />
   </connectionStrings>

3. Thay YOUR_SERVER bằng tên server của bạn:
   
   Cách tìm tên server:
   - Mở SQL Server Management Studio
   - Nhìn vào "Server name" field
   - Ví dụ: THIEUQUANGDAI\SQLEXPRESS
            localhost\SQLEXPRESS
            .\ SQLEXPRESS
   
   Cập nhật:
   <add name="CambridgeDbContext" 
        connectionString="Server=THIEUQUANGDAI\SQLEXPRESS;Database=CambridgeExamSystem;Trusted_Connection=true;" 
        providerName="System.Data.SqlClient" />

4. Lưu file (Ctrl+S)
```

### Bước 7: Cài NuGet Packages

```
1. Trong Visual Studio, mở Package Manager Console
   Tools → NuGet Package Manager → Package Manager Console

2. Đảm bảo "Default project" là: CambridgeExamSystem.Web

3. Chạy các lệnh sau:
   PM> Update-Package
   PM> Install-Package EntityFramework
   PM> Install-Package AutoMapper
   PM> Install-Package Newtonsoft.Json
   PM> Install-Package FluentValidation
   PM> Install-Package NLog

4. Chờ tất cả packages được cài đặt
```

### Bước 8: Build Solution

```
1. Trong Visual Studio
2. Build → Clean Solution
3. Build → Build Solution (hoặc Ctrl+Shift+B)
4. Chờ build hoàn tất
5. Kiểm tra Output window để đảm bảo không có lỗi
```

### Bước 9: Chạy Ứng Dụng

```
1. Nhấn F5 hoặc Click "Start Debugging"
2. Chọn "IIS Express" (khuyến nghị)
3. Ứng dụng sẽ mở trong trình duyệt
4. URL sẽ là: http://localhost:xxxx
```

## 👤 Đăng Nhập

Sau khi chạy Seeds, bạn có thể đăng nhập bằng:

```
Tài Khoản Học Viên:
- Username: student1
- Password: Password@123
- Level: Flyer

Tài Khoản Admin:
- Username: admin
- Password: Admin@123
```

## ✅ Kiểm Tra Cài Đặt

Sau khi hoàn tất các bước trên, hãy kiểm tra:

- [ ] Visual Studio mở được dự án
- [ ] Solution build thành công (không có lỗi)
- [ ] Có thể kết nối đến SQL Server trong SSMS
- [ ] Database "CambridgeExamSystem" tồn tại
- [ ] Tất cả bảng được tạo (Users, ExamPapers, Questions, etc.)
- [ ] Ứng dụng chạy được trên F5
- [ ] Có thể đăng nhập bằng tài khoản mẫu
- [ ] Trang chủ load được
- [ ] Danh sách đề thi hiển thị

## 🐛 Khắc Phục Sự Cố

### Vấn đề 1: "Cannot open connection to server"

**Giải pháp:**
```
1. Kiểm tra SQL Server đang chạy:
   - Windows Start → SQL Server Configuration Manager
   - Đảm bảo SQL Server (SQLEXPRESS) có status: Running

2. Kiểm tra tên server:
   - Mở SSMS
   - Ghi lại tên server từ "Server name" field
   - Cập nhật trong Web.config

3. Kiểm tra Database tồn tại:
   - Mở SSMS
   - Expand "Databases"
   - Tìm "CambridgeExamSystem"
   - Nếu không có, tạo mới bằng cách:
     Right-click Databases → New Database
```

### Vấn đề 2: "Invalid object name 'dbo.Users'"

**Giải pháp:**
```
1. Chạy lại SQL Scripts:
   - Mở SSMS
   - Connect to: CambridgeExamSystem database
   - File → Open → SQL Script
   - Chọn: Database/Schema/01_CreateTables.sql
   - Click Execute

2. Kiểm tra tất cả bảng được tạo:
   - Expand CambridgeExamSystem → Tables
   - Phải có: Users, ExamPapers, Sections, Questions, Answers, etc.
```

### Vấn đề 3: Build Error: "The type or namespace name does not exist"

**Giải pháp:**
```
1. Xóa NuGet Packages:
   Tools → NuGet Package Manager → Package Manager Console
   PM> Update-Package -Reinstall

2. Xóa bin/obj folders:
   - Đóng Visual Studio
   - Vào C:\...\CambridgeExamSystem
   - Xóa bin/ folder
   - Xóa obj/ folder
   - Mở lại Visual Studio
   - Build → Clean Solution
   - Build → Build Solution
```

### Vấn đề 4: Localhost không mở được

**Giải pháp:**
```
1. Kiểm tra IIS Express:
   - Visual Studio → Tools → Options → Projects and Solutions → Web Projects
   - Đảm bảo "Use the 64-bit version of IIS Express" được checked/unchecked đúng

2. Khởi động lại IIS Express:
   - Taskbar → Tìm IIS Express tray icon
   - Right-click → Exit
   - Nhấn F5 lại

3. Thay port:
   - Right-click project → Properties → Web
   - Thay Project URL port
```

## 📞 Cần Giúp Đỡ?

Nếu gặp vấn đề, hãy:

1. Kiểm tra lại các bước trên
2. Xem [QUICK_START.md](QUICK_START.md)
3. Xem [Troubleshooting Guide](TROUBLESHOOTING.md)
4. Liên hệ: thieuquangdai@example.com

---

**Chúc bạn cài đặt thành công! 🚀**
