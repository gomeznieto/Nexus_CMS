using Backend_portafolio.Entities;
using System.ComponentModel.DataAnnotations;

namespace Backend_portafolio.Models
{
    public class SocialNetworkViewModel
    {
        public int id { get; set; }
        [Display(Name = "Nombre de la Red Social")]
        public string name { get; set; }
        [Display(Name = "Dirección (url) de la Red Social")]
        public string url { get; set; }
        [Display(Name = "Nombre de Usuario de la Red Social")]
        public string username { get; set; }
        [Display(Name = "Icóno de la Red Social")]
        public string icon { get; set; }
        public int user_id { get; set; }
        public List<SocialNetwork> Networks { get; set; }
    }
}
