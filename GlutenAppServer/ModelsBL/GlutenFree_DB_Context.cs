using GlutenAppServer.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace GlutenAppServer.Models
{
    public partial class GlutenFree_DB_Context: DbContext
    {
        //get user
        public User? GetUser(string userEmail)
        {
            return this.Users.Where(u => u.UserEmail == userEmail).FirstOrDefault();
        }
        // get recipe
        public Recipe? GetRecipe(int recipeID, string userEmail)
        {
            User u = GetUser(userEmail);
            return this.Recipes.Where(r=>r.RecipeId==recipeID&&r.UserId==u.UserId).FirstOrDefault();
        }
        //get restaurant
        public Restaurant? GetRestaurant(int restaurantID, string userEmail)
        {
            User u = GetUser(userEmail);
            return this.Restaurants.Where(r => r.RestId == restaurantID && r.UserId == u.UserId).FirstOrDefault();
        }

        //check if restaurant exists - true if it does


        public bool IsRestExists(string restName)
        {
            if (this.Restaurants.Where(r => r.RestName == restName).FirstOrDefault() == null)
                return false;
            return true;
        }
        //get rest by status
        public List<Restaurant>? GetAllRestByStatus(int i)
        {
            return this.Restaurants.Where(u=> u.StatusId == i).ToList();
        }
        //gets all rest
        public List<Restaurant>? GetAllRestaurants()
        {
            return this.Restaurants.ToList();
        }
        //get recipe by status
        public List<Recipe>? GetAllRecipeByStatus(int i)
        {
            return this.Recipes.Where(u=> u.StatusId==i).ToList();
        }
        //get all recipes
        public List<Recipe>? GetAllRecipes()
        {
            return this.Recipes.ToList();   
        }
        //get all approved recipes
        public List<Recipe>? GetAllApprovedRecipes()
        {
            return this.Recipes.Where(r => r.StatusId == 1).ToList();
        }
        //get all approved restaurants
        public List<Restaurant>? GetAllApprovedRastaurants()
        {
            return this.Restaurants.Where(r => r.StatusId == 1).ToList();
        }
        //get reataurants that are approved and by their food type
        public List<Restaurant>? GetApprovedRestaurantsByChosenFoodType(int chosenFoodType)
        {
            return this.Restaurants.Where(r => r.StatusId == 1&& r.TypeFoodId == chosenFoodType).ToList();   
        }
        //get recipes that are approved and by type food
        public List<Recipe>? GettApprovedRecipesByChosenFoodType(int chosenFoodType)
        {
            return this.Recipes.Where(r => r.StatusId == 1 && r.TypeFoodId == chosenFoodType).ToList();
        }
        //get all the facts in information

        public List<Information>? GetAllFacts()
        {
            return this.Information.ToList();
        }


        //get critics for a restaurant
        public List<Critic>? GetCriticsByRestaurant(int id)
        {
            List<Critic>? critics = new List<Critic>();
            critics = this.Critics.Where(c => c.RestId == id).ToList();
            return critics;
        }

        //set restaurant status - true if worked false otherwise
        public bool SetStatusRest(int restId, int statusId)
        {
            try
            {
                Restaurant? r = this.Restaurants.Where(r => r.RestId == restId).FirstOrDefault();
                if (r != null)
                {
                    r.StatusId = statusId;
                    this.Update(r);
                    this.SaveChanges();
                    return true;
                }
                return false;
            }
            catch(Exception e)
            {
                return false;
            }
            
        }

        //set recipe status - true if worked false otherwise
        public bool SetRecipeStatus(int recipeID, int statusID)
        {
            try
            {
                Recipe? r = this.Recipes.Where(r => r.RecipeId == recipeID).FirstOrDefault();
                if (r != null)
                {
                    r.StatusId = statusID;
                    this.Update(r);
                    this.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        //get all statuses to list
        public List<Status>? GetAllStatuses()
        {
            List<Status>? list = new List<Status>();
            list = this.Statuses.ToList();
            return list;
        }
        //get all food types to list 
        public List<TypeFood>? GetAllFoodType()
        {
            List<TypeFood>? list = new List<TypeFood>();
            list = this.TypeFoods.ToList();
            return list;    
        }

        public List<Restaurant?> GetRestaurantByUser(int userID)
        {
            return this.Restaurants.Where(r => r.UserId == userID).ToList();
        }
    }
}
