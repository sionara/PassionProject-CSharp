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
using myPassionProject.Models.ViewModels;

namespace myPassionProject.Controllers
{
    public class EventController : Controller
    {
        //initialize the HttpClient and JavaScriptSerializer methods.
        private static readonly HttpClient httpClient;
        private static JavaScriptSerializer serializer = new JavaScriptSerializer();

        static EventController()
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false,
                //cookies are manually set in RequestHeader
                UseCookies = false
            };
            // setting an instance of HttpClient for the lifetime of our app to avoid repetitve code.
            httpClient = new HttpClient(handler); 
            httpClient.BaseAddress = new Uri("https://localhost:44389/api/");
        }

        /// <summary>
        /// Grabs the authentication cookie sent to this controller.
        /// For proper WebAPI authentication, you can send a post request with login credentials to the WebAPI and log the access token from the response. The controller already knows this token, so we're just passing it up the chain.
        /// 
        /// Here is a descriptive article which walks through the process of setting up authorization/authentication directly.
        /// https://docs.microsoft.com/en-us/aspnet/web-api/overview/security/individual-accounts-in-web-api
        /// </summary>
        private void GetApplicationCookie()
        {
            string token = "";
            //HTTP client is set up to be reused, otherwise it will exhaust server resources.
            //This is a bit dangerous because a previously authenticated cookie could be cached for
            //a follow-up request from someone else. Reset cookies in HTTP client before grabbing a new one.
            httpClient.DefaultRequestHeaders.Remove("Cookie");
            if (!User.Identity.IsAuthenticated) return;

            HttpCookie cookie = System.Web.HttpContext.Current.Request.Cookies.Get(".AspNet.ApplicationCookie");
            if (cookie != null) token = cookie.Value;

            //collect token as it is submitted to the controller
            //use it to pass along to the WebAPI.
            Debug.WriteLine("Token Submitted is : " + token);
            if (token != "") httpClient.DefaultRequestHeaders.Add("Cookie", ".AspNet.ApplicationCookie=" + token);

            return;
        }

        /// <summary>
        /// handles request to get list of all events. Calls ListEvents API to fetch the data.
        /// </summary>
        /// <returns>
        /// A view listing all events in the Db
        /// </returns>
        // GET: Event/List
        
        public ActionResult List()
        {
            //connects with EventData to fetch list of Events from Db.
            // curl https://localhost:44389/api/eventdata/listevents

            string url = "eventdata/listevents";
            HttpResponseMessage response = httpClient.GetAsync(url).Result;

            //Debug.WriteLine("The response code is ");
            //Debug.WriteLine(response.StatusCode); // this shows if we were able to access the API when calling Event/List method.

            //parse the response into type IEnumerable of Event objects
            IEnumerable<EventDto> events = response.Content.ReadAsAsync<IEnumerable<EventDto>>().Result;
            
            //Debug.WriteLine("Number of Events received: ");
            //Debug.WriteLine(events.Count());

            return View(events);
        }
        /// <summary>
        /// handles request to get details of one event based on Id. Calls FindEvent API.
        /// </summary>
        /// <param name="id">Id of the event being searched </param>
        /// <returns>
        /// A view with details of a particular event
        /// </returns>
        // GET: Event/Details/5
        public ActionResult Details(int id)
        {
            //connects with EventData to fetch list of Events from Db.
            // curl https://localhost:44389/api/eventdata/findevent

            string url = "eventdata/findevent/"+id;
            HttpResponseMessage response = httpClient.GetAsync(url).Result;

            //Debug.WriteLine("The response code is ");
            //Debug.WriteLine(response.StatusCode); // this shows if we were able to access the API when calling Event/List method.

            EventDto selectedEvent = response.Content.ReadAsAsync<EventDto>().Result;
           
            //Debug.WriteLine("Event Received: ");
            //Debug.WriteLine(selectedEvent.EventName);

            return View(selectedEvent);
        }


        /// <summary>
        /// handles request for the new view to add an event
        /// </summary>
        /// <returns>The New View to add events</returns>
        // GET: Event/New
        public ActionResult New()
        {
            //information about all location in the system.
            //GET api/locationData/listLocation

            //initialize ViewModel CreateEvent
            CreateEvent ViewModel = new CreateEvent();

            string url = "locationdata/listlocations";
            HttpResponseMessage response = httpClient.GetAsync(url).Result;
            IEnumerable<LocationDto> locationOptions = response.Content.ReadAsAsync<IEnumerable<LocationDto>>().Result;

            ViewModel.locationOptions = locationOptions; //convert LocationDto data into ViewModel.

            //all Organizations to choose from when updating this event
            url = "organizationdata/listOrganizations/";
            response = httpClient.GetAsync(url).Result;
            IEnumerable<OrganizationDto> organizationOptions = response.Content.ReadAsAsync<IEnumerable<OrganizationDto>>().Result;

            ViewModel.organizationOptions = organizationOptions;
            
            return View(ViewModel);
        }

        /// <summary>
        /// handles the request for the error page
        /// </summary>
        /// <returns>
        /// The Error.cshtml view
        /// </returns>
        //GET: Event/Error
        public ActionResult Error()
        {
            return View();
        }

        /// <summary>
        /// Handles POST request to create a new event. Calls the AddEvent API.
        /// </summary>
        /// <param name="Event">A new instance of Event object</param>
        /// <returns>
        /// Redirects to List.cshtml or Error.cshtml based on the result of query.
        /// </returns>
        // POST: Event/Create
        [HttpPost]
        [Authorize]
        public ActionResult Create(Event Event)
        {
            
            // calls helper function to pass cookie in api call
            GetApplicationCookie();

            //objective: add the new event data received into our db using our API.
            //curl -H "Content-type:application/json -d @Event.json https://localhost:44389/api/eventdata/addevent
            string url = "eventdata/addevent";

            //serialization of Event object into JSON to be transferred to server.
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

        /// <summary>
        /// Recevies request to edit a event with eventid of id and redirects to edit view.
        /// </summary>
        /// <param name="id">id of event being edited</param>
        /// <returns>
        /// Edit.cshtml with the form values populated by Event data with id of {id}
        /// </returns>
        // GET: Event/Edit/5
        [HttpGet]
        [Authorize]
        public ActionResult Edit(int id)
        {
            // calls helper function to pass cookie in api call
            GetApplicationCookie();

            UpdateEvent ViewModel = new UpdateEvent();

            //get the Event you would like to edit using its id
            //curl https://localhost:44389/api/eventdata/findevent/{id}

            string url = "eventdata/findevent/" + id;
            HttpResponseMessage response = httpClient.GetAsync(url).Result;

            EventDto selectedEvent = response.Content.ReadAsAsync<EventDto>().Result;
            ViewModel.SelectedEvent = selectedEvent;

            //all locations to choose from when updating this event
            url = "locationdata/listlocations/";
            response = httpClient.GetAsync(url).Result;
            IEnumerable<LocationDto> locationOptions = response.Content.ReadAsAsync<IEnumerable<LocationDto>>().Result;

            ViewModel.locationOptions = locationOptions;

            //all Organizations to choose from when updating this event
            url = "organizationdata/listOrganizations/";
            response = httpClient.GetAsync(url).Result;
            IEnumerable<OrganizationDto> organizationOptions = response.Content.ReadAsAsync<IEnumerable<OrganizationDto>>().Result;

            ViewModel.organizationOptions = organizationOptions;

            //Debug.WriteLine("Event Received: ");
            //Debug.WriteLine(selectedEvent.EventName);

            return View(ViewModel);
        }

        /// <summary>
        /// Handles request to update the information of an existing Event. Calls UpdateEvent API.
        /// </summary>
        /// <param name="id">Id of the Event being updated</param>
        /// <param name="Event">A new instance of Event object used to hold the httprequest as JSON</param>
        /// <returns>
        /// Redirect to details of Event with id of {id} with updated info.
        /// </returns>
        // POST: Event/Edit/5
        [HttpPost]
        [Authorize]
        public ActionResult Update(int id, Event Event)
        {
            // calls helper function to pass cookie in api call
            GetApplicationCookie();

            //objective: add the new event data received into our db using our API.
            //curl -H "Content-type:application/json -d @Event.json https://localhost:44389/api/eventdata/updateevent
            try
            {
                string url = "eventdata/UpdateEvent/" + id;
                //serialization of Event object into JSON to be transferred to server.
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
                return View("Error");
            }
        }

        /// <summary>
        /// Handles request to delete an Event with id = {id}. Calls FindEvent API.
        /// </summary>
        /// <param name="id">Id of Event being deleted</param>
        /// <returns>
        /// Redirects to ConfirmDelete.cshtml view.
        /// </returns>
        // GET: Event/ConfirmDelete/5
        [HttpGet]
        [Authorize]
        public ActionResult ConfirmDelete(int id)
        {
            // calls helper function to pass cookie in api call
            GetApplicationCookie();

            string url = "eventdata/findevent/" + id;
            HttpResponseMessage response = httpClient.GetAsync(url).Result;

            //Debug.WriteLine("The response code is ");
            //Debug.WriteLine(response.StatusCode); // this shows if we were able to access the API when calling Event/List method.

            EventDto selectedEvent = response.Content.ReadAsAsync<EventDto>().Result;

            //Debug.WriteLine("Event Received: ");
            //Debug.WriteLine(selectedEvent.EventName);

            return View(selectedEvent);
        }

        /// <summary>
        /// Handles request to confirm the deletion of an Event. Calls the DeleteEvent API.
        /// </summary>
        /// <param name="id">Id of the event being deleted</param>
        /// <param name="Event">A new instance of the Event object.</param>
        /// <returns>
        /// Redirect to List or Error view depending on server response.
        /// </returns>
        // POST: Event/Delete/5
        [HttpPost]
        [Authorize]
        public ActionResult Delete(int id, Event Event)
        {
            // calls helper function to pass cookie in api call
            GetApplicationCookie();

            //objective: add the new event data received into our db using our API.
            //curl -H "Content-type:application/json -d @Event.json https://localhost:44389/api/eventdata/deleteevent
            try
            {
                string url = "eventdata/DeleteEvent/" + id;

                // Serialization of Event object to be transferred to the server as JSON data.
                string jsonpayload = serializer.Serialize(Event);

                HttpContent content = new StringContent(jsonpayload);
                content.Headers.ContentType.MediaType = "application/json";

                HttpResponseMessage response = httpClient.PostAsync(url, content).Result;

                return RedirectToAction("List");
            }
            catch
            {
                return View("Error");
            }
        }
    }
}
