using Backend_portafolio.Entities;

namespace Backend_portafolio.Models
{
    public class UserDataListViewModel
    {
        // Edit User Role
        public int id { get; set; }
        public string username { get; set; }
        public string name { get; set; }
        public string  email { get; set; }
        public bool recoveryPass { get; set; }
        public bool recoveryApikey { get; set; }
        public int role { get; set; }

        // List of Users
        public List<UserViewModel> totalUserList{ get; set; }
        public List<UserViewModel> usersList{ get; set; }
        public int countUsers { get; set; }
        public List<Role> roles { get; set; }
    }
}
