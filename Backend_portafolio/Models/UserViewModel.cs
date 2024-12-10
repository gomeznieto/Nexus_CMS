using System.ComponentModel.DataAnnotations;

namespace Backend_portafolio.Models
{
    public class UserViewModel
    {
        [Display(Name = "Imagen de Perfil")]
        public string Image { get; set; }
        [Display(Name = "Nombre")]
        public string Name { get; set; }
        [Display(Name = "E-Mail")]
        public string Email { get; set; }
        [Display(Name = "Rol")]
        public string RoleName { get; set; }
        [Display(Name = "Listado de Roles")]
        public List<string> RolesName { get; set; }
        [Display(Name = "Link Curriculum")]
        public string CV { get; set; }
        [Display(Name = "Sobre mí")]
        public string About { get; set; }
        [Display(Name = "Hobbies")]
        public string Hobbies { get; set; }
        public List<Bio> Bios { get; set; }
        public List<SocialNetwork> Networks { get; set; }
        public string password { get; set; }
        public string passwordNuevo { get; set; }
    }
}
