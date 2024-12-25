namespace GlutenAppServer.DTO
{
    public class RecipeDTO
    {
        public string Recipe { get; set; } = null;
        public int RecipeID { get; set; }
        public int? UserID { get; set; }
        public int StatusID { get; set; }
        //צריך בונה
        public RecipeDTO(Models.Recipe restUser)
        {
            this.StatusID = 2;//PENDING
            this.Recipe = restUser.RecipeText;
            this.UserID = restUser.UserId;
            
            //recipe id adds auto
        }

        public RecipeDTO() { }

    }
}
