namespace Backend_portafolio.Models
{
    public class Media
    {
        public int id { get; set; }
        public string url { get; set; }
        public int post_id { get; set; }
        public int mediatype_id { get; set; }
        public string name { get; set; }
    }
}
