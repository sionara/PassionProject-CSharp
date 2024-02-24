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
using System.Web.UI.WebControls;

namespace myPassionProject.Controllers
{
    public class LocationController : Controller
    {
        //initialize the HttpClient and JavaScriptSerializer methods.
        private static readonly HttpClient httpClient;
        private static JavaScriptSerializer serializer = new JavaScriptSerializer();

        static LocationController()
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
        /// handles request to get list of all locations. Calls ListLocations API to fetch the data.
        /// </summary>
        /// <returns>
        /// A view listing all locations in the Db
        /// </returns>
        // GET: location/List
        public ActionResult List()
        {
            //connects with locationdata to fetch list of locations from Db.
            // curl https://localhost:44389/api/locationdata/listlocations

            string url = "locationdata/listlocations";
            HttpResponseMessage response = httpClient.GetAsync(url).Result;

            Debug.WriteLine("The response code is ");
            Debug.WriteLine(response.StatusCode); // this shows if we were able to access the API when calling location/List method.

            //parse the response into type IEnumerable of location objects
            IEnumerable<LocationDto> locations = response.Content.ReadAsAsync<IEnumerable<LocationDto>>().Result;

            Debug.WriteLine("Number of locations received: ");
            Debug.WriteLine(locations.Count());

            return View(locations);
        }

        /// <summary>
        /// handles request to get details of one location based on Id. Calls FindLocation API.
        /// Also fetches related event data by calling event API. Shows it on one ViewModel.
        /// </summary>
        /// <param name="id">Id of the location being searched </param>
        /// <returns>
        /// A view with details of a particular location
        /// </returns>
        // GET: location/Details/5
        public ActionResult Details(int id)
        {
            //connects with locationdata to fetch list of locations from Db.
            // curl https://localhost:44389/api/locationdata/findlocation

            // initalize instance of DetailsLocation ViewModel
            DetailsLocation ViewModel = new DetailsLocation();

            string url = "locationdata/findlocation/" + id;
            HttpResponseMessage response = httpClient.GetAsync(url).Result;

            //Debug.WriteLine("The response code is ");
            //Debug.WriteLine(response.StatusCode); // this shows if we were able to access the API when calling location/List method.

            LocationDto selectedlocation = response.Content.ReadAsAsync<LocationDto>().Result;

            //Debug.WriteLine("location Received: ");
            //Debug.WriteLine(selectedlocation.locationName);

            //turn response into ViewModel format
            ViewModel.selectedLocation = selectedlocation;

            //get data about Events
            url = "eventdata/listeventsforlocation/" + id;
            response = httpClient.GetAsync(url).Result;
            IEnumerable<EventDto> hostedEvents = response.Content.ReadAsAsync<IEnumerable<EventDto>>().Result;

            //convert related events into viewmodel format
            ViewModel.hostedEvents = hostedEvents;

            return View(ViewModel);
        }


        /// <summary>
        /// Receives the HTTP GET request for the new.cshtml page
        /// </summary>
        /// <returns>The New View</returns>
        // GET: location/New
        public ActionResult New()
        { 
            return View();
        }

        /// <summary>
        /// handles the request for the error page
        /// </summary>
        /// <returns>
        /// The Error.cshtml view
        /// </returns>
        //GET: location/Error
        public ActionResult Error()
        {
            return View();
        }

        /// <summary>
        /// Handles POST request to create a new location. Calls the AddLocation API.
        /// </summary>
        /// <param name="location">A new instance of location object</param>
        /// <returns>
        /// Redirects to List.cshtml or Error.cshtml based on the result of query.
        /// </returns>
        // POST: location/Create
        [HttpPost]
        [Authorize]
        public ActionResult Create(Location location)
        {
            // calls helper function to pass cookie in api call
            GetApplicationCookie();
            
            //objective: add the new location data received into our db using our API.
            //curl -H "Content-type:application/json -d @location.json https://localhost:44389/api/locationdata/addlocation
            string url = "locationData/addLocation";

            string jsonpayload = serializer.Serialize(location);

            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";

            //check payload
            Debug.WriteLine("Json payload is: ");
            Debug.WriteLine(jsonpayload);

            HttpResponseMessage response = httpClient.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                Debug.WriteLine(response);
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }

        }

        /// <summary>
        /// Recevies request to edit a location with locationid of id and redirects to edit view.
        /// </summary>
        /// <param name="id">id of location being edited</param>
        /// <returns>
        /// Edit.cshtml with the form values populated by location data with id of {id}
        /// </returns>
        // GET: location/Edit/5
        [Authorize]
        public ActionResult Edit(int id)
        {
            // calls helper function to pass cookie in api call
            GetApplicationCookie();

            //get the location you would like to edit using its id
            //curl https://localhost:44389/api/locationdata/findlocation/{id}

            string url = "locationdata/findlocation/" + id;
            HttpResponseMessage response = httpClient.GetAsync(url).Result;

            //Debug.WriteLine("The response code is ");
            //Debug.WriteLine(response.StatusCode); // this shows if we were able to access the API when calling location/List method.

            LocationDto selectedlocation = response.Content.ReadAsAsync<LocationDto>().Result;

            return View(selectedlocation);
        }
        
        /// <summary>
        /// Handles request to update the information of an existing location. Calls UpdateLocation API.
        /// </summary>
        /// <param name="id">Id of the location being updated</param>
        /// <param name="location">A new instance of Location object used to hold the httprequest as JSON</param>
        /// <returns>
        /// Redirect to details of location with id of {id} with updated info.
        /// </returns>
        // POST: location/Edit/5
        [HttpPost]
        [Authorize]
        public ActionResult Update(int id, Location location)
        {
            // calls helper function to pass cookie in api call
            GetApplicationCookie();

            //objective: add the new location data received into our db using our API.
            //curl -H "Content-type:application/json -d @location.json https://localhost:44389/api/locationdata/updatelocation
            try
            {
                string url = "locationdata/Updatelocation/" + id;

                string jsonpayload = serializer.Serialize(location);

                HttpContent content = new StringContent(jsonpayload);
                content.Headers.ContentType.MediaType = "application/json";

                Debug.WriteLine("Json payload is: ");
                Debug.WriteLine(jsonpayload);

                HttpResponseMessage response = httpClient.PostAsync(url, content).Result;

                return RedirectToAction("Details/" + id);
            }
            catch
            {
                return View("Error");
            }
        }

        /// <summary>
        /// Handles request to delete an location with id = {id}. Calls FindLocation API.
        /// </summary>
        /// <param name="id">Id of location being deleted</param>
        /// <returns>
        /// Redirects to ConfirmDelete.cshtml view.
        /// </returns>
        // GET: location/ConfirmDelete/5
        [Authorize]
        public ActionResult ConfirmDelete(int id)
        {
            // calls helper function to pass cookie in api call
            GetApplicationCookie();

            string url = "locationdata/findlocation/" + id;
            HttpResponseMessage response = httpClient.GetAsync(url).Result;

            //Debug.WriteLine("The response code is ");
            //Debug.WriteLine(response.StatusCode); // this shows if we were able to access the API when calling location/List method.

            LocationDto selectedlocation = response.Content.ReadAsAsync<LocationDto>().Result;

            //Debug.WriteLine("location Received: ");
            //Debug.WriteLine(selectedlocation.locationName);

            return View(selectedlocation);
        }

        /// <summary>
        /// Handles request to confirm the deletion of an location. Calls the DeleteLocation API.
        /// </summary>
        /// <param name="id">Id of the location being deleted</param>
        /// <param name="location">A new instance of the Location object.</param>
        /// <returns>
        /// Redirect to List or Error view depending on server response.
        /// </returns>
        // POST: location/Delete/5
        [HttpPost]
        [Authorize]
        public ActionResult Delete(int id, Location location)
        {
            // calls helper function to pass cookie in api call
            GetApplicationCookie();

            //objective: add the new location data received into our db using our API.
            //curl -H "Content-type:application/json -d @location.json https://localhost:44389/api/locationdata/deletelocation
            try
            {
                string url = "locationdata/Deletelocation/" + id;

                string jsonpayload = serializer.Serialize(location);

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
