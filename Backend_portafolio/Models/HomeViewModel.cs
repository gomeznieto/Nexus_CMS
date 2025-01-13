using Backend_portafolio.Entities;

namespace Backend_portafolio.Models
{
    public class HomeViewModel : Post
    {
        // Quick Draft
        public List<Format> formatList { get; set; }

        // Última actividad
        public List<Post> ultimosPosts { get; set; }

    }
}
