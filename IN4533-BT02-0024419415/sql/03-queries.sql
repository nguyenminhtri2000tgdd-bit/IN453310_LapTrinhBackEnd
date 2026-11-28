-- (a) Họ tên thành viên và tên khóa học của các phiếu confirmed, sắp xếp theo họ tên.
SELECT m.full_name, c.title
FROM registrations r
JOIN members m ON m.id = r.member_id
JOIN courses c ON c.id = r.course_id
WHERE r.status = 'confirmed'
ORDER BY m.full_name;

-- (b) Số phiếu đăng ký của từng khóa học, kể cả khóa chưa có phiếu nào.
-- Dùng LEFT JOIN vì JOIN (INNER) sẽ loại khóa không có phiếu; LEFT JOIN giữ lại và COUNT(r.id) cho ra 0.
SELECT c.title, COUNT(r.id) AS so_dang_ky
FROM courses c
LEFT JOIN registrations r ON r.course_id = c.id
GROUP BY c.id, c.title
ORDER BY c.id;

-- (c) Các thành viên chưa đăng ký khóa nào.
SELECT m.id, m.full_name
FROM members m
LEFT JOIN registrations r ON r.member_id = m.id
WHERE r.id IS NULL;
