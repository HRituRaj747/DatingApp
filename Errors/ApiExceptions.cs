namespace API.Errors
{
    public class ApiExceptions
    {
        public ApiExceptions(int statusCode, string messages = null, string details = null)
        {
            StatusCode = statusCode;
            Messages = messages;
            Details = details;
        }

        public int StatusCode { get; set; }

        public string Messages { get; set; }

        public string Details { get; set; }
    }
}