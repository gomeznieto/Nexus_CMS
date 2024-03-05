using System.ComponentModel.DataAnnotations;

namespace Backend_portafolio.Models
{
    public class MediaType
    {
        public int id { get; set; }
        [Display(Name = "Tipo de Medio")]
        [Required]
        public string name { get; set; }
    }
}
