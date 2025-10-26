
using System;
using System.Collections.Generic;
using System.Linq;

namespace RegisterAndLoginApp.Models
{
    
    public class UserCollection : IUserManager
    {
        private List<UserModel> _users;

        public UserCollection()
        {
            _users = new List<UserModel>();
            GenerateUserData(); 
        }

       
        public int AddUser(UserModel user)
        {
           
            user.Id = _users.Count + 1;
            _users.Add(user);              
            return user.Id;
        }

       
        public int CheckCredentials(string username, string password)
        {
          
            var uname = (username ?? string.Empty).Trim();
            var pwd = password ?? string.Empty;

            var match = _users.FirstOrDefault(u =>
                string.Equals(u.Username?.Trim(), uname, StringComparison.OrdinalIgnoreCase));

            if (match != null && match.VerifyPassword(pwd))
            {
                return match.Id; 
            }

            return 0; 
        }

       
        public void DeleteUser(UserModel user)
        {
            _users.Remove(user);
        }

       
        public List<UserModel> GetAllUsers()
        {
            return _users;
        }

      
        public UserModel? GetUserById(int id)
        {
            return _users.FirstOrDefault(u => u.Id == id);
        }

   
        public void UpdateUser(UserModel user)
        {
            var index = _users.FindIndex(u => u.Id == user.Id);
            if (index >= 0)
            {
                _users[index] = user;
            }
        }

      
        private void GenerateUserData()
        {
            var user1 = new UserModel { Username = "Harry", Groups = "Admin" };
            user1.SetPassword("prince");
            AddUser(user1);

            var user2 = new UserModel { Username = "Megan", Groups = "Admin, User" };
            user2.SetPassword("princess");
            AddUser(user2);
        }
    }
}
