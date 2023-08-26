using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Event
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public double Latitude { get; set; } // range: from -90 (South) to 90 (North)
        public double Longitude { get; set; } // range: from -180 (West) to 180 (East)
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public EventVisibility Visibility { get; set; }
        public SportsDiscipline SportsDiscipline { get; set; }
        public Difficulty Difficulty { get; set; }

        public Guid OrganizerId { get; set; }
        public User Organizer { get; set; }


       // public ICollection<User> Participants { get; set; }


    }
}
