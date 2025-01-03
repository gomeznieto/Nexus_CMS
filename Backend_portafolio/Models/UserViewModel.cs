using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Backend_portafolio.Models
{
    public class UserViewModel : User
    {
        [Display(Name = "Imagen")]
        public IFormFile ImageFile { get; set; }
        [Display(Name = "Rol")]
        public string RoleName { get; set; }
        [Display(Name = "Listado de Roles")]
        public List<string> RolesName { get; set; }
        public List<Bio> Bios { get; set; }
        public List<SocialNetwork> Networks { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Remote(action: "VerficarExistePass", controller: "Users")]
        public string password { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string passwordNuevo { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Remote(action: "VerificarPassNuevo", controller: "Users", AdditionalFields = nameof(passwordNuevo))]
        public string repetirPasswordNuevo { get; set; }
    }
}
