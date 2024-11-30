using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Backend_portafolio.Models
{
    public class Role
    {
        public int id { get; set; }
        [Display(Name = "Rol")]
        [Remote(action: "VerificarExisteFormato", controller: "Rol")]
        public string name { get; set; }
    }
}
