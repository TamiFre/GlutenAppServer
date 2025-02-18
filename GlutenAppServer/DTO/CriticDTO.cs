

namespace GlutenAppServer.DTO
{
    public class CriticDTO
    {
        public int CriticID { get; set; }
        public string CriticText { get; set; } = null;
        public int? UserID { get; set; }    
        public int? RestID { get; set; }
        public string ProfileImagePath { get; set; } = "";
        public CriticDTO() { }

        public CriticDTO(Models.Critic modelCritic)
        {
            this.CriticText = modelCritic.CriticText;
            this.CriticID = modelCritic.CriticId;
            this.UserID = modelCritic.UserId;
            this.RestID = modelCritic.RestId;

        }
    }
}
