using System;
using Microsoft.AspNetCore.Mvc;

namespace GitCIWeb.Controllers
{
    [Route("api/Test")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [Route("GetDate")]
        public DateTime GetDate()
        {
            return DateTime.Now;
        }
    }
}
