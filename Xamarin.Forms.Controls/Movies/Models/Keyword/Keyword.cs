using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Movies.Models.Keyword
{
    [DataContract]
    public class Keyword 
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }
    }
}