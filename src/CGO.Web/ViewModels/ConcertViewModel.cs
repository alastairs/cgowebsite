using System;
using System.ComponentModel.DataAnnotations;

namespace CGO.Web.ViewModels
{
    public class ConcertViewModel
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Please enter a title")]
        public string Title { get; set; }
        
        [Required(ErrorMessage = "Please choose a date")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Please choose a time")]
        public DateTime StartTime { get; set; }
        
        [Required(ErrorMessage = "Please enter the venue")]
        public string Location { get; set; }
        
        public string Description { get; set; }
        public bool IsPublished { get; set; }
    }
}