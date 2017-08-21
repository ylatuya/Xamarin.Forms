using System.Runtime.Serialization;

namespace Movies.Models.Company
{
    [DataContract]
    public class ParentCompany
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "logo_path")]
        public string LogoPath { get; set; }
    }
}