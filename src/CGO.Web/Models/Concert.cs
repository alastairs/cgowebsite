using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CGO.Web.Models
{
    public class Concert
    {
        public int Id { get; private set; }

        public string Title { get; private set; }
        public DateTime DateAndStartTime { get; private set; }
        public string Location { get; private set; }

        public Concert(int id, string title, DateTime dateAndStartTime, string location)
        {
            Id = id;
            Title = title;
            DateAndStartTime = dateAndStartTime;
            Location = location;
        }
    }
}