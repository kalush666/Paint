using System;
using MongoDB.Bson;

namespace Server.Models
{
    public class SketchEntry
    {
        public string Name { get; set; } = string.Empty;
        public ObjectId Id { get; set; }
    }
}