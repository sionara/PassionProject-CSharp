using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;
using static System.Net.WebRequestMethods;
using System.Diagnostics;
using myPassionProject.Models;
using System.Web.Script.Serialization;

namespace myPassionProject.Controllers
{
    public class EventController : Controller
    {
        private static readonly HttpClient httpClient;
        private static JavaScriptSerializer serializer = new JavaScriptSerializer();

        static EventController() 
        {
            httpClient = new HttpClient(); // setting an instance of HttpClient for the lifetime of our app to avoid repetitve code.
            httpClient.BaseAddress = new Uri("https://localhost:44389/api/eventdata/");
        }
        
        // GET: Event/List
        public ActionResult List()
        {
            //connects with EventData to fetch list of Events from Db.
            // curl https://localhost:44389/api/eventdata/listevents

            string url = "listevents";
            HttpResponseMessage response = httpClient.GetAsync(url).Result;

            //Debug.WriteLine("The response code is ");
            //Debug.WriteLine(response.StatusCode); // this shows if we were able to access the API when calling Event/List method.

            //parse the response into type IEnumerable of Event objects
            IEnumerable<EventDto> events = response.Content.ReadAsAsync<IEnumerable<EventDto>>().Result;
            
            //Debug.WriteLine("Number of Events received: ");
            //Debug.WriteLine(events.Count());

            return View(events);
        }

        // GET: Event/Details/5
        public ActionResult Details(int id)
        {
            //connects with EventData to fetch list of Events from Db.
            // curl https://localhost:44389/api/eventdata/findevent

            string url = "findevent/"+id;
            HttpResponseMessage response = httpClient.GetAsync(url).Result;

            //Debug.WriteLine("The response code is ");
            //Debug.WriteLine(response.StatusCode); // this shows if we were able to access the API when calling Event/List method.

            EventDto selectedEvent = response.Content.ReadAsAsync<EventDto>().Result;
           
            //Debug.WriteLine("Event Received: ");
            //Debug.WriteLine(selectedEvent.EventName);

            return View(selectedEvent);
        }

        // GET: Event/New
        /// <summary>
        /// Receives the HTTP GET request for the new.cshtml page
        /// </summary>
        /// <returns>The New View</returns>
        public ActionResult New()
        {
            return View();
        }

        //GET: Event/Error
        public ActionResult Error()
        {
            return View();
        }

        // POST: Event/Create
        [HttpPost]
        public ActionResult Create(Event Event)
        {
            // test to ensure we receive correct payload.
            //Debug.WriteLine("The inputted event name is: ");
            //Debug.WriteLine(Event.EventName);

            //objective: add the new event data received into our db using our API.
            //curl -H "Content-type:application/json -d @Event.json https://localhost:44389/api/eventdata/addevent
            string url = "addevent";

            string jsonpayload = serializer.Serialize(Event);

            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";

            //check payload
            Debug.WriteLine("Json payload is: ");
            Debug.WriteLine(jsonpayload);

            HttpResponseMessage response = httpClient.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }
       
        }


        // GET: Event/Edit/5
        /// <summary>
        /// Recevies request to edit a event with eventid of id and redirects to edit view.
        /// </summary>
        /// <param name="id">id of event being edited</param>
        /// <returns></returns>
        public ActionResult Edit(int id)
        {
            //get the Event you would like to edit using its id
            //curl https://localhost:44389/api/eventdata/findevent/{id}

            string url = "findevent/" + id;
            HttpResponseMessage response = httpClient.GetAsync(url).Result;

            //Debug.WriteLine("The response code is ");
            //Debug.WriteLine(response.StatusCode); // this shows if we were able to access the API when calling Event/List method.

            EventDto selectedEvent = response.Content.ReadAsAsync<EventDto>().Result;

            //Debug.WriteLine("Event Received: ");
            //Debug.WriteLine(selectedEvent.EventName);

            return View(selectedEvent);
        }

        // POST: Event/Edit/5
        [HttpPost]
        public ActionResult Update(int id, Event Event)
        {
            //objective: add the new event data received into our db using our API.
            //curl -H "Content-type:application/json -d @Event.json https://localhost:44389/api/eventdata/updateevent
            try
            {
                string url = "UpdateEvent/" + id;

                string jsonpayload = serializer.Serialize(Event);

                HttpContent content = new StringContent(jsonpayload);
                content.Headers.ContentType.MediaType = "application/json";

                Debug.WriteLine("Json payload is: ");
                Debug.WriteLine(jsonpayload);

                HttpResponseMessage response = httpClient.PostAsync(url, content).Result;
                
                return RedirectToAction("Details/"+id);
            }
            catch
            {
                return View();
            }
        }

        // GET: Event/ConfirmDelete/5
        public ActionResult ConfirmDelete(int id)
        {
            string url = "findevent/" + id;
            HttpResponseMessage response = httpClient.GetAsync(url).Result;

            //Debug.WriteLine("The response code is ");
            //Debug.WriteLine(response.StatusCode); // this shows if we were able to access the API when calling Event/List method.

            EventDto selectedEvent = response.Content.ReadAsAsync<EventDto>().Result;

            //Debug.WriteLine("Event Received: ");
            //Debug.WriteLine(selectedEvent.EventName);

            return View(selectedEvent);
        }

        // POST: Event/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, Event Event)
        {
            //objective: add the new event data received into our db using our API.
            //curl -H "Content-type:application/json -d @Event.json https://localhost:44389/api/eventdata/deleteevent
            try
            {
                string url = "DeleteEvent/" + id;

                string jsonpayload = serializer.Serialize(Event);

                HttpContent content = new StringContent(jsonpayload);
                content.Headers.ContentType.MediaType = "application/json";

                HttpResponseMessage response = httpClient.PostAsync(url, content).Result;

                return RedirectToAction("List");
            }
            catch
            {
                return View();
            }
        }
    }
}
