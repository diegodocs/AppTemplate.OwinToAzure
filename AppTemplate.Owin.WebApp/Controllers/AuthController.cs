using System.Collections.Generic;
using System.Web.Http;

namespace AppTemplate.OwinWebApp.Controllers
{
    [Authorize]
    public class AuthController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "role1", "role2" };
        }
    }
}