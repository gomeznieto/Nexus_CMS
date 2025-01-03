using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Backend_portafolio.Models
{
    public class User
    {
        public int id { get; set; }
        [Display(Name = "Nombre")]
        public string name { get; set; }
        public  int role { get; set; }
        [Display(Name = "Imagen de Perfil")]
        public string img { get; set; }
        [Display(Name = "Link Curriculum")]
        public  string cv { get; set; }
        [Display(Name = "Sobre mí")]
        public  string about { get; set; }
        [Display(Name = "Hobbies")]
        public string hobbies { get; set; }
        [Display(Name = "E-Mail")]
        [Remote(action: "VerificarExisteEmail", controller: "Users")]
        public string email { get; set; }
        public string emailNormalizado { get; set; }
        public string passwordHash { get; set; }

    }
}
