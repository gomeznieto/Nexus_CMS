using Backend_portafolio.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Backend_portafolio.Models
{
    public class PostViewModel
	{
        // ENTIDAD
        public int id { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength: 40, MinimumLength = 2, ErrorMessage = "El titulo debe tener entre {2} y {1} letras")]
        [Display(Name = "Titulo")]
        public string title { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Display(Name = "Descripción")]
        public string description { get; set; }
        [Display(Name = "Imagen principal")]
        public string cover { get; set; }
        [Display(Name = "Formato")]
        public int user_id { get; set; }
        public int format_id { get; set; }
        public DateTime created_at { get; set; }
        public DateTime modify_at { get; set; }
        public bool draft { get; set; }
        [Display(Name = "Imagen")]
        public IFormFile ImageFile { get; set; }

        // STRINGS
        public string userName { get; set; }
        public string format { get; set; }

        // LISTAS DE LA ENTRADA
        public IEnumerable<Media> mediaList { get; set; }
        public IEnumerable<Link> linkList { get; set; }
        public IEnumerable<Category_Post> categoryList { get; set; }
        public IEnumerable<SelectListItem> categories { get; set;}
		public IEnumerable<SelectListItem> formats { get; set; }
		public IEnumerable<SelectListItem> mediaTypes { get; set; }
		public IEnumerable<SelectListItem> sources { get; set; }
        public string mediaListString { get; set; }
        public string sourceListString { get; set; }
        public string categoryListString { get; set; }
    }
}
