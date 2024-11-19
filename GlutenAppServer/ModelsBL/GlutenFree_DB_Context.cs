using Microsoft.EntityFrameworkCore;

namespace GlutenAppServer.Models
{
    public partial class GlutenFree_DB_Context: DbContext
    {
        public User? GetUser(string password)
        {
            return this.Users.Where(u => u.UserPass == password).FirstOrDefault();
        }
    }
}
