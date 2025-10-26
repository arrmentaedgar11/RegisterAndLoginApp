
using System.Collections.Generic;

namespace RegisterAndLoginApp.Models
{
    public interface IUserManager
    {
        // Read
        List<UserModel> GetAllUsers();    
        UserModel? GetUserById(int id);     

        // Create
        int AddUser(UserModel user);       

        // Update
        void UpdateUser(UserModel user);    

        // Delete
        void DeleteUser(UserModel user);   

        // Auth
        int CheckCredentials(string username, string password); 
    }
}
