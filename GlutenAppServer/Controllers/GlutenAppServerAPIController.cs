using GlutenAppServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GlutenAppServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GlutenAppServerAPIController : ControllerBase
    {
        //a variable to hold a reference to the db context!
        private GlutenFree_DB_Context context;
        //a variable that hold a reference to web hosting interface (that provide information like the folder on which the server runs etc...)
        private IWebHostEnvironment webHostEnvironment;
        //Use dependency injection to get the db context and web host into the constructor
        public GlutenAppServerAPIController(GlutenFree_DB_Context context, IWebHostEnvironment env)
        {
            this.context = context;
            this.webHostEnvironment = env;
        }
    }
}
