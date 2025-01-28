using Backend_portafolio.Entities;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Backend_portafolio.Models
{
    public class RoleViewModel 
    {
        public int id { get; set; }
        [Display(Name = "Rol")]
        [Remote(action: "VerificarExisteRole", controller: "Role")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string name { get; set; }
        public List<Role> Roles { get; set; }
    }
}
