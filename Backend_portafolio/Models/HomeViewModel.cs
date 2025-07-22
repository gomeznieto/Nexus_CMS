using Backend_portafolio.Entities;

namespace Backend_portafolio.Models
{
    public class HomeViewModel : PostViewModel
    {
        // Quick Draft
        public List<FormatViewModel> formatList { get; set; }

        // Última actividad
        public List<PostViewModel> ultimosPosts { get; set; }

        //Listado de las secciones del home
        public List <HomeSectionModel> homeSectionList { get; set; }

        public HomeSectionModel SectionForm { get; set; } = new();

        // Bandera para saber si estamos en modo edición
        public bool IsEditHomeSectionMode { get; set; }
    }

}

