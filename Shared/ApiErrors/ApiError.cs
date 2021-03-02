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

            if (statusCode == 401)
                Message = "unauthorized";
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