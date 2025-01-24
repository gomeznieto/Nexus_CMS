using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Backend_portafolio.Entities
{
    public class Role
    {
        public int id { get; set; }
        [Display(Name = "Rol")]
        [Remote(action: "VerificarExisteRole", controller: "Role")]
        public string name { get; set; }
    }
}
