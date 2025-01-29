using Backend_portafolio.Entities;
using System.ComponentModel.DataAnnotations;

namespace Backend_portafolio.Models
{
    public class BioViewModel
    {
        public int id { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string work { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int year { get; set; }
        public int user_id { get; set; }
        public List<Bio> Bios { get; set; }
    }
}
