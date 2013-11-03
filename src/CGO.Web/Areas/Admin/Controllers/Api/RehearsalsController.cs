using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using CGO.Web.Areas.Admin.Models.Api;

namespace CGO.Web.Areas.Admin.Controllers.Api
{
    public class RehearsalsController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public HttpResponseMessage Post([FromBody]RehearsalApiModel value)
        {
            return new HttpResponseMessage(HttpStatusCode.Created);
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}