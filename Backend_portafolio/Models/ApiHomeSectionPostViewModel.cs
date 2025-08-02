namespace Backend_portafolio.Models
{
    public class ApiHomeSectionPostViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Cover { get; set; }
        public string Slug { get; set; }
        public List<Category_Post> CategoriesList { get; set; }
        public string Format { get; set; }

    }
}
