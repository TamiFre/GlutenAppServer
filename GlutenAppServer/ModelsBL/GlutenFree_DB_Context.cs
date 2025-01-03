﻿using GlutenAppServer.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace GlutenAppServer.Models
{
    public partial class GlutenFree_DB_Context: DbContext
    {
        //get user
        public User? GetUser(string userName)
        {
            return this.Users.Where(u => u.UserName == userName).FirstOrDefault();
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


        //get critics for a restaurant
        public List<Critic>? GetCriticsByRestaurant(RestaurantDTO restaurantDTO)
        {
            List<Critic>? critics = new List<Critic>();
            critics = this.Critics.Where(c => c.RestId == restaurantDTO.RestID).ToList();
            return critics;
        }

        //not working
        public void SetStatusRestToApproved(Restaurant restaurant)
        {
            var okay = this.Restaurants.Where(r => r.RestAddress == restaurant.RestAddress).FirstOrDefault();
            if (okay != null)
            {
                // Update the StatusID property of the found restaurant
                restaurant.StatusId = 1;
            }
        }
    }
}
