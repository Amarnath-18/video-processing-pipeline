namespace Common
{
    public static class AppStatusCode
    {
        // ✅ Success
        public const int SUCCESS = 0;

        // 🔹 1000 - General Errors
        public const int UNKNOWN_ERROR = 1000;
        public const int BAD_REQUEST = 1001;
        public const int RESOURCE_NOT_FOUND = 1002;
        public const int OPERATION_FAILED = 1003;
        public const int CONFLICT = 1004;

        // 🔹 2000 - Validation Errors
        public const int VALIDATION_ERROR = 2000;
        public const int REQUIRED_FIELD_MISSING = 2001;
        public const int INVALID_FORMAT = 2002;
        public const int VALUE_OUT_OF_RANGE = 2003;
        public const int INVALID_INPUT = 2004;

        // 🔹 3000 - Authentication
        public const int AUTH_FAILED = 3000;
        public const int INVALID_CREDENTIALS = 3001;
        public const int TOKEN_MISSING = 3002;
        public const int TOKEN_INVALID = 3003;
        public const int TOKEN_EXPIRED = 3004;
        public const int LOGIN_REQUIRED = 3005;

        // 🔹 4000 - Authorization
        public const int ACCESS_DENIED = 4000;
        public const int INSUFFICIENT_PERMISSIONS = 4001;
        public const int FORBIDDEN = 4003;

        // 🔹 5000 - Database / Data Layer
        public const int DATABASE_ERROR = 5000;
        public const int RECORD_NOT_FOUND = 5001;
        public const int DUPLICATE_RECORD = 5002;
        public const int DATA_INTEGRITY_VIOLATION = 5003;
        public const int TRANSACTION_FAILED = 5004;

        // 🔹 6000 - External Services
        public const int EXTERNAL_SERVICE_ERROR = 6000;
        public const int API_CALL_FAILED = 6001;
        public const int TIMEOUT = 6002;
        public const int SERVICE_UNAVAILABLE = 6003;

        // 🔹 9000 - System / Critical Errors
        public const int INTERNAL_SERVER_ERROR = 9000;
        public const int CONFIGURATION_ERROR = 9001;
        public const int DEPENDENCY_FAILURE = 9002;
    }
}
