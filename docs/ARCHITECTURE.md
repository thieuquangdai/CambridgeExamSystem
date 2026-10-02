# Kiến trúc

## Các project

```text
CambridgeExamSystem.Web
        │
        ├── CambridgeExamSystem.Application
        │              │
        │              └── CambridgeExamSystem.Domain
        │
        └── CambridgeExamSystem.Infrastructure
                       │
                       ├── CambridgeExamSystem.Application
                       └── CambridgeExamSystem.Domain
```

| Project | Trách nhiệm | Phụ thuộc |
|---------|-------------|-----------|
| **Domain** | Entity (`ExamPaper`, `Question`, `TestAttempt`...), enum, hằng số (`RoleNames`, `QuestionTypeCodes`) | Không có project nào (chỉ `Microsoft.Extensions.Identity.Stores` cho `IdentityUser<int>`) |
| **Application** | Interface service/repository, DTO, nghiệp vụ (`TestService`, `GradingService`, `ExamService`, `AchievementService`, `LeaderboardService`, `VocabularyService`), FluentValidation, AutoMapper | Domain |
| **Infrastructure** | `CambridgeDbContext`, cấu hình EF, migrations, repository + unit of work, `TranslationService` (HttpClient + cache), `FileStorageService`, `PasswordService`, `DbInitializer` | Application, Domain |
| **Web** | Controller, Razor View, ViewModel, Identity cookie auth, JS/CSS | Application, Infrastructure (chỉ để đăng ký DI) |

`ArchitectureTests` kiểm tra tự động rằng Domain không tham chiếu project khác và Application chỉ tham chiếu Domain.

## Luồng làm bài

```text
ExamController.Start ──► TestService.StartOrResumeAsync  (tái sử dụng bài làm mở, tạo AttemptEvent)
ExamController.Take  ──► TestService.GetTakeTestAsync     (DTO không chứa đáp án; hết giờ → tự chấm)
exam.js (debounce 0.7s, heartbeat 30s, localStorage khi offline)
        └─► POST /Exam/SaveAnswer (JSON + header RequestVerificationToken)
                └─► TestService.SaveAnswerAsync  (kiểm tra quyền sở hữu, câu hỏi/đáp án thuộc đề,
                                                  giới hạn thời gian theo đồng hồ server)
POST /Exam/Pause (sendBeacon khi đóng tab) ──► TestService.PauseAsync
POST /Exam/Submit ──► TestService.SubmitAsync ──► GradingService ──► AttemptResult, UserStatistics,
                                                  StarTransaction, AchievementService
ResultController.Details ──► TestService.GetResultAsync (đáp án + giải thích chỉ sau khi nộp)
```

Thời gian làm bài được tính phía server: mỗi lần lưu client gửi số giây kể từ lần đồng bộ trước, server
giới hạn giá trị này theo thời gian thực đã trôi qua, nên không thể "kéo dài" giờ làm bài bằng cách sửa request.

## Bảo mật

- ASP.NET Core Identity: hash mật khẩu, lockout 5 lần sai/15 phút, email duy nhất, cookie HttpOnly.
- Phân quyền theo vai trò `Student`, `Teacher`, `Admin`; `AdminController` yêu cầu Admin hoặc Teacher,
  quản lý người dùng chỉ Admin.
- `AutoValidateAntiforgeryToken` toàn cục; AJAX gửi token qua header `RequestVerificationToken`.
- Mọi truy vấn bài làm đều lọc theo `UserId` của người đăng nhập.
- Upload chỉ chấp nhận phần mở rộng ảnh/audio cho phép, giới hạn 50 MB, tên file ngẫu nhiên (GUID).
- Bí mật (connection string, mật khẩu admin) đặt qua User Secrets/biến môi trường.

## Gamification

- Sao mỗi bài: ≥90% → 3, ≥70% → 2, còn lại → 1; cộng thêm sao thưởng từ thành tích.
- Hạng: Bronze (<30), Silver (30), Gold (100), Platinum (250), Diamond (500 sao).
- Bảng xếp hạng: tổng sao ↓, điểm trung bình ↓, số bài ↓, tên ↑; theo tuần/tháng tính từ `StarTransactions`,
  admin có thể chụp `LeaderboardSnapshots`.
