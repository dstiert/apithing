using System;
using apithing.v1.Model;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace apithing.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IDatabase _redis;
        private readonly Random _random = new Random();

        public ValuesController(IDatabase redis)
        {
            _redis = redis;
        }

        [HttpGet("{id}", Name = "GetValue")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public ActionResult<StoredValue> Get(int id)
        {
            var result = _redis.StringGet(id.ToString());

            if(result.IsNull)
            {
                return NotFound();
            }

            return new StoredValue { Value = result.ToString() };
        }

        [HttpPost]
        [ProducesResponseType(201)]
        public ActionResult<StoredValue> Post([FromBody] StoredValue value)
        {
            var id = _random.Next();
            _redis.StringSet(id.ToString(), value.Value);
            return CreatedAtRoute("GetValue", new { id }, value);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        public ActionResult Put(int id, [FromBody] StoredValue value)
        {
            _redis.StringSet(id.ToString(), value.Value);
            return NoContent();
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public ActionResult Delete(int id)
        {
            if(_redis.KeyDelete(id.ToString()))
            {
                return NoContent();
            }
            else
            {
                return NotFound();
            }
            
        }
    }
}
