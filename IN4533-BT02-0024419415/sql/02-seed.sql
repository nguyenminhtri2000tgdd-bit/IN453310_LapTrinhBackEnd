-- sql/02-seed.sql: 4 thành viên, 3 khóa học, 6 phiếu đăng ký
INSERT INTO members (full_name, email) VALUES
 ('Nguyễn Văn An',  'an@example.com'),
 ('Trần Thị Bình',  'binh@example.com'),
 ('Lê Hoàng Cường', 'cuong@example.com'),
 ('Phạm Minh Dũng', 'dung@example.com');   -- Dũng chưa đăng ký khóa nào

INSERT INTO courses (title, capacity) VALUES
 ('Lập trình Back End', 40),
 ('Cơ sở dữ liệu',      40),
 ('An toàn thông tin',  30);               -- chưa có phiếu nào

INSERT INTO registrations (member_id, course_id, status) VALUES
 (1, 1, 'confirmed'),
 (1, 2, 'pending'),
 (2, 1, 'confirmed'),
 (2, 2, 'cancelled'),
 (3, 1, 'pending'),
 (3, 2, 'confirmed');
