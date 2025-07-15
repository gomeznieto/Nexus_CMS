using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Backend_portafolio.Models
{
    public class MediaTypeViewModel
    {
        public int id { get; set; }
        [Display(Name = "Tipo de Medio")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Remote(action: "VerificarExisteMediaType", controller: "Mediatype")]
        public string name { get; set; }
        public int user_id { get; set; }
        public List<MediaTypeDefaults> MediaTypeDefaults { get; set; } = new List<MediaTypeDefaults>();

    }
}
