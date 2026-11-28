using Npgsql;

var builder = WebApplication.CreateBuilder(args);
var cs = builder.Configuration.GetConnectionString("Default")
         ?? throw new InvalidOperationException("Thiếu ConnectionStrings:Default");
var app = builder.Build();

// GET /api/v1/courses/{courseId}/registrations: danh sách đăng ký của một khóa học
app.MapGet("/api/v1/courses/{courseId:int}/registrations", async (int courseId) =>
{
    await using var conn = new NpgsqlConnection(cs);
    await conn.OpenAsync();

    await using (var check = new NpgsqlCommand("SELECT 1 FROM courses WHERE id = @id", conn))
    {
        check.Parameters.AddWithValue("id", courseId);
        if (await check.ExecuteScalarAsync() is null)
            return Results.Problem(statusCode: 404, title: "Course not found",
                detail: $"Không có khóa học {courseId}.");
    }

    var list = new List<object>();
    await using var cmd = new NpgsqlCommand(
        "SELECT id, member_id, course_id, status FROM registrations WHERE course_id = @courseId ORDER BY id", conn);
    cmd.Parameters.AddWithValue("courseId", courseId);
    await using var rd = await cmd.ExecuteReaderAsync();
    while (await rd.ReadAsync())
        list.Add(new { id = rd.GetInt32(0), memberId = rd.GetInt32(1),
                       courseId = rd.GetInt32(2), status = rd.GetString(3) });
    return Results.Ok(list);
});

// POST /api/v1/courses/{courseId}/registrations: tạo đăng ký
app.MapPost("/api/v1/courses/{courseId:int}/registrations",
    async (int courseId, CreateRegistration? body) =>
{
    if (body is null || body.MemberId <= 0)
        return Results.Problem(statusCode: 400, title: "Invalid request",
            detail: "Body phải có memberId là số nguyên dương.");

    await using var conn = new NpgsqlConnection(cs);
    await conn.OpenAsync();
    try
    {
        await using var cmd = new NpgsqlCommand(
            "INSERT INTO registrations (member_id, course_id) VALUES (@m, @c) RETURNING id, status", conn);
        cmd.Parameters.AddWithValue("m", body.MemberId);
        cmd.Parameters.AddWithValue("c", courseId);
        await using var rd = await cmd.ExecuteReaderAsync();
        await rd.ReadAsync();
        var id = rd.GetInt32(0);
        return Results.Created($"/api/v1/registrations/{id}",
            new { id, memberId = body.MemberId, courseId, status = rd.GetString(1) });
    }
    catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.UniqueViolation)
    {
        return Results.Problem(statusCode: 409, title: "Duplicate registration",
            type: "https://eduhub.example.com/problems/duplicate-registration",
            detail: $"Thành viên {body.MemberId} đã đăng ký khóa học {courseId}.");
    }
    catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.ForeignKeyViolation)
    {
        return Results.Problem(statusCode: 404, title: "Member or course not found",
            detail: "Thành viên hoặc khóa học không tồn tại.");
    }
});

// PATCH /api/v1/registrations/{id}: xác nhận hoặc hủy đăng ký
app.MapPatch("/api/v1/registrations/{id:int}", async (int id, UpdateStatus? body) =>
{
    if (body is null || body.Status is not ("confirmed" or "cancelled"))
        return Results.Problem(statusCode: 400, title: "Invalid status",
            detail: "status chỉ nhận 'confirmed' hoặc 'cancelled'.");

    await using var conn = new NpgsqlConnection(cs);
    await conn.OpenAsync();

    string? current;
    int memberId, courseId;
    await using (var get = new NpgsqlCommand(
        "SELECT status, member_id, course_id FROM registrations WHERE id = @id", conn))
    {
        get.Parameters.AddWithValue("id", id);
        await using var rd = await get.ExecuteReaderAsync();
        if (!await rd.ReadAsync())
            return Results.Problem(statusCode: 404, title: "Registration not found",
                detail: $"Không có phiếu đăng ký {id}.");
        current = rd.GetString(0); memberId = rd.GetInt32(1); courseId = rd.GetInt32(2);
    }

    if (current == "cancelled")
        return Results.Problem(statusCode: 409, title: "Registration already cancelled",
            detail: "Phiếu đã hủy, không thể thay đổi trạng thái.");

    await using var upd = new NpgsqlCommand("UPDATE registrations SET status = @s WHERE id = @id", conn);
    upd.Parameters.AddWithValue("s", body.Status);
    upd.Parameters.AddWithValue("id", id);
    await upd.ExecuteNonQueryAsync();
    return Results.Ok(new { id, memberId, courseId, status = body.Status });
});

app.Run();

record CreateRegistration(int MemberId);
record UpdateStatus(string Status);
