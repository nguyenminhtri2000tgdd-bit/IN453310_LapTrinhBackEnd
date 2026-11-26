# Thiết kế REST API – EduHub mini

Quy ước: URI dùng danh từ số nhiều, không chứa động từ; phiên bản `/api/v1`; lỗi theo Problem Details (RFC 9457, `application/problem+json`).

## 1. Bảng thiết kế

| Chức năng | Method | URI | Mã thành công | Mã lỗi có thể gặp |
|---|---|---|---|---|
| Xem danh sách đăng ký của một khóa học | GET | `/api/v1/courses/{courseId}/registrations` | 200 OK | 404 (khóa học không tồn tại) |
| Tạo đăng ký | POST | `/api/v1/courses/{courseId}/registrations` | 201 Created | 400 (body sai/thiếu `memberId`), 404 (khóa học hoặc thành viên không tồn tại), 409 (trùng đăng ký), 422 (khóa học đã đủ chỗ) |
| Xác nhận đăng ký | PATCH | `/api/v1/registrations/{id}` – body `{"status":"confirmed"}` | 200 OK | 400 (status không hợp lệ), 404 (phiếu không tồn tại), 409 (phiếu đã hủy, không thể xác nhận) |
| Hủy đăng ký | PATCH | `/api/v1/registrations/{id}` – body `{"status":"cancelled"}` | 200 OK | 400, 404, 409 (phiếu đã hủy trước đó) |

**Giải thích lựa chọn:** xác nhận và hủy đều là thay đổi trạng thái của một phiếu nên dùng `PATCH` trên tài nguyên `registrations/{id}`, trạng thái nằm trong body; nhờ đó URI không chứa động từ (`/confirm`, `/cancel`). Phiếu bị hủy vẫn được lưu với `status = cancelled` nên không dùng `DELETE`.

## 2. JSON mẫu – Tạo đăng ký

### Request
```http
POST /api/v1/courses/2/registrations
Content-Type: application/json

{ "memberId": 3 }
```

### Response thành công
```http
HTTP/1.1 201 Created
Location: /api/v1/registrations/7
Content-Type: application/json

{ "id": 7, "memberId": 3, "courseId": 2, "status": "pending" }
```

### Response lỗi – trùng đăng ký (Problem Details)
```http
HTTP/1.1 409 Conflict
Content-Type: application/problem+json

{
  "type": "https://eduhub.example.com/problems/duplicate-registration",
  "title": "Duplicate registration",
  "status": 409,
  "detail": "Thành viên 3 đã đăng ký khóa học 2.",
  "instance": "/api/v1/courses/2/registrations"
}
```
