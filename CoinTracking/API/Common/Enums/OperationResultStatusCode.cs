namespace API.Common.Enums
{
    public enum OperationResultStatusCode
    {
        Ok = 200,
        Created = 201,
        Accepted = 202,
        NoContent = 204,
        BadRequest = 400,
        Unauthorized = 401,
        Forbidden = 403,
        NotFound = 404,
        NotAcceptable = 406,
        Conflict = 409,
        Unavailable = 503,
        TooManyRequest = 429
    }
}