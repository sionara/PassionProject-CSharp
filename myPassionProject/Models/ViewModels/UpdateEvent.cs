using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace myPassionProject.Models.ViewModels
{
    public class UpdateEvent
    {
        //This viewmodel is a class which stores information that we need to present to /event/Update/{}

        //the existing event information

        public EventDto SelectedEvent { get; set; }

        // all locations to choose from when updating this event

        public IEnumerable<LocationDto> locationOptions { get; set; }

        //all orgs in system to choose from when updating this event
        public IEnumerable<OrganizationDto> organizationOptions { get; set; }
    }
}