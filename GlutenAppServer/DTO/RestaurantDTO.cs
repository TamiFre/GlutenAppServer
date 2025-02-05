namespace GlutenAppServer.DTO
{
    public class RestaurantDTO
    {
        public int RestID { get; set; } 
        public int? UserID { get;  set; }
        public int? TypeFoodID { get;  set; }
        public string RestAddress { get; set; }
        public string RestName {  get; set; }
        public int StatusID { get; set; }
        public string ProfileImagePath { get; set; } = "";
        //צריך בונה

        public RestaurantDTO(Models.Restaurant restUser)
        {
            this.StatusID = 2;//PENDING
            this.RestID = restUser.RestId;
            this.RestAddress = restUser.RestAddress;
            this.UserID = restUser.UserId;
            this.TypeFoodID = restUser.TypeFoodId;
            this.RestName = restUser.RestName;
            //rest id adds auto
        }

        public RestaurantDTO() { }

        //public Models.Restaurant GetModels()
        //{
        //    Models.Restaurant modelsUser = new Models.Restaurant()
        //    {
        //        StatusId = this.StatusID,
        //        RestId=this.RestID,
        //        RestAddress=this.RestAddress,
        //        UserId = this.UserID, 
        //        TypeFoodId = this.TypeFoodID,
        //        RestName=this.RestName

        //    };

        //    return modelsUser;
        //}
    }
}
