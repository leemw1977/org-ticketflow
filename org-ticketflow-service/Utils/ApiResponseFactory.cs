public static class ApiResponseFactory
{
    public static ApiResponse<T> Ok<T>(T data, string message = "Success", int httpStatus = 200)
    {
        return new ApiResponse<T>
        {
            Status = "ok",
            Message = message,
            Data = data,
            HttpResponse = httpStatus
        };
    }

    public static ApiResponse<T> Created<T>(T data, string message = "Resource created")
    {
        return Ok(data, message, 201);
    }

    public static ApiResponse<T> Error<T>(string message, int httpStatus = 500, T? data = default)
    {
        return new ApiResponse<T>
        {
            Status = "error",
            Message = message,
            Data = data,
            HttpResponse = httpStatus
        };
    }
}
