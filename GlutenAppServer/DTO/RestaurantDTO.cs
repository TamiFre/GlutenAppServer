namespace GlutenAppServer.DTO
{
    public class RestaurantDTO
    {
        public int? RestID { get; set; } 
        public int? UserID { get;  set; }
        public int? TypeFoodID { get;  set; }
        public string RestAddress { get; set; }
        //צריך בונה

        public RestaurantDTO(Models.Restaurant restUser)
        {
            this.RestAddress = restUser.RestAddress;
            this.UserID = restUser.UserId;
            this.TypeFoodID = restUser.TypeFoodId;
            //rest id adds auto
        }

    }
}
