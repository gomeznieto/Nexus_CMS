using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Backend_portafolio.Models
{
    public class PasswordViewModel
    {
        public int id { get; set; }
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
