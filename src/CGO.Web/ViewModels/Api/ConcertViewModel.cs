using System;
using System.ComponentModel.DataAnnotations;

namespace CGO.Web.ViewModels.Api
{
    public class ConcertViewModel
    {
        public string Href { get; set; }

        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        // Automatically required, because this is a value type
        public DateTime DateAndStartTime { get; set; }

        [Required]
        public string Location { get; set; }

        public bool IsPublished { get; set; }
    }
}