using Backend_portafolio.Entities;
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
        public List<Role> RolesName { get; set; }
        public List<Bio> Bios { get; set; }
        public List<SocialNetwork> Networks { get; set; }
    }
}
