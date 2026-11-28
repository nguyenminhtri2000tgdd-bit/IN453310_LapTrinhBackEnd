# IN4533-BT02-MSSV: EduHub mini

- Họ tên: Nguyễn Minh Trí
- MSSV: 0024419415
- Học phần: IN4533 - Lập trình Back End

## 1. Cài đặt PostgreSQL

Cách cài: cài trực tiếp trên máy (không dùng Docker).

- Hệ điều hành: Windows 11
- Phiên bản: PostgreSQL 18.6
- Tải bộ cài tại: https://www.postgresql.org/download/ (bản của EDB cho Windows)
- Các bước chính khi cài: chọn thành phần PostgreSQL Server, pgAdmin 4, Command Line Tools; đặt mật khẩu cho người dùng; giữ cổng mặc định `5432`.

Tạo cơ sở dữ liệu và kết nối bằng `psql`:

```bash
psql -U postgres -c "CREATE DATABASE eduhub;"
psql -U postgres -d eduhub
```

Ảnh kết nối thành công: `docs/images/01-psql.png`.

## 2. Chạy cơ sở dữ liệu

```bash
psql -U postgres -d eduhub -f sql/01-schema.sql
psql -U postgres -d eduhub -f sql/02-seed.sql
psql -U postgres -d eduhub -f sql/03-queries.sql
```

## 3. Chạy API

```bash
cd src/EduHub.Api
cp appsettings.Example.json appsettings.Development.json
dotnet run --urls http://localhost:5000
```

Các lệnh curl và mã dự kiến: `docs/requests.md`. Thiết kế API: `docs/api.md`.
Đã cài đặt: GET danh sách đăng ký của khóa học, POST tạo đăng ký, PATCH xác nhận/hủy đăng ký.

## 4. Ảnh minh chứng (`docs/images/`)

- `01-psql.png`: kết nối psql thành công
- `02-queries.png`: kết quả 3 truy vấn
- `03-api-get.png`, `04-api-post.png`, `05-api-409.png`: kết quả gọi thử API
