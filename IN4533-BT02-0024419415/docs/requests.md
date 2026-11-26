# Kiểm thử API bằng curl

Chạy API: `cd src/EduHub.Api && dotnet run --urls http://localhost:5000`
Dữ liệu dựa trên `sql/02-seed.sql` (thành viên 4 chưa đăng ký khóa nào; thành viên 3 đã đăng ký khóa 2).

| # | Chức năng | Lệnh | Mã dự kiến |
|---|---|---|---|
| 1 | Xem danh sách đăng ký khóa 2 | `curl -i http://localhost:5000/api/v1/courses/2/registrations` | 200 |
| 2 | Tạo đăng ký (thành viên 4, khóa 3) | `curl -i -X POST http://localhost:5000/api/v1/courses/3/registrations -H "Content-Type: application/json" -d '{"memberId": 4}'` | 201 |
| 3 | Xác nhận phiếu số 1 | `curl -i -X PATCH http://localhost:5000/api/v1/registrations/5 -H "Content-Type: application/json" -d '{"status": "confirmed"}'` | 200 |
| 4 | Hủy phiếu số 1 | `curl -i -X PATCH http://localhost:5000/api/v1/registrations/5 -H "Content-Type: application/json" -d '{"status": "cancelled"}'` | 200 |
| 5 | (Lỗi) Tạo đăng ký trùng (thành viên 3, khóa 2) | `curl -i -X POST http://localhost:5000/api/v1/courses/2/registrations -H "Content-Type: application/json" -d '{"memberId": 3}'` | 409 |
| 6 | (Lỗi) Khóa học không tồn tại | `curl -i http://localhost:5000/api/v1/courses/999/registrations` | 404 |

Lưu ý: phiếu số 5 trong seed là `(member 3, course 1, pending)`, nên lệnh 3 → 200. Sau lệnh 4 phiếu chuyển `cancelled`; chạy lại lệnh 3 sẽ ra 409.
