namespace Backend_portafolio.Models
{
    public class HomeSectionPostModel
    {
        public int? Id { get; set; }
        public int HomeSectionId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int PostId { get; set; }
        public int Order { get; set; }
    }
}
