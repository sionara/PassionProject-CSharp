using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace myPassionProject.Models.ViewModels
{
    public class CreateEvent
    {
        //This viewmodel is a class which stores information that we need to present to /event/Update/{}

        // all locations to choose from when updating this event

        public IEnumerable<LocationDto> locationOptions { get; set; }

        //all orgs to choose from in system
        public IEnumerable<OrganizationDto> organizationOptions { get; set; }
    }
}