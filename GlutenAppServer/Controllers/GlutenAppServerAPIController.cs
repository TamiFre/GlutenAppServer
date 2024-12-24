using GlutenAppServer.DTO;
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

        #region register regular
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
        #endregion

        #region Login
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
        #endregion

        #region admin 
        //Add Fact to database - admin
        [HttpPost("AddFact")]
        public IActionResult AddFact([FromBody] DTO.InformationDTO informationDTO)
        {
            try
            {
                string? userName = HttpContext.Session.GetString("loggedInUser");
                if (string.IsNullOrEmpty(userName))
                {
                    return Unauthorized("User is not logged in");
                }

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
        #endregion

        #region Add Recipe

        //Check

        //Add Recipe to DataBase
        [HttpPost("AddRecipe")]
        public IActionResult AddRecipe([FromBody] DTO.RecipeDTO recipeDTO)
        {
            try
            {
                //יצירת מתכון חדש
                Models.Recipe newRcipe = new Recipe()
                {
                    //id = identity
                    RecipeText = recipeDTO.Recipe,
                    UserId = recipeDTO.UserID,
                    StatusId =2
                };
                context.Recipes.Add(newRcipe);
                context.SaveChanges();
                return Ok(newRcipe);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        //
        //REGISTER MANAGER
        //

        #region register manager restaurant
        //Check
        [HttpPost("RegisterManager")]
        public IActionResult RegisterManager([FromBody] DTO.ManagerDTO managerDTO)
        {
            try
            {
                //לעשות לוג אאוט לקודמים
                HttpContext.Session.Clear();

                //יצירת יוזר חדש
                Models.User user = new User()
                {
                    UserName = managerDTO.UserManager.Name,
                    UserPass = managerDTO.UserManager.Password,
                    TypeId = managerDTO.UserManager.TypeID,
                    UserId = 0
                };

                //יצירת מסעדה חדשה
                Models.Restaurant restaurant = new Restaurant()
                {
                    RestAddress = managerDTO.RestaurantManager.RestAddress,
                    UserId = managerDTO.UserManager.UserID,
                    TypeFoodId = managerDTO.RestaurantManager.TypeFoodID,
                    StatusId = 2,
                };

                //הוספת היוזר
                context.Users.Add(user);
                context.SaveChanges();

                //הוספת מסעדה
                restaurant.UserId = user.UserId;
                context.Restaurants.Add(restaurant);
                context.SaveChanges();

                DTO.UsersDTO u = new DTO.UsersDTO(user);
                DTO.RestaurantDTO r = new DTO.RestaurantDTO(restaurant);

                DTO.ManagerDTO dtoManager = new DTO.ManagerDTO(u, r);
                return Ok(dtoManager);
            }

            catch (Exception ex)
            {
                //אם לא הצלחתי לרשום
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region Get restaurants
        //get restaurants by status
        [HttpGet("GetRestByStatus")]
        public IActionResult GetRestByStatus([FromQuery] int statusID)
        {
            try
            {
                List<Models.Restaurant> listRestaurant = context.GetAllRestByStatus(statusID);
                return Ok(listRestaurant);
            }
            catch (Exception ex) 
            {
                return BadRequest(ex.Message);  
            } 
            
        }

        //get all restaurants
        [HttpGet("GetAllRests")]
        public IActionResult GetAllRest()
        {
            try
            {
                List<Models.Restaurant> listRestaurants = context.GetAllRestaurants();
                return Ok(listRestaurants);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }
        #endregion



    }
}
