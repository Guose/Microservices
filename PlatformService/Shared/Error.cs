namespace PlatformService.Shared
{
    public record Error(string Code, string Message)
    {
        public static Error None = new(string.Empty, string.Empty);
        public static Error NullValue = new("Error.NullValue", "Null value encountered");

        public static Error NotFound(string message) => new("NotFound", message);

        public static Error Invalid(string message) => new("Invalid", message);

        public static Error Unauthorized(string message) => new("Unauthorized", message);

        public static Error Forbidden(string message) => new("Forbidden", message);

        public static Error Conflict(string message) => new("Conflict", message);

        public static Error Internal(string message) => new("Internal", message);

        public static Error ServiceUnavailable(string message) => new("Service.Unavailable", message);
    }
}