namespace GlutenAppServer.DTO
{
    public class ManagerDTO
    {
       public Models.User UserManager { get; set; }
       public Models.Restaurant RestaurantManager { get; set; } 

        public ManagerDTO(Models.User user, Models.Restaurant rest) 
        {
            UserManager = user;
            RestaurantManager = rest;
        }
    }
}
