using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Backend_portafolio.Models
{
    public class Source
    {
        public int id { get; set; }
        [Display(Name = "Fuente")]
		[Required(ErrorMessage = "El campo {0} es requerido")]
		//[Remote(action: "VerificarExisteCategoria", controller: "Sources")]
		public string name { get; set; }
        [Display(Name = "Icono")]
		[Required(ErrorMessage = "El campo {0} es requerido")]
		public string icon { get; set; }

        public int user_id { get; set; }
    }
}
