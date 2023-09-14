using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiSecurity.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        // GET: api/<UsersController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<UsersController>/5
        [HttpGet("{id}")]
        //[Authorize(Policy = "MustHaveRules")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<UsersController>
        [HttpPost]
        [Authorize(Policy = "MusthaveRuleAdmin")]
        public string Post([FromBody] string value)
        {
            return "Only Admin is Create";
        }

        // PUT api/<UsersController>/5
        [HttpPut("{id}")]
        [Authorize(Policy = "MusthaveRuleUser")]
        public string Put(int id, [FromBody] string value)
        {
            return "Only User is Update";
        }

        // DELETE api/<UsersController>/5
        [HttpDelete("{id}")]
        [Authorize(Policy = "MusthaveRuleAdmin")]
        public string Delete(int id)
        {
            return "Only Admin is Delete";
        }
    }
}
