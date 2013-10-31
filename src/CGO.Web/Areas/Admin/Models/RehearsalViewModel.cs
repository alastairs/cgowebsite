using System;
using System.Collections.Generic;

namespace CGO.Web.Areas.Admin.Models
{
    public class RehearsalViewModel
    {
        public int Id { get; set; }
        public string Venue { get; set; }
        public DateTime Date { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsSectionalRehearsal { get; set; }
        public IEnumerable<string> RequiredSections { get; set; }

        public IEnumerable<string> Sections
        {
            get
            {
                return new[] {"Strings", "Woodwind", "Brass", "Percussion", "Tutti"};
            }
        }
    }
}