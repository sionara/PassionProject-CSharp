using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace myPassionProject.Models
{
    public class Event
    {
        [Key]
        public int EventId { get; set; }

        public string EventName { get; set; }

        public string registrationWebsite { get; set; }

        //an event is connected to one org
        //an org can plan many events.

        [ForeignKey("Organization")]
        public int OrganizationId { get; set; }
        public virtual Organization Organization { get; set; }

        //an Event is connected to one location
        //a location could host different events

        [ForeignKey("Location")]
        public int LocationId { get; set; }
        public virtual Location Location { get; set; }
    }

    public class EventDto
    {
        [Key]
        public int EventId { get; set; }

        public string EventName { get; set; }

        public string registrationWebsite { get; set; }

        public int OrganizationId { get; set; }
        public string OrganizationName { get; set; }

        public int LocationId { get; set; }
        public string LocationName { get; set; }
    }
}