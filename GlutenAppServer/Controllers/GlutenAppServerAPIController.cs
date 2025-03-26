using GlutenAppServer.DTO;
using GlutenAppServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
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
                    StatusId = 2,
                    TypeFoodId = recipeDTO.TypeFoodID,
                    RecipeHeadLine = recipeDTO.RecipeHeadLine
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



        #region Is Image
        //this function gets a file stream and check if it is an image
        private static bool IsImage(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);

            List<string> jpg = new List<string> { "FF", "D8" };
            List<string> bmp = new List<string> { "42", "4D" };
            List<string> gif = new List<string> { "47", "49", "46" };
            List<string> png = new List<string> { "89", "50", "4E", "47", "0D", "0A", "1A", "0A" };
            List<List<string>> imgTypes = new List<List<string>> { jpg, bmp, gif, png };

            List<string> bytesIterated = new List<string>();

            for (int i = 0; i < 8; i++)
            {
                string bit = stream.ReadByte().ToString("X2");
                bytesIterated.Add(bit);

                bool isImage = imgTypes.Any(img => !img.Except(bytesIterated).Any());
                if (isImage)
                {
                    return true;
                }
            }

            return false;
        }
        #endregion

        #region Image Recipe

        [HttpPost("UploadRecipeImage")]
        public async Task<IActionResult> UploadProfileImageAsync(IFormFile file, [FromQuery] int recipeID)
        {
            //Check if who is logged in
            string? userEmail = HttpContext.Session.GetString("loggedInUser");
            if (string.IsNullOrEmpty(userEmail))
            {
                return Unauthorized("User is not logged in");
            }

            //Get model recipe class from DB with matching email. 
            Models.Recipe recipe = context.GetRecipe(recipeID,userEmail);
            //Clear the tracking of all objects to avoid double tracking
            context.ChangeTracker.Clear();

            if (recipe == null)
            {
                return Unauthorized("User is not found in the database");
            }


            //Read all files sent
            long imagesSize = 0;

            if (file.Length > 0)
            {
                //Check the file extention!
                string[] allowedExtentions = { ".png", ".jpg" };
                string extention = "";
                if (file.FileName.LastIndexOf(".") > 0)
                {
                    extention = file.FileName.Substring(file.FileName.LastIndexOf(".")).ToLower();
                }
                if (!allowedExtentions.Where(e => e == extention).Any())
                {
                    //Extention is not supported
                    return BadRequest("File sent with non supported extention");
                }

                //Build path in the web root (better to a specific folder under the web root
                string filePath = $"{this.webHostEnvironment.WebRootPath}\\recipeimages\\{recipe.RecipeId}{extention}";

                using (var stream = System.IO.File.Create(filePath))
                {
                    await file.CopyToAsync(stream);

                    if (IsImage(stream))
                    {
                        imagesSize += stream.Length;
                    }
                    else
                    {
                        //Delete the file if it is not supported!
                        System.IO.File.Delete(filePath);
                    }

                }

            }
            DTO.RecipeDTO dtoRecipe = new DTO.RecipeDTO(recipe);
            dtoRecipe.ProfileImagePath = GetRecipeImageVirtualPath(dtoRecipe.RecipeID);
            
            return Ok(dtoRecipe);
        }

        //this function check which profile image exist and return the virtual path of it.
        //if it does not exist it returns the default profile image virtual path
        private string GetRecipeImageVirtualPath(int recipeID)
        {
            string virtualPath = $"/recipeimages/{recipeID}";
            string path = $"{this.webHostEnvironment.WebRootPath}\\recipeimages\\{recipeID}.png";
            if (System.IO.File.Exists(path))
            {
                virtualPath += ".png";
            }
            else
            {
                path = $"{this.webHostEnvironment.WebRootPath}\\recipeimages\\{recipeID}.jpg";
                if (System.IO.File.Exists(path))
                {
                    virtualPath += ".jpg";
                }
                else
                {
                    virtualPath = $"/recipeimages/default.png";
                }
            }

            return virtualPath;
        }


        #endregion

        #region Image Restaurant
        [HttpPost("UploadRestaurantImage")]
        public async Task<IActionResult> UploadRestaurantImage(IFormFile file, [FromQuery] int restaurantID)
        {
            //Check if who is logged in
            string? userEmail = HttpContext.Session.GetString("loggedInUser");
            if (string.IsNullOrEmpty(userEmail))
            {
                return Unauthorized("User is not logged in");
            }

            //Get model restaurant class from DB with matching email. 
            Models.Restaurant restaurant = context.GetRestaurant(restaurantID, userEmail);
            //Clear the tracking of all objects to avoid double tracking
            context.ChangeTracker.Clear();

            if (restaurant == null)
            {
                return Unauthorized("User is not found in the database");
            }


            //Read all files sent
            long imagesSize = 0;

            if (file.Length > 0)
            {
                //Check the file extention!
                string[] allowedExtentions = { ".png", ".jpg" };
                string extention = "";
                if (file.FileName.LastIndexOf(".") > 0)
                {
                    extention = file.FileName.Substring(file.FileName.LastIndexOf(".")).ToLower();
                }
                if (!allowedExtentions.Where(e => e == extention).Any())
                {
                    //Extention is not supported
                    return BadRequest("File sent with non supported extention");
                }

                //Build path in the web root (better to a specific folder under the web root
                string filePath = $"{this.webHostEnvironment.WebRootPath}\\restaurantimages\\{restaurant.RestId}{extention}";

                using (var stream = System.IO.File.Create(filePath))
                {
                    await file.CopyToAsync(stream);

                    if (IsImage(stream))
                    {
                        imagesSize += stream.Length;
                    }
                    else
                    {
                        //Delete the file if it is not supported!
                        System.IO.File.Delete(filePath);
                    }

                }

            }
            DTO.RestaurantDTO restaurantDTO = new DTO.RestaurantDTO(restaurant);
            restaurantDTO.ProfileImagePath = GetRestaurantImageVirtualPath(restaurantDTO.RestID);

            return Ok(restaurantDTO);
        }

        //this function check which profile image exist and return the virtual path of it.
        //if it does not exist it returns the default profile image virtual path
        private string GetRestaurantImageVirtualPath(int restaurantID)
        {
            string virtualPath = $"/restaurantimages/{restaurantID}";
            string path = $"{this.webHostEnvironment.WebRootPath}\\restaurantimages\\{restaurantID}.png";
            if (System.IO.File.Exists(path))
            {
                virtualPath += ".png";
            }
            else
            {
                path = $"{this.webHostEnvironment.WebRootPath}\\restaurantimages\\{restaurantID}.jpg";
                if (System.IO.File.Exists(path))
                {
                    virtualPath += ".jpg";
                }
                else
                {
                    virtualPath = $"/restaurantimages/default.png";
                }
            }

            return virtualPath;
        }
        #endregion

        #region Image Critic
        [HttpPost("UploadCriticImage")]
        public async Task<IActionResult> UploadCriticImage(IFormFile file, [FromQuery] int criticID)
        {
            //Check if who is logged in
            string? userEmail = HttpContext.Session.GetString("loggedInUser");
            if (string.IsNullOrEmpty(userEmail))
            {
                return Unauthorized("User is not logged in");
            }

            //Get model restaurant class from DB with matching email. 
            Models.Critic critic = context.GetCritic(criticID);
            //Clear the tracking of all objects to avoid double tracking
            context.ChangeTracker.Clear();

            if (critic == null)
            {
                return Unauthorized("User is not found in the database");
            }


            //Read all files sent
            long imagesSize = 0;

            if (file.Length > 0)
            {
                //Check the file extention!
                string[] allowedExtentions = { ".png", ".jpg" };
                string extention = "";
                if (file.FileName.LastIndexOf(".") > 0)
                {
                    extention = file.FileName.Substring(file.FileName.LastIndexOf(".")).ToLower();
                }
                if (!allowedExtentions.Where(e => e == extention).Any())
                {
                    //Extention is not supported
                    return BadRequest("File sent with non supported extention");
                }

                //Build path in the web root (better to a specific folder under the web root
                string filePath = $"{this.webHostEnvironment.WebRootPath}\\criticimages\\{critic.CriticId}{extention}";

                using (var stream = System.IO.File.Create(filePath))
                {
                    await file.CopyToAsync(stream);

                    if (IsImage(stream))
                    {
                        imagesSize += stream.Length;
                    }
                    else
                    {
                        //Delete the file if it is not supported!
                        System.IO.File.Delete(filePath);
                    }

                }

            }
            DTO.CriticDTO criticDTO = new DTO.CriticDTO(critic);
            criticDTO.ProfileImagePath = GetCriticImageVirtualPath(criticDTO.CriticID);

            return Ok(criticDTO);
        }

        //this function check which profile image exist and return the virtual path of it.
        //if it does not exist it returns the default profile image virtual path
        private string GetCriticImageVirtualPath(int criticID)
        {
            string virtualPath = $"/criticimages/{criticID}";
            string path = $"{this.webHostEnvironment.WebRootPath}\\criticimages\\{criticID}.png";
            if (System.IO.File.Exists(path))
            {
                virtualPath += ".png";
            }
            else
            {
                path = $"{this.webHostEnvironment.WebRootPath}\\criticimages\\{criticID}.jpg";
                if (System.IO.File.Exists(path))
                {
                    virtualPath += ".jpg";
                }
                else
                {
                    virtualPath = $"/criticimages/default.png";
                }
            }

            return virtualPath;
        }
        #endregion



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
                    IsSterile = managerDTO.RestaurantManager.IsSterile,
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
                List<DTO.RestaurantDTO> final = new List<RestaurantDTO>();
                foreach (Restaurant r in listRestaurant)
                {
                    final.Add
                        (new RestaurantDTO(r, statusID)
                        {
                            ProfileImagePath = GetRestaurantImageVirtualPath(r.RestId)
                        }
                        );
                }
                return Ok(final);
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
                List<DTO.RestaurantDTO> final = new List<RestaurantDTO>();
                foreach (Restaurant r in listRestaurants)
                {
                    final.Add
                        (new RestaurantDTO(r,(int)r.StatusId)
                        {
                            ProfileImagePath = GetRestaurantImageVirtualPath(r.RestId)
                        }
                        );
                }

                return Ok(final);
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

                List<Models.Recipe> listRecipe = context.GetAllRecipeByStatus(statusID);
                List<DTO.RecipeDTO> final = new List<RecipeDTO>();
                foreach (Recipe r in listRecipe)
                {
                    final.Add
                        (new RecipeDTO(r, statusID)
                        {
                            ProfileImagePath = GetRecipeImageVirtualPath(r.RecipeId)
                        }
                        );
                }
                return Ok(final);
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
                List<DTO.RecipeDTO> final = new List<RecipeDTO>();
                foreach (Recipe r in listRecipe)
                {
                    final.Add
                        (new RecipeDTO(r,(int)r.StatusId)
                        {
                            ProfileImagePath = GetRecipeImageVirtualPath(r.RecipeId)
                        }
                        );
                }

                return Ok(final);
                
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
                List<DTO.RecipeDTO> final = new List<RecipeDTO>();
                foreach (Recipe r in listApprovedRecipe)
                {
                    final.Add
                        (new RecipeDTO(r)
                        {
                            ProfileImagePath = GetRecipeImageVirtualPath(r.RecipeId)
                        }
                        );
                }
                return Ok(final);
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
                List<DTO.RestaurantDTO> final = new List<RestaurantDTO>();
                foreach (Restaurant r in listApprovedRestaurant)
                {
                    final.Add
                        (new RestaurantDTO(r)
                        {
                            ProfileImagePath = GetRestaurantImageVirtualPath(r.RestId)
                        }
                        );
                }
                return Ok(final);
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
                List<DTO.RestaurantDTO> final = new List<RestaurantDTO>();
                foreach (Restaurant r in listApprovedAndTypeFood)
                {
                    final.Add
                        (new RestaurantDTO(r)
                        {
                            ProfileImagePath = GetRestaurantImageVirtualPath(r.RestId)
                        }
                        );
                }
                return Ok(final);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion



        #region Get All Critics Of A Restaurant
        [HttpGet("GetCriticForRestaurant")]
        public IActionResult GetCriticForRestaurant([FromQuery] int restaurantID)
        {
            try
            {
                List<Models.Critic> listCritics = context.GetCriticsByRestaurant(restaurantID);
                List<DTO.CriticDTO> final = new List<CriticDTO>();
                foreach (Critic c in listCritics)
                {
                    final.Add
                        (new CriticDTO(c)
                        {
                            ProfileImagePath = GetRestaurantImageVirtualPath(c.CriticId)
                        }
                        );
                }
                return Ok(final);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region Get Average Rate For A Restaurant
        [HttpGet("GetAvgRatesForRest")]
        public IActionResult GetAvgRatesForRest([FromQuery] int restID)
        {
            try
            {
                int numRest = context.GetAllRestaurants().Count;
                if (restID > numRest)
                {
                    return BadRequest("this restaurant does not exist");
                }

                double avg = context.GetAvgRestRate(restID);
                return Ok(avg);
            }
            catch(Exception ex)
            {
                return BadRequest("Something Went Wrong With the Server");
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
                List<DTO.RecipeDTO> final = new List<RecipeDTO>();
                foreach (Recipe r in listApprovedAndTypeRecipe)
                {
                    final.Add
                        (new RecipeDTO(r)
                        {
                            ProfileImagePath = GetRecipeImageVirtualPath(r.RecipeId)
                        }
                        );
                }
                return Ok(final);
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

                bool success = context.SetRecipeStatus(recipeDTO.RecipeID, 1);
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
            restaurant.IsSterile = restaurantDTO.IsSterile;
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

        #region Get All Restaurants Of A User
        [HttpGet("GetAllUserRestaurants")]
        public async Task<IActionResult> GetAllUserRestaurants([FromQuery] int userID)
        {
            try
            {
                if (userID == 0)
                    return BadRequest("No User");
                List<Models.Restaurant> listRestaurant = context.GetRestaurantByUser(userID);
                List<DTO.RestaurantDTO> final = new List<RestaurantDTO>();
                foreach (Restaurant r in listRestaurant)
                {
                    final.Add
                        (new RestaurantDTO(r)
                        {
                            ProfileImagePath = GetRestaurantImageVirtualPath(r.RestId)
                        }
                        );
                }
                return Ok(final);

                
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        #endregion

        #region Add Restaurant
        [HttpPost("AddRestaurant")]
        public IActionResult AddRestaurant([FromBody] DTO.RestaurantDTO restaurantDTO)
        {
            try 
            {
            Models.Restaurant restaurant = new Restaurant()
            {
                RestAddress = restaurantDTO.RestAddress,
                UserId = restaurantDTO.UserID,
                TypeFoodId = restaurantDTO.TypeFoodID,
                RestName = restaurantDTO.RestName,
                StatusId = 2,
                IsSterile = restaurantDTO.IsSterile,
            };
            //add restaurant
                 context.Restaurants.Add(restaurant);
                 context.SaveChanges();
                 DTO.RestaurantDTO r = new DTO.RestaurantDTO(restaurant);
                 return Ok(restaurant);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }

        }
        #endregion

        #region Add Critic
        [HttpPost("AddCritic")]
        public IActionResult AddCritic([FromBody] DTO.CriticDTO criticDTO)
        {
            try
            {
                Models.Critic critic = new Critic()
                {
                    CriticId = criticDTO.CriticID,
                    UserId = criticDTO.UserID,
                    CriticText = criticDTO.CriticText,
                    RestId = criticDTO.RestID,
                    Rate = criticDTO.Rate
                };
                context.Critics.Add(critic);
                context.SaveChanges();
                DTO.CriticDTO c = new DTO.CriticDTO(critic);
                return Ok(c);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        #endregion

        #region Close Restaurant
        [HttpPost("DeleteRestaurant")]
        public IActionResult DeleteRestaurant([FromBody] DTO.RestaurantDTO restaurantDTO)
        {
            try
            {
                string? username = HttpContext.Session.GetString("loggedInUser");
                if (username == null)
                    return Unauthorized();
                User? u = context.GetUser(username);
                if (u == null || u.TypeId != 3)
                    return Unauthorized();
                if(restaurantDTO.RestID==0)
                    return BadRequest();
                context.DeleteRestaurant(restaurantDTO.RestID);
                return Ok();
            }
            catch(Exception ex) 
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion
    }
}

