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

        /// <summary>
        /// Returns all the Organizations from the Db
        /// </summary>
        /// <returns>
        /// List of OrganizationDtos
        /// </returns>
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

        /// <summary>
        /// Returns a particular Organization with id = {id}
        /// </summary>
        /// <param name="id">id of an organization</param>
        /// <returns>
        /// Not Found(404)
        /// or
        /// Organization objec
        /// </returns>
        // GET: api/OrganizationData/FindOrganization/5
        [ResponseType(typeof(OrganizationDto))]
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

            return Ok(OrganizationDto);
        }

        /// <summary>
        /// Updates data about a particular Organization in the system
        /// </summary>
        /// <param name="id">Id of an existing organization</param>
        /// <param name="organization">JSON data of an organization</param>
        /// <returns>
        /// 204 (success, no response) 
        /// BAD REQUESST (400)
        /// or NOT FOUND (404) response.
        /// </returns>
        /// 
        // POST: api/OrganizationData/UpdateOrganization/5
        [ResponseType(typeof(void))]
        [HttpPost]
        public IHttpActionResult UpdateOrganization(int id, Organization organization)
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

        /// <summary>
        /// Adds a new organization to the system
        /// </summary>
        /// <param name="organization">JSON data of an organization</param>
        /// <returns>
        /// 201 (created)
        /// or 400 (Bad request)
        /// response
        /// </returns>
        /// 
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

        /// <summary>
        /// Deletes a particular organization with id = {id}
        /// </summary>
        /// <param name="id">Id of an existing organization</param>
        /// <returns>
        /// 404 not found
        /// or 200 OK
        /// </returns>
        /// 
        // POST: api/OrganizationData/DeleteOrganization/5
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