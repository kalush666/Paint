namespace Common.Errors
{
    public static class AppErrors
    {
        public static class File
        {
            public const string NotFound = "File not found.";
            public const string Locked = "File is locked.";
            public const string AccessDenied = "File access denied.";
        }

        public static class Network
        {
            public const string ConnectionFailed = "Failed to connect to the server.";
            public const string Timeout = "Network timeout.";
            public const string Disconnected = "Connection was lost.";
        }

        public static class Mongo
        {
            public const string SketchNotFound = "Sketch not found in the database.";
            public const string ReadError = "Unable to read sketch from MongoDB.";
        }

        public static class Response
        {
            public const string UnableTosend = "Unable to send message";
        }

        public static class Generic
        {
            public const string Unknown = "An unknown error occurred.";
            public const string OperationFailed = "The operation failed.";
        }

        public static class Shapes
        {
            public const string NotAShape = "Given shape type is not a base shape";
        }
    }
}