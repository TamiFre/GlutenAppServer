namespace GlutenAppServer.DTO
{
    public class RecipeDTO
    {
        public string RecipeText { get; set; } = null;
        public int RecipeID { get; set; }
        public int? UserID { get; set; }
        public int StatusID { get; set; }
        public int TypeFoodID { get; set; }


        //צריך בונה
        public RecipeDTO(Models.Recipe restUser)
        {
            this.StatusID = 2;//PENDING
            this.RecipeText = restUser.RecipeText;
            this.UserID = restUser.UserId;
            this.TypeFoodID = restUser.TypeFoodId;
            //recipe id adds auto
        }

        public RecipeDTO() { }

    }
}
