# Cambridge English Exam Management System

## 🎯 Giới Thiệu Dự Án

Hệ thống quản lý bài thi Cambridge trực tuyến được xây dựng bằng **ASP.NET MVC 5** và **C#**

### 🌟 Tính Năng Chính

- 📝 Làm Bài Thi Trực Tuyến (Starts, Moves, Flyer, KET, PET)
- 🏆 Bảng Xếp Hạng với Sao & Cúp
- 🎖️ Hệ Thống Thành Tích (8+ loại Badge)
- 🔤 Dịch Văn Bản bằng quét chuột
- 💪 Hệ Thống Khích Lệ & Động Viên
- ⏱️ Lưu Tự Động (Phục hồi phiên)
- 📊 Phân Tích Chi Tiết Kết Quả

### 🛠️ Công Nghệ Sử Dụng

- **Frontend**: ASP.NET MVC 5, Razor, HTML5, CSS3, Bootstrap, jQuery
- **Backend**: C#, .NET Framework 4.7.2, Entity Framework 6
- **Database**: SQL Server 2019/2022
- **API**: Google Translate, Dictionary API

## 📁 Cấu Trúc Dự Án

```
CambridgeExamSystem/
├── CambridgeExamSystem.sln
├── CambridgeExamSystem.Web/          [ASP.NET MVC]
├── CambridgeExamSystem.Business/     [Business Logic]
├── CambridgeExamSystem.Data/         [Data Access]
├── CambridgeExamSystem.Common/       [Shared]
├── CambridgeExamSystem.Tests/        [Tests]
├── Database/                         [SQL Scripts]
└── Documentation/                    [Hướng dẫn]
```

## 🚀 Cài Đặt Nhanh

### Yêu Cầu Hệ Thống
- Visual Studio 2022 Community
- SQL Server 2019/2022 Express
- .NET Framework 4.7.2+

### Các Bước

#### 1. Giải Nén & Mở Solution
```
1. Giải nén file .zip
2. Mở CambridgeExamSystem.sln trong Visual Studio
```

#### 2. Thiết Lập Database
```
1. Mở SQL Server Management Studio
2. Chạy các file SQL theo thứ tự:
   - Database/Schema/01_CreateTables.sql
   - Database/Schema/02_CreateIndexes.sql
   - Database/Schema/03_CreateRelationships.sql
   - Database/StoredProcedures/[tất cả]
   - Database/Seeds/[dữ liệu mẫu]
```

#### 3. Cấu Hình Connection String
```
Mở: CambridgeExamSystem.Web/Web.config
Tìm và sửa:
<add name="CambridgeDbContext" 
     connectionString="Server=YOUR_SERVER;Database=CambridgeExamSystem;Trusted_Connection=true;" 
     providerName="System.Data.SqlClient" />

Thay YOUR_SERVER bằng tên server của bạn
Ví dụ: THIEUQUANGDAI\SQLEXPRESS hoặc localhost
```

#### 4. Cài NuGet Packages
```
Trong Visual Studio:
Tools → NuGet Package Manager → Package Manager Console
Chạy: Update-Package
```

#### 5. Chạy Ứng Dụng
```
Nhấn F5 hoặc Click Start Debugging
Ứng dụng mở tại: http://localhost:xxxx
```

## 👤 Tài Khoản Mẫu

```
Username: student1
Password: Password@123

hoặc

Username: admin
Password: Admin@123
```

## 📚 Tài Liệu

- [Hướng Dẫn Cài Đặt Chi Tiết](Documentation/INSTALLATION.md)
- [Bắt Đầu Nhanh](Documentation/QUICK_START.md)
- [Thiết Lập Database](Documentation/DATABASE_SETUP.md)

## 🐛 Khắc Phục Sự Cố

### Lỗi Connection String
- Kiểm tra tên server SQL (Tools → Connect to Database)
- Cập nhật trong Web.config
- Đảm bảo SQL Server đang chạy

### Lỗi Missing Tables
- Chạy lại file 01_CreateTables.sql
- Kiểm tra database name

### Lỗi 404 Pages
- Xóa bin/obj folders
- Rebuild solution (Ctrl+Shift+B)
- Xóa browser cache

## 📞 Liên Hệ

- GitHub: [thieuquangdai/CambridgeExamSystem](https://github.com/thieuquangdai/CambridgeExamSystem)
- Email: thieuquangdai@example.com

---

**Made with ❤️ for Cambridge English Learners**
