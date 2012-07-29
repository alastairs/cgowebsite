using System;
using System.ComponentModel.DataAnnotations;

namespace CGO.Web.ViewModels
{
    public class ConcertViewModel
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Please enter a title")]
        public string Title { get; set; }
        
        public DateTime Date { get; set; }
        public DateTime StartTime { get; set; }
        
        [Required]
        public string Location { get; set; }
        
        public string Description { get; set; }
        public bool Published { get; set; }
    }
}