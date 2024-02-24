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
    public class OrganizationController : Controller
    {
        //initialize the HttpClient and JavaScriptSerializer methods.
        private static readonly HttpClient httpClient;
        private static JavaScriptSerializer serializer = new JavaScriptSerializer();

        static OrganizationController()
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
        /// handles request to get list of all Organizations. Calls ListOrganizations API to fetch the data.
        /// </summary>
        /// <returns>
        /// A view listing all Organizations in the Db
        /// </returns>
        // GET: Organization/List
        public ActionResult List()
        {
            //connects with OrganizationData to fetch list of Organizations from Db.
            // curl https://localhost:44389/api/Organizationdata/listOrganizations

            string url = "organizationdata/listOrganizations";
            HttpResponseMessage response = httpClient.GetAsync(url).Result;

            //Debug.WriteLine("The response code is ");
            //Debug.WriteLine(response.StatusCode); // this shows if we were able to access the API when calling Organization/List method.

            //parse the response into type IEnumerable of Organization objects
            IEnumerable<OrganizationDto> Organizations = response.Content.ReadAsAsync<IEnumerable<OrganizationDto>>().Result;

            //Debug.WriteLine("Number of Organizations received: ");
            //Debug.WriteLine(Organizations.Count());

            return View(Organizations);
        }

        /// <summary>
        /// handles request to get details of one Organization based on Id. Calls FindOrganization API.
        /// </summary>
        /// <param name="id">Id of the Organization being searched </param>
        /// <returns>
        /// A view with details of a particular Organization
        /// </returns>
        // GET: Organization/Details/5
        public ActionResult Details(int id)
        {
            //connects with OrganizationData to fetch list of Organizations from Db.
            // curl https://localhost:44389/api/Organizationdata/findOrganization

            // initalize instance of DetailsOrganization ViewModel
            DetailsOrganization ViewModel = new DetailsOrganization();

            string url = "organizationdata/findOrganization/" + id;
            HttpResponseMessage response = httpClient.GetAsync(url).Result;

            OrganizationDto selectedOrganization = response.Content.ReadAsAsync<OrganizationDto>().Result;

            //turn response into ViewModel format
            ViewModel.selectedOrganization = selectedOrganization;

            //get data about Events
            url = "eventdata/listeventsfororganization/" + id;
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
        // GET: Organization/New
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
        //GET: Organization/Error
        public ActionResult Error()
        {
            return View();
        }

        /// <summary>
        /// Handles POST request to create a new Organization. Calls the AddOrganization API.
        /// </summary>
        /// <param name="Organization">A new instance of Organization object</param>
        /// <returns>
        /// Redirects to List.cshtml or Error.cshtml based on the result of query.
        /// </returns>
        // POST: Organization/Create
        [HttpPost]
        [Authorize]
        public ActionResult Create(Organization Organization)
        {
            // calls helper function to pass cookie in api call
            GetApplicationCookie();

            //objective: add the new Organization data received into our db using our API.
            //curl -H "Content-type:application/json -d @Organization.json https://localhost:44389/api/Organizationdata/addOrganization
            string url = "organizationdata/addOrganization";

            string jsonpayload = serializer.Serialize(Organization);

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
        /// Recevies request to edit a Organization with Organizationid of id and redirects to edit view. Calls FindOrganization API.
        /// </summary>
        /// <param name="id">id of Organization being edited</param>
        /// <returns>
        /// Edit.cshtml view
        /// </returns>
        // GET: Organization/Edit/5
        [Authorize]
        public ActionResult Edit(int id)
        {
            // calls helper function to pass cookie in api call
            GetApplicationCookie();

            //get the Organization you would like to edit using its id
            //curl https://localhost:44389/api/Organizationdata/findOrganization/{id}

            string url = "organizationdata/findOrganization/" + id;
            HttpResponseMessage response = httpClient.GetAsync(url).Result;

            //Debug.WriteLine("The response code is ");
            //Debug.WriteLine(response.StatusCode); // this shows if we were able to access the API when calling Organization/List method.

            OrganizationDto selectedOrganization = response.Content.ReadAsAsync<OrganizationDto>().Result;

            //Debug.WriteLine("Organization Received: ");
            //Debug.WriteLine(selectedOrganization.OrganizationName);

            return View(selectedOrganization);
        }

        /// <summary>
        /// Handles request to update the information of an existing Organization. Calls UpdateOrganization API.
        /// </summary>
        /// <param name="id">Id of the Organization being updated</param>
        /// <param name="Organization">A new instance of Organization object used to hold the httprequest as JSON</param>
        /// <returns>
        /// Redirect to details of Organization with id of {id} with updated info.
        /// </returns>
        // POST: Organization/Edit/5
        [HttpPost]
        [Authorize]
        public ActionResult Update(int id, Organization Organization)
        {
            // calls helper function to pass cookie in api call
            GetApplicationCookie();

            //objective: add the new Organization data received into our db using our API.
            //curl -H "Content-type:application/json -d @Organization.json https://localhost:44389/api/Organizationdata/updateOrganization
            try
            {
                string url = "organizationdata/UpdateOrganization/" + id;

                string jsonpayload = serializer.Serialize(Organization);

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
        /// Handles request to delete an Organization with id = {id}. Calls FindOrganization API.
        /// </summary>
        /// <param name="id">Id of Organization being deleted</param>
        /// <returns>
        /// Redirects to ConfirmDelete.cshtml view.
        /// </returns>
        // GET: Organization/ConfirmDelete/5
        [Authorize]
        public ActionResult ConfirmDelete(int id)
        {
            // calls helper function to pass cookie in api call
            GetApplicationCookie();

            string url = "organizationdata/findOrganization/" + id;
            HttpResponseMessage response = httpClient.GetAsync(url).Result;

            //Debug.WriteLine("The response code is ");
            //Debug.WriteLine(response.StatusCode); // this shows if we were able to access the API when calling Organization/List method.

            OrganizationDto selectedOrganization = response.Content.ReadAsAsync<OrganizationDto>().Result;

            //Debug.WriteLine("Organization Received: ");
            //Debug.WriteLine(selectedOrganization.OrganizationName);

            return View(selectedOrganization);
        }

        /// <summary>
        /// Handles request to confirm the deletion of an Organization. Calls the DeleteOrganization API.
        /// </summary>
        /// <param name="id">Id of the Organization being deleted</param>
        /// <param name="Organization">A new instance of the Organization object.</param>
        /// <returns>
        /// Redirect to List or Error view depending on server response.
        /// </returns>
        // POST: Organization/Delete/5
        [HttpPost]
        [Authorize]
        public ActionResult Delete(int id, Organization Organization)
        {
            // calls helper function to pass cookie in api call
            GetApplicationCookie();

            //objective: add the new Organization data received into our db using our API.
            //curl -H "Content-type:application/json -d @Organization.json https://localhost:44389/api/Organizationdata/deleteOrganization
            try
            {
                string url = "organizationdata/DeleteOrganization/" + id;

                string jsonpayload = serializer.Serialize(Organization);

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
