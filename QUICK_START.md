# Bắt Đầu Nhanh

## ⚡ 5 Phút để Chạy Dự Án

### Yêu Cầu
- ✅ Visual Studio 2022 đã cài
- ✅ SQL Server đã cài
- ✅ File CambridgeExamSystem.zip đã giải nén

### Bước 1: Mở Solution (30 giây)
```
1. Visual Studio → File → Open Project/Solution
2. Chọn: CambridgeExamSystem.sln
3. Click Open
```

### Bước 2: Chạy SQL Scripts (2 phút)
```
1. Mở SQL Server Management Studio
2. Connect to: localhost\SQLEXPRESS
3. File → Open → SQL Script
4. Chọn lần lượt:
   Database/Schema/01_CreateTables.sql → Execute
   Database/Schema/02_CreateIndexes.sql → Execute
   Database/Schema/03_CreateRelationships.sql → Execute
   Database/StoredProcedures/[tất cả] → Execute
   Database/Seeds/[tất cả] → Execute
```

### Bước 3: Cấu Hình Connection String (30 giây)
```
1. Visual Studio: Mở CambridgeExamSystem.Web/Web.config
2. Tìm dòng: <add name="CambridgeDbContext" ...
3. Thay: Server=YOUR_SERVER
   Bằng:  Server=localhost\SQLEXPRESS
4. Lưu file (Ctrl+S)
```

### Bước 4: Build & Run (2 phút)
```
1. Visual Studio: Ctrl+Shift+B (Build)
2. Đợi build hoàn tất
3. Nhấn F5 (Run)
4. Chọn IIS Express
5. Ứng dụng mở tại: http://localhost:xxxx
```

### Bước 5: Đăng Nhập (10 giây)
```
Username: student1
Password: Password@123
Click Login
```

## ✅ Xong!

Bạn đã có hệ thống Cambridge Exam đầy đủ chính! 🎉

## 🔍 Kiểm Tra Các Tính Năng

1. **Làm Bài Thi**
   - Home → Chọn Level → Chọn Exam → Start

2. **Xem Kết Quả**
   - Làm xong bài → Submit → View Results

3. **Bảng Xếp Hạng**
   - Menu → Leaderboard

4. **Thành Tích**
   - Menu → Achievements

5. **Dịch Văn Bản**
   - Quét chuột bất kỳ từ nào → Click dịch

6. **Từ Vựng**
   - User → Vocabulary Manager

## 🐛 Lỗi Phổ Biến

| Lỗi | Giải Pháp |
|-----|----------|
| Cannot connect to database | Kiểm tra SQL Server đang chạy, update Web.config |
| Invalid object name 'dbo.Users' | Chạy lại 01_CreateTables.sql |
| Build failed | Xóa bin/obj, rebuild |
| Localhost không mở | Khởi động lại IIS Express, thay port |

## 📚 Tài Liệu Chi Tiết

- [Hướng Dẫn Cài Đặt Đầy Đủ](INSTALLATION.md)
- [Thiết Lập Database](Database/DATABASE_SETUP.md)
- [Kiến Trúc Hệ Thống](Documentation/ARCHITECTURE.md)

## 💬 Cần Giúp?

- Email: thieuquangdai@example.com
- GitHub Issues: [Report Bug](https://github.com/thieuquangdai/CambridgeExamSystem/issues)

---

**Happy Coding! 🚀**
