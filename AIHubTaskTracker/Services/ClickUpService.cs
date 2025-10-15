using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using AIHubTaskTracker.Models;

public class ClickUpService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiToken; // Vẫn giữ biến này cho mục đích debug/tái sử dụng token nếu cần
    private readonly string _listId;

    // Constructor: Nhận HttpClient đã được cấu hình sẵn token
    public ClickUpService(HttpClient httpClient, string apiToken, string listId)
    {
        _httpClient = httpClient;
        _apiToken = apiToken ?? throw new ArgumentNullException(nameof(apiToken));
        _listId = listId ?? throw new ArgumentNullException(nameof(listId));

        // QUAN TRỌNG: Nếu bạn không sử dụng AddHttpClient<T> như trên,
        // thì bạn phải thêm header ở đây:
        // if (!_httpClient.DefaultRequestHeaders.Contains("Authorization"))
        // {
        //     _httpClient.DefaultRequestHeaders.Add("Authorization", _apiToken);
        // }
    }

    // ClickUpService.cs - SỬA LẠI HÀM CreateTaskAsync

    // ClickUpService.cs - SỬA LẠI HÀM CreateTaskAsync

    public async Task<string?> CreateTaskAsync(TaskItem task) // <--- THAY ĐỔI: TRẢ VỀ string (ID ClickUp)
    {
        if (task == null) throw new ArgumentNullException(nameof(task));

        // 1. Chuyển đổi Deadline
        long? unixDeadlineMs = task.deadline.HasValue
            ? new DateTimeOffset(task.deadline.Value.ToUniversalTime()).ToUnixTimeMilliseconds()
            : (long?)null;

        // 2. Xây dựng Payload JSON (giữ nguyên)
        var body = new
        {
            name = task.title,
            description = task.description,
            assignees = Array.Empty<int>(),
            status = task.status,
            due_date = unixDeadlineMs
        };

        var jsonPayload = JsonSerializer.Serialize(body);
        var url = $"https://api.clickup.com/api/v2/list/{_listId}/task";

        using (var request = new HttpRequestMessage(HttpMethod.Post, url))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue(_apiToken);
            request.Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.SendAsync(request);
                var respText = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"ClickUp API error ({response.StatusCode}): {respText}");
                    throw new HttpRequestException($"ClickUp creation failed. Status: {response.StatusCode}. Response: {respText}");
                }
                else
                {
                    // ************************************************
                    // CRITICAL: TRÍCH XUẤT ID TỪ PHẢN HỒI JSON
                    // ************************************************
                    using (JsonDocument doc = JsonDocument.Parse(respText))
                    {
                        if (doc.RootElement.TryGetProperty("id", out var idElement))
                        {
                            return idElement.GetString(); // ClickUp ID là chuỗi
                        }
                    }

                    Console.WriteLine($"ClickUp task created: {respText}");
                    return null; // Trả về null nếu không tìm thấy ID (rất hiếm)
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ClickUp API exception: {ex.Message}");
                throw;
            }
        }
    }
    // Trong ClickUpService.cs

    // ClickUpService.cs - Cải tiến

    public async Task UpdateTaskAsync(TaskItem task)
    {
        if (task == null) throw new ArgumentNullException(nameof(task));

        // ***************************************************************
        // 1. Cải tiến: Sử dụng task.id (ID nội bộ) cho logging
        // ***************************************************************
        if (string.IsNullOrEmpty(task.clickup_id))
        {
            // Thay task.task_id (có thể là lỗi cú pháp) bằng task.id 
            Console.WriteLine($"ClickUp Update Failed: Task {task.task_id} is missing a ClickUp ID.");
            return;
        }

        // 2. Chuyển đổi Deadline (mili giây)
        long? unixDeadlineMs = task.deadline.HasValue
            ? new DateTimeOffset(task.deadline.Value.ToUniversalTime()).ToUnixTimeMilliseconds()
            : (long?)null;

        // ***************************************************************
        // 3. CRITICAL FIX: Xây dựng Payload chỉ với các giá trị non-null
        // ***************************************************************
        var updatePayload = new Dictionary<string, object>();

        // Gửi Name nếu không phải null/empty
        if (!string.IsNullOrEmpty(task.title)) { updatePayload["name"] = task.title; }

        // Gửi Description nếu không phải null/empty
        if (!string.IsNullOrEmpty(task.description)) { updatePayload["description"] = task.description; }

        // Gửi Status
        if (!string.IsNullOrEmpty(task.status)) { updatePayload["status"] = task.status; }

        // Gửi Due Date (ngay cả khi là null/long? nếu muốn xóa deadline)
        updatePayload["due_date"] = unixDeadlineMs;

        // Kiểm tra và thêm các trường khác nếu bạn có (ví dụ: progress_percentage, expected_output)
        // if (task.progress_percentage.HasValue) { updatePayload["progress_percentage"] = task.progress_percentage.Value; }


        var jsonPayload = JsonSerializer.Serialize(updatePayload);
        var url = $"https://api.clickup.com/api/v2/task/{task.clickup_id}";

        using (var request = new HttpRequestMessage(HttpMethod.Put, url))
        {
            request.Headers.Add("Authorization", _apiToken);
            request.Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.SendAsync(request);
                var respText = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    // CRITICAL: Log response để xem ClickUp báo lỗi gì về dữ liệu
                    Console.WriteLine($"ClickUp API Update Error ({response.StatusCode}): {respText}");
                    throw new HttpRequestException($"ClickUp update failed. Status: {response.StatusCode}. Response: {respText}");
                }
                // Nếu thành công (thường là 200 OK), không cần làm gì thêm
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ClickUp API Update Exception: {ex.Message}");
                throw;
            }
        }
    }

    public async Task DeleteTaskAsync(string clickupId)
    {
        if (string.IsNullOrEmpty(clickupId))
        {
            Console.WriteLine("ClickUp Delete Failed: Missing ClickUp ID.");
            return;
        }

        var url = $"https://api.clickup.com/api/v2/task/{clickupId}"; // SỬ DỤNG CLICKUP ID

        using (var request = new HttpRequestMessage(HttpMethod.Delete, url)) // SỬ DỤNG PHƯƠNG THỨC DELETE
        {
            request.Headers.Add("Authorization", _apiToken);

            try
            {
                var response = await _httpClient.SendAsync(request);
                var respText = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    // Thường ClickUp trả về 204 No Content khi xóa thành công.
                    // Bất kỳ mã lỗi nào khác đều là thất bại.
                    Console.WriteLine($"ClickUp API Delete Error ({response.StatusCode}): {respText}");
                    throw new HttpRequestException($"ClickUp deletion failed. Status: {response.StatusCode}. Response: {respText}");
                }
                // Không cần xử lý phản hồi thành công (thường là 204 No Content)
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ClickUp API Delete Exception: {ex.Message}");
                throw;
            }
        }
    }

    public async Task AddTagToTaskAsync(string clickupId, string tagName)
    {

        var encodedTagName = Uri.EscapeDataString(tagName);
        var url = $"https://api.clickup.com/api/v2/task/{clickupId}/tag/{encodedTagName}";

        using (var request = new HttpRequestMessage(HttpMethod.Post, url))
        {
            request.Headers.Add("Authorization", _apiToken);
            request.Content = new StringContent("", Encoding.UTF8, "application/json"); 
            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                var respText = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"ClickUp Tagging Error ({response.StatusCode}) for tag '{tagName}': {respText}");
            }
        }
    }
}