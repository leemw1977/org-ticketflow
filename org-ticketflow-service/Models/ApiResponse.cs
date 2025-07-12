public class ApiResponse<T>
{
    public string Status { get; set; } = "ok";
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public int HttpResponse { get; set; } = 200;
}
