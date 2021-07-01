namespace Shared.ApiErrors
{
    public class ApiError
    {
        public int StatusCode { get; set; }

        public string StatusDescription { get; set; }

        public string Message { get; set; }

        public ApiError(int statusCode, string statusDescription)
        {
            this.StatusCode = statusCode;
            this.StatusDescription = statusDescription;

            Message = statusCode switch
            {
                401 => "unauthorized",
                403 => "no_permission",
                404 => "not_found",
                _ => Message
            };
        }

        public ApiError()
        {
        }

        public ApiError(int statusCode, string statusDescription, string message) : this(statusCode, statusDescription)
        {
            this.Message = message;
        }
    }
}