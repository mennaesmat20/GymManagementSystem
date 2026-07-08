namespace GymManagementSystem.BLL.Common
{
    public record Result(bool Success, string? ErrorMessage = null, ResultStatus Status = ResultStatus.Ok)
    {
        public static Result Ok() => new Result(true);
        public static Result Fail(string ErrorMessage, ResultStatus status = ResultStatus.Conflict) => new Result(false, ErrorMessage, status);
        public static Result NotFound(string ErrorMessage = "Not Found") => new Result(false, ErrorMessage, ResultStatus.NotFound);
        public static Result ValidationFailed(string ErrorMessage) => new Result(false, ErrorMessage, ResultStatus.ValidationFailed);
    }
    public record Result<T>(bool Success, T? Value, string? ErrorMessage = null, ResultStatus Status = ResultStatus.Ok)
    {
        public static Result<T> Ok(T value) => new(true, value);
        public static Result<T> Fail(string ErrorMessage, ResultStatus status = ResultStatus.Conflict) => new(false, default, ErrorMessage, status);
        public static Result<T> NotFound(string ErrorMessage = "Not Found") => new(false, default, ErrorMessage, ResultStatus.NotFound);
    }

    public enum ResultStatus
    {
        Ok,
        NotFound,
        Conflict,
        ValidationFailed,
        Forbidden
    }
}
