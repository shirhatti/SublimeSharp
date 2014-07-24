using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Diagnostics;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        [HttpPost]
        public List<List<string>> Complete([FromBody] PostModel post)
        {
            if (post == null)
            {
                return null;
            }
            var l =SimpleCompletionService.GetCompletions(post.position, post.document);
            return l;
        }
    }
}
