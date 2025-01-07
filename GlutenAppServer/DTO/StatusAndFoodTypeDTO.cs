using GlutenAppServer.Models;

namespace GlutenAppServer.DTO
{
    public class StatusAndFoodTypeDTO
    {
        public List<Status> Statuses { get; set; }
        public List<TypeFood> FoodTypes { get; set; }

    }
}
