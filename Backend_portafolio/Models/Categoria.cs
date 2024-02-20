using System.ComponentModel.DataAnnotations;

namespace Backend_portafolio.Models
{
	public class Categoria
	{
        public int id { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [Display(Name = "Categoria")]
        [StringLength(maximumLength: 50, MinimumLength = 2, ErrorMessage = "La longitud del campo {0} debe estar entre {2} y {1}")]
        public string name { get; set; }
    }
}
