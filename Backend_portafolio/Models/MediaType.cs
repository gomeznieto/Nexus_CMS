using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Backend_portafolio.Models
{
    public class MediaType
    {
        public int id { get; set; }
        [Display(Name = "Tipo de Medio")]
		[Required(ErrorMessage = "El campo {0} es requerido")]
		[Remote(action: "VerificarExisteCategoria", controller: "Mediatype")]
		public string name { get; set; }
    }
}
