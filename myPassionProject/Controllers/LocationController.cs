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
    public class LocationController : Controller
    {
        private static readonly HttpClient httpClient;
        private static JavaScriptSerializer serializer = new JavaScriptSerializer();

        static LocationController()
        {
            httpClient = new HttpClient(); // setting an instance of HttpClient for the lifetime of our app to avoid repetitve code.
            httpClient.BaseAddress = new Uri("https://localhost:44389/api/locationdata/");
        }

        // GET: location/List
        public ActionResult List()
        {
            //connects with locationdata to fetch list of locations from Db.
            // curl https://localhost:44389/api/locationdata/listlocations

            string url = "listlocations";
            HttpResponseMessage response = httpClient.GetAsync(url).Result;

            //Debug.WriteLine("The response code is ");
            //Debug.WriteLine(response.StatusCode); // this shows if we were able to access the API when calling location/List method.

            //parse the response into type IEnumerable of location objects
            IEnumerable<LocationDto> locations = response.Content.ReadAsAsync<IEnumerable<LocationDto>>().Result;

            //Debug.WriteLine("Number of locations received: ");
            //Debug.WriteLine(locations.Count());

            return View(locations);
        }

        // GET: location/Details/5
        public ActionResult Details(int id)
        {
            //connects with locationdata to fetch list of locations from Db.
            // curl https://localhost:44389/api/locationdata/findlocation

            string url = "findlocation/" + id;
            HttpResponseMessage response = httpClient.GetAsync(url).Result;

            //Debug.WriteLine("The response code is ");
            //Debug.WriteLine(response.StatusCode); // this shows if we were able to access the API when calling location/List method.

            LocationDto selectedlocation = response.Content.ReadAsAsync<LocationDto>().Result;

            //Debug.WriteLine("location Received: ");
            //Debug.WriteLine(selectedlocation.locationName);

            return View(selectedlocation);
        }

        // GET: location/New
        /// <summary>
        /// Receives the HTTP GET request for the new.cshtml page
        /// </summary>
        /// <returns>The New View</returns>
        public ActionResult New()
        {
            return View();
        }

        //GET: location/Error
        public ActionResult Error()
        {
            return View();
        }

        // POST: location/Create
        [HttpPost]
        public ActionResult Create(Location location)
        {
            // test to ensure we receive correct payload.
            //Debug.WriteLine("The inputted location name is: ");
            //Debug.WriteLine(location.locationName);

            //objective: add the new location data received into our db using our API.
            //curl -H "Content-type:application/json -d @location.json https://localhost:44389/api/locationdata/addlocation
            string url = "addlocation";

            string jsonpayload = serializer.Serialize(location);

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


        // GET: location/Edit/5
        /// <summary>
        /// Recevies request to edit a location with locationid of id and redirects to edit view.
        /// </summary>
        /// <param name="id">id of location being edited</param>
        /// <returns></returns>
        public ActionResult Edit(int id)
        {
            //get the location you would like to edit using its id
            //curl https://localhost:44389/api/locationdata/findlocation/{id}

            string url = "findlocation/" + id;
            HttpResponseMessage response = httpClient.GetAsync(url).Result;

            //Debug.WriteLine("The response code is ");
            //Debug.WriteLine(response.StatusCode); // this shows if we were able to access the API when calling location/List method.

            LocationDto selectedlocation = response.Content.ReadAsAsync<LocationDto>().Result;

            //Debug.WriteLine("location Received: ");
            //Debug.WriteLine(selectedlocation.locationName);

            return View(selectedlocation);
        }

        // POST: location/Edit/5
        [HttpPost]
        public ActionResult Update(int id, Location location)
        {
            //objective: add the new location data received into our db using our API.
            //curl -H "Content-type:application/json -d @location.json https://localhost:44389/api/locationdata/updatelocation
            try
            {
                string url = "Updatelocation/" + id;

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
                return View();
            }
        }

        // GET: location/ConfirmDelete/5
        public ActionResult ConfirmDelete(int id)
        {
            string url = "findlocation/" + id;
            HttpResponseMessage response = httpClient.GetAsync(url).Result;

            //Debug.WriteLine("The response code is ");
            //Debug.WriteLine(response.StatusCode); // this shows if we were able to access the API when calling location/List method.

            LocationDto selectedlocation = response.Content.ReadAsAsync<LocationDto>().Result;

            //Debug.WriteLine("location Received: ");
            //Debug.WriteLine(selectedlocation.locationName);

            return View(selectedlocation);
        }

        // POST: location/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, Location location)
        {
            //objective: add the new location data received into our db using our API.
            //curl -H "Content-type:application/json -d @location.json https://localhost:44389/api/locationdata/deletelocation
            try
            {
                string url = "Deletelocation/" + id;

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
