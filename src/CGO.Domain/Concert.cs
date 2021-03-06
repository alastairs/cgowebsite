﻿using System;

namespace CGO.Domain
{
    public class Concert
    {
        public int Id { get; private set; }

        public string Title { get; private set; }
        public DateTime DateAndStartTime { get; private set; }
        public string Location { get; private set; }
        public bool IsPublished { get; private set; }

        public string Description { get; set; }

        public static readonly DateTime DateOfFirstConcert = new DateTime(2005, 03, 07);

        public Concert(int id, string title, DateTime dateAndStartTime, string location)
        {
            Id = id;
            Title = title;
            DateAndStartTime = dateAndStartTime;
            Location = location;
        }

        public void ChangeTitle(string newTitle)
        {
            Title = newTitle;
        }

        public void ChangeDateAndStartTime(DateTime newDateAndStartTime)
        {
            DateAndStartTime = newDateAndStartTime;
        }

        public void ChangeLocation(string newLocation)
        {
            Location = newLocation;
        }

        public void Publish()
        {
            IsPublished = true;
        }
    }
}