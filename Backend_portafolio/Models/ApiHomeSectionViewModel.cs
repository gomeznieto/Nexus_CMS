namespace Backend_portafolio.Models
{
    public class ApiHomeSectionViewModel
    {
        public int HomeSectionId { get; set; }
        public string HomeSectionName { get; set; }
        public int SectionOrder { get; set; }
        public List<ApiHomeSectionPostViewModel> Posts { get; set; }
    }
}
