using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace DB_Benchmark.Models.MongoDB
{
    public class Movie
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public int MovieID { get; set; }
        public string Title { get; set; }
        public string Age { get; set; }
        public string MediaType { get; set; }
        public int Runtime { get; set; }
        public IEnumerable<Country> ReleasedInCountries { get; set; }
        public IEnumerable<MovieRating> Ratings { get; set; }
        public IEnumerable<string> Platforms { get; set; }
    }
}
