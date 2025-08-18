using Backend_portafolio.Entities;

namespace Backend_portafolio.Models
{
    public class HomeSectionModel
    {
        public int? Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? MaxItems { get; set; }
    }
}
