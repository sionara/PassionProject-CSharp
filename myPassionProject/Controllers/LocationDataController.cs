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
using System.Diagnostics;

namespace myPassionProject.Controllers
{
    public class LocationDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Returns a list of locations in db
        /// </summary>
        /// <returns>
        /// List of Location data
        /// </returns>
        // GET: api/LocationData/ListLocations
        [HttpGet]
        public IEnumerable<LocationDto> ListLocations()
        {
            List<Location> Locations = db.Locations.ToList();
            List<LocationDto> locationDtos = new List<LocationDto>();

            Locations.ForEach(l => locationDtos.Add(new LocationDto()
            {
                LocationId = l.LocationId,
                LocationName = l.LocationName,
                LocationAddress = l.LocationAddress
            }));
            Debug.WriteLine(locationDtos);

            return locationDtos;
        }

        /// <summary>
        /// Returns a particular location based on id
        /// </summary>
        /// <param name="id">Id of a location</param>
        /// <returns>
        /// NotFound message or data of a location with id = {id}
        /// </returns>
        // GET: api/LocationData/FindLocation/5
        [ResponseType(typeof(LocationDto))]
        [HttpGet]
        public IHttpActionResult FindLocation(int id)
        {
            Location location = db.Locations.Find(id);
            LocationDto locationDto = new LocationDto()
            {
                LocationId = location.LocationId,
                LocationName = location.LocationName,
                LocationAddress = location.LocationAddress
            };
            if (location == null)
            {
                return NotFound();
            }

            return Ok(locationDto);
        }

        /// <summary>
        /// Updates the data of a location with id = {id} based on user input
        /// </summary>
        /// <param name="id">Event Id</param>
        /// <param name="location"> JSON data of an event </param>
        /// <returns>
        /// 204 (success, no response) 
        /// BAD REQUESST (400)
        /// or NOT FOUND (404) response.
        /// </returns>
        // POST: api/LocationData/UpdateLocation/5
        [ResponseType(typeof(void))]
        [HttpPost]
        [Authorize]
        public IHttpActionResult UpdateLocation(int id, Location location)
        {
            Debug.WriteLine("Method is reached.");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != location.LocationId)
            {
                return BadRequest();
            }

            db.Entry(location).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LocationExists(id))
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
        /// Adds new location into the database
        /// </summary>
        /// <param name="location">JSON data of a location</param>
        /// <returns>
        /// 201 (created)
        /// or 400 (Bad request)
        /// response
        /// </returns>
        // POST: api/LocationData/AddLocation
        [ResponseType(typeof(Location))]
        [HttpPost]
        [Authorize]
        public IHttpActionResult AddLocation(Location location)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Locations.Add(location);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = location.LocationId }, location);
        }

        /// <summary>
        /// Delets a location from db with id = {id}
        /// </summary>
        /// <param name="id">location id</param>
        /// <returns>
        /// 404 not found
        /// or 200 OK
        /// </returns>
        /// 
        // POST: api/LocationData/DeleteLocation/5
        [ResponseType(typeof(Location))]
        [HttpPost]
        [Authorize]
        
        public IHttpActionResult DeleteLocation(int id)
        {
            Location location = db.Locations.Find(id);
            if (location == null)
            {
                return NotFound();
            }

            db.Locations.Remove(location);
            db.SaveChanges();

            return Ok(location);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool LocationExists(int id)
        {
            return db.Locations.Count(e => e.LocationId == id) > 0;
        }
    }
}