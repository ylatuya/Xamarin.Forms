using System.Runtime.Serialization;

namespace Movies.Models.Collections
{
    [DataContract]
    public class CollectionInfo
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "poster_path")]
        public string PosterPath { get; set; }

        [DataMember(Name = "backdrop_path")]
        public string BackdropPath { get; set; }
    }
}