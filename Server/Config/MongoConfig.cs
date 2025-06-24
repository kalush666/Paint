namespace Server.Config
{
    public static class MongoConfig
    {
        public static readonly string ConnectionString = "mongodb://root:jonathan06@localhost:27017";
        public static readonly string DatabaseName = "PainterDB";
        public static readonly string CollectionName = "Sketches";
    }
}
