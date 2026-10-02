# Cài đặt và triển khai

## Yêu cầu

| Thành phần | Phiên bản |
|------------|-----------|
| .NET SDK | 8.0.x (xem `Directory.Build.props`) |
| SQL Server | 2019, 2022 (Express/Developer/Standard), LocalDB hoặc Azure SQL |
| IDE (tuỳ chọn) | Visual Studio 2022 17.8+, Rider, VS Code + C# Dev Kit |

> `TargetFramework` được đặt duy nhất trong `Directory.Build.props`. Khi muốn nâng lên .NET 10 LTS chỉ cần đổi
> `net8.0` → `net10.0` và cập nhật phiên bản gói trong `Directory.Packages.props`.

## Cấu hình

Mọi cấu hình nằm trong `src/CambridgeExamSystem.Web/appsettings.json`. Giá trị bí mật **không** được commit;
đặt bằng User Secrets (máy dev) hoặc biến môi trường (server, dùng `__` thay cho `:`).

| Khoá | Ý nghĩa | Mặc định |
|------|---------|----------|
| `ConnectionStrings:DefaultConnection` | Chuỗi kết nối SQL Server | LocalDB `CambridgeExamDb` |
| `Seed:ApplyMigrations` | Tự áp dụng migrations khi khởi động | `true` |
| `Seed:SampleData` | Tạo đề mẫu `KET-SAMPLE-01` | `true` |
| `Seed:AdminEmail` / `Seed:AdminPassword` | Tài khoản admin được tạo lần đầu | Development: `admin@cambridge.local` / *(trống)* |
| `Translation:*` | Endpoint dịch (MyMemory) và từ điển (dictionaryapi.dev), timeout, cache | bật |
| `AutoMapper:LicenseKey` | License AutoMapper 15 (tuỳ chọn, chỉ ảnh hưởng log cảnh báo) | trống |

Ví dụ biến môi trường:

```bash
export ConnectionStrings__DefaultConnection="Server=db;Database=CambridgeExamDb;User Id=app;Password=...;TrustServerCertificate=True"
export Seed__AdminEmail="admin@truong.edu.vn"
export Seed__AdminPassword="..."
```

## Chạy trên máy dev

```bash
dotnet tool restore            # cài dotnet-ef theo .config/dotnet-tools.json
dotnet build
dotnet run --project src/CambridgeExamSystem.Web
```

### SQL Server bằng Docker (Linux/macOS)

```bash
docker run -d --name mssql -e ACCEPT_EULA=Y -e MSSQL_SA_PASSWORD='<mật khẩu mạnh>' -p 1433:1433 \
  mcr.microsoft.com/mssql/server:2022-latest
```

Rồi đặt `ConnectionStrings:DefaultConnection` thành
`Server=localhost,1433;Database=CambridgeExamDb;User Id=sa;Password=<mật khẩu>;TrustServerCertificate=True`.

## Triển khai Windows / IIS

1. Cài **ASP.NET Core 8 Hosting Bundle** trên server, khởi động lại IIS (`iisreset`).
2. Publish:
   ```powershell
   dotnet publish src/CambridgeExamSystem.Web -c Release -o C:\inetpub\CambridgeExam
   ```
3. Tạo Application Pool với **.NET CLR version = No Managed Code**, Pipeline = Integrated.
4. Tạo Website trỏ tới thư mục publish, gán Application Pool trên, cấu hình HTTPS binding.
5. Đặt biến môi trường cho app pool (IIS Manager → Configuration Editor → `system.webServer/aspNetCore`
   → `environmentVariables`) hoặc trong `web.config` sinh ra khi publish:
   `ASPNETCORE_ENVIRONMENT=Production`, `ConnectionStrings__DefaultConnection`, `Seed__AdminEmail`, `Seed__AdminPassword`.
6. Cấp quyền ghi cho identity của app pool (`IIS AppPool\<tên pool>`) vào thư mục `wwwroot\uploads`.
7. Nếu chạy nhiều instance, cấu hình Data Protection key ring dùng chung (thư mục mạng hoặc database).

Sau lần chạy đầu tiên có thể đặt `Seed__AdminPassword` thành rỗng; tài khoản đã tạo không bị ảnh hưởng.

## Cung cấp database thủ công (tuỳ chọn)

Nếu DBA không cho ứng dụng tự tạo schema, đặt `Seed:ApplyMigrations=false` và chạy các script trong `database/`
(xem [DATABASE.md](DATABASE.md)).
