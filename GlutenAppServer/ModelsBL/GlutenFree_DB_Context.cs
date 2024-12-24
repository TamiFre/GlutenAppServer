using Microsoft.EntityFrameworkCore;

namespace GlutenAppServer.Models
{
    public partial class GlutenFree_DB_Context: DbContext
    {
        //get user
        public User? GetUser(string password)
        {
            return this.Users.Where(u => u.UserPass == password).FirstOrDefault();
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
       
    }
}
