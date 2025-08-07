using Microsoft.AspNetCore.Mvc.Rendering;

namespace Backend_portafolio.Models
{
    public class UserHomeLayoutFormModel
    {
        public int? Id { get; set; }
        public int UserId { get; set; }
        public int DisplayOrder { get; set; }
        public string SectionType { get; set; }
        public int? SectionRefId { get; set; }
        public string Title { get; set; }

        public List<SelectListItem> SectionOptions { get; set; } = new();
    }
}