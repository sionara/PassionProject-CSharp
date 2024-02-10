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
        private ApplicationDbContext db = new ApplicationDbContext();

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

        // GET: api/EventData/FindEvent/5
        [ResponseType(typeof(Event))]
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

        // PUT: api/EventData/UpdateEvent/5
        [ResponseType(typeof(void))]
        [HttpPost]
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

        // POST: api/EventData/AddEvent
        [ResponseType(typeof(Event))]
        [HttpPost]
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

        // DELETE: api/EventData/DeleteEvent/5
        [ResponseType(typeof(Event))]
        [HttpPost]
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