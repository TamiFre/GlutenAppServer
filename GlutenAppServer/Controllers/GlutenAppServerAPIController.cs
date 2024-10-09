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

        //למשתמש רגיל בלי מנהל מסעדה
        [HttpPost("Register")]
        public IActionResult Register([FromBody] DTO.UsersDTO userDTO)
        {
            try
            {
                //לעשות לוג אאוט לקודמים
                HttpContext.Session.Clear();

                //יצירת יוזר חדש
                Models.User newUser = new User()
                {
                    UserName = userDTO.Name,
                    UserPass = userDTO.Password,
                    TypeId = userDTO.TypeID
                };

                //הוספת היוזר
                context.Users.Add(newUser);
                context.SaveChanges();

                DTO.UsersDTO dtoUser = new DTO.UsersDTO(newUser);
                return Ok(dtoUser);
            }

            catch (Exception ex)
            {
                //אם לא הצלחתי לרשום
                return BadRequest(ex.Message);
            }

        }

        //פעולת לוגין לכל המשתמשים
        //[HttpPost("Login")]
        //public IActionResult Login([FromBody] DTO.UsersDTO userDTO)
        //{
        //    try
        //    {
        //        //להעיף קודמים
        //        HttpContext.Session.Clear();


        //    }
        //    catch (Exception ex) 
        //    {

        //    }
        //}

    }
}
