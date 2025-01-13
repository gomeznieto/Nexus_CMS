using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Backend_portafolio.Entities
{
    public class Categoria
    {
        public int id { get; set; }

        [Display(Name = "Categoria")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [StringLength(maximumLength: 50, MinimumLength = 2, ErrorMessage = "La longitud del campo {0} debe estar entre {2} y {1}")]
        [Remote(action: "VerificarExisteCategoria", controller: "Categorias")]
        public string name { get; set; }
        public int user_id { get; set; }
    }
}
