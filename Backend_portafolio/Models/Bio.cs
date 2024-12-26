using System.ComponentModel.DataAnnotations;

namespace Backend_portafolio.Models
{
	public class Bio
	{
        public int id { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string  work { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int year { get; set; }
        public int user_id { get; set; }
    }
}
