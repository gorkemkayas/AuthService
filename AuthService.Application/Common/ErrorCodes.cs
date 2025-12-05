namespace AuthService.Application.Common
{
    public static class ErrorCodes
    {
        // Genel hatalar
        public const string Unexpected = "UNEXPECTED_ERROR";          // 500
        public const string ValidationError = "VALIDATION_ERROR";     // 400

        // Kimlik ve yetki
        public const string Unauthorized = "UNAUTHORIZED";            // 401
        public const string Forbidden = "FORBIDDEN";                  // 403

        // Kaynak hataları
        public const string NotFound = "NOT_FOUND";                   // 404
        public const string Conflict = "CONFLICT";                    // 409
        public const string AlreadyExists = "ALREADY_EXISTS";         // 409

        // İstek formatı / kabul edilebilirlik
        public const string NotAcceptable = "NOT_ACCEPTABLE";         // 406
        public const string UnsupportedMediaType = "UNSUPPORTED_MEDIA_TYPE"; // 415

        // Özel durumlar
        public const string TooManyRequests = "TOO_MANY_REQUESTS";    // 429
        public const string TokenExpired = "TOKEN_EXPIRED";           // 401
    }

}
