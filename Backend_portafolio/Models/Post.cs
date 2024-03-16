using System.ComponentModel.DataAnnotations;

namespace Backend_portafolio.Models
{
	public class Post
	{
		public int id { get; set; }
		[Required(ErrorMessage = "El campo {0} es requerido")]
		[StringLength(maximumLength:40, MinimumLength = 2, ErrorMessage = "El titulo debe tener entre {2} y {1} letras")]
		[Display(Name = "Titulo")]
		public string title { get; set; }
		[Required(ErrorMessage = "El campo {0} es requerido")]
		[Display(Name = "Descripción")]
		public string description { get; set; }
		[Required(ErrorMessage = "El campo {0} es requerido")]
		[Display(Name = "Imagen principal")]
		public string cover { get; set; }
		[Display(Name = "Categoria")]
		public int category_id { get; set; }
		[Display(Name = "Formato")]
		public int format_id { get; set; }
		[Required(ErrorMessage = "El campo {0} es requerido")]
		public int user_id { get; set; }
		[Required(ErrorMessage = "El campo {0} es requerido")]
		[Display(Name = "Fecha de creación")]
		public DateTime created_at { get; set; }
		public DateTime modify_at { get; set; }
        public  bool draft { get; set; }
        public string format { get; set; }
		public string category { get; set; }
		public string userName { get; set; }
        public IEnumerable<Media> mediaList { get; set; }

    }
}
