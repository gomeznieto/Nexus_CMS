namespace Backend_portafolio.Models
{
    public class Link
    {
        public int id { get; set; }
        public string url { get; set; }
        public int post_id { get; set; }
        public int source_id { get; set; }
    }
}
