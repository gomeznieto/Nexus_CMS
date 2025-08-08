namespace Backend_portafolio.Models
{
    public class UserHomeLayoutSectionModel
    {
        public int? Id { get; set; }
        public int UserId { get; set; }
        public int DisplayOrder { get; set; }
        public string SectionType { get; set; }
        public int? SectionId { get; set; }
        public string Title { get; set; }
    }
}
