using System;
using System.Runtime.Serialization;

namespace CGO.Web.ViewModels.Api
{
    [DataContract]
    public class ConcertViewModel
    {
        public string Href { get; set; }
        public int Id { get; set; }

        [DataMember(IsRequired = true)]
        public string Title { get; set; }

        [DataMember(IsRequired = true)]
        public DateTime Date { get; set; }

        [DataMember(IsRequired = true)]
        public DateTime StartTime { get; set; }

        [DataMember(IsRequired = true)]
        public string Location { get; set; }

        public bool IsPublished { get; set; }
    }
}