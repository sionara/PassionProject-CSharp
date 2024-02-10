using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace myPassionProject.Models
{
    public class Location
    {
        [Key]
        public int LocationId { get; set; }

        public string LocationName { get; set; }

        public string LocationAddress { get; set; }


    }

    public class LocationDto
    {
        [Key]
        public int LocationId { get; set; }

        public string LocationName { get; set; }

        public string LocationAddress { get; set; }

    }
}