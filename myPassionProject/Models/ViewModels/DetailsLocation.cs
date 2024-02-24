using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace myPassionProject.Models.ViewModels
{
    public class DetailsLocation
    {
        public LocationDto selectedLocation { get; set; }

        public IEnumerable<EventDto> hostedEvents { get; set; }
    }
}