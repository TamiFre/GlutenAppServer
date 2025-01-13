    namespace GlutenAppServer.DTO
{
    public class UsersDTO
    {
        public string Name { set; get; } = null;
        public string Password { set; get; } = null;
        public int? TypeID { set; get; }
        public int? UserID { set; get; }
        public string UserEmail { set; get; }



        //פעולה בונה לDTO

        public UsersDTO() { }
        public UsersDTO (Models.User modelUser)
        {
            this.Name = modelUser.UserName;
            this.Password = modelUser.UserPass;
            this.TypeID = modelUser.TypeId;
            this.UserID=modelUser.UserId;
            this.UserEmail = modelUser.UserEmail;
        }




    }
}
