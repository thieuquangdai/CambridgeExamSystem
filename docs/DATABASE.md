# Cơ sở dữ liệu

SQL Server, truy cập qua Entity Framework Core 8 (`CambridgeDbContext`, kế thừa `IdentityDbContext<User, Role, int>`).
Schema được quản lý bằng **EF Core migrations** trong `src/CambridgeExamSystem.Infrastructure/Migrations`.
Cấu hình từng bảng nằm trong `Data/Configurations` (Fluent API), không dùng data annotation trên entity.

## Bảng chính

| Nhóm | Bảng |
|------|------|
| Identity | `Users`, `Roles`, `UserRoles`, `UserClaims`, `UserLogins`, `UserTokens`, `RoleClaims` |
| Đề thi | `ExamLevels`, `ExamPapers`, `ExamSections`, `QuestionTypes`, `QuestionGroups`, `Questions`, `QuestionOptions`, `QuestionMedia`, `QuestionExplanations` |
| Làm bài | `ExamAttempts`, `AttemptAnswers`, `AttemptEvents`, `AttemptResults` |
| Gamification | `UserStatistics`, `StarTransactions`, `Achievements`, `UserAchievements`, `LeaderboardSnapshots`, `MotivationalMessages` |
| Từ vựng | `UserVocabulary` |

Quy ước:

- Thời gian lưu UTC trong cột `datetime2(0)` có hậu tố `AtUtc`, mặc định `SYSUTCDATETIME()`.
- Mã (`LevelCode`, `TypeCode`, `ExamCode`...) là `varchar`, nội dung hiển thị là `nvarchar` (hỗ trợ tiếng Việt).
- `ExamAttempts.Status` lưu tên enum (`InProgress`, `Paused`, `Submitted`, `Expired`, `Cancelled`) có CHECK constraint.
- Mỗi người dùng chỉ có **một** bài làm mở (`InProgress`/`Paused`) cho mỗi đề — `TestService` luôn tiếp tục bài làm mở thay vì tạo mới.
- Mỗi câu hỏi chỉ có một câu trả lời trong một lần làm (`UQ_AttemptAnswers_AttemptQuestion`).
- Câu trả lời phức tạp lưu JSON trong `AttemptAnswers.AnswerJson`:
  - nhiều đáp án: `{"selectedOptionIds":[1,3]}`
  - sắp xếp: `{"orderedOptionIds":[4,5,6,7]}`
  - nối: `{"matches":{"<optionId>":"<matchingKey>"}}`
- Đáp án điền/trả lời ngắn chấp nhận nhiều phương án, phân tách bằng `|` (`went to school|go to school`).
- Mật khẩu chỉ lưu dạng hash của ASP.NET Core Identity (`Users.PasswordHash`).

## Migrations

```bash
dotnet tool restore
# Thêm migration mới
dotnet ef migrations add <Ten> -p src/CambridgeExamSystem.Infrastructure -s src/CambridgeExamSystem.Infrastructure -o Migrations
# Áp dụng vào database (dùng biến CAMBRIDGE_DESIGN_CONNECTION, mặc định LocalDB)
CAMBRIDGE_DESIGN_CONNECTION="Server=...;Database=CambridgeExamDb;..." \
  dotnet ef database update -p src/CambridgeExamSystem.Infrastructure -s src/CambridgeExamSystem.Infrastructure
```

Ứng dụng web cũng tự gọi `Database.MigrateAsync()` khi `Seed:ApplyMigrations=true`.

## Script SQL (`database/`)

| File | Nội dung |
|------|----------|
| `01_CreateDatabase.sql` | Tạo `CambridgeExamDb` và toàn bộ schema (sinh từ migrations, idempotent) |
| `02_SeedData.sql` | Vai trò, cấp độ, dạng câu hỏi, thành tích, lời động viên (idempotent) |
| `StoredProcedures/usp_GetLeaderboard.sql` | Báo cáo bảng xếp hạng |
| `StoredProcedures/usp_GetUserExamHistory.sql` | Báo cáo lịch sử làm bài của một người dùng |

Stored procedure chỉ phục vụ báo cáo/DBA; ứng dụng truy vấn qua EF Core và repository, không có SQL trong controller.

Sau khi thêm migration, sinh lại `01_CreateDatabase.sql`:

```bash
dotnet ef migrations script --idempotent -p src/CambridgeExamSystem.Infrastructure -s src/CambridgeExamSystem.Infrastructure -o schema.sql
```

rồi thay phần schema trong file (giữ phần `CREATE DATABASE`/`USE` ở đầu).

Tài khoản admin **không** được tạo bằng SQL để mật khẩu luôn được hash bởi Identity; dùng `Seed:AdminEmail`/`Seed:AdminPassword`.
