using GlutenAppServer.DTO;
using GlutenAppServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


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

        //Remember - validate the user is logged in in all the places needed

        //FIRST ETERATION

        #region Register regular
        //למשתמש רגיל בלי מנהל מסעדה
        [HttpPost("RegisterRegular")]
        public IActionResult RegisterRegular([FromBody] DTO.UsersDTO userDTO)
        {
            try
            {
                //לעשות לוג אאוט לקודמים
                HttpContext.Session.Clear();
                //check if uername exists
                if (context.GetUser(userDTO.Name) != null)
                {
                    return BadRequest("User Already Exists");
                }

                //יצירת יוזר חדש
                Models.User newUser = new User()
                {
                    UserName = userDTO.Name,
                    UserPass = userDTO.Password,
                    TypeId = userDTO.TypeID,
                    UserEmail = userDTO.UserEmail
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

                //Get model user class from DB with matching user name
                Models.User? modelsUser = context.GetUser(loginDto.UserEmail);

                //Check if user exist for this password match, if not return Access Denied (Error 403) 
                if (modelsUser == null || modelsUser.UserPass != loginDto.Password)
                {
                    return Unauthorized();
                }

                //Login suceed! now mark login in session memory!
                HttpContext.Session.SetString("loggedInUser", modelsUser.UserEmail);

                DTO.UsersDTO dtoUser = new DTO.UsersDTO(modelsUser);

                return Ok(dtoUser);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        #endregion

        #region Admin 
        //Add Fact to database - admin
        [HttpPost("AddFact")]
        public IActionResult AddFact([FromBody] DTO.InformationDTO informationDTO)
        {
            try
            {
                //validate its an admin
                string? username = HttpContext.Session.GetString("loggedInUser");
                if (username == null)
                    return Unauthorized();
                User? u = context.GetUser(username);
                if (u == null || u.TypeId != 2)
                    return Unauthorized();

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
               //add user validation

                //יצירת מתכון חדש
                Models.Recipe newRcipe = new Recipe()
                {
                    //id = identity
                    RecipeText = recipeDTO.RecipeText,
                    UserId = recipeDTO.UserID,
                    StatusId = 2
                };
                context.Recipes.Add(newRcipe);
                context.SaveChanges();
                return Ok(newRcipe);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        //
        //REGISTER MANAGER
        //

        #region Register manager restaurant
        //Check
        [HttpPost("RegisterManager")]
        public IActionResult RegisterManager([FromBody] DTO.ManagerDTO managerDTO)
        {
            try
            {
                //לעשות לוג אאוט לקודמים
                HttpContext.Session.Clear();
                if (context.GetUser(managerDTO.UserManager.Name) != null)
                {
                    return BadRequest("User Already Exists");
                }
                //יצירת יוזר חדש
                Models.User user = new User()
                {
                    UserName = managerDTO.UserManager.Name,
                    UserPass = managerDTO.UserManager.Password,
                    TypeId = managerDTO.UserManager.TypeID,
                    UserEmail = managerDTO.UserManager.UserEmail,
                    UserId = 0
                };
                if (context.IsRestExists(managerDTO.RestaurantManager.RestName))
                {
                    return BadRequest("Restaurant Already Exists");
                }
                //יצירת מסעדה חדשה
                Models.Restaurant restaurant = new Restaurant()
                {
                    RestAddress = managerDTO.RestaurantManager.RestAddress,
                    UserId = managerDTO.UserManager.UserID,
                    TypeFoodId = managerDTO.RestaurantManager.TypeFoodID,
                    RestName = managerDTO.RestaurantManager.RestName,
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
                //validate its an admin 
                string? username = HttpContext.Session.GetString("loggedInUser");
                if (username == null)
                    return Unauthorized();
                User? u = context.GetUser(username);
                if (u == null || u.TypeId != 2)
                    return Unauthorized();
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

        #region Get Recipes
        //get recipe by status
        [HttpGet("GetRecipeByStatus")]
        public IActionResult GetRecipeByStatus([FromQuery] int statusID)
        {
            try
            {
                //validate its an admin
                string? username = HttpContext.Session.GetString("loggedInUser");
                if (username == null)
                    return Unauthorized();
                User? u = context.GetUser(username);
                if (u == null || u.TypeId != 2)
                    return Unauthorized();

                List <Models.Recipe> listRecipe = context.GetAllRecipeByStatus(statusID);
                return Ok(listRecipe);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //get all recipes 
        [HttpGet("GetAllRecipes")]
        public IActionResult GetAllRecipes()
        {
            try
            {
                List<Models.Recipe> listRecipe = context.GetAllRecipes();
                return Ok(listRecipe);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region Get All Approved Recipes
        [HttpGet("GetAllApprovedRecipes")]
        public IActionResult GetAllApprovedRecipes()
        {
            try
            {
                List<Models.Recipe> listApprovedRecipe = context.GetAllApprovedRecipes();    
                return Ok(listApprovedRecipe);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region Get All Approved Restaurants
        [HttpGet("GetAllApprovedRestaurants")]
        public IActionResult GetAllApprovedRestaurants()
        {
            try
            {
                List<Models.Restaurant> listApprovedRestaurant = context.GetAllApprovedRastaurants();
                return Ok(listApprovedRestaurant);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region Get All Approved Restaurants By Food Type
        [HttpGet("GetApprovedRestaurantsByChosenFoodType")]
        public IActionResult GetApprovedRestaurantsByChosenFoodType([FromQuery] int chosenFoodType)
        {
            try
            {
                List<Models.Restaurant> listApprovedAndTypeFood = context.GetApprovedRestaurantsByChosenFoodType(chosenFoodType);
                return Ok(listApprovedAndTypeFood);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region Get All Critics Of A Restaurant
        [HttpPost("GetCriticForRestaurant")]
        public IActionResult GetCriticForRestaurant([FromBody] RestaurantDTO restaurantDTO)
        {
            try
            {
                List<Models.Critic> listCritics = context.GetCriticsByRestaurant(restaurantDTO);
                return Ok(listCritics);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region Get All Approved Recipes By Food Type
        [HttpGet("GetApprovedRecipesByChosenFoodType")]
        public IActionResult GetApprovedRecipesByChosenFoodType([FromQuery] int chosenFoodType)
        {
            try
            {
                List<Models.Recipe> listApprovedAndTypeRecipe = context.GettApprovedRecipesByChosenFoodType(chosenFoodType);
                return Ok(listApprovedAndTypeRecipe);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region Get All Fun Facts
        [HttpGet("GetAllFacts")]
        public IActionResult GetAllFacts()
        {
            try
            {
                List<Models.Information> listAllFunFacts = context.GetAllFacts();
                return Ok(listAllFunFacts);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region Change Status - Restaurants
        //change the status of the restaurant
        [HttpPost("ChangeRestStatusToApprove")]
        public IActionResult ChangeRestStatusToApprove(DTO.RestaurantDTO restaurantDTO)
        {
            try
            {
                //validate its an admin
                string? username = HttpContext.Session.GetString("loggedInUser");
                if (username == null)
                    return Unauthorized();
                User? u = context.GetUser(username);
                if (u == null || u.TypeId != 2)
                    return Unauthorized();

                bool success = context.SetStatusRest(restaurantDTO.RestID, 1);
                if (success)
                    return Ok(success);
                else
                    return BadRequest("Either resturantID not found or DB connection problem!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("ChangeRestStatusToDecline")]
        public IActionResult ChangeRestStatusToDecline(DTO.RestaurantDTO restaurantDTO)
        {
            try
            {
                //validate its an admin
                string? username = HttpContext.Session.GetString("loggedInUser");
                if (username == null)
                    return Unauthorized();
                User? u = context.GetUser(username);
                if (u == null || u.TypeId != 2)
                    return Unauthorized();

                bool success = context.SetStatusRest(restaurantDTO.RestID, 3);
                if (success)
                    return Ok(success);
                else
                    return BadRequest("Either resturantID not found or DB connection problem!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region Change Status - Recipes
        [HttpPost("ChangeRecipeStatusToApprove")]
        public IActionResult ChangeRecipeStatusToApprove(DTO.RecipeDTO recipeDTO)
        {
            try
            {
                //validate its an admin
                string? username = HttpContext.Session.GetString("loggedInUser");
                if (username == null)
                    return Unauthorized();
                User? u = context.GetUser(username);
                if (u == null || u.TypeId != 2)
                    return Unauthorized();

                bool success = context.SetRecipeStatus(recipeDTO.RecipeID,1);
                if (success)
                    return Ok(success);
                else
                    return BadRequest("Either recipeID not found or DB connection problem!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("ChangeRecipeStatusToDecline")]
        public IActionResult ChangeRecipeStatusToDecline(DTO.RecipeDTO recipeDTO)
        {
            try
            {
                //validate its an admin
                string? username = HttpContext.Session.GetString("loggedInUser");
                if (username == null)
                    return Unauthorized();
                User? u = context.GetUser(username);
                if (u == null || u.TypeId != 2)
                    return Unauthorized();

                bool success = context.SetRecipeStatus(recipeDTO.RecipeID, 3);
                if (success)
                    return Ok(success);
                else
                    return BadRequest("Either recipeID not found or DB connection problem!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region Update Profile
        [HttpPost("UpdateProfile")]
        public async Task<IActionResult> UpdateProfile([FromBody] DTO.UsersDTO userDto)
        {
            if (userDto == null)
            {
                return BadRequest("User data is null");
            }

            // חיפוש המשתמש לפי Id
            var user = await context.Users.FindAsync(userDto.UserID);

            if (user == null)
            {
                return NotFound($"User with ID {userDto.UserID} not found");
            }

            // עדכון השדות של המשתמש
            user.UserName = userDto.Name;
            user.UserPass = userDto.Password;

            try
            {
                // שמירת השינויים למסד הנתונים
                await context.SaveChangesAsync();
                return Ok(new { message = "Profile updated successfully" });
            }
            catch (Exception ex)
            {
                // טיפול בשגיאות
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred", error = ex.Message });
            }

        }

        #endregion

        #region Update Restaurant
        [HttpPost("UpdateRestauratnt")]
        public async Task<IActionResult> UpdateRestaurant([FromBody] DTO.RestaurantDTO restaurantDTO)
        {
            if (restaurantDTO == null)
            {
                return BadRequest("Restaurant is null");
            }
            //search the restaurant with the matching id
            var restaurant = await context.Restaurants.FindAsync(restaurantDTO.RestID);
            if (restaurant == null)
            {
                return NotFound("Rstaurant doesnt exist in DB");
            }
            //update the fields
            restaurant.RestName = restaurantDTO.RestName;
            restaurant.RestAddress = restaurantDTO.RestAddress;
            restaurant.UserId = restaurantDTO.UserID;
            restaurant.TypeFoodId = restaurantDTO.TypeFoodID;
            restaurant.StatusId = 2;
            //save the data to DB
            try
            {
                await context.SaveChangesAsync();
                return Ok("Restaurant updated");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred", error = ex.Message });
            }
        }
        #endregion

        #region Get All Statuses
        [HttpGet("GetAllStatuses")]
        public IActionResult GetAllStatuses()
        {
            try
            {
                List<Models.Status> listStatusesWithString = context.GetAllStatuses();
                return Ok(listStatusesWithString);
            }
            catch (Exception ex)
            { 
                return BadRequest("Problem with the connection to DB");
            }
        }
        #endregion

        #region Get All FoodTypes
        [HttpGet("GetAllFoodTypes")]
        public IActionResult GetAllFoodTypes()
        {
            try
            {
                List<Models.TypeFood> list = context.GetAllFoodType();
                return Ok(list);
            }
            catch (Exception ex) 
            {
                return BadRequest("Problem with the connection to DB");
            }
        }
        #endregion
    }
}

