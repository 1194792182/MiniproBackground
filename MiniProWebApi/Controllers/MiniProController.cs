using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MiniProWebApi.Controllers
{
    [Route("MiniPro")]
    [ApiController]
    public class MiniProController : ControllerBase
    {
        [HttpPost]
        [Route("GetThirdSessionId")]
        public ActionResult GetThirdSessionId()
        {
            return Ok(new { testStr = "test" });
        }
    }
}