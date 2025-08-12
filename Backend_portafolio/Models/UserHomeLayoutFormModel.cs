using Backend_portafolio.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Backend_portafolio.Models
{
    public class UserHomeLayoutFormModel
    {
        public List<UserHomeLayoutSectionModel> Sections { get; set; }
        public List<SelectListItem> SectionOptions { get; set; } = new();
    }
}