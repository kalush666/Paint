using System;
using MongoDB.Bson;

namespace Server.Models
{
    public class SketchEntry
    {
        public string Name { get; set; } = "";
        public ObjectId Id { get; set; }
    }
}