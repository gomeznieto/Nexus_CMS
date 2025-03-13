using System.ComponentModel.DataAnnotations;

namespace Backend_portafolio.Models
{
    public class SourceViewModel
    {
        public int id { get; set; }
        [Display(Name = "Fuente")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        //[Remote(action: "VerificarExisteCategoria", controller: "Sources")]
        public string name { get; set; }
        //[Required(ErrorMessage = "El campo {0} es requerido")]
        [Display(Name = "Icono")]
        public string icon { get; set; }

        public int user_id { get; set; }

        //FORMULARIO
        [Display(Name = "Imagen")]
        public IFormFile ImageFile { get; set; }
    }
}
