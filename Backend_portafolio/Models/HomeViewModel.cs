using Backend_portafolio.Entities;

namespace Backend_portafolio.Models
{
    public class HomeViewModel : PostViewModel
    {
        // Quick Draft
        public List<FormatViewModel> formatList { get; set; }

        // Última actividad
        public List<PostViewModel> ultimosPosts { get; set; }

    }
}
