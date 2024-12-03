using GlutenAppServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace GlutenAppServer.Controllers
{
    [Route("api")]
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


        //FIRST ETERATION


        //למשתמש רגיל בלי מנהל מסעדה
        [HttpPost("RegisterRegular")]
        public IActionResult RegisterRegular([FromBody] DTO.UsersDTO userDTO)
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





        //לוגין 
        [HttpPost("Login")]
        public IActionResult Login([FromBody] DTO.UsersDTO loginDto)
        {
            try
            {
                HttpContext.Session.Clear(); //Logout any previous login attempt

                //Get model user class from DB with matching password
                Models.User? modelsUser = context.GetUser(loginDto.Password);

                //Check if user exist for this password match, if not return Access Denied (Error 403) 
                if (modelsUser == null || modelsUser.UserPass != loginDto.Password)
                {
                    return Unauthorized();
                }

                //Login suceed! now mark login in session memory!
                HttpContext.Session.SetString("loggedInUser", modelsUser.UserName);

                DTO.UsersDTO dtoUser = new DTO.UsersDTO(modelsUser);

                return Ok(dtoUser);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }


        //Add Fact to database
        [HttpPost("AddFact")]
        public IActionResult AddFact([FromBody] DTO.InformationDTO informationDTO)
        {
            try
            {
                //יצירת אינפו חדש
                Models.Information newInfo = new Information()
                {
                    //the id will be identity
                    InfoText = informationDTO.InfoText
                };
                context.Information.Add(newInfo);
                context.SaveChanges();
                return Ok(newInfo);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        //
        //NEXT ETERATION - SECOND ONE
        //

        [HttpPost("RegisterManager")]
        public IActionResult RegisterManager([FromBody] DTO.UsersDTO userDTO)
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

                //הוספת המסעדה במצב פנדינג
                //
              ///
              ////
                //ASK OFER



                DTO.UsersDTO dtoUser = new DTO.UsersDTO(newUser);
                return Ok(dtoUser);
            }

            catch (Exception ex)
            {
                //אם לא הצלחתי לרשום
                return BadRequest(ex.Message);
            }
        }


        



    }
}
