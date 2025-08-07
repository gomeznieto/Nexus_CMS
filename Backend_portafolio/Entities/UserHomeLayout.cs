using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend_portafolio.Entities
{
    public class UserHomeLayout
    {
        public int Id { get; set; }
        public int userId { get; set; }
        public string SectionType { get; set; } 
        public string SectionId { get; set; } 
        public int DisplayOrder { get; set; }
        public string Title { get; set; }
        public string SectionConfig { get; set; }
    }
}