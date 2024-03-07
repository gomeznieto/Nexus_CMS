using Microsoft.AspNetCore.Mvc.Rendering;

namespace Backend_portafolio.Models
{
	public class PostViewModel : Post
	{
		public IEnumerable<SelectListItem> categories { get; set;}
		public IEnumerable<SelectListItem> formats { get; set; }
		public IEnumerable<SelectListItem> mediaTypes { get; set; }
        public string mediaListString { get; set; }
		public IEnumerable<MediaForm> mediaForm { get; set; }
    }
}
