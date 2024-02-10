using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using myPassionProject.Models;

namespace myPassionProject.Controllers
{
    public class OrganizationDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/OrganizationData/ListOrganizations
        [HttpGet]
        public IEnumerable<OrganizationDto> ListOrganizations()
        {
            List<Organization> Organizations = db.Organizations.ToList();
            List<OrganizationDto> OrganizationDto = new List<OrganizationDto>();

            Organizations.ForEach(O => OrganizationDto.Add(new Models.OrganizationDto()
            {
                OrganizationId = O.OrganizationId,
                OrganizationName = O.OrganizationName,
                OrganizationContact = O.OrganizationContact
            }));

            return OrganizationDto;
        }

        // GET: api/OrganizationData/FindOrganization/5
        [ResponseType(typeof(Organization))]
        [HttpGet]
        public IHttpActionResult FindOrganization(int id)
        {
            Organization Organization = db.Organizations.Find(id);
            OrganizationDto OrganizationDto = new OrganizationDto()
            {
                OrganizationId = Organization.OrganizationId,
                OrganizationName = Organization.OrganizationName,
                OrganizationContact = Organization.OrganizationContact
            };
            if (Organization == null)
            {
                return NotFound();
            }

            return Ok(Organization);
        }

        // PUT: api/OrganizationData/UpdateOrganization/5
        [ResponseType(typeof(void))]
        [HttpPost]
        public IHttpActionResult PutOrganization(int id, Organization organization)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != organization.OrganizationId)
            {
                return BadRequest();
            }

            db.Entry(organization).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrganizationExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/OrganizationData/AddOrganization
        [ResponseType(typeof(Organization))]
        [HttpPost]
        public IHttpActionResult AddOrganization(Organization organization)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Organizations.Add(organization);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = organization.OrganizationId }, organization);
        }

        // DELETE: api/OrganizationData/DeleteOrganization/5
        [ResponseType(typeof(Organization))]
        [HttpPost]
        public IHttpActionResult DeleteOrganization(int id)
        {
            Organization organization = db.Organizations.Find(id);
            if (organization == null)
            {
                return NotFound();
            }

            db.Organizations.Remove(organization);
            db.SaveChanges();

            return Ok(organization);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool OrganizationExists(int id)
        {
            return db.Organizations.Count(e => e.OrganizationId == id) > 0;
        }
    }
}