# Cambridge English Exam Management System

Hệ thống luyện thi Cambridge English (Starters, Movers, Flyers, KET, PET) xây dựng bằng
**ASP.NET Core MVC (.NET 8 LTS) + C# + SQL Server + Entity Framework Core 8**.
Dự án không dùng ASP.NET MVC 5 hay `System.Web`, có thể chạy trên Windows/IIS, Linux hoặc container.

## Tính năng

- Làm bài thi trực tuyến: Listening, Reading, Writing, Speaking; ảnh, audio, video, tài liệu, transcript.
- 7 dạng câu hỏi: trắc nghiệm một/nhiều đáp án, đúng/sai, điền chỗ trống, trả lời ngắn, nối, sắp xếp.
- Tự động lưu (AJAX + localStorage khi mất mạng), tạm dừng và tiếp tục bài làm dở, đồng hồ đếm ngược phía server.
- Chấm điểm phía server; đáp án đúng không bao giờ được gửi xuống trình duyệt khi đang làm bài.
- Kết quả chi tiết theo phần và từng câu, giải thích, ghi chú ngữ pháp/từ vựng, lịch sử làm bài.
- Sao, hạng (Bronze → Diamond), thành tích, bảng xếp hạng tuần/tháng/mọi thời điểm.
- Dịch bằng cách bôi đen văn bản, lưu và đánh dấu từ vựng.
- Quản trị: đề thi, phần thi, câu hỏi, đáp án, upload ảnh/audio, xuất bản, người dùng và phân quyền.
- Đăng ký/đăng nhập bằng ASP.NET Core Identity (mật khẩu được hash, khoá tài khoản khi sai nhiều lần, anti-forgery).

## Cấu trúc

```text
CambridgeExamSystem/
├── CambridgeExamSystem.sln
├── Directory.Build.props / Directory.Packages.props   # net8.0 + phiên bản NuGet dùng chung
├── src/
│   ├── CambridgeExamSystem.Domain/          # Entity, enum, hằng số – không phụ thuộc project nào
│   ├── CambridgeExamSystem.Application/     # Interface, DTO, service, validator, AutoMapper – chỉ phụ thuộc Domain
│   ├── CambridgeExamSystem.Infrastructure/  # EF Core, SQL Server, Identity store, repository, dịch vụ ngoài, migrations
│   └── CambridgeExamSystem.Web/             # Controller, Razor View, authentication, wwwroot
├── tests/CambridgeExamSystem.Tests/         # xUnit: unit + integration (SQL Server)
├── database/                                # Script SQL (schema sinh từ migrations, seed, stored procedures)
└── docs/                                    # INSTALLATION, DATABASE, ARCHITECTURE
```

## Chạy nhanh

Yêu cầu: [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) và SQL Server 2019+ (LocalDB, Express, Developer hoặc Docker).

```bash
dotnet tool restore
cd src/CambridgeExamSystem.Web
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=CambridgeExamDb;User Id=sa;Password=<mật khẩu>;TrustServerCertificate=True"
dotnet user-secrets set "Seed:AdminPassword" "<mật khẩu admin>"
dotnet run
```

Khi khởi động, ứng dụng tự áp dụng migrations, tạo vai trò, dữ liệu tham chiếu, đề mẫu `KET-SAMPLE-01`
và tài khoản admin `admin@cambridge.local` (môi trường Development). Trên Windows có LocalDB thì không cần đặt
connection string. Chi tiết: [docs/INSTALLATION.md](docs/INSTALLATION.md) · [QUICK_START.md](QUICK_START.md).

## Kiểm thử

```bash
dotnet build
dotnet test                                    # integration test SQL Server sẽ được bỏ qua
CAMBRIDGE_TEST_SQLSERVER="Server=localhost;User Id=sa;Password=<mật khẩu>;TrustServerCertificate=True" dotnet test
```

Integration test tạo một database tạm riêng cho mỗi lần chạy và xoá sau khi xong.

## Tài liệu

- [Cài đặt & triển khai IIS](docs/INSTALLATION.md)
- [Cơ sở dữ liệu & migrations](docs/DATABASE.md)
- [Kiến trúc](docs/ARCHITECTURE.md)
