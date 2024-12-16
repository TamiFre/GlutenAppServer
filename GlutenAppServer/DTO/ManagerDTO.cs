namespace GlutenAppServer.DTO
{
    public class ManagerDTO
    {
       public UsersDTO UserManager { get; set; }
       public RestaurantDTO RestaurantManager { get; set; } 

        public ManagerDTO(UsersDTO user, RestaurantDTO rest) 
        {
            UserManager = user;
            RestaurantManager = rest;
        }

        public ManagerDTO() { }
    }
}
