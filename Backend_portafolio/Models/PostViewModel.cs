using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Backend_portafolio.Models
{
	public class PostViewModel : Post
	{
		public IEnumerable<SelectListItem> categories { get; set;}
		public IEnumerable<SelectListItem> formats { get; set; }
	}
}
