namespace Shared.ApiErrors
{
    public class ApiError
    {
        public int StatusCode { get; private set; }

        public string StatusDescription { get; private set; }

        public string Message { get; private set; }

        public ApiError(int statusCode, string statusDescription)
        {
            this.StatusCode = statusCode;
            this.StatusDescription = statusDescription;

            if (statusCode == 401)
                Message = "unauthorized";
        }

        public ApiError(int statusCode, string statusDescription, string message) : this(statusCode, statusDescription)
        {
            this.Message = message;
        }
    }
}