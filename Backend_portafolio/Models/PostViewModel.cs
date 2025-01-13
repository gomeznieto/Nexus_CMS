using Backend_portafolio.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Backend_portafolio.Models
{
    public class PostViewModel : Post
	{
		public IEnumerable<SelectListItem> categories { get; set;}
		public IEnumerable<SelectListItem> formats { get; set; }
		public IEnumerable<SelectListItem> mediaTypes { get; set; }
		public IEnumerable<SelectListItem> sources { get; set; }
        public string mediaListString { get; set; }
        public string sourceListString { get; set; }
        public string categoryListString { get; set; }
    }
}
