using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace myPassionProject.Models
{
    public class Organization
    {
        [Key]
        public int OrganizationId { get; set; }

        public string OrganizationName { get; set; }

        public string OrganizationContact {  get; set; }

    }

    public class OrganizationDto
    {
        [Key]
        public int OrganizationId { get; set; }

        public string OrganizationName { get; set; }

        public string OrganizationContact { get; set; }
    }
}