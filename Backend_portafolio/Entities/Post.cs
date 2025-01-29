using Backend_portafolio.Models;
using System.ComponentModel.DataAnnotations;

namespace Backend_portafolio.Entities
{
    public class Post
    {
        public int id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string cover { get; set; }
        public int format_id { get; set; }

        public string userName { get; set; }
        public int user_id { get; set; }

        public bool draft { get; set; }
        public string format { get; set; }

        public DateTime created_at { get; set; }
        public DateTime modify_at { get; set; }
    }
}
