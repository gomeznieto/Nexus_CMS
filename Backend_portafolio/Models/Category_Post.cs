using Backend_portafolio.Entities;

namespace Backend_portafolio.Models
{
    public class Category_Post
	{
        public int id { get; set; }
        public int post_id { get; set; }
        public virtual CategoryViewModel Categoria { get; set; }
    }
}
