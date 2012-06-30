using System;

namespace CGO.Web.Models
{
    public class Concert
    {
        public int Id { get; private set; }

        public string Title { get; private set; }
        public DateTime DateAndStartTime { get; private set; }
        public string Location { get; private set; }

        public string Description
        {
            get
            {
                return 
@"Following the successes of *The Planets* in March and our Classical concert in April, CGO presents a programme of music from around the world.  

Starting close to home with Malcolm Arnold's spirited *Four Scottish Dances*, we will transport you to Japan for Keiko Abe's *Prism Rhapsody for Marimba and Orchestra* by way of the Czech Republic with *Martinu's Oboe Concerto*.  The closing leg of the tour will be to the US for Dvorak's beloved ninth symphony, *From The New World*.  

We are joined again by Freddie Brown on the podium, with soloists Jemma Bausor (oboe) and Yin-Shan (Eva) Hsieh on marimba.  

**Arnold** *Four Scottish Dances*<br />
**Martinu** *Oboe Concerto* <br />
**Keiko Abe** *Prism Rhapsody for Marimba and Orchestra* <br />
**Dvorak** Symphony No. 9, *From the New World*<br />

Conductor Freddie Brown <br />
Soloists Jemma Bausor and Yin-Shan (Eva) Hsieh";
            }
        }

        public Concert(int id, string title, DateTime dateAndStartTime, string location)
        {
            Id = id;
            Title = title;
            DateAndStartTime = dateAndStartTime;
            Location = location;
        }
    }
}