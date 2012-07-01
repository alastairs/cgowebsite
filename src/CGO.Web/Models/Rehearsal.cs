using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CGO.Web.Models
{
    public class Rehearsal
    {
        public Rehearsal(int id, DateTime dateAndStartTime, DateTime finishTime, string loction)
        {
            Loction = loction;
            FinishTime = finishTime;
            DateAndStartTime = dateAndStartTime;
            Id = id;
        }

        public int Id { get; private set; }
        public DateTime DateAndStartTime { get; private set; }
        public DateTime FinishTime { get; private set; }
        public string Loction { get; private set; }
    }
}