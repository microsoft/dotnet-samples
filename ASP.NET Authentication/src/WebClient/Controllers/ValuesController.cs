using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace WebClient.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        // GET api/values
        // Because this API has two separate [Authorize] attributes one it, both have to
        // be satisfied. Only users who are in offices under 400 _and_ who are administrators may
        // access this API.
        // If, instead, the two conditions were included in a single [Authorize] attribute, then
        // any user who met either requirement would be able to call the API.
        [HttpGet]
        [Authorize(Roles = "Administrator")]
        [Authorize(Policy = "OfficeNumberUnder400")]
        public IEnumerable<string> Get()
        {
            return new string[] { "Important Info for people in low offices #1", "Important Info for people in low offices #2" };
        }

        // GET api/values/5
        // This API is only accessible to users in the 'Manager' role
        [HttpGet("{id}")]
        [Authorize(Roles = "Manager")]
        public string Get(int id)
        {
            return "Managerial Information";
        }
    }
}
