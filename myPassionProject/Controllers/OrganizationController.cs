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
    public class OrganizationController : Controller
    {
        private static readonly HttpClient httpClient;
        private static JavaScriptSerializer serializer = new JavaScriptSerializer();

        static OrganizationController()
        {
            httpClient = new HttpClient(); // setting an instance of HttpClient for the lifetime of our app to avoid repetitve code.
            httpClient.BaseAddress = new Uri("https://localhost:44389/api/Organizationdata/");
        }

        // GET: Organization/List
        public ActionResult List()
        {
            //connects with OrganizationData to fetch list of Organizations from Db.
            // curl https://localhost:44389/api/Organizationdata/listOrganizations

            string url = "listOrganizations";
            HttpResponseMessage response = httpClient.GetAsync(url).Result;

            //Debug.WriteLine("The response code is ");
            //Debug.WriteLine(response.StatusCode); // this shows if we were able to access the API when calling Organization/List method.

            //parse the response into type IEnumerable of Organization objects
            IEnumerable<OrganizationDto> Organizations = response.Content.ReadAsAsync<IEnumerable<OrganizationDto>>().Result;

            //Debug.WriteLine("Number of Organizations received: ");
            //Debug.WriteLine(Organizations.Count());

            return View(Organizations);
        }

        // GET: Organization/Details/5
        public ActionResult Details(int id)
        {
            //connects with OrganizationData to fetch list of Organizations from Db.
            // curl https://localhost:44389/api/Organizationdata/findOrganization

            string url = "findOrganization/" + id;
            HttpResponseMessage response = httpClient.GetAsync(url).Result;

            //Debug.WriteLine("The response code is ");
            //Debug.WriteLine(response.StatusCode); // this shows if we were able to access the API when calling Organization/List method.

            OrganizationDto selectedOrganization = response.Content.ReadAsAsync<OrganizationDto>().Result;

            //Debug.WriteLine("Organization Received: ");
            //Debug.WriteLine(selectedOrganization.OrganizationName);

            return View(selectedOrganization);
        }

        // GET: Organization/New
        /// <summary>
        /// Receives the HTTP GET request for the new.cshtml page
        /// </summary>
        /// <returns>The New View</returns>
        public ActionResult New()
        {
            return View();
        }

        //GET: Organization/Error
        public ActionResult Error()
        {
            return View();
        }

        // POST: Organization/Create
        [HttpPost]
        public ActionResult Create(Organization Organization)
        {
            // test to ensure we receive correct payload.
            //Debug.WriteLine("The inputted Organization name is: ");
            //Debug.WriteLine(Organization.OrganizationName);

            //objective: add the new Organization data received into our db using our API.
            //curl -H "Content-type:application/json -d @Organization.json https://localhost:44389/api/Organizationdata/addOrganization
            string url = "addOrganization";

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


        // GET: Organization/Edit/5
        /// <summary>
        /// Recevies request to edit a Organization with Organizationid of id and redirects to edit view.
        /// </summary>
        /// <param name="id">id of Organization being edited</param>
        /// <returns></returns>
        public ActionResult Edit(int id)
        {
            //get the Organization you would like to edit using its id
            //curl https://localhost:44389/api/Organizationdata/findOrganization/{id}

            string url = "findOrganization/" + id;
            HttpResponseMessage response = httpClient.GetAsync(url).Result;

            //Debug.WriteLine("The response code is ");
            //Debug.WriteLine(response.StatusCode); // this shows if we were able to access the API when calling Organization/List method.

            OrganizationDto selectedOrganization = response.Content.ReadAsAsync<OrganizationDto>().Result;

            //Debug.WriteLine("Organization Received: ");
            //Debug.WriteLine(selectedOrganization.OrganizationName);

            return View(selectedOrganization);
        }

        // POST: Organization/Edit/5
        [HttpPost]
        public ActionResult Update(int id, Organization Organization)
        {
            //objective: add the new Organization data received into our db using our API.
            //curl -H "Content-type:application/json -d @Organization.json https://localhost:44389/api/Organizationdata/updateOrganization
            try
            {
                string url = "UpdateOrganization/" + id;

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
                return View();
            }
        }

        // GET: Organization/ConfirmDelete/5
        public ActionResult ConfirmDelete(int id)
        {
            string url = "findOrganization/" + id;
            HttpResponseMessage response = httpClient.GetAsync(url).Result;

            //Debug.WriteLine("The response code is ");
            //Debug.WriteLine(response.StatusCode); // this shows if we were able to access the API when calling Organization/List method.

            OrganizationDto selectedOrganization = response.Content.ReadAsAsync<OrganizationDto>().Result;

            //Debug.WriteLine("Organization Received: ");
            //Debug.WriteLine(selectedOrganization.OrganizationName);

            return View(selectedOrganization);
        }

        // POST: Organization/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, Organization Organization)
        {
            //objective: add the new Organization data received into our db using our API.
            //curl -H "Content-type:application/json -d @Organization.json https://localhost:44389/api/Organizationdata/deleteOrganization
            try
            {
                string url = "DeleteOrganization/" + id;

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
