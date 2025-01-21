using Backend_portafolio.Entities;
namespace Backend_portafolio.Models
{
    public class ApiUserViewModel
    {
        public string name { get; set; }
        public string img { get; set; }
        public string cv { get; set; }
        public string about { get; set; }
        public string hobbies { get; set; }
        public string email { get; set; }
        public List<Bio> Bios { get; set; }
        public List<SocialNetwork> Networks { get; set; }

    }
}
