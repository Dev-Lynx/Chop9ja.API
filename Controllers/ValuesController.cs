using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chop9ja.API.Extensions.UnityExtensions;
using Chop9ja.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Logging;
using Unity;

namespace Chop9ja.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AutoBuild]
    public class ValuesController : ControllerBase
    {
        #region Properties
        [DeepDependency]
        IAuthService Auth { get; }
        [DeepDependency]
        ILogger Logger { get; }
        #endregion

        public ValuesController()
        {
            Core.Log.Debug("Values Controller Resolved");
            
        }

        /// <summary>
        /// Retrieve the employee by their ID.
        /// </summary>
        /// <returns>A string status</returns>
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            if (Auth != null)
                Auth.SayHello();
            else Core.Log.Debug("Auth is null");

            if (Logger != null) Logger.LogDebug("So here we are...");
            else Core.Log.Debug("Logger is null");

            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
