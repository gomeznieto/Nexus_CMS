using Backend_portafolio.Entities;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Backend_portafolio.Models
{
    public class UserViewModel
    {
        public int id { get; set; }
        [Display(Name = "Nombre")]
        public string name { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]

        public string username { get; set; }
        public string usernameNormalizado { get; set; }
        public int role { get; set; }
        [Display(Name = "Imagen de Perfil")]
        public string img { get; set; }
        [Display(Name = "Link Curriculum")]
        public string cv { get; set; }
        [Display(Name = "Sobre mí")]
        public string about { get; set; }
        [Display(Name = "Hobbies")]
        public string hobbies { get; set; }
        [Display(Name = "Email")]
        [Remote(action: "VerificarExisteEmail", controller: "Users")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string email { get; set; }
        public string emailNormalizado { get; set; }
        public string passwordHash { get; set; }
        [Display(Name = "ApiKey")]
        public string apiKey { get; set; }
        [Display(Name = "Imagen")]
        public IFormFile ImageFile { get; set; }
        [Display(Name = "Rol")]
        public string RoleName { get; set; }
        [Display(Name = "Listado de Roles")]
        public List<Role> RolesName { get; set; }
        public List<Bio> Bios { get; set; }
        public List<SocialNetwork> Networks { get; set; }
    }
}
