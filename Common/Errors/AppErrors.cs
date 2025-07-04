﻿namespace Common.Errors
{
    public static class AppErrors
    {
        public static class File
        {
            public const string NotFound = "File not found.";
            public const string Locked = "File is locked by another user.";
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
            public const string DeleteError = "Unable to delete sketch from MongoDB.";
            public const string AlreadyExists = "Sketch by this name already exist";
            public const string InvalidJson = "Invalid JSON format for sketch.";
            public const string UploadError = "Failed to upload sketch to MongoDB.";
        }

        public static class Response
        {
            public const string UnableToSend = "Unable to send message";
        }

        public static class Request
        {
            public const string InvalidFormat = "Request format is invalid.";
            public const string UnsupportedType = "Request type is not supported.";
            public const string NotFound = "Requested resource was not found.";
            public const string BadRequest = "Bad request.";
            public const string Unauthorized = "Unauthorized request.";
            public const string EmptyRequest = "Request cannot be empty.";
        }

        public static class Generic
        {
            public const string Unknown = "An unknown error occurred.";
            public const string OperationFailed = "The operation failed.";
            public const string OperationCancelled = "The operation was cancelled.";
            public const string UnsupportedOperation = "This operation is not supported.";
        }

        public static class Sketch
        {
            public const string EmptySketch = "Sketch cannot be empty.";
        }

        public static class Shapes
        {
            public const string NotAShape = "Given shape type is not a base shape";
            public const string NoShapes = "No shapes available to process.";
        }

        public static class Server
        {
            public const string Suspended = "Server is currently suspended.";
            public const string ListenerCancelled = "Server listener task was cancelled.";
            public const string AcceptClientError = "Error occurred while accepting a client.";
            public const string ClientHandlingCancelled = "Client handling was cancelled.";
        }

    }
}