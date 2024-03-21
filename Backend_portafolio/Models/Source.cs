using System.ComponentModel.DataAnnotations;

namespace Backend_portafolio.Models
{
    public class Source
    {
        public int id { get; set; }
        [Display(Name = "Fuente")]
        public string name { get; set; }
        [Display(Name = "Icono")]
        public string icon { get; set; }
    }
}
