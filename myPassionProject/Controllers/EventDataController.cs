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
    public class EventDataController : ApiController
    {
        /// <summary>
        /// login credentials
        /// user: sion@outlook.com
        /// </summary>
        
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Queries the Db for all Events and sends it back as a list of EventDTos
        /// </summary>
        /// <returns>
        /// List of EventDto objects
        /// </returns>
        /// 
        // GET: api/EventData/ListEvents
        [HttpGet]
        public IEnumerable<EventDto> ListEvents()
        {
            List<Event> Events = db.Events.ToList();
            List<EventDto> EventDtos = new List<EventDto>();

            Events.ForEach(e => EventDtos.Add(new EventDto()
            {
                EventId = e.EventId,
                EventName = e.EventName,
                registrationWebsite = e.registrationWebsite,
                OrganizationName = e.Organization.OrganizationName,
                LocationName = e.Location.LocationName
            })) ;

            return EventDtos;
        }

        /// <summary>
        /// Returns data of all events associated with a paritcular location with id = {id}
        /// </summary>
        /// <param name="id">Id of a particular Location in Db</param>
        /// <returns>
        /// OK: data of all events matching a particular locationId.
        /// </returns>
        /// 
        //GET: api/EventData/ListEventsForLocation
        [HttpGet]
        [ResponseType(typeof(EventDto))]
        public IHttpActionResult ListEventsForLocation(int id)
        {
            // store all events with locationId = id in a list
            List<Event> events = db.Events.Where(e => e.LocationId == id).ToList();
            // initialize eventdto list
            List<EventDto> eventdtos = new List<EventDto>();

            events.ForEach(e => eventdtos.Add(new EventDto()
            {
                EventId = e.EventId,
                EventName = e.EventName,
                registrationWebsite = e.registrationWebsite,
                OrganizationName = e.Organization.OrganizationName,
                LocationName = e.Location.LocationName
            }));

            return Ok(eventdtos);
        }

        /// <summary>
        /// Returns all events that are associated with an organizaiton with id = {id}
        /// </summary>
        /// <param name="id">Id of a existing organization</param>
        /// <returns>
        /// OK: data of all events matching a particular organizationid
        /// </returns>
        /// 
        //GET: api/EventData/ListEventsForOrganization
        [HttpGet]
        [ResponseType(typeof(EventDto))]
        public IHttpActionResult ListEventsForOrganization(int id)
        {
            // store all events with locationId = id in a list
            List<Event> events = db.Events.Where(e => e.OrganizationId == id).ToList();
            // initialize eventdto list
            List<EventDto> eventdtos = new List<EventDto>();

            events.ForEach(e => eventdtos.Add(new EventDto()
            {
                EventId = e.EventId,
                EventName = e.EventName,
                registrationWebsite = e.registrationWebsite,
                OrganizationName = e.Organization.OrganizationName,
                LocationName = e.Location.LocationName
            }));

            return Ok(eventdtos);
        }

        /// <summary>
        /// Finds a particular Event in Db based on id
        /// </summary>
        /// <param name="id">Id of Event object being searched</param>
        /// <returns>
        /// An EventDto object with Id ={id}
        /// </returns>
        // GET: api/EventData/FindEvent/5
        [ResponseType(typeof(EventDto))]
        [HttpGet]
        public IHttpActionResult FindEvent(int id)
        {
            Event @event = db.Events.Find(id);
            EventDto EventDto = new EventDto()
            {
                EventId = @event.EventId,
                EventName = @event.EventName,
                registrationWebsite = @event.registrationWebsite,
                LocationName = @event.Location.LocationName,
                OrganizationName = @event.Organization.OrganizationName

            };

            if (@event == null)
            {
                return NotFound();
            }

            return Ok(EventDto);
        }

        /// <summary>
        /// Updates a particular Event with id = {id} in the Db based on user input
        /// </summary>
        /// <param name="id">Id of Event being updated</param>
        /// <param name="@event">A new Event object</param>
        /// <returns>
        /// Bad Request(400), 
        /// Not Found(404), 
        /// or SaveChanges(204) response
        /// </returns>
        // POST: api/EventData/UpdateEvent/5
        [ResponseType(typeof(void))]
        [HttpPost]
        [Authorize] // this protects against attacks that could affect data
        public IHttpActionResult UpdateEvent(int id, Event @event)
        {
            Debug.WriteLine("Update method reached."); //check that the method is reached
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != @event.EventId)
            {
                Debug.WriteLine("ID mismatch"); //error with id matching.
                return BadRequest();
            }

            db.Entry(@event).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EventExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            Debug.WriteLine("Everything passed.");
            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Adds new Event data into the Db based on user input
        /// </summary>
        /// <param name="@event">JSON format of Event</param>
        /// <returns>
        /// HEADER: 201
        /// OR
        /// HEADER: 400
        /// </returns>
        // POST: api/EventData/AddEvent
        [ResponseType(typeof(Event))]
        [HttpPost]
        [Authorize]
        public IHttpActionResult AddEvent(Event @event)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Events.Add(@event);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = @event.EventId }, @event);
        }

        /// <summary>
        /// Deletes an Event in the system with id = {id}
        /// </summary>
        /// <param name="id">Id of a particular Event</param>
        /// <returns>
        /// 200 OK 
        /// or Not Found (404)
        /// </returns>
        // POST: api/EventData/DeleteEvent/5
        [ResponseType(typeof(Event))]
        [HttpPost]
        [Authorize]
        public IHttpActionResult DeleteEvent(int id)
        {
            Event @event = db.Events.Find(id);
            if (@event == null)
            {
                return NotFound();
            }

            db.Events.Remove(@event);
            db.SaveChanges();

            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool EventExists(int id)
        {
            return db.Events.Count(e => e.EventId == id) > 0;
        }
    }
}